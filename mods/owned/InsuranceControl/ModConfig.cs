namespace InsuranceControl;

public class ModConfig
{
    /// <summary>
    /// If &gt; 0, forces insurance return after this many seconds (debug).
    /// Overrides <see cref="ReturnTimeOverrideSeconds"/> and shortens the poll interval.
    /// Set to 0 to disable. Example: 60 = return in ~1 minute.
    /// </summary>
    public double DebugReturnSeconds { get; set; } = 0;

    /// <summary>
    /// If &gt; 0, overrides trader min/max return hours with this fixed delay (seconds).
    /// Set to 0 to use <see cref="TraderReturnHours"/> instead.
    /// Ignored when <see cref="DebugReturnSeconds"/> is &gt; 0.
    /// </summary>
    public double ReturnTimeOverrideSeconds { get; set; } = 3600;

    /// <summary>
    /// How often the server checks for ready insurance returns (seconds).
    /// Lower values make short return times feel more accurate. Vanilla is 600.
    /// </summary>
    public double RunIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// If &gt; 0, overrides how long returned insurance stays in mail (seconds).
    /// Set to 0 to keep trader defaults.
    /// </summary>
    public double StorageTimeOverrideSeconds { get; set; } = 0;

    /// <summary>
    /// Return magazines with cartridges still loaded.
    /// </summary>
    public bool ReturnMagazinesWithAmmo { get; set; } = true;

    /// <summary>
    /// Return backpacks and chest rigs together with their grid contents.
    /// </summary>
    public bool ReturnContainersWithContents { get; set; } = true;

    /// <summary>
    /// When true, SPT may still remove some items / attachments as if scavengers looted them.
    /// </summary>
    public bool SimulateItemsBeingTaken { get; set; } = true;

    /// <summary>
    /// Chance (0-100) that an insured item is permanently lost, per trader.
    /// LostChance 0 = always returned; 15 ≈ vanilla Prapor.
    /// </summary>
    public Dictionary<string, double> LostChancePercent { get; set; } = new()
    {
        ["Prapor"] = 0,
        ["Therapist"] = 0,
    };

    /// <summary>
    /// Used only when <see cref="ReturnTimeOverrideSeconds"/> is 0 (and debug is off).
    /// </summary>
    public Dictionary<string, TraderReturnHours> TraderReturnHours { get; set; } = new()
    {
        ["Prapor"] = new TraderReturnHours { Min = 1, Max = 2 },
        ["Therapist"] = new TraderReturnHours { Min = 1, Max = 1 },
    };
}

public class TraderReturnHours
{
    public double Min { get; set; } = 1;
    public double Max { get; set; } = 2;
}
