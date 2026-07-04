using UnityEngine;

namespace SOAP
{
    /// <summary>
    /// Apply to any ScriptableObject reference field to show an inline Create
    /// button in the inspector when the field is null.
    ///
    /// Works on direct references and array/List element slots.
    /// Abstract types (e.g. QuestObjectiveSO) show a dropdown of all concrete
    /// subtypes found in the project.
    ///
    /// Usage:
    ///   [SOCreate] public BoolVariable QuestActive;
    ///   [SOCreate] public QuestObjectiveSO[] Objectives;
    /// </summary>
    public class SOCreateAttribute : PropertyAttribute { }
}
