using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.DataMembers;
using SMART.WCS.UI.Plan.DataMembers.P1007_GAN;
using SMART.WCS.UI.Plan.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.Plan.Views.GANTRY
{
    /// <summary>
    /// Plan > Smart Gantry > 출고 계획관리
    /// </summary>
    public partial class P1007_GAN : UserControl
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

        #region > 화면에서 메뉴 여는 기능
        public delegate void SelectedMenuOpenEventHandler(MainWinParam _liValue);
        public event SelectedMenuOpenEventHandler SelectedMenuOpenEvent;
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
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의

        #region >> IsColumnChecked - 체크 컬럼 헤더 체크 여부
        public static readonly DependencyProperty IsColumnCheckedProperty
            = DependencyProperty.Register("IsColumnChecked", typeof(bool), typeof(P1007_GAN)
                , new PropertyMetadata(false));

        public bool IsColumnChecked
        {
            get { return (bool)GetValue(IsColumnCheckedProperty); }
            set { SetValue(IsColumnCheckedProperty, value); }
        }
        #endregion

        #region >> WrkPlanList - 출고 계획 리스트
        public static readonly DependencyProperty WrkPlanListProperty
            = DependencyProperty.Register("WrkPlanList", typeof(ObservableCollection<WrkPlanInfo>), typeof(P1007_GAN)
                , new PropertyMetadata(new ObservableCollection<WrkPlanInfo>()));

        public ObservableCollection<WrkPlanInfo> WrkPlanList
        {
            get { return (ObservableCollection<WrkPlanInfo>)GetValue(WrkPlanListProperty); }
            set { SetValue(WrkPlanListProperty, value); }
        }
        #endregion

        #region >> Grid Row수 (GridRowCount)
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(P1007_GAN),
                new PropertyMetadata(string.Empty));

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

        bool FirstLoad = true;

        #region ▩ 생성자
        public P1007_GAN()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public P1007_GAN(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                this.BaseInfo = ((SMART.WCS.Control.BaseApp)Application.Current).BASE_INFO;

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
            this.BaseClass.BindingCommonComboBox(this.cboWorkState, "WRK_STAT_CD", null, true);
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += P1007_GAN_Loaded;
            #endregion

            this.KeyDown += P1007_GAN_KeyDown;

            #region + 버튼 클릭 이벤트
            // 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 배치생성
            this.btnBtchCreate.PreviewMouseLeftButtonUp += BtnBtchCreate_PreviewMouseLeftButtonUp;
            // PLAN 수립
            this.btnPlanCreate.PreviewMouseLeftButtonUp += BtnPlanCreate_PreviewMouseLeftButtonUp;
            // 배치취소
            this.btnBtchCancel.PreviewMouseLeftButtonUp += BtnBtchCancel_PreviewMouseLeftButtonUp;
            // 배치종료
            this.btnBtchClose.PreviewMouseLeftButtonUp += BtnBtchClose_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 이벤트
            // Equipment 리스트 그리드 순번 채번을 위한 이벤트
            this.gridMaster.CustomUnboundColumnData += GridMaster_CustomUnboundColumnData;

            //
            this.tvMasterGrid.CellValueChanged += TvMasterGrid_CellValueChanged;
            #endregion
        }

        private void P1007_GAN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.LeftCtrl))
            {
                this.btnSearch.IsEnabled = true;
                this.btnBtchCreate.IsEnabled = true;
                this.btnPlanCreate.IsEnabled = true;
                this.btnBtchCancel.IsEnabled = true;
                this.btnExcelDownload.IsEnabled = true;
            }
        }

        /// <summary>
        /// TvMasterGrid_CellValueChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvMasterGrid_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            this.WrkPlanList.ForEach(p => p.ClearError());
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
                int iCheckedCount = 0;

                iCheckedCount = this.WrkPlanList.Where(p => p.IsSelected == true).Count();

                if (iCheckedCount == 0)
                {
                    bRtnValue = false;
                    BaseClass.MsgError("ERR_NO_SELECT");
                }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #endregion

        #region > 데이터 관련

        #region >> GetSP_COMM_TOTE_SEARCH - 출고계획 리스트 조회
        /// <summary>
        /// 데이터조회
        /// </summary>
        private DataSet GetSP_WRK_PLAN_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P1007_GAN.SP_WRK_PLAN_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                               // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            var strFrOrdYmd = this.BaseClass.GetCalendarValue(this.deFrOrdYmd);                     // 시작 오더일자
            var strToOrdYmd = this.BaseClass.GetCalendarValue(this.deToOrdYmd);                     // 종료 오더일자
            var strBtchNo = this.txtBatchNo.Text.Trim();                                            // 배치번호
            var strWrkStatCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboWorkState);          // 작업상태코드

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);              // 센터 코드
            dicInputParam.Add("P_FR_ORD_YMD", strFrOrdYmd);         // 시작 오더일자
            dicInputParam.Add("P_TO_ORD_YMD", strToOrdYmd);         // 종료 오더일자
            dicInputParam.Add("P_BTCH_NO", strBtchNo);              // 배치번호
            dicInputParam.Add("P_WRK_STAT_CD", strWrkStatCd);       // 작업상태코드
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

        #region >> SetSP_WRK_BTCH_CONFIRM - 배치컨펌

        private bool SetSP_WRK_BTCH_CONFIRM(BaseDataAccess da, string pBtchNo)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_P1007_GAN.SP_WRK_BTCH_CONFIRM";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUT_RESULT" };

            var strCoCd = BaseClass.CompanyCode;                    // 회사 코드
            var strCntrCd = BaseClass.CenterCD;                     // 센터 코드           
            var strBtchNo = pBtchNo;                                // 배치번호
            var strUserID = this.BaseClass.UserID;                  // 사용자 ID

            var strErrCode = string.Empty;                         // 오류 코드
            var strErrMsg = string.Empty;                          // 오류 메세지
            #endregion

            #region Input 파라메터     
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);              // 센터 코드
            dicInputParam.Add("P_BTCH_NO", strBtchNo);              // 배치번호
            dicInputParam.Add("P_USER_ID", strUserID);              // 사용자 ID
            #endregion

            dtRtnValue = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        BaseClass.MsgInfo(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    BaseClass.MsgError("ERR_SAVE"); // 저장실패? 배치취소실패?
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }
        #endregion

        #region >> SetSP_WRK_BTCH_CANCEL - 배치취소

        private bool SetSP_WRK_BTCH_CANCEL(BaseDataAccess da, string pBtchNo)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_P1007_GAN.SP_WRK_BTCH_CANCEL";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUT_RESULT" };

            var strCoCd = BaseClass.CompanyCode;                    // 회사 코드
            var strCntrCd = BaseClass.CenterCD;                     // 센터 코드           
            var strBtchNo = pBtchNo;                                // 배치번호
            var strUserID = this.BaseClass.UserID;                  // 사용자 ID

            var strErrCode = string.Empty;                         // 오류 코드
            var strErrMsg = string.Empty;                          // 오류 메세지
            #endregion

            #region Input 파라메터     
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);              // 센터 코드
            dicInputParam.Add("P_BTCH_NO", strBtchNo);              // 배치번호
            dicInputParam.Add("P_USER_ID", strUserID);              // 사용자 ID
            #endregion

            dtRtnValue = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        BaseClass.MsgInfo(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    BaseClass.MsgError("ERR_SAVE"); // 저장실패? 배치취소실패?
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }
        #endregion

        #region >> SetSP_WRK_BTCH_CLOSE - 배치종료
        private bool SetSP_WRK_BTCH_CLOSE(BaseDataAccess da, string pBtchNo)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_P1007_GAN.SP_WRK_BTCH_CLOSE";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUT_RESULT" };

            var strCoCd = BaseClass.CompanyCode;                    // 회사 코드
            var strCntrCd = BaseClass.CenterCD;                     // 센터 코드           
            var strBtchNo = pBtchNo;                                // 배치번호
            var strUserID = this.BaseClass.UserID;                  // 사용자 ID

            var strErrCode = string.Empty;                         // 오류 코드
            var strErrMsg = string.Empty;                          // 오류 메세지
            #endregion

            #region Input 파라메터     
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);              // 센터 코드
            dicInputParam.Add("P_BTCH_NO", strBtchNo);              // 배치번호
            dicInputParam.Add("P_USER_ID", strUserID);              // 사용자 ID
            #endregion

            dtRtnValue = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        BaseClass.MsgInfo(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    BaseClass.MsgError("ERR_SAVE"); // 저장실패? 배치취소실패?
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }
        #endregion

        #endregion

        #endregion

        #region ▩ 이벤트

        #region > Loaded 이벤트
        private void P1007_GAN_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstLoad)
                {
                    FirstLoad = false;
                    WrkPlanListSearch();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 
        #region >> 버튼 클릭 이벤트
        #region +  조회버튼 클릭 이벤트
        /// <summary>
        ///  조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            #region 주석
            //var ChangedRowData = this.WrkPlanList.Where(p => p.IsSelected == true).ToList();

            //if (ChangedRowData.Count > 0)
            //{
            //    var strMessage = this.BaseClass.GetResourceValue("ASK_EXISTS_NO_SAVE_TO_SEARCH", BaseEnumClass.ResourceType.MESSAGE);

            //    this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
            //    if (this.BaseClass.BUTTON_CONFIRM_YN == true)
            //    {
            //        WrkPlanListSearch();
            //    }
            //}
            //else
            //{
            //    WrkPlanListSearch();
            //}
            #endregion

            WrkPlanListSearch();

            //
            //CallR1007_GAN();

        }

        /// <summary>
        /// 출고계획 리스트 조회
        /// </summary>
        private void WrkPlanListSearch()
        {
            try
            {
                this.IsColumnChecked = false;

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_WRK_PLAN_SEARCH();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.WrkPlanList = new ObservableCollection<WrkPlanInfo>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.WrkPlanList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.WrkPlanList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.WrkPlanList;

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

                if (this.WrkPlanList.Count == 0)
                {
                    this.BaseClass.MsgInfo("INFO_NOT_INQ");
                }

                ButtonEnableSet(false, false, false, false);
            }
        }
        #endregion

        /// <summary>
        /// 배치생성
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBtchCreate_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isRtnValue = false;

                var SelectedRow = this.WrkPlanList.Where(w => w.IsSelected == true && string.IsNullOrEmpty(w.BTCH_NO)).ToList();

                if (SelectedRow.Count > 0)
                {
                    this.BaseClass.MsgQuestion("ASK_CHECK_WRK_BTCH_CREATE", SelectedRow.Count.ToString());
                    if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                    {
                        using (BaseDataAccess da = new BaseDataAccess())
                        {
                            try
                            {
                                // 상태바 (아이콘) 실행
                                this.loadingScreen.IsSplashScreenShown = true;

                                da.BeginTransaction();

                                // TODO : 배치번호 따야하는데 어디서 뽑아서 어떻게 던질지 고민 중(?)
                                //SetSP_WRK_BTCH_CREATE();
                                var strBtchNo = this.GetSP_WRK_BTCH_CREATE(da);

                                //PK_P1007.SP_WRK_BTCH_CREATE 호출 후 배치를번호 RETURN 후 
                                //ORDER 단위로 SP_WRK_BTCH_UPDATE 를 호출하며 배치번호 UPDATE 처리

                                foreach (var item in SelectedRow)
                                {
                                    isRtnValue = this.SetSP_WRK_BTCH_UPDATE(da, item, strBtchNo);

                                    if (isRtnValue == false)
                                    {
                                        break;
                                    }
                                }

                                if (isRtnValue == true)
                                {
                                    // 저장된 경우
                                    da.CommitTransaction();

                                    // 상태바 (아이콘) 제거
                                    this.loadingScreen.IsSplashScreenShown = false;

                                    // 저장되었습니다.
                                    BaseClass.MsgInfo("CMPT_SAVE");
                                }
                                else
                                {
                                    // 오류 발생하여 저장 실패한 경우
                                    da.RollbackTransaction();
                                }
                            }
                            catch
                            {
                                //if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                                //{
                                da.RollbackTransaction();
                                //}

                                BaseClass.MsgError("ERR_SAVE");
                                throw;
                            }
                            finally
                            {
                                // 상태바 (아이콘) 제거
                                this.loadingScreen.IsSplashScreenShown = false;

                                // 출고계획 리스트 재조회
                                WrkPlanListSearch();
                            }
                        }
                    }
                }
                else
                {
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private bool SetSP_WRK_BTCH_UPDATE(BaseDataAccess da, WrkPlanInfo item, string pBtchNo)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_P1007_GAN.SP_WRK_BTCH_UPDATE";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUT_RESULT" };

            var strCoCd = BaseClass.CompanyCode;                // 회사 코드
            var strCntrCd = BaseClass.CenterCD;                 // 센터 코드           
            var strOrdNo = item.ORD_NO.ToString();              // 오더번호
            var strOrdLineNo = item.ORD_LINE_NO.ToString();     // 오더번호
            var strWavNo = item.WAV_NO.ToString();              // 웨이브번호
            var strBtchNo = pBtchNo;                            // 배치번호
            var strUserID = this.BaseClass.UserID;              // 사용자 ID

            var strErrCode = string.Empty;                      // 오류 코드
            var strErrMsg = string.Empty;                       // 오류 메세지
            #endregion

            #region Input 파라메터     
            dicInputParam.Add("P_CO_CD", strCoCd);               // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);           // 센터 코드
            dicInputParam.Add("P_ORD_NO", strOrdNo);             // 오더번호
            dicInputParam.Add("P_ORD_LINE_NO", strOrdLineNo);    // 오더라인 번호
            dicInputParam.Add("P_WAV_NO", strWavNo);             // 웨이브번호
            dicInputParam.Add("P_BTCH_NO", strBtchNo);           // 배치번호
            dicInputParam.Add("P_USER_ID", strUserID);           // 사용자 ID
            #endregion

            dtRtnValue = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        BaseClass.MsgInfo(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }

        private string GetSP_WRK_BTCH_CREATE(BaseDataAccess da)
        {
            string RtnBatchNo = string.Empty;

            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P1007_GAN.SP_WRK_BTCH_CREATE";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;       // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;        // 센터 코드
            var strUserID = this.BaseClass.UserID;          // 사용자 ID

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                    // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                  // 센터 코드
            dicInputParam.Add("P_USER_ID", strUserID);        // 
            #endregion

            #region 데이터 조회
            dsRtnValue = da.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            #endregion

            if (dsRtnValue.Tables[1].Rows[0]["CODE"].ToString().Equals("0") == true)    // 0	SUCCESS
            {
                if (dsRtnValue.Tables[0].Rows.Count > 0)
                {
                    RtnBatchNo = dsRtnValue.Tables[0].Rows[0][0].ToString();          // 배치번호
                }
                else
                {
                    // 배치생성중 오류가 발생했습니다.

                }
            }
            else
            {

            }

            return RtnBatchNo;
        }

        /// <summary>
        /// PLAN수립
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlanCreate_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isRtnValue = false;

                var cBtchNo = this.WrkPlanList.Where(w => w.IsSelected == true).Select(s => s.BTCH_NO).FirstOrDefault();
                var cEqpId = this.WrkPlanList.Where(w => w.IsSelected == true).Select(s => s.EQP_ID).FirstOrDefault();

                if (!string.IsNullOrEmpty(cBtchNo))
                {
                    using (BaseDataAccess da = new BaseDataAccess())
                    {
                        try
                        {
                            // 상태바 (아이콘) 실행
                            this.loadingScreen.IsSplashScreenShown = true;

                            da.BeginTransaction();

                            // TODO : 배치생성하는 부분 (CWJ)
                            //isRtnValue = true;

                            //if(isRtnValue)
                            if (CallJsonWebService.CallServiceGantryPicking(cBtchNo, cEqpId) == true)
                            {
                                isRtnValue = this.SetSP_WRK_BTCH_CONFIRM(da, cBtchNo);

                                if (isRtnValue == true)
                                {
                                    // 저장된 경우
                                    da.CommitTransaction();

                                    // 상태바 (아이콘) 제거
                                    this.loadingScreen.IsSplashScreenShown = false;

                                    // 저장되었습니다.
                                    BaseClass.MsgInfo("CMPT_SAVE");

                                    // 출고작업 조회
                                    //MenuOpen_R1007_GAN_View();
                                }
                                else
                                {
                                    // 배치컨펌시 오류 발생하여 저장 실패한 경우
                                    da.RollbackTransaction();
                                }
                            }
                            else
                            {
                                // 배치생성시 오류 발생하여 저장 실패한 경우
                                da.RollbackTransaction();
                            }
                        }
                        catch
                        {
                            if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                            {
                                da.RollbackTransaction();
                            }

                            BaseClass.MsgError("ERR_SAVE");
                            throw;
                        }
                        finally
                        {
                            // 상태바 (아이콘) 제거
                            this.loadingScreen.IsSplashScreenShown = false;

                            // 출고계획 리스트 재조회
                            WrkPlanListSearch();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 배치 취소
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBtchCancel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //해당 배치가 배치생성, PLAN수립 상태일 경우만 취소 처리한다.
            try
            {
                bool isRtnValue = false;

                // 해당 배치가 배치생성(01), PLAN수립(02) 상태일 경우만 취소 처리한다.
                // 배치확정(03)일 경우도 취소 처리 추가 (191127 KJB)
                List<string> CheckTallyStatCd = new List<string> { "01", "02", "03" };

                var cancelBtchNo = this.WrkPlanList.Where(w => w.IsSelected == true
                && CheckTallyStatCd.Contains(w.WRK_STAT_CD)).Select(s => s.BTCH_NO).FirstOrDefault();

                if (!string.IsNullOrEmpty(cancelBtchNo))
                {
                    this.BaseClass.MsgQuestion("ASK_CANCEL_BTCH_SAVE");
                    if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                    {
                        using (BaseDataAccess da = new BaseDataAccess())
                        {
                            try
                            {
                                // 상태바 (아이콘) 실행
                                this.loadingScreen.IsSplashScreenShown = true;

                                da.BeginTransaction();

                                isRtnValue = this.SetSP_WRK_BTCH_CANCEL(da, cancelBtchNo);

                                if (isRtnValue == true)
                                {
                                    // 저장된 경우
                                    da.CommitTransaction();

                                    // 상태바 (아이콘) 제거
                                    this.loadingScreen.IsSplashScreenShown = false;

                                    // 저장되었습니다.
                                    BaseClass.MsgInfo("CMPT_SAVE");
                                }
                                else
                                {
                                    // 오류 발생하여 저장 실패한 경우
                                    da.RollbackTransaction();
                                }
                            }
                            catch
                            {
                                if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                                {
                                    da.RollbackTransaction();
                                }

                                BaseClass.MsgError("ERR_SAVE");
                                throw;
                            }
                            finally
                            {
                                // 상태바 (아이콘) 제거
                                this.loadingScreen.IsSplashScreenShown = false;

                                // 출고계획 리스트 재조회
                                WrkPlanListSearch();
                            }
                        }
                    }
                }
                else
                {
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 배치 종료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBtchClose_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isRtnValue = false;

                //// 해당 배치가 배치생성(01), PLAN수립(02) 상태일 경우만 취소 처리한다.
                //// 배치확정(03)일 경우도 취소 처리 추가 (191127 KJB)
                //List<string> CheckTallyStatCd = new List<string> { "01", "02", "03" };

                var closeBtchNo = this.WrkPlanList.Where(w => w.IsSelected == true).Select(s => s.BTCH_NO).FirstOrDefault();

                if (!string.IsNullOrEmpty(closeBtchNo))
                {
                    this.BaseClass.MsgQuestion("ASK_CLOSE_BTCH_SAVE");
                    if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                    {
                        using (BaseDataAccess da = new BaseDataAccess())
                        {
                            try
                            {
                                // 상태바 (아이콘) 실행
                                this.loadingScreen.IsSplashScreenShown = true;

                                da.BeginTransaction();

                                isRtnValue = this.SetSP_WRK_BTCH_CLOSE(da, closeBtchNo);

                                if (isRtnValue == true)
                                {
                                    // 저장된 경우
                                    da.CommitTransaction();

                                    // 상태바 (아이콘) 제거
                                    this.loadingScreen.IsSplashScreenShown = false;

                                    // 저장되었습니다.
                                    BaseClass.MsgInfo("CMPT_BTCH_CLOSE_SAVE");
                                }
                                else
                                {
                                    // 오류 발생하여 저장 실패한 경우
                                    da.RollbackTransaction();
                                }
                            }
                            catch
                            {
                                if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                                {
                                    da.RollbackTransaction();
                                }

                                BaseClass.MsgError("ERR_SAVE");
                                throw;
                            }
                            finally
                            {
                                // 상태바 (아이콘) 제거
                                this.loadingScreen.IsSplashScreenShown = false;

                                // 입고계획 리스트 재조회
                                WrkPlanListSearch();
                            }
                        }
                    }
                }
                else
                {
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

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
        #endregion

        #endregion

        #region >> 그리드 관련 이벤트
        
        #region + 그리드 컬럼 Indicator 영역에 순번 표현 관련 이벤트
        /// <summary>
        /// 그리드 컬럼 Indicator 영역에 순번 표현 관련 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMaster_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData == true)
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
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

            TableView tv = sender as TableView;

            if (tv.Name.Equals("tvMasterGrid") == true)
            {
                WrkPlanInfo dataMember = tv.Grid.CurrentItem as WrkPlanInfo;

                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    //  ID,  명 컬럼은 수정이 되지 않도록 처리한다.
                    case "TOT_BOX_ID":
                        //case "EQP_NM":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    default: break;
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MarkChecked((sender as CheckBox)?.IsChecked ?? false);
        }

        private void MarkChecked(bool isChecked)
        {
            if (WrkPlanList == null || WrkPlanList.Count == 0)
                return;

            gridMaster.SelectedItem = null;

            // 선택된 로우 해제
            this.WrkPlanList.ForEach(p => p.IsSelected = false);

            this.WrkPlanList.Where(w => w.BTCH_NO.Length == 0).ForEach(p => p.IsSelected = isChecked);

            ButtonEnableCheck();
        }

        private void Chked_Editor_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool SelectIsChecked = (sender as CheckEdit).IsChecked == true ? true : false;

                if (gridMaster.SelectedItem != null)
                {
                    var currentRowData = (gridMaster.SelectedItem as WrkPlanInfo);
                    string strBtchNo = string.Empty;
                    string strMessage = string.Empty;

                    // TODO: 체크/언체크 배치번호 확인 필요
                    // 배치번호가 있는 경우
                    if (currentRowData.BTCH_NO.Length > 0)
                    {
                        if (currentRowData.WRK_STAT_CD.Equals("05")
                            || currentRowData.WRK_STAT_CD.Equals("50"))
                        {
                            return;
                        }


                        if (SelectIsChecked)
                        {
                            this.WrkPlanList.Where(p => p.BTCH_NO != currentRowData.BTCH_NO).ForEach(p => p.IsSelected = false);
                            this.WrkPlanList.Where(p => p.BTCH_NO == currentRowData.BTCH_NO).ForEach(p => p.IsSelected = true);
                        }
                        else
                        {
                            this.WrkPlanList.Where(p => p.BTCH_NO == currentRowData.BTCH_NO).ForEach(p => p.IsSelected = false);
                        }
                    }
                    else // 배치번호가 없는 경우
                    {
                        if (SelectIsChecked)
                        {
                            this.WrkPlanList.Where(p => p.IsSelected == true && p.BTCH_NO.Length > 0).ForEach(p => p.IsSelected = false);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                ButtonEnableCheck();
            }
        }

        private void ButtonEnableCheck()
        {
            if (this.WrkPlanList.Where(w => w.IsSelected).Count() > 0)
            {
                if (this.WrkPlanList.Where(w => w.IsSelected && w.BTCH_NO.Length > 0).Count() > 0)
                {
                    // 배치확정
                    if (this.WrkPlanList.Where(w => w.IsSelected && w.BTCH_NO.Length > 0 && w.WRK_STAT_CD.Equals("03")).Count() > 0)
                    {
                        ButtonEnableSet(false, false, true, true);
                    }
                    //else if (this.WrkPlanList.Where(w => w.IsSelected && w.BTCH_NO.Length > 0 && w.WRK_STAT_CD.Equals("05")).Count() > 0)
                    //{
                    //    // 작업중
                    //    ButtonEnableSet(false, false, false, false);
                    //}
                    else
                    {
                        // 배치생성
                        ButtonEnableSet(false, true, true, true);
                    }
                }
                else
                {
                    // 미처리
                    ButtonEnableSet(true, false, false, false);
                }
            }
            else
            {
                // 미선택
                ButtonEnableSet(false, false, false, false);
            }
        }

        /// <summary>
        /// Button 활성화 변경
        /// </summary>
        /// <param name="pBtchCreate">배치생성</param>
        /// <param name="pPlanCreate">플랜생성</param>
        /// <param name="pBtchCancel">배치취소</param>
        /// /// <param name="pBtchClose">배치종료</param>
        /// <param name="pExcelDownload">엑셀다운로드</param>
        private void ButtonEnableSet(bool pBtchCreate = true, bool pPlanCreate = true, bool pBtchCancel = true, bool pBtchClose = true, bool pExcelDownload = true)
        {
            btnBtchCreate.IsEnabled = pBtchCreate;
            btnPlanCreate.IsEnabled = pPlanCreate;
            btnBtchCancel.IsEnabled = pBtchCancel;
            btnBtchClose.IsEnabled = pBtchClose;
            btnExcelDownload.IsEnabled = pExcelDownload;
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

        /// <summary>
        /// 출고 작업조회 화면 오픈
        /// </summary>
        private void MenuOpen_R1007_GAN_View()
        {
            MainWinParam objParam = new MainWinParam();

            #region Sample
            //objParam.EQP_ID = this.cboEQP.GetKeyValue(this.cboEQP.SelectedIndex).ToString();    // 설비 ID
            //objParam.GI_YMD = currentRowData.WRK_YMD;                                           // 출고일자
            //objParam.CST_CD = currentRowData.CST_CD;                                            // 고객사 코드
            //objParam.CST_NM = currentRowData.CST_NM;                                            // 고객사 명
            //objParam.BTCH_SEQ = currentRowData.BTCH_SEQ;                                        // 배치순번
            #endregion

            objParam.MENU_ID = "R1007_GAN";

            this.SelectedMenuOpenEvent(objParam);
        }

        #endregion

    }

}
