using UnityEngine.Playables;

namespace Phenix.Unity.AI.BT
{
    //[TaskIcon("TaskIcons/StopCG.png")]
    public class StopCG : Action<StopCGImpl> { }

    [System.Serializable]
    public class StopCGImpl : ActionImpl
    {
        public SharedGameObject cg;                

        PlayableDirector _playableDirector;

        public override void OnAwake()
        {
            base.OnAwake();
            _playableDirector = cg.Value.GetComponent<PlayableDirector>();
        }

        public override void OnStart()
        {
            base.OnStart();
            _playableDirector.Stop();
        }

        public override TaskStatus Run()
        {
            return TaskStatus.SUCCESS;
        }
    }
}