using System.Collections.Generic;
using EFT;
using EFT.HealthSystem;
using EFT.Communications;
using UnityEngine;

namespace DefibAllyRevive;

/// <summary>
/// Tracks allies that entered a revivable downed state instead of permanent death.
/// </summary>
internal static class AllyDownedTracker
{
    private sealed class DownedState
    {
        public float TimeLeft;
        public bool Permanent;
    }

    private static readonly Dictionary<string, DownedState> States = new();

    public static bool IsDowned(string profileId)
    {
        return States.TryGetValue(profileId, out var state) && !state.Permanent;
    }

    public static bool TryEnterDowned(Player player, EDamageType damageType)
    {
        if (player == null || player.IsYourPlayer)
        {
            return false;
        }

        if (!DefibAllyRevivePlugin.Enabled.Value)
        {
            return false;
        }

        var local = AllyUtil.GetLocalPlayer();
        if (local == null || !AllyUtil.IsAlly(local, player))
        {
            return false;
        }

        if (States.TryGetValue(player.ProfileId, out var existing) && existing.Permanent)
        {
            return false;
        }

        // Exhaustion / dehydration / stims → real death.
        if ((damageType & (EDamageType.Exhaustion | EDamageType.Dehydration | EDamageType.Stimulator)) != 0)
        {
            return false;
        }

        var hc = player.ActiveHealthController;
        if (hc == null)
        {
            return false;
        }

        // Keep alive with vital parts restored so the body stays in the world.
        hc.IsAlive = true;
        RestoreVitalParts(hc);

        ApplyDownedPose(player);

        var bleedout = DefibAllyRevivePlugin.BleedoutTime.Value;
        States[player.ProfileId] = new DownedState
        {
            TimeLeft = bleedout > 0f ? bleedout : float.PositiveInfinity,
            Permanent = false,
        };

        if (local.IsYourPlayer)
        {
            var name = player.Profile?.Nickname ?? "Ally";
            NotificationManagerClass.DisplayMessageNotification(
                $"{name} is down — use defibrillator from a quick slot to revive.",
                ENotificationDurationType.Default,
                ENotificationIconType.Default,
                Color.yellow
            );
        }

        DefibAllyRevivePlugin.DebugLog(
            $"Downed ally {player.ProfileId} ({player.Profile?.Nickname}), bleedout={bleedout}"
        );
        return true;
    }

    public static void Clear(string profileId)
    {
        States.Remove(profileId);
    }

    public static void ClearAll()
    {
        States.Clear();
    }

    public static void Tick(float deltaTime)
    {
        if (States.Count == 0)
        {
            return;
        }

        var local = AllyUtil.GetLocalPlayer();
        if (local == null || !SingletonReady())
        {
            return;
        }

        var world = Comfort.Common.Singleton<GameWorld>.Instance;
        var toKill = new List<string>();

        foreach (var pair in States)
        {
            if (pair.Value.Permanent)
            {
                continue;
            }

            if (!float.IsPositiveInfinity(pair.Value.TimeLeft))
            {
                pair.Value.TimeLeft -= deltaTime;
            }

            var player = world.GetAlivePlayerByProfileID(pair.Key);
            if (player == null)
            {
                toKill.Add(pair.Key);
                continue;
            }

            // Keep them pinned while downed.
            ApplyDownedPose(player);

            if (!float.IsPositiveInfinity(pair.Value.TimeLeft) && pair.Value.TimeLeft <= 0f)
            {
                pair.Value.Permanent = true;
                toKill.Add(pair.Key);
                FinalizeBleedout(player);
            }
        }

        foreach (var id in toKill)
        {
            if (States.TryGetValue(id, out var state) && state.Permanent)
            {
                // Keep permanent marker briefly so Kill prefix allows death; then remove.
                States.Remove(id);
            }
            else
            {
                States.Remove(id);
            }
        }
    }

    public static void OnRevived(Player player)
    {
        if (player == null)
        {
            return;
        }

        Clear(player.ProfileId);
        player.AIData?.BotOwner?.MovementResume();
        DefibAllyRevivePlugin.DebugLog($"Cleared downed state for {player.ProfileId}");
    }

    private static void FinalizeBleedout(Player player)
    {
        try
        {
            var hc = player.ActiveHealthController;
            if (hc == null || !hc.IsAlive)
            {
                return;
            }

            DefibAllyRevivePlugin.DebugLog($"Bleedout finished for {player.ProfileId}");
            hc.Kill(EDamageType.Undefined);
        }
        catch (System.Exception ex)
        {
            DefibAllyRevivePlugin.Log.LogError($"[DefibAllyRevive] Bleedout kill failed: {ex}");
        }
    }

    private static void ApplyDownedPose(Player player)
    {
        try
        {
            player.HandsController.IsAiming = false;
            player.MovementContext.EnableSprint(false);
            player.MovementContext.SetPoseLevel(0f, true);
            player.MovementContext.IsInPronePose = true;
            player.AIData?.BotOwner?.StopMove();
            player.AIData?.BotOwner?.MovementPause(0.5f);
        }
        catch
        {
            // Pose APIs can throw if the player is mid-despawn.
        }
    }

    private static void RestoreVitalParts(ActiveHealthController hc)
    {
        if (hc.IsBodyPartDestroyed(EBodyPart.Head))
        {
            hc.FullRestoreBodyPart(EBodyPart.Head);
        }

        if (hc.IsBodyPartDestroyed(EBodyPart.Chest))
        {
            hc.FullRestoreBodyPart(EBodyPart.Chest);
        }

        // Leave a sliver of HP so they stay "alive" but critical.
        TrySetMinimalHp(hc, EBodyPart.Head);
        TrySetMinimalHp(hc, EBodyPart.Chest);
    }

    private static void TrySetMinimalHp(ActiveHealthController hc, EBodyPart part)
    {
        try
        {
            var health = hc.GetBodyPartHealth(part);
            if (health.Current < 1f)
            {
                hc.ChangeHealth(part, 1f - health.Current, default);
            }
        }
        catch
        {
            // Ignore if body-part health API differs.
        }
    }

    private static bool SingletonReady()
    {
        return Comfort.Common.Singleton<GameWorld>.Instantiated;
    }
}
