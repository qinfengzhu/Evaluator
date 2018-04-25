# Evaluator
字符串表达式计算

# 写法
```csharp
var parser = new EvalParser();
string expression = "1+(2+3)*5-2+9/3";
var queue = parser.ParserInfixExpression(expression);
Assert.AreEqual(expression.ToArray().Length - 2, queue.Count);
var postFixExpression = parser.PrintPostfixExpression(queue);
string expectPostFixExpression = "123+5*+2-93/+";
Assert.AreEqual(expectPostFixExpression, postFixExpression);
```
# 理论基础
[逆波兰算法](/src/Evaluator/调度场算法.md)

# 后续扩展功能

后期会出，根据栈进行Expression(C#)动态的构建,扩展支持变量等功能

