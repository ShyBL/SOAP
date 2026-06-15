using UnityEditor;

namespace SOAP
{
    [CustomEditor(typeof(TransformList))]
    public class TransformListEditor : ScriptableListEditorBase<UnityEngine.Transform>
    {
        protected override ScriptableListSO<UnityEngine.Transform> GetTarget()
            => (TransformList)target;
    }
}