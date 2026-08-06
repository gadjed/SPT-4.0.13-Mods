using System;

namespace YellowFlareCurse.Client;

/// <summary>
/// Marks the next airplane/container loot request as the curse airdrop.
/// Needed because <c>InitAirdrop</c> only writes <c>ContainerTemplateId</c> when
/// <c>AirplaneLogicClass.offlineMode</c> is true, and pooled airplanes can lose the id.
/// </summary>
internal static class CurseAirdropGate
{
    private static int _pending;

    public static bool IsArmed => _pending > 0;

    public static void Arm()
    {
        _pending++;
        ModLogger.Info($"Curse airdrop gate armed (pending={_pending}).");
    }

    public static bool TryConsume()
    {
        if (_pending <= 0)
        {
            return false;
        }

        _pending--;
        ModLogger.Info($"Curse airdrop gate consumed (pending={_pending}).");
        return true;
    }

    /// <summary>
    /// Force the curse container id into the outbound loot request without consuming the gate
    /// (consumed once loot init actually runs).
    /// </summary>
    public static string ForceContainerId(string? current)
    {
        if (!IsArmed)
        {
            return current ?? string.Empty;
        }

        if (
            !string.IsNullOrEmpty(current)
            && string.Equals(current, YellowFlareCursePlugin.CurseContainerId, StringComparison.OrdinalIgnoreCase)
        )
        {
            return current;
        }

        ModLogger.Info(
            $"Forcing containerTemplateId '{current ?? "<null>"}' → '{YellowFlareCursePlugin.CurseContainerId}'."
        );
        return YellowFlareCursePlugin.CurseContainerId;
    }

    public static void Reset()
    {
        _pending = 0;
    }
}
