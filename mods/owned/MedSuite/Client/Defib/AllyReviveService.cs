using MedSuite.Client;
using System;
using System.Linq;
using Comfort.Common;
using EFT;
using EFT.Communications;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using HarmonyLib;
using UnityEngine;

namespace MedSuite.Client.Defib;

internal static class AllyReviveService
{
    private static bool _busy;

    public static bool TryStartReviveFromDefib(Player local, Item defib)
    {
        if (_busy || local == null || !MedSuitePlugin.Defib.Enabled.Value)
        {
            return false;
        }

        if (!AllyUtil.IsDefibrillator(defib))
        {
            return false;
        }

        if (!local.HealthController.IsAlive)
        {
            return false;
        }

        var ally = AllyUtil.FindNearestDownedAlly(local, MedSuitePlugin.Defib.ReviveRange.Value);
        if (ally == null)
        {
            // Soft Fika path: try ReviveInteractable on aim/nearby observed players.
            if (TryStartFikaRevive(local))
            {
                if (MedSuitePlugin.Defib.ConsumeDefibrillator.Value)
                {
                    ConsumeDefibrillator(local, defib);
                }

                return true;
            }

            NotificationManagerClass.DisplayMessageNotification(
                "No downed ally in range.",
                ENotificationDurationType.Default,
                ENotificationIconType.Alert,
                Color.yellow
            );
            return true; // consume the hotkey press; don't TryProceed barter item
        }

        if (local.CurrentState is not IdleStateClass)
        {
            NotificationManagerClass.DisplayWarningNotification("Stand still to revive.");
            return true;
        }

        // GamePlayerOwner is usually on the same GameObject as the local Player.
        var owner = local.GetComponent<GamePlayerOwner>()
            ?? local.GetComponentInParent<GamePlayerOwner>()
            ?? local.gameObject.GetComponentInChildren<GamePlayerOwner>();

        var reviveTime = MedSuitePlugin.Defib.ReviveTime.Value;
        var allyId = ally.ProfileId;
        var allyName = ally.Profile?.Nickname ?? "Ally";

        _busy = true;
        owner?.ShowObjectivesPanel($"Reviving {allyName}", reviveTime);

        try
        {
            local.CurrentManagedState.Plant(true, false, reviveTime, success =>
            {
                try
                {
                    owner?.CloseObjectivesPanel();

                    if (!success || !local.HealthController.IsAlive)
                    {
                        MedSuitePlugin.DefibDebug("Revive channel cancelled.");
                        return;
                    }

                    var target = Comfort.Common.Singleton<GameWorld>.Instance?.GetAlivePlayerByProfileID(allyId);
                    if (target == null || !AllyDownedTracker.IsDowned(allyId))
                    {
                        NotificationManagerClass.DisplayWarningNotification("Ally is no longer revivable.");
                        return;
                    }

                    ApplyRevive(target);

                    if (MedSuitePlugin.Defib.ConsumeDefibrillator.Value)
                    {
                        ConsumeDefibrillator(local, defib);
                    }

                    NotificationManagerClass.DisplayMessageNotification(
                        $"{allyName} revived.",
                        ENotificationDurationType.Default,
                        ENotificationIconType.Default,
                        Color.green
                    );
                }
                catch (Exception ex)
                {
                    MedSuitePlugin.Log.LogError($"[MedSuite:Defib] Revive callback failed: {ex}");
                }
                finally
                {
                    _busy = false;
                }
            });
        }
        catch (Exception ex)
        {
            _busy = false;
            owner?.CloseObjectivesPanel();
            MedSuitePlugin.Log.LogError($"[MedSuite:Defib] Failed to start revive plant: {ex}");
            return true;
        }

        MedSuitePlugin.DefibDebug($"Started revive channel on {allyId}");
        return true;
    }

    private static void ApplyRevive(Player ally)
    {
        var hc = ally.ActiveHealthController;
        if (hc == null)
        {
            return;
        }

        hc.IsAlive = true;

        if (MedSuitePlugin.Defib.FullHealOnRevive.Value)
        {
            hc.RestoreFullHealth();
        }
        else
        {
            foreach (EBodyPart part in Enum.GetValues(typeof(EBodyPart)))
            {
                if (part == EBodyPart.Common)
                {
                    continue;
                }

                if (hc.IsBodyPartDestroyed(part))
                {
                    hc.FullRestoreBodyPart(part);
                }

                try
                {
                    hc.RemoveNegativeEffects(part);
                }
                catch
                {
                    // Some builds gate RemoveNegativeEffects.
                }
            }
        }

        try
        {
            ally.MovementContext.IsInPronePose = false;
            ally.MovementContext.SetPoseLevel(1f, false);
        }
        catch
        {
            // ignore
        }

        AllyDownedTracker.OnRevived(ally);
    }

    private static void ConsumeDefibrillator(Player local, Item defib)
    {
        try
        {
            // Prefer the exact bound instance; fall back to any matching template in equipment.
            var item = defib;
            if (item == null || !AllyUtil.IsDefibrillator(item))
            {
                item = local.Inventory
                    .GetPlayerItems(EPlayerItems.Equipment)
                    .FirstOrDefault(AllyUtil.IsDefibrillator);
            }

            if (item == null)
            {
                MedSuitePlugin.Log.LogWarning("[MedSuite:Defib] No defibrillator to consume.");
                return;
            }

            // Spend the single charge first so UI (1/1 -> 0/1) matches behaviour,
            // then discard — ThrowItem only drops loot in the world and leaves the item usable.
            if (item.TryGetItemComponent<ResourceComponent>(out var resource))
            {
                resource.Value = 0f;
            }

            var controller = local.InventoryController;
            var result = InteractionsHandlerClass.DiscardWithoutRestrictions(item, controller);
            if (!result.Succeeded)
            {
                MedSuitePlugin.Log.LogWarning(
                    $"[MedSuite:Defib] Discard failed for {item.Id}: {result.Error}"
                );
                return;
            }

            try
            {
                result.Value.RaiseEvents(controller, CommandStatus.Begin);
                result.Value.RaiseEvents(controller, CommandStatus.Succeed);
            }
            catch (Exception ex)
            {
                MedSuitePlugin.Log.LogWarning($"[MedSuite:Defib] RaiseEvents after discard: {ex.Message}");
            }

            try
            {
                controller.TryRunNetworkTransaction(result, null);
            }
            catch (Exception ex)
            {
                MedSuitePlugin.Log.LogWarning(
                    $"[MedSuite:Defib] TryRunNetworkTransaction after discard: {ex.Message}"
                );
            }

            MedSuitePlugin.DefibDebug($"Consumed defibrillator {item.Id} (resource spent + discarded).");
        }
        catch (Exception ex)
        {
            MedSuitePlugin.Log.LogError($"[MedSuite:Defib] Consume failed: {ex}");
        }
    }

    /// <summary>
    /// Soft Fika integration: if a nearby player has ReviveInteractable, start its revive.
    /// </summary>
    private static bool TryStartFikaRevive(Player local)
    {
        if (!Singleton<GameWorld>.Instantiated)
        {
            return false;
        }

        var range = MedSuitePlugin.Defib.ReviveRange.Value;
        Player? best = null;
        Component? bestInteractable = null;
        var bestDist = range;

        foreach (var player in Singleton<GameWorld>.Instance.AllAlivePlayersList)
        {
            if (player == null || player.IsYourPlayer || !AllyUtil.IsAlly(local, player))
            {
                continue;
            }

            var dist = Vector3.Distance(local.Position, player.Position);
            if (dist > bestDist)
            {
                continue;
            }

            var interactable = FindReviveInteractable(player);
            if (interactable == null)
            {
                continue;
            }

            bestDist = dist;
            best = player;
            bestInteractable = interactable;
        }

        // Also scan disabled / observed objects under RegisteredPlayers.
        if (bestInteractable == null)
        {
            foreach (var iPlayer in Singleton<GameWorld>.Instance.RegisteredPlayers)
            {
                if (iPlayer is not Player player || player.IsYourPlayer)
                {
                    continue;
                }

                if (!AllyUtil.IsAlly(local, player))
                {
                    continue;
                }

                var dist = Vector3.Distance(local.Position, player.Position);
                if (dist > bestDist)
                {
                    continue;
                }

                var interactable = FindReviveInteractable(player);
                if (interactable == null)
                {
                    continue;
                }

                bestDist = dist;
                best = player;
                bestInteractable = interactable;
            }
        }

        if (bestInteractable == null)
        {
            return false;
        }

        var start = AccessTools.Method(bestInteractable.GetType(), "StartRevive");
        if (start == null)
        {
            MedSuitePlugin.DefibDebug("Fika ReviveInteractable.StartRevive not found.");
            return false;
        }

        start.Invoke(bestInteractable, null);
        MedSuitePlugin.DefibDebug($"Started Fika revive on {best?.ProfileId}");
        return true;
    }

    private static Component? FindReviveInteractable(Player player)
    {
        try
        {
            var comps = player.gameObject.GetComponents<Component>();
            foreach (var c in comps)
            {
                if (c != null && c.GetType().Name == "ReviveInteractable")
                {
                    return c;
                }
            }
        }
        catch
        {
            // ignore
        }

        return null;
    }
}
