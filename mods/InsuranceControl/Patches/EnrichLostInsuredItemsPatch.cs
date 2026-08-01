using System.Collections.Frozen;
using System.Reflection;
using HarmonyLib;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Services.InRaid;

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
    public static void Prefix(PmcData preRaidPmcProfile, EndLocalRaidRequestData request)
    {
        var config = InsuranceControlMod.Config;
        if (!config.ReturnMagazinesWithAmmo && !config.ReturnContainersWithContents)
        {
            return;
        }

        if (request.LostInsuredItems is null || !request.LostInsuredItems.Any())
        {
            return;
        }

        if (preRaidPmcProfile.InsuredItems is null || preRaidPmcProfile.Inventory?.Items is null)
        {
            return;
        }

        var itemHelper = InsuranceControlMod.ItemHelper;
        var lostList = request.LostInsuredItems.ToList();
        var knownIds = lostList.Select(item => item.Id).ToHashSet();

        // Children may still be on the corpse (death) or already present inside LostInsuredItems (dropped kits).
        var itemPool = lostList
            .Concat(preRaidPmcProfile.Inventory.Items)
            .GroupBy(item => item.Id)
            .Select(group => group.First())
            .ToList();

        var extras = new List<Item>();

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
                    continue;
                }

                if (includeContents)
                {
                    extras.Add(child);
                    EnsureTemporaryInsurance(preRaidPmcProfile, child.Id, parentInsurance.TId);
                }
            }
        }

        if (extras.Count == 0)
        {
            return;
        }

        request.LostInsuredItems = lostList.Concat(extras);
    }

    private static bool IsAmmoChild(Item child, SPTarkov.Server.Core.Helpers.Items.ItemHelper itemHelper)
    {
        if (!itemHelper.IsOfBaseclass(child.Template, BaseClasses.AMMO))
        {
            return false;
        }

        return child.SlotId is not null && AmmoSlotIds.Contains(child.SlotId);
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
