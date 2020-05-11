using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.Processing.DataMembers.R1003_SRT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SMART.WCS.UI.Processing.Views.SORTER
{
    /// <summary>
    /// R1003_SRT.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class R1003_SRT : UserControl
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
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// Base Info 선언
        /// </summary>
        private BaseInfo BaseInfo = new BaseInfo();

        /// <summary>
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        /// <summary>
        /// 조회조건 중 소터 저장
        /// </summary>
        private string searchedSRT = null;

        /// <summary>
        /// Header 클릭에 따른 관련 정보 수집
        /// </summary>
        private List<string> headerSource = new List<string>();
        #endregion

        #region ▩ 생성자
        public R1003_SRT()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public R1003_SRT(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
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
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의

        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(R1003_SRT), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 그리드 - 공통코드관리 헤더 리스트
        public static readonly DependencyProperty ChuteResultHeaderListProperty
            = DependencyProperty.Register("ChuteResultHeaderList", typeof(ObservableCollection<ChuteResultHeader>), typeof(R1003_SRT)
                , new PropertyMetadata(new ObservableCollection<ChuteResultHeader>()));

        private ObservableCollection<ChuteResultHeader> ChuteResultHeaderList
        {
            get { return (ObservableCollection<ChuteResultHeader>)GetValue(ChuteResultHeaderListProperty); }
            set { SetValue(ChuteResultHeaderListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty HeaderGridRowCountProperty
            = DependencyProperty.Register("HeaderGridRowCount", typeof(string), typeof(R1003_SRT), new PropertyMetadata(string.Empty));

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

        #region > 그리드 - 공통코드관리 디테일 리스트
        public static readonly DependencyProperty ChuteResultDetailListProperty
            = DependencyProperty.Register("ChuteResultDetailList", typeof(ObservableCollection<ChuteResultDetail>), typeof(R1003_SRT)
                , new PropertyMetadata(new ObservableCollection<ChuteResultDetail>()));

        private ObservableCollection<ChuteResultDetail> ChuteResultDetailList
        {
            get { return (ObservableCollection<ChuteResultDetail>)GetValue(ChuteResultDetailListProperty); }
            set { SetValue(ChuteResultDetailListProperty, value); }
        }

        #region >> Detail Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty DetailGridRowCountProperty
            = DependencyProperty.Register("DetailGridRowCount", typeof(string), typeof(R1003_SRT), new PropertyMetadata(string.Empty));

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
            // 공통코드 조회 파라메터 string[]
            string[] commonParam_EQP_ID = { BaseClass.CenterCD, "SRT", BaseClass.UserID, string.Empty };

            // 콤보박스 - 조회 (설비 ID)
            this.BaseClass.BindingCommonComboBox(this.CboSrt, "EQP_ID", commonParam_EQP_ID, false);

            // 소트오류코드
            this.BaseClass.BindingCommonComboBox(this.cboSrtErrCD, "SRT_ERR_CD", null, true);

            // ZONE ID
            this.BaseClass.BindingCommonComboBox(this.cboZoneID, "ZONE_ID", null, true);

            // 날짜, 시간 컨트롤 기본값 설정
            this.BaseClass.AutoSetDateTimeByCenter(this.FromDT, this.FromTM, this.ToDT, this.ToTM);
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 버튼 클릭 이벤트
            // 엑셀 다운로드
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_First_PreviewMouseLeftButtonUp;
            // 조회
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_First_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 클릭 이벤트
            // Header 그리드 클릭 이벤트 - Detail 조회
            this.gridMasterHeader.PreviewMouseLeftButtonUp += DetailSearch_PreviewMouseLeftButtonUp;
            #endregion
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

        #region >>GetSP_CHUTE_RSLT_INQ - 분류실적 Header 조회
        /// <summary>
        /// 분류실적 Header 조회
        /// </summary>
        /// <returns></returns>
        private DataSet GetSP_CHUTE_RSLT_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_R1003_SRT.SP_CHUTE_RSLT_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_CHUTE_BOX_LIST", "O_RSLT" };

            var strCntrCd           =  this.BaseClass.CenterCD;                                     // 센터코드
            var strSrt              = this.BaseClass.ComboBoxSelectedKeyValue(this.CboSrt);         // 소터
            var strFromScanDt       = this.FromDT.DateTime.ToString("yyyyMMdd");                    // 일자 - FromDate
            var strFromScanTm       = this.BaseClass.ReplaceText(this.FromTM, ":", string.Empty);   // 일자 - FromTime
            var strToScanDt         = this.ToDT.DateTime.ToString("yyyyMMdd");                      // 일자 - ToDate
            var strToScanTm         = this.BaseClass.ReplaceText(this.ToTM, ":", string.Empty);     // 일자 - ToTime
            var strRsltChute        = this.txtRsltChute.Text.Trim();                                // 실적슈트
            var strSrtErrCD         = this.BaseClass.ComboBoxSelectedKeyValue(this.cboSrtErrCD);    // 소트오류코드
            var strZoneID           = this.BaseClass.ComboBoxSelectedKeyValue(this.cboZoneID);      // ZONE ID
            
            var strErrCode          = string.Empty;     // 오류 코드
            var strErrMsg           = string.Empty;     // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCntrCd);         // 센터코드
            dicInputParam.Add("P_EQP_ID",           strSrt);            // 소터
            dicInputParam.Add("P_FM_SCAN_YMD",      strFromScanDt);     // 일자 - FromDate
            dicInputParam.Add("P_FM_SCAN_HM",       strFromScanTm);     // 일자 - FromTime
            dicInputParam.Add("P_TO_SCAN_YMD",      strToScanDt);       // 일자 - ToDate
            dicInputParam.Add("P_TO_SCAN_HM",       strToScanTm);       // 일자 - ToTime
            dicInputParam.Add("P_RSLT_CHUTE_ID",    strRsltChute);      // 실적슈트
            dicInputParam.Add("P_SRT_ERR_CD",       strSrtErrCD);       // 소트오류코드
            dicInputParam.Add("P_ZONE_ID",          strZoneID);         // ZONE ID
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

        #region >> GetSP_CHUTE_RSLT_DTL_INQ - 분류실적 Detail 조회
        private DataSet GetSP_CHUTE_RSLT_DTL_INQ(List<string> headerSource)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_R1003_SRT.SP_CHUTE_RSLT_DTL_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_SRT_BOX_DTL_LIST", "O_RSLT" };

            var strCntrCd = this.BaseClass.CenterCD;                                // 센터코드
            var strSrt = headerSource[0];                                           // 소터 (From.Header)
            var strScanDt = headerSource[1];                                        // 일자
            var strPlanChute = headerSource[2];                                     // 계획슈트
            var strRsltChute = headerSource[3];                                     // 실적슈트
            var strIndtId = headerSource[4];                                        // 인덕션
            var strRgnBcd = headerSource[5];                                        // 권역
            var strSrtErrCd = headerSource[6];                                      // 에러사유
            var strSrtRsnCd = headerSource[7];                                      // 설비사유
            var strSrtWrkStatCd = headerSource[8];                                 // 상태

            var strErrCode = string.Empty;                                          // 오류 코드
            var strErrMsg = string.Empty;                                           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                              // 센터코드
            dicInputParam.Add("P_EQP_ID", strSrt);                                  // 소터 (From.Header)
            dicInputParam.Add("P_SCAN_YMD", strScanDt);                             // 일자
            dicInputParam.Add("P_PLAN_CHUTE_ID1", strPlanChute);                    // 계획슈트
            dicInputParam.Add("P_RSLT_CHUTE_ID", strRsltChute);                     // 실적슈트
            dicInputParam.Add("P_INDT_ID", strIndtId);                              // 인덕션
            dicInputParam.Add("P_RGN_BCD", strRgnBcd);                              // 권역
            dicInputParam.Add("P_SRT_ERR_CD", strSrtErrCd);                         // 에러사유
            dicInputParam.Add("P_SRT_RSN_CD", strSrtRsnCd);                         // 설비사유
            dicInputParam.Add("P_SRT_WRK_STAT_CD", strSrtWrkStatCd);                // 상태
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

        #endregion

        #endregion

        #region ▩ 이벤트

        #region > 분류실적

        #region >> 버튼 클릭 이벤트

        #region + 조회버튼 클릭 이벤트 - 분류실적 헤더 조회
        /// <summary>
        /// 조회버튼 클릭 이벤트 - 분류실적 헤더 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            searchedSRT = null;
            searchedSRT = this.BaseClass.ComboBoxSelectedKeyValue(this.CboSrt);

            try
            {
                var iDiffDay = this.BaseClass.CheckDateTerm(this.FromDT, this.ToDT, BaseEnumClass.SystemCode.PCS);
                if (iDiffDay < 0)
                {
                    // ERR_SEARCH_COND_TERM - 일자 조건을 {0}일 이내로 설정하셔야 합니다.
                    this.BaseClass.MsgError("ERR_COND_TERM", this.BaseClass.ProcInqTerm.ToString());
                    return;
                }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 공통 코드 관리 헤더 리스트 조회
                DataSet dsRtnValue = this.GetSP_CHUTE_RSLT_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.ChuteResultHeaderList = new ObservableCollection<ChuteResultHeader>();
                    // 오라클인 경우 TableName = TB_SRT_BOX_RSLT
                    this.ChuteResultHeaderList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ChuteResultHeaderList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 소터오류코드 == "20" 인 경우 Row 색상 변경
                //this.ChuteResultHeaderList.Where(p => p.CODE == "20").ToList().ForEach(f => f.ForegroundBrush = new SolidColorBrush(Colors.Red));

                for (int i = 0; i < this.ChuteResultHeaderList.Count; i++)
                {
                    if (this.ChuteResultHeaderList[i].SRT_ERR_CD.Equals("20") == true)
                    {
                        this.ChuteResultHeaderList[i].ForegroundBrush = new SolidColorBrush(Colors.Red);
                    }
                }
                
                // 조회 데이터를 그리드에 바인딩
                this.gridMasterHeader.ItemsSource = this.ChuteResultHeaderList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText_Header();
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

        #region + 엑셀 다운로드 버튼 클릭 이벤트
            /// <summary>
            /// 엑셀 다운로드 버튼 클릭 이벤트
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void BtnExcelDownload_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

        #endregion

        #region >> 그리드 클릭 이벤트

        #region + 분류실적 헤더 그리드 클릭 이벤트 - 분류실적 디테일 조회
        /// <summary>
        /// 분류실적 헤더 그리드 클릭 이벤트 - 분류실적 디테일 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var view = (sender as GridControl).View as TableView;
            var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

            if (hi.InRowCell)
            {
                headerSource.Clear();

                int clicked = hi.RowHandle;

                // 조회 조건 중 클릭한 소터
                headerSource.Add(searchedSRT);

                // 스캔일자 (string 분리해서 가져오기)
                //var objScanDt = gridMasterHeader.GetCellValue(clicked, gridMasterHeader.Columns[0]);
                //string strScanDt = Convert.ToString(objScanDt);
                //string strYY = strScanDt.Substring(0, 4);
                //string strMM = strScanDt.Substring(5, 2);
                //string strDD = strScanDt.Substring(8, 2);
                //string scanDt = strYY + strMM + strDD;
                //headerSource.Add(scanDt);

                // 나머지 클릭한 셀 내용
                for (int i = 0; i <= 5; i++)
                {
                    var obj = gridMasterHeader.GetCellValue(clicked, gridMasterHeader.Columns[i]);
                    string obj_str = Convert.ToString(obj);
                    headerSource.Add(obj_str);
                }

                var temp = gridMasterHeader.GetCellValue(clicked, gridMasterHeader.Columns[7]);
                string temp_str = Convert.ToString(temp);
                headerSource.Add(temp_str);

                temp = this.ChuteResultHeaderList[clicked].SRT_WRK_STAT_CD;
                temp_str = Convert.ToString(temp);
                headerSource.Add(temp_str);

                try
                {
                    // 상태바 (아이콘) 실행
                    this.loadingScreen.IsSplashScreenShown = true;

                    // Detail 목록 조회
                    DataSet dsRtnValue = this.GetSP_CHUTE_RSLT_DTL_INQ(headerSource);

                    if (dsRtnValue == null) { return; }

                    var strErrCode = string.Empty;      //오류 코드
                    var strErrMsg = string.Empty;      // 오류 메세지

                    if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                    {
                        // 정상 처리된 경우
                        this.ChuteResultDetailList = new ObservableCollection<ChuteResultDetail>();
                        // 오라클인 경우 TableName = TB_SRT_BOX_RSLT
                        this.ChuteResultDetailList.ToObservableCollection(dsRtnValue.Tables[0]);
                    }
                    else
                    {
                        // 오류가 발생한 경우
                        this.ChuteResultDetailList.ToObservableCollection(null);
                        BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                    }

                    // 조회 데이터를 그리드에 바인딩
                    this.gridMasterDetail.ItemsSource = this.ChuteResultDetailList;


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
        #endregion

        #region + 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        private static void View_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            if (g_IsAuthAllYN == false)
            {
                e.Cancel = true;
                return;
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
            this.TreeControlRefreshEvent();
        }
        #endregion

        #endregion
    }
}
