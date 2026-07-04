using UnityEditor;
using UnityEngine;

namespace SOAP
{
    [CustomEditor(typeof(StringVariable))]
    public class StringVariableEditor : Editor
    {
        private void OnEnable()  => EditorApplication.update += ForceRepaintDuringPlay;
        private void OnDisable() => EditorApplication.update -= ForceRepaintDuringPlay;

        private void ForceRepaintDuringPlay()
        {
            if (Application.isPlaying) Repaint();
        }

        public override void OnInspectorGUI()
        {
            var script = (StringVariable)target;

            // --- Description ---
            EditorGUI.BeginChangeCheck();
            string newDesc = EditorGUILayout.DelayedTextField(
                new GUIContent("Description", "Optional notes about this variable's purpose."),
                script.description);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Description");
                script.description = newDesc;
                EditorUtility.SetDirty(target);
            }

            // --- Tag ---
            EditorGUI.BeginChangeCheck();
            string newTag = EditorGUILayout.DelayedTextField(
                new GUIContent("Tag", "Optional tag for filtering. E.g. 'Player', 'Enemy', 'UI'"),
                script.tag);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Tag");
                script.tag = newTag;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space();

            // --- Initial Value ---
            EditorGUI.BeginChangeCheck();
            string newInitial = EditorGUILayout.DelayedTextField("Initial (Reset) Value", script.initialValue);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Initial Value");
                script.initialValue = newInitial;
                EditorUtility.SetDirty(target);
            }

            // --- Reset Lifecycle ---
            EditorGUI.BeginChangeCheck();
            var newResetOn = (ValueReset)EditorGUILayout.EnumPopup("Reset On", script.valueReset);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Reset Mode");
                script.valueReset = newResetOn;
                EditorUtility.SetDirty(target);
            }

            // --- Debug Log ---
            EditorGUI.BeginChangeCheck();
            bool newDebug = EditorGUILayout.Toggle(
                new GUIContent("Debug Log Enabled", "Logs to console every time this value changes."),
                script.debugLogEnabled);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Toggle Debug Log");
                script.debugLogEnabled = newDebug;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space();

            // --- Live Runtime Values ---
            GUI.enabled = false;
            EditorGUILayout.TextField("Current (Runtime) Value", script.currentValue ?? string.Empty);
            EditorGUILayout.TextField("Previous (Runtime) Value", script.previousValue ?? string.Empty);
            GUI.enabled = true;

            if (Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Reset to Initial Value"))
                    script.ResetToInitial();
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}