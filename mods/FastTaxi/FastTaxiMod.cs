using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using Path = System.IO.Path;

namespace FastTaxi;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "gadjed.fasttaxi";
    public override string Name { get; init; } = "Fast Taxi";
    public override string Author { get; init; } = "gadjed";
    public override List<string>? Contributors { get; init; } = null;
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; } = null;
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = null;
    public override string? Url { get; init; } = "https://github.com/gadjed/FastTaxi-SPT-mod";
    public override bool? IsBundleMod { get; init; } = false;
    public override string? License { get; init; } = "MIT";
}

public class ModConfig
{
    /// <summary>
    /// Countdown (seconds) before a paid car / taxi extract departs.
    /// Vanilla is 60.
    /// </summary>
    public double WaitTimeSeconds { get; set; } = 8;
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class FastTaxiMod(
    ISptLogger<FastTaxiMod> logger,
    ModHelper modHelper,
    DatabaseService databaseService
) : IOnLoad
{
    public Task OnLoad()
    {
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var configPath = Path.Combine(pathToMod, "config.json");
        var config = File.Exists(configPath)
            ? modHelper.GetJsonDataFromFile<ModConfig>(pathToMod, "config.json")
            : new ModConfig();

        var waitTime = config.WaitTimeSeconds <= 0 ? 8 : config.WaitTimeSeconds;
        var changed = 0;

        foreach (var (locationId, location) in databaseService.GetLocations().GetDictionary())
        {
            if (location?.Base?.Exits is not null)
            {
                foreach (var exit in location.Base.Exits)
                {
                    if (TrySetTaxiWait(exit, waitTime, locationId, logger))
                    {
                        changed++;
                    }
                }
            }

            if (location?.AllExtracts is null)
            {
                continue;
            }

            foreach (var exit in location.AllExtracts)
            {
                if (TrySetTaxiWait(exit, waitTime, locationId, logger))
                {
                    changed++;
                }
            }
        }

        logger.Success($"[FastTaxi] Updated wait time on {changed} car/taxi extract(s) to {waitTime}s.");
        return Task.CompletedTask;
    }

    private static bool TrySetTaxiWait(
        Exit exit,
        double waitTime,
        string locationId,
        ISptLogger<FastTaxiMod> logger
    )
    {
        // Paid car / taxi extracts (V-Ex): TransferItem + SharedTimer, vanilla wait = 60s
        if (exit.PassageRequirement != RequirementState.TransferItem)
        {
            return false;
        }

        var previous = exit.ExfiltrationTime;
        exit.ExfiltrationTime = waitTime;
        exit.ExfiltrationTimePVE = waitTime;

        logger.LogWithColor(
            $"[FastTaxi] {locationId}/{exit.Name}: ExfiltrationTime {previous} -> {waitTime}s",
            LogTextColor.Cyan
        );

        return true;
    }
}
