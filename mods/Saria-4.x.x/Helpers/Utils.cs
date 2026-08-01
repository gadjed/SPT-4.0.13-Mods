using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Tables;
using Path = System.IO.Path;

namespace SariaShop.Helpers;

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class SariaHelpers(TemplateTable templateTable, ModHelper modHelper)
{
    public TemplateItem GetItemInTables(string itemId)
    {
        return templateTable.Items[itemId];
    }

    public string FetchIdFromMap(string key, Dictionary<string, MongoId> map)
    {
        if (MongoId.IsValidMongoId(key))
        {
            return key;
        }

        if (map.TryGetValue(key, out var fetchedKey))
        {
            return fetchedKey.ToString();
        }

        throw new ArgumentException($"'{key}' was not found in map.");
    }

    public T LoadConfig<T>(Assembly assembly, string pathFromAssets, string configName)
    {
        var pathToMod = modHelper.GetAbsolutePathToModFolder(assembly);
        var finalPath = Path.Combine(pathToMod, "Assets", pathFromAssets);
        return modHelper.GetJsonDataFromFile<T>(finalPath, configName);
    }
}
