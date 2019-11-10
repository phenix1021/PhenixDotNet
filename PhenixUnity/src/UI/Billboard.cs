using UnityEngine;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 广告牌效果（多用于场景对象头顶HUD，如血条等。确保UI始终对着主相机。注意：头顶HUD的canvas的Render Mode是World Space）
    /// </summary>        
    [AddComponentMenu("Phenix/UI/Billboard")]
    public class Billboard : MonoBehaviour
    {
        public Transform trans;

        private void Update()
        {
            if (trans == null || trans.gameObject.activeInHierarchy == false)
            {
                return;
            }

            Vector3 screenPos = UnityEngine.Camera.main.WorldToScreenPoint(trans.position);
            screenPos.z = 0; // 也可以是其它任何值
            trans.LookAt(UnityEngine.Camera.main.ScreenToWorldPoint(screenPos));
        }
    }
}