using UnityEngine;
using System;

namespace Phenix.Unity.AI
{
    [Serializable]
    public class BehaviorTreeReferenceParams
    {
        public string externalBTAssetName;
    }

    [Serializable]
    [TaskIcon("TaskIcons/BehaviorTree.png")]
    public class BehaviorTreeReference : Task
    {
        [HideInInspector]
        public BehaviorTreeReferenceParams taskParams = new BehaviorTreeReferenceParams();

        public BehaviorTreeAsset externalBTAsset;

        //BehaviorTree _externalBT;

        //public BehaviorTree ExternalBT { get { return _externalBT; } set { _externalBT = value; } }
        
        public override TaskStatus Run()
        {
            /*if (_externalBT == null)
            {
                return TaskStatus.Failure;
            }
            return _externalBT.Entry.Tick();*/
            if (externalBTAsset == null || externalBTAsset.BT == null)
            {
                return TaskStatus.Failure;
            }
            return externalBTAsset.BT.Entry.OnUpdate();
        }

        public override void OnAwake()
        {
            if (externalBTAsset == null || externalBTAsset.BT == null)
            {
                return;
            }
            externalBTAsset.BT.OnAwake();
        }

        public override void OnStart()
        {
            if (externalBTAsset == null || externalBTAsset.BT == null)
            {
                return;
            }
            externalBTAsset.BT.OnStart();
        }
    }
}