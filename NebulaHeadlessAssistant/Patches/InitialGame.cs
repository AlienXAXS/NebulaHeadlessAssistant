using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace NebulaHeadlessAssistant.Patches
{
    internal class InitialGame
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameLoader), nameof(GameLoader.FixedUpdate))]
        public static void Tick(ref GameLoader __instance)
        {
            if ( __instance.frame == 11 )
                HiveRestorer.HiveRestorerManager.Instance.FirstStart();
        }
    }
}
