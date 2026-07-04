using UnityEngine;

namespace SOAP.Utility
{
    [CreateAssetMenu(fileName = "New_Vector3Variable", menuName = "Signals/Variables/Vector3")]
    public class Vector3Variable : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("Info")]
        [TextArea(1, 3)]
        public string description;
        [Tooltip("Optional tag for filtering. E.g. 'Player', 'Enemy', 'UI'")]
        public string tag;

        [Header("Value")]
        public Vector3 initialValue;

        [Header("Reset")]
        public ValueReset resetOn = ValueReset.OnSingleSceneLoad;

        [Header("Debug")]
        public bool debugLogEnabled = false;

        [System.NonSerialized]
        public Vector3 currentValue;

        [System.NonSerialized]
        public Vector3 previousValue;
        
        public void OnAfterDeserialize() => currentValue = initialValue;
        public void OnBeforeSerialize() { }

        private void OnEnable()
        {
            if (resetOn == ValueReset.OnApplicationStart || resetOn == ValueReset.OnSingleSceneLoad)
                currentValue = initialValue;

            if (resetOn == ValueReset.OnSingleSceneLoad)
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (mode == UnityEngine.SceneManagement.LoadSceneMode.Additive) return;
            currentValue = initialValue;
        }

        public void Set(Vector3 value)
        {
            previousValue = currentValue;
            currentValue = value;
        }

        public void Add(Vector3 amount) => Set(currentValue + amount);

        public void ResetToInitial() => Set(initialValue);


    }
}