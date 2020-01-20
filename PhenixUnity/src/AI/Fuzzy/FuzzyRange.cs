namespace Phenix.Unity.AI.Fuzzy
{

    /// <summary>
    /// 模糊集合。如距离很远、距离较远、距离正好、距离较近、距离很近等等
    /// </summary>
    public class FuzzyRange
    {
        int _id;                // 模糊集合类型ID
        float _value;           // 当前模糊值
        FuzzyFunc _fuzzyFunc;   // 隶属函数

        public int ID { get { return _id; } }
        public float Value { get { return _value; } }

        public FuzzyRange(int id, FuzzyFunc func)
        {
            _id = id;
            _fuzzyFunc = func;
        }

        // 根据隶属函数获得输出的模糊值
        public float Calc(float inputVal)
        {
            _value = _fuzzyFunc.FuzzyValue(inputVal);
            return _value;
        }        
    }
}
