using System.Reflection;
using SPT.Reflection.Patching;

namespace YellowFlareCurse.Client.Patches;

/// <summary>
/// Ensures the curse synthetic container id reaches <c>InitContainerLoot</c> /
/// <c>getAirdropLoot</c>. Without it the server falls through to random
/// WEAPON / COMMON crates.
/// </summary>
public class AirplaneCurseContainerPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(OfflineAirplaneServerLogicClass).GetMethod(
            "method_0",
            BindingFlags.Public | BindingFlags.Instance
        )!;
    }

    [PatchPrefix]
    public static void PatchPrefix(ref string containerTemplateId)
    {
        containerTemplateId = CurseAirdropGate.ForceContainerId(containerTemplateId);
    }
}
