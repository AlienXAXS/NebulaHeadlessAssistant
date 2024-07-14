using UnityEngine;

namespace NebulaHeadlessAssistant.Scoreboard
{
    internal class UIStyle
    {
        public static GUIStyle HeaderLabelStyle = new GUIStyle(UnityEngine.GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter, 
            fontSize = 20
        };

        public static GUIStyle RowHeaderLabelsStyle = new GUIStyle(UnityEngine.GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter, 
            fontSize = 14, 
            fontStyle = FontStyle.Bold
        };

        //public static GUIStyle RowHeaderBottomLine = new GUIStyle(GUI.skin.horizontalSlider)

        public static GUIStyle LayoutBackgroundStyle = new GUIStyle(GUI.skin.box)
        {
            normal = new GUIStyleState()
            {
                background = Texture2D.whiteTexture
            }
        };
    }
}
