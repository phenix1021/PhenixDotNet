using System.Collections.Generic;
using System.Collections;

namespace Phenix.Unity.AI
{
    public class WorldStateBitData
    {
        public int bit;
        public bool val;        

        public WorldStateBitData(int bit, bool val)
        {
            this.bit = bit;
            this.val = val;            
        }
    }

   /* public class WorldStateBitDataEx
    {
        public WorldStateBitData bitData;
        public bool autoReverse;        

        public WorldStateBitDataEx(int bit, bool val, bool autoReverse)
        {
            bitData = new WorldStateBitData(bit, val);
            this.autoReverse = autoReverse;
        }

        public WorldStateBitDataEx(WorldStateBitData bitData, bool autoReverse)
        {
            this.bitData = bitData;
            this.autoReverse = autoReverse;
        }
    }*/

    public class WorldState
    {        
        BitArray _bits;

        public WorldState(int length)
        {            
            _bits = new BitArray(length);
        }

        public void Clear()
        {
            _bits.SetAll(false);
        }

        public bool Include(List<WorldStateBitData> props)
        {
            if (props == null)
            {
                return false;
            }

            foreach (var item in props)
            {
                if (Get(item.bit) != item.val)
                {
                    return false;
                }
            }
            
            return true;
        }

        /*public bool Include(List<WorldStateBitDataEx> props)
        {
            if (props == null)
            {
                return false;
            }

            foreach (var item in props)
            {
                if (Get(item.bitData.bit) != item.bitData.val)
                {
                    return false;
                }
            }

            return true;
        }*/

        public bool Get(int bit)
        {
            return _bits.Get(bit);
        }

        public void Set(int bit, bool val)
        {
            //bool v = Get(9);
            _bits.Set(bit, val);
            /*if (v == false && Get(9))
            {
                UnityEngine.Debug.Log("++++++++++++++++++++");
            }
            if (v && Get(9)==false)
            {
                UnityEngine.Debug.Log("--------------------");
            }*/
        }

        public void Set(List<WorldStateBitData> props)
        {
            if (props == null)
            {
                return;
            }

            foreach (var item in props)
            {                 
                Set(item.bit, item.val);
            }
        }

        public WorldState Clone()
        {
            WorldState copy = new WorldState(_bits.Count);
            copy._bits = (BitArray)_bits.Clone();
            return copy;
        }
    }

    /*
    [Serializable]
    public class WorldStateProp
    {
        public int WorldStatePropType { get; private set; }
        public object Value { get; set; }

        public WorldStateProp(int worldStatePropType, object val)            
        {
            WorldStatePropType = worldStatePropType;
            Value = val;
        }

        public override bool Equals(object obj)
        {
            WorldStateProp wsp = obj as WorldStateProp;
            if (wsp == null)
            {
                return false;
            }
            if (WorldStatePropType != wsp.WorldStatePropType)
            {
                return false;
            }
            return Value == wsp.Value;
        }

        public override int GetHashCode()
        {
            return (this as object).GetHashCode();
        }

        public static bool operator ==(WorldStateProp prop1, WorldStateProp prop2)
        {
            if (prop1 == null)
                return prop2 == null;            
            return prop1.Equals(prop2);
        }

        public static bool operator !=(WorldStateProp prop1, WorldStateProp prop2)
        {
            return !(prop1 == prop2);
        }        
    }

    public class WorldState
    {
        List<WorldStateProp> _props = new List<WorldStateProp>();

        public bool Has(int propType)
        {
            foreach (var item in _props)
            {
                if (item.WorldStatePropType == propType)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Has(WorldStateProp prop)
        {
            foreach (var item in _props)
            {
                if (item == prop)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否包含另一个WorldState的全部prop
        /// </summary>        
        public bool Has(WorldState other)
        {
            foreach (var item in other._props)
            {
                if (Has(item) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public WorldStateProp Get(int propType)
        {
            foreach (var item in _props)
            {
                if (item.WorldStatePropType == propType)
                {
                    return item;
                }
            }
            return null;
        }

        public void Set(int propType, object propVal)
        {
            bool found = false;
            foreach (var item in _props)
            {
                if (item.WorldStatePropType == propType)
                {
                    item.Value = propVal;
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                _props.Add(new WorldStateProp(propType, propVal));
            }
        }

        public void Set(WorldStateProp prop)
        {
            if (prop == null)
            {
                return;
            }

            Set(prop.WorldStatePropType, prop.Value);
        }

        public void Unset(int propType)
        {            
            foreach (var item in _props)
            {
                if (item.WorldStatePropType == propType)
                {
                    _props.Remove(item);
                    break;
                }
            }
        }

        public static WorldState operator +(WorldState ws1, WorldState ws2)
        {
            if (ws1 == null)
            {
                return ws2;
            }

            if (ws2 == null)
            {
                return ws1;
            }

            foreach (var item in ws2._props)
            {
                ws1.Set(item);
            }

            return ws1;
        }

        public static WorldState operator -(WorldState ws1, WorldState ws2)
        {
            if (ws1 == null)
            {
                return null;
            }

            if (ws2 == null)
            {
                return ws1;
            }

            foreach (var item in ws2._props)
            {
                ws1._props.Remove(item);
            }

            return ws1;
        }
    }*/
}