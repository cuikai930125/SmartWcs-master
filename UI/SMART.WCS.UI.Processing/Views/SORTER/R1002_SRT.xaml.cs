using DevExpress.Xpf.Grid;

using Newtonsoft.Json;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.Processing.DataMembers.R1002_SRT;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SMART.WCS.UI.Processing.Views.SORTER
{
    /// <summary>
    /// R1002_SRT.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class R1002_SRT : UserControl
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
        /// 조회한 EQP_ID 저장
        /// </summary>
        private string SearchedEQPId = null;
        #endregion

        #region ▩ 생성자
        public R1002_SRT()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public R1002_SRT(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                //this.BaseInfo = ((SMART.WCS.Control.BaseApp)Application.Current).BASE_INFO;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

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

        #region ▩ Enum
        private enum JsonWebServiceSendResult
        {
            NOT_SEND        = 0,
            ERROR           = 1,
            SEND            = 2
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(R1002_SRT), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 그리드 - 스캔 현황 리스트
        public static readonly DependencyProperty ScanResultMgmtListProperty
            = DependencyProperty.Register("ScanResultMgmtList", typeof(ObservableCollection<ScanResultMgmt>), typeof(R1002_SRT)
                , new PropertyMetadata(new ObservableCollection<ScanResultMgmt>()));

        private ObservableCollection<ScanResultMgmt> ScanResultMgmtList
        {
            get { return (ObservableCollection<ScanResultMgmt>)GetValue(ScanResultMgmtListProperty); }
            set { SetValue(ScanResultMgmtListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(R1002_SRT), new PropertyMetadata(string.Empty));

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
            this.BaseClass.BindingCommonComboBox(this.cboEqpId, "EQP_ID", commonParam_EQP_ID, false);
            this.BaseClass.BindingCommonComboBox(this.cboSrtErrCd, "SRT_ERR_CD", null, true);
            this.BaseClass.BindingCommonComboBox(this.cboZoneID, "ZONE_ID", null, true);                // ZONE ID
            this.BaseClass.BindingCommonComboBox(this.cboIFTypeYN, "IF_USE_YN", null, true);            // I/F 여부

            // 날짜, 시간 컨트롤 기본값 설정
            this.BaseClass.AutoSetDateTimeByCenter(this.FromScanDT, this.FromScanTM, this.ToScanDT, this.ToScanTM);
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += R1002_SRT_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 조회
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_First_PreviewMouseLeftButtonUp;
            // 재처리버튼 클릭
            this.btnReProcess_First.PreviewMouseLeftButtonUp += BtnReProcess_First_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            #endregion
        }
        #endregion
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
            iTotalRowCount = (this.gridMaster.ItemsSource as ICollection).Count;                      // 전체 데이터 수
            this.GridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                       // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion

        #region >> CheckGridRowSelected - 그리드 체크박스 선택 유효성 체크
        /// <summary>
        /// 그리드 체크박스 선택 유효성 체크
        /// </summary>
        /// <returns></returns>
        private bool CheckGridRowSelected()
        {
            try
            {
                bool bRtnValue = true;
                string strMessage = string.Empty;
                int iCheckedCount = 0;

                iCheckedCount = this.ScanResultMgmtList.Where(p => p.IsSelected == true).Count();

                if (iCheckedCount == 0)
                {
                    // ERR_NO_SELECT - 선택된 데이터가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT");

                    bRtnValue = false;
                }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 데이터 관련

        #region >> GetSP_RGN_LIST_INQ - 스캔 현황 조회
        /// <summary>
        /// 스캔 현황 조회
        /// </summary>
        private DataSet GetSP_SCAN_RSLT_INQ(string SearchedEQPId)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_R1002_SRT.SP_SCAN_RSLT_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_SRT_BOX_LIST", "O_RSLT" };

            var strFromScanDt       = this.FromScanDT.DateTime.ToString("yyyyMMdd");
            var strFromScanTm       = this.BaseClass.ReplaceText(this.FromScanTM, ":", string.Empty);
            var strToScanDt         = this.ToScanDT.DateTime.ToString("yyyyMMdd");
            var strToScanTm         = this.BaseClass.ReplaceText(this.ToScanTM, ":", string.Empty);

            var strCntrCd       = this.BaseClass.CenterCD;                                      // 센터코드
            var strEqpId        = SearchedEQPId;                                                // 설비 ID
            var strFromScan     = strFromScanDt + strFromScanTm;                                // 스캔일자 (From)
            var strToScan       = strToScanDt + strToScanTm;                                    // 스캔일자 (To)
            var strRsltChute    = this.txtRsltChute.Text.Trim();                                // 슈트 (실적슈트)
            var strBoxBcd       = this.txtBoxBcd.Text.Trim();                                   // 박스바코드
            var strSrtErrCd     = this.BaseClass.ComboBoxSelectedKeyValue(this.cboSrtErrCd);    // 소터오류코드
            var strInvBcd       = this.txtInvBcd.Text.Trim();                                   // 송장바코드
            var strZoneID       = this.BaseClass.ComboBoxSelectedKeyValue(this.cboZoneID);      // ZONE ID
            var strIFTypeYN     = this.BaseClass.ComboBoxSelectedKeyValue(this.cboIFTypeYN);    // I/F 여부

            var strErrCode = string.Empty;                                                  // 오류 코드
            var strErrMsg = string.Empty;                                                   // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCntrCd);     // 센터코드
            dicInputParam.Add("P_EQP_ID",           strEqpId);      // 설비 ID
            dicInputParam.Add("P_FM_SCAN_DT",       strFromScan);   // 스캔일자 (From)
            dicInputParam.Add("P_TO_SCAN_DT",       strToScan);     // 스캔일자 (To)
            dicInputParam.Add("P_RSLT_CHUTE_ID",    strRsltChute);  // 슈트 (실적슈트)
            dicInputParam.Add("P_BOX_BCD",          strBoxBcd);     // 박스바코드
            dicInputParam.Add("P_SRT_ERR_CD",       strSrtErrCd);   // 소터오류코드
            dicInputParam.Add("P_INV_BCD",          strInvBcd);     // 송장바코드
            dicInputParam.Add("P_ZONE_ID",          strZoneID);     // ZONE ID
            dicInputParam.Add("P_IF_YN",            strIFTypeYN);   // I/F 여부
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

        #region >> SaveSP_IF_SRT_RSLT_SND_UPD - 전송 결과 업데이트
        /// <summary>
        /// 전송 결과 업데이트
        /// </summary>
        /// <param name="_enumSendResult">Json 웹서비스 호출 후 수신 결과</param>
        /// <param name="_strPid">PID</param>
        /// <returns></returns>
        private void SaveSP_IF_SRT_RSLT_SND_UPD(JsonWebServiceSendResult _enumSendResult, string _strPid, string _strIndtYmdHms)
        {
            try
            {
                string strSendResult    = string.Empty;

                switch (_enumSendResult)
                {
                    case JsonWebServiceSendResult.NOT_SEND:
                        // 미전송
                        strSendResult = "N";
                        //// ERR_SEND_FAIL : 데이터 전송이 실패했습니다.
                        //this.BaseClass.MsgError("ERR_SEND_FAIL");
                        break;
                    case JsonWebServiceSendResult.ERROR:
                        // 오류
                        strSendResult = "E";
                        //// 데이터 수신측에서 오류를 반환했습니다.
                        //this.BaseClass.MsgError("ERR_RECEIVE_PART_ERR");
                        break;
                    case JsonWebServiceSendResult.SEND:
                        // 전송 (성공)
                        strSendResult = "Y";
                        break;
                }

                this.BaseClass.Info("ERROR : " + strSendResult);

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_IF_SRT_RSLT_HUB_S.SP_IF_SRT_RSLT_SND_UPD";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strCntrCd       = this.BaseClass.CenterCD;                                  // 센터코드
                var strEqpID        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqpId);   // 설비 ID
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTR_CD",          strCntrCd);                 // 센터코드
                dicInputParam.Add("P_EQP_ID",           strEqpID);                  // 설비 ID
                dicInputParam.Add("P_INDT_YMD_HMS",     _strIndtYmdHms);
                dicInputParam.Add("P_PID",              _strPid);                   // PID
                dicInputParam.Add("P_SND_YN",           strSendResult);             // 전송결과
                dicInputParam.Add("P_USER_ID",          this.BaseClass.UserID);     // 사용자 ID
                #endregion

                #region 데이터 저장
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dtRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
                }
                #endregion

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                        {
                            // CMPT_SEND : 재처리가 완료되었습니다.
                            //this.BaseClass.MsgInfo("CMPT_RESEND_PROC");
                            var strMessage  = this.BaseClass.GetResourceValue("CMPT_RESEND_PROC");
                            this.BaseClass.Info($"{strMessage}");
                        }
                    }
                    else
                    {
                        //this.BaseClass.MsgInfo("ERR_SAVE"); //CMPT_SAVE : 저장 중 오류가 발생했습니다.
                        var strErrMessage   = this.BaseClass.GetResourceValue("ERR_SAVE");
                        this.BaseClass.Info($"{strErrMessage}");
                    }
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion
        
        #endregion

        #region ▩ 이벤트

        #region > Loaded 이벤트
        private void R1002_SRT_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 스캔 현황

        #region >> 버튼 클릭 이벤트

        #region + 스캔 현황 조회버튼 클릭 이벤트
        /// <summary>
        /// 권역 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SearchedEQPId = null;
            SearchedEQPId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqpId);

            try
            {
                var iDiffDay = this.BaseClass.CheckDateTerm(this.FromScanDT, this.ToScanDT, BaseEnumClass.SystemCode.PCS);
                if (iDiffDay < 0)
                {
                    // ERR_SEARCH_COND_TERM - 일자 조건을 {0}일 이내로 설정하셔야 합니다.
                    this.BaseClass.MsgError("ERR_COND_TERM", this.BaseClass.ProcInqTerm.ToString());
                    return;
                }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_SCAN_RSLT_INQ(SearchedEQPId);

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.ScanResultMgmtList = new ObservableCollection<ScanResultMgmt>();
                    // 오라클인 경우 TableName = TB_SRT_BOX_RSLT
                    this.ScanResultMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ScanResultMgmtList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // PLAN_CHUTE 처리하기 (PLAN_CHUTE_ID1 + ';' + PLAN_CHUTE_ID2 + ';' + PLAN_CHUTE_ID3)

                // 소터오류코드 == "20" 인 경우 Row 색상 변경
                this.ScanResultMgmtList.Where(p => p.ERR_CODE == "20").ToList().ForEach(f => f.ForegroundBrush = new SolidColorBrush(Colors.Red));

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.ScanResultMgmtList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText();
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

        #region + 스캔 현황 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 슈트 관리 엑셀 다운로드 버튼 클릭 이벤트
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

        #region >> 재처리 버튼 클릭 이벤트
        /// <summary>
        /// 재처리 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReProcess_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                // ASK_REPROS - 재처리를 수행하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_REPROS");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                var selectedItems = this.ScanResultMgmtList.Where(p => p.IsSelected).ToList();

                // 부천 URL
                string strPostSortingUrl_BC     = "https://api-gateway.coupang.net/v2/providers/hub_api/apis/api/v1/sorters/BCSMS-2/sorting";
                // 양산 URL
                string strPostSortingUrl_YS     = "https://api-gateway.coupang.net/v2/providers/hub_api/apis/api/v1/sorters/YSSMS-1/sorting";
                string strSendURL               = this.BaseClass.CenterCD.Equals("BC") ? strPostSortingUrl_BC : strPostSortingUrl_YS;

                foreach (var item in selectedItems)
                {   
                    try
                    {
                        #region 파라메터 변수 선언 및 값 할당
                        DataSet dsRtnValue                          = null;
                        var strProcedureName                        = "PK_IF_SRT_RSLT_HUB_S.SP_IF_SRT_RSLT_INQ";
                        Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                        string[] arrOutputParam                     = { "O_IF_SND_LIST", "O_RSLT" };

                        var strCntrCd           = this.BaseClass.CenterCD;      // 센터코드
                        string strEqpID         = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqpId);   // 설비 ID
                        var strIndtYmdHms       = item.INDT_YMD_HMS;
                        var strPID              = item.PID;
                        #endregion

                        #region Input 파라메터
                        Dictionary<string, object> dicInputType     = new Dictionary<string, object>();
                        dicInputParam.Add("P_CNTR_CD",              strCntrCd);
                        dicInputParam.Add("P_EQP_ID",               strEqpID);
                        dicInputParam.Add("P_INDT_YMD_HMS",         strIndtYmdHms);
                        dicInputParam.Add("P_PID",                  strPID);
                        #endregion

                        using (BaseDataAccess da = new BaseDataAccess())
                        {
                            dsRtnValue = da.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);

                            foreach (DataRow drRow in dsRtnValue.Tables[0].Rows)
                            {
                                ReProcess model             = new ReProcess();
                                model.sortingId             = drRow["EQP_ID"].ToString();
                                model.trayCode              = drRow["CART_NO"].ToString();
                                model.invoiceNumber         = drRow["INV_BCD"].ToString();
                                model.boxCode               = drRow["BOX_BCD"].ToString();
                                model.sortingCode           = drRow["RGN_BCD"].ToString();
                                model.scanTime              = drRow["SCAN_DT"].ToString();
                                model.sortTime              = drRow["SRT_WRK_CMPT_DT"].ToString();
                                model.chuteNumber           = drRow["RSLT_CHUTE_ID"].ToString();
                                model.turnNumber            = Convert.ToInt32(drRow["RECIRC_CNT"]);
                                model.errorCode             = drRow["STR_ERR_CD"].ToString();
                                model.imagePath             = string.Empty;

                                var strJsonData             = JsonConvert.SerializeObject(model);
                                var strResult               = this.PostSendJson(strSendURL, strJsonData);

                                JsonWebServiceSendResult enumSendResult = strResult.ToUpper().Equals("OK") == true ? JsonWebServiceSendResult.SEND : JsonWebServiceSendResult.NOT_SEND;

                                this.SaveSP_IF_SRT_RSLT_SND_UPD(enumSendResult, item.PID, item.INDT_YMD_HMS);
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        this.BaseClass.Error(err);
                        this.BaseClass.Info("오류발생:Update 구문 수행");
                        this.SaveSP_IF_SRT_RSLT_SND_UPD(JsonWebServiceSendResult.ERROR, item.PID, item.INDT_YMD_HMS);
                    }
                }

                // CMPT_SEND : 재처리가 완료되었습니다.
                this.BaseClass.MsgInfo("CMPT_RESEND_PROC");

                // 셀 유형관리 데이터 조회
                DataSet dsSearch = this.GetSP_SCAN_RSLT_INQ(SearchedEQPId);

                if (dsSearch == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsSearch, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.ScanResultMgmtList = new ObservableCollection<ScanResultMgmt>();
                    // 오라클인 경우 TableName = TB_SRT_BOX_RSLT
                    this.ScanResultMgmtList.ToObservableCollection(dsSearch.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ScanResultMgmtList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // PLAN_CHUTE 처리하기 (PLAN_CHUTE_ID1 + ';' + PLAN_CHUTE_ID2 + ';' + PLAN_CHUTE_ID3)

                // 소터오류코드 == "20" 인 경우 Row 색상 변경
                this.ScanResultMgmtList.Where(p => p.SRT_RSN_CD == "20").ToList().ForEach(f => f.ForegroundBrush = new SolidColorBrush(Colors.Red));

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.ScanResultMgmtList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText();
            }
            catch (Exception err)
            {
                this.BaseClass.MsgError($"재처리 중 오류가 발생했습니다.\r\n{err.ToString()}");
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 그리드 관련 이벤트

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

        #region > PostSendJson - Post Json 
        /// <summary>
        /// Post Json 
        /// </summary>
        /// <param name="_strUrl">접속 Url</param>
        /// <param name="_strParamValue">Parameter값</param>
        /// <returns></returns>
        //public HttpWebResponse PostSendJson(string _strUrl, string _strParamValue)
        public string PostSendJson(string _strUrl, string _strParamValue)
        {
            try
            {
                HttpWebResponse response;

                // 헤더키
                var strHeaderTokenKey = BaseClass.DecryptAES256(BaseClass.GetAppSettings("HeaderKey"));
                // 헤더값
                var strHeaderTokenValue = BaseClass.DecryptAES256(BaseClass.GetAppSettings("HeaderValue"));

                HttpWebRequest request  = (HttpWebRequest)WebRequest.Create(_strUrl);
                request.ContentType     = "application/json";
                request.Method          = "POST";
                request.Headers.Add(strHeaderTokenKey, strHeaderTokenValue);

                using (StreamWriter stream = new StreamWriter(request.GetRequestStream()))
                {
                    stream.Write(_strParamValue);
                    stream.Flush();
                    stream.Close();
                }

                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding("UTF-8"), true))
                        {
                            //var strRtnValue = reader.ReadToEnd();
                            //strRtnValue = JsonConvert.DeserializeObject(strRtnValue).ToString();
                            //this.BaseClass.Info($"Post : {strRtnValue}");
                           
                            return response.StatusCode.ToString();
                        }
                    }
                }
            }
            catch (Exception err) { this.BaseClass.Error(err); throw; }
        }
        #endregion
    }
}
