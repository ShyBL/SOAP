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

        // ── Listener dictionaries ─────────────────────────────────────────────

        // Original overload — Action<object> — used by TopicResponder and all
        // single-topic subscribers. Untouched for backward compatibility.
        private readonly Dictionary<GameTopic, Action<object>> _listeners
            = new Dictionary<GameTopic, Action<object>>();

        // New overload — Action<GameTopic, object> — for dynamic multi-topic
        // subscribers that need to know which topic fired
        // without closures.
        private readonly Dictionary<GameTopic, Action<GameTopic, object>> _topicListeners
            = new Dictionary<GameTopic, Action<GameTopic, object>>();

        // ── Raise ─────────────────────────────────────────────────────────────

        public void Raise(GameTopic topic, object data = null)
        {
            if (_listeners.TryGetValue(topic, out var action))
                action.Invoke(data);

            if (_topicListeners.TryGetValue(topic, out var topicAction))
                topicAction.Invoke(topic, data);

            if (debugLogEnabled)
                Debug.Log($"[GameChannel] {name} raised '{topic}'. Data: {data}");
        }

        // ── Subscribe / Unsubscribe — Action<object> ───────────────

        public void Subscribe(GameTopic topic, Action<object> listener)
        {
            if (!_listeners.ContainsKey(topic))
                _listeners[topic] = delegate { };
            _listeners[topic] += listener;

#if UNITY_EDITOR
            TrackSubscribe(topic, listener);
#endif
        }

        public void Unsubscribe(GameTopic topic, Action<object> listener)
        {
            if (!_listeners.ContainsKey(topic)) return;
            _listeners[topic] -= listener;

            if (_listeners[topic] == null || _listeners[topic].GetInvocationList().Length == 0)
                _listeners.Remove(topic);

#if UNITY_EDITOR
            TrackUnsubscribe(topic, listener);
#endif
        }

        // ── Subscribe / Unsubscribe — Action<GameTopic, object> ─────────

        public void Subscribe(GameTopic topic, Action<GameTopic, object> listener)
        {
            if (!_topicListeners.ContainsKey(topic))
                _topicListeners[topic] = delegate { };
            _topicListeners[topic] += listener;

#if UNITY_EDITOR
            TrackSubscribe(topic, listener);
#endif
        }

        public void Unsubscribe(GameTopic topic, Action<GameTopic, object> listener)
        {
            if (!_topicListeners.ContainsKey(topic)) return;
            _topicListeners[topic] -= listener;

            if (_topicListeners[topic] == null || _topicListeners[topic].GetInvocationList().Length == 0)
                _topicListeners.Remove(topic);

#if UNITY_EDITOR
            TrackUnsubscribe(topic, listener);
#endif
        }

        // ── Editor-only subscriber tracking ───────────────────────────────────

#if UNITY_EDITOR
        private readonly Dictionary<GameTopic, List<string>> _debugSubscribers
            = new Dictionary<GameTopic, List<string>>();

        public IReadOnlyDictionary<GameTopic, List<string>> DebugSubscribers => _debugSubscribers;

        private void TrackSubscribe(GameTopic topic, Delegate listener)
        {
            if (!_debugSubscribers.ContainsKey(topic))
                _debugSubscribers[topic] = new List<string>();

            var label = BuildLabel(listener);
            if (!_debugSubscribers[topic].Contains(label))
                _debugSubscribers[topic].Add(label);
        }

        private void TrackUnsubscribe(GameTopic topic, Delegate listener)
        {
            if (!_debugSubscribers.TryGetValue(topic, out var list)) return;
            list.Remove(BuildLabel(listener));
            if (list.Count == 0)
                _debugSubscribers.Remove(topic);
        }

        private static string BuildLabel(Delegate listener)
        {
            var target     = listener.Target;
            var methodName = listener.Method.Name;

            // Compiler-generated names (lambdas, anonymous methods) contain '<'.
            // After our refactor no SOAP subscriber uses closures, but handle
            // gracefully in case third-party code does.
            bool isGenerated = methodName.Contains('<');

            if (target is Component component)
            {
                var goName   = component.gameObject.name;
                var typeName = target.GetType().Name;
                return isGenerated
                    ? $"{goName} ({typeName})"
                    : $"{goName} ({typeName}) → {methodName}";
            }

            if (target == null)
                return isGenerated ? "[static]" : $"[static] → {methodName}";

            var className = target.GetType().Name;
            return isGenerated ? className : $"{className} → {methodName}";
        }
#endif
    }
}
