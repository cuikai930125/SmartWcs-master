using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.DataMembers;
using SMART.WCS.UI.Processing.DataMembers.R1001_SRT;

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

namespace SMART.WCS.UI.Processing.Views.SORTER
{
    /// <summary>
    /// R1001_SRT.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class R1001_SRT : UserControl
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
        /// Base 클래서 선언
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
        /// 조회한 EQP_ID 저장
        /// </summary>
        private string SearchedEQPId = null;
        #endregion

        #region ▩ 생성자
        public R1001_SRT()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public R1001_SRT(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                //this.BaseInfo = ((SMART.WCS.Control.BaseApp)Application.Current).BASE_INFO;

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

                //// DateEdit 초기화
                //this.ToScanDT.EditValue = DateTime.Today.AddDays(1);
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 생성자 - 다른 화면에서 현 화면을 호출하는 경우
        /// </summary>
        /// <param name="_liMenuNavigation"></param>
        /// <param name="_objParam"></param>
        public R1001_SRT(List<string> _liMenuNavigation, MainWinParam _objParam)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                //this.BaseInfo = ((SMART.WCS.Control.BaseApp)Application.Current).BASE_INFO;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();

                // 파라메터로 넘겨받은 값으로 조건 컨트롤의 값을 설정한다.
                this.InitValue(_objParam);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> InitValue - 컨트롤 데이터 설정
        /// <summary>
        /// 컨트롤 데이터 설정
        /// </summary>
        /// <param name="_objParam"></param>
        private void InitValue(MainWinParam _objParam)
        {
            try
            {
                var iSrtComboIndex          = this.BaseClass.ComboBoxItemIndex(this.cboEqpId, _objParam.SRT);           // 소터
                var iSrtErrCDComboIndex     = this.BaseClass.ComboBoxItemIndex(this.cboSrtErrCd, _objParam.SRT_ERR_CD); // 소터 오류코드
                var iSrtRsnCDComboIndex     = this.BaseClass.ComboBoxItemIndex(this.cboSrtRsnCD, _objParam.SRT_RSN_CD); // 오류 사유코드

                // 소터
                if (iSrtComboIndex > -1) { this.cboEqpId.SelectedIndex = iSrtComboIndex; }
                // 소터 오류코드
                if (iSrtErrCDComboIndex > -1) { this.cboSrtErrCd.SelectedIndex = iSrtErrCDComboIndex; }
                // 소터 오류 사유코드
                if (iSrtRsnCDComboIndex > -1) { this.cboSrtRsnCD.SelectedIndex = iSrtRsnCDComboIndex; }

                this.FromScanDT.Text        = _objParam.FROM_DATE;
                this.txtFromScanHH.Text     = _objParam.FROM_TIME;
                this.ToScanDT.Text          = _objParam.TO_DATE;
                this.txtToScanHH.Text       = _objParam.TO_TIME;

                this.BtnSearch_First_PreviewMouseLeftButtonUp(null, null);
            }
            catch { throw; }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의

        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(R1001_SRT), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 그리드 - Reject 현황 조회
        public static readonly DependencyProperty RejectStatusMgmtListProperty
            = DependencyProperty.Register("RejectStatusMgmtList", typeof(ObservableCollection<RejectStatusMgmt>), typeof(R1001_SRT)
                , new PropertyMetadata(new ObservableCollection<RejectStatusMgmt>()));

        private ObservableCollection<RejectStatusMgmt> RejectStatusMgmtList
        {
            get { return (ObservableCollection<RejectStatusMgmt>)GetValue(RejectStatusMgmtListProperty); }
            set { SetValue(RejectStatusMgmtListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(R1001_SRT), new PropertyMetadata(string.Empty));

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

            // 콤보박스 - 조회 (설비 ID)
            this.BaseClass.BindingCommonComboBox(this.cboEqpId, "EQP_ID", commonParam_EQP_ID, false);
            this.BaseClass.BindingCommonComboBox(this.cboSrtErrCd, "SRT_ERR_CD", null, true);
            this.BaseClass.BindingCommonComboBox(this.cboSrtRsnCD, "SRT_RSN_CD", null, true);
            this.BaseClass.BindingCommonComboBox(this.cboZoneID, "ZONE_ID", null, true);                // ZONE ID

            // 날짜, 시간 컨트롤 기본값 설정
            this.BaseClass.AutoSetDateTimeByCenter(this.FromScanDT, this.txtFromScanHH, this.ToScanDT,  this.txtToScanHH);
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += R1002_SRT_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 조회
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_First_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            #endregion
        }
        #endregion
        #endregion

        #region > 기타 함수
        #region >> SetResultText - 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        private void SetResultText()
        {
            var strResource = string.Empty;                                                           // 텍스트 리소스 (전체 데이터 수)
            var iTotalRowCount = 0;                                                                   // 조회 데이터 수

            strResource = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                            // 텍스트 리소스
            iTotalRowCount = (this.gridMaster.ItemsSource as ICollection).Count;                      // 전체 데이터 수
            this.GridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                       // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion
        #endregion

        #region > 데이터 관련

        #region >> GetSP_RJT_LIST_INQ - Reject List 조회
        /// <summary>
        /// Reject List 조회
        /// </summary>
        private DataSet GetSP_RJT_LIST_INQ(string SearchedEQPId)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_R1001_SRT.SP_RJT_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RJT_LIST", "O_RSLT" };

            var strFromScanDt       = this.FromScanDT.DateTime.ToString("yyyyMMdd");                        // 스캔일자 (From)
            var strFromScanTm       = this.BaseClass.ReplaceText(this.txtFromScanHH, ":", string.Empty);    // 스캔시간 (From)
            var strToScanDt         = this.ToScanDT.DateTime.ToString("yyyyMMdd");                          // 스캔일자 (To)
            var strToScanTm         = this.BaseClass.ReplaceText(this.txtToScanHH, ":", string.Empty);      // 스캔시간 (To)

            var strEqpId        = SearchedEQPId;                                                // 설비 ID
            var strBoxInfo      = this.txtBoxInfo.Text.Trim();                                  // 바코드 정보
            var strFromScan     = strFromScanDt + strFromScanTm;                                // 스캔일자 (From)
            var strToScan       = strToScanDt + strToScanTm;                                    // 스캔일자 (To)
            var strCtrCd        = this.BaseClass.CenterCD;                                      // 센터코드
            var strInvBcd       = this.txtInvBcd.Text.Trim();                                   // 송장바코드
            var strSrtErrCd     = this.BaseClass.ComboBoxSelectedKeyValue(this.cboSrtErrCd);    // 소터오류코드
            var strSrtRsnCD     = this.BaseClass.ComboBoxSelectedKeyValue(this.cboSrtRsnCD);    // 소터오류 사유코드

            var strZoneID       = this.BaseClass.ComboBoxSelectedKeyValue(this.cboZoneID);      // ZONE ID
            
            var strErrCode = string.Empty;                                                  // 오류 코드
            var strErrMsg = string.Empty;                                                   // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCtrCd);          // 센터코드
            dicInputParam.Add("P_EQP_ID",               strEqpId);          // 설비 ID
            dicInputParam.Add("P_FM_DT",                strFromScan);       // 스캔일자 (From)
            dicInputParam.Add("P_TO_DT",                strToScan);         // 스캔일자 (To)
            dicInputParam.Add("P_BCD",                  strBoxInfo);        // 바코드 정보
            dicInputParam.Add("P_SRT_ERR_CD",           strSrtErrCd);       // 소터오류코드
            dicInputParam.Add("P_INV_BCD",              strInvBcd);         // 송장바코드
            dicInputParam.Add("P_ZONE_ID",              strZoneID);         // ZONE ID
            dicInputParam.Add("P_SRT_RSN_CD",           strSrtRsnCD);       // 오류 사유코드
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

        #endregion

        #endregion

        #region ▩ 이벤트

        #region > Loaded 이벤트
        private void R1002_SRT_Loaded(object sender, RoutedEventArgs e)
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

        #region > 스캔 현황

        #region >> 버튼 클릭 이벤트

        #region + 스캔 현황 조회버튼 클릭 이벤트
        /// <summary>
        /// 권역 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SearchedEQPId = null;
            SearchedEQPId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqpId);

            try
            {
                var iDiffDay    = this.BaseClass.CheckDateTerm(this.FromScanDT, this.ToScanDT, BaseEnumClass.SystemCode.PCS);
                if (iDiffDay < 0)
                {
                    // ERR_SEARCH_COND_TERM - 일자 조건을 {0}일 이내로 설정하셔야 합니다.
                    this.BaseClass.MsgError("ERR_COND_TERM", this.BaseClass.ProcInqTerm.ToString());
                    return;
                }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_RJT_LIST_INQ(SearchedEQPId);

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.RejectStatusMgmtList = new ObservableCollection<RejectStatusMgmt>();
                    // 오라클인 경우 TableName = TB_SRT_BOX_RSLT
                    this.RejectStatusMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.RejectStatusMgmtList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 소터오류코드 == "20" 인 경우 Row 색상 변경
                this.RejectStatusMgmtList.Where(p => p.SRT_RSN_CD == "20").ToList().ForEach(f => f.ForegroundBrush = new SolidColorBrush(Colors.Red));
                
                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.RejectStatusMgmtList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText();
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

        #region + 스캔 현황 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 슈트 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // ASK_EXCEL_DOWNLOAD - 엑셀 다운로드를 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EXCEL_DOWNLOAD");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                List<TableView> tv = new List<TableView>();
                tv.Add(this.tvMasterGrid);
                this.BaseClass.GetExcelDownload(tv);
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

        #endregion

        #region >> 그리드 관련 이벤트

        #region + 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        private static void View_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            if (g_IsAuthAllYN == false)
            {
                e.Cancel = true;
                return;
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