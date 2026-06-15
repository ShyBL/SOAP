using UnityEngine;
using UnityEngine.Serialization;

namespace SOAP
{
    [CreateAssetMenu(fileName = "New_FloatVariable", menuName = "SOAP/Global-Variable/Float")]

    public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("Info")]
        [TextArea(1, 3)]
        public string description;
        [Tooltip("Optional tag for filtering. E.g. 'Player', 'Enemy', 'UI'")]
        public string tag;
        
        [Header("Value")]
        public float initialValue;
        
        [System.NonSerialized]
        public float currentValue;

        [System.NonSerialized]
        public float previousValue;
        
        [Header("Clamping")]
        public bool isClamped = false;
        public float minValue = 0;
        public float maxValue = 100;
        
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

        public void Set(float value)
        {
            previousValue = currentValue;
            currentValue = isClamped ? Mathf.Clamp(value, minValue, maxValue) : value;
            
            if (debugLogEnabled)
            {
                Debug.Log($"Set {value} to {currentValue}");
            }
        }
        public void ResetToInitial() => Set(initialValue);
        
        public void Add(float amount) => Set(currentValue + amount);
        public void Subtract(float amount) => Set(currentValue - amount);
    }
}