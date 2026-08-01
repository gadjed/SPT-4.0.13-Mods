using QuestingBots.Configuration;
using QuestingBots.Helpers;
using QuestingBots.Services.Internal;
using QuestingBots.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Utils;

namespace QuestingBots.Services.Spawning
{
    [Injectable(TypePriority = OnLoadOrder.PostLoad + QuestingBots_Server.LOAD_ORDER_OFFSET + 2)]
    public class ScavReinforcementService : AbstractService
    {
        private const string SCAV_POPULATION_GUID = "gadjed.scavpopulation";

        private readonly LoggingUtil _logger;
        private readonly ConfigUtil _config;
        private readonly LocationTable _locationTable;
        private readonly RandomUtil _randomUtil;
        private readonly IReadOnlyList<SptMod> _loadedMods;

        public ScavReinforcementService(
            LoggingUtil logger,
            ConfigUtil config,
            LocationTable locationTable,
            RandomUtil randomUtil,
            IReadOnlyList<SptMod> loadedMods) : base(logger, config)
        {
            _logger = logger;
            _config = config;
            _locationTable = locationTable;
            _randomUtil = randomUtil;
            _loadedMods = loadedMods;
        }

        protected override void OnLoadIfModIsEnabled()
        {
            ContinuousPopulationConfig continuous = _config.CurrentConfig.BotSpawns.ContinuousPopulation;
            ScavReinforcementsConfig reinforcements = continuous.ScavReinforcements;

            if (!continuous.Enabled || !reinforcements.Enabled)
            {
                return;
            }

            if (!_config.CurrentConfig.BotSpawns.Enabled)
            {
                _logger.Info("Scav reinforcements skipped because bot_spawns is disabled.");
                return;
            }

            if (_loadedMods.Any(mod => mod.ModMetadata.ModGuid == SCAV_POPULATION_GUID))
            {
                _logger.Warning("Scav Population mod detected. Skipping Questing Bots scav reinforcements to avoid double-spawning. Uninstall Scav Population.");
                return;
            }

            Sanitize(reinforcements);

            HashSet<string> skipMaps = new HashSet<string>(reinforcements.SkipMaps ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            int mapsTouched = 0;
            int scavWavesAdded = 0;

            foreach (Location location in _locationTable.EnumerateLocations())
            {
                LocationBase? locationBase = location.Base;
                if (locationBase == null)
                {
                    continue;
                }

                string id = locationBase.Id ?? locationBase.Name ?? "unknown";
                if (skipMaps.Contains(id) || skipMaps.Contains(locationBase.Name ?? string.Empty))
                {
                    continue;
                }

                if (locationBase.EscapeTimeLimit is null or <= 0)
                {
                    continue;
                }

                int raidSeconds = (int)Math.Round(locationBase.EscapeTimeLimit.Value * 60);
                if (raidSeconds <= reinforcements.StartAfterSeconds)
                {
                    continue;
                }

                if (reinforcements.ExtendBotStop)
                {
                    int desiredBotStop = Math.Max(0, raidSeconds - 60);
                    if ((locationBase.BotStop ?? 0) < desiredBotStop)
                    {
                        locationBase.BotStop = desiredBotStop;
                    }
                }

                List<string> zones = GetZones(locationBase);
                List<int> times = BuildPulseTimes(reinforcements.StartAfterSeconds, reinforcements.IntervalSeconds, raidSeconds);
                if (times.Count == 0)
                {
                    continue;
                }

                int nextWaveNumber = (locationBase.Waves?.Count ?? 0) + 1000;
                int scavAdded = 0;
                foreach (int time in times)
                {
                    scavAdded += AddScavReinforcements(locationBase, zones, time, reinforcements, ref nextWaveNumber);
                }

                if (scavAdded > 0)
                {
                    mapsTouched++;
                    scavWavesAdded += scavAdded;
                    _logger.Info($"{id}: +{scavAdded} scav reinforcement wave(s) across {times.Count} pulse(s) (raid {raidSeconds}s).");
                }
            }

            _logger.Info($"Scav reinforcements ready on {mapsTouched} map(s): {scavWavesAdded} wave(s) added.");
        }

        private static void Sanitize(ScavReinforcementsConfig config)
        {
            config.StartAfterSeconds = Math.Max(60, config.StartAfterSeconds);
            config.IntervalSeconds = Math.Max(60, config.IntervalSeconds);
            config.SlotsMin = Math.Max(1, config.SlotsMin);
            config.SlotsMax = Math.Max(config.SlotsMin, config.SlotsMax);
            config.WavesPerInterval = Math.Max(1, config.WavesPerInterval);

            if (string.IsNullOrWhiteSpace(config.Difficulty))
            {
                config.Difficulty = "normal";
            }
        }

        private static List<int> BuildPulseTimes(int startAfter, int interval, int raidSeconds)
        {
            List<int> times = new List<int>();
            int lastSafe = raidSeconds - 90;
            for (int t = startAfter; t <= lastSafe; t += interval)
            {
                times.Add(t);
            }

            return times;
        }

        private static List<string> GetZones(LocationBase location)
        {
            List<string> zones = new List<string>();

            if (!string.IsNullOrWhiteSpace(location.OpenZones))
            {
                zones.AddRange(
                    location.OpenZones
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Where(z => !string.IsNullOrWhiteSpace(z))
                );
            }

            if (zones.Count == 0 && location.Waves is not null)
            {
                zones.AddRange(
                    location.Waves
                        .Select(w => w.SpawnPoints)
                        .Where(z => !string.IsNullOrWhiteSpace(z))!
                        .Cast<string>()
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                );
            }

            if (zones.Count == 0)
            {
                zones.Add("BotZone");
            }

            return zones;
        }

        private int AddScavReinforcements(
            LocationBase location,
            List<string> zones,
            int time,
            ScavReinforcementsConfig config,
            ref int waveNumber)
        {
            location.Waves ??= [];
            int added = 0;

            for (int i = 0; i < config.WavesPerInterval; i++)
            {
                string zone = zones[_randomUtil.GetInt(0, zones.Count - 1)];
                int slots = _randomUtil.GetInt(config.SlotsMin, config.SlotsMax);

                location.Waves.Add(
                    new Wave
                    {
                        BotPreset = config.Difficulty,
                        BotSide = "Savage",
                        KeepZoneOnSpawn = false,
                        SpawnPoints = zone,
                        WildSpawnType = WildSpawnType.assault,
                        IsPlayers = false,
                        Number = waveNumber++,
                        SlotsMin = Math.Max(0, slots - 1),
                        SlotsMax = slots,
                        TimeMin = time,
                        TimeMax = time + 90,
                        ChanceGroup = 100,
                        SpawnMode = new HashSet<string> { "regular", "pve" },
                        OpenZones = zone,
                        SptId = $"questingbots-scav-{location.Id}-{time}-{i}"
                    }
                );
                added++;
            }

            return added;
        }
    }
}
