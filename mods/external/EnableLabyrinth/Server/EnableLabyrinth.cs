using System.Reflection;
using System.Runtime.CompilerServices;
using _enableLabyrinth.Globals;
using _enableLabyrinth.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Cloners;
using SPTarkov.Server.Web;

namespace _enableLabyrinth;

public record ModMetadata : AbstractModMetadata, IModWebMetadata
{
    public override string ModGuid { get; init; } = "com.acidphantasm.enablelabyrinth";
    public override string Name { get; init; } = "Enable Labyrinth";
    public override string Author { get; init; } = "acidphantasm";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.2");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.3");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 90000)]
public class EnableLabyrinth(
    DatabaseService databaseService,
    ConfigServer configServer,
    ICloner cloner,
    ISptLogger<EnableLabyrinth> logger)
    : IOnLoad
{
    private readonly LocationConfig _locationConfig = configServer.GetConfig<LocationConfig>();
    private Location _labyrinthData = null!;
    private LocationBase _labyrinthBase = null!;
    private Dictionary<string, BotType?> _databaseBots = null!;
    
    public Task OnLoad()
    { 
        _labyrinthData = databaseService.GetLocations().Labyrinth;
        _labyrinthBase = databaseService.GetLocations().Labyrinth.Base;
        _databaseBots = databaseService.GetBots().Types;
        
        AdjustLabyrinthBase();
        AdjustLabyrinthScavRaidTimeSettings();
        AdjustExfils();
        AddSecretKeyGuarantee();

        return Task.CompletedTask;
    }
    
    private void AdjustLabyrinthBase()
    {
        if (ModConfig.Config.RemoveKeycardRequirement)
        {
            _labyrinthBase.AccessKeys = [];
            _labyrinthBase.AccessKeysPvE = [];
        }
        else
        {
            _labyrinthBase.AccessKeys = [ ItemTpl.KEYCARD_LABRYS_ACCESS ];
            _labyrinthBase.AccessKeysPvE = [ ItemTpl.KEYCARD_LABRYS_ACCESS ];
        }
        _labyrinthBase.IconY = 350f;
        _labyrinthBase.Enabled = true;
        _labyrinthBase.DisabledForScav = !ModConfig.Config.AllowScavEntryToLabyrinthFromMap;
        _labyrinthBase.ForceOnlineRaidInPVE = false;
    }

    private void AdjustLabyrinthScavRaidTimeSettings()
    {
        _locationConfig.ScavRaidTimeSettings.Maps["labyrinth"] = cloner.Clone(_locationConfig.ScavRaidTimeSettings.Maps["factory4_day"]);
    }

    private void AdjustExfils()
    {
        // Add Scav Exfil to AllExtracts
        AddScavExfilToAllExtracts();
        AddScavExfilToBaseExtracts();
        
        var extractList = _labyrinthBase.Exits.ToList();
        var pmcExit = extractList.FirstOrDefault(e => e.Name == "labir_exit");
        if (pmcExit != null)
        {
            if (ModConfig.Config.ChangePmcExfilTimers)
            {
                var exfiltrationTypeValueFromConfig = ModConfig.Config.PrimaryPmcExfilTimer.ExfiltrationType;

                if (!Enum.TryParse<ExfiltrationType>(exfiltrationTypeValueFromConfig, ignoreCase: true, out var parsedExfiltrationType))
                {
                    logger.Warning($"Invalid ExfiltrationType Config Value: '{exfiltrationTypeValueFromConfig}', defaulting to Individual.");
                    parsedExfiltrationType = ExfiltrationType.Individual;
                }
            
                pmcExit.ExfiltrationTime = ModConfig.Config.PrimaryPmcExfilTimer.ExfiltrationTime;
                pmcExit.ExfiltrationTimePVE = ModConfig.Config.PrimaryPmcExfilTimer.ExfiltrationTime;
                pmcExit.ExfiltrationType = parsedExfiltrationType;
                
                // Timer requirements do Random.Range(MinTime (inclusive), MaxTime (exclusive)) on the client side
                pmcExit.MinTime = ModConfig.Config.PrimaryPmcExfilTimer.ElapsedSecondsBeforeAvailable;
                pmcExit.MinTimePVE = ModConfig.Config.PrimaryPmcExfilTimer.ElapsedSecondsBeforeAvailable;
                pmcExit.MaxTime = ModConfig.Config.PrimaryPmcExfilTimer.ElapsedSecondsBeforeAvailable;
                pmcExit.MaxTimePVE = ModConfig.Config.PrimaryPmcExfilTimer.ElapsedSecondsBeforeAvailable;
            }
            else
            {
                pmcExit.ExfiltrationTime = 5;
                pmcExit.ExfiltrationTimePVE = 5;
                pmcExit.ExfiltrationType = ExfiltrationType.SharedTimer;
                
                // Timer requirements do Random.Range(MinTime (inclusive), MaxTime (exclusive)) on the client side
                pmcExit.MinTime = 900;
                pmcExit.MinTimePVE = 900;
                pmcExit.MaxTime = 905;
                pmcExit.MaxTimePVE = 905;
            }
        }
            
        // Reassign original Exfils
        _labyrinthData.Base.Exits = extractList;

    }

    private void AddScavExfilToAllExtracts()
    {
        var allExtractList = _labyrinthData.AllExtracts.ToList();
        
        allExtractList.Add(new AllExtractsExit
        {
            Chance = 100,
            ChancePVE = 100,
            Count = 0,
            CountPVE = 0,
            EntryPoints = "",
            EventAvailable = false,
            ExfiltrationTime = 30,
            ExfiltrationTimePVE = 30,
            ExfiltrationType = ExfiltrationType.Individual,
            Id = "",
            MaxTime = 0,
            MaxTimePVE = 0,
            MinTime = 0,
            MinTimePVE = 0,
            Name = "The Way Up (scav)",
            PassageRequirement = RequirementState.None,
            PlayersCount = 0,
            PlayersCountPVE = 0,
            RequiredSlot = EquipmentSlots.FirstPrimaryWeapon,
            RequirementTip = "",
            Side = "Scav"
        });
        
        _labyrinthData.AllExtracts = allExtractList;
    }

    private void AddScavExfilToBaseExtracts()
    {
        var labyrinthBaseExtracts = _labyrinthBase.Exits.ToList();
        
        labyrinthBaseExtracts.Add(new Exit
        {
            Chance = 100,
            ChancePVE = 100,
            Count = 0,
            CountPVE = 0,
            EntryPoints = "",
            EventAvailable = false,
            ExfiltrationTime = 30,
            ExfiltrationTimePVE = 30,
            ExfiltrationType = ExfiltrationType.Individual,
            Id = "",
            MaxTime = 0,
            MaxTimePVE = 0,
            MinTime = 0,
            MinTimePVE = 0,
            Name = "The Way Up (scav)",
            PassageRequirement = RequirementState.None,
            PlayersCount = 0,
            PlayersCountPVE = 0,
            RequirementTip = ""
        });
        
        _labyrinthBase.Exits = labyrinthBaseExtracts;
    }

    private void AddSecretKeyGuarantee()
    {
        if (!_databaseBots.TryGetValue("bosstagillaagro", out var shadowOfTagilla)) return;

        if (shadowOfTagilla is null) return;

        if (ModConfig.Config.GuaranteeSecretExfilKey)
        {
            shadowOfTagilla.BotInventory.Items.SpecialLoot[ItemTpl.KEY_ARIADNE_SYMBOL] = 1;
            shadowOfTagilla.BotGeneration.Items.SpecialItems.Weights = new Dictionary<double, double>
            {
                { 0, 0 },
                { 1, 1 }
            };
        }
        else
        {
            shadowOfTagilla.BotInventory.Items.SpecialLoot.Remove(ItemTpl.KEY_ARIADNE_SYMBOL);
            shadowOfTagilla.BotGeneration.Items.SpecialItems.Weights = new Dictionary<double, double>
            {
                { 0, 1 },
                { 1, 0 }
            };
        }
    }
}