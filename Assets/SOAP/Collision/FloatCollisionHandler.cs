using SOAP.Utility;
using UnityEngine;

namespace SOAP
{
    public class FloatCollisionHandler : CollisionHandler
    {
        [Header("Broadcast")]
        [SOCreate] public GameChannel channelToRaise;
        public GameTopic enterTopic = GameTopic.None;
        public GameTopic exitTopic = GameTopic.None;
        
        [Header("Data Configuration")]
        [Tooltip("The Global Variable asset to change (e.g., PlayerHealth).")]
        [SOCreate, SerializeField] private FloatVariable floatVariable;
        
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
        
        protected override void ApplyEffect(GameObject other, CollisionState state)
        {
// 1. Safely get the value to subtract
            float amountToSubtract = 0f;
    
            if (changeAmount != null)
            {
                // If your FloatReference script has a built-in .Value property, use this:
                // amountToSubtract = changeAmount.Value; 
        
                // Otherwise, safely check if the variable is assigned before reading it:
                if (changeAmount.variable != null)
                {
                    amountToSubtract = changeAmount.variable.currentValue;
                }
            }

            // 2. Only apply the subtraction on Enter
            if (state == CollisionState.Enter)
            {
                if (floatVariable != null) 
                {
                    floatVariable.Subtract(amountToSubtract);
            
                    if (channelToRaise != null)
                        channelToRaise.Raise(enterTopic, floatVariable.currentValue);
                }
            }
            // 3. Handle the Exit broadcast
            else if (state == CollisionState.Exit)
            {
                if (channelToRaise != null)
                    channelToRaise.Raise(exitTopic, floatVariable.currentValue);
            }
        }
    }
}