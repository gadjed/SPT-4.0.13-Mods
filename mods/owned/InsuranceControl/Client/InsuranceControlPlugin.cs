using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using InsuranceControl.Client.Patches;

namespace InsuranceControl.Client;

public enum InsuranceTrader
{
    Prapor,
    Therapist,
}

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInIncompatibility("gadjed.insureallprapor")]
public class InsuranceControlPlugin : BaseUnityPlugin
{
    // Matches server ModGuid / existing Forge listing (Insurance Control).
    public const string PluginGuid = "gadjed.insurancerefund";
    public const string PluginName = "gadjed-InsuranceControl";
    public const string PluginVersion = "1.1.0";

    public const string PraporTraderId = "54cb50c76803fa8b248b4571";
    public const string TherapistTraderId = "54cb57776803fa99248b456e";

    internal static ManualLogSource Log { get; private set; } = null!;

    // 1. Insure All
    public static ConfigEntry<bool> InsureAllEnabled { get; private set; } = null!;
    public static ConfigEntry<string> ButtonLabel { get; private set; } = null!;
    public static ConfigEntry<InsuranceTrader> Insurer { get; private set; } = null!;

    // 2. Button layout
    public static ConfigEntry<float> OffsetRight { get; private set; } = null!;
    public static ConfigEntry<float> GapAboveAnchor { get; private set; } = null!;
    public static ConfigEntry<float> ButtonWidth { get; private set; } = null!;
    public static ConfigEntry<float> ButtonHeight { get; private set; } = null!;
    public static ConfigEntry<int> FontSize { get; private set; } = null!;

    // 3. Debug
    public static ConfigEntry<bool> Debug { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;

        InsureAllEnabled = Config.Bind(
            "1. Insure All",
            "Enabled",
            true,
            "Show the Insure All button on the stash equipment panel."
        );
        ButtonLabel = Config.Bind(
            "1. Insure All",
            "Button Label",
            "Застраховать все",
            "Label on the stash button."
        );
        Insurer = Config.Bind(
            "1. Insure All",
            "Insurer",
            InsuranceTrader.Prapor,
            "Trader used when clicking Insure All (Prapor or Therapist)."
        );

        OffsetRight = Config.Bind(
            "2. Button Layout",
            "Offset Right",
            190f,
            new ConfigDescription(
                "Horizontal offset from the vest/armor slot center (pixels).",
                new AcceptableValueRange<float>(-400f, 400f)
            )
        );
        GapAboveAnchor = Config.Bind(
            "2. Button Layout",
            "Gap Above Anchor",
            12f,
            new ConfigDescription(
                "Vertical gap between the button and the vest/armor slot (pixels).",
                new AcceptableValueRange<float>(0f, 80f)
            )
        );
        ButtonWidth = Config.Bind(
            "2. Button Layout",
            "Button Width",
            140f,
            new ConfigDescription(
                "Button width in pixels.",
                new AcceptableValueRange<float>(80f, 320f)
            )
        );
        ButtonHeight = Config.Bind(
            "2. Button Layout",
            "Button Height",
            24f,
            new ConfigDescription(
                "Button height in pixels.",
                new AcceptableValueRange<float>(16f, 48f)
            )
        );
        FontSize = Config.Bind(
            "2. Button Layout",
            "Font Size",
            14,
            new ConfigDescription(
                "Button label font size.",
                new AcceptableValueRange<int>(10, 24)
            )
        );

        Debug = Config.Bind(
            "3. Debug",
            "Verbose Logging",
            false,
            "Extra logging of insure-all decisions in the BepInEx log."
        );

        OffsetRight.SettingChanged += (_, _) => InsureAllButtonController.RefreshLayout();
        GapAboveAnchor.SettingChanged += (_, _) => InsureAllButtonController.RefreshLayout();
        ButtonWidth.SettingChanged += (_, _) => InsureAllButtonController.RefreshLayout();
        ButtonHeight.SettingChanged += (_, _) => InsureAllButtonController.RefreshLayout();
        FontSize.SettingChanged += (_, _) => InsureAllButtonController.RefreshLayout();
        ButtonLabel.SettingChanged += (_, _) => InsureAllButtonController.RefreshLayout();
        InsureAllEnabled.SettingChanged += (_, _) => InsureAllButtonController.OnEnabledChanged();

        new EquipmentTabShowPatch().Enable();
        new ContainersPanelShowPatch().Enable();
        new EquipmentTabHidePatch().Enable();

        Log.LogInfo($"{PluginName} v{PluginVersion} loaded (SPT 4.0.13).");
    }

    public static string GetSelectedInsurerId()
    {
        return Insurer.Value == InsuranceTrader.Therapist ? TherapistTraderId : PraporTraderId;
    }

    public static string GetSelectedInsurerDisplayName()
    {
        return Insurer.Value == InsuranceTrader.Therapist ? "Терапевт" : "Прапор";
    }
}
