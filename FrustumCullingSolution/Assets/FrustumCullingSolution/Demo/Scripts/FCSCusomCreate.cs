using FrustumCullingSolution.Scripts;
using UnityEngine;

namespace FrustumCullingSolution.Demo.Scripts
{
    public class FCSCusomCreate : MonoBehaviour
    {
        private void Start()
        {
            var fco = gameObject.AddComponent<FrustumCullingObject>();
            fco.Initialise(FrustumCullingObjectType.Static, FrustumCullingBoundsSource.Renderers, expandBoundsBy: 1.0f);
        }
    }
}