using UnityEngine;

namespace Phenix.Unity.AI
{
    public class SimpleAct
    {
        public static void PlayAction(Animation anim, string animName, float fadeInTime)
        {            
            if (anim.IsPlaying(animName))
                anim.CrossFadeQueued(animName, fadeInTime, QueueMode.PlayNow);
            else
                anim.CrossFade(animName, fadeInTime);
        }

        public static bool Move(Transform trans, CharacterController cc, Vector3 velocity, bool slide /*= true*/ )
        {
            Vector3 old = trans.position;

            trans.position += Vector3.up * Time.deltaTime;

            velocity.y -= 9 * Time.deltaTime;
            CollisionFlags flags = cc.Move(velocity);

            //Debug.Log("move " + flags.ToString());

            if (slide == false && (flags & CollisionFlags.Sides) != 0)
            {
                trans.position = old;
                return false;
            }

            if ((flags & CollisionFlags.Below) == 0)
            {
                trans.position = old;
                return false;
            }

            return true;
        }

        public static bool MoveEx(Transform trans, CharacterController cc, Vector3 velocity)
        {
            Vector3 old = trans.position;
            trans.position += Vector3.up * Time.deltaTime;
            velocity.y -= 9 * Time.deltaTime;
            CollisionFlags flags = cc.Move(velocity);
            if (flags == CollisionFlags.None)
            {
                RaycastHit hit;
                if (Physics.Raycast(trans.position, -Vector3.up, out hit, 3) == false)
                {
                    trans.position = old;
                    return false;
                }
            }

            return true;
        }
    }
}