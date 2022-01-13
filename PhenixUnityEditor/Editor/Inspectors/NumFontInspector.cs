using UnityEditor;
using Phenix.Unity.UI;

namespace Phenix.Unity.Editor.Inspector
{
    [CustomEditor(typeof(NumFont)), CanEditMultipleObjects]
    public class NumFontInspector : BaseInspector
    {
        NumFont _numFont;

        protected override void OnEnable()
        {
            _numFont = target as NumFont;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (EditorApplication.isPlaying)
            {
                return;
            }

            _numFont.RefreshOnInspector();
        }
    }    
}