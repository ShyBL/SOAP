using UnityEngine;

namespace SOAP
{
    [CreateAssetMenu(fileName = "New_StringVariable", menuName = "SOAP/Global-Variable/String")]
    public class StringVariable : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("Info")]
        [TextArea(1, 3)]
        public string description;
        [Tooltip("Optional tag for filtering. E.g. 'Player', 'Enemy', 'UI'")]
        public string tag;

        [Header("Value")]
        public string initialValue;

        [System.NonSerialized] 
        public string currentValue;
        
        [System.NonSerialized] 
        public string previousValue;
        
        [Header("Reset")]
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

        private void OnSceneLoaded(
            UnityEngine.SceneManagement.Scene scene,
            UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (mode == UnityEngine.SceneManagement.LoadSceneMode.Additive) return;
            currentValue = initialValue;
        }

        public void Set(string value)
        {
            previousValue = currentValue;
            currentValue = value;

            if (debugLogEnabled)
                Debug.Log($"[StringVariable] {name}: \"{previousValue}\" → \"{currentValue}\"");
        }

        public void ResetToInitial() => Set(initialValue);
    }
}