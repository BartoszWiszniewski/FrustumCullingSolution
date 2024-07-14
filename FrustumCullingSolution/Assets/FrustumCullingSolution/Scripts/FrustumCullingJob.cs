using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace FrustumCullingSolution.Scripts
{
    
    [BurstCompile]
    public struct FrustumCullingJob : IJobParallelFor
    {
        [ReadOnly]
        private NativeArray<float4> _frustumPlanes;
        private NativeArray<FrustumCullingItemData> _objectDataArray;

        public FrustumCullingJob(NativeArray<float4> frustumPlanes, NativeArray<FrustumCullingItemData> objectDataArray)
        {
            _frustumPlanes = frustumPlanes;
            _objectDataArray = objectDataArray;
        }
        
        public void Execute(int index)
        {
            FrustumCullingItemData data = _objectDataArray[index];

            data.IsVisible = TestPlanesAABB(data.Center, data.Extents);
            _objectDataArray[index] = data;
        }

        private bool TestPlanesAABB(float3 center, float3 extents)
        {
            for (int i = 0; i < _frustumPlanes.Length; i++)
            {
                float4 plane = _frustumPlanes[i];
                float3 normal = plane.xyz;

                float3 point = center + (extents * math.sign(normal));

                float dot = math.dot(point, normal);
                if (dot + plane.w < 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}