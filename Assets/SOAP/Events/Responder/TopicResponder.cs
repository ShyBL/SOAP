using UnityEngine;
using UnityEngine.Events;

namespace SOAP
{
    public class TopicResponder<T> : MonoBehaviour
    {
        [Header("Subscription")]
        public GameChannel channel;
        public GameTopic topicToListenFor;
        
        [Header("Extra Responses (No Data)")]
        [Tooltip("Triggers for things like particles or sound effects that don't need the event data.")]
        public UnityEvent onResponseTriggered;
        
        [Header("Typed Specific Response")]
        public UnityEvent<T> onTypedResponse;
        
        private void OnValidate()
        {
            if (channel == null)
            {
                Debug.LogError($"GameChannel is missing on {name}!", this);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }
        
        private void OnEnable() => channel?.Subscribe(topicToListenFor, OnSignalReceived);
        private void OnDisable() => channel?.Unsubscribe(topicToListenFor, OnSignalReceived);
        
        protected virtual void OnSignalReceived(object data)
        {
            if (data is T typedData)
            {
                onTypedResponse?.Invoke(typedData);
            }
            
            onResponseTriggered?.Invoke(); 
        }
    }
}