using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using static NGPT.Path;

namespace NebulaHeadlessAssistant.Patches
{
    internal class PlanetATSheildFix_Headless
    {
        [HarmonyPrefix,HarmonyPatch(typeof(PlanetATField), nameof(PlanetATField.RecalculatePhysicsShape))]
        public static bool RecalculatePhysicsShape_Prefix(PlanetATField __instance)
        {
            // If we're the server, let's do some stuff manually!
            if (NebulaHeadlessAssistant.Instance.IsDedicated)
            {
                if (__instance.generatorCount == 0)
                {
                    __instance.ClearPhysics();
                    __instance.energyMaxTarget = 0L;
                }
                
                __instance.CreatePhysics();
                __instance.isSpherical = true;

                /*
                 * This is usually computed on the GPU, No idea what math the GPU does, so we're shoving 0.95 here.
                 * On my own testing this goes as low as 0.35 during initial planet shield spin up
                 * and when it hits 1.0 it seems to stop calling this method.
                 */
                __instance.energyMaxTarget = (long)(1200000000000.0 * 0.95 + 0.5);

                if (__instance.energy > 0)
                {
                    __instance.isEmpty = false;
                }

                // I believe these are used in raycasting tests to see if a relay would hit a shield, so we need them.
                if (__instance.colliderHotTicks > 0)
                    __instance.OpenColliderObject();
                else
                    __instance.CloseColliderObject();
                
                return false;
            }

            return true;
        }
    }
}
