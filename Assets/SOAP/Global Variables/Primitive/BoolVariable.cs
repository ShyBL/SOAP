using UnityEngine;
using UnityEngine.Serialization;

namespace SOAP
{
    [CreateAssetMenu(fileName = "New_BoolVariable", menuName = "SOAP/Global-Variable/Bool")]

    public class BoolVariable : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("Info")]
        [TextArea(1, 3)]
        public string description;
        [Tooltip("Optional tag for filtering. E.g. 'Player', 'Enemy', 'UI'")]
        public string tag;
        
        [Header("Value")]
        public bool initialValue;
        
        [System.NonSerialized]
        public bool currentValue;

        [System.NonSerialized]
        public bool previousValue;
        
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

        public void Set(bool value)
        {
            previousValue = currentValue;
            currentValue = value;
            
            if (debugLogEnabled)
            {
                Debug.Log($"Set {value} to {currentValue}");
            }
        }
        public void ResetToInitial() => Set(initialValue);
    }
}