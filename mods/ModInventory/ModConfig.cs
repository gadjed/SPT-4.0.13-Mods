namespace ModInventory;

public sealed class ModConfig
{
    public List<string> ScanRoots { get; set; } =
    [
        "BepInEx/plugins",
        "BepInEx/patchers",
        "SPT/user/mods",
        "user/mods",
    ];

    public List<string> ExcludeModFolders { get; set; } = ["ModInventory"];
}
