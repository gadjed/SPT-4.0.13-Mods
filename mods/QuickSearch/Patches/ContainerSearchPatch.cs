using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace QuickSearch.Patches;

/// <summary>
/// Speeds up vanilla container / corpse search by shrinking the hardcoded delays:
/// - initial "Searching..." delay (vanilla 2000 ms)
/// - per-item reveal delay scale (vanilla 1000 f multiplier on Random.Range(1,3) / skillSpeed)
/// </summary>
internal static class ContainerSearchPatch
{
    private const int VanillaInitialSearchDelayMs = 2000;
    private const float VanillaItemRevealDelayMs = 1000f;

    public static void Apply(Harmony harmony)
    {
        var targets = DiscoverSearchStateMachines();
        if (targets.InitialMoveNext is null || targets.RevealMoveNext is null)
        {
            QuickSearchPlugin.Log.LogError(
                "[QuickSearch] Could not locate container search state machines. Mod not applied."
            );
            return;
        }

        harmony.Patch(
            targets.InitialMoveNext,
            transpiler: new HarmonyMethod(typeof(ContainerSearchPatch), nameof(InitialDelayTranspiler))
        );

        harmony.Patch(
            targets.RevealMoveNext,
            transpiler: new HarmonyMethod(typeof(ContainerSearchPatch), nameof(ItemRevealDelayTranspiler))
        );

        QuickSearchPlugin.Log.LogInfo(
            $"[QuickSearch] Patched initial delay via {targets.InitialMoveNext.DeclaringType?.FullName}.MoveNext"
        );
        QuickSearchPlugin.Log.LogInfo(
            $"[QuickSearch] Patched item reveal via {targets.RevealMoveNext.DeclaringType?.FullName}.MoveNext"
        );
    }

    private static (MethodInfo? InitialMoveNext, MethodInfo? RevealMoveNext) DiscoverSearchStateMachines()
    {
        // SPT 4.1 deobfuscates client types; discover by IL constants (2000 ms / 1000f delays).
        var byIl = DiscoverByIlConstants();
        if (byIl.InitialMoveNext is not null && byIl.RevealMoveNext is not null)
        {
            return byIl;
        }

        // Legacy SPT 4.0.13 obfuscated nested state-machine names (kept as last-chance fallback).
        var knownInitial = AccessTools.TypeByName("GClass3515+Struct915");
        var knownReveal = AccessTools.TypeByName("GClass3515+Struct916");
        if (knownInitial is not null && knownReveal is not null)
        {
            return (
                AccessTools.Method(knownInitial, "MoveNext"),
                AccessTools.Method(knownReveal, "MoveNext")
            );
        }

        return (null, null);
    }

    private static (MethodInfo? InitialMoveNext, MethodInfo? RevealMoveNext) DiscoverByIlConstants()
    {
        var assembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp");

        if (assembly is null)
        {
            return (null, null);
        }

        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t is not null).Cast<Type>().ToArray();
        }

        foreach (var parent in types.Where(t => t.IsClass && !t.IsNested))
        {
            MethodInfo? initial = null;
            MethodInfo? reveal = null;

            foreach (var nested in parent.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
            {
                var moveNext = AccessTools.Method(nested, "MoveNext");
                if (moveNext is null)
                {
                    continue;
                }

                List<CodeInstruction> instructions;
                try
                {
                    instructions = PatchProcessor.GetOriginalInstructions(moveNext).ToList();
                }
                catch
                {
                    continue;
                }

                var hasInitial = instructions.Any(i =>
                    i.opcode == OpCodes.Ldc_I4 && i.operand is int n && n == VanillaInitialSearchDelayMs);
                var hasReveal = instructions.Any(i =>
                    i.opcode == OpCodes.Ldc_R4
                    && i.operand is float f
                    && Math.Abs(f - VanillaItemRevealDelayMs) < 0.01f);

                if (hasInitial)
                {
                    initial = moveNext;
                }

                if (hasReveal)
                {
                    reveal = moveNext;
                }
            }

            if (initial is not null && reveal is not null)
            {
                QuickSearchPlugin.Log.LogInfo(
                    $"[QuickSearch] Discovered search state machines under {parent.FullName}"
                );
                return (initial, reveal);
            }
        }

        return (null, null);
    }

    private static IEnumerable<CodeInstruction> InitialDelayTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_I4
                && instruction.operand is int value
                && value == VanillaInitialSearchDelayMs)
            {
                yield return new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(ContainerSearchPatch), nameof(GetInitialSearchDelayMs))
                );
                continue;
            }

            yield return instruction;
        }
    }

    private static IEnumerable<CodeInstruction> ItemRevealDelayTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_R4
                && instruction.operand is float value
                && Math.Abs(value - VanillaItemRevealDelayMs) < 0.01f)
            {
                var replacement = new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(ContainerSearchPatch), nameof(GetItemRevealDelayMs))
                );
                replacement.labels.AddRange(instruction.labels);
                replacement.blocks.AddRange(instruction.blocks);
                yield return replacement;
                continue;
            }

            yield return instruction;
        }
    }

    private static float DelayFactor()
    {
        var speed = QuickSearchPlugin.SearchSpeedMultiplier.Value;
        if (speed < 1f)
        {
            speed = 1f;
        }

        return 1f / speed;
    }

    private static int GetInitialSearchDelayMs()
    {
        return Math.Max(0, (int)(VanillaInitialSearchDelayMs * DelayFactor()));
    }

    private static float GetItemRevealDelayMs()
    {
        return Math.Max(0f, VanillaItemRevealDelayMs * DelayFactor());
    }
}
