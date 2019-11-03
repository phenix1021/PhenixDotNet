using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Collection;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Effect
{
    /// <summary>
    /// 动作轨迹捕捉（残影）
    /// </summary>
    [AddComponentMenu("Phenix/Effect/MotionTrace")]
    public class MotionTrace : MonoBehaviour
    {
        // 运动样本
        class MotionSample
        {
            public GameObject slice;
            public float expireTimer;   // 过期时刻

            public MotionSample()
            {
                slice = new GameObject();
                slice.hideFlags = HideFlags.HideAndDontSave;
            }

            public void Init(Mesh mesh, Material mat, Shader shader, float expireTimer,
                Vector3 position, Quaternion rotation)
            {
                if (slice == null)
                {
                    return;
                }

                MeshFilter meshFilter = slice.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    meshFilter = slice.AddComponent<MeshFilter>();
                }
                meshFilter.mesh = mesh;

                MeshRenderer renderer = slice.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                    renderer = slice.AddComponent<MeshRenderer>();
                }                
                if (mat != null)
                {
                    renderer.material = mat;
                }
                if (shader != null && renderer.material != null)
                {
                    renderer.material.shader = shader;
                }
                this.expireTimer = expireTimer;
                slice.transform.position = position;
                slice.transform.rotation = rotation;
            }
        }

        public GameObject target;
        public Material traceMat;
        public Shader traceShader;
        public float sampleInterval;       // 采样间隔时长（秒）
        public float sampleLife;           // 样本存在时长（秒）
        public string shaderColorProp;     // shader中用来控制显示的color属性名

        Pool<MotionSample> _pool = new Pool<MotionSample>(10);
        Pool<Mesh> _poolMesh = new Pool<Mesh>(10);
        List<MotionSample> _samples = new List<MotionSample>();
        List<MotionSample> _remove = new List<MotionSample>();

        bool _active = false;
        float _nextSamepleTimer = 0;

        // Update is called once per frame
        void Update()
        {
            if (target == null)
            {
                return;
            }

            // 采样
            if (_active && Time.timeSinceLevelLoad >= _nextSamepleTimer)
            {
                DoSample();
                _nextSamepleTimer = Time.timeSinceLevelLoad + sampleInterval;
            }
            foreach (var sample in _samples)
            {
                if (sample.expireTimer <= Time.timeSinceLevelLoad)
                {
                    _remove.Add(sample);
                }
            }
            foreach (var item in _remove)
            {
                _pool.Collect(item);
                _samples.Remove(item);
            }
        }

        void DoSample()
        {
            SkinnedMeshRenderer[] renderers = target.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in renderers)
            {
                Mesh mesh = _poolMesh.Get();
                renderer.BakeMesh(mesh);
                MotionSample sample = _pool.Get();
                sample.Init(mesh, 
                    traceMat != null ? traceMat : renderer.material, 
                    traceShader != null ? traceShader : traceMat.shader, 
                    Time.timeSinceLevelLoad + sampleLife,
                    renderer.gameObject.transform.position, renderer.gameObject.transform.rotation);                                
                MaterialTools.Instance.FadeOut(sample.slice.GetComponent<MeshRenderer>(), 
                    shaderColorProp, sampleLife, sample.slice);
            }            
        }

        public void Play()
        {
            if (target == null)
            {
                return;
            }
            _active = true;
        }

        public void Stop()
        {
            if (target == null)
            {
                return;
            }
            _active = false;
        }
    }
}