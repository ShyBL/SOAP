using UnityEditor;

namespace SOAP
{
    [CustomEditor(typeof(GameObjectList))]
    public class GameObjectListEditor : ScriptableListEditorBase<UnityEngine.GameObject>
    {
        protected override ScriptableListSO<UnityEngine.GameObject> GetTarget()
            => (GameObjectList)target;
    }
}