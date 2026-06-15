using UnityEditor;

namespace SOAP
{
    [CustomEditor(typeof(IntList))]
    public class IntListEditor : ScriptableListEditorBase<int>
    {
        protected override ScriptableListSO<int> GetTarget()
            => (IntList)target;
    }
}