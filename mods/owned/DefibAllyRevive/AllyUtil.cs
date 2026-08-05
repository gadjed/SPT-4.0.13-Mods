using System;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using UnityEngine;

namespace DefibAllyRevive;

internal static class AllyUtil
{
    public static bool IsDefibrillator(Item? item)
    {
        if (item == null)
        {
            return false;
        }

        return string.Equals(
            item.TemplateId.ToString(),
            DefibAllyRevivePlugin.DefibrillatorTemplateId,
            StringComparison.Ordinal
        );
    }

    public static Player? GetLocalPlayer()
    {
        if (!Singleton<GameWorld>.Instantiated)
        {
            return null;
        }

        var main = Singleton<GameWorld>.Instance.MainPlayer;
        if (main != null && main.IsYourPlayer)
        {
            return main;
        }

        return GamePlayerOwner.MyPlayer;
    }

    public static bool IsAlly(Player local, Player other)
    {
        if (local == null || other == null || other.IsYourPlayer || ReferenceEquals(local, other))
        {
            return false;
        }

        var localGroup = local.GroupId;
        var otherGroup = other.GroupId;
        var sameGroup = !string.IsNullOrEmpty(localGroup)
            && string.Equals(localGroup, otherGroup, StringComparison.Ordinal);

        if (sameGroup)
        {
            return true;
        }

        if (DefibAllyRevivePlugin.RequireSameGroup.Value)
        {
            return false;
        }

        if (!DefibAllyRevivePlugin.AllowSameSide.Value)
        {
            return false;
        }

        if (local.Side == EPlayerSide.Savage || other.Side == EPlayerSide.Savage)
        {
            return false;
        }

        return local.Side == other.Side;
    }

    public static Player? FindNearestDownedAlly(Player local, float maxRange)
    {
        if (!Singleton<GameWorld>.Instantiated)
        {
            return null;
        }

        var world = Singleton<GameWorld>.Instance;
        Player? best = null;
        var bestDist = maxRange;

        foreach (var player in world.AllAlivePlayersList)
        {
            if (player == null || !AllyDownedTracker.IsDowned(player.ProfileId))
            {
                continue;
            }

            if (!IsAlly(local, player))
            {
                continue;
            }

            var dist = Vector3.Distance(local.Position, player.Position);
            if (dist <= bestDist)
            {
                bestDist = dist;
                best = player;
            }
        }

        return best;
    }
}
