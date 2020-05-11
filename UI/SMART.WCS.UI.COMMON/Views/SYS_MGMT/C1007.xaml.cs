using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Control;
using SMART.WCS.UI.COMMON.DataMembers.C1007;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.Views.SYS_MGMT
{
    /// <summary>
    /// 배포 관리
    /// </summary>
    public partial class C1007 : UserControl
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
        /// Base 클래서 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        /// <summary>
        /// 서버 배포 파일 리스트
        /// </summary>
        private List<ServerFileList> g_liServerFileList = new List<ServerFileList>();

        /// <summary>
        /// 배포 대상 파일 리스트 (서버 파일 리스트 + 로컬 파일 리스트)
        /// </summary>
        private List<LocalFileList> g_liDeployFileInfoList = new List<LocalFileList>();
        #endregion

        #region ▩ 생성자
        public C1007()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation"></param>
        public C1007(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 상단에 설명 
                this.CommentArea.ScreenID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1007), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

        public static void SetIsEnabled(UIElement element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        private static void IsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                TableView view = source as TableView;
                view.ShowingEditor += View_ShowingEditor;
            }
        }
        #endregion

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1007), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string GridRowCount
        {
            get { return (string)GetValue(GridRowCountProperty); }
            set { SetValue(GridRowCountProperty, value); }
        }
        #endregion
        #endregion

        #region ▩ 웹서비스 호출
        #region > WebService
        #region >> CompressionBackupFileByWebService - 서버 백업파일을 수행한다. (웹서비스 호출)
        /// <summary>
        /// 서버 백업 완료된 파일명 (웹서비스 호출)
        /// </summary>
        /// <param name="_strBackupFileName"></param>
        /// <returns></returns>
        private bool CompressionBackupFileByWebService(ref string _strBackupFileName)
        {
            try
            {
                bool bRtnValue      = true;

                //// 상태바 (아이콘) 실행
                //this.loadingScreen.IsSplashScreenShown = true;

                using (DeployWebService.LiveUpdateWebServiceClient client = new DeployWebService.LiveUpdateWebServiceClient())
                {
                    var strBackupFileName = string.Empty;
                    client.CompressionBackupFile(ref _strBackupFileName);
                }

                //this.loadingScreen.IsSplashScreenShown = false;

                return bRtnValue;
            }
            catch { throw; }
            finally
            {
                //this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        #region >> CheckServerDeployDirectory - 서버 배포 경로 존재 여부 체크 및 생성 (웹서비스 호출)
        /// <summary>
        /// 서버 배포 경로 존재 여부 체크 및 생성 (웹서비스 호출)
        /// </summary>
        private void CheckServerDeployDirectory()
        {
            try
            {
                using (DeployWebService.LiveUpdateWebServiceClient client = new DeployWebService.LiveUpdateWebServiceClient())
                {
                    client.CheckServerDeployDirectory();
                }
            }
            catch { throw; }
        }
        #endregion

        #region >> GetServerFileListByWebService - WebService를 이용하여 서버 배포 경로의 파일 리스트를 가져온다. (웹서비스 호출)
        /// <summary>
        /// WebService를 이용하여 서버 배포 경로의 파일 리스트를 가져온다. (웹서비스 호출)
        /// <paramref name="_liLocalFileList">로컬 파일 리스트 정보</paramref>
        /// </summary>
        private List<ServerFileList> GetServerFileListByWebService(ref List<LocalFileList> _liLocalFileList)
        {
            try
            {
                // 서버 파일 리스트 저장 변수
                List<ServerFileList> liServerFileList = new List<ServerFileList>();

                //using (LocalDeployWebService.LiveUpdateWebServiceClient client = new LocalDeployWebService.LiveUpdateWebServiceClient())
                using (DeployWebService.LiveUpdateWebServiceClient client = new DeployWebService.LiveUpdateWebServiceClient())
                {
                    // 웹서비스 호출
                    var resp = client.GetServerDeployList();

                    // 호출 후 리턴값이 있는 경우
                    if (resp.Count() > 0)
                    {
                        foreach (KeyValuePair<string, string[]> list in resp)
                        {   
                            ServerFileList df           = new ServerFileList();
                            string strFileDirectory     = string.Empty;
                            string strResource          = string.Empty;

                            #region + 다국어 구분을 위한 구문
                            if (list.Key.Contains("ko-KR"))
                            {
                                strResource = "ko-KR";
                            }
                            else if (list.Key.Contains("th-TH"))
                            {
                                strResource = "th-TH";
                            }
                            #endregion

                            foreach (var item in list.Value)
                            {
                                if (item[2].ToString().Length > 0)
                                {
                                    strFileDirectory    = item[2].ToString();
                                    break;
                                }
                            }

                            for (int i = 0; i < list.Value.Count(); i++)
                            { 
                                df.FILE_NAME            = list.Value[0].ToString();     // 파일명
                                df.FILE_EXTENSION       = list.Value[1].ToString();     // 파일 확장자
                                df.FILE_DIRECTORY       = list.Value[2].ToString();     // 파일 경로
                                df.SERVER_FILE_VERSION  = list.Value[3].ToString();     // 서버파일 버전
                                df.RESOURCE_KIND        = strResource;                  // 리소스 파일 종류   

                                // 로컬리스트와 서버리스트를 비교해서 같은 파일인 경우 서버파일 버전을 로컬파일 버전 필드에 저장
                                _liLocalFileList.Where(p => p.FILE_NAME == df.FILE_NAME).ToList().ForEach(p => p.SERVER_FILE_VERSION = df.SERVER_FILE_VERSION);
                            }

                            liServerFileList.Add(df);
                        }
                    }
                }

                return liServerFileList;
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        private void InitControl()
        {
            try
            {
            }
            catch { throw; }
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            try
            {
                this.Loaded += C1007_Loaded;

                // 조회 버튼 클릭
                this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;

                // 배포 버튼 클릭
                this.btnDeploy.PreviewMouseLeftButtonUp += BtnDeploy_PreviewMouseLeftButtonUp;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > GetDeployLocalFileInfo - 로컬에 배포된 파일 리스트를 가져온다.
        /// <summary>
        /// 로컬에 배포된 파일 리스트를 가져온다.
        /// </summary>
        /// <param name="_strLocalFilePath">로컬 배포 경로</param>
        /// <returns></returns>
        private DataTable GetDeployLocalFileInfo(string _strLocalFilePath)
        {
            try
            {
                DataTable dtLocalFileList   = new DataTable();
                DataTable dtLanguageType    = this.GetLanguageType();       // 다국어 타입 (ko-KR, th-TH)
                DirectoryInfo di            = null;

                // 파일 리스트를 저장할 데이터테이블 스키마를 생성한다.
                dtLocalFileList = this.CreateDataTableSchema(dtLocalFileList);

                di              = new DirectoryInfo(_strLocalFilePath);
                DataRow drRow   = null;

                foreach (var fi in di.GetFiles())
                {
                    // 프로그램 실행에 사용하지 않는 파일 (.xml
                    if (fi.Extension.Equals(".xml")) { continue; }
                    if (fi.Extension.Equals(".pdb")) { continue; }
                    if (fi.Extension.Equals(".config"))
                    {
                        if (fi.Name.Equals("SMART.WCS.exe.config") == false) { continue; }
                    }

                    drRow                               = dtLocalFileList.NewRow();
                    drRow["FILE_NAME"]                  = fi.Name;              // 파일명
                    drRow["FILE_EXTENSION"]             = fi.Extension;         // 확장자
                    drRow["LOCAL_DIRECTORY"]            = fi.DirectoryName;     // 폴더명

                    // 어셈블리 정보가 있는 dll 파일만 버전 정보를 저장한다.
                    // 어셈블리 정보가 없는 파일을 `sion함수에 태우면 오류 발생함.
                    if (fi.Extension.ToUpper().Equals(".DLL") || fi.Extension.ToUpper().Equals(".EXE"))
                    {
                        drRow["LOCAL_FILE_VERSION"] = this.BaseClass.GetFileVersion($"{fi.DirectoryName}\\{fi.Name}");
                    }
                    
                    dtLocalFileList.Rows.Add(drRow);
                }

                // 서브 디렉토리 (다국어, 리소스 정보)를 조회
                DirectoryInfo[] arrSubDirInfo   = di.GetDirectories();
                foreach (DirectoryInfo dirInfo in arrSubDirInfo)
                {
                    // 컴파일 폴더에 다국어 폴더외에 기타 폴더들이 존재하기 때문에 
                    // 다국어 외 폴더를 필터링하기 위한 구문
                    var iCount = dtLanguageType.AsEnumerable().Where(p => p.Field<string>("CODE").Contains(dirInfo.Name)).Count();
                    if (iCount == 0) { continue; }

                    foreach (FileInfo fi in dirInfo.GetFiles())
                    {
                        drRow                           = dtLocalFileList.NewRow();
                        drRow["FILE_NAME"]              = fi.Name;              // 파일명
                        drRow["FILE_EXTENSION"]         = fi.Extension;         // 확장자
                        drRow["LOCAL_DIRECTORY"]        = fi.DirectoryName;     // 폴더명

                        if (fi.Extension.ToUpper().Equals(".DLL"))
                        {
                            // 파일 버전
                            drRow["LOCAL_FILE_VERSION"] = this.BaseClass.GetFileVersion($"{fi.DirectoryName}\\{fi.Name}");
                        }

                        dtLocalFileList.Rows.Add(drRow);
                    }
                }

                return dtLocalFileList;
            }
            catch { throw; }
        }
        #endregion

        #region > GetLanguageType - 언어 타입을 조회한다. (로컬 리스트 조회시 다국어 폴더 필터를 위해-다국어외에 기타 폴더들 때문에 다국어폴더 조회하는경우 오류 발생 가능성이 있음)
        /// <summary>
        /// 언어 타입을 조회한다.
        /// <br />로컬 리스트 조회시 다국어 폴더 필터를 위해
        /// <br />다국어외에 기타 폴더들 때문에 다국어폴더 조회하는경우 오류 발생 가능성이 있음
        /// </summary>
        /// <returns></returns>
        private DataTable GetLanguageType()
        {
            try
            {
                var strCommonCD     = "LANG_RESO_TYPE";
                return CommonComboBox.GetCommonData(strCommonCD, null, false);
            }
            catch { throw; }
        }
        #endregion

        #region > CompareLocalServerFileList - 로컬, 서버 파일 리스트를 비교한다.
        /// <summary>
        /// 로컬, 서버 파일 리스트를 비교한다.
        /// </summary>
        private List<LocalFileList> CompareLocalServerFileList()
        {
            try
            {
                string strServerDeployPath              = string.Empty;
                string strServerResourceDeployPath      = string.Empty;

                // 로컬 설치파일 경로
                var strCompilePath          = this.BaseClass.GetAppSettings("CompilePath");
                // 로컬 설치파일 리스트를 가져온다.
                var dtParam                 = this.GetDeployLocalFileInfo(strCompilePath);
                // 로컬 파일 리스트를 List형으로 변환한다.
                var liLocalFileList         = SMART.WCS.Common.Data.ConvertDataTableToList.DataTableToList<LocalFileList>(dtParam);

                #region + WebService를 이용하여 서버 배포 경로의 파일 리스트를 가져온다.
                this.g_liServerFileList = this.GetServerFileListByWebService(ref liLocalFileList);
                #endregion

                #region + 서버 경로를 지정한다.
                liLocalFileList.ForEach(p =>
                {
                    if (p.LOCAL_DIRECTORY.Contains("KR"))
                    {
                        // http://localhost:64500/DeployUpload_KR.aspx
                        p.SERVER_DIRECTORY = this.BaseClass.GetAppSettings("DeployServerPath_KR");
                    }
                    else if (p.LOCAL_DIRECTORY.Contains("TH"))
                    {

                    }
                    else
                    {
                        // http://localhost:64500/DeployUpload.aspx
                        p.SERVER_DIRECTORY = this.BaseClass.GetAppSettings("DeployServerPath");
                    }
                });
                #endregion

                if (this.g_liServerFileList.Count() == 0)
                {
                    // 서버에 배포된 파일이 없습니다.|전체 배포를 수행하세요.
                    this.BaseClass.MsgInfo("ERR_NOT_EXISTS_DEPLOY");
                    //liLocalFileList.ForEach(p => p.IsSelected = true);

                    liLocalFileList.ForEach(p =>
                    {
                        p.IsSelected            = true;
                        p.COMPARE_RESULT        = "NEW";
                        p.ForegroundBrush       = new SolidColorBrush(Colors.Red);
                        p.WeightBold            = System.Windows.FontWeights.Bold;
                    });

                    return liLocalFileList;
                }

                // 서버 배포본에는 없고, 로컬에 파일이 존재하는 경우
                // 신규로 배포되는 파일
                var newItem = liLocalFileList.Where(p => !this.g_liServerFileList.Where(t => t.FILE_NAME == p.FILE_NAME).Any()).ToList();
                liLocalFileList.Where(p => newItem.Where(t => t.FILE_NAME == p.FILE_NAME).Any()).ToList().ForEach(p => p.COMPARE_RESULT = "NEW");
                
                // 로컬, 서버 파일 버전이 다른 경우
                liLocalFileList.Where(p => this.g_liServerFileList.Where(t => t.FILE_NAME == p.FILE_NAME && t.SERVER_FILE_VERSION != p.LOCAL_FILE_VERSION).Any()).ToList().ForEach(p => p.COMPARE_RESULT = "UPDATE");

                // 비교 결과에 따른 그리드 글자 (Foreground) 색상 정의
                if (liLocalFileList.Count > 0)
                {
                    liLocalFileList.Where(p => p.COMPARE_RESULT.Equals("NEW")).ToList().ForEach(f =>
                    {
                        f.IsSelected        = true;
                        f.IsNew             = true;
                        f.ForegroundBrush   = new SolidColorBrush(Colors.Red);
                        f.WeightBold        = System.Windows.FontWeights.Bold;
                    });

                    liLocalFileList.Where(p => p.COMPARE_RESULT.Equals("UPDATE")).ToList().ForEach(f =>
                    {
                        f.IsSelected        = true;
                        f.IsNew             = true;
                        f.ForegroundBrush   = new SolidColorBrush(Colors.Blue);
                        f.WeightBold        = System.Windows.FontWeights.Bold;
                    });
                }

                return liLocalFileList;
            }
            catch { throw; }
        }
        #endregion

        #region > 기타 함수
        #region >> SetResultText - 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        private void SetResultText()
        {
            var strResource = string.Empty;                                                           // 텍스트 리소스 (전체 데이터 수)
            var iTotalRowCount = 0;                                                                   // 조회 데이터 수

            strResource = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                            // 텍스트 리소스
            iTotalRowCount = (this.gridMain.ItemsSource as ICollection).Count;                      // 전체 데이터 수
            this.GridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                       // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
        }
        #endregion
        #endregion

        #region > CreateDataTableSchema - 파일 정보를 저장할 데이터테이블 스키마를 생성한다.
        /// <summary>
        /// 파일 정보를 저장할 데이터테이블 스키마를 생성한다.
        /// </summary>
        /// <param name="_dtNewTable">파일정보 저장 데이터테이블</param>
        /// <returns></returns>
        private DataTable CreateDataTableSchema(DataTable _dtNewTable)
        {
            try
            {
                if (_dtNewTable == null) { _dtNewTable = new DataTable(); }

                _dtNewTable.Columns.Add("FILE_NAME",                typeof(string));    // 파일명
                _dtNewTable.Columns.Add("FILE_EXTENSION",           typeof(string));    // 파일 확장자
                _dtNewTable.Columns.Add("LOCAL_DIRECTORY",           typeof(string));    // 파일 디렉토리
                _dtNewTable.Columns.Add("LOCAL_FILE_VERSION",       typeof(string));    // 로컬 파일 버전
                _dtNewTable.Columns.Add("COMPARE_RESULT",           typeof(string));    // 파일 수정 결과

                return _dtNewTable;
            }
            catch { throw; }
        }
        #endregion

        #region > 데이터 관련
        #region GetComboBoxCommonData - 콤보박스 설정 데이터 조회 및 컨트롤 바인딩
        /// <summary>
        /// 콤보박스 설정 데이터 조회 및 컨트롤 바인딩
        /// </summary>
        private DataTable GetComboBoxCommonData()
        {
            var strCommonCD         = "PROG_KIND";
            return SMART.WCS.Common.Control.CommonComboBox.GetCommonData(strCommonCD, null, false);
        }


        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 로드 이벤트
        /// <summary>
        /// 로드 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void C1007_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Loaded -= C1007_Loaded;

                this.g_liDeployFileInfoList = this.CompareLocalServerFileList();

                this.gridMain.ItemsSource = this.g_liDeployFileInfoList;
            }
            catch (Exception err)
            {
                string[] arrMessage = err.Message.Split('\'');
                var strErrMessage   = $"{arrMessage[1]}|{arrMessage[2]}";
                this.BaseClass.MsgError(strErrMessage, BaseEnumClass.CodeMessage.MESSAGE);
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 버튼 이벤트
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.g_liDeployFileInfoList = this.CompareLocalServerFileList();

                this.gridMain.ItemsSource = this.g_liDeployFileInfoList;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 배포 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeploy_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try    
            {
                #region + 서버 배포파일 백업 (웹서비스 호출)
                string strBackupFileName = string.Empty;
                // 웹서비스 호출
                CompressionBackupFileByWebService(ref strBackupFileName);
                #endregion

                #region + 그리드 화면에서 선택된 데이터 - 서버 버전과 로컬 버전 체크 리스트
                var liSelectedList = this.g_liDeployFileInfoList.Where(p => p.IsSelected).ToList();
                #endregion

                if (liSelectedList.Count == 0) { return; }

                #region + 서버 배포를 위한 서버 폴더 체크 (웹서비스 호출)
                // 배포를 위한 서버 폴더 존재 여부 체크 및 생성
                this.CheckServerDeployDirectory();
                #endregion

                #region + 로컬 파일을 서버로 전송 (배포)하기 위해 팝업 오픈
                using (C1007_01P frmPopup = new C1007_01P(liSelectedList))
                {
                    frmPopup.WindowStartupLocation  = WindowStartupLocation.CenterScreen;
                    frmPopup.ShowDialog();
                }
                #endregion

                #region + 재조회 (서버, 로컬 리스트 비교 구문 호출)
                this.g_liDeployFileInfoList = this.CompareLocalServerFileList();

                this.gridMain.ItemsSource = this.g_liDeployFileInfoList;
                #endregion
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 그리드 이벤트
        #region >> 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        /// <summary>
        /// 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void View_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            try
            {
                if (g_IsAuthAllYN == false)
                {
                    e.Cancel = true;
                    return;
                }

                TableView tv                = sender as TableView;
                DeployFileInfo dataMember   = tv.Grid.CurrentItem as DeployFileInfo;

                if (dataMember == null) { return; }
            }
            catch { throw; }
        }
        #endregion

        #region >> 그리드 체크박스 선택 이벤트
        private void SelectorColumnBehavior_SelectorCellCheked(object sender, Modules.Ctrl.SelectorCellChekedEventArgs e)
        {
            try
            {
                //if (this.chkAllDeploy.IsChecked == true) { return; }

                ////this.CurrPartRefList.Where(p => p.CLS_DEV == "A").ForEach(p => p.IsSelected = false);
                //this.g_liDeployFileInfoList.Where(p => p.COMPARE_RESULT.Length == 0 && p.FILE_EXTENSION.Equals(".config") == false).ToList().ForEach(p => p.IsSelected = false);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
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
        // ~C1007()
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
