using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using SMART.WCS.UI.EMS.DataMembers.E3001;
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

namespace SMART.WCS.UI.EMS.Views.CHECK_MGMT
{
    /// <summary>
    /// E3001.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E3001 : UserControl
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
        public E3001()
        {
            InitializeComponent();
        }

        public E3001(List<string> _liMenuNavigation)
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

                // CHOO
                // 2020-03-15 오류 때문에 주석처리
                // 공통코드 정의한 후에 주석해제
                //GetComDev();

                this.BaseClass.BindingCommonComboBox(this.cboChkDptDev, "CHK_CLS_CD", null, true);
                this.BaseClass.BindingCommonComboBox(this.cboChkRstDev, "CHK_RST_DEV", null, true);
                //this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, true);


                SetBaseButton();

                gridMainView.ShowingEditor += (s, e) =>
                {
                    //EmsAlarmStatus curr = gridMain.SelectedItem as EmsAlarmStatus;

                    e.Cancel = gridMain.CurrentColumn.FieldName != "IsChecked";
                    //e.Cancel = true;
                };

                this.searchStart.DateTime   = DateTime.Now.AddDays(-10);
                this.searchEnd.DateTime     = DateTime.Now.AddDays(10);
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
        public ObservableCollection<EmsCommCode> StateList { get; private set; }

        /// <summary>
        /// 점검 구분
        /// </summary>
        public ObservableCollection<EmsCommCode> ChkDptDevList { get; private set; }

        /// <summary>
        /// 실적 여부
        /// </summary>
        public ObservableCollection<EmsCommCode> ChkRstDevList { get; private set; }

        /// <summary>
        /// Grid Data
        /// </summary>
        public ObservableCollection<EmsChkPlan> EmsChkPlanList { get; private set; }
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
                //this.btnSaveHigh.IsEnabled = isHighEnabled;
                //this.btnEditHigh.IsEnabled = isHighEnabled;
                ////this.btnRowAddHigh.IsEnabled = isHighNewEnabled;
                //this.btnRowAddHigh.IsEnabled = isHighEnabled;
                //this.btnRowDeleteHigh.IsEnabled = isHighEnabled;

            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 권한별 버튼 활성화

        #region 공통코드 조회
        void GetComDev()
        {
            try
            {
                var _USER_ID = this.BaseClass.UserID;

                var paramDpt = new Dictionary<string, object>
                {
                    {"P_USER_ID", _USER_ID},
                    {"P_COM_HEAD_CD", "EMS_CHK_DPT_DEV"}
                };

                var strOutParamDpt = new[] { "P_COM_DETAIL_CD_LIST", "P_RESULT" };
                string callProc = "PK_COMM_CODE_MGT.SP_COMM_CODE_DETAIL_SEARCH";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outDataDpt = da.GetSpDataSet(
                                    callProc                        // 호출 프로시저
                            ,    paramDpt                        // Input 파라메터
                            ,   strOutParamDpt                  // Output 파라메터
                    );

                    // 리턴 테이블 체크 
                    if (outDataDpt.Tables.Count > 1)
                    {
                        ChkDptDevList = new ObservableCollection<EmsCommCode>();
                        ChkDptDevList.ToObservableCollection(outDataDpt.Tables[0]);

                        ChkDptDevList.Insert(0, new EmsCommCode()
                        {
                            COM_DETAIL_CD = " ",
                            COM_DETAIL_NM = "전체",
                            COM_HEAD_CD = "EMS_CHK_DPT_DEV"
                        });

                        cboChkDptDev.ItemsSource = ChkDptDevList;
                        cboChkDptDev.SelectedItem = ChkDptDevList[0];

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }
                }


                var paramRst = new Dictionary<string, object>
                {
                    {"P_USER_ID", _USER_ID},
                    {"P_COM_HEAD_CD", "EMS_CHK_RST_DEV"}
                };

                var strOutParamRst = new[] { "P_COM_DETAIL_CD_LIST", "P_RESULT" };
                callProc = "PK_COMM_CODE_MGT.SP_COMM_CODE_DETAIL_SEARCH";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outDataRst = da.GetSpDataSet(
                                callProc                        // 호출 프로시저
                            ,   paramRst                        // Input 파라메터
                            ,   strOutParamRst                  // Output 파라메터
                    );

                    // 리턴 테이블 체크 
                    if (outDataRst.Tables.Count > 1)
                    {
                        ChkRstDevList = new ObservableCollection<EmsCommCode>();
                        ChkRstDevList.ToObservableCollection(outDataRst.Tables[0]);

                        cboChkRstDev.ItemsSource = ChkRstDevList;
                        cboChkRstDev.SelectedItem = ChkRstDevList[0];

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }
                }


                var param2 = new Dictionary<string, object>
                {
                    {"P_USER_ID", _USER_ID},
                    {"P_COM_HEAD_CD", "EMS_STOCKT_STAT_CD"}
                };

                var strOutParam2 = new[] { "P_COM_DETAIL_CD_LIST", "P_RESULT" };
                string callProc2 = "PK_COMM_CODE_MGT.SP_COMM_CODE_DETAIL_SEARCH";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData2 = da.GetSpDataSet(
                                callProc2                     // 호출 프로시저
                            ,   param2                       // Input 파라메터
                            ,   strOutParam2                 // Output 파라메터
                    );

                    // 리턴 테이블 체크 
                    if (outData2.Tables.Count > 1)
                    {
                        StateList = new ObservableCollection<EmsCommCode>();
                        StateList.ToObservableCollection(outData2.Tables[0]);

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }
                }

                searchStart.DateTime = DateTime.Now.AddMonths(-6);
                searchEnd.DateTime = searchStart.DateTime.AddMonths(12);
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 공통코드 조회

        #region 조회
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                focused_handle = -1;
                SearchEmsChkPlan();
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
        void SearchEmsChkPlan()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                EmsCommCode dptDev = cboChkDptDev.SelectedItem as EmsCommCode;
                EmsCommCode rstDev = cboChkRstDev.SelectedItem as EmsCommCode;

                var strDeptDev  = this.BaseClass.ComboBoxSelectedKeyValue(this.cboChkDptDev);
                var strRstDev   = this.BaseClass.ComboBoxSelectedKeyValue(this.cboChkRstDev);

                //DateTime start = new DateTime(searchStart.DateTime.Year, searchStart.DateTime.Month, searchStart.DateTime.Day);
                //DateTime end = new DateTime(searchEnd.DateTime.Year, searchEnd.DateTime.Month, searchEnd.DateTime.Day);

                var strStartDate        = this.BaseClass.GetCalendarValue(this.searchStart);
                var strEndDate          = this.BaseClass.GetCalendarValue(this.searchEnd);


                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_CHK_DEV_CD",    strDeptDev},
                    {"P_FROM_DT",       strStartDate},
                    {"P_TO_DT",         strEndDate},
                    {"P_CHK_RST_DEV",   strRstDev}
                };

                var strOutParam = new[] { "P_EMS_CHK_LIST" };
                string callProc = "PK_EMS_ECHK001.SP_EMS_CHK_PLAN_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    if (null != EmsChkPlanList)
                    {
                        EmsChkPlanList.Clear();
                        EmsChkPlanList = null;
                    }

                    EmsChkPlanList = new ObservableCollection<EmsChkPlan>();
                    
                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        EmsChkPlanList.ToObservableCollection(outData.Tables[0]);
                        this.EmsChkPlanList.Where(p => p.CHK_STAT_CD != "F").ForEach(p => p.CHK_STAT_CD = "N");
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    gridMain.ItemsSource = EmsChkPlanList;

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        if (-1 < focused_handle)
                        {
                            gridMain.View.FocusedRowHandle = focused_handle;
                            gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsChkPlan>)[focused_handle];
                        }
                        else
                        {
                            gridMain.View.FocusedRowHandle = 0;
                            gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsChkPlan>)[0];
                        }
                    }

                    //this.btnSaveHigh.IsEnabled = true;
                    //this.btnEditHigh.IsEnabled = true;
                    //this.btnRowAddHigh.IsEnabled = true;
                    //this.btnRowDeleteHigh.IsEnabled = true;

                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 조회

        #region row 추가 / 수정        
        /// <summary>
        /// 수정 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnEdit_Click(object sender, MouseButtonEventArgs e)
        {
            EmsChkPlan target = gridMain.SelectedItem as EmsChkPlan;

            if (null != target)
            {
                focused_handle = gridMain.View.FocusedRowHandle;

                ChkPlanEdit(target.CHK_ID);
            }
        }

        /// <summary>
        /// 추가 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowAdd_Click(object sender, MouseButtonEventArgs e)
        {
            ChkPlanEdit();
        }

        /// <summary>
        /// 추가/수정 함수
        /// </summary>
        /// <param name="target"></param>
        void ChkPlanEdit(int target = -1)
        {
            using (E3001_01P frmChkReg = new E3001_01P(target))
            {
                frmChkReg.ShowDialog();

                if (frmChkReg.IsSaved)
                {
                    SearchEmsChkPlan();
                }
            }
        }

        /// <summary>
        /// Grid Data Select
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridMainView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) { return; }

            if (gridMain.IsGroupRowHandle(rowHandle)) { return; } // A group row has been double clicked

            EmsChkPlan target = gridMain.SelectedItem as EmsChkPlan;

            if (null != target)
            {
                //if ("F" == target.CHK_STAT_CD)
                //{
                //    BaseClass.MsgInfo("확정된 내용을 수정할 수 없습니다.", this.BaseInfo.country_cd);
                //    return;
                //}

                focused_handle = gridMain.View.FocusedRowHandle;

                ChkRstEdit(target.CHK_ID, target.CHK_STAT_CD);
            }
        }

        /// <summary>
        /// 점검 실적 수정
        /// </summary>
        /// <param name="target"></param>
        /// <param name="state"></param>
        void ChkRstEdit(int target = -1, string state = "")
        {
            try
            {
                using (E3001_04P frmRstReg = new E3001_04P(target, state))
                {
                    frmRstReg.ShowDialog();

                    if (frmRstReg.IsSaved)
                    {
                        SearchEmsChkPlan();
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion row 추가 / 수정


        #endregion

        #region ▩ 이벤트
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
                this.BaseClass.GetExcelDownload(tableView);
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

        private void btnRowDel_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                focused_handle = -1;
                DelEmsChkPlan();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void DelEmsChkPlan()
        {
            //var _delItems = EmsChkPlanList.Where(f => f.IsChecked).ToList();
            var item = gridMain.SelectedItem as EmsChkPlan;

            //if (0 < _delItems.Count)
            if (null != item)
            {
                if ("F" == item.CHK_STAT_CD)
                {
                    // 확정된 내용을 삭제할 수 없습니다.
                    this.BaseClass.MsgError("EMS_FFDEL_NO");
                    return;
                }

                // 삭제하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_DEL");

                if (!this.BaseClass.BUTTON_CONFIRM_YN)
                {
                    return;
                }

                //var _USER_ID = this.BaseInfo.user_id.ToString();
                string _SUCCESS_CODE = "100";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        da.BeginTransaction();

                        //foreach (var item in _delItems)
                        {
                            var param = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                    { "P_CHK_ID", item.CHK_ID }
                                };

                            var strOutParam = new[] { "P_RESULT" };
                            string callProc = "PK_EMS_ECHK001.SP_EMS_CHK_PLAN_DELETE";

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
                                }
                            }
                            else
                            {
                                _SUCCESS_CODE = "0";
                                da.RollbackTransaction();
                                this.BaseClass.MsgError("ERR_INPUT_TYPE");
                            }
                        }

                        if (_SUCCESS_CODE == "100")
                        {
                            da.CommitTransaction();

                            focused_handle = -1;
                            SearchEmsChkPlan();
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
    }
}
