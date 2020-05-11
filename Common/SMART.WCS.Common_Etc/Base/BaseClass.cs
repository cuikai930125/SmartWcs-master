using SMART.WCS.Common_Etc.Controls;
using SMART.WCS.Common_Etc.Data;
using SMART.WCS.Common_Etc.File;
using SMART.WCS.Common_Etc.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Net.Sockets;

namespace SMART.WCS.Common_Etc
{
    public class BaseClass : DisposeClass
    {
        private int g_iRetryCount = 0;

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

        #region RoleCode - 권한 코드
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

            ResourceManager rm = new ResourceManager("SMART.WCS.Resource.Language.LanguageResource", typeof(SMART.WCS.Resource.Language.LanguageResource).Assembly);
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

            ResourceManager rm = new ResourceManager("SMART.WCS.Resource.Language.LanguageResource", typeof(SMART.WCS.Resource.Language.LanguageResource).Assembly);
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
        #endregion

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
                return SMART.WCS.Common_Etc.Configuration.GetAppSettings(_strTagName);
            }
            catch { throw; }
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
        public void BindingFirstComboBox(ComboBox _ctrlComboBox, string _strCommonCode)
        {
            CommonComboBox.BindingFirstComboBox(_ctrlComboBox, _strCommonCode);
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
                    ComboBox _ctrlComboBox
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
                    ComboBox _ctrlComboBox
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
        public string ComboBoxSelectedKeyValue(ComboBox _ctrlComboBoxEdit)
        {
            if (this.ComboBoxItemCount(_ctrlComboBoxEdit)== 0) { return string.Empty; }
            return _ctrlComboBoxEdit.SelectedItem.ToString();
        }
        #endregion

        #region >> ComboBoxSelectedDisplayValue - 콤보박스 컨트롤의 현재 선택된 Row의 명을 가져온다.
        /// <summary>
        /// 콤보박스 컨트롤의 현재 선택된 Row의 명을 가져온다.
        /// </summary>
        /// <param name="_ctrlComboBoxEdit">콤보박스 컨트롤 객체</param>
        /// <returns></returns>
        public string ComboBoxSelectedDisplayValue(ComboBox _ctrlComboBoxEdit)
        {
            if (this.ComboBoxItemCount(_ctrlComboBoxEdit) == 0) { return string.Empty; }
            return _ctrlComboBoxEdit.Text;
        }
        #endregion

        #region >> ComboBoxItemCount - 콤보박스 항목 (Item) 개수
        /// <summary>
        /// ComboBoxItemCount - 콤보박스 항목 (Item) 개수
        /// </summary>
        /// <param name="_ctrlComboBoxEdit">콤보박스 컨트롤</param>
        /// <returns></returns>
        public int ComboBoxItemCount(ComboBox _ctrlComboBoxEdit)
        {
            var liList = _ctrlComboBoxEdit.ItemsSource as List<ComboBox>;

            // 콤보박스의 ItemsSource가 null인경우 0을 리턴한다.
            if (liList == null) { return 0; }

            return liList.Count;
        }
        #endregion

        #region ModifyComboBoxColumnHeaderName - 공통 콤보박스가 아닌 경우 DataTable 컬럼명을 강제로 설정하기 위한 함수
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
        ///// <summary>
        ///// Information 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        ///// </summary>
        ///// <param name="_strMessage">팝업 메세지</param>
        ///// <param name="_enumCodeMessage">코드, 메세지 여부</param>
        //public void MsgInfo(string _strMessage, BaseEnumClass.CodeMessage _enumCodeMessage)
        //{
        //    if (_enumCodeMessage == BaseEnumClass.CodeMessage.CODE)
        //    {
        //        this.MsgInfo(_strMessage);
        //    }
        //    else
        //    {
        //        using (uMsgBox frmInformation = new uMsgBox(_strMessage, MsgBoxKind.Information))
        //        {
        //            frmInformation.ClickResult += FrmMsgBox_ClickResult;
        //            frmInformation.ShowDialog();
        //        }
        //    }
        //}
        #endregion

        #region >> MsgInfo - Information 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// <summary>
        /// Information 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        /// <param name="_strConditionValue">멀티 항목 조건값</param>
        public void MsgInfo(string _strMessageResourceCode, string _strConditionValue)
        {
            //string strMessage = string.Empty;

            //if (_strConditionValue.Length == 0)
            //{
            //    strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode);
            //}
            //else
            //{
            //    strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode, _strConditionValue);
            //}

            //using (uMsgBox frmInformation = new uMsgBox(strMessage, MsgBoxKind.Information))
            //{
            //    frmInformation.ClickResult += FrmMsgBox_ClickResult;
            //    frmInformation.ShowDialog();
            //}
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
            //if (_enumCodeMessage == BaseEnumClass.CodeMessage.CODE)
            //{
            //    this.MsgError(_strMessage);
            //}
            //else
            //{
            //    using (uMsgBox frmError = new uMsgBox(_strMessage, MsgBoxKind.Error))
            //    {
            //        frmError.ClickResult += FrmMsgBox_ClickResult;
            //        frmError.ShowDialog();
            //    }
            //}
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
            //string strMessage   = string.Empty;

            //if (_strConditionValue.Length == 0)
            //{
            //    strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode);
            //}
            //else
            //{
            //    strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode, _strConditionValue);
            //}

            //using (uMsgBox frmError = new uMsgBox(strMessage, MsgBoxKind.Error))
            //{
            //    frmError.ClickResult += FrmMsgBox_ClickResult;
            //    frmError.ShowDialog();   
            //}
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
            //if (_enumCodeMessage == BaseEnumClass.CodeMessage.CODE)
            //{
            //    this.MsgQuestion(_strMessage);
            //}
            //else
            //{
            //    using (uMsgBox frmQuestion = new uMsgBox(_strMessage, MsgBoxKind.Question))
            //    {
            //        frmQuestion.ClickResult += FrmMsgBox_ClickResult;
            //        frmQuestion.ShowDialog();
            //    }
            //}
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
            //string strMessage = string.Empty;

            //if (_strConditionValue.Length == 0)
            //{
            //    // 단일 메세지 처리
            //    strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode);
            //}
            //else
            //{
            //    // 멀티 항목 메세지 처리
            //    strMessage = this.ConvertMsgBoxValue(_strMessageResourceCode, _strConditionValue);
            //}

            //using (uMsgBox frmQuestion = new uMsgBox(strMessage, MsgBoxKind.Question))
            //{
            //    frmQuestion.ClickResult += FrmMsgBox_ClickResult;
            //    frmQuestion.ShowDialog();
            //}
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

        #region > ConvertDataTableToStringWriter - 데이터 테이블을 StringWrite형식으로 변환
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

        #region > ConvertStringToMediaBrush - 색상 문자열(헥사코드)을 SolidColorBrush형으로 반환
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

        #region > ConvertStringToMediaColor - 색상 문자열(헥사코드)을 (Media)Color형으로 반환
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

        #region > ConvertStringToDrawingColor - 색상 문자열(헥사코드)을 (Drawing)Color형으로 반환
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
        #endregion
        
        #region > 통신 관련
        public string PostJsonSource(string Url, string JsonData, string Encode, string ProxyServer, string Referer, ref CookieCollection CookieCollection, ref CookieContainer CookieContainer)
        {
            string retValue = string.Empty;
            int retryCount = this.g_iRetryCount;

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



                    if (retryCount != this.g_iRetryCount)

                    {

                        Console.WriteLine(string.Format("[{0}/{1}]WebException solved and resumed!", this.g_iRetryCount - retryCount, this.g_iRetryCount));

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

                        throw new Exception(string.Format("WebExceptionoccured...retry count {0} times exceeded...", this.g_iRetryCount), new WebException(retValue == "" ? err.Message : retValue));

                    }

                    else

                    {

                        Console.WriteLine(string.Format("[{0}/{1}]WebException occured...resting 5 seconds...{2}", this.g_iRetryCount - retryCount, this.g_iRetryCount, retValue == "" ? err.Message : retValue));

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

        #region PostSendJson - Json을 이용하여 서버로 데이터 전송
        /// <summary>
        /// Json을 이용하여 서버로 데이터 전송
        /// </summary>
        /// <param name="_strUrl">서버 URL</param>
        /// <param name="_strParamValue">파라메터, 값</param>
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

        #region > GetDeployFileList - 배포 대상 경로를 가져온다.
        /// <summary>
        /// 배포 대상 경로를 가져온다.
        /// </summary>
        /// <param name="_strDirectoryPath">조회 대상 디렉토리 경로</param>
        /// <param name="_enumCreateTableSchemaKind">테이블 스키마 종류</param>
        /// <returns></returns>
        public DataTable GetDeployFileList(string _strDirectoryPath, BaseEnumClass.CreateTableSchemaKind _enumCreateTableSchemaKind)
        {
            try
            {
                if (_enumCreateTableSchemaKind == BaseEnumClass.CreateTableSchemaKind.SERVER_DEPLOY_FILE_LIST)
                {
                    return File.File.GetDeployServerFileList(_strDirectoryPath);
                }
                else
                {
                    return File.File.GetDeployLocalFileList(_strDirectoryPath);
                }
            }
            catch { throw; }
        }
        #endregion
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
        #endregion

        public void CreateDirectory(string _strDirectoryName, bool _isValue)
        {
            try
            {
                File.File.CreateDirectory(_strDirectoryName, _isValue);
            }
            catch { throw; }
        }

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
        #endregion

        #region > 로그
        #region >> Error - 에러 발생시 로그를 작성한다.
        /// <summary>
        /// 에러 발생시 로그를 작성한다.
        /// </summary>
        /// <param name="_exception">에러 객체</param>
        public void Error(System.Exception _exception)
        {
            SMART.WCS.Common_Etc.Logger.Logger.Error(_exception);
        }
        #endregion

        #region >> Warning - 경고 로그를 작성한다.
        /// <summary>
        /// 경고 로그를 작성한다.
        /// </summary>
        /// <param name="_objWarning">경고 문구</param>
        public void Warning(object _objWarning)
        {
            SMART.WCS.Common_Etc.Logger.Logger.Warning(_objWarning);
        }
        #endregion

        #region >> Information - 정보 로그를 작성한다.
        /// <summary>
        /// 정보 로그를 작성한다.
        /// </summary>
        /// <param name="_objInfo">정보 문구</param>
        public void Info(object _objInfo)
        {
            SMART.WCS.Common_Etc.Logger.Logger.Info(_objInfo);
        }
        #endregion
        #endregion
        #endregion
    }
}
