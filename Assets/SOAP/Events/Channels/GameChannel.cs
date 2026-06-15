using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOAP
{
    [CreateAssetMenu(fileName = "New_GameChannel", menuName = "SOAP/Events/Channel")]
    public class GameChannel : ScriptableObject
    {
        [Header("Debug")]
        public bool debugLogEnabled = false;
        
        private readonly Dictionary<GameTopic, Action<object>> _listeners
            = new Dictionary<GameTopic, Action<object>>();
    
        public void Raise(GameTopic topic, object data = null)
        {
            if (_listeners.TryGetValue(topic, out var action))
                action.Invoke(data);

            if (debugLogEnabled)
            {
                Debug.Log($"GameChannel called topic: {topic}, invoked {action}, data: {data}, ");
            }
        }
    
        public void Subscribe(GameTopic topic, Action<object> listener)
        {
            if (!_listeners.ContainsKey(topic))
                _listeners[topic] = delegate { };
            _listeners[topic] += listener;
        }

        public void Unsubscribe(GameTopic topic, Action<object> listener)
        {
            if (!_listeners.ContainsKey(topic)) return;
            _listeners[topic] -= listener;

            // Clean up the key when no listeners remain so GetListenerCounts()
            // doesn't report phantom zero-count entries.
            if (_listeners[topic] == null || _listeners[topic].GetInvocationList().Length == 0)
                _listeners.Remove(topic);
        }
    }
}