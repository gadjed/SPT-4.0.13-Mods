using System.Reflection;
using HarmonyLib;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Eft.Location;
using SPTarkov.Server.Core.Services;

namespace YellowFlareCurse.Patches;

/// <summary>
/// Backup hook on <see cref="AirdropService.GenerateCustomAirdropLoot"/> in case
/// another mod calls the service directly (bypassing <c>LocationController</c>).
/// </summary>
public class CurseAirdropLootPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(AirdropService), nameof(AirdropService.GenerateCustomAirdropLoot));
    }

    [PatchPrefix]
    public static bool Prefix(GetAirdropLootRequest request, ref GetAirdropLootResponse __result)
    {
        var log = ModFileLogger.Instance;
        var container = request?.ContainerId.ToString() ?? "<null>";
        var empty = request is null || request.ContainerId.IsEmpty;
        log?.Info($"{YellowFlareCurseMod.Tag} GenerateCustomAirdropLoot. ContainerId={container}, empty={empty}.");

        if (empty || !CurseAirdropLootBuilder.IsCurseContainer(container))
        {
            log?.Info(
                $"{YellowFlareCurseMod.Tag} Not curse container (want={YellowFlareCurseMod.CurseContainerIdString}) — pass-through."
            );
            return true;
        }

        var lootGen = YellowFlareCurseMod.LootGenerator;
        if (lootGen is null)
        {
            log?.Error($"{YellowFlareCurseMod.Tag} LootGenerator is null — cannot build curse loot.");
            return true;
        }

        if (YellowFlareCurseMod.ForcedLoot.Count == 0)
        {
            log?.Warning($"{YellowFlareCurseMod.Tag} ForcedLoot empty — pass-through.");
            return true;
        }

        try
        {
            __result = CurseAirdropLootBuilder.Build(lootGen);
            var itemCount = __result.Container?.Count() ?? 0;
            log?.Success(
                $"{YellowFlareCurseMod.Tag} Built curse airdrop: icon={__result.Icon}, "
                    + $"items={itemCount}, forcedEntries={YellowFlareCurseMod.ForcedLoot.Count}, "
                    + $"crate=SUPPLY/техобеспечения."
            );
            return false;
        }
        catch (Exception ex)
        {
            log?.Error($"{YellowFlareCurseMod.Tag} Failed to build curse airdrop: {ex}");
            return true;
        }
    }
}
