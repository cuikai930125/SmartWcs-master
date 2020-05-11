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
    /// E3001_05P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E3001_05P : Window, IDisposable
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
        public E3001_05P()
        {
            InitializeComponent();
        }

        public E3001_05P(EmsChkRst target, int check, string name, DateTime date, string state)
        {
            try
            {
                InitializeComponent();

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                STATE = state;

                CurrentRst = target;
                CurrentChkID = check;
                CurrentChkNm = name;
                CurrentChkDt = new DateTime(date.Year, date.Month, date.Day);


                gridMainView.ShowingEditor += (s, e) =>
                {
                    EmsChkPlanInfo curr = gridMain.SelectedItem as EmsChkPlanInfo;

                    // 확정시 수정 불가
                    //
                    if ("F" == state)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        if (0 == curr.ORG_INST_QTY)
                        {
                            if (gridMain.CurrentColumn.FieldName == "NEW_INST_QTY")
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
                            e.Cancel = gridMain.CurrentColumn.FieldName == "PART_ID" ||
                                       gridMain.CurrentColumn.FieldName == "PART_NM" ||
                                       gridMain.CurrentColumn.FieldName == "ORG_INST_DT" ||
                                       gridMain.CurrentColumn.FieldName == "ORG_INST_QTY" ||
                                       gridMain.CurrentColumn.FieldName == "NEW_INST_QTY";
                        }
                    }
                };

                btnFormClose.Click += btnFormClose_Click;

                this.Loaded += ECHK001_05P_Loaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 속성
        private EmsChkRst CurrentRst { get; set; }
        private int CurrentChkID { get; set; }
        private string CurrentChkNm { get; set; }
        private DateTime CurrentChkDt { get; set; }

        /// <summary>
        /// 점검 List
        /// </summary>
        public ObservableCollection<EmsChkPlanInfo> ChkPlanInfoList { get; private set; }

        /// <summary>
        /// 첨부 화일
        /// </summary>
        public ObservableCollection<EmsAppdFile> ChkFileList { get; private set; }
        string STATE { get; set; }
        #endregion

        #region ▩ 함수
        #region 조회
        /// <summary>
        /// 조회
        /// </summary>
        void SearchEqpPartReferInfo()
        {
            tbxPlanNm.Text = CurrentChkNm;
            tbxPlanNm.Tag = CurrentChkID;

            tbxEqpNm.Text = CurrentRst.EQP_NM;
            tbxEqpNm.Tag = CurrentRst.EQP_ID;

            tbxEqpMnfact.Text = CurrentRst.EQP_MNFACT;

            try
            {
                var _USER_ID = this.BaseClass.UserID;

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD", this.BaseClass.CenterCD },
                    {"P_CHK_ID", CurrentChkID},
                    {"P_EQP_ID", CurrentRst.EQP_ID},
                    {"P_USER_ID", _USER_ID}
                };

                var strOutParam = new[] { "P_EMS_CHK_PLAN_INFO", "P_EMS_APPD_FILE" };
                string callProc = "PK_EMS_ECHK001_05P.SP_EMS_EQP_PART_REPR_INFO_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );


                    ChkPlanInfoList = new ObservableCollection<EmsChkPlanInfo>();

                    ChkFileList = new ObservableCollection<EmsAppdFile>();
                    //FileList = new Dictionary<FileToByteArray, EmsAppdFile>();

                    if (outData.Tables["P_EMS_CHK_PLAN_INFO"].Rows.Count > 0)
                    {
                        ChkPlanInfoList.ToObservableCollection(outData.Tables["P_EMS_CHK_PLAN_INFO"]);

                        foreach (EmsChkPlanInfo itm in ChkPlanInfoList)
                        {
                            itm.QtyErrorAction = QtyErrorAction;
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

                    gridMain.ItemsSource = ChkPlanInfoList;
                    //gridMainView.BestFitColumns();

                    gridFile.ItemsSource = ChkFileList;
                    gridFileView.AutoWidth = true;
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }

        /// <summary>
        /// 수량 수정시 Check 사항
        /// </summary>
        void QtyErrorAction()
        {
            // "교체수량/폐기수량 합은"
            string strErr1 = this.BaseClass.GetResourceValue("EMS_QTY_ERR1");
            // "당초설치수량보다 작거나 같아야합니다."
            string strErr2 = this.BaseClass.GetResourceValue("EMS_QTY_ERR2"); 

            BaseClass.MsgInfo(strErr1 + "\r\n" + strErr2, BaseEnumClass.CodeMessage.MESSAGE);
        }
        #endregion 조회

        #region  부품등록
        /// <summary>
        /// 부품등록
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PartAdd_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (ECOM001_05P frmPartRef = new ECOM001_05P())
                {
                    frmPartRef.ShowDialog();

                    if (null != frmPartRef.CurrPartRef)
                    {
                        EmsChkPlanInfo _item = new EmsChkPlanInfo()
                        {
                            EQP_ID = CurrentRst.EQP_ID,
                            PART_ID = frmPartRef.CurrPartRef.PART_ID,
                            PART_NM = frmPartRef.CurrPartRef.PART_NM,
                            PART_SERIAL_NO = 0,
                            ORG_INST_DT = DateTime.Now,
                            QtyErrorAction = QtyErrorAction,
                            IsNew = true
                        };

                        ChkPlanInfoList.Add(_item);

                        gridMain.Focus();
                        gridMain.CurrentColumn = gridMain.Columns.First();
                        gridMain.View.FocusedRowHandle = ChkPlanInfoList.Count - 1;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion 부품등록

        #region 부품삭제
        /// <summary>
        /// 부품삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PartDel_Click(object sender, MouseButtonEventArgs e)
        {
            var _delItems = ChkPlanInfoList.Where(f => f.IsChecked).ToList();

            if (0 < _delItems.Count)
            {
                foreach (var item in _delItems)
                {
                    if (0 < item.ORG_INST_QTY)
                    {
                        // "당초 설치수량이 있으면 삭제할 수 없습니다."
                        string strNoDel = this.BaseClass.GetResourceValue("EMS_QTY_NODEL");
                        BaseClass.MsgInfo(strNoDel, BaseEnumClass.CodeMessage.MESSAGE);
                        return;
                    }
                }

                //var _USER_ID = this.BaseInfo.user_id.ToString();
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
                                    { "P_CENTER_CD", this.BaseClass.CenterCD },
                                    { "P_CHK_ID", CurrentChkID },
                                    { "P_EQP_ID", item.EQP_ID },
                                    { "P_PART_SERIAL_NO", item.PART_SERIAL_NO }
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_ECHK001_05P.SP_EMS_EQP_RST_PART_DELETE";

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
                                    BaseClass.MsgInfo("INPUT_TYPE_ERR");
                                    break;
                                }
                            }
                        }

                        if (_SUCCESS_CODE == "100")
                        {
                            da.CommitTransaction();

                            foreach (var item in _delItems)
                            {
                                ChkPlanInfoList.Remove(item);
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
        #endregion 부품삭제

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
                        APPD_FILE_GROUP_CD = "BC_" + CurrentChkID.ToString() + "_" + CurrentRst.EQP_ID,
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

        #region File Delete
        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileDelete_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DelAttachList();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 삭제 함수
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
                                    { "P_CENTER_CD", this.BaseClass.CenterCD},
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
                                    BaseClass.MsgInfo("INPUT_TYPE_ERR");
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
        #endregion File Delete

        #region 저장
        /// <summary>
        /// 저장 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            if (EqpPartReferInfoSave())
            {
                BaseClass.MsgInfo("CMPT_SAVE_DATA");
                this.Close();
            }
        }

        /// <summary>
        /// 저장 함수
        /// </summary>
        /// <returns></returns>
        bool EqpPartReferInfoSave()
        {
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
                            // "전송 파일이 존재하지 않습니다."
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

                            // "전송 파일 구성에 실패하였습니다."
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

                                // "파일 전송에 실패하였습니다."
                                this.BaseClass.MsgError("EMS_FILETRANS_FAIL");
                                break;
                            }

                            System.IO.File.Delete(cpy);
                        }
                        catch (Exception)
                        {
                            _SUCCESS_CODE = "0";
                            da.RollbackTransaction();

                            // "파일 전송에 실패하였습니다."
                            this.BaseClass.MsgError("EMS_FILETRANS_FAIL");
                            break;
                        }

                        var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD", this.BaseClass.CenterCD },
                                { "P_APPD_FILE_GROUP_CD", item.APPD_FILE_GROUP_CD },    // File Group
                                { "P_APPD_FILE_GUID", item.APPD_FILE_GUID },            // File GUID
                                { "P_APPD_FILE_NM", aName },                            // File Name
                                { "P_APPD_FILE_DEV_CD", item.APPD_FILE_DEV_CD }         // File 구분
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
                            this.BaseClass.MsgError("INPUT_TYPE_ERR");
                            break;
                        }
                    }

                    if (_SUCCESS_CODE == "100")
                    {
                        foreach (var item in ChkPlanInfoList)
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
                                { "P_CENTER_CD", this.BaseClass.CenterCD },
                                { "P_CHK_ID", CurrentChkID },                       // 점검 번호
                                { "P_CHK_DT", CurrentChkDt },                       // 점검 일자
                                { "P_EQP_ID", item.EQP_ID },                        // 설비 ID
                                { "P_PART_ID", item.PART_ID },                      // 부품 ID
                                { "P_PART_SERIAL_NO", item.PART_SERIAL_NO },        // 부품 SERIAL
                                { "P_ORG_INST_DT", new DateTime(item.ORG_INST_DT.Year, item.ORG_INST_DT.Month, item.ORG_INST_DT.Day) }, // 최초 설치일
                                { "P_EXCHG_QTY", item.EXCHG_QTY },                  // 교체수량
                                { "P_NEW_INST_QTY", item.NEW_INST_QTY },            // 설치수량
                                { "P_FALL_QTY", item.FALL_QTY },                    // 폐기수량
                                { "P_WORK_NOTE", (null == item.WORK_NOTE) ? "" : item.WORK_NOTE},   // 작업내용
                                { "P_USER_ID", _USER_ID }
                            };

                            var strOutParam = new[] { "P_RESULT" };
                            string callProc = "PK_EMS_ECHK001_05P.SP_EMS_EQP_PART_REPR_INFO_SAVE";

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
                                this.BaseClass.MsgError("INPUT_TYPE_ERR");
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
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }

                return false;
            }
        }
        #endregion 저장
        #endregion

        #region ▩ 이벤트
        /// <summary>
        /// 화면 오픈시 선행 작업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ECHK001_05P_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ECHK001_05P_Loaded;


            SearchEqpPartReferInfo();

            // 확정시 수정 불가
            //
            if ("F" == STATE)
            {
                btnSave.IsEnabled = false;

                btnRowDeleteHigh.IsEnabled = false;
                btnRowAddHigh.IsEnabled = false;

                btnRowDeleteLow.IsEnabled = false;
                btnRowAddLow.IsEnabled = false;
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

        /// <summary>
        /// 선택된 장보 복사후 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            EmsChkPlanInfo target = gridMain.SelectedItem as EmsChkPlanInfo;

            if (null == target) { return; }

            EmsChkPlanInfo cpy = new EmsChkPlanInfo()
            {
                EQP_ID = target.EQP_ID,
                PART_ID = target.PART_ID,
                PART_NM = target.PART_NM,
                PART_SERIAL_NO = 0,
                ORG_INST_DT = new DateTime(target.ORG_INST_DT.Year, target.ORG_INST_DT.Month, target.ORG_INST_DT.Day),
                IsNew = true
            };

            int idx = ChkPlanInfoList.IndexOf(target);

            if (idx == ChkPlanInfoList.Count - 1)
            {
                ChkPlanInfoList.Add(cpy);
            }
            else
            {
                ChkPlanInfoList.Insert(idx, cpy);
            }
        }

        #region 첨부화일 다운로드
        /// <summary>
        /// 첨부화일 다운로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridFileView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) { return; }

            if (gridFile.IsGroupRowHandle(rowHandle)) { return; } // A group row has been double clicked

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
            catch (Exception err)
            {
                this.BaseClass.Error(err);
                // "파일 전송에 실패하였습니다."
                this.BaseClass.MsgError("EMS_FILETRANS_FAIL");
            }
        }


        #endregion
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
        // ~E3001_05P()
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
