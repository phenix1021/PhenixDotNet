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
    public class PathMgrInspector : BaseInspector
    {
        PathMgr _pathMgr;
        bool _switchForPath = true;     // 路径点foldout是否展开
        int _curPathPointSelected = -1;  // 当前选中的路径点

        List<Vector3> _fullPathPoints = new List<Vector3>();

        protected override void OnEnable()
        {
            _pathMgr = (PathMgr)target;
        }

        protected override void OnInspectorGUI()
        {
            Prepare();

            // 是否循环
            GUIControlHelper.BoolField("loop:", "", ref _pathMgr.loop);

            GUIControlHelper.Foldout("Path:", "", ref _switchForPath);
            if (_switchForPath)
            {
                // 各个路径点
                for (int i = 0; i < _pathMgr.points.Count; ++i)
                {
                    _pathMgr.points[i] = GUIControlHelper.Vector3Field(string.Format("point {0}:", i), "", _pathMgr.points[i]);
                }
            }           

            if (GUIControlHelper.Button("Add Point", ""))
            {                
                _pathMgr.AddPoint(_pathMgr.transform.position + Random.onUnitSphere);                             
            }

            if (GUIControlHelper.Button("Remove Point", ""))
            {
                _pathMgr.RemovePoint(_curPathPointSelected);
                _curPathPointSelected = _pathMgr.points.Count-1;
            }

            if (GUIControlHelper.Button("Clear All", ""))
            {
                _pathMgr.Clear();
                _curPathPointSelected = -1;
            }

            Submit();
        }

        protected override void OnSceneGUI()
        {
            for (int i = 0; i < _pathMgr.points.Count; ++i)
            {
                Vector3 pos = _pathMgr.transform.TransformPoint(_pathMgr.points[i]);
                bool selected = (_curPathPointSelected == i);
                if (selected)
                {
                    HandlesHelper.DrawXYZAxis(ref pos);
                }
                if (HandlesHelper.DrawDot(pos, Color.white))
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