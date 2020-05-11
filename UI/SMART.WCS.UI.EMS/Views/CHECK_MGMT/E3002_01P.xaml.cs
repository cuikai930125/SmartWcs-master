using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using Microsoft.Win32;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using SMART.WCS.UI.EMS.DataMembers.ECHK002;
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
    /// E3002_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E3002_01P : Window, IDisposable
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

        public bool IsSaved = false;
        #endregion

        #region ▩ 생성자
        public E3002_01P()
        {
            InitializeComponent();
        }

        public E3002_01P(int failNo, string state = "")
        {
            InitializeComponent();

            // 화면 전체권한 여부
            g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

            FailNo = failNo;

            STATE = state;

            // CHOO
            // 2020-03-15 오류 때문에 주석처리
            // 공통코드 정의한 후에 주석해제
            //GetComDev();

            btnFormClose.Click += btnFormClose_Click;

            this.BaseClass.BindingCommonComboBox(this.cboChkRepDev, "FAIL_TYPE_CD", null, false);

            this.BaseClass.BindingCommonComboBox(this.cboChkFallDev, "FAIL_REASON", null, false);

            gridFileView.ShowingEditor += (s, e) =>
            {
                // 확정시 수정 금지
                //
                if ("CONF" == STATE)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = gridFile.CurrentColumn.FieldName != "IsChecked";
                }
            };

            gridDetailView.ShowingEditor += (s, e) =>
            {
                EmsFailPart curr = gridDetail.SelectedItem as EmsFailPart;

                // 확정시 수정 금지
                //
                if ("CONF" == STATE)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (0 == curr.ORG_INST_QTY)
                    {
                        if (gridDetail.CurrentColumn.FieldName == "NEW_INST_QTY")
                        {
                            e.Cancel = false;
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        e.Cancel = gridDetail.CurrentColumn.FieldName == "PART_ID" ||
                                   gridDetail.CurrentColumn.FieldName == "PART_NM" ||
                                   gridDetail.CurrentColumn.FieldName == "ORG_INST_DT" ||
                                   gridDetail.CurrentColumn.FieldName == "ORG_INST_QTY" ||
                                   gridDetail.CurrentColumn.FieldName == "NEW_INST_QTY";
                    }
                    /*
                    if ("NEW_INST_QTY" == gridDetail.CurrentColumn.FieldName)
                    {
                        e.Cancel = 0 != curr.ORG_INST_QTY;
                    }
                    else if ("FALL_QTY" == gridDetail.CurrentColumn.FieldName)
                    {
                        e.Cancel = 0 == curr.ORG_INST_QTY;
                    }
                    else
                    {
                        e.Cancel = gridDetail.CurrentColumn.FieldName == "PART_ID" ||
                                   gridDetail.CurrentColumn.FieldName == "PART_NM" ||
                                   gridDetail.CurrentColumn.FieldName == "ORG_INST_DT" ||
                                   gridDetail.CurrentColumn.FieldName == "ORG_INST_QTY";
                    }
                    */
                }
            };

            this.Loaded += ECHK002_01P_Loaded;
        }

        public E3002_01P(string eqpId, string eqpNm)
        {
            InitializeComponent();
            this.Name = this.GetType().Name;

            // 화면 전체권한 여부
            g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

            FailNo = -1;

            // CHOO
            // 2020-03-15 오류 때문에 주석처리
            // 공통코드 정의한 후에 주석해제
            //GetComDev();

            btnFormClose.Click += btnFormClose_Click;

            gridFileView.ShowingEditor += (s, e) =>
            {
                // 확정시 수정 금지
                //
                if ("CONF" == STATE)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = gridFile.CurrentColumn.FieldName != "IsChecked";
                }
            };

            gridDetailView.ShowingEditor += (s, e) =>
            {
                EmsFailPart curr = gridDetail.SelectedItem as EmsFailPart;

                // 확정시 수정 금지
                //
                if ("CONF" == STATE)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (0 == curr.ORG_INST_QTY)
                    {
                        if (gridDetail.CurrentColumn.FieldName == "NEW_INST_QTY")
                        {
                            e.Cancel = false;
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        e.Cancel = gridDetail.CurrentColumn.FieldName == "PART_ID" ||
                                   gridDetail.CurrentColumn.FieldName == "PART_NM" ||
                                   gridDetail.CurrentColumn.FieldName == "ORG_INST_DT" ||
                                   gridDetail.CurrentColumn.FieldName == "ORG_INST_QTY" ||
                                   gridDetail.CurrentColumn.FieldName == "NEW_INST_QTY";
                    }
                    /*
                    if ("NEW_INST_QTY" == gridDetail.CurrentColumn.FieldName)
                    {
                        e.Cancel = 0 != curr.ORG_INST_QTY;
                    }
                    else if ("FALL_QTY" == gridDetail.CurrentColumn.FieldName)
                    {
                        e.Cancel = 0 == curr.ORG_INST_QTY;
                    }
                    else
                    {
                        e.Cancel = gridDetail.CurrentColumn.FieldName == "PART_ID" ||
                                   gridDetail.CurrentColumn.FieldName == "PART_NM" ||
                                   gridDetail.CurrentColumn.FieldName == "ORG_INST_DT" ||
                                   gridDetail.CurrentColumn.FieldName == "ORG_INST_QTY";
                    }
                    */
                }
            };


            tbxEqpNm.Text = eqpNm;
            tbxEqpId.Text = eqpId;

            this.Loaded += ECHK002_01P_Loaded;
        }
        #endregion

        #region ▩ 속성
        int FailNo { get; set; }
        EmsFail FailInfo { get; set; }

        /// <summary>
        /// 처리 구분
        /// </summary>
        public ObservableCollection<EmsCommCode> ChkRepDevList { get; private set; }

        /// <summary>
        /// 장애 사유
        /// </summary>
        public ObservableCollection<EmsCommCode> ChkFallDevList { get; private set; }

        /// <summary>
        /// 상세 List
        /// </summary>
        public ObservableCollection<EmsFailPart> FailPartList { get; private set; }

        /// <summary>
        /// 첨부화일
        /// </summary>
        public ObservableCollection<EmsAppdFile> ChkFileList { get; private set; }
        //private Dictionary<FileToByteArray, EmsAppdFile> FileList { get; set; }

        string STATE { get; set; }
        #endregion

        #region ▩ 함수
        #region 조회
        /// <summary>
        /// 조회
        /// </summary>
        void SearchEmsFailList()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD},
                    {"P_FAIL_NO", FailNo}
                };

                var strOutParam = new[] { "P_EMS_FAIL_INFO", "P_EMS_APD_FILE_LIST" };
                string callProc = "PK_EMS_ECHK002_01P.SP_EMS_FAIL_INFO_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    if (null != ChkFileList)
                    {
                        ChkFileList.Clear();
                        ChkFileList = null;
                    }

                    ChkFileList = new ObservableCollection<EmsAppdFile>();
                    //FileList = new Dictionary<FileToByteArray, EmsAppdFile>();

                    var fails = new ObservableCollection<EmsFail>();

                    if (outData.Tables["P_EMS_FAIL_INFO"].Rows.Count > 0)
                    {
                        fails.ToObservableCollection(outData.Tables["P_EMS_FAIL_INFO"]);

                        FailInfo = fails[0];

                        tbxFailNo.Text = FailInfo.FAIL_NO.ToString();

                        tbxEqpId.Text = FailInfo.EQP_ID;
                        tbxEqpNm.Text = FailInfo.EQP_NM;

                        CurrEqpId = FailInfo.EQP_ID;

                        fllDt.DateTime = (null == FailInfo.FALL_DT_N) ? DateTime.Now : FailInfo.FALL_DT;
                        msrDt.DateTime = (null == FailInfo.MSR_REG_DT_N) ? DateTime.Now : FailInfo.MSR_REG_DT;

                        tbxMngr.Text = FailInfo.MNGR_NM;
                        tbxMngr.Tag = FailInfo.FAIL_MNGR_ID;

                        //cboChkRepDev.SelectedItem = ChkRepDevList.Where(R => R.COM_DETAIL_CD == FailInfo.FAIL_DEV_CD).SingleOrDefault();
                        var strChkRepDev    = this.BaseClass.ComboBoxSelectedKeyValue(this.cboChkRepDev);

                        tbxFailNote.Text = FailInfo.FAIL_NOTE;

                        //cboChkFallDev.SelectedItem = ChkFallDevList.Where(R => R.COM_DETAIL_CD == FailInfo.FAIL_STATUS).SingleOrDefault();
                        var strChkFallDev   = this.BaseClass.ComboBoxSelectedKeyValue(this.cboChkFallDev);
                        tbxFailReason.Text = FailInfo.FAIL_REASON;

                        tbxMsrNote.Text = FailInfo.MSR_NOTE;

                        if (outData.Tables["P_EMS_APD_FILE_LIST"].Rows.Count > 0)
                        {
                            ChkFileList.ToObservableCollection(outData.Tables["P_EMS_APD_FILE_LIST"]);
                        }

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    gridFile.ItemsSource = ChkFileList;
                    //gridFileView.AutoWidth = true;
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }

            SearchEmsFailPartList();
        }

        /// <summary>
        /// 장애 조치 상세
        /// </summary>
        /// <param name="isNew"></param>
        void SearchEmsFailPartList(bool isNew = false)
        {
            if (null != FailPartList)
            {
                FailPartList.Clear();
                FailPartList = null;
            }

            FailPartList = new ObservableCollection<EmsFailPart>();

            btnRowDeleteLow.IsEnabled = true;
            btnRowAddLow.IsEnabled = true;

            try
            {
                var _USER_ID = this.BaseClass.UserID;

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD},
                    {"P_FAIL_NO", (isNew) ? 0: FailNo},
                    {"P_EQP_ID", tbxEqpId.Text.Trim()},
                    { "P_USER_ID", _USER_ID }
                };

                var strOutParam = new[] { "P_RESULT_FAIL_NO", "P_EMS_FAIL_PART_LIST" };
                string callProc = "PK_EMS_ECHK002_01P.SP_EMS_FAIL_PART_LIST_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    FailNo = int.Parse(outData.Tables[0].Rows[0][0].ToString());
                    FailInfo.FAIL_NO = FailNo;

                    tbxFailNo.Text = FailInfo.FAIL_NO.ToString();

                    if (outData.Tables[1].Rows.Count > 0)
                    {
                        FailPartList.ToObservableCollection(outData.Tables[1]);

                        foreach (EmsFailPart itm in FailPartList)
                        {
                            itm.QtyErrorAction = QtyErrorAction;
                        }

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    gridDetail.ItemsSource = FailPartList;

                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }

        /// <summary>
        /// 수량 수정시 Check 사항
        /// </summary>
        void QtyErrorAction()
        {
            string strErr1 = this.BaseClass.GetResourceValue("EMS_QTY_ERR1"); // "교체수량/폐기수량 합은"
            string strErr2 = this.BaseClass.GetResourceValue("EMS_QTY_ERR2"); // "당초설치수량보다 작거나 같아야합니다."

            BaseClass.MsgInfo(strErr1 + "\r\n" + strErr2, BaseEnumClass.CodeMessage.MESSAGE);
        }
        #endregion 조회

        #region 공통코드 조회
        void GetComDev()
        {
            try
            {
                var _USER_ID = this.BaseClass.UserID;

                var paramRep = new Dictionary<string, object>
                {
                    {"P_USER_ID", _USER_ID},
                    {"P_COM_HEAD_CD", "EMS_REP_DEV"}
                };

                var strOutParamRep = new[] { "P_COM_DETAIL_CD_LIST", "P_RESULT" };
                string callProcRep = "PK_COMM_CODE_MGT.SP_COMM_CODE_DETAIL_SEARCH";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outDataRep = da.GetSpDataSet(
                                callProcRep                     // 호출 프로시저
                            ,   paramRep                        // Input 파라메터
                            ,   strOutParamRep                  // Output 파라메터
                    );

                    // 리턴 테이블 체크 
                    if (outDataRep.Tables.Count > 1)
                    {
                        ChkRepDevList = new ObservableCollection<EmsCommCode>();
                        ChkRepDevList.ToObservableCollection(outDataRep.Tables[0]);

                        cboChkRepDev.ItemsSource = ChkRepDevList;
                        cboChkRepDev.SelectedItem = ChkRepDevList[0];

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }
                }


                var paramFal = new Dictionary<string, object>
                {
                    {"P_USER_ID", _USER_ID},
                    {"P_COM_HEAD_CD", "EMS_FALL_DEV"}
                };

                var strOutParamFal = new[] { "P_COM_DETAIL_CD_LIST", "P_RESULT" };
                string callProcFal = "PK_COMM_CODE_MGT.SP_COMM_CODE_DETAIL_SEARCH";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outDataFal = da.GetSpDataSet(
                                callProcFal                     // 호출 프로시저
                            ,   paramFal                        // Input 파라메터
                            ,   strOutParamFal                  // Output 파라메터
                    );

                    // 리턴 테이블 체크 
                    if (outDataFal.Tables.Count > 1)
                    {
                        ChkFallDevList = new ObservableCollection<EmsCommCode>();
                        ChkFallDevList.ToObservableCollection(outDataFal.Tables[0]);

                        cboChkFallDev.ItemsSource = ChkFallDevList;
                        cboChkFallDev.SelectedItem = ChkFallDevList[0];

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

        #region Popup
        string CurrEqpId = "";
        /// <summary>
        /// 설비 검색
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EqpSearch_Click(object sender, MouseButtonEventArgs e)
        {
            using (ECOM001_01P frmEqpSearch = new ECOM001_01P())
            {
                frmEqpSearch.ShowDialog();

                if (!string.IsNullOrEmpty(frmEqpSearch.CurrEqpId))
                {
                    if (-1 == FailNo)
                    {
                        tbxEqpId.Text = frmEqpSearch.CurrEqpId;
                        tbxEqpNm.Text = frmEqpSearch.CurrEqpName;

                        CurrEqpId = frmEqpSearch.CurrEqpId;

                        SearchEmsFailPartList(true);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(CurrEqpId))
                        {
                            tbxEqpId.Text = frmEqpSearch.CurrEqpId;
                            tbxEqpNm.Text = frmEqpSearch.CurrEqpName;

                            CurrEqpId = frmEqpSearch.CurrEqpId;
                        }
                        else
                        if (CurrEqpId != frmEqpSearch.CurrEqpId)
                        {
                            this.BaseClass.MsgInfo("상세 내용이 변경됩니다.", BaseEnumClass.CodeMessage.MESSAGE);

                            if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                            {
                                tbxEqpId.Text = frmEqpSearch.CurrEqpId;
                                tbxEqpNm.Text = frmEqpSearch.CurrEqpName;

                                CurrEqpId = frmEqpSearch.CurrEqpId;

                                SearchEmsFailPartList();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 담당자 검색
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MngrSearch_Click(object sender, MouseButtonEventArgs e)
        {
            ECOM001_07P frmEqpMngrSearch = new ECOM001_07P();
            frmEqpMngrSearch.ShowDialog();

            if (null != frmEqpMngrSearch.CurrMngr)
            {
                tbxMngr.Text = frmEqpMngrSearch.CurrMngr.MNGR_NM;
                tbxMngr.Tag = frmEqpMngrSearch.CurrMngr.EQP_MNGR_ID;
            }

            frmEqpMngrSearch = null;
        }

        /// <summary>
        /// 부품 참조
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPartRef_Click(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(tbxEqpId.Text.Trim()))
            {
                return;
            }

            using (ECOM001_05P frmPartRef = new ECOM001_05P())
            {
                frmPartRef.ShowDialog();

                if (null != frmPartRef.CurrPartRef)
                {
                    EmsFailPart _item = new EmsFailPart()
                    {
                        EQP_ID              = tbxEqpId.Text.Trim(),
                        PART_ID             = frmPartRef.CurrPartRef.PART_ID,
                        PART_NM             = frmPartRef.CurrPartRef.PART_NM,
                        PART_SERIAL_NO      = 0,
                        ORG_INST_DT         = DateTime.Now,
                        QtyErrorAction      = QtyErrorAction,
                        IsNew               = true,
                        IsSelected          = true
                    };

                    FailPartList.Add(_item);

                    gridDetail.Focus();
                    gridDetail.CurrentColumn = gridDetail.Columns.First();
                    gridDetail.View.FocusedRowHandle = FailPartList.Count - 1;
                }
            }
        }

        /// <summary>
        /// 부품 수리 현황
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridDetail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridDetailView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) { return; }

            if (gridDetail.IsGroupRowHandle(rowHandle)) { return; }// A group row has been double clicked

            EmsFailPart target = gridDetail.SelectedItem as EmsFailPart;

            if (null != target)
            {
                if (null != target)
                {
                    ECOM001_11P frmRepairHis = new ECOM001_11P(target.PART_ID, target.PART_NM);
                    frmRepairHis.ShowDialog();
                }
            }
        }
        #endregion Popup

        #region 파일 첨부 / 삭제
        /// <summary>
        /// 파일 첨부
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileAdd_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                EmsAppdFile aF = new EmsAppdFile()
                {
                    APPD_FILE_NM = openFileDialog.FileName,
                    APPD_FILE_DEV_CD = "D",
                    APPD_FILE_GROUP_CD = "CC_" + FailNo.ToString(),
                    APPD_FILE_GUID = Guid.NewGuid().ToString(),
                    IsNew = true
                };

                ChkFileList.Add(aF);

                gridFile.Focus();
                gridFile.CurrentColumn = gridFile.Columns.First();
                gridFile.View.FocusedRowHandle = ChkFileList.Count - 1;
            }
        }

        /// <summary>
        /// 파일 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileDelete_Click(object sender, MouseButtonEventArgs e)
        {
            DelAttachList();
        }

        /// <summary>
        /// 파일 삭제 함수
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
                                    { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                    { "P_APPD_FILE_GUID", item.APPD_FILE_GUID },
                                    { "P_USER_ID", _USER_ID }
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_COM001.SP_EMS_COM_APPD_FILE_DELETE";

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
        #endregion 파일 첨부 / 삭제        

        #region 저장
        /// <summary>
        /// 저장 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            SaveFailInfo();
        }

        /// <summary>
        /// 저장 함수
        /// </summary>
        /// <param name="from_set"></param>
        /// <returns></returns>
        bool SaveFailInfo(bool from_set = false)
        {
            if (string.IsNullOrEmpty(tbxEqpId.Text.Trim()))
            {
                // ID/명칭은 필수 항목입니다.
                this.BaseClass.MsgError("EMS_IDNM_MUST");
                return false;
            }


            if (null == tbxMngr.Tag || string.IsNullOrEmpty(tbxMngr.Tag.ToString()))
            {
                // 담당자를 선택하십시오.
                this.BaseClass.MsgError("EMS_MNGR_MUST");
                return false;
            }

            if (fllDt.DateTime > msrDt.DateTime)
            {
                // 조치 일시는 장애 일시 이후여야 합니다.
                this.BaseClass.MsgError("EMS_MNGR_MUST");
                return false;
            }

            if (null == cboChkRepDev.SelectedItem || null == cboChkFallDev.SelectedItem)
            {
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

                            // 전송 파일이 존재하지 않습니다.
                            this.BaseClass.MsgError("EMS_FILE_NOWHERE");
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
                            // 전송 파일 구성에 실패하였습니다.
                            this.BaseClass.MsgError("EMS_FILE_NOT");
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
                                // 파일 전송에 실패하였습니다.
                                this.BaseClass.MsgError("EMS_FILETRANS_FAIL");
                                break;
                            }

                            System.IO.File.Delete(cpy);
                        }
                        catch (Exception)
                        {
                            _SUCCESS_CODE = "0";
                            da.RollbackTransaction();
                            // 파일 전송에 실패하였습니다.
                            this.BaseClass.MsgError("EMS_FILETRANS_FAIL");
                            break;
                        }

                        var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                { "P_APPD_FILE_GROUP_CD", item.APPD_FILE_GROUP_CD },
                                { "P_APPD_FILE_GUID", item.APPD_FILE_GUID },
                                { "P_APPD_FILE_NM", aName },
                                { "P_APPD_FILE_DEV_CD", item.APPD_FILE_DEV_CD }
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
                        DateTime fdt = new DateTime(fllDt.DateTime.Year, fllDt.DateTime.Month, fllDt.DateTime.Day, fllDt.DateTime.Hour, fllDt.DateTime.Minute, 0);
                        DateTime dt = new DateTime(msrDt.DateTime.Year, msrDt.DateTime.Month, msrDt.DateTime.Day, msrDt.DateTime.Hour, msrDt.DateTime.Minute, 0);

                        var strFailType     = this.BaseClass.ComboBoxSelectedKeyValue(this.cboChkRepDev);
                        var strFailReason   = this.BaseClass.ComboBoxSelectedKeyValue(this.cboChkFallDev);

                        var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD", this.BaseClass.CenterCD },
                                { "P_FAIL_NO", (0 > FailNo) ? 0 : FailNo },         // 장애 번호
                                { "P_FALL_DT", fdt },                               // 장애 일시
                                { "P_EQP_ID", tbxEqpId.Text.Trim() },               // 설비 ID
                                { "P_MSR_REG_DT", dt },                             // 조치 일시
                                { "P_FAIL_NOTE", tbxFailNote.Text.Trim() },         // 장애 내용
                                { "P_FAIL_DEV_CD", strFailType },  // 장애 구분
                                { "P_FAIL_STATUS", strFailReason }, // 장애 사유(형태)
                                //{ "P_FAIL_DEV_CD", (cboChkRepDev.SelectedItem as EmsCommCode).COM_DETAIL_CD },  // 장애 구분
                                //{ "P_FAIL_STATUS", (cboChkFallDev.SelectedItem as EmsCommCode).COM_DETAIL_CD }, // 장애 사유(형태)
                                { "P_FAIL_REASON", tbxFailReason.Text.Trim() },     // 장애 사유(내용)
                                { "P_MSR_NOTE", tbxMsrNote.Text.Trim() },           // 조치 사항
                                { "P_FAIL_MNGR_ID", tbxMngr.Tag.ToString() },       // 담당자
                                { "P_USER_ID", _USER_ID }
                            };

                        var strOutParam = new[] { "P_FAIL_NO_RESULT", "P_RESULT" };
                        string callProc = "PK_EMS_ECHK002_01P.SP_EMS_FAIL_INFO_SAVE";

                        var outData = da.GetSpDataSet(
                                    callProc                      // 호출 프로시저
                                ,   param                        // Input 파라메터
                                ,   strOutParam                  // Output 파라메터
                        );

                        if (outData.Tables[0].Rows.Count > 0 && outData.Tables[1].Rows.Count > 0)
                        {
                            FailNo = int.Parse(outData.Tables[0].Rows[0][0].ToString());

                            if (outData.Tables[1].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
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
                        DateTime dt = new DateTime(msrDt.DateTime.Year, msrDt.DateTime.Month, msrDt.DateTime.Day);

                        foreach (var item in FailPartList)
                        {
                            if (!item.IsNew && !item.IsUpdate)
                            {
                                continue;
                            }

                            if (0 < item.ORG_INST_QTY && 0 < item.FALL_QTY && 0 < item.EXCHG_QTY)
                            {
                                if (item.FALL_QTY + item.EXCHG_QTY > item.ORG_INST_QTY)
                                {
                                    QtyErrorAction();
                                    da.RollbackTransaction();
                                    break;
                                }
                            }

                            var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                { "P_FAIL_NO", (0 > FailNo) ? 0 : FailNo },         // 장애 번호
                                { "P_PART_SERIAL_NO", item.PART_SERIAL_NO },        // 부품 SERIAL
                                { "P_EQP_ID", item.EQP_ID },                        // 설비 ID
                                { "P_PART_ID", item.PART_ID },                      // 부품 ID
                                { "P_MSR_REG_DT", dt },                             // 설치일자
                                { "P_EXCHG_QTY", item.EXCHG_QTY },                  // 교체수량
                                { "P_NEW_INST_QTY", item.NEW_INST_QTY },            // 설치수량
                                { "P_FALL_QTY", item.FALL_QTY },                    // 폐기수량
                                { "P_WORK_NOTE", (null == item.WORK_NOTE) ? "" : item.WORK_NOTE },
                                { "P_USER_ID", _USER_ID }
                            };

                            var strOutParam = new[] { "P_RESULT" };
                            string callProc = "PK_EMS_ECHK002_01P.SP_EMS_FAIL_PART_ITEM_SAVE";

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
                        this.BaseClass.MsgInfo("CMPT_SAVE");
                        IsSaved = true;

                        if (!from_set)
                        {
                            this.Close();
                        }

                        return true;
                        //SearchEmsFailList();
                    }

                }
                catch (Exception ex)
                {
                    da.RollbackTransaction();
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }

            return false;
        }
        #endregion 저장

        #region Title Menu
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
        #endregion Title Menu

        #region 조치 상세 삭제
        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPartRefDel_Click(object sender, MouseButtonEventArgs e)
        {
            DelFailPart();
        }

        /// <summary>
        /// 삭제 함수
        /// </summary>
        void DelFailPart()
        {
            //var _delItems = FailPartList.Where(f => f.IsChecked).ToList();

            if (this.FailPartList.Count() == 0) { return; }

            var liSelectedItems = this.FailPartList.Where(p => p.IsSelected);
            if (liSelectedItems.Count() == 0)
            {
                // Msg : 선택된 데이터가 없습니다.
                this.BaseClass.MsgInfo("ERR_NO_SELECT");
                return;
            }

            foreach (var item in liSelectedItems)
            {
                if (item.ORG_INST_QTY > 0)
                {
                    // Msg : 당초 설치수량이 있으면 삭제할 수 없습니다.
                    this.BaseClass.MsgError("EMS_QTY_NODEL");
                    return;
                }
            }

            // 추가된 Row 삭제
            var liFailPartList = this.FailPartList.Where(p => p.IsSelected == true && p.IsNew == true).ToList();
            if (liFailPartList.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }
            liFailPartList.ForEach(p => this.FailPartList.Remove(p));


            foreach (var item in liSelectedItems)
            {
                if (item.IsNew == false || item.IsUpdate == false)
                {
                    // Delete
                }
            }


            //if (0 < _delItems.Count)
            //{
            //    foreach (var item in _delItems)
            //    {
            //        if (0 < item.ORG_INST_QTY)
            //        {
            //            // 당초 설치수량이 있으면 삭제할 수 없습니다.
            //            this.BaseClass.MsgError("EMS_QTY_NODEL");
            //            return;
            //        }
            //    }

            //    //var _USER_ID = this.BaseInfo.user_id.ToString();
            //    string _SUCCESS_CODE = "100";

            //    using (BaseDataAccess da = new BaseDataAccess())
            //    {
            //        try
            //        {
            //            da.BeginTransaction();

            //            foreach (var item in _delItems)
            //            {
            //                if (!item.IsNew)
            //                {
            //                    var param = new Dictionary<string, object>
            //                    {
            //                        { "P_CENTER_CD",    this.BaseClass.CenterCD },
            //                        { "P_FAIL_NO", item.FAIL_NO },
            //                        { "P_PART_SERIAL_NO", item.PART_SERIAL_NO },
            //                    };

            //                    var strOutParam = new[] { "P_RESULT" };
            //                    string callProc = "PK_EMS_ECHK002_01P.SP_EMS_FAIL_PART_ITEM_DELETE";

            //                    var outData = da.GetSpDataSet(
            //                            callProc                      // 호출 프로시저
            //                        ,   param                        // Input 파라메터
            //                        ,   strOutParam                  // Output 파라메터
            //                    );

            //                    if (outData.Tables[0].Rows.Count > 0)
            //                    {
            //                        if (outData.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
            //                        {
            //                            _SUCCESS_CODE = "0";
            //                            da.RollbackTransaction();
            //                            BaseClass.MsgInfo(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
            //                            break;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        _SUCCESS_CODE = "0";
            //                        da.RollbackTransaction();
            //                        this.BaseClass.MsgError("ERR_INPUT_TYPE");
            //                        break;
            //                    }
            //                }
            //            }

            //            if (_SUCCESS_CODE == "100")
            //            {
            //                da.CommitTransaction();

            //                foreach (var item in _delItems)
            //                {
            //                    FailPartList.Remove(item);
            //                }

            //                gridDetail.RefreshData();
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            da.RollbackTransaction();
            //            this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            //        }
            //    }
            //}
        }
        #endregion 조치 상세 삭제
        #endregion

        #region ▩ 이벤트
        #region 첨부화일 다운로드
        private void gridFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridFileView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) { return; }

            if (gridFile.IsGroupRowHandle(rowHandle)) { return; }// A group row has been double clicked

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
                // 파일 전송에 실패하였습니다.
                this.BaseClass.MsgError("EMS_FILETRANS_FILE");
            }
        }
        #endregion 첨부화일 다운로드

        #region Confirm
        bool alreadySave = false;
        /// <summary>
        /// Confirm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, MouseButtonEventArgs e)
        {
            if (null == FailInfo || 1 > FailInfo.FAIL_NO)
            {
                return;
            }

            // 장애 내역을 확정하시겠습니까?
            this.BaseClass.MsgQuestion("ASK_EMS_ERR_CONF");   
            if (this.BaseClass.BUTTON_CONFIRM_YN != true)
            {
                return;
            }

            if (!alreadySave)
            {
                if (!SaveFailInfo(true))
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
                        { "P_CENTER_CD",    this.BaseClass.CenterCD },
                        { "P_FAIL_NO",  FailInfo.FAIL_NO},
                        { "P_FAIL_STAT_CD", "CONF" },
                        {"P_USER_ID", _USER_ID}
                    };

                    var strOutParam = new[] { "P_RESULT" };
                    string callProc = "PK_EMS_ECHK002_01P.SP_EMS_FAIL_FIX";

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

                        // 확정되었습니다.
                        this.BaseClass.MsgInfo("CMPT_CONF");

                        IsSaved = true;
                        this.Close();
                    }

                }
                catch (Exception ex)
                {
                    da.RollbackTransaction();
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }
        }
        #endregion Confirm

        /// <summary>
        /// 화면 오픈시 선행 작업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ECHK002_01P_Loaded(object sender, RoutedEventArgs e)
        {
            if (0 < FailNo)
            {
                SearchEmsFailList();
            }
            else
            {
                FailInfo = new EmsFail();

                ChkFileList = new ObservableCollection<EmsAppdFile>();
                //FileList = new Dictionary<FileToByteArray, EmsAppdFile>();                

                gridFile.ItemsSource = ChkFileList;
                gridFileView.AutoWidth = true;

                FailPartList = new ObservableCollection<EmsFailPart>();
                gridDetail.ItemsSource = FailPartList;

                btnRowDeleteLow.IsEnabled = true;
                btnRowAddLow.IsEnabled = true;

                if (!string.IsNullOrEmpty(tbxEqpId.Text))
                {
                    SearchEmsFailPartList(true);
                }
            }

            // 확정시 수정 금지
            //
            if ("CONF" == STATE)
            {
                btnConfirmHigh.IsEnabled = false;
                btnSaveHigh.IsEnabled = false;

                btnRowDeleteHigh.IsEnabled = false;
                btnRowAddHigh.IsEnabled = false;

                btnRowDeleteLow.IsEnabled = false;
                btnRowAddLow.IsEnabled = false;

                fllDt.IsEnabled = false;
                msrDt.IsEnabled = false;

                eqpNm.IsEnabled = false;
                mngrId.IsEnabled = false;

                cboChkRepDev.IsEnabled = false;
                cboChkFallDev.IsEnabled = false;

                tbxFailNote.IsReadOnly = true;
                tbxFailReason.IsReadOnly = true;
                tbxMsrNote.IsReadOnly = true;
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
        // ~E3002_01P()
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
