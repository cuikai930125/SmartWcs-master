using SMART.WCS.Common_Etc.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common_Etc.DataBase
{
    public class FirstDataAccess : DisposeClass
    {
        #region ▩ 전역변수
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 오라클 라이브러리 객체
        /// </summary>
        OracleLibrary g_oracleLibrary = null;

        /// <summary>
        /// MS-SQL 라이브러리 객체
        /// </summary>
        MSSqlLibrary g_msSqlLibrary = null;

        /// <summary>
        /// MariaDB 라이브러리 객체
        /// </summary>
        MariaDBLibrary g_mariaDBLibrary = null;
        #endregion

        public FirstDataAccess()
        {
            try
            {
                // App.config에 저장된 메인(기준) 데이터베이스 정보를 가져온다.
                var strMainDatabaseValue = this.BaseClass.MainDatabase;

                // 현재 접속한 데이터베이스 종류를 Attribute에 저장한다.
                // 동일 세션내에서 계속 사용하기 위해 Attribute에 저장
                this.SelectedDatabaseKindEnum = HelperClass.GetDatabaseKindValueByEnumType(strMainDatabaseValue);

                // 데이터베이스 연결 문자열 (복호화 된 데이터)
                this.ConnectionStringDecryptValue = Configuration.GetConnectionStringDecryptValue();

                switch (this.SelectedDatabaseKindEnum)
                {
                    case BaseEnumClass.SelectedDatabaseKind.ORACLE:
                        this.g_oracleLibrary = new OracleLibrary();
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        this.g_msSqlLibrary = new MSSqlLibrary();
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MARIA_DB:
                        this.g_mariaDBLibrary = new MariaDBLibrary();
                        break;
                }
            }
            catch { throw; }
        }

        #region ▩ 속성
        #region > ConnectionStringDecryptValue - 복호화 된 데이터베이스 연결 문자열
        /// <summary>
        /// 복호화 된 데이터베이스 연결 문자열
        /// </summary>
        public string ConnectionStringDecryptValue { get; set; }
        #endregion

        #region > DatabaseConnectionStringEncryptValue_ORACLE - 오라클 데이터베이스 연결 문자열 (암호화 데이터)
        /// <summary>
        /// 오라클 데이터베이스 연결 문자열 (암호화 데이터)
        /// </summary>
        public string DatabaseConnectionStringEncryptValue_ORACLE
        {
            get { return Base.Settings1.Default.DatabaseConnectionString_ORACLE; }
        }
        #endregion

        #region > DatabaseConnectionStringEncryptValue_MSSQL - MS-SQL 데이터베이스 연결 문자열 (암호화 데이터)
        /// <summary>
        /// MS-SQL 데이터베이스 연결 문자열 (암호화 데이터)
        /// </summary>
        public string DatabaseConnectionStringEncryptValue_MSSQL
        {
            get { return Base.Settings1.Default.DatabaseConnectionString_MSSQL; }
        }
        #endregion

        #region > DatabaseConnectionStringEncryptValue_MariaDB - MariaDB 데이터베이스 연결 문자열 (암호화 데이터)
        /// <summary>
        /// MariaDB 데이터베이스 연결 문자열 (암호화 데이터)
        /// </summary>
        public string DatabaseConnectionStringEncryptValue_MariaDB
        {
            get { return Base.Settings1.Default.DatabaseConnectionString_MariaDB; }
        }
        #endregion

        #region > SelectedDatabaseKindEnum - 현재 새션 데이터베이스 종류
        /// <summary>
        /// 현재 새션 데이터베이스 종류
        /// </summary>
        public BaseEnumClass.SelectedDatabaseKind SelectedDatabaseKindEnum { get; set; }
        #endregion
        #endregion

        #region > 데이터 핸들링
        #region >> GetSpDataSet - 데이터셋 형식으로 반환
        /// <summary>
        /// 데이터셋 형식으로 반환
        /// </summary>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_arrOutputParam">Output 파라메터</param>
        /// <returns></returns>
        public DataSet GetSpDataSet(string _strProcedureName, Dictionary<string, object> _dicInputParam, string[] _arrOutputParam)
        {
            try
            {
                var dsRtnValue = new DataSet();

                switch (this.SelectedDatabaseKindEnum)
                {
                    case BaseEnumClass.SelectedDatabaseKind.ORACLE:
                        if (this.g_oracleLibrary == null)
                        {
                            this.g_oracleLibrary = new OracleLibrary();
                        }

                        dsRtnValue = this.g_oracleLibrary.GetDataSet(this.ConnectionStringDecryptValue, _strProcedureName, _dicInputParam, _arrOutputParam);
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        if (this.g_msSqlLibrary == null)
                        {
                            this.g_msSqlLibrary = new MSSqlLibrary();
                        }

                        dsRtnValue = this.g_msSqlLibrary.GetDataSet(this.ConnectionStringDecryptValue, _strProcedureName, _dicInputParam);
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MARIA_DB:
                        if (this.g_mariaDBLibrary == null)
                        {
                            this.g_mariaDBLibrary = new MariaDBLibrary();
                        }

                        dsRtnValue = this.g_mariaDBLibrary.GetDataSet(this.ConnectionStringDecryptValue, _strProcedureName, _dicInputParam);
                        break;
                }

                return dsRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> GetSpDataTable - 데이터 테이블 형식으로 반환
        /// <summary>
        /// 데이터 테이블 형식으로 반환
        /// </summary>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_arrOutputParam">Output 파라메터</param>
        /// <returns></returns>
        public DataTable GetSpDataTable(string _strProcedureName, Dictionary<string, object> _dicInputParam, string[] _arrOutputParam)
        {
            try
            {
                var dtRtnValue = new DataTable();

                switch (this.SelectedDatabaseKindEnum)
                {
                    case BaseEnumClass.SelectedDatabaseKind.ORACLE:
                        if (this.g_oracleLibrary == null)
                        {
                            this.g_oracleLibrary = new OracleLibrary();
                        }

                        dtRtnValue = this.g_oracleLibrary.GetDataTable(this.ConnectionStringDecryptValue, _strProcedureName, _dicInputParam, _arrOutputParam);
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        if (this.g_msSqlLibrary == null)
                        {
                            this.g_msSqlLibrary = new MSSqlLibrary();
                        }

                        dtRtnValue = this.g_msSqlLibrary.GetDataTable(this.ConnectionStringDecryptValue, _strProcedureName, _dicInputParam);
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MARIA_DB:
                        if (this.g_mariaDBLibrary == null)
                        {
                            this.g_mariaDBLibrary = new MariaDBLibrary();
                        }

                        dtRtnValue = this.g_mariaDBLibrary.GetDataTable(this.ConnectionStringDecryptValue, _strProcedureName, _dicInputParam);
                        break;
                }

                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion
    }
}
