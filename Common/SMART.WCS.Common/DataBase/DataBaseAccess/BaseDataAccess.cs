using SMART.WCS.Common.Data;
using SMART.WCS.Common.Utility;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SMART.WCS.Common.DataBase
{
    /// <summary>
    /// DataAccess 클래스
    /// 2019-08-28
    /// 추성호
    /// </summary>
    public class BaseDataAccess : DisposeClass
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
        #region 생성자 - 데이터베이스가 지정되지 않은 경우 - App.config 파일내 MainDatabase값으로 데이터베이스를 연결한다.
        /// <summary>
        /// 데이터베이스가 지정되지 않은 경우 - App.config 파일내 MainDatabase값으로 데이터베이스를 연결한다.
        /// </summary>
        public BaseDataAccess()
        {
            try
            {
                // App.config에 저장된 메인(기준) 데이터베이스 정보를 가져온다.
                var strMainDatabaseValue            = this.BaseClass.MainDatabase;

                // 현재 접속한 데이터베이스 종류를 Attribute에 저장한다.
                // 동일 세션내에서 계속 사용하기 위해 Attribute에 저장
                this.SelectedDatabaseKindEnum       = HelperClass.GetDatabaseKindValueByEnumType(strMainDatabaseValue);

                // 로그인 화면 오픈할 때 조회한 각 데이터베이스 연결 문자열 (암호화 데이터)을 복호화하여 Attribute에 저장한다.
                // 데이터베이스 연결 및 트랜잭션에 복호화 한 문자열을 사용한다.
                switch (this.SelectedDatabaseKindEnum)
                {
                    case BaseEnumClass.SelectedDatabaseKind.ORACLE:
                        this.ConnectionStringDecryptValue = Cryptography.AES.DecryptAES256(this.DatabaseConnectionStringEncryptValue_ORACLE);
                        this.g_oracleLibrary = new OracleLibrary();
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        this.ConnectionStringDecryptValue = Cryptography.AES.DecryptAES256(this.DatabaseConnectionStringEncryptValue_MSSQL);
                        this.g_msSqlLibrary = new MSSqlLibrary();
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MARIA_DB:
                        this.ConnectionStringDecryptValue = Cryptography.AES.DecryptAES256(this.DatabaseConnectionStringEncryptValue_MariaDB);
                        this.g_mariaDBLibrary = new MariaDBLibrary();
                        break;
                }
            }
            catch { throw; }
        }
        #endregion

        #region 생성자 - 데이터베이스가 지정되는 경우 - Parameter로 넘어온 값으로 데이터베이스를 연결한다.
        /// <summary>
        /// 데이터베이스가 지정되는 경우 - Parameter로 넘어온 값으로 데이터베이스를 연결한다.
        /// </summary>
        /// <param name="_enumSelectedDatabaseKind">연결 데이터베이스 종류</param>
        public BaseDataAccess(BaseEnumClass.SelectedDatabaseKind _enumSelectedDatabaseKind)
        {
            try
            {
                // 메인(기준) 데이터베이스가 아닌 업무별로 접속 데이터베이스 종류가 다른 경우 Parameter로 받은 데이터베이스 종류 정보를
                // Attribute에 저장한다.
                this.SelectedDatabaseKindEnum = _enumSelectedDatabaseKind;

                switch (SelectedDatabaseKindEnum)
                {
                    case BaseEnumClass.SelectedDatabaseKind.ORACLE:
                        this.ConnectionStringDecryptValue = Cryptography.AES.DecryptAES256(this.DatabaseConnectionStringEncryptValue_ORACLE);
                        this.g_oracleLibrary = new OracleLibrary();
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        this.ConnectionStringDecryptValue = Cryptography.AES.DecryptAES256(this.DatabaseConnectionStringEncryptValue_MSSQL);
                        this.g_msSqlLibrary = new MSSqlLibrary();
                        break;
                    case BaseEnumClass.SelectedDatabaseKind.MARIA_DB:
                        this.ConnectionStringDecryptValue = Cryptography.AES.DecryptAES256(this.DatabaseConnectionStringEncryptValue_MariaDB);
                        this.g_mariaDBLibrary = new MariaDBLibrary();
                        break;
                }
            }
            catch { throw; }
        }
        #endregion
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

        #region ▩ 함수
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
                switch(this.SelectedDatabaseKindEnum)
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

        #region > 공통 콤보박스 핸들링
        //public List<ComboBoxInfo> GetComboBoxListItem(
        //    )
        #endregion

        #region > 데이터 핸들링
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

        #region >> GetSpDataSet - 데이터셋으로 반환 - Output 리턴값이 있는 경우 - Dictionary로 리턴
        /// <summary>
        /// Output 리턴값이 있는 경우
        /// <br />Dictionary로 리턴
        /// </summary>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_dicOutputParam">Output 파라메터</param>
        /// <param name="_dicRtnValue">Output 데이터 저장 변수</param>
        /// <returns></returns>
        public DataSet GetSpDataSet(string _strProcedureName
                ,   Dictionary<string, object> _dicInputParam
                ,   Dictionary<object, BaseEnumClass.MSSqlOutputDataType> _dicOutputParam
                ,   ref Dictionary<object, object> _dicRtnValue
            )
        {
            try
            {
                var dsRtnValue          = new DataSet();

                var strSelectedDatabaseKind = this.SelectedDatabaseKindEnum.ToString();
                var strCenterCD = BaseClass.CenterCD;

                Utility.HelperClass.ProcedureLogInfo(strSelectedDatabaseKind, strCenterCD, _strProcedureName, _dicInputParam, BaseEnumClass.ScreenType.WPF_FORM);

                switch (SelectedDatabaseKindEnum)
                {
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        dsRtnValue = this.g_msSqlLibrary.GetDataSet(this.ConnectionStringDecryptValue, _strProcedureName, _dicInputParam, _dicOutputParam, ref _dicRtnValue);
                        break;
                }

                return dsRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> GetSpDataSet - 데이터셋 형식으로 반환 (키오스크용)
        /// <summary>
        /// 데이터셋 형식으로 반환 (키오스크용)
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
                var dtRtnValue                  = new DataTable();
                var strSelectedDatabaseKind     = this.SelectedDatabaseKindEnum.ToString();
                var strCenterCD                 = BaseClass.CenterCD;

                Utility.HelperClass.ProcedureLogInfo(strSelectedDatabaseKind, strCenterCD, _strProcedureName, _dicInputParam, BaseEnumClass.ScreenType.WPF_FORM);

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

        #region >> GetSpDataSet - 데이터테이블로 반환 - Output 리턴값이 있는 경우 - Dictionary로 리턴
        /// <summary>
        /// 데이터테이블로 반환
        /// <br />Output 리턴값이 있는 경우 - Dictionary로 리턴
        /// </summary>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_dicOutputParam">Output 파라메터</param>
        /// <param name="_dicRtnValue">Output 데이터 저장 변수</param>
        /// <returns></returns>
        public DataTable GetSpDataTable(string _strProcedureName
                ,   Dictionary<string, object> _dicInputParam
                ,   Dictionary<object, BaseEnumClass.MSSqlOutputDataType> _dicOutputParam
                ,   ref Dictionary<object, object> _dicRtnValue
            )
        {
            try
            {
                 var dtRtnValue          = new DataTable();

                var strSelectedDatabaseKind = this.SelectedDatabaseKindEnum.ToString();
                var strCenterCD = BaseClass.CenterCD;

                Utility.HelperClass.ProcedureLogInfo(strSelectedDatabaseKind, strCenterCD, _strProcedureName, _dicInputParam, BaseEnumClass.ScreenType.WPF_FORM);

                switch (SelectedDatabaseKindEnum)
                {
                    case BaseEnumClass.SelectedDatabaseKind.MS_SQL:
                        dtRtnValue = this.g_msSqlLibrary.GetDataTable(this.ConnectionStringDecryptValue, _strProcedureName, _dicInputParam, _dicOutputParam, ref _dicRtnValue);
                        break;
                }

                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > GetSpDataTableCLOB - CLOB 데이터를 파라메터로 받은 후 데이터 테이블 형식으로 반환
        /// <summary>
        /// CLOB 데이터를 파라메터로 받은 후 데이터 테이블 형식으로 반환
        /// </summary>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_swCLOBData">CLOB 데이터</param>
        /// <param name="_arrOutputParam">Output 파라메터</param>
        /// <returns></returns>
        public DataTable GetSpDataTableCLOB(string _strProcedureName, Dictionary<string, object> _dicInputParam, StringWriter _swCLOBData, string[] _arrOutputParam)
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

                        dtRtnValue = this.g_oracleLibrary.GetDataTableCLOB(this.ConnectionStringDecryptValue, _strProcedureName, _dicInputParam, _swCLOBData, _arrOutputParam);
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

        #region >> GetSpDataTable - 데이터 테이블 형식으로 반환 (키오스크)
        /// <summary>
        /// 데이터 테이블 형식으로 반환 (키오스크)
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
                var dtRtnValue                  = new DataTable();
                var strSelectedDatabaseKind     = this.SelectedDatabaseKindEnum.ToString();
                var strCenterCD                 = BaseClass.CenterCD;

                Utility.HelperClass.ProcedureLogInfo(strSelectedDatabaseKind, strCenterCD, _strProcedureName, _dicInputParam, _enumScreenType);

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
        #endregion
    }
}
