using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Modules.Extensions
{
    public static class LambdaExpression
    {
        /// <summary>
        /// string으로 입력된 값을 람다식으로 변형해준다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortColumn"></param>
        /// <returns></returns>
        public static Expression<Func<T, object>> GetLambdaExpressionFor<T>(this string sortColumn)
        {
            var type = typeof(T);
            var parameterExpression = Expression.Parameter(type, "x");
            var body = Expression.PropertyOrField(parameterExpression, sortColumn);
            var convertedBody = Expression.MakeUnary(ExpressionType.Convert, body, typeof(object));

            var expression = Expression.Lambda<Func<T, object>>(convertedBody, new[] { parameterExpression });

            return expression;
        }
    }
}
