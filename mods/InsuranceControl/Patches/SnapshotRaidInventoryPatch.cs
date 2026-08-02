using System.Reflection;
using HarmonyLib;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Services;

namespace InsuranceControl.Patches;

/// <summary>
/// Captures a deep clone of PMC inventory at raid start for insurance content enrichment.
/// </summary>
public class SnapshotRaidInventoryPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(LocationLifecycleService), nameof(LocationLifecycleService.StartLocalRaid));
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionId, StartLocalRaidRequestData request)
    {
        // Insurance is PMC-only; scav raids use a different inventory.
        if (!string.Equals(request.PlayerSide, "pmc", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var profileHelper = InsuranceControlMod.ProfileHelper;
        var cloner = InsuranceControlMod.Cloner;
        if (profileHelper is null || cloner is null)
        {
            return;
        }

        var pmc = profileHelper.GetPmcProfile(sessionId);
        var items = pmc?.Inventory?.Items;
        if (items is null || items.Count == 0)
        {
            return;
        }

        var clone = cloner.Clone(items);
        if (clone is null)
        {
            return;
        }

        RaidInventorySnapshot.Store(sessionId, clone);
        InsuranceControlMod.Logger?.LogWithColor(
            $"[InsuranceControl] Pre-raid inventory snapshot stored ({clone.Count} items).",
            LogTextColor.Cyan
        );
    }
}
