using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common_Etc.DataBase
{
    public class OracleLibrary : DisposeClass
    {
        #region ▩ 전역변수
        /// <summary>
        /// Oracle Command 오브젝트 선언
        /// </summary>
        public OracleCommand g_oracleCommand = null;

        /// <summary>
        /// Oracle Connection 오브젝트 선언
        /// </summary>
        public OracleConnection g_oracleConnection = null;
        #endregion

        #region ▩ 속성
        #region > ConnectionState - 데이터베이스 연결 상태
        /// <summary>
        /// 데이터베이스 연결 상태
        /// </summary>
        public ConnectionState ConnectionState
        {
            get { return this.g_oracleConnection.State; }
        }
        #endregion

        #region > ProcedureName - 프로시저명
        /// <summary>
        /// 프로시저명
        /// </summary>
        public string ProcedureName { get; set; }
        #endregion

        #region > Input 파라메터
        /// <summary>
        /// Input 파라메터
        /// </summary>
        public Dictionary<string, object> InputParams { get; set; }
        #endregion

        #region > Input CLOB 파라메터
        /// <summary>
        /// Input CLOB 파라메터
        /// </summary>
        public StringWriter CLOBStringWriterData { get; set; }
        #endregion

        #region > Output 파라메터
        /// <summary>
        /// Output 파라메터
        /// </summary>
        public string[] OutputParams { get; set; }
        #endregion

        #region > TransactionState - 트랜잭션 상태
        /// <summary>
        /// 트랜잭션 상태
        /// </summary>
        public BaseEnumClass.TransactionState_ORACLE TransactionState { get; set; }
        #endregion
        #endregion

        #region ▩ 함수
        #region > Open - 데이터베이스 연결 (열기)
        /// <summary>
        /// 데이터베이스 연결 (열기)
        /// </summary>
        /// <param name="_strConnectionStringDecryptValue">복호화 된 데이터베이스 연결 문자열</param>
        public void Open(string _strConnectionStringDecryptValue)
        {
            try
            {
                if (this.g_oracleConnection == null)
                {
                    this.g_oracleConnection = new OracleConnection(_strConnectionStringDecryptValue);
                    this.g_oracleConnection.Open();

                    this.g_oracleCommand                = new OracleCommand();
                    this.g_oracleCommand.Connection     = this.g_oracleConnection;
                }
                else
                {
                    if (this.g_oracleConnection.State == ConnectionState.Closed == true)
                    {
                        this.g_oracleConnection.Open();
                        this.g_oracleCommand                = new OracleCommand();
                        this.g_oracleCommand.Connection     = this.g_oracleConnection;
                    }
                }
            }
            catch { throw; }
        }
        #endregion

        #region > Close - 데이터베이스 연결 닫기
        /// <summary>
        /// 데이터베이스 연결 닫기
        /// </summary>
        public void Close()
        {
            try
            {
                if ((this.TransactionState == BaseEnumClass.TransactionState_ORACLE.None) == true)
                {
                    if ((this.g_oracleConnection.State == ConnectionState.Open) == true)
                    {
                        this.g_oracleConnection.Close();
                    }
                }
            }
            catch { throw; }
        }
        #endregion

        #region > 데이터 셋 반환 구성
        #region >> GetDataSet - 데이터 셋 반환
        /// <summary>
        /// 데이터 세세 반환
        /// </summary>
        /// <param name="_strConnectionStringDecryptValue">복호화 된 데이터베이스 연결 문자열</param>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_arrOutputParam">Output 파라메터</param>
        /// <returns></returns>
        public DataSet GetDataSet(string _strConnectionStringDecryptValue, string _strProcedureName, Dictionary<string, object> _dicInputParam, string[] _arrOutputParam)
        {
            try
            {
                // 데이터베이스 연결을 오픈한다.
                this.Open(_strConnectionStringDecryptValue);
                this.ProcedureName      = _strProcedureName;
                this.InputParams        = _dicInputParam;
                this.OutputParams       = _arrOutputParam;

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

                // Output 변수의 형식을 정의한다. - 커서 반환
                this.DefineOutputParamCursor();

                this.g_oracleCommand.BindByName     = true;
                var dsRtnValue                      = new DataSet();
                var sqlAdapter                      = new OracleDataAdapter(this.g_oracleCommand);

                sqlAdapter.Fill(dsRtnValue);

                var liOutParams = this.g_oracleCommand.Parameters.Cast<OracleParameter>().Where(p => p.Direction == ParameterDirection.Output).ToList();

                if (liOutParams.Count() == dsRtnValue.Tables.Count)
                {
                    for (int i = 0; i < liOutParams.Count(); i++)
                    {
                        dsRtnValue.Tables[i].TableName = liOutParams[i].ParameterName;
                    }

                    dsRtnValue.AcceptChanges();
                }

                return dsRtnValue;
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
        /// <param name="_strConnectionStringDecryptValue">복호화 된 데이터베이스 연결 문자열</param>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_arrOutputParam">Output 파라메터</param>
        /// <returns></returns>
        public DataTable GetDataTable(string _strConnectionStringDecryptValue, string _strProcedureName, Dictionary<string, object> _dicInputParam, string[] _arrOutputParam)
        {
            try
            {
                // 데이터베이스 연결을 오픈한다.
                this.Open(_strConnectionStringDecryptValue);

                this.ProcedureName      = _strProcedureName;
                this.InputParams        = _dicInputParam;
                this.OutputParams       = _arrOutputParam;

                return this.ExecuteDataTable();
            }
            catch { throw; }
            finally { this.Close(); }
        }
        #endregion

        #region >> CLOB용 GetDataTable  - 데이터테이블 반환
        /// <summary>
        /// 데이터테이블 반환
        /// </summary>
        /// <param name="_strConnectionStringDecryptValue">복호화 된 데이터베이스 연결 문자열</param>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_arrOutputParam">Output 파라메터</param>
        /// <returns></returns>
        public DataTable GetDataTableCLOB(string _strConnectionStringDecryptValue, string _strProcedureName, Dictionary<string, object> _dicInputParam, StringWriter _swCLOBData, string[] _arrOutputParam)
        {
            try
            {
                // 데이터베이스 연결을 오픈한다.
                this.Open(_strConnectionStringDecryptValue);

                this.ProcedureName              = _strProcedureName;
                this.InputParams                = _dicInputParam;
                this.CLOBStringWriterData       = _swCLOBData;
                this.OutputParams               = _arrOutputParam;

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
        public DataTable ExecuteDataTable()
        {
            try
            {
                // 파라메터를 설정한다.
                this.SetParameters();

                // Output 변수의 형식을 정의한다. - 커서 반환
                this.DefineOutputParamCursor();

                var dtRtnValue      = new DataTable();
                var sqlAdapter      = new OracleDataAdapter(this.g_oracleCommand);
                sqlAdapter.Fill(dtRtnValue);

                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 파라메터 설정 (Input, Output)
        #region >> SetParameters - 파라메터 값을 정의한다.
        /// <summary>
        /// 파라메터 값을 정의한다.
        /// </summary>
        private void SetParameters() 
        {
            BaseClass baseClass = new BaseClass();

            try
            {
                // Parameter에 회사코드를 추가하기 위해 수행 카운트 체크용 매개변수
                int iProcessCount                   = 0;
                this.g_oracleCommand.CommandText    = this.ProcedureName;
                this.g_oracleCommand.CommandType    = System.Data.CommandType.StoredProcedure;

                while (this.g_oracleCommand.Parameters.Count > 0)
                {
                    this.g_oracleCommand.Parameters.RemoveAt(0);
                }

                if (this.CLOBStringWriterData != null && this.CLOBStringWriterData.ToString().Length > 0)
                {
                    this.g_oracleCommand.Parameters.Add("P_XML_DATA", OracleDbType.Clob).Value = this.CLOBStringWriterData.ToString();
                }

                // Input 파라메터가 없는 경우 회사코드를 이 구문에서 추가한다.
                // Input 파라메터가 있는 경우 회사코드는 아래 foreach 구문에서 추가한다.
                if (this.InputParams.Count == 0)
                {
                    this.g_oracleCommand.Parameters.Add("P_CO_CD", baseClass.CompanyCode); // 회사코드
                }

                foreach (System.Collections.Generic.KeyValuePair<string, object> dicEntity in this.InputParams)
                {
                    //if (dicEntity.Value == null) { dicEntity.Value = string.Empty; }

                    if (iProcessCount == 0)
                    {
                        // 파라메터 Key중 회사코드가 없는 경우 프로시저 파라메터에 회사코드키와 값을 추가한다.
                        if (this.InputParams.ContainsKey("P_CO_CD") == false)
                        {
                            this.g_oracleCommand.Parameters.Add("P_CO_CD", baseClass.CompanyCode); // 회사코드
                        }

                        iProcessCount++;
                    }

                    if (dicEntity.Value == null)
                    {
                        this.g_oracleCommand.Parameters.Add(dicEntity.Key.ToString(), OracleDbType.Varchar2).Value = string.Empty;
                    }
                    else
                    {
                        switch (dicEntity.Value.GetType().Name.ToUpper())
                        {
                            case "INT16":
                                this.g_oracleCommand.Parameters.Add(dicEntity.Key.ToString(), OracleDbType.Int16).Value = (System.Int16)(dicEntity.Value);
                                break;
                            case "INT32":
                                this.g_oracleCommand.Parameters.Add(dicEntity.Key.ToString(), OracleDbType.Int32).Value = (System.Int32)(dicEntity.Value);
                                break;
                            case "INT64":
                                this.g_oracleCommand.Parameters.Add(dicEntity.Key.ToString(), OracleDbType.Int64).Value = (System.Int64)(dicEntity.Value);
                                break;
                            case "DECIMAL":
                                this.g_oracleCommand.Parameters.Add(dicEntity.Key.ToString(), OracleDbType.Double).Value = (System.Decimal)(dicEntity.Value);
                                break;
                            case "NUMERIC":
                                this.g_oracleCommand.Parameters.Add(dicEntity.Key.ToString(), OracleDbType.Long).Value = (int)(dicEntity.Value);
                                break;
                            case "DOUBLE":
                                this.g_oracleCommand.Parameters.Add(dicEntity.Key.ToString(), OracleDbType.Double).Value = (System.Double)(dicEntity.Value);
                                break;
                            case "DATETIME":
                                this.g_oracleCommand.Parameters.Add(dicEntity.Key.ToString(), OracleDbType.Date).Value = (DateTime)(dicEntity.Value);
                                break;
                            case "BYTE[]":
                                this.g_oracleCommand.Parameters.Add(dicEntity.Key.ToString(), OracleDbType.Blob, ((byte[])(dicEntity.Value)).Length).Value = (byte[])(dicEntity.Value);
                                break;
                            case "STRINGWRITER":
                                //this.g_oracleCommand.Parameters.Add(dicEntity.Key.ToString(), OracleDbType.Clob).Value = dicEntity.Value.ToString();
                                this.g_oracleCommand.Parameters.Add("P_XML_DATA", OracleDbType.Clob).Value = this.CLOBStringWriterData.ToString();
                                break;
                            default:
                                this.g_oracleCommand.Parameters.Add(dicEntity.Key.ToString(), OracleDbType.Varchar2).Value = dicEntity.Value.ToString();
                                break;
                        }
                    }
                }
            }
            catch { throw; }
            finally
            {
                baseClass.Dispose();
                baseClass = null;
            }
        }
        #endregion

        #region >> DefineOutputParamCursor - 커서 형식으로 리턴하는 Output 변수 형식을 정의한다.
        /// <summary>
        /// 커서 형식으로 리턴하는 Output 변수 형식을 정의한다.
        /// </summary>
        private void DefineOutputParamCursor()
        {
            if (this.OutputParams != null)
            {
                for (int i = 0; i < this.OutputParams.Length; i++)
                {
                    this.g_oracleCommand.Parameters.Add(this.OutputParams[i].ToString(), OracleDbType.RefCursor).Direction
                        = System.Data.ParameterDirection.Output;
                }
            }
        }
        #endregion

        #region >> DefineOutputParamValiable - 문자열 형식으로 리턴하는 Output 변수 형식을 정의한다.
        /// <summary>
        /// 문자열 형식으로 리턴하는 Output 변수 형식을 정의한다.
        /// </summary>
        private void DefineOutputParamValiable()
        {

            for (int i = 0; i < this.OutputParams.Length; i++)
            {
                this.g_oracleCommand.Parameters.Add(this.OutputParams[i].ToString(), OracleDbType.Varchar2, 500).Direction
                    = System.Data.ParameterDirection.Output;
            }
        }
        #endregion
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

                if (this.g_oracleCommand.Transaction == null)
                {
                    this.g_oracleCommand.Transaction        = this.g_oracleConnection.BeginTransaction();
                    this.TransactionState                   = BaseEnumClass.TransactionState_ORACLE.TransactionStarted;
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
                this.g_oracleCommand.Transaction.Commit();
            }
            finally
            {
                this.TransactionState = BaseEnumClass.TransactionState_ORACLE.None;
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
                if (this.g_oracleCommand.Transaction != null)
                {
                    this.g_oracleCommand.Transaction.Rollback();
                    this.TransactionState = BaseEnumClass.TransactionState_ORACLE.None;
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion
    }
}
