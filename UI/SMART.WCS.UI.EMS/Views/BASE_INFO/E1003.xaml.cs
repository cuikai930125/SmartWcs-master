using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.UI.EMS.DataMembers.E1003;

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
    /// E1003.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E1003 : UserControl, TabCloseInterface
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
        public E1003()
        {
            InitializeComponent();
        }

        public E1003(List<string> _liMenuNavigation)
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

                //Task.Run(async() => await this.GetSP_EMS_EQP_CLS_SEL());
                this.GetSP_EMS_EQP_CLS_SEL();

                // 신규 작성시에만 ID 편집 가능
                this.treeListView.ShowingEditor += (s, e) =>
                {
                    EmsEqpCls item  = this.treeListControl.SelectedItem as EmsEqpCls;
                    e.Cancel        = this.treeListControl.CurrentColumn.FieldName == "EQP_CLS_ID" && item.IsNew == false;
                };
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(E1003), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        public static readonly DependencyProperty EmsEqpClsListProperty
                                  = DependencyProperty.Register("EmsEqpClsList",
                                      typeof(ObservableCollection<EmsEqpCls>),
                                             typeof(E1003), new PropertyMetadata(new ObservableCollection<EmsEqpCls>()));

        /// <summary>
        /// 설비 분류 List
        /// </summary>
        public ObservableCollection<EmsEqpCls> EmsEqpClsList
        {
            get { return (ObservableCollection<EmsEqpCls>)GetValue(EmsEqpClsListProperty); }
            private set { SetValue(EmsEqpClsListProperty, value); }
        }


        public static readonly DependencyProperty CategoriesListProperty
                                  = DependencyProperty.Register("CategoriesList",
                                      typeof(ObservableCollection<EqpClsTreeItem>),
                                             typeof(E1003), new PropertyMetadata(new ObservableCollection<EqpClsTreeItem>()));

        #region > Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(E1003), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string GridRowCount
        {
            get { return (string)GetValue(GridRowCountProperty); }
            set { SetValue(GridRowCountProperty, value); }
        }
        #endregion


        /// <summary>
        /// 분류 Tree
        /// </summary>
        public ObservableCollection<EqpClsTreeItem> CategoriesList
        {
            get { return (ObservableCollection<EqpClsTreeItem>)GetValue(CategoriesListProperty); }
            private set { SetValue(CategoriesListProperty, value); }
        }
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
                this.menuItemRowAdd.Header          = this.BaseClass.GetResourceValue("ROW_ADD");   // 행추가
                this.menuItemRowDelete.Header       = this.BaseClass.GetResourceValue("ROW_DEL");   // 행삭제
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
            this.Loaded += E1003_Loaded; ;
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


        #endregion
        #endregion

        #region > 기타 함수
        #region >> CheckModifyData - 데이터 저장 여부를 체크한다.
        /// <summary>
        /// 데이터 저장 여부를 체크한다.
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyData()
        {
            bool bRtnValue = true;

            if (this.EmsEqpClsList.Any(p => p.IsNew || p.IsDelete || p.IsUpdate) == true)
            {
                // Msg - 저장되지 않은 데이터가 있습니다.|조회 하시겠습니까?
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE_INQ");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            }

            return bRtnValue;
        }
        #endregion

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
        #endregion
        #endregion

        #region ▩ 이벤트
        private void E1003_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.g_isLoaded == true) { return; }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                //// 메뉴 관리 리스트
                //await this.GetSP_MENU_LIST_INQ();
                // 트리 조회


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

        #region > 버튼 이벤트

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

        #region 조회
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchClick(object sender, MouseButtonEventArgs e)
        {
            SearchEqpCls();

            //trEqpView.Nodes.Clear();
            //SetTreeList(CategoriesList, null);
            //trEqpView.ExpandAllNodes();
        }

        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (CheckModifyData() == false) { return; }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.GetSP_EMS_EQP_CLS_SEL();
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

        private void GetSP_EMS_EQP_CLS_SEL()
        {
            try
            {
                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                          = null;
                var strProcedureName                        = "PK_EMS_EBSE003.SP_EMS_EQP_CLS_SEL";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "P_EMS_EQP_CLS_LIST" };

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
                this.EmsEqpClsList   = new ObservableCollection<EmsEqpCls>();
                this.EmsEqpClsList.ToObservableCollection(dtRtnValue);

                // 데이터 바인딩
                this.treeListControl.ItemsSource = this.EmsEqpClsList;

                this.treeListView.ExpandAllNodes();
            }
            catch { throw; }
        }

        /// <summary>
        /// 조회 함수
        /// </summary>
        private void SearchEqpCls()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD", this.BaseClass.CenterCD },
                    {"P_USE_YN", this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN) }
                };

                var strOutParam = new[] { "P_EMS_EQP_CLS_LIST" };
                string callProc = "PK_EMS_EBSE003.SP_EMS_EQP_CLS_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        EmsEqpClsList = new ObservableCollection<EmsEqpCls>();
                        EmsEqpClsList.ToObservableCollection(outData.Tables[0]);
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));

                        foreach (EmsEqpCls a in EmsEqpClsList)
                        {
                            if (null == a.PARENT_EQP_CLS_ID)
                            {
                                a.PARENT_EQP_CLS_ID = "";
                                a.CRUDReset();
                            }
                        }
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    CategoriesList.Clear();
                    CategoriesList = null;

                    // EQP_SPR_NM - 설비분류명
                    string strClsNm = this.BaseClass.GetResourceValue("EQP_SPR_NM");
                    //root 
                    CategoriesList = new ObservableCollection<EqpClsTreeItem>()
                    {
                        new EqpClsTreeItem() { EQP_CLS_ID = "", EQP_CLS_NM = strClsNm, EQP_CLS_LV = 0, IsExpanded = true, IsStatic = true, IsHide = true }
                    };

                    SetCategoriesList(CategoriesList[0], CategoriesList[0].Items, 1);

                    //gridMain.ItemsSource = null;
                }

                this.btnSave.IsEnabled = true;
                //this.btnRowAddHigh.IsEnabled = true;
                //this.btnRowDeleteHigh.IsEnabled = true;
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }

        /// <summary>
        /// Tree 구조 구성
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="ItemList"></param>
        /// <param name="level"></param>
        private void SetCategoriesList(EqpClsTreeItem parent, ObservableCollection<EqpClsTreeItem> ItemList, int level)
        {

            if (level > 10)
            {
                //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("EMS_OVER_LEVEL"); // "레벨이 초과 되었습니다."
                //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                // ERR_OVER_LVL - 레벨이 초과되었습니다.
                this.BaseClass.MsgError("ERR_OVER_LVL");

                return;
            }

            var V = EmsEqpClsList.Where(f => f.PARENT_EQP_CLS_ID == parent.EQP_CLS_ID);

            foreach (var item in V)
            {
                EqpClsTreeItem n = new EqpClsTreeItem()
                {
                    EQP_CLS_ID = item.EQP_CLS_ID,
                    EQP_CLS_NM = item.EQP_CLS_NM,
                    EQP_QTY = item.EQP_QTY,
                    PARENT_ID = parent.EQP_CLS_ID,
                    EQP_CLS_LV = level,
                    Parent = parent
                };

                n.Items = new ObservableCollection<EqpClsTreeItem>();
                ItemList.Add(n);

                SetCategoriesList(n, n.Items, level + 1);
            }
        }

        /// <summary>
        /// Tree Node 구성
        /// </summary>
        /// <param name="itemList"></param>
        /// <param name="parentNode"></param>
        private void SetTreeList(ObservableCollection<EqpClsTreeItem> itemList, TreeListNode parentNode)
        {
            foreach (var item in itemList)
            {

                TreeListNode node = new TreeListNode(item);
                item.TreeNode = node;

                //if (parentNode == null)
                //{
                //    trEqpView.Nodes.Add(node);
                //}
                //else
                //{
                //    parentNode.Nodes.Add(node);
                //}

                SetTreeList(item.Items, node);
            }
        }

        EqpClsTreeItem parent = null;
        private EqpClsTreeItem SelectedItem = null;

        int focused_handle = -1;
        bool from_save = false;
        /// <summary>
        /// Tree View Select
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trEqpView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            //if (from_save)
            //{
            //    return;
            //}

            ////
            ////EqpClsTreeItem target = (trEqpView.FocusedRow) as EqpClsTreeItem;
            //EqpClsTreeItem target = this.trEqp.SelectedItem as EqpClsTreeItem;

            //if (null == target)
            //{
            //    gridMain.ItemsSource = null;
            //    focused_handle = -1;
            //    return;
            //}

            //focused_handle = trEqpView.FocusedRowHandle;

            //gridMain.ItemsSource = target.Items;
            //SelectedItem = target;
        }
        #endregion 조회

        #region > Tree List Control 이벤트

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
                string strSelectedMenuID    = (this.treeListControl.SelectedItem as EmsEqpCls).EQP_CLS_ID;      // 메뉴 ID
                string strSelectedTreeID    = (this.treeListControl.SelectedItem as EmsEqpCls).TREE_ID;      // 트리 ID
                var iSelectedMenuLevel      = (this.treeListControl.SelectedItem as EmsEqpCls).EQP_CLS_LV;     // 메뉴 레벨
                var strSelectedNewRowYN     = (this.treeListControl.SelectedItem as EmsEqpCls).IsNew;        // 신규 여부

                // 신규 추가된 Row의 경우 하위 Row를 추가하지 않기 때문에
                // isNew가 true인 경우 구문을 리턴한다.
                if (strSelectedNewRowYN == true) { return; }

                var liFilterData = this.EmsEqpClsList.Where(p => p.PARENT_EQP_CLS_ID.Equals(strSelectedTreeID) == true).ToList();

                int i = 0;
                foreach (var item in this.EmsEqpClsList)
                {
                    if (item.EQP_CLS_ID.Equals(strSelectedMenuID) == true) { break; }
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

                var newItem = new EmsEqpCls
                {
                        EQP_CLS_ID          = string.Empty              // 설비 ID
                    ,   EQP_CLS_NM          = string.Empty              // 설비명
                    ,   EQP_CLS_LV          = iSelectedMenuLevel + 1    // 메뉴 레벨
                    ,   TREE_ID             = strTreeID                 // 트리 ID
                    ,   PARENT_EQP_CLS_ID   = strSelectedTreeID         // 상위 메뉴 ID
                    ,   USE_YN              = "Y"                       // 사용 여부
                    ,   SORT_SEQ            = iSortSeq                  // 정렬 순서
                    ,   EQP_QTY             = 0
                    ,   IsSelected          = true
                    ,   IsNew               = true
                    ,   IsUpdate            = false
                };

                this.EmsEqpClsList.Add(newItem);
                this.treeListControl.Focus();
                this.treeListControl.CurrentColumn  = this.treeListControl.Columns.First();
                this.treeListControl.View.FocusedRowHandle  = this.EmsEqpClsList.Count - 1;

                this.EmsEqpClsList[this.EmsEqpClsList.Count - 1].BackgroundBrush      = new SolidColorBrush(Colors.GhostWhite);
                this.EmsEqpClsList[this.EmsEqpClsList.Count - 1].BaseBackgroundBrush  = new SolidColorBrush(Colors.GhostWhite);


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
                var strSelectedItemTreeID   = (this.treeListControl.SelectedItem as EmsEqpCls).TREE_ID;      // 메뉴 ID
                var isSelectedItemNewRowYN  = (this.treeListControl.SelectedItem as EmsEqpCls).IsNew;        // 신규 여부
                if (isSelectedItemNewRowYN != true) { return; }

                this.EmsEqpClsList.Where(p => p.IsNew && p.TREE_ID.Equals(strSelectedItemTreeID)).ToList().ForEach(p =>
                {
                    this.EmsEqpClsList.Remove(p);
                });
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private async void TreeListControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view                    = (sender as TreeListControl).View as TreeListView;
                var hitInfo                 = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hitInfo.Column == null) { return; }

                var strSelectedMenuID       = (this.treeListControl.SelectedItem as EmsEqpCls).EQP_CLS_ID;          // 메뉴 ID
                var strSelectedTreeID       = (this.treeListControl.SelectedItem as EmsEqpCls).TREE_ID;             // 트리 ID
                bool isUseYN                = !(this.treeListControl.SelectedItem as EmsEqpCls).USE_YN_CHECKED;     // 사용 여부
                bool isNewYN                = (this.treeListControl.SelectedItem as EmsEqpCls).IsNew;               // 신규 추가 여부
            
                if (isNewYN == true) { return; }

                var liCurrentRowData    = this.EmsEqpClsList.Where(p => p.EQP_CLS_ID.Equals(strSelectedMenuID) == true).ToList();
                var liFilterData        = this.EmsEqpClsList.Where(p => p.PARENT_EQP_CLS_ID.Equals(strSelectedTreeID) == true).ToList();

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
        #endregion



        #region New / Delete Node
        /// <summary>
        /// 추가 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowAddClick(object sender, MouseButtonEventArgs e)
        {

            //if (trEqpView.FocusedNode == null) { return; }

            ////EqpClsTreeItem target = (trEqpView.FocusedRow) as EqpClsTreeItem;
            //EqpClsTreeItem target = this.trEqp.SelectedItem as EqpClsTreeItem;
            //TreeListNode node = trEqpView.FocusedNode;

            //if (null == target)
            //    return;

            //NewEqpCls(target, node);
        }

        /// <summary>
        /// 추가 함수
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetNode"></param>
        private void NewEqpCls(EqpClsTreeItem target, TreeListNode targetNode)
        {
            EqpClsTreeItem newItem = new EqpClsTreeItem()
            {
                EQP_CLS_ID = target.EQP_CLS_ID,
                //EQP_CLS_NM = "New Node",
                EQP_CLS_NM = "",
                EQP_CLS_LV = target.EQP_CLS_LV + 1,
                PARENT_ID = target.EQP_CLS_ID,
                Parent = target,
                IsNew = true
            };

            TreeListNode node = new TreeListNode(newItem);
            newItem.TreeNode = node;
            targetNode.Nodes.Add(node);
            target.Items.Add(newItem);

            //DependencyObject tbx = ((ContextMenu)((sender as MenuItem).Parent)).PlacementTarget;

            //TreeViewItem t = (TreeViewItem)(treeView.ItemContainerGenerator.ContainerFromIndex(treeView.Items.CurrentPosition));
            //t.IsExpanded = true;

            //gridMain.Focus();
            //gridMain.CurrentColumn = gridMain.Columns.First();
            //gridMain.View.FocusedRowHandle = target.Items.Count - 1;
        }

        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowDeleteClick(object sender, MouseButtonEventArgs e)
        {
            //if (trEqpView.FocusedNode == null) { return; }

            //// EqpClsTreeItem target = (trEqpView.FocusedRow) as EqpClsTreeItem;
            //EqpClsTreeItem target = this.trEqp.SelectedItem as EqpClsTreeItem;
            ////TreeListNode node = trEqpView.FocusedNode;

            //if (null == target) { return; }

            //DelEqpCls(target, false);
        }

        List<EqpClsTreeItem> _delItems = new List<EqpClsTreeItem>();
        /// <summary>
        /// 삭제 함수
        /// </summary>
        /// <param name="target"></param>
        /// <param name="root"></param>
        private void DelEqpCls(EqpClsTreeItem target, bool root = true)
        {
            foreach (EqpClsTreeItem itm in target.Items)
            {
                if (itm.IsChecked)
                {
                    if (!CanDelete(itm)) { return; }
                }
            }

            if (0 < _delItems.Count)
            {
                string _SUCCESS_CODE = "100";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        da.BeginTransaction();

                        foreach (var item in _delItems)
                        {
                            if (item.IsNew != true)
                            {
                                var param = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                    { "P_EQP_CLS_ID",   item.EQP_CLS_ID }
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_EBSE003.SP_EMS_EQP_CLS_DELETE";

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
                            }
                        }

                        if (_SUCCESS_CODE == "100")
                        {
                            da.CommitTransaction();

                            for (int i = 0; i < _delItems.Count; ++i)
                            {
                                TreeListNode node = _delItems[i].TreeNode as TreeListNode;
                                node.ParentNode.Nodes.Remove(node);

                                _delItems[i].Parent.Items.Remove(_delItems[i]);
                            }
                            _delItems.Clear();

                            //gridMain.RefreshData();
                        }
                    }
                    catch (Exception ex)
                    {
                        _SUCCESS_CODE = "0";
                        da.RollbackTransaction();
                        this.BaseClass.Error(ex);
                        this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                    }
                }
            }
        }

        /// <summary>
        /// 삭제 가능 판단
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool CanDelete(EqpClsTreeItem target)
        {

            if (0 < target.Items.Count)
            {
                //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("EMS_CONTAIN_SUB"); // "하위 분류가 존재합니다."
                //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                // ERR_EXISTS_SUB - 하위 분류가 존재합니다.
                this.BaseClass.MsgError("ERR_EXISTS_SUB");
                _delItems.Clear();
                return false;
            }

            if (0 != target.EQP_QTY)
            {
                //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("EMS_CONTAIN_PBS"); // "설비가 존재합니다."
                //BaseClass.MsgInfo(strMessage, this.BaseInfo.country_cd);
                // ERR_EXISTS_EQP - 설비가 존재합니다.
                this.BaseClass.MsgError("ERR_EXISTS_EQP");
                _delItems.Clear();
                return false;
            }

            _delItems.Add(target);

            return true;
        }
        #endregion New / Delete Node

        #region 저장
        /// <summary>
        /// 저장 구성 확인
        /// </summary>
        /// <returns></returns>
        private bool CheckSaveData()
        {
            _newItems.Clear();
            _editItems.Clear();


            for (int i = 0; i < CategoriesList.Count; ++i)
            {
                GetSaveNode(CategoriesList[i]);
            }

            if (0 == _editItems.Count && _newItems.Count == 0) { return false; }


            if (!CheckSaveDataSub(_newItems)) { return false; }

            if (!CheckSaveDataSub(_editItems)) { return false; }

            return true;
        }

        /// <summary>
        /// 저장 구성
        /// </summary>
        /// <param name="node"></param>
        void GetSaveNode(EqpClsTreeItem node)
        {
            if (node.IsNew)
            {
                _newItems.Add(node);
            }
            else if (node.IsUpdate && !node.IsNew)
            {
                _editItems.Add(node);
            }

            for (int i = 0; i < node.Items.Count; ++i)
            {
                GetSaveNode(node.Items[i]);
            }
        }

        /// <summary>
        /// 저장 시 체크 항목
        /// </summary>
        /// <param name="checkData"></param>
        /// <returns></returns>
        private bool CheckSaveDataSub(List<EqpClsTreeItem> checkData)
        {

            foreach (var item in checkData)
            {
                //if (item.EQP_CLS_ID.Length != item.EQP_CLS_LV * 2)
                //{
                //    BaseClass.MsgInfo("ID 길이는 " + item.EQP_CLS_LV * 2 + " 자리 이어야 합니다. ", this.BaseInfo.country_cd);
                //    return false;
                //}
                if (string.IsNullOrEmpty(item.EQP_CLS_ID) || string.IsNullOrEmpty(item.EQP_CLS_NM))
                {
                    // ERR_REQ_ID_NM - ID/명칭은 필수항목입니다.
                    this.BaseClass.MsgError("ERR_REQ_ID_NM");
                    return false;
                }

                if (null != item.Parent)
                {
                    if (item.EQP_CLS_ID.Length == item.Parent.EQP_CLS_ID.Length)
                    {
                        // ERR_INVALID_ID - Invalid ID
                        this.BaseClass.MsgError("Invalid ID");
                        return false;
                    }

                    if (item.EQP_CLS_ID.Length > item.Parent.EQP_CLS_ID.Length)
                    {
                        if (item.Parent.EQP_CLS_ID != item.EQP_CLS_ID.Substring(0, item.Parent.EQP_CLS_ID.Length))
                        {
                            // ERR_PARTNT_CHILD_RELA - 하위ID는 부모ID와 연관이 있어야합니다.
                            this.BaseClass.MsgError("ERR_PARTNT_CHILD_RELA");
                            return false;
                        }
                    }

                    if (1 < item.Parent.Items.Where(f => f.EQP_CLS_ID == item.EQP_CLS_ID).ToList().Count)
                    {
                        // ERR_DUP_ID - 중복된 ID가 존재합니다.
                        this.BaseClass.MsgError("ERR_DUP_ID");
                        return false;
                    }
                }
                //else
                //{
                //    if (1 < CategoriesList.Where(f => f.EQP_CLS_ID == item.EQP_CLS_ID).ToList().Count)
                //    {
                //        BaseClass.MsgInfo("중복된 ID가 존재합니다.", this.BaseInfo.country_cd);
                //        return false;
                //    }
                //}
            }

            return true;
        }

        /// <summary>
        /// 저장 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveClick(object sender, MouseButtonEventArgs e)
        {
            if (!CheckSaveData()) { return; }

            if (SaveEqpCls())
            {
                // CMPT_SAVE - 저장되었습니다
                this.BaseClass.MsgInfo("CMPT_SAVE");

                from_save = true;

                SearchEqpCls();

                //trEqpView.Nodes.Clear();
                //SetTreeList(CategoriesList, null);
                //trEqpView.ExpandAllNodes();

                //from_save = false;

                //if (-1 < focused_handle)
                //{
                //    if (0 == focused_handle)
                //    {
                //        gridMain.ItemsSource = CategoriesList[0].Items;
                //    }

                //    trEqpView.FocusedRowHandle = focused_handle;

                //    gridMain.View.FocusedRowHandle = (gridMain.ItemsSource as ObservableCollection<EqpClsTreeItem>).Count - 1;
                //    gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EqpClsTreeItem>)[(gridMain.ItemsSource as ObservableCollection<EqpClsTreeItem>).Count - 1];
                //}
            }
        }

        private bool UpdateSP_EMS_EQP_CLS_UPDATE(BaseDataAccess _da, EmsEqpCls _item)
        {
            try
            {
                bool isRtnValue     = true;

                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_EMS_EBSE003.SP_EMS_EQP_CLS_UPDATE";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strCenterCD         = this.BaseClass.CenterCD;                                  // 센터코드
                var strEqpClsID         = _item.EQP_CLS_ID;                                         // 분류코드
                var strEqpClsNm         = _item.EQP_CLS_NM;                                         // 분류코드명
                var strEqpClsLvl        = _item.EQP_CLS_LV;                                         // 레벨
                var strEqpParent        = _item.PARENT_EQP_CLS_ID;                                  // 상위코드
                var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";                 // 사용여부
                var iSortSeq            = _item.SORT_SEQ;                                           // 정렬순서
                var strTreeID           = _item.TREE_ID;                                            // 트리 ID
                var strUserID           = this.BaseClass.UserID;                                    // 사용자 ID
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("P_CNTR_CD",              strCenterCD);           // 센터코드   
                dicInputParam.Add("P_EQP_CLS_ID",           strEqpClsID);           // 분류코드
                dicInputParam.Add("P_EQP_CLS_NM",           strEqpClsNm);           // 분류코드명
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

        #region > InsertSP_EMS_EQP_CLS_INSERT - 설비 분류 신규 데이터 저장
        /// <summary>
        /// 설비 분류 신규 데이터 저장
        /// </summary>
        /// <param name="_da"></param>
        /// <param name="_item"></param>
        /// <returns></returns>
        private bool InsertSP_EMS_EQP_CLS_INSERT(BaseDataAccess _da, EmsEqpCls _item)
        {
            try
            {
                bool isRtnValue     = true;

                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_EMS_EBSE003.SP_EMS_EQP_CLS_INSERT";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strCenterCD         = this.BaseClass.CenterCD;                                  // 센터코드
                var strEqpClsID         = _item.EQP_CLS_ID;                                         // 분류코드
                var strEqpClsNm         = _item.EQP_CLS_NM;                                         // 분류코드명
                var strEqpClsLvl        = _item.EQP_CLS_LV;                                         // 레벨
                var strEqpParent        = _item.PARENT_EQP_CLS_ID;                                  // 상위코드
                var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";                 // 사용여부
                var iSortSeq            = _item.SORT_SEQ;                                           // 정렬순서
                var strTreeID           = _item.TREE_ID;                                            // 트리 ID
                var strUserID           = this.BaseClass.UserID;                                    // 사용자 ID
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("P_CNTR_CD",              strCenterCD);           // 센터코드   
                dicInputParam.Add("P_EQP_CLS_ID",           strEqpClsID);           // 분류코드
                dicInputParam.Add("P_EQP_CLS_NM",           strEqpClsNm);           // 분류코드명
                dicInputParam.Add("P_EQP_CLS_LV",           strEqpClsLvl);          // 레벨
                dicInputParam.Add("P_PARENT_EQP_CLS_ID",    strEqpParent);          // 상위 분류코드
                dicInputParam.Add("P_USER_ID",              strUserID);             // 사용자 ID
                dicInputParam.Add("P_USE_YN",               strUseYN);              // 사용여부
                dicInputParam.Add("P_SORT_SEQ",             iSortSeq);              // 정렬순서
                dicInputParam.Add("P_TREE_ID",              strTreeID);             // 트리 ID
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

        private void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 신규, 수정 데이터가 없는 경우 구문을 리턴한다.
                if (this.EmsEqpClsList.Where(p => p.IsUpdate || p.IsNew).Count() == 0) 
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
                        foreach (var item in this.EmsEqpClsList.Where(p => p.IsUpdate || p.IsNew).ToList())
                        {
                            if (item.IsNew)
                            {
                                isRtnValue = this.InsertSP_EMS_EQP_CLS_INSERT(da, item);
                                if (isRtnValue == false) { break; }
                            }
                            else if (item.IsUpdate)
                            {
                                isRtnValue = this.UpdateSP_EMS_EQP_CLS_UPDATE(da, item);
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
                            this.GetSP_EMS_EQP_CLS_SEL();
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

        List<EqpClsTreeItem> _newItems = new List<EqpClsTreeItem>();
        List<EqpClsTreeItem> _editItems = new List<EqpClsTreeItem>();
        /// <summary>
        /// 저장 함수
        /// </summary>
        /// <returns></returns>
        private bool SaveEqpCls()
        {
            using (BaseDataAccess da = new BaseDataAccess())
            {
                try
                {
                    string _SUCCESS_CODE = "100";

                    var _USER_ID = this.BaseClass.UserID;

                    da.BeginTransaction();

                    // 수정본
                    //
                    foreach (var item in _editItems)
                    {
                        var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                { "P_EQP_CLS_ID", item.EQP_CLS_ID },        // ID
                                { "P_EQP_CLS_NM", item.EQP_CLS_NM },        // 이름
                                {"P_USER_ID", _USER_ID}
                            };

                        var strOutParam = new[] { "P_RESULT" };
                        string callProc = "PK_EMS_EBSE003.SP_EMS_EQP_CLS_UPDATE";

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
                            this.BaseClass.MsgError("ERR_INPUT_TYPE");
                            break;
                        }
                    }


                    if (_SUCCESS_CODE == "100" && 0 < _newItems.Count)
                    {
                        da.BeginTransaction();

                        // 신규
                        //
                        foreach (var item in _newItems)
                        {
                            var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                { "P_EQP_CLS_ID",   item.EQP_CLS_ID },        // ID
                                { "P_EQP_CLS_NM",   item.EQP_CLS_NM },        // 이름
                                { "P_EQP_CLS_LV",   item.EQP_CLS_LV },        // Level (Tree 구조)
                                { "P_PARENT_EQP_CLS_ID", (null == item.Parent) ? "" : item.Parent.EQP_CLS_ID },      // 부모
                                { "P_USER_ID", _USER_ID}
                            };

                            var strOutParam = new[] { "P_RESULT" };
                            string callProc = "PK_EMS_EBSE003.SP_EMS_EQP_CLS_INSERT";

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
                                this.BaseClass.MsgError("ERR_INPUT_TYPE");
                                break;
                            }
                        }
                    }


                    if (_SUCCESS_CODE == "100")
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
        #endregion 저장

        #region Cell 수정시 값 적용
        /// <summary>
        /// Cell 수정시 값 적용
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridMainView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            ////
            //EqpClsTreeItem item = e.Row as EqpClsTreeItem;

            //if (item.TreeNode != null)
            //{
            //    TreeListNode itemNode = item.TreeNode as TreeListNode;

            //    trEqpView.SetNodeValue(itemNode, e.Column.FieldName, e.Cell.Value);

            //}
        }
        #endregion Cell 수정시 값 적용


        #region > 기타
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

                var dataMember = (EmsEqpCls)e.Source.DataControl.GetRow(e.RowHandle);

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

        public bool TabClosing()
        {
            try
            {
                return true;
            }
            catch { throw; }
        }
        #endregion
        #endregion
    }
}
