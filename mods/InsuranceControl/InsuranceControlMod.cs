using System.Reflection;
using InsuranceControl.Patches;
using Spectre.Console;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers.Items;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;
using Path = System.IO.Path;

namespace InsuranceControl;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "gadjed.insurancerefund";
    public string Name { get; init; } = "Insurance Refund";
    public string Author { get; init; } = "gadjed";
    public List<string>? Contributors { get; init; } = null;
    public SemanticVersioning.Version Version { get; init; } = new("1.1.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.0");
    public bool HasPrepatcher { get; init; } = false;
    public List<string>? Incompatibilities { get; init; } = null;
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = null;
    public string? Url { get; init; } = "https://github.com/gadjed/Insurance-refund-SPT-mod";
    public string License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class InsuranceControlMod(
    ISptLogger<InsuranceControlMod> logger,
    ModHelper modHelper,
    InsuranceConfig insuranceConfig,
    TradersTable tradersTable,
    ItemHelper itemHelper,
    PatchManager patchManager
) : IOnLoad
{
    public static ModConfig Config { get; private set; } = new();
    public static ItemHelper ItemHelper { get; private set; } = null!;

    private static readonly Dictionary<string, MongoId> TraderIds = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Prapor"] = Traders.PRAPOR,
        ["Therapist"] = Traders.THERAPIST,
        [Traders.PRAPOR.ToString()] = Traders.PRAPOR,
        [Traders.THERAPIST.ToString()] = Traders.THERAPIST,
    };

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        ItemHelper = itemHelper;

        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var configPath = Path.Combine(pathToMod, "config.json");
        Config = File.Exists(configPath)
            ? modHelper.GetJsonDataFromFile<ModConfig>(pathToMod, "config.json")
            : new ModConfig();

        ApplyInsuranceConfig(insuranceConfig);
        ApplyTraderReturnHours(tradersTable);
        EnableContentPatches();

        logger.Success(
            $"[InsuranceControl] Loaded. ReturnTimeOverride={Config.ReturnTimeOverrideSeconds}s, "
                + $"MagsWithAmmo={Config.ReturnMagazinesWithAmmo}, "
                + $"ContainersWithContents={Config.ReturnContainersWithContents}."
        );

        return Task.CompletedTask;
    }

    private void ApplyInsuranceConfig(InsuranceConfig insurance)
    {
        insurance.ReturnTimeOverrideSeconds = Math.Max(0, Config.ReturnTimeOverrideSeconds);
        insurance.StorageTimeOverrideSeconds = Math.Max(0, Config.StorageTimeOverrideSeconds);
        insurance.SimulateItemsBeingTaken = Config.SimulateItemsBeingTaken;

        if (Config.RunIntervalSeconds > 0)
        {
            insurance.RunIntervalSeconds = Config.RunIntervalSeconds;
        }

        foreach (var (key, lostChance) in Config.LostChancePercent)
        {
            if (!TryResolveTraderId(key, out var traderId))
            {
                logger.Warning($"[InsuranceControl] Unknown trader in LostChancePercent: {key}");
                continue;
            }

            var clampedLost = Math.Clamp(lostChance, 0, 100);
            var returnChance = 100 - clampedLost;
            insurance.ReturnChancePercent[traderId] = returnChance;

            logger.LogWithColor(
                $"[InsuranceControl] {key}: lost chance {clampedLost}% (return chance {returnChance}%)",
                Color.Cyan
            );
        }
    }

    private void ApplyTraderReturnHours(TradersTable tradersTable)
    {
        if (Config.ReturnTimeOverrideSeconds > 0)
        {
            logger.LogWithColor(
                $"[InsuranceControl] Using ReturnTimeOverrideSeconds={Config.ReturnTimeOverrideSeconds}; TraderReturnHours ignored.",
                Color.Cyan
            );
            return;
        }

        foreach (var (key, hours) in Config.TraderReturnHours)
        {
            if (!TryResolveTraderId(key, out var traderId))
            {
                logger.Warning($"[InsuranceControl] Unknown trader in TraderReturnHours: {key}");
                continue;
            }

            var trader = tradersTable.GetTrader(traderId);
            if (trader?.Base?.Insurance is null)
            {
                logger.Warning($"[InsuranceControl] Trader insurance settings missing: {key}");
                continue;
            }

            var min = (int)Math.Max(0, hours.Min);
            var max = (int)Math.Max(min, hours.Max);
            trader.Base.Insurance.MinReturnHour = min;
            trader.Base.Insurance.MaxReturnHour = max;

            logger.LogWithColor(
                $"[InsuranceControl] {key}: return window {min}-{max}h",
                Color.Cyan
            );
        }
    }

    private void EnableContentPatches()
    {
        if (!Config.ReturnMagazinesWithAmmo && !Config.ReturnContainersWithContents)
        {
            return;
        }

        patchManager.PatcherName = "InsuranceControl";
        patchManager.EnablePatch(new EnrichLostInsuredItemsPatch());
        logger.LogWithColor("[InsuranceControl] Content enrichment patch enabled.", Color.Cyan);
    }

    private static bool TryResolveTraderId(string key, out MongoId traderId)
    {
        if (TraderIds.TryGetValue(key, out traderId))
        {
            return true;
        }

        if (MongoId.IsValidMongoId(key))
        {
            traderId = new MongoId(key);
            return true;
        }

        traderId = default;
        return false;
    }
}
