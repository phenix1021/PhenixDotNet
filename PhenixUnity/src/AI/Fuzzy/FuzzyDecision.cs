using System;
using System.Collections.Generic;

namespace Phenix.Unity.AI.Fuzzy
{
    public class DefuzzyValue
    {
        public int id;  // 解模糊值（去模糊值）类型ID
        public float value;
    }

    /// <summary>
    /// 模糊决策结果
    /// </summary>
    public class FuzzyDecisionResult
    {
        public int id;    // 决策结果类型ID  
        public float ret; // 决策结果值
        public List<DefuzzyValue> defuzzyValues = new List<DefuzzyValue>();  // 解模糊值（去模糊值）
    }

    /// <summary>
    /// 模糊决策（原理参见《游戏开发中的人工智能》模糊逻辑章节）
    /// </summary>
    public class FuzzyDecision
    {
        // 决策相关range
        List<FuzzyRange> _ranges = new List<FuzzyRange>();
        // 决策结果列表
        List<FuzzyDecisionResult> _results = new List<FuzzyDecisionResult>();

        public void AddRange(FuzzyRange range)
        {
            if (_ranges.Contains(range) == false)
            {
                _ranges.Add(range);
            }
        }

        public void AddResult(FuzzyDecisionResult result)
        {
            if (_results.Contains(result) == false)
            {
                _results.Add(result);
            }
        }

        public FuzzyDecisionResult DoDecision(List<float> inputs, FuzzyDecisionFunc func)
        {
            if (inputs.Count != _ranges.Count)
            {
                return null;
            }

            // 根据输入值计算每个相关range的模糊值
            for (int i = 0; i < inputs.Count; i++)
            {
                _ranges[i].Calc(inputs[i]);
            }

            float retSum = 0;
            float maxRet = float.NegativeInfinity;
            FuzzyDecisionResult decisionRet = null;
            foreach (var result in _results)
            {
                // 根据调用方提供的模糊规则计算有关逻辑结果值
                result.ret = func.ExecRule(_ranges, ref result.id);
                if (result.ret > maxRet)
                {
                    maxRet = result.ret;
                    decisionRet = result;
                }
                retSum += result.ret;
            }

            if (decisionRet != null)
            {  
                // 遍历最终结果的每个解模糊值
                for (int i = 0; i < decisionRet.defuzzyValues.Count; i++)
                {                    
                    float sum = 0;
                    for (int ii = 0; ii < _results.Count; ii++)
                    {
                        FuzzyDecisionResult eachRet = _results[ii];
                        sum += (eachRet.defuzzyValues[i].value * eachRet.ret);
                    }
                    decisionRet.defuzzyValues[i].value = sum / retSum;
                }                
            }

            return decisionRet;
        }

        // 模糊逻辑AND
        public static float FuzzyAND(FuzzyRange A, FuzzyRange B)
        {
            return Math.Min(A.Value, B.Value);
        }

        // 模糊逻辑OR
        public static float FuzzyOR(FuzzyRange A, FuzzyRange B)
        {
            return Math.Max(A.Value, B.Value);
        }

        // 模糊逻辑NOT
        public static float FuzzyNOT(FuzzyRange A, FuzzyRange B)
        {
            return 1 - A.Value;
        }
    }

    /// <summary>
    /// 模糊决策函数。即根据一众ranges计算得到某种result的模糊值
    /// </summary>
    public interface FuzzyDecisionFunc
    {
        /// <summary>
        /// 返回值范围[0, 1]
        /// </summary>        
        float ExecRule(List<FuzzyRange> ranges, ref int resultID);
    }
}
