using System;
using System.Linq;
using System.Reflection;
using NebulaAPI.GameState;
using NebulaAPI;
using HarmonyLib;

namespace NebulaHeadlessAssistant
{
    internal class NebulaHeadlessAssistant
    {
        private static readonly NebulaHeadlessAssistant _instance;
        public static NebulaHeadlessAssistant Instance = _instance ??= new NebulaHeadlessAssistant();

        public bool IsMultiplayerActive;
        public bool IsDedicated;

        private readonly Harmony _harmony = new(PluginInfo.PLUGIN_ID);

        public void OnAwake()
        {
            NebulaModAPI.OnMultiplayerGameStarted += OnMultiplayerGameStarted;
            NebulaModAPI.OnPlayerJoinedGame += OnPlayerJoinedGame;
            NebulaModAPI.OnPlayerLeftGame += OnPlayerLeftGame;

            if (Environment.GetCommandLineArgs().Contains("-server"))
            {
                Log.LogInfo("Running headless, Applying NoSteamFix.");
                _harmony.PatchAll(typeof(Patches.NoSteamFix));
                //_harmony.PatchAll(typeof(Patches.InitialGame));
            }
            else
            {
                Log.LogWarning("Not running in -server mode, assuming we are a client.");
            }
        }

        private void OnPlayerLeftGame(IPlayerData player)
        {
            //HiveRestorer.HiveRestorerManager.Instance.PlayerLeft();
        }

        private void OnPlayerJoinedGame(IPlayerData player)
        {
            //HiveRestorer.HiveRestorerManager.Instance.PlayerJoined();
        }

        private void OnMultiplayerGameStarted()
        {
            IsMultiplayerActive = NebulaModAPI.IsMultiplayerActive;
            IsDedicated = NebulaModAPI.MultiplayerSession.IsDedicated;
            
            // Only patch ThreatFix if we're on version 0.9.5.2
            if (IsDedicated && BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue("dsp.nebula-multiplayer", out var pluginInfo))
            {
                if (pluginInfo.Metadata.Version.Equals(new System.Version(0, 9, 5, 2)))
                {
                    Log.LogInfo("Nebula version 0.9.5.2 found, applying Threat Fix patch");
                    _harmony.PatchAll(typeof(Patches.ThreatFix0952));
                }
            }
        }
    }
}