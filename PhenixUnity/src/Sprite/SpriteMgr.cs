using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Pattern;

namespace Phenix.Unity.Sprite
{
    public class SpriteMgr : Singleton<SpriteMgr>
    {
        [SerializeField]
        List<SpriteTemplate> _templates = new List<SpriteTemplate>();

        List<SpriteBase> _sprites = new List<SpriteBase>();
        List<int> _expireList = new List<int>();

        public SpriteTemplate GetTP(int spriteCode)
        {
            foreach (var item in _templates)
            {
                if (item.spriteCode == spriteCode)
                {
                    return item;
                }
            }
            return null;
        }

        private void Update()
        {
            for (int i = 0; i < _sprites.Count; ++i)
            {
                SpriteBase sprite = _sprites[i];
                if (sprite.IsExpired())
                {
                    _expireList.Add(i);
                    sprite.Release();
                }
                else
                {
                    _sprites[i].OnUpdate();
                }
            }

            foreach (var idx in _expireList)
            {
                _sprites.RemoveAt(idx);
            }

            _expireList.Clear();
        }

        public void Add(SpriteBase sprite/*, Vector3 pos, Vector3 dir*/)
        {
            /*SpriteObject spriteObj = new SpriteObject();
            spriteObj.quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            spriteObj.quad.GetComponent<MeshCollider>().enabled = false;
            spriteObj.quad.GetComponent<MeshRenderer>().material = spriteEffect.materials[Random.Range(0, spriteEffect.materials.Length)];
            spriteObj.quad.transform.localPosition = pos;
            spriteObj.quad.transform.localEulerAngles = dir;
            spriteObj.effect = spriteEffect;*/
            _sprites.Add(sprite);
        }
    }
}
