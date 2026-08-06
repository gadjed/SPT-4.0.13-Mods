using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using MedSuite.Client.AutoMed.Patches;
using MedSuite.Client.Defib.Patches;
using MedSuite.Client.Healing.Patches;
using SPT.Reflection.Patching;

namespace MedSuite.Client;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInIncompatibility("com.lacyway.ch")]
[BepInIncompatibility("gadjed.automedhotkeys")]
[BepInIncompatibility("gadjed.defiballyrevive")]
[BepInIncompatibility("gadjed.medrebalance")]
public class MedSuitePlugin : BaseUnityPlugin
{
    public const string PluginGuid = "gadjed.medsuite";
    public const string PluginName = "Med Suite";
    public const string PluginVersion = "1.0.0";

    /// <summary>Portable defibrillator (barter MedicalSupplies).</summary>
    public const string DefibrillatorTemplateId = "5c052e6986f7746b207bc3c9";

    public static MedSuitePlugin Instance { get; private set; } = null!;
    internal static ManualLogSource Log { get; private set; } = null!;

    public static class AutoMed
    {
        public static ConfigEntry<bool> Enabled { get; internal set; } = null!;
        public static ConfigEntry<bool> OverwriteExisting { get; internal set; } = null!;
        public static ConfigEntry<bool> Debug { get; internal set; } = null!;
    }

    public static class Defib
    {
        public static ConfigEntry<bool> Enabled { get; internal set; } = null!;
        public static ConfigEntry<float> ReviveRange { get; internal set; } = null!;
        public static ConfigEntry<float> ReviveTime { get; internal set; } = null!;
        public static ConfigEntry<float> BleedoutTime { get; internal set; } = null!;
        public static ConfigEntry<bool> RequireSameGroup { get; internal set; } = null!;
        public static ConfigEntry<bool> AllowSameSide { get; internal set; } = null!;
        public static ConfigEntry<bool> ConsumeDefibrillator { get; internal set; } = null!;
        public static ConfigEntry<bool> FullHealOnRevive { get; internal set; } = null!;
        public static ConfigEntry<bool> Debug { get; internal set; } = null!;
    }

    public static class Healing
    {
        public static ConfigEntry<bool> ContinuousHealing { get; internal set; } = null!;
        public static ConfigEntry<bool> HealLimbs { get; internal set; } = null!;
        public static ConfigEntry<int> HealDelay { get; internal set; } = null!;
        public static ConfigEntry<bool> ResetAnimation { get; internal set; } = null!;
        public static ConfigEntry<bool> CancelOnDamage { get; internal set; } = null!;
        public static ConfigEntry<bool> ScratchHeal { get; internal set; } = null!;
        public static ConfigEntry<float> ScratchHealAmount { get; internal set; } = null!;
        public static ConfigEntry<float> ScratchMaxMissingHp { get; internal set; } = null!;
    }

    private void Awake()
    {
        Instance = this;
        Log = Logger;

        BindAutoMed();
        BindDefib();
        BindHealing();

        EnableAutoMedPatches();
        EnableDefibPatches();
        EnableHealingPatches();

        Log.LogInfo($"{PluginName} v{PluginVersion} loaded (SPT 4.0.13). Configure via F12.");
    }

    private void BindAutoMed()
    {
        AutoMed.Enabled = Config.Bind(
            "1. Auto Med Hotkeys",
            "Enabled",
            true,
            "Automatically bind medical items to quick slots 4 / 5 / 6."
        );
        AutoMed.OverwriteExisting = Config.Bind(
            "1. Auto Med Hotkeys",
            "OverwriteExisting",
            true,
            "If a different item is already on slot 4/5/6, replace it with the matching med."
        );
        AutoMed.Debug = Config.Bind(
            "1. Auto Med Hotkeys",
            "Debug",
            false,
            "Verbose logging of bind decisions."
        );
    }

    private void BindDefib()
    {
        Defib.Enabled = Config.Bind(
            "2. Defib Ally Revive",
            "Enabled",
            true,
            "Downed allies can be revived with a defibrillator bound to a quick slot."
        );
        Defib.ReviveRange = Config.Bind(
            "2. Defib Ally Revive",
            "ReviveRange",
            3.5f,
            new ConfigDescription(
                "Max distance (meters) to a downed ally when using the defibrillator hotkey.",
                new AcceptableValueRange<float>(1f, 15f)
            )
        );
        Defib.ReviveTime = Config.Bind(
            "2. Defib Ally Revive",
            "ReviveTime",
            5f,
            new ConfigDescription(
                "Channel time (seconds) to revive an ally.",
                new AcceptableValueRange<float>(1f, 30f)
            )
        );
        Defib.BleedoutTime = Config.Bind(
            "2. Defib Ally Revive",
            "BleedoutTime",
            90f,
            new ConfigDescription(
                "Seconds an ally stays downed before permanent death. 0 = until raid ends.",
                new AcceptableValueRange<float>(0f, 600f)
            )
        );
        Defib.RequireSameGroup = Config.Bind(
            "2. Defib Ally Revive",
            "RequireSameGroup",
            true,
            "Only GroupId teammates (e.g. Fika squad / shared group) count as allies."
        );
        Defib.AllowSameSide = Config.Bind(
            "2. Defib Ally Revive",
            "AllowSameSide",
            false,
            "Also treat same Side (USEC/BEAR) as allies when GroupId does not match."
        );
        Defib.ConsumeDefibrillator = Config.Bind(
            "2. Defib Ally Revive",
            "ConsumeDefibrillator",
            true,
            "Spend the 1/1 charge and remove the defibrillator from inventory after a successful revive."
        );
        Defib.FullHealOnRevive = Config.Bind(
            "2. Defib Ally Revive",
            "FullHealOnRevive",
            true,
            "Restore full health on revive. If false, only restores destroyed vital parts."
        );
        Defib.Debug = Config.Bind(
            "2. Defib Ally Revive",
            "Debug",
            false,
            "Verbose logging for ally downed / revive."
        );
    }

    private void BindHealing()
    {
        Healing.ContinuousHealing = Config.Bind(
            "3. Continuous Healing",
            "Enabled",
            true,
            "Continue healing across body parts instead of stopping after one limb."
        );
        Healing.HealLimbs = Config.Bind(
            "3. Continuous Healing",
            "Heal Limbs",
            true,
            "Also continue with surgery kits / splints (MedicalItemClass)."
        );
        Healing.HealDelay = Config.Bind(
            "3. Continuous Healing",
            "Heal Delay",
            0,
            new ConfigDescription(
                "Delay between limb heals. Game default is ~2; 0 is instant continue.",
                new AcceptableValueRange<int>(0, 5)
            )
        );
        Healing.ResetAnimation = Config.Bind(
            "3. Continuous Healing",
            "Reset Animations",
            true,
            "Play a fresh animation between limbs."
        );
        Healing.CancelOnDamage = Config.Bind(
            "4. Interrupt",
            "Cancel On Damage",
            true,
            "Any HP damage during healing cancels the med animation and restores the last weapon."
        );
        Healing.ScratchHeal = Config.Bind(
            "5. Scratch Heal",
            "Enabled",
            true,
            "While healing, also top up other limbs that only miss a few HP (uses medkit resource)."
        );
        Healing.ScratchHealAmount = Config.Bind(
            "5. Scratch Heal",
            "Heal Amount",
            2.5f,
            new ConfigDescription(
                "HP restored per scratched limb per continue tick.",
                new AcceptableValueRange<float>(1f, 5f)
            )
        );
        Healing.ScratchMaxMissingHp = Config.Bind(
            "5. Scratch Heal",
            "Max Missing HP",
            8f,
            new ConfigDescription(
                "Only limbs missing at most this much HP count as scratches.",
                new AcceptableValueRange<float>(2f, 20f)
            )
        );
    }

    private void EnableAutoMedPatches()
    {
        EnablePatch(new InventoryControllerCreatedPatch());
        EnablePatch(new StashInventoryControllerCreatedPatch());
        EnablePatch(new MainMenuInventoryReadyPatch());
        EnablePatch(new InventoryScreenShowPatch());
        EnablePatch(new MoveOperationRaiseEventsPatch());
        EnablePatch(new InventoryAddItemPatch());
        EnablePatch(new InventoryRemoveItemPatch());
        EnablePatch(new GridItemViewBindDisplayPatch());
    }

    private void EnableDefibPatches()
    {
        EnablePatch(new ActiveHealthControllerKillPatch());
        EnablePatch(new SetQuickSlotItemPatch());
        EnablePatch(new IsAtBindablePlacePatch());
        EnablePatch(new QuickSlotDefibResourcePatch());
        EnablePatch(new GridItemViewDefibResourcePatch());
        EnablePatch(new PlayerUpdateTickPatch());
    }

    private void EnableHealingPatches()
    {
        EnablePatch(new StartHealPatch());
        EnablePatch(new EndHealPatch());
        EnablePatch(new CancelHealPatch());
        EnablePatch(new DamageInterruptPatch());
    }

    private static void EnablePatch(ModulePatch patch)
    {
        try
        {
            patch.Enable();
        }
        catch (Exception ex)
        {
            Log.LogError($"[MedSuite] Failed to enable {patch.GetType().Name}: {ex}");
        }
    }

    internal static void DefibDebug(string message)
    {
        if (Defib.Debug.Value)
        {
            Log.LogInfo($"[MedSuite:Defib] {message}");
        }
    }
}
