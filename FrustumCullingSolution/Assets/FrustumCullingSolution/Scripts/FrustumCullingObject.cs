using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrustumCullingSolution.Scripts
{
    /// <summary>
    /// Represents an object that is used for frustum culling.
    /// </summary>
    public sealed class FrustumCullingObject : FrustumCullingItem
    {
        [SerializeField]
        private FrustumCullingObjectType objectType;
        public override FrustumCullingObjectType ObjectType => objectType;

        [SerializeField] 
        private FrustumCullingBoundsSource boundsSource;

        public FrustumCullingBoundsSource BoundsSource => boundsSource;
        
        [SerializeField]
        private Renderer[] renderers;
        
        [SerializeField]
        private ParticleSystem[] particleSystems;
        
        [SerializeField]
        private Behaviour[] behaviours;

        [SerializeField]
        private Collider[] colliders;
        
        private Vector3 _lastPosition;

        /// <summary>
        /// Initializes the FrustumCullingObject.
        /// </summary>
        /// <param name="frustumCullingObjectType">The type of the FrustumCullingObject.</param>
        /// <param name="frustumCullingBoundsSource">The source of the bounds for FrustumCullingObject.</param>
        /// <param name="bounds">The custom bounds for FrustumCullingObject when frustumCullingBoundsSource is Custom</param>
        public void Initialise(FrustumCullingObjectType frustumCullingObjectType, FrustumCullingBoundsSource frustumCullingBoundsSource, Bounds bounds = default, float expandBoundsBy = 0.5f)
        {
            expandBounds = expandBoundsBy;
            objectType = frustumCullingObjectType;
            boundsSource = frustumCullingBoundsSource;
            FetchComponents(false);
            if (frustumCullingBoundsSource != FrustumCullingBoundsSource.Custom)
            {
                UpdateBounds();
            }
            else
            {
                Bounds = bounds;
            }
        }

        private void Start()
        {
            FetchComponents(true);
            if (boundsSource != FrustumCullingBoundsSource.Custom)
            {
                UpdateBounds();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _lastPosition = transform.position;
        }

        private void Update()
        {
            if (ObjectType == FrustumCullingObjectType.Static)
            {
                return;
            }

            var currentPosition = transform.position;
            if (Vector3.Distance(currentPosition, _lastPosition) > 0.01f)
            {
                _lastPosition = currentPosition;
                var bounds = Bounds; 
                bounds.center = currentPosition;
                Bounds = bounds;
                FrustumCullingController.UpdateItem(this);
            }
        }

        /// <summary>
        /// Do not use this method. It's called by FrustumCullingController. Sets the visibility of the object.
        /// </summary>
        /// <param name="visible">The visibility state to set.</param>
        public override void SetVisibility(bool visible)
        {
            if (!IsActive || !NeedsUpdate(visible))
            {
                return;
            }
            
            if (renderers != null)
            {
                foreach (var r in renderers)
                {
                    r.enabled = visible;
                }
            }

            if (behaviours != null)
            {
                foreach (var b in behaviours)
                {
                    b.enabled = visible;
                }
            }

            if (particleSystems != null)
            {
                foreach (var p in particleSystems)
                {
                    if (visible)
                    {
                        p.Play();
                    }
                    else
                    {
                        p.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    }
                }
            }
        }

        protected override Bounds CalculateBounds()
        {
            switch (boundsSource)
            {
                case FrustumCullingBoundsSource.Renderers:
                    FetchComponents(true);
                    return CalculateBounds(renderers.Where(x => x != null).Select(x => x.bounds).ToArray());
                case FrustumCullingBoundsSource.Colliders:
                    FetchComponents(true);
                    return CalculateBounds(colliders.Where(x => x != null).Select(x => x.bounds).ToArray());
                case FrustumCullingBoundsSource.Custom:
                    return Bounds;
                default:
                {
                    Debug.LogError($"{nameof(FrustumCullingObject)} {nameof(CalculateBounds)} unsupported bounds source: {boundsSource}");
                    return Bounds;
                }
            }
            
        }

        /// <summary>
        /// Adds behaviours which will be enabled or disabled when visibility changes.
        /// </summary>
        /// <param name="behavioursToAdd">An enumerable collection of behaviours to add.</param>
        public void AddBehaviours(IEnumerable<Behaviour> behavioursToAdd)
        {
            behaviours = behaviours.Concat(behavioursToAdd.Where(x => x != null)).ToArray();
        }
        
        /// <summary>
        /// Adds renderers which will be enabled or disabled when visibility changes.
        /// </summary>
        /// <param name="behavioursToAdd">An enumerable collection of renderers to add.</param>
        public void AddRenderers(IEnumerable<Renderer> behavioursToAdd)
        {
            renderers = renderers.Concat(behavioursToAdd.Where(x => x != null)).Where(x => x != null).ToArray();
            if (BoundsSource != FrustumCullingBoundsSource.Custom)
            {
                UpdateBounds();
            }
        }
        
        /// <summary>
        /// Adds behaviours to the FrustumCullingObject which will be enabled or disabled when visiblity changes.
        /// </summary>
        /// <param name="behavioursToAdd">An enumerable collection of behaviours to add.</param>
        public void AddColliders(IEnumerable<Collider> behavioursToAdd)
        {
            colliders = colliders.Concat(behavioursToAdd.Where(x => x != null)).ToArray();
            if (BoundsSource != FrustumCullingBoundsSource.Custom)
            {
                UpdateBounds();
            }
        }

        private Bounds CalculateBounds(Bounds[] bounds)
        {
            if (!bounds.Any())
            {
                return Bounds;
            }
            
            var result = bounds.First();

            foreach (var bound in bounds.Skip(1))
            {
                result.Encapsulate(bound);
            }

            return result;
        }
        
        protected override void Reset()
        {
            FetchComponents(false);
            base.Reset();
        }

        /// <summary>
        /// Fetches renderers, colliders, particleSystems, animator from object and children.
        /// </summary>
        /// <param name="updateOnlyNotExisting">If set to true, only updates the components that are not already assigned.</param>
        public void FetchComponents(bool updateOnlyNotExisting)
        {
            if (updateOnlyNotExisting)
            {
                if (renderers == null || renderers.All(x => x == null))
                {
                    renderers = GetComponentsInChildren<Renderer>(true);
                }

                if (colliders == null || colliders.All(x => x == null))
                {
                    colliders = GetComponentsInChildren<Collider>(true);
                }

                if (particleSystems == null || particleSystems.All(x => x == null))
                {
                    particleSystems = GetComponentsInChildren<ParticleSystem>(true);
                }

                if (behaviours == null || behaviours.All(x => x == null))
                {
                    behaviours = GetComponentsInChildren<Animator>(true).Cast<Behaviour>().ToArray();
                }
            }
            else
            {
                renderers = GetComponentsInChildren<Renderer>();
                colliders = GetComponentsInChildren<Collider>();
                particleSystems = GetComponentsInChildren<ParticleSystem>();
                behaviours = GetComponentsInChildren<Animator>().Cast<Behaviour>().ToArray();
            }
        }

        private void OnValidate()
        {
            if (boundsSource != FrustumCullingBoundsSource.Custom)
            {
                UpdateBounds();
            }
        }
    }
    
    public enum FrustumCullingBoundsSource
    {
        Renderers,
        Colliders,
        Custom,
    }
}
