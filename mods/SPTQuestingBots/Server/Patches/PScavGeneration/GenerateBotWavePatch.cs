using HarmonyLib;
using QuestingBots.Utils;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.Generators.Bot;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Bot;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Bots;
using SPTarkov.Server.Core.Services.Bot;
using System.Reflection;
using System.Text.Json;

namespace QuestingBots.Patches.PScavGeneration
{
    public class GenerateBotWavePatch : AbstractPatch
    {
        private static LoggingUtil _loggingUtil = null!;
        private static BotNameService _botNameService = null!;
        private static BotGenerator _botGenerator = null!;
        private static MethodInfo _setRandomisedGameVersionAndCategoryMethod = null!;

        public GenerateBotWavePatch(LoggingUtil loggingUtil, BotNameService botNameService, BotGenerator botGenerator)
        {
            _loggingUtil = loggingUtil;
            _botNameService = botNameService;
            _botGenerator = botGenerator;
        }

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(
                typeof(BotController),
                "GenerateBotWave",
                [typeof(MongoId), typeof(GenerateCondition), typeof(BotGenerationDetails)]
            )!;
        }

        [PatchPostfix]
        public static void PatchPostfix(ref IEnumerable<BotBase?> __result, GenerateCondition generateRequest)
        {
            if (!generateRequest.ExtensionData!.TryGetValue("GeneratePScav", out var generatePScavObj))
            {
                _loggingUtil.Error("GenerateCondition did not contain the required GeneratePScav flag. Falling back to default SPT behavior.");

                return;
            }

            if (generatePScavObj is JsonElement generatePScavElement && generatePScavElement.GetBoolean())
            {
                __result = ConvertAllToPScav(__result, generateRequest.Limit);
            }
        }

        private static List<BotBase?> ConvertAllToPScav(IEnumerable<BotBase?> bots, int targetCount)
        {
            List<BotBase?> UpdatedBots = new List<BotBase?>();
            int convertedBots = 0;

            foreach (BotBase? bot in bots)
            {
                if (bot == null)
                {
                    _loggingUtil.Error("A null bot was generated");
                    continue;
                }

                if (CanConvertToPScav(bot))
                {
                    ConvertToPScav(bot);
                    convertedBots++;
                }

                UpdatedBots.Add(bot);
            }

            if (convertedBots < targetCount)
            {
                _loggingUtil.Warning($"{targetCount} player Scavs were requested, but only {convertedBots} were created");
            }

            return UpdatedBots;
        }

        private static bool CanConvertToPScav(BotBase bot)
        {
            if (bot.Info?.Settings?.Role == null)
            {
                _loggingUtil.Error("A bot with a null role was generated");

                return false;
            }

            if (bot.Info.Settings.Role != "assault")
            {
                return false;
            }

            return true;
        }

        private static void ConvertToPScav(BotBase bot)
        {
            _botNameService.AddRandomPmcNameToBotMainProfileNicknameProperty(bot);
            SetRandomisedGameVersionAndCategory(bot);
        }

        private static void SetRandomisedGameVersionAndCategory(BotBase bot)
        {
            _setRandomisedGameVersionAndCategoryMethod ??= GetSetRandomisedGameVersionAndCategoryMethod();
            _setRandomisedGameVersionAndCategoryMethod.Invoke(_botGenerator, [bot.Info]);
        }

        private static MethodInfo GetSetRandomisedGameVersionAndCategoryMethod()
        {
            const string methodName = "SetRandomisedGameVersionAndCategory";
            MethodInfo? method = typeof(BotGenerator).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
            {
                throw new InvalidOperationException($"Cannot find method {methodName} in BotGenerator");
            }

            return method;
        }
    }
}
