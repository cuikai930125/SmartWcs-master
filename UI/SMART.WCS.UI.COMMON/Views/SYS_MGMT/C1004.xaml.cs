using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.UI.COMMON.DataMembers.C1004;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SMART.WCS.UI.COMMON.Views.SYS_MGMT
{
    /// <summary>
    /// 메뉴 관리
    /// </summary>
    public partial class C1004 : UserControl, TabCloseInterface
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
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        /// <summary>
        /// 화면 로드 여부
        /// </summary>
        private bool g_isLoaded = false;
        #endregion

        #region ▩ 생성자
        public C1004()
        {
            InitializeComponent();
        }

        public C1004(List<string> _liMenuNavigation)
        {
            InitializeComponent();

            try
            {
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

                this.RoleCD_ComboBox = this.BaseClass.CenterCD;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1004), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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
                TreeListView view = source as TreeListView;
                view.ShowingEditor += View_ShowingEditor;
            }
        }
        #endregion

        #region > 메뉴 관리
        public static readonly DependencyProperty MenuMgntListProperty
            = DependencyProperty.Register("MenuMgntList", typeof(ObservableCollection<MenuMgmt>), typeof(C1004)
                , new PropertyMetadata(new ObservableCollection<MenuMgmt>()));

        public ObservableCollection<MenuMgmt> MenuMgntList
        {
            get { return (ObservableCollection<MenuMgmt>)GetValue(MenuMgntListProperty); }
            set { SetValue(MenuMgntListProperty, value); }
        }
        #endregion

        #region > Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1004), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string GridRowCount
        {
            get { return (string)GetValue(GridRowCountProperty); }
            set { SetValue(GridRowCountProperty, value); }
        }
        #endregion

        public string RoleCD_ComboBox { get; set; }
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        private void InitControl()
        {
            // 콤보박스 설정
            this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, true);

            // ContextMenu 리소스 설정
            this.menuItemRowAdd.Header      = this.BaseClass.GetResourceValue("ROW_ADD");   // 행추가
            this.menuItemRowDelete.Header   = this.BaseClass.GetResourceValue("ROW_DEL");   // 행삭제
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 로드 이벤트
            this.Loaded += C1004_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 조회 버튼 클릭 이벤트
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드 버튼 클릭 이벤트
            this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            // 저장 버튼 클릭 이벤트
            this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;

            // 트리 컨트롤 전체 펼침 버튼 클릭 이벤트
            this.btnAllOpen.PreviewMouseLeftButtonUp += BtnAllOpen_PreviewMouseLeftButtonUp;
            // 트리 컨트롤 전체 닫힘 버튼 클릭 이벤트
            this.btnAllClose.PreviewMouseLeftButtonUp += BtnAllClose_PreviewMouseLeftButtonUp;
            #endregion

            #region + 트리 이벤트
            // 트리 리스트 내 클릭 이벤트
            this.treeListControl.PreviewMouseLeftButtonUp += TreeListControl_PreviewMouseLeftButtonUp;
            // 트리 리스트 ContextMenu (행추가) 클릭 이벤트
            this.menuItemRowAdd.PreviewMouseLeftButtonUp += MenuItemRowAdd_PreviewMouseLeftButtonUp;
            // 트리 리스트 ContextMenu (행삭제) 클릭 이벤트
            this.menuItemRowDelete.PreviewMouseLeftButtonUp += MenuItemRowDelete_PreviewMouseLeftButtonUp;
            #endregion
        }
        #endregion
        #endregion

        #region > 기타 함수
        #region >> SetResultText - 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        /// <param name="_iTabIndex">Tab Index값</param>
        private void SetResultText()
        {
            var strResource         = this.BaseClass.GetResourceValue("TOT_DATA_CNT");              // 텍스트 리소스
            var iTotalRowCount      = (this.treeListControl.ItemsSource as ICollection).Count;      // 전체 데이터 수
            this.GridRowCount       = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";        // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource             = this.BaseClass.GetResourceValue("DATA_INQ");                  // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
        }
        #endregion

        #region >> TabClosing - 탭을 닫을 때 데이터 저장 여부 체크
        /// <summary>
        /// 탭을 닫을 때 데이터 저장 여부 체크
        /// </summary>
        /// <returns></returns>
        public bool TabClosing()
        {
            return this.CheckModifyData();
        }
        #endregion

        #region >> CheckModifyData - 데이터 저장 여부를 체크한다.
        /// <summary>
        /// 데이터 저장 여부를 체크한다.
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyData()
        {   
            bool bRtnValue              = true;

            if (this.MenuMgntList.Any(p => p.IsNew || p.IsDelete || p.IsUpdate) == true)
            {
                // Msg - 저장되지 않은 데이터가 있습니다.|조회 하시겠습니까?
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE_INQ");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            }
            
            return bRtnValue;
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_MENU_LIST_INQ - 메뉴 관리 리스트 조회
        /// <summary>
        /// 메뉴 관리 리스트 조회
        /// </summary>
        /// <returns></returns>
        private async Task GetSP_MENU_LIST_INQ()
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1004.SP_MENU_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_MENU_LIST", "O_RSLT" };

            var strCenterCD     = this.BaseClass.CenterCD;                                  // 센터코드
            var strMenuID       = this.txtMenuID.Text.Trim();                               // 메뉴 ID
            var strMenuNM       = this.txtMenuNM.Text.Trim();                               // 메뉴명
            var strUserYN       = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);   // 사용여부
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCenterCD);       // 센터코드
            dicInputParam.Add("P_MENU_ID",          strMenuID);         // 메뉴 ID
            dicInputParam.Add("P_MENU_NM",          strMenuNM);         // 메뉴명
            dicInputParam.Add("P_USE_YN",           strUserYN);         // 사용여부
            #endregion

            #region + 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            if (dsRtnValue == null) { return; }

            string strErrCode           = string.Empty;     // 오류 코드
            string strErrMsg            = string.Empty;     // 오류 메세지

            if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
            {
                // 정상 처리된 경우
                this.MenuMgntList   = new ObservableCollection<MenuMgmt>();
                this.MenuMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
            }
            else
            {
                // 오류가 발생한 경우
                this.MenuMgntList.ToObservableCollection(null);
                this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
            }

            // 데이터 바인딩
            this.treeListControl.ItemsSource = this.MenuMgntList;

            this.treeListView.ExpandAllNodes();
        }
        #endregion

        #region >> InsertSP_MENU_INS - 메뉴 관리 신규 데이터 저장
        /// <summary>
        /// 메뉴 관리 신규 데이터 저장
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_item">신규 저장 대상 데이터</param>
        /// <returns></returns>
        private async Task<bool> InsertSP_MENU_INS(BaseDataAccess _da, MenuMgmt _item)
        {
            bool isRtnValue     = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_C1004.SP_MENU_INS";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                      // 센터코드
            var strMenuID           = _item.MENU_ID;                                // 메뉴 ID
            var strMenuNM           = _item.MENU_NM;                                // 메뉴명
            var strMenuDesc         = _item.MENU_DESC;                              // 메뉴 상세
            var strMenuLevel        = _item.MENU_LVL;                               // 메뉴 레벨
            var strMenuType         = _item.MENU_TYPE;                              // 메뉴 타입
            var strMenuUrl          = _item.MENU_URL;                               // 메뉴 URL
            var strMenuIcon         = _item.MENU_ICON;                              // 메뉴 아이콘
            var strTreeID           = _item.TREE_ID;                                // 트리 ID
            var strParentID         = _item.PARENT_ID;                              // 상위 메뉴 ID
            var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";     // 사용여부
            var strUserID           = this.BaseClass.UserID;                        // 사용자 ID
            var iSortSeq            = _item.SORT_SEQ;                               // 정렬 순서
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);           // 센터코드   
            dicInputParam.Add("P_MENU_ID",              strMenuID);             // 메뉴 ID
            dicInputParam.Add("P_MENU_NM",              strMenuNM);             // 메뉴명
            dicInputParam.Add("P_MENU_DESC",            strMenuDesc);           // 메뉴 상세
            dicInputParam.Add("P_MENU_LVL",             strMenuLevel);          // 메뉴 레벨
            dicInputParam.Add("P_MENU_TYPE",            strMenuType);           // 메뉴 타입
            dicInputParam.Add("P_MENU_URL",             strMenuUrl);            // 메뉴 URL
            dicInputParam.Add("P_MENU_ICON",            strMenuIcon);           // 메뉴 아이콘
            dicInputParam.Add("P_TREE_ID",              strTreeID);             // 트리 ID
            dicInputParam.Add("P_PARENT_ID",            strParentID);           // 상위 메뉴 ID
            dicInputParam.Add("P_USE_YN",               strUseYN);              // 사용여부
            dicInputParam.Add("P_USER_ID",              strUserID);             // 사용자 ID
            dicInputParam.Add("P_SORT_SEQ",             iSortSeq);              // 정렬 순서
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);
            #endregion

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
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

        #region >> UpdateSP_MENU_UPD - 메뉴 관리 데이터 수정
        /// <summary>
        /// 메뉴 관리 데이터 수정
        /// </summary>
        /// <param name="_da"></param>
        /// <param name="_item"></param>
        /// <returns></returns>
        private async Task<bool> UpdateSP_MENU_UPD(BaseDataAccess _da, MenuMgmt _item)
        {
            bool isRtnValue     = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_C1004.SP_MENU_UPD";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                      // 센터코드
            var strMenuID           = _item.MENU_ID;                                // 메뉴 ID
            var strMenuNM           = _item.MENU_NM;                                // 메뉴명
            var strMenuDesc         = string.IsNullOrWhiteSpace(_item.MENU_DESC);   // 메뉴 상세
            var strMenuType         = _item.MENU_TYPE;                              // 메뉴 타입
            var strMenuUrl          = _item.MENU_URL;                               // 메뉴 URL
            var strMenuIcon         = _item.MENU_ICON;                              // 메뉴 아이콘
            //var strParentID         = _item.PARENT_ID;                              // 상위 메뉴 ID
            var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";     // 사용여부
            var strSortSeq          = _item.SORT_SEQ.ToString();                    // 정렬 순서
            var strUserID           = this.BaseClass.UserID;                        // 사용자 ID
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);           // 센터코드   
            dicInputParam.Add("P_MENU_ID",              strMenuID);             // 메뉴 ID
            dicInputParam.Add("P_MENU_NM",              strMenuNM);             // 메뉴명
            dicInputParam.Add("P_MENU_DESC",            strMenuDesc);           // 메뉴 상세
            dicInputParam.Add("P_MENU_TYPE",            strMenuType);           // 메뉴 타입
            dicInputParam.Add("P_MENU_URL",             strMenuUrl);            // 메뉴 URL
            dicInputParam.Add("P_MENU_ICON",            strMenuIcon);           // 메뉴 아이콘
            //dicInputParam.Add("P_PARENT_ID",            strParentID);           // 상위 메뉴 ID
            dicInputParam.Add("P_USE_YN",               strUseYN);              // 사용여부
            dicInputParam.Add("P_SORT_SEQ",             strSortSeq);            // 정렬 순서
            dicInputParam.Add("P_USER_ID",              strUserID);             // 사용자 ID
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);
            #endregion

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
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
        #region > 로드 이벤트
        #region >> 화면 로드 이벤트
        /// <summary>
        /// 화면 로드 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void C1004_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.g_isLoaded == true) { return; }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 메뉴 관리 리스트
                await this.GetSP_MENU_LIST_INQ();

                this.g_isLoaded = true;
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
        #endregion

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
                if (CheckModifyData() == false) { return; }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 메뉴 관리 리스트
                await this.GetSP_MENU_LIST_INQ();
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

        #region >> 엑셀다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 엑셀다운로드 버튼 클릭 이벤트
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

                List<TreeListView> tv = new List<TreeListView>();
                tv.Add(this.treeListView);

                this.BaseClass.GetExcelDownload(tv);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 삭제
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        #region >> 저장 버튼 클릭 이벤트
        /// <summary>
        /// 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // ASK_SAVE - 저장 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue             = false;
                // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                string strInputMessage      = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.MESSAGE);
                // ERR_NOT_SELECT - {0}이(가) 선택되지 않았습니다.
                string strSelectMessage     = this.BaseClass.GetResourceValue("ERR_NOT_SELECT", BaseEnumClass.ResourceType.MESSAGE);

                this.MenuMgntList.ForEach(p => p.ClearError());

                foreach (var item in this.MenuMgntList)
                {
                    if (item.MENU_LVL > 1)
                    {
                        if (item.IsNew || item.IsUpdate)
                        {
                            if (string.IsNullOrWhiteSpace(item.MENU_ID) == true)
                            {
                                item.CellError("MENU_ID", string.Format(strInputMessage, this.BaseClass.GetResourceValue("MENU_ID")));
                                return;
                            }

                            if (string.IsNullOrWhiteSpace(item.MENU_TYPE) == true)
                            {
                                item.CellError("MENU_TYPE", string.Format(strSelectMessage, this.BaseClass.GetResourceValue("MENU_TYPE")));
                                return;
                            }
                        }
                    }
                }

                var liInsertRowData     = this.MenuMgntList.Where(p => p.IsNew == true).ToList();
                var liUpdateRowData     = this.MenuMgntList.Where(p => p.IsUpdate == true && p.IsNew == false).ToList();

                #region + 데이터 저장
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        // 트랜잭션 시작
                        da.BeginTransaction();

                        foreach (var item in liInsertRowData)
                        {
                            isRtnValue  = await this.InsertSP_MENU_INS(da, item);
                            if (isRtnValue == false) { break; }
                        }

                        foreach (var item in liUpdateRowData)
                        {
                            isRtnValue = await this.UpdateSP_MENU_UPD(da, item);
                            if (isRtnValue == false) { break; }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();

                            // CMPT_SAVE - 저장 되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");

                            // 메뉴 관리 리스트
                            await this.GetSP_MENU_LIST_INQ();
                        }
                        //else
                        //{
                        //    // ERR_SAVE - 저장 중 오류가 발생 했습니다.
                        //    this.BaseClass.MsgError("ERR_SAVE");
                        //}
                    }
                    catch { throw; }
                    finally
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        // 상태바 (아이콘) 제거
                        this.loadingScreen.IsSplashScreenShown = false;
                    }
                }
                #endregion

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 트리 컨트롤 전체 펼침 버튼 클릭 이벤트
        /// <summary>
        /// 트리 컨트롤 전체 펼침 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAllOpen_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.treeListView.ExpandAllNodes();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 트리 컨트롤 전체 닫힘 버튼 클릭 이벤트
        /// <summary>
        /// 트리 컨트롤 전체 닫힘 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAllClose_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
        #endregion

        #region > 트리 관련 이벤트
        /// <summary>
        /// 트리 관련 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TreeListControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view                    = (sender as TreeListControl).View as TreeListView;
                var hitInfo                 = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hitInfo.Column == null) { return; }

                var strSelectedMenuID       = (this.treeListControl.SelectedItem as MenuMgmt).MENU_ID;              // 메뉴 ID
                var strSelectedTreeID       = (this.treeListControl.SelectedItem as MenuMgmt).TREE_ID;              // 트리 ID
                bool isUseYN                = !(this.treeListControl.SelectedItem as MenuMgmt).USE_YN_CHECKED;      // 사용 여부
                bool isNewYN                = (this.treeListControl.SelectedItem as MenuMgmt).IsNew;                // 신규 추가 여부
            
                if (isNewYN == true) { return; }

                var liCurrentRowData    = this.MenuMgntList.Where(p => p.MENU_ID.Equals(strSelectedMenuID) == true).ToList();
                var liFilterData        = this.MenuMgntList.Where(p => p.PARENT_ID.Equals(strSelectedTreeID) == true).ToList();

                if (hitInfo.InRowCell)
                {
                    switch (hitInfo.Column.FieldName)
                    {
                        case "MENU_TYPE":
                            if (view.ActiveEditor == null)
                            {
                                view.ShowEditor();

                                if (view.ActiveEditor == null) { return; }
                                await Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                                }), DispatcherPriority.Render);
                            }
                            break;

                        case "USE_YN":
                            if (view.ActiveEditor == null)
                            {
                                view.ShowEditor();

                                if (view.ActiveEditor == null) { return; }
                                await Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                                }), DispatcherPriority.Render);
                            }

                            if (liFilterData.Count > 0)
                            {
                                if (isUseYN == true)
                                {
                                    foreach (var item in liFilterData)
                                    {
                                        if (item.IsNew == false)
                                        {
                                            item.USE_YN = "Y";
                                            item.IsSelected = true;

                                            if (item.IsNew == false)
                                            {
                                                item.IsUpdate = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var item in liFilterData)
                                    {
                                        if (item.IsNew == false)
                                        {
                                            item.USE_YN = "N";
                                            item.IsSelected = true;

                                            if (item.IsNew == false)
                                            {
                                                item.IsUpdate = true;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #region >> MenuItem - (행추가) ContextMenu 이벤트
        /// <summary>
        /// (행추가) ContextMenu 이벤트 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemRowAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string strSelectedMenuID    = (this.treeListControl.SelectedItem as MenuMgmt).MENU_ID;      // 메뉴 ID
                string strSelectedTreeID    = (this.treeListControl.SelectedItem as MenuMgmt).TREE_ID;      // 트리 ID
                var iSelectedMenuLevel      = (this.treeListControl.SelectedItem as MenuMgmt).MENU_LVL;     // 메뉴 레벨
                var strSelectedNewRowYN     = (this.treeListControl.SelectedItem as MenuMgmt).IsNew;        // 신규 여부

                // 신규 추가된 Row의 경우 하위 Row를 추가하지 않기 때문에
                // isNew가 true인 경우 구문을 리턴한다.
                if (strSelectedNewRowYN == true) { return; }

                var liFilterData = this.MenuMgntList.Where(p => p.PARENT_ID.Equals(strSelectedTreeID) == true).ToList();

                int i = 0;
                foreach (var item in this.MenuMgntList)
                {
                    if (item.MENU_ID.Equals(strSelectedMenuID) == true) { break; }
                    i++;
                }

                string strTreeIDFirst       = string.Empty;
                string strTreeID            = string.Empty;
                int iTreeIDSecond           = 0;
                int iSortSeq                = 0;

                if (liFilterData.Count > 0)
                {
                    strTreeID       = liFilterData.OrderByDescending(p => p.TREE_ID).FirstOrDefault().TREE_ID;
                    iSortSeq        = liFilterData.OrderByDescending(p => p.SORT_SEQ).FirstOrDefault().SORT_SEQ + 1;
                    strTreeIDFirst  = strTreeID.Substring(0, strTreeID.Length - 2);
                    iTreeIDSecond   = Convert.ToInt32(strTreeID.Substring(strTreeID.Length - 2, 2)) + 1;
                    strTreeID       = $"{strTreeIDFirst}{iTreeIDSecond.ToString("D2")}";
                }
                else
                {
                    iSortSeq        = 1;
                    strTreeID       = $"{strSelectedTreeID}01";
                }

                var newItem = new MenuMgmt
                {
                        MENU_ID         = string.Empty              // 메뉴 ID
                    ,   MENU_NM         = string.Empty              // 메뉴명
                    ,   MENU_DESC       = string.Empty              // 메뉴 상세
                    ,   MENU_LVL        = iSelectedMenuLevel + 1    // 메뉴 레벨
                    ,   MENU_TYPE       = string.Empty              // 메뉴 타입
                    ,   MENU_URL        = string.Empty              // 메뉴 URL
                    ,   MENU_ICON       = string.Empty              // 메뉴 아이콘
                    ,   TREE_ID         = strTreeID                 // 트리 ID
                    ,   PARENT_ID       = strSelectedTreeID         // 상위 메뉴 ID
                    ,   USE_YN          = "Y"                       // 사용 여부
                    ,   SORT_SEQ        = iSortSeq                  // 정렬 순서
                    ,   IsSelected      = true
                    ,   IsNew           = true
                    ,   IsUpdate        = false
                };

                this.MenuMgntList.Add(newItem);
                this.treeListControl.Focus();
                this.treeListControl.CurrentColumn  = this.treeListControl.Columns.First();
                this.treeListControl.View.FocusedRowHandle  = this.MenuMgntList.Count - 1;

                this.MenuMgntList[this.MenuMgntList.Count - 1].BackgroundBrush      = new SolidColorBrush(Colors.GhostWhite);
                this.MenuMgntList[this.MenuMgntList.Count - 1].BaseBackgroundBrush  = new SolidColorBrush(Colors.GhostWhite);


                #region 추가된 트리의 상위 메뉴 ID로 포커스를 이동하기 위한 구문
                int j = 0;
                for (j = 0; j < this.treeListControl.VisibleRowCount; j++)
                {   
                    var rowData         = this.treeListControl.GetRow(j);
                    var strMenuID       = ((SMART.WCS.UI.COMMON.DataMembers.C1004.MenuMgmt)rowData).MENU_ID;

                    if (strSelectedMenuID.Equals(strMenuID) == true) { break; }
                }

                this.BaseClass.SetTreeListControlRowAddFocus(this.treeListControl, j);
                #endregion

                this.treeListView.ExpandAllNodes();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> MenuItem - (행삭제) ContextMenu 이벤트
        private void MenuItemRowDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strSelectedItemTreeID   = (this.treeListControl.SelectedItem as MenuMgmt).TREE_ID;      // 메뉴 ID
                var isSelectedItemNewRowYN  = (this.treeListControl.SelectedItem as MenuMgmt).IsNew;        // 신규 여부
                if (isSelectedItemNewRowYN != true) { return; }

                this.MenuMgntList.Where(p => p.IsNew && p.TREE_ID.Equals(strSelectedItemTreeID)).ToList().ForEach(p =>
                {
                    this.MenuMgntList.Remove(p);
                });
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region > 기타 이벤트
        #region >> 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        private static void View_ShowingEditor(object sender, DevExpress.Xpf.Grid.TreeList.TreeListShowingEditorEventArgs e)
        {
            try
            {
                if (g_IsAuthAllYN == false)
                {
                    e.Cancel = true;
                    return;
                }

                var dataMember = (MenuMgmt)e.Source.DataControl.GetRow(e.RowHandle);

                switch (e.Column.FieldName)
                {
                    case "MENU_TYPE":
                        // 메뉴 타입이 공백인 경우 (상위 메뉴) 콤보박스가 선택되지 않도록 한다.
                        if (dataMember.MENU_TYPE.Length == 0 && dataMember.IsNew == false) { e.Cancel = true; }
                        break;
                    case "MENU_ID":
                    //case "SORT_SEQ":
                        if (dataMember.IsNew == false)
                        {
                            if (dataMember.IsSelected == true) { dataMember.IsSelected = false; }
                            e.Cancel = true;
                        }
                        break;
                }
            }
            catch { throw; }
        }
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
    }
}
