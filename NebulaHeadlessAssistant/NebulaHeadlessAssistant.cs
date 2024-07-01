using NebulaAPI.GameState;
using NebulaAPI;

namespace NebulaHeadlessAssistant
{
    internal class NebulaHeadlessAssistant
    {
        private static readonly NebulaHeadlessAssistant _instance;
        public static NebulaHeadlessAssistant Instance = _instance ??= new NebulaHeadlessAssistant();

        public bool IsMultiplayerActive;
        public bool IsDedicated;

        public void OnAwake()
        {
            IsMultiplayerActive = NebulaModAPI.IsMultiplayerActive;
            IsDedicated = NebulaModAPI.MultiplayerSession.IsDedicated;

            var harmony = new HarmonyLib.Harmony(PluginInfo.PLUGIN_ID);

            if (IsDedicated)
            {
                Log.LogInfo("Running headless, Applying NebulaHeadlessAssistant.");

                NebulaModAPI.OnMultiplayerGameStarted += OnMultiplayerGameStarted;
                NebulaModAPI.OnPlayerJoinedGame += OnPlayerJoinedGame;
                NebulaModAPI.OnPlayerLeftGame += OnPlayerLeftGame;

                //var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "NebulaHeadlessAssistant");
                harmony.PatchAll(typeof(Patches.NebulaHeadlessAssistant));

                // Only patch ThreatFix if we're on version 0.9.5.2
                if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue("dsp.nebula-multiplayer", out var pluginInfo))
                {
                    if (pluginInfo.Metadata.Version.Equals(new System.Version(0, 9, 5, 2)))
                    {
                        Log.LogInfo("Nebula version 0.9.5.2 found, applying Threat Fix patch");
                        harmony.PatchAll(typeof(Patches.ThreatFix0952));
                    }
                }
            }
        }

        private void OnPlayerLeftGame(IPlayerData obj)
        {
            
        }

        private void OnPlayerJoinedGame(IPlayerData obj)
        {
            
        }

        private void OnMultiplayerGameStarted()
        {
            foreach (var hive in GameMain.spaceSector.dfHives)
            {
                Log.LogInfo(
                    $"Hive {hive.hiveAstroId} Threat: {hive.evolve.threat}, Level: {hive.evolve.level}, Units: {hive.units.count}");
            }
        }
        
    }
}
