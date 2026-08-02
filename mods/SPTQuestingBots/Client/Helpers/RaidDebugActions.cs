using System;
using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using EFT;
using EFT.AssetsManager;
using EFT.Interactive;
using QuestingBots.Components.Spawning;
using QuestingBots.Utils;

namespace QuestingBots.Helpers
{
    public static class RaidDebugActions
    {
        public static void RespawnAllBots()
        {
            if (!IsInRaid())
            {
                Singleton<LoggingUtil>.Instance.LogWarning("Respawn All Bots: not in a raid.");
                return;
            }

            int despawned = DespawnAllAliveBots();
            Singleton<LoggingUtil>.Instance.LogInfo("Respawn All Bots: despawned " + despawned + " AI bot(s).");

            GameWorld gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld.TryGetComponent(out PScavGenerator pScavGenerator) && pScavGenerator != null)
            {
                pScavGenerator.ForceRefillAliveBots();
            }
        }

        public static void RemoveAllCorpses()
        {
            if (!IsInRaid())
            {
                Singleton<LoggingUtil>.Instance.LogWarning("Remove All Corpses: not in a raid.");
                return;
            }

            GameWorld gameWorld = Singleton<GameWorld>.Instance;
            List<Corpse> corpses = new List<Corpse>();

            for (int i = 0; i < gameWorld.LootList.Count; i++)
            {
                if (gameWorld.LootList[i] is Corpse corpse)
                {
                    corpses.Add(corpse);
                }
            }

            int removed = 0;
            foreach (Corpse corpse in corpses)
            {
                try
                {
                    gameWorld.DestroyLoot(corpse);
                    removed++;
                }
                catch (Exception e)
                {
                    Singleton<LoggingUtil>.Instance.LogWarning("Remove All Corpses: failed to destroy a corpse: " + e.Message);
                }
            }

            Singleton<LoggingUtil>.Instance.LogInfo("Remove All Corpses: removed " + removed + " corpse(s).");
        }

        private static bool IsInRaid()
        {
            return Singleton<GameWorld>.Instantiated
                && Singleton<IBotGame>.Instantiated
                && Singleton<GameWorld>.Instance.MainPlayer != null;
        }

        private static int DespawnAllAliveBots()
        {
            BotsController botsController = Singleton<IBotGame>.Instance.BotsController;
            if (botsController?.Bots?.BotOwners == null)
            {
                return 0;
            }

            BotOwner[] bots = botsController.Bots.BotOwners.ToArray();
            int despawned = 0;

            foreach (BotOwner botOwner in bots)
            {
                if (botOwner == null || botOwner.IsDead)
                {
                    continue;
                }

                if (TryDespawnBot(botsController, botOwner))
                {
                    despawned++;
                }
            }

            return despawned;
        }

        private static bool TryDespawnBot(BotsController botsController, BotOwner botOwner)
        {
            try
            {
                Player player = botOwner.GetPlayer;
                if (player == null)
                {
                    return false;
                }

                botsController.Bots.Remove(botOwner);
                botsController.BotDied(botOwner);
                botsController.DestroyInfo(player);
                AssetPoolObject.ReturnToPool(botOwner.gameObject, true);

                try
                {
                    botOwner.Dispose();
                }
                catch (Exception)
                {
                    // Dispose can throw if BotDied/ReturnToPool already cleaned up the owner.
                }

                return true;
            }
            catch (Exception e)
            {
                Singleton<LoggingUtil>.Instance.LogWarning("Failed to despawn bot " + (botOwner?.Profile?.Nickname ?? "???") + ": " + e.Message);
                return false;
            }
        }
    }
}
