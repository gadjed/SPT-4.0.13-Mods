using System;
using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using EFT.Communications;
using EFT.InventoryLogic;
using EFT.UI;

namespace InsureAllPrapor;

/// <summary>
/// Collects equippable player gear and insures it with Prapor via vanilla <see cref="InsuranceCompanyClass"/>.
/// No confirmation dialog — same purchase path as context-menu insure.
/// </summary>
internal static class InsureAllService
{
    private static bool _busy;

    public static bool IsBusy => _busy;

    public static void InsureEquippedGear(
        InventoryController inventoryController,
        InsuranceCompanyClass insurance,
        Action? onFinished = null)
    {
        if (_busy)
        {
            return;
        }

        if (inventoryController?.Inventory == null || insurance == null)
        {
            Notify("Застраховать все: инвентарь недоступен.", true);
            onFinished?.Invoke();
            return;
        }

        var prapor = insurance.Insurers?.FirstOrDefault(t =>
            string.Equals(t.Id, InsureAllPraporPlugin.PraporTraderId, StringComparison.OrdinalIgnoreCase));
        if (prapor == null)
        {
            Notify("Застраховать все: Прапор недоступен как страховщик.", true);
            onFinished?.Invoke();
            return;
        }

        var rootItems = CollectEquipmentItems(inventoryController.Inventory);
        if (rootItems.Count == 0)
        {
            Notify("Застраховать все: нечего страховать на персонаже.", false);
            onFinished?.Invoke();
            return;
        }

        var insuranceItems = rootItems.Select(ItemClass.FindOrCreate).ToList();
        var insurable = BuildInsurableList(insurance, insuranceItems);

        if (insurable.Count == 0)
        {
            Notify("Застраховать все: всё уже застраховано.", false);
            onFinished?.Invoke();
            return;
        }

        _busy = true;
        if (InsureAllPraporPlugin.Debug.Value)
        {
            InsureAllPraporPlugin.Log.LogInfo(
                $"[InsureAllPrapor] Pricing {insurable.Count} item(s) with Prapor ({prapor.Id}).");
        }

        try
        {
            insurance.GetInsurePriceAsync(insurable, _ =>
            {
                try
                {
                    int price = SumPrice(insurance, prapor.Id, insurable);
                    int rubles = GetPlayerRubles(inventoryController.Inventory);
                    if (price > rubles)
                    {
                        Notify($"Застраховать все: недостаточно денег (нужно {price:N0} ₽).", true);
                        Finish(onFinished);
                        return;
                    }

                    insurance.SelectedInsurerId = prapor.Id;
                    insurance.InsureItems(insurable, result =>
                    {
                        try
                        {
                            if (result.Failed)
                            {
                                Notify($"Страховка не удалась: {result.Error}", true);
                                InsureAllPraporPlugin.Log.LogWarning(
                                    $"[InsureAllPrapor] InsureItems failed: {result.Error}");
                            }
                            else
                            {
                                Notify($"Застраховано у Прапора: {insurable.Count} шт. ({price:N0} ₽).", false);
                                if (InsureAllPraporPlugin.Debug.Value)
                                {
                                    InsureAllPraporPlugin.Log.LogInfo(
                                        $"[InsureAllPrapor] Insured {insurable.Count} item(s) for {price} RUB.");
                                }
                            }
                        }
                        finally
                        {
                            Finish(onFinished);
                        }
                    });
                }
                catch (Exception ex)
                {
                    InsureAllPraporPlugin.Log.LogError($"[InsureAllPrapor] Price callback error: {ex}");
                    Notify("Страховка не удалась (см. лог BepInEx).", true);
                    Finish(onFinished);
                }
            });
        }
        catch (Exception ex)
        {
            InsureAllPraporPlugin.Log.LogError($"[InsureAllPrapor] GetInsurePriceAsync error: {ex}");
            Notify("Страховка не удалась (см. лог BepInEx).", true);
            Finish(onFinished);
        }
    }

    private static void Finish(Action? onFinished)
    {
        _busy = false;
        onFinished?.Invoke();
    }

    private static List<Item> CollectEquipmentItems(Inventory inventory)
    {
        // Equipment = PMC loadout (slots + nested vest/backpack/pocket contents). Not stash grids.
        return inventory.GetPlayerItems(EPlayerItems.Equipment)
            .Where(i => i != null)
            .ToList();
    }

    private static List<ItemClass> BuildInsurableList(
        InsuranceCompanyClass insurance,
        List<ItemClass> roots)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<ItemClass>();

        void TryAdd(ItemClass item)
        {
            if (item == null || string.IsNullOrEmpty(item.Id) || !seen.Add(item.Id))
            {
                return;
            }

            if (!insurance.ItemTypeAvailableForInsurance(item) || insurance.InsuredItems.Contains(item))
            {
                return;
            }

            result.Add(item);
        }

        foreach (var root in roots)
        {
            TryAdd(root);
            foreach (var child in insurance.GetFlattenChildren(root))
            {
                TryAdd(child);
            }
        }

        return result;
    }

    private static int SumPrice(InsuranceCompanyClass insurance, string insurerId, List<ItemClass> items)
    {
        int total = 0;
        foreach (var item in items)
        {
            var summary = insurance.InsureSummary.GetInsurePrice(item, insurerId);
            if (summary != null && summary.Loaded)
            {
                total += summary.Amount;
            }
        }

        return total;
    }

    private static int GetPlayerRubles(Inventory inventory)
    {
        var stash = inventory?.Stash;
        if (stash?.Grid == null)
        {
            return 0;
        }

        var sums = GClass3373.GetMoneySums(stash.Grid.ContainedItems.Keys);
        return sums.TryGetValue(ECurrencyType.RUB, out int rub) ? rub : 0;
    }

    private static void Notify(string message, bool warning)
    {
        if (warning)
        {
            NotificationManagerClass.DisplayWarningNotification(message);
        }
        else
        {
            NotificationManagerClass.DisplayMessageNotification(message);
        }
    }
}
