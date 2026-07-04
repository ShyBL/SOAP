using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SOAP
{
    [CustomPropertyDrawer(typeof(GameChannel))]
    public class GameChannelDrawer : PropertyDrawer
    {
        private const float IndentWidth = 12f;

        // ── Persistent state ──────────────────────────────────────────────────

        // Per-topic foldout state, keyed on channel instance ID + property path + topic name.
        private static readonly Dictionary<string, bool> foldoutStates
            = new Dictionary<string, bool>();

        // Scene reference count cache keyed on GameChannel instance ID.
        // Refreshed at most once per second to avoid per-frame FindObjectsOfType calls.
        private static readonly Dictionary<int, (int count, double time)> sceneRefCache
            = new Dictionary<int, (int count, double time)>();

        // ── Height ────────────────────────────────────────────────────────────

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h    = EditorGUIUtility.singleLineHeight;
            float line = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var channel = property.objectReferenceValue as GameChannel;
            if (channel == null) return h;

            h += line; // scene ref count row (edit) or subscriber header (play)

            if (!Application.isPlaying) return h;

            foreach (var kvp in channel.DebugSubscribers)
            {
                h += line; // topic row

                if (GetFoldoutOpen(property, kvp.Key.ToString()))
                    h += kvp.Value.Count * line; // one row per subscriber
            }

            return h;
        }

        // ── Draw ──────────────────────────────────────────────────────────────

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Object field — always full width on the first row
            var lineRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.ObjectField(lineRect, property, label);

            var channel = property.objectReferenceValue as GameChannel;
            if (channel != null)
            {
                float y = lineRect.yMax + EditorGUIUtility.standardVerticalSpacing;

                if (!Application.isPlaying)
                    DrawEditMode(position.x, position.width, ref y, channel);
                else
                    DrawPlayMode(position.x, position.width, ref y, property, channel);
            }

            EditorGUI.EndProperty();
        }

        // ── Edit mode ─────────────────────────────────────────────────────────

        private static void DrawEditMode(float x, float width, ref float y, GameChannel channel)
        {
            int count = GetSceneReferenceCount(channel);

            string msg = count == 0
                ? "No scene components reference this channel"
                : $"{count} component{(count == 1 ? "" : "s")} in scene reference this channel";

            EditorGUI.LabelField(
                new Rect(x + IndentWidth, y, width - IndentWidth, EditorGUIUtility.singleLineHeight),
                msg, EditorStyles.miniLabel);

            y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        // ── Play mode ─────────────────────────────────────────────────────────

        private static void DrawPlayMode(
            float x, float width, ref float y,
            SerializedProperty property, GameChannel channel)
        {
            float line = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (channel.DebugSubscribers.Count == 0)
            {
                EditorGUI.LabelField(
                    new Rect(x + IndentWidth, y, width - IndentWidth, EditorGUIUtility.singleLineHeight),
                    "No active subscribers", EditorStyles.miniLabel);
                y += line;
                return;
            }

            foreach (var kvp in channel.DebugSubscribers)
            {
                var topicKey = kvp.Key.ToString();
                var subs     = kvp.Value;
                bool isOpen  = GetFoldoutOpen(property, topicKey);

                var topicRect = new Rect(
                    x + IndentWidth, y,
                    width - IndentWidth, EditorGUIUtility.singleLineHeight);

                bool newOpen = EditorGUI.Foldout(
                    topicRect, isOpen,
                    $"{topicKey}  ({subs.Count})",
                    toggleOnLabelClick: true);

                if (newOpen != isOpen)
                    SetFoldoutOpen(property, topicKey, newOpen);

                y += line;

                if (!newOpen) continue;

                foreach (var name in subs)
                {
                    EditorGUI.LabelField(
                        new Rect(x + IndentWidth * 2, y, width - IndentWidth * 2, EditorGUIUtility.singleLineHeight),
                        $"• {name}", EditorStyles.miniLabel);
                    y += line;
                }
            }
        }

        // ── Scene reference count ─────────────────────────────────────────────

        private static int GetSceneReferenceCount(GameChannel channel)
        {
            int id = channel.GetInstanceID();

            if (sceneRefCache.TryGetValue(id, out var cached) &&
                EditorApplication.timeSinceStartup - cached.time < 1.0)
                return cached.count;

            int count = 0;

            foreach (var mb in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var type = mb.GetType();
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (field.FieldType != typeof(GameChannel)) continue;
                    try
                    {
                        if (field.GetValue(mb) as GameChannel == channel)
                        {
                            count++;
                            break; // count the component once even if it has multiple channel fields
                        }
                    }
                    catch { /* skip unreadable fields */ }
                }
            }

            sceneRefCache[id] = (count, EditorApplication.timeSinceStartup);
            return count;
        }

        // ── Foldout state helpers ─────────────────────────────────────────────

        private static string FoldoutKey(SerializedProperty property, string topic)
            => $"{property.serializedObject.targetObject.GetInstanceID()}_{property.propertyPath}_{topic}";

        private static bool GetFoldoutOpen(SerializedProperty property, string topic)
            => foldoutStates.TryGetValue(FoldoutKey(property, topic), out var open) && open;

        private static void SetFoldoutOpen(SerializedProperty property, string topic, bool open)
            => foldoutStates[FoldoutKey(property, topic)] = open;
    }
}