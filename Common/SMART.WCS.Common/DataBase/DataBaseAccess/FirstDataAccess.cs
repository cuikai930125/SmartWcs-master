using SMART.WCS.Common.Utility;

using System.Collections.Generic;
using System.Data;

namespace SMART.WCS.Common.DataBase
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

        #region ▩ 생성자
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
        #endregion

        #region ▩ 속성
        #region > 트랜잭션 상태
        #region >> TransactionState_Oracle - 오라클 트랜잭션 상태
        /// <summary>
        /// 오라클 트랜잭션 상태
        /// </summary>
        public BaseEnumClass.TransactionState_ORACLE TransactionState_Oracle
        {
            get { return this.g_oracleLibrary.TransactionState; }
        }
        #endregion

        #region >> TransactionState_MSSQL - MS-SQL 트랜잭션 상태 
        /// <summary>
        /// MS-SQL 트랜잭션 상태 
        /// </summary>
        public BaseEnumClass.TransactionState_MSSQL TransactionState_MSSQL
        {
            get { return this.g_msSqlLibrary.TransactionState; }
        }
        #endregion

        #region >> TransactionState_MariaDB - MariaDB 트랜잭션 상태
        /// <summary>
        /// MariaDB 트랜잭션 상태
        /// </summary>
        public BaseEnumClass.TransactionState_MariaDB TransactionState_MariaDB
        {
            get { return this.g_mariaDBLibrary.TransactionState; }
        }
        #endregion
        #endregion

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
        #region > 트랜잭션
        #region >> BeginTransaction - 트랜잭션 시작
        public void BeginTransaction()
        {
            try
            {
                switch (this.SelectedDatabaseKindEnum)
                {
                    case BaseEnumClass.SelectedDatabaseKind.ORACLE:
                        if (this.g_oracleLibrary == null)
                        {
                            using (this.g_oracleLibrary = new OracleLibrary())
                            {
                                this.g_oracleLibrary.BeginTransaction(this.ConnectionStringDecryptValue);
                            }
                        }
                        else
                        {
                            this.g_oracleLibrary.BeginTransaction(this.ConnectionStringDecryptValue);
                        }
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        if (this.g_msSqlLibrary == null)
                        {
                            using (this.g_msSqlLibrary = new MSSqlLibrary())
                            {
                                this.g_msSqlLibrary.BeginTransaction(this.ConnectionStringDecryptValue);
                            }
                        }
                        else
                        {
                            this.g_msSqlLibrary.BeginTransaction(this.ConnectionStringDecryptValue);
                        }
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MARIA_DB:
                        if (this.g_mariaDBLibrary == null)
                        {
                            using (this.g_mariaDBLibrary = new MariaDBLibrary())
                            {
                                this.g_mariaDBLibrary.BeginTransaction(this.ConnectionStringDecryptValue);
                            }
                        }
                        else
                        {
                            this.g_mariaDBLibrary.BeginTransaction(this.ConnectionStringDecryptValue);
                        }
                        break;
                }
            }
            catch { throw; }
        }
        #endregion

        #region >> CommitTransaction - 트랜잭션 커밋
        /// <summary>
        /// 트랜잭션 커밋
        /// </summary>
        public void CommitTransaction()
        {
            try
            {
                switch (this.SelectedDatabaseKindEnum)
                {
                    case BaseEnumClass.SelectedDatabaseKind.ORACLE:
                        this.g_oracleLibrary.CommitTransaction();
                        this.g_oracleLibrary.Close();
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        this.g_msSqlLibrary.CommitTransaction();
                        this.g_msSqlLibrary.Close();
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MARIA_DB:
                        this.g_mariaDBLibrary.CommitTransaction();
                        this.g_mariaDBLibrary.Close();
                        break;
                }
            }
            catch { throw; }
        }
        #endregion

        #region >> RollbackTransaction - 트랜잭션 롤백
        public void RollbackTransaction()
        {
            try
            {
                switch (this.SelectedDatabaseKindEnum)
                {
                    case BaseEnumClass.SelectedDatabaseKind.ORACLE:
                        this.g_oracleLibrary.RollBackTransaction();
                        this.g_oracleLibrary.Close();
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        this.g_msSqlLibrary.RollBackTransaction();
                        this.g_msSqlLibrary.Close();
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MARIA_DB:
                        this.g_mariaDBLibrary.RollBackTransaction();
                        this.g_mariaDBLibrary.Close();
                        break;
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region >> GetSpDataSet - 데이터셋 형식으로 반환 (오라클 외의 DB에서 사용 - 필수사용 함수 아님)
        /// <summary>
        /// 데이터셋 형식으로 반환 (오라클 외의 DB에서 사용)
        /// </summary>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <returns></returns>
        public DataSet GetSpDataSet(string _strProcedureName, Dictionary<string, object> _dicInputParam)
        {
            try
            {
                return this.GetSpDataSet(_strProcedureName, _dicInputParam, null);
            }
            catch { throw; }
        }
        #endregion

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

                var strSelectedDatabaseKind     = this.SelectedDatabaseKindEnum.ToString();
                var strCenterCD                 = BaseClass.CenterCD;

                Utility.HelperClass.ProcedureLogInfo(strSelectedDatabaseKind, strCenterCD, _strProcedureName, _dicInputParam, BaseEnumClass.ScreenType.WPF_FORM);

                switch(this.SelectedDatabaseKindEnum)
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

        #region >> GetSpDataSet - 데이터셋 형식으로 반환
        /// <summary>
        /// 데이터셋 형식으로 반환
        /// </summary>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_arrOutputParam">Output 파라메터</param>
        /// <param name="_enumScreenType">화면 타입</param>
        /// <returns></returns>
        public DataSet GetSpDataSet(string _strProcedureName, Dictionary<string, object> _dicInputParam, string[] _arrOutputParam, BaseEnumClass.ScreenType _enumScreenType)
        {
            try
            {
                var dsRtnValue = new DataSet();

                var strSelectedDatabaseKind     = this.SelectedDatabaseKindEnum.ToString();
                var strCenterCD                 = BaseClass.CenterCD;

                Utility.HelperClass.ProcedureLogInfo(strSelectedDatabaseKind, strCenterCD, _strProcedureName, _dicInputParam, _enumScreenType);

                switch(this.SelectedDatabaseKindEnum)
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

        #region >> GetSpDataTable - 데이터 테이블 형식으로 반환 (오라클 이외 DB에 사용)
        /// <summary>
        /// 데이터 테이블 형식으로 반환 (오라클 이외 DB에 사용)
        /// </summary>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <returns></returns>
        public DataTable GetSpDataTable(string _strProcedureName, Dictionary<string, object> _dicInputParam)
        {
            try
            {
                return this.GetSpDataTable(_strProcedureName, _dicInputParam, null);
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
                var dtRtnValue  = new DataTable();

                var strSelectedDatabaseKind     = this.SelectedDatabaseKindEnum.ToString();
                var strCenterCD                 = BaseClass.CenterCD;

                Utility.HelperClass.ProcedureLogInfo(strSelectedDatabaseKind, strCenterCD, _strProcedureName, _dicInputParam, BaseEnumClass.ScreenType.WPF_FORM);

                switch(this.SelectedDatabaseKindEnum)
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

        #region >> GetSpDataTable - 데이터 테이블 형식으로 반환 (키오스크용)
        /// <summary>
        /// 데이터 테이블 형식으로 반환 (키오스크용)
        /// </summary>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_arrOutputParam">Output 파라메터</param>
        /// <param name="_enumScreenType">화면 타입</param>
        /// <returns></returns>
        public DataTable GetSpDataTable(string _strProcedureName, Dictionary<string, object> _dicInputParam, string[] _arrOutputParam, BaseEnumClass.ScreenType _enumScreenType)
        {
            try
            {
                var dtRtnValue  = new DataTable();

                var strSelectedDatabaseKind     = this.SelectedDatabaseKindEnum.ToString();
                var strCenterCD                 = BaseClass.CenterCD;

                Utility.HelperClass.ProcedureLogInfo(strSelectedDatabaseKind, strCenterCD, _strProcedureName, _dicInputParam, _enumScreenType);

                switch(this.SelectedDatabaseKindEnum)
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
