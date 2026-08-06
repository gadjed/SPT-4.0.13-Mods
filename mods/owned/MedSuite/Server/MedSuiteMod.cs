using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using Path = System.IO.Path;

namespace MedSuite;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "gadjed.medsuite";
    public override string Name { get; init; } = "Med Suite";
    public override string Author { get; init; } = "gadjed";
    public override List<string>? Contributors { get; init; } = null;
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.13");
    public override List<string>? Incompatibilities { get; init; } = null;
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = null;
    public override string? Url { get; init; } = "https://github.com/gadjed/MedSuite-SPT-mod";
    public override bool? IsBundleMod { get; init; } = false;
    public override string License { get; init; } = "MIT";
}

public class ModConfig
{
    /// <summary>Surgery / splint use time in seconds.</summary>
    public double UseTimeSeconds { get; set; } = 5;

    /// <summary>Friendly item name (or template id) -> enabled for use-time rebalance.</summary>
    public Dictionary<string, bool> Items { get; set; } = new();

    /// <summary>Set portable defibrillator MaxResource/Resource to 1 so UI shows a charge.</summary>
    public bool FixDefibrillatorResource { get; set; } = true;
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class MedSuiteMod(
    ISptLogger<MedSuiteMod> logger,
    ModHelper modHelper,
    DatabaseService databaseService
) : IOnLoad
{
    public const string DefibrillatorTemplateId = "5c052e6986f7746b207bc3c9";

    public Task OnLoad()
    {
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var configPath = Path.Combine(pathToMod, "config.json");
        var config = File.Exists(configPath)
            ? modHelper.GetJsonDataFromFile<ModConfig>(pathToMod, "config.json")
            : new ModConfig { Items = new Dictionary<string, bool>(ItemCatalog.DefaultEnabledItems) };

        ApplySurgeryUseTimes(config);
        if (config.FixDefibrillatorResource)
        {
            ApplyDefibrillatorResource();
        }

        return Task.CompletedTask;
    }

    private void ApplySurgeryUseTimes(ModConfig config)
    {
        var useTime = config.UseTimeSeconds <= 0 ? 5 : config.UseTimeSeconds;
        var items = databaseService.GetItems();
        var changed = 0;

        foreach (var (key, enabled) in config.Items)
        {
            if (!enabled)
            {
                continue;
            }

            if (!ItemCatalog.TryResolve(key, out var tpl, out var displayName))
            {
                logger.Warning($"[MedSuite] Unknown item key: {key}");
                continue;
            }

            if (!MongoId.IsValidMongoId(tpl) || !items.TryGetValue(tpl, out var item) || item is null)
            {
                logger.Warning($"[MedSuite] Item not found in database: {displayName} ({tpl})");
                continue;
            }

            if (item.Properties is null)
            {
                logger.Warning($"[MedSuite] Item has no properties: {displayName} ({tpl})");
                continue;
            }

            var previous = item.Properties.MedUseTime;
            item.Properties.MedUseTime = useTime;
            changed++;

            logger.LogWithColor(
                $"[MedSuite] {displayName}: medUseTime {previous} -> {useTime}s",
                LogTextColor.Cyan
            );
        }

        logger.Success($"[MedSuite] Updated use time on {changed} medical item(s).");
    }

    private void ApplyDefibrillatorResource()
    {
        var items = databaseService.GetItems();
        if (!MongoId.IsValidMongoId(DefibrillatorTemplateId)
            || !items.TryGetValue(DefibrillatorTemplateId, out var item)
            || item?.Properties is null)
        {
            logger.Warning("[MedSuite] Portable defibrillator template not found.");
            return;
        }

        var props = item.Properties;
        var prevMax = props.MaxResource;
        var prevRes = props.Resource;
        props.MaxResource = 1;
        props.Resource = 1;

        logger.Success(
            $"[MedSuite] Defibrillator resource {prevRes}/{prevMax} -> 1/1 (single revive charge)."
        );
    }
}
