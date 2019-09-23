using UnityEngine;

namespace Phenix.Unity.Sprite
{
    [System.Serializable]
    public sealed class SpriteTemplate
    {
        public int spriteCode;              // 类型
        public float lifeTime = 0;          // 存在时长（秒），0表示持久
        public Material[] materials;        // 材质（使用时任选其一）        
    }

    [System.Serializable]
    public abstract class SpriteBase
    {
        protected SpriteTemplate TP { get; private set; }
        protected float PassTime { get; private set; }       // 已经过时间（秒）
        protected GameObject QuadObject { get; private set; }      // 面片

        protected SpriteBase()
        {
            QuadObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            QuadObject.GetComponent<MeshCollider>().enabled = false;
            Reset();
        }

        protected void Init(int spriteCode, Vector3 pos, Vector3 dir)
        {
            TP = SpriteMgr.Instance.GetTP(spriteCode);
            QuadObject.GetComponent<MeshRenderer>().material = TP.materials[Random.Range(0, TP.materials.Length)];
            QuadObject.transform.localPosition = pos;
            QuadObject.transform.localEulerAngles = dir;
            Show();
        }

        public static void Reset(SpriteBase sprite)
        {
            sprite.Reset();
        }

        protected virtual void Reset()
        {
            PassTime = 0;
        }

        public virtual bool IsExpired()
        {
            return TP != null && TP.lifeTime > 0 && PassTime >= TP.lifeTime;
        }

        public virtual void OnUpdate()
        {
            PassTime += Time.deltaTime;
        }

        public void Hide()
        {
            QuadObject.SetActive(false);
        }

        public void Show()
        {
            QuadObject.SetActive(true);
        }

        public virtual void Release() { }
    }
}