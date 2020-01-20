using Phenix.Unity.Extend;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/RandomSelector.png")]
    [TaskDescription("RandomSelector")]
    public class RandomSelector : Selector
    {
        //public 自定义class taskParams;

        protected override void OnStart()
        {
            Children.Shuffle();
        }

    }
}