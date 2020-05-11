using SMART.WCS.Common_Etc.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common_Etc.Data
{
    public static class ConvertDataTableToList
    {
        #region ▩ 전역변수
        private const string _ROW_SEQ_COLUMN = "ROW_DATA_SEQ";

        private static int _RowSEQ = 0;
        #endregion

        /// <summary>
        /// 비동기 추가
        /// ObservableCollection<T>가 null이면 생성하여 추가
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="datatable"></param>
        public static void ToObservableCollection<T>(this System.Collections.ObjectModel.ObservableCollection<T> collection, DataTable dataTable)
        {
            if (collection == null)
            {
                //throw new Exception(collection.GetType().Name + " Is Null");
            }

            collection.Clear();

            foreach (var item in DataTableToList<T>(dataTable))
            {
                collection.Add(item);
            }
        }


        public static IEnumerable<T> DataTableToEnumerable<T>(this DataTable dataTable)
        {
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataTable.Columns[_ROW_SEQ_COLUMN] != null)
                {
                    dataRow[_ROW_SEQ_COLUMN] = _RowSEQ;
                    _RowSEQ++;
                }

                object classObj = Activator.CreateInstance(typeof(T));
                classObj.RowToObject(dataRow, dataTable.Columns);

                yield return (T)classObj;
            }
        }

        public static List<T> DataTableToList<T>(this DataTable dataTable)
        {
            return dataTable.DataTableToEnumerable<T>().ToList();
        }

        private static void RowToObject(this object ConvertObject, DataRow dataRow, DataColumnCollection columns)
        {
            foreach (DataColumn dtField in columns)
            {
                System.Reflection.PropertyInfo propertyInfos = ConvertObject.GetType().GetProperty(dtField.ColumnName);

                var value = dataRow[dtField.ColumnName];

                if (propertyInfos != null)
                {
                    if (propertyInfos.PropertyType == typeof(String) && value != null)
                    {
                        propertyInfos.SetValue
                                (ConvertObject, Convert.ChangeType(dataRow[dtField.ColumnName], propertyInfos.PropertyType), null);
                    }
                    else if (value?.ToWhiteSpaceOrString() != "")
                    {
                        Type _convertType = propertyInfos.PropertyType;

                        if (IsNullable(_convertType) && dataRow[dtField.ColumnName] != DBNull.Value)
                        {
                            _convertType = Nullable.GetUnderlyingType(propertyInfos.PropertyType);
                        }

                        propertyInfos.SetValue
                                (ConvertObject, Convert.ChangeType(dataRow[dtField.ColumnName], _convertType), null);
                    }
                }
            }
        }

        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || Nullable.GetUnderlyingType(t) != null;
        }


        /// <summary>
        /// IEnumerable<T>을 System.Collections.ObjectModel.ObservableCollection<T> collection에 넣는다.
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="source"></param>
        public static void AddCollection<T>(this System.Collections.ObjectModel.ObservableCollection<T> collection, System.Collections.IEnumerable source) where T : new()
        {
            foreach (var item in source)
            {
                collection.Add((T)item);
            }
        }

        private static string ConvertToDateString(object date)
        {
            if (date == null)
            {
                return string.Empty;
            }

            return date == null ? string.Empty : Convert.ToDateTime(date).ConvertDate();
        }

        private static string ConvertToString(object value)
        {
            return Convert.ToString(ReturnEmptyIfNull(value));
        }

        private static int ConvertToInt(object value)
        {
            return Convert.ToInt32(ReturnZeroIfNull(value));
        }

        private static long ConvertToLong(object value)
        {
            return Convert.ToInt64(ReturnZeroIfNull(value));
        }

        private static decimal ConvertToDecimal(object value)
        {
            return Convert.ToDecimal(ReturnZeroIfNull(value));
        }

        private static DateTime ConvertToDateTime(object date)
        {
            return Convert.ToDateTime(ReturnDateTimeMinIfNull(date));
        }

        public static string ConvertDate(this DateTime datetTime, bool excludeHoursAndMinutes = false)
        {
            if (datetTime != DateTime.MinValue)
            {
                if (excludeHoursAndMinutes)
                {
                    return datetTime.ToString("yyyy-MM-dd");
                }

                return datetTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            return null;
        }
        public static object ReturnEmptyIfNull(this object value)
        {
            if (value == DBNull.Value)
            {
                return string.Empty;
            }

            if (value == null)
            {
                return string.Empty;
            }

            return value;
        }
        public static object ReturnZeroIfNull(this object value)
        {
            if (value == DBNull.Value)
            {
                return 0;
            }

            if (value == null)
            {
                return 0;
            }

            return value;
        }
        public static object ReturnDateTimeMinIfNull(this object value)
        {
            if (value == DBNull.Value)
            {
                return DateTime.MinValue;
            }

            if (value == null)
            {
                return DateTime.MinValue;
            }

            return value;
        }
    }
}
