using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace LocalVarExpression
{
    [TestFixture]
    public class LocalExpressionTest
    {
        private int _intField = 2;
        public int IntProp { get; set; }
        [SetUp]
        public void SetUp()
        {
            IntProp = 4;
        }
        [Test]
        public void TestGetLocalValue()
        {
            var localVar = 7;

            var t1 = LocalExpression.GetValue(() => 1);
            var t2 = LocalExpression.GetValue(() => _intField);
            var t3 = LocalExpression.GetValue(() => IntProp);
            var t4 = LocalExpression.GetValue(() => localVar);

            Assert.AreEqual(1, 1);
        }
    }
}
