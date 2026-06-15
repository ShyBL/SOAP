using SOAP.Utility;
using UnityEngine;

namespace SOAP
{
    public class FloatCollisionHandler : CollisionHandler
    {
        [Header("Broadcast")]
        public GameChannel channelToRaise;
        public GameTopic enterTopic = GameTopic.None;
        public GameTopic exitTopic = GameTopic.None;
        
        [Header("Data Configuration")]
        [Tooltip("The Global Variable asset to change (e.g., PlayerHealth).")]
        [SerializeField] private FloatVariable floatVariable;
        
        [Tooltip("The amount to change the variable by on collision.")]
        [SerializeField] private FloatReference changeAmount;
        
        private void OnValidate()
        {
            if (channelToRaise == null)
            {
                Debug.LogError($"GameChannel is missing on {name}!", this);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            
            if (floatVariable == null)
            {
                Debug.LogError($"FloatVariable to change is missing on {name}!", this);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            
            if (changeAmount == null)
            {
                Debug.LogError($"FloatVariable change amount is missing on {name}!", this);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }
        
        protected override void ApplyEffect(GameObject other)
        {
            floatVariable.Subtract(changeAmount.variable.currentValue);
            channelToRaise.Raise(enterTopic, floatVariable.currentValue);
        }
    }
}