using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace DefibAllyRevive;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "gadjed.defiballyrevive";
    public override string Name { get; init; } = "Defib Ally Revive";
    public override string Author { get; init; } = "gadjed";
    public override List<string>? Contributors { get; init; } = null;
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.1");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.13");
    public override List<string>? Incompatibilities { get; init; } = null;
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = null;
    public override string? Url { get; init; } = "https://github.com/gadjed/Defib-ally-revive-SPT-mod";
    public override bool? IsBundleMod { get; init; } = false;
    public override string License { get; init; } = "MIT";
}

/// <summary>
/// Vanilla portable defibrillator is barter loot with MaxResource/Resource = 0, which the
/// client hotkey UI renders as "0/0". Give it a single charge so uses display correctly.
/// </summary>
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class DefibAllyReviveMod(
    ISptLogger<DefibAllyReviveMod> logger,
    DatabaseService databaseService
) : IOnLoad
{
    public const string DefibrillatorTemplateId = "5c052e6986f7746b207bc3c9";

    public Task OnLoad()
    {
        var items = databaseService.GetItems();
        if (!MongoId.IsValidMongoId(DefibrillatorTemplateId)
            || !items.TryGetValue(DefibrillatorTemplateId, out var item)
            || item?.Properties is null)
        {
            logger.Warning("[DefibAllyRevive] Portable defibrillator template not found.");
            return Task.CompletedTask;
        }

        var props = item.Properties;
        var prevMax = props.MaxResource;
        var prevRes = props.Resource;
        props.MaxResource = 1;
        props.Resource = 1;

        logger.Success(
            $"[DefibAllyRevive] Defibrillator resource {prevRes}/{prevMax} -> 1/1 (single revive charge)."
        );
        return Task.CompletedTask;
    }
}
