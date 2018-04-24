using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LocalVarExpression
{
    /// <summary>
    /// 支持本地变量的Expression
    /// </summary>
    public class LocalExpression
    {
        public static object GetValue<T>(Expression<Func<T>> func)
        {
            object value=new object();
            var body = func.Body;

            if (body.NodeType == ExpressionType.Constant)
            {
                value = ((ConstantExpression)body).Value;
            }
            else
            {
                var memberExpression = (MemberExpression)body;

                var @object =
                  ((ConstantExpression)(memberExpression.Expression)).Value; //这个是重点

                if (memberExpression.Member.MemberType == MemberTypes.Field)
                {
                    value = ((FieldInfo)memberExpression.Member).GetValue(@object);
                }
                else if (memberExpression.Member.MemberType == MemberTypes.Property)
                {
                    value = ((PropertyInfo)memberExpression.Member).GetValue(@object);
                }
            }
            return value;
        }
    }
}
