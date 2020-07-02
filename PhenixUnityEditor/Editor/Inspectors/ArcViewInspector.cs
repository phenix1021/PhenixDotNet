using UnityEditor;
using Phenix.Unity.UI;

namespace Phenix.Unity.Editor.Inspector
{
    [CustomEditor(typeof(ArcView)), CanEditMultipleObjects]
    public class ArcViewInspector : BaseInspector
    {
        ArcView _arcView;
        //int _oriCellCount;
        //float _oriAxisRadius;
        //float _oriAxisOffsetDegree;
        //float _oriCellSpaceDegree;

        protected override void OnEnable()
        {
            _arcView = target as ArcView;
            //_oriCellCount = _arcView.cells.Count;
            //_oriAxisRadius = serializedObject.FindProperty("_axisRadius").floatValue;
            //_oriAxisOffsetDegree = serializedObject.FindProperty("_axisOffsetDegree").floatValue;
            //_oriCellSpaceDegree = serializedObject.FindProperty("_cellSpaceDegree").floatValue;            
        }

        /*bool IsDirty()
        {
            return _oriCellCount != _arcView.cells.Count ||
                _oriAxisRadius != serializedObject.FindProperty("_axisRadius").floatValue ||
                _oriAxisOffsetDegree != serializedObject.FindProperty("_axisOffsetDegree").floatValue ||
                _oriCellSpaceDegree != serializedObject.FindProperty("_cellSpaceDegree").floatValue;
        }*/

        protected override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (EditorApplication.isPlaying)
            {
                return;
            }

            //if (IsDirty())
            //{
            _arcView.Reset();
            //}
        }
    }
}