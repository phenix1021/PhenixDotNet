using UnityEditor;
using Phenix.Unity.UI;

namespace Phenix.Unity.Editor.Inspector
{
    [CustomEditor(typeof(TabView)), CanEditMultipleObjects]
    public class TabViewInspector : BaseInspector
    {
        TabView _tabView;

        protected override void OnEnable()
        {
            _tabView = target as TabView;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (EditorApplication.isPlaying)
            {
                return;
            }

            _tabView.InitTabsOnInspetor();
        }
    }
}