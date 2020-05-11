using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.Processing.DataMembers.R0000_SRT;
using System;
using System.Collections;
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
using SMART.WCS.UI.Processing.DataMembers.R0000_SRT.ECS.Common;
using System.Configuration;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Processing.Views.SORTER
{
    /// <summary>
    /// 권한관리 및 권한별 메뉴 리스트 관리
    /// </summary>
    public partial class R0000_SRT : UserControl
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
        CReference _ref_Sorter = null;
        CReference _ref_IPS = null;
        CReference _ref_AI = null;
        string curTime = string.Empty;
        Dictionary<string, string> BCD_PIDMap = new Dictionary<string, string>();
        int PID = 1;
        enumReferenceState state_ips;
        enumReferenceState state_sorter;
        enumReferenceState state_ai;
        #endregion

        #region ▩ 생성자 
        public R0000_SRT()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation"></param>
        public R0000_SRT(List<string> _liMenuNavigation)
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
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(R0000_SRT), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 바코드 샘플 관리
        public static readonly DependencyProperty BCDSampleListProperty
            = DependencyProperty.Register("BCDSampleList", typeof(ObservableCollection<BCDSample>), typeof(R0000_SRT)
                , new PropertyMetadata(new ObservableCollection<BCDSample>()));

        public ObservableCollection<BCDSample> BCDSampleList
        {
            get { return (ObservableCollection<BCDSample>)GetValue(BCDSampleListProperty); }
            set { SetValue(BCDSampleListProperty, value); }
        }
        #endregion

        #region > 결과 리스트
        public static readonly DependencyProperty BCDResultListProperty
            = DependencyProperty.Register("BCDResultList", typeof(ObservableCollection<BCDResult>), typeof(R0000_SRT)
                , new PropertyMetadata(new ObservableCollection<BCDResult>()));

        public ObservableCollection<BCDResult> BCDResultList
        {
            get { return (ObservableCollection<BCDResult>)GetValue(BCDResultListProperty); }
            set { SetValue(BCDResultListProperty, value); }
        }
        #endregion

        #region > Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(R0000_SRT), new PropertyMetadata(string.Empty));

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
            this.BtnInducted.ButtonWidthLength = 120;
            this.BtnDischarged.ButtonWidthLength = 120;
            this.BtnIPSScan.ButtonWidthLength = 120;
            this.BtnSortedConfirm.ButtonWidthLength = 120;
            this.BtnReset.ButtonWidthLength = 120;

            this.gridLeft_RoleList.ItemsSource = BCDSampleList;
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 화면 이벤트
            this.Loaded += R0000_SRT_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 연결 버튼 클릭 이벤트
            this.BtnConnect.PreviewMouseLeftButtonUp += BtnConnect_PreviewMouseLeftButtonUp;
            // 연결 해제 버튼 클릭 이벤트
            this.BtnDisconnect.PreviewMouseLeftButtonUp += BtnDisconnect_PreviewMouseLeftButtonUp;
            // 바코드 조회 버튼 클릭 이벤트
            //this.BtnSearchBcr.PreviewMouseLeftButtonUp += BtnSearchBcr_PreviewMouseLeftButtonUp;
            // Inducted 버튼 클릭 이벤트
            this.BtnInducted.PreviewMouseLeftButtonUp += BtnInducted_PreviewMouseLeftButtonUp;
            // Discharged 버튼 클릭 이벤트
            this.BtnDischarged.PreviewMouseLeftButtonUp += BtnDischarged_PreviewMouseLeftButtonUp;
            // IPS Scan 버튼 클릭 이벤트
            this.BtnIPSScan.PreviewMouseLeftButtonUp += BtnIPSScan_PreviewMouseLeftButtonUp;
            // Sorted Confirm 버튼 클릭 이벤트
            this.BtnSortedConfirm.PreviewMouseLeftButtonUp += BtnSortedConfirm_PreviewMouseLeftButtonUp;
            // Reset 버튼 클릭 이벤트
            this.BtnReset.PreviewMouseLeftButtonUp += BtnReset_PreviewMouseLeftButtonUp;
            // 그리드 Row추가 버튼 클릭 이벤트
            this.btnRowAdd.PreviewMouseLeftButtonUp += BtnRowAdd_PreviewMouseLeftButtonUp;
            // 그리드 추가Row 삭제 버튼 클릭 이벤트
            this.btnRowDelete.PreviewMouseLeftButtonUp += BtnRowDelete_PreviewMouseLeftButtonUp;
            // 임시용 AIScan 버튼 클릭 이벤트
            this.BtnAIScan.PreviewMouseLeftButtonUp += BtnAIScan_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 클릭 이벤트
            this.gridLeft_RoleList.PreviewMouseLeftButtonUp += GridLeft_RoleList_PreviewMouseLeftButtonUp;
            #endregion
        }
        
        private void R0000_SRT_Loaded(object sender, RoutedEventArgs e)
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

            if (this.BCDSampleList.Any(p => p.IsNew || p.IsDelete || p.IsUpdate) == true)
            {
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            }

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

            if (this.BCDResultList.Any(p => p.IsNew || p.IsDelete || p.IsUpdate) == true)
            {
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            }

            return bRtnValue;
        }
        #endregion

        #region >> TextEdit 숫자만 넣는 정규식
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #region >> CheckGridRowSelected - 그리드 Row 수정 여부 (IsSelected 값으로 판단)
        /// <summary>
        /// 그리드 Row 수정 여부 - (IsSelected 값으로 판단)
        /// </summary>
        /// <param name="_enumScreenGridKind">그리드 종류</param>
        /// <returns></returns>
        private bool CheckGridRowSelected()
        {
            var isRtnValue          = true;
            var strMessage          = string.Empty;
            var iCheckedHrdCount    = this.BCDSampleList.Where(p => p.IsSelected == true).Count();

            if (iCheckedHrdCount == 0)
            {
                // ERR_NO_SELECT - 선택된 데이터가 없습니다.
                this.BaseClass.MsgError("ERR_NO_SELECT");
                isRtnValue = false;
            }

            return isRtnValue;
        }
        #endregion

        #region >> CheckGridRowChangeData - 그리드 컬럼값 수정 여부 (IsSelected, IsUpdate 값으로 판단)
        /// <summary>
        /// 그리드 컬럼값 수정 여부 (IsSelected, IsUpdate 값으로 판단)
        /// </summary>
        /// <returns></returns>
        private bool CheckGridRowChangeData()
        {
            var isRtnValue          = true;
            var strMessage          = string.Empty;
            var iCheckedDtlCount    = this.BCDResultList.Where(p => p.IsSelected && p.IsUpdate).Count();

            if (iCheckedDtlCount == 0)
            {
                // ERR_NOT_CHANGE_DATA - 변경된 데이터가 없습니다.
                this.BaseClass.MsgError("ERR_NOT_CHANGE_DATA");
                isRtnValue = false;
            }

            return isRtnValue;
        }
        #endregion

        #region >> DeleteGridRowItem - 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// <summary>
        /// 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// </summary>
        private void DeleteGridRowItem()
        {
            this.BCDSampleList.Where(p => p.IsSelected == true && p.IsNew == true).ToList().ForEach(p =>
            {
                this.BCDSampleList.Remove(p);
            });
        }
        #endregion

        #region >> 중복되지 않는 난수 생성 함수
        public int[] GetRandomNoOverlap(int length, int min, int max)
        {
            int[] randArray = new int[length];
            bool isSame;
            Random r = new Random();
            for(int i = 0; i < length; i++)
            {
                while (true)
                {
                    randArray[i] = r.Next(min, max);
                    isSame = false;
                    for(int j = 0; j < i; j++)
                    {
                        if(randArray[i] == randArray[j])
                        {
                            isSame = true;
                            break;
                        }
                    }
                    if (!isSame) break;
                }
            }
            return randArray;
        }
        #endregion

        #region >> 중복되는 난수 생성 함수
        public int[] GetRandomOverlap(int length, int min, int max)
        {
            int[] randArray = new int[length];
            Random r = new Random();
            for (int i = 0; i < length; i++)
                    randArray[i] = r.Next(min, max);

            return randArray;
        }
        #endregion

        #region >> IPS_Simul에서 송신한 데이터를 받는 함수
        private void _ref_IPS_OnElementEvent(CReference reference, int iElementNo, string strElementPath, string strSubjectName, int iEventID, params object[] args)
        {
            Get_Result_ITEM(BCDResultList.Where(p => p.IsSelected == true).ToList());
        }
        #endregion

        #region >> Sorter_Simul에서 송신한 데이터를 받는 함수
        private void _ref_Sorter_OnElementEvent(CReference reference, int iElementNo, string strElementPath, string strSubjectName, int iEventID, params object[] args)
        {
            Get_Result_ITEM(BCDResultList.Where(p => p.IsSelected == true).ToList());
        }
        #endregion

        #region >> AI_Simul에서 송신한 데이터를 받는 함수
        private void _ref_AI_OnElementEvent(CReference reference, int iElementNo, string strElementPath, string strSubjectName, int iEventID, params object[] args)
        {
            Get_Result_ITEM(BCDResultList.Where(p => p.IsSelected == true).ToList());
        }
        #endregion

        #region >> IPS_Simul과 통신 연결 상태를 나타내는 함수
        private void _ref_IPS_OnStateChanged(CReference sender, enumReferenceState state)
        {
            if (state == LGCNS.ezControl.Common.enumReferenceState.Active && state == LGCNS.ezControl.Common.enumReferenceState.Active)
            {
                this.lblConnIPS.Content = "IPS Connected";
                this.bdConnIPS.Background = new SolidColorBrush(Colors.Green);
                this.BtnConnect.IsEnabled = false;
                this.BtnDisconnect.IsEnabled = true;
            }
            else if (state == LGCNS.ezControl.Common.enumReferenceState.Fault)
            {
                this.lblConnIPS.Content = "IPS Disconnected";
                this.bdConnIPS.Background = new SolidColorBrush(Colors.Red);
                this.BtnConnect.IsEnabled = true;
                this.BtnDisconnect.IsEnabled = false;
            }
            state_ips = state;
        }
        #endregion

        #region >> Sorter_Simul과 통신 연결 상태를 나타내는 함수
        private void _ref_Sorter_OnStateChanged(CReference sender, enumReferenceState state)
        {
            if (state == LGCNS.ezControl.Common.enumReferenceState.Active && state == LGCNS.ezControl.Common.enumReferenceState.Active)
            {
                this.lblConnSorter.Content = "Sorter Connected";
                this.bdConnSorter.Background = new SolidColorBrush(Colors.Green);
                this.BtnConnect.IsEnabled = false;
                this.BtnDisconnect.IsEnabled = true;

                this.BaseClass.MsgInfo("CMPT_CONNECT");
            }
            else if (state == LGCNS.ezControl.Common.enumReferenceState.Fault && state == LGCNS.ezControl.Common.enumReferenceState.Fault)
            {
                this.lblConnSorter.Content = "Sorter Disconnected";
                this.bdConnSorter.Background = new SolidColorBrush(Colors.Red);
                this.BtnConnect.IsEnabled = true;
                this.BtnDisconnect.IsEnabled = false;

                this.BaseClass.MsgInfo("CMPT_DISCONNECT");
            }
            state_sorter = state;
        }
        #endregion
        
        #region >> AI_Simul과 통신 연결 상태를 나타내는 함수
        private void _ref_AI_OnStateChanged(CReference sender, enumReferenceState state)
        {
            if (state == LGCNS.ezControl.Common.enumReferenceState.Active && state == LGCNS.ezControl.Common.enumReferenceState.Active)
            {
                this.lblConnAI.Content = "AI Connected";
                this.bdConnAI.Background = new SolidColorBrush(Colors.Green);
                this.BtnConnect.IsEnabled = false;
                this.BtnDisconnect.IsEnabled = true;
            }
            else if (state == LGCNS.ezControl.Common.enumReferenceState.Fault)
            {
                this.lblConnAI.Content = "AI Disconnected";
                this.bdConnAI.Background = new SolidColorBrush(Colors.Red);
                this.BtnConnect.IsEnabled = true;
                this.BtnDisconnect.IsEnabled = false;
            }
            state_ai = state;
        }
        #endregion

        #region >> 값을 시뮬레이터로 보내는 함수
        private object SendData(short check, CEnum2.EnumToCoreEventForSimulator eventName, params object[] args)
        {
            object data = new object();
            if (check == 1)
                data = _ref_Sorter.Execute(CEnum2.UIEventReceiver, (int)eventName, args);
            else if(check == 2)
                data = _ref_IPS.Execute(CEnum2.UIEventReceiver, (int)eventName, args);
            else if(check == 3)
                data = _ref_AI.Execute(CEnum2.UIEventReceiver, (int)eventName, args);

            return data;
        }
        #endregion

        #endregion
        
        #region >> Get_Result_ITEM - 아이템 결과 리스트 조회
        private async void Get_Result_ITEM(List<BCDResult> input)
        {
            //System.Threading.Thread.Sleep(1000);
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_R0000_SRT.SP_RESULT_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT_LIST" };
            #endregion

            #region + Input 파라메터
            var DATE                =               curTime;  //DATE
            #endregion
            dicInputParam.Add("P_DATE", DATE);
            #region + 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion
            
            if (dsRtnValue == null) { return; }

            BCDResultList.Clear();

            for (int i = 0; i < dsRtnValue.Tables[0].Rows.Count; i++)
            {
                BCDResult data = new BCDResult();
                data.PID = dsRtnValue.Tables[0].Rows[i][0].ToString();
                data.INDT_YMD_HMS = dsRtnValue.Tables[0].Rows[i][1].ToString();
                data.PLAN_CD = dsRtnValue.Tables[0].Rows[i][2].ToString();
                data.BOX_CD = dsRtnValue.Tables[0].Rows[i][3].ToString();
                data.RGN_CD = dsRtnValue.Tables[0].Rows[i][4].ToString();
                data.BCD_INFO = dsRtnValue.Tables[0].Rows[i][5].ToString();
                data.INV_BCD = dsRtnValue.Tables[0].Rows[i][6].ToString();
                data.PLAN_CHUTE_ID1 = dsRtnValue.Tables[0].Rows[i][7].ToString();
                data.SRT_ERR_CD = dsRtnValue.Tables[0].Rows[i][8].ToString();
                data.SRT_RSN_CD = dsRtnValue.Tables[0].Rows[i][9].ToString();
                data.RSLT_CHUTE_ID = dsRtnValue.Tables[0].Rows[i][10].ToString();
                data.SRT_WRK_STAT_CD = dsRtnValue.Tables[0].Rows[i][11].ToString();
                data.INDUCTION_NO = dsRtnValue.Tables[0].Rows[i][12].ToString();
                data.CART_NO = dsRtnValue.Tables[0].Rows[i][13].ToString();
                data.CART_CNT = dsRtnValue.Tables[0].Rows[i][14].ToString();
                foreach(var res in input)
                {
                    if(data.PID == res.PID)
                    {
                        data.IsSelected = true;
                        break;
                    }
                }
                BCDResultList.Add(data);
            }
            gridMaster.ItemsSource = BCDResultList;
        }
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

        #region >> SortedConfim 버튼 클릭 이벤트
        /// <summary>
        /// SortedConfim 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSortedConfirm_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (state_ips != LGCNS.ezControl.Common.enumReferenceState.Active || state_sorter != LGCNS.ezControl.Common.enumReferenceState.Active)
            {
                this.BaseClass.MsgInfo("시뮬레이터와 연결해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }

            List<BCDResult> SelectedBCDResult = BCDResultList.Where(p => p.IsSelected == true).ToList();
            for (int i = 0; i < SelectedBCDResult.Count(); i++)
            {
                List<short> data = new List<short> { short.Parse(SelectedBCDResult[i].CART_NO), short.Parse(SelectedBCDResult[i].CART_CNT),
                                                    short.Parse(SelectedBCDResult[i].PID), 0,  short.Parse(SelectedBCDResult[i].INDUCTION_NO), 1,
                                                    short.Parse(SelectedBCDResult[i].PLAN_CHUTE_ID1), 0, 0, 1 };
                SendData(1, CEnum2.EnumToCoreEventForSimulator.TestSortedConfirm, data);
            }
        }
        #endregion

        #region >> AI Scan 버튼 클릭 이벤트
        private void BtnAIScan_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            List<BCDResult> SelectedBCDResult = BCDResultList.Where(p => p.IsSelected == true).ToList();
            string[] type = { "P", "B" };
            Random r = new Random();

            if(SelectedBCDResult.Count() == 2 && SelectedBCDResult[0].CART_CNT == "1" && SelectedBCDResult[1].CART_CNT =="1")
            {
                SendData(3, CEnum2.EnumToCoreEventForSimulator.TestAIScan, SelectedBCDResult[0].PID, SelectedBCDResult[0].CART_NO, SelectedBCDResult[0].CART_CNT,
                                    SelectedBCDResult[1].PID, SelectedBCDResult[1].CART_NO, SelectedBCDResult[1].CART_CNT, type[r.Next(0, 2)], type[r.Next(0, 2)]);
            }
            else
            {
                for (int i = 0; i < SelectedBCDResult.Count(); i++)
                {
                    if (SelectedBCDResult[i].CART_CNT == "1")
                    {
                        int tmp = r.Next(1, 3);
                        switch (tmp)
                        {
                            case 1: //화물 1개, 빈카트 1개
                                SendData(3, CEnum2.EnumToCoreEventForSimulator.TestAIScan, SelectedBCDResult[i].PID, SelectedBCDResult[i].CART_NO, SelectedBCDResult[i].CART_CNT,
                                    "0", Convert.ToInt32(SelectedBCDResult[i].CART_NO + 1).ToString(), SelectedBCDResult[i].CART_CNT, type[r.Next(0, 2)], "?");
                                break;
                            case 2:
                                //빈카트 1개, 화물 1개
                                SendData(3, CEnum2.EnumToCoreEventForSimulator.TestAIScan, "0", (Convert.ToInt32(SelectedBCDResult[i].CART_NO) - 1).ToString(), SelectedBCDResult[i].CART_CNT,
                                    SelectedBCDResult[i].PID, SelectedBCDResult[i].CART_NO, SelectedBCDResult[i].CART_CNT, "?", type[r.Next(0, 2)]);
                                break;
                        }

                    }
                    if (SelectedBCDResult[i].CART_CNT == "2")
                    {
                        string temp = type[r.Next(0, 2)];
                        SendData(3, CEnum2.EnumToCoreEventForSimulator.TestAIScan, SelectedBCDResult[i].PID, SelectedBCDResult[i].CART_NO, SelectedBCDResult[i].CART_CNT,
                        SelectedBCDResult[i].PID, (Convert.ToInt32(SelectedBCDResult[i].CART_NO) + 1).ToString(), SelectedBCDResult[i].CART_CNT, temp, temp);
                    }
                }
            }
        }
        #endregion

        #region >> IPS Scan 버튼 클릭 이벤트
        /// <summary>
        /// IPS Scan 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnIPSScan_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (state_ips != LGCNS.ezControl.Common.enumReferenceState.Active || state_sorter != LGCNS.ezControl.Common.enumReferenceState.Active)
            {
                this.BaseClass.MsgInfo("시뮬레이터와 연결해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }
            List<BCDResult> SelectedBCDResult = BCDResultList.Where(p => p.IsSelected == true).ToList();
            
            for (int i = 0; i < SelectedBCDResult.Count(); i++)
                SendData(2, CEnum2.EnumToCoreEventForSimulator.TestIPSScan, SelectedBCDResult[i].CART_NO, SelectedBCDResult[i].CART_CNT,
                    SelectedBCDResult[i].PID, SelectedBCDResult[i].INDUCTION_NO, BCD_PIDMap[SelectedBCDResult[i].PID]);
           
            
        }
        #endregion

        #region >> Discharged 버튼 클릭 이벤트
        /// <summary>
        /// Discharged 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDischarged_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (state_ips != LGCNS.ezControl.Common.enumReferenceState.Active || state_sorter != LGCNS.ezControl.Common.enumReferenceState.Active)
            {
                this.BaseClass.MsgInfo("시뮬레이터와 연결해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }

            List<BCDResult> SelectedBCDResult = BCDResultList.Where(p => p.IsSelected == true).ToList();
            for (int i = 0; i < SelectedBCDResult.Count(); i++)
            {
                List<short> data = new List<short> { short.Parse(SelectedBCDResult[i].CART_NO), short.Parse(SelectedBCDResult[i].CART_CNT),
                                                    short.Parse(SelectedBCDResult[i].PID), 0,  short.Parse(SelectedBCDResult[i].INDUCTION_NO), 1,
                                                    short.Parse(SelectedBCDResult[i].PLAN_CHUTE_ID1), 0 };
                SendData(1, CEnum2.EnumToCoreEventForSimulator.TestDischarged, data);
            }
        }
        #endregion

        #region >> Inducted 버튼 클릭 이벤트
        /// <summary>
        /// Inducted 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInducted_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (state_ips != LGCNS.ezControl.Common.enumReferenceState.Active || state_sorter != LGCNS.ezControl.Common.enumReferenceState.Active)
            {
                this.BaseClass.MsgInfo("시뮬레이터와 연결해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }
            if (this.CheckGridRowSelected() == false) { return; }
            
            int num = 0;
            var SelectedBCD = BCDSampleList.Where(p => p.IsSelected == true).ToList();
            int length = SelectedBCD.Count();
            var CartNo = GetRandomNoOverlap(length, 1, 350);
            int[] ParcelID = new int[length];
            for (int i = 0; i < length; i++)
            {
                ParcelID[i] = PID++;
                if (PID >= 30000)
                    PID = 1;
            }
            var CartCount = GetRandomOverlap(length, 1, 3);
            var InductionNo = GetRandomOverlap(length, 1, 4);
            int Mode = 1;
            var Destination = GetRandomOverlap(length * 5, 1, 5);
            var Weight = GetRandomOverlap(length, 1, 5);

            foreach (var bcd in SelectedBCD)
            {
                List<short> data = new List<short>();
                data.Add((short)CartNo[num]);
                data.Add((short)CartCount[num]);
                data.Add((short)ParcelID[num]);
                data.Add(0);
                data.Add((short)InductionNo[num]);
                data.Add((short)Mode);
                for (int i = 5 * num; i < 5 * num + 5; i++)
                    data.Add((short)Destination[i]);
                data.Add((short)Weight[num]);
                data.Add(0);
                SendData(1, CEnum2.EnumToCoreEventForSimulator.TestInducted, data);
                BCD_PIDMap.Add(Convert.ToString(ParcelID[num]), bcd.BCD_SAMPLE);
                
                num++;
            }
        }
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

            if (_ref_Sorter.ReferenceState == enumReferenceState.Active)
            {
                _ref_Sorter.Stop();
                _ref_Sorter = null;
            }

            if (_ref_IPS.ReferenceState == enumReferenceState.Active)
            {
                _ref_IPS.Stop();
                _ref_IPS = null;
            }

            if(_ref_AI.ReferenceState == enumReferenceState.Active)
            {
                _ref_AI.Stop();
                _ref_AI = null;
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
            // ECS 프로그램과 연결
            if (String.IsNullOrEmpty(this.txtSorterNo.Text))
            {
                this.BaseClass.MsgError("ERR_NOT_INPUT", "SORTER_ELEMENT_NO");
                return;
            }

            if (String.IsNullOrEmpty(this.txtIPSNo.Text))
            {
                this.BaseClass.MsgError("ERR_NOT_INPUT", "IPS_ELEMENT_NO");
                return;
            }

            this.BaseClass.MsgQuestion("ASK_CONNECT");
            if (this.BaseClass.BUTTON_CONFIRM_YN == false)
                return;
            
            _ref_Sorter = CReferenceManager.GetReference(GetFactovaConnection("COUPANG_YS"), "SorterServer", Convert.ToInt32(this.txtSorterNo.Text));
            _ref_Sorter.OnStateChanged += _ref_Sorter_OnStateChanged;
            _ref_Sorter.OnElementEvent += _ref_Sorter_OnElementEvent;
            _ref_Sorter.Start();
            
            _ref_IPS = CReferenceManager.GetReference(GetFactovaConnection("COUPANG_YS"), "IPSServer", Convert.ToInt32(this.txtIPSNo.Text));
            _ref_IPS.OnStateChanged += _ref_IPS_OnStateChanged;
            _ref_IPS.OnElementEvent += _ref_IPS_OnElementEvent;
            _ref_IPS.Start();

            if(txtAINo.Text != string.Empty)
            {
                _ref_AI = CReferenceManager.GetReference(GetFactovaConnection("COUPANG_YS"), "AIServer", Convert.ToInt32(this.txtAINo.Text));
                _ref_AI.OnStateChanged += _ref_AI_OnStateChanged;
                _ref_AI.OnElementEvent += _ref_AI_OnElementEvent;
                _ref_AI.Start();
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

        #region >> Reset 버튼 클릭 이벤트
        /// <summary>
        /// Reset 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReset_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            curTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            BCDResultList.Clear();
            BCDSampleList.Clear();
        }
        #endregion

        #region >> 그리드 Row 추가 버튼 클릭 이벤트
        /// <summary>
        /// 그리드 Row 추가 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var newItem = new BCDSample
            {
                BCD_SAMPLE = string.Empty,
                IsSelected = true,
                IsNew = true
            };

            this.BCDSampleList.Add(newItem);
            this.gridLeft_RoleList.Focus();
            this.gridLeft_RoleList.CurrentColumn = this.gridLeft_RoleList.Columns.First();
            this.gridLeft_RoleList.View.FocusedRowHandle = this.BCDSampleList.Count - 1;

            this.BCDSampleList[this.BCDSampleList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
            this.BCDSampleList[this.BCDSampleList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);
            
            this.BaseClass.SetGridRowAddFocuse(this.gridLeft_RoleList, this.BCDSampleList.Count - 1);
        }
        #endregion

        #region >> 그리드 추가 Row 삭제 버튼 클릭 이벤트
        /// <summary>
        /// 그리드 추가 Row 삭제 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CheckGridRowSelected() == false) { return; }

            // 행 추가된 그리드의 Row 중 선택된 Row를 삭제한다.
            this.DeleteGridRowItem();

            //this.BaseClass.SetGridRowAddFocuse(this.gridLeft_RoleList, this.RoleMgntList.Count - 1);
        }
        #endregion
        #endregion

        #region > 그리드 관련 이벤트
        #region >> 그리드 클릭 이벤트
        private void GridLeft_RoleList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tvLeftGrid_RoleList.FocusedRowHandle = this.BaseClass.GetCurrentGridControlRowIndex(this.tvLeftGrid_RoleList);
            tvLeftGrid_RoleList.Grid.CurrentColumn = tvLeftGrid_RoleList.Grid.Columns["BCD_SAMPLE"];
            //tvLeftGrid_RoleList.ShowEditor();
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