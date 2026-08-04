using System.Collections.Concurrent;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace InsuranceControl;

/// <summary>
/// Pre-raid inventory clones keyed by session id.
/// Used when corpse contents were emptied before insurance packaging (e.g. LootingBots).
/// </summary>
public static class RaidInventorySnapshot
{
    private static readonly ConcurrentDictionary<string, List<Item>> Snapshots = new();

    public static void Store(MongoId sessionId, List<Item> items)
    {
        Snapshots[sessionId.ToString()] = items;
    }

    public static bool TryGet(MongoId sessionId, out List<Item> items)
    {
        return Snapshots.TryGetValue(sessionId.ToString(), out items!);
    }

    public static void Clear(MongoId sessionId)
    {
        Snapshots.TryRemove(sessionId.ToString(), out _);
    }
}
