using QuestingBots.Helpers;
using QuestingBots.Patches.PScavGeneration;
using QuestingBots.Services.Internal;
using QuestingBots.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Generators.Bot;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Services.Bot;

namespace QuestingBots.Services.Spawning
{
    [Injectable(TypePriority = OnLoadOrder.PostLoad + QuestingBots_Server.LOAD_ORDER_OFFSET + 1)]
    public class InitSpawningSystemService : AbstractService
    {
        private LoggingUtil _logger;
        private ConfigUtil _config;
        private PmcConfig _pmcConfig;
        private BotConfig _botConfig;
        private LocationConfig _locationConfig;
        private LocationTable _locationTable;
        private PatchManager _patchManager;
        private BotNameService _botNameService;
        private BotGenerator _botGenerator;

        public InitSpawningSystemService(
            LoggingUtil logger,
            ConfigUtil config,
            PmcConfig pmcConfig,
            BotConfig botConfig,
            LocationConfig locationConfig,
            LocationTable locationTable,
            PatchManager patchManager,
            BotNameService botNameService,
            BotGenerator botGenerator) : base(logger, config)
        {
            _logger = logger;
            _config = config;
            _pmcConfig = pmcConfig;
            _botConfig = botConfig;
            _locationConfig = locationConfig;
            _locationTable = locationTable;
            _patchManager = patchManager;
            _botNameService = botNameService;
            _botGenerator = botGenerator;
        }

        protected override void OnLoadIfModIsEnabled()
        {
            EnablePlayerScavGeneration();

            if (!_config.CurrentConfig.BotSpawns.Enabled)
            {
                return;
            }

            DisableRogueSpawningDelayOnLighthouse();
            RemovePvEPMCWaves();
            RemoveCustomBotWaves(_locationConfig.CustomWaves?.Boss, "PMC Boss");
            RemoveCustomBotWaves(_locationConfig.CustomWaves?.Normal, "Scav");
            RemoveCustomBotWaves(_pmcConfig.CustomPmcWaves, "PMC");
            UseEFTBotCaps();
        }

        private void EnablePlayerScavGeneration()
        {
            if (!ShouldDisablePlayerScavConversionChance())
            {
                return;
            }

            _logger.Info("Player scav spawning will be managed by the Questing Bots spawning system");

            _botConfig.ChanceAssaultScavHasPlayerScavName = 0;

            _patchManager.PatcherName = ModInfo.MODNAME;
            _patchManager.AddPatch(new GenerateBotWavePatch(_logger, _botNameService, _botGenerator));
            _patchManager.EnablePatches();
        }

        private bool ShouldDisablePlayerScavConversionChance()
        {
            if (_config.CurrentConfig.AdjustPScavChance.Enabled)
            {
                return true;
            }

            if (_config.CurrentConfig.BotSpawns.Enabled && _config.CurrentConfig.BotSpawns.PScavs.Enabled)
            {
                return true;
            }

            return false;
        }

        private void DisableRogueSpawningDelayOnLighthouse()
        {
            if (!_config.CurrentConfig.BotSpawns.LimitInitialBossSpawns.DisableRogueDelay)
            {
                return;
            }

            if (_locationConfig.RogueLighthouseSpawnTimeSettings.WaitTimeSeconds > -1)
            {
                _locationConfig.RogueLighthouseSpawnTimeSettings.WaitTimeSeconds = -1;
                _logger.Info("Removed SPT Rogue spawn delay on Lighthouse");
            }
        }

        private void RemovePvEPMCWaves()
        {
            int removedWaves = 0;
            foreach(Location location in _locationTable.GetDictionary().Values)
            {
                removedWaves += RemovePvEPMCWavesFromLocation(location);
            }

            if (removedWaves > 0)
            {
                _logger.Info($"Removed {removedWaves} PvE PMC waves");
            }
        }

        private int RemovePvEPMCWavesFromLocation(Location location)
        {
            if (location?.Base?.BossLocationSpawn == null)
            {
                return 0;
            }

            List<BossLocationSpawn> bossSpawnsToRemove = new List<BossLocationSpawn>();
            foreach (BossLocationSpawn bossLocationSpawn in location.Base.BossLocationSpawn)
            {
                if (DatabaseHelpers.PMCRoleNames.Contains(bossLocationSpawn.BossName))
                {
                    bossSpawnsToRemove.Add(bossLocationSpawn);
                }
            }

            return location.Base.BossLocationSpawn.RemoveAll(bossSpawnsToRemove.Contains);
        }

        private void RemoveCustomBotWaves<T>(Dictionary<string, List<T>>? botWaves, string waveName)
        {
            if (botWaves == null)
            {
                return;
            }

            int removedWaves = 0;
            foreach (var key in botWaves.Keys)
            {
                removedWaves += botWaves[key].Count;
                botWaves[key].Clear();
            }

            if (removedWaves > 0)
            {
                _logger.Info($"Removed {removedWaves} custom {waveName} waves");
            }
        }

        private void UseEFTBotCaps()
        {
            foreach (Location location in _locationTable.GetDictionary().Values)
            {
                if (location?.Base == null)
                {
                    continue;
                }

                string locationId = location.Base.Id.ToLower();
                if (!_botConfig.MaxBotCap.ContainsKey(locationId))
                {
                    continue;
                }

                int originalSPTBotCap = _botConfig.MaxBotCap[locationId];
                int eftBotCap = location.Base.BotMax;

                bool shouldAdjustBotCap = !_config.CurrentConfig.BotSpawns.BotCapAdjustments.OnlyDecreaseBotCaps;
                shouldAdjustBotCap |= originalSPTBotCap > eftBotCap;

                if (shouldAdjustBotCap && _config.CurrentConfig.BotSpawns.BotCapAdjustments.UseEFTBotCaps)
                {
                    _botConfig.MaxBotCap[locationId] = location.Base.BotMax;
                }

                int fixedAdjustment = _config.CurrentConfig.BotSpawns.BotCapAdjustments.MapSpecificAdjustments[locationId];
                _botConfig.MaxBotCap[locationId] += fixedAdjustment;

                int newBotCap = _botConfig.MaxBotCap[locationId];
                if (newBotCap != originalSPTBotCap)
                {
                    _logger.Info($"Updated bot cap for {locationId} to {newBotCap} (Original SPT: {originalSPTBotCap}, EFT: {eftBotCap}, Fixed Adjustment: {fixedAdjustment})");
                }
            }
        }
    }
}
