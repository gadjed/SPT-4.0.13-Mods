using System.Reflection;
using EFT;
using EFT.HealthSystem;
using SPT.Reflection.Patching;

namespace DefibAllyRevive.Patches;

/// <summary>
/// Divert lethal damage on allies into a revivable downed state.
/// </summary>
internal sealed class ActiveHealthControllerKillPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(ActiveHealthController).GetMethod(nameof(ActiveHealthController.Kill));
    }

    [PatchPrefix]
    private static bool Prefix(ActiveHealthController __instance, EDamageType damageType)
    {
        if (!DefibAllyRevivePlugin.Enabled.Value)
        {
            return true;
        }

        var player = __instance.Player;
        if (player == null || player.IsYourPlayer)
        {
            return true;
        }

        // Already bled out → allow real death.
        if (AllyDownedTracker.IsDowned(player.ProfileId))
        {
            AllyDownedTracker.Clear(player.ProfileId);
            return true;
        }

        if (AllyDownedTracker.TryEnterDowned(player, damageType))
        {
            return false;
        }

        return true;
    }
}
