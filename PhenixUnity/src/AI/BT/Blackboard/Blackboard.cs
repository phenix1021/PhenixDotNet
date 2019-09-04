using System.Collections.Generic;

namespace Phenix.Unity.AI
{
    public class Blackboard
    {
        Dictionary<string, object> _shareObjs = new Dictionary<string, object>();

        public List<string> Keys
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (var item in _shareObjs)
                {
                    ret.Add(item.Key);
                }
                return ret;
            }
        }

        public void Remove(string name)
        {
            _shareObjs.Remove(name);
        }

        public void Load(List<string> names)
        {
            _shareObjs.Clear();
            foreach (var item in names)
            {
                Set(item, null, false);
            }
        }

        public object Get(string name)
        {
            if (_shareObjs.ContainsKey(name) == false)
            {
                return null;
            }
            return _shareObjs[name];
        }

        public void Set(string name, object obj, bool replace = true)
        {
            if (string.IsNullOrEmpty(name))
            {                
                return;
            }
            if (_shareObjs.ContainsKey(name))
            {
                if (replace)
                {
                    _shareObjs[name] = obj;
                }                
            }
            else
            {
                _shareObjs.Add(name, obj);
            }
        }
    }
}