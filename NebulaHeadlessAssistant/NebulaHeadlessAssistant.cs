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

            if (Environment.GetCommandLineArgs().Contains("-server"))
            {
                IsDedicated = true;
                _harmony.PatchAll(typeof(Patches.NoSteamFix));
            }
        }

        private void OnMultiplayerGameStarted()
        {
        }
    }
}