using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using InsureAllPrapor.Patches;

namespace InsureAllPrapor;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class InsureAllPraporPlugin : BaseUnityPlugin
{
    public const string PluginGuid = "gadjed.insureallprapor";
    public const string PluginName = "gadjed-InsureAllPrapor";
    public const string PluginVersion = "1.0.3";

    /// <summary>Prapor trader MongoID.</summary>
    public const string PraporTraderId = "54cb50c76803fa8b248b4571";

    internal static ManualLogSource Log { get; private set; } = null!;

    public static ConfigEntry<bool> Enabled { get; private set; } = null!;
    public static ConfigEntry<string> ButtonLabel { get; private set; } = null!;
    public static ConfigEntry<bool> Debug { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;

        Enabled = Config.Bind(
            "1. General",
            "Enabled",
            true,
            "Show the Insure All button on the stash equipment panel."
        );
        ButtonLabel = Config.Bind(
            "1. General",
            "ButtonLabel",
            "Застраховать все",
            "Label on the stash button."
        );
        Debug = Config.Bind(
            "1. General",
            "Debug",
            false,
            "Verbose logging of insure-all decisions."
        );

        new EquipmentTabShowPatch().Enable();
        new ContainersPanelShowPatch().Enable();
        new EquipmentTabHidePatch().Enable();

        Log.LogInfo($"{PluginName} v{PluginVersion} loaded (SPT 4.0.13).");
    }
}
