using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.E1006;
using SMART.WCS.UI.EMS.Views.COM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SMART.WCS.UI.EMS.Views.BASE_INFO
{
    /// <summary>
    /// E1006.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E1006 : UserControl
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
        #endregion

        #region ▩ 생성자
        public E1006()
        {
            InitializeComponent();
        }

        public E1006(List<string> _liMenuNavigation)
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

                SetBaseButton();

                // 분류 행 색상
                //
                var profitFormatCondition = new FormatCondition()
                {
                    ValueRule = ConditionRule.Expression,
                    Expression = "[ORDER_DEV] == 'A'",
                    FieldName = null,
                    Format = new DevExpress.Xpf.Core.ConditionalFormatting.Format()
                    {
                        //Background = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0x7F, 0x50))
                    }
                };
                gridMainView.FormatConditions.Add(profitFormatCondition);

                // 분류 행 색상 (Excel)
                //
                var profitFormatConditionX = new FormatCondition()
                {
                    ValueRule = ConditionRule.Expression,
                    Expression = "[ORDER_DEV] == 'A'",
                    FieldName = null,
                    Format = new DevExpress.Xpf.Core.ConditionalFormatting.Format()
                    {
                        //Background = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0x7F, 0x50))
                    }
                };
                gridMainExcelView.FormatConditions.Add(profitFormatConditionX);

                // 신규 작성시에만 ID 편집 가능
                //
                gridMainView.ShowingEditor += (s, e) =>
                {
                    //EqpClsTreeItem curr = gridMain.SelectedItem as EqpClsTreeItem;

                    //e.Cancel = (gridMain.CurrentColumn.FieldName == "EQP_CLS_ID" && !curr.IsNew) || gridMain.CurrentColumn.FieldName == "EQP_QTY";
                    e.Cancel = gridMain.CurrentColumn.FieldName != "IsChecked";
                };

                this.SearchEmsPartList(true);
                gridMainView.ExpandAllNodes();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        public static readonly DependencyProperty EmsPartMngListProperty
                                  = DependencyProperty.Register("EmsPartMngList",
                                      typeof(ObservableCollection<EmsPartInfo>),
                                             typeof(E1006), new PropertyMetadata(new ObservableCollection<EmsPartInfo>()));
        /// <summary>
        /// 부품 List
        /// </summary>
        public ObservableCollection<EmsPartInfo> EmsPartMngList
        {
            get { return (ObservableCollection<EmsPartInfo>)GetValue(EmsPartMngListProperty); }
            private set { SetValue(EmsPartMngListProperty, value); }
        }
        #endregion

        #region ▩ 함수
        #region 권한별 버튼 활성화
        private void SetBaseButton()
        {
            try
            {
                bool isHighEnabled = false;
                bool isHighNewEnabled = false;

                isHighNewEnabled = g_IsAuthAllYN;

                //// 그리드 초기화
                ////this.btnSaveHigh.IsEnabled = isHighEnabled;
                ////this.btnRowAddHigh.IsEnabled = isHighNewEnabled;
                //this.btnExcelHigh.IsEnabled = isHighEnabled;
                //this.btnRowAddHigh.IsEnabled = isHighEnabled;
                //this.btnRowDeleteHigh.IsEnabled = isHighEnabled;

            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 권한별 버튼 활성화

        #region 조회
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SearchEmsPartList(true);
                gridMainView.ExpandAllNodes();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        int focused_handle = -1;
        /// <summary>
        /// 조회 함수
        /// </summary>
        /// <param name="btn"></param>
        private void SearchEmsPartList(bool btn = false)
        {
            focused_handle = -1;

            if (!btn && null != EmsPartMngList)
            {
                focused_handle = gridMain.View.FocusedRowHandle;
            }

            fromClear = true;
            foreach (CheckBox it in Checks)
            {
                it.IsChecked = false;
            }
            Checks.Clear();
            fromClear = false;

            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();
                string target = "";

                if (null != tbxSearchPart.Tag && !string.IsNullOrEmpty(tbxSearchPart.Tag.ToString()))
                {
                    target = string.IsNullOrEmpty(tbxSearchPart.Text.Trim()) ? "" : tbxSearchPart.Tag.ToString();
                }

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_PART_CLS_ID", target}
                };

                var strOutParam = new[] { "P_EMS_PART_MNG_LIST" };
                string callProc = "PK_EMS_EBSE006.SP_EMS_PART_MNG_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(  
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        EmsPartMngList = new ObservableCollection<EmsPartInfo>();
                        EmsPartMngList.ToObservableCollection(outData.Tables[0]);

                        this.gridMain.ItemsSource = this.EmsPartMngList;
                        //if (-1 < focused_handle)
                        //{
                        //    gridMain.View.FocusedRowHandle = focused_handle;
                        //    gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsPartInfo>)[focused_handle];
                        //}
                        //else
                        //{
                        //    gridMain.View.FocusedRowHandle = 0;
                        //    gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsPartInfo>)[0];
                        //}

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    //this.btnSaveHigh.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 조회

        #region 조회 팝업
        /// <summary>
        /// 부품 분류 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPopupSearchClick(object sender, MouseButtonEventArgs e)
        {
            Image target = sender as Image;

            if (null == target)
            {
                return;
            }

            using (ECOM001_10P frmPartClsSearch = new ECOM001_10P())
            {
                frmPartClsSearch.ShowDialog();

                string id = "";
                string nm = "";

                if (null != frmPartClsSearch.CurrPartCls)
                {
                    tbxSearchPart.Text = frmPartClsSearch.CurrPartCls.PART_CLS_NM;
                    tbxSearchPart.Tag = frmPartClsSearch.CurrPartCls.PART_CLS_ID;

                    id = frmPartClsSearch.CurrPartCls.PART_CLS_ID;
                    nm = frmPartClsSearch.CurrPartCls.PART_CLS_NM;
                }

                tbxSearchPart.Focus();
            }
        }
        #endregion 조회 팝업

        #region row 추가 / 수정

        /// <summary>
        /// 부품 등록 (추가/수정)
        /// </summary>
        /// <param name="cls_id"></param>
        /// <param name="cls_nm"></param>
        /// <param name="target"></param>
        void EqpPartEdit(string cls_id, string cls_nm, string target = "")
        {
            try
            {
                using (E1006_01P frmPartReg = new E1006_01P(cls_id, cls_nm, target))
                {
                    frmPartReg.ShowDialog();

                    SearchEmsPartList();
                    gridMainView.ExpandAllNodes();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// Grid Data 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridMainView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) { return; }

            if (gridMain.IsGroupRowHandle(rowHandle)) { return; }// A group row has been double clicked

            EmsPartInfo target = gridMain.SelectedItem as EmsPartInfo;

            if (null != target)
            {
                if ("A" == target.ORDER_DEV)
                {
                    EqpPartEdit(target.PART_CLS_ID, target.PART_CLS_NM);
                }
                else
                {
                    EqpPartEdit(target.PART_CLS_ID, target.PART_CLS_NM, target.PART_ID);
                }
            }
        }
        #endregion row 추가 / 수정

        #region row 삭제

        /// <summary>
        /// 삭제 함수
        /// </summary>
        private void DelEmsEqpMng()
        {
            var _delItems = EmsPartMngList.Where(p => p.IsSelected).ToList();

            if (0 < _delItems.Count)
            {
                this.BaseClass.MsgQuestion("ASK_DEL");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                var _USER_ID =  this.BaseClass.UserID;
                string _SUCCESS_CODE = "100";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        da.BeginTransaction();

                        foreach (var item in _delItems)
                        {
                            var param = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD", this.BaseClass.CenterCD },
                                    { "P_PART_ID", item.PART_ID },
                                    { "P_USER_ID", _USER_ID }
                                };

                            var strOutParam = new[] { "P_RESULT" };
                            string callProc = "PK_EMS_EBSE006_01P.SP_EMS_PART_INFO_DELETE";

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
                                this.BaseClass.MsgError("ERR_INPUT_TYPE");

                                break;
                            }
                        }

                        if (_SUCCESS_CODE == "100")
                        {
                            da.CommitTransaction();

                            this.BaseClass.MsgInfo("CMPT_DEL");

                            SearchEmsPartList();
                            gridMainView.ExpandAllNodes();
                        }
                    }
                    catch (Exception ex)
                    {
                        da.RollbackTransaction();
                        this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                    }
                }
            }
        }
        #endregion row 삭제


        #region CheckBox Action

        List<CheckBox> Checks = new List<CheckBox>();
        bool fromClear = false;

        // <summary>
        /// CheckBox Check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Editor_Checked(object sender, RoutedEventArgs e)
        {
            EditGridCellData o = (sender as CheckBox).Tag as EditGridCellData;

            var target = EmsPartMngList.Where(f => f.PART_ID == o.Value.ToString()).SingleOrDefault();

            if (null != target)
            {
                target.IsChecked = true;

                Checks.Add(sender as CheckBox);
            }
        }

        /// <summary>
        /// CheckBox Uncheck
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Editor_Unchecked(object sender, RoutedEventArgs e)
        {
            if (fromClear)
            {
                return;
            }

            EditGridCellData o = (sender as CheckBox).Tag as EditGridCellData;

            var target = EmsPartMngList.Where(f => f.PART_ID == o.Value.ToString()).SingleOrDefault();

            if (null != target)
            {
                target.IsChecked = false;

                Checks.Remove(sender as CheckBox);
            }
        }
        #endregion CheckBox Action
        #endregion

        #region ▩ 이벤트
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

        #region btnExcelDownload_PreviewMouseLeftButtonUp - 엑셀다운로드 버튼 클릭
        /// <summary>
        /// 엑셀다운로드 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Msg : DOWN_EXCEL - 엑셀 다운로드를 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EXCEL_DOWNLOAD");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                gridMainExcel.ItemsSource = EmsPartMngList;
                gridMainExcelView.ExpandAllNodes();

                await Task.Delay(100);

                List<TreeListView> treeListView = new List<TreeListView>();
                treeListView.Add(this.gridMainExcelView);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #endregion

        private void btnDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.EmsPartMngList.Where(p => p.IsSelected).Count() == 0)
                {
                    this.BaseClass.MsgInfo("ERR_NO_SELECT");
                    return;
                }

                this.DelEmsEqpMng();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void btnRegist_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                EmsPartInfo target = gridMain.SelectedItem as EmsPartInfo;

                if (null != target)
                {
                    EqpPartEdit(target.PART_CLS_ID, target.PART_CLS_NM);
                }
                else
                {
                    EqpPartEdit("", "");
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
    }
}
