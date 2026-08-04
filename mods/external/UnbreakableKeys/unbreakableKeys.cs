using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using System.Reflection;
using System.Text.Json.Serialization;

namespace unbreakableKeys;

public record ModMetadata : AbstractModMetadata
{
    /// Any string can be used for a modId, but it should ideally be unique and not easily duplicated
    public override string ModGuid { get; init; } = "com.toha3673.unbreakablekeys";

    /// The name of your mod
    public override string Name { get; init; } = "Unbreakable Keys";

    /// Who created the mod (you!)
    public override string Author { get; init; } = "Toha3673";

    /// A list of people who helped you create the mod
    public override List<string>? Contributors { get; init; }

    ///  The version of the mod, follows SEMVER rules (https://semver.org/)
    public override SemanticVersioning.Version Version { get; init; } = new("2.0.0");

    /// What version of SPT is your mod made for, follows SEMVER rules (https://semver.org/)
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");

    /// ModIds that you know cause problems with your mod
    public override List<string>? Incompatibilities { get; init; }

    /// ModIds your mod REQUIRES to function
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }

    /// Where to find your mod online
    public override string? Url { get; init; } = "https://github.com/sp-tarkov/server-mod-examples";

    /// Does your mod load bundles? (e.g. new weapon/armor mods)
    public override bool? IsBundleMod { get; init; } = false;

    /// What Licence does your mod use
    public override string? License { get; init; } = "MIT";
}

// We want to load after PreSptModLoader is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class EditDatabaseValues(ISptLogger<EditDatabaseValues> logger, ModHelper modHelper, DatabaseServer databaseServer) : IOnLoad
{
    public Task OnLoad()
    {
        var itemsDb = databaseServer.GetTables().Templates.Items;

        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var config = modHelper.GetJsonDataFromFile<ModCondig>(pathToMod, "config.jsonc");

        foreach (var item in itemsDb.Values.Where(item => item.Parent == "5c99f98d86f7745c314214b3"))///Mechanical keys
        {
            if (config.keys_blacklist.Contains(item.Id)) { continue; }

            if (config.marked_keys.Contains(item.Id))
            {
                if (!config.unbreakable_marked_keys) { continue; }

                MakeKeyUnbreakable(item);
            }

            if (config.unbreakable_keys) { MakeKeyUnbreakable(item); }
        }

        foreach (var item in itemsDb.Values.Where(item => item.Parent == "5c164d2286f774194c5e69fa")) ///Keycards
        {
            if (config.keys_blacklist.Contains(item.Id)) { continue; }

            if (config.colored_keycards.Contains(item.Id))
            {
                if (!config.unbreakable_colored_keycards) { continue; }

                MakeKeyUnbreakable(item);
            }

            if (config.unbreakable_keycards) { MakeKeyUnbreakable(item); }
        }

        logger.Success("[Unbreakable Keys] Mod has Loaded!");

        return Task.CompletedTask;
    }

    private void MakeKeyUnbreakable(TemplateItem item)
    {
        item.Properties.MaximumNumberOfUsage = 0;
    }
}

public class ModCondig
{
    public bool unbreakable_keys { get; set; } = true;

    public bool unbreakable_marked_keys { get; set; } = false;
    public List<string> marked_keys { get; set; } = new List<string>();

    public bool unbreakable_keycards { get; set; } = true;

    public bool unbreakable_colored_keycards { get; set; } = true;
    public List<string> colored_keycards { get; set; } = new List<string>();

    public bool infinite_labs_acess_card { get; set; } = false;
    public bool infinite_labrys_acess_card { get; set; } = false;

    public List<string> keys_blacklist { get; set; } = new List<string>();
}
