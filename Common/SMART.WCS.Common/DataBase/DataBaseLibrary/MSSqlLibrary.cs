using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SMART.WCS.Common.DataBase
{
    /// <summary>
    /// 데이터베이스 라이브러리 (MS-SQL) 클래스
    /// 2019-08-26
    /// 추성호
    /// </summary>
    public class MSSqlLibrary : DisposeClass
    {
        #region ▩ 매개변수
        /// <summary>
        /// MSSQL Command 오브젝트 선언
        /// </summary>
        public SqlCommand g_sqlCommand = null;

        /// <summary>
        /// MSSQL Connection 오브젝트 선언
        /// </summary>
        public SqlConnection g_sqlConnection = null;
        #endregion
        
        #region ▩ 속성
        #region > ConnectionState - Database 연결 상태
        /// <summary>
        /// Database 연결 상태
        /// </summary>
        public System.Data.ConnectionState ConnectionState
        {
            get { return this.g_sqlConnection.State; }
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

        #region > MSSqlOutputParams - MS-SQL Output 파라메터 (프로시저에서 Output 변수를 받기위한 변수)
        /// <summary>
        /// MS-SQL Output 파라메터 (프로시저에서 Output 변수를 받기위한 변수)
        /// </summary>
        public Dictionary<object, BaseEnumClass.MSSqlOutputDataType> MSSqlOutputParams { get; set; }
        #endregion

        #region > TransactionState - 트랜잭션 상태
        /// <summary>
        /// 트랜잭션 상태
        /// </summary>
        public BaseEnumClass.TransactionState_MSSQL TransactionState { get; set; }
        #endregion

        #region > ConnectionTimeOut - 접속 타임아웃 시간
        /// <summary>
        /// 접속 타임아웃 시간
        /// </summary>
        public int ConnectionTimeOut { get; set; }
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
                if (this.g_sqlConnection == null)
                {
                    this.g_sqlConnection = new SqlConnection(_strConnectionStringDecryptValue);

                    this.g_sqlConnection.Open();

                    this.g_sqlCommand               = new SqlCommand();
                    this.g_sqlCommand.Connection    = this.g_sqlConnection;
                }
                else
                {
                    if (this.g_sqlConnection.State == ConnectionState.Closed == true)
                    {
                        this.g_sqlConnection.Open();
                        this.g_sqlCommand               = new SqlCommand();
                        this.g_sqlCommand.Connection    = this.g_sqlConnection;
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
                if ((this.TransactionState == BaseEnumClass.TransactionState_MSSQL.None) == true)
                {
                    if ((this.g_sqlConnection.State == ConnectionState.Open) == true)
                    {
                        this.g_sqlConnection.Close();
                    }
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
        /// <returns></returns>
        public DataSet GetDataSet(string _strConnectionStringDecryptValue, string _strProcedureName, Dictionary<string, object> _dicInputParam)
        {
            try
            {
                // Database 연결을 오픈한다.
                this.Open(_strConnectionStringDecryptValue);

                var dsRtnValue          = new System.Data.DataSet();
                this.ProcedureName      = _strProcedureName;            // 프로시저명
                this.InputParams        = _dicInputParam;               // Input 파라메터
                
                dsRtnValue              = this.ExecuteDataSet();

                return dsRtnValue;
            }
            catch { throw; }
            finally { this.Close(); }
        }
        #endregion

        #region >> GetDataSet - 데이터테이블 반환 - Output 리턴값이 있는 경우 - Dictionary로 리턴
        /// <summary>
        /// Output 리턴값이 있는 경우 - Dictionary로 리턴
        /// </summary>
        /// <param name="_strConnectionStringDecryptValue">데이터베이스 연결 문자열</param>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_dicOutputParam">Output 파라메터</param>
        /// <param name="_dicRtnValue">Output 리턴값 저장 변수</param>
        /// <returns></returns>
        public DataSet GetDataSet(string _strConnectionStringDecryptValue
                ,   string _strProcedureName
                ,   Dictionary<string, object> _dicInputParam
                ,   Dictionary<object, BaseEnumClass.MSSqlOutputDataType> _dicOutputParam
                ,   ref Dictionary<object, object> _dicRtnValue
            )
        {
            try
            {
                // 데이터베이스 연결을 오픈한다.
                this.Open(_strConnectionStringDecryptValue);

                this.ProcedureName          = _strProcedureName;        // 프로시저명
                this.InputParams            = _dicInputParam;           // Input 파라메터
                this.MSSqlOutputParams      = _dicOutputParam;          // Output 파라메터

                return this.ExecuteDataSet(ref _dicRtnValue);
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

                var dsRtnValue                  = new System.Data.DataSet();
                 var sqlAdapter                 = new SqlDataAdapter(this.g_sqlCommand);

                sqlAdapter.Fill(dsRtnValue);

                return dsRtnValue;
            }
            catch (SqlException oe)
            {
                oe.HelpLink = this.g_sqlCommand.CommandText;
                throw;
            }
            catch { throw; }
        }
        #endregion

        #region > ExecuteDataSet - 데이터셋 반환 실행 - Output 변수가 있는 경우
        /// <summary>
        /// 데이터셋 반환 실행 - Output 변수가 있는 경우
        /// </summary>
        /// <param name="_dicRtnValue"></param>
        /// <returns></returns>
        private DataSet ExecuteDataSet(ref Dictionary<object, object> _dicRtnValue)
        {
            try
            {
                // 파라메터를 설정한다.
                this.SetParameters();

                if (this.MSSqlOutputParams != null)
                {
                    this.SetOutputParameters();
                }

                var dsRtnValue = new DataSet();

                using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(this.g_sqlCommand))
                {
                    sqlAdapter.Fill(dsRtnValue);
                }

                if (this.MSSqlOutputParams != null)
                {
                    for (int i = 0; i < this.g_sqlCommand.Parameters.Count; i++)
                    {
                        if (this.g_sqlCommand.Parameters[i].Direction == ParameterDirection.Output)
                        {
                            _dicRtnValue.Add(this.g_sqlCommand.Parameters[i].ParameterName, this.g_sqlCommand.Parameters[i].Value);
                        }
                    }
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
        /// <param name="_strConnectionStringDecryptValue">복호화 된 데이터베이스 연결문자열</param>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_arrOutputParam">Output 파라메터</param>
        /// <returns></returns>
        public DataTable GetDataTable(string _strConnectionStringDecryptValue, string _strProcedureName, Dictionary<string, object> _dicInputParam)
        {
            try
            {
                //_strConnectionStringDecryptValue = "Data Source=mssql.ctfe9ihnllwc.ap-northeast-2.rds.amazonaws.com;Initial Catalog=smartwcs;Persist Security Info=True;User ID=smartwcs;Pwd=smartwcs1234";

                // 데이터베이스 연결을 오픈한다.
                this.Open(_strConnectionStringDecryptValue);

                this.ProcedureName      = _strProcedureName;
                this.InputParams        = _dicInputParam;

                return this.ExecuteDataTable();
            }
            catch { throw; }
            finally { this.Close(); }
        }
        #endregion

        #region >> GetDataTable - 데이터테이블 반환 (Output 리턴이 있는 경우 - Dictionary로 리턴)
        /// <summary>
        /// 데이터테이블 반환
        /// <br />(Output 리턴이 있는 경우 - Dictionary로 리턴)
        /// </summary>
        /// <param name="_strConnectionStringDecryptValue">데이터베이스 연결 문자열</param>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <param name="_dicOutputParam">Output 파라메터</param>
        /// <param name="_dicRtnValue">Output파라메터 데이터 저장변수<br />(Reference Valiable)</param>
        /// <returns></returns>
        public DataTable GetDataTable(string _strConnectionStringDecryptValue
                ,   string _strProcedureName
                ,   Dictionary<string, object> _dicInputParam
                ,   Dictionary<object, BaseEnumClass.MSSqlOutputDataType> _dicOutputParam
                ,   ref Dictionary<object, object> _dicRtnValue
            )
        {
            try
            {
                // 데이터베이스 연결을 오픈한다.
                this.Open(_strConnectionStringDecryptValue);

                this.ProcedureName              = _strProcedureName;        // 프로시저명
                this.InputParams                = _dicInputParam;           // Input 파라메터
                this.MSSqlOutputParams          = _dicOutputParam;          // Output 파라메터

                return this.ExecuteDataTable(ref _dicRtnValue);
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
                SqlDataAdapter sqlAdapter   = new SqlDataAdapter(this.g_sqlCommand);

                sqlAdapter.Fill(dtRtnValue);
                return dtRtnValue;
            }
            catch (SqlException oe)
            {
                oe.HelpLink = this.g_sqlCommand.CommandText;
                throw;
            }
            catch { throw; }
        }
        #endregion

        #region + ExecuteDataTable - 데이터테이블 반환 - Output 리턴이 있는 경우 - Dictionary로 리턴
        /// <summary>
        /// Output 리턴이 있는 경우<br />Dictionary로 리턴
        /// </summary>
        /// <param name="_dicRtnValue">Output 데이터 저장 변수</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(ref Dictionary<object, object> _dicRtnValue)
        {
            try
            {
                // 파라메터를 설정한다.
                this.SetParameters();

                if (this.MSSqlOutputParams != null)
                {
                    this.SetOutputParameters();
                }

                var dtRtnValue      = new DataTable();

                using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(this.g_sqlCommand))
                {
                    sqlAdapter.Fill(dtRtnValue);
                }

                if (this.MSSqlOutputParams != null)
                {
                    for(int i = 0; i < this.g_sqlCommand.Parameters.Count; i++)
                    {
                        if (this.g_sqlCommand.Parameters[i].Direction == ParameterDirection.Output)
                        {
                            _dicRtnValue.Add(this.g_sqlCommand.Parameters[i].ParameterName, this.g_sqlCommand.Parameters[i].Value);
                        }
                    }
                }

                return dtRtnValue;
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
            BaseClass baseClass = new BaseClass();

            try
            {
                // Parameter에 회사코드를 추가하기 위해 수행 카운트 체크용 매개변수를 선언
                int iProcessCount               = 0;
                this.g_sqlCommand.CommandText   = this.ProcedureName;
                this.g_sqlCommand.CommandType   = System.Data.CommandType.StoredProcedure;

                while (this.g_sqlCommand.Parameters.Count > 0)
                {
                    this.g_sqlCommand.Parameters.RemoveAt(0);
                }

                // Input 파라메터가 없는 경우 회사코드를 이 구문에서 추가한다.
                // Input 파라메터가 있는 경우 회사코드는 아래 foreach 구문에서 추가한다.
                if (this.InputParams.Count == 0)
                {
                    this.g_sqlCommand.Parameters.AddWithValue("P_CO_CD", baseClass.CompanyCode); // 회사코드
                }

                foreach (System.Collections.Generic.KeyValuePair<string, object> dicEntity in this.InputParams)
                {
                    if (iProcessCount == 0)
                    {
                        // 파라메터 Key중 회사코드가 없는 경우 프로시저 파라메터에 회사코드키와 값을 추가한다.
                        if (this.InputParams.ContainsKey("P_CO_CD") == false)
                        {
                            this.g_sqlCommand.Parameters.AddWithValue("P_CO_CD", baseClass.CompanyCode); // 회사코드
                        }

                        iProcessCount++;
                    }

                    if (dicEntity.Value == null)
                    {
                        this.g_sqlCommand.Parameters.AddWithValue(dicEntity.Key.ToString(), SqlDbType.VarChar).Value = string.Empty;
                    }
                    else
                    {
                        switch (dicEntity.Value.GetType().Name.ToUpper())
                        {
                            case "INT16":
                                this.g_sqlCommand.Parameters.Add(dicEntity.Key.ToString(), SqlDbType.SmallInt).Value = (System.Int16)(dicEntity.Value);
                                break;
                            case "INT32":
                                this.g_sqlCommand.Parameters.Add(dicEntity.Key.ToString(), SqlDbType.Int).Value = (System.Int32)(dicEntity.Value);
                                break;
                            case "INT64":
                                this.g_sqlCommand.Parameters.Add(dicEntity.Key.ToString(), SqlDbType.Decimal).Value = (System.Int64)(dicEntity.Value);
                                break;
                            case "DECIMAL":
                                this.g_sqlCommand.Parameters.Add(dicEntity.Key.ToString(), SqlDbType.Decimal).Value = (System.Decimal)(dicEntity.Value);
                                break;
                            case "NUMERIC":
                                this.g_sqlCommand.Parameters.Add(dicEntity.Key.ToString(), SqlDbType.Decimal).Value = (int)(dicEntity.Value);
                                break;
                            case "DATATABLE":
                                this.g_sqlCommand.Parameters.Add(dicEntity.Key.ToString(), SqlDbType.Structured).Value = (DataTable)(dicEntity.Value);
                                break;
                            default:
                                this.g_sqlCommand.Parameters.Add(dicEntity.Key.ToString(), SqlDbType.NVarChar).Value = dicEntity.Value.ToString();
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

        #region > SetOutputParameters - Output 파라메터를 정의한다.
        /// <summary>
        /// Output 파라메터를 정의한다.
        /// </summary>
        private void SetOutputParameters()
        {
            try
            {
                foreach (KeyValuePair<object, BaseEnumClass.MSSqlOutputDataType> dicEntry in this.MSSqlOutputParams)
                {
                    // 파라메터 명
                    var strParamterName     = dicEntry.Key.ToString();

                    switch (dicEntry.Value)
                    {
                        case BaseEnumClass.MSSqlOutputDataType.INT16:
                            this.g_sqlCommand.Parameters.Add(strParamterName, SqlDbType.SmallInt).Direction = ParameterDirection.Output;
                            break;

                        case BaseEnumClass.MSSqlOutputDataType.INT32:
                            this.g_sqlCommand.Parameters.Add(strParamterName, SqlDbType.Int).Direction = ParameterDirection.Output;
                            break;

                        case BaseEnumClass.MSSqlOutputDataType.INT64:
                        case BaseEnumClass.MSSqlOutputDataType.DECIMAL:
                        case BaseEnumClass.MSSqlOutputDataType.NUMERIC:
                            this.g_sqlCommand.Parameters.Add(strParamterName, SqlDbType.Decimal).Direction = ParameterDirection.Output;
                            break;

                        case BaseEnumClass.MSSqlOutputDataType.VARCHAR:
                            this.g_sqlCommand.Parameters.Add(strParamterName, SqlDbType.VarChar).Direction = ParameterDirection.Output;
                            break;

                        case BaseEnumClass.MSSqlOutputDataType.NVARCHAR:
                            this.g_sqlCommand.Parameters.Add(strParamterName, SqlDbType.NVarChar).Direction = ParameterDirection.Output;
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

                if (this.g_sqlCommand.Transaction == null)
                {
                    this.g_sqlCommand.Transaction       = this.g_sqlConnection.BeginTransaction();
                    this.TransactionState               = BaseEnumClass.TransactionState_MSSQL.TransactionStarted;
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
                this.g_sqlCommand.Transaction.Commit();
            }
            finally
            {
                this.TransactionState = BaseEnumClass.TransactionState_MSSQL.None;
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
                if (this.g_sqlCommand.Transaction != null)
                {
                    this.g_sqlCommand.Transaction.Rollback();
                    this.TransactionState = BaseEnumClass.TransactionState_MSSQL.None;
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion
    }
}
