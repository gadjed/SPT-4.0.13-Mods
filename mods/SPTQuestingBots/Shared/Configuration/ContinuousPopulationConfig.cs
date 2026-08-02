using System.Runtime.Serialization;

namespace QuestingBots.Configuration
{
    [DataContract]
    public class ContinuousPopulationConfig
    {
        [DataMember(Name = "enabled", IsRequired = true)]
        public bool Enabled { get; set; } = true;

        [DataMember(Name = "scav_reinforcements", IsRequired = true)]
        public ScavReinforcementsConfig ScavReinforcements { get; set; } = new ScavReinforcementsConfig();

        public ContinuousPopulationConfig()
        {
        }
    }
}
