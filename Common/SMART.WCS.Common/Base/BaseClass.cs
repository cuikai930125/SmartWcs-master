using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using Microsoft.Win32;
using Newtonsoft.Json;
using SMART.WCS.Common.Control;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.File;
using SMART.WCS.Common.Utility;
using SMART.WCS.MsgBox.Views;

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Media;

using static SMART.WCS.MsgBox.Views.uMsgBox;

namespace SMART.WCS.Common
{
    /// <summary>
    /// BaseClass (기본 클래스)
    /// </summary>
    public class BaseClass : DisposeClass
    {
        private int _retryCount = 0;

        #region ▩ 속성
        #region > CompanyCode - 회사 코드 설정
        /// <summary>
        /// 회사 코드 설정
        /// </summary>
        public string CompanyCode
        {
            get { return Base.Settings1.Default.CompanyCode; }
            set
            {
                Base.Settings1.Default.CompanyCode = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > CenterCode - 센터 코드 설정
        /// <summary>
        /// 센터 코드 설정
        /// </summary>
        public string CenterCD
        {
            get { return Base.Settings1.Default.CenterCD; }
            set
            {
                Base.Settings1.Default.CenterCD = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > CenterName - 센터명
        /// <summary>
        /// 센터명
        /// </summary>
        public string CenterName
        {
            get { return Base.Settings1.Default.CenterName; }
            set
            {
                Base.Settings1.Default.CenterName = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > Login_CenterCD - 로그인 센터코드
        public string LoginCenterCD
        {
            get { return Base.Settings1.Default.LoginCenter; }
            set
            {
                Base.Settings1.Default.LoginCenter = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > UserID - 사용자 ID
        /// <summary>
        /// 사용자 ID
        /// </summary>
        public string UserID
        {
            get { return Base.Settings1.Default.UserID; }
            set
            {
                Base.Settings1.Default.UserID = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region UserName - 사용자명
        /// <summary>
        /// 사용자명
        /// </summary>
        public string UserName
        {
            get { return Base.Settings1.Default.UserName; }
            set
            {
                Base.Settings1.Default.UserName = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > LoginUserID - 로그인 ID
        /// <summary>
        /// 로그인 ID
        /// </summary>
        public string LoginUserID
        {
            get { return Base.Settings1.Default.LoginUserID; }
            set
            {
                Base.Settings1.Default.LoginUserID = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > RememberChecked - 로그인 창에서 사용자 ID 기억
        /// <summary>
        /// 로그인 창에서 사용자 ID 기억
        /// </summary>
        public bool RememberChecked
        {
            get { return Base.Settings1.Default.RememberChecked; }
            set
            {
                Base.Settings1.Default.RememberChecked = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > CountryCode - 국가코드
        /// <summary>
        /// 국가코드
        /// </summary>
        public string CountryCode
        {
            get { return Base.Settings1.Default.CountryCode; }
            set
            {
                Base.Settings1.Default.CountryCode = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > RoleCode - 권한 코드
        /// <summary>
        /// 권한 코드
        /// </summary>
        public string RoleCode
        {
            get { return Base.Settings1.Default.RoleCD; }
            set
            {
                Base.Settings1.Default.RoleCD = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > ProcInqTerm - Process 날짜조건 제약값 
        /// <summary>
        /// Process 날짜조건 제약값 
        /// </summary>
        public int ProcInqTerm
        {
            get { return Base.Settings1.Default.ProcInqTerm; }
            set
            {
                Base.Settings1.Default.ProcInqTerm = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > AnlInqTerm - Analysis 날짜조건 제약값
        /// <summary>
        /// Analysis 날짜조건 제약값
        /// </summary>
        public int AnlInqTerm
        {
            get { return Base.Settings1.Default.AnlInqTerm; }
            set
            {
                Base.Settings1.Default.AnlInqTerm = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > MainDatabase - 메인 데이터베이스 설정
        /// <summary>
        /// 메인 데이터베이스 설정
        /// </summary>
        public string MainDatabase
        {
            get { return Base.Settings1.Default.MainDatabase; }
            set
            {
                Base.Settings1.Default.MainDatabase = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > DatabaseConnectionString_ORACLE - 데이터베이스 연결 문자열 (오라클) - 암호화 된 문자열
        /// <summary>
        /// 데이터베이스 연결 문자열 (오라클) - 암호화 된 문자열
        /// </summary>
        public string DatabaseConnectionString_ORACLE
        {
            get { return Base.Settings1.Default.DatabaseConnectionString_ORACLE; }
            set
            {
                Base.Settings1.Default.DatabaseConnectionString_ORACLE = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > DatabaseConnectionString_MSSQL - 데이터베이스 연결 문자열 (MS-SQL) - 암호화 된 문자열
        /// <summary>
        /// 데이터베이스 연결 문자열 (MS-SQL) - 암호화 된 문자열
        /// </summary>
        public string DatabaseConnectionString_MSSQL
        {
            get { return Base.Settings1.Default.DatabaseConnectionString_MSSQL; }
            set
            {
                Base.Settings1.Default.DatabaseConnectionString_MSSQL = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        #region > DatabaseConnectionString_MariaDB - 데이터베이스 연결 문자열 (MariaDB) - 암호화 된 문자열
        public string DatabaseConnectionString_MariaDB
        {
            get { return Base.Settings1.Default.DatabaseConnectionString_MariaDB; }
            set
            {
                Base.Settings1.Default.DatabaseConnectionString_MariaDB = value;
                Base.Settings1.Default.Save();
            }
        }

        public void BindingCommonComboBox(object cboEqpId, string v1, string[] commonParam_EQP_ID, bool v2)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region > SessionUserID - 로그인 시 저장된 아이디
        /// <summary>
        /// 로그인 시 저장된 아이디
        /// </summary>
        public string SessionUserID
        {
            get { return Base.Settings1.Default.UserID; }
            set
            {
                Base.Settings1.Default.UserID = value;
                Base.Settings1.Default.Save();
            }
        }
        #endregion

        /// <summary>
        /// 팝업창에서 확인 버튼 클릭값 저장
        /// true : 확인, false : 취소
        /// </summary>
        public bool BUTTON_CONFIRM_YN { get; set; }
        #endregion

        #region ▩ 함수
        #region > 시스템, 화면 관련 함수
        #region GetIPAddress - 로컬 IP 주소를 가져온다.
        /// <summary>
        /// 로컬 IP 주소를 가져온다.
        /// </summary>
        /// <returns></returns>
        public IPAddress GetIPAddress()
        {
            try
            {
                IPAddress[] hostAddresses = Dns.GetHostAddresses("");

                foreach (IPAddress hostAddress in hostAddresses)
                {
                    if (hostAddress.AddressFamily == AddressFamily.InterNetwork &&
                        !IPAddress.IsLoopback(hostAddress) &&  // ignore loopback addresses
                        !hostAddress.ToString().StartsWith("169.254."))  // ignore link-local addresses
                    {
                        return hostAddress;
                    }
                }
                return null; // or IPAddress.None if you prefer
            }
            catch { throw; }
        }
        #endregion

        #region >> GetDBConnectTypeStringValue - DB접속 타입 (REAL-운영, DEV-개발)
        /// <summary>
        /// DB접속 타입 (REAL-운영, DEV-개발)
        /// </summary>
        /// <param name="_iRadioButtonIndex"></param>
        /// <returns></returns>
        public string GetDBConnectTypeStringValue(int _iRadioButtonIndex)
        {
            var strDBConnectString = "";

            switch (_iRadioButtonIndex)
            {
                case 0:
                    strDBConnectString = "REAL";
                    break;
                default:
                    strDBConnectString = "DEV";
                    break;
            }

            return strDBConnectString;
        }
        #endregion

        #region >> GetScreenNavigation - 화면 네비게이션 정보를 가져온다.
        /// <summary>
        /// 화면 네비게이션 정보를 가져온다.
        /// </summary>
        /// <param name="_liMenuName">화면 Depth 리스트</param>
        /// <returns></returns>
        public string GetScreenNavigation(List<string> _liMenuName)
        {
            try
            { 
                var strMenuNavigation = string.Empty;

                for (int i = 0; i < _liMenuName.Count; i++)
                {
                    if (_liMenuName[i] != null)
                    {
                        if (_liMenuName[i].ToString().Length > 0)
                        {
                            strMenuNavigation += $"{_liMenuName[i].ToString()} >";
                        }
                    }
                }

                return strMenuNavigation.Substring(0, strMenuNavigation.Length - 3);
            }
            catch { throw; }
        }
        #endregion

        #region >> GetMacAddress - 맥 어드레스 주소 조회한다.
        /// <summary>
        /// 맥 어드레스 주소 조회한다.
        /// </summary>
        /// <returns></returns>
        public string GetMacAddress()
        {
            return HelperClass.GetMacAddress();
        }
        #endregion

        #region >> GetFileVersion - 파일버전을 가져온다.
        /// <summary>
        /// 파일버전을 가져온다.
        /// </summary>
        /// <param name="_strFilePath">버전 조회 대상 파일 경로 + 파일명 + 확장자</param>
        /// <returns></returns>
        public string GetFileVersion(string _strFilePath)
        {
            try
            {
                return File.File.GetFileVersion(_strFilePath);
            }
            catch { throw; }
        }
        #endregion

        #region >> 화면에 출력하는 메세지 조회 (공통 메세지, 화면별 메세지)
        #region >>> GetMessageByCommon - 공통 메세지 조회
        public string GetMessageByCommon(string _strMessageCode)
        {
            try
            {
                return string.Empty;
            }
            catch { throw; }
        }
        #endregion

        #region >>> GetResourceValue - 리소스 정보를 조회한다. (언어코드가 없는 경우)
        /// <summary>
        /// 리소스 정보를 조회한다. (언어코드가 없는 경우)
        /// </summary>
        /// <param name="_strCodeValue">코드값</param>
        /// <returns></returns>
        public string GetResourceValue(string _strCodeValue)
        {
            return this.GetResourceValue(_strCodeValue, BaseEnumClass.ResourceType.NORMAL);
        }
        #endregion

        #region >>> GetResourceValue - 리소스 정보를 조회한다. (언어코드가 없는 경우)
        /// <summary>
        /// 리소스 정보를 조회한다. (언어코드가 없는 경우)
        /// </summary>
        /// <param name="_strCodeValue">코드값</param>
        /// <param name="_enumResourceType">리소스 타입</param>
        /// <returns></returns>
        public string GetResourceValue(string _strCodeValue, BaseEnumClass.ResourceType _enumResourceType)
        {
            switch (_enumResourceType)
            {
                case BaseEnumClass.ResourceType.MESSAGE:
                    _strCodeValue = $"(MSG){_strCodeValue}";
                    break;
                default: break;
            }

            ResourceManager rm          = new ResourceManager("SMART.WCS.Resource.Language.LanguageResource", typeof(SMART.WCS.Resource.Language.LanguageResource).Assembly);
            CultureInfo cultureInfo     = Utility.HelperClass.GetCountryName(this.CountryCode);
            string strResourceInfo;
            
            if (rm.GetString(_strCodeValue, cultureInfo) == null)
            {
                strResourceInfo     = _strCodeValue;
            }
            else
            {
                if (rm.GetString(_strCodeValue, cultureInfo).Length == 0)
                {
                    strResourceInfo = _strCodeValue;
                }
                else
                {
                    strResourceInfo = rm.GetString(_strCodeValue, cultureInfo);
                }
            }

            return strResourceInfo;
        }
        #endregion

        #region >>> GetResourceValue - 리소스 정보를 조회한다. (언어코드를 파라메터로 설정하는 경우)
        /// <summary>
        /// 리소스 정보를 조회한다.
        /// </summary>
        /// <param name="_strCodeValue">코드값</param>
        /// <param name="_strCntryCode">국가코드</param>
        /// <returns></returns>
        public string GetResourceValue(string _strCodeValue, string _strCntryCode)
        {
            return this.GetResourceValue(_strCodeValue, BaseEnumClass.ResourceType.NORMAL, _strCntryCode);
        }
        #endregion

        #region >>> GetResourceValue - 리소스 정보를 조회한다. (언어코드를 파라메터로 설정하는 경우)
        /// <summary>
        /// 리소스 정보를 조회한다.
        /// </summary>
        /// <param name="_strCodeValue">코드값</param>
        /// <param name="_enumResourceType">리소스 타입</param>
        /// <param name="_strCntryCode">국가코드</param>
        /// <returns></returns>
        public string GetResourceValue(string _strCodeValue, BaseEnumClass.ResourceType _enumResourceType, string _strCntryCode)
        {
            switch (_enumResourceType)
            {
                case BaseEnumClass.ResourceType.MESSAGE:
                    _strCodeValue = $"(MSG){_strCodeValue}";
                    break;
                default: break;
            }

            // 리소스 코드가 없는 경우 공백으로 리턴한다.
            if (_strCodeValue.Length == 0) { return string.Empty; }

            ResourceManager rm          = new ResourceManager("SMART.WCS.Resource.Language.LanguageResource", typeof(SMART.WCS.Resource.Language.LanguageResource).Assembly);
            CultureInfo cultureInfo     = Utility.HelperClass.GetCountryName(_strCntryCode);
            string strResourceInfo;

            if (rm.GetString(_strCodeValue, cultureInfo) == null)
            {
                strResourceInfo     = _strCodeValue;
            }
            else
            {
                if (rm.GetString(_strCodeValue, cultureInfo).Length == 0)
                {
                    strResourceInfo = _strCodeValue;
                }
                else
                {
                    strResourceInfo = rm.GetString(_strCodeValue, cultureInfo);
                }
            }

            return strResourceInfo;
        }
        #endregion
        #endregion

        #region >> CheckResultDataProcess_Oracle - 데이터 조회 후 결과 정보 리턴 (오라클)
        /// <summary>
        /// 데이터 조회 후 결과 정보 리턴
        /// </summary>
        /// <param name="_dsResult">결과 데이터셋</param>
        /// <param name="_strErrCode">Output 에러코드</param>
        /// <param name="_strErrorMsg">Output 에러 메세지</param>
        /// <returns></returns>
        public bool CheckResultDataProcess(DataSet _dsResult, ref string _strErrCode, ref string _strErrMsg)
        {
            try
            {
                bool isRtnValue         = true;
                DataTable dtValue       = null;

                if (_dsResult.Tables.Count < 2)
                {
                    _strErrCode = "99998";
                    _strErrMsg  = "결과 테이블이 없거나 하나입니다.";
                    return false;
                }

                // Table명이 없는 경우 MS-SQL, Maria DB
                dtValue = _dsResult.Tables[_dsResult.Tables.Count - 1];

                if (dtValue.Rows.Count == 0)
                {
                    var strCountryCode      = string.Empty;

                    if (this.CountryCode.Length == 0)
                    {
                        var strCultureName      = CultureInfo.CurrentCulture.ToString();
                        strCountryCode          = strCultureName.Substring(3, 2);
                    }
                    else
                    {
                        strCountryCode          = this.CountryCode;
                    }

                    _strErrCode     = "99999";
                    _strErrMsg      = this.GetResourceValue("INFO_NOT_INQ", strCountryCode);
                    isRtnValue      = false;
                }
                else
                {
                    _strErrCode     = dtValue.Rows[0]["CODE"].ToString();
                    _strErrMsg      = dtValue.Rows[0]["MSG"].ToString();

                    if (_strErrCode.Equals("0") == false) { isRtnValue = false; }
                }

                return isRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> CheckResultDataProcess_Oracle - 데이터 조회 후 결과 정보 리턴 (오라클 외 데이터베이스)
        /// <summary>
        /// 데이터 조회 후 결과 정보 리턴
        /// </summary>
        /// <param name="_dsResult">결과 데이터셋</param>
        /// <param name="_strErrCode">Output 에러 코드</param>
        /// <param name="_strErrMsg">Output 에러 메세지</param>
        /// <param name="_enumSelectedDatabaseKind">데이터베이스 종류</param>
        /// <returns></returns>
        public bool CheckResultDataProcess(DataSet _dsResult, ref string _strErrCode, ref string _strErrMsg, BaseEnumClass.SelectedDatabaseKind _enumSelectedDatabaseKind)
        {
            try
            {
                bool bRtnValue  = true;

                switch (_enumSelectedDatabaseKind)
                {
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        if (_dsResult.Tables[0].Rows.Count > 0)
                        {
                            if (_dsResult.Tables[0].Columns.Count > 0)
                            {
                                if (_dsResult.Tables[0].Columns[0].ColumnName.ToUpper().Equals("ERR_CODE"))
                                {
                                    // 오류코드
                                    _strErrCode = _dsResult.Tables[0].Rows[0]["ERR_CODE"].ToString();
                                }

                                if (_dsResult.Tables[0].Columns[1].ColumnName.ToUpper().Equals("ERR_MSG"))
                                {
                                    // 오류메세지
                                    _strErrMsg  = _dsResult.Tables[0].Rows[0]["ERR_MSG"].ToString();
                                }
                            }
                        }
                        break;

                    case BaseEnumClass.SelectedDatabaseKind.MARIA_DB:

                        break;
                }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> CheckResultDataProcess_Oracle - 데이터 조회 후 결과 정보 리턴 (오라클 외 데이터베이스)
        /// <summary>
        /// 데이터 조회 후 결과 정보 리턴
        /// </summary>
        /// <param name="_dtResult">결과 데이터셋</param>
        /// <param name="_strErrCode">Output 에러 코드</param>
        /// <param name="_strErrMsg">Output 에러 메세지</param>
        /// <param name="_enumSelectedDatabaseKind">데이터베이스 종류</param>
        /// <returns></returns>
        public bool CheckResultDataProcess(DataTable _dtResult, ref string _strErrCode, ref string _strErrMsg, BaseEnumClass.SelectedDatabaseKind _enumSelectedDatabaseKind)
        {
            try
            {
                bool bRtnValue = true;

                /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
                 * 데이터 조회, 저장할 때 오류가 발생하는 경우
                 * 리턴 데이터 테이블의 첫번째 컬럼이 오류코드, 두번째 컬럼이 오류메세지이다.
                 *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/ 

                switch (_enumSelectedDatabaseKind)
                {
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        if (_dtResult.Rows.Count > 0)
                        {
                            if (_dtResult.Columns.Count > 0)
                            {
                                if (_dtResult.Columns[0].ColumnName.ToUpper().Equals("ERR_CODE"))
                                {
                                    // 오류코드
                                    _strErrCode = _dtResult.Rows[0]["ERR_CODE"].ToString();
                                }

                                if (_dtResult.Columns[1].ColumnName.ToUpper().Equals("ERR_MSG"))
                                {
                                    // 오류메세지
                                    _strErrMsg = _dtResult.Rows[0]["ERR_MSG"].ToString();
                                }
                            }
                        }
                        break;

                    case BaseEnumClass.SelectedDatabaseKind.MARIA_DB:

                        break;
                }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> GetCurrentCultrueInfo - 현재 시스템 설정 언어를 가져온다.
        /// <summary>
        /// 현재 시스템 설정 언어를 가져온다.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentCultureInfo()
        {
            return HelperClass.GetCurrentCultureInfo();
        }
        #endregion

        #region >> GetCurrentClassName- 현재 클래스명을 가져온다. (확장자 제외)
        /// <summary>
        /// 현재 클래스명을 가져온다.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentClassName()
        {
            return MethodBase.GetCurrentMethod().DeclaringType.Name;
        }
        #endregion
        #endregion

        //#region > 공통 데이터 조회
        //#region >> GetCommonComboBoxList - 공통코드 정보를 조회한다. (데이터만 조회)
        ///// <summary>
        ///// 공통코드 정보를 조회한다. (데이터만 조회)
        ///// </summary>
        ///// <param name="_enumDatabaseKindShort">데이터베이스 종류</param>
        ///// <param name="_strCommonCode">공통코드</param>
        ///// <param name="_arrAttribute">조회 파라메터 (배열)</param>
        ///// <param name="_isFirstRowEmpty">콤보박스 내 전체 항목 표현 여부</param>
        ///// <param name="_strCntryCode">국가코드</param>
        ///// <returns></returns>
        //public List<ComboBoxInfo> GetCommonComboBoxList(
        //        BaseEnumClass.DatabaseKind _enumDatabaseKind
        //        ,   string _strCommonCode
        //        ,   string[] _arrAttribute
        //        ,   bool _isFirstRowEmpty
        //        ,   string _strCntryCode
        //    )
        //{
        //    try
        //    {
        //        var dtCommonData = new DataTable();

        //        switch (_enumDatabaseKind)
        //        {
        //            case BaseEnumClass.DatabaseKind.ORACLE:    // 오라클
        //                dtCommonData = this.GetCommonDataByOracle(_strCommonCode, _arrAttribute, _isFirstRowEmpty, _strCntryCode);
        //                break;
        //            case BaseEnumClass.DatabaseKind.MS_SQL:    // MS-SQL
        //                dtCommonData = this.GetCommonDataByMSSQL(_strCommonCode, _arrAttribute, _isFirstRowEmpty, _strCntryCode);
        //                break;
        //            case BaseEnumClass.DatabaseKind.MARIA_DB:    // MariaDB
        //                dtCommonData = this.GetCommonDataByMariaDB(_strCommonCode, _arrAttribute, _isFirstRowEmpty, _strCntryCode);
        //                break;
        //            default: break;
        //        }

        //        return SMART.WCS.Common.Data.ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtCommonData);
        //    }
        //    catch { throw; }
        //}
        //#endregion

        ////public void ApplyCommonComboBoxList()

        ///// <summary>
        ///// 공통코드를 조회한다. (오라클)
        ///// </summary>
        ///// <param name="8_strCommonCode">공통코드</param>
        ///// <param name="_arrAttribute">조회 파라메터 (배열)</param>
        ///// <param name="_isFirstRowEmpty">콤보박스 내 전체 항목 표현 여부</param>
        ///// <param name="_strCntryCode">국가코드</param>
        ///// <returns></returns>
        //private DataTable GetCommonDataByOracle(string _strCommonCode, string[] _arrAttribute, bool _isFirstRowEmpty, string _strCntryCode)
        //{
        //    try
        //    {
        //        var dtCommonData            = new DataTable();
        //        var strProcedureName        = "PK_COMM_UI_RUN.SP_COMM_UI_COMBOBOX";
        //        var dicInputParam           = new Dictionary<string, object>();
        //        string[] arrOutputParam     = { "P_COMBO_LIST", "P_RESULT" };

        //        if (_arrAttribute != null && _arrAttribute.Length > 0)
        //        {
        //            dicInputParam.Add("P_TYPE_CD",          _strCommonCode);        // 공통코드
        //            dicInputParam.Add("P_ATTR_ONE",         _arrAttribute[0]);      // 공통코드 조회 파라메터 1
        //            dicInputParam.Add("P_ATTR_TWO",         _arrAttribute[1]);      // 공통코드 조회 파라메터 2
        //            dicInputParam.Add("P_ATTR_THREE",       _arrAttribute[2]);      // 공통코드 조회 파라메터 3
        //            dicInputParam.Add("P_ATTR_FOUR",        _arrAttribute[3]);      // 공통코드 조회 파라메터 4
        //        }

        //        //using (OracleDataAccess da = new OracleDataAccess(BaseEnumClass.DatabaseKind.ORACLE))
        //        //{
        //        //    dtCommonData = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
        //        //}

        //        //if (_isFirstRowEmpty == true)
        //        //{
        //        //    System.Data.DataRow row = dtCommonData.NewRow();
        //        //    // DevExpress의 comboedit에서 key가 null인 경우 선택이 안됨. ( 2018-07-31 김태성)
        //        //    row["CODE"] = "ALL";
        //        //    row["NAME"] = this.GetAllValueByLanguage(_strCntryCode);
        //        //    dtCommonData.Rows.InsertAt(row, 0);
        //        //}

        //        return dtCommonData;
        //    }
        //    catch { throw; }
        //}

        ///// <summary>
        ///// 공통코드를 조회한다. (MS-SQL)
        ///// </summary>
        ///// <param name="_strCommonCode">공통코드</param>
        ///// <param name="_arrAttribute">조회 파라메터 (배열)</param>
        ///// <param name="_isFirstRowEmpty">콤보박스 내 전체 항목 표현 여부</param>
        ///// <param name="_strCntryCode">국가코드</param>
        ///// <returns></returns>
        //private DataTable GetCommonDataByMSSQL(string _strCommonCode, string[] _arrAttribute, bool _isFirstRowEmpty, string _strCntryCode)
        //{
        //    try
        //    {
        //        var dtCommonData            = new DataTable();
        //        var strProcedureName        = "SP_AISTRA_SELECT_COMM_CODE_LIST2";
        //        var dicInputParam           = new Dictionary<string, object>();

        //        if (_arrAttribute != null && _arrAttribute.Length > 0)
        //        { 
        //            dicInputParam.Add("P_TYPE_CD",          _strCommonCode);        // 공통코드
        //            dicInputParam.Add("P_ATTR_ONE",         _arrAttribute[0]);      // 공통코드 조회 파라메터 1
        //            dicInputParam.Add("P_ATTR_TWO",         _arrAttribute[1]);      // 공통코드 조회 파라메터 2
        //            dicInputParam.Add("P_ATTR_THREE",       _arrAttribute[2]);      // 공통코드 조회 파라메터 3
        //            dicInputParam.Add("P_ATTR_FOUR",        _arrAttribute[3]);      // 공통코드 조회 파라메터 4
        //        }

        //        //using (MSSqlDataAccess da = new MSSqlDataAccess(BaseEnumClass.DatabaseKind.MS_SQL))
        //        //{
        //        //    dtCommonData = da.GetSpDataTable(strProcedureName, dicInputParam);
        //        //}

        //        //if (_isFirstRowEmpty == true)
        //        //{
        //        //    System.Data.DataRow row = dtCommonData.NewRow();
        //        //    // DevExpress의 comboedit에서 key가 null인 경우 선택이 안됨. ( 2018-07-31 김태성)
        //        //    row["CODE"] = "ALL";
        //        //    row["NAME"] = this.GetAllValueByLanguage(_strCntryCode);
        //        //    dtCommonData.Rows.InsertAt(row, 0);
        //        //}

        //        return dtCommonData;
        //    }
        //    catch { throw; }
        //}

        ///// <summary>
        ///// 공통코드를 조회한다. (MariaDB)
        ///// </summary>
        ///// <param name="_strCommonCode">공통코드</param>
        ///// <param name="_arrAttribute">조회 파라메터 (배열)</param>
        ///// <param name="_isFirstRowEmpty">콤보박스 내 전체 항목 표현 여부</param>
        ///// <param name="_strCntryCode">국가코드</param>
        ///// <returns></returns>
        //private DataTable GetCommonDataByMariaDB(string _strCommonCode, string[] _arrAttribute, bool _isFirstRowEmpty, string _strCntryCode)
        //{
        //    try
        //    {
        //        var dtCommonData            = new DataTable();
        //        var strProcedureName        = "";
        //        var dicInputParam           = new Dictionary<string, object>();

        //        if (_arrAttribute != null && _arrAttribute.Length > 0)
        //        {
        //            dicInputParam.Add("P_TYPE_CD",          _strCommonCode);        // 공통코드
        //            dicInputParam.Add("P_ATTR_ONE",         _arrAttribute[0]);      // 공통코드 조회 파라메터 1
        //            dicInputParam.Add("P_ATTR_TWO",         _arrAttribute[1]);      // 공통코드 조회 파라메터 2
        //            dicInputParam.Add("P_ATTR_THREE",       _arrAttribute[2]);      // 공통코드 조회 파라메터 3
        //            dicInputParam.Add("P_ATTR_FOUR",        _arrAttribute[3]);      // 공통코드 조회 파라메터 4
        //        }

        //        //using (MariaDBDataAccess da = new MariaDBDataAccess(BaseEnumClass.DatabaseKind.MARIA_DB))
        //        //{
        //        //    dtCommonData = da.GetSpDataTable(strProcedureName, dicInputParam);
        //        //}

        //        //if (_isFirstRowEmpty == true)
        //        //{
        //        //    System.Data.DataRow row = dtCommonData.NewRow();
        //        //    // DevExpress의 comboedit에서 key가 null인 경우 선택이 안됨. ( 2018-07-31 김태성)
        //        //    row["CODE"] = "ALL";
        //        //    row["NAME"] = this.GetAllValueByLanguage(_strCntryCode);
        //        //    dtCommonData.Rows.InsertAt(row, 0);
        //        //}

        //        return dtCommonData;

        //    }
        //    catch { throw; }
        //}

        #region GetAllValueByLanguage - ComboBox에 ALL추가 시 국가코드별로 Display 값 설정
        /// <summary>
        /// ComboBox에 ALL추가 시 국가코드별로 Display 값 설정
        /// </summary>
        /// <returns></returns>
        public string GetAllValueByLanguage()
        {
            string strChangedAllValue   = string.Empty;

            switch (this.CountryCode)
            {
                case "KR":
                    strChangedAllValue = "전체";
                    break;
                case "EN":
                    strChangedAllValue = "ALL";
                    break;
            }

            return strChangedAllValue;
        }
        #endregion
        //#endregion

        #region > Configuration 내용 조회
        #region >> GetAppSettings - 테그 조건에 맞는 AppSettings값 조회
        /// <summary>
        /// 테그 조건에 맞는 AppSettings값 조회
        /// </summary>
        /// <param name="_strTagName">테그명</param>
        /// <returns></returns>
        public string GetAppSettings(string _strTagName)
        {
            try
            {
                return SMART.WCS.Common.Configuration.GetAppSettings(_strTagName);
            }
            catch { throw; }
        }
        #endregion

        #region >> SetAppSettings - 테그가 일치하는 기존값을 삭제하고 신규값을 저장한다.
        /// <summary>
        /// 테그가 일치하는 기존값을 삭제하고 신규값을 저장한다.
        /// </summary>
        /// <param name="_strTagName">AppSettings 테그명</param>
        /// <param name="_strValue">AppSettings 값</param>
        public void SetAppSettings(string _strTagName, string _strValue)
        {
            try
            {
                SMART.WCS.Common.Configuration.SetAppSettings(_strTagName, _strValue);
            }
            catch { throw; }
        }
        #endregion

        #region >> AddAppSettings - 신규 설정값을 저장한다.
        /// <summary>
        /// 신규 설정값을 저장한다.
        /// </summary>
        /// <param name="_strTagName">AppSettings 테그명</param>
        /// <param name="_strValue">AppSettings 값</param>
        public void AddAppSettings(string _strTagName, string _strValue)
        {
            try
            {
                SMART.WCS.Common.Configuration.AddAppSettings(_strTagName, _strValue);
            }
            catch { throw; }
        }
        #endregion

        #endregion

        #region > 그리드 관련 (GridControl)
        #region >> GetCurrentRowIndex - 그리드 내 현재 선택된 Row 인덱스를 반환한다.
        /// <summary>
        /// GetCurrentRowIndex - 그리드 내 현재 선택된 Row 인덱스를 반환한다.
        /// </summary>
        /// <param name="_tv">그리드 컨트롤 테이블 뷰</param>
        /// <returns></returns>
        public int GetCurrentGridControlRowIndex(TableView _tv)
        {
            return _tv.FocusedRowHandle;
        }
        #endregion

        #region >> GetCurrentTreeListControlRowIndex - 트리 컨트롤 내 현재 선택된 Row 인덱스를 반환한다.
        /// <summary>
        /// 트리 컨트롤 내 현재 선택된 Row 인덱스를 반환한다.
        /// </summary>
        /// <param name="_tlv">트리 컨트롤 리스트 뷰</param>
        /// <returns></returns>
        public int GetCurrentTreeListControlRowIndex(TreeListView _tlv)
        {
            return _tlv.FocusedRowHandle;
        }
        #endregion

        #region >> ApplySelectedColumnDataChanged - 그리드 
        public void ApplySelectedColumnDataChanged(TableView _tv)
        {

        }
        #endregion

        #region >> 그리드 Row 추가 후 Focus 이동
        /// <summary>
        /// 그리드 Row 추가 후 Focus 이동
        /// </summary>
        /// <param name="_objGridControl">그리드 컨트롤</param>
        /// <param name="_iFocusedIndex">추가된 Row 인덱스</param>
        public void SetGridRowAddFocuse(GridControl _objGridControl, int _iFocusedIndex)
        {
            var iVisibleRowCount        = _objGridControl.VisibleRowCount;

            if (iVisibleRowCount > 0)
            {
                _objGridControl.BeginSelection();
                _objGridControl.UnselectAll();

                _objGridControl.View.FocusedRowHandle   = _iFocusedIndex;
                _objGridControl.SelectItem(_iFocusedIndex);
                _objGridControl.EndSelection();
            }
        }
        #endregion

        #region >> 그리드 컬럼 수정 가능 여부
        /// <summary>
        /// 그리드 컬럼 수정 가능 여부
        /// </summary>
        /// <param name="_tv">그리드 테이블 뷰</param>
        /// <param name="_isAllowEditing">수정여부 (true:수정, false,수정불가)</param>
        public void SetGridAllowEditing(TableView _tv, bool _isAllowEditing)
        {
            try
            {
                var isCancel    = !_isAllowEditing;

                _tv.ShowingEditor += (s, e) =>
                {
                    e.Cancel = isCancel;
                };
            }
            catch { throw; }
        }
        #endregion

        //public static void CheckGridSelected<T>(this System.Collections.ObjectModel.ObservableCollection<T> _Collection)
        //{
        //    if (_Collection == null) { return; }

        //    //foreach (var item in _Collection)
        //    //{
        //    //    _Collection.
        //    //}

        //    //if (collection == null)
        //    //{
        //    //    //throw new Exception(collection.GetType().Name + " Is Null");
        //    //}

        //    //collection.Clear();

        //    //foreach (var item in DataTableToList<T>(dataTable))
        //    //{
        //    //    collection.Add(item);
        //    //}
        //}

        //#region > GetVisibleRowIndex - 그리드에 바인딩 된 데이터와 일치하는 Row Index를 리턴한다.
        ///// <summary>
        ///// 그리드에 바인딩 된 데이터와 일치하는 Row Index를 리턴한다.
        ///// </summary>
        ///// <param name="_objGridControl"></param>
        ///// <param name="_objData"></param>
        ///// <returns></returns>
        //public int GetVisibleRowIndex(GridControl _objGridControl, object _objData)
        //{
        //    int _result = -1;

        //    for (int i = 0; i < _objGridControl.VisibleRowCount; i++)
        //    {
        //        if (_objData.Equals(_objGridControl.GetRow(i)))
        //        {
        //            _result = i;
        //            break;
        //        }
        //    }

        //    return _result;

        //}
        //#endregion
        #endregion

        #region > 트리 리스트 컨트롤 관련 (TreeListControl)
        #region >> 트리 리스트 Row Focus 이동
        /// <summary>
        /// 트리 리스트 Row Focus 이동
        /// </summary>
        /// <param name="_objTreeListControl">트리 리스트 컨트롤</param>
        /// <param name="_iFocusedIndex">포커스 이동 인덱스</param>
        public void SetTreeListControlRowAddFocus(TreeListControl _objTreeListControl, int _iFocusedIndex)
        {
            var iVisibleRowCount    = _objTreeListControl.VisibleRowCount;

            if (iVisibleRowCount > 0)
            {
                _objTreeListControl.BeginSelection();
                _objTreeListControl.UnselectAll();

                _objTreeListControl.View.FocusedRowHandle = _iFocusedIndex;
                _objTreeListControl.SelectItem(_iFocusedIndex);
                _objTreeListControl.EndSelection();
            }
        }
        #endregion
        #endregion

        #region > 컨트롤 관련 (SimpleButton)
        #region >> SetSimpleButtonIsEnabled - SimpleButton Enabled 속성 및 커서 모양 설정
        /// <summary>
        /// SimpleButton Enabled 속성 및 커서 모양 설정
        /// </summary>
        /// <param name="_ctrlSimpleButton">SimpleButton 컨트롤</param>
        /// <param name="_isEnabled">Enabled 여부</param>
        public void SetSimpleButtonIsEnabled(SimpleButton _ctrlSimpleButton, bool _isEnabled)
        {
            HelperClass.SetSimpleButtonIsEnabled(_ctrlSimpleButton, _isEnabled);
        }
        #endregion
        #endregion

        #region > 컨트롤 관련 (콤보박스)
        #region >> BindingFirstComboBox - 로그인 화면 오픈 시 콤보박스 설정
        /// <summary>
        /// 로그인 화면 오픈 시 콤보박스 설정
        /// </summary>
        /// <param name="_ctrlComboBox">콤보박스 컨트롤</param>
        /// <param name="_strCommonCode">공통코드</param>
        public void BindingFirstComboBox(ComboBoxEdit _ctrlComboBox, string _strCommonCode)
        {
            SMART.WCS.Common.Control.CommonComboBox.BindingFirstComboBox(_ctrlComboBox, _strCommonCode);
        }
        #endregion

        #region >> BindingCommonComboBox - 공통코드 콤보박스 설정 (데이터베이스가 지정되지 않는 경우)
        /// <summary>
        /// 공통코드 콤보박스 설정 (데이터베이스가 지정되지 않는 경우)
        /// </summary>
        /// <param name="_ctrlComboBox">콤보박스 컨트롤</param>
        /// <param name="_strCommonCode">공통코드</param>
        /// <param name="_arrComboBoxInputParam">공통코드 조회 파라메터</param>
        /// <param name="_isFirstRowEmpty">전체 Row 여부</param>
        public void BindingCommonComboBox(
                    ComboBoxEdit _ctrlComboBox
                ,   string _strCommonCode
                ,   string[] _arrComboBoxInputParam
                ,   bool _isFirstRowEmpty
            )
        {
            CommonComboBox.BindingCommonComboBox(_ctrlComboBox, _strCommonCode, _arrComboBoxInputParam, _isFirstRowEmpty);
        }
        #endregion

        #region >> BindingCommonComboBox - 공통코드 콤보박스 설정 (데이터베이스가 지정되는 경우)
        /// <summary>
        /// 공통코드 콤보박스 설정 (데이터베이스가 지정되지 않는 경우)
        /// </summary>
        /// <param name="_ctrlComboBox">콤보박스 컨트롤</param>
        /// <param name="_strCommonCode">공통코드</param>
        /// <param name="_arrComboBoxInputParam">공통코드 조회 파라메터</param>
        /// <param name="_isFirstRowEmpty">전체 Row 여부</param>
        public void BindingCommonComboBox(
                    ComboBoxEdit _ctrlComboBox
                ,   BaseEnumClass.SelectedDatabaseKind _enumSelectedDatabaseKind
                ,   string _strCommonCode
                ,   string[] _arrComboBoxInputParam
                ,   bool _isFirstRowEmpty
            )
        {
            CommonComboBox.BindingCommonComboBox(_ctrlComboBox, _enumSelectedDatabaseKind, _strCommonCode, _arrComboBoxInputParam, _isFirstRowEmpty);
        }
        #endregion

        #region >> ComboBoxSelectedKeyValue - 콤보박스 컨트롤의 현재 선택된 Row의 키값을 가져온다.
        /// <summary>
        /// 콤보박스 컨트롤의 현재 선택된 Row의 키값을 가져온다.
        /// </summary>
        /// <param name="_ctrlComboBoxEdit">콤보박스 컨트롤 객체</param>
        /// <returns></returns>
        public string ComboBoxSelectedKeyValue(ComboBoxEdit _ctrlComboBoxEdit)
        {
            if (this.ComboBoxItemCount(_ctrlComboBoxEdit)== 0) { return string.Empty; }
            return _ctrlComboBoxEdit.GetKeyValue(_ctrlComboBoxEdit.SelectedIndex).ToString().Trim();
        }
        #endregion

        #region >> ComboBoxSelectedDisplayValue - 콤보박스 컨트롤의 현재 선택된 Row의 명을 가져온다.
        /// <summary>
        /// 콤보박스 컨트롤의 현재 선택된 Row의 명을 가져온다.
        /// </summary>
        /// <param name="_ctrlComboBoxEdit">콤보박스 컨트롤 객체</param>
        /// <returns></returns>
        public string ComboBoxSelectedDisplayValue(ComboBoxEdit _ctrlComboBoxEdit)
        {
            if (this.ComboBoxItemCount(_ctrlComboBoxEdit) == 0) { return string.Empty; }
            return _ctrlComboBoxEdit.GetDisplayValue(_ctrlComboBoxEdit.SelectedIndex).ToString();
        }
        #endregion

        #region >> ComboBoxItemIndex - 콤보박스 코드와 일치하는 Index값을 구한다. (값과 일치하는 콤보박스 선택을 위한 구문)
        /// <summary>
        /// 콤보박스 코드와 일치하는 Index값을 구한다. (값과 일치하는 콤보박스 선택을 위한 구문)
        /// </summary>
        /// <param name="_ctrlComboBoxEdit">콤보박스 컨트롤 객체</param>
        /// <param name="_strComboBoxItemValue">조건값</param>
        /// <returns></returns>
        public int ComboBoxItemIndex(ComboBoxEdit _ctrlComboBoxEdit, string _strComboBoxItemValue)
        {
            try
            {
                int iRtnValue   = -1;

                var liList = _ctrlComboBoxEdit.ItemsSource as List<ComboBoxInfo>;
                for (int i = 0; i < liList.Count; i++)
                {
                    if (liList[i].CODE.ToString().Equals(_strComboBoxItemValue))
                    {
                        iRtnValue = i;
                        break;
                    }
                }

                return iRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> ComboBoxItemCount - 콤보박스 항목 (Item) 개수
        /// <summary>
        /// ComboBoxItemCount - 콤보박스 항목 (Item) 개수
        /// </summary>
        /// <param name="_ctrlComboBoxEdit">콤보박스 컨트롤</param>
        /// <returns></returns>
        public int ComboBoxItemCount(ComboBoxEdit _ctrlComboBoxEdit)
        {
            var liList = _ctrlComboBoxEdit.ItemsSource as List<ComboBoxInfo>;

            // 콤보박스의 ItemsSource가 null인경우 0을 리턴한다.
            if (liList == null) { return 0; }

            return liList.Count;
        }
        #endregion

        #region >> ModifyComboBoxColumnHeaderName - 공통 콤보박스가 아닌 경우 DataTable 컬럼명을 강제로 설정하기 위한 함수
        /// <summary>
        /// 공통 콤보박스가 아닌 경우 DataTable 컬럼명을 강제로 설정하기 위한 함수
        /// </summary>
        /// <param name="_dtOriginalValue">원본 데이터테이블</param>
        /// <returns></returns>
        public DataTable ModifyComboBoxColumnHeaderName(DataTable _dtOriginalValue)
        {
            string strColumnName       = string.Empty;

            for (int i = 0; i < _dtOriginalValue.Columns.Count; i++)
            {
                if (i == 0)
                {
                    strColumnName   = "CODE";
                }
                else if (i == 1)
                {
                    strColumnName   = "NAME";
                }

                _dtOriginalValue.Columns[i].ColumnName = strColumnName;
            }

            return _dtOriginalValue;
        }
        #endregion

        public int MatchComboIndex(ComboBoxEdit _ctrlComboBoxEdit, string _strComboValue)
        {
            int iRtnValue   = 0;

            if (_strComboValue.Length > 0)
            {
                for(int i = 0; i < this.ComboBoxItemCount(_ctrlComboBoxEdit); i++)
                {
                    if (_ctrlComboBoxEdit.GetKeyValue(i).ToString().Trim().Equals(_strComboValue) == true)
                    {
                        iRtnValue = i;
                        break;
                    }
                    //if (_ctrlComboBoxEdit.Items[i].ToString().Equals(_strComboValue) == true)
                    //{
                    //    
                    //}
                }
            }

            return iRtnValue;
        }
        #endregion

        #region > 컨트롤 관련 (달력 컨트롤)
        #region >> 이벤트에 따른 달력 컨틀롤 데이터 가져오기
        /// <summary>
        /// 이벤트에 따른 달력 컨틀롤 데이터 가져오기
        /// </summary>
        /// <param name="_ctrlDateEdit">달력 컨트롤</param>
        /// <returns></returns>
        public string GetCalendarValue(DateEdit _ctrlDateEdit)
        {
            return HelperClass.GetCalendarValue(_ctrlDateEdit);
        }
        #endregion

        #region >> AutoSetDateTimeByCenter - 센터에 따른 날짜 시간 자동 설정 (컨트롤 2개)
        /// <summary>
        /// 센터에 따른 날짜 시간 자동 설정 (컨트롤 2개)
        /// </summary>
        /// <param name="_ctrlDateEditFrom">날짜 컨트롤</param>
        /// <param name="_ctrlTextEditFrom">시간 컨트롤</param>
        public void AutoSetDateTimeByCenter(DateEdit _ctrlDateEditFrom, TextEdit _ctrlTextEditFrom)
        {
            try
            {
                DataTable dtSetDateTime = Utility.HelperClass.GetDate_INIT_INQ();

                if (dtSetDateTime.Rows.Count > 0)
                {
                    _ctrlDateEditFrom.Text  = dtSetDateTime.Rows[0][0].ToString();      // 날짜 컨트롤
                    _ctrlTextEditFrom.Text  = dtSetDateTime.Rows[0][1].ToString();      // 시간 컨트롤 (TextEdit)
                }
            }
            catch { throw; }
        }
        #endregion

        #region >> AutoSetDateTimeByCenter - 센터에 따른 날짜 시간 자동 설정 (컨트롤 4개)
        /// <summary>
        /// 센터에 따른 날짜 시간 자동 설정 (컨트롤 4개)
        /// </summary>
        /// <param name="_ctrlDateEditFrom"></param>
        /// <param name="_ctrlTextEditFrom"></param>
        /// <param name="_ctrlDateEditTo"></param>
        /// <param name="_ctrlTextEditTo"></param>
        public void AutoSetDateTimeByCenter(DateEdit _ctrlDateEditFrom, TextEdit _ctrlTextEditFrom, DateEdit _ctrlDateEditTo, TextEdit _ctrlTextEditTo)
        {
            try
            {
                DataTable dtSetDateTime = Utility.HelperClass.GetDate_INIT_INQ();

                if (dtSetDateTime.Rows.Count > 0)
                {
                    string strFromHour      = dtSetDateTime.Rows[0][1].ToString().Substring(0, 2);
                    string strFromMinute    = dtSetDateTime.Rows[0][1].ToString().Substring(2, 2);

                    string strToHour        = dtSetDateTime.Rows[0][3].ToString().Substring(0, 2);
                    string strToMinute      = dtSetDateTime.Rows[0][3].ToString().Substring(2, 2);

                    _ctrlDateEditFrom.Text = dtSetDateTime.Rows[0][0].ToString();       // 날짜 컨트롤 (From)
                    _ctrlTextEditFrom.Text = $"{strFromHour}:{strFromMinute}";          // 시간 컨트롤 (TextEdit)

                    _ctrlDateEditTo.Text = dtSetDateTime.Rows[0][2].ToString();         // 날짜 컨트롤 (To)
                    _ctrlTextEditTo.Text = $"{strToHour}:{strToMinute}";                // 시간 컨트롤
                }
            }
            catch { throw; }
        }
        #endregion

        #region >> 
        public int CheckDateTerm(DateEdit _ctrlDateEditFrom, DateEdit _ctrlDateEditTo, BaseEnumClass.SystemCode _enumSystemCode)
        {
            try
            {
                int iRtnValue   = -1;
                var iTermValue  = -1;

                // 일자차이
                var iDiffDay = (_ctrlDateEditTo.DateTime - _ctrlDateEditFrom.DateTime).Days;

                if (_enumSystemCode == BaseEnumClass.SystemCode.PCS)
                {
                    // Processing
                    iTermValue      = this.ProcInqTerm;
                    iRtnValue       = iTermValue - iDiffDay;
                }
                else if (_enumSystemCode == BaseEnumClass.SystemCode.ANL)
                {
                    // Analysis
                    iTermValue      = this.AnlInqTerm;
                    iRtnValue       = iTermValue - iDiffDay;
                }

                return iRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 컨트롤 관련 (공통 메세지 박스)
        #region >> MsgInfo - Information 메세지 리소스 설정 및 메세지 박스 호출
        /// <summary>
        /// Information 메세지 리소스 설정 및 메세지 박스 호출
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        public void MsgInfo(string _strMessageResourceCode)
        {   
            this.MsgInfo(_strMessageResourceCode, string.Empty);
        }
        #endregion

        #region MsgInfo - Information 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// <summary>
        /// Information 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// </summary>
        /// <param name="_strMessage">팝업 메세지</param>
        /// <param name="_enumCodeMessage">코드, 메세지 여부</param>
        public void MsgInfo(string _strMessage, BaseEnumClass.CodeMessage _enumCodeMessage)
        {
            if (_enumCodeMessage == BaseEnumClass.CodeMessage.CODE)
            {
                this.MsgInfo(_strMessage);
            }
            else
            {
                using (uMsgBox frmInformation = new uMsgBox(_strMessage, MsgBoxKind.Information))
                {
                    frmInformation.ClickResult += FrmMsgBox_ClickResult;
                    frmInformation.ShowDialog();
                }
            }
        }
        #endregion

        #region >> MsgInfo - Information 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// <summary>
        /// Information 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        /// <param name="_strConditionValue">멀티 항목 조건값</param>
        public void MsgInfo(string _strMessageResourceCode, string _strConditionValue)
        {
            string strMessage = string.Empty;

            if (_strConditionValue.Length == 0)
            {
                strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode);
            }
            else
            {
                strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode, _strConditionValue);
            }

            using (uMsgBox frmInformation = new uMsgBox(strMessage, MsgBoxKind.Information))
            {
                frmInformation.ClickResult += FrmMsgBox_ClickResult;
                frmInformation.ShowDialog();
            }
        }
        #endregion

        #region >> MsgError - Error 메세지 리소스 설정 및 메세지 박스 호출
        /// <summary>
        /// Error 메세지 리소스 설정 및 메세지 박스 호출
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        public void MsgError(string _strMessageResourceCode)
        {
            this.MsgError(_strMessageResourceCode, string.Empty);
        }
        #endregion

        #region >> MsgError - Error 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// <summary>
        /// Error 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// </summary>
        /// <param name="_strMessage">팝업 메세지</param>
        /// <param name="_enumCodeMessage">코드, 메세지 구분</param>
        public void MsgError(string _strMessage, BaseEnumClass.CodeMessage _enumCodeMessage)
        {
            if (_enumCodeMessage == BaseEnumClass.CodeMessage.CODE)
            {
                this.MsgError(_strMessage);
            }
            else
            {
                using (uMsgBox frmError = new uMsgBox(_strMessage, MsgBoxKind.Error))
                {
                    frmError.ClickResult += FrmMsgBox_ClickResult;
                    frmError.ShowDialog();
                }
            }
        }
        #endregion

        #region >> MsgError - Error 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// <summary>
        /// Error 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        /// <param name="_strConditionValue">멀티 항목 조건값</param>
        public void MsgError(string _strMessageResourceCode, string _strConditionValue)
        {
            string strMessage   = string.Empty;

            if (_strConditionValue.Length == 0)
            {
                strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode);
            }
            else
            {
                strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode, _strConditionValue);
            }

            using (uMsgBox frmError = new uMsgBox(strMessage, MsgBoxKind.Error))
            {
                frmError.ClickResult += FrmMsgBox_ClickResult;
                frmError.ShowDialog();   
            }
        }
        #endregion

        #region >> MsgQuestion - Question 메세지 리소스 설정 및 메세지 박스 호출
        /// <summary>
        /// Question 메세지 리소스 설정 및 메세지 박스 호출
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        public void MsgQuestion(string _strMessageResourceCode)
        {
             this.MsgQuestion(_strMessageResourceCode, string.Empty);
        }
        #endregion

        #region >> MsgQuestion - Question 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// <summary>
        /// Question 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// </summary>
        /// <param name="_strMessage">팝업 메세지</param>
        /// <param name="_enumCodeMessage">코드, 메세지 구분</param>
        public void MsgQuestion(string _strMessage, BaseEnumClass.CodeMessage _enumCodeMessage)
        {
            if (_enumCodeMessage == BaseEnumClass.CodeMessage.CODE)
            {
                this.MsgQuestion(_strMessage);
            }
            else
            {
                using (uMsgBox frmQuestion = new uMsgBox(_strMessage, MsgBoxKind.Question))
                {
                    frmQuestion.ClickResult += FrmMsgBox_ClickResult;
                    frmQuestion.ShowDialog();
                }
            }
        }
        #endregion

        #region >> MsgQuestion - Question 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// <summary>
        /// Question 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        /// <param name="_strConditionValue">멀티 항목 조건값</param>
        public void MsgQuestion(string _strMessageResourceCode, string _strConditionValue)
        {
            string strMessage = string.Empty;

            if (_strConditionValue.Length == 0)
            {
                // 단일 메세지 처리
                strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode);
            }
            else
            {
                // 멀티 항목 메세지 처리
                strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode, _strConditionValue);
            }

            using (uMsgBox frmQuestion = new uMsgBox(strMessage, MsgBoxKind.Question))
            {
                frmQuestion.ClickResult += FrmMsgBox_ClickResult;
                frmQuestion.ShowDialog();
            }
        }
        #endregion

        #region >> Question 메시지 박스 Yes, No 여부값
        private void FrmMsgBox_ClickResult(bool _bResult)
        {
            this.BUTTON_CONFIRM_YN = _bResult;
        }
        #endregion
        #endregion

        #region > 암호화 / 복호화
        #region >> EncryptAES256 - AES256 암호화 (양방향)
        /// <summary>
        /// AES256 암호화 (양방향)
        /// </summary>
        /// <param name="_strToEncryptValue">암호화 대상값</param>
        /// <returns></returns>
        public string EncryptAES256(string _strToEncryptValue)
        {
            try
            {
                return Cryptography.AES.EncryptAES256(_strToEncryptValue);
            }
            catch { throw; }
        }
        #endregion

        #region >> DecryptAES256 - AES256 복호화
        /// <summary>
        /// AES256 복호화
        /// </summary>
        /// <param name="_strToDecryptValue">복호화 대상값</param>
        /// <returns></returns>
        public string DecryptAES256(string _strToDecryptValue)
        {
            try
            {
                return Cryptography.AES.DecryptAES256(_strToDecryptValue);
            }
            catch { throw; }
        }
        #endregion

        #region >> EncryptSHA256 - AES256 암호화 (단방향)
        /// <summary>
        /// AES256 암호화 (단방향)
        /// </summary>
        /// <param name="_strToencryptValue"></param>
        /// <returns></returns>
        public string EncryptSHA256(string _strToencryptValue)
        {
            try
            {
                return Cryptography.AES.EncryptSHA256(_strToencryptValue);
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 데이터 변환
        #region >> ConvertExcelToDataTable - 엑셀 데이터를 데이터테이블로 변환
        /// <summary>
        /// 엑셀 데이터를 데이터테이블로 변환
        /// </summary>
        /// <param name="_strFileName">Reference (엑셀 업로드 파일명)</param>
        /// <returns></returns>
        public DataTable ConvertExcelToDataTable(ref string _strFileName)
        {
            try
            {
                return ExcelUpload.ConvertExcelToDataTable(ref _strFileName);
            }
            catch { throw; }
        }
        #endregion

        #region >> DataTableToArrayList - DataTable To ArrayList 
        /// <summary>
        /// DataTable To ArrayList 
        /// </summary>
        /// <param name="_dtParam">변환 대상 데이터테이블</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> DataTableToArrayList(DataTable _dtParam)
        {
            return Data.DataConvert.ConvertDataTableToArrayList(_dtParam);
        }
        #endregion

        #region ConvertArrayListToDataTable - ArrayList To DataTable
        /// <summary>
        /// ArrayList To DataTable
        /// </summary>
        /// <param name="_liParam">변환전 리스트 변수</param>
        /// <returns></returns>
        public DataTable ConvertArrayListToDataTable(List<Dictionary<string, object>> _liParam)
        {
            return Data.DataConvert.ConvertArrayListToDataTable(_liParam);
        }
        #endregion

        #region >> ConvertDataTableToStringWriter - 데이터 테이블을 StringWrite형식으로 변환
        /// <summary>
        /// 데이터 테이블을 StringWrite형식으로 변환
        /// </summary>
        /// <param name="_dtExcelData">엑셀 데이터 (데이터 테이블)</param>
        /// <returns></returns>
        public StringWriter ConvertDataTableToStringWriter(DataTable _dtExcelData)
        {
            return ExcelUpload.ConvertDataTableToStringWriter(_dtExcelData);
        }
        #endregion

        #region >> ConvertStringToMediaBrush - 색상 문자열(헥사코드)을 SolidColorBrush형으로 반환
        /// <summary>
        /// 색상 문자열을 Brush 형으로 반환
        /// </summary>
        /// <param name="_strColorValue">색상 문자열</param>
        /// <returns></returns>
        public System.Windows.Media.Brush ConvertStringToMediaBrush(string _strColorValue)
        {
            return DataConvert.ConvertStringToMediaBrush(_strColorValue);
        }
        #endregion


        #region >> ConvertStringToSolidColorBrush - 색상 문자열을 Color object로 변환
        /// <summary>
        /// 색상 문자열을 Color object로 변환
        /// </summary>
        /// <param name="_strColorValue">색상 문자열</param>
        /// <returns></returns>
        public SolidColorBrush ConvertStringToSolidColorBrush(string _strColorValue)
        {
            return DataConvert.ConvertStringToSolidColorBrush(_strColorValue);
        }
        #endregion

        #region >> ConvertStringToMediaColor - 색상 문자열(헥사코드)을 (Media)Color형으로 반환
        /// <summary>
        /// 색상 문자열(헥사코드)을 (Media)Color형으로 반환
        /// </summary>
        /// <param name="_strColorValue">색상 문자열</param>
        /// <returns></returns>
        public System.Windows.Media.Color ConvertStringToMediaColor(string _strColorValue)
        {
            return DataConvert.ConvertStringToMediaColor(_strColorValue);
        }
        #endregion

        #region >> ConvertStringToDrawingColor - 색상 문자열(헥사코드)을 (Drawing)Color형으로 반환
        /// <summary>
        /// 색상 문자열(헥사코드)을 (Drawing)Color형으로 반환
        /// </summary>
        /// <param name="_strColorValue">색상 문자열</param>
        /// <returns></returns>
        public System.Drawing.Color ConvertStringToDrawingColor(string _strColorValue)
        {
            return DataConvert.ConvertStringToDrawingColor(_strColorValue);
        }
        #endregion

        #region > Imageurl - 이미지 테그 소스 (이미지)를 설정한다.
        /// <summary>
        /// 이미지 테그 소스 (이미지)를 설정한다.
        /// </summary>
        /// <param name="_imageCtrl">이미지 컨트롤</param>
        /// <param name="_strImageUrl">이미지 경로</param>
        public void ImageSource(System.Windows.Controls.Image _imageCtrl, string _strImageUrl)
        {
            try
            {
                Data.DataConvert.ImageSource(_imageCtrl, _strImageUrl);
            }
            catch { throw; }
        }
        #endregion

        #region 문자열을 특정값으로 Replace처리한다.
        /// <summary>
        /// 문자열을 특정값으로 Replace처리한다.
        /// </summary>
        /// <param name="_strValue"></param>
        /// <param name="_strOldChar"></param>
        /// <param name="_strNewChar"></param>
        /// <returns></returns>
        public string ReplaceText(TextEdit _ctrlTextEdit, string _strOldChar, string _strNewChar)
        {
            try
            {
                string strRtnValue   = string.Empty;

                if (_ctrlTextEdit.DisplayText.Trim().Length > 0)
                {
                    strRtnValue = _ctrlTextEdit.DisplayText.Trim().Replace(_strOldChar, _strNewChar);
                }

                return strRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 엑셀 다운로드
        #region > GetDataSetToExcelDownload - 데이터테이블 데이터를 엑셀로 변환후 다운로드
        /// <summary>
        /// 데이터테이블 데이터를 엑셀로 변환후 다운로드
        /// </summary>
        /// <param name="_dsParam">엑셀 다운로드 대상 데이터 셋</param>
        public void GetDataSetToExcelDownload(DataSet _dsParam)
        {
            try
            {
             //   var strNowDateTime  = DateTime.Now.ToString("yyyyMMdd_HHmmss");
             //   SaveFileDialog sfd  = new SaveFileDialog()
             //   {
             //       FileName            = strNowDateTime,
             //       DefaultExt          = ".xlsx",
             //       Filter              = "Excel Document|*.xlsx",
             //       InitialDirectory    = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).ToString()
             //   };

             //   Nullable<bool> result = sfd.ShowDialog();

             //   if (result == true)
             //   {
             //       using (var workbook = SpreadsheetDocument.Create(sfd.FileName, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
             //       {
             //           var workbookPart = workbook.AddWorkbookPart();

             //           workbook.WorkbookPart.Workbook          = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
             //           workbook.WorkbookPart.Workbook.Sheets   = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

             //           foreach (DataTable table in _dsParam.Tables)
             //           {
             //               var sheetPart       = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
					        //var sheetData       = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
					        //sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

					        //DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
					        //string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

					        //uint sheetId = 1;
					        //if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
					        //{
						       // sheetId = sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
					        //}

             //               DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
					        //sheets.Append(sheet);

					        //DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

					        //List<String> columns = new List<string>();

					        //foreach (System.Data.DataColumn column in table.Columns) 
             //               {
						       // columns.Add(column.ColumnName);
						       // DocumentFormat.OpenXml.Spreadsheet.Cell cell    = new DocumentFormat.OpenXml.Spreadsheet.Cell();
						       // cell.DataType                                   = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
						       // cell.CellValue                                  = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
						       // headerRow.AppendChild(cell);
					        //}

					        //sheetData.AppendChild(headerRow);

					        //foreach (System.Data.DataRow dsrow in table.Rows)
					        //{
						       // DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
						       // foreach (String col in columns)
						       // {
							      //  DocumentFormat.OpenXml.Spreadsheet.Cell cell    = new DocumentFormat.OpenXml.Spreadsheet.Cell();
							      //  cell.DataType                                   = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
							      //  cell.CellValue                                  = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString()); //
							      //  newRow.AppendChild(cell);
						       // }

						       // sheetData.AppendChild(newRow);
					        //}
             //           }
             //       }
             //   }
            }
            catch { throw; }
        }
        #endregion

        #region >> GetExcelDownload - (데이터 그리드) 엑셀 다운로드
        /// <summary>
        /// (데이터 그리드) 엑셀 다운로드
        /// </summary>
        /// <param name="_liTableView">데이터 그리드 뷰 배열</param>
        public void GetExcelDownload(List<TableView> _liTableView)
        {
            try
            {
                this.GetExcelDownload(_liTableView, BaseEnumClass.ExcelFileOpenType.None);
            }
            catch { throw; }
        }
        #endregion

        #region >> GetExcelDownload - (데이터 그리드) 엑셀 다운로드
        /// <summary>
        /// (데이터 그리드) 엑셀 다운로드
        /// </summary>
        /// <param name="_liTableView">데이터 그리드 뷰 배열</param>
        /// <param name="_enumExcelFileOpenType">엑셀 다운로드 파일 저장후 파일 오픈 여부</param>
        public void GetExcelDownload(List<TableView> _liTableView, BaseEnumClass.ExcelFileOpenType _enumExcelFileOpenType)
        {
            try
            {
                var strNowDateTime  = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                SaveFileDialog sfd  = new SaveFileDialog()
                {
                    FileName            = strNowDateTime,
                    DefaultExt          = ".xlsx",
                    Filter              = "Excel Document|*.xlsx",
                    InitialDirectory    = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).ToString()
                };

                Nullable<bool> result = sfd.ShowDialog();

                if (result == true)
                {
                    string strArray         = string.Empty;
                    int iTableViewCount     = _liTableView.Count;

                    // TableView 개수만큼 그리드 정보를 저장할 임시 파일명을 생성한다.
                    for (int i = 1; i <= iTableViewCount; i++)
                    {
                        strArray += string.Format("strFileName{0}|", i);
                    }

                    // TableView 개수만큼 파일명이 생성된다.
                    strArray = strArray.Substring(0, strArray.Length - 1);
                    string[] arr = strArray.Split('|');

                    for (int i = 0; i < iTableViewCount; i++)
                    {
                        // TableView 개수만큼 Grid의 값을 엑셀 형식으로 변환한다.
                        arr[i] = System.IO.Path.GetTempFileName();
                        ExcelDownload.ExportToXlsx(_liTableView[i], arr[i]);
                    }

                    // TableView 개수만큼 만들어진 엑셀 데이터를 한개의 엑셀파일에 머지한다.
                    switch (iTableViewCount)
                    {
                        case 1:
                            ExcelDownload.MergeXlsxFiles(sfd.FileName, arr[0]);
                            break;
                        case 2:
                            ExcelDownload.MergeXlsxFiles(sfd.FileName, arr[0], arr[1]);
                            break;
                        case 3:
                            ExcelDownload.MergeXlsxFiles(sfd.FileName, arr[0], arr[1], arr[2]);
                            break;
                        case 4:
                            ExcelDownload.MergeXlsxFiles(sfd.FileName, arr[0], arr[1], arr[2], arr[3]);
                            break;
                        default: break;
                    }

                    // 가상으로 만들어진 엑셀 데이터를 삭제한다.
                    for (int i = 0; i < iTableViewCount; i++)
                    {
                        ExcelDownload.DeleteFileIfExist(arr[i]);
                    }

                    if (_enumExcelFileOpenType == BaseEnumClass.ExcelFileOpenType.Open)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch { throw; }
        }
        #endregion

        #region >> GetExcelDownload - (트리 리스트 컨트롤) 엑셀 다운로드 
        /// <summary>
        /// (트리 리스트 컨트롤) 엑셀 다운로드 
        /// </summary>
        /// <param name="_liTreeListView">트리 리스트 뷰 배열</param>
        public void GetExcelDownload(List<TreeListView> _liTreeListView)
        {
            try
            {
                var strNowDateTime  = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                SaveFileDialog sfd  = new SaveFileDialog()
                {
                    FileName            = strNowDateTime,
                    DefaultExt          = ".xlsx",
                    Filter              = "Excel Document|*.xlsx",
                    InitialDirectory    = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).ToString()
                };

                Nullable<bool> result = sfd.ShowDialog();

                if (result == true)
                {
                    string strArray         = string.Empty;
                    int iTableViewCount     = _liTreeListView.Count;

                    // TableView 개수만큼 그리드 정보를 저장할 임시 파일명을 생성한다.
                    for (int i = 1; i <= iTableViewCount; i++)
                    {
                        strArray += string.Format("strFileName{0}|", i);
                    }

                    // TableView 개수만큼 파일명이 생성된다.
                    strArray = strArray.Substring(0, strArray.Length - 1);
                    string[] arr = strArray.Split('|');

                    for (int i = 0; i < iTableViewCount; i++)
                    {
                        // TableView 개수만큼 Grid의 값을 엑셀 형식으로 변환한다.
                        arr[i] = System.IO.Path.GetTempFileName();
                        ExcelDownload.ExportToXlsx(_liTreeListView[i], arr[i]);
                    }

                    // TableView 개수만큼 만들어진 엑셀 데이터를 한개의 엑셀파일에 머지한다.
                    switch (iTableViewCount)
                    {
                        case 1:
                            ExcelDownload.MergeXlsxFiles(sfd.FileName, arr[0]);
                            break;
                        case 2:
                            ExcelDownload.MergeXlsxFiles(sfd.FileName, arr[0], arr[1]);
                            break;
                        case 3:
                            ExcelDownload.MergeXlsxFiles(sfd.FileName, arr[0], arr[1], arr[2]);
                            break;
                        case 4:
                            ExcelDownload.MergeXlsxFiles(sfd.FileName, arr[0], arr[1], arr[2], arr[3]);
                            break;
                        default: break;
                    }

                    // 가상으로 만들어진 엑셀 데이터를 삭제한다.
                    for (int i = 0; i < iTableViewCount; i++)
                    {
                        ExcelDownload.DeleteFileIfExist(arr[i]);
                    }

                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 통신 관련
        public string PostJsonSource(string Url, string JsonData, string Encode, string ProxyServer, string Referer, ref CookieCollection CookieCollection, ref CookieContainer CookieContainer)
        {
            string retValue = string.Empty;
            int retryCount = this._retryCount;

            while (true)
            {
                try

                {

                    //Request개체생성

                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);

                    myRequest.ContentType = "application/json;charset=utf-8";

                    myRequest.Method = "POST";

                    myRequest.ServicePoint.Expect100Continue = false;

                    myRequest.CookieContainer = CookieContainer;

                    myRequest.Timeout = 600000;//10분

                    if (!string.IsNullOrEmpty(ProxyServer))

                    {

                        myRequest.Proxy = new WebProxy(ProxyServer);

                    }

                    if (!string.IsNullOrEmpty(Referer))

                    {

                        myRequest.Referer = Referer;

                    }



                    using (var streamWriter = new StreamWriter(myRequest.GetRequestStream()))

                    {

                        streamWriter.Write(JsonData);

                    }

                    var myResponse = (HttpWebResponse)myRequest.GetResponse();

                    myResponse.Cookies = myRequest.CookieContainer.GetCookies(myRequest.RequestUri);

                    CookieCollection.Add(myResponse.Cookies);



                    using (var streamReader = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.GetEncoding(Encode)))

                    {

                        retValue = streamReader.ReadToEnd();

                    }



                    if (retryCount != this._retryCount)

                    {

                        Console.WriteLine(string.Format("[{0}/{1}]WebException solved and resumed!", this._retryCount - retryCount, this._retryCount));

                    }

                    break;

                }

                catch (WebException err)

                {

                    if (err.Response != null)

                    {

                        using (var errorResponse = (HttpWebResponse)err.Response)

                        {

                            using (var reader = new StreamReader(errorResponse.GetResponseStream()))

                            {

                                retValue = reader.ReadToEnd();

                            }

                        }

                    }

                    else

                    {

                        retValue = err.Message;

                    }



                    //재시도처리

                    retryCount--;

                    if (retryCount < 0)

                    {

                        throw new Exception(string.Format("WebExceptionoccured...retry count {0} times exceeded...", this._retryCount), new WebException(retValue == "" ? err.Message : retValue));

                    }

                    else

                    {

                        Console.WriteLine(string.Format("[{0}/{1}]WebException occured...resting 5 seconds...{2}", this._retryCount - retryCount, this._retryCount, retValue == "" ? err.Message : retValue));

                        System.Threading.Thread.Sleep(1000 * 5);

                    }

                }

                catch (Exception err)

                {

                    err.HelpLink = Url;

                    retValue = err.Message;

                    throw;

                }
            }

            return retValue;
        }

        #region > PostSendJson - 헤더키를 전송하는 경우 (쿠팡만 사용)
        /// <summary>
        /// 헤더키를 전송하는 경우 (쿠팡만 사용)
        /// </summary>
        /// <param name="_strUrl">서버 URL</param>
        /// <param name="_strParamValue">파라메터, 값</param>
        /// <param name="_isAddHeaderKey">헤더키</param>
        /// <returns></returns>
        public string PostSendJson(string _strUrl, string _strParamValue)
        {
            try
            { 
                string strRtnValue          = string.Empty;

                HttpWebRequest  request     = (HttpWebRequest)WebRequest.Create(_strUrl);
                request.ContentType         = "application/json";
                request.Method              = "POST";

                using (StreamWriter stream = new StreamWriter(request.GetRequestStream()))
                {
                    stream.Write(_strParamValue);
                    stream.Flush();
                    stream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    strRtnValue = reader.ReadToEnd();
                    strRtnValue = JsonConvert.DeserializeObject(strRtnValue).ToString();
                }

                return strRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 그래프 색상 정의
        public SolidColorBrush DefineGraphColor(int _iValue)
        {
            SolidColorBrush solidColor = null;

            switch (_iValue)
            {
                case 0:
                    solidColor = this.ConvertStringToSolidColorBrush("#FF0000");
                    break;
                case 1:
                    solidColor = this.ConvertStringToSolidColorBrush("#3300FF");
                    break;
                case 2:
                    solidColor = this.ConvertStringToSolidColorBrush("#222222");
                    break;
                case 3:
                    solidColor = this.ConvertStringToSolidColorBrush("#009900");
                    break;
                case 4:
                    solidColor = this.ConvertStringToSolidColorBrush("#FFFF00");
                    break;
                case 5:
                    solidColor = this.ConvertStringToSolidColorBrush("#330000");
                    break;
            }

            return solidColor;
        }
        #endregion

        #region > 파일 관련
        #region > GetFileName - 파일명을 가져온다. (확장자 제외)
        /// <summary>
        /// 파일명을 가져온다. (확장자 제외)
        /// </summary>
        /// <param name="_strFileFullPath"></param>
        /// <returns></returns>
        public string GetFileName(string _strFileFullPath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(_strFileFullPath);
        }
        #endregion

        #region > GetFileNameExtension - 파일명을 가져온다. (확장자 포함)
        /// <summary>
        /// 파일명을 가져온다. (확장자 포함)
        /// </summary>
        /// <param name="_strFileFullPath"></param>
        /// <returns></returns>
        public string GetFileNameExtension(string _strFileFullPath)
        {
            return System.IO.Path.GetFileName(_strFileFullPath);
        }
        #endregion

        #region > GetDirectoryFileList - 폴더내 파일 리스트를 가져온다. (리스트 형식으로 반환)
        /// <summary>
        /// 폴더내 파일 리스트를 가져온다. (리스트 형식으로 반환)
        /// </summary>
        /// <param name="_strDirectoryPath">디렉토리 경로</param>
        /// <returns></returns>
        public List<string> GetDirectoryFileList(string _strDirectoryPath)
        {
            return File.File.GetDirectoryFileList(_strDirectoryPath);
        }
        #endregion

        #region > GetFileList - 파일 리스트를 조회한다.
        /// <summary>
        /// 파일 리스트를 조회한다.
        /// </summary>
        /// <param name="_strDirectoryPath"></param>
        /// <returns></returns>
        public List<string> GetFileList(string _strDirectoryPath)
        {
            return File.File.GetFileList(_strDirectoryPath);
        }
        #endregion

        #region > GetListOnDirectory - 폴더내 폴더리스트를 가져온다.
        /// <summary>
        /// 폴더내 폴더리스트를 가져온다.
        /// </summary>
        /// <param name="_strDirectoryPath">폴더</param>
        /// <returns></returns>
        public List<string> GetListOnDictionary(string _strDirectoryPath)
        {
            try
            {
                return File.File.GetListOnDictionary(_strDirectoryPath);
            }
            catch { throw; }
        }
        #endregion

        //#region > GetDeployFileList - 배포 대상 경로를 가져온다.
        ///// <summary>
        ///// 배포 대상 경로를 가져온다.
        ///// </summary>
        ///// <param name="_strDirectoryPath">조회 대상 디렉토리 경로</param>
        ///// <param name="_enumCreateTableSchemaKind">테이블 스키마 종류</param>
        ///// <returns></returns>
        //public DataTable GetDeployFileList(string _strDirectoryPath, BaseEnumClass.CreateTableSchemaKind _enumCreateTableSchemaKind)
        //{
        //    try
        //    {
        //        if (_enumCreateTableSchemaKind == BaseEnumClass.CreateTableSchemaKind.SERVER_DEPLOY_FILE_LIST)
        //        {
        //            return File.File.GetDeployServerFileList(_strDirectoryPath);
        //        }
        //        else
        //        {
        //            return File.File.GetDeployLocalFileList(_strDirectoryPath);
        //        }
        //    }
        //    catch { throw; }
        //}
        //#endregion

        #region WriteIniFile - INI파일 작성
        /// <summary>
        /// INI파일 작성
        /// </summary>
        /// <param name="_strFileName">파일명</param>
        /// <param name="_strKeyName">키값</param>
        /// <param name="_strValue">저장 대상값</param>
        public void WriteIniFile(string _strFileName, string _strKeyName, string _strValue)
        {
            try
            {
                File.File.WriteIniFile(_strFileName, _strKeyName, _strValue);
            }
            catch { throw; }
        }
        #endregion

        #region GetIniFile - INI파일 읽기
        /// <summary>
        /// INI파일 읽기
        /// </summary>
        /// <param name="_strFileName">파일명</param>
        /// <param name="_strKeyName">키값</param>
        /// <returns></returns>
        public string GetIniFile(string _strFileName, string _strKeyName)
        {
            try
            {
                return File.File.ReadIniFile(_strFileName, _strKeyName);
            }
            catch { throw; }
        }
        #endregion

        #region > CreateDirectory - 폴더를 생성한다. (폴더 존재여부 확인 후)
        /// <summary>
        /// 폴더를 생성한다. (폴더 존재여부 확인 후)
        /// </summary>
        /// <param name="_strDirectoryName">폴더명</param>
        public void CreateDirectory(string _strDirectoryName)
        {
            try
            {
                File.File.CreateDirectory(_strDirectoryName);
            }
            catch { throw; }
        }

        public void CreateDirectory(string _strDirectoryName, bool _isValue)
        {
            try
            {
                File.File.CreateDirectory(_strDirectoryName, _isValue);
            }
            catch { throw; }
        }
        #endregion

        #region > CompressToZip - 파일 압축
        /// <summary>
        /// 파일 압축
        /// </summary>
        /// <param name="_strSourcePath">압축 대상폴더</param>
        /// <param name="_strZipPath">압축 파일 백업폴더</param>
        public void CompressToZip(string _strSourcePath, string _strZipPath)
        {
            try
            {
                File.File.CompressToZip(_strSourcePath, _strZipPath);
            }
            catch { throw; }
        }
        #endregion

        #region > CompressToZip - 파일 압축 (여러개 파일을 하나의 압축파일로 변환)
        /// <summary>
        /// 파일 압축 (여러개 파일을 하나의 압축파일로 변환)
        /// </summary>
        /// <param name="_strFileName">파일명</param>
        /// <param name="_dicFileList">파일 리스트</param>
        public void CompressToZip(string _strFileName, Dictionary<string, byte[]> _dicFileList)
        {
            try
            {
                File.File.CompressToZip(_strFileName, _dicFileList);
            }
            catch { throw; }
        }
        #endregion

        #region > ArrayByteToFile - byte 배열 (압축파일) 데이터를 파일 형식으로 변환한다.
        /// <summary>
        /// byte 배열 (압축파일) 데이터를 파일 형식으로 변환한다.
        /// </summary>
        /// <param name="_strFilePath">파일경로/파일명</param>
        /// <param name="_arrByteZip">압축파일을 byte[] 형으로 변환한 데이터</param>
        /// <returns></returns>
        public bool ArrayByteToFile(string _strFilePath, byte[] _arrByteZip)
        {
            try
            {
                return File.File.ArrayByteToFile(_strFilePath, _arrByteZip);
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 기타
        #region >> CreateDataTableSchema - 데이터테이블 스키마 생성
        /// <summary>
        /// 데이터테이블 스키마 생성
        /// </summary>
        /// <param name="_dtNewTable">신규 데이터테이블</param>
        /// <param name="_enumCreateTableKind">데이터테이블 신규 스키마 종류</param>
        /// <returns></returns>
        public DataTable CreateDataTableSchema(DataTable _dtNewTable, BaseEnumClass.CreateTableSchemaKind _enumCreateTableSchemaKind)
        {
            return Utility.HelperClass.CreateDataTableSchema(_dtNewTable, _enumCreateTableSchemaKind);
        }
        #endregion
        #endregion

        #region  > 내부함수
        #region ConvertMsgBoxValue - 메세지 박스 값 코드에 맞는 리소스 값을 반환한다.
        /// <summary>
        /// 메세지 박스 값 코드에 맞는 리소스 값을 반환한다.
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        /// <returns></returns>
        private string ConvertMsgBoxValue(string _strMessageResourceCode)
        {
            return this.GetResourceValue(_strMessageResourceCode, BaseEnumClass.ResourceType.MESSAGE);
        }
        #endregion

        #region ConvertMsgBoxValue - 메세지 박스 값 코드에 맞는 리소스 값을 반환한다.
        /// <summary>
        /// 메세지 박스 값 코드에 맞는 리소스 값을 반환한다.
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        /// 
        /// <returns></returns>
        private string ConvertMsgBoxValue(string _strMessageResourceCode, string _strConditionValue)
        {
            string strResourceValue                 = string.Empty;
            string strConditionResourceValue        = string.Empty;
            string strMessageValue                  = this.GetResourceValue(_strMessageResourceCode, BaseEnumClass.ResourceType.MESSAGE);

            for (int i = 0; i < _strConditionValue.Split('|').Length; i++)
            {
                strConditionResourceValue  += this.GetResourceValue(_strConditionValue.Split('|')[i]) + "|";
            }

            if (strConditionResourceValue.Length > 0)
            {
                strConditionResourceValue = strConditionResourceValue.Substring(0, strConditionResourceValue.Length - 1);
            }


            for (int i = 0; i < strConditionResourceValue.Split('|').Length; i++)
            {

                strMessageValue = strMessageValue.Replace("{" + i.ToString() + "}", strConditionResourceValue.Split('|')[i]);
            }

            return strMessageValue;
        }
        #endregion

        #region >> IsLanguageKorean - 문자열에 한글 포함 여부를 체크한다.
        /// <summary>
        /// 문자열에 한글 포함 여부를 체크한다.
        /// </summary>
        /// <param name="_strValue">한글 체크 대상 문자열</param>
        /// <returns></returns>
        private bool IsLanguageKorean(string _strValue)
        {
            bool isLangKorean = false;

            char[] arrChar = _strValue.ToCharArray();
            foreach (char c in arrChar)
            {
                if (char.GetUnicodeCategory(c).Equals(System.Globalization.UnicodeCategory.OtherLetter) == true)
                {
                    isLangKorean = true;
                    break;
                }
            }

            return isLangKorean;
        }
        #endregion

        #region >> 날짜
        #region + ConvertStringToDate - 문자형을 일자형으로 변경 (날짜)
        /// <summary>
        /// 문자형을 일자형으로 변경 (날짜)
        /// </summary>
        /// <param name="_strValue">날짜 변환 대상 데이터</param>
        /// <returns></returns>
        public string ConvertStringToDate(string _strValue)
        {
            try
            {
                return DataConvert.ConvertStringToDate(_strValue);
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion

        #region > 로그
        #region >> Error - 에러 발생시 로그를 작성한다.
        /// <summary>
        /// 에러 발생시 로그를 작성한다.
        /// </summary>
        /// <param name="_exception">에러 객체</param>
        public void Error(System.Exception _exception)
        {
            SMART.WCS.Common.Logger.Logger.Error(_exception);
        }
        #endregion

        #region >> Error - 에러 발생시 로그를 작성한다. (키오스크용)
        /// <summary>
        /// 에러 발생시 로그를 작성한다. (키오스크용)
        /// </summary>
        /// <param name="_exception">에러 객체</param>
        public void Error(System.Exception _exception, BaseEnumClass.ScreenType _enumScreenType)
        {
            SMART.WCS.Common.Logger.Logger.Error(_exception, _enumScreenType);
        }
        #endregion

        #region >> Warning - 경고 로그를 작성한다.
        /// <summary>
        /// 경고 로그를 작성한다.
        /// </summary>
        /// <param name="_objWarning">경고 문구</param>
        public void Warning(object _objWarning)
        {
            SMART.WCS.Common.Logger.Logger.Warning(_objWarning);
        }
        #endregion

        #region >> Information - 정보 로그를 작성한다.
        /// <summary>
        /// 정보 로그를 작성한다.
        /// </summary>
        /// <param name="_objInfo">정보 문구</param>
        public void Info(object _objInfo)
        {
            SMART.WCS.Common.Logger.Logger.Info(_objInfo);
        }
        #endregion

        #region >> UserLog - 사용자 지정 로그를 작성한다.
        /// <summary>
        /// 사용자 지정 로그를 작성한다.
        /// </summary>
        /// <param name="_objInfo">로그 내용</param>
        /// <param name="_strDirectoryName">사용자 지정 폴더명</param>
        public void UserLog(object _objInfo, string _strDirectoryName)
        {
            try
            {
                SMART.WCS.Common.Logger.Logger.UserLog(_objInfo, _strDirectoryName);
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion
    }
}
