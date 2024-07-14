using Unity.Mathematics;

namespace FrustumCullingSolution.Scripts
{
    public  struct FrustumCullingItemData
    {
        public bool IsVisible;
        public float3 Center;
        public float3 Extents;
        
        public FrustumCullingItemData(float3 center, float3 extents)
        {
            IsVisible = false;
            Center = center;
            Extents = extents;
        }
    }
}