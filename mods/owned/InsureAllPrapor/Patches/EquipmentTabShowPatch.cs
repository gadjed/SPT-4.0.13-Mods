using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using EFT.UI;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace InsureAllPrapor.Patches;

internal class EquipmentTabShowPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(
            typeof(EquipmentTab),
            nameof(EquipmentTab.Show),
            [
                typeof(ItemContextAbstractClass),
                typeof(InventoryEquipment),
                typeof(InventoryController),
                typeof(SkillManager),
                typeof(InsuranceCompanyClass),
                typeof(bool),
            ]
        )!;
    }

    [PatchPostfix]
    public static void Postfix(
        EquipmentTab __instance,
        InventoryController inventoryController,
        InsuranceCompanyClass insurance,
        bool inRaid,
        SlotView ____headwearSlot)
    {
        if (inRaid || inventoryController == null || insurance == null)
        {
            InsureAllButtonController.Hide();
            return;
        }

        InsureAllButtonController.Show(__instance, ____headwearSlot, inventoryController, insurance);
    }
}

internal class EquipmentTabHidePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(EquipmentTab), nameof(EquipmentTab.Hide))!;
    }

    [PatchPostfix]
    public static void Postfix()
    {
        InsureAllButtonController.Hide();
    }
}
