using SMART.WCS.Modules.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class GridControlHelper
    {
        public static IEnumerable<T> OrderByGridControl<T>(this IEnumerable<T> collection, DevExpress.Xpf.Grid.GridControl gridControl)
        {
            IEnumerable<T> _result = collection;

            foreach(var sortinfo in gridControl.SortInfo)
            {
                var expression = LambdaExpression.GetLambdaExpressionFor<T>(sortinfo.FieldName);
                var exp = expression.Compile();

                if(sortinfo.SortOrder == System.ComponentModel.ListSortDirection.Ascending)
                {
                    _result = collection.OrderBy(exp);
                }
                else
                {
                    _result = collection.OrderByDescending(exp);
                }
            }

            return _result;
        }
    }
}
