using System.Reflection;
using EFT;
using SPT.Reflection.Patching;

namespace DefibAllyRevive.Patches;

/// <summary>
/// Tick bleedout timers for downed allies.
/// </summary>
internal sealed class PlayerUpdateTickPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(Player).GetMethod(nameof(Player.UpdateTick));
    }

    [PatchPostfix]
    private static void Postfix(Player __instance)
    {
        if (!DefibAllyRevivePlugin.Enabled.Value || !__instance.IsYourPlayer)
        {
            return;
        }

        AllyDownedTracker.Tick(UnityEngine.Time.deltaTime);
    }
}
