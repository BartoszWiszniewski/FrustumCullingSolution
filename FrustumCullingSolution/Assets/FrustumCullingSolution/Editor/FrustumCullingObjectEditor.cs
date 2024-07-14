#if UNITY_EDITOR
using FrustumCullingSolution.Scripts;
using UnityEditor;
using UnityEngine;

namespace FrustumCullingSolution.Editor
{
    [CustomEditor(typeof(FrustumCullingObject))] 
    public class FrustumCullingObjectEditor : UnityEditor.Editor
    {
        private FrustumCullingObject _frustumCullingObject;
        private Vector3 _lastPosition;
        
        private void OnEnable()
        {
            _frustumCullingObject = (FrustumCullingObject)target;
            _lastPosition = _frustumCullingObject.transform.position;

            switch (_frustumCullingObject.BoundsSource)
            {
                case FrustumCullingBoundsSource.Renderers:
                case FrustumCullingBoundsSource.Colliders:
                    _frustumCullingObject.UpdateBounds();
                    break;
                case FrustumCullingBoundsSource.Custom:
                default:
                    break;
            }
        }

        private void Init()
        {
            _frustumCullingObject = (FrustumCullingObject)target;
            
            _lastPosition = _frustumCullingObject.transform.position;

            switch (_frustumCullingObject.BoundsSource)
            {
                case FrustumCullingBoundsSource.Renderers:
                case FrustumCullingBoundsSource.Colliders:
                    _frustumCullingObject.UpdateBounds();
                    break;
                case FrustumCullingBoundsSource.Custom:
                default:
                    break;
            }
        }


        private bool _renderBounds = true;
        private bool _showComponents = true;
        private bool _showEvents = true;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var currentTarget = (FrustumCullingObject)target;
            if (currentTarget != _frustumCullingObject)
            {
                Init();
            }
            
            EditorEnumToggleButtons.Draw<FrustumCullingObjectType>(serializedObject, "objectType");
            
            EditorFoldout.Draw(ref _renderBounds, "Bounds", RenderBounds);
            EditorFoldout.Draw(ref _showComponents, "Components", RenderComponents);
            EditorFoldout.Draw(ref _showEvents, "Events", RenderEvents);
            
            serializedObject.ApplyModifiedProperties();
        }

        private void RenderBounds()
        {
            EditorEnumToggleButtons.Draw<FrustumCullingBoundsSource>(serializedObject, "boundsSource");
            if (_frustumCullingObject.BoundsSource != FrustumCullingBoundsSource.Custom)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("expandBounds"));
            }
            
            GUI.enabled = _frustumCullingObject.BoundsSource == FrustumCullingBoundsSource.Custom;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bounds"));
            
            GUI.enabled = _frustumCullingObject.BoundsSource != FrustumCullingBoundsSource.Custom;
            if (GUILayout.Button("Recalculate Bounds"))
            {
                serializedObject.ApplyModifiedProperties();
                _frustumCullingObject.UpdateBounds();
                serializedObject.Update();
            }
            
            GUI.enabled = true;
        }

        private void RenderComponents()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("renderers"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("colliders"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("particleSystems"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("behaviours"));
            
            if (GUILayout.Button("Fetch Components"))
            {
                serializedObject.ApplyModifiedProperties();
                _frustumCullingObject.FetchComponents(false);
                _frustumCullingObject.UpdateBounds();
                serializedObject.Update();
            }
        }

        private void RenderEvents()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onBecomeVisible"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onBecomeInvisible"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onCalculateBounds"));
        }
        
        public void OnSceneGUI()
        {
            if (Application.isPlaying && _frustumCullingObject.ObjectType == FrustumCullingObjectType.Dynamic)
            {
                return;
            }
            
            var position = _frustumCullingObject.transform.position;
            if (_lastPosition != position)
            {
                var change = position - _lastPosition;
                _lastPosition = position;
                var bounds = _frustumCullingObject.Bounds;
                bounds.center += change;
                _frustumCullingObject.Bounds = bounds;
                EditorUtility.SetDirty(target);

                if (Application.isPlaying)
                {
                    FrustumCullingController.UpdateItem(_frustumCullingObject);
                }
            }
        }
    }
}
#endif