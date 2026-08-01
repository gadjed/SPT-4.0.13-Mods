using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using QuickSearch.Patches;

namespace QuickSearch;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class QuickSearchPlugin : BaseUnityPlugin
{
    public const string PluginGuid = "gadjed.quicksearch";
    public const string PluginName = "Quick Search";
    public const string PluginVersion = "1.1.0";

    /// <summary>
    /// How many times faster container / corpse search should be.
    /// 3 = three times faster (delays become 1/3 of vanilla).
    /// </summary>
    public static ConfigEntry<float> SearchSpeedMultiplier { get; private set; } = null!;

    internal static ManualLogSource Log { get; private set; } = null!;

    private Harmony? _harmony;

    private void Awake()
    {
        Log = Logger;

        SearchSpeedMultiplier = Config.Bind(
            "Search Speed",
            "SearchSpeedMultiplier",
            3f,
            new ConfigDescription(
                "How many times faster searching containers and corpses is. 1 = vanilla, 3 = three times faster, higher = faster.",
                new AcceptableValueRange<float>(1f, 100f)
            )
        );

        _harmony = new Harmony(PluginGuid);
        ContainerSearchPatch.Apply(_harmony);

        Log.LogInfo($"{PluginName} v{PluginVersion} loaded (x{SearchSpeedMultiplier.Value} search speed).");
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }
}
