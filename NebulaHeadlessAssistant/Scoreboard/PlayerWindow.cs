using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NebulaHeadlessAssistant.Scoreboard
{
    internal class PlayerWindow
    {
        private const string WindowName = "";

        private Rect windowSize = new Rect(10f, 10f, 1000f, 400f);
        private Vector2 playerListScrollPosition = Vector2.zero;

        private bool windowVisible = false;

        private static PlayerWindow instance;
        public static PlayerWindow Instance = instance ??= new PlayerWindow();

        public void OnGUI()
        {
            //TODO: Check if the game is ready and the player has a mecha.

            if (windowVisible)
            {
                windowSize = GUI.Window(6245814, windowSize, WindowHandler, WindowName);
                windowSize.x = (int)(Screen.width * 0.5f - windowSize.width * 0.5f);
                windowSize.y = (int)(Screen.height * 0.5f - windowSize.height * 0.5f);
            }
        }

        public void SetWindowVisibleState(bool visible)
        {
            windowVisible = visible;
        }

        //private void AutoscaleWindow

        public void WindowHandler(int id)
        {
            try
            {
                GUILayout.BeginArea(new Rect(5f, 20f, windowSize.width - 10f, windowSize.height - 55f));
                GUILayout.BeginVertical();
                GUILayout.Space(2);
                GUILayout.Label("Online Players", UIStyle.HeaderLabelStyle);
                
                // Headers
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name", GUILayout.Width(200));
                GUILayout.Label("Location", GUILayout.Width(200));
                GUILayout.Label("Health", GUILayout.Width(200));
                GUILayout.Label("Distance", GUILayout.Width(200));
                GUILayout.Label("Latency", GUILayout.Width(100));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Box("", GUI.skin.horizontalSlider, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1), GUILayout.MaxHeight(1) } );
                GUILayout.EndHorizontal();

                GUILayout.Space(10f);

                playerListScrollPosition = GUILayout.BeginScrollView(playerListScrollPosition,
                    GUILayout.Width(windowSize.width - 10), GUILayout.ExpandHeight(true));

                for (int i = 0; i < 10; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"Example Name {i}", GUILayout.Width(200));
                    GUILayout.Label("CorLeonis I", GUILayout.Width(200));
                    GUILayout.Label("H: 100% S:100%", GUILayout.Width(200));
                    GUILayout.Label($"{Random.Range(200f,4000f)} AU", GUILayout.Width(200));
                    GUILayout.Label($"{(int)Random.Range(5f,100f)}", GUILayout.Width(100));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10f);
                }

                GUILayout.EndVertical();
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }

        }

    }
}
