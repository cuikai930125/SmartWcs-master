using DevExpress.Xpf.Editors;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Common.Utility;
using SMART.WCS.Control;
using SMART.WCS.UI.Optimization.Views.QPS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SMART.WCS.Main
{
    /// <summary>
    /// 로그인
    /// </summary>
    public partial class LoginSample : Window
    {
        #region ▩ 전역변수
        /// <summary>
        /// Base Class
        /// </summary>
        BaseClass BaseClass                     = new BaseClass();

        /// <summary>
        /// 데이터테이블 : 데이터베이스 연결 문자열 정보
        /// </summary>
        DataTable g_dtDatabaseConnectionInfo    = new DataTable();

        /// <summary>
        /// App.config에 설정된 DB 접속 타입값
        /// </summary>
        private string g_strConfigDBConnectType = string.Empty;
        #endregion

        #region ▩ 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_dtDatabaseConnectionInfo">데이터베이스 연결 문자열 정보</param>
        public LoginSample(DataTable _dtDatabaseConnectionInfo)
        {
            InitializeComponent();

            //this.BaseInfo = ((SMART.WCS.Control.BaseApp)System.Windows.Application.Current).BASE_INFO;

            this.txtUserID.Text = this.BaseClass.UserID.Trim();

            // App.config에 설정된 DB 접속 타입값
            this.g_strConfigDBConnectType   = this.BaseClass.GetAppSettings("DBConnectType_DEV_REAL").Equals("DEV") == true ? "DEV" : "REAL";
            // CHOO 2020-01-20
            //if (this.g_strConfigDBConnectType.ToUpper().Equals("REAL") == true)
            //{
            //    this.lbRadio.Visibility = Visibility.Collapsed;
            //}

            // 로그인 버튼 클릭 시 센터별 데이터베이스 연결 문자열을 가져오기 위해 데이터테이블을 전역변수에 저장한다.
            this.g_dtDatabaseConnectionInfo = _dtDatabaseConnectionInfo;

            // 센터 콤보박스 설정
            this.BindingComboBoxCenter();
            // 언어 콤보박스 설정
            this.BaseClass.BindingFirstComboBox(this.cboLang, "LANG");

            if (this.cboLang.Items.Count > 0)
            {
                this.cboLang.SelectedIndex = 0;
            }

            // 첫번째 Row 선택이 Defult가 아닌 경우 아래 구분 수행
            var liVar = this.cboLang.ItemsSource as List<ComboBoxInfo>;
            if (liVar.Count > 0)
            {
                this.cboLang.SelectedIndex = 0;
            }

            //var aaa = System.Globalization.CultureInfo.GetCultures(CultureTypes.AllCultures);
        }
        #endregion

        #region ▩ 함수
        #region > BindingComboBoxCenter - 콤보박스 바인딩 (센터코드) - 공통 콤보박스 사용하지 않음
        /// <summary>
        /// 콤보박스 바인딩 (센터코드) - 공통 콤보박스 사용하지 않음
        /// </summary>
        private void BindingComboBoxCenter()
        {
            #region 중복 센터 정보를 필터링 하기 위해 GroupBy 처리한다.
            var query = from p in this.g_dtDatabaseConnectionInfo.AsEnumerable()
                        group p by new
                        {
                            CNTR_CD = p.Field<string>("CNTR_CD") ,
                            CNTR_NM = p.Field<string>("CNTR_NM")
                        } into q
                        select new
                        {
                            CODE    = q.Key.CNTR_CD ,
                            NAME    = q.Key.CNTR_NM
                        };
            #endregion

            // 콤보박스 바인딩을 위해 데이터를 저장하기 위한 데이터테이블
            DataTable dtNewTable    = null;
            // 데이터테이블 스키마를 정의한다. (공통코드 형식)
            dtNewTable              = this.BaseClass.CreateDataTableSchema(dtNewTable, BaseEnumClass.CreateTableSchemaKind.COMMON_CODE);

            // GroupBy 처리한 데이터를 데이터 테이블에 저장한다.
            foreach (var itemCenterInfo in query)
            {
                var aaa = itemCenterInfo.CODE;
                var bbb = itemCenterInfo.NAME;

                DataRow drNewRow    = dtNewTable.NewRow();
                drNewRow["CODE"]    = itemCenterInfo.CODE;
                drNewRow["NAME"]    = itemCenterInfo.NAME;
                dtNewTable.Rows.Add(drNewRow);
            }

            // 데이터테이블에 저장한 센터정보를 콤보박스 바인딩을 위해 List형식으로 저장한다.
            var liComboBoxInfo = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtNewTable);
            // 센터 정보를 콤보박스에 바인딩한다.
            this.cboCenter.ItemsSource = liComboBoxInfo;

            // 바인딩 데이터가 있는 경우 첫번째 Row를 선택하도록 한다.
            if (liComboBoxInfo.Count > 0)
            {
                this.cboCenter.SelectedIndex = 0;
            }
        }
        #endregion

        #region > CallMainWindow - 메인 화면을 호출한다.
        /// <summary>
        /// 메인 화면을 호출한다.
        /// </summary>
        /// <param name="_dsLoginInfo">로그인 정보</param>
        private void CallMainWindow(DataSet _dsLoginInfo, DataSet _dsNoticeInfo)
        {
            try
            {
                if (this.chkRememberID.IsChecked == true)
                {
                    this.BaseClass.UserID = this.txtUserID.Text.Trim();
                }

                MainWindow frmMain = new MainWindow(_dsLoginInfo, _dsNoticeInfo.Tables[0]);
                frmMain.Show();

                this.Close();
            }
            catch { throw; }
        }
        #endregion

        #region > 데이터 관련
        #region >> [조회] GetMenuList - 메뉴 정보를 조회한다.
        /// <summary>
        /// 메뉴 정보를 조회한다.
        /// </summary>
        /// <returns></returns>
        private DataSet GetMenuList()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_C1000.SP_MENU_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_MENU_LIST", "O_USER_INFO_LIST", "O_RSLT" };
                #endregion

                #region Input 파라메터
                var strCenterCD         = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCenter);
                var strUserID           = this.txtUserID.Text.Trim();
                #endregion

                dicInputParam.Add("P_CNTR_CD",      strCenterCD);       // 센터 코드
                dicInputParam.Add("P_USER_ID",      strUserID);         // 사용자 ID

                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }

                return dsRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #region >> [조회] GetNoticeList - 공지사항 정보를 조회한다.
        /// <summary>
        /// 공지사항 정보를 조회한다.
        /// </summary>
        /// <returns></returns>
        /// 
        private DataSet GetNoticeList()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue = null;
                var strProcedureName = "PK_C1000.SP_NOTICE_TALLY_INQ";
                Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
                string[] arrOutputPram = { "O_NEW_NOTICE_RSLT", "O_RSLT" };
                #endregion

                #region Input 파라메터
                var strCenterCD = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCenter);
                var strUserID = this.txtUserID.Text.Trim();
                var strDateTime = DateTime.Now.ToString("yyyyMMdd");
                #endregion

                dicInputParam.Add("P_CNTR_CD", strCenterCD);       // 센터 코드
                dicInputParam.Add("P_USER_ID", strUserID);         // 사용자 ID
                dicInputParam.Add("P_DATE", strDateTime);          // 오늘날짜 (yyyymmdd)

                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputPram);
                }
                return dsRtnValue;
            }
            catch { throw; }

        }
        #endregion

        #endregion
        #endregion

        #region ▩ 이벤트
        #region BtnLogin_PreviewMouseLeftButtonUp - 로그인 버튼 클릭 이벤트
        /// <summary>
        /// 로그인 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLogin_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                /// 데이터 복호화 변경용
                var strDecryptValue = this.BaseClass.DecryptAES256("+BEI3BwkjVJVby3MzHwfFjlxrucH5QxAtKipcP25ZIU=");

                //// 데이터 암호화 변경용
                //string strEncryptValue = this.BaseClass.EncryptAES256("http://localhost:7002/");
                //string strEncryptValue2 = this.BaseClass.EncryptAES256("http://localhost:7002/LiveUpdate/WCS/File/");
                //string strEncryptValue3 = this.BaseClass.EncryptAES256("http://localhost:7002/LiveUpdate/WCS/Version/");

                // 로그인 성공 후

                // 센터코드를 저장한다.
                this.BaseClass.CenterCD             = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCenter);
                string strSelectedDBConnectType     = string.Empty;
                
                if (this.g_strConfigDBConnectType.ToUpper().Equals("DEV") == true)
                {
                    // 개발/운영 선택 RadioButton이 활성화 된 경우 선택값에 따라 값을 설정한다.
                    strSelectedDBConnectType = this.rdoDEV.IsSelected == true ? "DEV" : "REAL";
                }
                else
                {
                    // 개발/운영 선택 RadioButton이 비활성화 된 경우 운영("REAL")으로 설정한다.
                    strSelectedDBConnectType = this.g_strConfigDBConnectType;
                }

                var query = this.g_dtDatabaseConnectionInfo.AsEnumerable().Where(p => p.Field<string>("DB_CONN_TYPE") == strSelectedDBConnectType).FirstOrDefault();

                if (query == null)
                {
                    this.BaseClass.MsgError("데이터베이스 연결 문자열이 존재하지 않습니다.");
                    return;
                }
                else
                {
                    this.BaseClass.DatabaseConnectionString_ORACLE      = query.Field<string>("ORCL_CONN_STR");  // 오라클 연결 문자열
                    this.BaseClass.DatabaseConnectionString_MSSQL       = query.Field<string>("MS_CONN_STR");    // MS-SQL 연결 문자열
                    this.BaseClass.DatabaseConnectionString_MariaDB     = query.Field<string>("MR_CONN_STR");    // MariaDB 연결 문자열
                }

                this.BaseClass.CountryCode = this.BaseClass.ComboBoxSelectedKeyValue(this.cboLang);

                var dsLoginInfo = this.GetMenuList();
                var dsNoticeInfo = this.GetNoticeList();
                if (dsLoginInfo.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("메뉴 데이터가 없습니다.");
                    return;
                }
                else if (dsLoginInfo.Tables[0].Rows.Count == 1)
                {
                    if (dsLoginInfo.Tables[0].Rows[0][0].ToString().Equals("FVRT") == true)
                    {
                        MessageBox.Show("메뉴 데이터가 없습니다.");
                        return;
                    }
                }
                else
                {
                    //this.BaseClass.MsgQuestion("ASK_EXCEL_DOWNLOAD");
                    //var aaa = this.BaseClass.BUTTON_CONFIRM_YN;

                    //var strParam = "CST_CD|SKU_CD";
                    //// {0}는 {1}의 ㅁㅁㅁㅁ
                    //this.BaseClass.MsgQuestion("ASK_EXCEL_DOWNLOAD", strParam);
                    //bool bRtnVAlue = this.BaseClass.BUTTON_CONFIRM_YN;

                    this.CallMainWindow(dsLoginInfo, dsNoticeInfo);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region RemoveSystemPropertyInfo - 프로그램 종료 시 시스템 프로퍼티에 저장한 데이터를 초기화한다.
        /// <summary>
        /// 프로그램 종료 시 시스템 프로퍼티에 저장한 데이터를 초기화한다.
        /// </summary>
        private void RemoveSystemPropertyInfo()
        {
            this.BaseClass.MainDatabase                         = string.Empty;
            this.BaseClass.DatabaseConnectionString_ORACLE      = string.Empty;
            this.BaseClass.DatabaseConnectionString_MSSQL       = string.Empty;
            this.BaseClass.DatabaseConnectionString_MariaDB     = string.Empty;
            this.BaseClass.CountryCode                          = string.Empty;

        }
        #endregion

        private void BtnTest_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //// 저장되었습니다.
            //this.BaseClass.MsgInfo("CMPT_SAVE");
            //// 저장 중 오류가 발생했습니다.
            //this.BaseClass.MsgError("ERR_SAVE");
            //// 저장하시겠습니까?
            //this.BaseClass.MsgQuestion("ASK_SAVE");

            //this.BaseClass.MsgInfo("테스트 프로젝트입니다.", BaseEnumClass.CodeMessage.MESSAGE);
            //this.BaseClass.MsgError("테스트 프로젝트입니다.", BaseEnumClass.CodeMessage.MESSAGE);
            //this.BaseClass.MsgQuestion("테스트 프로젝트입니다.", BaseEnumClass.CodeMessage.MESSAGE);


            //// {0}는 {1}입니다.
            //var strConditionValue   = "CST_CD|CNTR_CD";
            //this.BaseClass.MsgInfo("ERR_NOT_INPUT", strConditionValue);
            //this.BaseClass.MsgError("ERR_NOT_INPUT", strConditionValue);

            //this.BaseClass.MsgQuestion("ERR_NOT_INPUT", strConditionValue);
            //if (this.BaseClass.BUTTON_CONFIRM_YN == true) { MessageBox.Show("TRUE"); } else { MessageBox.Show("FALSE"); }

            string strUploadNo = "ORD_20191025141532";
            using (O1007_QPS_01P frmPopup = new O1007_QPS_01P(strUploadNo))
            {
                frmPopup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                frmPopup.ShowDialog();
            }
        }
    }
}
