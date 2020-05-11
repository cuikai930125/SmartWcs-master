using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common_Etc.Utility
{
    public class HelperClass
    {
        #region ▩ 전역변수
        BaseClass BaseClass = new BaseClass();
        #endregion

        #region ▩ 속성
       
        #endregion

        #region ▩ 함수
        #region > 시스템 관련 함수
        #region >> GetMacAddress - 맥 어드레스 주소 조회한다.
        /// <summary>
        /// 맥 어드레스 주소 조회한다.
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            try
            {
                return ConvertMacAddressFormat(NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString());
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 데이터베이스 연결 문자열 관련
        #region >> GetDatabaseConfigurationTagNameList - 데이터베이스 연결문자열 리스트를 조회한다.
        /// <summary>
        /// 데이터베이스 연결문자열 리스트를 조회한다.
        /// </summary>
        /// <returns></returns>
        private static List<string> GetDatabaseConfigurationTagNameList()
        {
            try
            {
                List<string> liReturnTagName    = new List<string>();

                foreach (var configItem in System.Configuration.ConfigurationManager.ConnectionStrings)
                {
                    liReturnTagName.Add(((System.Configuration.ConnectionStringSettings)configItem).Name);
                }

                return liReturnTagName;
            }
            catch { throw; }
        }
        #endregion

        #region >> GetDatabaseConfigurationTagName - 데이터베이스 연결문자열 테그를 조회한다.
        /// <summary>
        /// 데이터베이스 연결문자열 테그를 조회한다.
        /// </summary>
        /// <param name="_strDatabaseConnectionType_DEV_REAL">데이터베이스 접속 타입 (DEV-개발, REAL-운영)</param>
        /// <returns></returns>
        public static string GetDatabaseConfigurationTagName(string _strDatabaseConnectionType_DEV_REAL)
        {
            try
            {
                // 공백으로 넘어오는 경우는 REAL(운영 DB)로 인식하도록 값을 변경한다.
                _strDatabaseConnectionType_DEV_REAL = _strDatabaseConnectionType_DEV_REAL.Equals("DEV") ? "DEV" : "REAL"; 

                // App.config파일 내 설정된 데이터베이스 테그명을 리스트 형식으로 가져온다.
                var liDatabaseConfigurationTagName = GetDatabaseConfigurationTagNameList();
                
                // 리스트 변수에서 데이터베이스 종류와 접속 타입이 일치하는 값을 구한다.
                return liDatabaseConfigurationTagName.Where(p => p.Contains(_strDatabaseConnectionType_DEV_REAL)).FirstOrDefault();
            }
            catch { throw; }
        }
        #endregion

        #region >> GetDatabaseKindValueByEnumType - 데이터베이스 종류 문자열 데이터를 Enum형식으로 변경한다.
        /// <summary>
        /// 데이터베이스 문자열 데이터를 Enum형식으로 변경한다.
        /// </summary>
        /// <param name="_strDatabaseKind">데이터베이스 종류 문자열</param>
        /// <returns></returns>
        public static BaseEnumClass.SelectedDatabaseKind GetDatabaseKindValueByEnumType(string _strDatabaseKind)
        {
            try
            {
                BaseEnumClass.SelectedDatabaseKind enumDatabaseKind = BaseEnumClass.SelectedDatabaseKind.NONE;

                switch (_strDatabaseKind.ToUpper())
                {
                    case "ORACLE":
                        enumDatabaseKind = BaseEnumClass.SelectedDatabaseKind.ORACLE;
                        break;
                    case "MSSQL":
                        enumDatabaseKind = BaseEnumClass.SelectedDatabaseKind.MS_SQL;
                        break;
                    case "MARIADB":
                        enumDatabaseKind = BaseEnumClass.SelectedDatabaseKind.MARIA_DB;
                        break;
                }

                return enumDatabaseKind;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 리소스 관련 함수
        #region >> GetResource - 리소스 내용 반환
        /// <summary>
        /// 리소스 내용 반환
        /// </summary>
        /// <param name="_strResourcePath">리소스 경로</param>
        /// <returns></returns>
        public static string GetResource(string _strResourcePath)
        {
            try
            {
                var strRtnValue     = string.Empty;
                var assembly        = Assembly.GetExecutingAssembly();

                using (Stream st = assembly.GetManifestResourceStream(_strResourcePath))
                {
                    using (StreamReader sr = new StreamReader(st))
                    {
                        strRtnValue = sr.ReadToEnd();
                    }
                }

                return strRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> GetCurrentCultrueInfo - 현재 시스템 설정 언어를 가져온다.
        /// <summary>
        /// 현재 시스템 설정 언어를 가져온다.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentCultureInfo()
        {
            var strCurrentCultureName   = CultureInfo.CurrentCulture.Name;
            return Data.DataConvert.GetSplitToString(strCurrentCultureName, '-', 1);
        }
        #endregion

        

        #region >> GetCultureInfo - 국가코드와 일치하는 언어 리소스를 가져온다.
        /// <summary>
        /// 국가코드와 일치하는 코드를 가져온다.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetCountryName(string _strCntryCode)
        {
            CultureInfo ci = null;

            switch (_strCntryCode)
            {
                case "KR":
                    ci = new CultureInfo("ko-KR");
                    break;
                case "EN":
                    ci = new CultureInfo("en-US");
                    break;
                case "CN":
                    break;
                case "TH":
                    ci = new CultureInfo("th-TH");
                    break;
            }

            return ci;
        }
        #endregion

        #region >> GetCultureInfo - 국가코드와 일치하는 언어 리소스를 가져온다.
        /// <summary>
        /// 국가코드와 일치하는 코드를 가져온다.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetCountryNameEmptyCntryCD()
        {
            BaseClass BaseClass = new BaseClass();

            CultureInfo ci = null;

            switch (BaseClass.CountryCode)
            {
                case "KR":
                    ci = new CultureInfo("ko-KR");
                    break;
                case "EN":
                    ci = new CultureInfo("en-US");
                    break;
                case "CN":
                    break;
                case "TH":
                    ci = new CultureInfo("th-TH");
                    break;
            }

            return ci;
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
        public static DataTable CreateDataTableSchema(DataTable _dtNewTable, BaseEnumClass.CreateTableSchemaKind _enumCreateTableSchemaKind)
        {
            if (_dtNewTable == null) { _dtNewTable = new DataTable(); }

            switch (_enumCreateTableSchemaKind)
            {
                case BaseEnumClass.CreateTableSchemaKind.COMMON_CODE: // 공통코드 스키마
                    _dtNewTable.Columns.Add("CODE",     typeof(string));    // 코드
                    _dtNewTable.Columns.Add("NAME",     typeof(string));    // 명
                    break;

                case BaseEnumClass.CreateTableSchemaKind.DEPLOY_FILE_LIST:
                    _dtNewTable.Columns.Add("FILE_NAME",             typeof(string));   // 파일명
                    _dtNewTable.Columns.Add("FILE_EXTENSION",        typeof(string));   // 파일 확장자
                    _dtNewTable.Columns.Add("FILE_LAST_MODIFY_DT",   typeof(string));   // 파일 최종 수정일시
                    _dtNewTable.Columns.Add("FILE_DIRECTORY",        typeof(string));   // 파일 디렉토리
                    _dtNewTable.Columns.Add("COMPARE_RESULT",        typeof(string));   // 파일 수정 결과
                    break;
            }

            return _dtNewTable;
        }
        #endregion
        #endregion

        #region > 내부함수
        #region ConvertMacAddressFormat - 맥 어드레스 주소 포멧을 변경한다.
        /// <summary>
        /// 맥 어드레스 주소 포멧을 변경한다.
        /// </summary>
        /// <param name="_strMacAddress">맥 어드레스 주소</param>
        /// <returns></returns>
        private static string ConvertMacAddressFormat(string _strMacAddress)
        {
            string strRtnValue = string.Empty;

            char[] chrArr = _strMacAddress.ToCharArray();
            for (int i = 0; i < chrArr.Length; i++)
            {
                if (i % 2 == 0)
                {
                    strRtnValue += chrArr[i].ToString();
                }
                else
                {
                    strRtnValue += chrArr[i].ToString();

                    if (i != chrArr.Length - 1)
                    {
                        strRtnValue += "-";
                    }
                }
            }
            return strRtnValue;
        }
        #endregion

        /// <summary>
        /// 프로시저 로그 정보를 저장한다.
        /// </summary>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        public static void ProcedureLogInfo(string _strDatabaseKind, string _strCenterCD, string _strProcedureName, Dictionary<string, object> _dicInputParam)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Microsoft.VisualBasic.ControlChars.CrLf);
            sb.Append($"데이터베이스 : {_strDatabaseKind}");
            sb.Append(Microsoft.VisualBasic.ControlChars.CrLf);
            sb.Append($"센터 : {_strCenterCD}");
            sb.Append(Microsoft.VisualBasic.ControlChars.CrLf);
            sb.Append($"프로시저명 : {_strProcedureName}");
            sb.Append(Microsoft.VisualBasic.ControlChars.CrLf);

            foreach (var item in _dicInputParam)
            {
                sb.Append($"Key : {item.Key.ToString()}          value : {item.Value.ToString()}");
                sb.Append(Microsoft.VisualBasic.ControlChars.CrLf);
            }

            Logger.Logger.InputParamInfo(sb.ToString());
        }
        #endregion
        #endregion
    }
}
