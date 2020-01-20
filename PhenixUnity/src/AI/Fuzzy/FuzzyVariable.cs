using System.Collections.Generic;

namespace Phenix.Unity.AI.Fuzzy
{
    /// <summary>
    /// 模糊变量。如距离、数量等衡量范畴
    /// </summary>
    public class FuzzyVariable
    {
        int _id; // 模糊变量类型ID

        // 所辖模糊集合
        List<FuzzyRange> _ranges = new List<FuzzyRange>();

        public FuzzyVariable(int id)
        {
            _id = id;
        }

        public void Add(FuzzyRange range)
        {
            if (_ranges.Contains(range))
            {
                return;
            }
            _ranges.Add(range);
        }

        /// <summary>
        /// 获取模糊值最大的range
        /// </summary>
        public FuzzyRange Max(float inputVal)
        {
            float maxVal = float.NegativeInfinity;
            FuzzyRange ret = null;
            foreach (var range in _ranges)
            {
                float val = range.Calc(inputVal);
                if (val > maxVal)
                {
                    maxVal = val;
                    ret = range;
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取指定模糊集合的模糊值
        /// </summary>
        public float Calc(int rangeID, float inputVal)
        {
            foreach (var range in _ranges)
            {                
                if (rangeID == range.ID)
                {
                    return range.Calc(inputVal);
                }
            }
            return 0;
        }
    }

}
