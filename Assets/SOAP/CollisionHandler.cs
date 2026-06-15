using UnityEngine;
using UnityEngine.Events;

namespace SOAP
{
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
        
        protected abstract void ApplyEffect(GameObject other);
        
        #region Physics Callbacks
        private void OnTriggerEnter(Collider other) { if (displayMode != PhysicsDisplayMode.CollisionOnly) Process(other.gameObject, onEnter); }
        private void OnTriggerExit(Collider other) { if (displayMode != PhysicsDisplayMode.CollisionOnly) Process(other.gameObject, onExit); }
        private void OnCollisionEnter(Collision col) { if (displayMode != PhysicsDisplayMode.TriggerOnly) Process(col.gameObject, onEnter); }
        private void OnCollisionExit(Collision col) { if (displayMode != PhysicsDisplayMode.TriggerOnly) Process(col.gameObject, onExit); }
        
        private void Process(GameObject other, UnityEvent evt)
        {
            if (!IsValidLayer(other)) return;
            if (doOnce && hasFired) return;

            ApplyEffect(other);
            evt?.Invoke();

            if (doOnce) hasFired = true;
        }
        #endregion
    }
}