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
        protected SharedGameObject targetObject;
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
            
            _withinDistance.usePhysics2D = usePhysics2D;
            _withinDistance.targetObject = targetObject.Value;
            _withinDistance.targetTag = targetTag;
            _withinDistance.objectLayerMask = objectLayerMask;
            _withinDistance.magnitude = magnitude;
            _withinDistance.lineOfSight = lineOfSight;
            _withinDistance.ignoreLayerMask = ignoreLayerMask;
            _withinDistance.offset = offset;
            _withinDistance.targetOffset = targetOffset;

            _withinDistance.OnStart();
        }
        
        public override bool Check()
        {            
            if (_withinDistance.OnUpdate() == Locomotion.LocomotionStatus.SUCCESS)
            {
                returnedObject.Value = _withinDistance.returnedObject;
                return true;
            }
            return false;
        }

        // Draw the seeing radius
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Transform == null || magnitude == 0)
            {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(Transform.position, usePhysics2D ?
                Transform.forward : Transform.up, magnitude);
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}