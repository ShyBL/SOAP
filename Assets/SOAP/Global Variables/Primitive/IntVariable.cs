using UnityEngine;
using UnityEngine.Serialization;

namespace SOAP
{
    [CreateAssetMenu(fileName = "New_IntVariable", menuName = "SOAP/Global-Variable/Int")]
    public class IntVariable : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("Info")]
        [TextArea(1, 3)]
        public string description;
        [Tooltip("Optional tag for filtering. E.g. 'Player', 'Enemy', 'UI'")]
        public string tag;
        
        [Header("Value")]
        public int initialValue;
        
        [System.NonSerialized]
        public int currentValue;

        [System.NonSerialized]
        public int previousValue;
        
        [Header("Clamping")]
        public bool isClamped = false;
        public int minValue = 0;
        public int maxValue = 100;
        
        [FormerlySerializedAs("valueResetOn")] [Header("Reset")]
        public ValueReset valueReset = ValueReset.OnSingleSceneLoad;
        
        [Header("Debug")]
        public bool debugLogEnabled = false;
        
        public void OnAfterDeserialize() => currentValue = initialValue;
        public void OnBeforeSerialize() { }
        
        protected virtual void OnEnable()
        {
            if (valueReset == ValueReset.OnApplicationStart || valueReset == ValueReset.OnSingleSceneLoad)
                currentValue = initialValue;

            if (valueReset == ValueReset.OnSingleSceneLoad)
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected virtual void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (mode == UnityEngine.SceneManagement.LoadSceneMode.Additive) return;
            currentValue = initialValue;
        }

        public void Set(int value)
        {
            previousValue = currentValue;
            currentValue = isClamped ? Mathf.Clamp(value, minValue, maxValue) : value;
            
            if (debugLogEnabled)
            {
                Debug.Log($"Set {value} to {currentValue}");
            }
        }
        public void ResetToInitial() => Set(initialValue);
        
        public void Add(int amount) => Set(currentValue + amount);
        public void Subtract(int amount) => Set(currentValue - amount);
    }
}