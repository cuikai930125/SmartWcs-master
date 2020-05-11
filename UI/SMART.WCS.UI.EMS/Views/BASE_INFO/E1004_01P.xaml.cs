using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using Microsoft.Win32;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using SMART.WCS.UI.EMS.DataMembers.E1004;
using SMART.WCS.UI.EMS.Views.COM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SMART.WCS.UI.EMS.Views.BASE_INFO
{
    /// <summary>
    /// E1004_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E1004_01P : Window, IDisposable
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
        public E1004_01P()
        {
            InitializeComponent();
        }

        public E1004_01P(string clsId, string clsNm, string eqpId)
        {
            try
            {
                InitializeComponent();

                TargetClsID = clsId;
                TargetClsNm = clsNm;
                TargetEqpID = eqpId;

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                if (!string.IsNullOrEmpty(eqpId))
                {
                    tbxEqpId.IsReadOnly = true;
                    eqpSearch.Visibility = Visibility.Collapsed;
                }
                else
                {
                    IsNew = true;
                }

                //EmsAppdFileDev();

                btnFormClose.Click += btnFormClose_Click;

                this.Loaded += EBSE004_01P_Loaded;
                this.Closing += EBSE004_01P_Closing;

                this.BaseClass.SetGridAllowEditing(this.gridChkListView, true);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 속성
        private string TargetEqpID { get; set; }

        private string TargetClsID { get; set; }
        private string TargetClsNm { get; set; }

        private EmsEqpInfo CurrEqpInfo { get; set; }

        /// <summary>
        /// 파일분류
        /// </summary>
        public ObservableCollection<EmsCommCode> EmsAppdFileDevList { get; private set; }

        /// <summary>
        /// 첨부파일
        /// </summary>
        private ObservableCollection<EmsAppdFile> CurrAppdFiles { get; set; }

        /// <summary>
        /// 체크리스트
        /// </summary>
        private ObservableCollection<EmsEqpChkList> CurrEqpChkList { get; set; }
        #endregion

        #region ▩ 함수
        #region 파일 구분
        private void EmsAppdFileDev()
        {
            try
            {
                var _USER_ID = this.BaseClass.UserID;

                var param = new Dictionary<string, object>
                {
                    {"P_USER_ID", _USER_ID},
                    {"P_COM_HEAD_CD", "EMS_APPD_FILE_DEV"}
                };

                var strOutParam = new[] { "P_COM_DETAIL_CD_LIST", "P_RESULT" };
                string callProc = "PK_COMM_CODE_MGT.SP_COMM_CODE_DETAIL_SEARCH";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        EmsAppdFileDevList = new ObservableCollection<EmsCommCode>();
                        EmsAppdFileDevList.ToObservableCollection(outData.Tables[0]);

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
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 파일 구분

        #region Eqp Image Attach
        //
        // https://www.akadia.com/services/dotnet_orablobs.html
        //

        MemoryStream _imageStream = null;
        private string _imagePath = "";
        private string _imageId = "";
        private int _imageLength;
        private byte[] _imageData;
        /// <summary>
        /// 이미지 지정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgContainer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";

            if (openFileDialog.ShowDialog() == true)
            {
                _imagePath = openFileDialog.FileName;

                // FileStream to get the Photo
                FileStream fs;

                // Get Image Data from the Filesystem if User has loaded a Photo
                fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                _imageLength = (int)fs.Length;

                // Create a byte array of file stream length
                _imageData = new byte[fs.Length];

                // Read block of bytes from stream into the byte array
                fs.Read(_imageData, 0, System.Convert.ToInt32(fs.Length));

                // Close the File Stream
                fs.Close();

                if (null != _imageStream)
                {
                    _imageStream.Close();
                    _imageStream = null;
                }

                if (!string.IsNullOrEmpty(_imageId))
                {
                    string aDown = _imageId + ".jpg";

                    string path = System.IO.Path.GetDirectoryName(
                                    Environment.GetFolderPath(
                                        Environment.SpecialFolder.Personal));

                    path = System.IO.Path.Combine(path, "Downloads\\");

                    if (System.IO.File.Exists(path + aDown))
                    {
                        System.IO.File.Delete(path + aDown);
                    }
                }

                _imageId = Guid.NewGuid().ToString();

                var imageSource = new BitmapImage();

                // Get the primitive byte data into in-memory data stream
                _imageStream = new MemoryStream(_imageData);
                imageSource.BeginInit();
                imageSource.StreamSource = _imageStream;
                imageSource.EndInit();

                // Assign the Source property of your image
                imgEqpImage.Source = imageSource as ImageSource;
            }
        }
        #endregion Eqp Image Attach

        #region File Append
        
        #endregion File Append

        #region 첨부화일 삭제
        

        /// <summary>
        /// 첨부화일 삭제 함수
        /// </summary>
        private void DelAttachList()
        {
            var _delItems = CurrAppdFiles.Where(f => f.IsSelected).ToList();

            if (0 < _delItems.Count)
            {
                var _USER_ID        = this.BaseClass.UserID;
                string _SUCCESS_CODE = "100";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        da.BeginTransaction();

                        foreach (var item in _delItems)
                        {
                            if (item.IsNew != true && "AA" + tbxEqpId.Text.Trim() == item.APPD_FILE_GROUP_CD)
                            {
                                var param = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD", this.BaseClass.CenterCD },
                                    { "P_APPD_FILE_GROUP_CD", item.APPD_FILE_GROUP_CD },
                                    { "P_APPD_FILE_GUID", item.APPD_FILE_GUID },
                                    { "P_USER_ID", _USER_ID }
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                //string callProc = "PK_EMS_COM001.SP_EMS_COM_APPD_FILE_DELETE";
                                string callProc = "PK_EMS_COM001.SP_EMS_COM_GROUP_FILE_DELETE";

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

                            this.BaseClass.MsgInfo("CMPT_DEL");

                            foreach (var item in _delItems)
                            {
                                //if (item.IsNew != true)
                                CurrAppdFiles.Remove(item);
                            }

                            gridAttrachList.RefreshData();
                            //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_DELETE"));
                        }
                        else
                        {
                            da.RollbackTransaction();
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
        #endregion 첨부화일 삭제

        #region > 데이터 관련
        #region Eqp Info Sel
        /// <summary>
        /// Eqp Info Sel
        /// </summary>
        private void EqpInfoSel()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_EQP_ID",        TargetEqpID}
                };

                var strOutParam = new[] { "P_EMS_EQP_INFO", "P_EMS_EQP_APPD_LIST", "P_EMS_EQP_CHK_LIST" };
                string callProc = "PK_EMS_EBSE004_01P.SP_EMS_EQP_INFO_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                    callProc                      // 호출 프로시저
                                , param                         // Input 파라메터
                                , strOutParam                   // Output 파라메터
                    );


                    CurrEqpChkList = new ObservableCollection<EmsEqpChkList>();
                    CurrAppdFiles = new ObservableCollection<EmsAppdFile>();
                    //FileList = new Dictionary<FileToByteArray, EmsAppdFile>();

                    if (0 < outData.Tables.Count)
                    {
                        for (int i = 0; i < outData.Tables.Count; ++i)
                        {
                            if ("P_EMS_EQP_INFO" == outData.Tables[i].TableName)
                            {
                                if (outData.Tables[i].Rows.Count > 0)
                                {
                                    var info = new ObservableCollection<EmsEqpInfo>();
                                    info.ToObservableCollection(outData.Tables[i]);

                                    CurrEqpInfo = info[0];
                                    CurrEqpInfo.IsUpdate = true;

                                    _imageId = (null == CurrEqpInfo.EQP_IMG_FILE_ID) ? "" : CurrEqpInfo.EQP_IMG_FILE_ID;

                                    if (!string.IsNullOrEmpty(_imageId))
                                    {
                                        string aDown = _imageId + ".jpg";

                                        string path = System.IO.Path.GetDirectoryName(
                                                        Environment.GetFolderPath(
                                                            Environment.SpecialFolder.Personal));

                                        path = System.IO.Path.Combine(path, "Downloads\\");

                                        try
                                        {
                                            WebClient myWebClient = new WebClient();

                                            myWebClient.DownloadFile(EmsSession.Instance.FileDownloadUrl + aDown, path + aDown);

                                            myWebClient.Dispose();

                                            if (null != _imageStream)
                                            {
                                                _imageStream.Close();
                                                _imageStream = null;
                                            }

                                            // FileStream to get the Photo
                                            FileStream fs;

                                            // Get Image Data from the Filesystem if User has loaded a Photo
                                            fs = new FileStream(path + aDown, FileMode.Open, FileAccess.Read);
                                            _imageLength = (int)fs.Length;

                                            // Create a byte array of file stream length
                                            _imageData = new byte[fs.Length];

                                            // Read block of bytes from stream into the byte array
                                            fs.Read(_imageData, 0, System.Convert.ToInt32(fs.Length));

                                            // Close the File Stream
                                            fs.Close();

                                            var imageSource = new BitmapImage();

                                            // Get the primitive byte data into in-memory data stream
                                            _imageStream = new MemoryStream(_imageData);
                                            imageSource.BeginInit();
                                            imageSource.StreamSource = _imageStream;
                                            imageSource.EndInit();

                                            // Assign the Source property of your image
                                            imgEqpImage.Source = imageSource as ImageSource;
                                        }
                                        catch
                                        {
                                            this.BaseClass.MsgError("ERR_FAIL_SEND_IMG");
                                        }
                                    }

                                    tbxEqpId.Text = CurrEqpInfo.EQP_ID;
                                    tbxEqpNm.Text = CurrEqpInfo.EQP_NM;

                                    tbxEqpMnfact.Text = CurrEqpInfo.EQP_MNFACT;
                                    tbxPbsStnd.Text = CurrEqpInfo.EQP_STND;
                                    tbxInstModel.Text = CurrEqpInfo.EQP_MODEL;

                                    tbxEqpPbsNm.Text = CurrEqpInfo.PBS_NM.Trim();
                                    tbxEqpPbsNm.Tag = CurrEqpInfo.PBS_ID;

                                    tbxEqpClsNm.Text = CurrEqpInfo.EQP_CLS_NM.Trim();
                                    tbxEqpClsNm.Tag = CurrEqpInfo.EQP_CLS_ID;

                                    rbEqpUseY.IsChecked = "Y" == CurrEqpInfo.USE_YN;
                                    rbEqpUseN.IsChecked = !rbEqpUseY.IsChecked.Value;

                                    tbxEqpMngrNm.Text = CurrEqpInfo.MNGR_NM;
                                    tbxEqpMngrNm.Tag = CurrEqpInfo.MNG_CHGR_ID;
                                    tbxEqpMngrTelNo.Text = CurrEqpInfo.MNGR_TEL_NO;
                                    tbxEqpVndrNm.Text = CurrEqpInfo.VNDR_NM;

                                    deInstDt.DateTime = CurrEqpInfo.INST_DT;
                                    deFinalChkDt.DateTime = CurrEqpInfo.FILENAL_CHK_DT;

                                    if (null != CurrEqpInfo.EQP_IMG)
                                    {
                                        _imageData = CurrEqpInfo.EQP_IMG.ToArray();

                                        var imageSource = new BitmapImage();

                                        // Get the primitive byte data into in-memory data stream
                                        _imageStream = new MemoryStream(CurrEqpInfo.EQP_IMG);
                                        imageSource.BeginInit();
                                        imageSource.StreamSource = _imageStream;
                                        imageSource.EndInit();

                                        // Assign the Source property of your image
                                        imgEqpImage.Source = imageSource as ImageSource;
                                    }

                                    info.Clear();
                                    info = null;
                                }
                                else
                                {
                                    CurrEqpInfo = new EmsEqpInfo();
                                    CurrEqpInfo.IsNew = true;
                                }
                            }

                            if ("P_EMS_EQP_APPD_LIST" == outData.Tables[i].TableName)
                            {
                                if (outData.Tables[i].Rows.Count > 0)
                                {
                                    CurrAppdFiles.ToObservableCollection(outData.Tables[i]);
                                }
                            }

                            if ("P_EMS_EQP_CHK_LIST" == outData.Tables[i].TableName)
                            {
                                if (outData.Tables[i].Rows.Count > 0)
                                {
                                    CurrEqpChkList.ToObservableCollection(outData.Tables[i]);

                                    if (IsNew)
                                    {
                                        for (int j = 0; j < CurrEqpChkList.Count; ++j)
                                        {
                                            CurrEqpChkList[j].CHK_SERIAL_NO = 0;
                                            CurrEqpChkList[j].IsNew = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        CurrEqpInfo = new EmsEqpInfo();
                        CurrEqpInfo.IsNew = true;
                    }

                    gridAttrachList.ItemsSource = null;
                    gridAttrachList.ItemsSource = CurrAppdFiles;

                    gridChkList.ItemsSource = null;
                    gridChkList.ItemsSource = CurrEqpChkList;
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion Eqp Info Sel        

        /// <summary>
        /// 저장 함수
        /// </summary>
        void SaveEmsEqp()
        {
            if (string.IsNullOrEmpty(tbxEqpId.Text.Trim()) || string.IsNullOrEmpty(tbxEqpNm.Text.Trim()))
            {
                // ERR_REQ_ID_NM - ID/명칭은 필수 항목입니다
                this.BaseClass.MsgError("ERR_REQ_ID_NM");
                return;
            }

            if (string.IsNullOrEmpty(tbxEqpPbsNm.Text.Trim()))
            {
                // ERR_REQ_EQP_LOC - 설비위치는 필수항목입니다.
                this.BaseClass.MsgError("ERR_REQ_EQP_LOC");
                return;
            }

            if (string.IsNullOrEmpty(tbxEqpClsNm.Text.Trim()))
            {
                // ERR_REQ_EQP_SPR - 설비 분류는 필수입력 입니다.
                this.BaseClass.MsgError("ERR_REQ_EQP_SPR");
                return;
            }

            if (!string.IsNullOrEmpty(_imagePath))
            {
                if (!System.IO.File.Exists(_imagePath))
                {
                    // ERR_NOT_EXISTS_IMG - 이미지 파일이 존재하지 않습니다.
                    this.BaseClass.MsgError("ERR_NOT_EXISTS_IMG");
                    return;
                }

                string aName = System.IO.Path.GetFileName(_imagePath);
                string aExt = ".jpg"; // System.IO.Path.GetExtension(aName);

                string cpy = _imagePath.Replace(aName, _imageId + aExt);

                try
                {
                    System.IO.File.Copy(_imagePath, cpy);
                }
                catch (Exception)
                {
                    // ERR_NOT_IMG_PROC - 이미지 파일 구성에 실패하였습니다.
                    this.BaseClass.MsgError("ERR_FAIL_IMG_PROC");
                    return;
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
                        // ERR_FAIL_IMG_TRANS - 이미지 전송이 실패하였습니다.
                        this.BaseClass.MsgError("ERR_FAIL_IMG_TRANS");
                        return;
                    }

                    System.IO.File.Delete(cpy);
                }
                catch (Exception)
                {
                    // ERR_FAIL_IMG_TRANS - 이미지 전송이 실패하였습니다.
                    this.BaseClass.MsgError("ERR_FAIL_IMG_TRANS");
                    return;
                }
            }

            string saveEqpId = tbxEqpId.Text.Trim();
            string saveEqpNm = tbxEqpNm.Text.Trim();

            IsNew = (TargetEqpID != saveEqpId);

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
                        { "P_EQP_ID", saveEqpId },                          // ID
                        { "P_EQP_NM", saveEqpNm },                          // 이름
                        { "P_EQP_CLS_ID", tbxEqpClsNm.Tag.ToString() },     // 분류
                        { "P_EQP_STND", tbxPbsStnd.Text.Trim() },           // 규격
                        { "P_EQP_MODEL", tbxInstModel.Text.ToString() },    // 모델
                        { "P_EQP_MNFACT", tbxEqpMnfact.Text.Trim() },       // 제조사
                        { "P_MNG_CHGR_ID", (null == tbxEqpMngrNm.Tag || string.IsNullOrEmpty(tbxEqpMngrNm.Text.Trim())) ? "" : tbxEqpMngrNm.Tag.ToString() },   // 관리자
                        { "P_USE_YN", (rbEqpUseY.IsChecked.Value) ? "Y" : "N" },
                        //{ "P_EQP_IMG", (null ==_imageData) ? new byte[0] : _imageData },
                        { "P_EQP_IMG_FILE_ID", string.IsNullOrEmpty(_imageId) ? "" : _imageId },    // 이미지
                        { "P_INST_DT", new DateTime(deInstDt.DateTime.Year, deInstDt.DateTime.Month, deInstDt.DateTime.Day) },  // 설치 일자
                        { "P_PBS_ID", tbxEqpPbsNm.Tag.ToString() },         // 설비 위치
                        { "P_EQP_ID_ORG", TargetEqpID },                    // Orig ID
                        { "P_USER_ID", _USER_ID }
                    };

                    var strOutParam = new[] { "P_RESULT" };
                    string callProc = (IsNew) ? "PK_EMS_EBSE004_01P.SP_EMS_EQP_INFO_INSERT" : "PK_EMS_EBSE004_01P.SP_EMS_EQP_INFO_SAVE";

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
                        foreach (var item in CurrAppdFiles)
                        {
                            string aName = "";
                            string aExt = "";

                            if (item.IsNew)
                            {
                                if (!System.IO.File.Exists(item.APPD_FILE_NM))
                                {
                                    _SUCCESS_CODE = "0";
                                    da.RollbackTransaction();

                                    // ERR_NOT_EXISTS_TRANS_FILE - 전송파일이 존재하지 않습니다.
                                    this.BaseClass.MsgError("ERR_NOT_EXISTS_TRANS_FILE");
                                    break;
                                }

                                aName = System.IO.Path.GetFileName(item.APPD_FILE_NM);
                                aExt = System.IO.Path.GetExtension(aName);

                                string cpy = item.APPD_FILE_NM.Replace(aName, item.APPD_FILE_GUID + aExt);

                                try
                                {
                                    System.IO.File.Copy(item.APPD_FILE_NM, cpy);
                                }
                                catch (Exception)
                                {
                                    _SUCCESS_CODE = "0";
                                    da.RollbackTransaction();
                                    // ERR_FAIL_TRANS_FILE_PROC - 전송파일 구성이 실패하였습니다.
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
                            }
                            else
                            {
                                aName = System.IO.Path.GetFileName(item.APPD_FILE_NM);
                            }

                            if (item.IsNew || ("AA" + saveEqpId) != item.APPD_FILE_GROUP_CD)
                            {
                                var paramF = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD", this.BaseClass.CenterCD },
                                    //{ "P_APPD_FILE_GROUP_CD", "EQP_" + saveEqpId },
                                    { "P_APPD_FILE_GROUP_CD", "AA" + saveEqpId },       // File Group
                                    { "P_APPD_FILE_GUID", item.APPD_FILE_GUID },        // File GUID
                                    { "P_APPD_FILE_NM", aName },                        // File Name
                                    { "P_APPD_FILE_DEV_CD", item.APPD_FILE_DEV_CD }     // File 구분
                                };

                                var strOutParamF = new[] { "P_RESULT" };
                                string callProcF = "PK_EMS_COM001.SP_EMS_COM_APPD_FILE_SAVE";

                                var outDataF = da.GetSpDataSet(
                                            callProcF                      // 호출 프로시저
                                        , paramF                        // Input 파라메터
                                        , strOutParamF                  // Output 파라메터
                                );

                                if (outDataF.Tables[0].Rows.Count > 0)
                                {
                                    if (outDataF.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
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
                            int idx = 1;
                            foreach (var item in CurrEqpChkList)
                            {
                                if (item.IsDelete == true)
                                {
                                    param = new Dictionary<string, object>
                                    {
                                        { "P_CENTER_CD", this.BaseClass.CenterCD.ToString() },
                                        { "P_EQP_ID", item.EQP_ID },
                                        { "P_CHK_SERIAL_NO", item.CHK_SERIAL_NO },
                                        { "P_USER_ID", _USER_ID }
                                    };

                                    strOutParam = new[] { "P_RESULT" };
                                    callProc = "PK_EMS_EBSE004_01P.SP_EMS_EQP_CHK_LIST_DELETE";

                                    outData = da.GetSpDataSet(      // 데이터베이스 연결 문자열
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
                                        BaseClass.MsgInfo("INPUT_TYPE_ERR");
                                        break;
                                    }
                                }
                                else
                                {
                                    item.CHK_SERIAL_NO = idx++;

                                    var paramL = new Dictionary<string, object>
                                    {
                                        { "P_CENTER_CD", this.BaseClass.CenterCD },
                                        { "P_EQP_ID", saveEqpId },
                                        { "P_CHK_SERIAL_NO", item.CHK_SERIAL_NO },
                                        { "P_CHK_ITEM", (null == item.CHK_ITEM) ? "" : item.CHK_ITEM},      // 점검 항목
                                        { "P_CHK_NOTE", (null == item.CHK_NOTE) ? "" : item.CHK_NOTE },     // 점검 방법
                                        { "P_BASE_VAL", (null == item.BASE_VAL) ? "" : item.BASE_VAL },     // 기준값
                                        { "P_UPPER_VAL", (null == item.UPPER_VAL) ? "" : item.UPPER_VAL},   // 상한값
                                        { "P_LOWER_VAL", (null == item.LOWER_VAL) ? "" :  item.LOWER_VAL},  // 하한값
                                        { "P_CHK_UNIT", (null == item.CHK_UNIT) ? "" : item.CHK_UNIT},      // 단위
                                        { "P_USER_ID", _USER_ID }
                                    };

                                    var strOutParamL = new[] { "P_RESULT" };
                                    string callProcL = "PK_EMS_EBSE004_01P.SP_EMS_EQP_CHK_LIST_SAVE";

                                    var outDataL = da.GetSpDataSet(
                                                callProcL                     // 호출 프로시저
                                            , paramL                        // Input 파라메터
                                            , strOutParamL                  // Output 파라메터
                                    );

                                    if (outDataL.Tables[0].Rows.Count > 0)
                                    {
                                        if (outDataL.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
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
                                //this.Close();
                            }
                        }
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
        #endregion

        private void DelChkList()
        {
            var _delItems = CurrEqpChkList.Where(f => f.IsChecked).ToList();

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
                            if (item.IsNew != true && tbxEqpId.Text.Trim() == item.EQP_ID)
                            {
                                var param = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD", this.BaseClass.CenterCD.ToString() },
                                    { "P_EQP_ID", item.EQP_ID },
                                    { "P_CHK_SERIAL_NO", item.CHK_SERIAL_NO },
                                    { "P_USER_ID", _USER_ID }
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_EBSE004_01P.SP_EMS_EQP_CHK_LIST_DELETE";

                                var outData = da.GetSpDataSet(      // 데이터베이스 연결 문자열
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
                                CurrEqpChkList.Remove(item);
                            }

                            gridChkList.RefreshData();
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
        #endregion

        #region ▩ 이벤트
        /// <summary>
        /// 화면 오픈시 선행 작업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EBSE004_01P_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= EBSE004_01P_Loaded;


            tbxEqpClsNm.Text = TargetClsNm;
            tbxEqpClsNm.Tag = TargetClsID;

            if (!string.IsNullOrEmpty(TargetEqpID))
            {
                EqpInfoSel();
            }
            else
            {
                CurrEqpChkList = new ObservableCollection<EmsEqpChkList>();
                CurrAppdFiles = new ObservableCollection<EmsAppdFile>();
                //FileList = new Dictionary<FileToByteArray, EmsAppdFile>();

                gridAttrachList.ItemsSource = null;
                gridAttrachList.ItemsSource = CurrAppdFiles;

                gridChkList.ItemsSource = null;
                gridChkList.ItemsSource = CurrEqpChkList;
            }
        }

        /// <summary>
        /// 창 종료시 화면 정리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EBSE004_01P_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Closing -= EBSE004_01P_Closing;

            if (null != _imageStream)
            {
                _imageStream.Close();
                _imageStream = null;
            }

            if (!string.IsNullOrEmpty(_imageId))
            {
                string aDown = _imageId + ".jpg";

                string path = System.IO.Path.GetDirectoryName(
                                Environment.GetFolderPath(
                                    Environment.SpecialFolder.Personal));

                path = System.IO.Path.Combine(path, "Downloads\\");

                if (System.IO.File.Exists(path + aDown))
                {
                    System.IO.File.Delete(path + aDown);
                }
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

        #region > 버튼 이벤트
        #region >> 조회 버튼 클릭
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TargetEqpID))
                {
                    EqpInfoSel();
                }
                else
                {
                    CurrEqpChkList = new ObservableCollection<EmsEqpChkList>();
                    CurrAppdFiles = new ObservableCollection<EmsAppdFile>();
                    //FileList = new Dictionary<FileToByteArray, EmsAppdFile>();

                    gridAttrachList.ItemsSource = null;
                    gridAttrachList.ItemsSource = CurrAppdFiles;

                    gridChkList.ItemsSource = null;
                    gridChkList.ItemsSource = CurrEqpChkList;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 저장 버튼 클릭
        /// <summary>
        /// 저장 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            SaveEmsEqp();
        }
        #endregion

        #region >> 관리자 검색 버튼 클릭
        /// <summary>
        /// 관리자 검색
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EqpMngrSearch_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (ECOM001_07P frmEqpMngrSearch = new ECOM001_07P())
                {
                    frmEqpMngrSearch.ShowDialog();

                    if (null != frmEqpMngrSearch.CurrMngr)
                    {
                        tbxEqpMngrNm.Text = frmEqpMngrSearch.CurrMngr.MNGR_NM;
                        tbxEqpMngrNm.Tag = frmEqpMngrSearch.CurrMngr.EQP_MNGR_ID;
                        tbxEqpMngrTelNo.Text = frmEqpMngrSearch.CurrMngr.MNGR_TEL_NO;
                        tbxEqpVndrNm.Text = frmEqpMngrSearch.CurrMngr.VNDR_NM;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 설비 위치 검색 버튼 클릭
        /// <summary>
        /// 설비 위치 검색
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PbsSearch_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (ECOM001_02P frmPbsSearch = new ECOM001_02P())
                {
                    frmPbsSearch.ShowDialog();

                    if (null != frmPbsSearch.CurrPbs)
                    {
                        tbxEqpPbsNm.Text = frmPbsSearch.CurrPbs.PBS_NM.Trim();
                        tbxEqpPbsNm.Tag = frmPbsSearch.CurrPbs.PBS_ID;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 설비 검색 버튼 클릭
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
                        TargetEqpID = frmEqpSearch.CurrEqpId;
                        EqpInfoSel();
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 설비 분류 검색 버튼 클릭
        /// <summary>
        /// 설비 분류 검색
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EqpClsSearch_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (ECOM001_08P frmEqpClsSearch = new ECOM001_08P())
                {
                    frmEqpClsSearch.ShowDialog();

                    if (null != frmEqpClsSearch.CurrEqpCls)
                    {
                        tbxEqpClsNm.Text = frmEqpClsSearch.CurrEqpCls.EQP_CLS_NM.Trim();
                        tbxEqpClsNm.Tag = frmEqpClsSearch.CurrEqpCls.EQP_CLS_ID;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 장애이력 버튼 클릭
        /// <summary>
        /// 장애이력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnErrHis_Click(object sender, MouseButtonEventArgs e)
        {
            if (null == CurrEqpInfo)
            {
                return;
            }

            try
            {
                using (ECOM001_09P frmErrHisSearch = new ECOM001_09P(CurrEqpInfo.EQP_ID, CurrEqpInfo.EQP_NM))
                {
                    frmErrHisSearch.ShowDialog();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 부품정보 버튼 클릭
        /// <summary>
        /// 부품정보
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPartList_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (null != CurrEqpInfo)
                {
                    using (E1004_02P frmPartList = new E1004_02P(CurrEqpInfo))
                    {
                        frmPartList.ShowDialog();
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 첨부파일 추가
        /// <summary>
        /// 첨부 파일 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileAdd_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                //FileToByteArray ftB = new FileToByteArray();
                //ftB.Init(openFileDialog.FileName);

                EmsAppdFile aF = new EmsAppdFile()
                {
                    APPD_FILE_NM = openFileDialog.FileName,
                    APPD_FILE_DEV_CD = "B",
                    APPD_FILE_GROUP_CD = "",
                    APPD_FILE_GUID = Guid.NewGuid().ToString(),
                    IsNew = true,
                    IsSelected = true
                };
                //FileList.Add(ftB, aF);

                CurrAppdFiles.Add(aF);

                gridAttrachList.Focus();
                gridAttrachList.CurrentColumn = gridAttrachList.Columns.First();
                gridAttrachList.View.FocusedRowHandle = CurrAppdFiles.Count - 1;
            }
        }
        #endregion

        #region >> 첨부파일 삭제 버튼 클릭
        /// <summary>
        /// 첨부화일 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileDelete_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.CurrAppdFiles.Count() == 0) { return; }

                if (this.CurrAppdFiles.Where(p => p.IsSelected).Count() == 0)
                {
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return;
                }

                if (this.CurrAppdFiles.Where(p => p.IsNew).Count() > 0)
                {
                    // Msg : 조회된 데이터는 행삭제를 할 수 없습니다.|삭제버튼을 이용하여 삭제해주세요.
                    this.BaseClass.MsgError("ERR_EXISTS_INQ_DATA");
                    return;
                }

                var liList = this.CurrAppdFiles.Where(p => p.IsSelected == true && p.IsNew == true).ToList();
                if (liList.Count() <= 0)
                {
                    BaseClass.MsgError("ERR_DELETE");
                }
                liList.ForEach(p => CurrAppdFiles.Remove(p));
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 체크리스트 (마스터 참조) 추가
        /// <summary>
        /// 체크리스트 (마스터 참조) 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMasterRef_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (ECOM001_03P frmChkMasterRef = new ECOM001_03P())
                {
                    frmChkMasterRef.ShowDialog();

                    for (int i = 0; i < frmChkMasterRef.CurrChkList.Count; ++i)
                    {
                        if (frmChkMasterRef.onApply && frmChkMasterRef.CurrChkList[i].IsSelected)
                        {
                            //var already = CurrEqpChkList.Where(CK => frmChkMasterRef.CurrChkList[i].CHK_NO == CK.CHK_SERIAL_NO).ToList();

                            //if (0 == already.Count)
                            {
                                CurrEqpChkList.Add(new EmsEqpChkList()
                                {
                                    CENTER_CD = this.BaseClass.CenterCD,
                                    EQP_ID = TargetEqpID,
                                    CHK_SERIAL_NO = 0,
                                    CHK_ITEM = frmChkMasterRef.CurrChkList[i].CHK_CONTENT,
                                    CHK_NOTE = frmChkMasterRef.CurrChkList[i].CHK_NOTE,
                                    RSTR_ID = this.BaseClass.UserID,
                                    UPDR_ID = this.BaseClass.UserID,
                                    FromMaster = true,
                                    IsNew = true
                                });
                            }
                        }
                    }

                    frmChkMasterRef.CurrChkList.Clear();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 체크리스트 Row 추가
        /// <summary>
        /// 체크리스트 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheckListAdd_Click(object sender, MouseButtonEventArgs e)
        {
            CurrEqpChkList.Add(new EmsEqpChkList()
            {
                RSTR_ID = this.BaseClass.UserID,
                UPDR_ID = this.BaseClass.UserID,
                IsNew = true,
                IsSelected = true
            });

            gridChkList.Focus();
            gridChkList.CurrentColumn = gridChkList.Columns.First();
            gridChkList.View.FocusedRowHandle = CurrEqpChkList.Count - 1;
        }
        #endregion

        #region >> 체크리스트 항목 삭제 버튼 (내부 플래그 변환으로 그리드에서만 사라지고 저장할 때 Delete 로직 처리)
        /// <summary>
        /// Checklist 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChlDeleteChk_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.CurrEqpChkList.Count() == 0) { return; }

                var liSelectedItems = this.CurrEqpChkList.Where(p => p.IsSelected);

                if (liSelectedItems.Count() == 0)
                {
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return;
                }

                // 신규 추가된 Row는 삭제
                this.CurrEqpChkList.Where(p => p.IsSelected == true && p.IsNew == true).ToList().ForEach(p =>
                {
                    this.CurrEqpChkList.Remove(p);
                });

                // 조회, 
                this.CurrEqpChkList.Where(p => p.IsSelected == true && (p.IsNew == false || p.IsUpdate == true)).ToList().ForEach(p =>
                {
                    p.IsDelete = true;
                });

                gridChkList.ItemsSource = null;
                gridChkList.ItemsSource = CurrEqpChkList;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region > 첨부파일 다운로드
        /// <summary>
        /// 첨부화일 다운로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridAttrachList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridAttachListView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) { return; }

            if (gridAttrachList.IsGroupRowHandle(rowHandle)) { return; }// A group row has been double clicked

            EmsAppdFile target = gridAttrachList.SelectedItem as EmsAppdFile;

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
                this.BaseClass.MsgError("ERR_FAIL_FILE_TRANS");
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
        // ~E1004_01P()
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

        private void btnDeleteFile_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.CurrAppdFiles.Count() == 0) { return; }

                if (this.CurrAppdFiles.Where(p => p.IsSelected).Count() == 0)
                {
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return;
                }

                this.DelAttachList();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void btnDeleteCheckList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.CurrEqpChkList.Count() == 0) { return; }

                if (this.CurrEqpChkList.Where(p => p.IsSelected).Count() == 0)
                {
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return;
                }

                this.DelChkList();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
    }
}
