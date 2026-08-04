namespace _enableLabyrinth.Models;

public record ServerConfig
{
    public bool ChangePmcExfilTimers { get; set; }
    public required ExfilTimers PrimaryPmcExfilTimer { get; init; }
    public bool GuaranteeSecretExfilKey { get; set; }
    public bool RemoveKeycardRequirement { get; set; }
    public bool AllowScavEntryToLabyrinthFromMap { get; set; }
    public required ConfigAppSettings ConfigAppSettings { get; set; }
}

public record ExfilTimers
{
    public double ExfiltrationTime { get; set; }
    public required string ExfiltrationType { get; set; }
    public double ElapsedSecondsBeforeAvailable { get; set; }
}

public record ConfigAppSettings
{
    public bool ShowUndo { get; set; }
    public bool ShowDefault { get; set; }
    public bool DisableAnimations { get; set; }
    public bool AllowUpdateChecks { get; set; }
}