using SMART.WCS.Common_Etc.DataBase;
using SMART.WCS.Common_Etc.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SMART.WCS.Common_Etc.Controls
{
    public class CommonComboBox : DisposeClass
    {
        #region ▩ 함수 (콤보박스 공통 데이터 조회 및 컨트롤 바인딩)
        #region > BindingFirstComboBox - 로그인 화면 오픈 시 콤보박스 설정
        /// <summary>
        /// 로그인 화면 오픈 시 콤보박스 설정
        /// </summary>
        /// <param name="_ctrlComboBox">콤보박스 컨트롤</param>
        /// <param name="_strCommonCode">공통코드</param>
        public static void BindingFirstComboBox(ComboBox _ctrlComboBox, string _strCommonCode)
        {
            DataTable dtCommonData = GetFirstCommonData(_strCommonCode);
            
            if (dtCommonData != null)
            {
                _ctrlComboBox.ItemsSource = Data.ConvertDataTableToList.DataTableToList<ComboBox>(dtCommonData);

                if (dtCommonData.Rows.Count > 0)
                {
                    _ctrlComboBox.SelectedIndex = 0;
                }
            }
        }
        #endregion

        #region > BindingCommonComboBox - 공통코드 콤보박스 설정 (데이터베이스가 지정되지 않는 경우)
        /// <summary>
        /// 공통코드 콤보박스 설정 (데이터베이스가 지정되지 않는 경우)
        /// </summary>
        /// <param name="_ctrlComboBox">콤보박스 컨트롤</param>
        /// <param name="_strCommonCode">공통코드</param>
        /// <param name="_arrComboBoxInputParam">공통코드 조회 파라메터</param>
        /// <param name="_isfirstRowEmpty">전체 Row 여부</param>
        public static void BindingCommonComboBox(
                    ComboBox _ctrlComboBox
                ,   string _strCommonCode
                ,   string[] _arrComboBoxInputParam
                ,   bool _isfirstRowEmpty
            )
        {
            DataTable dtCommonData = GetCommonData(_strCommonCode, _arrComboBoxInputParam, _isfirstRowEmpty);

            if (dtCommonData != null)
            {
                _ctrlComboBox.ItemsSource = Data.ConvertDataTableToList.DataTableToList<ComboBox>(dtCommonData);

                if (dtCommonData.Rows.Count > 0)
                {
                    _ctrlComboBox.SelectedIndex = 0;
                }
            }
        }
        #endregion

        #region > BindingCommonComboBox - 공통코드 콤보박스 설정 (데이터베이스가 지정된 경우)
        /// <summary>
        /// 공통코드 콤보박스 설정 (데이터베이스가 지정된 경우)
        /// </summary>
        /// <param name="_ctrlComboBox">콤보박스 컨트롤</param>
        /// <param name="_enumSelectedDatabaseKind">데이터베이스 지정값</param>
        /// <param name="_strCommonCode">공통코드</param>
        /// <param name="_arrComboBoxInputParam">공통코드 조회 파라메터</param>
        /// <param name="_isFirstRowEmpty">전체 Row 여부</param>
        public static void BindingCommonComboBox(
                    ComboBox _ctrlComboBox
                ,   BaseEnumClass.SelectedDatabaseKind _enumSelectedDatabaseKind
                ,   string _strCommonCode
                ,   string[] _arrComboBoxInputParam
                ,   bool _isFirstRowEmpty
            )
        {
            DataTable dtCommonData = GetCommonData(_enumSelectedDatabaseKind, _strCommonCode, _arrComboBoxInputParam, _isFirstRowEmpty);

            if (dtCommonData != null)
            {
                _ctrlComboBox.ItemsSource = Data.ConvertDataTableToList.DataTableToList<ComboBox>(dtCommonData);

                if (dtCommonData.Rows.Count > 0)
                {
                    _ctrlComboBox.SelectedIndex = 0;
                }
            }
        }
        #endregion
        #endregion

        #region ▩ 함수 (데이터 조회)
        #region > GetFirstCommonData - 로그인 화면 오픈 시 공통콤보 데이터 조회
        public static DataTable GetFirstCommonData(string _strCommonCode)
        {
            DataTable dtCommonData                      = null;
            var strProcedureName                        = "PK_COMMON.SP_COM_UI_CMB";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_CMB_LIST", "O_RSLT" };

            dicInputParam.Add("P_TYPE_CD",          _strCommonCode);            // 공통코드
            dicInputParam.Add("P_ATTR_ONE",         string.Empty);              // 공통코드 조회 파라메터 1
            dicInputParam.Add("P_ATTR_TWO",         string.Empty);              // 공통코드 조회 파라메터 2
            dicInputParam.Add("P_ATTR_THREE",       string.Empty);              // 공통코드 조회 파라메터 3
            dicInputParam.Add("P_ATTR_FOUR",        string.Empty);              // 공통코드 조회 파라메터 4

            if (Base.Settings1.Default.MainDatabase.ToUpper().Equals("ORACLE") == false)
            {
                strProcedureName = Data.DataConvert.GetSplitToString(strProcedureName, '.', 1);
            }

            using (FirstDataAccess da = new FirstDataAccess())
            {
                dtCommonData = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }

            return dtCommonData;
        }
        #endregion

        #region > GetCommonData - 공통 데이터 조회 (데이터베이스가 지정되지 않는 경우)
        /// <summary>
        /// 공통 데이터 조회 (데이터베이스가 지정되지 않는 경우)
        /// </summary>
        /// <param name="_strCommonCode">공통코드</param>
        /// <param name="_arrComboBoxInputParam">공통코드 조회 파라메터</param>
        /// <param name="_isfirstRowEmpty">전체 Row 여부</param>
        /// <returns></returns>
        public static DataTable GetCommonData(string _strCommonCode, string[] _arrComboBoxInputParam, bool _isfirstRowEmpty)
        {
            DataTable dtCommonData                      = null;

            var strProcedureName                        = "PK_COMMON.SP_COM_UI_CMB";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_CMB_LIST", "O_RSLT" };

            if (_arrComboBoxInputParam == null)
            {
                dicInputParam.Add("P_TYPE_CD",      _strCommonCode);            // 공통코드
                dicInputParam.Add("P_ATTR_ONE",     string.Empty);              // 공통코드 조회 파라메터 1
                dicInputParam.Add("P_ATTR_TWO",     string.Empty);              // 공통코드 조회 파라메터 2
                dicInputParam.Add("P_ATTR_THREE",   string.Empty);              // 공통코드 조회 파라메터 3
                dicInputParam.Add("P_ATTR_FOUR",    string.Empty);              // 공통코드 조회 파라메터 4
            }
            else if (_arrComboBoxInputParam != null && _arrComboBoxInputParam.Length > 0)
            {
                dicInputParam.Add("P_TYPE_CD",      _strCommonCode);                    // 공통코드
                dicInputParam.Add("P_ATTR_ONE",     _arrComboBoxInputParam[0]);         // 공통코드 조회 파라메터 1
                dicInputParam.Add("P_ATTR_TWO",     _arrComboBoxInputParam[1]);         // 공통코드 조회 파라메터 2
                dicInputParam.Add("P_ATTR_THREE",   _arrComboBoxInputParam[2]);         // 공통코드 조회 파라메터 3
                dicInputParam.Add("P_ATTR_FOUR",    _arrComboBoxInputParam[3]);         // 공통코드 조회 파라메터 4
            }

            using (BaseDataAccess da = new BaseDataAccess())
            {
                dtCommonData = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }

            if (_isfirstRowEmpty == true)
            {
                DataRow drNewRow    = dtCommonData.NewRow();
                drNewRow["CODE"]    = " ";
                drNewRow["NAME"]    = GetAllValueByLanguage(HelperClass.GetCurrentCultureInfo());
                dtCommonData.Rows.InsertAt(drNewRow, 0);
                dtCommonData.AcceptChanges();
            }

            return dtCommonData;
        }
        #endregion

        #region > GetCommonData - 공통 데이터 조회 (데이터베이스가 지정된 경우)
        /// <summary>
        /// 공통 데이터 조회 (데이터베이스가 지정된 경우)
        /// </summary>
        /// <param name="_enumSelectedDatabaseKind">데이터베이스 지정값</param>
        /// <param name="_strCommonCode">공통코드</param>
        /// <param name="_arrComboBoxInputParam">공통코드 조회 파라메터</param>
        /// <param name="_isfirstRowEmpty">전체 Row 여부</param>
        /// <returns></returns>
        public static DataTable GetCommonData(
                    BaseEnumClass.SelectedDatabaseKind _enumSelectedDatabaseKind
                ,   string _strCommonCode
                ,   string[] _arrComboBoxInputParam
                ,   bool _isfirstRowEmpty
            )
        {
            DataTable dtCommonData                      = null;

            var strProcedureName                        = "PK_COMMON.SP_COM_UI_CMB";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_CMB_LIST", "O_RSLT" };

            if (_arrComboBoxInputParam == null)
            {
                dicInputParam.Add("P_TYPE_CD",      _strCommonCode);            // 공통코드
                dicInputParam.Add("P_ATTR_ONE",     string.Empty);              // 공통코드 조회 파라메터 1
                dicInputParam.Add("P_ATTR_TWO",     string.Empty);              // 공통코드 조회 파라메터 2
                dicInputParam.Add("P_ATTR_THREE",   string.Empty);              // 공통코드 조회 파라메터 3
                dicInputParam.Add("P_ATTR_FOUR",    string.Empty);              // 공통코드 조회 파라메터 4
            }
            else if (_arrComboBoxInputParam != null && _arrComboBoxInputParam.Length > 0)
            {
                dicInputParam.Add("P_TYPE_CD",      _strCommonCode);            // 공통코드
                dicInputParam.Add("P_ATTR_ONE",     _arrComboBoxInputParam[0]);         // 공통코드 조회 파라메터 1
                dicInputParam.Add("P_ATTR_TWO",     _arrComboBoxInputParam[1]);         // 공통코드 조회 파라메터 2
                dicInputParam.Add("P_ATTR_THREE",   _arrComboBoxInputParam[2]);         // 공통코드 조회 파라메터 3
                dicInputParam.Add("P_ATTR_FOUR",    _arrComboBoxInputParam[3]);         // 공통코드 조회 파라메터 4
            }

            using (BaseDataAccess da = new BaseDataAccess(_enumSelectedDatabaseKind))
            {
                dtCommonData = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }

            if (_isfirstRowEmpty == true)
            {
                DataRow drNewRow    = dtCommonData.NewRow();
                drNewRow["CODE"]    = "ALL";
                //drNewRow["NAME"]    = HelperClass.GetCurrentCultureInfo();
                drNewRow["NAME"]    = GetAllValueByLanguage(HelperClass.GetCurrentCultureInfo());
                dtCommonData.Rows.Add(drNewRow);
            }

            return dtCommonData;
        }
        #endregion

        #endregion

        #region ▩ 함수 (내부함수)
        #region > GetAllValueByLanguage - 콤보박스 첫번째 Row (ALL, 전체) 텍스트 값
        /// <summary>
        /// 콤보박스 첫번째 Row (ALL, 전체) 텍스트 값
        /// </summary>
        /// <param name="_strCntryCode">국가코드</param>
        /// <returns></returns>
        private static string GetAllValueByLanguage(string _strCntryCode)
        {
            var strChangedAllValue = string.Empty;

            switch (_strCntryCode)
            {
                case "KR":
                    strChangedAllValue = "전체";
                    break;
                case "EN":
                    strChangedAllValue = "ALL";
                    break;
                default: break;
            }

            return strChangedAllValue;
        }
        #endregion
        #endregion
    }
}
