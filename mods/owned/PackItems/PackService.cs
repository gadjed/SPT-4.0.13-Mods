using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using UnityEngine;

namespace PackItems;

/// <summary>
/// Moves loose stash-grid items into a target container until the first compatible item
/// no longer fits. Skips filter-incompatible, pinned, and locked items without stopping.
/// </summary>
internal static class PackService
{
    private static bool _busy;

    public static bool IsBusy => _busy;

    public static bool CanPack(Item? item)
    {
        return item is CompoundItem container
            && container.Grids != null
            && container.Grids.Length > 0
            && item is not StashItemClass;
    }

    public static void PackIntoContainer(CompoundItem container)
    {
        if (_busy)
        {
            Notify("Скласти предмети: вже виконується.", true);
            return;
        }

        if (!CanPack(container))
        {
            Notify("Скласти предмети: це не контейнер.", true);
            return;
        }

        if (container.Owner is not InventoryController inventoryController)
        {
            Notify("Скласти предмети: інвентар недоступний.", true);
            return;
        }

        var stash = inventoryController.Inventory?.Stash;
        if (stash?.Grid == null)
        {
            Notify("Скласти предмети: схрон недоступний.", true);
            return;
        }

        _ = PackAsync(container, inventoryController, stash);
    }

    private static async Task PackAsync(
        CompoundItem container,
        InventoryController inventoryController,
        StashItemClass stash)
    {
        _busy = true;
        int moved = 0;

        try
        {
            // Top-level stash cells only, left-to-right / top-to-bottom (no nested cases).
            List<Item> candidates = stash.Grid.ContainedItems
                .OrderBy(pair => pair.Value.y)
                .ThenBy(pair => pair.Value.x)
                .Select(pair => pair.Key)
                .Where(item => item != null && item != container)
                .ToList();

            if (PackItemsPlugin.Debug.Value)
            {
                PackItemsPlugin.Log.LogInfo(
                    $"[PackItems] Packing into {container.ShortName.Localized()} " +
                    $"from {candidates.Count} stash item(s).");
            }

            foreach (Item item in candidates)
            {
                if (item.PinLockState is EItemPinLockState.Pinned or EItemPinLockState.Locked)
                {
                    continue;
                }

                if (!ContainerAccepts(container, item))
                {
                    continue;
                }

                var operation = InteractionsHandlerClass.QuickFindAppropriatePlace(
                    item,
                    inventoryController,
                    new CompoundItem[] { container },
                    InteractionsHandlerClass.EMoveItemOrder.Apply,
                    true);

                if (operation.Failed)
                {
                    // First compatible item that does not fit — stop (no further optimization).
                    if (PackItemsPlugin.Debug.Value)
                    {
                        PackItemsPlugin.Log.LogInfo(
                            $"[PackItems] Stop at {item.ShortName.Localized()}: {operation.Error}");
                    }

                    break;
                }

                var result = await inventoryController.TryRunNetworkTransaction(operation);
                if (result.Failed)
                {
                    if (PackItemsPlugin.Debug.Value)
                    {
                        PackItemsPlugin.Log.LogWarning(
                            $"[PackItems] Network move failed for {item.ShortName.Localized()}: {result.Error}");
                    }

                    break;
                }

                moved++;
            }

            if (moved > 0)
            {
                Notify($"Складено предметів: {moved}.", false);
            }
            else
            {
                Notify("Немає підходящих предметів у схроні (або немає місця).", false);
            }
        }
        catch (Exception ex)
        {
            PackItemsPlugin.Log.LogError($"[PackItems] Pack failed: {ex}");
            Notify("Скласти предмети: помилка (див. лог BepInEx).", true);
        }
        finally
        {
            _busy = false;
        }
    }

    private static bool ContainerAccepts(CompoundItem container, Item item)
    {
        foreach (StashGridClass grid in container.Grids)
        {
            ItemFilter[]? filters = grid.Filters;
            if (filters == null || filters.Length == 0)
            {
                return true;
            }

            if (GClass3124.CheckItemFilter(filters, item))
            {
                return true;
            }
        }

        return false;
    }

    private static bool InRaid()
    {
        AbstractGame? game = Singleton<AbstractGame>.Instance;
        return game != null && game.InRaid;
    }

    public static bool IsAllowedView(EItemViewType viewType)
    {
        if (InRaid())
        {
            return false;
        }

        return viewType is EItemViewType.Inventory or EItemViewType.TradingPlayer;
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

    public static Sprite? MenuIcon()
    {
        try
        {
            return CacheResourcesPopAbstractClass.Pop<Sprite>("Characteristics/Icons/UnloadAmmo");
        }
        catch
        {
            return null;
        }
    }
}
