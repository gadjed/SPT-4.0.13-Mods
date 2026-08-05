using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DefibAllyRevive.Patches;
using SPT.Reflection.Patching;

namespace DefibAllyRevive;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class DefibAllyRevivePlugin : BaseUnityPlugin
{
    public const string PluginGuid = "gadjed.defiballyrevive";
    public const string PluginName = "Defib Ally Revive";
    public const string PluginVersion = "1.0.0";

    /// <summary>Portable defibrillator (barter MedicalSupplies).</summary>
    public const string DefibrillatorTemplateId = "5c052e6986f7746b207bc3c9";

    public static DefibAllyRevivePlugin Instance { get; private set; } = null!;
    internal static ManualLogSource Log { get; private set; } = null!;

    public static ConfigEntry<bool> Enabled { get; private set; } = null!;
    public static ConfigEntry<float> ReviveRange { get; private set; } = null!;
    public static ConfigEntry<float> ReviveTime { get; private set; } = null!;
    public static ConfigEntry<float> BleedoutTime { get; private set; } = null!;
    public static ConfigEntry<bool> RequireSameGroup { get; private set; } = null!;
    public static ConfigEntry<bool> AllowSameSide { get; private set; } = null!;
    public static ConfigEntry<bool> ConsumeDefibrillator { get; private set; } = null!;
    public static ConfigEntry<bool> FullHealOnRevive { get; private set; } = null!;
    public static ConfigEntry<bool> Debug { get; private set; } = null!;

    private void Awake()
    {
        Instance = this;
        Log = Logger;

        Enabled = Config.Bind(
            "1. General",
            "Enabled",
            true,
            "Downed allies can be revived with a defibrillator bound to a quick slot."
        );
        ReviveRange = Config.Bind(
            "1. General",
            "ReviveRange",
            3.5f,
            new ConfigDescription(
                "Max distance (meters) to a downed ally when using the defibrillator hotkey.",
                new AcceptableValueRange<float>(1f, 15f)
            )
        );
        ReviveTime = Config.Bind(
            "1. General",
            "ReviveTime",
            5f,
            new ConfigDescription(
                "Channel time (seconds) to revive an ally.",
                new AcceptableValueRange<float>(1f, 30f)
            )
        );
        BleedoutTime = Config.Bind(
            "1. General",
            "BleedoutTime",
            90f,
            new ConfigDescription(
                "Seconds an ally stays downed before permanent death. 0 = until raid ends.",
                new AcceptableValueRange<float>(0f, 600f)
            )
        );
        RequireSameGroup = Config.Bind(
            "2. Allies",
            "RequireSameGroup",
            true,
            "Only GroupId teammates (e.g. Fika squad / shared group) count as allies."
        );
        AllowSameSide = Config.Bind(
            "2. Allies",
            "AllowSameSide",
            false,
            "Also treat same Side (USEC/BEAR) as allies when GroupId does not match. Ignored if RequireSameGroup is true and GroupIds differ."
        );
        ConsumeDefibrillator = Config.Bind(
            "3. Item",
            "ConsumeDefibrillator",
            true,
            "Destroy the defibrillator after a successful revive."
        );
        FullHealOnRevive = Config.Bind(
            "3. Item",
            "FullHealOnRevive",
            true,
            "Restore full health on revive. If false, only restores destroyed vital parts to a minimal amount."
        );
        Debug = Config.Bind(
            "4. Debug",
            "Debug",
            false,
            "Verbose logging."
        );

        EnablePatch(new ActiveHealthControllerKillPatch());
        EnablePatch(new SetQuickSlotItemPatch());
        EnablePatch(new IsAtBindablePlacePatch());
        EnablePatch(new PlayerUpdateTickPatch());

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
            Log.LogError($"[DefibAllyRevive] Failed to enable {patch.GetType().Name}: {ex}");
        }
    }

    internal static void DebugLog(string message)
    {
        if (Debug.Value)
        {
            Log.LogInfo($"[DefibAllyRevive] {message}");
        }
    }
}
