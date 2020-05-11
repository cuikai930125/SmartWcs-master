using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.DataMembers;
using SMART.WCS.Main.DataModels;
using SMART.WCS.UI.COMMON.DataMembers.C1007;
using SMART.WCS.UI.COMMON.Views.SYS_MGMT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SMART.WCS.Main
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ▩ dll 선언
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// 공통함수 클래스
        /// </summary>
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 메인화면 상단 메뉴 컨트롤
        /// </summary>
        TextBlock g_tbTopMenuCtrl;

        /// <summary>
        /// 자식창에서 부모창 화면 오픈을 위해 전달한 값을 저장하는 매개변수
        /// </summary>
        private MainWinParam MainWinParam = new MainWinParam();

        /// <summary>
        /// Timer - 시계
        /// </summary>
        private DispatcherTimer g_timerClock = new DispatcherTimer();

        /// <summary>
        /// Timer - 인터넷 연결 상태
        /// </summary>
        private DispatcherTimer g_timerConnectionStatus = new DispatcherTimer();

        /// <summary>
        /// Timer - 세션 체크용
        /// </summary>
        private DispatcherTimer g_timerSessionCheck = new DispatcherTimer();

        /// <summary>
        /// 서버, 로컬 프로그램 버전 Display
        /// </summary>
        private DispatcherTimer g_timerVersionInfo = new DispatcherTimer();

        /// <summary>
        /// 모니터 화면 정보
        /// </summary>
        System.Windows.Forms.Screen[] g_ctrlScreens = System.Windows.Forms.Screen.AllScreens;

        private List<string> g_liMenuNavigation = new List<string>();

        /// <summary>
        /// 메뉴 정보 및 사용자 정보
        /// </summary>
        private DataSet g_dsMenuInfo = new DataSet();

        private DataTable g_dtOtherProgOpen = null;

        /// <summary>
        /// 상위 메뉴 ID
        /// </summary>
        private string g_strUpperMenuID = string.Empty;

        /// <summary>
        /// 화면 경로
        /// </summary>
        private string g_strMenuUrl = string.Empty;

        /// <summary>
        /// 선택된 상단 메뉴 ID (화면을 호출할 때 url 구성에 필요)
        /// </summary>
        private string g_strSelectedTopMenuID = string.Empty;

        private string g_strSelectedTopMenuName = string.Empty;

        /// <summary>
        /// 상단 컨트롤 (동적 생성) 테그명
        /// </summary>
        private string g_strTopMenuCtrlTagName = string.Empty;

        /// <summary>
        /// 공지 정보
        /// </summary>
        private DataTable g_dtNoticeInfo = new DataTable();

        /// <summary>
        /// 현재 선택된 TreeView Row 포커스 인덱스
        /// </summary>
        private int g_iTreeViewFocusedRowIndex = -1;
        #endregion

        #region ▩ 속성
        /// <summary>
        /// 탭 컨트롤 정보
        /// </summary>
        public ObservableCollection<DataModels.MainWindowDataModel> TabControlItems { get; set; }
        #endregion





        #region ▩ 생성자
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            if (System.Windows.Media.RenderCapability.Tier == 0)
            {
                var hwndSource = PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource;

                if (hwndSource != null)
                {
                    hwndSource.CompositionTarget.RenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
                }
            }

            base.OnSourceInitialized(e);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_dtMenuInfo">메뉴 정보</param>
        public MainWindow(DataSet _dsMainInfo, DataTable _dtNoticeInfo)
        {
            InitializeComponent();

            try
            {
                var imgUrlCoupang   = "/SMART.WCS.Resource;component/Image/CoupangMainLogo.png";
                var imgUrlLgcns     = "/SMART.WCS.Resource;component/Image/logo_lgcns_large.png";

                // 메인 이미지 상단 로고를 설정한다.
                if (this.BaseClass.GetAppSettings("ImageLogoKind").Equals("CUST") == true)
                {
                    this.BaseClass.ImageSource(this.imgLogo, imgUrlCoupang);
                    this.imgLogo.Width = 100;
                }
                else
                {   
                    this.BaseClass.ImageSource(this.imgLogo, imgUrlLgcns);
                    this.imgLogo.Height = 25;
                }

                // 다른창 오픈 대상 프로그램 정보를 저장하는 테이블 스키마를 복사한다.
                if (this.g_dtOtherProgOpen == null) { this.g_dtOtherProgOpen = new DataTable(); }
                var iRowCount = _dsMainInfo.Tables[0].AsEnumerable().Where(p => new[] { "C1006", "R1005_GAN", "R1007_GAN", "R1001_SRT" }.Contains(p.Field<string>("MENU_ID"))).ToList();
                if (iRowCount.Count() > 0)
                {
                    this.g_dtOtherProgOpen = _dsMainInfo.Tables[0].AsEnumerable().Where(p => new[] { "C1006", "R1005_GAN", "R1007_GAN", "R1001_SRT" }.Contains(p.Field<string>("MENU_ID"))).ToList().CopyToDataTable();
                }

                // 메인 화면 상단 메뉴 순서가 변경되는 경우 체크해야함.
                this.g_strTopMenuCtrlTagName = "COM";

                // 컨트롤 속성을 초기화한다.
                this.InitControl();

                // 데이터를 초기화한다.
                this.InitValue(_dsMainInfo, _dtNoticeInfo);

                // 메뉴 코드와 일치하는 리소스 데이터를 적용한다.
                this.InitResourceByLanguage();

                // 이벤트를 초기화한다.
                this.InitEvent();

                // 상단 메뉴 데이터 설정 및 컨트롤 바인딩
                this.BindingMainTopMenu();

                // 메인 화면 오픈 시 새로운 공지사항 조회
                this.BindingNoticeAlarm(_dtNoticeInfo);

                // (최초) 메인 화면 오픈 시 좌측 트리 메뉴 데이터 설정 및 컨트롤 바인딩
                this.BindingFirstLeftTreeControlList();

                // 탭 컨트롤 정보를 저장한다.
                this.TabControlItems = new ObservableCollection<DataModels.MainWindowDataModel>();

                // 로그 파일 삭제 (한달 기준으로 삭제)
                this.CheckLogDateByFileDelete();

                //공지정보를 설정
                SetNoticeInfo(this.g_dtNoticeInfo);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > 메뉴 리스트 (트리 리스트 컨트롤)
        public static readonly DependencyProperty MenuListProperty
            = DependencyProperty.Register("MenuList", typeof(ObservableCollection<MainWindowDataModel>), typeof(MainWindow)
                , new PropertyMetadata(new ObservableCollection<MainWindowDataModel>()));

        public ObservableCollection<MainWindowDataModel> MenuList
        {
            get { return (ObservableCollection<MainWindowDataModel>)GetValue(MenuListProperty); }
            set { SetValue(MenuListProperty, value); }
        }
        #endregion
        #endregion

        #region ▩ 함수
        #region > InitControl - 컨트롤 초기화 및 속성값을 정의한다.
        /// <summary>
        /// 컨트롤 초기화 및 속성값을 정의한다.
        /// </summary>
        private void InitControl()
        {
            // 트리 리스트 속성을 설정한다.
            this.treeListView.ShowColumnHeaders = false;
            this.treeListView.AutoWidth = true;
            this.treeListView.ShowHorizontalLines = false;
            this.treeListView.ShowVerticalLines = false;

            /// 메뉴 접기 버튼을 숨긴다.
            this.BtnMenuExp.Visibility = Visibility.Hidden;

            // 모든 탭 종료
            this.menuItemTabAllClose.Header     = this.BaseClass.GetResourceValue("CLOSE_ALL_TAB");
        }
        #endregion

        #region > InitValue - 데이터를 초기화한다.
        /// <summary>
        /// 데이터를 초기화한다.
        /// </summary>
        /// <param name="_dsMenuInfo">메뉴 정보 및 사용자 정보</param>
        /// <param name="_dtNoticeInfo">공지 정보</param>
        private void InitValue(DataSet _dsMenuInfo, DataTable _dtNoticeInfo)
        {
            try
            {
                this.g_dsMenuInfo       = _dsMenuInfo;          // 메뉴 정보 및 사용자 정보
                this.g_dtNoticeInfo     = _dtNoticeInfo;        // 공지 정보

                if (_dsMenuInfo.Tables.Count == 3)
                {
                    if (_dsMenuInfo.Tables[1].Rows.Count > 0)
                    {
                        this.lblUserName.Text   = _dsMenuInfo.Tables[1].Rows[0]["USER_NM"].ToString();
                        this.lblCenterName.Text = _dsMenuInfo.Tables[1].Rows[0]["CNTR_NM"].ToString();

                        this.BaseClass.UserName = this.lblUserName.Text.Trim();
                    }
                }
            }
            catch { throw; }
        }
        #endregion

        #region InitResourceByLanguage - 메뉴 코드와 일치하는 리소스 데이터를 적용한다.
        /// <summary>
        /// 메뉴 코드와 일치하는 리소스 데이터를 적용한다.
        /// </summary>
        private void InitResourceByLanguage()
        {
            foreach (DataRow dr in this.g_dsMenuInfo.Tables[0].Rows)
            {
                // CHOO_POINT 
                // 2020-04-21 / 추성호 / 상단 메뉴명 다국어 등록 확인 후 변경
                dr["LANG_CD"] = this.BaseClass.GetResourceValue($"(MENU){dr["MENU_ID"].ToString()}");
                //dr["MENU_NM"] = this.BaseClass.GetResourceValue($"(MENU){dr["MENU_ID"].ToString()}");
            }

            this.g_dsMenuInfo.Tables[0].AcceptChanges();
        }
        #endregion

        #region InitEvent - 이벤트를 초기화한다.
        /// <summary>
        /// 이벤트를 초기화한다.
        /// </summary>
        private void InitEvent()
        {
            try
            {
                this.Closed += MainWindow_Closed;

                this.menuItemTabAllClose.PreviewMouseLeftButtonUp += MenuItemTabAllClose_PreviewMouseLeftButtonUp;

                #region 타이머 객체
                // 시계
                this.g_timerClock.Interval = TimeSpan.FromSeconds(1);
                this.g_timerClock.Tick += TimerClock_Tick;
                this.g_timerClock.Start();

                // 인터넷 연결 상태
                this.g_timerConnectionStatus.Interval = TimeSpan.FromSeconds(1);
                this.g_timerConnectionStatus.Tick += TimerConnectionStatus_Tick;
                this.g_timerConnectionStatus.Start();

                // 세션 체크용 (30분)
                this.g_timerSessionCheck.Interval = TimeSpan.FromSeconds(1800);
                this.g_timerSessionCheck.Tick += timerSessionCheck_Tick;
                this.g_timerSessionCheck.Start();

                // 서버, 로컬 프로그램 버전 Display
                this.g_timerVersionInfo.Interval = TimeSpan.FromMinutes(60);
                this.g_timerVersionInfo.Tick += timerVersionInfo_Tick;

#if DEBUG
#else
                this.g_timerVersionInfo.Start();
#endif
                #endregion

                this.PreviewMouseDown += MainWindow_PreviewMouseDown;
            }
            catch { throw; }
        }
        #endregion

        #region InitTabVisibled - 탭 컨트롤의 숨김 여부를 설정한다.
        /// <summary>
        /// 탭 컨트롤의 숨김 여부를 설정한다.
        /// </summary>
        /// <param name="_bVisiabled">숨김 여부 (true:Visible, false:Hidden)</param>
        private void InitTabVisiabled(bool _bVisiabled)
        {
            try
            {
                if (_bVisiabled == true)
                {
                    this.tabControl.Visibility = Visibility.Visible;
                }
                else
                {
                    this.tabControl.Visibility = Visibility.Hidden;
                }
            }
            catch { throw; }
        }
        #endregion

        #region RunAnimate - 좌측 메뉴 (트리 컨트롤)의 위치를 조절한다.
        /// <summary>
        /// 좌측 메뉴 (트리 컨트롤)의 위치를 조절한다.
        /// </summary>
        /// <param name="targetWidth"></param>
        private void RunAnimate(double targetWidth)
        {
            CubicEase easing = new CubicEase();
            easing.EasingMode = EasingMode.EaseInOut;
            DoubleAnimation slideAnimation = new DoubleAnimation();
            slideAnimation.From = this.layoutGrpTree.ActualWidth;
            slideAnimation.To = targetWidth;
            slideAnimation.EasingFunction = easing;
            slideAnimation.Duration = TimeSpan.FromSeconds(0.5);
            slideAnimation.FillBehavior = FillBehavior.Stop;

            EventHandler eventHandler = null;
            eventHandler = new EventHandler((d, e) =>
            {
                slideAnimation.Completed -= eventHandler;
                this.layoutGrpTree.Width = targetWidth;
            });

            slideAnimation.Completed += eventHandler;
            this.layoutGrpTree.BeginAnimation(LayoutGroup.WidthProperty, slideAnimation);


        }
        #endregion

        #region GetTreeListControlSelectedValue - 선택한 트리 컨트롤의 값을 가져온다.
        /// <summary>
        /// 선택한 트리 컨트롤의 값을 가져온다.
        /// </summary>
        /// <param name="_strColumnName">트리메뉴 컬럼명 (필드명)</param>
        /// <returns></returns>
        private object GetTreeListControlSelectedValue(string _strColumnName)
        {
            try
            {
                // 현재 선택된 트리 컨트롤의 노드 값을 가져온다.
                return this.treeListControl.View.GetNodeValue(this.treeListControl.View.FocusedNode, _strColumnName);
            }
            catch { throw; }
        }
        #endregion

        #region SetMenuAttributeValue - 레벨에 따른 메뉴명을 메뉴 속성에 저장한다.
        /// <summary>
        /// 레벨에 따른 메뉴명을 메뉴 속성에 저장한다. - 재귀함수
        /// </summary>
        /// <param name="_strMenuName">메뉴명</param>
        /// <param name="_iMenuLevel">메뉴 레벨</param>
        /// <param name="_bFirst">함수 처음 실행 여부</param>
        private void SetMenuAttributeValue(string _strMenuName, int _iMenuLevel, bool _bFirst)
        {
            try
            {
                // 함수가 최초 실행하는 경우
                if (_bFirst)
                {
                    // 재귀호출로 함수를 실행하는 경우

                    // 상위 메뉴 ID
                    this.g_strUpperMenuID = this.GetTreeListControlSelectedValue("PARENT_ID").ToString();
                    
                    // Tree ID
                    string strTreeID = this.GetTreeListControlSelectedValue("TREE_ID").ToString();

                    // 상위메뉴 ID가 FVRT (즐겨찾기)인 경우 상위메뉴 ID를 현재 화면 상위 ID로 변경한다.
                    if (this.g_strUpperMenuID.Equals("FVRT"))
                    {
                        this.g_strUpperMenuID = strTreeID.Substring(0, strTreeID.Length - 2).Replace("FVRT", string.Empty);
                    }
                    
                    // 현재 선택한 메뉴명
                    string strMenuName = this.GetTreeListControlSelectedValue("MENU_NM").ToString();

                    // 메뉴 레벨에 따라 메뉴 속성에 메뉴명을 저장한다.
                    switch (_iMenuLevel)
                    {
                        case 1:
                            this.g_liMenuNavigation.Add(strMenuName);
                            break;
                        case 2:
                            this.g_liMenuNavigation.Add(strMenuName);
                            break;
                        case 3:
                            this.g_liMenuNavigation.Add(strMenuName);
                            break;
                        case 4:
                            this.g_liMenuNavigation.Add(strMenuName);
                            break;
                        default: break;
                    }
                }
                else
                {


                    // 현재 선택한 메뉴의 상위메뉴값과 메뉴 리스트의 값을 비교한다.
                    var query = from p in this.g_dsMenuInfo.Tables[0].AsEnumerable()
                                .Where(p => p.Field<string>("TREE_ID") == this.g_strUpperMenuID)
                                select p;

                    if (query.Count() > 0)
                    {
                        foreach (DataRow dr in query.CopyToDataTable().Rows)
                        {
                            this.g_strUpperMenuID = dr["PARENT_ID"].ToString();

                            // 메뉴 레벨에 따라 메뉴 속성에 메뉴명을 저장한다.
                            switch (Convert.ToInt32(dr["MENU_LVL"]))
                            {
                                case 1:
                                    this.g_liMenuNavigation.Add(dr["MENU_NM"].ToString());
                                    break;
                                case 2:
                                    this.g_liMenuNavigation.Add(dr["MENU_NM"].ToString());
                                    break;
                                case 3:
                                    this.g_liMenuNavigation.Add(dr["MENU_NM"].ToString());
                                    break;
                                default: break;
                            }
                        }
                    }
                }

                // 상위메뉴 값을 조회하기 위해 메뉴 레벨 값을 감해준다.
                _iMenuLevel--;

                // 메뉴는 1레벨 ~ 4레벨까지이다.
                // 메뉴 레벨이 0레벨인 경우 구문을 종료한다.
                if (_iMenuLevel > 0)
                {
                    // 레벨에 따른 메뉴명을 메뉴 속성에 저장한다.
                    this.SetMenuAttributeValue(string.Empty, _iMenuLevel, false);
                }
            }
            catch { throw; }
        }
        #endregion

        #region SetNoticeInfo  공지정보
        private void SetNoticeInfo(DataTable _dtNoticeInfo)
        {
            try
            {
                //if (_dtNoticeInfo == null) { return; }

                ////탭이 5이상 일경우는 메세지 표시하고, 처리중지
                //if (this.tabControl.Items.Count >= 5)
                //{
                //    string strMessage = CJFC.Modules.Utility.GetMessageByCommon("CHK_OPEN_TAB");
                //    this.BaseClass.MsgWarn(strMessage, this.BaseInfo.country_cd);
                //    return;
                //}

                //DXTabItem tabItem = new DXTabItem();
                //ScrollViewer scrollViewer = new ScrollViewer();
                //scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                //scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                //var query = from p in _dtNoticeInfo.AsEnumerable()
                //            select p;

                //DataRow dr = query.FirstOrDefault();

                //if (query.Count() == 0 || dr["NOTI_CNT"].ToString().Equals("0"))
                //{
                //    if (tabControl.Items.Count == 0)
                //    {
                //        imgMain.Visibility = Visibility.Visible;
                //    }

                //    return;
                //}

                //string sMenuID = dr["MENU_ID"].ToString();
                //string sMenuNM = dr["MENU_NM"].ToString();
                //string sMenuURL = dr["MENU_URL"].ToString();
                //this.lblNoticeCnt.Text = dr["NOTI_CNT"].ToString();

                ////이미 공지사항 탭이 표시되어져 있는 경우
                //for (int i = 0; i < this.tabControl.Items.Count; i++)
                //{
                //    // 선택된 메뉴 ID가 현재 활성화 되어 있는 (탭 컨트롤에 있는) 경우 해당 탭으로 이동하고
                //    // 화면 객체는 생성하지 않는다.
                //    if (((System.Windows.FrameworkElement)(tabControl.Items[i])).Tag.Equals(sMenuID))
                //    {
                //        this.tabControl.SelectedIndex = i;
                //        return;
                //    }
                //}

                ////탭생성과 관련된 처리
                //object frm = null;
                //Type getType = Type.GetType(string.Format("{0}.{1}", NAMESPACE, sMenuURL));

                //this.g_liMenuNavigation.Add("");
                //frm = (object)Activator.CreateInstance(getType, this.g_liMenuNavigation);

                //tabItem.Header = sMenuNM;                                   // 탭명
                //tabItem.Content = frm;                                      // UserControl   (CJFC.Control.Views.NOTICE)

                //tabItem.AllowHide = DevExpress.Utils.DefaultBoolean.True;     // 추가된 TabItem을 Remove하기 위한 버튼을 활성화한다.
                //tabItem.Width = 195;
                //tabItem.Height = 30;
                //tabItem.Margin = new Thickness(0, 0, 0, 0);
                //tabItem.Tag = sMenuID;

                //tabItem.CloseCommand = new RelayCommand<object>(i => OnTabCloseCommand(tabItem, sMenuID));
                //tabItem.Margin = new Thickness(0, 0, 0, 0);
                //this.TabControlItems.Add(new DataModels.MainWindowDataModel() { MENU_ID = sMenuID, TAB_HEADER_TITLE = sMenuID, TAB_CONTENT = frm });

                //int iCount = this.tabControl.Items.Count;
                //this.tabControl.Items.Insert(iCount, tabItem);
                //this.tabControl.SelectedIndex = iCount;

                //tabItem.IsSelected = true;
                ////메인이미지 비표시
                //imgMain.Visibility = Visibility.Collapsed;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region > MoveNoticeClickMenu - 읽지않은 공지사항 수 클릭 시 공지사항 화면 오픈
        /// <summary>
        /// 읽지않은 공지사항 수 클릭 시 공지사항 화면 오픈
        /// </summary>
        private void MoveNoticeClickMenu(string _strMenuID)
        {
            try
            {
                string strSelectedMenuName = string.Empty;

                #region + 자식창에서 부모창의 메뉴를 호출할 때 수행하는 구문
                for (int i = 0; i < this.treeListControl.View.TotalNodesCount; i++)
                {
                    var visiableditem = this.treeListControl.VisibleItems[i];

                    if (((DataModels.MainWindowDataModel)visiableditem).MENU_ID.Equals(_strMenuID))
                    {
                        if (this.MainWinParam.MENU_ID.Length > 0)
                        {
                            strSelectedMenuName         = ((DataModels.MainWindowDataModel)visiableditem).MENU_NM;
                            this.BaseClass.RoleCode     = ((DataModels.MainWindowDataModel)visiableditem).ROLE_MENU_CD;

                            //this.treeListControl.Focus();
                            // 트리 컨트롤의 해당 Row로 이동한다.
                            this.BaseClass.SetTreeListControlRowAddFocus(this.treeListControl, i);
                            break;
                        }
                    }
                }

                object frm              = null;
                var strSelectedMenuURL  = "SMART.WCS.UI.Common.Views.SYS_MGMT.C1006";
                var strNameSpace        = "SMART.WCS.UI.Common";
                Type getType            = Type.GetType($"{strSelectedMenuURL}, {strNameSpace}");

                List<string> liFilterMenuNavigator  = new List<string>();
                liFilterMenuNavigator.Add("공통관리");
                liFilterMenuNavigator.Add("시스템관리");
                liFilterMenuNavigator.Add("게시판관리");
                frm     = (object)Activator.CreateInstance(getType, liFilterMenuNavigator);

                #region + Delegate - 메인 상태바
                EventInfo statusLabelEvent = frm.GetType().GetEvent("ToolStripChangeStatusLabelEvent");

                if (statusLabelEvent != null)
                {
                    Type type           = statusLabelEvent.EventHandlerType;
                    MethodInfo method   = this.GetType().GetMethod("childFrm_ToolStripChangeStatusLabelEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                    Delegate del        = Delegate.CreateDelegate(type, this, method, false);
                    statusLabelEvent.AddEventHandler(frm, del);
                }
                #endregion

                #region + Delegate - 자식창에서 부모창을 호출하여 화면을 오픈한다.
                EventInfo menuOpenEvent = frm.GetType().GetEvent("selectedMenuOpenEvent");
                
                if (menuOpenEvent != null)
                {
                    Type type           = menuOpenEvent.EventHandlerType;
                    MethodInfo method   = this.GetType().GetMethod("childFrm_selectedMenuOpenEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                    Delegate del        = Delegate.CreateDelegate(type, this, method, false);
                    menuOpenEvent.AddEventHandler(frm, del);
                }
                #endregion

                #region + Delegate - 즐겨찾기 변경 후 메인 화면 트리뷰 Refresh후 포커스 이동
                EventInfo mainFormTreeControlRefresh = frm.GetType().GetEvent("TreeControlRefreshEvent");

                if (mainFormTreeControlRefresh != null)
                {
                    Type type           = mainFormTreeControlRefresh.EventHandlerType;
                    MethodInfo method   = this.GetType().GetMethod("childFrm_TreeControlRefreshEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                    Delegate del        = Delegate.CreateDelegate(type, this, method, false);
                    mainFormTreeControlRefresh.AddEventHandler(frm, del);
                }
                #endregion

                #region + 탭 컨트롤에 추가하기 위해 TabItem 컨트롤 객체를 생성한다.

                this.tabControl.SelectionChanged -= TabControl_SelectionChanged;

                DXTabItem tabItem                           = new DXTabItem();
                ScrollViewer scrollViewer                   = new ScrollViewer();
                scrollViewer.HorizontalScrollBarVisibility  = ScrollBarVisibility.Auto;
                scrollViewer.VerticalScrollBarVisibility    = ScrollBarVisibility.Auto;

                string strScreenName            = string.Empty;
                string strSelectedTreeID        = string.Empty;

#if DEBUG
                string[] arrValue = strSelectedMenuURL.Split('.');
                strScreenName = string.Format("{0}-({1})", strSelectedMenuName, arrValue[arrValue.Length - 1]);
#else
            strScreenName       = strSelectedMenuName;
#endif

                tabItem.Header      = strScreenName;
                tabItem.Content     = frm;                                      // UserControl
                tabItem.AllowHide   = DevExpress.Utils.DefaultBoolean.True;     // 추가된 TabItem을 Remove하기 위한 버튼을 활성화한다.
                tabItem.Width       = 200;
                tabItem.Height      = 30;
                tabItem.Margin      = new Thickness(0, 0, 0, 0);
                tabItem.Tag         = strSelectedTreeID;        // TreeID
                tabItem.Name        = _strMenuID;                // 메뉴 ID

                tabItem.CloseCommand = new RelayCommand<object>(i => OnTabCloseCommand(tabItem, strSelectedTreeID));
                tabItem.Margin = new Thickness(0, 0, 0, 0);
                this.TabControlItems.Add(new DataModels.MainWindowDataModel() { MENU_ID = strSelectedTreeID, TAB_HEADER_TITLE = strSelectedTreeID, TAB_CONTENT = frm });

                if (this.MainWinParam != null && this.MainWinParam.MENU_ID != null)
                {
                    if (this.MainWinParam.MENU_ID.Length > 0)
                    {
                        this.childFrm_TreeControlRefreshEvent();
                        this.SettingTopMenuColor(this.g_strSelectedTopMenuID);
                        this.MainWinParam.MENU_ID = null;
                    }
                }

                int iCount = this.tabControl.Items.Count;
                this.tabControl.Items.Insert(iCount, tabItem);
                this.tabControl.SelectedIndex = iCount;


                tabItem.IsSelected = true;
                this.tabControl.SelectionChanged += TabControl_SelectionChanged;

                //메인이미지 비표시
                imgMain.Visibility = Visibility.Collapsed;
                #endregion

                #endregion
            }
            catch { throw; }
        }


        #endregion

        #region > 로그파일을 삭제한다.
        /// <summary>
        /// 로그파일을 삭제한다.
        /// </summary>
        private void CheckLogDateByFileDelete()
        {
            try
            {
                // 삭제대상 경로 + 파일명
                string strPathFilePathInfo = string.Empty;

                var strCurrentPath          = System.Windows.Forms.Application.StartupPath;                     // WCS 프로그램이 실행되는 경로
                var iDateNow                = Convert.ToInt32(DateTime.Now.AddMonths(-1).ToString("yyyyMMdd")); // 현재일자 기준 한달전
                var strLogPath              = string.Format("{0}\\{1}", strCurrentPath, "Log\\");               // WCS 로그 저장 폴더

                // 로그 디렉토리 내 디렉토리를 가져온다.
                // Error, Info, DBInputParameter 폴더
                List<string> liDirectory    = this.BaseClass.GetListOnDictionary(strLogPath);

                Dictionary<string, List<string>> dicFileInfo = new Dictionary<string, List<string>>();

                // 
                foreach (var itemDirectory in liDirectory)
                {
                    dicFileInfo.Add(itemDirectory, this.BaseClass.GetDirectoryFileList(string.Format("{0}{1}", strLogPath, itemDirectory)));
                }

                foreach (KeyValuePair<string, List<string>> itemDirectoryInfo in dicFileInfo)
                {
                    foreach (var itemFileInfo in itemDirectoryInfo.Value)
                    {
                        if (Convert.ToInt32(itemFileInfo.Substring(0, 8)) <= iDateNow)
                        {
                            strPathFilePathInfo = string.Format("{0}{1}\\{2}", strLogPath, itemDirectoryInfo.Key, itemFileInfo);

                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(strPathFilePathInfo);
                            if (fileInfo.Exists)
                            {
                                fileInfo.Delete();
                                
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 컨트롤 바인딩
        #region BindingMainTopMenu - 상단 메뉴 데이터 설정 및 컨트롤 바인딩
        /// <summary>
        /// 상단 메뉴 데이터 설정 및 컨트롤 바인딩
        /// </summary>
        private void BindingMainTopMenu()
        {
            // 메뉴 정보 중 0 레벨 데이터만 필터한다.
            var arrDrFilterTopLevelMenuList = this.g_dsMenuInfo.Tables[0].Select("MENU_LVL = " + 0).ToList();

            if (arrDrFilterTopLevelMenuList != null)
            {
                if (arrDrFilterTopLevelMenuList.Count > 0)
                {
                    int i = 0;

                    foreach (var item in arrDrFilterTopLevelMenuList)
                    {
                        var columnDefinition = new ColumnDefinition();
                        columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
                        this.topGridMenuArea.ColumnDefinitions.Add(columnDefinition);

                        this.g_tbTopMenuCtrl            = new TextBlock();
                        // CHOO_POINT 
                        // 2020-04-21 / 추성호 / 상단 메뉴명 다국어 등록 확인 후 변경
                        this.g_tbTopMenuCtrl.Text       = item.Field<string>("LANG_CD");       // 메뉴명
                        // this.g_tbTopMenuCtrl.Text       = item.Field<string>("MENU_NM");        // 메뉴명
                        this.g_tbTopMenuCtrl.Tag        = item.Field<string>("MENU_ID");        // 메뉴 ID
                        this.g_tbTopMenuCtrl.Cursor     = Cursors.Hand;
                        this.g_tbTopMenuCtrl.Margin     = new Thickness(0, 0, 30, 0);
                        this.g_tbTopMenuCtrl.FontSize   = 16;

                        if (i == 0)
                        {
                            this.g_tbTopMenuCtrl.Foreground = this.BaseClass.ConvertStringToSolidColorBrush("#222222");
                        }
                        else
                        {
                            this.g_tbTopMenuCtrl.Foreground = this.BaseClass.ConvertStringToSolidColorBrush("#A0A0A0");
                        }

                        this.g_tbTopMenuCtrl.FontWeight                     = FontWeights.Bold;
                        this.g_tbTopMenuCtrl.HorizontalAlignment            = HorizontalAlignment.Left;
                        this.g_tbTopMenuCtrl.MouseEnter                     += TbCtrl_MouseEnter;
                        this.g_tbTopMenuCtrl.MouseLeave                     += TbCtrl_MouseLeave;
                        this.g_tbTopMenuCtrl.PreviewMouseLeftButtonUp       += TbCtrl_PreviewMouseLeftButtonUp;
                        Grid.SetColumn(this.g_tbTopMenuCtrl, i);
                        this.topGridMenuArea.Children.Add(this.g_tbTopMenuCtrl);

                        i++;
                    }
                }
            }
        }

        private void TbCtrl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TextBlock tbCtrl                = (TextBlock)sender;
                this.BindingLeftTreeControlList(tbCtrl.Tag.ToString());
                this.g_strSelectedTopMenuID     = tbCtrl.Tag.ToString();
                this.g_strSelectedTopMenuName   = tbCtrl.Text.Trim();
            

                for(int i = 0; i < this.topGridMenuArea.Children.Count; i++)
                {
                    var childCtrl = this.topGridMenuArea.Children[i];

                    if (childCtrl is TextBlock)
                    {
                        TextBlock tbCtrlOld     = (TextBlock)childCtrl;
                      
                        if (this.g_strTopMenuCtrlTagName.Equals(tbCtrlOld.Tag) == true)
                        {
                            tbCtrlOld.Foreground = this.BaseClass.ConvertStringToSolidColorBrush("#A0A0A0");
                            break;
                        }
                    }
                }

                this.g_strTopMenuCtrlTagName    = tbCtrl.Tag.ToString();
                tbCtrl.Foreground               = this.BaseClass.ConvertStringToSolidColorBrush("#222222");
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void TbCtrl_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock tbCtrl = (TextBlock)sender;
            if (this.g_strTopMenuCtrlTagName.Equals(tbCtrl.Tag) == true) { return; }

            tbCtrl.Foreground = this.BaseClass.ConvertStringToSolidColorBrush("#A0A0A0");
        }

        private void TbCtrl_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tbCtrl = (TextBlock)sender;
            if (this.g_strTopMenuCtrlTagName.Equals(tbCtrl.Tag) == true) { return; }

            tbCtrl.Foreground = this.BaseClass.ConvertStringToSolidColorBrush("#385FF2");
        }
        #endregion

        #region BindingNoticeAlarm - 메인 화면 오픈 시 우측 공지사항 데이터 설정 및 컨트롤 바인딩
        /// <summary>
        /// 메인 화면 오픈 시 우측 공지사항 데이터 설정 및 컨트롤 바인딩
        /// </summary>
        /// 
        private void BindingNoticeAlarm(DataTable _dtNoticeInfo)
        {
            // 새로운 공지사항 유무를 확인한다.

            //string checkYN = null;
            //foreach (DataRow dr in _dtNoticeInfo.Rows)
            //{
            //    checkYN = dr["CHECKYN"].ToString();
            //}

            //if (checkYN.Equals("T"))
            //{
            //    // 컨트롤 변경 불필요
            //}
            //else
            //{
            //    // 컨트롤 변경 필요 잠시 정지 
            //    //this.BaseClass.MsgInfo("새로운 공지사항이 있습니다. 게시판을 확인해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
            //}

            // 공지사항 건수
            var iNoticeRowCount = Convert.ToInt32(_dtNoticeInfo.Rows[0]["CNT"]);

            this.gridAreaNoticeCount.Visibility  = Visibility.Hidden;

            if (iNoticeRowCount > 0)
            {
                this.eliNotice.Cursor                   = Cursors.Hand;
                this.lblNotice.Text                     = iNoticeRowCount.ToString();
                this.gridAreaNoticeCount.Visibility     = Visibility.Visible;

                #region 이미지 버튼
                // 공지 알람 버튼 영역 클릭 이벤트
                //this.gridAreaNotice.PreviewMouseLeftButtonUp += GridAreaNotice_PreviewMouseLeftButtonUp;
                this.eliNotice.PreviewMouseLeftButtonUp += GridAreaNotice_PreviewMouseLeftButtonUp;
                this.lblNotice.PreviewMouseLeftButtonUp += GridAreaNotice_PreviewMouseLeftButtonUp;
                #endregion
            }
        }
        #endregion

        #region BindingFirstLeftTreeControlList - (최초) 메인 화면 오픈 시 좌측 트리 메뉴 데이터 설정 및 컨트롤 바인딩
        /// <summary>
        /// (최초) 메인 화면 오픈 시 좌측 트리 메뉴 데이터 설정 및 컨트롤 바인딩
        /// </summary>
        private void BindingFirstLeftTreeControlList()
        {
            string strFirstMenuID = string.Empty;

            for (int i = 0; i < this.topGridMenuArea.Children.Count; i++)
            {
                var childCtrl = this.topGridMenuArea.Children[i];

                if (childCtrl is TextBlock)
                {
                    TextBlock tbCtrl = (TextBlock)childCtrl;
                    strFirstMenuID = tbCtrl.Tag.ToString();
                    this.g_strSelectedTopMenuID     = tbCtrl.Tag.ToString();    // 현재 선택된 상단 메뉴 ID
                    this.g_strSelectedTopMenuName   = tbCtrl.Text;
                    break;
                }
            }

            // strTextBlockTag 값이 0이면 바인딩 된 상위메뉴가 없는 경우
            if (strFirstMenuID.Length == 0) { return; }

            DataTable dtFilterTreeMenuList = null;

            #region + 좌측 트리 리스트 구성
            var liFilterTreeMenuList = this.g_dsMenuInfo.Tables[0].AsEnumerable().Where(p => p.Field<string>("TREE_ID").StartsWith(strFirstMenuID)
                                       && p.Field<string>("TREE_ID").Length > 3).Union
                                        (this.g_dsMenuInfo.Tables[0].AsEnumerable().Where(p => p.Field<string>("TREE_ID").StartsWith("FVRT")));

            if (liFilterTreeMenuList != null)
            {
                if (liFilterTreeMenuList.Count() > 0)
                {
                    dtFilterTreeMenuList = liFilterTreeMenuList.CopyToDataTable();

                    foreach (DataRow drRow in dtFilterTreeMenuList.Rows)
                    {
                        if (drRow["MENU_NM"].Equals("FVRT") == true)
                        {
                            drRow["MENU_NM"] = drRow["LANG_CD"];
                            dtFilterTreeMenuList.AcceptChanges();
                            break;
                        }
                    }
                }
            }
            #endregion

            #region + 메뉴 리스트 (트리 리스트) 컨트롤 바인딩
            if (dtFilterTreeMenuList == null)
            {
                this.MenuList.ToObservableCollection(null);
            }
            else
            {
                if (dtFilterTreeMenuList.Rows.Count > 0)
                {
                    this.MenuList = new ObservableCollection<MainWindowDataModel>();
                    this.MenuList.ToObservableCollection(dtFilterTreeMenuList);
                }
                else
                {
                    this.MenuList.ToObservableCollection(null);
                }
            }

            this.treeListControl.ItemsSource = this.MenuList;
            #endregion
        }
        #endregion

        #region BindingLeftTreeControlList - 좌측 트리 메뉴 데이터 설정 및 컨트롤 바인딩
        /// <summary>
        /// 좌측 트리 메뉴 데이터 설정 및 컨트롤 바인딩
        /// </summary>
        /// <param name="_strSelectedMenuID">상단 메뉴중 선택된 값</param>
        private void BindingLeftTreeControlList(string _strSelectedMenuID)
        {
            DataTable dtFilterTreeMenuList = null;

            #region + 좌측 트리 리스트 구성
            var liFilterTreeMenuList = this.g_dsMenuInfo.Tables[0].AsEnumerable().Where(p => p.Field<string>("TREE_ID").StartsWith(_strSelectedMenuID)
                                       && p.Field<string>("TREE_ID").Length > 3).Union
                                        (this.g_dsMenuInfo.Tables[0].AsEnumerable().Where(p => p.Field<string>("TREE_ID").StartsWith("FVRT")));

            if (liFilterTreeMenuList != null)
            {
                if (liFilterTreeMenuList.Count() > 0)
                {
                    dtFilterTreeMenuList = liFilterTreeMenuList.CopyToDataTable();

                    foreach (DataRow drRow in dtFilterTreeMenuList.Rows)
                    {
                        if (drRow["MENU_NM"].Equals("FVRT") == true)
                        {
                            drRow["MENU_NM"] = drRow["LANG_CD"];
                            dtFilterTreeMenuList.AcceptChanges();
                            break;
                        }
                    }
                }
            }
            #endregion

            #region + 메뉴 리스트 (트리 리스트) 컨트롤 바인딩
            if (dtFilterTreeMenuList == null)
            {
                this.MenuList.ToObservableCollection(null);
            }
            else
            {
                if (dtFilterTreeMenuList.Rows.Count > 0)
                {
                    this.MenuList = new ObservableCollection<MainWindowDataModel>();
                    this.MenuList.ToObservableCollection(dtFilterTreeMenuList);
                }
                else
                {
                    this.MenuList.ToObservableCollection(null);
                }
            }

            this.treeListControl.ItemsSource = this.MenuList;
            #endregion
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> [조회] GetMenuList - 메뉴 정보를 조회한다.
        /// <summary>
        /// 메뉴 정보를 조회한다.
        /// </summary>
        /// <returns></returns>
        private DataSet GetMenuList()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                var strProcedureName                        = "PK_C1000.SP_MENU_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_MENU_LIST", "O_USER_INFO_LIST", "O_RSLT" };
                #endregion

                #region Input 파라메터
                var strCenterCD         = this.BaseClass.CenterCD;      // 센터 코드
                var strUserID           = this.BaseClass.UserID;        // 사용자 ID
                #endregion

                dicInputParam.Add("P_CNTR_CD",      strCenterCD);       // 센터 코드
                dicInputParam.Add("P_USER_ID",      strUserID);         // 사용자 ID

                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    this.g_dsMenuInfo = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }

                if (this.g_dsMenuInfo.Tables[0].Rows.Count > 0)
                {
                    this.InitResourceByLanguage();
                }

                return this.g_dsMenuInfo;
            }
            catch { throw; }
        }
        #endregion

        #region >> [조회] GetNoticeList - 공지사항 정보를 조회한다.
        /// <summary>
        /// 공지사항 정보를 조회한다.
        /// </summary>
        /// <returns></returns>
        private DataSet GetNoticeList()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_C1000.SP_NOTICE_TALLY_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputPram                      = { "O_NEW_NOTICE_RSLT", "O_RSLT" };
                #endregion

                #region Input 파라메터
                var strCenterCD         = this.BaseClass.CenterCD;      // 센터코드
                var strUserID           = this.BaseClass.UserID;        // 사용자 ID
                var strDateTime         = DateTime.Now.ToString("yyyyMMdd");
                #endregion

                dicInputParam.Add("P_CNTR_CD", strCenterCD);       // 센터 코드
                dicInputParam.Add("P_USER_ID", strUserID);         // 사용자 ID
                dicInputParam.Add("P_DATE", strDateTime);          // 오늘날짜 (yyyymmdd)

                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputPram);
                }
                return dsRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region 메인 화면
        #region frmMainWindow_Loaded - 폼 로드
        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
//#if DEBUG
//#else

//                //// 서버, 로컬 프로그램 버전 정보 Display
//                //this.lblVersionInfo.Text = this.BaseClass.GetLocalVersionInfo(this.BaseInfo.center_cd_string, EnumClass.SystemCode.MLD, this.BaseInfo.db_conn_info);
//#endif

                
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region 마우스 휠 클릭 방지를 위한 이벤트 (탭화면에서 마우스 휠 클릭하면 화면 닫히는 현상 방지)
        /// <summary>
        /// 마우스 휠 클릭 방지를 위한 이벤트
        /// <br />(탭화면에서 마우스 휠 클릭하면 화면 닫히는 현상 방지)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = e.ChangedButton == MouseButton.Middle;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region 화면 종료 버튼 클릭
        /// <summary>
        /// 화면 종료 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                if (this.MainWinParam != null) { this.MainWinParam = null; }

                if (this.g_timerClock != null)
                {
                    this.g_timerClock.Stop();
                    this.g_timerClock = null;
                }

                if (this.g_timerConnectionStatus != null)
                {
                    this.g_timerConnectionStatus.Stop();
                    this.g_timerConnectionStatus = null;
                }

                if (this.g_timerSessionCheck != null)
                {
                    this.g_timerSessionCheck.Stop();
                    this.g_timerSessionCheck = null;
                }

                if (this.g_timerVersionInfo != null)
                {
                    this.g_timerVersionInfo.Stop();
                    this.g_timerVersionInfo = null;
                }

                if (this.g_ctrlScreens != null)
                {
                    this.g_ctrlScreens = null;
                }

                if (this.g_dsMenuInfo != null)
                {
                    this.g_dsMenuInfo = null;
                }

                if (this.g_dtNoticeInfo != null)
                {
                    this.g_dtNoticeInfo = null;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region MainWindow ContextMenu 클릭 이벤트 (전체 탭 종료)
        /// <summary>
        /// MainWindow ContextMenu 클릭 이벤트 (전체 탭 종료)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemTabAllClose_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.BaseClass.MsgQuestion("ASK_CLOSE_ALL_TAB");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                List<string> liTabContent = new List<string>();

                for (int i = 0; i < this.tabControl.Items.Count; i++)
                {
                    liTabContent.Add(((System.Windows.FrameworkElement)this.tabControl.Items[i]).Name);
                }

                for (int i = 0; i < liTabContent.Count; i++)
                {
                    for (int j = this.tabControl.Items.Count - 1; j >= 0; j--)
                    {
                        var strUrl = ((System.Windows.FrameworkElement)this.tabControl.Items[j]).Name;

                        if (liTabContent[i].Equals(strUrl) == true)
                        {
                            this.tabControl.Items.RemoveAt(j);
                            this.TabControlItems.RemoveAt(j);
                            break;
                        }
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

        #region 타이머
        #region timerClock_Tick - 메인 화면 하단 시계 표현을 위한 타이머
        /// <summary>
        /// 메인 화면 하단 시계 표현을 위한 타이머
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerClock_Tick(object sender, EventArgs e)
        {
            try
            {
                string strClock = string.Format("{0:yyyy-MM-dd hh:mm:ss tt}", DateTime.Now);
                this.lblClock.Text = strClock;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region timerConnectionStatus_Tick - 메인 화면 하단 인터넷 연결 상태 표현을 위한 타이머
        /// <summary>
        /// 메인 화면 하단 인터넷 연결 상태 표현을 위한 타이머
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerConnectionStatus_Tick(object sender, EventArgs e)
        {
            try
            {
                //if (this.BaseClass.CheckInternetConnectionStatus() == true)
                //{
                //    this.lblConnected.Visibility = Visibility.Visible;
                //    this.lblDisConnected.Visibility = Visibility.Hidden;

                //    this.imgConnected.Visibility = Visibility.Visible;
                //    this.imgDisConnected.Visibility = Visibility.Hidden;
                //}
                //else
                //{
                //    this.lblConnected.Visibility = Visibility.Hidden;
                //    this.lblDisConnected.Visibility = Visibility.Visible;

                //    this.imgConnected.Visibility = Visibility.Hidden;
                //    this.imgDisConnected.Visibility = Visibility.Visible;
                //}
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }

        }
        #endregion

        #region timerSessionCheck_Tick - 세션 체크용 타이머
        /// <summary>
        /// 세션 체크용 타이머
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerSessionCheck_Tick(object sender, EventArgs e)
        {
            try
            {
                //string strMessage = string.Empty;

                //string strConnectionStringValue = this.BaseInfo.db_conn_info;   // 연결문자열
                //string strUserID = this.BaseInfo.user_id;        // 사용자 ID

                //string strRtnValue = this.BaseClass.GetCOMM_SESSION_DEL_SEARCH(strConnectionStringValue, strUserID);

                //if (strRtnValue.Equals("DELETE") == true)
                //{
                //    // 경고창 출력 후 프로그램 종료
                //    strMessage = CJFC.Modules.Utility.GetMessageByCommon("END_SESSION");
                //    this.BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);

                //    Logout frmLogin = new Logout(this.BaseInfo.country_cd);
                //    frmLogin.Show();

                //    this.Close();
                //}
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region timerVersionInfo_Tick - 서버, 로컬 프로그램 버전 Display
        /// <summary>
        /// 서버, 로컬 프로그램 버전 Display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerVersionInfo_Tick(object sender, EventArgs e)
        {
            try
            {
                //this.lblVersionInfo.Text = this.BaseClass.GetLocalVersionInfo(this.BaseInfo.center_cd_string, EnumClass.SystemCode.MLD, this.BaseInfo.db_conn_info);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region 트리뷰
        #region treeListControl_PreviewMouseLeftButtonUp - 트리 컨트롤 클릭 이벤트
        /// <summary>
        /// 트리 컨트롤 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeListControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isFvrtClickYN  = false;

                if (sender != null)
                {
                    var view = (sender as TreeListControl).View as TreeListView;
                    var hitInfo = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                    if (hitInfo.Column == null) { return; }
                }
                
                // 트리 리스트 클릭 시 버튼 (트리 확대, 축소)을 클릭할 경우 구문을 리턴한다.
                string strCurrentCtrlKind = string.Empty;
                if (e != null)
                {
                    strCurrentCtrlKind = ((System.Windows.FrameworkElement)e.OriginalSource).Name.ToString().ToString();
                }

                if (strCurrentCtrlKind.Equals("TOGGLEBUTTON") == true) { return; }

                #region 자식창에서 부모창의 메뉴를 호출할 때 수행하는 구문
                for (int i = 0; i < this.treeListControl.View.TotalNodesCount; i++)
                {
                    var visiabledItem = this.treeListControl.VisibleItems[i];

                    if (this.MainWinParam == null) { break; ; }
                    if (this.MainWinParam.MENU_ID == null) { break; }
                    if (this.MainWinParam.MENU_ID.Length == 0) { break; }

                    if (((DataModels.MainWindowDataModel)visiabledItem).MENU_ID == this.MainWinParam.MENU_ID.Trim())
                    {
                        if (this.MainWinParam.MENU_ID.Length > 0)
                        {
                            this.treeListControl.Focus();
                            this.treeListControl.View.FocusedRowHandle = i;
                            break;
                        }
                    }
                }
                #endregion

                if (this.tabControl != null)
                {
                    if (this.MainWinParam == null || this.MainWinParam.MENU_ID == null)
                    {
                        #region + 일반 화면 호출
                        if (this.tabControl.Items.Count > 0)
                        {
                            var treeSelectedIndex       = this.treeListControl.View.FocusedRowHandle;
                            var treeSelectedValue       = this.treeListControl.VisibleItems[treeSelectedIndex];
                            var selectedTreeID          = ((MainWindowDataModel)treeSelectedValue).TREE_ID.Replace("FVRT", string.Empty);

                            for (int i = 0; i < this.tabControl.Items.Count; i++)
                            {
                                // 좌측 메뉴에서 선택한 메뉴의 탭이 열려 있는 경우 해당 탭을 선택한 후 구문을 리턴한다.
                                if (((System.Windows.FrameworkElement)(this.tabControl.Items[i])).Tag.Equals(selectedTreeID))
                                {
                                    this.tabControl.SelectedIndex = i;
                                    return;
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region + 자식창에서 부모창 호출하는 경우
                        switch (this.MainWinParam.MENU_ID)
                        {
                            case "C1006":       // 공지 게시판
                                for (int i = 0; i < this.tabControl.Items.Count; i++)
                                {
                                    if (((System.Windows.FrameworkElement)(tabControl.Items[i])).Tag.Equals(this.MainWinParam.MENU_ID))
                                    {
                                        this.tabControl.Items.RemoveAt(i);
                                        this.TabControlItems.RemoveAt(i);
                                        this.tabControl.Items.Refresh();
                                        break;
                                    }
                                }
                                break;

                            case "R1001_SRT":
                                for (int i = 0; i < this.tabControl.Items.Count; i++)
                                {
                                    if (((System.Windows.FrameworkElement)(tabControl.Items[i])).Tag.Equals(this.MainWinParam.MENU_ID))
                                    {
                                        this.tabControl.Items.RemoveAt(i);
                                        this.TabControlItems.RemoveAt(i);
                                        this.tabControl.Items.Refresh();
                                        break;
                                    }
                                }
                                break;
                            case "R1007_GAN":       // 
                                for (int i = 0; i < this.tabControl.Items.Count; i++)
                                {
                                    if (((System.Windows.FrameworkElement)(tabControl.Items[i])).Tag.Equals(this.MainWinParam.MENU_ID))
                                    {
                                        this.tabControl.Items.RemoveAt(i);
                                        this.TabControlItems.RemoveAt(i);
                                        this.tabControl.Items.Refresh();
                                        break;
                                    }
                                }
                                break;
                            case "P1005_GAN":       // 
                                for (int i = 0; i < this.tabControl.Items.Count; i++)
                                {
                                    if (((System.Windows.FrameworkElement)(tabControl.Items[i])).Tag.Equals(this.MainWinParam.MENU_ID))
                                    {
                                        this.tabControl.Items.RemoveAt(i);
                                        this.TabControlItems.RemoveAt(i);
                                        this.tabControl.Items.Refresh();
                                        break;
                                    }
                                }
                                break;
                        }
                        #endregion
                    }
                }

                string strMenuID            = string.Empty;
                string strSelectedTreeID    = string.Empty;
                string strSelectedMenuName  = string.Empty;
                string strSelectedMenuType  = string.Empty;
                string strSelectedMenuURL   = string.Empty;

                if (this.MainWinParam != null && this.MainWinParam.MENU_ID != null)
                {
                    if (this.MainWinParam.MENU_ID.Length == 0) { return; }

                    if (this.g_dtOtherProgOpen.Rows.Count > 0)
                    { 
                        var liFilterMenuData = this.g_dtOtherProgOpen.AsEnumerable().Where(p => p.Field<string>("MENU_ID").Equals(this.MainWinParam.MENU_ID)).FirstOrDefault();
            
                        strMenuID           = liFilterMenuData.Field<string>("MENU_ID");        // 메뉴 ID
                        strSelectedTreeID   = liFilterMenuData.Field<string>("TREE_ID");        // 트리 메뉴 ID
                        strSelectedMenuName = liFilterMenuData.Field<string>("MENU_NM");        // 메뉴명
                        strSelectedMenuType = liFilterMenuData.Field<string>("MENU_TYPE");      // 메뉴 타입
                        strSelectedMenuURL  = liFilterMenuData.Field<string>("MENU_URL");       // 메뉴 URL

                        if (liFilterMenuData.Field<string>("PARENT_ID").Length == 3)
                        {
                            this.g_strSelectedTopMenuID = liFilterMenuData.Field<string>("PARENT_ID");
                        }
                        else
                        {
                            this.g_strSelectedTopMenuID = liFilterMenuData.Field<string>("PARENT_ID").Substring(0, 3);
                        }
                    }
                }
                else
                {
                    // 자식창에서 부모창을 호출하는 경우

                    strMenuID               = this.GetTreeListControlSelectedValue("MENU_ID").ToString().ToUpper();
                    strSelectedTreeID       = this.GetTreeListControlSelectedValue("TREE_ID").ToString().ToUpper();

                    if (strSelectedTreeID.Substring(0, 4).Equals("FVRT") == true)
                    {
                        // 좌측 메뉴 리스트 중 즐겨찾기의 메뉴를 선택한 경우
                        strSelectedTreeID   = strSelectedTreeID.Substring(4, strSelectedTreeID.Length - 4);
                        isFvrtClickYN       = true;
                    }

                    strSelectedMenuName     = this.GetTreeListControlSelectedValue("MENU_NM").ToString();
                    strSelectedMenuType     = this.GetTreeListControlSelectedValue("MENU_TYPE").ToString().ToUpper();
                    strSelectedMenuURL      = this.GetTreeListControlSelectedValue("MENU_URL").ToString();

                    #region 즐겨찾기 메뉴를 선택하는 경우 해당되는 상단 메뉴 설정을 해야한다.
                    var strSelectedTopMenuID    = strSelectedTreeID.Substring(0, 3);

                    // CHOO
                    //this.BindingLeftTreeControlList(strSelectedTopMenuID);

                    

                    #region 상단 메인메뉴 색상 초기화
                    //for (int j = 0; j < this.topGridMenuArea.Children.Count; j++)
                    //{
                    //    var childCtrl = this.topGridMenuArea.Children[j];

                    //    if (childCtrl is TextBlock)
                    //    {
                    //        TextBlock tbCtrl = (TextBlock)childCtrl;

                    //        if (tbCtrl.Tag.Equals(strSelectedTopMenuID) == true)
                    //        {
                    //            // 현재 선택된 탭의 최상단 메뉴 ID가 동일한 경우 선택된 색상으로 변경한다.
                    //            this.g_strTopMenuCtrlTagName = strSelectedTopMenuID;
                    //            tbCtrl.Foreground = this.BaseClass.ConvertStringToSolidColorBrush("#222222");
                    //        }
                    //        else
                    //        {
                    //            tbCtrl.Foreground = this.BaseClass.ConvertStringToSolidColorBrush("#A0A0A0");
                    //        }
                    //    }
                    //}
                    this.SettingTopMenuColor(strSelectedTopMenuID);
                    #endregion
                }

                // 오픈된 탭을 선택하는 경우 탭에 해당하는 메뉴를 TreeListControl에 포커스가 이동 되도록 한다.
                //for (int i = 0; i < this.treeListControl.VisibleRowCount; i++)


                for (int i = 0; i < this.treeListControl.View.TotalNodesCount; i++)
                {
                    var item                = this.treeListControl.VisibleItems[i];
                    var strSelectedMenuID   = ((MainWindowDataModel)item).MENU_ID;

                    //if (strSelectedMenuID.Equals(item) == true)
                    if (strMenuID.Equals(strSelectedMenuID))
                    {
                        this.BaseClass.SetTreeListControlRowAddFocus(this.treeListControl, i);
                        break;
                    }
                }

                var iMenuLevel                          = Convert.ToInt32(this.GetTreeListControlSelectedValue("MENU_LVL"));
                var strScreenName                       = string.Empty;
                List<string> liFilterMenuNavigator      = new List<string>();

                #region + 외부 프로그램 실행
                // 외부 실행 프로그램 여부를 체크해서 외부 실행인 경우 해당 화면 호출하고 구문을 리턴한다.
                // CHOO strSelectedTreeID 값으로 팝업 여부 체크가 가능한지 확인필요
                //if (strSelectedTreeID.Equals("POP_UP_YN") == true)
                //{
                //    this.ProcessExecute(strSelectedTreeID, strSelectedMenuType);
                //    return;
                //}
                
                if (strSelectedMenuType.Equals("SCADA") == true)
                {
                    // 권한 정보를 BaseInfo 속성(ROLE_CD)에 저장한다.
                    this.BaseClass.RoleCode = this.GetTreeListControlSelectedValue("ROLE_MENU_CD").ToString().ToUpper();
                    this.ProcessExecute(strSelectedTreeID, strSelectedMenuType);
                    return;
                }
                else if (strSelectedMenuType.ToUpper().Equals("KIOSK") == true)
                {
                    // 권한 정보를 BaseInfo 속성(ROLE_CD)에 저장한다.
                    this.BaseClass.RoleCode = this.GetTreeListControlSelectedValue("ROLE_MENU_CD").ToString().ToUpper();
                    this.ProcessExecute(strSelectedTreeID, strSelectedMenuType);
                    return;
                }

                #endregion

                // 선택된 트리 메뉴의 Menu Url이 공백인 경우 구문을 리턴한다.
                if (strSelectedMenuURL.Length == 0) { return; }

                // 선택한 트리 메뉴(탭)가 활성화 되어 있는 경우 구문을 리턴한다.
                for (int i = 0; i < this.tabControl.Items.Count; i++)
                {
                    var strMenuUrl = ((System.Windows.Controls.ContentControl)this.tabControl.Items[i]).Content.ToString();

                    if (this.MainWinParam == null && this.MainWinParam.MENU_ID == null)
                    {
                        if (this.MainWinParam.MENU_ID.Length == 0)
                        {
                            if (strSelectedMenuURL.Equals(strMenuUrl) == true) { return; }
                        }
                    }
                }

                #region + 활성화 된 탭 개수 체크 (10개까지)
                if (this.tabControl.Items.Count > 10)
                {
                    var strMessage = this.BaseClass.GetResourceValue("CHK_OPEN_TAB");
                    //MessageBox.Show(strMessage);
                    this.BaseClass.MsgInfo(strMessage);
                    this.treeListControl.View.FocusedRowHandle = this.g_iTreeViewFocusedRowIndex;
                }
                #endregion

                //var iSystemIDLength = strSelectedMenuURL.Split('.')[0].Length;
                //strSelectedMenuURL = strSelectedMenuURL.Substring(4, strSelectedMenuURL.Length - iSystemIDLength - 1);

                //var query = this.g_dtMenuInfo.AsEnumerable().Where(p => p.Field<string>("PARENT_ID").Equals(strSelectedMenuID)).ToList();

                //if (query.Count() > 0)
                //{
                var iTabCount = -1;

                // 현재 클릭(트리 메뉴)한 항목(화면)이 탭으로 열려있는지 여부를 체크한다.
                // 같은 경우 해당 화면으로 이동한다.
                // 다른 경우 해당 화면 탭을 연다.
                foreach (DXTabItem item in this.tabControl.Items)
                {
                    iTabCount++;

                    if (strSelectedTreeID.Equals(item.Tag.ToString().ToUpper()) == true)
                    {
                        if (this.MainWinParam == null && this.MainWinParam.MENU_ID == null)
                        {
                            if (this.MainWinParam.MENU_ID.Length == 0)
                            {
                                this.tabControl.SelectedIndex = iTabCount;
                                return;
                            }
                        }
                    }
                }

                // 권한 정보를 BaseInfo 속성(ROLE_CD)에 저장한다.
                this.BaseClass.RoleCode     = this.GetTreeListControlSelectedValue("ROLE_MENU_CD").ToString().ToUpper();
                //MessageBox.Show(this.BaseClass.RoleCode);

                var arrSelectedMenuURL      = strSelectedMenuURL.Split('.');
                var strNamespace            = string.Empty;
                
                for (int i = 0; i < arrSelectedMenuURL.Length; i++)
                {
                    if (i == 4) { break; }

                    strNamespace    +=  $"{arrSelectedMenuURL[i]}.";
                }

                if (strNamespace.Length > 0)
                {
                    strNamespace = strNamespace.Substring(0, strNamespace.Length - 1);
                }

                Type getType = Type.GetType($"{strSelectedMenuURL}, {strNamespace}");

                if (getType != null)
                {
                    // 클릭하는 화면에 따른 메뉴정보를 저장하기 위해 변수를 초기화 한 후 메뉴정보를 저장한다.
                    if (this.g_liMenuNavigation.Count > 0)
                    {
                        this.g_liMenuNavigation.Clear();
                    }

                    // 레벨에 따른 메뉴 정보를 저장한다.
                    this.SetMenuAttributeValue(strSelectedTreeID, iMenuLevel, true);

                    if (isFvrtClickYN == true)
                    {
                        this.g_strSelectedTopMenuName = arrSelectedMenuURL[3];

                        this.SettingTopMenuColor(this.g_strTopMenuCtrlTagName);
                    }

                    // CHOO - CHECK (2020-02-11)
                    if (this.MainWinParam != null)
                    {
                        this.g_strSelectedTopMenuName = arrSelectedMenuURL[3];
                    }

                    this.g_liMenuNavigation.Add(this.g_strSelectedTopMenuName);

                    if (this.g_liMenuNavigation.Count > 1)
                    {
                        for (int i = this.g_liMenuNavigation.Count- 1; i >= 0; i--)
                        {
                            liFilterMenuNavigator.Add(this.g_liMenuNavigation[i]);
                        }
                    }

                    //strSelectedMenuType
                    switch (strSelectedMenuType.ToUpper())
                    {
                        case "POPUP":
                            if (strMenuID == "C1028_01P")
                            {
                                C1028_01P frmPopup = new C1028_01P();
                                frmPopup.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                frmPopup.ShowDialog();
                            }
                            break;

                        case "TAB":
                            object frm = null;

                            if (this.MainWinParam == null || this.MainWinParam.MENU_ID == null)
                            {
                                // 팝업이 열려야 하는 경우 여기 기술 (if문으로) - MENU_ID로 체크하면 됨.
                                //if (팝업 == true)
                                //{
                                //}
                                //else
                                //{
                                    frm = (object)Activator.CreateInstance(getType, liFilterMenuNavigator);
                                //}
                            }
                            else if (this.MainWinParam.MENU_ID.Length == 0)
                            {
                                frm = (object)Activator.CreateInstance(getType, liFilterMenuNavigator);
                            }
                            else
                            {
                                switch (this.MainWinParam.MENU_ID)
                                {
                                    case "C1006":
                                        frm = (object)Activator.CreateInstance(getType, liFilterMenuNavigator);
                                        break;
                                    case "R1005_GAN":
                                    case "R1007_GAN":
                                    case "R1001_SRT":
                                        frm = (object)Activator.CreateInstance(getType, liFilterMenuNavigator, this.MainWinParam);
                                        break;
                                }
                            }

                            if (this.tabControl.Items.Count == 0)
                            {
                                this.InitTabVisiabled(true);
                            }

                            #region + Delegate - 메인 상태바
                            EventInfo statusLabelEvent = frm.GetType().GetEvent("ToolStripChangeStatusLabelEvent");

                            if (statusLabelEvent != null)
                            {
                                Type type = statusLabelEvent.EventHandlerType;
                                MethodInfo method = this.GetType().GetMethod("childFrm_ToolStripChangeStatusLabelEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                                Delegate del = Delegate.CreateDelegate(type, this, method, false);
                                statusLabelEvent.AddEventHandler(frm, del);
                            }
                            #endregion

                            #region + Delegate - 자식창에서 부모창을 호출하여 화면을 오픈한다.
                            EventInfo menuOpenEvent = frm.GetType().GetEvent("SelectedMenuOpenEvent");

                            if (menuOpenEvent != null)
                            {
                                Type type = menuOpenEvent.EventHandlerType;
                                MethodInfo method = this.GetType().GetMethod("childFrm_SelectedMenuOpenEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                                Delegate del = Delegate.CreateDelegate(type, this, method, false);
                                menuOpenEvent.AddEventHandler(frm, del);
                            }
                            #endregion

                            #region + Delegate - 즐겨찾기 변경 후 메인 화면 트리뷰 Refresh후 포커스 이동
                            EventInfo mainFormTreeControlRefresh = frm.GetType().GetEvent("TreeControlRefreshEvent");

                            if (mainFormTreeControlRefresh != null)
                            {
                                Type type = mainFormTreeControlRefresh.EventHandlerType;
                                MethodInfo method = this.GetType().GetMethod("childFrm_TreeControlRefreshEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                                Delegate del = Delegate.CreateDelegate(type, this, method, false);
                                mainFormTreeControlRefresh.AddEventHandler(frm, del);
                            }
                            #endregion

                            #region + 탭 컨트롤에 추가하기 위해 TabItem 컨트롤 객체를 생성한다.

                            this.tabControl.SelectionChanged -= TabControl_SelectionChanged;

                            DXTabItem tabItem = new DXTabItem();
                            ScrollViewer scrollViewer = new ScrollViewer();
                            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

#if DEBUG
                            string[] arrValue = strSelectedMenuURL.Split('.');
                            strScreenName = string.Format("{0}-({1})", strSelectedMenuName, arrValue[arrValue.Length - 1]);
                            //string strAAa = $"{strSelectedMenuID}-{arrValue[arrValue.Length - 1]}";
#else
                        strScreenName       = strSelectedMenuName;
#endif

                            tabItem.Header = strScreenName;
                            tabItem.Content = frm;                                      // UserControl
                            tabItem.AllowHide = DevExpress.Utils.DefaultBoolean.True;     // 추가된 TabItem을 Remove하기 위한 버튼을 활성화한다.
                            tabItem.Width = 200;
                            tabItem.Height = 30;
                            tabItem.Margin = new Thickness(0, 0, 0, 0);
                            tabItem.Tag = strSelectedTreeID;        // TreeID
                            tabItem.Name = strMenuID;                // 메뉴 ID

                            tabItem.CloseCommand = new RelayCommand<object>(i => OnTabCloseCommand(tabItem, strSelectedTreeID));
                            tabItem.Margin = new Thickness(0, 0, 0, 0);
                            this.TabControlItems.Add(new DataModels.MainWindowDataModel() { MENU_ID = strSelectedTreeID, TAB_HEADER_TITLE = strSelectedTreeID, TAB_CONTENT = frm });

                            if (this.MainWinParam != null && this.MainWinParam.MENU_ID != null)
                            {
                                if (this.MainWinParam.MENU_ID.Length > 0)
                                {
                                    this.childFrm_TreeControlRefreshEvent();
                                    this.SettingTopMenuColor(this.g_strSelectedTopMenuID);
                                    this.MainWinParam.MENU_ID = null;
                                }
                            }
                            else
                            {
                                if (isFvrtClickYN == true)
                                {
                                    //this.BindingLeftTreeControlList(this.g_strSelectedTopMenuID);
                                    this.BindingLeftTreeControlList(this.g_strUpperMenuID);
                                    this.SettingTopMenuColor(this.g_strUpperMenuID);
                                    this.MainWinParam.MENU_ID = null;
                                }
                            }

                            int iCount = this.tabControl.Items.Count;
                            this.tabControl.Items.Insert(iCount, tabItem);
                            this.tabControl.SelectedIndex = iCount;


                            tabItem.IsSelected = true;
                            this.tabControl.SelectionChanged += TabControl_SelectionChanged;

                            //메인이미지 비표시
                            imgMain.Visibility = Visibility.Collapsed;
                            #endregion

                            break;
                    }
                }
            }
            catch (Exception err)
            {
                if (this.MainWinParam != null && this.MainWinParam.MENU_ID != null) { this.MainWinParam.MENU_ID = string.Empty; }
                this.BaseClass.Error(err);
            }
        }

        #region OnTabCloseCommand - 탭을 종료할 때 이벤트
        /// <summary>
        /// 탭을 종료할 때 이벤트
        /// </summary>
        /// <param name="tabitem"></param>
        /// <param name="MenuId"></param>
        public void OnTabCloseCommand(object tabitem, string MenuId)
        {
            try
            {
                this.tabControl.Items.Remove(tabitem);

                DataModels.MainWindowDataModel item = this.TabControlItems.Where(f => f.MENU_ID == MenuId).FirstOrDefault();

                if (item != null)
                {
                    this.TabControlItems.Remove(item);
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

        #region 탭 컨트롤
        #region tabControl_TabHidden - 오픈된 탭이 모두 제거된 경우 탭 컨트롤을 화면에서 보이지 않도록 처리
        /// <summary>
        /// 오픈된 탭이 모두 제거된 경우 탭 컨트롤을 화면에서 보이지 않도록 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_TabHidden(object sender, TabControlTabHiddenEventArgs e)
        {
            try
            {
                if (this.tabControl.Items.Count == 1)
                {
                    this.InitTabVisiabled(false);

                    int iSelectedTabIndex = e.TabIndex;
                    this.tabControl.Items.RemoveAt(iSelectedTabIndex);
                    // CHOO
                    this.TabControlItems.RemoveAt(iSelectedTabIndex);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region tabControl_TabRemoving - 탭 컨트롤 제거 시 이벤트
        /// <summary>
        /// 탭 컨트롤 제거 시 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_TabRemoving(object sender, TabControlTabRemovingEventArgs e)
        {
            foreach (var item in this.tabControl.Items)
            {
                if (e.Item == item)
                {
                    this.tabControl.Items.Remove(e.Item);
                    break;
                }
            }
        }
        #endregion

        #region tabControl_SelectionChagned - 탭 컨트롤 선택 변경 시 이벤트
        /// <summary>
        /// 탭 컨트롤 선택 변경 시 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, TabControlSelectionChangedEventArgs e)
        {
            try
            {
                DXTabControl tabCtrl        = (DXTabControl)sender;
                var iNewSelectedIndex       = e.NewSelectedIndex;
                var strSelectedMenuID       = ((FrameworkElement)tabCtrl.Items[iNewSelectedIndex]).Name;    // 현재 선택된 탭의 MenuID
                var strSelectedTopMenuID    = ((FrameworkElement)tabCtrl.Items[iNewSelectedIndex]).Tag.ToString().Substring(0, 3);

                this.BindingLeftTreeControlList(strSelectedTopMenuID);

                this.SettingTopMenuColor(strSelectedTopMenuID);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region > SettingTopMenuColor - 상단 메인 메뉴 색상 초기화
        /// <summary>
        /// 상단 메인 메뉴 색상 초기화
        /// </summary>
        /// <param name="_strSelectedTopMenuID">선택된 상위 메뉴ID</param>
        private void SettingTopMenuColor(string _strSelectedTopMenuID)
        {
            #region 상단 메인메뉴 색상 초기화
            for (int j = 0; j < this.topGridMenuArea.Children.Count; j++)
            {
                var childCtrl = this.topGridMenuArea.Children[j];

                if (childCtrl is TextBlock)
                {
                    TextBlock tbCtrl = (TextBlock)childCtrl;

                    if (tbCtrl.Tag.ToString().Equals(_strSelectedTopMenuID) == true)
                    {
                        // 현재 선택된 탭의 최상단 메뉴 ID가 동일한 경우 선택된 색상으로 변경한다.
                        this.g_strTopMenuCtrlTagName = _strSelectedTopMenuID;
                        tbCtrl.Foreground = this.BaseClass.ConvertStringToSolidColorBrush("#222222");
                    }
                    else
                    {
                        tbCtrl.Foreground = this.BaseClass.ConvertStringToSolidColorBrush("#A0A0A0");
                    }
                }
            }
            #endregion
        }

        #region 버튼
        #region 윈도우 상단 - BtnLogout_Click - 로그아웃 버튼 클릭
        /// <summary>
        /// 로그아웃 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("LogOut");
                ////20180910 Image Opacity 김형준
                //imgLogout.Opacity = 1;

                //// Msg : ASK_LOGOUT - 로그아웃 하시겠습니까?
                //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("ASK_LOGOUT");
                //this.BaseClass.MsgQust(strMessage, this.BaseInfo.country_cd);

                //if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                //{
                //    Logout frmLogin = new Logout(this.BaseInfo.country_cd);
                //    frmLogin.Show();

                //    this.Close();
                //}
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region 윈도우 상단 - BtnMinimize_Click - 메인 화면 최소화 버튼 클릭
        /// <summary>
        /// 메인 화면 최소화 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //this.WindowState = WindowState.Minimized;

                ////20180910 Image Opacity 김형준
                //imgMinimize.Opacity = 1;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region 윈도우 상단 - BtnMaximize_Click - 메인 화면 최대화 버튼 클릭
        /// <summary>
        /// 메인 화면 최대화 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;

            //    //20180907 이미지 변경
            //    if (this.WindowState == WindowState.Normal)
            //    {
            //        //this.BtnMaximizeImg.Source = new BitmapImage(new Uri(@"/Resources/Btn_hd_maximize.png", UriKind.Relative));
            //        this.BtnMaximize.Content = FindResource("maximizeImg");
            //    }
            //    else
            //    {
            //        //this.BtnMaximizeImg.Source = new BitmapImage(new Uri(@"/Resources/Btn_hd_restore.png", UriKind.Relative));
            //        this.BtnMaximize.Content = FindResource("restoreImg");
            //    }

            //    //20180910 Image Opacity 김형준
            //    Image imgMaximize = BtnMaximize.Content as Image;
            //    imgMaximize.Opacity = 1;
            //}
            //catch (Exception err)
            //{
            //    this.BaseClass.Error(err);
            //}
        }
        #endregion

        #region 윈도우 상단 - BtnWindowClose_Click - 메인 화면 창 닫기 버튼 클릭
        /// <summary>
        /// 메인 화면 창 닫기 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnWindowClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ////20180910 Image Opacity 김형준
                //imgWindowClose.Opacity = 1;

                //MessageBox.Show("Close");
                ////// Msg : ASK_WIN_CLOSE - 창을 종료하시겠습니까?
                ////string strMessage = CJFC.Modules.Utility.GetMessageByCommon("ASK_WIN_CLOSE");
                ////this.BaseClass.MsgQust(strMessage, this.BaseInfo.country_cd);

                ////if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                ////{
                ////    this.Close();
                ////}
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region 좌측 메뉴 - BtnMenuAllShow_Click - 트리 메뉴 펼치기
        /// <summary>
        /// 트리 메뉴 펼치기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMenuAllShow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                treeListView.ExpandAllNodes();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region 좌측 메뉴 - BtnMenuSelectedFocus_Click - 현재 선택된 트리 메뉴외 기타 메뉴 접기
        /// <summary>
        /// 현재 선택된 트리 메뉴외 기타 메뉴 접기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMenuSelectedFocus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeListNode treeNode = this.treeListView.FocusedNode;

                this.treeListView.CollapseAllNodes();
                this.treeListView.FocusedNode = treeNode;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region 좌측 메뉴 - BtnMenuAllHide_Click - 트리 메뉴 접기
        /// <summary>
        /// 트리 메뉴 접기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMenuAllHide_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.treeListView.CollapseAllNodes();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region 좌측 메뉴 - BtnMenuExp - 메뉴 열기
        /// <summary>
        /// 메뉴 열기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMenuExp_Click(object sender, RoutedEventArgs e)
        {
            this.BtnMenuAllShow.Visibility = Visibility.Visible;
            this.BtnMenuAllHide.Visibility = Visibility.Visible;
            this.BtnMenuSelectedFocus.Visibility = Visibility.Visible;

            this.BtnMenuExp.Visibility = Visibility.Hidden;
            this.BtnMenuFold.Visibility = Visibility.Visible;

            LayoutControl.SetAllowHorizontalSizing(this.layoutGrpTree, true);

            //20180907 김형준 treeList Border 비표시
            //treeListControl.ShowBorder  = true;
            this.layoutGrpTree.MinWidth = 200;
            this.layoutGrpTree.Width = 190;
            this.RunAnimate(199);
            this.treeListView.Visibility = Visibility.Visible;
            //20180907 김형준
            LayoutControl.SetAllowHorizontalSizing(this.layoutGrpTab, true);
        }
        #endregion

        #region 좌측 메뉴 - BtnMenuFold_Click - 메뉴 닫기
        /// <summary>
        /// 메뉴 닫기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMenuFold_Click(object sender, RoutedEventArgs e)
        {
            this.BtnMenuAllShow.Visibility = Visibility.Hidden;
            this.BtnMenuAllHide.Visibility = Visibility.Hidden;
            this.BtnMenuSelectedFocus.Visibility = Visibility.Hidden;

            this.BtnMenuExp.Visibility = Visibility.Visible;
            this.BtnMenuFold.Visibility = Visibility.Hidden;

            LayoutControl.SetAllowHorizontalSizing(this.layoutGrpTree, false);

            treeListControl.ShowBorder = false;
            this.layoutGrpTree.MinWidth = 30;
            this.layoutGrpTree.Width = 30;
            this.RunAnimate(30);
            this.treeListView.Visibility = Visibility.Hidden;

            //20180907 김형준
            LayoutControl.SetAllowHorizontalSizing(this.layoutGrpTab, false);
        }
        #endregion

        #region 공지사항 이미지 클릭
        private void GridAreaNotice_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.MainWinParam.MENU_ID = "C1006";    // 메뉴 ID (게시판)

                this.MoveNoticeClickMenu(this.MainWinParam.MENU_ID);

                this.gridAreaNoticeCount.Visibility = Visibility.Hidden;

                // 공지 알람 버튼 영역 클릭 이벤트
                this.gridAreaNotice.PreviewMouseLeftButtonUp -= GridAreaNotice_PreviewMouseLeftButtonUp;
                this.gridAreaNotice.Cursor      = Cursors.None;
                MainWinParam.MENU_ID = null;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region 공지사항 이미지에 마우스 오버 상태일 때 이미지 투명도 조절
        /// <summary>
        /// 공지사항 이미지에 마우스 오버 상태일 때 이미지 투명도 조절
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridAreaNotice_MouseEnter(object sender, MouseEventArgs e)
        {
            //this.imgNotice.Opacity = 0.9;
        }
        #endregion

        #region 공지사항 이미지 클릭시 이미지 투명도 조절
        /// <summary>
        /// 공지사항 이미지 클릭시 이미지 투명도 조절
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridAreaNotice_MouseLeave(object sender, MouseEventArgs e)
        {
            //this.imgNotice.Opacity = 0.7;
        }
        #endregion
        #endregion

        #region > Delegate - 메인 상태바
        /// <summary>
        /// Delegate - 메인 상태바
        /// </summary>
        /// <param name="value"></param>
        private void childFrm_ToolStripChangeStatusLabelEvent(string value)
        {
            // 자식창에서 설정한 메세지를 메인창 하단에 출력
            this.txtMsg.Text = value;
        }
        #endregion

        #region > Delegate - 즐겨찾기 변경 후 메인 화면 트리뷰 Refresh후 포커스 이동
        /// <summary>
        /// 즐겨찾기 변경 후 메인 화면 트리뷰 Refresh후 포커스 이동
        /// </summary>
        private void childFrm_TreeControlRefreshEvent()
        {
            var iCurrentRowIndex = this.BaseClass.GetCurrentTreeListControlRowIndex(this.treeListView);

            var dtMenuList = this.GetMenuList().Tables[0];
            if (dtMenuList.Rows.Count > 0)
            {
                string strFirstMenuID = this.g_strSelectedTopMenuID;

                //for (int i = 0; i < this.topGridMenuArea.Children.Count; i++)
                //{
                //    var childCtrl = this.topGridMenuArea.Children[i];

                //    if (childCtrl is TextBlock)
                //    {
                //        TextBlock tbCtrl = (TextBlock)childCtrl;
                //        strFirstMenuID = tbCtrl.Tag.ToString();
                //        this.g_strSelectedTopMenuID     = tbCtrl.Tag.ToString();    // 현재 선택된 상단 메뉴 ID
                //        this.g_strSelectedTopMenuName   = tbCtrl.Text;
                //        break;
                //    }
                //}

                // strTextBlockTag 값이 0이면 바인딩 된 상위메뉴가 없는 경우
                if (strFirstMenuID.Length == 0) { return; }

                DataTable dtFilterTreeMenuList = null;

                #region + 좌측 트리 리스트 구성
                var liFilterTreeMenuList = this.GetMenuList().Tables[0].AsEnumerable().Where(p => p.Field<string>("TREE_ID").StartsWith(strFirstMenuID)
                                           && p.Field<string>("TREE_ID").Length > 3).Union
                                            (dtMenuList.AsEnumerable().Where(p => p.Field<string>("TREE_ID").StartsWith("FVRT")));

                if (liFilterTreeMenuList != null)
                {
                    if (liFilterTreeMenuList.Count() > 0)
                    {
                        dtFilterTreeMenuList = liFilterTreeMenuList.CopyToDataTable();

                        foreach (DataRow drRow in dtFilterTreeMenuList.Rows)
                        {
                            if (drRow["MENU_NM"].Equals("FVRT") == true)
                            {
                                drRow["MENU_NM"] = this.BaseClass.GetResourceValue("FVRT");
                                dtFilterTreeMenuList.AcceptChanges();
                                break;
                            }
                        }
                    }
                }
                #endregion

                #region + 메뉴 리스트 (트리 리스트) 컨트롤 바인딩
                if (dtFilterTreeMenuList == null)
                {
                    this.MenuList.ToObservableCollection(null);
                }
                else
                {
                    if (dtFilterTreeMenuList.Rows.Count > 0)
                    {
                        this.MenuList = new ObservableCollection<MainWindowDataModel>();
                        this.MenuList.ToObservableCollection(dtFilterTreeMenuList);
                    }
                    else
                    {
                        this.MenuList.ToObservableCollection(null);
                    }
                }

                this.treeListControl.ItemsSource = this.MenuList;
                this.treeListControl.RefreshData();
                #endregion
            }

            //this.BaseClass.SetTreeListControlRowAddFocus(this.treeListControl, iCurrentRowIndex);
        }
        #endregion

        #region > Delegate - 자식창에서 부모창을 호출하여 화면을 오픈한다.
        private void childFrm_SelectedMenuOpenEvent(MainWinParam _liValue)
        {
            try
            {
                this.MainWinParam   = _liValue;
                this.TreeListControl_PreviewMouseLeftButtonUp(null, null);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        private void TabControl_TabHiding(object sender, TabControlTabHidingEventArgs e)
        {
            //// 사용시 SBOX201 소스 참고
            //// 1. 클래스 상속

            var tabitem = this.tabControl.Items[e.TabIndex];

            if (tabitem is DevExpress.Xpf.Core.DXTabItem)
            {
                var _item = tabitem as DevExpress.Xpf.Core.DXTabItem;

                if (_item.Content != null)
                {
                    if (_item.Content is SMART.WCS.Control.Modules.Interface.TabCloseInterface)
                    {
                        var _result = (_item.Content as SMART.WCS.Control.Modules.Interface.TabCloseInterface).TabClosing();
                        e.Cancel = !_result;
                    }
                }
            }
        }

        #region MainMenuSubItem_ItemClick - 상단 메뉴 클릭 이벤트
        /// <summary>
        /// 상단 메뉴 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuSubItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            var strSelectedMenuID = e.Item.Tag.ToString();
            var strSelctedMenuNm = e.Item.Content.ToString();

            try
            {
                this.BindingLeftTreeControlList(strSelectedMenuID);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        //20180912 hj.kim
        #region 메뉴선택 로그 저장
        private void SaveMenuAccessInfo()//(string strMenuId)
        {
            try
            {
                //string _USER_ID = this.BaseInfo.user_id.ToString();
                //string _MENU_ID = strMenuId;
                //string _CNTRY_CD = this.BaseInfo.country_cd.ToString();
                //string _ACT_DESC = "MENU_ACCESS";

                ////트랜잭션과 관련된 확인이 필요
                //using (DataAccess da = new DataAccess())
                //{
                //    DataSet _outData = new System.Data.DataSet();

                //    string _callProc = null;
                //    string strOutputParam = "P_RESULT";

                //    var dicParam = new Dictionary<object, object>
                //    {
                //        {"P_USER_ID", _USER_ID},
                //        {"P_MENU_ID", _MENU_ID},
                //        {"P_CNTRY_CD", _CNTRY_CD},
                //        {"P_ACT_DESC", _ACT_DESC}
                //    };

                //    _callProc = "COMADM.PK_WWCS117.SP_COMM_UI_LOG_INSERT";

                //    if (dicParam.Count() > 0 && !string.IsNullOrWhiteSpace(_callProc))
                //    {
                //        _outData = da.GetSpDataSet(this.BaseInfo.db_conn_info// 데이터베이스 연결 문자열
                //                , _callProc       // 프로시저명
                //                , dicParam        // Input 파라메터
                //                , strOutputParam  // Output 파라메터
                //        );

                //        if (_outData == null)
                //        {
                //            //로그작성필요 !!!
                //            //return;
                //        }
                //    }
                //}
            }
            catch
            {
                //로그작성필요 !!!
                //throw;
            }
        }

        private void LblNoticeInfo_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("공지사항 팝업창 작업 예정");
        }
        #endregion

        //20180920 hj.kim 외부프로세스 실행 start
        #region 외부 프로세스 실행
        /// <summary>
        /// 외부 프로그램 실행
        /// </summary>
        /// <param name="_strMenuID">트리내 메뉴 ID</param>
        /// <param name="_strMenuType">트리내 메뉴 타입</param>
        private void ProcessExecute(string _strMenuID, string _strMenuType)
        {
            Process process = new Process();

            try
            {
                string strMenuUrl           = this.GetTreeListControlSelectedValue("MENU_URL").ToString();     // 메뉴 경로
                string strUserID            = string.Empty;     // 사용자 ID
                string strUserNM            = string.Empty;     // 사용자 명  
                string strRoleCode          = string.Empty;     // 권한코드
                string strCenterCode        = string.Empty;     // 센터코드
                string strCenterNM          = string.Empty;     // 센터명
                string strLanguage          = string.Empty;     // 언어코드

                switch (_strMenuType.ToUpper())
                {
                    case "SCADA":
                        #region + SCADA
                        var strScadaUrl     = string.Empty;
                        strCenterCode       = this.BaseClass.CenterCD;      // 권한코드

                        if (strCenterCode.Equals("BC") == true)
                        {
                            // 부천센터
                            //strScadaUrl     = @"C:\SMART WCS\SCADA Bucheon\SCADA.UI.exe";
                            strScadaUrl     = this.BaseClass.GetAppSettings("SCADA_BC");
                        }
                        else
                        {
                            // 양산센터
                            //strScadaUrl     = @"C:\SMART WCS\SCADA Yangsan\SCADA.UI.exe";
                            strScadaUrl     = this.BaseClass.GetAppSettings("SCADA_YS");
                        }

                        ProcessStartInfo pScadaInfo = new ProcessStartInfo(strScadaUrl);
                        pScadaInfo.UseShellExecute  = false;
                        pScadaInfo.CreateNoWindow   = false;

                        if (System.IO.File.Exists(strScadaUrl) == true)
                        {
                            /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
                                * Argument
                                * [0] : 사용자 ID
                                * [1] : 사용자명
                                * [2] : 권한코드
                                * [3] : 센터코드
                                * [4] : 센터명
                                * [5] : 언어코드
                                *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/ 
                            strUserID           = this.BaseClass.UserID;            // [0] 사용자 ID
                            strUserNM           = this.lblUserName.Text.Trim();     // [1] 사용자 명  
                            strRoleCode         = this.BaseClass.RoleCode;          // [2] 권한코드
                            strCenterCode       = this.BaseClass.CenterCD;          // [3] 센터코드
                            strCenterNM         = this.lblCenterName.Text.Trim();   // [4] 센터명
                            strLanguage         = this.BaseClass.CountryCode;       // [5] 언어코드

                            pScadaInfo.Arguments = $"{strUserID} {strUserNM} {strRoleCode} {strCenterCode} {strCenterNM} {strLanguage}";
                        }
                        else
                        {
                            this.BaseClass.MsgError("ERR_NOT_EXIST_PATH");
                            return;
                        }

                        process.StartInfo = pScadaInfo;
                        process.Start();
                        #endregion
                        break;

                    case "KIOSK":
                        #region + KIOSK (LG 생활건강)
                        using (Kiosk.Lib.KioskMain frmKiosk = new Kiosk.Lib.KioskMain())
                        {

                            frmKiosk.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            frmKiosk.ShowDialog();
                        }
                        #endregion

                        //#region + KIOSK (롯데마트)
                        //MessageBox.Show(this.BaseClass.RoleCode);
                        //ProcessStartInfo pKioskInfo = new ProcessStartInfo(@"C:\\SMART.WCS\KIOSK\SMART.WCS.Kiosk.exe");
                        //pKioskInfo.UseShellExecute      = false;
                        //pKioskInfo.CreateNoWindow       = false;
    
                        //if (System.IO.File.Exists(@"C:\\SMART.WCS\KIOSK\SMART.WCS.Kiosk.exe") == true)
                        //{
                        //    /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
                        //     * Argument
                        //     * [0] : 사용자 ID
                        //     * [1] : 사용자명
                        //     * [2] : 권한코드
                        //     * [3] : 센터코드
                        //     * [4] : 센터명
                        //     * [5] : 언어코드
                        //     *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/ 
                        //    strUserID           = this.BaseClass.UserID;            // [0] 사용자 ID
                        //    strUserNM           = this.lblUserName.Text.Trim();     // [1] 사용자 명  
                        //    strRoleCode         = this.BaseClass.RoleCode;          // [2] 권한코드
                        //    strCenterCode       = this.BaseClass.CenterCD;          // [3] 센터코드
                        //    strCenterNM         = this.lblCenterName.Text.Trim();   // [4] 센터명
                        //    strLanguage         = this.BaseClass.CountryCode;       // [5] 언어코드

                        //    pKioskInfo.Arguments = $"{strUserID} {strUserNM} {strRoleCode} {strCenterCode} {strCenterNM} {strLanguage}";
                        //}
                        //else
                        //{
                        //    this.BaseClass.MsgError("ERR_NOT_EXIST_PATH");
                        //    return;
                        //}
                        
                        //process.StartInfo = pKioskInfo;
                        //process.Start();
                        //#endregion
                        break;

                    case "INT_EXE":

                        break;
                    //case "EXT_EXE":
                    //    pInfo.UseShellExecute = false;
                    //    pInfo.CreateNoWindow = false;

                    //    pInfo.FileName = this.GetTreeListControlSelectedValue("MENU_URL").ToString();
                    //    pInfo.Arguments = strToken + " " + this.BaseInfo.user_id;

                    //    process.StartInfo = pInfo;
                    //    process.Start();
                    //    break;
                    //case "EXT_WEB":
                    //    pInfo.UseShellExecute = false;
                    //    pInfo.CreateNoWindow = false;

                    //    string strUrl = this.GetTreeListControlSelectedValue("MENU_URL").ToString();
                    //    string strPostData = "?token=" + strToken + "&user_id=" + this.BaseInfo.user_id;

                    //    //if (!Uri.IsWellFormedUriString(strUrl, UriKind.RelativeOrAbsolute))
                    //    if (string.IsNullOrWhiteSpace(strUrl))
                    //    {
                    //        process.Dispose();
                    //        return;
                    //    }

                    //    //pInfo = new ProcessStartInfo(strUrl + strPostData);
                    //    pInfo = new ProcessStartInfo("chrome.exe", strUrl + strPostData);
                    //    process.StartInfo = pInfo;
                    //    process.Start();

                    //    break;

                    default: break;

                }
            }
            catch
            {
                process.Dispose();
                throw;
            }
        }
        //20180920 hj.kim 외부프로세스 실행 end
        #endregion
        #endregion
        #endregion
        #endregion

        // Library 복사 로직
        //C:\Windows\System32\xcopy "$(ProjectDir)Library\Oracle.ManagedDataAccess.dll" "$(SolutionDir)bin\$(ConfigurationName)\*" /f /y
        //C:\Windows\System32\xcopy "$(ProjectDir)Library\MySql.Data.dll" "$(SolutionDir)bin\$(ConfigurationName)\*" /f /y
        //C:\Windows\System32\xcopy "$(ProjectDir)Library\WPFToolkit.Extended.dll" "$(SolutionDir)bin\$(ConfigurationName)\*" /f /y
        //C:\Windows\System32\xcopy "$(ProjectDir)Library\LGCNS.ezControl.dll" "$(SolutionDir)bin\$(ConfigurationName)\*" /f /y
        //C:\Windows\System32\xcopy "$(ProjectDir)Library\LGCNS.ezControl.Presentation.dll" "$(SolutionDir)bin\$(ConfigurationName)\*" /f /y
        //C:\Windows\System32\xcopy "$(ProjectDir)Library\DevExpress.Xpf.Themes.Office2016White.v19.1.dll" "$(SolutionDir)bin\$(ConfigurationName)\*" /f /y
        //C:\Windows\System32\xcopy "$(ProjectDir)Library\DevExpress.Xpf.Themes.Office2016WhiteSE.v19.1.dll" "$(SolutionDir)bin\$(ConfigurationName)\*" /f /y
    }
}
