using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuestingBots.Configuration
{
    [DataContract]
    public class BreakForLootingConfig
    {
        [DataMember(Name = "enabled", IsRequired = true)]
        public bool Enabled { get; set; } = true;

        [DataMember(Name = "min_time_between_looting_checks", IsRequired = true)]
        public float MinTimeBetweenLootingChecks { get; set; } = 20;

        [DataMember(Name = "min_time_between_follower_looting_checks", IsRequired = true)]
        public float MinTimeBetweenFollowerLootingChecks { get; set; } = 15;

        [DataMember(Name = "min_time_between_looting_events", IsRequired = true)]
        public float MinTimeBetweenLootingEvents { get; set; } = 35;

        [DataMember(Name = "max_time_to_start_looting", IsRequired = true)]
        public float MaxTimeToStartLooting { get; set; } = 3;

        [DataMember(Name = "max_loot_scan_time", IsRequired = true)]
        public float MaxLootScanTime { get; set; } = 40;

        [DataMember(Name = "max_distance_from_boss", IsRequired = true)]
        public float MaxDistanceFromBoss { get; set; } = 80;

        public BreakForLootingConfig()
        {

        }
    }
}
