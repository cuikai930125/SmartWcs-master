using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using SMART.WCS.UI.EMS.DataMembers.ECHK003;

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

namespace SMART.WCS.UI.EMS.Views.CHECK_MGMT
{
    /// <summary>
    /// E3003.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E3003 : UserControl
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
        /// 공통 Class를 이용하기 위한 BaseClass 선언
        /// </summary>
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        /// <summary>
        /// 좌측 하단 메인 상태 메세지
        /// </summary>
        private string g_strStatusMessage   = string.Empty;

        private bool g_isFirstLoad = true;
        #endregion

        #region ▩ 생성자
        public E3003()
        {
            InitializeComponent();
        }

        public E3003(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                this.InitEvent();

                this.BaseClass.BindingCommonComboBox(this.cboQRY_DAYS, "ALARM_DAYS", null, false);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        /// <summary>
        /// 조회 조건  변수
        /// </summary>
        public ObservableCollection<EmsCommCode> EmsAlarmQryDevList { get; private set; }


        /// <summary>
        /// 알람현황
        /// </summary>
        public static readonly DependencyProperty EmsAlarmStatusListProperty
                                  = DependencyProperty.Register("EmsAlarmStatusList",
                                      typeof(ObservableCollection<EmsAlarmStatus>),
                                             typeof(E3003), new PropertyMetadata(new ObservableCollection<EmsAlarmStatus>()));
        public ObservableCollection<EmsAlarmStatus> EmsAlarmStatusList
        {
            get { return (ObservableCollection<EmsAlarmStatus>)GetValue(EmsAlarmStatusListProperty); }
            private set { SetValue(EmsAlarmStatusListProperty, value); }
        }


        /// <summary>
        /// 일람 현황 디테일 
        /// 조회 시 디테일을 모두 조회하고, 그리드 더블 클릭 시 해당 부분을 필터링하여 데이타 뷰에 표시한다.
        /// </summary>
        DataTable dtAlarmSTatusDetail = null;
        public static readonly DependencyProperty EmsAlarmStatusDetailListProperty
                                  = DependencyProperty.Register("EmsAlarmStatusDetail",
                                      typeof(ObservableCollection<EmsAlarmStatusDetail>),
                                             typeof(E3003), new PropertyMetadata(new ObservableCollection<EmsAlarmStatusDetail>()));
        public ObservableCollection<EmsAlarmStatusDetail> EmsAlarmStatusDetailList
        {
            get { return (ObservableCollection<EmsAlarmStatusDetail>)GetValue(EmsAlarmStatusDetailListProperty); }
            private set { SetValue(EmsAlarmStatusDetailListProperty, value); }
        }
        #endregion

        #region ▩ 함수 
        #region > 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            try
            {
                this.gridMain.PreviewMouseLeftButtonUp += GridMain_PreviewMouseLeftButtonUp;
                this.Loaded += E3003_Loaded;
            }
            catch { throw; }
        }

        private void E3003_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.g_isFirstLoad == true)
                {
                    this.g_isFirstLoad = false;

                    this.loadingScreen.IsSplashScreenShown = true;

                    this.SearchEmsAlarmStatus();

                    this.SetResultText();
                }
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
        #endregion

        #region 이벤트 
        #region RowDoubleClick - 그리드 Row 더블 클릭 이벤트       
        private void gridMainView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            try
            {
                if (this.gridDetail.ItemsSource != null)
                {
                    this.gridDetail.ItemsSource = null;
                }

                // 선택한 ROW 
                TableView tableView = sender as TableView;
                TableViewHitInfo tableViewHitInfo = e.HitInfo as TableViewHitInfo;
                object row = tableView.Grid.GetRow(tableViewHitInfo.RowHandle);

                if (row != null)
                {

                    // 부품 ID
                    string strPartID = ((EmsAlarmStatus)row).PART_ID;

                    // 디테일 데이타 QUERY 
                    var results = from myRow in dtAlarmSTatusDetail.AsEnumerable()
                                  where myRow.Field<string>("PART_ID") == strPartID
                                  select myRow;

                    DataTable dtTmp = results.CopyToDataTable();

                    //디테일 그리드 바인딩 
                    EmsAlarmStatusDetailList = new ObservableCollection<EmsAlarmStatusDetail>();
                    EmsAlarmStatusDetailList.ToObservableCollection(dtTmp);
                    this.gridDetail.ItemsSource = EmsAlarmStatusDetailList;

                    this.btnExcelHigh.IsEnabled = true;
                }

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        

        #endregion

        private void cboQRY_DAYS_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            // 조회 조건이 변경되면 , 조회한 데이타를 CLEAR한다.
            EmsAlarmStatusList = null;
            gridDetail.ItemsSource = null;

        }
        #endregion

        #region >> SetResultText_Detail - Detail 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// Header 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        private void SetResultText()
        {
            this.g_strStatusMessage = string.Empty;

            var strResource = string.Empty;                                                           // 텍스트 리소스 (전체 데이터 수)                                                                 // 조회 데이터 수
            var strMasterGridTitle      = "[Master]";
            var strDetailGridTitle      = "[Detail]";
            int iMasterInqCount         = 0;
            int iDetailInqCount         = 0;

            if (this.EmsAlarmStatusList != null)
            {
                iMasterInqCount = this.EmsAlarmStatusList.Count();
            }

            if (this.EmsAlarmStatusDetailList != null)
            {
                iDetailInqCount = this.EmsAlarmStatusDetailList.Count();
            }

            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{strMasterGridTitle}:{iMasterInqCount.ToString()}, {strDetailGridTitle}:{iDetailInqCount.ToString()}{strResource}");
        }
        #endregion



        #region > 데이터 관련
        private void SearchEmsAlarmStatus()
        {
            // 조회 기간 

            var qryDays = this.BaseClass.ComboBoxSelectedKeyValue(this.cboQRY_DAYS);

            try
            {
                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD", this.BaseClass.CenterCD}
                    ,{"P_ALARM_DAYS", qryDays}
                };

                var strOutParam = new[] { "P_EMS_ALARM_LIST", "P_EMS_ALARM_DETAIL_LIST" };
                string callProc = "PK_EMS_ECHK003.SP_EMS_ALARM_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 1)
                    {
                        EmsAlarmStatusList = new ObservableCollection<EmsAlarmStatus>();
                        EmsAlarmStatusList.ToObservableCollection(outData.Tables[0]);

                        this.dtAlarmSTatusDetail = outData.Tables[1];
                    }
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 버튼 클릭 이벤트
        #region >> 조회 버튼 클릭 이벤트
        /// <summary>
        /// 조회 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.loadingScreen.IsSplashScreenShown = true;

                SearchEmsAlarmStatus();

                this.SetResultText();
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
        #endregion

        #region btnExcelDownload_PreviewMouseLeftButtonUp - 엑셀다운로드 버튼 클릭
        /// <summary>
        /// 엑셀다운로드 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Msg : DOWN_EXCEL - 엑셀 다운로드를 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EXCEL_DOWNLOAD");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                List<TableView> tableView = new List<TableView>();
                tableView.Add(this.gridMainView);
                tableView.Add(this.gridDetailView);
                this.BaseClass.GetExcelDownload(tableView);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region > 그리드 이벤트
        /// <summary>
        /// 그리드 클릭 이벤트 - 상세 데이터 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMain_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.loadingScreen.IsSplashScreenShown = true;

                var view = (sender as GridControl).View as TableView;
                var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hi.InRowCell)
                {
                    // 클릭한 행의 COM_HDR_CD 가져오기
                    int clicked         = hi.RowHandle;
                    var obj_cd          = this.gridMain.GetCellValue(clicked, gridMain.Columns["PART_ID"]);
                    string strPartID    = Convert.ToString(obj_cd);

                    try
                    {
                        var dtDetail                        = this.dtAlarmSTatusDetail.AsEnumerable().Where(p => p.Field<string>("PART_ID").Equals(strPartID)).CopyToDataTable();
                        this.EmsAlarmStatusDetailList       = new ObservableCollection<EmsAlarmStatusDetail>();
                        this.EmsAlarmStatusDetailList.ToObservableCollection(dtDetail);
                        this.gridDetail.ItemsSource         = this.EmsAlarmStatusDetailList;
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
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void gridDetailView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            // 화면 이동 

            try
            {
                // 선택한 ROW 
                TableView tableView                 = sender as TableView;
                TableViewHitInfo tableViewHitInfo   = e.HitInfo as TableViewHitInfo;
                object row                          = tableView.Grid.GetRow(tableViewHitInfo.RowHandle);

                if (row != null)
                {

                    // 부품
                    string strEqpId = ((EmsAlarmStatusDetail)row).EQP_ID;
                    string strEqpNm = ((EmsAlarmStatusDetail)row).EQP_NM;

                    using (E3002_01P frmEchkErrReg = new E3002_01P(strEqpId, strEqpNm))
                    {
                        frmEchkErrReg.ShowDialog();
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// <summary>
        /// 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// </summary>
        private void NavigationBar_UserControlCallEvent()
        {
            try
            {
                this.TreeControlRefreshEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion
    }
}
