namespace Phenix.Unity.AI.Fuzzy
{
    /// <summary>
    /// 模糊隶属函数
    /// </summary>
    public interface FuzzyFunc
    {
        // 返回值范围[0, 1]
        float FuzzyValue(float inputVal);
    }

    /// <summary>
    /// 直线型隶属函数。形如“/”
    /// </summary>
    public class FuzzyFuncGrade : FuzzyFunc
    {
        float _x0, _x1;

        public FuzzyFuncGrade(float x0, float x1)
        {
            _x0 = x0;
            _x1 = x1;
        }

        public float FuzzyValue(float inputVal)
        {
            float result = 0;

            if (inputVal <= _x0)
                result = 0;
            else if (inputVal >= _x1)
                result = 1;
            else
                result = (inputVal / (_x1 - _x0)) - (_x0 / (_x1 - _x0));

            return result;
        }
    }

    /// <summary>
    /// 三角形隶属函数。形如“/\”
    /// </summary>
    public class FuzzyFuncTriangle : FuzzyFunc
    {
        float _x0, _x1, _x2;

        public FuzzyFuncTriangle(float x0, float x1, float x2)
        {
            _x0 = x0;
            _x1 = x1;
            _x2 = x2;
        }

        public float FuzzyValue(float inputVal)
        {
            float result = 0;            

            if (inputVal <= _x0)
                result = 0;
            else if (inputVal == _x1)
                result = 1;
            else if ((inputVal > _x0) && (inputVal < _x1))
                result = (inputVal / (_x1 - _x0)) - (_x0 / (_x1 - _x0));
            else
                result = (-inputVal / (_x2 - _x1)) + (_x2 / (_x2 - _x1));

            return result;
        }
    }

    /// <summary>
    /// 梯形隶属函数
    /// </summary>
    public class FuzzyFuncTrapezoid : FuzzyFunc
    {
        float _x0, _x1, _x2, _x3;

        public FuzzyFuncTrapezoid(float x0, float x1, float x2, float x3)
        {
            _x0 = x0;
            _x1 = x1;
            _x2 = x2;
            _x3 = x3;
        }

        public float FuzzyValue(float inputVal)
        {
            float result = 0;

            if (inputVal <= _x0)
                result = 0;
            else if ((inputVal >= _x1) && (inputVal <= _x2))
                result = 1;
            else if ((inputVal > _x0) && (inputVal < _x1))
                result = (inputVal / (_x1 - _x0)) - (_x0 / (_x1 - _x0));
            else
                result = (-inputVal / (_x3 - _x2)) + (_x3 / (_x3 - _x2));

            return result;
        }
    }
}
