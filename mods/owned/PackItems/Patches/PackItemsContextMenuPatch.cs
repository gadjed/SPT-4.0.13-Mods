using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace PackItems.Patches;

/// <summary>
/// Adds «Скласти предмети» to the item context menu for stash containers.
/// </summary>
public sealed class PackItemsContextMenuPatch : ModulePatch
{
    private const string ActionKey = "PackItems";

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ItemUiContext), nameof(ItemUiContext.GetItemContextInteractions));
    }

    [PatchPostfix]
    private static void Postfix(
        ItemContextAbstractClass itemContext,
        ItemInfoInteractionsAbstractClass<EItemInfoButton> __result)
    {
        if (!PackItemsPlugin.Enabled.Value || __result == null || itemContext?.Item == null)
        {
            return;
        }

        if (!PackService.IsAllowedView(itemContext.ViewType))
        {
            return;
        }

        if (!PackService.CanPack(itemContext.Item))
        {
            return;
        }

        var container = (CompoundItem)itemContext.Item;
        string label = PackItemsPlugin.MenuLabel.Value;
        Sprite? icon = PackService.MenuIcon();

        __result.Dictionary_0[ActionKey] = new DynamicInteractionClass(
            ActionKey,
            label,
            () => PackService.PackIntoContainer(container),
            icon);
    }
}
