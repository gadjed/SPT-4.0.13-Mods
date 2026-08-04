using System.Runtime.Serialization;

namespace QuestingBots.Configuration
{
    [DataContract]
    public class ContinuousPopulationConfig
    {
        [DataMember(Name = "enabled", IsRequired = true)]
        public bool Enabled { get; set; } = true;

        [DataMember(Name = "pmc_topup_enabled", IsRequired = true)]
        public bool PmcTopUpEnabled { get; set; } = true;

        [DataMember(Name = "pmc_topup_interval_seconds", IsRequired = true)]
        public float PmcTopUpIntervalSeconds { get; set; } = 45;

        [DataMember(Name = "pmc_topup_start_after_seconds", IsRequired = true)]
        public float PmcTopUpStartAfterSeconds { get; set; } = 180;

        [DataMember(Name = "scav_reinforcements", IsRequired = true)]
        public ScavReinforcementsConfig ScavReinforcements { get; set; } = new ScavReinforcementsConfig();

        public ContinuousPopulationConfig()
        {
        }
    }
}
