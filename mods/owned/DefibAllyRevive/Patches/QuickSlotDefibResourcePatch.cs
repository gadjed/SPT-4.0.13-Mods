using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI.DragAndDrop;
using SPT.Reflection.Patching;

namespace DefibAllyRevive.Patches;

/// <summary>
/// Vanilla defibrillator is barter MedicalSupplies with MaxResource=0, so the hotkey
/// panel paints "0/0". Show a single revive charge instead.
/// </summary>
internal sealed class QuickSlotDefibResourcePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(QuickSlotItemView).GetMethod(
            "smethod_3",
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public
        )!;
    }

    [PatchPrefix]
    private static bool Prefix(Item item, ref string text, ref bool __result)
    {
        if (!AllyUtil.IsDefibrillator(item))
        {
            return true;
        }

        text = "1";
        __result = true;
        return false;
    }
}
