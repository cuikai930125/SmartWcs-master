using SMART.WCS.Common_Etc;
using SMART.WCS.Common_Etc.Controls;
using SMART.WCS.Common_Etc.Data;
using SMART.WCS.Common_Etc.Data.DataMembers;
using SMART.WCS.Common_Etc.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SMART.WCS.Kiosk
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : Window
    {
        #region ▩ 전역변수
        #region > 상수 선언
        //private readonly string NORMAL_IMAGE_PATH = "pack://application:,,,/SMART.WCS.Resource;component/Image/Loginbutton.png";
        //private readonly string HOVER_IMAGE_PATH = "pack://application:,,,/SMART.WCS.Resource;component/Image/Loginbutton_hover.png";
        //private readonly string CLICK_IMAGE_PATH = "pack://application:,,,/SMART.WCS.Resource;component/Image/Loginbutton.png";

        /// <summary>
        /// 회사코드 (CP - 쿠팡)
        /// </summary>
        public readonly string COMPANY_CODE = "CP";
        #endregion
        
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 데이터테이블 : 데이터베이스 연결 문자열 정보
        /// </summary>
        DataTable g_dtDatabaseConnectionInfo = new DataTable();

        /// <summary>
        /// App.config에 설정된 DB 접속 타입값
        /// </summary>
        private string g_strConfigDBConnectType = string.Empty;
        #endregion

        #region ▩ 생성자
        public Login()
        {
            try
            {
                InitializeComponent();

                this.SetLanguage();

                // 컨트롤 데이터 초기화
                this.InitValue();

                //센터코드, 센터명, 아이디, 이름, 권한, 언어
                this.InitControl();

                //// 언어 초기화
                //// 윈도우 언어로 설정
                //this.SetLanguage();

                // 이벤트 초기화
                this.InitEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitValue - 컨트롤 데이터 초기화
        /// <summary>
        /// 컨트롤 데이터 초기화
        /// </summary>
        private void InitValue()
        {
            try
            {
                //this.lblLogin.Text = Application.Current.Resources["LOG_IN"].ToString();
            }
            catch { throw; }
        }
        #endregion

        #region > InitControl - 컨트롤 초기화
        /// <summary>
        /// 컨트롤 초기화
        /// </summary>
        private void InitControl()
        {
            try
            {
                //// 언어 콤보박스 설정
                //this.BaseClass.BindingFirstComboBox(this.cboLang, "LANG");

                //if (this.cboLang.Items.Count > 0)
                //{
                //    this.cboLang.SelectedIndex = 0;
                //}

                //// 첫번째 Row 선택이 Defult가 아닌 경우 아래 구분 수행
                //var liVar = this.cboLang.ItemsSource as List<System.Windows.Controls.ComboBox>;
                //if (liVar.Count > 0)
                //{
                //    this.cboLang.SelectedIndex = 0;
                //}

                // 언어
                this.BindingComboBoxLangInfo();

                // 센터코드
                this.BindingComboBoxCenterInfo();
            }
            catch { throw; }
        }
        #endregion

        #region > InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            try
            {
                this.imgLoginBtn.PreviewMouseLeftButtonUp += ImgLoginBtn_PreviewMouseLeftButtonUp;
                this.imgLoginBtn.MouseLeave += ImgLoginBtn_MouseLeave;
                this.imgLoginBtn.MouseEnter += ImgLoginBtn_MouseEnter;
                this.imgLoginBtn.MouseUp += ImgLoginBtn_MouseLeave;

                this.lblLogin.PreviewMouseLeftButtonUp += ImgLoginBtn_PreviewMouseLeftButtonUp;
                this.lblLogin.MouseLeave += ImgLoginBtn_MouseLeave;
                this.lblLogin.MouseEnter += ImgLoginBtn_MouseEnter;
                this.lblLogin.MouseUp += ImgLoginBtn_MouseLeave;

                this.txtPwd.KeyDown += TxtPwd_KeyDown;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > BindingComboBoxLangInfo - 콤보박스 바인딩 (언어)
        /// <summary>
        /// 콤보박스 바인딩 (센터코드)
        /// </summary>
        private void BindingComboBoxLangInfo()
        {
            try
            {
                DataTable dtCommonData = CommonComboBox.GetFirstCommonData("LANG");

                var liComboBoxInfo          = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtCommonData);
                this.cboLang.ItemsSource  = liComboBoxInfo;

                if (dtCommonData.Rows.Count > 0)
                {
                    this.cboLang.SelectedIndex = 0;
                }
            }
            catch { throw; }
        }
        #endregion

        #region > BindingComboBoxCenterInfo - 콤보박스 바인딩 (센터코드)
        /// <summary>
        /// 콤보박스 바인딩 (센터코드)
        /// </summary>
        private void BindingComboBoxCenterInfo()
        {
            try
            {
                DataTable dtCenterInfo = this.GetDatabaseConnectionInfo();

                var query = from p in dtCenterInfo.AsEnumerable()
                            group p by new
                            {
                                CNTR_CD             = p.Field<string>("CNTR_CD"),
                                CNTR_NM             = p.Field<string>("CNTR_NM"),
                                DB_CONN_TYPE        = p.Field<string>("DB_CONN_TYPE")
                            } into q
                            where q.Key.DB_CONN_TYPE.Equals("DEV")
                            select new
                            {
                                CODE        = q.Key.CNTR_CD,
                                NAME        = q.Key.CNTR_NM,
                            };

                DataTable dtNewTable = new DataTable();
                dtNewTable.Columns.Add("CODE",       typeof(string));
                dtNewTable.Columns.Add("NAME",       typeof(string));

                foreach (var itemCenterInfo in query)
                {
                    DataRow drNewRow = dtNewTable.NewRow();
                    drNewRow["CODE"] = itemCenterInfo.CODE;
                    drNewRow["NAME"] = itemCenterInfo.NAME;
                    dtNewTable.Rows.    Add(drNewRow);
                }

                var liComboBoxInfo          = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtNewTable);
                this.cboCenter.ItemsSource  = liComboBoxInfo;

                if (dtNewTable.Rows.Count > 0)
                {
                    this.cboCenter.SelectedIndex = 0;
                }
            }
            catch { throw; }
        }
        #endregion

        void SetLanguage()
        {
            try
            {
                // CHOO
                var strCultureName      = CultureInfo.CurrentCulture.ToString();
                var strCountryCode      = strCultureName.Substring(3, 2);

                Uri langDictUri;
                //String value = CUtil.GetConfigValue("LANGUAGE");
                ResourceDictionary dic = Application.Current.Resources.MergedDictionaries[0] as ResourceDictionary;

                if (strCountryCode.ToUpper().Contains("KR"))
                {
                    langDictUri = new Uri("pack://application:,,,/SCADA.UI;component/Resources/Lang/Lang_Kor.xaml");
                    dic.Source = langDictUri;
                }
                else if (strCountryCode.ToUpper().Contains("EN"))
                {
                    langDictUri = new Uri("pack://application:,,,/SCADA.UI;component/Resources/Lang/Lang_Eng.xaml");
                    dic.Source = langDictUri;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void LoginProcess()
        {
            try
            {
                var strUserID       = this.txtUserID.Text.Trim();   // 사용자 ID

                #region + 로그인 성공
                if (this.chkRememberID.IsChecked == true)
                {
                    this.BaseClass.LoginUserID = strUserID;
                }
                else
                {
                    this.BaseClass.LoginUserID = string.Empty;
                }

                // 센터코드를 저장한다.
                this.BaseClass.CenterCD     = KioskBaseClass.ComboBoxSelectedKeyValue(this.cboCenter);
                this.BaseClass.CountryCode  = KioskBaseClass.ComboBoxSelectedKeyValue(this.cboLang);

                var dsLoginInfo         = this.GetMenuList();

                var query = dsLoginInfo.Tables[0].AsEnumerable().Where(p => p.Field<string>("MENU_TYPE") == "KIOSK" && p.Field<string>("PARENT_ID") != "FVRT").ToList();


                if (dsLoginInfo.Tables[0].Rows.Count == 0)
                {
                    // ERR_NOT_MENU_DATA - 메뉴 데이터가 없습니다.
                    //this.BaseClass.MsgError("ERR_NOT_MENU_DATA");
                    return;
                }
                else
                {
                    if (query.Count > 0)
                    {
                        this.BaseClass.RememberChecked = this.chkRememberID.IsChecked == true ? true : false;
                        
                        foreach (var item in query)
                        {
                            // 권한코드를 설정한다.
                            this.BaseClass.RoleCode = item.Field<string>("ROLE_MENU_CD");
                        }

                        this.CallMainWindow();
                    }
                }
                #endregion
            }
            catch { throw; }
        }

        private void CallMainWindow()
        {
            try
            {
                // 전역으로 사용하는 사용자 ID를 사용자 정보에 저장한다.
                this.BaseClass.UserID       = this.txtUserID.Text.Trim();

                if (this.chkRememberID.IsChecked == true)
                {
                    this.BaseClass.LoginUserID  = this.txtUserID.Text.Trim();   // 사용자 ID
                }
                else
                {
                    this.BaseClass.LoginUserID  = string.Empty;
                }

                // 선택한 센터코드를 사용자 정보에 저장한다.
                this.BaseClass.LoginCenterCD    = KioskBaseClass.ComboBoxSelectedKeyValue(this.cboCenter);

                MainWindow frmMain = new MainWindow();
                frmMain.Show();

                this.Close();
            }
            catch { throw; }
        }

        #region > 데이터 관련
        #region >> GetDatabaseConnectionInfo - 센터 정보를 조회한다.
        /// <summary>
        /// 센터 정보를 조회한다.
        /// </summary>
        /// <returns></returns>
        private DataTable GetDatabaseConnectionInfo()
        {
            try
            {
                DataTable dtRtnValue        = null;
                var strProcedureName        = "PK_C1001.SP_CNTR_LIST_INQ";
                var dicInputParam           = new Dictionary<string, object>();
                string[] arrOutputParam     = {"O_CNTR_LIST", "O_RSLT"};

                using (FirstDataAccess dataAccess = new FirstDataAccess())
                {
                    var dsDatabaseConnectionInfo = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);

                    var strErrCode      = string.Empty;
                    var strErrMsg       = string.Empty;

                    if (this.BaseClass.CheckResultDataProcess(dsDatabaseConnectionInfo, ref strErrCode, ref strErrMsg) == true)
                    {
                        dtRtnValue = dsDatabaseConnectionInfo.Tables["O_CNTR_LIST"];
                    }
                    else
                    {
                        dtRtnValue = null;
                        MessageBox.Show(strErrMsg);
                    }
                }

                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> [조회] GetSP_LOGIN_LIST_INQ - 로그인
        /// <summary>
        /// 로그인
        /// </summary>
        /// <returns></returns>
        private int GetSP_LOGIN_LIST_INQ()
        {
            try
            {
                int iRtnValue   = -1;

                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_C1001.SP_LOGIN_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };
                #endregion
                
                #region Input 파라메터
                var strCenterCD             = ((ComboBoxInfo)this.cboCenter.SelectedItem).CODE;      // 센터코드
                var strIpAddess             = this.BaseClass.GetIPAddress();
                var strUserID               = this.txtUserID.Text.Trim();
                var strPwd                  = this.txtPwd.Password.Trim();
                
                dicInputParam.Add("P_CNTR_CD",          strCenterCD);                               // 센터코드
                dicInputParam.Add("P_USER_ID",          strUserID);                                // 사용자 ID
                dicInputParam.Add("P_PWD",              this.BaseClass.EncryptSHA256(strPwd));     // 비밀번호
                dicInputParam.Add("P_IP_NO",            strIpAddess);                               // 로컬 IP
                #endregion

                using (FirstDataAccess dataAccess = new FirstDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }

                if (dsRtnValue.Tables[0].Rows.Count == 0)
                {
                    //this.BaseClass.MsgError("", BaseEnumClass.CodeMessage.MESSAGE);
                    iRtnValue = 99;
                }
                else
                {
                    switch (dsRtnValue.Tables[0].Rows[0]["CODE"].ToString())
                    {
                        case "0":
                            #region + 로그인 성공
                            iRtnValue = 0;
                            #endregion
                            break;
                        case "1":
                            #region + 비밀번호 재설정

                            // INFO_PASSWORD_RESETTING_OPEN_WINDOW - 비밀번호 재설정이 필요합니다.|창이 오픈되면 비밀번호를 재설정해주세요.
                            //this.BaseClass.MsgInfo("INFO_PASSWORD_RESETTING_OPEN_WINDOW");
                            iRtnValue = 1;
                            #endregion
                            break;

                        case "2":
                            #region + 사용자 정보 없음
                            iRtnValue = 2;
                            #endregion
                            break;
                        case "9":
                            #region 연결 IP 체크
                            //this.BaseClass.MsgError(dsRtnValue.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                            iRtnValue = 9;
                            #endregion
                            break;
                    }
                }

                return iRtnValue;
            }
            catch { throw; }
        }
        #endregion

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
                DataSet dsRtnValue = null;
                var strProcedureName = "PK_C1000.SP_MENU_LIST_INQ";
                Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
                string[] arrOutputParam = { "O_MENU_LIST", "O_USER_INFO_LIST", "O_RSLT" };
                #endregion

                #region Input 파라메터
                var strCenterCD     = Kiosk.KioskBaseClass.ComboBoxSelectedKeyValue(this.cboCenter);
                var strUserID       = this.txtUserID.Text.Trim();

                dicInputParam.Add("P_CNTR_CD", strCenterCD);       // 센터 코드
                dicInputParam.Add("P_USER_ID", strUserID);         // 사용자 ID
                #endregion

                using (FirstDataAccess dataAccess = new FirstDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }

                return dsRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 로그인 버튼 관련 이벤트
        #region >> 버튼 이미지 마우스 후버 관련 이벤트
        private void ImgLoginBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            //this.imgLoginBtn.Source = new BitmapImage(new Uri(HOVER_IMAGE_PATH));
        }

        private void ImgLoginBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            //this.imgLoginBtn.Source = new BitmapImage(new Uri(NORMAL_IMAGE_PATH));
        }
        #endregion

        #region >> 로그인 버튼 이미지 클릭 이벤트
        /// <summary>
        /// 로그인 버튼 이미지 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgLoginBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //this.imgLoginBtn.Source = new BitmapImage(new Uri(CLICK_IMAGE_PATH));

            try
            {
                var strUserID       = this.txtUserID.Text.Trim();   // 사용자 ID
                var strPwd          = this.txtPwd.Password.Trim();  // 비밀번호

                if (strUserID.Length == 0)
                {
                    MessageBox.Show(Application.Current.Resources["MSG_WARNING"].ToString(), Application.Current.Resources["MSG_NOT_INPUT_USER_ID"].ToString(), MessageBoxButton.OK);
                    this.txtUserID.Focus();
                    return;
                }

                if (strPwd.Length == 0)
                {
                    MessageBox.Show(Application.Current.Resources["MSG_WARNING"].ToString(), Application.Current.Resources["MSG_NOT_PWD"].ToString(), MessageBoxButton.OK);
                    this.txtPwd.Focus();
                    return;
                }

                // 로그인 처리
                var iResult = this.GetSP_LOGIN_LIST_INQ();

                if (iResult == 0)
                {
                    this.LoginProcess();
                }
                else if (iResult == 1)
                {
                    #region + 비밀번호 재설정
                    //var strCenterCD = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCenter);

                    //using (SWCS201_02P frmPopup = new SWCS201_02P(strUserID, strCenterCD))
                    //{
                    //    frmPopup.PasswordChangeResult += FrmPopup_PasswordChangeResult;
                    //    frmPopup.ShowDialog();
                    //}

                    //if (this.g_bInitPasswordSaveYN == true)
                    //{
                    //    this.LoginProcess(strUserID);
                    //}
                    //else
                    //{
                    //    this.Close();
                    //}
                    #endregion
                }
                else if (iResult == 2)
                {
                    #region + 사용자 정보 없음
                    //if (strPwd.Trim().Length == 0)
                    //{
                    // ERR_NOT_USER_INFO - 일치하는 사용자 정보가 없습니다.
                    //this.BaseClass.MsgError("ERR_NOT_USER_INFO");
                    return;
                    //}
                    #endregion
                }
            }
            catch // (Exception err)
            {
                //this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region > 비밀번호 재설정 팝업 결과값 수신 이벤트 (Delegate)
        /// <summary>
        /// 비밀번호 재설정 팝업 결과값 수신 이벤트 (Delegate)
        /// </summary>
        /// <param name="_bResultYN"></param>
        private void FrmPopup_PasswordChangeResult(bool _bResultYN)
        {
            try
            {
                //this.g_bInitPasswordSaveYN = _bResultYN;
            }
            catch //(Exception err)
            {
                //this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 비밀번호 TextBox Enter키 입력 이벤트
        /// <summary>
        /// 비밀번호 TextBox Enter키 입력 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPwd_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    this.ImgLoginBtn_PreviewMouseLeftButtonUp(null, null);
                }
            }
            catch // (Exception err)
            {
                //this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion
    }
}
