using UnityEditor;
using Phenix.Unity.UI;

namespace Phenix.Unity.Editor.Inspector
{
    [CustomEditor(typeof(SimpleScrollView)), CanEditMultipleObjects]
    public class SimpleScrollViewInspector : BaseInspector
    {
        SimpleScrollView _simpleScrollView;

        protected override void  OnEnable()
        {
            _simpleScrollView = target as SimpleScrollView;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (EditorApplication.isPlaying)
            {
                return;
            }

            _simpleScrollView.InitCellsOnInspector();
        }
    }
}