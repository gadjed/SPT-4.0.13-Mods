using System.Reflection;
using HarmonyLib;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.Models.Eft.Location;

namespace YellowFlareCurse.Patches;

/// <summary>
/// Primary server hook — <c>client/location/getAirdropLoot</c> entry.
/// Replaces curse-container responses with SUPPLY + ForcedLoot before SPT can
/// fall through to random WEAPON / COMMON.
/// </summary>
public class CurseGetAirDropLootPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(LocationController), nameof(LocationController.GetAirDropLoot));
    }

    [PatchPrefix]
    public static bool Prefix(GetAirdropLootRequest? request, ref GetAirdropLootResponse __result)
    {
        var log = ModFileLogger.Instance;
        var container = request?.ContainerId.ToString() ?? "<null>";
        var empty = request is null || request.ContainerId.IsEmpty;
        log?.Info($"{YellowFlareCurseMod.Tag} GetAirDropLoot. ContainerId={container}, empty={empty}.");

        if (empty || !CurseAirdropLootBuilder.IsCurseContainer(container))
        {
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
                $"{YellowFlareCurseMod.Tag} GetAirDropLoot curse replace: icon={__result.Icon}, "
                    + $"items={itemCount}, crate=SUPPLY/техобеспечения "
                    + $"({CurseAirdropLootBuilder.SupplyCrateTpl})."
            );
            return false;
        }
        catch (Exception ex)
        {
            log?.Error($"{YellowFlareCurseMod.Tag} GetAirDropLoot curse build failed: {ex}");
            return true;
        }
    }
}
