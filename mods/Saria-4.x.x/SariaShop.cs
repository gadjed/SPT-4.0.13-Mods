using System.Reflection;
using SariaShop.Generators;
using SariaShop.Helpers;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Routers;
using Path = System.IO.Path;

namespace SariaShop;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "nameless.sariashop";
    public string Name { get; init; } = "Saria Trader 2.0";
    public string Author { get; init; } = "gadjed";
    public List<string>? Contributors { get; init; } = ["nameless"];
    public SemanticVersioning.Version Version { get; init; } = new("2.1.1");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.0");
    public bool HasPrepatcher { get; init; } = false;
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; } = "https://github.com/gadjed/SariaTrader2.0-SPT-mod";
    public string License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class Saria(
    ISptLogger<Saria> logger,
    ModHelper modHelper,
    ImageRouter imageRouter,
    TraderConfig traderConfig,
    RagfairConfig ragfairConfig,
    TradersTable tradersTable,
    SariaTraderHelper addCustomTraderHelper,
    SariaAssortGenerator sariaGenerator
) : IOnLoad
{
    public ModConfig? config;

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var pathToMod = modHelper.GetAbsolutePathToModFolder(assembly);
        var traderImagePath = Path.Combine(pathToMod, "Assets/Saria.jpg");
        var traderBase = modHelper.GetJsonDataFromFile<TraderBase>(pathToMod, "Assets/base.json");

        imageRouter.AddRoute(traderBase.Avatar.Replace(".jpg", ""), traderImagePath);
        addCustomTraderHelper.SetTraderUpdateTime(traderConfig, traderBase, 1800, 7200);
        ragfairConfig.Traders.TryAdd(traderBase.Id, true);
        addCustomTraderHelper.AddTraderWithEmptyAssortToDb(traderBase);
        addCustomTraderHelper.AddTraderToLocales(
            traderBase,
            "Saria",
            "A soldier with questionable motives, an unknown background, and a large supply of military goods. She's willing to trade, for a price of course."
        );

        config = modHelper.GetJsonDataFromFile<ModConfig>(pathToMod, "config.json");
        sariaGenerator.PassConfig(config);
        sariaGenerator.CreateSariaAssort();

        ApplyLoyaltyLevelChanges(traderBase.Id, tradersTable);

        logger.LogWithColor("[Saria] Mission accomplished, returning to base.", Spectre.Console.Color.Cyan);

        return Task.CompletedTask;
    }

    private void ApplyLoyaltyLevelChanges(string traderId, TradersTable tradersTable)
    {
        if (config == null)
        {
            return;
        }

        var trader = tradersTable.GetTrader(traderId);
        var traderLoyaltyLevels = trader?.Base.LoyaltyLevels;

        if (traderLoyaltyLevels == null)
        {
            return;
        }

        if (config.RemoveLevelLlRequirements)
        {
            foreach (var level in traderLoyaltyLevels)
            {
                level.MinLevel = 1;
            }
        }

        if (config.RemoveMoneyLlRequirements)
        {
            foreach (var level in traderLoyaltyLevels)
            {
                level.MinSalesSum = 0;
            }
        }
    }
}

public class ModConfig
{
    public bool RandomizeStockCount { get; set; }
    public bool RemoveMoneyLlRequirements { get; set; }
    public bool RemoveLevelLlRequirements { get; set; }
}
