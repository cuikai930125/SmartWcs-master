using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using SMART.WCS.UI.EMS.DataMembers.ESPA002;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.EMS.Views.SPACE_PARTS
{
    /// <summary>
    /// E2002.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E2002 : UserControl
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
        public E2002()
        {
            InitializeComponent();
        }

        public E2002(List<string> _liMenuNavigation)
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

                //GetInspStatDev();

                searchEnd.DateTime = DateTime.Now;
                searchStart.DateTime = searchEnd.DateTime.AddYears(-1);

                //SetBaseButton();

                gridMainView.ShowingEditor += (s, e) =>
                {
                    e.Cancel = gridMain.CurrentColumn.FieldName != "NOTE";
                };

                gridDetailView.ShowingEditor += (s, e) =>
                {
                    // 확정시 수정 금지
                    // 확정완료 : CMPT (F)
                    // 진행중 : PROC (P)
                    if ("CMPT" == CurrentStock.INSP_STAT_CD)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        e.Cancel = gridDetail.CurrentColumn.FieldName != "INSP_QTY";
                    }
                };
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        /// <summary>
        /// 진행상태
        /// </summary>
        public ObservableCollection<EmsCommCode> InspStateList { get; private set; }

        /// <summary>
        /// Master Grid Data
        /// </summary>
        public ObservableCollection<EmsStock> StockList { get; private set; }

        /// <summary>
        /// Detail Grid Data
        /// </summary>
        public ObservableCollection<EmsStockDetail> CurrentStockDetail { get; private set; }

        private EmsStock CurrentStock { get; set; }
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

                // 그리드 초기화
                this.btnExcelHigh.IsEnabled = isHighEnabled;
                this.btnConfirmHigh.IsEnabled = isHighEnabled;
                this.btnSaveHigh.IsEnabled = isHighEnabled;
                this.btnAddHigh.IsEnabled = isHighEnabled;
                this.btnRowDeleteHigh.IsEnabled = isHighEnabled;

            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 권한별 버튼 활성화

        #region 진행 상태
        /// <summary>
        /// 진행 상태
        /// </summary>
        private void GetInspStatDev()
        {
            try
            {
                var _USER_ID = this.BaseClass.UserID;

                var param = new Dictionary<string, object>
                {
                    {"P_USER_ID", _USER_ID},
                    {"P_COM_HEAD_CD", "EMS_STOCKT_STAT_CD"}
                };

                var strOutParam = new[] { "P_COM_DETAIL_CD_LIST", "P_RESULT" };
                string callProc = "PK_COMM_CODE_MGT.SP_COMM_CODE_DETAIL_SEARCH";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            , param                        // Input 파라메터
                            , strOutParam                  // Output 파라메터
                    );

                    // 리턴 테이블 체크 
                    if (outData.Tables.Count > 1)
                    {
                        InspStateList = new ObservableCollection<EmsCommCode>();
                        InspStateList.ToObservableCollection(outData.Tables[0]);

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 진행 상태 

        #region 조회
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStockSearch_Click(object sender, MouseButtonEventArgs e)
        {
            focused_handle = -1;
            SearchStock();
        }

        int focused_handle = -1;
        /// <summary>
        /// 조회 함수
        /// </summary>
        void SearchStock()
        {
            try
            {
                var strStartDate        = this.BaseClass.GetCalendarValue(this.searchStart);
                var strEndDate          = this.BaseClass.GetCalendarValue(this.searchEnd);

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_FROM_DT", strStartDate },
                    {"P_TO_DT", strEndDate}
                };

                var strOutParam = new[] { "P_EMS_STOCK_LIST" };
                //string callProc = "PK_EMS_ESPA002.SP_EMS_STOCK_SEL";
                string callProc = "PK_EMS_ESPA002.SP_EMS_STOCK_PERIOD_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            , param                        // Input 파라메터
                            , strOutParam                  // Output 파라메터
                    );

                    if (null != StockList)
                    {
                        StockList.Clear();
                        StockList = null;
                    }

                    StockList = new ObservableCollection<EmsStock>();

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        StockList.ToObservableCollection(outData.Tables[0]);

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    gridMain.ItemsSource = StockList;

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        if (-1 < focused_handle)
                        {
                            gridMain.View.FocusedRowHandle = focused_handle;
                            gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsStock>)[focused_handle];
                        }
                        else
                        {
                            gridMain.View.FocusedRowHandle = 0;
                            gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsStock>)[0];
                        }

                        EmsStock target = gridMain.SelectedItem as EmsStock;

                        if (null != target)
                        {
                            CurrentStock = target;
                            SearchStockDetail(target.STOCK_INSP_DT);
                        }
                    }
                    else
                    {
                        CurrentStock = null;

                        if (null != CurrentStockDetail)
                        {
                            CurrentStockDetail.Clear();
                            CurrentStockDetail = null;
                        }
                    }

                    this.btnConfirmHigh.IsEnabled = true;
                    this.btnSaveHigh.IsEnabled = true;
                    this.btnAddHigh.IsEnabled = true;
                    this.btnRowDeleteHigh.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }

        /// <summary>
        /// Detail Grid Data Binding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridMainView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) { return; }

            if (gridMain.IsGroupRowHandle(rowHandle)) { return; }// A group row has been double clicked

            EmsStock target = gridMain.SelectedItem as EmsStock;

            if (null != target)
            {
                CurrentStock = target;
                SearchStockDetail(target.STOCK_INSP_DT);
            }
        }

        /// <summary>
        /// Detail Grid Data Search
        /// </summary>
        /// <param name="target"></param>
        void SearchStockDetail(DateTime target)
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();
                var strTargetDate   = target.ToString("yyyyMMdd");

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    { "P_STOCK_INSP_DT", strTargetDate }
                };

                var strOutParam = new[] { "P_EMS_STOCK_DETAIL_LIST" };
                string callProc = "PK_EMS_ESPA002.SP_EMS_STOCK_DETAIL_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            , param                        // Input 파라메터
                            , strOutParam                  // Output 파라메터
                    );

                    if (null != CurrentStockDetail)
                    {
                        CurrentStockDetail.Clear();
                        CurrentStockDetail = null;
                    }

                    CurrentStockDetail = new ObservableCollection<EmsStockDetail>();

                    if (outData.Tables[0].Rows.Count > 1)
                    {
                        CurrentStockDetail.ToObservableCollection(outData.Tables[0]);

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    gridDetail.ItemsSource = CurrentStockDetail;

                    this.btnExcelHigh.IsEnabled = true;

                    if (checkYes.IsChecked.Value)
                    {
                        foreach (EmsStockDetail item in CurrentStockDetail)
                        {
                            item.IsHide = ("Y" != item.STOCK_MNG_YN);
                        }

                        gridDetail.RefreshData();
                    }
                    else
                    {
                        checkYes.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 조회

        #region 저장
        /// <summary>
        /// 저장 함수 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStockDetailSave_Click(object sender, MouseButtonEventArgs e)
        {
            if (StockDetailSave())
            {
                this.BaseClass.MsgInfo("CMPT_SAVE");

                focused_handle = gridMain.View.FocusedRowHandle;
                SearchStock();
            }
        }

        /// <summary>
        /// 저장 함수
        /// </summary>
        /// <returns></returns>
        bool StockDetailSave(bool from_confirm = false)
        {
            bool result = false;

            using (BaseDataAccess da = new BaseDataAccess())
            {
                try
                {
                    string _SUCCESS_CODE = "100";

                    var _USER_ID = this.BaseClass.UserID;

                    var _stockSave = StockList.Where(f => f.IsUpdate).ToList();

                    if (0 < _stockSave.Count)
                    {
                        da.BeginTransaction();

                        foreach (var item in _stockSave)
                        {
                            var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                { "P_STOCK_INSP_DT", item.STOCK_INSP_DT.ToString("yyyyMMdd") },// new DateTime(item.STOCK_INSP_DT.Year, item.STOCK_INSP_DT.Month, item.STOCK_INSP_DT.Day) }, // 재고 조사 일자
                                { "P_NOTE", item.NOTE },                                                                                        // 비고
                                { "P_USER_ID", _USER_ID }
                            };

                            var strOutParam = new[] { "P_RESULT" };
                            string callProc = "PK_EMS_ESPA002_01P.SP_EMS_STOCK_INSP_DT_UPDATE";

                            var outData = da.GetSpDataSet(
                                        callProc                      // 호출 프로시저
                                    , param                        // Input 파라메터
                                    , strOutParam                  // Output 파라메터
                            );

                            if (outData.Tables[0].Rows.Count > 0)
                            {
                                if (outData.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
                                {
                                    _SUCCESS_CODE = "0";
                                    da.RollbackTransaction();
                                    BaseClass.MsgInfo(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);

                                    return false;
                                }
                            }
                            else
                            {
                                _SUCCESS_CODE = "0";
                                da.RollbackTransaction();
                                this.BaseClass.MsgError("ERR_INPUT_TYPE");

                                return false;
                            }
                        }

                        if (_SUCCESS_CODE == "100")
                        {
                            result = true;
                        }
                        else
                        {
                            da.RollbackTransaction();
                            return false;
                        }
                    }


                    var _saveItems = CurrentStockDetail.Where(f => f.IsUpdate).ToList();

                    if (0 == _saveItems.Count)
                    {
                        if (result)
                        {
                            da.CommitTransaction();
                        }

                        return result ? true : from_confirm;
                    }

                    if (!result)
                    {
                        da.BeginTransaction();
                    }

                    foreach (var item in _saveItems)
                    {
                        var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                { "P_STOCK_INSP_DT", item.STOCK_INSP_DT.Replace("-", string.Empty) }, //new DateTime(item.STOCK_INSP_DT.Year, item.STOCK_INSP_DT.Month, item.STOCK_INSP_DT.Day) }, // 재고 조사 일자
                                { "P_PART_ID", item.PART_ID },          // ID
                                { "P_INSP_QTY", item.INSP_QTY },        // 재고 조사 수량
                                { "P_WH_NOTE", item.NOTE },             // 비고
                                {"P_USER_ID", _USER_ID}
                            };

                        var strOutParam = new[] { "P_RESULT" };
                        string callProc = "PK_EMS_ESPA002.SP_EMS_STOCK_DETAIL_UPDATE";

                        var outData = da.GetSpDataSet(
                                    callProc                      // 호출 프로시저
                                , param                        // Input 파라메터
                                , strOutParam                  // Output 파라메터
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
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_SAVE_DATA"));

                        return true;
                    }

                }
                catch (Exception ex)
                {
                    result = false;

                    da.RollbackTransaction();
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }

            return result;
        }
        #endregion 저장

        #region 재고조사일자 등록
        private void btnRegDt_Click(object sender, MouseButtonEventArgs e)
        {
            using (E2002_01P frmRegDt = new E2002_01P())
            {
                frmRegDt.ShowDialog();

                if (frmRegDt.IsSaved)
                {
                    StockList.Insert(0, frmRegDt.CurrentStock);

                    gridMain.View.FocusedRowHandle = 0;
                    gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsStock>)[0];

                    EmsStock target = gridMain.SelectedItem as EmsStock;

                    if (null != target)
                    {
                        CurrentStock = target;
                        SearchStockDetail(target.STOCK_INSP_DT);
                    }
                }
            }
        }
        #endregion 재고조사일자 등록

        #region 삭제
        /// <summary>
        /// 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, MouseButtonEventArgs e)
        {
            EmsStock target = gridMain.SelectedItem as EmsStock;

            if (null == target)
            {
                return;
            }

            if ("CMPT" == target.INSP_STAT_CD)
            {
                // 미확정 항목만 삭제 됩니다.
                this.BaseClass.MsgError("EMS_NOF_DEL");
                return;
            }

            var strConDate      = target.STOCK_INSP_DT.ToString("yyyy-MM-dd");
            var strConString    = this.BaseClass.GetResourceValue("EMS_REFHIS_DEL");   // 재고 조사 이력을 삭제합니다. 
            this.BaseClass.MsgQuestion($"{strConDate} : {strConString}", BaseEnumClass.CodeMessage.MESSAGE);
            if (this.BaseClass.BUTTON_CONFIRM_YN != true)
            {
                return;
            }

            using (BaseDataAccess da = new BaseDataAccess())
            {
                string _SUCCESS_CODE = "100";

                try
                {
                    da.BeginTransaction();

                    var _USER_ID = this.BaseClass.UserID;
                    var strStockInspDate    = target.STOCK_INSP_DT.ToString("yyyyMMdd");


                    var param = new Dictionary<string, object>
                    {
                        { "P_CENTER_CD", this.BaseClass.CenterCD },
                        //{ "P_STOCK_INSP_DT", new DateTime(target.STOCK_INSP_DT.Year, target.STOCK_INSP_DT.Month, target.STOCK_INSP_DT.Day) },
                        { "P_STOCK_INSP_DT",    strStockInspDate},
                        {"P_USER_ID", _USER_ID}
                    };

                    var strOutParam = new[] { "P_RESULT" };
                    string callProc = "PK_EMS_ESPA002.SP_EMS_STOCK_DELETE";

                    var outData = da.GetSpDataSet(
                            callProc                      // 호출 프로시저
                        , param                        // Input 파라메터
                        , strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        if (outData.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
                        {
                            _SUCCESS_CODE = "0";
                            da.RollbackTransaction();
                            BaseClass.MsgInfo(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        }
                    }
                    else
                    {
                        _SUCCESS_CODE = "0";
                        da.RollbackTransaction();
                        this.BaseClass.MsgError("ERR_INPUT_TYPE");
                    }

                    if (_SUCCESS_CODE == "100")
                    {
                        da.CommitTransaction();

                        StockList.Remove(target);

                        gridMain.RefreshData();
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_DELETE"));
                    }
                }
                catch (Exception ex)
                {
                    da.RollbackTransaction();
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }
        }
        #endregion 삭제


        #region Confirm
        /// <summary>
        /// Confirm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStockConfirm_Click(object sender, MouseButtonEventArgs e)
        {
            EmsStock target = gridMain.SelectedItem as EmsStock;

            if (null == target)
            {
                return;
            }

            if ("CMPT" == target.INSP_STAT_CD)
            {
                // 이미 확정된 항목입니다.
                this.BaseClass.MsgError("EMS_ALREADY_F");
                return;
            }

            var strConDate = target.STOCK_INSP_DT.ToString("yyyy-MM-dd");
            var strConString = this.BaseClass.GetResourceValue("EMS_REF_FF");    // 재고 조사를 확정합니다.

            var strMessage = this.BaseClass.GetResourceValue("ASK_EMS_CONF_COND", BaseEnumClass.ResourceType.MESSAGE);
            this.BaseClass.MsgQuestion(string.Format(strMessage, strConDate), BaseEnumClass.CodeMessage.MESSAGE);

            //this.BaseClass.MsgQuestion($"{strConDate}{strConString}", BaseEnumClass.CodeMessage.MESSAGE);
            if (this.BaseClass.BUTTON_CONFIRM_YN != true)
            {
                return;
            }

            if (!StockDetailSave(true))
            {
                return;
            }

            using (BaseDataAccess da = new BaseDataAccess())
            {
                try
                {
                    string _SUCCESS_CODE = "100";

                    var _USER_ID = this.BaseClass.UserID;

                    da.BeginTransaction();


                    var param = new Dictionary<string, object>
                    {
                        { "P_CENTER_CD", this.BaseClass.CenterCD },
                        { "P_STOCK_INSP_DT", new DateTime(target.STOCK_INSP_DT.Year, target.STOCK_INSP_DT.Month, target.STOCK_INSP_DT.Day) },
                        { "P_INSP_STAT_CD", "CMPT" },   // 확정 (value : F -> CMPT로 수정)
                        {"P_USER_ID", _USER_ID}
                    };

                    var strOutParam = new[] { "P_RESULT" };
                    string callProc = "PK_EMS_ESPA002.SP_EMS_STOCK_FIX";

                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            , param                        // Input 파라메터
                            , strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        if (outData.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
                        {
                            _SUCCESS_CODE = "0";
                            da.RollbackTransaction();
                            BaseClass.MsgInfo(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        }
                    }
                    else
                    {
                        _SUCCESS_CODE = "0";
                        da.RollbackTransaction();
                        this.BaseClass.MsgError("ERR_INPUT_TYPE");
                    }

                    if (_SUCCESS_CODE == "100")
                    {
                        da.CommitTransaction();

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_SAVE_DATA"));

                        // 확정되었습니다.
                        this.BaseClass.MsgInfo("CMPT_CONF");

                        focused_handle = gridMain.View.FocusedRowHandle;
                        SearchStock();
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
        #endregion Confirm

        #region btnExcelDownload_PreviewMouseLeftButtonUp - 엑셀다운로드 버튼 클릭
        /// <summary>
        /// 엑셀다운로드 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Msg : DOWN_EXCEL - 엑셀 다운로드를 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EXCEL_DOWNLOAD");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                List<TableView> tableView = new List<TableView>();
                tableView.Add(this.gridMainView);
                tableView.Add(this.gridDetailView);
                this.BaseClass.GetExcelDownload(tableView);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region 재고관리여부
        private void RadioButtonYes_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (null == CurrentStockDetail || 0 == CurrentStockDetail.Count)
            {
                return;
            }

            foreach (EmsStockDetail item in CurrentStockDetail)
            {
                item.IsHide = ("Y" != item.STOCK_MNG_YN);
            }

            gridDetail.RefreshData();
        }

        private void RadioButtonAll_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (null == CurrentStockDetail || 0 == CurrentStockDetail.Count)
            {
                return;
            }

            foreach (EmsStockDetail item in CurrentStockDetail)
            {
                item.IsHide = false;
            }

            gridDetail.RefreshData();
        }

        #endregion 재고관리여부
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

        #endregion

        private void gridMain_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int rowHandle = gridMainView.GetRowHandleByMouseEventArgs(e);
                if (rowHandle == GridControl.InvalidRowHandle) { return; }

                if (gridMain.IsGroupRowHandle(rowHandle)) { return; }// A group row has been double clicked

                EmsStock target = gridMain.SelectedItem as EmsStock;

                if (null != target)
                {
                    CurrentStock = target;
                    SearchStockDetail(target.STOCK_INSP_DT);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
    }
}
