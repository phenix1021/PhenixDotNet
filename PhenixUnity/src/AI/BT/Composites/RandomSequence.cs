using Phenix.Unity.Extend;

namespace Phenix.Unity.AI
{
    [TaskIcon("TaskIcons/RandomSequence.png")]
    [TaskDescription("RandomSequence")]
    public class RandomSequence : Sequence
    {
        //public 自定义class taskParams;

        protected override void OnTurnBegin()
        {
            Children.Shuffle();
        }
    }
}