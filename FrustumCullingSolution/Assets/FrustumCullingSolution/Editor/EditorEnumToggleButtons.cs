#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace FrustumCullingSolution.Editor
{
    public static class EditorEnumToggleButtons
    {
        public static void Draw<T>(SerializedObject serializedObject, string propertyName) where T : Enum
        {
            var prop = serializedObject.FindProperty(propertyName);
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.PrefixLabel(EditorGuiUtils.ToLabelText(propertyName));
            
            prop.enumValueIndex = GUILayout.Toolbar(
                prop.enumValueIndex,
                Enum.GetNames(typeof(T))
            );
            
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif