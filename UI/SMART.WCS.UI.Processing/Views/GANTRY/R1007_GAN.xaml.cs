using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.Processing.DataMembers.R1007_GAN;
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
    /// Plan > Smart Gantry > 출고 계획관리
    /// </summary>
    public partial class R1007_GAN : UserControl
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
        public static readonly DependencyProperty WrkListProperty
            = DependencyProperty.Register("WrkList", typeof(ObservableCollection<WrkInfo>), typeof(R1007_GAN)
                , new PropertyMetadata(new ObservableCollection<WrkInfo>()));

        public ObservableCollection<WrkInfo> WrkList
        {
            get { return (ObservableCollection<WrkInfo>)GetValue(WrkListProperty); }
            set { SetValue(WrkListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(R1007_GAN), new PropertyMetadata(string.Empty));

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

        #region ▩ 생성자
        public R1007_GAN()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public R1007_GAN(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

               // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

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
            this.BaseClass.BindingCommonComboBox(this.cboGanWorkState, "WRK_STAT_CD", null, true);
        }

        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += R1007_GAN_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp; ;
            // 엑셀 다운로드
            this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp; ;

            #endregion


        }
        #endregion
        #endregion

        bool FirstLoad = true;

        #endregion


        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void R1007_GAN_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstLoad)
                {
                    FirstLoad = false;
                    WrkListSearch();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }            
        }
        #endregion

        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WrkListSearch();
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

        #region > 데이터 관련
        /// <summary>
        /// 출고 계획 리스트 조회
        /// </summary>
        private void WrkListSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_WRK_SEARCH();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.WrkList = new ObservableCollection<WrkInfo>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.WrkList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.WrkList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.WrkList;

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

                if (this.WrkList.Count == 0)
                {
                    this.BaseClass.MsgInfo("INFO_NOT_INQ");
                }
            }
        }

        #region >> GetSP_WRK_SEARCH - 출고 계획 리스트 조회
        /// <summary>
        /// 출고 계획 리스트 데이터조회
        /// </summary>
        private DataSet GetSP_WRK_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_R1007_GAN.SP_WRK_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                               // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            var strFrWrkYmd = this.BaseClass.GetCalendarValue(this.deFrWrkYmd);                                     // 시작 출고 계획 번호
            var strToWrkYmd = this.BaseClass.GetCalendarValue(this.deToWrkYmd);                                     // 종료 출고 계획 번호
            var strBtchNo = this.txtBatchNo.Text.Trim();   // 배치번호
            var strGanWrkStatCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboGanWorkState);     // 출고 계획 상태

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                    // 
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                // 
            dicInputParam.Add("P_FR_WRK_YMD", strFrWrkYmd);        // 
            dicInputParam.Add("P_TO_WRK_YMD", strToWrkYmd);        // 
            dicInputParam.Add("P_BTCH_NO", strBtchNo);           // 
            dicInputParam.Add("P_GAN_WRK_STAT_CD", strGanWrkStatCd);    // 
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

        #endregion

        private void tvMasterGrid_CellMerge(object sender, CellMergeEventArgs e)
        {
            try
            {
                List<string> _Fields = new List<string> { "ECS_JOB_NO" };

                if (_Fields.Contains(e.Column.FieldName))
                {
                    var _row1 = this.gridMaster.GetRow(e.RowHandle1);
                    var _row2 = this.gridMaster.GetRow(e.RowHandle2);

                    if (_row1 != null && _row2 != null)
                    {
                        if (_row1 is WrkInfo && _row2 is WrkInfo)
                        {
                            e.Merge = (_row1 as WrkInfo).GroupEquals(_row2 as WrkInfo);
                            e.Handled = true;                            
                        }
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
    }
}
