using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Utils;
using Path = System.IO.Path;

namespace ModInventory;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader)]
public class ModInventoryMod(
    ISptLogger<ModInventoryMod> logger,
    ModHelper modHelper,
    InventoryService inventoryService
) : IOnLoad
{
    public Task OnLoad()
    {
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var configPath = Path.Combine(pathToMod, "config.json");
        var config = File.Exists(configPath)
            ? modHelper.GetJsonDataFromFile<ModConfig>(pathToMod, "config.json")
            : new ModConfig();

        inventoryService.Initialize(pathToMod, config);
        logger.Success($"[ModInventory] Ready. Game root: {inventoryService.GameRoot}");
        return Task.CompletedTask;
    }
}
