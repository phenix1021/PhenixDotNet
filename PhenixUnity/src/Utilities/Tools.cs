using UnityEngine;

namespace Phenix.Unity.Utilities
{
    public class Tools
    {
        public static string ColorString(string content, Color color)
        {
            return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(color), content);
        }

        public static void PlayAnimation(Animation animEngine, string anim, float fadeInTime, 
            QueueMode queueMode = QueueMode.PlayNow)
        {
            if (animEngine.IsPlaying(anim))
            {
                // 如果自身正在被播放
                animEngine.CrossFadeQueued(anim, fadeInTime, queueMode);
            }
            else
            {
                animEngine.CrossFade(anim, fadeInTime);
            }
        }
    }
}