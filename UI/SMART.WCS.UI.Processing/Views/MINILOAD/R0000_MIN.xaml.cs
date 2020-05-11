using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LGCNS.ezControl.Common;
using LGCNS.ezControl.Core;
using System.Text.RegularExpressions;
using System.Configuration;
using SMART.WCS.UI.Processing.DataMembers.R0000_MIN.ECS.Common;
using DevExpress.Xpf.Editors;

namespace SMART.WCS.UI.Processing.Views.MINILOAD
{
    /// <summary>
    /// 권한관리 및 권한별 메뉴 리스트 관리
    /// </summary>
    public partial class R0000_MIN : UserControl
    {
        #region ▩ Detegate 선언
        #region > 메인화면 하단 좌측 상태바 값 반영
        public delegate void ToolStripStatusEventHandler(string value);
        #endregion

        #region > 즐겨찾기 변경후 메인화면 트리 컨트롤 Refresh 및 포커스 이동
        public delegate void TreeControlRefreshEventHandler();
        public event TreeControlRefreshEventHandler TreeControlRefreshEvent;
        #endregion
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// Base 클래서 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        /// <summary>
        /// 화면 로드 여부
        /// </summary>
        private bool g_isLoaded = false;

        /// <summary>
        /// ECS서버와 통신하기 위한 레퍼런스 객체
        /// </summary>
        CReference _ref_Conveyor = null;
        string curTime = string.Empty;
        enumReferenceState state_conveyor;
        #endregion

        #region ▩ 생성자 
        public R0000_MIN()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation"></param>
        public R0000_MIN(List<string> _liMenuNavigation)
        {
            InitializeComponent();

            try
            {
                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();

                curTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }                                                       
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        #region >> 그리드 컨트롤
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(R0000_MIN), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

        public static void SetIsEnabled(UIElement element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        private static void IsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                TableView view = source as TableView;
                //view.ShowingEditor += View_ShowingEditor;
            }
        }
        #endregion

        #endregion

        #region > Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(R0000_MIN), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string GridRowCount
        {
            get { return (string)GetValue(GridRowCountProperty); }
            set { SetValue(GridRowCountProperty, value); }
        }
        #endregion
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        private void InitControl()
        {
            //신호 송신 버튼 강제 길이 설정
            this.BtnImportLoadingSend.ButtonWidthLength = 120;
            this.BtnImportUnloadingSend.ButtonWidthLength = 120;
            this.BtnReleaseLoadingSend.ButtonWidthLength = 120;
            this.BtnReleaseUnloadingSend.ButtonWidthLength = 120;
            this.BtnReset.ButtonWidthLength = 120;

            //this.gridLeft_RoleList.ItemsSource = BCDSampleList;
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 화면 이벤트
            this.Loaded += R0000_MIN_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 연결 버튼 클릭 이벤트
            this.BtnConnect.PreviewMouseLeftButtonUp += BtnConnect_PreviewMouseLeftButtonUp;
            // 연결 해제 버튼 클릭 이벤트
            this.BtnDisconnect.PreviewMouseLeftButtonUp += BtnDisconnect_PreviewMouseLeftButtonUp;
            // Import Loading Send 버튼 클릭 이벤트
            this.BtnImportLoadingSend.PreviewMouseLeftButtonUp += BtnImportLoadingSend_PreviewMouseLeftButtonUp;
            // Import Unloading Send 버튼 클릭 이벤트
            this.BtnImportUnloadingSend.PreviewMouseLeftButtonUp += BtnImportUnloadingSend_PreviewMouseLeftButtonUp;
            // Release Loading Send 버튼 클릭 이벤트
            this.BtnReleaseLoadingSend.PreviewMouseLeftButtonUp += BtnReleaseLoadingSend_PreviewMouseLeftButtonUp;
            // Release Unloading Send 버튼 클릭 이벤트
            this.BtnReleaseUnloadingSend.PreviewMouseLeftButtonUp += BtnReleaseUnloadingSend_PreviewMouseLeftButtonUp;
            // Reset 버튼 클릭 이벤트
            this.BtnReset.PreviewMouseLeftButtonUp += BtnReset_PreviewMouseLeftButtonUp;
            #endregion
        }
        
        private void R0000_MIN_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.g_isLoaded == true) { return; }
            
            this.g_isLoaded = true;
        }
        #endregion
        #endregion

        #region > 기타 함수

        #region >> TabClosing - 탭을 닫을 때 데이터 저장 여부 체크
        /// <summary>
        /// 탭을 닫을 때 데이터 저장 여부 체크
        /// </summary>
        /// <returns></returns>
        public bool TabClosing()
        {
            bool isRtnValue     = true;

            isRtnValue = this.CheckModifyRoleMstData();

            if (isRtnValue == false)
            {
                isRtnValue = this.CheckModifyMenuListByRoleData();
            }

            return isRtnValue;
        }
        #endregion

        #region >> CheckModifyRoleMstData - 데이터 저장 여부를 체크한다. (권한 마스터 리스트)
        /// <summary>
        /// 각 탭의 데이터 저장 여부를 체크한다. (권한 마스터 데이터)
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyRoleMstData()
        {
            bool bRtnValue = true;

            //if (this.BCDSampleList.Any(p => p.IsNew || p.IsDelete || p.IsUpdate) == true)
            //{
            //    this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
            //    bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            //}

            return bRtnValue;
        }
        #endregion

        #region >> CheckModifyMenuListByRoleData - 데이터 저장 여부를 체크한다. (권한별 메뉴 리스트)
        /// <summary>
        /// 데이터 저장 여부를 체크한다. (권한별 메뉴 리스트)
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyMenuListByRoleData()
        {
            bool bRtnValue = true;

            //if (this.BCDResultList.Any(p => p.IsNew || p.IsDelete || p.IsUpdate) == true)
            //{
            //    this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
            //    bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            //}

            return bRtnValue;
        }
        #endregion

        #region >> TextEdit 숫자만 넣는 정규식
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

            if(e.Handled == true)
                this.BaseClass.MsgError(string.Format("{0}은 숫자만 입력할 수 있습니다.", 
                    ((TextEdit)sender).Name.Substring(3)), BaseEnumClass.CodeMessage.MESSAGE);
        }
        #endregion
       
        #region >> 값을 시뮬레이터로 보내는 함수
        private object SendData(CEnum2.EnumToCoreEventForSimulator eventName, params object[] args)
        {
            object data = new object();
            object[] send = new object[args.Count()];
            send[0] = ((TextEdit)args[0]).Text;

            for(int i = 1; i < args.Count(); i++)
            {
                if (((TextEdit)args[i]).Name.Contains("Box"))
                {
                    if (((TextEdit)args[i]).Text.Length < 60)
                    {
                        while (true)
                        {
                            ((TextEdit)args[i]).Text += " ";
                            if (((TextEdit)args[i]).Text.Length == 60)
                                break;
                        }
                        
                    }
                    else if (((TextEdit)args[i]).Text.Length > 60)
                        ((TextEdit)args[i]).Text = ((TextEdit)args[i]).Text.Substring(0, 60);
                }
                send[i] = ((TextEdit)args[i]).Text;
            }
            data = _ref_Conveyor.Execute(CEnum2.UIEventReceiver, (int)eventName, send);

            return data;
        }
        #endregion

        #region >> Conveyor_Simul과 통신 연결을 나타내는 함수
        private void _ref_Conveyor_OnStateChanged(CReference sender, enumReferenceState state)
        {
            if (state == LGCNS.ezControl.Common.enumReferenceState.Active && state == LGCNS.ezControl.Common.enumReferenceState.Active)
            {
                this.lblConnConveyor.Content = "Conveyor Connected";
                this.bdConnConveyor.Background = new SolidColorBrush(Colors.Green);
                this.BtnConnect.IsEnabled = false;
                this.BtnDisconnect.IsEnabled = true;
            }
            else if (state == LGCNS.ezControl.Common.enumReferenceState.Fault)
            {
                this.lblConnConveyor.Content = "Conveyor Disconnected";
                this.bdConnConveyor.Background = new SolidColorBrush(Colors.Red);
                this.BtnConnect.IsEnabled = true;
                this.BtnDisconnect.IsEnabled = false;
            }
            state_conveyor = state;
        }
        #endregion

        #region >> Conveyor_Simul에서 송신한 데이터를 받는 함수
        private void _ref_Conveyor_OnElementEvent(CReference reference, int iElementNo, string strElementPath, string strSubjectName, int iEventID, params object[] args)
        {

        }
        #endregion
        #endregion

        #region >> Get_Result_ITEM - 아이템 결과 리스트 조회
        //private async void Get_Result_ITEM(List<BCDResult> input)
        //{
        //    //System.Threading.Thread.Sleep(1000);
        //    #region + 파라메터 변수 선언 및 값 할당
        //    DataSet dsRtnValue                          = null;
        //    var strProcedureName                        = "PK_R0000_MIN.SP_RESULT_INQ";
        //    Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
        //    string[] arrOutputParam                     = { "O_RSLT_LIST" };
        //    #endregion

        //    #region + Input 파라메터
        //    var DATE                =               curTime;  //DATE
        //    #endregion
        //    dicInputParam.Add("P_DATE", DATE);
        //    #region + 데이터 조회
        //    using (BaseDataAccess dataAccess = new BaseDataAccess())
        //    {
        //        await System.Threading.Tasks.Task.Run(() =>
        //        {
        //            dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
        //        }).ConfigureAwait(true);
        //    }
        //    #endregion

        //    if (dsRtnValue == null) { return; }

        //    BCDResultList.Clear();

        //    for (int i = 0; i < dsRtnValue.Tables[0].Rows.Count; i++)
        //    {
        //        BCDResult data = new BCDResult();
        //        data.PID = dsRtnValue.Tables[0].Rows[i][0].ToString();
        //        data.INDT_YMD_HMS = dsRtnValue.Tables[0].Rows[i][1].ToString();
        //        data.PLAN_CD = dsRtnValue.Tables[0].Rows[i][2].ToString();
        //        data.BOX_CD = dsRtnValue.Tables[0].Rows[i][3].ToString();
        //        data.RGN_CD = dsRtnValue.Tables[0].Rows[i][4].ToString();
        //        data.BCD_INFO = dsRtnValue.Tables[0].Rows[i][5].ToString();
        //        data.INV_BCD = dsRtnValue.Tables[0].Rows[i][6].ToString();
        //        data.PLAN_CHUTE_ID1 = dsRtnValue.Tables[0].Rows[i][7].ToString();
        //        data.SRT_ERR_CD = dsRtnValue.Tables[0].Rows[i][8].ToString();
        //        data.SRT_RSN_CD = dsRtnValue.Tables[0].Rows[i][9].ToString();
        //        data.RSLT_CHUTE_ID = dsRtnValue.Tables[0].Rows[i][10].ToString();
        //        data.SRT_WRK_STAT_CD = dsRtnValue.Tables[0].Rows[i][11].ToString();
        //        data.INDUCTION_NO = dsRtnValue.Tables[0].Rows[i][12].ToString();
        //        data.CART_NO = dsRtnValue.Tables[0].Rows[i][13].ToString();
        //        data.CART_CNT = dsRtnValue.Tables[0].Rows[i][14].ToString();
        //        foreach(var res in input)
        //        {
        //            if(data.PID == res.PID)
        //            {
        //                data.IsSelected = true;
        //                break;
        //            }
        //        }
        //        BCDResultList.Add(data);
        //    }
        //    gridMaster.ItemsSource = BCDResultList;
        //}
        #endregion

        #endregion

        #region ▩ 이벤트
        #region > 버튼 클릭 이벤트
        #region >> 바코드 조회 버튼 클릭 이벤트
        /// <summary>
        /// 바코드 조회 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void BtnSearchBcr_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        // 상태바 (아이콘) 실행
        //        this.loadingScreen.IsSplashScreenShown = true;

        //        //// 사용자 관리 리스트 조회
        //        //await this.GetSP_ROLE_HDR_LIST_INQ();
        //    }
        //    catch (Exception err)
        //    {
        //        this.BaseClass.Error(err);
        //    }
        //    finally
        //    {
        //        // 상태바 (아이콘) 제거
        //        this.loadingScreen.IsSplashScreenShown = false;
        //    }
        //}
        #endregion

        #region >> Disconnect 버튼 클릭 이벤트
        /// <summary>
        /// Disconnect 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDisconnect_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.BaseClass.MsgQuestion("ASK_DISCONNECT");
            if (this.BaseClass.BUTTON_CONFIRM_YN == false)
                return;

            if (_ref_Conveyor.ReferenceState == enumReferenceState.Active)
            {
                _ref_Conveyor.Stop();
                _ref_Conveyor = null;
            }
        }
        #endregion

        #region >> Connect 버튼 클릭 이벤트
        /// <summary>
        /// Connect 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConnect_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //string test = this.BaseClass.EncryptAES256("Persist Security Info = False; Integrated Security = false; database = LGHealthy; server = localhost; User ID = sa; Password = 123");
            if (string.IsNullOrEmpty(this.txtConveyorNo.Text))
            {
                this.BaseClass.MsgError(string.Format("{0}를 입력해주세요.", this.txtConveyorNo.Name.Substring(3)), BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }

            if (_ref_Conveyor == null)
            {
                _ref_Conveyor = CReferenceManager.GetReference(GetFactovaConnection("LGHealthy"), "LGHealth", 5);
                _ref_Conveyor.OnStateChanged += _ref_Conveyor_OnStateChanged;
                _ref_Conveyor.OnElementEvent += _ref_Conveyor_OnElementEvent;
                _ref_Conveyor.Start();

                this.BaseClass.MsgInfo("연결되었습니다", BaseEnumClass.CodeMessage.MESSAGE);
            }
        }

        public ConnectionStringSettings GetFactovaConnection(string site)
        {
            ConnectionStringSettings con = new ConnectionStringSettings();
            con.ConnectionString = this.BaseClass.DecryptAES256(ConfigurationManager.ConnectionStrings[$"DB_CONNECTION_{site}"].ToString());
            con.ProviderName = "System.Data.SqlClient";
            return con;
        }
        #endregion

        #region >> Release Unload 버튼 클릭 이벤트
        private void BtnReleaseUnloadingSend_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (state_conveyor != enumReferenceState.Active)
            {
                this.BaseClass.MsgError("시뮬레이터와 연결해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }

            TextEdit[] arr = { this.txtParcelID, this.txtCellID1, this.txtCellID2, this.txtCellID3, this.txtCellID4 };
            for (int i = 0; i < arr.Count(); i++)
            {
                if (string.IsNullOrEmpty(arr[i].Text))
                {
                    this.BaseClass.MsgError(string.Format("{0}를 입력해주세요.", arr[i].Name.Substring(3)), BaseEnumClass.CodeMessage.MESSAGE);
                    arr[i].Focus();
                    return;
                }
            }

            SendData(CEnum2.EnumToCoreEventForSimulator.TestReleaseUnloadingSend, arr);
        }
        #endregion

        #region >> Release Load 버튼 클릭 이벤트
        private void BtnReleaseLoadingSend_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (state_conveyor != enumReferenceState.Active)
            {
                this.BaseClass.MsgError("시뮬레이터와 연결해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }

            TextEdit[] arr = { this.txtParcelID, this.txtCellID1, this.txtCellID2, this.txtCellID3, this.txtCellID4 };
            for (int i = 0; i < arr.Count(); i++)
            {
                if (string.IsNullOrEmpty(arr[i].Text))
                {
                    this.BaseClass.MsgError(string.Format("{0}를 입력해주세요.", arr[i].Name.Substring(3)), BaseEnumClass.CodeMessage.MESSAGE);
                    arr[i].Focus();
                    return;
                }
            }

            SendData(CEnum2.EnumToCoreEventForSimulator.TestReleaseUnloadingSend, arr);
        }
        #endregion

        #region >> Import Unload 버튼 클릭 이벤트
        private void BtnImportUnloadingSend_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (state_conveyor != enumReferenceState.Active)
            {
                this.BaseClass.MsgError("시뮬레이터와 연결해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }

            TextEdit[] arr = { this.txtParcelID, this.txtBoxID1, this.txtCellID1, this.txtBoxID2, this.txtCellID2, this.txtBoxID3, this.txtCellID3, this.txtBoxID4, this.txtCellID4 };
            for (int i = 0; i < arr.Count(); i++)
            {
                if (string.IsNullOrEmpty(arr[i].Text))
                {
                    this.BaseClass.MsgError(string.Format("{0}를 입력해주세요.", arr[i].Name.Substring(3)), BaseEnumClass.CodeMessage.MESSAGE);
                    arr[i].Focus();
                    return;
                }
            }

            SendData(CEnum2.EnumToCoreEventForSimulator.TestImportUnloadingSend, arr);
        }
        #endregion

        #region >> Import Load 버튼 클릭 이벤트
        private void BtnImportLoadingSend_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (state_conveyor != enumReferenceState.Active)
            {
                this.BaseClass.MsgError("시뮬레이터와 연결해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }

            TextEdit[] arr = { this.txtParcelID, this.txtBoxID1, this.txtBoxID2, this.txtBoxID3, this.txtBoxID4 };
            for (int i = 0; i < arr.Count(); i++)
            {
                if (string.IsNullOrEmpty(arr[i].Text))
                {
                    this.BaseClass.MsgError(string.Format("{0}를 입력해주세요.", arr[i].Name.Substring(3)), BaseEnumClass.CodeMessage.MESSAGE);
                    arr[i].Focus();
                    return;
                }
            }

            SendData(CEnum2.EnumToCoreEventForSimulator.TestImportLoadingSend, arr);
        }
        #endregion
        
        #region >> Reset 버튼 클릭 이벤트
        private void BtnReset_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.txtBoxID1.Clear();
            this.txtBoxID2.Clear();
            this.txtBoxID3.Clear();
            this.txtBoxID4.Clear();
            this.txtCellID1.Clear();
            this.txtCellID2.Clear();
            this.txtCellID3.Clear();
            this.txtCellID4.Clear();
            this.txtParcelID.Clear();
        }
        #endregion
        #endregion

        #region > 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// <summary>
        /// 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// </summary>
        private void NavigationBar_UserControlCallEvent()
        {
            this.TreeControlRefreshEvent();
        }
        #endregion
        
        #endregion
        
    }
}