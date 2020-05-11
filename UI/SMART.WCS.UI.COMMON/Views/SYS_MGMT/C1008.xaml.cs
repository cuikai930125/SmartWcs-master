using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using Newtonsoft.Json;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.COMMON.DataMembers.C1008;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.COMMON.Views.SYS_MGMT
{
    /// <summary>
    /// C1008.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1008 : UserControl
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
        #region > Json 인터페이스 호출 URL
        private const string URL_GET_BARCODE            = "https://api-gateway.coupang.net/v2/providers/hub_api/apis/api/v1/sorters/BCSMS-2/barcode/definition";
        private const string URL_GET_CONFIGURATION      = "https://api-gateway.coupang.net/v2/providers/hub_api/apis/api/v1/sorters/BCSMS-2/configuration";
        private const string URL_POST_SORTING           = "https://api-gateway.coupang.net/v2/providers/hub_api/apis/api/v1/sorters/BCSMS-2/sorting";
        #endregion

        /// <summary>
        /// Base 클래서 선언
        /// </summary>
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        /// <summary>
        /// Header 클릭에 따른 관련 정보 수집
        /// </summary>
        private string headerSource = string.Empty;
        #endregion

        #region  ▩ Enum
        private enum JsonWebServiceSendResult
        {
            NOT_SEND        = 0,
            ERROR           = 1,
            SEND            = 2
        }
        #endregion

        #region ▩ 생성자
        public C1008()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1008(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                //// 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                //// 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 상단에 설명 
                this.CommentArea.ScreenID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();

                //  공통코드를 사용하지 않는 콤보박스 설정
                this.InitComboBoxInfo();
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1008), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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
                //view.ShowingEditor += View_ShowingEditor;
            }
        }
        #endregion

        #region > 그리드 - Batch Job 모니터링 헤더 리스트
        public static readonly DependencyProperty HeaderBatchJobMonitoringListProperty
            = DependencyProperty.Register("HeaderBatchJobMonitoringList", typeof(ObservableCollection<HeaderBatchJobMonitoring>), typeof(C1008)
                , new PropertyMetadata(new ObservableCollection<HeaderBatchJobMonitoring>()));

        private ObservableCollection<HeaderBatchJobMonitoring> HeaderBatchJobMonitoringList
        {
            get { return (ObservableCollection<HeaderBatchJobMonitoring>)GetValue(HeaderBatchJobMonitoringListProperty); }
            set { SetValue(HeaderBatchJobMonitoringListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty HeaderGridRowCountProperty
            = DependencyProperty.Register("HeaderGridRowCount", typeof(string), typeof(C1008), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string HeaderGridRowCount
        {
            get { return (string)GetValue(HeaderGridRowCountProperty); }
            set { SetValue(HeaderGridRowCountProperty, value); }
        }
        #endregion
        #endregion

        #region > 그리드 - Batch Job 모니터링 디테일 리스트
        public static readonly DependencyProperty DetailBatchJobMonitoringListProperty
            = DependencyProperty.Register("DetailBatchJobMonitoringList", typeof(ObservableCollection<DetailBatchJobMonitoring>), typeof(C1008)
                , new PropertyMetadata(new ObservableCollection<DetailBatchJobMonitoring>()));

        private ObservableCollection<DetailBatchJobMonitoring> DetailBatchJobMonitoringList
        {
            get { return (ObservableCollection<DetailBatchJobMonitoring>)GetValue(DetailBatchJobMonitoringListProperty); }
            set { SetValue(DetailBatchJobMonitoringListProperty, value); }
        }

        #region >> Detail Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty DetailGridRowCountProperty
            = DependencyProperty.Register("DetailGridRowCount", typeof(string), typeof(C1008), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string DetailGridRowCount
        {
            get { return (string)GetValue(DetailGridRowCountProperty); }
            set { SetValue(DetailGridRowCountProperty, value); }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            // 콤보박스 - 조회 (배치 JOB 구분, JOB_ID, 결과)
            this.BaseClass.BindingCommonComboBox(this.cboBtchJobType, "BTCH_JOB_TYPE_CD", null, false);
            this.BaseClass.BindingCommonComboBox(this.cboSuccType, "SUCC_TYPE", null, true);

            // 공통코드 조회 파라메터 string[]
            string[] commonParam_EQP_ID = { BaseClass.CenterCD, "SRT", BaseClass.UserID, string.Empty };

            // 콤보박스
            this.BaseClass.BindingCommonComboBox(this.CboEqpId, "EQP_ID", commonParam_EQP_ID, false);//설비 ID
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        private void InitEvent()
        {
            #region + Loaded 이벤트
            //this.Loaded += C1010_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // Header 조회
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_Header_PreviewMouseLeftButtonUp;
            // Header 및 Detail 엑셀 다운로드
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            // 재처리 버튼 클릭
            this.btnReProcess_First.PreviewMouseLeftButtonUp += BtnReProcess_First_PreviewMouseLeftButtonUp;
            #endregion

            #region + 콤보박스 클릭 이벤트
            // Batch Job 구분 ComboBox 클릭 -> Batch Job 구분 선택 (-> Job ID로 Batch Job Type 데이터 전달)
            this.cboBtchJobType.EditValueChanged += CboBtchJobType_EditValueChanged;
            #endregion

            #region + 그리드 이벤트
            // Header 그리드 클릭 이벤트 - Detail 조회
            this.gridMasterHeader.PreviewMouseLeftButtonUp += DetailSearch_PreviewMouseLeftButtonUp;

            #endregion
        }
        #endregion

        #region >> InitComboBoxInfo - 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// <summary>
        /// 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// </summary>
        private void InitComboBoxInfo()
        {
            var strBtchJobType = this.BaseClass.ComboBoxSelectedKeyValue(this.cboBtchJobType);              // Batch Job 구분
            string[] commonParam_JOB_ID = { strBtchJobType, string.Empty, string.Empty, string.Empty };
            this.BaseClass.BindingCommonComboBox(this.cboJobId, "JOB_ID", commonParam_JOB_ID, false);
        }
        #endregion
        
        #endregion

        #region > 기타 함수

        #region >> SetResultText_Header - Header 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// Header 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        private void SetResultText_Header()
        {
            var strResource = string.Empty;                                                           // 텍스트 리소스 (전체 데이터 수)
            var iTotalRowCount = 0;                                                                   // 조회 데이터 수
            var strGridTitle = "[Header] ";                                                           // Grid 종류 - Header

            strResource = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                            // 텍스트 리소스
            iTotalRowCount = (this.gridMasterHeader.ItemsSource as ICollection).Count;                // 전체 데이터 수
            this.HeaderGridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                 // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.

            this.ToolStripChangeStatusLabelEvent($"{strGridTitle}{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion

        #region >> SetResultText_Detail - Detail 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// Header 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        private void SetResultText_Detail()
        {
            var strResource = string.Empty;                                                           // 텍스트 리소스 (전체 데이터 수)
            var iTotalRowCount = 0;                                                                   // 조회 데이터 수
            var strGridTitle = "[Detail] ";                                                           // Grid 종류 - Detail

            strResource = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                            // 텍스트 리소스
            iTotalRowCount = (this.gridMasterDetail.ItemsSource as ICollection).Count;                // 전체 데이터 수
            this.DetailGridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                 // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.

            this.ToolStripChangeStatusLabelEvent($"{strGridTitle}{iTotalRowCount.ToString()}{strResource}");
        }
        #endregion
        
        #endregion

        #region > 데이터 관련

        #region >> GetSP_BTCH_JOB_HDR_LIST_INQ - Batch Header 리스트 조회
        /// <summary>
        /// Common Code Header List 조회
        /// </summary>
        private DataSet GetSP_BTCH_JOB_HDR_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1008.SP_BTCH_JOB_HDR_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_BTCH_JOB_HDR_LIST", "O_RSLT" };

            var strCntrCd           = this.BaseClass.CenterCD;                                                    // 센터코드
            var strBtchJobType      = this.BaseClass.ComboBoxSelectedKeyValue(this.cboBtchJobType);          // Batch Job 구분
            var strJobId            = this.BaseClass.ComboBoxSelectedKeyValue(this.cboJobId);                      // Job ID
            var strFromDate         = this.FromYmd.DateTime.ToString("yyyyMMdd");                               // 기간(To)
            var strToDate           = this.ToYmd.DateTime.ToString("yyyyMMdd");                                   // 기간(From)
            var strSuccType         = this.BaseClass.ComboBoxSelectedKeyValue(this.cboSuccType);                // 결과

            var strErrCode      = string.Empty;          // 오류 코드
            var strErrMsg       = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCntrCd);             // 센터코드
            dicInputParam.Add("P_BTCH_JOB_TYPE_CD",     strBtchJobType);        // Batch Job 구분
            dicInputParam.Add("P_JOB_ID",               strJobId);              // Job ID
            dicInputParam.Add("P_JOB_FR_YMD",           strFromDate);           // 기간(To)
            dicInputParam.Add("P_JOB_TO_YMD",           strToDate);             // 기간(From)
            dicInputParam.Add("P_PROC_CD",              strSuccType);           // 결과
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion

        #region >> GetSP_BTCH_JOB_DTL_LIST_INQ - Batch Detail 리스트 조회 SP (Header 더블 클릭 시)
        /// <summary>
        /// Common Code Detail List 조회 SP
        /// </summary>
        /// <param name="_strHeaderJobNo">Job 번호</param>
        /// <param name="_strHeaderJobID">Job ID</param>
        /// <returns></returns>
        private DataSet GetSP_BTCH_JOB_DTL_LIST_INQ(string _strHeaderJobNo, string _strHeaderJobID)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1008.SP_BTCH_JOB_DTL_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_BTCH_JOB_DTL_LIST", "O_RSLT" };

            var strCntrCd       = this.BaseClass.CenterCD;          // 센터코드
            var strHdrJobNo     = _strHeaderJobNo;                  // Job 번호
            var strHdrJobID     = _strHeaderJobID;                  // Job ID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",  strCntrCd);         // 센터코드
            dicInputParam.Add("P_JOB_NO",   strHdrJobNo);       // Job 번호
            dicInputParam.Add("P_JOB_ID",   strHdrJobID);       // Job ID
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion

        #region >> GetIF_RGN_MST_HUB_R - 바코드 수신값을 테이블에 저장
        /// <summary>
        /// 바코드 수신값을 테이블에 저장
        /// </summary>
        /// <param name="_barcode">웹서비스를 통해 수신한 바코드 (Validation 조건)</param>
        /// <returns></returns>
        private DataTable GetIF_RGN_MST_HUB_R(Barcode _barcode)
        { 
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "SP_IF_REGEX_MST_HUB_R";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RTN_MSG" };
                #endregion

                var strEqpID = this.BaseClass.CenterCD.Equals("BC") ? "BCSMS-2" : "YSSMS-1";

                #region Input 파라메터
                dicInputParam.Add("P_EQP_ID",               strEqpID);
                dicInputParam.Add("P_INV_NO_REGEX",         _barcode.INVOICE_NUM);  
                dicInputParam.Add("P_BOX_BCD_REGEX",        _barcode.BOX_CODE);     
                dicInputParam.Add("P_RGN_BCD_REGEX",        _barcode.SORTING_CODE);
                #endregion

                using (FirstDataAccess dataAccess = new FirstDataAccess())
                {
                    dtRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
                }

                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> GetSP_IF_PROC_CHK - 인터페이스 받은 데이터 저장 수행 여부 체크
        /// <summary>
        /// 인터페이스 받은 데이터 저장 수행 여부 체크
        /// </summary>
        /// <param name="_strSorterID">소터 ID</param>
        /// <param name="_strVersion">버전</param>
        /// <param name="_strInterfaceNo">인터페이스 번호</param>
        /// <returns></returns>
        private bool GetSP_IF_PROC_CHK(string _strSorterID, string _strVersion, string _strInterfaceNo)
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                bool isRtnValue                             = false;
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_IF_CHUTE_PLAN_HUB_R.SP_IF_PROC_CHK";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_IF_PROC_YN", "O_RSLT" };
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_IF_NO",                _strInterfaceNo);
                dicInputParam.Add("P_EQP_ID",               _strSorterID);
                dicInputParam.Add("P_VER",                  _strVersion);
                #endregion

                //#region 데이터 조회
                using (FirstDataAccess dataAccess = new FirstDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }

                if (dsRtnValue.Tables.Count > 0)
                {
                    if (dsRtnValue.Tables[0].Rows.Count > 0)
                    {
                        isRtnValue = dsRtnValue.Tables[0].Rows[0]["PROC_YN"].Equals("Y") ? true : false;
                    }
                }

                return isRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> GetSP_IF_NO_INQ - 인터페이스 번호 채번
        /// <summary>
        /// 인터페이스 번호 채번
        /// </summary>
        /// <returns></returns>
        private string GetSP_IF_NO_INQ()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                string strRtnValue                          = string.Empty;
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_IF_CHUTE_PLAN_HUB_R.SP_IF_NO_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_IF_NO", "O_RSLT" };
                #endregion

                //#region 데이터 조회
                using (FirstDataAccess dataAccess = new FirstDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }

                if (dsRtnValue.Tables[0].Rows.Count > 0)
                {
                    strRtnValue = dsRtnValue.Tables[0].Rows[0]["IF_NO"].ToString();
                }

                return strRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> SaveSP_IF_CHUTE_PLAN_HUB_R - 수신한 슈트 플랜 데이터 저장
        /// <summary>
        /// 수신한 슈트 플랜 데이터 저장
        /// </summary>
        /// <param name="_sorter"></param>
        /// <returns></returns>
        private DataTable SaveSP_IF_CHUTE_PLAN_HUB_R(Sorter _sorter)
        {
            try
            {
                // 인터페이스 번호 체번
                var strInterfaceNo = this.GetSP_IF_NO_INQ();

                // 수행 여부 체크
                bool isProCYN   = this.GetSP_IF_PROC_CHK(_sorter.sorterId, _sorter.version, strInterfaceNo);

                if (isProCYN == false) { return null; }

                if (strInterfaceNo.Length == 0) { return null; }

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_IF_CHUTE_PLAN_HUB_R.SP_IF_CHUTE_PLAN_HUB_R";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };
                #endregion

                #region Input 파라메터
                foreach (var item in _sorter.rules)
                {
                    try
                    { 
                        dicInputParam.Add("P_IF_NO",        strInterfaceNo);        // Interface 번호
                        dicInputParam.Add("P_EQP_ID",       _sorter.sorterId);      // 설비 ID
                        dicInputParam.Add("P_SRT_MODE",     _sorter.sortingMode);
                        dicInputParam.Add("P_VER",          _sorter.version);
                        dicInputParam.Add("P_CHUTE_ID",     item.chuteNumber);
                        dicInputParam.Add("P_RGN_CD",       item.sortingCode);
                        dicInputParam.Add("P_RGN_CD_LGCY",  item.sortingCodeLegacy);

                        using (FirstDataAccess dataAccess = new FirstDataAccess())
                        {
                            dtRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
                        }
                    }
                    catch (Exception err) 
                    {
                        this.BaseClass.Error(err);
                    }
                }
                #endregion

                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> GetSP_IF_SRT_RSLT_INQ - 재처리 리스트 조회
        /// <summary>
        /// 재처리 리스트 조회
        /// </summary>
        /// <returns></returns>
        private async Task<DataSet> GetSP_IF_SRT_RSLT_INQ()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_IF_SRT_RSLT_HUB_S.SP_IF_SRT_RSLT_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_IF_SND_LIST", "O_RSLT" };

                var strCntrCd       = this.BaseClass.CenterCD;                                  // 센터코드
                var strEqpID        = this.BaseClass.ComboBoxSelectedKeyValue(this.CboEqpId);   // 설비 ID
                var strCurrentDate  = DateTime.Now.ToString("yyyyMMdd");                        // 현재일자
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTR_CD",          strCntrCd);         // 센터코드
                dicInputParam.Add("P_EQP_ID",           strEqpID);          // 설비 ID
                dicInputParam.Add("P_INDT_YMD_HMS",     strCurrentDate);    // 현재일자
                dicInputParam.Add("P_PID",              string.Empty);      // PID
                #endregion

                #region 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    await System.Threading.Tasks.Task.Run(() =>
                    {
                        dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                    }).ConfigureAwait(true);
                }
                #endregion

                return dsRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> SaveSP_IF_SRT_RSLT_SND_UPD - 전송 결과 업데이트
        /// <summary>
        /// 전송 결과 업데이트
        /// </summary>
        /// <param name="_enumSendResult">Json 웹서비스 호출 후 수신 결과</param>
        /// <returns></returns>
        private async Task SaveSP_IF_SRT_RSLT_SND_UPD(JsonWebServiceSendResult _enumSendResult)
        {
            try
            {
                string strSendResult                        = string.Empty;

                switch (_enumSendResult)
                {
                    case JsonWebServiceSendResult.NOT_SEND:
                        // 미전송
                        strSendResult = "N";
                        // ERR_SEND_FAIL : 데이터 전송이 실패했습니다.
                        this.BaseClass.MsgError("ERR_SEND_FAIL");
                        break;
                    case JsonWebServiceSendResult.ERROR:
                        // 오류
                        strSendResult = "E";
                        // 데이터 수신측에서 오류를 반환했습니다.
                        this.BaseClass.MsgError("ERR_RECEIVE_PART_ERR");
                        break;
                    case JsonWebServiceSendResult.SEND:
                        // 전송 (성공)
                        strSendResult = "Y";
                        break;
                }

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_IF_SRT_RSLT_HUB_S.SP_IF_RSLT_SND_UPD";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_IF_SND_LIST", "O_RSLT" };

                var strCntrCd       = this.BaseClass.CenterCD;                                  // 센터코드
                var strEqpID        = this.BaseClass.ComboBoxSelectedKeyValue(this.CboEqpId);   // 설비 ID
                var strCurrentDate  = DateTime.Now.ToString("yyyyMMdd");                        // 현재일자
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTR_CD",          strCntrCd);                 // 센터코드
                dicInputParam.Add("P_EQP_ID",           strEqpID);                  // 설비 ID
                dicInputParam.Add("P_INDT_YMD_HMS",     strCurrentDate);            // 현재일자
                dicInputParam.Add("P_PID",              string.Empty);              // PID
                dicInputParam.Add("P_SND_YN",           strSendResult);             // 전송결과
                dicInputParam.Add("P_USER_ID",          this.BaseClass.UserID);     // 사용자 ID
                #endregion

                #region 데이터 저장
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    await System.Threading.Tasks.Task.Run(() =>
                    {
                        dtRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
                    }).ConfigureAwait(true);
                }
                #endregion

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                        {
                            // CMPT_SEND : 재처리가 완료되었습니다.
                            this.BaseClass.MsgInfo("CMPT_RESEND_PROC");
                        }
                    }
                    else
                    {
                        this.BaseClass.MsgInfo("ERR_SAVE"); //CMPT_SAVE : 저장 중 오류가 발생했습니다.
                    }
                }
            }
            catch { throw; }
        }
        #endregion

        #endregion
        #endregion

        #region ▩ 이벤트

        #region > Batch Job 모니터링

        #region >> 버튼 클릭 이벤트

        #region + Batch Job 모니터링 Header 리스트 조회 클릭 이벤트
        /// <summary>
        /// Batch Job 모니터링 Header 리스트 조회 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_Header_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // Batch Job 모니터링 헤더 리스트 조회
                DataSet dsRtnValue = this.GetSP_BTCH_JOB_HDR_LIST_INQ();
                
                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.HeaderBatchJobMonitoringList = new ObservableCollection<HeaderBatchJobMonitoring>();
                    // 오라클인 경우 TableName = TB_COM_BTCH_WRK_LOG_HDR
                    this.HeaderBatchJobMonitoringList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.HeaderBatchJobMonitoringList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }
                
                // 조회 데이터를 그리드에 바인딩
                this.gridMasterHeader.ItemsSource = this.HeaderBatchJobMonitoringList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText_Header();

                #region 첫 번째 Row의 Detail 리스트 조회

                // 첫 번째 Row Data 가져오기
                var strHeaderJobNo      = Convert.ToString(gridMasterHeader.GetCellValue(0, gridMasterHeader.Columns[1]));
                var strHeaderJobID      = Convert.ToString(gridMasterHeader.GetCellValue(0, gridMasterHeader.Columns[3]));

                // Batch Job 모니터링 디테일 리스트 조회
                DataSet dsRtnValue_DTL = this.GetSP_BTCH_JOB_DTL_LIST_INQ(strHeaderJobNo, strHeaderJobID);

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue_DTL, ref strErrCode, ref strErrMsg) == true)
                {
                    this.DetailBatchJobMonitoringList = new ObservableCollection<DetailBatchJobMonitoring>();
                    this.DetailBatchJobMonitoringList.ToObservableCollection(dsRtnValue_DTL.Tables[0]);
                }
                else
                {
                    this.DetailBatchJobMonitoringList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                this.gridMasterDetail.ItemsSource = this.DetailBatchJobMonitoringList;

                #endregion
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }

        }
        #endregion

        #region + Batch Job 모니터링 Header/Detail 리스트 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 설비 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // ASK_EXCEL_DOWNLOAD - 엑셀 다운로드를 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EXCEL_DOWNLOAD");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                List<TableView> tv = new List<TableView>();
                tv.Add(this.tvMasterGrid);
                tv.Add(this.tvDetailGrid);
                this.BaseClass.GetExcelDownload(tv);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        #region PostResultUploadAPI - 
        private async Task PostResultUploadAPI()
        {
            try
            {
                // 설비 ID 콤보박스 선택 여부 체크


                // 재처리 리스트 조회
                DataSet dsRtnValue = await this.GetSP_IF_SRT_RSLT_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;     // 오류 코드
                var strErrMsg = string.Empty;     // 오류 메세지

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 조회된 경우
                    // 결과 업로드 I/F 호출
                    foreach (DataRow drRow in dsRtnValue.Tables[1].Rows)
                    {
                        //var strEqpID = drRow["EQP_ID"].ToString();       // 설비 ID (sorterId)

                        ReProcess modelReProcess        = new ReProcess();
                        modelReProcess.sortingId        = string.Empty;
                        modelReProcess.trayCode         = drRow["CART_NO"].ToString();
                        modelReProcess.invoiceNumber    = drRow["INV_BCD"].ToString();
                        modelReProcess.boxCode          = drRow["BOX_BCD"].ToString();
                        modelReProcess.scanTime         = Convert.ToDateTime(drRow["SCAN_DT"]).ToString("yyyyMMddHHmmss");
                        modelReProcess.sortTime         = Convert.ToDateTime(drRow["SRT_WRK_CMPT_DT"]).ToString("yyyyMMddHHmmss");
                        modelReProcess.chuteNumber      = drRow["RSLT_CHUTE_ID"].ToString();
                        modelReProcess.turnNumber       = 0;    // Convert.ToInt32(drRow["RECIRC_CNT"]);
                        modelReProcess.errorCode        = drRow["STR_ERR_CD"].ToString();
                        modelReProcess.imagePath        = string.Empty;

                        //var strURL          = $"https://api-gateway.coupang.net/v2/providers/hub_api/apis/api/v1/sorters/{ strEqpID}/sorting";
                        //var strJsonData     = JsonConvert.SerializeObject(modelReProcess);

                        //var strRtnValue     = this.BaseClass.PostSendJson(strURL, strJsonData);
                        //BaseResponse resp   = JsonConvert.DeserializeObject<BaseResponse>(strRtnValue);

                        //#region 샘플
                        //var strCode         = resp.code;
                        //var strDesc         = resp.descryption;
                        //#endregion

                        var strJsonData = JsonConvert.SerializeObject(modelReProcess);
                        HttpWebResponse response = SMART.WCS.UI.COMMON.Utility.HelperClass.PostSendJson(URL_POST_SORTING, strJsonData);
                        var strResult = response.StatusCode.ToString();

                        JsonWebServiceSendResult enumSendRsult = strResult.ToUpper().Equals("OK") == true ? JsonWebServiceSendResult.SEND : JsonWebServiceSendResult.NOT_SEND;

                        await this.SaveSP_IF_SRT_RSLT_SND_UPD(enumSendRsult);
                    }
                }
                else
                {
                    // 오류가 발생한 경우
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }
            catch
            {
                await this.SaveSP_IF_SRT_RSLT_SND_UPD(JsonWebServiceSendResult.NOT_SEND);
                throw;
            }
        }
        #endregion

        #region + 재처리 버튼 클릭 이벤트
        /// <summary>
        /// 재처리 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnReProcess_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // ASK_REPROS - 재처리를 수행하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_REPROS");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                var strSelectedComboValue   = this.BaseClass.ComboBoxSelectedKeyValue(this.cboJobId);   // JOB ID
                var strEqpID                = this.BaseClass.ComboBoxSelectedKeyValue(this.CboEqpId);   // 설비 ID
                string strResponseValue     = string.Empty;

                switch (strSelectedComboValue)
                {
                    case "BCD_RULE_R":
                        #region +  바코드 체계수신
                        this.BaseClass.Info("바코드 수신 시작");
                        strResponseValue    = Utility.HelperClass.GetSendJson(URL_GET_BARCODE);
                        Barcode bcd         = JsonConvert.DeserializeObject<Barcode>(strResponseValue);
                        this.BaseClass.Info("바코드 수신 종료");

                        this.BaseClass.Info("바코드 수신 데이터 저장 시작");
                        this.GetIF_RGN_MST_HUB_R(bcd);
                        this.BaseClass.Info("바코드 수신 데이터 저장 종료");

                        #endregion
                        break;

                    case "CHUTE_PLAN_R":
                        #region + 슈트플랜수신
                        this.BaseClass.Info("슈트플랜 수신 시작");
                        strResponseValue = Utility.HelperClass.GetSendJson(URL_GET_CONFIGURATION);
                        Sorter sorter = JsonConvert.DeserializeObject<Sorter>(strResponseValue);
                        this.BaseClass.Info("슈트플랜 수신 종료");

                        this.BaseClass.Info("슈트플랜 수신 데이터 저장 시작");
                        this.SaveSP_IF_CHUTE_PLAN_HUB_R(sorter);
                        this.BaseClass.Info("슈트플랜 수신 데이터 저장 종료");
                        #endregion
                        break;

                    case "SORT_RSLT_S":
                        #region + 분류실적송신
                        await this.PostResultUploadAPI();
                        #endregion
                        break;
                }

                // 재처리가 완료되었습니다.
                this.BaseClass.MsgInfo("CMPT_REPROS");
            }
            catch (Exception err)
            {
                // 재처리 중 오류가 발생했습니다.
                var strMessage = this.BaseClass.GetResourceValue("ERR_REPROS");
                this.BaseClass.MsgError($"{strMessage} \r\n{err.Message}");
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion
        #endregion

        #region >> 콤보박스 클릭 이벤트

        #region + Batch Job Type 콤보박스 클릭 이벤트
        #region >> 달력 컨트롤 변경 이벤트
        /// <summary>
        /// 달력 컨트롤 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CboBtchJobType_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            try
            {
                this.InitComboBoxInfo();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #endregion

        #region >> 그리드 클릭 이벤트

        #region > Header 리스트 클릭 이벤트 (Batch Detail 리스트 조회)
        private void DetailSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view = (sender as GridControl).View as TableView;
                var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hi.InRowCell)
                {
                    // 클릭한 행의 JOB_ID 가져오기
                    int clicked     = hi.RowHandle;
                    var obj_nm      = gridMasterHeader.GetCellValue(clicked, gridMasterHeader.Columns[1]);
                    var obj_JobID   = this.gridMasterHeader.GetCellValue(clicked, gridMasterHeader.Columns[3]);
                    string headerJobNo  = Convert.ToString(obj_nm);
                    var headerJobId     = Convert.ToString(obj_JobID);

                    headerSource = headerJobNo;

                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        // Detail 목록 조회
                        DataSet dsRtnValue = this.GetSP_BTCH_JOB_DTL_LIST_INQ(headerJobNo, headerJobId);

                        if (dsRtnValue == null) { return; }

                        var strErrCode = string.Empty;      //오류 코드
                        var strErrMsg = string.Empty;      // 오류 메세지

                        if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                        {
                            // 정상 처리된 경우
                            this.DetailBatchJobMonitoringList = new ObservableCollection<DetailBatchJobMonitoring>();
                            // 오라클인 경우 TableName = TB_COM_BTCH_RSK_LOG_DTL
                            this.DetailBatchJobMonitoringList.ToObservableCollection(dsRtnValue.Tables[0]);
                        }
                        else
                        {
                            // 오류가 발생한 경우
                            this.DetailBatchJobMonitoringList.ToObservableCollection(null);
                            BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                        }

                        // 조회 데이터를 그리드에 바인딩
                        this.gridMasterDetail.ItemsSource = this.DetailBatchJobMonitoringList;


                        // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                        this.SetResultText_Detail();
                    }
                    catch (Exception err)
                    {
                        this.BaseClass.Error(err);
                    }
                    finally
                    {
                        this.loadingScreen.IsSplashScreenShown = false;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #endregion
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
    }
}