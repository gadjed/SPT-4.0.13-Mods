using Comfort.Common;
using EFT;
using QuestingBots.Helpers;
using UnityEngine;

namespace QuestingBots.Models.DebugGizmos
{
    public class BotPopulationOverlayGizmo : AbstractDebugGizmo
    {
        public DebugOverlay Overlay { get; }

        private string overlayText = "";

        public BotPopulationOverlayGizmo() : base(500)
        {
            Overlay = new DebugOverlay(UpdateGUIStyle);
        }

        public override bool ReadyToDispose() => false;

        protected override void OnDispose()
        {
            Overlay.Dispose();
        }

        protected override void OnUpdate()
        {
            if (!QuestingBotsPluginConfig.ShowBotPopulationOverlay.Value)
            {
                return;
            }

            if (!Singleton<GameWorld>.Instantiated)
            {
                return;
            }

            GameWorld gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld.AllAlivePlayersList == null)
            {
                return;
            }

            int aliveBots = 0;
            int alivePmcs = 0;

            foreach (Player player in gameWorld.AllAlivePlayersList)
            {
                if (!player.IsAI)
                {
                    continue;
                }

                aliveBots++;

                if (player.Profile.WillBeAPMC())
                {
                    alivePmcs++;
                }
            }

            overlayText = $"PMC: {alivePmcs}  |  Bots: {aliveBots}";
        }

        public override GUIStyle UpdateGUIStyle()
        {
            Overlay.GuiStyle = DebugHelpers.CreateGuiStylePlayerCoordinates();
            return Overlay.GuiStyle;
        }

        public override void Draw()
        {
            if (!QuestingBotsPluginConfig.ShowBotPopulationOverlay.Value)
            {
                return;
            }

            if (string.IsNullOrEmpty(overlayText))
            {
                return;
            }

            Overlay.Draw(overlayText, getGizmoPosition);
        }

        private Vector2 getGizmoPosition(DebugOverlay.GizmoPositionRequestParams requestParams)
        {
            return new Vector2(3, 3);
        }
    }
}
