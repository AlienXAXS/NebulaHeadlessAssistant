using HarmonyLib;
using NebulaAPI;
using NebulaAPI.GameState;
using System;
using System.Linq;
using UnityEngine;

namespace NebulaHeadlessAssistant
{
    internal class NebulaHeadlessAssistant
    {
        private static readonly NebulaHeadlessAssistant _instance;
        public static NebulaHeadlessAssistant Instance = _instance ??= new NebulaHeadlessAssistant();

        public bool IsDedicated;

        private readonly Harmony _harmony = new(PluginInfo.PLUGIN_ID);

        public void OnAwake()
        {
            NebulaModAPI.OnMultiplayerGameStarted += OnMultiplayerGameStarted;
            NebulaModAPI.OnPlayerJoinedGame += OnPlayerJoinedGame;
            NebulaModAPI.OnPlayerLeftGame += OnPlayerLeftGame;

            _harmony.PatchAll(typeof(Patches.PlanetATSheildFix_Headless));

            if (Environment.GetCommandLineArgs().Contains("-server"))
            {
                IsDedicated = true;
                _harmony.PatchAll(typeof(Patches.NoSteamFix));

                // Hive Restorer Idea
                _harmony.PatchAll(typeof(Patches.GameUtility));
                // Skip this for now as it's not working.
                //HiveRestorer.HiveRestorerManager.Instance.Init();
            }
        }

        public void OnGUI()
        {
            if (IsDedicated) return;

            Scoreboard.PlayerWindow.Instance.OnGUI();

            if (Event.current.isKey && Event.current.keyCode == KeyCode.Tab)
            {
                if (Event.current.type == EventType.KeyDown)
                {
                    // Show the UI
                    Scoreboard.PlayerWindow.Instance.SetWindowVisibleState(true);
                }

                if (Event.current.type == EventType.KeyUp)
                {
                    Scoreboard.PlayerWindow.Instance.SetWindowVisibleState(false);
                }
            }
        }

        private void OnPlayerLeftGame(IPlayerData player)
        {
            HiveRestorer.HiveRestorerManager.Instance.PlayerLeft();
        }

        private void OnPlayerJoinedGame(IPlayerData player)
        {
            HiveRestorer.HiveRestorerManager.Instance.PlayerJoined();
        }

        private void OnMultiplayerGameStarted()
        {
        }
    }
}