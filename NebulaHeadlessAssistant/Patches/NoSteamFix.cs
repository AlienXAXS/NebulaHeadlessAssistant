using HarmonyLib;
using Steamworks;
using System.Reflection;

namespace NebulaHeadlessAssistant.Patches
{
    internal class NoSteamFix
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), nameof(SteamManager.Awake))]
        public static bool SteamManagerAwake(SteamManager __instance)
        {
            // Fill out some AccountData so the game has something to work with.
            AccountData.me.platform = ESalePlatform.Standalone;
            AccountData.me.userId = 1337u;
            AccountData.me.detail = new AccountData.Detail() { userName = "NebulaHeadlessAssistant" };

            // Without this, s_instance would never be set which causes some problems later on loading
            __instance.OnEnable();

            // Force the game to thinking steam has successfully initialised.
            __instance.m_bInitialized = true;

            Log.LogInfo($"SteamManager Platform: {AccountData.me.platform} UserId: {AccountData.me.userId} UserName: {AccountData.me.detail.userName}");

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(STEAMX), nameof(STEAMX.Awake))]
        [HarmonyPatch(typeof(SteamLeaderboardManager_PowerConsumption), nameof(SteamLeaderboardManager_PowerConsumption.Start))]
        [HarmonyPatch(typeof(SteamLeaderboardManager_ClusterGeneration), nameof(SteamLeaderboardManager_ClusterGeneration.Start))]
        [HarmonyPatch(typeof(SteamLeaderboardManager_SolarSail), nameof(SteamLeaderboardManager_SolarSail.Start))]
        [HarmonyPatch(typeof(SteamLeaderboardManager_UniverseMatrix), nameof(SteamLeaderboardManager_UniverseMatrix.Start))]
        [HarmonyPatch(typeof(SteamAchievementManager), nameof(SteamAchievementManager.Start))]
        [HarmonyPatch(typeof(SteamClient), nameof(SteamClient.SetWarningMessageHook))]
        [Obfuscation(Exclude = true)]
        public static bool NoOperation(object __instance)
        {
            if (__instance != null)
            {
                Log.LogInfo($"Patched a call to {__instance.GetType().Name}");
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UICommunicatorIndicator), nameof(UICommunicatorIndicator._OnLateUpdate))]
        public static bool NoOperationSilent()
        {
            return false;
        }
    }
}