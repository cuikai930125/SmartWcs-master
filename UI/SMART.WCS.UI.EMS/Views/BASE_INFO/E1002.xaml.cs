using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.EMS.DataMembers.COM;
using SMART.WCS.UI.EMS.DataMembers.E1002;

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
using System.Windows.Media;
using System.Windows.Threading;

namespace SMART.WCS.UI.EMS.Views.BASE_INFO
{
    /// <summary>
    /// E1002.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E1002 : UserControl
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
        /// 화면 로드 여부
        /// </summary>
        private bool g_isLoaded = false;
        #endregion

        #region ▩ 생성자
        public E1002()
        {
            InitializeComponent();
        }

        public E1002(List<string> _liMenuNavigation)
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

                // 컨트롤 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.GetSP_EMS_PBS_SEL();

                if (this.EmsPbsList.Count() > 0)
                {
                    this.BaseClass.SetTreeListControlRowAddFocus(this.treeListControl, 0);
                }

                // 신규 작성시에만 ID 편집 가능
                this.treeListView.ShowingEditor += (s, e) =>
                {
                    EmsPbs item     = this.treeListControl.SelectedItem as EmsPbs;
                    e.Cancel        = this.treeListControl.CurrentColumn.FieldName == "PBS_ID" && item.IsNew == false;
                };
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

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(E1002), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        /// <summary>
        /// PBS 구분
        /// </summary>
        public ObservableCollection<EmsCommCode> EmsPbsDevList { get; private set; }

        public static readonly DependencyProperty EmsPbsListProperty
                                  = DependencyProperty.Register("EmsPbsList",
                                      typeof(ObservableCollection<EmsPbs>),
                                             typeof(E1002), new PropertyMetadata(new ObservableCollection<EmsPbs>()));
        /// <summary>
        /// Pbs List
        /// </summary>
        public ObservableCollection<EmsPbs> EmsPbsList
        {
            get { return (ObservableCollection<EmsPbs>)GetValue(EmsPbsListProperty); }
            private set { SetValue(EmsPbsListProperty, value); }
        }

        public static readonly DependencyProperty CategoriesListProperty
                                  = DependencyProperty.Register("CategoriesList",
                                      typeof(ObservableCollection<PBSTreeItem>),
                                             typeof(E1002), new PropertyMetadata(new ObservableCollection<PBSTreeItem>()));

        /// <summary>
        /// Pbs Tree
        /// </summary>
        public ObservableCollection<PBSTreeItem> CategoriesList
        {
            get { return (ObservableCollection<PBSTreeItem>)GetValue(CategoriesListProperty); }
            private set { SetValue(CategoriesListProperty, value); }
        }

        #region > Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(E1002), new PropertyMetadata(string.Empty));

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

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        private void InitControl()
        {
            try
            {
                this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, true);

                // ContextMenu 리소스 설정
                this.menuItemRowAdd.Header = this.BaseClass.GetResourceValue("ROW_ADD");   // 행추가
                this.menuItemRowDelete.Header = this.BaseClass.GetResourceValue("ROW_DEL");   // 행삭제
            }
            catch { throw; }
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 로드 이벤트
            this.Loaded += E1002_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 조회 버튼 클릭 이벤트
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp; ;
            // 저장 버튼 클릭 이벤트
            this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp; ;

            // 트리 컨트롤 전체 펼침 버튼 클릭 이벤트
            this.btnAllOpen.PreviewMouseLeftButtonUp += BtnAllOpen_PreviewMouseLeftButtonUp; ;
            // 트리 컨트롤 전체 닫힘 버튼 클릭 이벤트
            this.btnAllClose.PreviewMouseLeftButtonUp += BtnAllClose_PreviewMouseLeftButtonUp; ;
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

        private void E1002_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.g_isLoaded == true) { return; }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

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

        #region >> SetResultText - 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        /// <param name="_iTabIndex">Tab Index값</param>
        private void SetResultText()
        {
            var strResource                 = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                  // 텍스트 리소스
            var iTotalRowCount              = (this.treeListControl.ItemsSource as ICollection).Count;          // 전체 데이터 수
            this.GridRowCount               = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";            // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource                     = this.BaseClass.GetResourceValue("DATA_INQ");                      // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
        }
        #endregion

        #region > 데이터 관련
        private void GetSP_EMS_PBS_SEL()
        {
            try
            {
                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                          = null;
                var strProcedureName                        = "PK_EMS_EBSE002.SP_EMS_PBS_SEL";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "P_EMS_PBS_LIST" };

                var strCenterCD     = this.BaseClass.CenterCD;                                  // 센터코드
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("P_CNTR_CD",          strCenterCD);       // 센터코드
                dicInputParam.Add("P_USE_YN",           this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN));
                #endregion

                #region + 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    //await System.Threading.Tasks.Task.Run(() =>
                    //{
                        dtRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
                    //}).ConfigureAwait(true);
                }
                #endregion

                if (dtRtnValue == null) { return; }

                string strErrCode           = string.Empty;     // 오류 코드
                string strErrMsg            = string.Empty;     // 오류 메세지

                // 정상 처리된 경우
                this.EmsPbsList   = new ObservableCollection<EmsPbs>();
                this.EmsPbsList.ToObservableCollection(dtRtnValue);

                // 데이터 바인딩
                this.treeListControl.ItemsSource = this.EmsPbsList;

                this.treeListView.ExpandAllNodes();
            }
            catch { throw; }
        }

        private bool InsertSP_EMS_PBS_INSERT(BaseDataAccess _da, EmsPbs _item)
        {
            try
            { 
                bool isRtnValue     = true;

                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_EMS_EBSE002.SP_EMS_PBS_INSERT";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "P_RESULT" };

                var strCenterCD         = this.BaseClass.CenterCD;                                      // 센터코드
                var strPbsID            = _item.PBS_ID;
                var strPbsNm            = _item.PBS_NM;
                var strPbsDevCD         = _item.PBS_DEV_CD == null ? string.Empty : _item.PBS_DEV_CD;   
                var strPbsClsLvl        = _item.PBS_LV;                                                 // 레벨
                var strParentPbsClsID   = _item.PARENT_PBS_ID;                                          // 상위코드
                var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";                     // 사용여부
                var iSortSeq            = _item.SORT_SEQ;                                               // 정렬순서
                var strTreeID           = _item.TREE_ID;                                                // 트리 ID
                var strUserID           = this.BaseClass.UserID;                                    // 사용자 ID
                #endregion
                    
                #region + Input 파라메터
                dicInputParam.Add("P_CNTR_CD",              strCenterCD);           // 센터코드   
                dicInputParam.Add("P_PBS_ID",               strPbsID);              
                dicInputParam.Add("P_PBS_NM",               strPbsNm); 
                dicInputParam.Add("P_PBS_LV",               strPbsClsLvl);          // 레벨
                dicInputParam.Add("P_PBS_DEV_CD",           strPbsDevCD);
                dicInputParam.Add("P_PARENT_PBS_ID",        strParentPbsClsID);     // 상위 ID
                dicInputParam.Add("P_USER_ID",              strUserID);             // 사용자 ID
                dicInputParam.Add("P_TREE_ID",              strTreeID);             // 트리 ID
                dicInputParam.Add("P_USE_YN",               strUseYN);              // 사용여부
                dicInputParam.Add("P_SORT_SEQ",             iSortSeq);              // 정렬순서
                #endregion

                #region + 데이터 신규저장
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
                #endregion

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("100") == false)
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
            catch { throw; }
        }

        private bool UpdateSP_EMS_PBS_UPDATE(BaseDataAccess _da, EmsPbs _item)
        {
            try
            {
                bool isRtnValue     = true;

                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_EMS_EBSE002.SP_EMS_PBS_UPDATE";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "P_RESULT" };

                var strCenterCD         = this.BaseClass.CenterCD;                                  // 센터코드
                var strPbsID            = _item.PBS_ID;
                var strPbsNm            = _item.PBS_NM;
                var strPbsDevCD         = _item.PBS_DEV_CD == null ? string.Empty : _item.PBS_DEV_CD;
                var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";                 // 사용여부
                var iSortSeq            = _item.SORT_SEQ;                                           // 정렬순서
                var strTreeID           = _item.TREE_ID;                                            // 트리 ID
                var strUserID           = this.BaseClass.UserID;                                    // 사용자 ID
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("P_CNTR_CD",              strCenterCD);           // 센터코드   
                dicInputParam.Add("P_PBS_ID",               strPbsID);              // 분류코드
                dicInputParam.Add("P_PBS_NM",               strPbsNm);              // 분류코드명
                dicInputParam.Add("P_PBS_DEV_CD",           strPbsDevCD);
                dicInputParam.Add("P_USER_ID",              strUserID);             // 사용자 ID
                dicInputParam.Add("P_USE_YN",               strUseYN);              // 사용여부
                dicInputParam.Add("P_SORT_SEQ",             iSortSeq);              // 정렬순서
                #endregion

                #region + 데이터 조회
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
                #endregion

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("100") == false)
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
            catch { throw; }
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 버튼 이벤트
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (CheckModifyData() == false) { return; }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.GetSP_EMS_PBS_SEL();

                if (this.EmsPbsList.Count() > 0)
                {
                    this.BaseClass.SetTreeListControlRowAddFocus(this.treeListControl, 0);
                }
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

        private void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strMessage = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.NORMAL);

                foreach (var item in this.EmsPbsList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.PBS_ID) == true)
                        {
                            item.CellError("PBS_ID", string.Format(strMessage, this.GetLabelDesc("PBS_ID")));
                            return;
                        }
                    }
                }

                // 신규, 수정 데이터가 없는 경우 구문을 리턴한다.
                if (this.EmsPbsList.Where(p => p.IsUpdate || p.IsNew).Count() == 0) 
                {
                    // ERR_NOT_CHANGE_DATA - 변경된 데이터가 없습니다.
                    this.BaseClass.MsgInfo("ERR_NOT_CHANGE_DATA");
                    return; 
                }

                // ASK_SAVE - 저장 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                string strInputMessage      = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.MESSAGE);
                // ERR_NOT_SELECT - {0}이(가) 선택되지 않았습니다.
                string strSelectMessage     = this.BaseClass.GetResourceValue("ERR_NOT_SELECT", BaseEnumClass.ResourceType.MESSAGE);
                bool isRtnValue             = true;


                using (BaseDataAccess da = new BaseDataAccess())
                {
                    da.BeginTransaction();

                    try
                    {
                        foreach (var item in this.EmsPbsList.Where(p => p.IsUpdate || p.IsNew).ToList())
                        {
                            if (item.IsNew)
                            {
                                isRtnValue = this.InsertSP_EMS_PBS_INSERT(da, item);
                                if (isRtnValue == false) { break; }
                            }
                            else if (item.IsUpdate)
                            {
                                isRtnValue = this.UpdateSP_EMS_PBS_UPDATE(da, item);
                                if (isRtnValue == false) { break; }
                            }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();

                            // CMPT_SAVE - 저장 되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");

                            // 메뉴 관리 리스트
                            this.GetSP_EMS_PBS_SEL();
                        }
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
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

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

        #region > 트리 컨트롤
        private async void TreeListControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view                    = (sender as TreeListControl).View as TreeListView;
                var hitInfo                 = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hitInfo.Column == null) { return; }

                var strSelectedMenuID       = (this.treeListControl.SelectedItem as EmsPbs).PBS_ID;
                var strSelectedTreeID       = (this.treeListControl.SelectedItem as EmsPbs).TREE_ID;             // 트리 ID
                bool isUseYN                = !(this.treeListControl.SelectedItem as EmsPbs).USE_YN_CHECKED;     // 사용 여부
                bool isNewYN                = (this.treeListControl.SelectedItem as EmsPbs).IsNew;               // 신규 추가 여부
            
                if (isNewYN == true) { return; }

                var liCurrentRowData    = this.EmsPbsList.Where(p => p.PBS_ID.Equals(strSelectedMenuID) == true).ToList();
                var liFilterData        = this.EmsPbsList.Where(p => p.PARENT_PBS_ID.Equals(strSelectedTreeID) == true).ToList();

                if (hitInfo.InRowCell)
                {
                    switch (hitInfo.Column.FieldName)
                    {  
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
                string strSelectedMenuID    = (this.treeListControl.SelectedItem as EmsPbs).PBS_ID;         // 메뉴 ID
                string strSelectedTreeID    = (this.treeListControl.SelectedItem as EmsPbs).TREE_ID;        // 트리 ID
                var iSelectedMenuLevel      = (this.treeListControl.SelectedItem as EmsPbs).PBS_LV;         // 메뉴 레벨
                var strSelectedNewRowYN     = (this.treeListControl.SelectedItem as EmsPbs).IsNew;          // 신규 여부

                // 신규 추가된 Row의 경우 하위 Row를 추가하지 않기 때문에
                // isNew가 true인 경우 구문을 리턴한다.
                if (strSelectedNewRowYN == true) { return; }

                var liFilterData = this.EmsPbsList.Where(p => p.PARENT_PBS_ID.Equals(strSelectedTreeID) == true).ToList();

                int i = 0;
                foreach (var item in this.EmsPbsList)
                {
                    if (item.PBS_ID.Equals(strSelectedMenuID) == true) { break; }
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

                var newItem = new EmsPbs
                {
                        PBS_ID              = string.Empty              // 설비 ID
                    ,   PBS_NM              = string.Empty              // 설비명
                    ,   PBS_LV              = iSelectedMenuLevel + 1    // 메뉴 레벨
                    ,   TREE_ID             = strTreeID                 // 트리 ID
                    ,   PARENT_PBS_ID       = strSelectedTreeID         // 상위 메뉴 ID
                    ,   USE_YN              = "Y"                       // 사용 여부
                    ,   SORT_SEQ            = iSortSeq                  // 정렬 순서
                    ,   EQP_QTY             = 0
                    ,   IsSelected          = true
                    ,   IsNew               = true
                    ,   IsUpdate            = false
                };

                this.EmsPbsList.Add(newItem);
                this.treeListControl.Focus();
                this.treeListControl.CurrentColumn  = this.treeListControl.Columns.First();
                this.treeListControl.View.FocusedRowHandle  = this.EmsPbsList.Count - 1;

                this.EmsPbsList[this.EmsPbsList.Count - 1].BackgroundBrush      = new SolidColorBrush(Colors.GhostWhite);
                this.EmsPbsList[this.EmsPbsList.Count - 1].BaseBackgroundBrush  = new SolidColorBrush(Colors.GhostWhite);


                //#region 추가된 트리의 상위 메뉴 ID로 포커스를 이동하기 위한 구문
                //int j = 0;
                //for (j = 0; j < this.treeListControl.VisibleRowCount; j++)
                //{
                //    var rowData         = this.treeListControl.GetRow(j);
                //    var strMenuID       = ((SMART.WCS.UI.EMS.DataMembers.E1003.EqpClsTreeItem)rowData).EQP_CLS_ID;

                //    if (strSelectedMenuID.Equals(strMenuID) == true) { break; }
                //}

                //this.BaseClass.SetTreeListControlRowAddFocus(this.treeListControl, j);
                //#endregion

                this.treeListView.ExpandAllNodes();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        /// <summary>
        /// (행삭제) ContextMenu 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemRowDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strSelectedItemTreeID   = (this.treeListControl.SelectedItem as EmsPbs).TREE_ID;      // 메뉴 ID
                var isSelectedItemNewRowYN  = (this.treeListControl.SelectedItem as EmsPbs).IsNew;        // 신규 여부
                if (isSelectedItemNewRowYN != true) { return; }

                this.EmsPbsList.Where(p => p.IsNew && p.TREE_ID.Equals(strSelectedItemTreeID)).ToList().ForEach(p =>
                {
                    this.EmsPbsList.Remove(p);
                });
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 기타 함수
        #region >> CheckModifyData - 데이터 저장 여부를 체크한다.
        /// <summary>
        /// 데이터 저장 여부를 체크한다.
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyData()
        {
            try
            {
                bool bRtnValue = true;

                if (this.EmsPbsList.Any(p => p.IsNew || p.IsDelete || p.IsUpdate) == true)
                {
                    // Msg - 저장되지 않은 데이터가 있습니다.|조회 하시겠습니까?
                    this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE_INQ");
                    bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
                }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region 저장
        /// <summary>
        /// 저장 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveClick(object sender, MouseButtonEventArgs e)
        {
            

            if (SaveEmsPbs())
            {
                //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("CMPT_SAVE_DATA"); // "저장되었습니다."
                //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                // CMPT_SAVE - 저장되었습니다.
                this.BaseClass.MsgInfo("CMPT_SAVE");

                //trPbsView.Nodes.Clear();
                //SetTreeList(CategoriesList, null);
                //trPbsView.ExpandAllNodes();

                //from_save = false;

                //if (-1 < focused_handle)
                //{
                //    if (0 == focused_handle)
                //    {
                //        gridMain.ItemsSource = CategoriesList[0].Items;
                //    }

                //    trPbsView.FocusedRowHandle = focused_handle;

                //    gridMain.View.FocusedRowHandle = (gridMain.ItemsSource as ObservableCollection<PBSTreeItem>).Count - 1;
                //    gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<PBSTreeItem>)[(gridMain.ItemsSource as ObservableCollection<PBSTreeItem>).Count - 1];
                //}
            }
        }

        List<PBSTreeItem> _newItems = new List<PBSTreeItem>();
        List<PBSTreeItem> _editItems = new List<PBSTreeItem>();
        /// <summary>
        /// 저장 함수
        /// </summary>
        /// <returns></returns>
        private bool SaveEmsPbs()
        {
            _newItems.Clear();
            _editItems.Clear();

            using (BaseDataAccess da = new BaseDataAccess())
            {
                try
                {
                    string _SUCCESS_CODE = "100";

                    // 수정본
                    //
                    for (int i = 0; i < CategoriesList.Count; ++i)
                    {
                        GetEditNode(CategoriesList[i]);
                    }

                    // 신규
                    //
                    for (int i = 0; i < CategoriesList.Count; ++i)
                    {
                        GetNewNode(CategoriesList[i]);
                    }

                    // 저장 항목 없음
                    //
                    if ((0 == _newItems.Count && 0 == _editItems.Count))
                    {
                        return false;
                    }


                    var _USER_ID = this.BaseClass.UserID;

                    // 수정본
                    //
                    if (0 < _editItems.Count)
                    {
                        foreach (var item in _editItems)
                        {
                            //if (item.PBS_ID.Length != item.PBS_LV * 2)
                            //{
                            //    BaseClass.MsgInfo("ID 길이는 " + item.PBS_LV * 2 + " 자리 이어야 합니다. ", this.BaseInfo.country_cd);
                            //    return false;
                            //}

                            if (string.IsNullOrEmpty(item.PBS_ID) || string.IsNullOrEmpty(item.PBS_NM))
                            {
                                //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("EMS_IDNM_MUST"); // "ID/명칭은 필수 항목입니다."
                                //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                                // ERR_REQ_ID_NM - ID/명칭은 필수항목입니다.
                                this.BaseClass.MsgError("ERR_REQ_ID_NM");
                                return false;
                            }

                            if (null != item.Parent)
                            {
                                if (item.PBS_ID.Length == item.Parent.PBS_ID.Length)
                                {
                                    //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("EMS_INVALID_ID"); // "Invalid ID !"
                                    //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                                    // ERR_INVALID_ID - Invalid ID
                                    this.BaseClass.MsgError("Invalid ID");
                                    return false;
                                }

                                if (item.PBS_ID.Length > item.Parent.PBS_ID.Length)
                                {
                                    if (item.Parent.PBS_ID != item.PBS_ID.Substring(0, item.Parent.PBS_ID.Length))
                                    {
                                        //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("EMS_PARTNT_CHILD"); // "하위 ID는 부모 ID와 연관성이 있어야 합니다."
                                        //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                                        // ERR_PARTNT_CHILD_RELA - 하위ID는 부모ID와 연관이 있어야합니다.
                                        this.BaseClass.MsgError("ERR_PARTNT_CHILD_RELA");
                                        return false;
                                    }
                                }

                                if (1 < item.Parent.Items.Where(f => f.PBS_ID == item.PBS_ID).ToList().Count)
                                {
                                    //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("EMS_SAME_ID"); // "중복된 ID가 존재합니다."
                                    //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                                    // ERR_DUP_ID - 중복된 ID가 존재합니다.
                                    this.BaseClass.MsgError("ERR_DUP_ID");
                                    return false;
                                }
                            }
                            //else
                            //{
                            //    if (1 < CategoriesList.Where(f => f.PBS_ID == item.PBS_ID).ToList().Count)
                            //    {
                            //        BaseClass.MsgInfo("중복된 ID가 존재합니다.", this.BaseInfo.country_cd);
                            //        return false;
                            //    }
                            //}
                        }

                        da.BeginTransaction();

                        foreach (var item in _editItems)
                        {
                            var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                { "P_PBS_ID",       item.PBS_ID },              // ID
                                { "P_PBS_NM",       item.PBS_NM },              // 이름
                                { "P_PBS_DEV_CD",   item.PBS_DEV_CD },          // 구분
                                {"P_USER_ID",       _USER_ID}
                            };

                            var strOutParam = new[] { "P_RESULT" };
                            string callProc = "PK_EMS_EBSE002.SP_EMS_PBS_UPDATE";

                            var outData = da.GetSpDataSet(
                                        callProc                      // 호출 프로시저
                                    ,   param                        // Input 파라메터
                                    ,   strOutParam                  // Output 파라메터
                            );

                            if (outData.Tables[0].Rows.Count > 0)
                            {
                                if (outData.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
                                {
                                    _SUCCESS_CODE = "0";
                                    da.RollbackTransaction();
                                    BaseClass.MsgInfo(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                                    break;
                                }
                            }
                            else
                            {
                                _SUCCESS_CODE = "0";
                                da.RollbackTransaction();
                                //BaseClass.MsgInfo(HelperClass.GetMessageByCountryCode("INPUT_TYPE_ERR"), BaseInfo.country_cd.ToString());
                                this.BaseClass.MsgError("ERR_INPUT_TYPE");
                                break;
                            }
                        }
                    }

                    if (_SUCCESS_CODE == "100")
                    {
                        // 신규
                        //
                        if (0 < _newItems.Count)
                        {
                            foreach (var item in _newItems)
                            {
                                //if (item.PBS_ID.Length != item.PBS_LV * 2)
                                //{
                                //    BaseClass.MsgInfo("ID 길이는 " + item.PBS_LV * 2 + " 자리 이어야 합니다. ", this.BaseInfo.country_cd);
                                //    return false;
                                //}
                                if (string.IsNullOrEmpty(item.PBS_ID) || string.IsNullOrEmpty(item.PBS_NM))
                                {
                                    //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("EMS_IDNM_MUST"); // "ID/명칭은 필수 항목입니다."
                                    //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                                    // ERR_REQ_ID_NM - ID/명칭은 필수항목입니다.
                                    this.BaseClass.MsgError("ERR_REQ_ID_NM");
                                    return false;
                                }


                                if (null != item.Parent)
                                {
                                    if (item.PBS_ID.Length == item.Parent.PBS_ID.Length)
                                    {
                                        //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("EMS_INVALID_ID"); // "Invalid ID !"
                                        //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                                        // ERR_INVALID_ID - Invalid ID
                                        this.BaseClass.MsgError("Invalid ID");
                                        return false;
                                    }

                                    if (item.PBS_ID.Length > item.Parent.PBS_ID.Length)
                                    {
                                        if (item.Parent.PBS_ID != item.PBS_ID.Substring(0, item.Parent.PBS_ID.Length))
                                        {
                                            //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("EMS_PARTNT_CHILD"); // "하위 ID는 부모 ID와 연관성이 있어야 합니다."
                                            //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                                            // ERR_PARTNT_CHILD_RELA - 하위ID는 부모ID와 연관이 있어야합니다.
                                            this.BaseClass.MsgError("ERR_PARTNT_CHILD_RELA");
                                            return false;
                                        }
                                    }

                                    if (1 < item.Parent.Items.Where(f => f.PBS_ID == item.PBS_ID).ToList().Count)
                                    {
                                        //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("EMS_SAME_ID"); // "중복된 ID가 존재합니다."
                                        //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                                        // ERR_DUP_ID - 중복된 ID가 존재합니다.
                                        this.BaseClass.MsgError("ERR_DUP_ID");
                                        return false;
                                    }
                                }
                                //else
                                //{
                                //    if (1 < CategoriesList.Where(f => f.PBS_ID == item.PBS_ID).ToList().Count)
                                //    {
                                //        BaseClass.MsgInfo("중복된 ID가 존재합니다.", this.BaseInfo.country_cd);
                                //        return false;
                                //    }
                                //}
                            }

                            da.BeginTransaction();

                            foreach (var item in _newItems)
                            {
                                var param = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD",        this.BaseClass.CenterCD },
                                    { "P_PBS_ID",           item.PBS_ID },                                      // ID
                                    { "P_PBS_NM",           item.PBS_NM },                                      // 이름
                                    { "P_PBS_LV",           item.PBS_LV },                                      // Level (Tree 구조)
                                    { "P_PBS_DEV_CD",       item.PBS_DEV_CD },                                  // 구분
                                    { "P_PARENT_PBS_ID",    (null == item.Parent) ? "" : item.Parent.PBS_ID },  // 부모
                                    { "P_USER_ID",          _USER_ID}
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_EBSE002.SP_EMS_PBS_INSERT";

                                var outData = da.GetSpDataSet(
                                            callProc                    // 호출 프로시저
                                        ,   param                       // Input 파라메터
                                        ,   strOutParam                 // Output 파라메터
                                );

                                if (outData.Tables[0].Rows.Count > 0)
                                {
                                    if (outData.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
                                    {
                                        _SUCCESS_CODE = "0";
                                        da.RollbackTransaction();
                                        BaseClass.MsgInfo(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                                        break;
                                    }
                                }
                                else
                                {
                                    _SUCCESS_CODE = "0";
                                    da.RollbackTransaction();
                                    //BaseClass.MsgInfo(HelperClass.GetMessageByCountryCode("INPUT_TYPE_ERR"), BaseInfo.country_cd.ToString());
                                    this.BaseClass.MsgError("ERR_INPUT_TYPE");
                                    break;
                                }
                            }
                        }
                    }

                    if ((0 < _newItems.Count || 0 < _editItems.Count) && _SUCCESS_CODE == "100")
                    {
                        da.CommitTransaction();
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_SAVE_DATA"));

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    da.RollbackTransaction();
                    this.BaseClass.Error(ex);
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }

                return false;
            }
        }

        /// <summary>
        /// 신규 구성
        /// </summary>
        /// <param name="node"></param>
        void GetNewNode(PBSTreeItem node)
        {
            if (node.IsNew)
            {
                _newItems.Add(node);
            }

            if (0 == node.Items.Count)
            {
                return;
            }

            for (int i = 0; i < node.Items.Count; ++i)
            {
                GetNewNode(node.Items[i]);
            }
        }

        /// <summary>
        /// 수정 구성
        /// </summary>
        /// <param name="node"></param>
        void GetEditNode(PBSTreeItem node)
        {
            if (node.IsUpdate && !node.IsNew)
            {
                _editItems.Add(node);
            }

            if (0 == node.Items.Count)
            {
                return;
            }

            for (int i = 0; i < node.Items.Count; ++i)
            {
                GetEditNode(node.Items[i]);
            }
        }

        #endregion 저장

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

                var dataMember = (EmsPbs)e.Source.DataControl.GetRow(e.RowHandle);

                switch (e.Column.FieldName)
                {
                    //////case "MENU_TYPE":
                    //////    // 메뉴 타입이 공백인 경우 (상위 메뉴) 콤보박스가 선택되지 않도록 한다.
                    //////    if (dataMember.MENU_TYPE.Length == 0 && dataMember.IsNew == false) { e.Cancel = true; }
                    //////    break;
                    //////case "MENU_ID":
                    //////    //case "SORT_SEQ":
                    //////    if (dataMember.IsNew == false)
                    //////    {
                    //////        if (dataMember.IsSelected == true) { dataMember.IsSelected = false; }
                    //////        e.Cancel = true;
                    //////    }
                    //////    break;
                }
            }
            catch { throw; }
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
