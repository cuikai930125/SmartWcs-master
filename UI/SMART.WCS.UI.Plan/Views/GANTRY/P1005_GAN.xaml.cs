using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.DataMembers;
using SMART.WCS.UI.Plan.DataMembers.P1005_GAN;
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
    /// Plan > Smart Gantry > 입고 계획관리
    /// </summary>
    public partial class P1005_GAN : UserControl
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
            = DependencyProperty.Register("IsColumnChecked", typeof(bool), typeof(P1005_GAN)
                , new PropertyMetadata(false));

        public bool IsColumnChecked
        {
            get { return (bool)GetValue(IsColumnCheckedProperty); }
            set { SetValue(IsColumnCheckedProperty, value); }
        }
        #endregion

        #region >> WrkPlanList - 입고 계획 리스트
        public static readonly DependencyProperty WrkPlanListProperty
            = DependencyProperty.Register("WrkPlanList", typeof(ObservableCollection<WrkPlanInfo>), typeof(P1005_GAN)
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
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(P1005_GAN),
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

        bool FirstLoad = true;

        #endregion

        #region ▩ 생성자
        public P1005_GAN()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public P1005_GAN(List<string> _liMenuNavigation)
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

        public P1005_GAN(List<string> _liMenuNavigation, MainWinParam _liParam)
        {
            InitializeComponent();
        }
        #endregion

        #region ▩ 이벤트

        #region > Loaded 이벤트
        private void P1005_GAN_Loaded(object sender, RoutedEventArgs e)
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

        #region P1005_GAN_KeyDown Event
        private void P1005_GAN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.LeftCtrl))
            {
                this.btnSearch.IsEnabled = true;
                this.btnBtchCreate.IsEnabled = true;
                this.btnPlanCreate.IsEnabled = true;
                this.btnBtchCancel.IsEnabled = true;
                this.btnBtchClose.IsEnabled = true;
                this.btnExcelDownload.IsEnabled = true;
            }
        }
        #endregion

        #region BtnSearch_PreviewMouseLeftButtonUp (조회) Event
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
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
            WrkPlanListSearch();
            //}
        }
        #endregion

        #region BtnBtchCreate_PreviewMouseLeftButtonUp (배치생성) Event
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

                                //PK_P1005.SP_WRK_BTCH_CREATE 호출 후 배치를번호 RETURN 후 
                                var strBtchNo = this.GetSP_WRK_BTCH_CREATE(da);

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
                                if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                                {
                                    da.RollbackTransaction();
                                }

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
        #endregion

        #region BtnPlanCreate_PreviewMouseLeftButtonUp (PLAN 수립) Event
        /// <summary>
        /// PLAN 수립
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
                            if (CallJsonWebService.CallServiceGantryStocking(cBtchNo, cEqpId) == true)
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

                            // 입고계획 리스트 재조회
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

        #endregion

        #region BtnBtchCancel_PreviewMouseLeftButtonUp (배치 취소) Event
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

                                    // 입고 작업조회
                                    //MenuOpen_R1005_GAN_View();
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
        #endregion
        
        #region BtnBtchClose_PreviewMouseLeftButtonUp (배치 종료) Event
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

        #endregion

        #region BtnExcelDownload_PreviewMouseLeftButtonUp (엑셀 다운로드) Event
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

        #region ▩ 함수

        #region > 초기화

        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            // 콤보박스 - 조회 (작업상태)
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
            this.Loaded += P1005_GAN_Loaded;
            #endregion

            this.KeyDown += P1005_GAN_KeyDown;

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
        }

        #endregion

        #endregion

        #region > 컨트롤 이벤트 관련
        /// <summary>
        /// 입고계획 리스트 조회
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

                // 주석 : 컬럼헤더 초기화 문제 확인 (CWJ)

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
        #endregion

        #region > 데이터 관련

        #region >> GetSP_COMM_TOTE_SEARCH - 입고계획 리스트 조회
        /// <summary>
        /// 데이터조회
        /// </summary>
        private DataSet GetSP_WRK_PLAN_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P1005_GAN.SP_WRK_PLAN_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                   // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                    // 센터 코드

            var strFrOrdYmd = this.BaseClass.GetCalendarValue(this.deFrOrdYmd);         // 시작 입고예정일자
            var strToOrdYmd = this.BaseClass.GetCalendarValue(this.deToOrdYmd);         // 종료 입고예정일자
            var strBtchNo = this.txtBatchNo.Text.Trim();                                // 배치번호
            var strWrkStatCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboWorkState);       // 작업상태코드

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);              // 센터 코드
            dicInputParam.Add("P_FR_ORD_YMD", strFrOrdYmd);         // 시작 입고예정일자 
            dicInputParam.Add("P_TO_ORD_YMD", strToOrdYmd);         // 종료 입고예정일자 
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

        #region >> GetSP_WRK_BTCH_CREATE - 배치 번호 생성
        /// <summary>
        /// 배치 번호 생성
        /// </summary>
        /// <param name="da"></param>
        /// <returns></returns>
        private string GetSP_WRK_BTCH_CREATE(BaseDataAccess da)
        {
            string RtnBatchNo = string.Empty;

            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P1005_GAN.SP_WRK_BTCH_CREATE";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;       // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;        // 센터 코드
            var strUserID = this.BaseClass.UserID;          // 사용자 ID

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);          // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);      // 센터 코드
            dicInputParam.Add("P_USER_ID", strUserID);      // 사용자 ID
            #endregion

            #region 데이터 조회
            dsRtnValue = da.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            #endregion

            if (dsRtnValue.Tables[1].Rows[0]["CODE"].ToString().Equals("0") == true)    // 0	SUCCESS
            {
                if (dsRtnValue.Tables[0].Rows.Count > 0)
                {
                    RtnBatchNo = dsRtnValue.Tables[0].Rows[0][0].ToString();        // 배치번호
                }
                else
                {
                    // 배치생성중 오류가 발생했습니다. => 정상처리 후 배치번호가 없는 경우
                    this.BaseClass.MsgError("ERR_BTCH_SAVE");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dsRtnValue.Tables[1].Rows[0]["MSG"].ToString()))
                {
                    this.BaseClass.MsgError(dsRtnValue.Tables[1].Rows[0]["MSG"].ToString());
                }
                else
                {
                    // 배치생성중 오류가 발생했습니다. => 배치생성 실패
                    this.BaseClass.MsgError("ERR_BTCH_SAVE");
                }
            }

            return RtnBatchNo;
        }
        #endregion

        #region >> SetSP_WRK_BTCH_UPDATE - 배치 정보 업데이트 (TB_GAN_IB_ORD)
        /// <summary>
        /// 배치 정보 업데이트 (TB_GAN_IB_ORD)
        /// </summary>
        /// <param name="da"></param>
        /// <param name="item"></param>
        /// <param name="pBtchNo"></param>
        /// <returns></returns>
        private bool SetSP_WRK_BTCH_UPDATE(BaseDataAccess da, WrkPlanInfo item, object pBtchNo)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_P1005_GAN.SP_WRK_BTCH_UPDATE";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUT_RESULT" };

            var strCoCd = BaseClass.CompanyCode;                // 회사 코드
            var strCntrCd = BaseClass.CenterCD;                 // 센터 코드  
            var strBtchNo = pBtchNo;                            // 배치번호
            var strAsnNo = item.ASN_NO.ToString();              // 입고번호
            var strTotBoxId = item.TOT_BOX_ID.ToString();       // 토트박스번호
            var strSkuCd = item.SKU_CD.ToString();              // 상품코드
            var strUserID = this.BaseClass.UserID;              // 사용자 ID

            var strErrCode = string.Empty;                      // 오류 코드
            var strErrMsg = string.Empty;                       // 오류 메세지
            #endregion

            #region Input 파라메터     
            dicInputParam.Add("P_CO_CD", strCoCd);              // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);          // 센터 코드
            dicInputParam.Add("P_BTCH_NO", strBtchNo);          // 배치번호
            dicInputParam.Add("P_ASN_NO", strAsnNo);            // 입고번호
            dicInputParam.Add("P_TOT_BOX_ID", strTotBoxId);     // 웨이브번호 
            dicInputParam.Add("P_SKU_CD", strSkuCd);            // 웨이브번호 
            dicInputParam.Add("P_USER_ID", strUserID);          // 사용자 ID
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
        #endregion

        #region >> SetSP_WRK_BTCH_CONFIRM - 배치 컨펌
        private bool SetSP_WRK_BTCH_CONFIRM(BaseDataAccess da, string pBtchNo)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_P1005_GAN.SP_WRK_BTCH_CONFIRM";
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
        /// <summary>
        /// 배치 취소
        /// </summary>
        /// <param name="da"></param>
        /// <param name="pBtchNo"></param>
        /// <returns></returns>
        private bool SetSP_WRK_BTCH_CANCEL(BaseDataAccess da, string pBtchNo)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_P1005_GAN.SP_WRK_BTCH_CANCEL";
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

        #region >> 배치종료
        private bool SetSP_WRK_BTCH_CLOSE(BaseDataAccess da, string pBtchNo)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_P1005_GAN.SP_WRK_BTCH_CLOSE";
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

        #region ButtonEnableCheck - 버튼 활성화 여부 처리 메서드
        /// <summary>
        /// 버튼 활성화 여부 처리 메서드
        /// </summary>
        private void ButtonEnableCheck()
        {
            if (this.WrkPlanList.Where(w => w.IsSelected).Count() > 0)
            {
                if (this.WrkPlanList.Where(w => w.IsSelected && w.BTCH_NO.Length > 0).Count() > 0)
                {
                    if (this.WrkPlanList.Where(w => w.IsSelected && w.BTCH_NO.Length > 0 && w.WRK_STAT_CD.Equals("03")).Count() > 0)    // 배치확정
                    {
                        ButtonEnableSet(false, false, false, false);
                    }
                    else
                    {
                        ButtonEnableSet(false, true, true, true);
                    }
                }
                else
                {
                    ButtonEnableSet(true, false, false, false);
                }
            }
            else
            {
                ButtonEnableSet(false, false, false, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pBtchCreate"></param>
        /// <param name="pPlanCreate"></param>
        /// <param name="pBtchCancel"></param>
        /// <param name="pBtchClose"></param>
        /// <param name="pExcelDownload"></param>
        private void ButtonEnableSet(bool pBtchCreate = true, bool pPlanCreate = true, bool pBtchCancel = true, bool pBtchClose = true, bool pExcelDownload = true)
        {
            btnBtchCreate.IsEnabled = pBtchCreate;
            btnPlanCreate.IsEnabled = pPlanCreate;
            btnBtchCancel.IsEnabled = pBtchCancel;
            btnBtchClose.IsEnabled = pBtchClose;
            btnExcelDownload.IsEnabled = pExcelDownload;
        }
        #endregion

        #region MenuOpen_R1005_GAN_View - 입고작업조회 화면 오픈 메서드
        /// <summary>
        /// 입고작업조회 화면 오픈
        /// </summary>
        private void MenuOpen_R1005_GAN_View()
        {
            MainWinParam objParam = new MainWinParam();
            objParam.MENU_ID = "R1005_GAN";

            this.SelectedMenuOpenEvent(objParam);
        }
        #endregion

        #endregion

        #endregion
    }
}