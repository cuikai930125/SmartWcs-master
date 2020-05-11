using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using Microsoft.Win32;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using SMART.WCS.UI.EMS.DataMembers.E3001;
using SMART.WCS.UI.EMS.Views.COM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SMART.WCS.UI.EMS.Views.CHECK_MGMT
{
    /// <summary>
    /// E3001_04P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E3001_04P : Window, IDisposable
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

        bool alreadySave = false;
        #endregion

        #region ▩ 생성자
        public E3001_04P()
        {
            InitializeComponent();
        }

        public E3001_04P(int _iTarget = -1, string _strState = "")
        {
            try
            {
                InitializeComponent();

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;
                
                CurrentChkID = _iTarget;

                STATE = _strState;

                // CHOO
                // 2020-03-15 오류 때문에 주석처리
                // 공통코드 정의한 후에 주석해제
                //GetComDev();

                this.BaseClass.BindingCommonComboBox(this.cboChkDptDev, "CHK_CLS_CD", null, false);

                this.BaseClass.ComboBoxSelectedKeyValue(this.cboChkDptDev);

                if (this.BaseClass.ComboBoxItemCount(this.cboChkDptDev) > 0)
                {
                    this.tbxDev.Text = this.BaseClass.ComboBoxSelectedDisplayValue(this.cboChkDptDev);
                }
                
                

                gridMainView.ShowingEditor += (s, e) =>
                {
                    // 확정시 수정 불가
                    //
                    if ("F" == STATE)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        e.Cancel = gridMain.CurrentColumn.FieldName == "EQP_ID" ||
                                   gridMain.CurrentColumn.FieldName == "EQP_NM" ||
                                   gridMain.CurrentColumn.FieldName == "PBS_NM" ||
                                   gridMain.CurrentColumn.FieldName == "CHK_TRGT_YN";
                    }
                };

                btnFormClose.Click += btnFormClose_Click;

                this.Loaded += ECHK001_04P_Loaded;

                //this.cboChkDptDev.PreviewMouseLeftButtonUp += (s, e) =>
                //{
                    
                //};
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

        /// <summary>
        /// 진행상태
        /// </summary>
        public ObservableCollection<EmsCommCode> StateList { get; private set; }

        /// <summary>
        /// 설비 (참조)
        /// </summary>
        ObservableCollection<EmsPbsList> PbsList { get; set; }

        /// <summary>
        /// 점검 실적 List
        /// </summary>
        public ObservableCollection<EmsChkRst> ChkRstList { get; private set; }

        /// <summary>
        /// 첨부화일
        /// </summary>
        public ObservableCollection<EmsAppdFile> ChkFileList { get; private set; }

        int CurrentChkID { get; set; }
        private EmsChkRstInfo CurreentRst { get; set; }
        string STATE { get; set; }

        public bool IsSaved = false;
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
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 공통코드 조회

        #region 점검 실적 정보 조회
        /// <summary>
        /// 점검 실적 정보
        /// </summary>
        void SearchEmsChkRst()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_CHK_ID", CurrentChkID}
                };

                var strOutParam = new[] { "P_EMS_CHK_RST_INFO", "P_EMS_PBS_LIST", "P_EMS_CHK_RST_LIST", "P_EMS_APPD_FILE" };
                string callProc = "PK_EMS_ECHK001_04P.SP_EMS_EQP_RST_INFO_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    PbsList = new ObservableCollection<EmsPbsList>();

                    var rsts = new ObservableCollection<EmsChkRstInfo>();

                    ChkRstList = new ObservableCollection<EmsChkRst>();
                    ChkFileList = new ObservableCollection<EmsAppdFile>();
                    //FileList = new Dictionary<FileToByteArray, EmsAppdFile>();

                    if (outData.Tables["P_EMS_PBS_LIST"].Rows.Count > 0)
                    {
                        PbsList.ToObservableCollection(outData.Tables["P_EMS_PBS_LIST"]);
                    }

                    if (outData.Tables["P_EMS_CHK_RST_INFO"].Rows.Count > 0)
                    {
                        rsts.ToObservableCollection(outData.Tables["P_EMS_CHK_RST_INFO"]);

                        CurreentRst = rsts[0];

                        //var dev = ChkDptDevList.Where(D => D.COM_DETAIL_CD == CurreentRst.CHK_DEV_CD).SingleOrDefault();

                        //if (null != dev)
                        //{
                        //    //tbxDev.Text = dev.COM_DETAIL_NM;
                        //}

                        tbxPlanNo.Text = CurreentRst.CHK_ID.ToString();
                        tbxPlanNm.Text = CurreentRst.CHK_PLAN_NM;
                        tbxPlanNm.Tag = CurreentRst.CHK_ID;
                        tbxPlanDt.Text = (null == CurreentRst.CHK_PLAN_DT_N) ? "" : CurreentRst.CHK_PLAN_DT.ToString("yyyy-MM-dd");

                        tbxManager.Text = CurreentRst.CHK_MNGR_NM;
                        tbxManager.Tag = CurreentRst.CHK_MNGR_ID;

                        tbxInfoRst.Text = CurreentRst.CHK_RST;
                        tbxState.Text = CurreentRst.CHK_STAT_CD;


                        if (outData.Tables["P_EMS_CHK_RST_LIST"].Rows.Count > 0)
                        {
                            ChkRstList.ToObservableCollection(outData.Tables["P_EMS_CHK_RST_LIST"]);

                            foreach (EmsChkRst rst in ChkRstList)
                            {
                                var pbs = PbsList.Where(P => P.PBS_ID == rst.PBS_ID).SingleOrDefault();

                                rst.PBS_NM = (null == pbs) ? "" : pbs.PBS_NM;
                            }
                        }

                        if (outData.Tables["P_EMS_APPD_FILE"].Rows.Count > 0)
                        {
                            ChkFileList.ToObservableCollection(outData.Tables["P_EMS_APPD_FILE"]);
                        }
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    gridMain.ItemsSource = ChkRstList;
                    //gridMainView.BestFitColumns();

                    gridFile.ItemsSource = ChkFileList;
                    //gridFileView.AutoWidth = true;

                    checkYes.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }

        /// <summary>
        /// 점검 대상만 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonYes_Checked(object sender, RoutedEventArgs e)
        {
            foreach (EmsChkRst item in ChkRstList)
            {
                item.IsHide = ("Y" != item.CHK_TRGT_YN);
            }

            gridMain.RefreshData();
        }

        /// <summary>
        /// 전체 표시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (EmsChkRst item in ChkRstList)
            {
                item.IsHide = false;
            }

            gridMain.RefreshData();
        }
        #endregion 점검 실적 정보 조회

        #region 설비 검색
        /// <summary>
        /// 설비 검색
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EqpSearch_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (ECOM001_01P frmEqpSearch = new ECOM001_01P())
                {
                    frmEqpSearch.ShowDialog();

                    if (!string.IsNullOrEmpty(frmEqpSearch.CurrEqpId))
                    {
                        EmsChkRst _item = new EmsChkRst()
                        {
                            EQP_ID = frmEqpSearch.CurrEqpId,
                            EQP_NM = frmEqpSearch.CurrEqpName,
                            PBS_ID = frmEqpSearch.CurrPbsId,
                            PBS_NM = frmEqpSearch.CurrPbsName,
                            CHK_TRGT_YN = "Y",
                            IsNew = true
                        };

                        ChkRstList.Add(_item);

                        gridMain.Focus();
                        gridMain.CurrentColumn = gridMain.Columns.First();
                        gridMain.View.FocusedRowHandle = ChkRstList.Count - 1;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion 설비 검색

        #region File Append
        /// <summary>
        /// File Append
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileAdd_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == true)
                {
                    EmsAppdFile aF = new EmsAppdFile()
                    {
                        APPD_FILE_NM = openFileDialog.FileName,
                        APPD_FILE_DEV_CD = "D",
                        APPD_FILE_GROUP_CD = "BB_" + CurrentChkID.ToString(),
                        APPD_FILE_GUID = Guid.NewGuid().ToString(),
                        IsNew = true
                    };

                    ChkFileList.Add(aF);

                    gridFile.Focus();
                    gridFile.CurrentColumn = gridFile.Columns.First();
                    gridFile.View.FocusedRowHandle = ChkFileList.Count - 1;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion File Append

        #region 담당자 검색
        /// <summary>
        /// 담당자
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkMngrSearch_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (ECOM001_07P frmEqpMngrSearch = new ECOM001_07P())
                {
                    frmEqpMngrSearch.ShowDialog();

                    if (null != frmEqpMngrSearch.CurrMngr)
                    {
                        tbxManager.Text = frmEqpMngrSearch.CurrMngr.MNGR_NM;
                        tbxManager.Tag = frmEqpMngrSearch.CurrMngr.EQP_MNGR_ID;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion 담당자 검색

        #region 저장
        /// <summary>
        /// 저장 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            if (SaveRst())
            {
                this.BaseClass.MsgInfo("CMPT_SAVE");

                IsSaved = true;
                this.Close();
            }
        }

        /// <summary>
        /// 저장 함수
        /// </summary>
        /// <returns></returns>
        bool SaveRst()
        {
            if (null == tbxManager.Tag || string.IsNullOrEmpty(tbxManager.Tag.ToString()))
            {
                // INFO_SEL_MUST - 담당자를 선택하세요.
                this.BaseClass.MsgInfo("INFO_SEL_MUST");
                return false;
            }

            using (BaseDataAccess da = new BaseDataAccess())
            {
                try
                {
                    string _SUCCESS_CODE = "100";

                    var _USER_ID = this.BaseClass.UserID;

                    da.BeginTransaction();

                    foreach (var item in ChkFileList)
                    {
                        if (!item.IsNew)
                        {
                            continue;
                        }

                        if (!System.IO.File.Exists(item.APPD_FILE_NM))
                        {
                            _SUCCESS_CODE = "0";
                            da.RollbackTransaction();
                            // ERR_NOT_EXISTS_TRANS_FILE - 전송파일이 존재하지 않습니다.
                            this.BaseClass.MsgError("ERR_NOT_EXISTS_TRANS_FILE");
                            break;
                        }

                        string aName = System.IO.Path.GetFileName(item.APPD_FILE_NM);
                        string aExt = System.IO.Path.GetExtension(aName);

                        string cpy = item.APPD_FILE_NM.Replace(aName, item.APPD_FILE_GUID + aExt);

                        try
                        {
                            System.IO.File.Copy(item.APPD_FILE_NM, cpy);
                        }
                        catch (Exception)
                        {
                            _SUCCESS_CODE = "0";
                            da.RollbackTransaction();
                            // ERR_FAIL_TRANS_FILE_PROC - 전송 파일 구성이 실패하였습니다.
                            this.BaseClass.MsgError("ERR_FAIL_TRANS_FILE_PROC");
                            break;
                        }

                        try
                        {
                            WebClient myWebClient = new WebClient();

                            byte[] responseArray = myWebClient.UploadFile(EmsSession.Instance.FileUploadUrl,
                                                                          "POST",
                                                                          cpy);

                            string responseUpload = System.Text.Encoding.ASCII.GetString(responseArray);

                            myWebClient.Dispose();

                            if (!responseUpload.Contains("Complete."))
                            {
                                _SUCCESS_CODE = "0";
                                da.RollbackTransaction();
                                // ERR_FAIL_FILE_TRANS - 파일 전송이 실패하였습니다.
                                this.BaseClass.MsgError("ERR_FAIL_FILE_TRANS");
                                break;
                            }

                            System.IO.File.Delete(cpy);
                        }
                        catch (Exception)
                        {
                            _SUCCESS_CODE = "0";
                            da.RollbackTransaction();

                            // ERR_FAIL_FILE_TRANS - 파일 전송이 실패하였습니다.
                            this.BaseClass.MsgError("ERR_FAIL_FILE_TRANS");
                            break;
                        }

                        var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                { "P_APPD_FILE_GROUP_CD", item.APPD_FILE_GROUP_CD },            // File Group
                                { "P_APPD_FILE_GUID", item.APPD_FILE_GUID },                    // File GUID
                                { "P_APPD_FILE_NM", aName },                                    // File Name
                                { "P_APPD_FILE_DEV_CD", item.APPD_FILE_DEV_CD }                 // File 구분
                            };

                        var strOutParam = new[] { "P_RESULT" };
                        string callProc = "PK_EMS_COM001.SP_EMS_COM_APPD_FILE_SAVE";

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
                        var strCheckDate = dtCheck.DateTime.ToString("yyyyMMdd");

                        var param = new Dictionary<string, object>
                        {
                            { "P_CENTER_CD", this.BaseClass.CenterCD },
                            { "P_CHK_ID", CurrentChkID },                       // 점검 번호
                            { "P_CHK_DT", strCheckDate },// new DateTime(dtCheck.DateTime.Year, dtCheck.DateTime.Month, dtCheck.DateTime.Day) },  // 일시
                            { "P_CHK_MNGR_ID", tbxManager.Tag.ToString() },     // 담당자
                            { "P_CHK_RST", tbxInfoRst.Text.Trim() },            // 결과
                            { "P_USER_ID", _USER_ID }
                        };

                        var strOutParam = new[] { "P_RESULT" };
                        string callProc = "PK_EMS_ECHK001_04P.SP_EMS_EQP_RST_INFO_SAVE";

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
                        foreach (var item in ChkRstList)
                        {
                            if (!item.IsNew && !item.IsUpdate)
                            {
                                continue;
                            }

                            var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                { "P_CHK_ID", CurrentChkID },                       // 점검 번호
                                { "P_EQP_ID", item.EQP_ID },                        // 설비 ID
                                { "P_CHK_TRG_YN", item.CHK_TRGT_YN },               // 점검 대상 여부
                                { "P_CHK_YN", item.CHK_YN },                        // 점검 여부
                                { "P_CHK_RST_NOTE", item.CHK_RST_NOTE },            // 점검 결과
                                { "P_USER_ID", _USER_ID }
                            };

                            var strOutParam = new[] { "P_RESULT" };
                            string callProc = "PK_EMS_ECHK001_04P.SP_EMS_EQP_RST_LIST_SAVE";

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

        #region 점검내용 삭제
        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkDelete_Click(object sender, MouseButtonEventArgs e)
        {
            DelChkList();
        }

        /// <summary>
        /// 삭제 함수
        /// </summary>
        private void DelChkList()
        {
            var _delItems = ChkRstList.Where(f => f.IsChecked).ToList();

            if (0 < _delItems.Count)
            {
                var _USER_ID = this.BaseClass.UserID;
                string _SUCCESS_CODE = "100";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        da.BeginTransaction();

                        foreach (var item in _delItems)
                        {
                            if (!item.IsNew)
                            {
                                var param = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                    { "P_CHK_ID", CurrentChkID },
                                    { "P_EQP_ID", item.EQP_ID }
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_ECHK001_04P.SP_EMS_EQP_RST_DELETE";

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
                        }

                        if (_SUCCESS_CODE == "100")
                        {
                            da.CommitTransaction();

                            foreach (var item in _delItems)
                            {
                                ChkRstList.Remove(item);
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
        #endregion 점검내용 삭제

        #region 첨부화일 삭제
        /// <summary>
        /// 화일 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileDelete_Click(object sender, MouseButtonEventArgs e)
        {
            DelAttachList();
        }

        /// <summary>
        /// 화일 삭제 함수
        /// </summary>
        private void DelAttachList()
        {
            var _delItems = ChkFileList.Where(f => f.IsChecked).ToList();

            if (0 < _delItems.Count)
            {
                var _USER_ID = this.BaseClass.UserID;
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
                                    { "P_CENTER_CD", this.BaseClass.CenterCD },
                                    { "P_APPD_FILE_GUID", item.APPD_FILE_GUID },
                                    { "P_USER_ID", _USER_ID }
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_COM001.SP_EMS_COM_APPD_FILE_DELETE";

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
                        }

                        if (_SUCCESS_CODE == "100")
                        {
                            da.CommitTransaction();

                            foreach (var item in _delItems)
                            {
                                //if (item.IsNew != true)
                                ChkFileList.Remove(item);
                            }

                            gridFile.RefreshData();
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
        }
        #endregion 첨부화일 삭제

        #region 점검 실적 Popup
        /// <summary>
        /// Grid Data 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridMainView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) { return; }

            if (gridMain.IsGroupRowHandle(rowHandle)) { return; } // A group row has been double clicked

            EmsChkRst target = gridMain.SelectedItem as EmsChkRst;

            if (null != target)
            {
                // CHOO
                //ECHK001_05P frmRstReg = new ECHK001_05P(target, CurrentChkID, CurreentRst.CHK_PLAN_NM, dtCheck.DateTime, STATE);
                //frmRstReg.ShowDialog();
            }
        }
        #endregion 점검 실적 Popup
        #endregion

        #region ▩ 이벤트
        /// <summary>
        /// 화면 오픈시 선행 작업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ECHK001_04P_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Loaded -= ECHK001_04P_Loaded;

                SearchEmsChkRst();

                // 확정시 수정 불가
                //
                if ("F" == STATE)
                {
                    btnConfirmHigh.IsEnabled = false;
                    btnSaveHigh.IsEnabled = false;

                    btnRowDeleteLow.IsEnabled = false;
                    btnRowAddLow.IsEnabled = false;

                    dtCheck.IsEnabled = false;
                    mngrSearch.IsEnabled = false;
                    tbxInfoRst.IsReadOnly = true;   
                }

                //var state = StateList.Where(S => S.COM_DETAIL_CD == STATE).SingleOrDefault();
                //tbxState.Text = (null == state) ? "" : state.COM_DETAIL_NM;

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

        #region region btnExcelDownload_PreviewMouseLeftButtonUp - 엑셀다운로드 버튼 클릭
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
                this.BaseClass.MsgQuestion("ASK_DOWN_EXCEL");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_CHK_ID", CurrentChkID}
                };

                var strOutParam = new[] { "P_EMS_CHK_LIST" };
                string callProc = "PK_EMS_ECHK001_04P.SP_EMS_EQP_CHECK_LIST_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    var list = new ObservableCollection<DataMembers.E3001.EmsChkList>();

                    if (outData.Tables["P_EMS_CHK_LIST"].Rows.Count > 0)
                    {
                        list.ToObservableCollection(outData.Tables["P_EMS_CHK_LIST"]);

                        gridHide.ItemsSource = list;
                    }
                }

                await Task.Delay(100);

                List<TableView> tableView = new List<TableView>();
                tableView.Add(this.gridHideView);
                this.BaseClass.GetExcelDownload(tableView);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region 첨부 화일 다운로드
        /// <summary>
        /// 첨부 화일 다운로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridFileView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) return;

            if (gridFile.IsGroupRowHandle(rowHandle)) return; // A group row has been double clicked

            EmsAppdFile target = gridFile.SelectedItem as EmsAppdFile;

            string aName = System.IO.Path.GetFileName(target.APPD_FILE_NM);
            string aExt = System.IO.Path.GetExtension(aName);

            string aDown = target.APPD_FILE_GUID + aExt;

            string path = System.IO.Path.GetDirectoryName(
                              Environment.GetFolderPath(
                                  Environment.SpecialFolder.Personal));

            path = System.IO.Path.Combine(path, "Downloads\\");

            try
            {
                WebClient myWebClient = new WebClient();

                myWebClient.DownloadFile(EmsSession.Instance.FileDownloadUrl + aDown, path + aName);

                myWebClient.Dispose();


                Process p = new Process();
                ProcessStartInfo pi = new ProcessStartInfo();
                pi.UseShellExecute = true;
                pi.FileName = path + aName;
                p.StartInfo = pi;

                p.Start();
            }
            catch (Exception)
            {
                // ERR_FAIL_FILE_TRANS - 파일전송이 실패하였습니다.
                this.BaseClass.MsgError("ERR_FAIL_FILE_TRANS");
            }
        }
        #endregion 첨부 화일 다운로드      

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
        // ~E3001_04P()
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

        private void btnConfirm_Click(object sender, MouseButtonEventArgs e)
        {
            if (null == CurreentRst || 1 > CurreentRst.CHK_ID)
            {
                return;
            }

            // CHOO
            this.BaseClass.MsgQuestion("점검 내용을 확정합니다.", BaseEnumClass.CodeMessage.MESSAGE);

            if (this.BaseClass.BUTTON_CONFIRM_YN != true)
            {
                return;
            }

            if (!alreadySave)
            {
                if (!SaveRst())
                {
                    return;
                }
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
                        { "P_CHK_ID",  CurreentRst.CHK_ID},
                        { "P_CHK_STAT_CD", "F" },
                        {"P_USER_ID", _USER_ID}
                    };

                    var strOutParam = new[] { "P_RESULT" };
                    string callProc = "PK_EMS_ECHK001_04P.SP_EMS_EQP_RST_FIX";

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

                    if (_SUCCESS_CODE == "100")
                    {
                        da.CommitTransaction();

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_SAVE_DATA"));

                        // CMPT_CONFIRM - 확정되었습니다.
                        this.BaseClass.MsgInfo("CMPT_CONFIRM");

                        IsSaved = true;
                        this.Close();
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
