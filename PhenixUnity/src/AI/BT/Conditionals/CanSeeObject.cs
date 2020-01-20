using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/CanSeeObject.png")]
    public class CanSeeObject : Conditional<CanSeeObjectImpl> { }

    [System.Serializable]
    public class CanSeeObjectImpl : ConditionalImpl
    {
        [SerializeField]
        protected bool usePhysics2D;        
        [SerializeField]
        protected string targetTag;
        [SerializeField]
        protected LayerMask objectLayerMask;
        [SerializeField]
        protected int maxCollisionCount = 200;
        [SerializeField]
        protected LayerMask ignoreLayerMask;
        [SerializeField]
        protected float fieldOfViewAngle = 90;
        [SerializeField]
        protected float viewDistance = 1000;
        [SerializeField]
        protected Vector3 positionOffset;
        [SerializeField]
        protected Vector3 targetOffset;
        [SerializeField]
        protected float angleOffset2D;
        [SerializeField]
        protected bool useTargetBone;
        [SerializeField]
        protected HumanBodyBones targetBone;
        [SerializeField]
        protected bool disableAgentColliderLayer;

        [SerializeField]
        protected SharedGameObject targetObject;
        [SerializeField]
        protected SharedGameObject[] targetObjects;

        [SerializeField]
        protected SharedGameObject returnedObject;

        Locomotion.CanSeeObject _canSeeObject = new Locomotion.CanSeeObject();

        public override void OnStart()
        {
            base.OnStart();

            _canSeeObject.agent = Transform;

            _canSeeObject.usePhysics2D = usePhysics2D;
            _canSeeObject.targetTag = targetTag;
            _canSeeObject.objectLayerMask = objectLayerMask;
            _canSeeObject.maxCollisionCount = maxCollisionCount;
            _canSeeObject.ignoreLayerMask = ignoreLayerMask;
            _canSeeObject.fieldOfViewAngle = fieldOfViewAngle;
            _canSeeObject.viewDistance = viewDistance;
            _canSeeObject.positionOffset = positionOffset;
            _canSeeObject.targetOffset = targetOffset;
            _canSeeObject.angleOffset2D = angleOffset2D;
            _canSeeObject.useTargetBone = useTargetBone;
            _canSeeObject.targetBone = targetBone;
            _canSeeObject.disableAgentColliderLayer = disableAgentColliderLayer;

            if (targetObject.Value != null)
            {
                _canSeeObject.targetObject = targetObject.Value.transform;
            }

            foreach (var target in targetObjects)
            {
                _canSeeObject.targetObjects.Add(target.Value.transform);
            }

            _canSeeObject.OnStart();
        }
        
        public override bool Check()
        {
            if (_canSeeObject.OnUpdate() == Locomotion.LocomotionStatus.SUCCESS)
            {
                returnedObject.Value = _canSeeObject.returnedObject;
                return true;
            }
            return false;
        }

        public override void OnDrawGizmosSelected()
        {
            _canSeeObject.OnDrawGizmosSelected();   
        }
    }
}