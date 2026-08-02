using System.Collections.Generic;
using System.Runtime.Serialization;

namespace QuestingBots.Configuration
{
    [DataContract]
    public class HearingSensorConfig
    {
        [DataMember(Name = "enabled", IsRequired = true)]
        public bool Enabled { get; set; } = false;

        [DataMember(Name = "min_corrected_sound_power", IsRequired = true)]
        public float MinCorrectedSoundPower { get; set; } = 17;

        [DataMember(Name = "max_distance_footsteps", IsRequired = true)]
        public float MaxDistanceFootsteps { get; set; } = 20;

        [DataMember(Name = "max_distance_gunfire", IsRequired = true)]
        public float MaxDistanceGunfire { get; set; } = 90;

        [DataMember(Name = "max_distance_gunfire_suppressed", IsRequired = true)]
        public float MaxDistanceGunfireSuppressed { get; set; } = 60;

        /// <summary>
        /// Chance (0-100) that a bot becomes a "PVP hunter" and will abandon quests to investigate distant gunfire.
        /// </summary>
        [DataMember(Name = "pvp_hunter_chance")]
        public float PvpHunterChance { get; set; } = 40;

        [DataMember(Name = "max_distance_gunfire_hunter")]
        public float MaxDistanceGunfireHunter { get; set; } = 220;

        [DataMember(Name = "max_distance_gunfire_suppressed_hunter")]
        public float MaxDistanceGunfireSuppressedHunter { get; set; } = 110;

        [DataMember(Name = "loudness_multiplier_footsteps", IsRequired = true)]
        public float LoudnessMultiplierFootsteps { get; set; } = 1f;

        [DataMember(Name = "loudness_multiplier_headset", IsRequired = true)]
        public float LoudnessMultiplierHeadset { get; set; } = 1.3f;

        [DataMember(Name = "loudness_multiplier_helmet_low_deaf", IsRequired = true)]
        public float LoudnessMultiplierHelmetLowDeaf { get; set; } = 0.8f;

        [DataMember(Name = "loudness_multiplier_helmet_high_deaf", IsRequired = true)]
        public float LoudnessMultiplierHelmetHighDeaf { get; set; } = 0.6f;

        [DataMember(Name = "suspicious_time", IsRequired = true)]
        public MinMaxConfig SuspiciousTime { get; set; } = new MinMaxConfig();

        [DataMember(Name = "hunter_suspicious_time")]
        public MinMaxConfig HunterSuspiciousTime { get; set; } = new MinMaxConfig(20, 50);

        [DataMember(Name = "max_suspicious_time", IsRequired = true)]
        public Dictionary<string, int> MaxSuspiciousTime { get; set; } = new Dictionary<string, int>();

        [DataMember(Name = "suspicion_cooldown_time", IsRequired = true)]
        public float SuspicionCooldownTime { get; set; } = 30;

        public HearingSensorConfig()
        {
        }
    }
}
