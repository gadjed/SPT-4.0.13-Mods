using System;
using System.Collections.Generic;
using Comfort.Common;
using EFT.InventoryLogic;
using EFT.UI;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using UnityEngine;

namespace InsuranceControl.Client;

/// <summary>
/// Clones a vanilla <see cref="DefaultUIButton"/> and places it above the tactical vest
/// (chest rig) on the stash containers panel.
/// </summary>
internal static class InsureAllButtonController
{
    private const string ButtonName = "InsuranceControlInsureAllButton";

    private static DefaultUIButton? _button;
    private static InventoryController? _inventoryController;
    private static InsuranceCompanyClass? _insurance;
    private static EquipmentTab? _equipmentTab;
    private static SlotView? _armorSlot;
    private static SlotView? _headwearSlot;

    public static void Show(
        EquipmentTab equipmentTab,
        SlotView? armorSlot,
        SlotView? headwearSlot,
        InventoryController inventoryController,
        InsuranceCompanyClass insurance)
    {
        _equipmentTab = equipmentTab;
        _armorSlot = armorSlot;
        _headwearSlot = headwearSlot;
        _inventoryController = inventoryController;
        _insurance = insurance;

        if (!InsuranceControlPlugin.InsureAllEnabled.Value)
        {
            Hide(keepContext: true);
            return;
        }

        EnsureButton(equipmentTab);
        if (_button == null)
        {
            return;
        }

        var vest = FindTacticalVestSlot(equipmentTab);
        PositionButton(_button.RectTransform, equipmentTab, vest, armorSlot, headwearSlot);
        ApplyVisuals();
        _button.Interactable = !InsureAllService.IsBusy;
        _button.gameObject.SetActive(true);
    }

    public static void RepositionAboveVest(ContainersPanel containersPanel)
    {
        if (_button == null || !_button.gameObject.activeSelf || !InsuranceControlPlugin.InsureAllEnabled.Value)
        {
            return;
        }

        var vest = GetSlotFromContainers(containersPanel, EquipmentSlot.TacticalVest);
        if (vest == null)
        {
            return;
        }

        PositionRelativeToSlot(_button.RectTransform, vest, above: true);
        ApplyVisuals();
    }

    public static void RefreshLayout()
    {
        if (_button == null || _equipmentTab == null || !InsuranceControlPlugin.InsureAllEnabled.Value)
        {
            return;
        }

        if (!_button.gameObject.activeSelf)
        {
            return;
        }

        var vest = FindTacticalVestSlot(_equipmentTab);
        PositionButton(_button.RectTransform, _equipmentTab, vest, _armorSlot, _headwearSlot);
        ApplyVisuals();
    }

    public static void OnEnabledChanged()
    {
        if (!InsuranceControlPlugin.InsureAllEnabled.Value)
        {
            Hide(keepContext: true);
            return;
        }

        if (_equipmentTab != null && _inventoryController != null && _insurance != null)
        {
            Show(_equipmentTab, _armorSlot, _headwearSlot, _inventoryController, _insurance);
        }
    }

    public static void Hide(bool keepContext = false)
    {
        if (_button != null)
        {
            _button.gameObject.SetActive(false);
        }

        if (!keepContext)
        {
            _equipmentTab = null;
            _armorSlot = null;
            _headwearSlot = null;
            _inventoryController = null;
            _insurance = null;
        }
    }

    private static void ApplyVisuals()
    {
        if (_button == null)
        {
            return;
        }

        _button.SetHeaderText(InsuranceControlPlugin.ButtonLabel.Value, InsuranceControlPlugin.FontSize.Value);
        _button.RectTransform.sizeDelta = new Vector2(
            InsuranceControlPlugin.ButtonWidth.Value,
            InsuranceControlPlugin.ButtonHeight.Value
        );
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
            InsuranceControlPlugin.Log.LogError("[InsuranceControl] Could not find a DefaultUIButton template.");
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
        InsuranceControlPlugin.Log.LogInfo("[InsuranceControl] Stash insure-all button created.");
    }

    private static void PositionButton(
        RectTransform buttonRt,
        EquipmentTab equipmentTab,
        SlotView? vestSlot,
        SlotView? armorSlot,
        SlotView? headwearSlot)
    {
        buttonRt.localScale = Vector3.one;
        buttonRt.sizeDelta = new Vector2(
            InsuranceControlPlugin.ButtonWidth.Value,
            InsuranceControlPlugin.ButtonHeight.Value
        );

        if (vestSlot != null)
        {
            PositionRelativeToSlot(buttonRt, vestSlot, above: true);
            return;
        }

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
        buttonRt.anchoredPosition = new Vector2(InsuranceControlPlugin.OffsetRight.Value, 180f);
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

        float width = InsuranceControlPlugin.ButtonWidth.Value;
        float height = InsuranceControlPlugin.ButtonHeight.Value;
        float gap = InsuranceControlPlugin.GapAboveAnchor.Value;
        float offsetRight = InsuranceControlPlugin.OffsetRight.Value;

        buttonRt.SetParent(parent, false);
        buttonRt.anchorMin = slotRt.anchorMin;
        buttonRt.anchorMax = slotRt.anchorMax;
        buttonRt.pivot = new Vector2(0.5f, 0.5f);
        buttonRt.sizeDelta = new Vector2(width, height);

        float y = (slotRt.rect.height * 0.5f) + (height * 0.5f) + gap;
        if (!above)
        {
            y = -y;
        }

        buttonRt.anchoredPosition = slotRt.anchoredPosition + new Vector2(offsetRight, y);
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
            InsuranceControlPlugin.Log.LogDebug($"[InsuranceControl] Vest lookup failed: {ex.Message}");
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
            InsuranceControlPlugin.Log.LogDebug($"[InsuranceControl] CommonUI back button lookup failed: {ex.Message}");
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
        if (!InsuranceControlPlugin.InsureAllEnabled.Value || InsureAllService.IsBusy)
        {
            return;
        }

        if (_inventoryController == null || _insurance == null)
        {
            InsuranceControlPlugin.Log.LogWarning("[InsuranceControl] Click ignored — no inventory context.");
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
