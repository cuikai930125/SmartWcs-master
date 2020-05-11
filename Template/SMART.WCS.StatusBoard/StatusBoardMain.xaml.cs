using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Common.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMART.WCS.StatusBoard
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StatusBoardMain : Window, INotifyPropertyChanged
    {
        #region ▩ 전역변수
        /// <summary>
        /// Base클래스 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 Refresh 타이머
        /// </summary>
        private static System.Timers.Timer g_timerRefresh;

        /// <summary>
        /// 우측 하단 시계 타이머
        /// </summary>
        DispatcherTimer g_timerWatch = new DispatcherTimer();

        /// <summary>
        /// 현재 시간
        /// </summary>
        private string g_strCurrentTime = string.Empty;

        /// <summary>
        /// 조회 Thread
        /// </summary>
        Thread g_threadSearch;
        #endregion

        #region ▩ 생성자
        public StatusBoardMain()
        {
            InitializeComponent();

            try
            {
                InitializeComponent();

                //this.BaseClass.CenterCD = "463";
                this.g_strCurrentTime = DateTime.Today.ToString("yyyyMMdd");
                //this.strToday = "20190501";

                //this.g_threadSearch = new Thread(new ThreadStart(WorkMonSearch));
                //this.g_threadSearch.Start();

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 관련 초기화
                this.InitEvent();

                //GetSP_CHUTE_RSLT_LIST_INQ();
                WorkMonSearch();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private DataMembers.StatusBoard _FIRST;
        public DataMembers.StatusBoard FIRST
        {
            get { return this._FIRST; }
            set { this._FIRST= value; OnPropertyChanged("FIRST"); }
        }

        private DataMembers.StatusBoard _SECOND;
        public DataMembers.StatusBoard SECOND
        {
            get { return this._SECOND; }
            set { this._SECOND = value; OnPropertyChanged("SECOND"); }
        }

        private DataMembers.StatusBoard _TOTAL;
        public DataMembers.StatusBoard TOTAL
        {
            get { return this._TOTAL; }
            set { this._TOTAL = value; OnPropertyChanged("TOTAL"); }
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
            try
            {
                // 리프레스 체크박스 초기 설정
                this.chkRefresh.IsChecked = true;

                // 리프레시 주기 초기 설정
                g_timerRefresh = new System.Timers.Timer
                {
                    Interval = 10000,
                    AutoReset = true,
                    Enabled = true
                };
                g_timerRefresh.Start();

                // 시계
                this.g_timerWatch.IsEnabled = true;
                this.g_timerWatch.Interval = TimeSpan.FromSeconds(1);
                this.g_timerWatch.Tick += G_Timer_Tick;
                this.g_timerWatch.Start();
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
                throw;
            }
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 
        /// </summary>
        private void InitEvent()
        {
            try
            {
                // 폼 로드 이벤트
                this.Loaded += StatusBoard_Loaded; ;

                // 닫기 버튼 클릭 이벤트
                this.btnFormClose.Click += BtnFormClose_Click;
                // 최소화 버튼 클릭 이벤트
                this.btnFormMinimize.Click += BtnFormMinimize_Click;
                // 체크박스 클릭 이벤트
                this.chkRefresh.Click += ChkRefresh_Click;

                // 화면 리프레시 이벤트
                g_timerRefresh.Elapsed += G_timerRefresh_Elapsed; ;
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
                throw;
            }
        }

        private void StatusBoard_Loaded(object sender, RoutedEventArgs e)
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
        #endregion

        #region > 기타함수
        #region >> G_Timer_Tick - 시계 설정
        /// <summary>
        /// 시계 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void G_Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.lblClock.Text = DateTime.Now.ToString("yyyy-MM-dd (ddd) HH:mm:ss");
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> g_timerRefresh_Elapsed - 리프레시 주기 경과 이벤트
        /// <summary>
        /// 화면 리프레시 주기 경과 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void G_timerRefresh_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //g_timerRefresh.Stop();
                //this.g_threadSearch = new Thread(new ThreadStart(WorkMonSearch));
                //this.g_threadSearch.Start();
                //g_timerRefresh.Start();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 폼 로드 이벤트
        private void W3002_Loaded(object sender, RoutedEventArgs e)
        {
            //MaximixeToFirstMonitor(this);
            MaximixeToSecondMonitor(this);
        }

        public void MaximixeToFirstMonitor(Window window)
        {
            var secondaryScreen = Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();

            if (secondaryScreen != null)
            {
                MaximizeWindow(window, secondaryScreen);
            }
        }

        public void MaximixeToSecondMonitor(Window window)
        {
            var secondaryScreen = Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

            if (secondaryScreen != null)
            {
                MaximizeWindow(window, secondaryScreen);
            }
        }

        private void MaximizeWindow(Window window, Screen secondaryScreen)
        {
            if (!window.IsLoaded)
                window.WindowStartupLocation = WindowStartupLocation.Manual;

            var workingArea = secondaryScreen.WorkingArea;
            window.Left = workingArea.Left;
            window.Top = workingArea.Top;
            window.Width = workingArea.Width;
            window.Height = workingArea.Height;

            if (window.IsLoaded)
                window.WindowState = WindowState.Maximized;
        }
        #endregion

        #region > 클릭 이벤트
        #region >> ChkRefresh_Click - 리프레시 주기 체크박스 클릭 이벤트
        /// <summary>
        /// 리프레시 주기 체크박스 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (this.chkRefresh.IsChecked == true)
            {
                this.g_threadSearch = new Thread(new ThreadStart(WorkMonSearch));
                this.g_threadSearch.Start();

                g_timerRefresh = new System.Timers.Timer
                {
                    Interval = 10000,
                    AutoReset = true,
                    Enabled = true
                };
                g_timerRefresh.Start();
                g_timerRefresh.Elapsed += G_timerRefresh_Elapsed;
            }

            else if (this.chkRefresh.IsChecked == false)
            {
                g_timerRefresh.Stop();
                g_timerRefresh.Elapsed -= G_timerRefresh_Elapsed;
            }
        }
        #endregion

        #region >> BtnFormMinimize_Click - 창 최소화 버튼 클릭 이벤트
        /// <summary>
        /// 창 최소화 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFormMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion

        #region >> BtnFormClose_Click - 창닫기 버튼 클릭 이벤트
        /// <summary>
        /// 창닫기 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFormClose_Click(object sender, RoutedEventArgs e)
        {
            this.g_timerWatch.Stop();
            g_timerRefresh.Stop();

            this.Close();
        }
        #endregion
        #endregion

        private DataSet GetSP_CHUTE_RSLT_LIST_INQ()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_D1001_SRT.SP_CHUTE_RSLT_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_CHUTE_RSLT_LIST", "O_RSLT" };
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


        #region > 조회 이벤트
        /// <summary>
        /// 조회 이벤트
        /// </summary>
        private void WorkMonSearch()
        {
            try
            {
                //this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                //{
                DataSet dsRtnValue = this.GetSP_CHUTE_RSLT_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    foreach (DataRow drRow in dsRtnValue.Tables[0].Rows)
                    {
                        switch(drRow["ZONE_ID"].ToString())
                        {
                            #region > 1호기
                            case "1":
                                this.FIRST = new DataMembers.StatusBoard
                                {
                                    ZONE_NM             = drRow["ZONE_NM"].ToString(),                  // 1호기 라벨
                                    NML_RSLT_QTY        = Convert.ToInt32(drRow["NML_RSLT_QTY"]),       // 1호기 정상수량
                                    ERR_QTY             = Convert.ToInt32(drRow["ERR_QTY"]),            // 1호기 오류수량
                                    TOT_RSLT_QTY        = Convert.ToInt32(drRow["TOT_RSLT_QTY"]),       // 1호기 합계
                                    RT_TOT_QTY          = Convert.ToDecimal(drRow["RT_TOT_QTY"])        // 1호기 정상분류율
                                };
                                break;
                                #endregion

                                #region > 2호기
                            case "2":
                                this.SECOND = new DataMembers.StatusBoard
                                {
                                    ZONE_NM             = drRow["ZONE_NM"].ToString(),                  // 1호기 라벨
                                    NML_RSLT_QTY        = Convert.ToInt32(drRow["NML_RSLT_QTY"]),       // 1호기 정상수량
                                    ERR_QTY             = Convert.ToInt32(drRow["ERR_QTY"]),            // 1호기 오류수량
                                    TOT_RSLT_QTY        = Convert.ToInt32(drRow["TOT_RSLT_QTY"]),       // 1호기 합계
                                    RT_TOT_QTY          = Convert.ToDecimal(drRow["RT_TOT_QTY"])        // 1호기 정상분류율
                                };
                                break;
                            #endregion

                            default: break;
                        }
                    }

                    #region > 전체 (합계)
                    var strZoneName     = "전체";
                    var iNmlRsltQty     = this.FIRST.NML_RSLT_QTY + this.SECOND.NML_RSLT_QTY;
                    var iErrorQty       = this.FIRST.ERR_QTY + this.SECOND.ERR_QTY;
                    var iTotRsltQty     = this.FIRST.TOT_RSLT_QTY + this.SECOND.TOT_RSLT_QTY;
                    var dRtTotQty       = this.FIRST.RT_TOT_QTY + this.SECOND.RT_TOT_QTY;

                    this.TOTAL  = new DataMembers.StatusBoard
                    {
                        ZONE_NM         = strZoneName,
                        NML_RSLT_QTY    = iNmlRsltQty,
                        ERR_QTY         = iErrorQty,
                        TOT_RSLT_QTY    = iTotRsltQty,
                        RT_TOT_QTY      = dRtTotQty
                    };
                    #endregion

                    //        
                    //    }
                    //    else
                    //    {
                    //        // 오류가 발생한 경우
                    //        SMART.WCS.StatusBoard.Utility.HelperClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                    //    }
                    ////});
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 키보드 이벤트
        /// <summary>
        /// 키보드 입력 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
            {
                this.Close();
            }
        }
        #endregion
        #endregion
    }
}
