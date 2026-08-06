using System.Reflection;
using SPT.Reflection.Patching;

namespace YellowFlareCurse.Client.Patches;

/// <summary>
/// Last client gate before the HTTP loot request: force curse container id and
/// consume the armed gate so only one drop is rewritten.
/// </summary>
public class InitContainerLootCursePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(AirdropEventClass).GetMethod(
            nameof(AirdropEventClass.InitContainerLoot),
            BindingFlags.Public | BindingFlags.Instance
        )!;
    }

    [PatchPrefix]
    public static void PatchPrefix(ref string lootTemplateId)
    {
        var wasArmed = CurseAirdropGate.IsArmed;
        var before = lootTemplateId;
        lootTemplateId = CurseAirdropGate.ForceContainerId(lootTemplateId);

        if (wasArmed)
        {
            CurseAirdropGate.TryConsume();
            ModLogger.Info(
                $"InitContainerLoot curse rewrite: '{before ?? "<null>"}' → '{lootTemplateId}'."
            );
        }
    }
}
