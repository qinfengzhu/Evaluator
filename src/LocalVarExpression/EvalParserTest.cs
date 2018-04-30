using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Evaluator;
using System.Linq.Expressions;

namespace LocalVarExpression
{
    [TestFixture]
    public class EvalParserTest
    {
        [Test]
        public void ParserExpressionTest()
        {
            var parser = new EvalParser();
            string expression = "1+(2+3)*5-2+9/3";
            var queue = parser.ParserInfixExpression(expression);
            Assert.AreEqual(expression.ToArray().Length - 2, queue.Count);
            var postFixExpression = parser.PrintPostfixExpression(queue);
            string expectPostFixExpression = "123+5*+2-93/+";
            Assert.AreEqual(expectPostFixExpression, postFixExpression);
        }
        [Test]
        public void ParserExpressionOtherTest()
        {
            var parser = new EvalParser();
            string expression = "9+(3-1)*3+10/2";
            var queue = parser.ParserInfixExpression(expression);
            var postFixExpression = parser.PrintPostfixExpression(queue);
            Assert.AreEqual("931-3*+102/+", postFixExpression);
        }
        [Test]
        public void EvalExpreesionValueNoParametersTest()
        {
            var evalParser = new EvalParser();
            string expression = "9+(3-1)*3+10/2";
            decimal value = evalParser.Eval(expression);
            
            Assert.AreEqual(20, value);
        }
        [Test]
        public void EvalExpressionValueWithParametersTest()
        {
            var evalParser = new EvalParser();
            string expression = "a+b+c/2";
            decimal value = evalParser.Eval(expression, new { a = 5, b = 2, c = 6 });

            Assert.AreEqual(10, value);
        }
    }
}
