using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.Common_Etc.Data
{
    public class DataConvert
    {
        #region ▩ 문자열
        #region > GetSplitToString - 한개의 캐릭터 값으로 값을 구한다. - Character값 이용
        /// <summary>
        /// 한개의 캐릭터 값으로 값을 구한다. - Character값 이용
        /// </summary>
        /// <param name="_strValue">필터 대상값</param>
        /// <param name="_charKeyword">필터 조건</param>
        /// <param name="_iFindIndex">찾는 결과값의 인덱스 (예:abc.png - 파일명 0, 파일 확장자:1)</param>
        /// <returns></returns>
        public static string GetSplitToString(string _strValue, char _charKeyword, int _iFindIndex)
        {
            try
            {
                string strRtnValue = string.Empty;
                strRtnValue = _strValue.Split(_charKeyword)[_iFindIndex];

                return strRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region ▩ 숫자
        #region > GetAddZeroValue - 숫자를 문자열로 변환 (숫자 자리만큼 앞자리를 0으로 채움)
        /// <summary>
        /// 숫자를 문자열로 변환 (숫자 자리만큼 앞자리를 0으로 채움)
        /// </summary>
        /// <param name="_iValue">숫자 값</param>
        /// <param name="_iDigit">숫자 자리수</param>
        /// <returns>앞자리에 0이 포함된 문자열 변환 숫자값</returns>
        public static string GetAddZeroValue(int _iValue, int _iDigit)
        {
            try
            {
                string strReturnValue = string.Empty;

                switch (_iDigit)
                {
                    case 2:
                        if (_iValue.ToString().Length == 1)
                        {
                            strReturnValue = string.Format("0{0}", _iValue.ToString());
                        }
                        else
                        {
                            strReturnValue = _iValue.ToString();
                        }
                        break;
                    case 3:
                        if (_iValue.ToString().Length == 1)
                        {
                            strReturnValue = string.Format("00{0}", _iValue.ToString());
                        }
                        else if (_iValue.ToString().Length == 2)
                        {
                            strReturnValue = string.Format("0{0}", _iValue.ToString());
                        }
                        else
                        {
                            strReturnValue = _iValue.ToString();
                        }
                        break;
                    case 4:
                        if (_iValue.ToString().Length == 1)
                        {
                            strReturnValue = string.Format("000{0}", _iValue.ToString());
                        }
                        else if (_iValue.ToString().Length == 2)
                        {
                            strReturnValue = string.Format("00{0}", _iValue.ToString());
                        }
                        else if (_iValue.ToString().Length == 3)
                        {
                            strReturnValue = string.Format("0{0}", _iValue.ToString());
                        }
                        else
                        {
                            strReturnValue = _iValue.ToString();
                        }
                        break;
                    case 5:
                        if (_iValue.ToString().Length == 1)
                        {
                            strReturnValue = string.Format("0000{0}", _iValue.ToString());
                        }
                        else if (_iValue.ToString().Length == 2)
                        {
                            strReturnValue = string.Format("000{0}", _iValue.ToString());
                        }
                        else if (_iValue.ToString().Length == 3)
                        {
                            strReturnValue = string.Format("00{0}", _iValue.ToString());
                        }
                        else if (_iValue.ToString().Length == 4)
                        {
                            strReturnValue = string.Format("0{0}", _iValue.ToString());
                        }
                        else
                        {
                            strReturnValue = _iValue.ToString();
                        }
                        break;
                    default:
                        break;
                }

                return strReturnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region ▩ 날짜
        #region > ConvertStringToDate - 문자형을 일자형으로 변경 (날짜)
        /// <summary>
        /// 문자형을 일자형으로 변경 (날짜)
        /// </summary>
        /// <param name="_strValue">날짜 변환 대상 데이터</param>
        /// <returns></returns>
        public static string ConvertStringToDate(string _strValue)
        {
            try
            {
                string strRtnValue      = string.Empty;

                if (_strValue.Length == 8)
                {
                    strRtnValue     = $"{_strValue.Substring(0, 4)}-{_strValue.Substring(4, 2)}-{_strValue.Substring(6, 2)}";
                }
                else if (_strValue.Length == 10)
                {
                    strRtnValue     = _strValue;
                }

                return strRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region ▩ List
        #region > ConvertDataTableToArrayList - DataTable To ArrayList 
        /// <summary>
        /// DataTable To ArrayList 
        /// </summary>
        /// <param name="_dtParam">변환 대상 데이터테이블</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> ConvertDataTableToArrayList(DataTable _dtParam)
        {
            List<Dictionary<string, object>> dicRtnValue = new List<Dictionary<string, object>>();

            int columnCount = _dtParam.Columns.Count;

            for (int i = 0; i < _dtParam.Rows.Count; i++)
            {
                DataRow queryRow                = _dtParam.Rows[i];
                Dictionary<string, object> dic  = new Dictionary<string, object>();

                for (int cols = 0; cols < columnCount; cols++)
                {
                    dic.Add(_dtParam.Columns[cols].ColumnName, queryRow[_dtParam.Columns[cols].ColumnName]);
                }

                dicRtnValue.Add(dic);
            }

            return dicRtnValue;
        }
        #endregion

        #region ConvertArrayListToDataTable - ArrayList To DataTable
        /// <summary>
        /// ArrayList To DataTable
        /// </summary>
        /// <param name="_liParam">변환전 리스트 변수</param>
        /// <returns></returns>
        public static DataTable ConvertArrayListToDataTable(List<Dictionary<string, object>> _liParam)
        {
            DataTable dtRtnValue = new DataTable();
            if (_liParam.Count == 0) { return dtRtnValue; }

            dtRtnValue.Columns.AddRange(
                _liParam.First().Select(r => new DataColumn(r.Key)).ToArray()
            );

            _liParam.ForEach(r => dtRtnValue.Rows.Add(r.Select(c => c.Value).Cast<object>().ToArray()));

            return dtRtnValue;
        }
        #endregion
        #endregion

        #region ▩ Object
        #region > ConvertStringToMediaBrush - 색상 문자열(헥사코드)을 SolidColorBrush형으로 반환
        /// <summary>
        /// 색상 문자열을 Brush 형으로 반환
        /// </summary>
        /// <param name="_strColorValue">색상 문자열</param>
        /// <returns></returns>
        public static System.Windows.Media.Brush ConvertStringToMediaBrush(string _strColorValue)
        {
            return (System.Windows.Media.Brush)(new BrushConverter().ConvertFrom(_strColorValue));
        }
        #endregion

        #region > ConvertStringToSolidColorBrush - 색상 문자열(헥사코드)을 SolidColorBrush형으로 반환
        /// <summary>
        /// 색상 문자열을 SolidColorBrush 형으로 반환
        /// </summary>
        /// <param name="_strColorValue">색상 문자열</param>
        /// <returns></returns>
        public static SolidColorBrush ConvertStringToSolidColorBrush(string _strColorValue)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(_strColorValue));
        }
        #endregion

        #region > ConvertStringToMediaColor - 색상 문자열(헥사코드)을 (Media)Color형으로 반환
        /// <summary>
        /// 색상 문자열(헥사코드)을 (Media)Color형으로 반환
        /// </summary>
        /// <param name="_strColorValue">색상 문자열</param>
        /// <returns></returns>
        public static System.Windows.Media.Color ConvertStringToMediaColor(string _strColorValue)
        {
            return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(_strColorValue);
        }
        #endregion

        #region > ConvertStringToDrawingColor - 색상 문자열(헥사코드)을 (Drawing)Color형으로 반환
        /// <summary>
        /// 색상 문자열(헥사코드)을 Color형으로 반환
        /// </summary>
        /// <param name="_strColorValue">색상 문자열</param>
        /// <returns></returns>
        public static System.Drawing.Color ConvertStringToDrawingColor(string _strColorValue)
        {
            return ColorTranslator.FromHtml(_strColorValue);
        }
        #endregion


        #endregion

        #region ▩ Json 
        #region > ConvertJsonToDataTable - Json 값을 DataTable로 변환
        /// <summary>
        /// Json 값을 DataTable로 변환
        /// </summary>
        /// <param name="_strJsonData">Json 데이터</param>
        /// <returns></returns>
        public static DataTable ConvertJsonToDataTable(string _strJsonData)
        {
            return JsonConvert.DeserializeObject<DataTable>(_strJsonData);
        }
        #endregion
        #endregion
    }
}
