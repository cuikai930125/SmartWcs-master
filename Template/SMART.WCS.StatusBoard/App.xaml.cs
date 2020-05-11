using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.StatusBoard
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        BaseClass BaseClass = new BaseClass();

        public App()
        {
            this.Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {

                this.BaseClass.CompanyCode = this.BaseClass.GetAppSettings("CompanyCode");

                // 데이터베이스 종류를 설정한다.
                this.BaseClass.MainDatabase = this.BaseClass.GetAppSettings("MainDatabase");

                // App.config에 설정한 Database 연결 타입을 가져온다.
                var strDBConnectionType = this.BaseClass.GetAppSettings("DBConnectType_DEV_REAL");

                string[] args = Environment.GetCommandLineArgs();

                DataTable dtRtnValue = this.GetDatabaseConnectionInfo(strDBConnectionType);
                if (dtRtnValue == null) { return; }

                this.BaseClass.CenterCD         = this.BaseClass.GetAppSettings("CenterCode");
                var strConfigDBConnectType      = this.BaseClass.GetAppSettings("DBConnectType_DEV_REAL").Equals("DEV") == true ? "DEV" : "REAL";
                var query                       = dtRtnValue.AsEnumerable().Where(p => p.Field<string>("DB_CONN_TYPE").Equals(strConfigDBConnectType) && p.Field<string>("CNTR_CD").Equals(this.BaseClass.CenterCD)).FirstOrDefault();

                if (query == null)
                {
                    // ERR_NOT_EXIST_DATABASE_CONNECT_STRING - 데이터베이스 연결 문자열이 존재하지 않습니다.
                    this.BaseClass.MsgError("ERR_NOT_EXIST_DATABASE_CONNECT_STRING");
                    return;
                }
                else
                {
                    this.BaseClass.DatabaseConnectionString_ORACLE = query.Field<string>("ORCL_CONN_STR");  // 오라클 연결 문자열
                    this.BaseClass.DatabaseConnectionString_MSSQL = query.Field<string>("MS_CONN_STR");    // MS-SQL 연결 문자열
                    this.BaseClass.DatabaseConnectionString_MariaDB = query.Field<string>("MR_CONN_STR");    // MariaDB 연결 문자열
                }

            }
            catch (Exception err)
            {
                MessageBox.Show("프로그램 실행 중 오류가 발생했습니다.\r\n" + "(" + err.Message + ")" + "\r\n프로그램이 종료됩니다.");
                this.BaseClass.Error(err);
                App.Current.Shutdown();
            }
        }

        #region GetDatabaseConnectionInfo - 프로그램 실행 후 센터 정보와, 데이터베이스 연결 문자열을 조회한다.
        /// <summary>
        /// 프로그램 실행 후 센터별 데이터베이스 연결 문자열을 조회한다.
        /// </summary>
        /// <param name="_strDBConnectionType">데이터베이션 연결 타입</param>
        /// <returns></returns>
        private DataTable GetDatabaseConnectionInfo(string _strDBConnectionType)
        {
            try
            {
                DataTable dtRtnValue = null;
                var strProcedureName = "PK_C1001.SP_CNTR_LIST_INQ";
                var dicInputParam = new Dictionary<string, object>();
                string[] arrOutputParam = { "O_CNTR_LIST", "O_RSLT" };

                using (FirstDataAccess dataAccess = new FirstDataAccess())
                {
                    var dsDatabaseConnectionInfo = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);

                    var strErrCode = string.Empty;
                    var strErrMsg = string.Empty;

                    if (this.BaseClass.CheckResultDataProcess(dsDatabaseConnectionInfo, ref strErrCode, ref strErrMsg) == true)
                    {
                        dtRtnValue = dsDatabaseConnectionInfo.Tables["O_CNTR_LIST"];
                    }
                    else
                    {
                        dtRtnValue = null;
                        MessageBox.Show("프로그램 실행 중 오류가 발생했습니다.\r\n" + "(" + strErrMsg + ")" + "\r\n프로그램이 종료됩니다.");
                        App.Current.Shutdown();
                    }
                }

                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion
    }
}
