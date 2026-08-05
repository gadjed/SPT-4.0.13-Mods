using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using PackItems.Patches;
using SPT.Reflection.Patching;

namespace PackItems;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class PackItemsPlugin : BaseUnityPlugin
{
    public const string PluginGuid = "gadjed.packitems";
    public const string PluginName = "Pack Items";
    public const string PluginVersion = "1.0.0";

    internal static ManualLogSource Log { get; private set; } = null!;

    public static ConfigEntry<bool> Enabled { get; private set; } = null!;
    public static ConfigEntry<string> MenuLabel { get; private set; } = null!;
    public static ConfigEntry<bool> Debug { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;

        Enabled = Config.Bind(
            "1. General",
            "Enabled",
            true,
            "Show the Pack Items context-menu action on stash containers."
        );
        MenuLabel = Config.Bind(
            "1. General",
            "MenuLabel",
            "Скласти предмети",
            "Context menu caption."
        );
        Debug = Config.Bind(
            "1. General",
            "Debug",
            false,
            "Verbose packing logs."
        );

        EnablePatch(new PackItemsContextMenuPatch());

        Log.LogInfo($"{PluginName} v{PluginVersion} loaded (SPT 4.0.13).");
    }

    private static void EnablePatch(ModulePatch patch)
    {
        try
        {
            patch.Enable();
        }
        catch (Exception ex)
        {
            Log.LogError($"[PackItems] Failed to enable {patch.GetType().Name}: {ex}");
        }
    }
}
