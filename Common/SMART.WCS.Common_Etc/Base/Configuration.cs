using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common_Etc
{
    public class Configuration
    {
        #region GetConnectionStringDecryptValue - 구성 파일에서 암호화 된 연결 문자열을 복호화하여 가져온다.
        /// <summary>
        /// 구성 파일에서 암호화 된 연결 문자열을 복호화하여 가져온다. (로그인 화면 로드시)
        /// </summary>
        /// <returns>복호화 된 문자열</returns>
        public static string GetConnectionStringDecryptValue()
        {
            try
            {
                // connectionString 값을 조회한다.
                var strConnectionStringEncryptValue = System.Configuration.ConfigurationManager.ConnectionStrings["WCS"].ToString();

                // 복호화 로직을 수행한다.
                return Cryptography.AES.DecryptAES256(strConnectionStringEncryptValue);
            }
            catch { throw; }
        }
        #endregion

        #region GetConnectionStringDecryptValue - 구성 파일에서 암호화 된 연결 문자열을 복호화하여 가져온다.
        /// <summary>
        /// 구성 파일에서 암호화 된 연결 문자열을 복호화하여 가져온다. (로그인 화면 로드시)
        /// </summary>
        /// <param name="_enumDatabaseKindShort">데이터베이스 종류</param>
        /// <returns>복호화 된 문자열</returns>
        public static string GetConnectionStringDecryptValue(string _strDatabaseConnectionType_DEV_REAL)
        {
            try
            {
                if (_strDatabaseConnectionType_DEV_REAL.Length == 0)
                {
                    _strDatabaseConnectionType_DEV_REAL = Configuration.GetAppSettings("DBConnectType_DEV_REAL");
                }

                _strDatabaseConnectionType_DEV_REAL = _strDatabaseConnectionType_DEV_REAL.Equals("DEV") ? "WCS_DEV" : "WCS_REAL";

                //// 데이터 베이스 연결 문자열을 조회한다.
                //var strDatabaseConnectionTagName        = Utility.HelperClass.GetDatabaseConfigurationTagName(_strDatabaseConnectionType_DEV_REAL);

                // connectionString 값을 조회한다.
                var strConnectionStringEncryptValue     = System.Configuration.ConfigurationManager.ConnectionStrings[_strDatabaseConnectionType_DEV_REAL].ToString();

                // 복호화 로직을 수행한다.
                return Cryptography.AES.DecryptAES256(strConnectionStringEncryptValue);
            }
            catch { throw; }
        }
        #endregion

        #region GetAppSettings - 구성 파일에서 AppSettings 값 가져온다. 
        /// <summary>
        /// 구성 파일에서 AppSettings 값 가져온다. 
        /// </summary>
        /// <param name="_strTagName">연결문자열 테그명</param>
        /// <returns>String - AppSettings값을 리턴한다.</returns>
        public static string GetAppSettings(string _strTagName)
        {
            try
            {
                string strRtnValue      = string.Empty;
                strRtnValue             = System.Configuration.ConfigurationManager.AppSettings[_strTagName];       // 테그명 (_strTagName) 조건에 맞는 AppSettings값을 가져온다.

                // AppSettings값이 비어있는 경우 예러처리를 한다.
                if (strRtnValue.Length == 0)
                {
                    ExceptionProcess(BaseEnumClass.ErrorConnectType.APP_SETTINGS_VALUE_IS_EMPTY);
                }

                return strRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region GetAppSettingsStringDecrypt - 구성 파일에서 암호화 된 AppSettings 값을 가져온 후 복호화하여 반환한다.
        /// <summary>
        /// 구성 파일에서 암호화 된 AppSettings 값을 가져온 후 복호화하여 반환한다.
        /// </summary>
        /// <param name="_strTagName">AppSettings 테그명</param>
        /// <returns></returns>
        public static string GetAppSettingsStringDecrypt(string _strTagName)
        {
            try
            {
                var strAppSettingByTagName = System.Configuration.ConfigurationManager.AppSettings[_strTagName];
                // 복호화 로직을 수행한다.
                return Cryptography.AES.DecryptAES256(strAppSettingByTagName);
            }
            catch { throw; }
        }
        #endregion

        #region ExceptionProcess - 오류 타입별로 임의로 에러를 발생시키는 구문
        /// <summary>
        /// 오류 타입별로 임의로 에러를 발생시키는 구문
        /// </summary>
        /// <param name="_enumErrorType">오류 타입</param>
        public static void ExceptionProcess(BaseEnumClass.ErrorConnectType _enumErrorType)
        {
            try
            {
                if (_enumErrorType == BaseEnumClass.ErrorConnectType.CONNECTION_STRINGS_VALUE_IS_EMPTY)
                {
                    // ConnectionStrings 값 (DB연결 문자열)이 없는 경우 데이터베이스 접속을 할 수 없기 때문에 에러처리를 한다.
                    throw new System.Exception("ConnectionString_value_is_empty");
                }
                else if (_enumErrorType == BaseEnumClass.ErrorConnectType.APP_SETTINGS_VALUE_IS_EMPTY)
                {
                    // AppSettings 값이 없는 경우 에러처리를 한다.
                    throw new System.Exception("AppSettings_(Triple_DES)_value_is_empty");
                }
                else if (_enumErrorType == BaseEnumClass.ErrorConnectType.ENCRYPTION_VALUE_IS_EMPTY)
                {
                    // 암호화 한 데이터가 없는 경우 암호화가 실패한 것으로 판단되어 에러처리를 한다.
                    throw new System.Exception("Encryption_value_is_empty");
                }
                else if (_enumErrorType == BaseEnumClass.ErrorConnectType.DESCRYPTION_VALUE_IS_EMPTY)
                {
                    // 복호화 한 데이터가 없는 경우 암호화가 실패한 것으로 판단되어 에러처리를 한다.
                    throw new System.Exception("Descryption_value_is_empty");
                }
            }
            catch { throw; }
        }
        #endregion
    }
}
