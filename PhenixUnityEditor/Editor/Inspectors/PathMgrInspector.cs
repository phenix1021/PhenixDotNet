using UnityEditor;
using Phenix.Unity.Movement;
using Phenix.Unity.Editor.Utilities;
using Phenix.Unity.Editor.Control;
using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Editor.Inspector
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(PathMgr))]
    public class PathMgrInspector : UnityEditor.Editor
    {
        PathMgr _pathMgr;
        bool _switchForPath = true;     // 路径点foldout是否展开
        int _curPathPointSelected = -1;  // 当前选中的路径点

        List<Vector3> _fullPathPoints = new List<Vector3>();

        private void OnEnable()
        {
            _pathMgr = (PathMgr)target;
        }

        public override void OnInspectorGUI()
        {
            // 同步最新值
            EditorHelper.UpdateValue(serializedObject);

            // 是否循环
            EditorControl.BoolField("loop:", "", ref _pathMgr.loop);

            EditorControl.Foldout("Path:", "", ref _switchForPath);
            if (_switchForPath)
            {
                // 各个路径点
                for (int i = 0; i < _pathMgr.points.Count; ++i)
                {
                    _pathMgr.points[i] = EditorControl.Vector3Field(string.Format("point {0}:", i), "", _pathMgr.points[i]);
                }
            }           

            if (EditorControl.Button("Add Point", ""))
            {                
                _pathMgr.AddPoint(_pathMgr.transform.position + Random.onUnitSphere);                             
            }

            if (EditorControl.Button("Remove Point", ""))
            {
                _pathMgr.RemovePoint(_curPathPointSelected);
                _curPathPointSelected = _pathMgr.points.Count-1;
            }

            if (EditorControl.Button("Clear All", ""))
            {
                _pathMgr.Clear();
                _curPathPointSelected = -1;
            }

            // 提交改变
            EditorHelper.Submit(serializedObject);
        }

        private void OnSceneGUI()
        {
            for (int i = 0; i < _pathMgr.points.Count; ++i)
            {
                Vector3 pos = _pathMgr.transform.TransformPoint(_pathMgr.points[i]);
                bool selected = (_curPathPointSelected == i);
                if (selected)
                {
                    EditorHelper.AddMoveAxis(ref pos);
                }
                if (EditorHelper.Dot(pos, Color.white))
                {                    
                    _curPathPointSelected = i;                    
                }                
                _pathMgr.points[i] = _pathMgr.transform.InverseTransformPoint(pos);
            }

            MathTools.GetCatmullRomSplineFullPathPoints(_pathMgr.points, _pathMgr.loop, ref _fullPathPoints);
            Handles.color = Color.yellow;
            for (int i = 0; i < _fullPathPoints.Count-1; i++)
            {
                Vector3 from = _pathMgr.transform.TransformPoint(_fullPathPoints[i]);
                Vector3 to = _pathMgr.transform.TransformPoint(_fullPathPoints[i+1]);                
                Handles.DrawLine(from, to);
            }
        }
    }
}