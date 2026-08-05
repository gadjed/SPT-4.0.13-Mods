using System.Reflection;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;

namespace DefibAllyRevive.Patches;

/// <summary>
/// Vanilla quick-slot binding only allows weapons/meds/food/etc.
/// Allow the portable defibrillator (barter) so it can sit on the hotkey bar.
/// </summary>
internal sealed class IsAtBindablePlacePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(InventoryController).GetMethod(nameof(InventoryController.IsAtBindablePlace));
    }

    [PatchPostfix]
    private static void Postfix(Item item, ref bool __result)
    {
        if (__result || item == null || !DefibAllyRevivePlugin.Enabled.Value)
        {
            return;
        }

        if (AllyUtil.IsDefibrillator(item))
        {
            __result = true;
            DefibAllyRevivePlugin.DebugLog($"Allowed defibrillator bind for {item.Id}");
        }
    }
}
