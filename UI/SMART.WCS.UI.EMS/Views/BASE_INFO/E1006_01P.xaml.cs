using DevExpress.Mvvm.Native;
using Microsoft.Win32;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.E1006;
using SMART.WCS.UI.EMS.Views.COM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace SMART.WCS.UI.EMS.Views.BASE_INFO
{
    /// <summary>
    /// E1006_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E1006_01P : Window, IDisposable
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
        #endregion

        #region ▩ 생성자
        public E1006_01P()
        {
            InitializeComponent();
        }

        public E1006_01P(string _strClsID, string _strClsNm, string _strPartID)
        {
            try
            {
                InitializeComponent();

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                if (!string.IsNullOrEmpty(_strClsID) && !string.IsNullOrEmpty(_strClsNm))
                {
                    TargetClsID = _strClsID;
                    TargetClsNm = _strClsNm;

                    tbxPartClsId.Text = _strClsNm;
                    tbxPartClsId.Tag = _strClsID;
                }

                TargetPartID = _strPartID;

                btnFormClose.Click += btnFormClose_Click;

                this.Loaded += EBSE006_01P_Loaded;
                this.Closing += EBSE006_01P_Closing;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion


        #region ▩ 속성
        private string TargetClsID { get; set; }
        private string TargetClsNm { get; set; }
        private string TargetPartID { get; set; }
        private EmsPartInfo CurrPartInfo { get; set; }
        #endregion

        #region ▩ 함수
        #region Part Info Sel
        /// <summary>
        /// Part Info Sel
        /// </summary>
        private void PartInfoSel()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD", this.BaseClass.CenterCD },
                    {"P_PART_ID", TargetPartID}
                };

                var strOutParam = new[] { "P_EMS_PART_INFO" };
                string callProc = "PK_EMS_EBSE006_01P.SP_EMS_PART_INFO_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                    callProc                      // 호출 프로시저
                                ,   param                         // Input 파라메터
                                ,   strOutParam                   // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        var info = new ObservableCollection<EmsPartInfo>();
                        info.ToObservableCollection(outData.Tables[0]);

                        CurrPartInfo = info[0];
                        CurrPartInfo.IsUpdate = true;

                        _imageId = (null == CurrPartInfo.PART_IMG_FILE_ID) ? "" : CurrPartInfo.PART_IMG_FILE_ID;

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
                                imgPartImage.Source = imageSource as ImageSource;
                            }
                            catch (Exception)
                            {
                                this.BaseClass.MsgError("ERR_FAIL_IMG_TRANS");
                            }
                        }

                        tbxPartId.Text = CurrPartInfo.PART_ID;
                        tbxPartNm.Text = CurrPartInfo.PART_NM;

                        tbxPartMnfact.Text = CurrPartInfo.PART_MNFACT;
                        tbxPbsStnd.Text = CurrPartInfo.PART_STND;
                        tbxInstModel.Text = CurrPartInfo.PART_MODEL;
                        tbxPurchNm.Text = CurrPartInfo.PURCH_NM;

                        tbxLifrCle.Text = CurrPartInfo.LIFE_CLE.ToString();
                        tbxAlmLeadTime.Text = CurrPartInfo.ALM_LEAD_TIME.ToString();

                        tbxPartClsId.Text = CurrPartInfo.PART_CLS_NM;
                        tbxPartClsId.Tag = CurrPartInfo.PARENT_ID;

                        rbStokMngY.IsChecked = "Y" == CurrPartInfo.STOCK_MNG_YN;
                        rbStokMngN.IsChecked = !rbStokMngY.IsChecked.Value;

                        if (null != CurrPartInfo.PART_IMG)
                        {
                            _imageData = CurrPartInfo.PART_IMG.ToArray();

                            var imageSource = new BitmapImage();

                            // Get the primitive byte data into in-memory data stream
                            _imageStream = new MemoryStream(CurrPartInfo.PART_IMG);
                            imageSource.BeginInit();
                            imageSource.StreamSource = _imageStream;
                            imageSource.EndInit();

                            // Assign the Source property of your image
                            imgPartImage.Source = imageSource as ImageSource;
                        }

                        info.Clear();
                        info = null;
                    }
                    else
                    {
                        CurrPartInfo = new EmsPartInfo();
                        CurrPartInfo.IsNew = true;
                    }
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion Eqp Info Sel        

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
                imgPartImage.Source = imageSource as ImageSource;
            }
        }

        #endregion Eqp Image Attach

        #region 저장
        /// <summary>
        /// 저장 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            SaveEmsPart();
        }

        /// <summary>
        /// 저장 함수
        /// </summary>
        private void SaveEmsPart()
        {
            if (string.IsNullOrEmpty(tbxPartId.Text.Trim()) || string.IsNullOrEmpty(tbxPartNm.Text.Trim()))
            {
                // ERR_REQ_ID_NM - ID/명칭은 필수 항목입니다.
                this.BaseClass.MsgError("ERR_REQ_ID_NM");
                return;
            }

            if (null == tbxPartClsId.Tag || string.IsNullOrEmpty(tbxPartClsId.Text.Trim()))
            {
                // ERR_REQ_PART_SPR - 부품 분류는 필수입력입니다.
                this.BaseClass.MsgError("ERR_REQ_PART_SPR");
                return;
            }

            if (string.IsNullOrEmpty(tbxLifrCle.Text.Trim()))
            {
                // ERR_REQ_LIFECYLE - 수명주기는 필수항목입니다.
                this.BaseClass.MsgError("ERR_REQ_LIFECYLE");
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
                    // ERR_FAIL_IMG_PROC - 이미지 파일 구성에 실패하였습니다.
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
                        // ERR_FAIL_SEND_IMG - 이미지 전송에 실패하였습니다.
                        this.BaseClass.MsgError("ERR_FAIL_SEND_IMG");
                        return;
                    }

                    System.IO.File.Delete(cpy);
                }
                catch (Exception)
                {
                    // ERR_FAIL_SEND_IMG - 이미지 전송에 실패하였습니다.
                    this.BaseClass.MsgError("ERR_FAIL_SEND_IMG");
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
                        { "P_PART_ID", tbxPartId.Text.Trim() },                 // ID
                        { "P_PART_NM", tbxPartNm.Text.Trim() },                 // 이름
                        { "P_PART_MNFACT", tbxPartMnfact.Text.Trim() },         // 제조사
                        { "P_LIFE_CLE", int.Parse(tbxLifrCle.Text.Trim()) },    // 수명주기
                        { "P_ALM_LEAD_TIME", int.Parse(string.IsNullOrEmpty(tbxAlmLeadTime.Text.Trim()) ? "0" : tbxAlmLeadTime.Text.Trim()) },  // 사전 알림 일수
                        { "P_PART_MODEL", tbxInstModel.Text.ToString() },               // 모델
                        { "P_PART_STND", tbxPbsStnd.Text.Trim() },                      // 규격
                        { "P_STOCK_MNG_YN", (rbStokMngY.IsChecked.Value) ? "Y" : "N" }, // 재고 관리 여부
                        { "P_PURCH_NM", tbxPurchNm.Text.ToString() },                   // 구입처
                        //{ "P_PART_IMG", (null ==_imageData) ? new byte[0] : _imageData},
                        { "P_PART_IMG_FILE_ID", string.IsNullOrEmpty(_imageId) ? "" : _imageId },   // Image ID
                        { "P_PART_CLS_ID", tbxPartClsId.Tag.ToString() },                           // 분류 ID
                        { "P_USER_ID", _USER_ID }
                    };

                    var strOutParam = new[] { "P_RESULT" };
                    string callProc = "PK_EMS_EBSE006_01P.SP_EMS_PART_INFO_SAVE";

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

                    if (_SUCCESS_CODE == "100")
                    {
                        da.CommitTransaction();
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_SAVE_DATA"));

                        this.BaseClass.MsgInfo("CMPT_SAVE");
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
        #endregion 저장

        #region 검색 Popup
        /// <summary>
        /// 부품분류 검색 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EqpSearch_Click(object sender, MouseButtonEventArgs e)
        {
            EqpSearch();
        }

        /// <summary>
        /// 부품분류 검색
        /// </summary>
        private void EqpSearch()
        {
            try
            {
                using (ECOM001_10P frmPartClsSearch = new ECOM001_10P())
                {
                    frmPartClsSearch.ShowDialog();

                    if (null != frmPartClsSearch.CurrPartCls)
                    {
                        tbxPartClsId.Text = frmPartClsSearch.CurrPartCls.PART_CLS_NM.Trim();
                        tbxPartClsId.Tag = frmPartClsSearch.CurrPartCls.PART_CLS_ID;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 사용설비 검색 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUseEqp_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                UseEqp();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 사용설비 검색
        /// </summary>
        private void UseEqp()
        {
            try
            {
                using (ECOM001_06P frmUseEqpSearch = new ECOM001_06P(tbxPartId.Text.Trim(), tbxPartNm.Text.Trim()))
                {
                    frmUseEqpSearch.ShowDialog();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 입출고 이력 검색 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWhHis_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PartWhHis();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 입출고 이력 검색
        /// </summary>
        private void PartWhHis()
        {
            try
            {
                using (ECOM001_04P frmPartWhHis = new ECOM001_04P(tbxPartId.Text.Trim(), tbxPartNm.Text.Trim()))
                {
                    frmPartWhHis.ShowDialog();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion 검색 Popup
        #endregion

        #region ▩ 이벤트
        /// <summary>
        /// 화면 오픈시 선행 작업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EBSE006_01P_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Loaded -= EBSE006_01P_Loaded;

                if (!string.IsNullOrEmpty(TargetPartID))
                {
                    PartInfoSel();
                    tbxPartNm.IsReadOnly = false;
                }
                else
                {
                    tbxPartId.IsReadOnly = false;
                    tbxPartNm.IsReadOnly = false;
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
        /// 창 종료시 화면 정리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EBSE006_01P_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.Closing -= EBSE006_01P_Closing;

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
        // ~E1006_01P()
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
