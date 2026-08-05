using System;
using System.Collections.Generic;
using Comfort.Common;
using EFT.InventoryLogic;
using EFT.UI;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using UnityEngine;

namespace InsureAllPrapor;

/// <summary>
/// Clones a vanilla <see cref="DefaultUIButton"/> and places it above the tactical vest
/// (chest rig) on the stash containers panel.
/// </summary>
internal static class InsureAllButtonController
{
    private const string ButtonName = "InsureAllPraporButton";
    private const float ButtonWidth = 140f;
    private const float ButtonHeight = 24f;
    private const float GapAboveAnchor = 12f;
    /// <summary>~5 cm right of the vest slot center (≈96 DPI).</summary>
    private const float OffsetRight = 190f;
    private const int FontSize = 14;

    private static DefaultUIButton? _button;
    private static InventoryController? _inventoryController;
    private static InsuranceCompanyClass? _insurance;

    public static void Show(
        EquipmentTab equipmentTab,
        SlotView? armorSlot,
        SlotView? headwearSlot,
        InventoryController inventoryController,
        InsuranceCompanyClass insurance)
    {
        _inventoryController = inventoryController;
        _insurance = insurance;

        if (!InsureAllPraporPlugin.Enabled.Value)
        {
            Hide();
            return;
        }

        EnsureButton(equipmentTab);
        if (_button == null)
        {
            return;
        }

        // Vest lives on ContainersPanel and is created after EquipmentTab.Show — try now, then again from ContainersPanel patch.
        var vest = FindTacticalVestSlot(equipmentTab);
        PositionButton(_button.RectTransform, equipmentTab, vest, armorSlot, headwearSlot);
        _button.SetHeaderText(InsureAllPraporPlugin.ButtonLabel.Value, FontSize);
        _button.Interactable = !InsureAllService.IsBusy;
        _button.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called after <see cref="ContainersPanel.Show"/> when the tactical vest SlotView exists.
    /// </summary>
    public static void RepositionAboveVest(ContainersPanel containersPanel)
    {
        if (_button == null || !_button.gameObject.activeSelf || !InsureAllPraporPlugin.Enabled.Value)
        {
            return;
        }

        var vest = GetSlotFromContainers(containersPanel, EquipmentSlot.TacticalVest);
        if (vest == null)
        {
            return;
        }

        PositionRelativeToSlot(_button.RectTransform, vest, above: true);
    }

    public static void Hide()
    {
        if (_button != null)
        {
            _button.gameObject.SetActive(false);
        }

        _inventoryController = null;
        _insurance = null;
    }

    private static void EnsureButton(EquipmentTab equipmentTab)
    {
        if (_button != null)
        {
            return;
        }

        var template = FindButtonTemplate(equipmentTab);
        if (template == null)
        {
            InsureAllPraporPlugin.Log.LogError("[InsureAllPrapor] Could not find a DefaultUIButton template.");
            return;
        }

        var clone = UnityEngine.Object.Instantiate(template, equipmentTab.transform, false);
        clone.name = ButtonName;
        clone.gameObject.SetActive(true);

        var layout = clone.GetComponent<UnityEngine.UI.LayoutElement>();
        if (layout != null)
        {
            layout.minWidth = -1f;
            layout.preferredWidth = -1f;
            layout.flexibleWidth = -1f;
            layout.ignoreLayout = true;
        }

        clone.OnClick.RemoveAllListeners();
        clone.OnClick.AddListener(OnClicked);

        _button = clone;
        InsureAllPraporPlugin.Log.LogInfo("[InsureAllPrapor] Stash insure-all button created.");
    }

    private static void PositionButton(
        RectTransform buttonRt,
        EquipmentTab equipmentTab,
        SlotView? vestSlot,
        SlotView? armorSlot,
        SlotView? headwearSlot)
    {
        buttonRt.localScale = Vector3.one;
        buttonRt.sizeDelta = new Vector2(ButtonWidth, ButtonHeight);

        if (vestSlot != null)
        {
            PositionRelativeToSlot(buttonRt, vestSlot, above: true);
            return;
        }

        // Fallback until ContainersPanel finishes: sit above armor (just above where the rig appears).
        var fallback = armorSlot ?? headwearSlot;
        if (fallback != null)
        {
            PositionRelativeToSlot(buttonRt, fallback, above: true);
            return;
        }

        buttonRt.SetParent(equipmentTab.transform, false);
        buttonRt.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRt.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRt.pivot = new Vector2(0.5f, 0.5f);
        buttonRt.anchoredPosition = new Vector2(OffsetRight, 180f);
        buttonRt.SetAsLastSibling();
    }

    private static void PositionRelativeToSlot(RectTransform buttonRt, SlotView slot, bool above)
    {
        var slotRt = slot.RectTransform;
        var parent = slotRt.parent as RectTransform;
        if (parent == null)
        {
            return;
        }

        buttonRt.SetParent(parent, false);
        // Same anchors as the slot → anchoredPosition is in the same space.
        buttonRt.anchorMin = slotRt.anchorMin;
        buttonRt.anchorMax = slotRt.anchorMax;
        buttonRt.pivot = new Vector2(0.5f, 0.5f);
        buttonRt.sizeDelta = new Vector2(ButtonWidth, ButtonHeight);

        float y = (slotRt.rect.height * 0.5f) + (ButtonHeight * 0.5f) + GapAboveAnchor;
        if (!above)
        {
            y = -y;
        }

        buttonRt.anchoredPosition = slotRt.anchoredPosition + new Vector2(OffsetRight, y);
        buttonRt.SetAsLastSibling();
    }

    private static SlotView? FindTacticalVestSlot(EquipmentTab equipmentTab)
    {
        try
        {
            var itemsPanel = equipmentTab.GetComponentInParent<ItemsPanel>();
            if (itemsPanel == null)
            {
                return null;
            }

            var containers = Traverse.Create(itemsPanel).Field("_containers").GetValue<ContainersPanel>();
            return GetSlotFromContainers(containers, EquipmentSlot.TacticalVest);
        }
        catch (Exception ex)
        {
            InsureAllPraporPlugin.Log.LogDebug($"[InsureAllPrapor] Vest lookup failed: {ex.Message}");
            return null;
        }
    }

    private static SlotView? GetSlotFromContainers(ContainersPanel? containers, EquipmentSlot slot)
    {
        if (containers == null)
        {
            return null;
        }

        var dict = Traverse.Create(containers)
            .Field("dictionary_0")
            .GetValue<Dictionary<EquipmentSlot, SlotView>>();

        if (dict != null && dict.TryGetValue(slot, out var view) && view != null)
        {
            return view;
        }

        return null;
    }

    private static DefaultUIButton? FindButtonTemplate(EquipmentTab equipmentTab)
    {
        try
        {
            var inventoryScreen = Singleton<CommonUI>.Instance?.InventoryScreen;
            if (inventoryScreen != null)
            {
                var back = Traverse.Create(inventoryScreen).Field("_backButton").GetValue<DefaultUIButton>();
                if (back != null)
                {
                    return back;
                }
            }
        }
        catch (Exception ex)
        {
            InsureAllPraporPlugin.Log.LogDebug($"[InsureAllPrapor] CommonUI back button lookup failed: {ex.Message}");
        }

        var fromParents = equipmentTab.GetComponentsInParent<DefaultUIButton>(true);
        if (fromParents != null && fromParents.Length > 0)
        {
            return fromParents[0];
        }

        return UnityEngine.Object.FindObjectOfType<DefaultUIButton>();
    }

    private static void OnClicked()
    {
        if (!InsureAllPraporPlugin.Enabled.Value || InsureAllService.IsBusy)
        {
            return;
        }

        if (_inventoryController == null || _insurance == null)
        {
            InsureAllPraporPlugin.Log.LogWarning("[InsureAllPrapor] Click ignored — no inventory context.");
            return;
        }

        if (_button != null)
        {
            _button.Interactable = false;
        }

        InsureAllService.InsureEquippedGear(_inventoryController, _insurance, RestoreInteractable);
    }

    private static void RestoreInteractable()
    {
        if (_button != null)
        {
            _button.Interactable = true;
        }
    }
}
