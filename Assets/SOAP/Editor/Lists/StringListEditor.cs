using UnityEditor;

namespace SOAP
{
    [CustomEditor(typeof(StringList))]
    public class StringListEditor : ScriptableListEditorBase<string>
    {
        protected override ScriptableListSO<string> GetTarget()
            => (StringList)target;
    }
}