using System;
using System.Runtime.Serialization;

namespace QuestingBots.Configuration
{
    [DataContract]
    public class ScavReinforcementsConfig
    {
        [DataMember(Name = "enabled", IsRequired = true)]
        public bool Enabled { get; set; } = true;

        [DataMember(Name = "start_after_seconds", IsRequired = true)]
        public int StartAfterSeconds { get; set; } = 90;

        [DataMember(Name = "interval_seconds", IsRequired = true)]
        public int IntervalSeconds { get; set; } = 120;

        [DataMember(Name = "slots_min", IsRequired = true)]
        public int SlotsMin { get; set; } = 3;

        [DataMember(Name = "slots_max", IsRequired = true)]
        public int SlotsMax { get; set; } = 6;

        [DataMember(Name = "waves_per_interval", IsRequired = true)]
        public int WavesPerInterval { get; set; } = 3;

        [DataMember(Name = "difficulty", IsRequired = true)]
        public string Difficulty { get; set; } = "normal";

        [DataMember(Name = "extend_bot_stop", IsRequired = true)]
        public bool ExtendBotStop { get; set; } = true;

        [DataMember(Name = "skip_maps", IsRequired = true)]
        public string[] SkipMaps { get; set; } =
        {
            "laboratory",
            "labyrinth",
            "hideout"
        };

        public ScavReinforcementsConfig()
        {
        }
    }
}
