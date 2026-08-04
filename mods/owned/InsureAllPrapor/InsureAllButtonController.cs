using System;
using Comfort.Common;
using EFT.InventoryLogic;
using EFT.UI;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using UnityEngine;

namespace InsureAllPrapor;

/// <summary>
/// Clones a vanilla <see cref="DefaultUIButton"/> and places it next to the helmet slot.
/// </summary>
internal static class InsureAllButtonController
{
    private const string ButtonName = "InsureAllPraporButton";

    private static DefaultUIButton? _button;
    private static InventoryController? _inventoryController;
    private static InsuranceCompanyClass? _insurance;

    public static void Show(EquipmentTab equipmentTab, SlotView headwearSlot, InventoryController inventoryController, InsuranceCompanyClass insurance)
    {
        _inventoryController = inventoryController;
        _insurance = insurance;

        if (!InsureAllPraporPlugin.Enabled.Value)
        {
            Hide();
            return;
        }

        EnsureButton(equipmentTab, headwearSlot);
        if (_button == null)
        {
            return;
        }

        _button.SetHeaderText(InsureAllPraporPlugin.ButtonLabel.Value);
        _button.Interactable = !InsureAllService.IsBusy;
        _button.gameObject.SetActive(true);
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

    private static void EnsureButton(EquipmentTab equipmentTab, SlotView headwearSlot)
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

        var parent = headwearSlot != null
            ? headwearSlot.transform.parent
            : equipmentTab.transform;

        var clone = UnityEngine.Object.Instantiate(template, parent, false);
        clone.name = ButtonName;
        clone.gameObject.SetActive(true);

        // Drop layout constraints from the cloned back/ready button so we can place freely.
        var layout = clone.GetComponent<UnityEngine.UI.LayoutElement>();
        if (layout != null)
        {
            layout.minWidth = -1f;
            layout.preferredWidth = -1f;
            layout.flexibleWidth = -1f;
            layout.ignoreLayout = true;
        }

        PositionNearHelmet(clone.RectTransform, headwearSlot);
        clone.OnClick.RemoveAllListeners();
        clone.OnClick.AddListener(OnClicked);

        _button = clone;
        InsureAllPraporPlugin.Log.LogInfo("[InsureAllPrapor] Stash insure-all button created.");
    }

    private static void PositionNearHelmet(RectTransform buttonRt, SlotView? headwearSlot)
    {
        buttonRt.localScale = Vector3.one;
        buttonRt.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRt.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRt.pivot = new Vector2(0.5f, 0.5f);
        buttonRt.sizeDelta = new Vector2(180f, 28f);

        if (headwearSlot == null)
        {
            buttonRt.anchoredPosition = new Vector2(120f, 200f);
            return;
        }

        var helmetRt = headwearSlot.RectTransform;
        // Place to the right of the helmet slot within the same parent.
        Vector2 local = parentLocalPoint(buttonRt.parent as RectTransform, helmetRt);
        buttonRt.anchoredPosition = local + new Vector2(helmetRt.rect.width * 0.5f + 100f, 0f);
    }

    private static Vector2 parentLocalPoint(RectTransform? parent, RectTransform child)
    {
        if (parent == null)
        {
            return child.anchoredPosition;
        }

        Vector3 world = child.TransformPoint(child.rect.center);
        Vector3 local = parent.InverseTransformPoint(world);
        return new Vector2(local.x, local.y);
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
