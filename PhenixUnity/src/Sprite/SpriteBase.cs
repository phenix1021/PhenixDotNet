using UnityEngine;

namespace Phenix.Unity.Sprite
{
    [System.Serializable]
    public abstract class SpriteBase
    {
        int _spriteCode;              // 类型
        float _lifeTime = 0;          // 存在时长（秒），0表示持久
        //Material _mat;                // 面片材质

        protected float PassTime { get; private set; }       // 已经过时间（秒）
        protected GameObject Quad { get; private set; }      // 面片

        protected SpriteBase()
        {
            Quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Quad.GetComponent<MeshCollider>().enabled = false;
            Reset();
        }

        protected void Init(int spriteCode, float lifeTime, Material mat, Vector3 pos, Vector3 dir,
            Transform parent = null)
        {
            _spriteCode = spriteCode;
            _lifeTime = lifeTime;
            Quad.GetComponent<MeshRenderer>().material = mat;
            Quad.transform.SetParent(parent);
            Quad.transform.position = pos;
            Quad.transform.eulerAngles = dir;            
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
            return _lifeTime > 0 && PassTime >= _lifeTime;
        }

        public virtual void OnUpdate()
        {
            PassTime += Time.deltaTime;
        }

        public void Hide()
        {
            Quad.SetActive(false);
        }

        public void Show()
        {
            Quad.SetActive(true);
        }

        public virtual void Release() { }
    }
}