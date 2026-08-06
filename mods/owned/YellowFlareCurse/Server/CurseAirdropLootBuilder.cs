using SPTarkov.Server.Core.Generators;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Location;
using SPTarkov.Server.Core.Models.Enums;

namespace YellowFlareCurse;

/// <summary>
/// Builds the curse airdrop payload: SUPPLY / «Ящик техобеспечения» + ForcedLoot.
/// Never COMMON («общей поддержки») or WEAPON — those come from SPT random fallback
/// when ContainerId is missing/unmapped.
/// </summary>
public static class CurseAirdropLootBuilder
{
    /// <summary>LOOTCONTAINER_AIRDROP_SUPPLY_CRATE — «Ящик техобеспечения».</summary>
    public static readonly MongoId SupplyCrateTpl = ItemTpl.LOOTCONTAINER_AIRDROP_SUPPLY_CRATE;

    public static bool IsCurseContainer(MongoId containerId)
    {
        if (containerId.IsEmpty)
        {
            return false;
        }

        return IsCurseContainer(containerId.ToString());
    }

    public static bool IsCurseContainer(string? container)
    {
        if (string.IsNullOrWhiteSpace(container))
        {
            return false;
        }

        return string.Equals(container, YellowFlareCurseMod.CurseContainerIdString, StringComparison.OrdinalIgnoreCase)
            || string.Equals(
                container,
                YellowFlareCurseMod.CurseContainerId.ToString(),
                StringComparison.OrdinalIgnoreCase
            );
    }

    public static GetAirdropLootResponse Build(LootGenerator lootGenerator)
    {
        var crateId = new MongoId();
        var crate = new Item
        {
            Id = crateId,
            // NOT LOOTCONTAINER_AIRDROP_COMMON_SUPPLY_CRATE («общей поддержки»).
            Template = SupplyCrateTpl,
            Upd = new Upd { SpawnedInSession = true, StackObjectsCount = 1 },
        };

        var forcedStacks = lootGenerator.CreateForcedLoot(YellowFlareCurseMod.ForcedLoot);
        var containerItems = new List<Item> { crate };
        var crateIdString = crateId.ToString();

        foreach (var stack in forcedStacks)
        {
            if (stack == null || stack.Count == 0)
            {
                continue;
            }

            foreach (var item in stack)
            {
                if (string.IsNullOrEmpty(item.ParentId))
                {
                    item.ParentId = crateIdString;
                    item.SlotId = "main";
                }

                containerItems.Add(item);
            }
        }

        return new GetAirdropLootResponse
        {
            // Client: Enum.TryParse(Icon) → EAirdropType.Supply (decal / parachute type).
            Icon = AirdropTypeEnum.Supply,
            Container = containerItems,
        };
    }
}
