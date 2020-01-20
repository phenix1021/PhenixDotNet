using Phenix.Unity.Extend;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/PrioritySelector.png")]
    [TaskDescription("PrioritySelector")]
    public class PrioritySelector : Selector
    {
        //public 自定义class taskParams;

        protected override void OnStart()
        {
            Children.Sort((Task one, Task another) => { return another.GetPriority().CompareTo(one.GetPriority()); });
        }

    }
}