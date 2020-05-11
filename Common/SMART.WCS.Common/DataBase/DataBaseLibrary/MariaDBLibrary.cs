using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common.DataBase
{
    /// <summary>
    /// 데이터베이스 라이브러리 (MariaDB) 클래스
    /// 2019-08-26
    /// 추성호
    /// </summary>
    public class MariaDBLibrary : DisposeClass
    {
        #region ▩ 매개변수
        /// <summary>
        /// MariaDB Command 오브젝트 선언
        /// </summary>
        public MySqlCommand g_mariaDBSqlCommand     = null; 

        /// <summary>
        /// MariaDB Connection 오브젝트 선언
        /// </summary>
        public MySqlConnection g_mariaDBConnection  = null;
        #endregion

        #region ▩ 속성
        #region > ConnectionState - 데이터베이스 연결 상태
        /// <summary>
        /// 데이터베이스 연결 상태
        /// </summary>
        public ConnectionState ConnectionState
        {
            get { return this.g_mariaDBConnection.State; }
        }
        #endregion

        #region > ProcedureName - 프로시저 명
        /// <summary>
        /// 프로시저 명
        /// </summary>
        public string ProcedureName { get; set; }
        #endregion

        #region > InputParams - Input 파라메터
        /// <summary>
        /// Input 파라메터
        /// </summary>
        public Dictionary<string, object> InputParams { get; set; }
        #endregion

        #region > TransactionState - 트랜잭션 상태
        /// <summary>
        /// 트랜잭션 상태
        /// </summary>
        public BaseEnumClass.TransactionState_MariaDB TransactionState { get; set; }
        #endregion
        #endregion

        #region ▩ 함수
        #region > 데이터베이스 연결 / 연결 종료
        #region >> Open - Database 연결 (열기)
        /// <summary>
        /// Database 연결 (열기)
        /// </summary>
        /// <param name="_strConnectionStringDecryptValue">복호화 된 데이터베이스 연결 문자열</param>
        public void Open(string _strConnectionStringDecryptValue)
        {
            try
            {   
                if (this.g_mariaDBConnection == null)
                {
                    this.g_mariaDBConnection = new MySqlConnection(_strConnectionStringDecryptValue);

                    this.g_mariaDBConnection.Open();

                    this.g_mariaDBSqlCommand                = new MySqlCommand();
                    this.g_mariaDBSqlCommand.Connection     = this.g_mariaDBConnection;
                }
                else
                {
                    if (this.g_mariaDBConnection.State == ConnectionState.Closed == true)
                    {
                        this.g_mariaDBConnection.Open();
                        this.g_mariaDBSqlCommand            = new MySqlCommand();
                        this.g_mariaDBSqlCommand.Connection = this.g_mariaDBConnection;
                    }
                }
            }
            catch { throw; }
        }
        #endregion

        #region >> Close - Database 연결 닫기
        /// <summary>
        /// Database 연결 닫기
        /// </summary>
        public void Close()
        {
            try
            {
                if (this.g_mariaDBConnection != null)
                {
                    this.g_mariaDBConnection.Close();
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 데이터 셋 반환 구성
        #region >> GetDataSet - 데이터 셋 반환
        /// <summary>
        /// 데이터 셋 반환
        /// </summary>
        /// <param name="_strConnectionStringDecryptValue">복호화 된 데이터베이스 연결문자열</param>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_arrOutputParam">Output 파라메터</param>
        /// <returns></returns>
        public DataSet GetDataSet(string _strConnectionStringDecryptValue, string _strProcedureName, Dictionary<string, object> _dicInputParam)
        {
            try
            {
                // Database 연결을 오픈한다.
                this.Open(_strConnectionStringDecryptValue);

                this.ProcedureName      = _strProcedureName;            // 프로시저명
                this.InputParams        = _dicInputParam;               // Input 파라메터
                return this.ExecuteDataSet();
            }
            catch { throw; }
            finally { this.Close(); }
        }
        #endregion

        #region >> ExecuteDataSet - 데이터 셋 반환 실행
        /// <summary>
        /// 데이터 셋 반환 실행
        /// </summary>
        /// <returns></returns>
        private DataSet ExecuteDataSet()
        {
            try
            {
                // 파라메터를 설정한다.
                this.SetParameters();

                var dsRtnValue = new System.Data.DataSet();
                var sqlAdapter = new MySqlDataAdapter(this.g_mariaDBSqlCommand);

                sqlAdapter.Fill(dsRtnValue);

                return dsRtnValue;
            }
            catch (MySqlException oe)
            {
                oe.HelpLink = this.g_mariaDBSqlCommand.CommandText;
                throw;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 데이터 테이블 반환 구성
        #region >> GetDataTable - 데이터테이블 반환
        /// <summary>
        /// 데이터테이블 반환
        /// </summary>
        /// <param name="_strConnectionStringDecryptValue">복호화 된 데이터베이스 연결문자열</param>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_arrOutputParam">Output 파라메터</param>
        /// <returns></returns>
        public DataTable GetDataTable(string _strConnectionStringDecryptValue, string _strProcedureName, Dictionary<string, object> _dicInputParam)
        {
            try
            {
                // 데이터베이스 연결을 오픈한다.
                this.Open(_strConnectionStringDecryptValue);

                this.ProcedureName  = _strProcedureName;
                this.InputParams    = _dicInputParam;

                return this.ExecuteDataTable();
            }
            catch { throw; }
            finally { this.Close(); }
        }
        #endregion

        #region >> ExecuteDataTable - 데이터 테이블 반환 실행
        /// <summary>
        /// 데이터 테이블 반환 실행
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable ExecuteDataTable()
        {
            try
            {
                // 파라메터를 설정한다.
                this.SetParameters();

                var dtRtnValue              = new System.Data.DataTable();
                MySqlDataAdapter sqlAdapter = new MySqlDataAdapter(this.g_mariaDBSqlCommand);

                sqlAdapter.Fill(dtRtnValue);
                return dtRtnValue;
            }
            catch (MySqlException oe)
            {
                oe.HelpLink = this.g_mariaDBSqlCommand.CommandText;
                throw;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > SetParameters - 파라메터 값을 정의한다.
        /// <summary>
        /// 파라메터 값을 정의한다.
        /// </summary>
        private void SetParameters()
        {
            try
            {
                this.g_mariaDBSqlCommand.CommandText    = this.ProcedureName;
                this.g_mariaDBSqlCommand.CommandType    = System.Data.CommandType.StoredProcedure;

                while (this.g_mariaDBSqlCommand.Parameters.Count > 0)
                {
                    this.g_mariaDBSqlCommand.Parameters.RemoveAt(0);
                }

                foreach (System.Collections.Generic.KeyValuePair<string, object> dicEntity in this.InputParams)
                {
                    switch (dicEntity.Value.GetType().Name.ToUpper())
                    {
                        case "INT16":
                            this.g_mariaDBSqlCommand.Parameters.Add(dicEntity.Key.ToString(), MySqlDbType.Int16).Value = (System.Int16)(dicEntity.Value);
                            break;
                        case "INT32":
                            this.g_mariaDBSqlCommand.Parameters.Add(dicEntity.Key.ToString(), MySqlDbType.Int32).Value = (System.Int32)(dicEntity.Value);
                            break;
                        case "INT64":
                            this.g_mariaDBSqlCommand.Parameters.Add(dicEntity.Key.ToString(), MySqlDbType.Int64).Value = (System.Int64)(dicEntity.Value);
                            break;
                        case "DECIMAL":
                            this.g_mariaDBSqlCommand.Parameters.Add(dicEntity.Key.ToString(), MySqlDbType.Decimal).Value = (System.Decimal)(dicEntity.Value);
                            break;
                        case "NUMERIC":
                            this.g_mariaDBSqlCommand.Parameters.Add(dicEntity.Key.ToString(), MySqlDbType.Decimal).Value = (int)(dicEntity.Value);
                            break;
                        default:
                            this.g_mariaDBSqlCommand.Parameters.Add(dicEntity.Key.ToString(), MySqlDbType.String).Value = dicEntity.Value.ToString();
                            break;
                    }
                }
            }
            catch { throw; }
        }
        #endregion

        #region > Transaction 정의
        #region >> BeginTransction - 트랜잭션 시작
        /// <summary>
        /// 트랜잭션 시작
        /// </summary>
        /// <param name="_strConnectionStringDecryptValue">복호화 된 데이터베이스 연결 문자열</param>
        public void BeginTransaction(string _strConnectionStringDecryptValue)
        {
            try
            {
                this.Open(_strConnectionStringDecryptValue);

                if (this.g_mariaDBSqlCommand.Transaction == null)
                {
                    this.g_mariaDBSqlCommand.Transaction    = this.g_mariaDBConnection.BeginTransaction();
                    this.TransactionState                   = BaseEnumClass.TransactionState_MariaDB.TransactionStarted;
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
                this.g_mariaDBSqlCommand.Transaction.Commit();
            }
            finally
            {
                this.TransactionState = BaseEnumClass.TransactionState_MariaDB.None;
            }
        }
        #endregion

        #region >> RollbackTransaction - 트랜잭션 롤백
        /// <summary>
        /// 트랜잭션 롤백
        /// </summary>
        public void RollBackTransaction()
        {
            try
            {
                if (this.g_mariaDBSqlCommand.Transaction != null)
                {
                    this.g_mariaDBSqlCommand.Transaction.Rollback();
                    this.TransactionState = BaseEnumClass.TransactionState_MariaDB.None;
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion
    }
}
