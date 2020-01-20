using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/WithinDistance.png")]
    public class WithinDistance : Conditional<WithinDistanceImpl> { }

    [System.Serializable]
    public class WithinDistanceImpl : ConditionalImpl
    {
        [SerializeField]
        protected bool usePhysics2D;
        [SerializeField]
        protected GameObject targetObject;
        [SerializeField]
        protected string targetTag;
        [SerializeField]
        protected LayerMask objectLayerMask;
        [SerializeField]
        protected float magnitude = 5;
        [SerializeField]
        protected bool lineOfSight;
        [SerializeField]
        protected LayerMask ignoreLayerMask;
        [SerializeField]
        protected Vector3 offset;
        [SerializeField]
        protected Vector3 targetOffset;

        [SerializeField]
        protected SharedGameObject returnedObject;

        Locomotion.WithinDistance _withinDistance = new Locomotion.WithinDistance();

        public override void OnStart()
        {
            base.OnStart();
            /*
            _withinDistance.usePhysics2D = usePhysics2D;
            _withinDistance.usePhysics2D = usePhysics2D;
            _withinDistance.usePhysics2D = usePhysics2D;
            _withinDistance.usePhysics2D = usePhysics2D;
            _withinDistance.usePhysics2D = usePhysics2D;
            _withinDistance.usePhysics2D = usePhysics2D;
            _withinDistance.usePhysics2D = usePhysics2D;
            _withinDistance.usePhysics2D = usePhysics2D;

            if (target.Value != null)
            {
                _follow.target = target.Value.transform;
            }

            _follow.OnStart();*/
        }
        
        public override bool Check()
        {
            return _withinDistance.OnUpdate() == Locomotion.LocomotionStatus.SUCCESS;            
        }
    }
}