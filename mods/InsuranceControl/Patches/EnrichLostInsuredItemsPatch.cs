using System.Collections.Frozen;
using System.Reflection;
using HarmonyLib;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Services;

namespace InsuranceControl.Patches;

/// <summary>
/// Expands lost insured packages so magazines keep ammo and backpacks/rigs keep grid contents.
/// </summary>
public class EnrichLostInsuredItemsPatch : AbstractPatch
{
    private static readonly FrozenSet<string> AmmoSlotIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "cartridges",
        "patron_in_weapon",
        "patron_in_weapon_000",
        "patron_in_weapon_001",
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(LocationLifecycleService), "HandleInsuredItemLostEvent");
    }

    [PatchPrefix]
    public static void Prefix(MongoId sessionId, PmcData preRaidPmcProfile, EndLocalRaidRequestData request)
    {
        var config = InsuranceControlMod.Config;
        if (!config.ReturnMagazinesWithAmmo && !config.ReturnContainersWithContents)
        {
            RaidInventorySnapshot.Clear(sessionId);
            return;
        }

        if (request.LostInsuredItems is null || !request.LostInsuredItems.Any())
        {
            RaidInventorySnapshot.Clear(sessionId);
            return;
        }

        if (preRaidPmcProfile.InsuredItems is null || preRaidPmcProfile.Inventory?.Items is null)
        {
            RaidInventorySnapshot.Clear(sessionId);
            return;
        }

        var itemHelper = InsuranceControlMod.ItemHelper;
        var lostList = request.LostInsuredItems.ToList();
        var knownIds = lostList.Select(item => item.Id).ToHashSet();

        RaidInventorySnapshot.TryGet(sessionId, out var snapshot);
        var itemPool = BuildItemPool(lostList, preRaidPmcProfile.Inventory.Items, snapshot);

        var extras = new List<Item>();
        var ammoAdded = 0;
        var contentsAdded = 0;

        foreach (var lostItem in lostList)
        {
            var parentInsurance = preRaidPmcProfile.InsuredItems.FirstOrDefault(insured => insured.ItemId == lostItem.Id);
            if (parentInsurance is null)
            {
                continue;
            }

            var includeAmmo = config.ReturnMagazinesWithAmmo
                && (
                    itemHelper.IsOfBaseclass(lostItem.Template, BaseClasses.MAGAZINE)
                    || itemHelper.IsOfBaseclass(lostItem.Template, BaseClasses.CYLINDER_MAGAZINE)
                    || itemHelper.IsOfBaseclass(lostItem.Template, BaseClasses.WEAPON)
                );

            var includeContents = config.ReturnContainersWithContents
                && (
                    itemHelper.IsOfBaseclass(lostItem.Template, BaseClasses.BACKPACK)
                    || itemHelper.IsOfBaseclass(lostItem.Template, BaseClasses.VEST)
                );

            if (!includeAmmo && !includeContents)
            {
                continue;
            }

            foreach (var child in itemPool.GetItemWithChildren(lostItem.Id).Skip(1))
            {
                if (!knownIds.Add(child.Id))
                {
                    continue;
                }

                if (includeAmmo && IsAmmoChild(child, itemHelper))
                {
                    extras.Add(child);
                    EnsureTemporaryInsurance(preRaidPmcProfile, child.Id, parentInsurance.TId);
                    ammoAdded++;
                    continue;
                }

                if (includeContents)
                {
                    extras.Add(child);
                    EnsureTemporaryInsurance(preRaidPmcProfile, child.Id, parentInsurance.TId);
                    contentsAdded++;
                }
            }
        }

        RaidInventorySnapshot.Clear(sessionId);

        if (extras.Count == 0)
        {
            InsuranceControlMod.Logger?.Warning(
                "[InsuranceControl] Enrichment found no nested ammo/contents for lost insured items. "
                    + $"LostCount={lostList.Count}, Snapshot={(snapshot is null ? "none" : snapshot.Count.ToString())}."
            );
            return;
        }

        InsuranceControlMod.Logger?.LogWithColor(
            $"[InsuranceControl] Enriched insurance package: +{ammoAdded} ammo, +{contentsAdded} container items "
                + $"(snapshot={(snapshot is null ? "no" : "yes")}).",
            LogTextColor.Cyan
        );

        request.LostInsuredItems = lostList.Concat(extras);
    }

    /// <summary>
    /// Merge lost list + post-raid inventory + pre-raid snapshot.
    /// Post-raid wins on id conflicts (correct mid-raid ammo counts); snapshot fills items looted off the corpse.
    /// </summary>
    private static List<Item> BuildItemPool(List<Item> lostList, List<Item> inventoryItems, List<Item>? snapshot)
    {
        var byId = new Dictionary<MongoId, Item>();

        if (snapshot is not null)
        {
            foreach (var item in snapshot)
            {
                byId[item.Id] = item;
            }
        }

        foreach (var item in inventoryItems)
        {
            byId[item.Id] = item;
        }

        foreach (var item in lostList)
        {
            byId.TryAdd(item.Id, item);
        }

        return byId.Values.ToList();
    }

    private static bool IsAmmoChild(Item child, ItemHelper itemHelper)
    {
        if (!itemHelper.IsOfBaseclass(child.Template, BaseClasses.AMMO))
        {
            return false;
        }

        if (child.SlotId is null)
        {
            return false;
        }

        if (AmmoSlotIds.Contains(child.SlotId))
        {
            return true;
        }

        // Revolver cylinders use camora_0, camora_1, ...
        return child.SlotId.StartsWith("camora", StringComparison.OrdinalIgnoreCase);
    }

    private static void EnsureTemporaryInsurance(PmcData profile, MongoId itemId, MongoId traderId)
    {
        if (profile.InsuredItems!.Any(insured => insured.ItemId == itemId))
        {
            return;
        }

        profile.InsuredItems!.Add(new InsuredItem { ItemId = itemId, TId = traderId });
    }
}
