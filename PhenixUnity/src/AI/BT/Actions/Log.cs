using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Log.png")]
    public class Log : Action<LogImpl> { }

    [System.Serializable]
    public class LogImpl : ActionImpl
    {
        public string text;
        public bool logError = false;

        /*public override void OnTurnBegin()
        {
            base.OnTurnBegin();
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
        }*/

        public override TaskStatus Run()
        {
            if (logError)
            {
                Debug.LogError(text);
            }
            else
            {
                Debug.Log(text);
            }
            return TaskStatus.SUCCESS;
        }
    }
}