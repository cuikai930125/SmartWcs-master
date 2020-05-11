using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.Analysis.DataMembers.A1007_SRT;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.Analysis.Views.SORTER
{
    /// <summary>
    /// 시간대별 슈트별 실적조회
    /// </br> 2020-01-06
    /// </br> 추성호
    /// </summary>
    public partial class A1007_SRT : UserControl
    {
        #region ▩ Detegate 선언
        #region > 메인화면 하단 좌측 상태바 값 반영
        //public delegate void ToolStripStatusEventHandler(string value);
        //public event ToolStripStatusEventHandler ToolStripChangeStatusLabelEvent;
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
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        #endregion

        #region ▩ 생성자
        public A1007_SRT()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public A1007_SRT(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 상단에 설명 
                this.CommentArea.ScreenID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                this.InitControl();

                this.InitEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        /// <summary>
        /// 시간대별 슈트별 실적조회
        /// </summary>
        public static readonly DependencyProperty MasterRsltListProperty
            = DependencyProperty.Register("MasterRsltList", typeof(ObservableCollection<MasterRslt>), typeof(A1007_SRT)
                , new PropertyMetadata(new ObservableCollection<MasterRslt>()));

        /// <summary>
        /// 시간대별 슈트별 실적조회
        /// </summary>
        public ObservableCollection<MasterRslt> MasterRsltList
        {
            get { return (ObservableCollection<MasterRslt>)GetValue(MasterRsltListProperty); }
            set { SetValue(MasterRsltListProperty, value); }
        }
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 컨트롤 초기화
        /// <summary>
        /// 컨트롤 초기화
        /// </summary>
        private void InitControl()
        {
            try
            {
                #region >> 설비 ID 콤보박스 설정
                string[] commonParam_EQP_ID = { BaseClass.CenterCD, "SRT", BaseClass.UserID, string.Empty };
                this.BaseClass.BindingCommonComboBox(this.cboEqp, "EQP_ID", commonParam_EQP_ID, true);
                #endregion

                #region >> Zone 콤보박스 설정
                this.BaseClass.BindingCommonComboBox(this.cboZone, "ZONE_ID", null, true);
                #endregion

                #region >> 시간 설정
                this.BaseClass.AutoSetDateTimeByCenter(this.dteFromDate, this.dteFromTime, this.dteToDate, this.dteToTime);
                #endregion
            }
            catch { throw; }
        }
        #endregion

        #region > InitEvent - 이벤트 초기화
        private void InitEvent()
        {
            try
            {
                // 조회 버튼 이벤트
                this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;

                // 엑셀다운로드 버튼 이벤트
                this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_HM_CHUTE_LIST_INQ - 시간대별 슈트별 실적조회
        /// <summary>
        /// 시간대별 슈트별 실적조회
        /// </summary>
        /// <returns></returns>
        private DataSet GetSP_HM_CHUTE_LIST_INQ()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_A1007_SRT.SP_HM_CHUTE_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_HM_TITLE_LIST", "O_HM_TOT_LIST", "O_HM_CHUTE_LIST", "O_HM_ZONE_LIST", "O_RSLT" };

                var strCenterCD         = this.BaseClass.CenterCD;
                var strEqpID            = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);
                var strZone             = this.BaseClass.ComboBoxSelectedKeyValue(this.cboZone);
                var strFromDate         = this.dteFromDate.DateTime.ToString("yyyyMMdd");
                var strFromTime         = this.BaseClass.ReplaceText(this.dteFromTime, ":", string.Empty);
                var strToDate           = this.dteToDate.DateTime.ToString("yyyyMMdd");
                var strToTime           = this.BaseClass.ReplaceText(this.dteToTime, ":", string.Empty);
                
                var strErrCode          = string.Empty;
                var strErrMsg           = string.Empty;
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTR_CD",              strCenterCD);                       // 센터 코드
                dicInputParam.Add("P_EQP_ID",               strEqpID);                          // 설비 ID
                dicInputParam.Add("P_ZONE_ID",              strZone);                           // Zone ID
                dicInputParam.Add("P_FM_YMD",               $"{strFromDate}{strFromTime}");     // 시작일자
                dicInputParam.Add("P_TO_YMD",               $"{strToDate}{strToTime}");         // 종료일자
                #endregion

                #region 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }
                #endregion

                return dsRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 버튼 이벤트
        #region >> 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 엑셀 다운로드 버튼 클릭 이벤트
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
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        #region >> 조회 버튼 클릭 이벤트
        /// <summary>
        /// 조회 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var iDiffDay = this.BaseClass.CheckDateTerm(this.dteFromDate, this.dteToDate, BaseEnumClass.SystemCode.ANL);
                if (iDiffDay < 0)
                {
                    // ERR_SEARCH_COND_TERM - 일자 조건을 {0}일 이내로 설정하셔야 합니다.
                    this.BaseClass.MsgError("ERR_COND_TERM", this.BaseClass.AnlInqTerm.ToString());
                    return;
                }

                DataSet dsRtnValue      = this.GetSP_HM_CHUTE_LIST_INQ();
                DataTable dtBinding     = new DataTable();

                if (dsRtnValue.Tables.Count != 5)
                {
                    // 데이터 반환 오류
                }
                else
                {
                    if (dsRtnValue.Tables[0].Rows.Count > 0)
                    {
                        if (dsRtnValue.Tables[0].Columns.Count == 24)
                        {
                            for (int i = 0; i < dsRtnValue.Tables[0].Columns.Count; i++)
                            {
                                var strTitle = dsRtnValue.Tables[0].Rows[0][i].ToString();
                                //this.gridMaster.Columns[0].Header
                                this.gridMaster.Columns[i + 1].Header = $"{strTitle.Substring(0, 2)}:{strTitle.Substring(2, 2)}";

                            }
                        }
                    }

                    if (dsRtnValue.Tables[1].Columns[0].ColumnName.Equals("'전체'") == true)
                    {
                        dsRtnValue.Tables[1].Columns[0].ColumnName = "ZONE_ID";
                    }

                    var liUnionData = dsRtnValue.Tables[1].AsEnumerable().Union
                        (dsRtnValue.Tables[3].AsEnumerable().Union
                        (dsRtnValue.Tables[2].AsEnumerable()));


                    if (liUnionData.Count() > 0)
                    {
                        dtBinding = liUnionData.CopyToDataTable();
                    }

                    this.MasterRsltList = new ObservableCollection<MasterRslt>();
                    // 오라클인 경우 TableName = O_CELL_LIST
                    this.MasterRsltList.ToObservableCollection(dtBinding);
                    this.gridMaster.ItemsSource = MasterRsltList;
                }
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
            this.TreeControlRefreshEvent();
        }
        #endregion
        #endregion
    }
}
