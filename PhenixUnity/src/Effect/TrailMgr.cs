//using UnityEngine;
//using System.Collections.Generic;
//using Phenix.Unity.Collection;
//using Phenix.Unity.Utilities;
//using Phenix.Unity.Anim;

//namespace Phenix.Unity.Effect
//{
//    /// <summary>
//    /// 采样点数据
//    /// </summary>
//    public class SamplePointData
//    {
//        public static Pool<SamplePointData> pool = new Pool<SamplePointData>(50, Reset);

//        public Vector3 pos = Vector3.zero;
//        public Vector3 forward = Vector3.zero;
//        public Vector3 right = Vector3.zero;
//        public float sampleTimer = 0;       // sample被采集的时刻        

//        public void Reset()
//        {
//            forward = right = pos = Vector3.zero;
//            sampleTimer = 0;
//        }

//        public static void Reset(SamplePointData data)
//        {
//            data.Reset();
//        }
//    }

//    [System.Serializable]
//    public class SampleClipNormalizedTime
//    {
//        public float startNormalizedTime;   // 开始播放拖尾时clip的normalizedTime
//        public float stopNormalizedTime;    // 停止播放拖尾时clip的normalizedTime    
//        [System.NonSerialized]
//        public bool sampling = false;      // 是否正在采样
//    }

//    /// <summary>
//    /// 每个trail的数据配置（基于指定trail对象、招式动画clip）
//    /// </summary>
//    [System.Serializable]
//    public class TrailData
//    {
//        public bool isStab;          // 刺（纵向移动）或斩（横向移动）
//        public GameObject trail;     // 约定位于武器尖端位置，z轴前指，x轴和刃平行）
//        public Material mat;         // trail材质  
//        public float sampleLife = 0.1f;    // 采样点存在时长（秒），即影响trail显示时长
//        public float trailWidth = 0.5f;    // trail宽度（一般来说如果isStab则为刃宽，否则为刃长）
//        public AnimationClip clip;         // 对应动画        
//        // clip的各个采样时段（注意：时段之间不能有重叠）
//        public List<SampleClipNormalizedTime> clipNormalizedTimes = new List<SampleClipNormalizedTime>();

//        List<SamplePointData> _samples = new List<SamplePointData>(); // 所有样点        
//        MeshFilter _meshFilter;             // trail的MeshFilter
//        MeshRenderer _meshRenderer;         // trail的MeshRenderer        

//        Vector3 _preWeaponPointPos = Vector3.zero;
//        Vector3 _preWeaponPointForward = Vector3.zero;
//        Vector3 _preWeaponPointRight = Vector3.zero;

//        public void Init(Material mat)
//        {
//            _meshFilter = trail.GetComponent<MeshFilter>();
//            if (_meshFilter == null)
//            {
//                _meshFilter = trail.AddComponent<MeshFilter>();
//            }
//            _meshRenderer = trail.GetComponent<MeshRenderer>();
//            if (_meshRenderer == null)
//            {
//                _meshRenderer = trail.AddComponent<MeshRenderer>();
//            }
//            _meshRenderer.material = mat;
//        }

//        public void OnUpdate(AnimProxy animProxy, float sampleInterval, float minSampleDistance)
//        {
//            if (animProxy == null || sampleInterval <= 0)
//            {
//                return;
//            }

//            if (animProxy.IsPlaying(clip) == false)
//            {
//                return;
//            }

//            Sample(animProxy, sampleInterval, minSampleDistance);
//            BuildTrailMesh();
//        }

//        /// <summary>
//        /// 依据clip中固定在武器上的trail对象的移动轨迹获得一系列采样点
//        /// </summary>
//        void Sample(AnimProxy animProxy, float sampleInterval, float minSampleDistance)
//        {
//            float normalizedTime = animProxy.GetNormalizedTime(clip);
//            // 遍历设定的每个动画采样时段（注意：时段之间不能有重叠）
//            foreach (var clipNormalizedTime in clipNormalizedTimes)
//            {
//                if (normalizedTime < clipNormalizedTime.startNormalizedTime)
//                {
//                    // 本时段尚未开始
//                    continue;
//                }

//                if (normalizedTime < clipNormalizedTime.stopNormalizedTime)
//                {
//                    if (clipNormalizedTime.sampling == false)
//                    {
//                        // 开始本时段采样
//                        _samples.Clear();
//                        clipNormalizedTime.sampling = true;
//                        _preWeaponPointPos = trail.transform.position;
//                        _preWeaponPointForward = trail.transform.forward;
//                        _preWeaponPointRight = trail.transform.right;
//                        // 从下一帧开始采样
//                        continue;
//                    }
//                }
//                else
//                {
//                    if (clipNormalizedTime.sampling)
//                    {
//                        // 停止采样
//                        clipNormalizedTime.sampling = false;
//                        _preWeaponPointPos = Vector3.zero;
//                        _preWeaponPointForward = Vector3.zero;
//                        _preWeaponPointRight = Vector3.zero;
//                    }
//                    continue;
//                }

//                // 根据采样间隔在Time.deltaTime时间里采样
//                float elapseTime = 0;
//                while (elapseTime < Time.deltaTime)
//                {
//                    DoSample(elapseTime / Time.deltaTime,
//                        Time.timeSinceLevelLoad - Time.deltaTime + elapseTime,
//                        minSampleDistance);
//                    elapseTime += sampleInterval;
//                }

//                _preWeaponPointPos = trail.transform.position;
//                _preWeaponPointForward = trail.transform.forward;
//                _preWeaponPointRight = trail.transform.right;
//            }
//        }

//        void DoSample(float progress, float timer, float minSampleDistance)
//        {
//            Vector3 pos = Vector3.Slerp(_preWeaponPointPos, trail.transform.position, progress);
//            Vector3 forward = Vector3.Slerp(_preWeaponPointForward, trail.transform.forward, progress).normalized;
//            Vector3 right = Vector3.Slerp(_preWeaponPointRight, trail.transform.right, progress).normalized;
//            if (_samples.Count > 0 && Vector3.Distance(pos, _samples[0].pos) < minSampleDistance)
//            {
//                // 防止采样点过于紧密
//                return;
//            }

//            /*if (_anim != null)
//            {
//                _anim.Sample();貌似要不要这行没差别
//            } */

//            // 创建样本数据
//            SamplePointData newSample = SamplePointData.pool.Get();
//            newSample.pos = pos;
//            newSample.forward = forward;
//            newSample.right = right;
//            newSample.sampleTimer = timer;

//            // 插入样本
//            _samples.Insert(0, newSample);
//        }

//        /// <summary>
//        /// 依据采样点构建trail对象的mesh
//        /// </summary>
//        void BuildTrailMesh()
//        {
//            // 移除超期样本
//            while (_samples.Count > 0 &&
//                Time.timeSinceLevelLoad > _samples[_samples.Count - 1].sampleTimer + sampleLife)
//            {
//                SamplePointData.pool.Collect(_samples[_samples.Count - 1]);
//                _samples.RemoveAt(_samples.Count - 1);
//            }

//            if (_samples.Count <= 1)
//            {
//                _meshFilter.mesh.Clear();
//                return;
//            }

//            Vector3[] vertices = new Vector3[_samples.Count * (isStab ? 3 : 2)];
//            Vector2[] uv = new Vector2[_samples.Count * (isStab ? 3 : 2)];

//            // Use matrix instead of transform.TransformPoint for performance reasons
//            Matrix4x4 localSpaceTransform = trail.transform.worldToLocalMatrix;

//            // 创建顶点、UV
//            for (var i = 0; i < _samples.Count; i++)
//            {
//                SamplePointData sample = _samples[i];

//                if (isStab)
//                {
//                    // 一个样本创建三个顶点: 刃面中心，刃宽左侧，刃宽右侧
//                    vertices[i * 3 + 0] = localSpaceTransform.MultiplyPoint(sample.pos);
//                    vertices[i * 3 + 1] = localSpaceTransform.MultiplyPoint(sample.pos -
//                        sample.right * trailWidth * 0.5f);
//                    vertices[i * 3 + 2] = localSpaceTransform.MultiplyPoint(sample.pos +
//                        sample.right * trailWidth * 0.5f);

//                    // 一个样本创建三个UV
//                    float u = 0.0f;
//                    if (i != 0)
//                        u = Mathf.Clamp01((Time.timeSinceLevelLoad - sample.sampleTimer) / sampleLife);

//                    uv[i * 3 + 0] = new Vector2(u, 0);
//                    uv[i * 3 + 1] = new Vector2(u, 1);
//                    uv[i * 3 + 2] = new Vector2(u, 1);
//                }
//                else
//                {
//                    // 一个样本创建两个顶点: 刃面中心，刃长底端
//                    vertices[i * 2 + 0] = localSpaceTransform.MultiplyPoint(sample.pos);
//                    vertices[i * 2 + 1] = localSpaceTransform.MultiplyPoint(sample.pos - sample.forward * trailWidth);

//                    // 一个样本创建两个UV
//                    float u = 0.0f;
//                    if (i != 0)
//                        u = Mathf.Clamp01((Time.timeSinceLevelLoad - sample.sampleTimer) / sampleLife);

//                    uv[i * 2 + 0] = new Vector2(u, 0);
//                    uv[i * 2 + 1] = new Vector2(u, 1);
//                }
//            }

//            // 创建三角形
//            int triangleCount = (_samples.Count - 1) * 2 * (isStab ? 2 : 1);    // 三角形数量
//            int trianglesVerticeCount = triangleCount * 3;                      // 三角形顶点数量
//            int[] triangles = new int[trianglesVerticeCount];
//            int groupCount = triangles.Length / (6 * (isStab ? 2 : 1));
//            for (int i = 0; i < groupCount; i++)
//            {
//                if (isStab)
//                {
//                    triangles[i * 12 + 0] = i * 3;
//                    triangles[i * 12 + 1] = i * 3 + 1;
//                    triangles[i * 12 + 2] = i * 3 + 3;

//                    triangles[i * 12 + 3] = i * 3;
//                    triangles[i * 12 + 4] = i * 3 + 3;
//                    triangles[i * 12 + 5] = i * 3 + 2;

//                    triangles[i * 12 + 6] = i * 3 + 3;
//                    triangles[i * 12 + 7] = i * 3 + 1;
//                    triangles[i * 12 + 8] = i * 3 + 4;

//                    triangles[i * 12 + 9] = i * 3 + 3;
//                    triangles[i * 12 + 10] = i * 3 + 5;
//                    triangles[i * 12 + 11] = i * 3 + 1;
//                }
//                else
//                {
//                    triangles[i * 6 + 0] = i * 2;
//                    triangles[i * 6 + 1] = i * 2 + 1;
//                    triangles[i * 6 + 2] = i * 2 + 2;

//                    triangles[i * 6 + 3] = i * 2 + 2;
//                    triangles[i * 6 + 4] = i * 2 + 1;
//                    triangles[i * 6 + 5] = i * 2 + 3;
//                }
//            }

//            MeshTools.Instance.MakeMesh(_meshFilter, vertices, triangles, uv);
//        }
//    }

//    /// <summary>
//    /// 拖尾效果管理
//    /// </summary>
//    /// <mark>
//    /// 挂接在表示拖尾的gameobject上，并且需要调整拖尾对象transform的pos为握柄处，且z轴指向武器纵向，x轴指向武器横向        
//    /// </mark>>

//    [AddComponentMenu("Phenix/Effect/TrailMgr")]
//    public class TrailMgr : MonoBehaviour
//    {
//        [SerializeField]
//        float _sampleInterval = 0.001f;     // 采样间隔（秒）        
//        [SerializeField]
//        float _minSampleDistance = 0.01f;    // 采样点的最小间距，防止过分密集      
//        [SerializeField]
//        List<TrailData> _trailConfigList = new List<TrailData>();
//        [SerializeField]
//        AnimProxy _animProxy;

//        private void Start()
//        {
//            foreach (var item in _trailConfigList)
//            {
//                item.Init(item.mat);
//            }
//        }

//        void LateUpdate()
//        {
//            foreach (var item in _trailConfigList)
//            {
//                item.OnUpdate(_animProxy, _sampleInterval, _minSampleDistance);
//            }
//        }
//    }
//}






using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Collection;
using Phenix.Unity.Utilities;
using Phenix.Unity.Anim;

namespace Phenix.Unity.Effect
{
    /// <summary>
    /// 采样点数据
    /// </summary>
    public class SamplePointData
    {
        public static Pool<SamplePointData> pool = new Pool<SamplePointData>(50, Reset);

        public Vector3 pos = Vector3.zero;
        public Vector3 forward = Vector3.zero;
        public Vector3 right = Vector3.zero;
        public float sampleTimer = 0;       // sample被采集的时刻        

        public void Reset()
        {
            forward = right = pos = Vector3.zero;
            sampleTimer = 0;
        }

        public static void Reset(SamplePointData data)
        {
            data.Reset();
        }
    }

    [System.Serializable]
    public class SampleClipNormalizedTime
    {
        public float startNormalizedTime;   // 开始播放拖尾时clip的normalizedTime
        public float stopNormalizedTime;    // 停止播放拖尾时clip的normalizedTime    
        [System.NonSerialized]
        public bool sampling = false;      // 是否正在采样
    }

    /// <summary>
    /// 每个trail的数据配置（基于指定trail对象、招式动画clip）
    /// </summary>
    [System.Serializable]
    public class TrailData
    {
        public bool isStab;                // 刺（纵向移动）或斩（横向移动）
        public GameObject trail;           // 约定位于武器尖端位置，z轴前指，x轴和刃平行）
        public Material mat;               // trail材质  
        public float sampleLife = 0.1f;    // 采样点存在时长（秒），即影响trail显示时长
        public float trailWidth = 0.5f;    // trail宽度（一般来说如果isStab则为刃宽，否则为刃长）
        public AnimationClip clip;         // 对应动画        

        // clip的各个采样时段（注意：时段之间不能有重叠）
        public List<SampleClipNormalizedTime> clipNormalizedTimes = new List<SampleClipNormalizedTime>();

        // 原始采样点(记录真实每帧轨迹点)
        List<SamplePointData> _rawSamples = new List<SamplePointData>();
        // 投入构造mesh的样点（由原始采样点插值获得，以此生成mesh）
        List<SamplePointData> _samples = new List<SamplePointData>();

        MeshFilter _meshFilter;             // trail的MeshFilter
        MeshRenderer _meshRenderer;         // trail的MeshRenderer        

        int _smoothVal = 10;                // 平滑值。影响插值生成的samples数量
        float _minSampleDistance = 0.01f;   // sample的最小间距，防止samples中的点过分密集

        List<Vector3> _rawPosList = new List<Vector3>();    // 记录_rawSamples各点位置，样条插值使用
        List<Vector3> _samplePosList = new List<Vector3>(); // 记录样条插值后生成的各点位置

        public void Init(Material mat, float smooth)
        {
            _meshFilter = trail.GetComponent<MeshFilter>();
            if (_meshFilter == null)
            {
                _meshFilter = trail.AddComponent<MeshFilter>();
            }
            _meshRenderer = trail.GetComponent<MeshRenderer>();
            if (_meshRenderer == null)
            {
                _meshRenderer = trail.AddComponent<MeshRenderer>();
            }
            _meshRenderer.material = mat;
            _smoothVal = (int)(smooth * 2);
        }

        public void OnUpdate(AnimProxy animProxy, TrailMgr.InterpolationMode interpolationMode)
        {
            if (animProxy && animProxy.IsPlaying(clip))
            {
                RawSample(animProxy, interpolationMode);                
            }

            BuildTrailMesh();
        }

        /// <summary>
        /// 依据clip中固定在武器上的trail对象的移动轨迹获得一系列采样点
        /// </summary>
        void RawSample(AnimProxy animProxy, TrailMgr.InterpolationMode interpolationMode)
        {
            // 动画的播放进度
            float normalizedTime = animProxy.GetNormalizedTime(clip);
            // 遍历设定的每个动画采样时段（注意：时段之间不能有重叠）
            foreach (var clipNormalizedTime in clipNormalizedTimes)
            {
                if (normalizedTime < clipNormalizedTime.startNormalizedTime)
                {
                    // 本时段尚未开始
                    continue;
                }

                if (normalizedTime < clipNormalizedTime.stopNormalizedTime)
                {
                    // 进入指定动画进度
                    if (clipNormalizedTime.sampling == false)
                    {
                        // 开始本时段采样                        
                        clipNormalizedTime.sampling = true;

                        for (int i = 0; i < _rawSamples.Count; ++i)
                        {
                            SamplePointData.pool.Collect(_rawSamples[i]);
                        }
                        _rawSamples.Clear();
                    }
                }
                else
                {
                    // 离开指定动画进度
                    if (clipNormalizedTime.sampling)
                    {
                        // 停止采样
                        clipNormalizedTime.sampling = false;
                    }
                    continue;
                }

                // 添加原始样本
                SamplePointData raw = SamplePointData.pool.Get();
                raw.pos = trail.transform.position;
                raw.forward = trail.transform.forward;
                raw.right = trail.transform.right;
                raw.sampleTimer = Time.timeSinceLevelLoad;
                _rawSamples.Add(raw);

                // 平滑插值处理
                MakeSmooth(/*Mathf.InverseLerp(clipNormalizedTime.startNormalizedTime,
                    clipNormalizedTime.stopNormalizedTime, normalizedTime)*/interpolationMode);
            }
        }

        void MakeSmooth(TrailMgr.InterpolationMode interpolationMode)
        {
            /*if (_rawSamples.Count < 3)
            {
                return;
            }*/

            _rawPosList.Clear();
            _samplePosList.Clear();

            foreach (var raw in _rawSamples)
            {
                _rawPosList.Add(raw.pos);
            }

            for (int i = 0; i < _samples.Count; ++i)
            {
                SamplePointData.pool.Collect(_samples[i]);
            }
            _samples.Clear();

            switch (interpolationMode)
            {
                case TrailMgr.InterpolationMode.LINE:
                    MakeSamplesByLine();
                    break;
                case TrailMgr.InterpolationMode.CATMULLROM:
                    // catmullrom样条平滑构建新的样品位置点
                    MathTools.GetCatmullRomSplineFullPathPoints(_rawPosList, false, ref _samplePosList, _smoothVal);
                    MakeSamplesBySpline();
                    break;
                case TrailMgr.InterpolationMode.BEZIER:
                    // bezier样条平滑构建新的样品位置点
                    MathTools.GetBezierSplineFullPathPoints(_rawPosList, ref _samplePosList, _smoothVal * 10);
                    MakeSamplesBySpline();
                    break;
                default:
                    return;
            }            
        }

        void MakeSamplesByLine()
        {
            if (_rawSamples.Count < 2)
            {
                return;
            }

            for (int i = 1; i < _rawSamples.Count; i++)
            {
                for (int ii = 0; ii < _smoothVal; ii++)
                {
                    float progress = i * 1f / (_smoothVal - 1);
                    Vector3 pos = Vector3.Slerp(_rawSamples[i-1].pos, _rawSamples[i].pos, progress);
                    Vector3 forward = Vector3.Slerp(_rawSamples[i-1].forward, _rawSamples[i].forward, progress).normalized;
                    Vector3 right = Vector3.Slerp(_rawSamples[i-1].right, _rawSamples[i].right, progress).normalized;
                    float timer = Mathf.Lerp(_rawSamples[i-1].sampleTimer, _rawSamples[i].sampleTimer, progress);

                    InsertSample(pos, forward, right, timer);
                }
            }
        }

        void MakeSamplesBySpline()
        {
            for (int i = 0; i < _samplePosList.Count; i++)
            {
                float progress = i * 1f / (_samplePosList.Count - 1);
                Vector3 pos = _samplePosList[i];
                Vector3 forward = Vector3.Lerp(_rawSamples[0].forward, _rawSamples[_rawSamples.Count - 1].forward, progress).normalized;
                Vector3 right = Vector3.Lerp(_rawSamples[0].right, _rawSamples[_rawSamples.Count - 1].right, progress).normalized;
                float timer = Mathf.Lerp(_rawSamples[0].sampleTimer, _rawSamples[_rawSamples.Count - 1].sampleTimer, progress);

                InsertSample(pos, forward, right, timer);
            }
        }

        bool InsertSample(Vector3 pos, Vector3 forward, Vector3 right, float timer)
        {
            if (_samples.Count > 0 && Vector3.Distance(pos, _samples[0].pos) < _minSampleDistance)
            {
                // 过滤距离过近的点
                return false;
            }

            if (Time.timeSinceLevelLoad > timer + sampleLife)
            {
                // 过滤超时的样品
                return false;
            }

            // 创建mesh样本
            SamplePointData newSample = SamplePointData.pool.Get();
            newSample.pos = pos;
            newSample.forward = forward;
            newSample.right = right;
            newSample.sampleTimer = timer;

            // 插入样本
            _samples.Insert(0, newSample);

            return true;
        }

        /// <summary>
        /// 依据采样点构建trail对象的mesh
        /// </summary>
        void BuildTrailMesh()
        {
            if (_samples.Count == 0)
            {
                return;
            }
                
            // 移除超期样本
            while (_samples.Count > 0 &&
                Time.timeSinceLevelLoad > _samples[_samples.Count - 1].sampleTimer + sampleLife)
            {
                SamplePointData.pool.Collect(_samples[_samples.Count - 1]);
                _samples.RemoveAt(_samples.Count - 1);
            }

            if (_samples.Count <= 1)
            {
                _meshFilter.mesh.Clear();
                return;
            }

            Vector3[] vertices = new Vector3[_samples.Count * (isStab ? 3 : 2)];
            Vector2[] uv = new Vector2[_samples.Count * (isStab ? 3 : 2)];

            // Use matrix instead of transform.TransformPoint for performance reasons
            Matrix4x4 localSpaceTransform = trail.transform.worldToLocalMatrix;

            // 创建顶点、UV
            for (var i = 0; i < _samples.Count; i++)
            {
                SamplePointData sample = _samples[i];

                if (isStab)
                {
                    // 一个样本创建三个顶点: 刃面中心，刃宽左侧，刃宽右侧
                    vertices[i * 3 + 0] = localSpaceTransform.MultiplyPoint(sample.pos);
                    vertices[i * 3 + 1] = localSpaceTransform.MultiplyPoint(sample.pos -
                        sample.right * trailWidth * 0.5f);
                    vertices[i * 3 + 2] = localSpaceTransform.MultiplyPoint(sample.pos +
                        sample.right * trailWidth * 0.5f);

                    // 一个样本创建三个UV
                    float u = 0.0f;
                    if (i != 0)
                        u = Mathf.Clamp01((Time.timeSinceLevelLoad - sample.sampleTimer) / sampleLife);

                    uv[i * 3 + 0] = new Vector2(u, 0);
                    uv[i * 3 + 1] = new Vector2(u, 1);
                    uv[i * 3 + 2] = new Vector2(u, 1);
                }
                else
                {
                    // 一个样本创建两个顶点: 刃面中心，刃长底端
                    vertices[i * 2 + 0] = localSpaceTransform.MultiplyPoint(sample.pos);
                    vertices[i * 2 + 1] = localSpaceTransform.MultiplyPoint(sample.pos - sample.forward * trailWidth);

                    // 一个样本创建两个UV
                    float u = 0.0f;
                    if (i != 0)
                        u = Mathf.Clamp01((Time.timeSinceLevelLoad - sample.sampleTimer) / sampleLife);

                    uv[i * 2 + 0] = new Vector2(u, 0);
                    uv[i * 2 + 1] = new Vector2(u, 1);
                }
            }

            // 创建三角形
            int triangleCount = (_samples.Count - 1) * 2 * (isStab ? 2 : 1);    // 三角形数量
            int trianglesVerticeCount = triangleCount * 3;                      // 三角形顶点数量
            int[] triangles = new int[trianglesVerticeCount];
            int groupCount = triangles.Length / (6 * (isStab ? 2 : 1));
            for (int i = 0; i < groupCount; i++)
            {
                if (isStab)
                {
                    triangles[i * 12 + 0] = i * 3;
                    triangles[i * 12 + 1] = i * 3 + 1;
                    triangles[i * 12 + 2] = i * 3 + 3;

                    triangles[i * 12 + 3] = i * 3;
                    triangles[i * 12 + 4] = i * 3 + 3;
                    triangles[i * 12 + 5] = i * 3 + 2;

                    triangles[i * 12 + 6] = i * 3 + 3;
                    triangles[i * 12 + 7] = i * 3 + 1;
                    triangles[i * 12 + 8] = i * 3 + 4;

                    triangles[i * 12 + 9] = i * 3 + 3;
                    triangles[i * 12 + 10] = i * 3 + 5;
                    triangles[i * 12 + 11] = i * 3 + 1;
                }
                else
                {
                    triangles[i * 6 + 0] = i * 2;
                    triangles[i * 6 + 1] = i * 2 + 1;
                    triangles[i * 6 + 2] = i * 2 + 2;

                    triangles[i * 6 + 3] = i * 2 + 2;
                    triangles[i * 6 + 4] = i * 2 + 1;
                    triangles[i * 6 + 5] = i * 2 + 3;
                }
            }

            MeshTools.Instance.MakeMesh(_meshFilter, vertices, triangles, uv);
        }
    }

    /// <summary>
    /// 拖尾效果管理
    /// </summary>
    /// <mark>
    /// 挂接在表示拖尾的gameobject上，并且需要调整拖尾对象transform的pos为握柄处，且z轴指向武器纵向，x轴指向武器横向        
    /// </mark>>

    [AddComponentMenu("Phenix/Effect/TrailMgr")]
    public class TrailMgr : MonoBehaviour
    {
        // 插值模式
        [System.Serializable]
        public enum InterpolationMode
        {
            LINE = 0,   // 线性插值
            CATMULLROM, // CatmullRom样条插值
            BEZIER,     // 贝塞尔样条插值
        }

        [SerializeField]
        float _smooth = 10;     // 拖尾平滑值

        [SerializeField]
        AnimProxy _animProxy;

        [SerializeField]
        InterpolationMode _interpolationMode;

        [SerializeField]
        List<TrailData> _trailConfigList = new List<TrailData>();             

        private void Start()
        {
            foreach (var item in _trailConfigList)
            {
                item.Init(item.mat, _smooth);
            }
        }

        void LateUpdate()
        {
            foreach (var item in _trailConfigList)
            {
                item.OnUpdate(_animProxy, _interpolationMode);
            }
        }
    }
}