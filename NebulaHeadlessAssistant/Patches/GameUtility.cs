using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace NebulaHeadlessAssistant.Patches
{
    internal class GameUtility
    {
        /// <summary>
        /// This is used to let the plugin know when the game has fully loaded the save file.
        /// This way all information in the game is present and ready to be read.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameLoader), nameof(GameLoader.FixedUpdate))]
        public static void Tick(ref GameLoader __instance)
        {
            if ( __instance.frame == 11 )
                HiveRestorer.HiveRestorerManager.Instance.FirstStart();
        }
    }
}
