using MedSuite.Client.Defib;
using MedSuite.Client;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;

namespace MedSuite.Client.Defib.Patches;

/// <summary>
/// When the player activates a quick slot that holds a defibrillator, try ally revive first.
/// </summary>
internal sealed class SetQuickSlotItemPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(Player).GetMethod(nameof(Player.SetQuickSlotItem));
    }

    [PatchPrefix]
    private static bool Prefix(Player __instance, EBoundItem quickSlot, Callback<IHandsController> callback)
    {
        if (!MedSuitePlugin.Defib.Enabled.Value || !__instance.IsYourPlayer)
        {
            return true;
        }

        Item? item = null;
        try
        {
            item = __instance.InventoryController.Inventory.FastAccess.GetBoundItem(quickSlot);
        }
        catch
        {
            return true;
        }

        if (!AllyUtil.IsDefibrillator(item))
        {
            return true;
        }

        MedSuitePlugin.DefibDebug($"Defibrillator quick-slot {quickSlot} pressed.");

        if (AllyReviveService.TryStartReviveFromDefib(__instance, item!))
        {
            // Skip vanilla TryProceed for the barter defibrillator.
            callback?.Invoke(new Result<IHandsController>(null!));
            return false;
        }

        return true;
    }
}
