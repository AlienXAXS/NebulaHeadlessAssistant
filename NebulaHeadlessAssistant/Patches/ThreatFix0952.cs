using HarmonyLib;

namespace NebulaHeadlessAssistant.Patches
{
    internal class ThreatFix0952
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameMain), nameof(GameMain.Update))]
        public static void GameMainUpdate_Postfix()
        {
            if (!GameMain.mainPlayer.isAlive)
            {
                GameMain.mainPlayer.isAlive = true;
                Log.LogInfo($"Host mecha forced to be alive, giving full hp every tick.");
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyDFGroundSystem), nameof(EnemyDFGroundSystem.GameTickLogic))]
        public static void EnemyDFGroundSystem_GameTickLogic_Prefix(ref EnemyDFGroundSystem __instance)
        {
            // Set max hp constantly
            GameMain.mainPlayer.mecha.hp = GameMain.mainPlayer.mecha.hpMax;
        }
    }
}
