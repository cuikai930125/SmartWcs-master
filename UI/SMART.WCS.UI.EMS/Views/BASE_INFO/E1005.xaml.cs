using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.E1005;

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
    /// E1005.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E1005 : UserControl
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
        public E1005()
        {
            InitializeComponent();
        }

        public E1005(List<string> _liMenuNavigation)
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

                // "부품 분류명"
                string strPartNm = this.BaseClass.GetResourceValue("PART_SPR_NM");

                this.InitControl();

                this.InitEvent();

                this.GetSP_EMS_PART_CLS_SEL();

                // 신규 작성시에만 ID 편집 가능
                this.treeListView.ShowingEditor += (s, e) =>
                {
                    EmsPartCls item = this.treeListControl.SelectedItem as EmsPartCls;
                    e.Cancel = this.treeListControl.CurrentColumn.FieldName == "PART_CLS_ID" && item.IsNew == false;
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
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(E1005), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        public static readonly DependencyProperty EmsPartClsListProperty
                                  = DependencyProperty.Register("EmsPartClsList",
                                      typeof(ObservableCollection<EmsPartCls>),
                                             typeof(E1005), new PropertyMetadata(new ObservableCollection<EmsPartCls>()));
        /// <summary>
        /// 부품분류 List
        /// </summary>
        public ObservableCollection<EmsPartCls> EmsPartClsList
        {
            get { return (ObservableCollection<EmsPartCls>)GetValue(EmsPartClsListProperty); }
            private set { SetValue(EmsPartClsListProperty, value); }
        }

        public static readonly DependencyProperty CategoriesListProperty
                                  = DependencyProperty.Register("CategoriesList",
                                      typeof(ObservableCollection<PartClsTreeItem>),
                                             typeof(E1005), new PropertyMetadata(new ObservableCollection<PartClsTreeItem>()));
        /// <summary>
        /// 부품분류 Tree
        /// </summary>
        public ObservableCollection<PartClsTreeItem> CategoriesList
        {
            get { return (ObservableCollection<PartClsTreeItem>)GetValue(CategoriesListProperty); }
            private set { SetValue(CategoriesListProperty, value); }
        }

        #region > Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(E1005), new PropertyMetadata(string.Empty));

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
                this.menuItemRowAdd.Header      = this.BaseClass.GetResourceValue("ROW_ADD");   // 행추가
                this.menuItemRowDelete.Header   = this.BaseClass.GetResourceValue("ROW_DEL");   // 행삭제
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
            this.Loaded += E1003_Loaded;
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

        private void E1003_Loaded(object sender, RoutedEventArgs e)
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

                if (this.EmsPartClsList.Any(p => p.IsNew || p.IsDelete || p.IsUpdate) == true)
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

        #region > 데이터 관련
        private void GetSP_EMS_PART_CLS_SEL()
        {
            try
            {
                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                          = null;
                var strProcedureName                        = "PK_EMS_EBSE005.SP_EMS_PART_CLS_SEL";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "P_EMS_CLS_LIST" };

                var strCenterCD     = this.BaseClass.CenterCD;      // 센터코드
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
                this.EmsPartClsList = new ObservableCollection<EmsPartCls>();
                this.EmsPartClsList.ToObservableCollection(dtRtnValue);

                // 데이터 바인딩
                this.treeListControl.ItemsSource = this.EmsPartClsList;

                this.treeListView.ExpandAllNodes();
            }
            catch { throw; }
        }

        private bool InsertSP_EMS_PART_CLS_INSERT(BaseDataAccess _da, EmsPartCls _item)
        {
            try
            { 
                bool isRtnValue     = true;

                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_EMS_EBSE005.SP_EMS_PART_CLS_INSERT";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "P_RESULT" };

                var strCenterCD         = this.BaseClass.CenterCD;                                  // 센터코드
                var strPbsID            = _item.PART_CLS_ID;
                var strPbsNm            = _item.PART_CLS_NM;
                var strPbsClsLvl        = _item.PART_CLS_LV;                                         // 레벨
                var strPartClsNote      = _item.PART_CLS_NOTE;
                var strParentPbsClsID   = _item.PARENT_PART_CLS_ID;                                 // 상위코드
                var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";                 // 사용여부
                var iSortSeq            = _item.SORT_SEQ;                                           // 정렬순서
                var strTreeID           = _item.TREE_ID;                                            // 트리 ID
                var strUserID           = this.BaseClass.UserID;                                    // 사용자 ID
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("P_CNTR_CD",              strCenterCD);           // 센터코드   
                dicInputParam.Add("P_PART_CLS_ID",          strPbsID);              
                dicInputParam.Add("P_PART_CLS_NM",          strPbsNm); 
                dicInputParam.Add("P_PART_CLS_LV",          strPbsClsLvl);          // 레벨
                dicInputParam.Add("P_PART_CLS_NOTE",        strPartClsNote);
                dicInputParam.Add("P_PARENT_PART_CLS_ID",   strParentPbsClsID);     // 상위 ID
                dicInputParam.Add("P_USER_ID",              strUserID);             // 사용자 ID
                dicInputParam.Add("P_SORT_SEQ",             iSortSeq);              // 정렬순서
                dicInputParam.Add("P_TREE_ID",              strTreeID);
                dicInputParam.Add("P_USE_YN",               strUseYN);              // 사용여부
                
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

        private bool UpdateSP_EMS_PART_CLS_UPDATE(BaseDataAccess _da, EmsPartCls _item)
        {
            try
            {
                bool isRtnValue     = true;

                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_EMS_EBSE005.SP_EMS_PART_CLS_UPDATE";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "P_RESULT" };

                var strCenterCD         = this.BaseClass.CenterCD;                                  // 센터코드
                var strPbsID            = _item.PART_CLS_ID;
                var strPbsNm            = _item.PART_CLS_NM;
                var strPartClsNote      = _item.PART_CLS_NOTE;
                var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";                 // 사용여부
                var iSortSeq            = _item.SORT_SEQ;                                           // 정렬순서
                var strUserID           = this.BaseClass.UserID;                                    // 사용자 ID
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("P_CNTR_CD",              strCenterCD);           // 센터코드   
                dicInputParam.Add("P_PART_CLS_ID",          strPbsID);              // 분류코드
                dicInputParam.Add("P_PART_CLS_NM",          strPbsNm);              // 분류코드명
                dicInputParam.Add("P_PART_CLS_NOTE",        strPartClsNote);
                dicInputParam.Add("P_USER_ID",              strUserID);             // 사용자 ID
                dicInputParam.Add("P_SORT_SEQ",             iSortSeq);              // 정렬순서
                dicInputParam.Add("P_USE_YN",               strUseYN);              // 사용여부
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

                this.GetSP_EMS_PART_CLS_SEL();
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
                // 신규, 수정 데이터가 없는 경우 구문을 리턴한다.
                if (this.EmsPartClsList.Where(p => p.IsUpdate || p.IsNew).Count() == 0) 
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
                        foreach (var item in this.EmsPartClsList.Where(p => p.IsUpdate || p.IsNew).ToList())
                        {
                            if (item.IsNew)
                            {
                                isRtnValue = this.InsertSP_EMS_PART_CLS_INSERT(da, item);
                                if (isRtnValue == false) { break; }
                            }
                            else if (item.IsUpdate)
                            {
                                isRtnValue = this.UpdateSP_EMS_PART_CLS_UPDATE(da, item);
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
                            this.GetSP_EMS_PART_CLS_SEL();
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

                var strSelectedMenuID       = (this.treeListControl.SelectedItem as EmsPartCls).PART_CLS_ID;
                var strSelectedTreeID       = (this.treeListControl.SelectedItem as EmsPartCls).TREE_ID;             // 트리 ID
                bool isUseYN                = !(this.treeListControl.SelectedItem as EmsPartCls).USE_YN_CHECKED;     // 사용 여부
                bool isNewYN                = (this.treeListControl.SelectedItem as EmsPartCls).IsNew;               // 신규 추가 여부
            
                if (isNewYN == true) { return; }

                var liCurrentRowData    = this.EmsPartClsList.Where(p => p.PART_CLS_ID.Equals(strSelectedMenuID) == true).ToList();
                var liFilterData        = this.EmsPartClsList.Where(p => p.PARENT_PART_CLS_ID.Equals(strSelectedTreeID) == true).ToList();

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

                var dataMember = (EmsPartCls)e.Source.DataControl.GetRow(e.RowHandle);

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
                string strSelectedMenuID    = (this.treeListControl.SelectedItem as EmsPartCls).PART_CLS_ID;        // 메뉴 ID
                string strSelectedTreeID    = (this.treeListControl.SelectedItem as EmsPartCls).TREE_ID;            // 트리 ID
                var iSelectedMenuLevel      = (this.treeListControl.SelectedItem as EmsPartCls).PART_CLS_LV;        // 메뉴 레벨
                var strSelectedNewRowYN     = (this.treeListControl.SelectedItem as EmsPartCls).IsNew;              // 신규 여부

                // 신규 추가된 Row의 경우 하위 Row를 추가하지 않기 때문에
                // isNew가 true인 경우 구문을 리턴한다.
                if (strSelectedNewRowYN == true) { return; }

                var liFilterData = this.EmsPartClsList.Where(p => p.PARENT_PART_CLS_ID.Equals(strSelectedTreeID) == true).ToList();

                int i = 0;
                foreach (var item in this.EmsPartClsList)
                {
                    if (item.PART_CLS_ID.Equals(strSelectedMenuID) == true) { break; }
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

                var newItem = new EmsPartCls
                {
                        PART_CLS_ID         = string.Empty              // 설비 ID
                    ,   PART_CLS_NM         = string.Empty              // 설비명
                    ,   PART_CLS_LV         = iSelectedMenuLevel + 1    // 메뉴 레벨
                    ,   TREE_ID             = strTreeID                 // 트리 ID
                    ,   PARENT_PART_CLS_ID  = strSelectedTreeID         // 상위 메뉴 ID
                    ,   USE_YN              = "Y"                       // 사용 여부
                    ,   SORT_SEQ            = iSortSeq                  // 정렬 순서
                    ,   IsSelected          = true
                    ,   IsNew               = true
                    ,   IsUpdate            = false
                };

                this.EmsPartClsList.Add(newItem);
                this.treeListControl.Focus();
                this.treeListControl.CurrentColumn  = this.treeListControl.Columns.First();
                this.treeListControl.View.FocusedRowHandle  = this.EmsPartClsList.Count - 1;

                this.EmsPartClsList[this.EmsPartClsList.Count - 1].BackgroundBrush      = new SolidColorBrush(Colors.GhostWhite);
                this.EmsPartClsList[this.EmsPartClsList.Count - 1].BaseBackgroundBrush  = new SolidColorBrush(Colors.GhostWhite);


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
                var strSelectedItemTreeID   = (this.treeListControl.SelectedItem as EmsPartCls).TREE_ID;      // 메뉴 ID
                var isSelectedItemNewRowYN  = (this.treeListControl.SelectedItem as EmsPartCls).IsNew;        // 신규 여부
                if (isSelectedItemNewRowYN != true) { return; }

                this.EmsPartClsList.Where(p => p.IsNew && p.TREE_ID.Equals(strSelectedItemTreeID)).ToList().ForEach(p =>
                {
                    this.EmsPartClsList.Remove(p);
                });
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region  >>>

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
    }
}
