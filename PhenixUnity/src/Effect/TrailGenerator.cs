//using UnityEngine;
//using System.Collections.Generic;
//using Phenix.Unity.Collection;

//namespace Phenix.Unity.Effect
//{
//    /// <summary>
//    /// 采样数据
//    /// </summary>
//    public class SampleData
//    {
//        public static Pool<SampleData> pool = new Pool<SampleData>(50, Reset);        

//        public Vector3 pos = Vector3.zero;
//        public Vector3 sideDir = Vector3.zero;      // 边指向，用于生成正对面的顶点
//        public float sampleTimer = 0;               // 采样时间

//        public void Reset()
//        {
//            pos = Vector3.zero;
//            sideDir = Vector3.zero;
//            sampleTimer = 0;
//        }

//        public static void Reset(SampleData data)
//        {
//            data.Reset();
//        }
//    }

//    /// <summary>
//    /// 拖尾效果生成器
//    /// </summary>
//    /// <mark>
//    /// 挂接在表示拖尾的gameobject上，并且需要调整拖尾对象transform的pos为握柄处，且z轴指向武器纵向，x轴指向武器横向        
//    /// </mark>>
//    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
//    [AddComponentMenu("Phenix/Effect/TrailGenerator")]
//    public class TrailGenerator : MonoBehaviour
//    {
//        //[SerializeField]
//        //Animation _anim;

//        [SerializeField]
//        float _sampleInterval = 0.02f;     // 采样间隔（秒）

//        public float sampleLife = 0.2f;         // 横向攻击（如抡、斩）时样本存在时长（秒）
//        public float sampleLifeStab = 0.1f;     // 纵向攻击（如刺）时样本存在时长（秒）

//        MeshFilter _meshFilter;
//        MeshRenderer _meshRenderer;

//        List<SampleData> _samples = new List<SampleData>();

//        bool _sampling = false;             // 是否正在采样
//        int _sampleStartFrameCount = 0;     // 开始采样时的帧ID
//        bool _isStab = false;               // 是否刺（纵向攻击）
//        float _sampleLife = 0;              // 样本存在时长（秒）

//        public float weaponLength = 0;      // 武器刀刃长度
//        public float weaponWidth = 0;       // 武器刀刃宽度

//        Vector3 _baseTrailPos;              // trail起始位置

//        Vector3 _lastTransformPos = Vector3.zero;
//        //Vector3 _lastTransformAngle = Vector3.zero;

//        [SerializeField]
//        float _minSampleDistance = 0.1f;

//        void Awake()
//        {
//            _meshFilter = GetComponent<MeshFilter>();
//            _meshRenderer = GetComponent<MeshRenderer>();
//            _baseTrailPos = transform.position;
//        }        
        
//        void LateUpdate()
//        {
//            if (_sampling)
//            {
//                if (_sampleStartFrameCount == Time.frameCount)
//                {
//                    // 跳过调用Play的帧，从下一帧开始采样
//                    return;
//                }

//                Vector3 bakPos = transform.position;
//                Vector3 bakEulerAngle = transform.eulerAngles;

//                float progress = 0;
//                while (progress < Time.deltaTime)
//                {
//                    // 根据采样间隔时间帧间采样
//                    Sample(progress / Time.deltaTime, Time.timeSinceLevelLoad - Time.deltaTime + progress);
//                    progress += _sampleInterval;
//                }

//                _lastTransformPos = transform.position = bakPos;
//                //_lastTransformAngle = transform.eulerAngles = bakEulerAngle;
//            }            

//            BuildTrailMesh();
//        }

//        void Sample(float progress, float timer)
//        {
//            /*transform.eulerAngles = new Vector3(Mathf.LerpAngle(_lastTransformAngle.x, transform.eulerAngles.x, progress), 
//                Mathf.LerpAngle(_lastTransformAngle.y, transform.eulerAngles.y, progress), 
//                Mathf.LerpAngle(_lastTransformAngle.z, transform.eulerAngles.z, progress));
//                */
//            //transform.position = Vector3.Lerp(_lastTransformPos, transform.position, progress);            
//            Vector3 pos = Vector3.Lerp(_lastTransformPos, transform.position, progress);

//            if (_samples.Count > 0 && (_samples[0].pos - pos/*transform.position*/).sqrMagnitude < _minSampleDistance * _minSampleDistance)
//            {
//                // 防止采样点过于紧密
//                return;
//            }

//            //if (_anim != null)
//            //{
//                //GetComponentInParent<Animation>().Sample();//貌似要不要这行没差别
//            //}

//            // 创建样本数据
//            SampleData newSample = SampleData.pool.Get();
//            newSample.pos = pos;// transform.position;
//            newSample.sampleTimer = timer;
//            if (_isStab == false)
//            {
//                // 横向攻击，如抡、斩                
//                newSample.sideDir = transform.forward;
//            }
//            else
//            {
//                // 纵向攻击，如刺                
//                newSample.sideDir = transform.right;                
//            }

//            // 插入样本
//            _samples.Insert(0, newSample);            
//        }
        
//        void BuildTrailMesh()
//        {
//            UnityEngine.Mesh mesh = _meshFilter.mesh;
//            mesh.Clear();

//            // 移除超期样本
//            while (_samples.Count > 0 && Time.timeSinceLevelLoad > _samples[_samples.Count - 1].sampleTimer + _sampleLife)
//            {
//                SampleData.pool.Collect(_samples[_samples.Count - 1]);
//                _samples.RemoveAt(_samples.Count - 1);
//            }

//            if (_samples.Count <= 1)
//            {
//                return;
//            }

//            Vector3[] vertices = new Vector3[_samples.Count * (_isStab ? 3 : 2)];
//            Vector2[] uv = new Vector2[_samples.Count * (_isStab ? 3 : 2)];

//            // Use matrix instead of transform.TransformPoint for performance reasons
//            Matrix4x4 localSpaceTransform = transform.worldToLocalMatrix;

//            // 创建顶点、UV
//            for (var i = 0; i < _samples.Count; i++)
//            {
//                SampleData sample = _samples[i];

//                if (_isStab)
//                {
//                    // 一个样本创建三个顶点
//                    vertices[i * 3 + 0] = localSpaceTransform.MultiplyPoint(sample.pos);
//                    vertices[i * 3 + 1] = localSpaceTransform.MultiplyPoint(sample.pos + sample.sideDir * weaponWidth);
//                    vertices[i * 3 + 2] = localSpaceTransform.MultiplyPoint(sample.pos - sample.sideDir * weaponWidth);

//                    // 一个样本创建三个UV
//                    float u = 0.0f;
//                    if (i != 0)
//                        u = Mathf.Clamp01((Time.timeSinceLevelLoad - sample.sampleTimer) / _sampleLife);

//                    uv[i * 3 + 0] = new Vector2(u, 0);
//                    uv[i * 3 + 1] = new Vector2(u, 1);
//                    uv[i * 3 + 2] = new Vector2(u, 1);
//                }
//                else
//                {
//                    // 一个样本创建两个顶点
//                    vertices[i * 2 + 0] = localSpaceTransform.MultiplyPoint(sample.pos);
//                    vertices[i * 2 + 1] = localSpaceTransform.MultiplyPoint(sample.pos + sample.sideDir * weaponLength);

//                    // 一个样本创建两个UV
//                    float u = 0.0f;
//                    if (i != 0)
//                        u = Mathf.Clamp01((Time.timeSinceLevelLoad - sample.sampleTimer) / _sampleLife);

//                    uv[i * 2 + 0] = new Vector2(u, 0);
//                    uv[i * 2 + 1] = new Vector2(u, 1);
//                }
//            }

//            // 创建三角形
//            int triangleCount = (_samples.Count - 1) * 2 * (_isStab ? 2 :1);    // 三角形数量
//            int trianglesVerticeCount = triangleCount * 3;                      // 三角形顶点数量
//            int[] triangles = new int[trianglesVerticeCount];
//            int groupCount = triangles.Length / (6 * (_isStab ? 2 : 1));
//            for (int i = 0; i < groupCount; i++)
//            {
//                if (_isStab)
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

//            mesh.vertices = vertices;
//            mesh.uv = uv;
//            mesh.triangles = triangles;
//        }

//        public void Play(bool isStab)
//        {
//            if (_sampling)
//            {
//                return;
//            }

//            _isStab = isStab;
//            if (_isStab)
//            {
//                transform.position += transform.forward * weaponLength;  // 将拖尾对象transform移动到尖锋位置
//                _sampleLife = sampleLifeStab;
//            }
//            else
//            {
//                _sampleLife = sampleLife;
//            }

//            _lastTransformPos = transform.position;
//            //_lastTransformAngle = transform.eulerAngles;

//            _sampling = true;
//            _sampleStartFrameCount = Time.frameCount;
//        }

//        public void Stop()
//        {
//            if (_sampling == false)
//            {
//                return;
//            }
//            _sampling = false;
//            transform.position = _baseTrailPos; // 恢复拖尾对象基本位置
//        }
//    }
//}