using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.E1004;
using SMART.WCS.UI.EMS.Views.COM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMART.WCS.UI.EMS.Views.BASE_INFO
{
    /// <summary>
    /// E1004.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E1004 : UserControl
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
        public E1004()
        {
            InitializeComponent();
        }

        public E1004(List<string> _liMenuNavigation)
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

                // 분류 행 색상
                //  
                var profitFormatCondition = new FormatCondition()
                {
                    ValueRule = ConditionRule.Expression,
                    Expression = "[ORDER_DEV] == 'A'",
                    FieldName = null,
                    Format = new DevExpress.Xpf.Core.ConditionalFormatting.Format()
                    {
                        Background = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0x7F, 0x50))
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
                        Background = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0x7F, 0x50))
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
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        public static readonly DependencyProperty EmsEqpMngListProperty
                                  = DependencyProperty.Register("EmsEqpMngList",
                                      typeof(ObservableCollection<EmsEqpMng>),
                                             typeof(E1004), new PropertyMetadata(new ObservableCollection<EmsEqpMng>()));
        /// <summary>
        /// 설비 List
        /// </summary>
        public ObservableCollection<EmsEqpMng> EmsEqpMngList
        {
            get { return (ObservableCollection<EmsEqpMng>)GetValue(EmsEqpMngListProperty); }
            private set { SetValue(EmsEqpMngListProperty, value); }
        }
        #endregion

        #region ▩ 함수

        #endregion

        #region ▩ 이벤트
        #region 조회
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchClick(object sender, MouseButtonEventArgs e)
        {
            SearchEmsEqpMng(true);
            gridMainView.ExpandAllNodes();

            this.btnExcelHigh.IsEnabled = true;
        }

        int focused_handle = -1;
        /// <summary>
        /// 조회 함수
        /// </summary>
        /// <param name="btn"></param>
        private void SearchEmsEqpMng(bool btn = false)
        {
            focused_handle = -1;

            if (!btn && null != EmsEqpMngList)
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

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_EQP_CLS_ID", (string.IsNullOrEmpty(tbxSearchEqp.Text.Trim()) ? "" :  tbxSearchEqp.Tag.ToString())},
                    {"P_PBS_ID", (string.IsNullOrEmpty(tbxSearchPbs.Text.Trim()) ? "" :  tbxSearchPbs.Tag.ToString())},
                };

                var strOutParam = new[] { "P_EMS_EQP_MNG_LIST" };
                string callProc = "PK_EMS_EBSE004.SP_EMS_EQP_MNG_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        EmsEqpMngList = new ObservableCollection<EmsEqpMng>();
                        EmsEqpMngList.ToObservableCollection(outData.Tables[0]);

                        this.gridMain.ItemsSource = this.EmsEqpMngList;

                        //if (-1 < focused_handle)
                        //{
                        //    gridMain.View.FocusedRowHandle = focused_handle;
                        //    gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsEqpMng>)[focused_handle];
                        //}
                        //else
                        //{
                        //    gridMain.View.FocusedRowHandle = 0;
                        //    gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsEqpMng>)[0];
                        //}

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    //this.btnSaveHigh.IsEnabled = true;
                    this.btnRowAddHigh.IsEnabled = true;
                    this.btnRowDeleteHigh.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 조회

        #region row 추가 / 수정
        /// <summary>
        /// 추가 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowAddClick(object sender, MouseButtonEventArgs e)
        {
            //EqpRegEdit((string.IsNullOrEmpty(tbxSearchEqp.Text.Trim()) ? "" : tbxSearchEqp.Tag.ToString()), tbxSearchEqp.Text.Trim());

            EmsEqpMng target = gridMain.SelectedItem as EmsEqpMng;

            if (null != target)
            {
                EqpRegEdit(target.EQP_CLS_ID, target.EQP_CLS_NM);
            }
            else
            {
                EqpRegEdit("", "");
            }
        }

        /// <summary>
        /// 추가 / 수정 창 호출
        /// </summary>
        /// <param name="cls_target"></param>
        /// <param name="cls_name"></param>
        /// <param name="target"></param>
        void EqpRegEdit(string cls_target, string cls_name, string target = "")
        {
            try
            {
                using (E1004_01P frmEqpReg = new E1004_01P(cls_target, cls_name, target))
                {
                    frmEqpReg.ShowDialog();
                }

                SearchEmsEqpMng();
                gridMainView.ExpandAllNodes();
            }
            catch { throw; }
        }

        /// <summary>
        /// 선택 행 수정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int rowHandle = gridMainView.GetRowHandleByMouseEventArgs(e);
                if (rowHandle == GridControl.InvalidRowHandle) { return; }

                if (gridMain.IsGroupRowHandle(rowHandle)) { return; } // A group row has been double clicked

                EmsEqpMng target = gridMain.SelectedItem as EmsEqpMng;

                if (null != target)
                {
                    if ("A" == target.ORDER_DEV)
                    {
                        //EqpRegEdit(target.EQP_CLS_ID, target.EQP_CLS_NM);
                    }
                    else
                    {
                        EqpRegEdit(target.EQP_CLS_ID, target.EQP_CLS_NM, target.EQP_ID);
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion row 추가 / 수정

        #region row 삭제
        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowDelClick(object sender, MouseButtonEventArgs e)
        {
            DelEmsEqpMng();
        }

        /// <summary>
        /// 삭제 함수
        /// </summary>
        private void DelEmsEqpMng()
        {
            var _delItems = EmsEqpMngList.Where(f => f.IsChecked).ToList();

            if (0 < _delItems.Count)
            {
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
                                    { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                    { "P_EQP_ID",       item.EQP_ID },
                                    { "P_USER_ID",      _USER_ID }
                                };

                            var strOutParam = new[] { "P_RESULT" };
                            string callProc = "PK_EMS_EBSE004_01P.SP_EMS_EQP_INFO_DELETE";

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

                            SearchEmsEqpMng();
                            gridMainView.ExpandAllNodes();
                        }
                    }
                    catch (Exception ex)
                    {
                        da.RollbackTransaction();
                        this.BaseClass.Error(ex);
                        this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                    }
                }
            }
        }
        #endregion row 삭제

        #region 설비 분류 검색
        private void EqpClsSearch_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                tbxSearchEqp.Focus();

                using (ECOM001_08P frmEqpClsSearch = new ECOM001_08P())
                {
                    frmEqpClsSearch.ShowDialog();

                    if (null != frmEqpClsSearch.CurrEqpCls)
                    {
                        tbxSearchEqp.Text = frmEqpClsSearch.CurrEqpCls.EQP_CLS_NM.Trim();
                        tbxSearchEqp.Tag = frmEqpClsSearch.CurrEqpCls.EQP_CLS_ID;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion 설비 분류 검색

        #region 설비 위치 검색
        private void PbsSearch_Click(object sender, MouseButtonEventArgs e)
        {
            tbxSearchPbs.Focus();

            //ECOM001_02P frmPbsSearch = new ECOM001_02P();
            //frmPbsSearch.ShowDialog();

            //if (null != frmPbsSearch.CurrPbs)
            //{
            //    tbxSearchPbs.Text = frmPbsSearch.CurrPbs.PBS_NM.Trim();
            //    tbxSearchPbs.Tag = frmPbsSearch.CurrPbs.PBS_ID;
            //}

            //frmPbsSearch = null;
        }
        #endregion 설비 위치 검색

        #region CheckBox Action
        List<CheckBox> Checks = new List<CheckBox>();
        bool fromClear = false;

        /// <summary>
        /// CheckBox Check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Editor_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                EditGridCellData o = (sender as CheckBox).Tag as EditGridCellData;

                var target = EmsEqpMngList.Where(f => f.EQP_ID == o.Value.ToString()).SingleOrDefault();

                if (null != target)
                {
                    target.IsChecked = true;

                    Checks.Add(sender as CheckBox);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// CheckBox Uncheck
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Editor_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fromClear)
                {
                    return;
                }

                EditGridCellData o = (sender as CheckBox).Tag as EditGridCellData;

                var target = EmsEqpMngList.Where(f => f.EQP_ID == o.Value.ToString()).SingleOrDefault();

                if (null != target)
                {
                    target.IsChecked = false;

                    Checks.Remove(sender as CheckBox);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion CheckBox Action

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
                // CHOO
                //// Msg : DOWN_EXCEL - 엑셀 다운로드를 하시겠습니까?
                //string strMessage = CJFC.Modules.Utility.GetMessageByCommon("ASK_DOWN_EXCEL");
                //this.BaseClass.MsgQust(strMessage, this.BaseInfo.country_cd);
                //if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                //gridMainExcel.ItemsSource = EmsEqpMngList;
                //gridMainExcelView.ExpandAllNodes();

                //await Task.Delay(100);

                //List<TreeListView> treeListView = new List<TreeListView>();
                //treeListView.Add(this.gridMainExcelView);
                //this.BaseClass.GetExcelDownloadTreeListView(treeListView);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
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

        private void btnRowDeleteClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
