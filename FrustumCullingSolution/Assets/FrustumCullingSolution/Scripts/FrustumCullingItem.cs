using UnityEngine;
using UnityEngine.Events;

namespace FrustumCullingSolution.Scripts
{
    [DefaultExecutionOrder(100)]
    public abstract class FrustumCullingItem : MonoBehaviour
    {
        public int Index { get; set; } = -1;
        public FrustumCullingObjectState State { get; private set; }
        
        public abstract FrustumCullingObjectType ObjectType { get; }
        
        private bool _isActive;
        public bool IsActive => _isActive;
        
        public UnityEvent onBecomeVisible;
        public UnityEvent onBecomeInvisible;
        public UnityEvent<FrustumCullingObjectEvent> onCalculateBounds;

        [SerializeField]
        [Range(0.0f, 2.0f)]
        protected float expandBounds = 0.5f;
        
        [SerializeField]
        private Bounds bounds;

        public Bounds Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                boundsOffset = transform.position - bounds.center;
            }
        }

        [SerializeField]
        private Vector3 boundsOffset;

        protected virtual void OnEnable()
        {
            Index = -1;
            State = FrustumCullingObjectState.None;
            _isActive = true;

            var currentBoundsOffset = transform.position - bounds.center;
            if (currentBoundsOffset != boundsOffset)
            {
                bounds.center = transform.position - boundsOffset;
            }
            
            FrustumCullingController.Add(this);
        }

        protected virtual void OnDisable()
        {
            _isActive = false;
            FrustumCullingController.Remove(this);
        }
        
        public abstract void SetVisibility(bool visible);
        
        protected bool NeedsUpdate(bool visible)
        {
            var newState = visible ? FrustumCullingObjectState.Visible : FrustumCullingObjectState.Invisible;
            
            if (State == newState)
            {
                return false;
            }
            
            State = newState;
            if (visible)
            {
                onBecomeVisible?.Invoke();
            }
            else
            {
                onBecomeInvisible?.Invoke();
            }

            return true;
        }

        public void UpdateBounds()
        {
            var result = CalculateBounds();
            
            if (onCalculateBounds != null)
            {
                var boundsEvent = new FrustumCullingObjectEvent
                {
                    Bounds = result
                };
                onCalculateBounds.Invoke(boundsEvent);
                result = boundsEvent.Bounds;
            }
            
            result.Expand(expandBounds);
            
            Bounds = result;
        }

        protected abstract Bounds CalculateBounds();
        
        protected virtual void Reset()
        {
            UpdateBounds();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        }
    }
    
    public enum FrustumCullingObjectType
    {
        Static,
        Dynamic
    }

    public class FrustumCullingObjectEvent
    {
        public Bounds Bounds { get; set; }
    }
}