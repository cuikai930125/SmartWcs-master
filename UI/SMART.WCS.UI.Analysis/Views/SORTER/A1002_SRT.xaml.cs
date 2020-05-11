using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.Data.DataMembers;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.DataMembers;
using SMART.WCS.UI.Analysis.DataMembers.A1002_SRT;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SMART.WCS.UI.Analysis.Views.SORTER
{
    /// <summary>
    /// A1002_SRT.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class A1002_SRT : UserControl
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

        #region > 화면에서 메뉴 여는 기능
        public delegate void SelectedMenuOpenEventHandler(MainWinParam _liValue);
        public event SelectedMenuOpenEventHandler SelectedMenuOpenEvent;
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
        /// 조회한 SRT_ID 저장
        /// </summary>
        private string SearchedSRTId = null;

        /// <summary>
        /// 3D 그래프 객체
        /// </summary>
        PieSeries3D g_Is3D;
        #endregion

        #region ▩ 생성자
        public A1002_SRT()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public A1002_SRT(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

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
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(A1002_SRT), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 그리드 - Reject 사유별 현황
        public static readonly DependencyProperty RejectReasonMgmtListProperty
            = DependencyProperty.Register("RejectReasonMgmtList", typeof(ObservableCollection<RejectReasonMgmt>), typeof(A1002_SRT)
                , new PropertyMetadata(new ObservableCollection<RejectReasonMgmt>()));

        private ObservableCollection<RejectReasonMgmt> RejectReasonMgmtList
        {
            get { return (ObservableCollection<RejectReasonMgmt>)GetValue(RejectReasonMgmtListProperty); }
            set { SetValue(RejectReasonMgmtListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(A1002_SRT), new PropertyMetadata(string.Empty));

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

            // 콤보박스 - 조회 (소터)
            this.BaseClass.BindingCommonComboBox(this.CboSrt, "EQP_ID", commonParam_EQP_ID, false);

            // 날짜, 시간 컨트롤 기본값 설정
            this.BaseClass.AutoSetDateTimeByCenter(this.FromDT, this.FromTM, this.ToDT, this.ToTM);
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += A1002_SRT_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 조회
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_First_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 더블클릭 이벤트
            this.gridMasterTable.PreviewMouseDoubleClick += GridMasterTable_PreviewMouseDoubleClick;
            #endregion
        }
        #endregion
        #endregion

        #region > 차트 바인딩
        private void BindingChartRejectRslt(DataTable _dtChartData)
        {
            try
            {
                if (this.pieDiagramRsltMain.Series.Count > 0) { this.pieDiagramRsltMain.Series.Clear(); }
                this.chartErrPrctPie.BeginInit();

                // 차트 색 지정 (색상 수 : 7개)
                CustomPalette customPalette = new CustomPalette();
                customPalette.Colors.Add(Colors.LightBlue);
                customPalette.Colors.Add(Colors.IndianRed);
                customPalette.Colors.Add(Colors.SteelBlue);
                customPalette.Colors.Add(Colors.Orange);
                customPalette.Colors.Add(Colors.OliveDrab);
                customPalette.Colors.Add(Colors.PapayaWhip);
                customPalette.Colors.Add(Colors.LightCoral);

                chartErrPrctPie.Palette = customPalette;

                this.g_Is3D = new PieSeries3D();
                this.g_Is3D.ArgumentScaleType = ScaleType.Auto;
                this.g_Is3D.DataSourceSorted = false;
                this.g_Is3D.ArgumentDataMember = "Argument_str";
                this.g_Is3D.ValueDataMember = "Value";

                List<ChartDataPointMember> pies = this.GetGenerateDataCell(_dtChartData);
                this.g_Is3D.DataSource = pies;
  
                this.g_Is3D.LegendTextPattern = "{S} : {A:0}";
                
                if (this.pieDiagramRsltMain.Series.Count > 0) { this.pieDiagramRsltMain.Series.Clear(); }
                this.pieDiagramRsltMain.Series.Add(this.g_Is3D);

                // 파이차트 마우스 회전 X
                this.pieDiagramRsltMain.RuntimeRotation = false;
                
                // 파이차트 Label
                this.g_Is3D.LabelsVisibility = true;
                this.g_Is3D.Label = new SeriesLabel();
                this.g_Is3D.Label.TextPattern = "{A}: {VP:P0}";
                PieSeries3D.SetLabelPosition(this.g_Is3D.Label, PieLabelPosition.TwoColumns);
            }
            catch { throw; }
            finally
            {
                this.chartErrPrctPie.EndInit();
                this.g_Is3D = null;
            }
        }
        

        #region >> GetGenerateDataCell - 수량 결과 차트 데이터 구성 함수
        /// <summary>
        /// 수량 결과 차트 데이터 구성 함수
        /// </summary>
        /// <param name="_dtValue">차트 데이터</param>
        /// <returns></returns>
        private List<ChartDataPointMember> GetGenerateDataCell(DataTable _dtValue)
        {
            List<ChartDataPointMember> pies = new List<ChartDataPointMember>();

            foreach (DataRow drRow in _dtValue.Rows)
            {
                string dLegend = Convert.ToString(drRow["REJECT_TYPE"]);
                double dValue = Convert.ToDouble(drRow["REJECT_QTY"]);
                string dArgument = Convert.ToString(drRow["REJECT_TYPE"]);
                pies.Add(new ChartDataPointMember(dLegend, dArgument, dValue));
            }

            return pies;
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
            iTotalRowCount = (this.gridMasterTable.ItemsSource as ICollection).Count;                      // 전체 데이터 수
            this.GridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                       // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
        }
        #endregion
        #endregion

        #region > 데이터 관련

        #region >> GetSP_RJT_RSN_LIST_INQ - 스캔 현황 조회
        /// <summary>
        /// 스캔 현황 조회
        /// </summary>
        private DataSet GetSP_RJT_RSN_LIST_INQ(string SearchedSRTId)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_A1002_SRT.SP_RJT_RSN_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSN_LIST", "O_RSLT" };

            var strCntrCd = this.BaseClass.CenterCD;                            // 센터코드
            var strEqpId = SearchedSRTId;                                       // 소터 ID

            var strFromDt = this.FromDT.DateTime.ToString("yyyyMMdd");                              // 기간 - From Date
            var strFromTm = this.BaseClass.ReplaceText(this.FromTM, ":", string.Empty);             // 기간 - From Tm
            var strToDt = this.ToDT.DateTime.ToString("yyyyMMdd");                                  // 기간 - To Date
            var strToTm = this.BaseClass.ReplaceText(this.ToTM, ":", string.Empty);                 // 기간 - To Tm

            strFromDt += strFromTm;
            strToDt += strToTm;

            var strErrCode = string.Empty;                                      // 오류 코드
            var strErrMsg = string.Empty;                                       // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                     // 센터코드
            dicInputParam.Add("P_EQP_ID", strEqpId);                       // 소터 ID
            dicInputParam.Add("P_FM_DT", strFromDt);                       // 스캔일자 (From)
            dicInputParam.Add("P_TO_DT", strToDt);                         // 스캔일자 (To)
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
        private void A1002_SRT_Loaded(object sender, RoutedEventArgs e)
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
            SearchedSRTId = null;
            SearchedSRTId = this.BaseClass.ComboBoxSelectedKeyValue(this.CboSrt);

            try
            {
                var iDiffDay = this.BaseClass.CheckDateTerm(this.FromDT, this.ToDT, BaseEnumClass.SystemCode.ANL);
                if (iDiffDay < 0)
                {
                    // ERR_SEARCH_COND_TERM - 일자 조건을 {0}일 이내로 설정하셔야 합니다.
                    this.BaseClass.MsgError("ERR_COND_TERM", this.BaseClass.AnlInqTerm.ToString());
                    return;
                }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_RJT_RSN_LIST_INQ(SearchedSRTId);

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.RejectReasonMgmtList = new ObservableCollection<RejectReasonMgmt>();
                    
                    // 오라클인 경우 TableName = TB_SRT_BOX_HM_SUM
                    this.RejectReasonMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);

                    // 차트 바인딩
                    this.BindingChartRejectRslt(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.RejectReasonMgmtList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }
                
                // 조회 데이터를 Table 그리드에 바인딩한다.
                this.gridMasterTable.ItemsSource = this.RejectReasonMgmtList;

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

        #region + 슈트 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 스캔 현황 엑셀 다운로드 버튼 클릭 이벤트
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
                tv.Add(this.tvMasterGridTable);
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
        #region + 그리드 더블클릭 이벤트
        private void GridMasterTable_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view = (sender as GridControl).View as TableView;
                var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hi.Column.FieldName.Equals("REJECT_TYPE") == false) { return; }

                if (hi.InRowCell)
                {
                    // 화면 호출을 위한 파라메터를 매개변수에 저장한다.
                    MainWinParam objParam   = new MainWinParam();
                    objParam.MENU_ID        = "R1001_SRT";                                              // 메뉴 ID
                    objParam.SRT            = this.BaseClass.ComboBoxSelectedKeyValue(this.CboSrt);     // 소터
                    objParam.FROM_DATE      = this.FromDT.DateTime.ToString("yyyy-MM-dd");              // 시작일자
                    objParam.FROM_TIME      = this.FromTM.Text.Trim();                                  // 시작시간
                    objParam.TO_DATE        = this.ToDT.DateTime.ToString("yyyy-MM-dd");                // 종료일자
                    objParam.TO_TIME        = this.ToTM.Text.Trim();                                    // 종료시간
                    objParam.SRT_ERR_CD     = ((RejectReasonMgmt)this.gridMasterTable.SelectedItem).SRT_ERR_CD;
                    objParam.SRT_RSN_CD     = ((RejectReasonMgmt)this.gridMasterTable.SelectedItem).SRT_RSN_CD;

                    // Reject 유형 더블클릭 시 Reject 현황조회 화면 오픈
                    this.SelectedMenuOpenEvent(objParam);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
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
