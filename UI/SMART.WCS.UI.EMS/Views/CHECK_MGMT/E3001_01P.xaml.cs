using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using SMART.WCS.UI.EMS.DataMembers.E3001;
using SMART.WCS.UI.EMS.Views.COM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.EMS.Views.CHECK_MGMT
{
    /// <summary>
    /// E3001_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E3001_01P : Window, IDisposable
    {
        #region ▩ Delegate
        public delegate void SearchResult(string _strCode, string _strName);
        public event SearchResult CustomerSearchResult;
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

        bool IsNew = false;
        #endregion

        #region ▩ 생성자
        public E3001_01P()
        {
            InitializeComponent();
        }

        public E3001_01P(int _iTarget = -1)
        {
            try
            {
                InitializeComponent();

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                CurrentChkID = _iTarget;

                // CHOO
                // 2020-03-15 오류 때문에 주석처리
                // 공통코드 정의한 후에 주석해제
                //GetComDev();

                this.BaseClass.BindingCommonComboBox(this.cboChkDptDev, "CHK_CLS_CD", null, false);

                btnFormClose.Click += btnFormClose_Click;

                this.Loaded += ECHK001_01P_Loaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 속성
        /// <summary>
        /// 점검 구분
        /// </summary>
        public ObservableCollection<EmsCommCode> ChkDptDevList { get; private set; }

        int CurrentChkID { get; set; }
        public EmsChkPlan CurrentPlan { get; private set; }

        /// <summary>
        /// 점검 위치 List
        /// </summary>
        public ObservableCollection<EmsChkPbs> ChkPbsList { get; private set; }

        public bool IsSaved = false;

        /// <summary>
        /// 점검 위치 List (선택된 항목)
        /// </summary>
        private List<EmsChkPbs> CheckedPbsList { get; set; }
        #endregion

        #region ▩ 함수
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
                            ,   paramDpt                        // Input 파라메터
                            ,   strOutParamDpt                  // Output 파라메터
                    );

                    // 리턴 테이블 체크 
                    if (outDataDpt.Tables.Count > 1)
                    {
                        ChkDptDevList = new ObservableCollection<EmsCommCode>();
                        ChkDptDevList.ToObservableCollection(outDataDpt.Tables[0]);

                        cboChkDptDev.ItemsSource = ChkDptDevList;
                        cboChkDptDev.SelectedItem = ChkDptDevList[0];

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }
                }

                chkDate.DateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 공통코드 조회

        #region 점검 계획 조회
        /// <summary>
        /// 점검 계획
        /// </summary>
        void SearchEmsChkPlan()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD", this.BaseClass.CenterCD },
                    {"P_CHK_ID", CurrentChkID}
                };

                var strOutParam = new[] { "P_EMS_CHK_PLAN_INFO", "P_EMS_CHK_PLAN_PBS" };
                string callProc = "PK_EMS_ECHK001_01P.SP_EMS_CHK_PLAN_INFO_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    var plans = new ObservableCollection<EmsChkPlan>();
                    ChkPbsList = new ObservableCollection<EmsChkPbs>();
                    CheckedPbsList = null;

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        plans.ToObservableCollection(outData.Tables[0]);

                        CurrentPlan = plans[0];

                        this.BaseClass.ComboBoxItemIndex(this.cboChkDptDev, CurrentPlan.CHK_DEV_CD);


                        //cboChkDptDev.SelectedItem = ChkDptDevList.Where(D => D.COM_DETAIL_CD == CurrentPlan.CHK_DEV_CD).SingleOrDefault();
                        tbxChkName.Text = CurrentPlan.CHK_PLAN_NM;
                        if (null != CurrentPlan.CHK_PLAN_DT_N)
                        {
                            chkDate.DateTime = CurrentPlan.CHK_PLAN_DT;
                        }

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    if (outData.Tables[1].Rows.Count > 0)
                    {
                        ChkPbsList.ToObservableCollection(outData.Tables[1]);
                        this.ChkPbsList.Where(p => p.IS_CHECKED == 1).ForEach(p => p.IsSelected = true);
                        CheckedPbsList = ChkPbsList.Where(P => P.IS_CHECKED == 1).ToList();
                    }

                    gridPbs.ItemsSource = ChkPbsList;
                    gridPbsView.ExpandAllNodes();
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 점검 계획 조회

        #region 저장
        /// <summary>
        /// 저장 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SaveChkPlan();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 저장 함수
        /// </summary>
        void SaveChkPlan()
        {
            using (BaseDataAccess da = new BaseDataAccess())
            {
                try
                {   
                    string _SUCCESS_CODE = "100";

                    var _USER_ID = this.BaseClass.UserID;

                    da.BeginTransaction();

                    //DateTime dt         = new DateTime(chkDate.DateTime.Year, chkDate.DateTime.Month, chkDate.DateTime.Day);
                    var strChkDate      = this.BaseClass.GetCalendarValue(this.chkDate);
                    var strChkDptDev    = this.BaseClass.ComboBoxSelectedKeyValue(this.cboChkDptDev);
                    

                    var param = new Dictionary<string, object>
                    {
                        { "P_CENTER_CD",        this.BaseClass.CenterCD },
                        { "P_CHK_ID", (-1 == CurrentChkID) ? 0 : CurrentChkID },    // 일련번호
                        { "P_CHK_PLAN_NM",      tbxChkName.Text.Trim() },           // 점검 명
                        { "P_CHK_DEV_CD",       strChkDptDev },                     // 점검구분
                        { "P_CHK_PLAN_DT",      strChkDate },                       // 점검읿자
                        { "P_USER_ID",          _USER_ID }
                    };

                    var strOutParam = new[] { "P_RESULT" };
                    string callProc = "PK_EMS_ECHK001_01P.SP_EMS_CHK_PLAN_INFO_SAVE";

                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        if (outData.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
                        {
                            da.RollbackTransaction();
                            BaseClass.MsgError(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        }
                    }
                    else
                    {
                        _SUCCESS_CODE = "0";
                        da.RollbackTransaction();
                        this.BaseClass.MsgError("ERR_INPUT_TYPE");
                    }

                    if ("0" != _SUCCESS_CODE)
                    {
                        int chk_id = int.Parse(outData.Tables[0].Rows[0]["CHK_ID"].ToString());

                        var chkd = ChkPbsList.Where(P => P.IsSelected).ToList();

                        if (null == CheckedPbsList || 0 == CheckedPbsList.Count)
                        {
                            // 기 체크된 항목 없음
                            //
                            foreach (var item in chkd)
                            {
                                var paramP = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                    { "P_CHK_ID", chk_id },
                                    { "P_PBS_ID", item.PBS_ID },
                                    { "P_ISCHECK", 1 },
                                    { "P_USER_ID", _USER_ID }
                                };

                                var strOutParamP = new[] { "P_RESULT" };
                                string callProcP = "PK_EMS_ECHK001_01P.SP_EMS_CHK_PLAN_PBS_SAVE";

                                var outDataP = da.GetSpDataSet(
                                        callProcP                      // 호출 프로시저
                                    ,   paramP                        // Input 파라메터
                                    ,   strOutParamP                  // Output 파라메터
                                );

                                if (outDataP.Tables[0].Rows.Count > 0)
                                {
                                    if (outDataP.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
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
                        else
                        {
                            // 새로 체크된(추가) 항목 추룰
                            //
                            var add = chkd.Except(CheckedPbsList).ToList();

                            // 체크 해제된(삭제) 항목 추룰
                            //
                            var removed = CheckedPbsList.Except(chkd).ToList();


                            // 추가 항목 처리
                            //
                            foreach (var item in add)
                            {
                                var paramP = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                    { "P_CHK_ID", chk_id },
                                    { "P_PBS_ID", item.PBS_ID },
                                    { "P_ISCHECK", 1 },
                                    { "P_USER_ID", _USER_ID }
                                };

                                var strOutParamP = new[] { "P_RESULT" };
                                string callProcP = "PK_EMS_ECHK001_01P.SP_EMS_CHK_PLAN_PBS_SAVE";

                                var outDataP = da.GetSpDataSet(
                                        callProcP                      // 호출 프로시저
                                    ,   paramP                        // Input 파라메터
                                    ,   strOutParamP                  // Output 파라메터
                                );

                                if (outDataP.Tables[0].Rows.Count > 0)
                                {
                                    if (outDataP.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
                                    {
                                        _SUCCESS_CODE = "0";
                                        da.RollbackTransaction();
                                        BaseClass.MsgError(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
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

                            // 삭제 항목 처리
                            //
                            foreach (var item in removed)
                            {
                                var paramP = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                    { "P_CHK_ID", chk_id },
                                    { "P_PBS_ID", item.PBS_ID },
                                    { "P_ISCHECK", 0 },
                                    { "P_USER_ID", _USER_ID }
                                };

                                var strOutParamP = new[] { "P_RESULT" };
                                string callProcP = "PK_EMS_ECHK001_01P.SP_EMS_CHK_PLAN_PBS_SAVE";

                                var outDataP = da.GetSpDataSet(
                                        callProcP                      // 호출 프로시저
                                    ,   paramP                        // Input 파라메터
                                    ,   strOutParamP                  // Output 파라메터
                                );

                                if (outDataP.Tables[0].Rows.Count > 0)
                                {
                                    if (outDataP.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
                                    {
                                        _SUCCESS_CODE = "0";
                                        da.RollbackTransaction();
                                        BaseClass.MsgError(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
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

                            this.BaseClass.MsgInfo("CMPT_SAVE");
                            IsSaved = true;
                            this.Close();
                        }
                    }

                }
                catch (Exception ex)
                {
                    da.RollbackTransaction();
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }
        }
        #endregion 저장

        #region Select All / Off
        /// <summary>
        /// Select All
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (EmsChkPbs tarhet in ChkPbsList)
            {
                tarhet.IsChecked = true;
            }
        }

        /// <summary>
        /// Select Off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (EmsChkPbs tarhet in ChkPbsList)
            {
                tarhet.IsChecked = false;
            }
        }
        #endregion Select All / Off

        #region CheckBox Action
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

                var target = ChkPbsList.Where(f => f.PBS_ID == o.Value.ToString()).SingleOrDefault();

                if (null != target)
                {
                    target.IsChecked = true;
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
                EditGridCellData o = (sender as CheckBox).Tag as EditGridCellData;

                var target = ChkPbsList.Where(f => f.PBS_ID == o.Value.ToString()).SingleOrDefault();

                if (null != target)
                {
                    target.IsChecked = false;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// Check 항목 화면 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_Editor_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                EditGridCellData o = (sender as CheckBox).Tag as EditGridCellData;

                var target = ChkPbsList.Where(f => f.PBS_ID == o.Value.ToString()).SingleOrDefault();

                if (null != target && target.IsChecked)
                {
                    (sender as CheckBox).IsChecked = true;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion CheckBox Action
        #endregion

        #region ▩ 이벤트
        /// <summary>
        /// 화면 오픈시 선행 작업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ECHK001_01P_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Loaded -= ECHK001_01P_Loaded;

                SearchEmsChkPlan();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// Close 이미지 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFormClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void SelectorColumnBehavior_SelectorCellCheked(object sender, Modules.Ctrl.SelectorCellChekedEventArgs e)
        {
            try
            {
                var selectedRowData = e.RowData.Row as EmsChkPbs;

                if (selectedRowData.IsSelected == true)
                {
                    selectedRowData.IS_CHECKED = 1;
                }
                else
                {
                    selectedRowData.IS_CHECKED = 0;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion


        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~E3001_01P()
        // {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion

        
    }
}
