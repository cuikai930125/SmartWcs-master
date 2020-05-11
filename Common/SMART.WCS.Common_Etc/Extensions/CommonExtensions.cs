using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common_Etc.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// datetime에서 date만 추출하여 yyyymmdd형식으로 출력
        /// </summary>
        /// <param name="dateTime">yyyyMMdd</param>
        /// <returns></returns>
        public static string ToShortDateDBString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        #region ToDateString - Datetime값에서 날짜만 추출하여 yyyy-MM-dd형식으로 출력
        /// <summary>
        /// DateTime값에서 날짜만 추출하여 yyyy-MM-dd형식으로 출력
        /// </summary>
        /// <param name="_dtDate">날짜/시간</param>
        /// <returns></returns>
        public static string ToDateString(this DateTime _dtDate)
        {
            return _dtDate.ToString("yyyy-MM-dd");
        }
        #endregion

        #region ToDateTimeString - DateTime값에서 날짜와 시간을 추출하여 yyyy-MM-dd HH:mm:ss형식으로 출력
        /// <summary>
        /// DateTime값에서 날짜와 시간을 추출하여 yyyy-MM-dd HH:mm:ss형식으로 출력
        /// </summary>
        /// <param name="_dtDate"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime _dtDate)
        {
            return _dtDate.ToString("yyyy-MM-dd HH:mm:ss");
        }
        #endregion

        /// <summary>
        /// string반환하며 obj가 null이면 null이 아닌 빈값으로 반환
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToWhiteSpaceOrString(this object obj)
        {
            if (obj != null)
            {
                return obj.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// datatable의 데이터를 ObservableCollection<T>에 추가 한다
        /// ObservableCollection<T>가 null이면 생성하여 추가
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="datatable"></param>
        public static void AddRange<T>(this System.Collections.ObjectModel.ObservableCollection<T> collection, List<T> itemList)
        {

            if (collection == null)
            {
                collection = new System.Collections.ObjectModel.ObservableCollection<T>();
            }

            for (int i = 0; i < itemList.Count; i++)
            {
                collection.Add(itemList[i]);
            }
        }
    }
}
