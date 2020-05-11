using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.Monitoring.DataMembers.M1003;
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

namespace SMART.WCS.UI.Monitoring.Views.SORTER
{
    /// <summary>
    /// Monitoring > 설비 알람 현황 조회
    /// </summary>
    public partial class M1003 : UserControl
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
        /// BaseInfo 선언
        /// </summary>
        BaseInfo BaseInfo = new BaseInfo();

        /// <summary>
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public M1003()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        /// 
        public M1003(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
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

                // DateEdit 초기화
                this.ToOcurYmd.EditValue = DateTime.Today.AddDays(1);
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }

        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(M1003), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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
            }
        }
        #endregion

        #region > 센터 관리
        #region >> 배치 차수 조회
        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public static readonly DependencyProperty EqpAlarmInfoProperty
            = DependencyProperty.Register("EqpAlarmInfoList", typeof(ObservableCollection<EqpAlarmInfo>), typeof(M1003)
                , new PropertyMetadata(new ObservableCollection<EqpAlarmInfo>()));

        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public ObservableCollection<EqpAlarmInfo> EqpAlarmInfoList
        {
            get { return (ObservableCollection<EqpAlarmInfo>)GetValue(EqpAlarmInfoProperty); }
            set { SetValue(EqpAlarmInfoProperty, value); }
        }
        #endregion

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(M1003), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string TabFirstGridRowCount
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
            //string[] commonParam_ALARM_STAT_CD = { BaseClass.CenterCD, "ALARM_STAT", BaseClass.UserID, string.Empty };

            this.BaseClass.BindingCommonComboBox(this.CboSrt, "EQP_ID", commonParam_EQP_ID, true);                      //설비 ID
            this.BaseClass.BindingCommonComboBox(this.CboAlarmStat, "ALARM_STAT_CD", null, true);   //설비 알람 상태 코드

        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += M1003_Loaded;
            #endregion

            #region + 거래처 관리
            #region ++ 버튼 클릭 이벤트
            // 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            ;
            #endregion

            #region ++ Row 순번 채번 이벤트
            this.gridMaster.CustomUnboundColumnData += GridMaster_CustomUnboundColumnData;
            #endregion

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
            this.TabFirstGridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";               // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion
        #endregion


        #region > 데이터 관련
        #region >>  GetSP_ALARM_LIST_INQ - 설비 알람 현황 조회
        /// <summary>
        /// 설비 알람 현황 조회
        /// </summary>
        private DataSet GetSP_ALARM_LIST_INQ()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue = null;
                var strProcedureName = "PK_M1003.SP_ALARM_LIST_INQ";
                Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
                string[] arrOutputParam = { "O_ALARM_LIST", "O_RSLT" };

                var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
                var strEqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.CboSrt);                    // 설비 ID
                var strFromYmd = this.BaseClass.GetCalendarValue(this.FrOcurYmd);                       // 발생기간 FROM
                var strToYmd = this.BaseClass.GetCalendarValue(this.ToOcurYmd);                         // 발생기간 TO
                var strAlarmStatCd = this.BaseClass.ComboBoxSelectedKeyValue(this.CboAlarmStat);        // 알람 상태 코드
                var UserId = this.BaseClass.UserID;                                                     // User ID

                var strErrCode = string.Empty;          // 오류 코드
                var strErrMsg = string.Empty;           // 오류 메세지
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTR_CD", strCntrCd);                 // 센터 코드
                dicInputParam.Add("P_EQP_ID", strEqpId);                  // 설비 ID
                dicInputParam.Add("P_FM_OCUR_YMD", strFromYmd);                // 발생기간 FROM
                dicInputParam.Add("P_TO_OCUR_YMD", strToYmd);                  // 발생기간 TO
                dicInputParam.Add("P_ALARM_STAT_CD", strAlarmStatCd);            // 알람 상태 코드
                dicInputParam.Add("P_USER_ID", UserId);                    // User ID
                #endregion

                #region 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }

                #endregion
                return dsRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void M1003_Loaded(object sender, RoutedEventArgs e)
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

        #region > 설비 알람 현황 조회
        #region >> 버튼 클릭 이벤트
        #region + 설비 알람 현황 조회버튼 클릭 이벤트
        /// <summary>
        /// 출고처 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.EqpAlarmInfoList.ForEach(p => p.ClearError());

                // 출고처 데이터 조회
                DataSet dsRtnValue = this.GetSP_ALARM_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.EqpAlarmInfoList = new ObservableCollection<EqpAlarmInfo>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.EqpAlarmInfoList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.EqpAlarmInfoList.ToObservableCollection(null);
                    this.BaseClass.MsgInfo(strErrMsg);
                }

                if (this.EqpAlarmInfoList.Count == 0)
                {
                    //this.BaseClass.MsgInfo("INFO_NOT_INQ"); //INFO_NOT_INQ : 조회된 데이터가 없습니다.
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.EqpAlarmInfoList;

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


        #region + 설비 알람 현황 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 설비 알람 현황 엑셀 다운로드 버튼 클릭 이벤트
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

                tv.Add(this.MasterGrid);
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
