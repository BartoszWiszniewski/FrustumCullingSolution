#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace FrustumCullingSolution.Editor
{
    public static class EditorFoldout
    {
        public static GUIStyle HeaderStyle { get; set; }
        public static GUIStyle ContentStyle { get; set; }

        static EditorFoldout()
        {
            HeaderStyle = new GUIStyle(GUI.skin.box)
            {
                normal =
                {
                    background = EditorGuiUtils.MakeBackground(new Color(0.3f, 0.3f, 0.3f, 1.0f))
                },
                padding = new RectOffset(5, 5, 2, 2),
                fontStyle = FontStyle.Bold,
                border = new RectOffset(1, 1, 1, 1)
            };
            
            ContentStyle = new GUIStyle(GUI.skin.box)
            {
                normal =
                {
                    background = EditorGuiUtils.MakeBackground(new Color(0.25f, 0.25f, 0.25f, 1.0f))
                },
                padding = new RectOffset(0, 5, 0, 5),
                border = new RectOffset(1, 1, 1, 1)
            };
        }

        public static void Draw(ref bool foldout, string title, Action drawContent)
        {
            var headerRect = GUILayoutUtility.GetRect(GUIContent.none, HeaderStyle, GUILayout.ExpandWidth(true), GUILayout.Height(20f));
            EditorGUI.DrawRect(new Rect(headerRect.x - 1, headerRect.y - 1, headerRect.width + 2, headerRect.height + 2),  Color.black);
            GUI.Box(headerRect, GUIContent.none, HeaderStyle);
            
            Rect arrowRect = new Rect(headerRect.x + 4, headerRect.y + (headerRect.height - 16) / 2, 16, 16);
            GUI.Label(arrowRect, foldout ? EditorGUIUtility.IconContent("IN foldout act on@2x") : EditorGUIUtility.IconContent("IN foldout act@2x"));
            
            Rect labelRect = new Rect(arrowRect.xMax + 4, headerRect.y, headerRect.width - arrowRect.width - 8, headerRect.height);
            GUI.Label(labelRect, title);
            
            HandleInputs(ref foldout, headerRect);
            
            if (foldout)
            {
                GUILayout.Space(-3);
                var contentRect = EditorGUILayout.BeginVertical(ContentStyle);
                //contentRect.y -= 3;
                EditorGUI.DrawRect(new Rect(contentRect.x - 1, contentRect.y - 1, contentRect.width + 2, contentRect.height + 2),  Color.black);
                GUI.Box(contentRect, GUIContent.none, ContentStyle);
                
                EditorGUI.indentLevel++;
                drawContent();
                EditorGUI.indentLevel--;
                
                EditorGUILayout.EndVertical();
            }
        }

        private static void HandleInputs(ref bool foldout, Rect rect)
        {
            var e = Event.current;
            if (e.type != EventType.MouseDown || e.button != 0 || !rect.Contains(e.mousePosition))
            {
                return;
            }
            
            foldout = !foldout;
            e.Use();
        }
    }
}
#endif