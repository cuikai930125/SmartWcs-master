using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.Plan.DataMembers.P1004_SRT;

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
using System.Windows.Threading;

namespace SMART.WCS.UI.Plan.Views.JOB_PLAN
{
    /// <summary>
    /// P1004_SRT.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class P1004_SRT : UserControl
    {
        #region ▩ Detegate 선언
        #region > 메인화면 하단 좌측 상태바 값 반영
        public delegate void ToolStripStatusEventHandler(string value);
        public event ToolStripStatusEventHandler ToolStripChangeStatusLabelEvent;
        #endregion

        #region > 즐겨찾기 변경후 메인화면 트리 컨트롤 Refresh 및 포커스 이동
        public delegate void TreeControlRefreshEventHandler();
        public event TreeControlRefreshEventHandler TreeControlRefreshEvent;
        #endregion
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// Base 클래스 선언
        /// </summary>
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// Base Info 선언
        /// </summary>
        private BaseInfo BaseInfo = new BaseInfo();

        /// <summary>
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        /// <summary>
        /// Header 클릭에 따른 관련 정보 수집
        /// </summary>
        private List<string> headerSource = new List<string>();

        #endregion

        #region ▩ 생성자
        public P1004_SRT()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public P1004_SRT(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                this.BaseInfo = ((SMART.WCS.Control.BaseApp)Application.Current).BASE_INFO;

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 상단에 설명 
                this.CommentArea.ScreenID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();

                // 공통코드를 사용하지 않는 콤보박스를 설정한다.
                //this.InitComboBoxInfo();
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(P1004_SRT), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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
                view.ShowingEditor += View_ShowingEditor;
            }
        }
        #endregion

        #region > 그리드 - 슈트작업계획 헤더 리스트
        public static readonly DependencyProperty ChutePlanHeaderListProperty
            = DependencyProperty.Register("ChutePlanHeaderList", typeof(ObservableCollection<ChutePlanHeaderMgmt>), typeof(P1004_SRT)
                , new PropertyMetadata(new ObservableCollection<ChutePlanHeaderMgmt>()));

        public ObservableCollection<ChutePlanHeaderMgmt> ChutePlanHeaderList
        {
            get { return (ObservableCollection<ChutePlanHeaderMgmt>)GetValue(ChutePlanHeaderListProperty); }
            set { SetValue(ChutePlanHeaderListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty HeaderGridRowCountProperty
            = DependencyProperty.Register("HeaderGridRowCount", typeof(string), typeof(P1004_SRT), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string HeaderGridRowCount
        {
            get { return (string)GetValue(HeaderGridRowCountProperty); }
            set { SetValue(HeaderGridRowCountProperty, value); }
        }
        #endregion
        #endregion

        #region > 그리드 - 슈트작업계획 디테일 리스트
        public static readonly DependencyProperty ChutePlanDetailListProperty
            = DependencyProperty.Register("ChutePlanDetailList", typeof(ObservableCollection<ChutePlanDetailMgmt>), typeof(P1004_SRT)
                , new PropertyMetadata(new ObservableCollection<ChutePlanDetailMgmt>()));

        private ObservableCollection<ChutePlanDetailMgmt> ChutePlanDetailList
        {
            get { return (ObservableCollection<ChutePlanDetailMgmt>)GetValue(ChutePlanDetailListProperty); }
            set { SetValue(ChutePlanDetailListProperty, value); }
        }

        #region >> Detail Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty DetailGridRowCountProperty
            = DependencyProperty.Register("DetailGridRowCount", typeof(string), typeof(P1004_SRT), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string DetailGridRowCount
        {
            get { return (string)GetValue(DetailGridRowCountProperty); }
            set { SetValue(DetailGridRowCountProperty, value); }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            // 공통코드 조회 파라메터 string[]
            string[] commonParam_EQP_ID = { BaseClass.CenterCD, "SRT", BaseClass.UserID, string.Empty };

            // 콤보박스
            this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, true); //사용여부
            this.BaseClass.BindingCommonComboBox(this.CboEqpId, "EQP_ID", commonParam_EQP_ID, false);   //설비 ID
            this.BaseClass.BindingCommonComboBox(this.cboZoneID, "ZONE_ID", null, true);                // ZONE ID

            if (this.BaseClass.CenterCD.Equals("BC") || this.BaseClass.CenterCD.Equals("YS"))
            {
                this.btnApply.Visibility = Visibility.Hidden;
                this.btnNew.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += P1004_SRT_Loaded; //????
            #endregion

            #region + 버튼 클릭 이벤트
            // Header 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_Header_PreviewMouseLeftButtonUp;
            // Header 사용으로 적용
            this.btnApply.PreviewMouseLeftButtonUp += BtnApply_PreviewMouseLeftButtonUp;

            #endregion

            #region + 그리드 이벤트
            // Header 그리드 더블클릭 이벤트 - Detail 조회
            this.gridMasterHeader.PreviewMouseDoubleClick += DetailSearch_PreviewMouseDoublicClicked;
            // Header 그리드 클릭 이벤트
            this.gridMasterHeader.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;
            // Detail 그리드 클릭 이벤트
            this.gridMasterDetail.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;

            #endregion
        }
        #endregion
        #endregion

        #region > 기타 함수

        #region >> SetResultText_Header - Header 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// Header 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        private void SetResultText_Header()
        {
            var strResource = string.Empty;                                                           // 텍스트 리소스 (전체 데이터 수)
            var iTotalRowCount = 0;                                                                   // 조회 데이터 수
            var strGridTitle = "[Header] ";                                                           // Grid 종류 - Header

            strResource = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                            // 텍스트 리소스
            iTotalRowCount = (this.gridMasterHeader.ItemsSource as ICollection).Count;                // 전체 데이터 수
            this.HeaderGridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                 // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.

            this.ToolStripChangeStatusLabelEvent($"{strGridTitle}{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion

        #region >> SetResultText_Detail - Detail 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// Detail 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        private void SetResultText_Detail()
        {
            var strResource = string.Empty;                                                           // 텍스트 리소스 (전체 데이터 수)
            var iTotalRowCount = 0;                                                                   // 조회 데이터 수
            var strGridTitle = "[Detail] ";                                                           // Grid 종류 - Detail

            strResource = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                            // 텍스트 리소스
            iTotalRowCount = (this.gridMasterDetail.ItemsSource as ICollection).Count;                // 전체 데이터 수
            this.DetailGridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                 // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.

            this.ToolStripChangeStatusLabelEvent($"{strGridTitle}{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion

        #region >> CheckHeaderGridRowSelected - Header 그리드 체크박스 선택 유효성 체크
        /// <summary>
        /// Header 그리드 체크박스 선택 유효성 체크
        /// </summary>
        /// <returns></returns>
        private bool CheckHeaderGridRowSelected()
        {
            try
            {
                bool bRtnValue = true;
                int iCheckedCount = 0;

                iCheckedCount = this.ChutePlanHeaderList.Where(p => p.IsSelected == true).Count();

                if (iCheckedCount == 0) { bRtnValue = false; BaseClass.MsgError("ERR_NO_SELECT"); }  //선택된 데이터가 없습니다.
                if (iCheckedCount > 1) { bRtnValue = false; BaseClass.MsgError("ERR_NO_SELECT_1"); } //1개의 행만 선택할 수 있습니다.

                //if (bRtnValue == false) { BaseClass.MsgError("ERR_NO_SELECT"); }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion

        //#region >> CheckDetailGridRowSelected - Detail 그리드 체크박스 선택 유효성 체크
        ///// <summary>
        ///// Detail 그리드 체크박스 선택 유효성 체크
        ///// </summary>
        ///// <returns></returns>
        //private bool CheckDetailGridRowSelected()
        //{
        //    try
        //    {
        //        bool bRtnValue = true;
        //        int iCheckedCount = 0;

        //        iCheckedCount = this.ChutePlanDetailList.Where(p => p.IsSelected == true).Count();

        //        if (iCheckedCount == 0)
        //        {
        //            bRtnValue = false;
        //            BaseClass.MsgError("ERR_NO_SELECT");
        //        }

        //        return bRtnValue;
        //    }
        //    catch { throw; }
        //}
        //#endregion

        #region >> TabClosing - 탭을 닫을 때 데이터 저장 여부 체크
        /// <summary>
        /// 탭을 닫을 때 데이터 저장 여부 체크
        /// </summary>
        /// <returns></returns>
        public bool TabClosing()
        {
            return this.CheckModifyData();
        }
        #endregion

        #region >> CheckModifyData - 각 탭의 데이터 저장 여부를 체크한다.
        /// <summary>
        /// 각 탭의 데이터 저장 여부를 체크한다.
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyData()
        {
            bool bRtnValue = true;

            if (this.ChutePlanHeaderList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                bRtnValue = false;
            }

            if (this.ChutePlanDetailList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                bRtnValue = false;
            }

            if (bRtnValue == false)
            {
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            }

            return bRtnValue;
        }
        #endregion
        #endregion

        #region > 데이터 관련

        #region >> GetSP_CHUTE_PLAN_LIST_INQ - Chute Plan Header List조회
        /// <summary>
        /// Chute Plan Header 조회
        /// </summary>
        private DataSet GetSP_CHUTE_PLAN_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P1004_SRT.SP_CHUTE_PLAN_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_CHUTE_PLAN_LIST", "O_RSLT" };

            var strCenterCD     = this.BaseClass.CenterCD;
            var strEqpId        = this.BaseClass.ComboBoxSelectedKeyValue(this.CboEqpId);   // 설비 ID
            var strPlanNm       = this.txtPlanNm.Text.Trim();                               // Plan명 
            var strChuteId      = this.txtChuteId.Text.Trim();                              // 슈트 ID
            var strRgnCd        = this.txtRgnCd.Text.Trim();                                // 권역 코드
            var strZoneID       = this.BaseClass.ComboBoxSelectedKeyValue(this.cboZoneID);  // ZONE ID
            var strUseYn        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);   // 사용 여부

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",      strCenterCD);       // 센터코드
            dicInputParam.Add("P_EQP_ID",       strEqpId);          //설비 ID
            dicInputParam.Add("P_PLAN_NM",      strPlanNm);         //Plan명
            dicInputParam.Add("P_RGN_CD",       strRgnCd);          //권역 코드
            dicInputParam.Add("P_CHUTE_ID",     strChuteId);        //슈트 ID
            dicInputParam.Add("P_USE_YN",       strUseYn);          // 사용 여부
            dicInputParam.Add("P_ZONE_ID",      strZoneID);         // ZONE ID
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion

        #region >> GetSP_CHUTE_PLAN_DTL_LIST_INQ - Chute Plan Detail List 조회 SP (Header 더블 클릭 시)
        /// <summary>
        /// Chute Plan Detail List 조회 SP
        /// </summary>
        /// <param name="_da"></param>
        /// <param name="_item"></param>
        /// <returns></returns>
        private DataSet GetSP_CHUTE_PLAN_DTL_LIST_INQ(string PlanCd)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P1004_SRT.SP_CHUTE_PLAN_DTL_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_CHUTE_PLAN_DTL_LIST", "O_RSLT" };

            var strCenterCD = this.BaseClass.CenterCD;
            var strEqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.CboEqpId);  //설비 ID
            var strPlanCd = PlanCd;
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCenterCD);       // 센터코드
            dicInputParam.Add("P_EQP_ID", strEqpId);             //설비 ID
            dicInputParam.Add("P_PLAN_CD", strPlanCd);          //Plan코드
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion

        #region >> ApplySP_CHUTE_PLAN_APPLY - 적용 시 Header의 사용 여부를 사용으로 변환('N'->'Y')
        /// <summary>
        /// Common Code Header 수정 SP
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool ApplySP_CHUTE_PLAN_APPLY(BaseDataAccess _da, ChutePlanHeaderMgmt _item,string PlanCd)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_P1004_SRT.SP_CHUTE_PLAN_APPLY";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strCenterCD = this.BaseClass.CenterCD;
            var strEqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.CboEqpId);  //설비 ID
            var strPlanCd = PlanCd;
            var strUserID = this.BaseClass.UserID;                          // 사용자 ID       

            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCenterCD);        // 센터코드
            dicInputParam.Add("P_EQP_ID", strEqpId);            //설비 ID
            dicInputParam.Add("P_PLAN_CD", strPlanCd);          //Plan코드
            dicInputParam.Add("P_USER_ID", strUserID);          // 사용자 ID

            #endregion

            dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        BaseClass.MsgInfo(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }
        #endregion

        #endregion
        #endregion

        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void P1004_SRT_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 슈트 작업 계획 관리

        #region >> 버튼 클릭 이벤트

        #region + 슈트 작업 계획 Header 리스트 조회 클릭 이벤트
        /// <summary>
        /// 슈트 작업 계획 Header 리스트 조회 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_Header_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 슈트 작업 계획 Header 리스트 조회
                DataSet dsRtnValue = this.GetSP_CHUTE_PLAN_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.ChutePlanHeaderList = new ObservableCollection<ChutePlanHeaderMgmt>();
                    // 오라클인 경우 TableName = TB_
                    this.ChutePlanHeaderList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ChutePlanHeaderList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩 - 헤더
                this.gridMasterHeader.ItemsSource = this.ChutePlanHeaderList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText_Header();


                //////슈트 작업 계획 헤더의 첫번째 행에 관한 디테일 조회
                // 슈트 작업 계획 Detail 리스트 조회
                DataSet dsRtnValue_Detail = this.GetSP_CHUTE_PLAN_DTL_LIST_INQ(this.ChutePlanHeaderList[0].PLAN_CD);

                if (dsRtnValue_Detail == null) { return; }

                var strErrCode_Detail = string.Empty;
                var strErrMsg_Detail = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue_Detail, ref strErrCode_Detail, ref strErrMsg_Detail) == true)
                {
                    // 정상 처리된 경우
                    this.ChutePlanDetailList = new ObservableCollection<ChutePlanDetailMgmt>();
                    // 오라클인 경우 TableName = TB_
                    this.ChutePlanDetailList.ToObservableCollection(dsRtnValue_Detail.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ChutePlanDetailList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩 - 디테일
                this.gridMasterDetail.ItemsSource = this.ChutePlanDetailList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                //this.SetResultText_Detail();

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }

        }
        #endregion

        #region + Header 사용으로 변환('N'->'Y')
        /// <summary>
        /// 적용 버튼 클릭 이벤트 - Header 사용으로 변환('N'->'Y')
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnApply_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckHeaderGridRowSelected() == false)
                {
                    return;
                }

                // ASK_APPLY - 적용하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_APPLY");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;

                this.ChutePlanHeaderList.ForEach(p => p.ClearError());

                //var strMessage = "{0} 이(가) 입력되지 않았습니다.";

                var liSelectedHeaderRowData = this.ChutePlanHeaderList.Where(p => p.IsSelected == true).ToList();


                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        da.BeginTransaction();

                        foreach (var item in liSelectedHeaderRowData)
                        {
                            if (item.USE_YN.Equals("Y"))
                            {
                                BaseClass.MsgError("ERR_ISAPPLIED"); //메시지 : 이미 적용중인 PLAN입니다.
                                break;
                            }
                            isRtnValue = this.ApplySP_CHUTE_PLAN_APPLY(da, item, item.PLAN_CD);

                            if (isRtnValue == false)
                            {
                                break;
                            }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우
                            da.CommitTransaction();

                            BaseClass.MsgInfo("CMPT_APPLIED");

                            foreach (var item in liSelectedHeaderRowData)
                            {
                                item.IsApplied = true;
                            }
                        }
                        else
                        {
                            // 오류 발생하여 저장 실패한 경우
                            da.RollbackTransaction();
                        }
                    }
                    catch
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        BaseClass.MsgError("ERR_SAVE");
                        throw;
                    }
                    finally
                    {
                        // 상태바 (아이콘) 제거
                        this.loadingScreen.IsSplashScreenShown = false;

                        // 체크박스 해제
                        foreach (var item in liSelectedHeaderRowData)
                        {
                            item.IsSelected = false;
                        }

                        // 저장 후 저장내용 List에 출력 : Header
                        DataSet dsRtnValue = this.GetSP_CHUTE_PLAN_LIST_INQ();

                        this.ChutePlanHeaderList = new ObservableCollection<ChutePlanHeaderMgmt>();
                        this.ChutePlanHeaderList.ToObservableCollection(dsRtnValue.Tables[0]);

                        this.gridMasterHeader.ItemsSource = this.ChutePlanHeaderList;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }

        }
        #endregion



        #endregion

        #region >> 그리드 관련 이벤트

        #region + 그리드 클릭 이벤트
        /// <summary>
        /// 그리드 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMaster_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var view = (sender as GridControl).View as TableView;
            var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

            if (hi.InRowCell)
            {
                if (hi.Column.FieldName.Equals("USE_YN") == false) { return; }

                if (view.ActiveEditor == null)
                {
                    view.ShowEditor();

                    if (view.ActiveEditor == null) { return; }
                    Dispatcher.BeginInvoke(new Action(() => {
                        view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                    }), DispatcherPriority.Render);
                }
            }
        }
        #endregion

        #region + Header List클릭 이벤트 (Detail 리스트 조회)
        /// <summary>
        /// Header List클릭 이벤트 (Detail 리스트 조회)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailSearch_PreviewMouseDoublicClicked(object sender, MouseButtonEventArgs e)
        {
            var view = (sender as GridControl).View as TableView;
            var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

            if (hi.InRowCell)
            {
                // 클릭한 행의 PlanCd 가져오기
                int clicked = hi.RowHandle;
                var obj_cd = gridMasterHeader.GetCellValue(clicked, gridMasterHeader.Columns[1]);
                string PlanCd = Convert.ToString(obj_cd);

                headerSource.Clear();
                headerSource.Add(PlanCd);

                try
                {
                    // 상태바 (아이콘) 실행
                    this.loadingScreen.IsSplashScreenShown = true;

                    // Detail 목록 조회
                    DataSet dsRtnValue = this.GetSP_CHUTE_PLAN_DTL_LIST_INQ(PlanCd);

                    if (dsRtnValue == null) { return; }

                    var strErrCode = string.Empty;      //오류 코드
                    var strErrMsg = string.Empty;      // 오류 메세지

                    if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                    {
                        // 정상 처리된 경우
                        this.ChutePlanDetailList = new ObservableCollection<ChutePlanDetailMgmt>();
                        // 오라클인 경우 TableName = TB_SRT_CHUTE_PLAN_DTL
                        this.ChutePlanDetailList.ToObservableCollection(dsRtnValue.Tables[0]);
                    }
                    else
                    {
                        // 오류가 발생한 경우
                        this.ChutePlanDetailList.ToObservableCollection(null);
                        BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                    }

                    // 조회 데이터를 그리드에 바인딩
                    this.gridMasterDetail.ItemsSource = this.ChutePlanDetailList;


                    // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                    this.SetResultText_Detail();
                }
                catch (Exception err)
                {
                    this.BaseClass.Error(err);
                }
                finally
                {
                    this.loadingScreen.IsSplashScreenShown = false;
                }
            }
        }

        #endregion

        #region + 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        private static void View_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            if (g_IsAuthAllYN == false)
            {
                e.Cancel = true;
                return;
            }

            TableView tv = sender as TableView;

            if (tv.Name.Equals("tvMasterGrid") == true)
            {
                ChutePlanHeaderMgmt dataMember = tv.Grid.CurrentItem as ChutePlanHeaderMgmt;

                if (dataMember == null) { return; }
            }

            if (tv.Name.Equals("tvDetailGrid") == true)
            {
                ChutePlanDetailMgmt dataMember = tv.Grid.CurrentItem as ChutePlanDetailMgmt;

                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    // 상세 코드 컬럼은 수정이 되지 않도록 처리한다.
                    case "COM_DTL_CD":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

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
