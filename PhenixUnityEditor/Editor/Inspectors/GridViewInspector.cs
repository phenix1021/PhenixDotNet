using UnityEditor;
using Phenix.Unity.UI;

namespace Phenix.Unity.Editor.Inspector
{
    [CustomEditor(typeof(GridView)), CanEditMultipleObjects]
    public class GridViewInspector : BaseInspector
    {
        GridView _gridView;

        protected override void OnEnable()
        {
            _gridView = target as GridView;
        }

        protected override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (EditorApplication.isPlaying)
            {
                return;
            }

            _gridView.InitCellsOnInspector();
        }
    }
}