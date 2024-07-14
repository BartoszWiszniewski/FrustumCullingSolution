#if UNITY_EDITOR
using FrustumCullingSolution.Scripts;
using UnityEditor;

namespace FrustumCullingSolution.Editor
{
    [CustomEditor(typeof(FrustumCullingController))] 
    public class FrustumCullingControllerEditor : UnityEditor.Editor
    {
        private FrustumCullingController _frustumCullingController;

        private void OnEnable()
        {
            _frustumCullingController = (FrustumCullingController)target;
        }

        public override void OnInspectorGUI()
        {
            _frustumCullingController = (FrustumCullingController)target;
            serializedObject.Update();
            EditorEnumToggleButtons.Draw<FrustumCullingSystemCameraTarget>(serializedObject, "cameraTarget");

            if (_frustumCullingController.CameraTarget == FrustumCullingSystemCameraTarget.Selected)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("targetCamera"));
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("How many frames has to pass before update.", MessageType.Info);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("refreshRate"));
                
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Initial size of objects list.", MessageType.Info);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bufferSize"));
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif