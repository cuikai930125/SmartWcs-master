using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.Analysis.DataMembers.A1003_SRT;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SMART.WCS.UI.Analysis.Views.SORTER
{
    /// <summary>
    /// A1003_SRT.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class A1003_SRT : UserControl
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
        public A1003_SRT()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public A1003_SRT(List<string> _liMenuNavigation)
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
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의

        #region > 슈트별 처리현황 관리
        #region >> 그리드 - 좌측 테이블 리스트 출력
        /// <summary>
        /// 좌측 테이블 리스트
        /// </summary>
        public static readonly DependencyProperty ResultbyChuteMgmtProperty
            = DependencyProperty.Register("ResultbyChuteMgmtList", typeof(ObservableCollection<ResultbyChuteMgmt>), typeof(A1003_SRT)
                , new PropertyMetadata(new ObservableCollection<ResultbyChuteMgmt>()));

        public ObservableCollection<ResultbyChuteMgmt> ResultbyChuteMgmtList
        {
            get { return (ObservableCollection<ResultbyChuteMgmt>)GetValue(ResultbyChuteMgmtProperty); }
            set { SetValue(ResultbyChuteMgmtProperty, value); }
        }
        #endregion
        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty LeftGridRowCountProperty
            = DependencyProperty.Register("LeftGridRowCount", typeof(string), typeof(A1003_SRT), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string LeftGridRowCount
        {
            get { return (string)GetValue(LeftGridRowCountProperty); }
            set { SetValue(LeftGridRowCountProperty, value); }
        }
        #endregion

        #region >> 그리드 - 우측 차트 출력
        /// <summary>
        /// 좌측 테이블 리스트
        /// </summary>
        public static readonly DependencyProperty ResultbyChuteGraphProperty
            = DependencyProperty.Register("ResultbyChuteGraphList", typeof(ObservableCollection<ResultbyChuteMgmt>), typeof(A1003_SRT)
                , new PropertyMetadata(new ObservableCollection<ResultbyChuteMgmt>()));

        public ObservableCollection<ResultbyChuteMgmt> ResultbyChuteGraphList
        {
            get { return (ObservableCollection<ResultbyChuteMgmt>)GetValue(ResultbyChuteGraphProperty); }
            set { SetValue(ResultbyChuteGraphProperty, value); }
        }
        #endregion
        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty RightGridRowCountProperty
            = DependencyProperty.Register("RightGridRowCount", typeof(string), typeof(A1003_SRT), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string RightGridRowCount
        {
            get { return (string)GetValue(RightGridRowCountProperty); }
            set { SetValue(RightGridRowCountProperty, value); }
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

            this.BaseClass.BindingCommonComboBox(this.CboSrt, "EQP_ID", commonParam_EQP_ID, false); //설비 ID
            this.BaseClass.BindingCommonComboBox(this.cboZoneID, "ZONE_ID", null, true);            // ZONE ID

            this.chartXContent.Content = this.BaseClass.GetResourceValue("CHUTE_ID");         //X축 : CHUTE 번호(CHUTE_ID)
            this.chartYContent.Content = this.BaseClass.GetResourceValue("RSLT_BY_CHUTE");    // Y축 : 물량(정상수량+에러수량)
            

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
            this.Loaded += A1003_SRT_Loaded;
            #endregion

            #region + 슈트별 처리현황
            #region ++ 버튼 클릭 이벤트
            // 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_First_PreviewMouseLeftButtonUp;
            // 엑셀다운로드
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_First_PreviewMouseLeftButtonUp;
            #endregion

            #region ++ Row 순번 채번 이벤트
            this.gridMaster.CustomUnboundColumnData += GridMaster_CustomUnboundColumnData;
            #endregion

            #region ++ 그리드 이벤트
            // 그리드 클릭 이벤트
            this.gridMaster.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;
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
            this.LeftGridRowCount = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";        // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion

        #region >> BindingChart - 시뮬레이션 결과 차트
        /// <summary>
        /// 시뮬레이션 결과 차트
        /// </summary>
        /// <param name="_dtChartData">차트 데이터</param>
        private void BindingChart(DataTable _dtChartData)
        {
            try
            {
                foreach (DataRow drRow in _dtChartData.Rows)
                {
                    switch (drRow["FLAG"].ToString())
                    {
                        case "NML": // 정상
                            drRow["FLAG_NM"] = this.BaseClass.GetResourceValue("NML_PROC");
                            break;
                        case "ERR": // 오류
                            drRow["FLAG_NM"] = this.BaseClass.GetResourceValue("ERR");
                            break;
                    }
                }

                _dtChartData.AcceptChanges();

                this.chartRslt.DataSource = _dtChartData;
            }
            catch { throw; }
            finally
            {
                this.chartRslt.EndInit();
            }
        }
        #endregion


        #endregion

        #region > 데이터 관련
        #region >> GetSP_CHUTE_RSLT_LIST_INQ - 슈트별 처리현황 조회
        /// <summary>
        /// 시간대별 처리현황 조회
        /// </summary>
        private DataSet GetSP_CHUTE_RSLT_LIST_INQ()
       {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_A1003_SRT.SP_CHUTE_RSLT_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_CHUTE_RSLT_LIST", "O_RSLT" };

            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            var strEqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.CboSrt);                    //설비 ID

            var strFromDt = this.FromDT.DateTime.ToString("yyyyMMdd");
            var strFromTm = this.BaseClass.ReplaceText(this.FromTM, ":", string.Empty);
            var strToDt = this.ToDT.DateTime.ToString("yyyyMMdd");
            var strToTm = this.BaseClass.ReplaceText(this.ToTM, ":", string.Empty);                          //ToYmd

            var strZoneID       = this.BaseClass.ComboBoxSelectedKeyValue(this.cboZoneID);      // Zone ID

            strFromDt += strFromTm;
            strToDt += strToTm;

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                  // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpId);                    //설비 ID  
            dicInputParam.Add("P_FM_YMD", strFromDt);                  //FromYmd
            dicInputParam.Add("P_TO_YMD", strToDt);                    //ToYmd
            dicInputParam.Add("P_ZONE_ID",          strZoneID);     // Zone ID
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

        #region >> GetSP_CHUTE_RSLT_GRAPH_INQ - 슈트별 처리현황 조회 (그래프)
        /// <summary>
        /// 시간대별 처리현황 조회
        /// </summary>
        private DataSet GetSP_CHUTE_RSLT_GRAPH_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_A1003_SRT.SP_CHUTE_RSLT_GRAPH_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_CHUTE_RSLT_GRAPH", "O_RSLT" };

            var strCntrCd       = this.BaseClass.CenterCD;                                          // 센터 코드
            var strEqpId        = this.BaseClass.ComboBoxSelectedKeyValue(this.CboSrt);             //설비 ID
            var strFromDt       = this.FromDT.DateTime.ToString("yyyyMMdd");
            var strFromTm       = this.BaseClass.ReplaceText(this.FromTM, ":", string.Empty);
            var strToDt         = this.ToDT.DateTime.ToString("yyyyMMdd");  
            var strToTm         = this.BaseClass.ReplaceText(this.ToTM, ":", string.Empty);         //ToYmd
            var strZoneID       = this.BaseClass.ComboBoxSelectedKeyValue(this.cboZoneID);          // Zone ID

            strFromDt += strFromTm;
            strToDt += strToTm;

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",      strCntrCd);         // 센터 코드
            dicInputParam.Add("P_EQP_ID",       strEqpId);          //설비 ID  
            dicInputParam.Add("P_FM_YMD",       strFromDt);         //FromYmd
            dicInputParam.Add("P_TO_YMD",       strToDt);           //ToYmd
            dicInputParam.Add("P_ZONE_ID",      strZoneID);         // Zone ID
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
        private void A1003_SRT_Loaded(object sender, RoutedEventArgs e)
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

        #region > 슈트별 처리현황 관리
        #region >> 버튼 클릭 이벤트
        #region + 슈트별 처리현황 조회 버튼 클릭 이벤트
        /// <summary>
        /// 슈트별 처리현황 조회 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
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

                #region + 좌측 그리드 - 슈트별 처리현황 리스트 (표)
                // 시간대별 처리현황 데이터 조회
                DataSet dsRtnValue = this.GetSP_CHUTE_RSLT_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.ResultbyChuteMgmtList = new ObservableCollection<ResultbyChuteMgmt>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.ResultbyChuteMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ResultbyChuteMgmtList.ToObservableCollection(null);
                    this.BaseClass.MsgInfo(strErrMsg);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.ResultbyChuteMgmtList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText();
                #endregion

                #region + 우측 차트 - 슈트별 처리현황 그래프 (차트)

                DataSet dsRtnValue_Graph = this.GetSP_CHUTE_RSLT_GRAPH_INQ();   //우측 차트 정보
                if (dsRtnValue_Graph == null) { return; }

                var strErrCode_Graph = string.Empty;
                var strErrMsg_Graph = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue_Graph, ref strErrCode_Graph, ref strErrMsg_Graph) == true)
                {
                    // 정상 처리된 경우
                    this.ResultbyChuteGraphList = new ObservableCollection<ResultbyChuteMgmt>();
                    // 오라클인 경우
                    this.ResultbyChuteGraphList.ToObservableCollection(dsRtnValue_Graph.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ResultbyChuteGraphList.ToObservableCollection(null);
                    this.BaseClass.MsgInfo(strErrMsg_Graph);
                }

                //// 시뮬레이션 결과 차트 바인딩
                this.BindingChart(dsRtnValue_Graph.Tables[0]);
                #endregion

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

        #region >> 엑셀 다운로드 버튼 클릭 이벤트
        private void BtnExcelDownload_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

   