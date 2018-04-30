# Evaluator
字符串表达式计算

# 逆波兰转换测试

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

# 计算调用

1. 不传递参数的调用
```csharp 
var evalParser = new EvalParser();
string expression = "9+(3-1)*3+10/2";
decimal value = evalParser.Eval(expression);
            
Assert.AreEqual(20, value);
```

2. 传递参数的调用
```csharp 
var evalParser = new EvalParser();
string expression = "a+b+c/2";
decimal value = evalParser.Eval(expression, new { a = 5, b = 2, c = 6 });

Assert.AreEqual(10, value);
```



