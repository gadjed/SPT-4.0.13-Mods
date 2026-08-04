using System.Collections.Generic;
using SAIN.Components;
using SAIN.Preset.GlobalSettings.Categories;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Helpers;

/// <summary>
/// StealthEngage fork helpers: PMCs that hear nearby gunfire while at peace
/// start a careful search instead of freezing or charging straight in.
/// </summary>
public static class StealthEngageHelpers
{
    /// <summary>
    /// Approximate longer horizontal playable axis (meters). Customs measured ~938×527.
    /// Used when StealthEngageUseMapRelativeDistance is enabled.
    /// </summary>
    private static readonly Dictionary<ELocation, float> MapLongerAxisMeters = new()
    {
        { ELocation.Customs, 940f },
        { ELocation.Factory, 80f },
        { ELocation.FactoryNight, 80f },
        { ELocation.Interchange, 450f },
        { ELocation.Labs, 120f },
        { ELocation.Lighthouse, 900f },
        { ELocation.Reserve, 650f },
        { ELocation.GroundZero, 350f },
        { ELocation.Shoreline, 1100f },
        { ELocation.Streets, 900f },
        { ELocation.Woods, 1500f },
        { ELocation.Terminal, 400f },
        { ELocation.Town, 300f },
        { ELocation.Labyrinth, 150f },
    };

    public static EHeardFromPeaceBehavior GetEffectiveHeardFromPeaceBehavior(BotComponent bot)
    {
        EHeardFromPeaceBehavior configured = bot.Info.PersonalitySettings.Search.HeardFromPeaceBehavior;

        // Keep intentional Charge / None / StealthEngage.
        // Convert Freeze/SearchNow to StealthEngage for PMCs so they approach carefully.
        if (!bot.Info.Profile.IsPMC)
        {
            return configured;
        }

        if (configured == EHeardFromPeaceBehavior.Freeze || configured == EHeardFromPeaceBehavior.SearchNow)
        {
            return EHeardFromPeaceBehavior.StealthEngage;
        }

        return configured;
    }

    public static float GetStealthEngageMaxDistance()
    {
        MindSettings mind = SAINPlugin.LoadedPreset.GlobalSettings.Mind;
        float absoluteMax = mind.StealthEngageAbsoluteMaxDistance;
        float absoluteMin = mind.StealthEngageAbsoluteMinDistance;

        if (!mind.StealthEngageUseMapRelativeDistance)
        {
            return Mathf.Clamp(absoluteMax, absoluteMin, absoluteMax);
        }

        float mapSize = GetMapLongerHorizontalSize();
        if (mapSize <= 1f)
        {
            return absoluteMax;
        }

        float relative = mapSize * mind.StealthEngageMapSizeFraction;
        return Mathf.Clamp(relative, absoluteMin, absoluteMax);
    }

    /// <summary>
    /// Longer horizontal axis for the current location. Customs ≈ 940 m.
    /// </summary>
    public static float GetMapLongerHorizontalSize()
    {
        GameWorldComponent gameWorld = GameWorldComponent.Instance;
        if (gameWorld?.Location == null)
        {
            return -1f;
        }

        ELocation location = gameWorld.Location.Location;
        if (MapLongerAxisMeters.TryGetValue(location, out float size))
        {
            return size;
        }

        return -1f;
    }

    public static bool WantsImmediateStealthSearch(BotComponent bot, Enemy enemy)
    {
        EHeardFromPeaceBehavior behavior = GetEffectiveHeardFromPeaceBehavior(bot);
        if (behavior != EHeardFromPeaceBehavior.StealthEngage && behavior != EHeardFromPeaceBehavior.SearchNow)
        {
            return false;
        }

        if (!enemy.Hearing.EnemyHeardFromPeace)
        {
            return false;
        }

        // StealthEngage prioritizes gunfire heard from peace; SearchNow keeps upstream any-sound behavior.
        if (behavior == EHeardFromPeaceBehavior.StealthEngage && !enemy.Hearing.EnemyGunshotHeardFromPeace)
        {
            return false;
        }

        float maxEngageDistance = GetStealthEngageMaxDistance();
        if (enemy.RealDistance > maxEngageDistance && !bot.Info.PersonalitySettings.Search.WillChaseDistantGunshots)
        {
            return false;
        }

        return true;
    }

    public static bool ShallBeStealthyApproaching(BotComponent bot, Enemy enemy)
    {
        if (!enemy.Hearing.EnemyHeardFromPeace)
        {
            return false;
        }

        if (GetEffectiveHeardFromPeaceBehavior(bot) != EHeardFromPeaceBehavior.StealthEngage)
        {
            return false;
        }

        // Stay stealthy for the whole gunshot-from-peace approach; otherwise use sneaky distance.
        if (enemy.Hearing.EnemyGunshotHeardFromPeace)
        {
            return true;
        }

        float maxDist = SAINPlugin.LoadedPreset.GlobalSettings.Mind.MaximumDistanceToBeSneaky;
        return enemy.RealDistance < maxDist;
    }
}
