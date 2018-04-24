using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Evaluator
{
    /// <summary>
    /// 解析器
    /// </summary>
    public class EvalParser
    {
        public const char AddOprator = '+';
        public const char SubOperator = '-';
        public const char DivOperator = '/';
        public const char MulOperator = '*';
        public const char LBraceOperator = '(';
        public const char RBraceOperator = ')';

        private static readonly OperatorChar AddOpratorChar = new OperatorChar() { Operator = AddOprator };
        private static readonly OperatorChar SubOperatorChar = new OperatorChar() { Operator = SubOperator };
        private static readonly OperatorChar DivOperatorChar = new OperatorChar() { Operator = DivOperator };
        private static readonly OperatorChar MulOperatorChar = new OperatorChar() { Operator = MulOperator };
        private static readonly OperatorChar LBraceOperatorChar = new OperatorChar() { Operator = LBraceOperator };
        private static readonly OperatorChar RBraceOperatorChar = new OperatorChar() { Operator = RBraceOperator };
        public Queue<EvalItem> ParserInfixExpression(string expression)
        {
            var queue = new Queue<EvalItem>();
            var operatorStack = new Stack<OperatorChar>();

            int index = 0;
            int itemLength = 0;
            int expressionLength = expression.Length;
            //当第一个字符为+或者-的时候
            char firstChar = expression[0];
            if (firstChar == AddOprator || firstChar == SubOperator)
            {
                expression = string.Concat("0", expression);
            }
            using (var scanner = new StringReader(expression))
            {
                string operatorPreItem = string.Empty;
                while (scanner.Peek() > -1)
                {
                    char currentChar = (char)scanner.Read();
                    switch (currentChar)
                    {
                        case AddOprator:
                        case SubOperator:
                        case DivOperator:
                        case MulOperator:
                        case LBraceOperator:
                        case RBraceOperator:
                            //直接把数字压入到队列中
                            operatorPreItem = expression.Substring(index, itemLength);
                            if (operatorPreItem != "")
                            {
                                var numberItem = new EvalItem(EItemType.Value, operatorPreItem);
                                queue.Enqueue(numberItem);
                            }
                            index = index + itemLength + 1;
                            itemLength = -1;
                            //当前操作符
                            var currentOperChar = new OperatorChar() { Operator = currentChar };
                            if (operatorStack.Count == 0)
                            {
                                operatorStack.Push(currentOperChar);
                                break;
                            }
                            //处理当前操作符与操作字符栈进出
                            var topOperator = operatorStack.Peek();
                            //若当前操作符为(或者栈顶元素为(则直接入栈
                            if (currentOperChar == LBraceOperatorChar || topOperator == LBraceOperatorChar)
                            {
                                operatorStack.Push(currentOperChar);
                                break;
                            }
                            //若当前操作符为),则栈顶元素顺序输出到队列,至到栈顶元素(输出为止,单(不进入队列,它自己也不进入队列
                            if (currentOperChar == RBraceOperatorChar)
                            {
                                while (operatorStack.Count > 0)
                                {
                                    var topActualOperator = operatorStack.Pop();
                                    if (topActualOperator != LBraceOperatorChar)
                                    {
                                        var operatorItem = new EvalItem(EItemType.Operator, topActualOperator.GetContent());
                                        queue.Enqueue(operatorItem);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                break;
                            }
                            //若栈顶元素优先级高于当前元素，则栈顶元素输出到队列,当前元素入栈
                            if (topOperator.Level > currentOperChar.Level || topOperator.Level == currentOperChar.Level)
                            {
                                var topActualOperator = operatorStack.Pop();
                                var operatorItem = new EvalItem(EItemType.Operator, topActualOperator.GetContent());
                                queue.Enqueue(operatorItem);

                                while (operatorStack.Count > 0)
                                {
                                    var tempTop = operatorStack.Peek();
                                    if (tempTop.Level > currentOperChar.Level || tempTop.Level == currentOperChar.Level)
                                    {
                                        var topTemp = operatorStack.Pop();
                                        var operatorTempItem = new EvalItem(EItemType.Operator, topTemp.GetContent());
                                        queue.Enqueue(operatorTempItem);
                                    }
                                }
                                operatorStack.Push(currentOperChar);
                            }
                            //当当前元素小于栈顶元素的时候，当前元素直接入栈
                            else
                            {
                                operatorStack.Push(currentOperChar);
                            }
                            break;
                        default:
                            break;
                    }
                    itemLength++;
                }
            }
            //剩余无符号的字符串
            if (index < expressionLength)
            {
                string lastNumber = expression.Substring(index, expressionLength - index);
                var lastNumberItem = new EvalItem(EItemType.Value, lastNumber);
                queue.Enqueue(lastNumberItem);
            }
            //弹出栈中所有操作符号
            if (operatorStack.Count > 0)
            {
                while (operatorStack.Count != 0)
                {
                    var topOperator = operatorStack.Pop();
                    var operatorItem = new EvalItem(EItemType.Operator, topOperator.GetContent());
                    queue.Enqueue(operatorItem);
                }
            }
            return queue;
        }
        public string PrintPostfixExpression(Queue<EvalItem> queue)
        {
            StringBuilder text = new StringBuilder();
            while (queue.Count != 0)
            {
                var evalItem = queue.Dequeue();
                text.AppendFormat("{0}", evalItem.Content);
            }
            return text.ToString();
        }
    }
}
