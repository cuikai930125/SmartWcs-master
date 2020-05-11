using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Modules.Extensions
{
    public static class DataMemberExtensions
    {
        public static IEnumerable<T> PrimaryEquals<T>(this IEnumerable<T> collection)
        {
            if (collection != null)
            {
                //var projectName = collection.First().PrimaryKeyPropertyInfo();

                // collection.Select(f=>
                // {

                // })

                //foreach (var item in collection.First().PrimaryKeyValue()))
                //{

                //}

            }

            return collection;
        }

        public static IEnumerable<PrimaryItem> PrimaryKeyAndValueList<T>(this T data)
        {
            return (from p in data.GetType().GetProperties()
                    let attr = p.GetCustomAttributes(typeof(PrimaryKeyAttribute), true)
                    where attr.Length == 1
                    select new PrimaryItem { Property = p, Value = p.GetValue(p) });
        }

        public static string AllTypeToString<T>(this T obj)
        {
            string _retValue = "";

            if (obj != null)
            {
                if (obj.GetType() == typeof(string))
                {
                    _retValue = obj.ToString();
                }
            }

            return _retValue;
        }
    }

    public class PrimaryItem
    {
        public System.Reflection.PropertyInfo Property { get; set; }

        public object Value { get; set; }
    }
}
