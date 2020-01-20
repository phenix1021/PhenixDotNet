using UnityEngine;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace Phenix.Unity.AI.BT
{
    /// <summary>
    /// 描述：行为树数据文件
    /// 备注：ScriptableObject的asset文件和JsonUtility的序列化都不支持ScriptableObject对象，并且
    ///       对非ScriptableObject类型的对象，如果包含递归（如树节点），两者最多只支持七层。.net
    ///       自带的xml序列化工具则完全不支持递归。所以这里使用了纯xml和json结合的方式进行序列化
    ///       和反序列化：树结构部分使用xml，而bt和task的可编辑属性用JsonUtility的json。
    ///       unity的inspector只支持UnityEngine.Object对象，为了能直接使用unity的既成方案，这里将
    ///       bt和task都处理为ScriptableObject类型。
    /// </summary>
    [SerializeField]
    public class BehaviorTreeAsset : ScriptableObject
    {        
        BehaviorTree _bt;

        [HideInInspector]
        public float zoom = 1.0f;

        [SerializeField, HideInInspector]        
        string _serializedXMLText;
                
        bool _dirty = false;

        public BehaviorTree BT { get { return _bt; } }
        public string SerializedXMLText { get { return _serializedXMLText; } }
        //public bool Dirty { get { return _dirty; } set { _dirty = value; } }
        public bool Dirty
        {
            get
            {
                if (Playing)
                {
                    return false;
                }
                return _dirty;
            }

            set
            {
                if (Playing == false)
                {
                    _dirty = value;
                }
            }
        }

        public bool Playing { get { return string.IsNullOrEmpty(MonitorName) == false || MonitorAsset != null; } }
        public string MonitorName { get; set; }
        public BehaviorTreeAsset MonitorAsset { get; set; }

        public static readonly string prefixGlobalBlackboard = "global:";

        private void OnEnable()
        {
            Debug.Log("OnEnable", this);
            /*if (string.IsNullOrEmpty(_serializedXMLText))
            {
                Init();         // 新建文件时触发
                Serialize();
            }
            else
            {                
                Deserialize(); // game运行时、首次加载时触发
            }*/
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable", this);
        }

        void Init()
        {
            _bt = ScriptableObject.CreateInstance<BehaviorTree>();
            _bt.Entry = _bt.CreateTask<EntryTask>();
            Serialize();
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy", this);
            DestroyImmediate(_bt);           
        }

        public static BehaviorTreeAsset CreateBehaviorTreeAsset()
        {
            BehaviorTreeAsset asset = ScriptableObject.CreateInstance<BehaviorTreeAsset>();
            asset.Init();            
            return asset;
        }

        /// <summary>
        /// [摘要]：序列化BehaviorTree对象到内部XML字符串
        /// </summary>        
        public string Serialize(bool setSerializedXMLText = true)
        {
            if (_bt == null || _bt.Entry == null)
            {
                return string.Empty;
            }
            XmlDocument doc = new XmlDocument();            
            XmlElement btEle = doc.CreateElement("BehaviorTree");
            doc.AppendChild(btEle);
            // 处理bt属性和blackboard
            _bt.btParams.shareVariableNames = _bt.Blackboard.Keys;
            btEle.SetAttribute("BTParams", JsonUtility.ToJson(_bt.btParams));            

            XmlElement trunkEle = doc.CreateElement("Trunk");
            btEle.AppendChild(trunkEle);
            XmlElement apartBranchesEle = doc.CreateElement("ApartBranches");
            btEle.AppendChild(apartBranchesEle);
            //XmlElement blackboardEle = doc.CreateElement("Blackboard");
            //btEle.AppendChild(blackboardEle);

            // 处理trunk
            Serialize(_bt.Entry, trunkEle);
            // 处理apartBranches
            for (int i = 0; i < _bt.Tasks.Count; i++)
            {
                if (_bt.Tasks[i].Parent == null && (_bt.Tasks[i] is EntryTask) == false)
                {
                    Serialize(_bt.Tasks[i], apartBranchesEle);
                }
            }
            if (setSerializedXMLText)
            {
                _serializedXMLText = doc.InnerXml;
            }
            return doc.InnerXml;            
        }

        void Serialize(Task taskObj, XmlElement parentEL, string name = "")
        {
            XmlElement ele;
            ele = parentEL.OwnerDocument.CreateElement("Task");
            ele.SetAttribute("ClassName", taskObj.GetType().FullName);
            parentEL.AppendChild(ele);

            HandleFields(ele, taskObj);

            if (taskObj is ParentTask)
            {
                foreach (var child in (taskObj as ParentTask).Children)
                {
                    Serialize(child, ele);
                };
            }
        }

        void HandleFields(XmlElement ele, Task taskObj)
        {
            HandleNodeDataSerialization(ele, taskObj);            
            HandleTaskParamsSerialization(ele, taskObj);                       
        }

        void HandleNodeDataSerialization(XmlElement ele, Task taskObj)
        {
            // 处理nodeData
            FieldInfo nodeDataInfo = taskObj.GetType().GetField("nodeData", BindingFlags.Public | BindingFlags.Instance);
            if (nodeDataInfo == null)
            {
                Debug.LogError("fail to find the public field 'nodeData'.");
            }
            else
            {
                ele.SetAttribute("NodeData", JsonUtility.ToJson(nodeDataInfo.GetValue(taskObj)));
            }
        }

        void HandleTaskParamsSerialization(XmlElement ele, Task taskObj)
        {
            //HandleExternalBT(taskObj);

            // 处理taskParams（允许有些Task没有taskParams字段）
            FieldInfo paramsInfo = taskObj.GetType().GetField("taskParams", BindingFlags.Public | BindingFlags.Instance);
            if (paramsInfo != null)
            {                
                object taskParamsObj = paramsInfo.GetValue(taskObj);
                List<FieldInfo> paramsList = new List<FieldInfo>(taskParamsObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance));
                foreach (var param in taskParamsObj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    SerializeField[] attrs = (SerializeField[])param.GetCustomAttributes(typeof(SerializeField), true);
                    if (attrs.Length > 0)
                    {
                        paramsList.Add(param);
                    }
                }
                foreach (var param in paramsList)
                {
                    object paramObj = param.GetValue(taskParamsObj);
                    //HandleShareVariableSerialization(paramObj);
                    HandleSharedVariableSerialization(paramObj);
                }

                ele.SetAttribute("TaskParams", JsonUtility.ToJson(paramsInfo.GetValue(taskObj)));
            }
        }

        /*void HandleExternalBT(object taskObj)
        {
            if (taskObj is CallExternalBehaviorTree == false)
            {
                return;
            }
            CallExternalBehaviorTree callExternalBT = taskObj as CallExternalBehaviorTree;
            if (callExternalBT.externalBTAsset == null)
            {
                return;
            }
            callExternalBT.taskParams.externalBTAssetName = callExternalBT.externalBTAsset.nam
        }*/

        /*void HandleShareVariableSerialization(object paramObj)
        {
            // 处理TaskParams中的ShareVariable成员，如果其_name不在本地黑板/全局黑板之列，则设置为空
            if (paramObj == null)
            {
                return;
            }
            if (paramObj.GetType().IsSubclassOf(typeof(ShareVariable)))
            {
                PropertyInfo nameProp = paramObj.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                string nameVal = nameProp.GetValue(paramObj, null) as string;
                if (_bt.Blackboard.Keys.Contains(nameVal) == false)
                {
                    if (BehaviorTree.globalBlackboard.Keys.Contains(nameVal) == false)
                    {
                        nameProp.SetValue(paramObj, string.Empty, null);
                    }
                }
            }
        }*/

        void HandleSharedVariableSerialization(object paramObj)
        {
            // 处理TaskParams中的SharedVariable成员，如果其_name不在本地黑板/全局黑板之列，则设置为空
            if (paramObj == null)
            {
                return;
            }
            if (paramObj.GetType().IsSubclassOf(typeof(SharedVariable)))
            {
                MethodInfo dynamicMethod = paramObj.GetType().GetMethod("IsDynamic", BindingFlags.Public | BindingFlags.Instance);
                if ((bool)dynamicMethod.Invoke(paramObj, null))
                {
                    PropertyInfo nameProp = paramObj.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                    string nameVal = nameProp.GetValue(paramObj, null) as string;
                    if (_bt.Blackboard.Keys.Contains(nameVal) == false)
                    {
                        if (BehaviorTree.globalBlackboard.Keys.Contains(nameVal) == false)
                        {
                            nameProp.SetValue(paramObj, string.Empty, null);
                        }
                    }
                }                
            }
        }

        /// <summary>
        /// [摘要]：从内部XML字符串反序列化BehaviorTree对象        
        /// </summary>        
        public bool Deserialize(Transform transform = null)
        {
            return Deserialize(_serializedXMLText, transform);
        }

        /// <summary>
        /// [摘要]：从内部XML字符串反序列化BehaviorTree对象        
        /// </summary>        
        public bool Deserialize(string xml, Transform transform = null)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (System.Xml.XmlException)
            {
                return false;
            }            
            
            _bt = ScriptableObject.CreateInstance<BehaviorTree>();
            _bt.Transform = transform; // 编辑期可为null

            // 处理bt属性和blackboard
            XmlElement btEle = doc.GetElementsByTagName("BehaviorTree")[0] as XmlElement;
            _bt.btParams = JsonUtility.FromJson<BehaviorTreeParams>(btEle.GetAttribute("BTParams"));                    

            /*XmlElement blackboardEle = doc.GetElementsByTagName("Blackboard")[0] as XmlElement;
            List<string> keys = JsonUtility.FromJson<List<string>>(blackboardEle.GetAttribute("Keys"));
            foreach (var key in keys)
            {
                _bt.Blackboard.Set(key, null);
            }*/

            // 处理trunk
            XmlElement entryEle = doc.GetElementsByTagName("Trunk")[0].FirstChild as XmlElement;
            try
            {
                _bt.Entry = Deserialize(entryEle, _bt) as EntryTask;
            }
            catch (System.ArgumentException)
            {
                return false;
            }

            // 处理apartBranches
            XmlElement apartBranchesEle = (XmlElement)doc.GetElementsByTagName("ApartBranches").Item(0);
            foreach (XmlElement apartBranch in apartBranchesEle.ChildNodes)
            {
                try
                {
                    Deserialize(apartBranch, _bt);
                }
                catch (System.ArgumentException)
                {
                    return false;
                }                
            }

            /*if (Dirty)
            {
                // 比如反序列化时因为全局黑板条目的删减修改了Shared变量的name值
                Serialize();
                Dirty = false;
            }*/
            return true;
        }

        /// <summary>        
        /// [异常]：ArgumentException
        /// </summary>        
        Task Deserialize(XmlElement element, BehaviorTree bt)
        {            
            if (element.Name != "Task")
            {
                Debug.LogError("wrong data format");
                return null;                
            }

            // 创建task
            string className = element.GetAttribute("ClassName");
            Task taskObj = bt.CreateTask(className);
            if (taskObj == null)
            {
                Debug.LogError(string.Format("fail to create task instance for '{0}'.", className));
                return null;
            }

            // 尝试获取task对应的taskParam元数据
            FieldInfo taskParamsInfo = taskObj.GetType().GetField("taskParams", BindingFlags.Public | BindingFlags.Instance);
            if (taskParamsInfo != null)
            {
                // 获取xml节点属性的TaskParams内容
                string taskParamsVal = element.GetAttribute("TaskParams");
                if (string.IsNullOrEmpty(taskParamsVal))
                {
                    Debug.LogError("invalid TaskParams.");                    
                }
                else
                {
                    // 反序列化TaskParams
                    System.Type type = taskParamsInfo.GetValue(taskObj).GetType(); // 注意这里不能用paramsInfo.FieldType，否则只会得到基类的Type          
                    object taskParamsObj = JsonUtility.FromJson(taskParamsVal, type);
                    //HandleShareVariableDeserialization(taskParamsObj, bt);
                    HandleSharedVariableDeserialization(taskParamsObj, bt);
                    taskParamsInfo.SetValue(taskObj, taskParamsObj);
                    HandleExternalBTDeserialization(taskObj); // 注意：要放在taskParamsInfo.SetValue(taskObj, taskParamsObj)后调用
                }
            }

            // 获取task对应的nodeData元数据
            FieldInfo nodeDataInfo = taskObj.GetType().GetField("nodeData", BindingFlags.Public | BindingFlags.Instance);
            if (nodeDataInfo != null)
            {
                // 获取xml节点属性的nodeData内容
                string nodeDataVal = element.GetAttribute("NodeData");
                if (string.IsNullOrEmpty(nodeDataVal))
                {
                    Debug.LogError("invalid NodeData.");
                }
                else
                {
                    // 反序列化NodeData
                    System.Type type = nodeDataInfo.FieldType;       
                    object nodeDataObj = JsonUtility.FromJson(nodeDataVal, type);
                    nodeDataInfo.SetValue(taskObj, nodeDataObj);
                }
            }
            else
            {
                Debug.LogError(string.Format("fail to get the nodeData field in {0}", taskObj.GetType()));
            }
            
            // 递归处理子节点
            foreach (XmlElement subEle in element.ChildNodes)
            {
                if (subEle.Name == "Task")
                {
                    (taskObj as ParentTask).AddChild(Deserialize(subEle, bt) as Task);
                    continue;
                }
                else
                {
                    Debug.LogError("wrong data format");
                }                
            }

            return taskObj;
        }

        /*void HandleShareVariableDeserialization(object paramsObj, BehaviorTree bt)
        {
            if (Playing == false)
            {
                return;
            }
            // 处理TaskParams中的ShareVariable成员
            List<FieldInfo> paramsList = new List<FieldInfo>(paramsObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance));
            foreach (var param in paramsObj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                SerializeField[] attrs = (SerializeField[])param.GetCustomAttributes(typeof(SerializeField), true);
                if (attrs.Length > 0)
                {
                    paramsList.Add(param);
                }
            }
            foreach (var param in paramsList)
            {
                if (param.FieldType.IsSubclassOf(typeof(ShareVariable)))
                {
                    object shareVariableObj = param.GetValue(paramsObj);
                    PropertyInfo nameProp = shareVariableObj.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo blackboardProp = shareVariableObj.GetType().GetProperty("Blackboard", BindingFlags.NonPublic | BindingFlags.Instance);

                    string nameVal = nameProp.GetValue(shareVariableObj, null) as string;
                    if (nameVal.StartsWith(prefixGlobalBlackboard))
                    {
                        // 给sv对象设置全局黑板
                        blackboardProp.SetValue(shareVariableObj, BehaviorTree.globalBlackboard, null);
                        // 添加运行时全局黑板数据
                        BehaviorTree.globalBlackboard.Set(nameVal, null);
                    }
                    else
                    {
                        // 给sv对象设置本地黑板
                        blackboardProp.SetValue(shareVariableObj, bt.Blackboard, null);
                        // 添加运行时本地黑板数据
                        bt.Blackboard.Set(nameVal, null);
                    }
                }
            }
        }*/

        void HandleSharedVariableDeserialization(object paramsObj, BehaviorTree bt)
        {
            // 搜集TaskParams中可序列化的成员
            List<FieldInfo> paramsList = new List<FieldInfo>(paramsObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance));
            foreach (var param in paramsObj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                SerializeField[] attrs = (SerializeField[])param.GetCustomAttributes(typeof(SerializeField), true);
                if (attrs.Length > 0)
                {
                    paramsList.Add(param);
                }
            }

            // 遍历搜集后的序列化成员
            foreach (var param in paramsList)
            {                
                if (param.FieldType.IsArray)
                {
                    // 目前对于SharedVariable的容器暂时只支持数组，且不能嵌套
                    foreach (object elementObj in param.GetValue(paramsObj) as Array)
                    {
                        // 遍历数组
                        if (elementObj.GetType().IsSubclassOf(typeof(SharedVariable)))
                        {                            
                            HandleSharedVariableDeserializationImpl(elementObj, bt);
                        }
                    }
                }
                else if (param.FieldType.IsSubclassOf(typeof(SharedVariable)))
                {
                    object shareVariableObj = param.GetValue(paramsObj);
                    HandleSharedVariableDeserializationImpl(shareVariableObj, bt);
                }
            }
        }

        void HandleSharedVariableDeserializationImpl(object shareVariableObj, BehaviorTree bt)
        {
            PropertyInfo nameProp = shareVariableObj.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo blackboardProp = shareVariableObj.GetType().GetProperty("Blackboard", BindingFlags.NonPublic | BindingFlags.Instance);

            MethodInfo dynamicMethod = shareVariableObj.GetType().GetMethod("IsDynamic", BindingFlags.Public | BindingFlags.Instance);
            if ((bool)dynamicMethod.Invoke(shareVariableObj, null) == false)
            {
                // 如果是静态变量
                PropertyInfo valProp = shareVariableObj.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                HandleStaticSharedGameObject(valProp, nameProp, shareVariableObj);
                return;
            }

            // 以下针对动态变量                   

            string nameVal = nameProp.GetValue(shareVariableObj, null) as string;

            if (nameVal.StartsWith(prefixGlobalBlackboard))
            {
                if (Playing)
                {
                    // 给sv对象设置全局黑板
                    blackboardProp.SetValue(shareVariableObj, BehaviorTree.globalBlackboard, null);
                    // 添加运行时全局黑板数据
                    BehaviorTree.globalBlackboard.Set(nameVal, null);
                }
                else if (BehaviorTree.globalBlackboard.Keys.Contains(nameVal) == false)
                {
                    // 非运行时
                    nameProp.SetValue(shareVariableObj, string.Empty, null);
                    Dirty = true;
                }
            }
            else
            {
                if (Playing)
                {
                    // 给sv对象设置本地黑板
                    blackboardProp.SetValue(shareVariableObj, bt.Blackboard, null);
                    // 添加运行时本地黑板数据
                    bt.Blackboard.Set(nameVal, null);
                }
                else if (_bt.Blackboard.Keys.Contains(nameVal) == false)
                {
                    // 非运行时
                    nameProp.SetValue(shareVariableObj, string.Empty, null);
                    Dirty = true;
                }
            }
        }

        void HandleStaticSharedGameObject(PropertyInfo valProp, PropertyInfo nameProp, object shareVariableObj)
        {            
            if (shareVariableObj.GetType() != typeof(SharedGameObject))
            {
                return;
            }

            GameObject go = null;
            string nameVal = nameProp.GetValue(shareVariableObj, null) as string;
            if (nameVal.StartsWith("0:"))
            {
                // 参数name以“0:”开头约定为场景对象
                go = GameObject.Find(nameVal.Substring(2));
            }
            else if (nameVal.StartsWith("1:"))
            {
                // 参数name以“1:”开头约定为prefab
                go = Resources.Load(nameVal.Substring(2)) as GameObject;
            }
            else if (string.IsNullOrEmpty(nameVal) == false)
            {
                Debug.LogError("invalid name in SharedGameObject, must start with '0:' or '1:'!");                
            }
            
            valProp.SetValue(shareVariableObj, go, null);
        }

        void HandleExternalBTDeserialization(Task taskObj)
        {            
            if (taskObj is BehaviorTreeReference == false)
            {
                return;
            }
            BehaviorTreeReference callExternalBehaviorTree = taskObj as BehaviorTreeReference;
            string externalBTAssetName = callExternalBehaviorTree.taskParams.externalBTAssetName;
            if (string.IsNullOrEmpty(externalBTAssetName))
            {
                return;
            }
            
            callExternalBehaviorTree.externalBTAsset = Resources.Load<BehaviorTreeAsset>("BTAssets/" + System.IO.Path.GetFileNameWithoutExtension(externalBTAssetName));
            if (callExternalBehaviorTree.externalBTAsset == null)
            {
                Debug.LogError(string.Format("fail to find the asset file '{0}'", externalBTAssetName));
                return;
            }
            if (Playing)
            {
                BehaviorTreeAsset tmp = ScriptableObject.Instantiate(callExternalBehaviorTree.externalBTAsset);
                //if (Application.isEditor)
                //{
                    tmp.MonitorAsset = callExternalBehaviorTree.externalBTAsset;
                //}                
                if (tmp.Deserialize(taskObj.BT.Transform) == false)
                {
                    Debug.LogError(string.Format("externalBTAsset {0} deserialization failure", externalBTAssetName));
                    return;
                }
                callExternalBehaviorTree.externalBTAsset = tmp;
            }
        }
    }
}

