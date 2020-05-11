using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.COMMON.DataMembers.C1009;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMART.WCS.UI.COMMON.Views.SYS_MGMT
{
    /// <summary>
    /// C1009.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1009 : UserControl
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

        #endregion

        #region ▩ 생성자
        public C1009()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1009(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                //// 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                //// 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 상단에 설명 
                this.CommentArea.ScreenID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();

                //  공통코드를 사용하지 않는 콤보박스 설정
                this.InitComboBoxInfo();

                // DateEdit 초기화
                this.ToYmd.EditValue = DateTime.Today.AddDays(1);

            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1009), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > ECS Log 
        #region >> 배치 차수 조회
        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public static readonly DependencyProperty EcsLogProperty
            = DependencyProperty.Register("EcsLogMgmtList", typeof(ObservableCollection<EcsLogMgmt>), typeof(C1009)
                , new PropertyMetadata(new ObservableCollection<EcsLogMgmt>()));

        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public ObservableCollection<EcsLogMgmt> EcsLogMgmtList
        {
            get { return (ObservableCollection<EcsLogMgmt>)GetValue(EcsLogProperty); }
            set { SetValue(EcsLogProperty, value); }
        }
        #endregion

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1009), new PropertyMetadata(string.Empty));

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
            string[] commonParam_EQPCode = { BaseClass.CenterCD, string.Empty, BaseClass.UserID, string.Empty };

            // 콤보박스
            this.BaseClass.BindingCommonComboBox(this.CboEqpId, "EQP_ID", commonParam_EQPCode, false);  // EQP_ID ComboBox 맵핑
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + ECS 로그 조회
            #region ++ 버튼 클릭 이벤트
            // 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            #endregion

            #region + 콤보박스 클릭 이벤트
            // EQP_ID 구분 ComboBox 클릭 -> EQP_ID에 따라 PRC_ID 맵핑 (EQP_ID의 ATTR01(C or W) = 공통코드의 PRC_ID의 ATTR01(C or W))
            this.CboEqpId.EditValueChanged += CboEqpId_EditValueChanged;
            #endregion
        }
        #endregion

        #region >> InitComboBoxInfo - 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// <summary>
        /// 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// </summary>
        private void InitComboBoxInfo()
        {
            var strEqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.CboEqpId);              
            string[] commonParam_EQP_ID = { strEqpId, string.Empty, string.Empty, string.Empty };
            this.BaseClass.BindingCommonComboBox(this.CboPrcId, "PRC_ID", commonParam_EQP_ID, false);
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
            this.GridRowCount = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";                // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion

        #endregion        
        #endregion

        #region > 데이터 관련
        #region >> GetSP_LOG_INFO_LIST_INQ - ECS LOG 조회
        /// <summary>
        /// ECS LOG 데이터 조회
        /// </summary>
        private DataSet GetSP_LOG_INFO_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1009.SP_LOG_INFO_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LOG_INFO_LIST", "O_RSLT" };

            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            var strEqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.CboEqpId);                  // 설비 ID
            var strPrcId = this.BaseClass.ComboBoxSelectedKeyValue(this.CboPrcId);                  // 프로세스 ID
            var strPid = this.txtPid.Text.Trim();                                                   // PID
            var strBcrInfo = this.txtBcrInfo.Text.Trim();                                           // 바코드INFO

            var strFrYmd = this.FrYmd.DateTime.ToString("yyyyMMdd");                                // 기간 - From Date
            var strFrHm = this.FrHm.DateTime.ToString("HH00");                                      // 기간 - From Time
            var strToYmd = this.ToYmd.DateTime.ToString("yyyyMMdd");                                // 기간 - To Date
            var strToHm = this.ToHm.DateTime.ToString("HH00");                                      // 기간 - To Time

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                                              // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpId);                                                // 설비 ID
            dicInputParam.Add("P_PRC_ID", strPrcId);                                                // 프로세스 ID
            dicInputParam.Add("P_PID", strPid);                                                     // PID
            dicInputParam.Add("P_JOB_FR_YMD", strFrYmd);                                            // 기간 - From Date
            dicInputParam.Add("P_JOB_FR_HM", strFrHm);                                              // 기간 - From Time
            dicInputParam.Add("P_JOB_TO_YMD", strToYmd);                                            // 기간 - To Date
            dicInputParam.Add("P_JOB_TO_HM", strToHm);                                              // 기간 - To Time
            dicInputParam.Add("P_BCR_INFO", strBcrInfo);                                            // 바코드INFO
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

        #region > ECS Log 조회
        #region >> 버튼 클릭 이벤트
        #region + ECS Log 조회 버튼 클릭 이벤트
        /// <summary>
        /// ECS Log 조회 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 센터 데이터 조회
                DataSet dsRtnValue = this.GetSP_LOG_INFO_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.EcsLogMgmtList = new ObservableCollection<EcsLogMgmt>();
                    // 오라클인 경우 TableName = TB_COM_PRC_LOG_HIST
                    this.EcsLogMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.EcsLogMgmtList.ToObservableCollection(null);
                    this.BaseClass.MsgInfo(strErrMsg);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.EcsLogMgmtList;

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

        #region >> 그리드 관련 이벤트
        #region + 그리드 클릭 이벤트
        /// <summary>
        /// 그리드 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void GridMaster_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    var view = (sender as GridControl).View as TableView;
        //    var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);
        //    if (hi.InRowCell)
        //    {
        //        if (hi.Column.FieldName.Equals("USE_YN") == false) { return; }

        //        if (view.ActiveEditor == null)
        //        {
        //            view.ShowEditor();

        //            if (view.ActiveEditor == null) { return; }
        //            Dispatcher.BeginInvoke(new Action(() =>
        //            {
        //                view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
        //            }), DispatcherPriority.Render);
        //        }
        //    }
        //}
        #endregion

        #region + 그리드 컬럼 Indicator 영역에 순번 표현 관련 이벤트
        /// <summary>
        /// 그리드 컬럼 Indicator 영역에 순번 표현 관련 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMaster_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData == true)
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #endregion
        #endregion

        #region >> 콤보박스 클릭 이벤트

        #region + EQP_ID 콤보박스 클릭 이벤트
        #region >> EQP_ID 선택시 PRC_ID 맵핑
        /// <summary>
        /// EQP_ID 선택시 해당되는 ATTR01의 값(C or W)에 따라 PRC_ID 맵핑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CboEqpId_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            try
            {
                this.InitComboBoxInfo();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
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
        #endregion
    }
}
