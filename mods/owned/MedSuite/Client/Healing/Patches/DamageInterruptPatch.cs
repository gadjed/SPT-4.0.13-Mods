using MedSuite.Client.Healing;
using MedSuite.Client;
using System.Reflection;
using EFT;
using SPT.Reflection.Patching;

namespace MedSuite.Client.Healing.Patches;

/// <summary>
/// Abort healing and return the last weapon when the local player takes HP damage.
/// </summary>
internal class DamageInterruptPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(Player).GetMethod(
            nameof(Player.ApplyDamageInfo),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
    }

    [PatchPrefix]
    private static void Prefix(Player __instance, DamageInfoStruct damageInfo)
    {
        if (!MedSuitePlugin.Healing.CancelOnDamage.Value || !HealingSession.IsHealing)
        {
            return;
        }

        if (__instance == null || !__instance.IsYourPlayer)
        {
            return;
        }

        if (damageInfo.Damage <= 0f)
        {
            return;
        }

        HealingSession.InterruptAndRestoreWeapon(__instance);
    }
}
