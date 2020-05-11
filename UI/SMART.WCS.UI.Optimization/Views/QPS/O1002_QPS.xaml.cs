using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Editors;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.Data.DataMembers;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.Optimization.DataMembers.OPT0102;
using SMART.WCS.UI.Optimization.Utility;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.Optimization.Views.QPS
{
    /// <summary>
    /// 최적화 수행처리 - ROC 수행
    /// </summary>
    public partial class O1002_QPS : UserControl
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
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 선 그래프 객체
        /// </summary>
        LineSeries2D g_ls2D;

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public O1002_QPS()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public O1002_QPS(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 공통코드를 사용하지 않는 콤보박스를 설정한다.
                this.InitComboBoxInfo();

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
        #region > 상단 그리드 - 오더 리스트
        public static readonly DependencyProperty MainGridOrderListProperty
            = DependencyProperty.Register("MainGridOrderList", typeof(ObservableCollection<OrderList>), typeof(O1002_QPS)
            , new PropertyMetadata(new ObservableCollection<OrderList>()));

        public ObservableCollection<OrderList> MainGridOrderList
        {
            get { return (ObservableCollection<OrderList>)GetValue(MainGridOrderListProperty); }
            set { SetValue(MainGridOrderListProperty, value); }
        }
        #endregion

        #region > 우측 하단 그리드 - 배치 분할 총량 피킹 SKU
        public static readonly DependencyProperty RightBottomBtchDivTotPickListProperty
            = DependencyProperty.Register("RightBottomBtchDivTotPickList", typeof(ObservableCollection<BtchDivTotPickList>), typeof(O1002_QPS)
            , new PropertyMetadata(new ObservableCollection<BtchDivTotPickList>()));

        public ObservableCollection<BtchDivTotPickList> RightBottomBtchDivTotPickList
        {
            get { return (ObservableCollection<BtchDivTotPickList>)GetValue(RightBottomBtchDivTotPickListProperty); }
            set { SetValue(RightBottomBtchDivTotPickListProperty, value); }
        }
        #endregion

        #region > 좌측 하단 그리드 - ROC 최적화 결과
        public static readonly DependencyProperty LeftBottomRocOptRsltListProperty
            = DependencyProperty.Register("LeftBottomRocOptRsltList", typeof(ObservableCollection<RocOptRslt>), typeof(O1002_QPS)
            , new PropertyMetadata(new ObservableCollection<RocOptRslt>()));

        public ObservableCollection<RocOptRslt> LeftBottomRocOptRsltList
        {
            get { return (ObservableCollection<RocOptRslt>)GetValue(LeftBottomRocOptRsltListProperty); }
            set { SetValue(LeftBottomRocOptRsltListProperty, value); }
        }
        #endregion

        //#region > 좌측 하단 컨트롤 - ROC 최적화 결과
        //#region >> TotalOrderCnt - TOTAL 오더수
        //public static readonly DependencyProperty TotalOrderCntProperty
        //    = DependencyProperty.Register("TotalOrderCnt", typeof(int), typeof(OPT0104_QPS), new PropertyMetadata(null));

        //public int TotalOrderCnt
        //{
        //    get { return (int)GetValue(TotalOrderCntProperty); }
        //    set { SetValue(TotalOrderCntProperty, value); }
        //}
        //#endregion

        //#region >> MaxOrderCnt - MAX 오더수
        //public static readonly DependencyProperty MaxOrderCntProperty
        //    = DependencyProperty.Register("MaxOrderCnt", typeof(decimal), typeof(OPT0104_QPS), new PropertyMetadata(null));

        //public decimal MaxOrderCnt
        //{
        //    get { return (decimal)GetValue(MaxOrderCntProperty); }
        //    set { SetValue(MaxOrderCntProperty, value); }
        //}
        //#endregion

        //#region >> AverageOrderCnt AVG 오더수
        //public static readonly DependencyProperty AverageOrderCntProperty
        //    = DependencyProperty.Register("AverageOrderCnt", typeof(decimal), typeof(OPT0104_QPS), new PropertyMetadata(null));

        //public decimal AverageOrderCnt
        //{
        //    get { return (decimal)GetValue(AverageOrderCntProperty); }
        //    set { SetValue(AverageOrderCntProperty, value); }
        //}
        //#endregion

        //#region >> TotalSkuCnt - TOTAL 오더수
        //public static readonly DependencyProperty TotalSkuCntProperty
        //    = DependencyProperty.Register("TotalSkurCnt", typeof(int), typeof(OPT0104_QPS), new PropertyMetadata(null));

        //public int TotalSkuCnt
        //{
        //    get { return (int)GetValue(TotalSkuCntProperty); }
        //    set { SetValue(TotalSkuCntProperty, value); }
        //}
        //#endregion

        //#region >> MaxSkuCnt - MAX 오더수
        //public static readonly DependencyProperty MaxSkuCntProperty
        //    = DependencyProperty.Register("MaxSkuCnt", typeof(decimal), typeof(OPT0104_QPS), new PropertyMetadata(null));

        //public decimal MaxSkuCnt
        //{
        //    get { return (decimal)GetValue(MaxSkuCntProperty); }
        //    set { SetValue(MaxSkuCntProperty, value); }
        //}
        //#endregion

        //#region >> AverageOrderCnt - AVG 오더수
        //public static readonly DependencyProperty AverageSkuCntProperty
        //    = DependencyProperty.Register("AverageSkuCnt", typeof(decimal), typeof(OPT0104_QPS), new PropertyMetadata(null));

        //public decimal AverageSkuCnt
        //{
        //    get { return (decimal)GetValue(AverageSkuCntProperty); }
        //    set { SetValue(AverageSkuCntProperty, value); }
        //}
        //#endregion
        //#endregion
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            // 달력 컨트롤 기본값 설정
            this.deWrkPlanWmd.DateTime = DateTime.Now;

            #region + 콤보박스 설정 
            // Location Balance 
            // 공통코드
            // IGNORE_LOC	SKU별 구역무시
            // USE_LOC      SKU별 구역고려
            this.BaseClass.BindingCommonComboBox(this.cboLocBal, "LOC_BAL", null, false);

            // Sorting Mode
            // 공통코드
            // RANDOM       무작위 SKU순 정렬
            // ROC          ROC 로직 수행
            // SKU_ORD_CNT  연결 오더수 SKU순 정렬
            this.BaseClass.BindingCommonComboBox(this.cboSortMode, "SORT_MODE", null, false);
            #endregion
        }
        #endregion

        #region >> InitComboBoxInfo - 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// <summary>
        /// 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// </summary>
        private async void InitComboBoxInfo()
        {
            #region ++ 공통코드 사용하지 않는 콤보박스 설정

            var strWrkPlanYmd       = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);   // 출고일자
            // (공통코드 사용하지 않음)
            DataTable dtComboData   = await Utility.HelperClass.GetSP_OPT_QPS_DATA_SET_LIST_INQ(strWrkPlanYmd);

            // 조회 데이터가 없는 경우 구문을 리턴한다.
            if (dtComboData.Rows.Count == 0)
            {
                // DATA 그룹 데이터가 없는 경우 (콤보박스 설정이 안되는 경우) ROC 수행 버튼을 비활성화한다.
                this.BaseClass.SetSimpleButtonIsEnabled(this.btnRocExec, false);
                this.cboDataSetID.ItemsSource = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);
                this.cboRocOptSeq.ItemsSource = null;

                return;
            }

            // 콤보박스 설정을 위해 컬럼명을 변경한다.
            dtComboData = this.BaseClass.ModifyComboBoxColumnHeaderName(dtComboData);

            // DATA 그룹 데이터가 1개 컬럼으로 조회되기 때문에 콤보박스 설정을 위해 컬럼을 추가 생성한 후 데이터를 복사한다.
            dtComboData.Columns.Add("NAME", typeof(string));

            foreach (DataRow drRow in dtComboData.Rows)
            {
                drRow["NAME"] = drRow["CODE"];
            }

            this.cboDataSetID.ItemsSource = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);

            this.cboDataSetID.SelectedIndexChanged -= CboDataSetID_SelectedIndexChanged;
            this.cboDataSetID.SelectedIndex = 0;
            this.cboDataSetID.SelectedIndexChanged += CboDataSetID_SelectedIndexChanged;

            // DATA 그룹 데이터가 있는 경우 (콤보박스 설정이 된 경우) ROC 수행 버튼을 활성화한다.
            this.BaseClass.SetSimpleButtonIsEnabled(this.btnRocExec, true);

            this.CboDataSetID_SelectedIndexChanged(null, null);
            #endregion
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 달력 컨트롤 변경 이벤트
            // 달력 컨트롤 선택 변경
            this.deWrkPlanWmd.EditValueChanged += DeWrkPlanWmd_EditValueChanged;
            #endregion

            #region + 콤보박스 변경 이벤트
            // Data Set 정보
            this.cboDataSetID.SelectedIndexChanged += CboDataSetID_SelectedIndexChanged;
            #endregion

            #region + 텍스트 박스 이벤트
            // 텍스트 박스 클릭시 값이 전체 선택되도록 설정하는 이벤트
            this.txtBtchDivNum.GotFocus += TxtBtchDivNum_GotFocus;
            #endregion

            #region + 버튼 이벤트
            // 조회 버튼 클릭 이벤트
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드 버튼 클릭 이벤트
            this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;

            // 최적화 실행 버튼 클릭 이벤트
            this.btnRocExec.PreviewMouseLeftButtonUp += BtnRocExec_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 이벤트
            // 오더리스트 그리드 순번 채번을 위한 이벤트
            this.gridMain.CustomUnboundColumnData += GridMain_CustomUnboundColumnData;

            // 배치 분할 총량 피킹 SKU 그리드 순번 채번을 위한 그리드
            this.gridRightBottom.CustomUnboundColumnData += GridRightBottom_CustomUnboundColumnData;
            #endregion
        }
        #endregion
        #endregion

        #region > 차트 바인딩
        private void BindingChartOptRslt(DataTable _dtChartData)
        {
            try
            {
                if (this.xyDiagramRsltMain.Series.Count > 0) { this.xyDiagramRsltMain.Series.Clear(); }

                this.chartBtchDivTotSku.BeginInit();
                this.g_ls2D                         = new LineSeries2D();
                this.g_ls2D.ArgumentScaleType       = ScaleType.Numerical;
                this.g_ls2D.DataSourceSorted        = true;
                this.g_ls2D.LineStyle               = new LineStyle(1);
                this.g_ls2D.CrosshairLabelPattern   = "{S}: {V:0.0}";
                this.g_ls2D.ArgumentDataMember      = "Argument";
                this.g_ls2D.ValueDataMember         = "Value";

                List<ChartDataPointMember> points   = this.GetGenerateDataCell(_dtChartData);
                this.g_ls2D.DataSource              = points;
                
                if (this.xyDiagramRsltMain.Series.Count > 0) { this.xyDiagramRsltMain.Series.Clear(); }
                this.xyDiagramRsltMain.Series.Add(this.g_ls2D);
            }
            catch { }
            finally
            {
                this.chartBtchDivTotSku.EndInit();
                this.g_ls2D = null;
            }
        }
        #endregion

        #region >> GetGenerateDataCell - 최적화 수량 결과 차트 데이터 구성 함수
        /// <summary>
        /// 최적화 수량 결과 차트 데이터 구성 함수
        /// </summary>
        /// <param name="_dtValue">차트 데이터</param>
        /// <returns></returns>
        private List<ChartDataPointMember> GetGenerateDataCell(DataTable _dtValue)
        {
            List<ChartDataPointMember> points = new List<ChartDataPointMember>();

            foreach (DataRow drRow in _dtValue.Rows)
            {
                double dValue       = Convert.ToDouble(drRow["BTCH_DIV_CNT"]);
                double dArgument    = Convert.ToDouble(drRow["SKU_CNT"]); 
                points.Add(new ChartDataPointMember(dValue, dArgument));
            }

            return points;
        }
        #endregion

        #region > 데이터 관련
        #region >> GetSP_OPT_QPS_MAX_ROC_OPT_SET_INQ - 가장 최근의 QPS ROC OPTION SET LIST 조회
        /// <summary>
        /// 가장 최근의 QPS ROC OPTION SET LIST 조회
        /// </summary>
        /// <returns></returns>
        private async Task<DataSet> GetSP_OPT_QPS_MAX_ROC_OPT_SET_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "SP_OPT_QPS_MAX_ROC_OPT_SET_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_ROC_OPT_SET_LIST", "O_RTN_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                                      // 센터코드
            var strDataSetID        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);   // 데이터 그룹 ID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCenterCD);       // 센터코드
            dicInputParam.Add("P_DATA_SET_ID",      strDataSetID);      // 데이터 그룹 ID
            #endregion

            #region 데이터 조회
            using (BaseDataAccess da = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = da.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            if (dsRtnValue.Tables.Count != 2)
            {
                throw new Exception("조회 데이터에 잘못되었습니다. 리턴 테이블이 2개가 아닙니다.");
            }

            return dsRtnValue;
        }
        #endregion

        #region >> GetRocOptRsltInfo - 최적화 결과 조회
        private async Task<DataSet> GetRocOptRsltInfo()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue          = null;
            var strProcedureName        = "PK_O1002_QPS.SP_ROC_RSLT_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam     = { "O_ROC_OPT_RSLT_LIST", "O_ROC_OPT_SUM_LIST", "O_ROC_OPT_BTCH_DIV_LIST", "O_RTN_RSLT" };

            var strCenterCD             = this.BaseClass.CenterCD;                                      // 센터코드
            var strWrkPlanYmd           = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);           // 출고일자
            var strDataSetID            = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);   // Data 그룹
            var strRocOptSeq            = this.BaseClass.ComboBoxSelectedKeyValue(this.cboRocOptSeq);   // ROC 최적화 차수
            var strFilterDelYNExcept    = this.chkFltrDelYnExcpt.IsChecked == true ? "Y" : "N";         // 필터삭제건 제외값
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCenterCD);           // 센터코드
            dicInputParam.Add("P_WRK_PLAN_YMD", strWrkPlanYmd);         // 출고 일자
            dicInputParam.Add("P_DATA_SET_ID", strDataSetID);          // Data 그룹
            dicInputParam.Add("P_ROC_OPT_SEQ", strRocOptSeq);          // ROC 최적화 차수
            dicInputParam.Add("P_FLTR_DEL_YN_EXCPT_YN", strFilterDelYNExcept);  // 필터삭제건 제외값
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            if (dsRtnValue.Tables.Count != 4)
            {
                throw new Exception("조회된 데이터에 문제가 있습니다.|테이블이 4개 아닙니다.");
            }

            return dsRtnValue;
        }
        #endregion

        #region >> SaveSP_ROC_OPT_OPT_SEQ_SAVE - ROC 수행 (저장)
        /// <summary>
        /// ROC 수행 (저장)
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_dicInputParam">저장 파라메터</param>
        /// <returns></returns>
        private async Task<DataSet> SaveSP_ROC_OPT_OPT_SEQ_SAVE(BaseDataAccess _da, Dictionary<string, object> _dicInputParam)
        {
             #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_O1002_QPS.SP_ROC_OPT_OPT_SEQ_SAVE";
            string[] arrOutputParam                     = { "O_OPT_SEQ", "O_RTN_RSLT" };
            #endregion

            await System.Threading.Tasks.Task.Run(() =>
            {
                dsRtnValue = _da.GetSpDataSet(strProcedureName, _dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);

            return dsRtnValue;
        }
        #endregion
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
        private async void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 최적화 수행관리 오더 리스트 조회
                DataSet dsRtnValue = await this.GetRocOptRsltInfo();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;     // 오류 코드
                var strErrMsg = string.Empty;     // 오류 메세지

                //this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
                this.ToolStripChangeStatusLabelEvent(string.Empty);

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우

                    #region + 오더리스트
                    this.MainGridOrderList = new ObservableCollection<OrderList>();
                    this.MainGridOrderList.ToObservableCollection(dsRtnValue.Tables[0]);
                    #endregion

                    #region + 배치 분할 총량 피킹 SKU
                    this.RightBottomBtchDivTotPickList = new ObservableCollection<BtchDivTotPickList>();
                    this.RightBottomBtchDivTotPickList.ToObservableCollection(dsRtnValue.Tables[2]);
                    #endregion

                    #region + ROC 최적화 결과
                    this.LeftBottomRocOptRsltList = new ObservableCollection<RocOptRslt>();
                    this.LeftBottomRocOptRsltList.ToObservableCollection(dsRtnValue.Tables[1]);

                    //if (dsRtnValue.Tables[1].Rows.Count > 0)
                    //{
                    //    var drRowData   = dsRtnValue.Tables[1].Rows[0];

                    //    this.TotalOrderCnt      = Convert.ToInt32(drRowData["ORD_CNT"]);        // TOTAL 오더수
                    //    this.MaxOrderCnt        = Convert.ToDecimal(drRowData["MAX_ORD_LEN"]);  // MAX 오더수
                    //    this.AverageOrderCnt    = Convert.ToDecimal(drRowData["AVG_ORD_LEN"]);  // Average

                    //    this.TotalSkuCnt        = Convert.ToInt32(drRowData["SKU_CNT"]);        // TOTAL SKU수
                    //    this.MaxSkuCnt          = Convert.ToDecimal(drRowData["MAX_SKU_LEN"]);  // MAX SKU수
                    //    this.AverageSkuCnt      = Convert.ToDecimal(drRowData["AVG_SKU_LEN"]);  // Average SKU수
                    //}

                    #endregion

                    #region 차트 - 배치분할 총량 피킹 SKU
                    this.BindingChartOptRslt(dsRtnValue.Tables[2]);
                    #endregion
                }
                else
                {
                    // 오류 발생한 경우

                    #region + 오더리스트
                    this.MainGridOrderList.ToObservableCollection(null);
                    #endregion

                    #region + 배치 분할 총량 피킹 SKU
                    this.RightBottomBtchDivTotPickList.ToObservableCollection(null);
                    #endregion

                    #region + ROC 최적화 결과
                    this.LeftBottomRocOptRsltList.ToObservableCollection(null);

                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);

                    //this.TotalOrderCnt      = 0;
                    //this.MaxOrderCnt        = 0.00m;
                    //this.AverageOrderCnt    = 0.00m;

                    //this.TotalSkuCnt        = 0;
                    //this.MaxSkuCnt          = 0.00m;
                    //this.AverageOrderCnt    = 0.00m;
                    #endregion
                }

                //// 그리드 바인딩
                //this.gridMain.ItemsSource               = this.MainGridOrderList;
                //this.gridRightBottom.ItemsSource        = this.RightBottomBtchDivTotPickList;
                //this.gridLeftBottom.ItemsSource         = this.LeftBottomRocOptRsltList;

                // 정상 조회된 후 
                // 가장 최근의 QPS ROC OPTION SET LIST를 조회한다.
                DataSet dsMaxRocOptSetInqData = await this.GetSP_OPT_QPS_MAX_ROC_OPT_SET_INQ();

                foreach (DataRow drRow in dsMaxRocOptSetInqData.Tables[0].Rows)
                {
                    this.txtBtchDivNum.Text = drRow["BTCH_DIV_LIM"].ToString();

                    if (this.cboLocBal.ItemsSource != null)
                    {
                        //this.cboLocBal.SelectedItem = drRow["LOC_BAL"].ToString();
                        this.cboLocBal.EditValue = drRow["LOC_BAL"].ToString();
                    }

                    if (this.cboSortMode.ItemsSource != null)
                    {
                        //this.cboSortMode.SelectedItem = drRow["SORT_MODE"].ToString();
                        this.cboSortMode.EditValue = drRow["SORT_MODE"].ToString();
                    }

                    this.chkAll.IsChecked = drRow["SET_MAX_DIV"].ToString().ToUpper() == "OFF" ? true : false;
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

        #region >> 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 엑셀 다운로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }
        #endregion

        #region >> 최적화 실행 버튼 클릭 이벤트
        /// <summary>
        /// 최적화 실행 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnRocExec_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.BaseClass.MsgQuestion("ROC 수행을 처리하시겠습니까?");
            if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

            // 상태바 (아이콘) 실행
            this.loadingScreen.IsSplashScreenShown = true;
            DataSet dsRtnValue = null;

            string strWrkPlanYmd    = string.Empty;
            string strDataSetID     = string.Empty;

            try
            { 
                #region 파라메터 변수 선언 및 값 할당
                Dictionary<string, object> dicInputParam = new Dictionary<string, object>();

                var strCenterCD             = this.BaseClass.CenterCD;                                      // 센터코드
                strWrkPlanYmd               = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);           // 출고일자
                strDataSetID                = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);   // Data 그룹
                var BtchDivLim              = this.txtBtchDivNum.Text.Trim();                               // 배치분배차수
                var strSetMaxDiv            = this.chkAll.IsChecked == true ? "OFF" : "ON";                 // 체크박스
                var strLocBal               = this.BaseClass.ComboBoxSelectedKeyValue(this.cboLocBal);      // Location Balance
                var strSortMode             = this.BaseClass.ComboBoxSelectedKeyValue(this.cboSortMode);    // Sorting Mode
                var strUserID               = this.BaseClass.UserID;                                        // 사용자 ID
                #endregion

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    #region Input 파라메터
                    dicInputParam.Add("P_CNTR_CD",          strCenterCD);       // 센터코드
                    dicInputParam.Add("P_WRK_PLAN_YMD",     strWrkPlanYmd);     // 출고일자
                    dicInputParam.Add("P_DATA_SET_ID",      strDataSetID);      // Data 그룹
                    dicInputParam.Add("P_BTCH_DIV_LIM",     BtchDivLim);        // 배치분배차수
                    dicInputParam.Add("P_SET_MAX_DIV",      strSetMaxDiv);      // 체크박스  
                    dicInputParam.Add("P_LOC_BAL",          strLocBal);         // Location Balance
                    dicInputParam.Add("P_SORT_MODE",        strSortMode);       // Sorting Mode
                    dicInputParam.Add("P_USER_ID",          strUserID);         // 사용자 ID
                    #endregion

                    dsRtnValue = await this.SaveSP_ROC_OPT_OPT_SEQ_SAVE(da, dicInputParam);

                    if (dsRtnValue != null)
                    {
                        if (dsRtnValue.Tables.Count == 1)
                        {
                            var strMessage = dsRtnValue.Tables[0].Rows[0]["MSG"].ToString();
                            //this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

                            throw new Exception(strMessage);
                        }

                        if (dsRtnValue.Tables[1].Rows.Count > 0)
                        {
                            if (dsRtnValue.Tables[1].Rows[0]["CODE"].ToString().Equals("0") == false)
                            {
                                var strMessage = dsRtnValue.Tables[1].Rows[0]["MSG"].ToString();
                                throw new Exception(strMessage);
                            }
                        }
                        else
                        {
                            // ERR_SAVE - 저장 중 오류가 발생했습니다.
                            this.BaseClass.MsgError("ERR_SAVE");
                        }
                    }
                }
            }
            catch (Exception err)
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = false;
                // ERR_SAVE - 저장 중 오류가 발생했습니다.
                this.BaseClass.MsgError("ERR_SAVE");
                this.BaseClass.Error(err);
                return;
            }

            #region 웹 서비스 호출 영역
            try
            {
                // 저장 후 리턴받은 값 (OPT_SEQ)
                var strOptSeq = dsRtnValue.Tables[0].Rows[0][0].ToString();

                if (CallJsonWebService.CallServiceRocExec(strWrkPlanYmd, strDataSetID, strOptSeq) == true)
                {
                    await this.GetRocOptRsltInfo();
                }
                else
                {
                    BaseClass.MsgError("ERR_DATA_SEND");
                }
            }
            catch (Exception err)
            {
                BaseClass.MsgError("ERR_DATA_SEND");
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = false;
            }
            #endregion
            
            // ROC 수행이 완료되었습니다. - CMPT_ROC_EXEC
            this.BaseClass.MsgInfo("CMPT_ROC_EXEC");

            this.CboDataSetID_SelectedIndexChanged(null, null);
        }
        #endregion
        #endregion

        #region > 달력 컨트롤 이벤트
        #region >> 달력 컨트롤 변경 이벤트
        /// <summary>
        /// 달력 컨트롤 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeWrkPlanWmd_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            this.InitComboBoxInfo();
        }
        #endregion
        #endregion

        #region > 텍스트박스 이벤트
        #region >> 텍스트 박스 클릭시 값이 전체 선택되도록 설정하는 이벤트
        /// <summary>
        /// 텍스트 박스 클릭시 값이 전체 선택되도록 설정하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBtchDivNum_GotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => ((TextEdit)sender).SelectAll()));
        }
        #endregion
        #endregion

        #region > 콤보박스 이벤트
        #region >> Data Set 콤보박스 변경 이벤트
        /// <summary>
        /// Data Set 콤보박스 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CboDataSetID_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            var strWrkPlanYmd       = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);           // 출고일자
            var iSelectedValue      = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);   // 선택된 데이터 그룹
            DataTable dtComboData   = await Utility.HelperClass.GetSP_OPT_QPS_ROC_SEQ_LIST_INQ(strWrkPlanYmd, iSelectedValue);

            // 조회 데이터가 없는 경우 구문을 리턴한다.
            if (dtComboData.Rows.Count == 0)
            {
                this.cboRocOptSeq.ItemsSource = null;
                return;
            }

            dtComboData                         = this.BaseClass.ModifyComboBoxColumnHeaderName(dtComboData);
            this.cboRocOptSeq.ItemsSource       = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);
            this.cboRocOptSeq.SelectedIndex     = 0;
        }
        #endregion
        #endregion

        #region > 그리드 이벤트
        #region >> 오더리스트 그리드 순번 채번을 위한 이벤트
        /// <summary>
        /// 오더리스트 그리드 순번 채번을 위한 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridRightBottom_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            if (e.IsGetData == true)
            {
                e.Value = e.ListSourceRowIndex + 1;
            }
        }
        #endregion

        #region >> 배치 분할 총량 피킹 SKU 그리드 순번 채번을 위한 그리드
        /// <summary>
        /// 배치 분할 총량 피킹 SKU 그리드 순번 채번을 위한 그리드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMain_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            if (e.IsGetData == true)
            {
                e.Value = e.ListSourceRowIndex + 1;
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