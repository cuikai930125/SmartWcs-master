using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.COMMON.Views.GANTRY;
using SMART.WCS.UI.Processing.DataMembers.R1008_GAN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.Processing.Views.GANTRY
{
    /// <summary>
    /// Processing > Smart Gantry > 재고현황조회
    /// </summary>
    public partial class R1008_GAN : UserControl
    {
        #region ▩ Detegate 선언
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
        /// Header 클릭에 따른 관련 정보 수집
        /// </summary>
        private List<string> headerSource = new List<string>();
        #endregion

        #region ▩ Detegate 선언
        #region > 메인화면 하단 좌측 상태바 값 반영
        public delegate void ToolStripStatusEventHandler(string value);
        public event ToolStripStatusEventHandler ToolStripChangeStatusLabelEvent;
        #endregion
        #endregion

        #region > 그리드 -  리스트
        public static readonly DependencyProperty StockListProperty
            = DependencyProperty.Register("StockList", typeof(ObservableCollection<StockInfo>), typeof(R1008_GAN)
                , new PropertyMetadata(new ObservableCollection<StockInfo>()));

        /// <summary>
        /// 재고리스트
        /// </summary>
        public ObservableCollection<StockInfo> StockList
        {
            get { return (ObservableCollection<StockInfo>)GetValue(StockListProperty); }
            set { SetValue(StockListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(R1008_GAN), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string GridRowCount
        {
            get { return (string)GetValue(GridRowCountProperty); }
            set { SetValue(GridRowCountProperty, value); }
        }
        #endregion

        public static readonly DependencyProperty _StockHistList
            = DependencyProperty.Register("StockHistList", typeof(ObservableCollection<StockHistInfo>), typeof(R1008_GAN)
                , new PropertyMetadata(new ObservableCollection<StockHistInfo>()));

        /// <summary>
        /// 재고리스트
        /// </summary>
        public ObservableCollection<StockHistInfo> StockHistList
        {
            get { return (ObservableCollection<StockHistInfo>)GetValue(_StockHistList); }
            set { SetValue(_StockHistList, value); }
        }
        #endregion

        bool FirstLoad = true;

        #region ▩ 생성자
        public R1008_GAN()
        {
            InitializeComponent();
        }

        /// <summary>
        /// R1008_GAN - 재고현황조회
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public R1008_GAN(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

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

        #region ▩ 함수

        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            // 콤보박스 - 공통코드
            this.BaseClass.BindingCommonComboBox(this.cboCellType, "CELL_TYPE_CD", null, true);

            this.BaseClass.BindingCommonComboBox(this.cboGanStockWrkTypeCd, "GAN_STOCK_WRK_TYPE_CD", null, true);

            // 콤보박스 - 설비정보
            this.GetEqpInfoData();
        }

        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += R1008_GAN_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            #region 재고현황
            // 팝업
            this.btnPopup.PreviewMouseLeftButtonUp += BtnPopup_PreviewMouseLeftButtonUp;
            // 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            #endregion


            #region 재고처리이력
            // 조회
            this.btnSearch_Second.PreviewMouseLeftButtonUp += BtnSearch_Second_PreviewMouseLeftButtonUp; ;
            // 엑셀 다운로드
            this.btnExcelDownload_Second.PreviewMouseLeftButtonUp += BtnExcelDownload_Second_PreviewMouseLeftButtonUp;
            #endregion


            #endregion

        }

        private DataSet GetSP_STOCK_HIST_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_R1008_GAN.SP_STOCK_HIST_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                       // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                        // 센터 코드
            var strEqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);            // 설비ID
            var strFrYmd = this.BaseClass.GetCalendarValue(this.deFrWrkYmd);                                     // 작업 시작 일자
            var strToYmd = this.BaseClass.GetCalendarValue(this.deToWrkYmd);                                     // 작업 종료 일자
            var strWrkType = this.BaseClass.ComboBoxSelectedKeyValue(this.cboGanStockWrkTypeCd);  // 셀타입 코드

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);              // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpId);                // 설비ID
            dicInputParam.Add("P_FR_YMD", strFrYmd);              // 셀ID
            dicInputParam.Add("P_TO_YMD", strToYmd);              // 셀ID
            dicInputParam.Add("P_WRK_TYPE", strWrkType);     // 셀타입 코드
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }




        #region R1008_GAN_Loaded
        private void R1008_GAN_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstLoad)
                {
                    FirstLoad = false;
                    StockListSearch();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region  +  팝업 버튼 클릭 이벤트
        private void BtnPopup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var EqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);

                if (string.IsNullOrEmpty(EqpId) || EqpId.Equals(""))    // "전체"
                {
                    this.BaseClass.MsgError("설비를 선택하세요.", BaseEnumClass.CodeMessage.MESSAGE);

                    return;
                }

                C1022_GAN_01P frmChild = new C1022_GAN_01P(EqpId);
                frmChild.Owner = Window.GetWindow(this);
                frmChild.Closed += FrmChild_Closed;
                frmChild.ShowDialog();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #endregion

        #region +  조회 버튼 클릭 이벤트
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StockListSearch();
        }

        private void BtnSearch_Second_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StockHistListSearch();
        }

        #endregion

        #region +  엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        ///  엑셀 다운로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strMessage = this.BaseClass.GetResourceValue("ASK_EXCEL_DOWNLOAD", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    // 상태바 (아이콘) 실행
                    this.loadingScreen.IsSplashScreenShown = true;

                    List<TableView> tv = new List<TableView>();

                    tv.Add(this.tvMasterGrid);

                    this.BaseClass.GetExcelDownload(tv);
                }
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

        private void BtnExcelDownload_Second_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strMessage = this.BaseClass.GetResourceValue("ASK_EXCEL_DOWNLOAD", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    // 상태바 (아이콘) 실행
                    this.loadingScreen.IsSplashScreenShown = true;

                    List<TableView> tv = new List<TableView>();

                    tv.Add(this.tvMasterGrid_Second);

                    this.BaseClass.GetExcelDownload(tv);
                }
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

        #endregion

        private void FrmChild_Closed(object sender, EventArgs e)
        {
            StockListSearch();
        }

        private void StockListSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_STOCK_SEARCH();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.StockList = new ObservableCollection<StockInfo>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.StockList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.StockList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.StockList;

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

                if (this.StockList.Count == 0)
                {
                    this.BaseClass.MsgInfo("INFO_NOT_INQ");
                }
            }
        }

        private DataSet GetSP_STOCK_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_R1008_GAN.SP_STOCK_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                       // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                        // 센터 코드
            var strEqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);            // 설비ID
            var strCellId = string.Empty;                                                   // 셀ID
            var strCellTypeCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCellType);  // 셀타입 코드

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);              // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpId);                // 설비ID
            dicInputParam.Add("P_CELL_ID", strCellId);              // 셀ID
            dicInputParam.Add("P_CELL_TYPE_CD", strCellTypeCd);     // 셀타입 코드
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }

        private void StockHistListSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_STOCK_HIST_SEARCH();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.StockHistList = new ObservableCollection<StockHistInfo>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.StockHistList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.StockList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.StockList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResult2Text();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;

                if (this.StockList.Count == 0)
                {
                    this.BaseClass.MsgInfo("INFO_NOT_INQ");
                }
            }
        }

        private void GetEqpInfoData()
        {
            DataTable dtComboData = null;

            var strProcedureName = "PK_C1022_GAN.SP_GET_EQP_INFO";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;       // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;        // 센터 코드

            dicInputParam.Add("P_CO_CD", strCoCd);
            dicInputParam.Add("P_CNTR_CD", strCntrCd);

            using (BaseDataAccess da = new BaseDataAccess())
            {
                dtComboData = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }

            List<ComboBoxInfo> ComboBoxInfo = new List<ComboBoxInfo>();

            foreach (DataRow citem in dtComboData.Rows)
            {
                ComboBoxInfo.Add(new ComboBoxInfo { CODE = citem["EQP_ID"].ToString(), NAME = citem["EQP_NM"].ToString() });
            }

            // 바인딩 데이터가 있는 경우 첫번째 Row를 선택하도록 한다.
            if (ComboBoxInfo.Count > 0)
            {
                ComboBoxInfo.Insert(0, new ComboBoxInfo { CODE = " ", NAME = BaseClass.GetAllValueByLanguage() });  // 전체
            }

            this.cboEqp.ItemsSource = ComboBoxInfo;
            this.cboEqp.SelectedIndex = 0;

            this.cboEqp_Second.ItemsSource = ComboBoxInfo;
            this.cboEqp_Second.SelectedIndex = 0;
        }

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

        private void SetResult2Text()
        {
            var strResource = string.Empty;                                                           // 텍스트 리소스 (전체 데이터 수)
            var iTotalRowCount = 0;                                                                   // 조회 데이터 수

            strResource = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                            // 텍스트 리소스
            iTotalRowCount = (this.gridMaster_Second.ItemsSource as ICollection).Count;                      // 전체 데이터 수
            this.GridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                       // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion
        #endregion

        #endregion

        #region ▩ 이벤트
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
