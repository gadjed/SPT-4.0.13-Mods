using Comfort.Common;
using EFT;
using QuestingBots.BotLogic.ExternalMods;
using QuestingBots.BotLogic.ExternalMods.Functions.Hearing;
using QuestingBots.BotLogic.HiveMind;
using QuestingBots.Configuration;
using QuestingBots.Helpers;
using QuestingBots.Utils;
using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace QuestingBots.BotLogic.BotMonitor.Monitors
{
    public class BotHearingMonitor : AbstractBotMonitor
    {
        public bool IsSuspicious { get; private set; } = false;
        public bool IsPvpHunter { get; private set; } = false;

        private bool soundPlayedEventAdded = false;
        private float lastEnemySoundHeardTime = 0;
        private AbstractHearingFunction hearingFunction = null!;
        private double suspiciousTime = Singleton<ConfigUtil>.Instance.CurrentConfig.Questing.BotQuestingRequirements.HearingSensor.SuspiciousTime.Min;
        private float maxSuspiciousTime = 60;
        private float nextTimeSuspicionAllowed = 0;
        private Stopwatch totalSuspiciousTimer = new Stopwatch();
        private Stopwatch notSuspiciousTimer = Stopwatch.StartNew();
        private readonly System.Random random = new System.Random();

        public bool SuspicionAllowedByTime => Time.time >= nextTimeSuspicionAllowed;

        public BotHearingMonitor(BotOwner _botOwner) : base(_botOwner) { }

        public override void Start()
        {
            hearingFunction = ExternalModHandler.CreateHearingFunction(BotOwner);

            HearingSensorConfig hearingConfig = Singleton<ConfigUtil>.Instance.CurrentConfig.Questing.BotQuestingRequirements.HearingSensor;
            IsPvpHunter = random.NextDouble() * 100.0 < hearingConfig.PvpHunterChance;
            if (IsPvpHunter)
            {
                Singleton<LoggingUtil>.Instance.LogDebug(BotOwner.GetText() + " rolled as a PVP hunter");
            }

            if (!hearingConfig.Enabled)
            {
                return;
            }

            Singleton<BotEventHandler>.Instance.OnSoundPlayed += enemySoundHeard;
            soundPlayedEventAdded = true;

            BotOwner.GetPlayer.OnIPlayerDeadOrUnspawn += (player) => { removeSoundPlayedEvent(); };

            updateMaxSuspiciousTime();
        }

        public override void UpdateIfQuesting()
        {
            IsSuspicious = isSuspicious();
        }

        public override void OnDestroy()
        {
            removeSoundPlayedEvent();
        }

        private void removeSoundPlayedEvent()
        {
            if (!soundPlayedEventAdded)
            {
                return;
            }

            Singleton<BotEventHandler>.Instance.OnSoundPlayed -= enemySoundHeard;
            soundPlayedEventAdded = false;
        }

        public bool TrySetIgnoreHearing(float duration, bool value, bool ignoreUnderFire)
        {
            bool hearingIgnored = hearingFunction.TryIgnoreHearing(value, ignoreUnderFire, duration);
            if (hearingIgnored && value)
            {
                nextTimeSuspicionAllowed = Time.time + duration;
            }
            else
            {
                nextTimeSuspicionAllowed = 0;
            }

            return hearingIgnored;
        }

        private bool isSuspicious()
        {
            bool wasSuspiciousTooLong = totalSuspiciousTimer.ElapsedMilliseconds / 1000 > maxSuspiciousTime;

            if (!wasSuspiciousTooLong && shouldBeSuspicious(suspiciousTime))
            {
                if (!BotHiveMindMonitor.GetValueForBot(BotHiveMindSensorType.IsSuspicious, BotOwner))
                {
                    suspiciousTime = updateSuspiciousTime();
                    BotMonitor.GetMonitor<BotLootingMonitor>().TryPreventBotFromLooting((float)suspiciousTime);
                }

                totalSuspiciousTimer.Start();
                notSuspiciousTimer.Reset();

                BotMonitor.GetMonitor<BotHealthMonitor>().PauseHealthMonitoring();

                BotHiveMindMonitor.UpdateValueForBot(BotHiveMindSensorType.IsSuspicious, BotOwner, true);
                return true;
            }

            if (notSuspiciousTimer.ElapsedMilliseconds / 1000 > Singleton<ConfigUtil>.Instance.CurrentConfig.Questing.BotQuestingRequirements.HearingSensor.SuspicionCooldownTime)
            {
                totalSuspiciousTimer.Reset();
            }
            else
            {
                totalSuspiciousTimer.Stop();
            }

            notSuspiciousTimer.Start();

            BotMonitor.GetMonitor<BotHealthMonitor>().ResumeHealthMonitoring();

            BotHiveMindMonitor.UpdateValueForBot(BotHiveMindSensorType.IsSuspicious, BotOwner, false);
            return false;
        }

        private bool shouldBeSuspicious(double maxTimeSinceDangerSensed)
        {
            return (Time.time - lastEnemySoundHeardTime) < maxTimeSinceDangerSensed;
        }

        private int updateSuspiciousTime()
        {
            HearingSensorConfig hearingConfig = Singleton<ConfigUtil>.Instance.CurrentConfig.Questing.BotQuestingRequirements.HearingSensor;
            MinMaxConfig range = hearingConfig.SuspiciousTime;
            if (IsPvpHunter && hearingConfig.HunterSuspiciousTime != null)
            {
                range = hearingConfig.HunterSuspiciousTime;
            }

            int min = (int)range.Min;
            int max = Math.Max(min + 1, (int)range.Max);
            return random.Next(min, max);
        }

        private void updateMaxSuspiciousTime()
        {
            string locationId = Singleton<GameWorld>.Instance.GetComponent<Components.LocationData>().CurrentLocation.Id;
            HearingSensorConfig hearingConfig = Singleton<ConfigUtil>.Instance.CurrentConfig.Questing.BotQuestingRequirements.HearingSensor;

            if (hearingConfig.MaxSuspiciousTime.ContainsKey(locationId))
            {
                maxSuspiciousTime = hearingConfig.MaxSuspiciousTime[locationId];
            }
            else if (hearingConfig.MaxSuspiciousTime.ContainsKey("default"))
            {
                maxSuspiciousTime = hearingConfig.MaxSuspiciousTime["default"];
            }
            else
            {
                Singleton<LoggingUtil>.Instance.LogError("Could not set max suspicious time for " + BotOwner.GetText() + ". Defaulting to 60s.");
            }

            if (IsPvpHunter)
            {
                maxSuspiciousTime *= 1.5f;
            }
        }

        private void enemySoundHeard(IPlayer iplayer, Vector3 position, float power, AISoundType type)
        {
            if ((iplayer == null) || !iplayer.HealthController.IsAlive)
            {
                return;
            }

            if (iplayer.ProfileId == BotOwner.ProfileId)
            {
                return;
            }

            bool isGunfire = type == AISoundType.gun || type == AISoundType.silencedGun;
            bool isKnownEnemy = BotOwner.EnemiesController.EnemyInfos.Any(e => e.Key.ProfileId == iplayer.ProfileId);

            // Hunters chase distant gunfights even before the shooter is marked as an enemy.
            // Everyone else only reacts to already-known enemies (original behavior).
            if (!isKnownEnemy)
            {
                if (!(IsPvpHunter && isGunfire) || isFriendly(iplayer))
                {
                    return;
                }
            }

            float adjustedPower = power * BotOwner.HearingMultiplier();
            adjustedPower *= (type == AISoundType.step)
                ? Singleton<ConfigUtil>.Instance.CurrentConfig.Questing.BotQuestingRequirements.HearingSensor.LoudnessMultiplierFootsteps
                : 1;
            if (adjustedPower < Singleton<ConfigUtil>.Instance.CurrentConfig.Questing.BotQuestingRequirements.HearingSensor.MinCorrectedSoundPower)
            {
                return;
            }

            float hearingRange = BotOwner.Settings.Current.CurrentHearingSense * adjustedPower;
            float dist = Vector3.Distance(BotOwner.Position, position);
            if (dist > hearingRange)
            {
                return;
            }

            if (shouldIgnoreSound(type, dist))
            {
                return;
            }

            lastEnemySoundHeardTime = Time.time;
        }

        private bool isFriendly(IPlayer iplayer)
        {
            if (BotOwner.BotsGroup == null)
            {
                return false;
            }

            foreach (BotOwner member in BotOwner.BotsGroup.GetAllMembers())
            {
                if (member != null && member.ProfileId == iplayer.ProfileId)
                {
                    return true;
                }
            }

            return false;
        }

        private bool shouldIgnoreSound(AISoundType soundType, float distance)
        {
            if (!SuspicionAllowedByTime)
            {
                return true;
            }

            HearingSensorConfig hearingConfig = Singleton<ConfigUtil>.Instance.CurrentConfig.Questing.BotQuestingRequirements.HearingSensor;

            switch (soundType)
            {
                case AISoundType.step:
                    if (distance < hearingConfig.MaxDistanceFootsteps)
                    {
                        return false;
                    }
                    break;
                case AISoundType.gun:
                    float gunMax = IsPvpHunter ? hearingConfig.MaxDistanceGunfireHunter : hearingConfig.MaxDistanceGunfire;
                    if (distance < gunMax)
                    {
                        return false;
                    }
                    break;
                case AISoundType.silencedGun:
                    float suppressedMax = IsPvpHunter
                        ? hearingConfig.MaxDistanceGunfireSuppressedHunter
                        : hearingConfig.MaxDistanceGunfireSuppressed;
                    if (distance < suppressedMax)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }
    }
}
