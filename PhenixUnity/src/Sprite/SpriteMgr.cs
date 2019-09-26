using System.Collections.Generic;

namespace Phenix.Unity.Sprite
{
    public class SpriteMgr
    {        
        List<SpriteBase> _sprites = new List<SpriteBase>();
        List<int> _expireList = new List<int>();

        public void OnUpdate()
        {
            for (int i = 0; i < _sprites.Count; ++i)
            {
                SpriteBase sprite = _sprites[i];
                if (sprite.IsExpired())
                {
                    _expireList.Add(i);
                    sprite.Hide();
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

        public void Add(SpriteBase sprite)
        {
            _sprites.Add(sprite);
        }
    }
}
