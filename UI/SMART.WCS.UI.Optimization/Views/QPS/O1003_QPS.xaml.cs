using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Editors;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.UI.Optimization.DataMembers.OPT0103;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.Optimization.Views.QPS
{
    /// <summary>
    /// 최적화 결과
    /// </summary>
    public partial class O1003_QPS : UserControl, TabCloseInterface
    {
        #region ▩ Detegate 선언
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

        BackgroundWorker _backgroundWork;

        Thread g_threadLeftChart;

        Thread g_threadRightChart;

        PointSeries2D g_ps2D;

        LineSeries2D g_ls2D;

        SeriesPoint sp;

        DataSet g_dsLeftValue;

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        /// <summary>
        /// 오더 전체 수량
        /// </summary>
        private int g_iTotOrd = 0;
        #endregion

        private enum RSLT_FLTR_TYPE
        {
            RSLT = 0,   // 결과
            FLTR = 1    // 필터
        };

        #region ▩ 생성자
        public O1003_QPS()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public O1003_QPS(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                this._backgroundWork = new BackgroundWorker();
                this._backgroundWork.DoWork += _backgroundWork_DoWork;

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

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            // 달력 컨트롤 기본값 설정
            this.deWrkPlanWmd.DateTime  = DateTime.Now;

            this.txtOrd.Text            = "0";
            this.txtSku.Text            = "0";
            this.txtOrdConnCnt.Text     = "0";

            #region + Average 값 라벨 데이터
            #region ++ 결과 데이터 (Average)
            // 오더 Average
            this.lblRsltRight.Text = $"Avg = 0";
            // SKU Average
            this.lblRsltBottom.Text = $"Avg = 0";

            this.lblRsltRight.Visibility    = Visibility.Hidden;
            this.lblRsltBottom.Visibility   = Visibility.Hidden;
            #endregion

            #region ++ 필터 데이터 (Average)
            // 오더 Average
            this.lblFltrRight.Text = $"Avg = 0";
            // SKU Average
            this.lblFltrBottom.Text = $"Avg = 0";

            this.lblFltrRight.Visibility    = Visibility.Hidden;
            this.lblFltrBottom.Visibility   = Visibility.Hidden;
            #endregion
            #endregion

            #region + 공통 콤보박스 설정
            this.BaseClass.BindingFirstComboBox(this.cboDivCnt, "DIV_CNT");
            #endregion

            #region + 그래프 X, Y축 Content
            this.chartLeftXContent.Content      = this.BaseClass.GetResourceValue("SKU");       // SKU
            this.chartLeftYContent.Content      = this.BaseClass.GetResourceValue("ORD");       // 오더

            this.chartRightXContent.Content     = this.BaseClass.GetResourceValue("SKU");       // SKU
            this.chartRightYContent.Content     = this.BaseClass.GetResourceValue("ORD");       // 오더
            #endregion
        }
        #endregion

        #region >> RefreshControl - 데이터 조회 후 컨트롤 속성 및 데이터를 Refresh 한다..
        /// <summary>
        /// 데이터 조회 후 컨트롤 속성 및 데이터를 Refresh 한다..
        /// </summary>
        /// <param name="_enumRsltFltrType">결과, 필터 구분</param>
        private void RefreshControl(RSLT_FLTR_TYPE _enumRsltFltrType)
        {
            if (_enumRsltFltrType == RSLT_FLTR_TYPE.RSLT)
            {
                this.lblRsltRight.Visibility        = Visibility.Hidden;
                this.lblRsltBottom.Visibility       = Visibility.Hidden;
                
                this.chartRsltMain.BeginInit();
                this.pointRsltChart.Points.Clear();
                this.chartRsltMain.EndInit();

                this.chartRsltRight.BeginInit();
                this.xyDiagramRsltRight.Series.Clear();
                this.chartRsltRight.EndInit();

                this.chartRsltBottom.BeginInit();
                this.xyDiagramRsltBottom.Series.Clear();
                this.chartRsltBottom.EndInit();

                #region 결과 데이터 (Average)
                // 오더 Average
                this.lblRsltRight.Text = $"Avg = 0";
                // SKU Average
                this.lblRsltBottom.Text = $"Avg = 0";
                #endregion

                this.lblRsltIncludeOrd.Text     = string.Empty;
            }
            else if (_enumRsltFltrType == RSLT_FLTR_TYPE.FLTR)
            {
                this.lblFltrRight.Visibility        = Visibility.Hidden;
                this.lblFltrBottom.Visibility       = Visibility.Hidden;

                this.chartFltrtMain.BeginInit();
                this.pointFltrChart.Points.Clear();
                this.chartFltrtMain.EndInit();

                this.chartFltrRight.BeginInit();
                this.xyDiagramFltrRight.Series.Clear();
                this.chartFltrRight.EndInit();

                this.chartFltrBottom.BeginInit();
                this.xyDiagramFltrBottom.Series.Clear();
                this.chartFltrBottom.EndInit();

                #region 필터 데이터 (Average)
                // 오더 Average
                this.lblFltrRight.Text = $"Avg = 0";
                // SKU Average
                this.lblFltrBottom.Text = $"Avg = 0";
                #endregion
            }
        }
        #endregion

        #region >> InitComboBoxInfo - 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// <summary>
        /// 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// </summary>
        private async void InitComboBoxInfo()
        {
            #region ++ 공통코드 사용하지 않는 콤보박스 설정
            var strWrkPlanYmd       = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);       // 출고일자
            // (공통코드 사용하지 않음)
            DataTable dtComboData   = await Utility.HelperClass.GetSP_OPT_QPS_DATA_SET_LIST_INQ(strWrkPlanYmd);

            // 조회 데이터가 없는 경우 구문을 리턴한다.
            if (dtComboData.Rows.Count == 0)
            {
                this.cboDataSetID.ItemsSource = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);
                this.cboRocOptSeq.ItemsSource = null;

                return;
            }

            // 콤보박스 설정을 위해 컬럼명을 변경한다.
            dtComboData = this.BaseClass.ModifyComboBoxColumnHeaderName(dtComboData);

            // DATA 그룹 데이터가 1개 컬럼으로 조회되기 때문에 콤보박스 설정을 위해 컬럼을 추가 생성한 후 데이터를 복사한다.
            dtComboData.Columns.Add("NAME", typeof(string));

            for (int i = 0; i < dtComboData.Rows.Count; i++)
            {
                dtComboData.Rows[i]["NAME"] = dtComboData.Rows[i]["CODE"].ToString();
            }

            this.cboDataSetID.ItemsSource   = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);

            this.cboDataSetID.SelectedIndexChanged -= CboDataSetID_SelectedIndexChanged;
            this.cboDataSetID.SelectedIndex = 0;
            this.cboDataSetID.SelectedIndexChanged += CboDataSetID_SelectedIndexChanged;

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

            #region + 텍스트박스 이벤트
            // 오더 텍스트박스 클릭 시 데이터 전체 선택되도록 설정하는 이벤트
            this.txtOrd.GotFocus += TxtOrd_GotFocus;
            // SKU 텍스트박스 클릭 시 데이터 전체 선택되도록 설정하는 이벤트
            this.txtSku.GotFocus += TxtSku_GotFocus;
            // 오더 연결수 텍스트박스 클릭 시 데이터 전체 선택되도록 설정하는 이벤트
            this.txtOrdConnCnt.GotFocus += TxtOrdConnCnt_GotFocus;
            #endregion

            #region + 버튼 이벤트
            // 조회 버튼 클릭 이벤트
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드 버튼 클릭 이벤트
            this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;

            // 필터 버튼 클릭 이벤트
            this.btnFltr.PreviewMouseLeftButtonUp += BtnFltr_PreviewMouseLeftButtonUp;
            #endregion
        }
        #endregion
        #endregion

        private void CaptureChart()
        {
            try
            {
                ////var dPoitionLeft        = Convert.ToDouble(Canvas.GetLeft(this.gridAreaChartLeft));
                ////var dPoitionTop         = Convert.ToDouble(Canvas.GetTop(this.gridAreaChartLeft));

                //var p = this.txtDummyLeft.TranslatePoint(new System.Windows.Point(0, 0), this);
                //var dPoitionLeft        = p.X;
                //var dPoitionTop         = p.Y;

                //int iWidth = 1000; //Convert.ToInt32(this.gridAreaChartLeft.ActualWidth);      // 그리드 가로 영역 사이즈
                //int iHeight = 1000; //Convert.ToInt32(this.gridAreaChartLeft.ActualHeight);     // 그리드 세로 영역 사이즈

                ////int iWidth  = Canvas.Actural

                ////RenderTargetitmap bmp = new RenderTargetBitmap(iWidth, iHeight, 96, 96, PixelFormats.Pbgra32);

                ////bmp.Render(this.gridAreaChartLeft);

                ////using (Graphics gr = Graphics.FromImage(bmp))
                ////{
                ////}

                ////Bitmap bitmap = new Bitmap(iWidth, iHeight);
                ////Graphics g = Graphics.FromImage(bitmap);
                ////g.CopyFromScreen(new System.Drawing.Point((int)dPoitionLeft, (int)dPoitionTop), new System.Drawing.Point(0, 0), bitmap.Size);
                ////bitmap.Save("111", ImageFormat.Bmp);


                //using (Bitmap bmp = new Bitmap(iWidth, iHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                //{
                //    using (Graphics grp = Graphics.FromImage(bmp))
                //    {
                //        grp.CopyFromScreen((int)dPoitionLeft, (int)dPoitionTop, iWidth, iHeight, bmp.Size);
                //        bmp.Save("test.jpg", ImageFormat.Jpeg);
                //    }
                //}

                ////BitmapSource source = Capture.CutAreaToImage((int)dPoitionLeft, (int)dPoitionTop,  iWidth, iHeight);
                ////Capture.Save(source, false);
                ///
                //System.Windows.Point locationFromWindow     = this.gridAreaChartLeft.TranslatePoint(new System.Windows.Point(0, 0), this);
                //System.Windows.Point locationFromScreen     = this.gridAreaChartLeft.PointToScreen(locationFromWindow); 

                //UIElement container = VisualTreeHelper.GetParent(this.txtDummyLeft) as UIElement;
                //var p2 = this.txtDummyLeft.TranslatePoint(new System.Windows.Point(0, 0), container);


                //var p = this.txtDummyLeft.TranslatePoint(new System.Windows.Point(0, 0), this.gridAreaMainForm);
                //var p = this.TranslatePoint(new System.Windows.Point(0, 0), this.txtDummyLeft);

                var p = this.txtDummyLeft.TranslatePoint(new System.Windows.Point(0, 0), Application.Current.MainWindow);

                var dPoitionLeft = p.X;
                var dPoitionTop = p.Y;

                int iWidth = 1000; //Convert.ToInt32(this.gridAreaChartLeft.ActualWidth);      // 그리드 가로 영역 사이즈
                int iHeight = 1000; //Convert.ToInt32(this.gridAreaChartLeft.ActualHeight);     // 그리드 세로 영역 사이즈

                using (Bitmap bmp = new Bitmap(iWidth, iHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (Graphics grp = Graphics.FromImage(bmp))
                    {
                        grp.CopyFromScreen(((int)dPoitionLeft), ((int)dPoitionTop), 0, 0, bmp.Size);
                        //grp.CopyFromScreen((int)dPoitionLeft, (int)dPoitionTop, (int)dPoitionLeft, (int)dPoitionTop,
                        //new System.Drawing.Size(iWidth, iHeight),
                        //CopyPixelOperation.SourceCopy);
                        bmp.Save("test.jpg", ImageFormat.Jpeg);
                    }
                }

            }
            catch { throw; }
        }

        #region > TabClosing - 탭을 닫을 때 데이터 저장 여부 체크
        /// <summary>
        /// 탭을 닫을 때 데이터 저장 여부 체크
        /// </summary>
        /// <returns></returns>
        public bool TabClosing()
        {
            if (this.g_ls2D != null) { this.g_ls2D = null; }
            if (this.g_ps2D != null) { this.g_ps2D = null; }
            if (this.sp != null) { this.sp = null; }

            if (this.g_threadLeftChart != null && this.g_threadLeftChart.ThreadState != ThreadState.Aborted) { this.g_threadLeftChart.Abort(); }
            if (this.g_threadRightChart != null &&  this.g_threadRightChart.ThreadState != ThreadState.Aborted) { this.g_threadRightChart.Abort(); }


            this.g_threadLeftChart = null;
            this.g_threadRightChart = null;
            //System.GC.Collect();

            //this.BaseClass.Info("가비지 컬렉터 호출 후 : " + GC.GetTotalMemory(false).ToString());

            return true;
        }
        #endregion

        #region > 차트 바인딩
        #region >> 최적화 결과 차트 (좌측)
        #region BindingChartRsltMain - 최적화 결과 오더, SKU 차트 바인딩
        /// <summary>
        /// 최적화 오더, SKU 차트 바인딩
        /// </summary>
        /// <param name="_dsValue">결과 조회 데이터 셋</param>
        private void BindingChartRsltMain(DataSet _dsValue)
        {   
            try
            {
                this.chartRsltMain.BeginInit();
                this.g_ps2D                         = new PointSeries2D();
                this.g_ps2D.ArgumentScaleType       = ScaleType.Numerical;
                this.g_ps2D.DataSourceSorted        = true;
                this.g_ps2D.CrosshairLabelPattern   = "{S}: {V:0.0}";
                this.g_ps2D.ArgumentDataMember      = "Argument";
                this.g_ps2D.ValueDataMember         = "Value";

                List<DataPointMember> dataMemberRight = new List<DataPointMember>();

                this.pointRsltChart.Points.Clear();

                for (int i = 0; i < _dsValue.Tables[0].Rows.Count; i++)
                {
                    this.sp = new SeriesPoint(_dsValue.Tables[0].Rows[i]["OPT_SKU_SEQ"].ToString(), Convert.ToDouble(_dsValue.Tables[0].Rows[i]["OPT_ORD_SEQ"]));
                    this.pointRsltChart.Points.Add(this.sp);
                }
            }
            catch { throw; }
            finally
            {
                if (this.sp != null) { this.sp = null; }
                if (this.g_ps2D != null) { this.g_ps2D = null; }

                this.chartRsltMain.EndInit();
            }
        }
        #endregion

        #region BindingChartRsltRight - 우측 결과 오더 평균 차트 바인딩
        /// <summary>
        /// 우측 결과 오더 평균 차트 바인딩
        /// </summary>
        /// <param name="_dsValue">결과 조회 데이터 셋</param>
        private void BindingChartRsltRight(DataSet _dsValue)
        {
            try
            {
                this.chartRsltRight.BeginInit();
                this.g_ls2D = new LineSeries2D();
                this.g_ls2D.ArgumentScaleType       = ScaleType.Numerical;
                this.g_ls2D.DataSourceSorted        = true;
                this.g_ls2D.LineStyle               = new LineStyle(1);
                this.g_ls2D.CrosshairLabelPattern   = "{S}: {V:0.0}";
                this.g_ls2D.ArgumentDataMember      = "Argument";
                this.g_ls2D.ValueDataMember         = "Value";

                List<DataPointMember> dataMemberRight = new List<DataPointMember>();

                if (this.g_ls2D.DataSource != null)
                {
                    this.g_ls2D.DataSource = null;
                }

                for (int i = 0; i < _dsValue.Tables[3].Rows.Count; i++)
                {
                    double value = Convert.ToDouble(_dsValue.Tables[3].Rows[i]["OPT_ORD_LEN"]);
                    double delta = i + 1;
                    dataMemberRight.Add(new DataPointMember(delta, value));
                }

                this.g_ls2D.DataSource = dataMemberRight;

                if (this.xyDiagramRsltRight.Series.Count > 0) { this.xyDiagramRsltRight.Series.Clear(); }
                this.xyDiagramRsltRight.Series.Add(this.g_ls2D);
            }
            catch { throw; }
            finally
            {
                if (this.g_ls2D != null) { this.g_ls2D = null; }

                chartRsltRight.EndInit();
            }
        }
        #endregion

        #region BindingChartRsltBottom - 하단 결과 SKU 평균 차트 바인딩
        /// <summary>
        /// 하단 결과 SKU 평균 차트 바인딩
        /// </summary>
        /// <param name="_dsValue">결과 조회 데이터 셋</param>
        private void BindingChartRsltBottom(DataSet _dsValue)
        {
            try
            {
                this.chartRsltBottom.BeginInit();
                this.g_ls2D                         = new LineSeries2D();
                this.g_ls2D.ArgumentScaleType       = ScaleType.Auto;
                this.g_ls2D.DataSourceSorted        = true;
                this.g_ls2D.LineStyle               = new LineStyle(1);
                this.g_ls2D.CrosshairLabelPattern   = "{S}: {V:0.0}";
                this.g_ls2D.ArgumentDataMember      = "Argument";
                this.g_ls2D.ValueDataMember         = "Value";

                List<DataPointMember> points = new List<DataPointMember>();

                if (this.g_ls2D.DataSource != null)
                {
                    this.g_ls2D.DataSource = null;
                }

                for (int i = 0; i < _dsValue.Tables[2].Rows.Count; i++)
                {
                    double value = Convert.ToDouble(_dsValue.Tables[2].Rows[i]["OPT_SKU_LEN"]);
                    double delta = i + 1;
                    points.Add(new DataPointMember(delta, value));
                }

                this.g_ls2D.DataSource = points;

                if (this.xyDiagramRsltBottom.Series.Count > 0) { this.xyDiagramRsltBottom.Series.Clear(); }
                this.xyDiagramRsltBottom.Series.Add(this.g_ls2D);
            }
            catch { throw; }
            finally
            {
                if (this.g_ls2D != null) { this.g_ls2D = null; }
                this.chartRsltBottom.EndInit();
            }
        }
        #endregion
        #endregion

        #region >> 최적화 결과 차트 (우측)
        #region BindingChartFltrMain - 최적화 필터 오더, SKU 차트 바인딩
        /// <summary>
        /// 최적화 필터 오더, SKU 차트 바인딩
        /// </summary>
        /// <param name="_dsValue">결과 조회 데이터 셋</param>
        private void BindingChartFltrMain(DataSet _dsValue)
        {
            PointSeries2D seriesFltrMain = null;

            try
            {
                this.chartFltrtMain.BeginInit();
                seriesFltrMain                          = new PointSeries2D();
                seriesFltrMain.ArgumentScaleType        = ScaleType.Numerical;
                seriesFltrMain.DataSourceSorted         = true;
                seriesFltrMain.CrosshairLabelPattern    = "{S}: {V:0.0}";
                seriesFltrMain.ArgumentDataMember       = "Argument";
                seriesFltrMain.ValueDataMember          = "Value";

                //if (_dsValue.Tables[0].Rows.Count > 0)
                //{
                //    this.axisX2DRsltRight.Visible = true;
                //    this.axisY2DRsltRight.Visible = true;
                //}

                List<DataPointMember> dataMemberRight = new List<DataPointMember>();

                this.pointFltrChart.Points.Clear();

                for (int i = 0; i < _dsValue.Tables[0].Rows.Count; i++)
                {
                    SeriesPoint sp = new SeriesPoint(_dsValue.Tables[0].Rows[i]["OPT_SKU_SEQ"].ToString(), Convert.ToDouble(_dsValue.Tables[0].Rows[i]["OPT_ORD_SEQ"]));
                    this.pointFltrChart.Points.Add(sp);
                }
            }
            catch { throw; }
            finally
            {
                this.chartFltrtMain.EndInit();
            }
        }
        #endregion

        #region BindingChartFltrRight - 우측 필터 오더 평균 차트 바인딩
        /// <summary>
        /// 우측 필터 오더 평균 차트 바인딩
        /// </summary>
        /// <param name="_dsValue">결과 조회 데이터 셋</param>
        private void BindingChartFltrRight(DataSet _dsValue)
        {
            LineSeries2D seriesFltrRight    = null;

            try
            {
                this.chartFltrRight.BeginInit();
                seriesFltrRight                             = new LineSeries2D();
                seriesFltrRight.ArgumentScaleType           = ScaleType.Numerical;
                seriesFltrRight.DataSourceSorted            = true;
                seriesFltrRight.LineStyle                   = new LineStyle(1);
                seriesFltrRight.CrosshairLabelPattern       = "{S}: {V:0.0}";
                seriesFltrRight.ArgumentDataMember          = "Argument";
                seriesFltrRight.ValueDataMember             = "Value";

                //if (_dsValue.Tables[0].Rows.Count > 0)
                //{
                //    this.axisX2DRsltRight.Visible = true;
                //    this.axisY2DRsltRight.Visible = true;
                //}

                List<DataPointMember> dataMemberRight = new List<DataPointMember>();

                if (seriesFltrRight.DataSource != null)
                {
                    seriesFltrRight.DataSource = null;
                }

                for (int i = 0; i < _dsValue.Tables[3].Rows.Count; i++)
                {
                    double value = Convert.ToDouble(_dsValue.Tables[3].Rows[i]["OPT_ORD_LEN"]);
                    double delta = i + 1;
                    dataMemberRight.Add(new DataPointMember(delta, value));
                }

                seriesFltrRight.DataSource = dataMemberRight;
                if (this.xyDiagramFltrRight.Series.Count > 0) { this.xyDiagramFltrRight.Series.Clear(); }
                this.xyDiagramFltrRight.Series.Add(seriesFltrRight);
            }
            catch { throw; }
            finally
            {
                this.chartFltrRight.EndInit();

                if (seriesFltrRight != null) { seriesFltrRight = null; }
            }
        }
        #endregion

        #region BindingChartFltrBottom - 하단 필터 SKU 평균 차트 바인딩
        /// <summary>
        /// 하단 필터 SKU 평균 차트 바인딩
        /// </summary>
        /// <param name="_dsValue">결과 조회 데이터 셋</param>
        private void BindingChartFltrBottom(DataSet _dsValue)
        {
            LineSeries2D seriesFltrBottom       = null;

            try
            {
                this.chartFltrBottom.BeginInit();
                seriesFltrBottom                            = new LineSeries2D();
                seriesFltrBottom.ArgumentScaleType          = ScaleType.Auto;
                seriesFltrBottom.DataSourceSorted           = true;
                seriesFltrBottom.LineStyle                  = new LineStyle(1);
                seriesFltrBottom.CrosshairLabelPattern      = "{S}: {V:0.0}";
                seriesFltrBottom.ArgumentDataMember         = "Argument";
                seriesFltrBottom.ValueDataMember            = "Value";

                //if (_dsValue.Tables[0].Rows.Count > 0)
                //{
                //    this.axisX2DRsltBottom.Visible      = true;
                //    this.axisY2DRsltBottom.Visible      = true;
                //}

                List<DataPointMember> points = new List<DataPointMember>();

                if (seriesFltrBottom.DataSource != null)
                {
                    seriesFltrBottom.DataSource = null;
                }

                for (int i = 0; i < _dsValue.Tables[2].Rows.Count; i++)
                {
                    double value = Convert.ToDouble(_dsValue.Tables[2].Rows[i]["OPT_SKU_LEN"]);
                    double delta = i + 1;
                    points.Add(new DataPointMember(delta, value));
                }

                seriesFltrBottom.DataSource = points;

                if (this.xyDiagramFltrBottom.Series.Count > 0) { this.xyDiagramFltrBottom.Series.Clear(); }
                this.xyDiagramFltrBottom.Series.Add(seriesFltrBottom);
            }
            catch { throw; }
            finally
            {
                this.chartFltrBottom.EndInit();

                if (seriesFltrBottom != null) { seriesFltrBottom = null; }
            }
        }
        #endregion
        #endregion
        #endregion

        #region 최적화 정보 (좌측 차트)
        private void BindingChartLeft()
        {
            try
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    // 최적화 결과를 조회한다.
                    DataSet dsRtnValue = this.GetSP_OPT_RSLT_INQ();
                    this.g_dsLeftValue = dsRtnValue;

                    if (dsRtnValue == null) { return; }

                    var strErrCode = string.Empty;     // 오류 코드
                    var strErrMsg = string.Empty;     // 오류 메세지


                    if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == false)
                    {
                        this.BaseClass.MsgError(strErrMsg);
                    }
                    else
                    {
                        this.RefreshControl(RSLT_FLTR_TYPE.RSLT);
                        this.RefreshControl(RSLT_FLTR_TYPE.FLTR);

                        this.lblRsltRight.Visibility    = Visibility.Visible;
                        this.lblRsltBottom.Visibility   = Visibility.Visible;

                        // SKU, 오더 점 그래프 (차트)
                        this.BindingChartRsltMain(dsRtnValue);
                        // SKU LEN (차트 - 상단 우측)
                        this.BindingChartRsltRight(dsRtnValue);
                        // SKU LEN (차트 - 하단)
                        this.BindingChartRsltBottom(dsRtnValue);

                        #region 결과 데이터 (Average)
                        
                        this.lblRsltRight.Text      = $"Avg = {dsRtnValue.Tables[3].Rows[0]["AVG_ORD_LEN"].ToString()}";    // 오더 Average
                        this.lblRsltBottom.Text     = $"Avg = {dsRtnValue.Tables[2].Rows[0]["AVG_SKU_LEN"].ToString()}";    // SKU Average
                        #endregion

                        if (dsRtnValue.Tables[1].Rows.Count > 0)
                        {
                        // INCLD_ORD - 포함 오더
                        var strIncludeOrdLabel = this.BaseClass.GetResourceValue("INCLD_ORD");
                            this.g_iTotOrd = Convert.ToInt32(dsRtnValue.Tables[1].Rows[0][0]);
                            this.lblRsltIncludeOrd.Text = $"{strIncludeOrdLabel} : {this.g_iTotOrd} / {this.g_iTotOrd}";
                        }
                        else
                        {
                            this.lblRsltIncludeOrd.Text = string.Empty;
                        }
                    }
                });
            }
            catch { throw; }
        }
        #endregion

        private void BindingChartRight()
        {
            try
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    // 최적화 결과를 조회한다.
                    DataSet dsRtnValue  = this.GetSP_OPT_FLTR_INQ();

                    if (dsRtnValue == null) { return; }

                    var strErrCode          = string.Empty;     // 오류 코드
                    var strErrMsg           = string.Empty;     // 오류 메세지

                    if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == false)
                    {
                        // 오류 발생시 프로시저에서 리턴받은 메세지를 출력한다.
                        this.BaseClass.MsgError(strErrMsg);
                    }
                    else
                    {
                        if (dsRtnValue.Tables[0].Rows.Count == 0)
                        {
                            this.RefreshControl(RSLT_FLTR_TYPE.FLTR);
                        }
                        else
                        {
                            this.RefreshControl(RSLT_FLTR_TYPE.FLTR);

                            this.lblFltrRight.Visibility        = Visibility.Visible;
                            this.lblFltrBottom.Visibility       = Visibility.Visible;

                            #region + 결과 데이터 (차트)
                            #region ++ SKU, 오더 점 그래프 (차트)
                            this.BindingChartFltrMain(dsRtnValue);
                            #endregion

                            #region ++ SKU LEN (차트 - 상단 우측)
                            this.BindingChartFltrRight(dsRtnValue);
                            #endregion

                            #region ++ SKU LEN (차트 - 하단)
                            this.BindingChartFltrBottom(dsRtnValue);
                            #endregion
                            #endregion

                            #region 결과 데이터 (Average)
                            // 오더 Average
                            this.lblFltrRight.Text = $"Avg = {dsRtnValue.Tables[3].Rows[0]["AVG_ORD_LEN"].ToString()}";
                            // SKU Average
                            this.lblFltrBottom.Text = $"Avg = {dsRtnValue.Tables[2].Rows[0]["AVG_SKU_LEN"].ToString()}";
                            #endregion
                        }

                        if (dsRtnValue.Tables[1].Rows.Count > 0)
                        {
                            // INCLD_ORD - 포함 오더
                            var strIncludeOrdLabel          = this.BaseClass.GetResourceValue("INCLD_ORD");
                            var iCludeOrdCnt                = Convert.ToInt32(dsRtnValue.Tables[1].Rows[0][0]);
                            this.lblFltrIncludeOrd.Text     = $"{strIncludeOrdLabel} : {iCludeOrdCnt} / {this.g_iTotOrd}";
                        }
                        else
                        {
                            this.lblFltrIncludeOrd.Text     = string.Empty;
                        }
                    }
                });
            }
            catch { throw; }
        }

        #region > 데이터 관련
        #region >> GetSP_OPT_RSLT_INQ - 최적화 결과 조회
        /// <summary>
        /// 최적화 결과 조회
        /// </summary>
        /// <returns></returns>
        private DataSet GetSP_OPT_RSLT_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_O1003_QPS.SP_OPT_RSLT_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_OPT_RSLT_LIST", "O_OPT_RSLT_ORD_CNT", "O_SKU_LEN_LIST", "O_ORD_LEN_LIST", "O_RTN_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                                      // 센터코드
            var strWrkPlanYmd       = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);           // 출고일자
            var strDataSetID        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);   // 데이터 그룹
            var iOptSeq             = this.BaseClass.ComboBoxSelectedKeyValue(this.cboRocOptSeq);   // 최적화 순번
            var iDivCnt             = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDivCnt);      // 분할 개수
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_WRK_PLAN_YMD",         strWrkPlanYmd);     // 출고일자
            dicInputParam.Add("P_DATA_SET_ID",          strDataSetID);      // 데이터 그룹
            dicInputParam.Add("P_OPT_SEQ",              iOptSeq);           // 최적화 순번
            dicInputParam.Add("P_CNT",                  iDivCnt);           // 분할 개수
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

        #region >> GetSP_OPT_FLTR_INQ - 최적화 필터 조회
        private DataSet GetSP_OPT_FLTR_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_O1003_QPS.SP_OPT_FLTR_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_FLTR_RSLT_LIST", "O_FLTR_RSLT_ORD_CNT", "O_SKU_LEN_LIST", "O_ORD_LEN_LIST", "O_RTN_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                                      // 센터코드
            var strWrkPlanYmd       = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);           // 출고일자
            var strDataSetID        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);   // 데이터 그룹
            var iOptSeq             = this.BaseClass.ComboBoxSelectedKeyValue(this.cboRocOptSeq);   // 최적화 순번
            var iDivCnt             = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDivCnt);      // 분할 개수
            var iOptOrdLen          = this.txtOrd.Text.Trim().Length > 0 
                                        ? Convert.ToInt32(this.txtOrd.Text.Trim()) : this.txtOrd.Text.Trim().Length;    // 필터조건 (오더)
            var iOptSkuLen          = this.txtSku.Text.Trim().Length > 0
                                        ? Convert.ToInt32(this.txtSku.Text.Trim()) : this.txtSku.Text.Trim().Length;    // 필터조건 (SKU)
            var iOrdConnCnt         = this.txtOrdConnCnt.Text.Trim().Length > 0
                                        ? Convert.ToInt32(this.txtOrdConnCnt.Text.Trim()) : this.txtOrdConnCnt.Text.Trim().Length;  // 필터조건 (오더 연결수)
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_WRK_PLAN_YMD",         strWrkPlanYmd);     // 출고일자
            dicInputParam.Add("P_DATA_SET_ID",          strDataSetID);      // 데이터 그룹
            dicInputParam.Add("P_OPT_SEQ",              iOptSeq);           // 최적화 순번
            dicInputParam.Add("P_CNT",                  iDivCnt);           // 분할 개수
            dicInputParam.Add("P_OPT_ORD_LEN",          iOptOrdLen);        // 필터조건 (오더)
            dicInputParam.Add("P_OPT_SKU_LEN",          iOptSkuLen);           // 필터조건 (SKU)
            dicInputParam.Add("P_ORD_CONN_CNT",         iOrdConnCnt);       // 필터조건 (오더 접속수)
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

        #region >> SaveSP_OPT_FLTR_SAVE - 필터 적용 결과 저장
        /// <summary>
        /// 필터 적용 결과 저장
        /// </summary>
        /// <param name="_da"></param>
        /// <returns></returns>
        private async Task<bool> SaveSP_OPT_FLTR_SAVE(BaseDataAccess _da)
        {
            bool isRtnValue  = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_O1003_QPS.SP_OPT_FLTR_SAVE";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RTN_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                                      // 센터코드
            var strWrkPlanYmd       = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);           // 출고일자
            var strDataSetID        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);   // 데이터 그룹 ID
            var isFilterUseYN       = this.chkDataFilterUseYN.IsChecked == true ? "Y" : "N";        // 필터 사용여부
            var iOptSeq             = this.BaseClass.ComboBoxSelectedKeyValue(this.cboRocOptSeq);   // ROC 최적화 차수
            var iOptOrdLen          = this.txtOrd.Text.Trim().Length > 0 
                                        ? Convert.ToInt32(this.txtOrd.Text.Trim()) : this.txtOrd.Text.Trim().Length;    // 필터조건 (오더)
            var iOptSkuLen          = this.txtSku.Text.Trim().Length > 0
                                        ? Convert.ToInt32(this.txtSku.Text.Trim()) : this.txtSku.Text.Trim().Length;    // 필터조건 (SKU)
            var iOrdConnCnt         = this.txtOrdConnCnt.Text.Trim().Length > 0
                                        ? Convert.ToInt32(this.txtOrdConnCnt.Text.Trim()) : this.txtOrdConnCnt.Text.Trim().Length;  // 필터조건 (오더 연결수)
            var strUserID           = this.BaseClass.UserID;                                        // 사용자 ID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_WRK_PLAN_YMD",         strWrkPlanYmd);     // 출고일자
            dicInputParam.Add("P_DATA_SET_ID",          strDataSetID);      // 데이터 셋 ID
            dicInputParam.Add("P_OPT_SEQ",              iOptSeq);           // ROC 최적화 차수
            dicInputParam.Add("P_FLTR_USE_YN",          isFilterUseYN);     // 필터 사용여부
            dicInputParam.Add("P_OPT_ORD_LEN",          iOptOrdLen);        // 필터 조건 (오더)
            dicInputParam.Add("P_OPT_SKU_LEN",          iOptSkuLen);        // 필터 조건 (SKU)
            dicInputParam.Add("P_ORD_CONN_CNT",         iOrdConnCnt);       // 오더 접속수
            dicInputParam.Add("P_USER_ID",              strUserID);         // 사용자 ID
            #endregion

            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);
            
            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                        this.BaseClass.MsgError(strMessage);
                        isRtnValue  = false;
                    }
                }
                else
                {
                    // ERR_SAVE - 저장 중 오류가 발생했습니다.
                    this.BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }

            return isRtnValue;
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
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this._backgroundWork.RunWorkerAsync(1000);


                //this.g_threadLeftChart = new Thread(new ThreadStart(BindingChartLeft));
                //this.g_threadLeftChart.Start();


            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                this.loadingScreen.IsSplashScreenShown = false;

                this.txtOrd.Text                    = "0";
                this.txtSku.Text                    = "0";
                this.txtOrdConnCnt.Text             = "0";
                this.lblFltrIncludeOrd.Text         = string.Empty;
            }
        }
        #endregion

        #region >> 필터 버튼 클릭 이벤트
        /// <summary>
        /// 필터 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFltr_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.g_threadRightChart = new Thread(new ThreadStart(BindingChartRight));
                this.g_threadRightChart.Start();
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

        #region >> 저장 버튼 클릭 이벤트
        /// <summary>
        /// 엑셀 다운로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue     = false;

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        da.BeginTransaction();

                        //// 그래프 필터 적용 결과 저장
                        //isRtnValue = await this.SaveSP_OPT_FLTR_SAVE(da);

                        if (isRtnValue == true)
                        {
                            // 저장된 경우
                            da.CommitTransaction();

                            // CMPT_SAVE - 저장되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");

                            this.GetSP_OPT_RSLT_INQ();
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

                        // ERR_SAVE - 저장 중 오류가 발생했습니다.
                        this.BaseClass.MsgError("ERR_SAVE");
                        throw;
                    }
                    finally
                    {
                        // 상태바 (아이콘) 제거
                        this.loadingScreen.IsSplashScreenShown = false;
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

        #region > 체크박스 이벤트
        #region >> 데이터 필터링 사용 체크 이벤트
        private void ChkDataFilterUseYN_Checked(object sender, RoutedEventArgs e)
        {
            //this.gridFilterArea.Visibility = Visibility.Visible;
            //this.BaseClass.SetSimpleButtonIsEnabled(btnFltr, true);
        }
        #endregion

        #region >> 데이터 필터링 사용 체크해제 이벤트
        private void ChkDataFilterUseYN_Unchecked(object sender, RoutedEventArgs e)
        {
            //this.gridFilterArea.Visibility = Visibility.Hidden;
            //this.BaseClass.SetSimpleButtonIsEnabled(btnFltr, false);
        }
        #endregion
        #endregion

        #region > 텍스트박스 이벤트
        #region >> 오더 - 텍스트 박스 클릭시 값이 전체 선택되도록 설정하는 이벤트
        /// <summary>
        /// 오더 - 텍스트 박스 클릭시 값이 전체 선택되도록 설정하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtOrdConnCnt_GotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => ((TextEdit)sender).SelectAll()));
        }
        #endregion

        #region >> SKU - 텍스트 박스 클릭시 값이 전체 선택되도록 설정하는 이벤트
        /// <summary>
        /// SKU - 텍스트 박스 클릭시 값이 전체 선택되도록 설정하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtSku_GotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => ((TextEdit)sender).SelectAll()));
        }
        #endregion

        #region >> 오더 연결 수 - 텍스트 박스 클릭시 값이 전체 선택되도록 설정하는 이벤트
        /// <summary>
        /// 오더 연결 수 - 텍스트 박스 클릭시 값이 전체 선택되도록 설정하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtOrd_GotFocus(object sender, RoutedEventArgs e)
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
            var iSelectedValue      = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);   // 데이터 그룹 선택값
            DataTable dtComboData   = await Utility.HelperClass.GetSP_OPT_QPS_ROC_SEQ_LIST_INQ(strWrkPlanYmd, iSelectedValue);

            // 조회 데이터가 없는 경우 구문을 리턴한다.
            if (dtComboData.Rows.Count == 0)
            {
                this.cboRocOptSeq.ItemsSource = null;
                return;
            }

            dtComboData                     = this.BaseClass.ModifyComboBoxColumnHeaderName(dtComboData);
            this.cboRocOptSeq.ItemsSource   = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);
            this.cboRocOptSeq.SelectedIndex = 0;
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

        delegate void ThreadWork();

        private void _backgroundWork_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new ThreadWork(BindingChartLeft));
                
        }
    }
}
