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
        SlotView ____armorSlot,
        SlotView ____headwearSlot)
    {
        if (inRaid || inventoryController == null || insurance == null)
        {
            InsureAllButtonController.Hide();
            return;
        }

        // _tacticalVestSlot is NOT on EquipmentTab — vest is on ContainersPanel (created after this Show).
        InsureAllButtonController.Show(
            __instance,
            ____armorSlot,
            ____headwearSlot,
            inventoryController,
            insurance);
    }
}

/// <summary>
/// Vest / backpack / pockets SlotViews are built here, after EquipmentTab.Show.
/// </summary>
internal class ContainersPanelShowPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(
            typeof(ContainersPanel),
            nameof(ContainersPanel.Show),
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
    public static void Postfix(ContainersPanel __instance, bool inRaid)
    {
        if (inRaid)
        {
            return;
        }

        InsureAllButtonController.RepositionAboveVest(__instance);
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
