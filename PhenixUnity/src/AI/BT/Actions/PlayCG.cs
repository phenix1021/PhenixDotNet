using UnityEngine.Playables;

namespace Phenix.Unity.AI.BT
{
    //[TaskIcon("TaskIcons/PlayCG.png")]
    public class PlayCG : Action<PlayCGImpl> { }

    [System.Serializable]
    public class PlayCGImpl : ActionImpl
    {
        public SharedGameObject cg;
        public bool waitUntilPlayDone = true;  // 是否播放完毕才返回（success）

        bool _hasBeenPlayed;        // 已经播放过

        PlayableDirector _playableDirector;

        public override void OnAwake()
        {
            base.OnAwake();
            _playableDirector = cg.Value.GetComponent<PlayableDirector>();
        }

        public override void OnStart()
        {
            base.OnStart();
            if (_hasBeenPlayed == false)
            {
                _playableDirector.Play();
                _hasBeenPlayed = true;
            }            
        }

        public override TaskStatus Run()
        {
            if (_playableDirector.state == PlayState.Playing && waitUntilPlayDone)
            {
                return TaskStatus.RUNNING;
            }

            return TaskStatus.SUCCESS;
        }
    }
}