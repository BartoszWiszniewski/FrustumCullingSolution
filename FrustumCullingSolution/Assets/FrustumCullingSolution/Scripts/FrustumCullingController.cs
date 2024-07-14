using FrustumCullingSolution.Scripts.Collections;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace FrustumCullingSolution.Scripts
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-10000)]
    public sealed class FrustumCullingController : MonoBehaviour
    {
        private static FrustumCullingController Instance { get; set; }

        [SerializeField]
        private FrustumCullingSystemCameraTarget cameraTarget;

        public FrustumCullingSystemCameraTarget CameraTarget => cameraTarget;

        [SerializeField]
        private Camera targetCamera;
        public Camera TargetCamera => _currentCamera;

        private Camera _currentCamera;
        public Camera CurrentCamera => _currentCamera;

        [SerializeField]
        private int bufferSize = 128;

        [SerializeField, Range(0, 10)]
        private int refreshRate = 1;

        private int _sinceLastUpdate = 0;
        
        private CVector<FrustumCullingItem> _frustumCullingItems;
        private CNativeVector<FrustumCullingItemData> _frustumCullingData;
        
        private int _processorCount = 4;
        private void Awake()
        {
            _processorCount = SystemInfo.processorCount;
            _sinceLastUpdate = refreshRate + 1;
            
            if (Instance != null)
            {
                Destroy(Instance);
                return;
            }
        
            Instance = this;
            if (bufferSize < 32)
            {
                bufferSize = 32;
            }

            _frustumCullingItems ??= new CVector<FrustumCullingItem>(bufferSize);
            _frustumCullingData ??= new CNativeVector<FrustumCullingItemData>(bufferSize, Allocator.Persistent);
        }

        private void OnEnable()
        {
            Instance = this;
            _frustumCullingItems ??= new CVector<FrustumCullingItem>(bufferSize);
            if (_frustumCullingData == null)
            {
                var size = bufferSize;
                if (_frustumCullingItems != null && _frustumCullingItems.Count > size)
                {
                    size = _frustumCullingItems.Count;
                }
                _frustumCullingData = new CNativeVector<FrustumCullingItemData>(size, Allocator.Persistent);

                for (var i = 0; i < _frustumCullingItems.Count; i++)
                {
                    var frustumCullingItem = _frustumCullingItems[i];
                    var bounds = frustumCullingItem.Bounds;
                    _frustumCullingData.Add(
                        new FrustumCullingItemData(new float3(bounds.center), new float3(bounds.extents)), null);
                }
            }
        }

        private void OnDisable()
        {
            if (_frustumCullingItems != null)
            {
                foreach (var item in _frustumCullingItems)
                {
                    item.SetVisibility(true);
                }
            }

            _frustumCullingData.Dispose();
            _frustumCullingData = null;
            Instance = null;
        }
        
        private void OnDestroy()
        {
            Instance = null;
        }

        private void Update()
        {
            _sinceLastUpdate++;
        }

        //We run this in late update
        private void LateUpdate()
        {
            if (_sinceLastUpdate < refreshRate)
            {
                return;
            }

            _sinceLastUpdate = 0;
            _currentCamera = GetCamera();
            if (_currentCamera == null || !_currentCamera.gameObject.activeSelf || !_currentCamera.enabled)
            {
                return;
            }

            if (_frustumCullingData == null || _frustumCullingItems == null)
            {
                return;
            }
            
            var planes = GeometryUtility.CalculateFrustumPlanes(_currentCamera);
            var frustumPlanesNative = new NativeArray<float4>(6, Allocator.TempJob);
            for (var i = 0; i < 6; i++)
            {
                frustumPlanesNative[i] = new float4(planes[i].normal.x, planes[i].normal.y,
                    planes[i].normal.z, planes[i].distance);
            }

            var frustumCullingJob = new FrustumCullingJob(frustumPlanesNative, _frustumCullingData.Items);

            var size = _frustumCullingItems.Count;
            var jobHandle = frustumCullingJob.Schedule(size, Mathf.CeilToInt((float)size / _processorCount));
            jobHandle.Complete();
            
            for (var i = 0; i < size; i++)
            {
                _frustumCullingItems[i].SetVisibility(_frustumCullingData[i].IsVisible);
            }

            frustumPlanesNative.Dispose();
        }

        private Camera GetCamera()
        {
            //Select proper camera
            var result = cameraTarget switch
            {
                FrustumCullingSystemCameraTarget.Main => Camera.main,
                FrustumCullingSystemCameraTarget.Current => Camera.current,
                FrustumCullingSystemCameraTarget.Selected => targetCamera,
                _ => null
            };

            if (result == null)
            {
                return null;
            }

            //We want to use only game camera, using editor changes current camera and makes system unusable
            if (result.cameraType != CameraType.Game)
            {
                return _currentCamera;
            }

            return result;
        }


        /// <summary>
        /// Changes the target camera for the frustum culling system.
        /// </summary>
        /// <param name="type">The camera target type.</param>
        /// <param name="target">The new target camera only set when type is Selected</param>
        public void ChangeTargetCamera(FrustumCullingSystemCameraTarget type, Camera target = null)
        {
            cameraTarget = type;
            if (target != null && cameraTarget == FrustumCullingSystemCameraTarget.Selected)
            {
                targetCamera = target;
            }
        }

        /// <summary>
        /// Do not use. FrustumCullingItem calls it OnEnable. Adds a FrustumCullingItem to the FrustumCullingController.
        /// </summary>
        /// <param name="frustumCullingItem">The FrustumCullingItem to add.</param>
        public static void Add(FrustumCullingItem frustumCullingItem)
        {
            if (Instance == null)
            {
                return;
            }

            if (frustumCullingItem.Index >= 0)
            {
                return;
            }
            
            Instance._frustumCullingItems.Add(frustumCullingItem, (item, i) => item.Index = i);
            var bounds = frustumCullingItem.Bounds;
            Instance._frustumCullingData.Add(
                new FrustumCullingItemData(new float3(bounds.center), new float3(bounds.extents)), null);
        }
        
        /// <summary>
        /// Do not use. FrustumCullingItem calls it OnDisable. Removes a FrustumCullingItem to the FrustumCullingController.
        /// </summary>
        /// <param name="frustumCullingItem">The FrustumCullingItem to remove.</param>
        public static void Remove(FrustumCullingItem frustumCullingItem)
        {
            if (Instance == null)
            {
                return;
            }

            if (frustumCullingItem.Index < 0)
            {
                return;
            }
            
            Instance._frustumCullingData.RemoveAt(frustumCullingItem.Index, null);
            Instance._frustumCullingItems.RemoveAt(frustumCullingItem.Index, (item, i) =>
            {
                item.Index = i;
                Instance.InternalUpdateItem(item);
            });
            frustumCullingItem.Index = -1;
        }

        /// <summary>
        /// Do not use. FrustumCullingItem calls it on enable and when needs update.
        /// </summary>
        /// <param name="frustumCullingItem">The FrustumCullingItem to update.</param>
        public static void UpdateItem(FrustumCullingItem frustumCullingItem)
        {
            if (Instance == null)
            {
                return;
            }

            Instance.InternalUpdateItem(frustumCullingItem);
        }

        private void InternalUpdateItem(FrustumCullingItem frustumCullingItem)
        {
            var bounds = frustumCullingItem.Bounds;
            Instance._frustumCullingData[frustumCullingItem.Index] =
                new FrustumCullingItemData(new float3(bounds.center), new float3(bounds.extents));
        }
    }
    
        
    public enum FrustumCullingSystemCameraTarget
    {
        Main,
        Current,
        Selected
    }
}
