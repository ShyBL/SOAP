using UnityEngine;
using UnityEngine.Events;

namespace SOAP
{
    public enum CollisionState
    {
        Enter,
        Stay,
        Exit
    }

    public abstract class CollisionHandler : MonoBehaviour
    {
        public PhysicsDisplayMode displayMode = PhysicsDisplayMode.TriggerOnly;
        
        [Header("Filtering")]
        public bool useLayerFilter = false;
        public LayerMask targetLayers;
        public bool doOnce = false;
        private bool hasFired = false;
        
        [Header("Local Events")] 
        public UnityEvent onEnter;
        public UnityEvent onStay;
        public UnityEvent onExit;
        
        private bool IsValidLayer(GameObject obj)
        {
            if (!useLayerFilter) return true;
            return (targetLayers.value & (1 << obj.layer)) > 0;
        }
        
        protected abstract void ApplyEffect(GameObject other, CollisionState state);
        
        #region Physics Callbacks
        private void OnTriggerEnter(Collider other) { if (displayMode != PhysicsDisplayMode.CollisionOnly) Process(other.gameObject, onEnter, CollisionState.Enter); }
        // private void OnTriggerStay(Collider other)  { if (displayMode != PhysicsDisplayMode.CollisionOnly) Process(other.gameObject, onStay, CollisionState.Stay); }
        private void OnTriggerExit(Collider other)  { if (displayMode != PhysicsDisplayMode.CollisionOnly) Process(other.gameObject, onExit, CollisionState.Exit); }
        
        private void OnCollisionEnter(Collision col) { if (displayMode != PhysicsDisplayMode.TriggerOnly) Process(col.gameObject, onEnter, CollisionState.Enter); }
       // private void OnCollisionStay(Collision col)  { if (displayMode != PhysicsDisplayMode.TriggerOnly) Process(col.gameObject, onStay, CollisionState.Stay); }
        private void OnCollisionExit(Collision col)  { if (displayMode != PhysicsDisplayMode.TriggerOnly) Process(col.gameObject, onExit, CollisionState.Exit); }
        
        private void Process(GameObject other, UnityEvent evt, CollisionState state)
        {
            if (!IsValidLayer(other)) return;
            
            // Prevent re-triggering if doOnce is true, but allow the first full Enter -> Exit cycle to complete
            if (doOnce && hasFired && state == CollisionState.Enter) return;

            ApplyEffect(other, state);
            evt?.Invoke();

            // Lock it down only after the interaction cycle is complete
            if (doOnce && state == CollisionState.Exit) hasFired = true;
        }
        #endregion
    }
}