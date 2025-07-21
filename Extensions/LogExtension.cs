#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Framework.Extensions
{
    public static class LogExtension
    {
        public static string GUITextField(string text, float width)
        {
            return GUILayout.TextField(text, EditorStyles.toolbarSearchField, GUILayout.MaxWidth(width));
        }

        public static bool GUIToggleButton(bool toggle, GUIContent guiContent, float width)
        {
            return GUILayout.Toggle(toggle, guiContent, EditorStyles.toolbarButton, GUILayout.MinWidth(width),
                GUILayout.ExpandWidth(true));
        }

        public static bool GUIButton(GUIContent guiContent, float width, GUIStyle editorStyle)
        {
            return GUILayout.Button(guiContent, editorStyle, GUILayout.Width(width));
        }

        public static void GUILabel(string text, Texture icon = null)
        {
            GUILayout.Label(new GUIContent(text, icon));
        }

        public static GUIContent TextContent(string title, string tooltip = null, Texture icon = null)
        {
            return EditorGUIUtility.TrTextContent(title, tooltip, icon);
        }

        public static string Color(string s, string color) => $"<color={color.ToUpper()}>{s}</color>";
    }
}
#endif