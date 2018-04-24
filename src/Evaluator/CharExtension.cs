using System;
using System.Collections.Generic;
namespace Evaluator
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class CharExtension
    {
        public static int GetLevel(this char op)
        {
            switch (op)
            {
                case EvalParser.AddOprator:
                case EvalParser.SubOperator:
                    return 1;
                case EvalParser.MulOperator:
                case EvalParser.DivOperator:
                    return 2;
                case EvalParser.LBraceOperator:
                case EvalParser.RBraceOperator:
                    return 3;
                default:
                    return 0;
            }
        }
        public static string GetContent(this OperatorChar opChar)
        {
            return opChar.Operator.ToString();
        }
    }
}
