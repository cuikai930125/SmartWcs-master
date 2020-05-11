using DevExpress.Xpf.Charts;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.Data.DataMembers;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.Optimization.DataMembers.OPT0103;
using SMART.WCS.UI.Optimization.DataMembers.OPT0105;
using SMART.WCS.UI.Optimization.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.Optimization.Views.QPS
{
    /// <summary>
    /// 셀 시뮬레이션 결과 조회
    /// </summary>
    public partial class O1005_QPS : UserControl
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
        /// List 변수 (선 그래프 객체)
        /// </summary>
        List<LineSeries2D> g_li2D;

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public O1005_QPS()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation"></param>
        public O1005_QPS(List<string> _liMenuNavigation)
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

                //  공통코드를 사용하지 않는 콤보박스 설정
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
        #region >> 셀 시뮬레이션 결과 리스트
        /// <summary>
        /// 셀 시뮬레이션 결과 리스트
        /// </summary>
        public static readonly DependencyProperty CellSimuResltLeftGridProperty
            = DependencyProperty.Register("CellSimuResltLeftGridList", typeof(ObservableCollection<CellSimuRsltList>), typeof(O1005_QPS)
                , new PropertyMetadata(new ObservableCollection<CellSimuRsltList>()));

        /// <summary>
        /// 셀 시뮬레이션 결과 리스트
        /// </summary>
        public ObservableCollection<CellSimuRsltList> CellSimuResltLeftGridList
        {
            get { return (ObservableCollection<CellSimuRsltList>)GetValue(CellSimuResltLeftGridProperty); }
            set { SetValue(CellSimuResltLeftGridProperty, value); }
        }
        #endregion

        #region >> 셀 유형 별 최대 점유 셀 수
        /// <summary>
        /// 셀 유형 별 최대 점유 셀 수
        /// </summary>
        public static readonly DependencyProperty CellMaxResltRightBottomGridProperty
            = DependencyProperty.Register("CellMaxResltRightBottomGridList", typeof(ObservableCollection<CellMaxRslt>), typeof(O1005_QPS)
                , new PropertyMetadata(new ObservableCollection<CellMaxRslt>()));

        /// <summary>
        /// 셀 유형 별 최대 점유 셀 수
        /// </summary>
        public ObservableCollection<CellMaxRslt> CellMaxResltRightBottomGridList
        {
            get { return (ObservableCollection<CellMaxRslt>)GetValue(CellMaxResltRightBottomGridProperty); }
            set { SetValue(CellMaxResltRightBottomGridProperty, value); }
        }
        #endregion

        #region >> 최적화 옵션 항목 컨트롤
        #region + 보충 배치 분할개수
        public static readonly DependencyProperty SUP_BTCH_DIV_CNTProperty
            = DependencyProperty.Register("SUP_BTCH_DIV_CNT", typeof(int), typeof(O1005_QPS), new PropertyMetadata(null));

        public int SUP_BTCH_DIV_CNT
        {
            get { return (int)GetValue(SUP_BTCH_DIV_CNTProperty); }
            set { SetValue(SUP_BTCH_DIV_CNTProperty, value); }
        }
        #endregion

        #region + 보충 SKU 최소단위
        public static readonly DependencyProperty MIN_SUP_SKU_UNITProperty
            = DependencyProperty.Register("MIN_SUP_SKU_UNIT", typeof(int), typeof(O1005_QPS), new PropertyMetadata(null));

        public int MIN_SUP_SKU_UNIT
        {
            get { return (int)GetValue(MIN_SUP_SKU_UNITProperty); }
            set { SetValue(MIN_SUP_SKU_UNITProperty, value); }
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
            // 콤보박스 바인딩
            this.BaseClass.BindingFirstComboBox(this.cboBtchInfo, "LOC_BTCH_INFO");         // 로케이션 배치관리
            this.BaseClass.BindingFirstComboBox(this.cboSkuCellType, "SKU_CEL_TYPE");       // SKU별 셀 유형

            this.chartXContent.Content  = this.BaseClass.GetResourceValue("BTCH_SEQ");          // 배치 순번 (X축 Title)
            this.chartYContent.Content  = this.BaseClass.GetResourceValue("OCCUP_CELL_CNT");    // 점유 셀 수 (Y축 Title)
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
            DataTable dtComboData = await Utility.HelperClass.GetSP_OPT_QPS_DATA_SET_LIST_INQ(strWrkPlanYmd);

            // 조회 데이터가 없는 경우 구문을 리턴한다.
            if (dtComboData.Rows.Count == 0)
            {
                // DATA 그룹 데이터가 없는 경우 (콤보박스 설정이 안되는 경우) 셀 시뮬레이션 버튼을 비활성화한다.
                this.BaseClass.SetSimpleButtonIsEnabled(this.btnCellSimul, false);
                this.cboDataSetID.ItemsSource       = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);
                this.cboRocOptSeq.ItemsSource       = null;
                this.cboCellOptSeq.ItemsSource      = null;

                return;
            }

            // 콤보박스 설정을 위해 컬럼명을 변경한다.
            dtComboData = this.BaseClass.ModifyComboBoxColumnHeaderName(dtComboData);

            // DATA 그룹 데이터가 1개 컬럼으로 조회되기 때문에 콤보박스 설정을 위해 컬럼을 추가 생성한 후 데이터를 복사한다.
            dtComboData.Columns.Add("NAME", typeof(string));

            foreach (DataRow drRow in dtComboData.Rows)
            {
                drRow["NAME"]   = drRow["CODE"];
            }

            this.cboDataSetID.ItemsSource = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);

            this.cboDataSetID.SelectedIndexChanged -= CboDataSetID_SelectedIndexChanged;
            this.cboDataSetID.SelectedIndex = 0;
            this.cboDataSetID.SelectedIndexChanged += CboDataSetID_SelectedIndexChanged;

            // DATA 그룹 데이터가 있는 경우 (콤보박스 설정이 된 경우) ROC 수행 버튼을 활성화한다.
            this.BaseClass.SetSimpleButtonIsEnabled(this.btnCellSimul, true);

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
            #region + Loaded 이벤트
            this.Loaded += OPT0105_Loaded;
            #endregion

            #region + 버튼 이벤트
            // 조회 버튼 클릭 이벤트
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;

            // 셀 시뮬레이션 버튼 클릭 이벤트
            this.btnCellSimul.PreviewMouseLeftButtonUp += BtnCellSimul_PreviewMouseLeftButtonUp;
            #endregion

            #region + 달력 컨트롤 이벤트
            this.deWrkPlanWmd.EditValueChanged += DeWrkPlanWmd_EditValueChanged;
            #endregion

            #region + 콤보박스 이벤트
            // 데이터 그룹 ID 값 변경 이벤트
            this.cboDataSetID.SelectedIndexChanged += CboDataSetID_SelectedIndexChanged;
            // ROC 최적화 차수 값 변경 이벤트
            this.cboRocOptSeq.SelectedIndexChanged += CboRocOptSeq_SelectedIndexChanged;
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
            var iTotalRowCount      = (this.gridLeft.ItemsSource as ICollection).Count;     // 전체 데이터 수
            var strResource         = this.BaseClass.GetResourceValue("DATA_INQ");          // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
        }
        #endregion

        #region MergeRows - linq join문 처리
        /// <summary>
        /// linq join문 처리
        /// </summary>
        /// <param name="_dtNewTable">신규 데이터테이블</param>
        /// <param name="_drOptCellQtyList">셀 시뮬레이션 리스트</param>
        /// <param name="_drOptCellQtyRslt">셀 시뮬레이션 결과</param>
        /// <returns></returns>
        private static DataRow MergeRows(DataTable _dtNewTable, DataRow _drOptCellQtyList, DataRow _drOptCellQtyRslt)
        {
            List<object> list = new List<object>();
            list.AddRange(_drOptCellQtyList.ItemArray);
            list.AddRange(_drOptCellQtyRslt.ItemArray);

            return _dtNewTable.LoadDataRow(list.ToArray(), false);
        }
        #endregion
        #endregion

        #region > 차트 바인딩
        #region >> BindingChart - 시뮬레이션 결과 차트
        /// <summary>
        /// 시뮬레이션 결과 차트
        /// </summary>
        /// <param name="_dtChartData">차트 데이터</param>
        private void BindingChart(DataTable _dtChartData)
        {
            try
            {
                if (this.xyDiagramRslt.Series.Count > 0) { this.xyDiagramRslt.Series.Clear();}

                this.chartSimulRslt.BeginInit();

                this.g_li2D         = new List<LineSeries2D>();

                // 셀타입코드 기준으로 그룹핑 처리
                // CELL_TYPE_CD는 범레로 사용한다.
                var query = _dtChartData.AsEnumerable().GroupBy(p => p.Field<string>("CELL_TYPE_CD")).ToList();

                foreach (var item in query)
                {
                    LineSeries2D ls2D                   = new LineSeries2D();
                    ls2D.DisplayName                    = item.Key;
                    ls2D.ArgumentScaleType              = ScaleType.Auto;
                    ls2D.DataSourceSorted               = false;
                    ls2D.LineStyle                      = new LineStyle(1);
                    ls2D.CrosshairLabelPattern          = "{S}: {A:0},{V:0}";
                    ls2D.ArgumentDataMember             = "Argument";
                    ls2D.ValueDataMember                = "Value";
                    ls2D.ShowInLegend                   = true;

                    List<ChartDataPointMember> points   = this.GetGenerateData(item.Key, _dtChartData);
                    ls2D.DataSource                     = points;
                    this.g_li2D.Add(ls2D);
                }

                foreach (var item in query)
                {
                    LineSeries2D ls2D                   = new LineSeries2D();
                    ls2D.DisplayName                    = item.Key;
                    ls2D.ArgumentScaleType              = ScaleType.Auto;
                    ls2D.DataSourceSorted               = false;
                    ls2D.LineStyle                      = new LineStyle(1);
                    ls2D.CrosshairLabelPattern          = string.Empty;
                    ls2D.ArgumentDataMember             = "Argument";
                    ls2D.ValueDataMember                = "Value";
                    ls2D.ShowInLegend                   = false;

                    List<ChartDataPointMember> points   = this.GetGenerateDataCell(item.Key, _dtChartData);
                    ls2D.DataSource                     = points;
                    this.g_li2D.Add(ls2D);
                }

                // 그래프 대상 객체 수를 2로 나눈 값이 범례의 개수다.
                int iLegendCount = (this.g_li2D.Count / 2);

                for (int i = 0; i < g_li2D.Count - iLegendCount; i++)
                {
                    this.g_li2D[i].Brush = this.BaseClass.DefineGraphColor(i);
                    this.xyDiagramRslt.Series.Add(g_li2D[i]);
                }

                for (int i = iLegendCount; i < this.g_li2D.Count; i++)
                {
                    this.g_li2D[i].Brush = this.BaseClass.DefineGraphColor(i - iLegendCount);
                    this.xyDiagramRslt.Series.Add(g_li2D[i]);
                }
            }
            catch { throw; }
            finally
            {   
                this.chartSimulRslt.EndInit();
                this.g_li2D = null;
            }
            
        }
        #endregion

        #region >> GetGenerateData - 최적화 셀 리스트 차트 데이터 구성 함수
        /// <summary>
        /// 최적화 셀 리스트 차트 데이터 구성 함수
        /// </summary>
        /// <param name="_strRegend">범례 데이터</param>
        /// <param name="_dtValue">차트 데이터</param>
        /// <returns></returns>
        private List<ChartDataPointMember> GetGenerateData(string _strRegend, DataTable _dtValue)
        {
            List<ChartDataPointMember> points = new List<ChartDataPointMember>();

            var query = _dtValue.AsEnumerable().Where(p => p.Field<string>("CELL_TYPE_CD").Equals(_strRegend) == true).ToList();
            DataTable dtFilterData = query.CopyToDataTable();
                
            foreach (DataRow drRow in dtFilterData.Rows)
            {
                double dArgument        = Convert.ToDouble(drRow["CELL_QTY"]);
                double dValue           = Convert.ToDouble(drRow["BTCH_SEQ"]);
                //double dValue           = Convert.ToDouble(drRow["ADD_SUPP_SKU_CNT"]);
                points.Add(new ChartDataPointMember(_strRegend, dValue, dArgument));
            }

            return points;
        }
        #endregion

        #region >> GetGenerateDataCell - 최적화 수량 결과 차트 데이터 구성 함수
        /// <summary>
        /// 최적화 수량 결과 차트 데이터 구성 함수
        /// </summary>
        /// <param name="_strRegend">범례 데이터</param>
        /// <param name="_dtValue">차트 데이터</param>
        /// <returns></returns>
        private List<ChartDataPointMember> GetGenerateDataCell(string _strRegend, DataTable _dtValue)
        { 
            List<ChartDataPointMember> points = new List<ChartDataPointMember>();

            var query = _dtValue.AsEnumerable().Where(p => p.Field<string>("CELL_TYPE_CD").Equals(_strRegend) == true).ToList();
            DataTable dtFilterData = query.CopyToDataTable();

            double dValue;
            double dArgument;
            int i = 0;

            foreach (DataRow drRow in dtFilterData.Rows)
            {
                if (i == 0)
                {
                    dArgument = 0;
                }
                else
                {
                    dArgument = Convert.ToDouble(dtFilterData.Rows[0]["END_CELL_MST_QTY"]);
                }

                dValue = Convert.ToDouble(dtFilterData.Rows[0]["SUPP_SKU_CNT_GRAPH"]); ;

                points.Add(new ChartDataPointMember(_strRegend, dArgument, dValue));

                if (i == 1) { break; }
                i++;
            }

            return points;
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_OPT_CELL_QTY_LIST_INQ - 셀 시뮬레이션 결과 리스트
        /// <summary>
        /// 셀 시뮬레이션 결과 리스트
        /// </summary>
        private async Task<DataSet> GetSP_OPT_CELL_QTY_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_O1005_QPS.SP_OPT_CELL_QTY_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_OPT_CELL_QTY_LIST", "O_RTN_RSLT" };

            var strCenterCD     = this.BaseClass.CenterCD;                                      // 센터코드
            var strWrkPlanYmd   = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);           // 출고일자
            var strDataSetID    = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);   // 데이터 그룹
            var iOptSeq         = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCellOptSeq);   // 최적화 차수
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCenterCD);       // 센터코드
            dicInputParam.Add("P_WRK_PLAN_YMD",     strWrkPlanYmd);     // 출고일자
            dicInputParam.Add("P_DATA_SET_ID",      strDataSetID);      // 데이터 그룹
            dicInputParam.Add("P_OPT_SEQ",          iOptSeq);           // 최적화 차수
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

            return dsRtnValue;
        }
        #endregion

        #region >> GetSP_OPT_CELL_QTY_RSLT_INQ - 최적화 결과 조회 (최대 셀수와 마스터의 물리 셀수 그래프 비교)
        /// <summary>
        /// 최적화 결과 조회 (최대 셀수와 마스터의 물리 셀수 그래프 비교)
        /// </summary>
        /// <returns></returns>
        private async Task<DataSet> GetSP_OPT_CELL_QTY_RSLT_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_O1005_QPS.SP_OPT_CELL_QTY_RSLT_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_OPT_CELL_QTY_RSLT", "O_OPT_CELL_MAX_RSLT", "O_RTN_RSLT" };

            var strCenterCD     = this.BaseClass.CenterCD;                                      // 센터코드
            var strDataSetID    = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);   // 데이터 그룹
            var iOptSeq         = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCellOptSeq);  // 셀 최적화 차수
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCenterCD);       // 센터코드
            dicInputParam.Add("P_DATA_SET_ID",      strDataSetID);      // 데이터 그룹
            dicInputParam.Add("P_OPT_SEQ",          iOptSeq);           // 최적화 차수
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

            return dsRtnValue;
        }
        #endregion

        #region >> SaveSP_CELL_SIMUL_OPT_SEQ_SAVE - 최적화 셀 시뮬레이션 처리 (저장)
        /// <summary>
        /// 최적화 셀 시뮬레이션 처리 (저장)
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_dicInputParam">Input 파라메터</param>
        /// <returns></returns>
        private async Task<DataSet> SaveSP_CELL_SIMUL_OPT_SEQ_SAVE(BaseDataAccess _da, Dictionary<string, object> _dicInputParam)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_O1005_QPS.SP_CELL_SIMUL_OPT_SEQ_SAVE";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_CEL_OPT_SEQ", "O_RTN_RSLT" };
            
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
        #region > Loaded 이벤트
        private void OPT0105_Loaded(object sender, RoutedEventArgs e)
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

        #region > 버튼 이벤트
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

                #region + 최적화 옵션 리스트 조회
                var strDataSetID            = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);       // 데이터 그룹 ID
                DataTable dtOptOptionList   = await HelperClass.GetSP_OPT_QPS_MAX_CELL_OPT_INQ("CELL_SIMUL", strDataSetID);

                if (dtOptOptionList.Rows.Count > 0)
                {
                    DataRow drRow = dtOptOptionList.Rows[0];
                    this.SUP_BTCH_DIV_CNT               = Convert.ToInt32(drRow["SUP_BTCH_DIV_CNT"]);   // 보충 배치 분할개수
                    this.MIN_SUP_SKU_UNIT               = Convert.ToInt32(drRow["MIN_SUP_SKU_UNIT"]);   // 보충 SKU 최소단위
                    this.cboBtchInfo.EditValue          = drRow["LOC_BTCH_INFO"].ToString();            // 로케이션 배치관리
                    this.cboSkuCellType.EditValue       = drRow["SKU_CEL_TYPE"].ToString();             // SKU별 셀 유형
                }
                else
                {
                    this.SUP_BTCH_DIV_CNT           = 0;    // 보충 배치 분할개수
                    this.MIN_SUP_SKU_UNIT           = 0;    // 보충 SKU 최소단위
                }

                #endregion

                var strErrCode          = string.Empty;     // 오류 코드
                var strErrMsg           = string.Empty;     // 오류 메세지

                // 좌측 그리드 정보
                DataSet dsOptCellQtyList = await this.GetSP_OPT_CELL_QTY_LIST_INQ();
                // 우측 차트 정보
                DataSet dsOptCellQtyRslt = await this.GetSP_OPT_CELL_QTY_RSLT_INQ();

                #region + 조회 데이터 오류 여부 체크
                if (this.BaseClass.CheckResultDataProcess(dsOptCellQtyList, ref strErrCode, ref strErrMsg) == false)
                {
                    // 오류가 발생한 경우
                    this.CellSimuResltLeftGridList.ToObservableCollection(null);
                    // ERR_INQ - 조회 중 오류가 발생했습니다.
                    this.BaseClass.MsgError("ERR_INQ");
                    return;
                }

                if (this.BaseClass.CheckResultDataProcess(dsOptCellQtyRslt, ref strErrCode, ref strErrMsg) == false)
                {
                    // 오류가 발생한 경우
                    this.CellMaxResltRightBottomGridList.ToObservableCollection(null);
                    // ERR_INQ - 조회 중 오류가 발생했습니다.
                    this.BaseClass.MsgError("ERR_INQ");
                    return;
                }
                #endregion

                #region + 좌측 그리드 - 셀 시뮬레이션 결과 리스트
                // 정상 처리된 경우
                this.CellSimuResltLeftGridList = new ObservableCollection<CellSimuRsltList>();
                // 오라클인 경우 TableName = O_CELL_LIST
                this.CellSimuResltLeftGridList.ToObservableCollection(dsOptCellQtyList.Tables[0]);

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText();
                #endregion

                #region + 우측 하단 그리드 - 셀 유형 별 최대 점유 셀 수
                // 정상 처리된 경우
                this.CellMaxResltRightBottomGridList = new ObservableCollection<CellMaxRslt>();
                this.CellMaxResltRightBottomGridList.ToObservableCollection(dsOptCellQtyRslt.Tables[0]);
                // 데이터 바인딩
                this.gridRightBottom.ItemsSource = this.CellMaxResltRightBottomGridList;
                #endregion

                #region + 우측 촤트 - 보충 진행에 따른 셀 시뮬레이션 결과 (차트)

                if (dsOptCellQtyList.Tables[0].Rows.Count == 0 || dsOptCellQtyRslt.Tables[1].Rows.Count == 0)
                {
                    if (this.xyDiagramRslt.Series.Count > 0) { this.xyDiagramRslt.Series.Clear(); }

                    return;
                }

                DataTable dtNewSchema = new DataTable();
                dtNewSchema.Columns.Add("BTCH_SEQ",                 typeof(int));
                dtNewSchema.Columns.Add("CELL_TYPE_CD",             typeof(string));
                dtNewSchema.Columns.Add("CELL_QTY",                 typeof(int));
                dtNewSchema.Columns.Add("SUPP_SKU_CNT",             typeof(int));
                dtNewSchema.Columns.Add("ADD_SUPP_SKU_CNT",         typeof(decimal));
                dtNewSchema.Columns.Add("STRT_CELL_MST_QTY",        typeof(int));
                dtNewSchema.Columns.Add("END_CELL_MST_QTY",         typeof(int));
                dtNewSchema.Columns.Add("CELL_TYPE_CD_MIL",         typeof(string));
                dtNewSchema.Columns.Add("CELL_QTY_GRAPH",           typeof(int));
                dtNewSchema.Columns.Add("SUPP_SKU_CNT_GRAPH",       typeof(int));

                #region ++ 셀 시뮬레이션 결과 리스트와 보충 진행에 따른 셀 시뮬레이션 결과를 Join 한다. (CELL_TYPE_CD 기준)
                var joinResult =    from p in dsOptCellQtyList.Tables[0].AsEnumerable()
                                    join q in dsOptCellQtyRslt.Tables[1].AsEnumerable()
                                        on p.Field<string>("CELL_TYPE_CD") equals q.Field<string>("CELL_TYPE_CD")
                                    select MergeRows(dtNewSchema, p, q);

                var dtResult = joinResult.ToList().CopyToDataTable();

                // 시뮬레이션 결과 차트 바인딩
                this.BindingChart(dtResult);
                #endregion
                #endregion
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        #region >> 셀 시뮬레이션 버튼 클릭 이벤트
        /// <summary>
        /// 셀 시뮬레이션 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnCellSimul_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.BaseClass.MsgQuestion("ASK_CELL_SIMUL");
            if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

            // 상태바 (아이콘) 실행
            this.loadingScreen.IsSplashScreenShown = true;
            DataSet dsRtnValue = null;

            string strWrkPlanYmd    = string.Empty;
            string strDataSetID     = string.Empty;
            int iRocOptSeq          = 0;

            try
            {   
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    Dictionary<string, object> dicInputParam = new Dictionary<string, object>();

                    #region 파라메터 변수 선언 및 값 할당
                    var strCenterCD         = this.BaseClass.CenterCD;                                                      // 센터코드
                    strWrkPlanYmd           = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);                           // 출고일자
                    strDataSetID            = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);                   // 데이터 그룹 ID
                    iRocOptSeq              = Convert.ToInt32(this.BaseClass.ComboBoxSelectedKeyValue(this.cboRocOptSeq));  // ROC 최적화 차수
                    var strCellOptSeq       = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCellOptSeq);                  // 셀최적화 차수
                    var iSupBtchDivCnt      = Convert.ToInt32(this.txtSupBtchDivCnt.Text.Trim());                           // 보충배치 분할개수
                    var iMinSupSkuUnit      = Convert.ToInt32(this.txtMinSupSkuUnit.Text.Trim());                           // 보충 SKU 최소단위
                    var strLocBtchInfo      = this.BaseClass.ComboBoxSelectedKeyValue(this.cboBtchInfo);                    // 구역간 배치정보
                    var strSkuCelType       = this.BaseClass.ComboBoxSelectedKeyValue(this.cboSkuCellType);                 // SKU별 셀유형
                    var strUserID           = this.BaseClass.UserID;                                                        // 사용자 ID
                    #endregion

                    #region Input 파라메터
                    dicInputParam.Add("P_CNTR_CD",                  strCenterCD);           // 센터코드
                    dicInputParam.Add("P_WRK_PLAN_YMD",             strWrkPlanYmd);         // 출고일자
                    dicInputParam.Add("P_DATA_SET_ID",              strDataSetID);          // 데이터 그룹 ID
                    dicInputParam.Add("P_ROC_OPT_SEQ",              iRocOptSeq);            // ROC 최적화 차수
                    dicInputParam.Add("P_SUP_BTCH_DIV_CNT",         iSupBtchDivCnt);        // 보충배치 분할개수
                    dicInputParam.Add("P_MIN_SUP_SKU_UNIT",         iMinSupSkuUnit);        // 보충 SKU 최소단위
                    dicInputParam.Add("P_LOC_BTCH_INFO",            strLocBtchInfo);        // 구역간 배치정보
                    dicInputParam.Add("P_SKU_CEL_TYPE",             strSkuCelType);         // SKU별 셀유형
                    dicInputParam.Add("P_USER_ID",                  strUserID);             // 사용자 ID
                    #endregion

                    // 최적화 셀 시뮬레이션 처리
                    dsRtnValue = await this.SaveSP_CELL_SIMUL_OPT_SEQ_SAVE(da, dicInputParam);

                    if (dsRtnValue != null)
                    {
                        if (dsRtnValue.Tables[1].Rows.Count > 0)
                        {
                            if (dsRtnValue.Tables[1].Rows[0]["CODE"].ToString().Equals("0") == false)
                            {
                                // 프로시저에서 오류 리턴시 오류 메세지를 출력한다.
                                var strErrMessage = dsRtnValue.Tables[1].Rows[0]["MSG"].ToString();
                                this.BaseClass.MsgError(strErrMessage);
                                return;
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

                if (CallJsonWebService.CallServiceCellSimul(strWrkPlanYmd, strDataSetID, strOptSeq, iRocOptSeq) == true)
                {   
                    await this.GetSP_OPT_CELL_QTY_LIST_INQ();
                }
                else
                {
                    // ERR_CELL_OPTI_PROC - 셀 시뮬레이션 처리중 오류가 발생했습니다.
                    BaseClass.MsgError("ERR_CELL_SIMUL_PROC");
                }
            }
            catch (Exception err)
            {
                // ERR_CELL_OPTI_PROC - 셀 시뮬레이션 처리중 오류가 발생했습니다.
                BaseClass.MsgError("ERR_CELL_SIMUL_PROC");
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = false;
            }
            #endregion


            // CMPT_CELL_SIMUL - 셀 시뮬레이션 처리가 완료되었습니다.
            this.BaseClass.MsgInfo("CMPT_CELL_SIMUL");
            this.CboRocOptSeq_SelectedIndexChanged(null, null);
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

        #region > 콤보박스 이벤트
        #region >> Data Set 콤보박스 변경 이벤트
        /// <summary>
        /// Data Set 콤보박스 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CboDataSetID_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            var strWrkPlanYmd       = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);       // 출고일자
            var iSelectedValue      = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);
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

        #region ROC 최적화 차수 콤보박스 변경 이벤트
        /// <summary>
        /// ROC 최적화 차수 콤보박스 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CboRocOptSeq_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            var strWrkPlanYmd           = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);                           // 출고일자
            var strSelectedDataSetID    = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);                   // 데이터 그룹 ID
            var iRocOptSeq              = Convert.ToInt32(this.BaseClass.ComboBoxSelectedKeyValue(this.cboRocOptSeq));  // ROC 최적화 코드
            DataTable dtComboData       = await Utility.HelperClass.GetSP_OPT_QPS_CELL_OPT_LIST_INQ(strWrkPlanYmd, strSelectedDataSetID, "CELL_SIMUL", iRocOptSeq);

            // 조회 데이터가 없는 경우 구문을 리턴한다.
            if (dtComboData.Rows.Count == 0)
            {
                this.cboCellOptSeq.ItemsSource = null;
                return;
            }

            dtComboData                         = this.BaseClass.ModifyComboBoxColumnHeaderName(dtComboData);
            this.cboCellOptSeq.ItemsSource      = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);
            this.cboCellOptSeq.SelectedIndex    = 0;
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
