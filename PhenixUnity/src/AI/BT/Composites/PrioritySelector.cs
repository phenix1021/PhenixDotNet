using Phenix.Unity.Extend;

namespace Phenix.Unity.AI
{
    [TaskIcon("TaskIcons/PrioritySelector.png")]
    [TaskDescription("PrioritySelector")]
    public class PrioritySelector : Selector
    {
        //public 自定义class taskParams;

        protected override void OnTurnBegin()
        {
            Children.Sort((Task one, Task another) => { return another.GetPriority().CompareTo(one.GetPriority()); });
        }

    }
}