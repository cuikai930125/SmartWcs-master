using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using SMART.WCS.UI.EMS.DataMembers.ECHK002;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.EMS.Views.CHECK_MGMT
{
    /// <summary>
    /// E3002.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E3002 : UserControl
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
        public E3002()
        {
            InitializeComponent();
        }

        public E3002(List<string> _liMenuNavigation)
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

            //SetBaseButton();

            this.btnRegist.PreviewMouseLeftButtonUp += BtnRegist_PreviewMouseLeftButtonUp;
            this.btnDelete.PreviewMouseLeftButtonUp += BtnDelete_PreviewMouseLeftButtonUp;
            this.gridMain.PreviewMouseDoubleClick += GridMain_PreviewMouseDoubleClick;

            this.BaseClass.BindingCommonComboBox(this.cboChkRepDev, "FAIL_TYPE_CD", null, true);

        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        /// <summary>
        /// 진행상태
        /// </summary>
        public ObservableCollection<EmsCommCode> StateList { get; private set; }

        /// <summary>
        /// 장애 구분
        /// </summary>
        public ObservableCollection<EmsCommCode> ChkRepDevList { get; private set; }

        /// <summary>
        /// Grid Data
        /// </summary>
        public ObservableCollection<EmsFail> EmsFailList { get; private set; }
        #endregion

        #region ▩ 함수
        #region 조회
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, MouseButtonEventArgs e)
        {
            focused_handle = -1;
            SearchEmsFailList();
        }

        int focused_handle = -1;
        /// <summary>
        /// 조회 함수
        /// </summary>
        void SearchEmsFailList()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                EmsCommCode repDev = cboChkRepDev.SelectedItem as EmsCommCode;
                //DateTime start = new DateTime(searchStart.DateTime.Year, searchStart.DateTime.Month, searchStart.DateTime.Day);
                //DateTime end = new DateTime(searchEnd.DateTime.Year, searchEnd.DateTime.Month, searchEnd.DateTime.Day);
                var strStartDate        = this.BaseClass.GetCalendarValue(this.searchStart);
                var strEndDate          = this.BaseClass.GetCalendarValue(this.searchEnd);
                var strFailType = this.BaseClass.ComboBoxSelectedKeyValue(this.cboChkRepDev);

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",         this.BaseClass.CenterCD },
                    {"P_FROM_DT",           strStartDate},
                    {"P_TO_DT",             strEndDate},
                    {"P_FAIL_DEV_CD",       strFailType}
                };

                var strOutParam     = new[] { "P_EMS_FAIL_LIST" };
                string callProc     = "PK_EMS_ECHK002.SP_EMS_FAIL_LIST_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                                ,   param                        // Input 파라메터
                            , strOutParam                  // Output 파라메터
                    );

                    if (null != EmsFailList)
                    {
                        EmsFailList.Clear();
                        EmsFailList = null;
                    }

                    EmsFailList = new ObservableCollection<EmsFail>();

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        EmsFailList.ToObservableCollection(outData.Tables[0]);
                    }

                    gridMain.ItemsSource = EmsFailList;

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        if (-1 < focused_handle)
                        {
                            gridMain.View.FocusedRowHandle = focused_handle;
                            gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsFail>)[focused_handle];
                        }
                        else
                        {
                            gridMain.View.FocusedRowHandle = 0;
                            gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsFail>)[0];
                        }
                    }
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
        /// 장애등록
        /// </summary>
        /// <param name="failNo"></param>
        /// <param name="state"></param>
        void EchkErrReg(int failNo = -1, string state = "")
        {
            using (E3002_01P frmEchkErrReg = new E3002_01P(failNo, state))
            {
                frmEchkErrReg.ShowDialog();

                if (frmEchkErrReg.IsSaved)
                {
                    if (-1 == failNo)
                    {
                        //focused_handle = EmsFailList.Count;
                        focused_handle = 0;
                    }

                    SearchEmsFailList();
                }
            }
        }

        /// <summary>
        /// Grid Data 선택 (수정)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridMainView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) { return; }

            if (gridMain.IsGroupRowHandle(rowHandle)) { return; } // A group row has been double clicked

            EmsFail target = gridMain.SelectedItem as EmsFail;

            if (null != target)
            {
                //if ("F" == target.FAIL_STAT_CD)
                //{
                //    BaseClass.MsgInfo("확정된 내용을 수정할 수 없습니다.", this.BaseInfo.country_cd);
                //    return;
                //}

                focused_handle = gridMain.View.FocusedRowHandle;

                EchkErrReg(target.FAIL_NO, target.FAIL_STAT_CD);
            }
        }
        #endregion row 추가 / 수정

        #region row 삭제
        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowDel_Click(object sender, MouseButtonEventArgs e)
        {
            DelEmsFail();
        }

        /// <summary>
        /// 삭제 함수
        /// </summary>
        void DelEmsFail()
        {
            //var _delItems = EmsFailList.Where(f => f.IsChecked).ToList();
            var item = gridMain.SelectedItem as EmsFail;

            //if (0 < _delItems.Count)
            if (null != item)
            {
                if ("F" == item.FAIL_STAT_CD)
                {
                    // 미확정 항목만 삭제 됩니다.
                    this.BaseClass.MsgError("EMS_NOF_DEL");
                    return;
                }

                // 삭제하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_DEL");

                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                //var _USER_ID = this.BaseInfo.user_id.ToString();
                string _SUCCESS_CODE = "100";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        da.BeginTransaction();

                        //foreach (var item in _delItems)
                        {
                            if (!item.IsNew)
                            {
                                var param = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                    { "P_FAIL_NO", item.FAIL_NO }
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_ECHK002.SP_EMS_FAIL_DELETE";

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
                                        //break;
                                    }
                                }
                                else
                                {
                                    _SUCCESS_CODE = "0";
                                    da.RollbackTransaction();
                                    this.BaseClass.MsgError("ERR_INPUT_TYPE");
                                    //break;
                                }
                            }
                        }

                        if (_SUCCESS_CODE == "100")
                        {
                            da.CommitTransaction();

                            //foreach (var item in _delItems)
                            {
                                EmsFailList.Remove(item);
                            }

                            gridMain.RefreshData();
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
        #endregion

        private bool DeleteSP_EMS_FAIL_DELETE(BaseDataAccess _da, EmsFail _item)
        {
            try
            {
                bool isRtnValue = true;

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_EMS_ECHK002.SP_EMS_FAIL_DELETE";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strCenterCD     = this.BaseClass.CenterCD;
                var iFailNo         = _item.FAIL_NO;

                var strErrCode      = string.Empty;                         // 오류 코드
                var strErrMsg       = string.Empty;                         // 오류 메세지
                #endregion

                #region Input 파라메터       
                dicInputParam.Add("P_CENTER_CD",            strCenterCD);
                dicInputParam.Add("P_FAIL_NO",              iFailNo);
                #endregion

                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("100") == false)
                        {
                            this.BaseClass.MsgInfo(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                            isRtnValue = false;
                        }
                    }
                    else
                    {
                        this.BaseClass.MsgInfo("ERR_SAVE"); //CMPT_SAVE : 저장 중 오류가 발생했습니다.
                        isRtnValue = false;
                    }
                }

                return isRtnValue;
            }
            catch { throw; }
        }

        #region ▩ 이벤트
        #region > 등록 버튼 클릭 이벤트
        /// <summary>
        /// 등록 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRegist_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.EchkErrReg();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        /// <summary>
        /// 삭제버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isRtnValue = true;

                var liSelectedRowData = this.EmsFailList.Where(p => p.IsSelected).ToList();
                if (liSelectedRowData.Count() == 0)
                {
                    this.BaseClass.MsgError("ERR_NOT_SELECT");
                    return;
                }

                foreach (var item in liSelectedRowData)
                {
                    if (item.FAIL_STAT_CD.Equals("CONF"))
                    {
                        // 확정된 항목은 삭제하실 수 없습니다.
                        this.BaseClass.MsgError("ERR_NOT_DEL_CONF");
                        return;
                    }
                }

                // ASK_DEL - 삭제하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_DEL");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    da.BeginTransaction();

                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        foreach (var item in liSelectedRowData)
                        {
                            isRtnValue = this.DeleteSP_EMS_FAIL_DELETE(da, item);

                            if (isRtnValue == false)
                            {
                                break;
                            }

                            if (isRtnValue == true)
                            {
                                // 저장된 경우
                                da.CommitTransaction();

                                this.BaseClass.MsgInfo("CMPT_SAVE"); //CMPT_SAVE : 저장되었습니다.
                                                                     //this.GetSP_CNTR_LIST_INQ();

                                ////저장된 데이터 포함하여 데이터 조회
                                //DataSet dsRtnValue = this.GetSP_CNTR_LIST_INQ();

                                //if (dsRtnValue == null) { return; }

                                //if (dsRtnValue.Tables[0].Rows.Count > 0)
                                //{
                                //    dsRtnValue = this.ModifyTableData(dsRtnValue);
                                //}

                                //this.CenterMgntList = new ObservableCollection<CenterMgnt>();
                                //this.CenterMgntList.ToObservableCollection(dsRtnValue.Tables[0]);

                                //this.gridMaster.ItemsSource = this.CenterMgntList;

                                //// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                                //this.SetResultText();

                                this.SearchEmsFailList();
                            }
                            else
                            {
                                // 오류 발생하여 저장 실패한 경우
                                da.RollbackTransaction();
                            }
                        }
                    }
                    catch
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        this.BaseClass.MsgInfo("ERR_SAVE"); //CMPT_SAVE : 저장 중 오류가 발생했습니다.
                        throw;
                    }
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
                this.BaseClass.MsgQuestion("ASK_DOWNLOAD");
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


        private void GridMain_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view = (sender as GridControl).View as TableView;
                var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hi.InRowCell)
                {
                    var strFailNo       = ((EmsFail)this.gridMain.SelectedItem).FAIL_NO;
                    var strStatus       = ((EmsFail)this.gridMain.SelectedItem).FAIL_STATUS;

                    using (E3002_01P frm = new E3002_01P(strFailNo, strStatus))
                    {
                        frm.ShowDialog();

                        if (frm.IsSaved == true)
                        {
                            this.SearchEmsFailList();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

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
