using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
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
        /// <summary>
        /// 逆波兰-中缀表达式转换为后缀表达式
        /// </summary>
        /// <param name="expression">中缀表达式</param>
        /// <returns></returns>
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
        /// <summary>
        /// 计算表达式的计算结果
        /// </summary>
        /// <param name="expreesion">计算表达式</param>
        /// <returns>计算的结果</returns>
        public decimal Eval(string expreesion)
        {
            return Eval(expreesion, null);
        }
        /// <summary>
        /// 计算表达式的计算结果
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="dynamicObject">动态对象</param>
        /// <returns>计算的结果</returns>
        public decimal Eval(string expression,object dynamicObject)
        {
            var queue = ParserInfixExpression(expression);
            var values = GetParameterValues(dynamicObject);

            var cacheStack = new Stack<Expression>();
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                if (item.ItemType == EItemType.Value && item.IsConstant)
                {
                    var itemExpression = Expression.Constant(item.Value);
                    cacheStack.Push(itemExpression);
                    continue;
                }
                if (item.ItemType == EItemType.Value && !item.IsConstant)
                {
                    var propertyName = item.Content;
                    var propertyValue = values[propertyName];
                    var itemExpression = Expression.Constant(propertyValue);
                    cacheStack.Push(itemExpression);
                }
                if (item.ItemType == EItemType.Operator)
                {
                    var firstParamterExpression = cacheStack.Pop();
                    var secondParamterExpression = cacheStack.Pop();
                    switch (item.Content[0])
                    {
                        case EvalParser.AddOprator:
                            var addExpression = Expression.Add(secondParamterExpression, firstParamterExpression);
                            cacheStack.Push(addExpression);
                            break;
                        case EvalParser.DivOperator:
                            var divExpression = Expression.Divide(secondParamterExpression, firstParamterExpression);
                            cacheStack.Push(divExpression);
                            break;
                        case EvalParser.MulOperator:
                            var mulExpression = Expression.Multiply(secondParamterExpression, firstParamterExpression);
                            cacheStack.Push(mulExpression);
                            break;
                        case EvalParser.SubOperator:
                            var subExpression = Expression.Subtract(secondParamterExpression, firstParamterExpression);
                            cacheStack.Push(subExpression);
                            break;
                        default:
                            throw new Exception("wrong operator");
                    }
                }
            }
            var lambdaExpression = Expression.Lambda<Func<decimal>>(cacheStack.Pop());
            var value = lambdaExpression.Compile()();

            return value;
        }
        /// <summary>
        /// 获取动态对象的属性与值的键值对集合
        /// </summary>
        /// <param name="dynamicObject">动态对象</param>
        /// <returns>属性与值的键值对集合</returns>
        private Dictionary<string, decimal> GetParameterValues(object dynamicObject)
        {
            var values = new Dictionary<string, decimal>();
            if (dynamicObject == null)
                return values;
           var properties = dynamicObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(dynamicObject);
                values.Add(property.Name, Convert.ToDecimal(propertyValue));
            }
            return values;
        }
        /// <summary>
        /// 打印逆波兰后缀表达式
        /// </summary>
        /// <param name="queue">后缀表达式队列</param>
        /// <returns>后缀表达式字符串</returns>
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
