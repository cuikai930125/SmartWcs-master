using SMART.WCS.Common.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common.DataBase
{
    /// <summary>
    /// MariaDB 데이터 엑세스 클래스
    /// 2019-08-26
    /// 추성호
    /// </summary>
    public class MariaDBDataAccess : DisposeClass
    {
        #region ▩ 전역변수
        /// <summary>
        /// MariaDB 객체
        /// </summary>
        MariaDBLibrary g_mariaDBLibrary = new MariaDBLibrary();
        #endregion

        #region ▩ 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public MariaDBDataAccess()
        {
            try
            {
                // PC에 저장된 메인 데이터베이스 정보로 Enum형식의 데이터베이스 종류값을 조회한다.
                BaseEnumClass.DatabaseKind enumDatabaseKindShort        = HelperClass.GetDatabaseKind();
                // 데이터베이스 연결 문자열 (복호화 된 데이터)
                this.ConnectionStringDecryptValue                       = Configuration.GetConnectionStringDecryptValue(enumDatabaseKind);
            }
            catch { throw; }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_enumDatabaseKindShort">데이터베이스 종류</param>
        public MariaDBDataAccess(BaseEnumClass.DatabaseKind _enumDatabaseKind)
        {
            try
            {
                // 데이터베이스 연결 문자열 (복호화 된 데이터)
                this.ConnectionStringDecryptValue = Configuration.GetConnectionStringDecryptValue(_enumDatabaseKindShort);
            }
            catch { throw; }
        }
        #endregion

        #region ▩ 속성
        #region > ConnectionState - 데이터베이스 연결 상태
        /// <summary>
        /// 데이터베이스 연결 상태
        /// </summary>
        public ConnectionState ConnectionState
        {
            get { return this.g_mariaDBLibrary.ConnectionState; }
        }
        #endregion

        #region > TransactionState - 트랜잭션 상태
        /// <summary>
        /// 트랜잭션 상태
        /// </summary>
        public BaseEnumClass.TransactionState_MariaDB TransactionState
        {
            get { return this.g_mariaDBLibrary.TransactionState; }
        }
        #endregion

        #region ConnectionStringDecryptValue - 복호화 된 데이터베이스 연결 문자열
        /// <summary>
        /// 복호화 된 데이터베이스 연결 문자열
        /// </summary>
        public string ConnectionStringDecryptValue { get; set; }
        #endregion
        #endregion

        #region ▩ 함수
        #region > 트랜잭션
        #region >> BeginTransaction - 트랜잭션 시작
        /// <summary>
        /// 트랜잭션
        /// </summary>
        public void BeginTransaction()
        {
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
        }
        #endregion

        #region >> CommitTransaction - 트랜잭션 완료
        /// <summary>
        /// 트랜잭션 완료
        /// </summary>
        public void CommitTransaction()
        {
            try
            {
                this.g_mariaDBLibrary.CommitTransaction();
                this.g_mariaDBLibrary.Close();
            }
            catch { throw; }
        }
        #endregion

        #region >> RollbackTransaction - 트랜잭션 롤백
        /// <summary>
        /// 트랜잭션 롤백
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                this.g_mariaDBLibrary.RollBackTransaction();
                this.g_mariaDBLibrary.Close();
            }
            catch { throw; }
        }
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
        public DataSet GetSpDataSet(string _strProcedureName, Dictionary<string, object> _dicInputParam)
        {
            try
            {
                if (this.g_mariaDBLibrary == null)
                {
                    this.g_mariaDBLibrary = new MariaDBLibrary();
                }

                return this.g_mariaDBLibrary.GetDataSet(this.ConnectionStringDecryptValue, _strProcedureName, _dicInputParam);
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
        public DataTable GetSpDataTable(string _strProcedureName, Dictionary<string, object> _dicInputParam)
        {
            try
            {
                if (this.g_mariaDBLibrary == null)
                {
                    this.g_mariaDBLibrary = new MariaDBLibrary();
                }

                return this.g_mariaDBLibrary.GetDataTable(this.ConnectionStringDecryptValue, _strProcedureName, _dicInputParam);
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion
    }
}
    