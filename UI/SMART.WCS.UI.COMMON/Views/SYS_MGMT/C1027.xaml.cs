using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.UI.COMMON.DataMembers.C1027;

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
using System.Windows.Media;
using System.Windows.Threading;

namespace SMART.WCS.UI.COMMON.Views.SYS_MGMT
{
    /// <summary>
    /// 백업관리
    /// </summary>
    public partial class C1027 : UserControl, TabCloseInterface
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
        /// Base 클래스 선언
        /// </summary>
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public C1027()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1027(List<string> _liMenuNavigation)
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

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();

                this.BtnSearch_First_PreviewMouseLeftButtonUp(null, null);
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1027), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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
                view.ShowingEditor += view_ShowingEditor;
            }
        }
        #endregion

        #region > 백업관리
        #region >> 배치 차수 조회
        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public static readonly DependencyProperty BackMstMgntListProperty
            = DependencyProperty.Register("BackMstMgntList", typeof(ObservableCollection<BackMstMgnt>), typeof(C1027)
                , new PropertyMetadata(new ObservableCollection<BackMstMgnt>()));

        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public ObservableCollection<BackMstMgnt> BackMstMgntList
        {
            get { return (ObservableCollection<BackMstMgnt>)GetValue(BackMstMgntListProperty); }
            set { SetValue(BackMstMgntListProperty, value); }
        }
        #endregion

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1027), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string TabFirstGridRowCount
        {
            get { return (string)GetValue(GridRowCountProperty); }
            set { SetValue(GridRowCountProperty, value); }
        }
        #endregion

        public static readonly DependencyProperty DTLProperty = DependencyProperty.Register("DTL",
            typeof(string), typeof(C1027), new PropertyMetadata(null));

        public string DTL
        {
            get { return (string)GetValue(DTLProperty); }
            set
            {
                SetValue(DTLProperty, value);
                //this.UPDATE_FLAG = true;
            }
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
            try
            {
                // 콤보박스
                //this.BaseClass.BindingCommonComboBox(this.cboUseYN_First, "USE_YN", null, false); //사용여부
                // 버튼(행추가/행삭제) 툴팁 처리
                this.btnRowAdd_First.ToolTip = this.BaseClass.GetResourceValue("ROW_ADD");   //행추가
                this.btnRowDelete_First.ToolTip = this.BaseClass.GetResourceValue("ROW_DEL"); //행삭제
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
            #region + Loaded 이벤트
            this.Loaded += C1027_Loaded;
            #endregion

            #region + 센터 관리
            #region ++ 버튼 클릭 이벤트
            // 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_First_PreviewMouseLeftButtonUp;
            // 저장
            this.btnSave_First.PreviewMouseLeftButtonUp += BtnSave_First_PreviewMouseLeftButtonUp;

            // 행 추가
            this.btnRowAdd_First.PreviewMouseLeftButtonUp += BtnRowAdd_First_PreviewMouseLeftButtonUp;
            // 행 삭제
            this.btnRowDelete_First.PreviewMouseLeftButtonUp += BtnRowDelete_First_PreviewMouseLeftButtonUp;
            #endregion

            #region ++ Row 순번 채번 이벤트
            this.gridMaster.CustomUnboundColumnData += GridMaster_CustomUnboundColumnData;
            #endregion

            #region ++ 그리드 이벤트
            // 그리드 클릭 이벤트
            this.gridMaster.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;
            #endregion
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
            this.TabFirstGridRowCount = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";        // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion

        #region >> TabClosing - 탭을 닫을 때 데이터 저장 여부 체크
        /// <summary>
        /// 탭을 닫을 때 데이터 저장 여부 체크
        /// </summary>
        /// <returns></returns>
        public bool TabClosing()
        {
            return this.CheckModifyData(BaseEnumClass.ClickedButtonKind.TAB);
        }
        #endregion

        #region >> CheckModifyData - 데이터 저장 여부를 체크한다. (조회, 탭 종료시)
        /// <summary>
        /// 데이터 저장 여부를 체크한다. (조회, 탭 종료시)
        /// </summary>
        /// <param name="_enumClickedButtonKind">클릭 버튼 종류</param>
        /// <returns></returns>
        private bool CheckModifyData(BaseEnumClass.ClickedButtonKind _enumClickedButtonKind)
        {
            bool bRtnValue      = true;
            string strMessage   = string.Empty;
            //string strSelectedName = string.Empty;

            if (this.BackMstMgntList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                if (_enumClickedButtonKind == BaseEnumClass.ClickedButtonKind.SEARCH)
                {
                    // 조회 버튼 클릭
                    this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE_INQ");
                    bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
                }
                else
                {
                    // 탭 종료 버튼 클릭
                    this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE_TAB");
                    bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
                }

                if (bRtnValue == true) { this.BackMstMgntList.Clear(); }
            }
            
            return bRtnValue;
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

                iCheckedCount = this.BackMstMgntList.Where(p => p.IsSelected == true).Count();

                if (iCheckedCount == 0)
                {
                    this.BaseClass.MsgInfo("ERR_NO_SELECT");  // ERR_NO_SELECT : 선택된 데이터가 없습니다.
                    bRtnValue = false;
                }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> DeleteGridRowItem - 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// <summary>
        /// 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// </summary>
        private void DeleteGridRowItem()
        {
            //var DeleteRowItem = this.BackMstMgntList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();
            ////이미 등록된 데이터를 삭제하려고 할 때 에러메시지
            //if (DeleteRowItem.Count() <= 0)
            //{
            //    BaseClass.MsgError("ERR_DELETE");
            //}
            //DeleteRowItem.ForEach(p => BackMstMgntList.Remove(p));
        }

        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_BACKUP_LIST_INQ - 백업리스트 조회
        /// <summary>
        /// 백업리스트 조회
        /// </summary>
        /// <returns></returns>
        private DataSet GetSP_BACKUP_LIST_INQ()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_C1027.SP_BACKUP_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_BACKUP_LIST", "O_RSLT" };

                var strTableName    = this.txtTableName.Text.Trim();        // 테이블명
                var strErrCode      = string.Empty;                         // 오류 코드
                var strErrMsg       = string.Empty;                         // 오류 메세지
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_TB_NM",            strTableName);     // 테이블명
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

        #region >> InsertSP_BACKUP_LIST_INS - 백업리스트 신규 저장
        /// <summary>
        /// 백업리스트 신규 저장
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_item">선택된 (저장 대상) 항목</param>
        /// <returns></returns>
        private bool InsertSP_BACKUP_LIST_INS(BaseDataAccess _da, BackMstMgnt _item)
        {
            try
            {
                bool isRtnValue = true;

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_C1027.SP_BACKUP_LIST_INS";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strOwner            = _item.OWNER;                                  // Ower
                var strTableName        = _item.TB_NM;                                  // 테이블명
                var strBackupYN         = _item.BK_YN_Checked == true ? "Y" : "N";      // 백업여부
                var strDelYN            = _item.DEL_YN_Checked == true ? "Y" : "N";     // 삭제여부
                var strUseYN            = _item.USE_YN_Checked == true ? "Y" : "N";     // 사용여부
                var strBackupCond       = _item.BK_COND;                                // 백업조건
                var strBackupOwner      = _item.BK_OWNER;                               // 백업 Owner
                var iBackupOrder        = _item.BK_SEQ;                                 // 백업순서
                var strBackupTableName  = _item.BK_TB_NM;                               // 백업 테이블명
                var strRebuildYN        = _item.REBUILD_YN_Checked == true ? "Y" : "N"; // 백업후 리빌드 여부
                var strUserID           = this.BaseClass.UserID;                        // 사용자 ID

                var strErrCode      = string.Empty;                         // 오류 코드
                var strErrMsg       = string.Empty;                         // 오류 메세지
                #endregion

                #region Input 파라메터       
                dicInputParam.Add("P_OWNER",            strOwner);              // Owner
                dicInputParam.Add("P_TB_NM",            strTableName);          // 테이블명
                dicInputParam.Add("P_BK_YN",            strBackupYN);           // 백업여부
                dicInputParam.Add("P_DEL_YN",           strDelYN);              // 삭제여부
                dicInputParam.Add("P_USE_YN",           strUseYN);              // 사용여부
                dicInputParam.Add("P_BK_COND",          strBackupCond);         // 백업조건
                dicInputParam.Add("P_BK_OWNER",         strBackupOwner);        // 백업 Owner
                dicInputParam.Add("P_BK_SEQ",           iBackupOrder);          // 백업순서
                dicInputParam.Add("P_BK_TB_NM",         strBackupTableName);    // 백업 테이블명
                dicInputParam.Add("P_REBUILD_YN",       strRebuildYN);          // 백업후 리빌드 여부
                dicInputParam.Add("P_USER_ID",          strUserID);             // 사용자 ID
                #endregion

                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                        {
                            this.BaseClass.MsgInfo(dtRtnValue.Rows[0]["MSG"].ToString());
                            isRtnValue = false;
                        }
                    }
                    else
                    {
                        this.BaseClass.MsgInfo("ERR_SAVE"); //CMPT_SAVE : 저장 중 오류가 발생했습니다.
                        isRtnValue = false;
                    }
                }

                return isRtnValue;
            }
            catch { throw; }
        }
        #endregion

        private bool UpdateSP_BACKUP_LIST_UPD(BaseDataAccess _da, BackMstMgnt _item)
        {
            try
            {
                bool isRtnValue = true;

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_C1027.SP_BACKUP_LIST_UPD";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strOwner            = _item.OWNER;                                  // Ower
                var strTableName        = _item.TB_NM;                                  // 테이블명
                var strBackupYN         = _item.BK_YN_Checked == true ? "Y" : "N";      // 백업여부
                var strDelYN            = _item.DEL_YN_Checked == true ? "Y" : "N";     // 삭제여부
                var strUseYN            = _item.USE_YN_Checked == true ? "Y" : "N";     // 사용여부
                var strBackupCond       = _item.BK_COND;                                // 백업조건
                var strBackupOwner      = _item.BK_OWNER;                               // 백업 Owner
                var iBackupOrder        = _item.BK_SEQ;                                 // 백업순서
                var strBackupTableName  = _item.BK_TB_NM;                               // 백업 테이블명
                var strRebuildYN        = _item.REBUILD_YN_Checked == true ? "Y" : "N"; // 백업후 리빌드 여부
                var strUserID           = this.BaseClass.UserID;                        // 사용자 ID

                var strErrCode      = string.Empty;                         // 오류 코드
                var strErrMsg       = string.Empty;                         // 오류 메세지
                #endregion

                #region Input 파라메터       
                dicInputParam.Add("P_OWNER",            strOwner);              // Owner
                dicInputParam.Add("P_TB_NM",            strTableName);          // 테이블명
                dicInputParam.Add("P_BK_YN",            strBackupYN);           // 백업여부
                dicInputParam.Add("P_DEL_YN",           strDelYN);              // 삭제여부
                dicInputParam.Add("P_USE_YN",           strUseYN);              // 사용여부
                dicInputParam.Add("P_BK_COND",          strBackupCond);         // 백업조건
                dicInputParam.Add("P_BK_OWNER",         strBackupOwner);        // 백업 Owner
                dicInputParam.Add("P_BK_TB_NM",         strBackupTableName);    // 백업 테이블명
                dicInputParam.Add("P_BK_SEQ",           iBackupOrder);          // 백업순서
                dicInputParam.Add("P_REBUILD_YN",       strRebuildYN);          // 백업후 리빌드 여부
                dicInputParam.Add("P_USER_ID",          strUserID);             // 사용자 ID
                #endregion

                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                        {
                            this.BaseClass.MsgInfo(dtRtnValue.Rows[0]["MSG"].ToString());
                            isRtnValue = false;
                        }
                    }
                    else
                    {
                        this.BaseClass.MsgInfo("ERR_SAVE"); //CMPT_SAVE : 저장 중 오류가 발생했습니다.
                        isRtnValue = false;
                    }
                }

                return isRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void C1027_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 백업관리
        #region >> 버튼 클릭 이벤트
        #region + 백업 관리 조회버튼 클릭 이벤트
        /// <summary>
        /// 백업 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var query = this.BackMstMgntList.Where(p => p.IsUpdate).Count();
                if (query > 0)
                {
                    // ASK_EXISTS_NO_SAVE_TO_SEARCH - 저장되지 않은 데이터가 있습니다.|계속 조회하시겠습니까?
                    this.BaseClass.MsgQuestion("ASK_EXISTS_NO_SAVE_TO_SEARCH");
                    if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }
                }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 센터 데이터 조회
                DataSet dsRtnValue = this.GetSP_BACKUP_LIST_INQ();

                if (dsRtnValue == null) { return; }

                if (dsRtnValue.Tables[0].Rows.Count > 0)
                {
                    //dsRtnValue = this.ModifyTableData(dsRtnValue);
                }

                var strErrCode  = string.Empty;
                var strErrMsg   = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.BackMstMgntList = new ObservableCollection<BackMstMgnt>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.BackMstMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.BackMstMgntList.ToObservableCollection(null);
                    this.BaseClass.MsgInfo(strErrMsg);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.BackMstMgntList;
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

        #region + 백업 관리 저장 버튼 클릭 이벤트
        /// <summary>
        /// 백업 관리 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void BtnSave_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                bool isRtnValue = false;

                this.BackMstMgntList.ForEach(p => p.ClearError());

                // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                string strInputMessage = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.MESSAGE);
                var liSelectedRowData = this.BackMstMgntList.Where(p => p.IsSelected == true).ToList();

                foreach (var item in liSelectedRowData)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        // 테이블 OWNER
                        if (string.IsNullOrWhiteSpace(item.TB_NM) == true)
                        {
                            item.CellError("TB_OWNER", string.Format(strInputMessage, this.BaseClass.GetResourceValue("TBL_OWNER")));
                            return;
                        }

                        // 테이블
                        if (string.IsNullOrWhiteSpace(item.TB_NM) == true)
                        {
                            item.CellError("TBL", string.Format(strInputMessage, this.BaseClass.GetResourceValue("TBL")));
                            return;
                        }

                        // 백업조건
                        if (string.IsNullOrWhiteSpace(item.BK_COND) == true)
                        {
                            item.CellError("TBL_COND", string.Format(strInputMessage, this.BaseClass.GetResourceValue("BK_COND")));
                            return;
                        }

                        // 백업테이블 OWNER
                        if (string.IsNullOrWhiteSpace(item.BK_OWNER) == true)
                        {
                            item.CellError("BK_TBL_OWNER", string.Format(strInputMessage, this.BaseClass.GetResourceValue("BK_TBL_OWNER")));
                            return;
                        }

                        // 백업테이블
                        if (string.IsNullOrWhiteSpace(item.BK_TB_NM) == true)
                        {
                            item.CellError("BK_TBL", string.Format(strInputMessage, this.BaseClass.GetResourceValue("BK_TBL")));
                            return;
                        }
                    }
                }

                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            if (item.IsNew == true)
                            {
                                isRtnValue = this.InsertSP_BACKUP_LIST_INS(da, item);
                            }
                            else
                            {
                                isRtnValue = this.UpdateSP_BACKUP_LIST_UPD(da, item);
                            }

                            if (isRtnValue == false)
                            {
                                break;
                            }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우
                            da.CommitTransaction();

                            this.BaseClass.MsgInfo("CMPT_SAVE"); //CMPT_SAVE : 저장되었습니다.
                            //this.GetSP_CNTR_LIST_INQ();

                            foreach (var item in liSelectedRowData)
                            {
                                //item.IsSaved = true;
                                item.IsSelected = false;
                            }

                            //저장된 데이터 포함하여 데이터 조회
                            DataSet dsRtnValue = this.GetSP_BACKUP_LIST_INQ();

                            if (dsRtnValue == null) { return; }

                            if (dsRtnValue.Tables[0].Rows.Count > 0)
                            {
                                //dsRtnValue = this.ModifyTableData(dsRtnValue);
                            }

                            this.BackMstMgntList = new ObservableCollection<BackMstMgnt>();
                            this.BackMstMgntList.ToObservableCollection(dsRtnValue.Tables[0]);

                            this.gridMaster.ItemsSource = this.BackMstMgntList;

                            // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                            this.SetResultText();
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

                        this.BaseClass.MsgInfo("ERR_SAVE"); //CMPT_SAVE : 저장 중 오류가 발생했습니다.
                        throw;
                    }
                    finally
                    {
                        // 상태바 (아이콘) 제거
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

        #region + 행추가 버튼 클릭 이벤트
        private void BtnRowAdd_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var newItem = new BackMstMgnt
                {
                    //CNTR_CD                 = string.Empty,
                    //CNTR_NM                 = string.Empty,
                    ////DB_CONN_TYPE = string.Empty,
                    ////ORCL_CONN_STR = string.Empty,
                    ////MS_CONN_STR = string.Empty,
                    ////MR_CONN_STR = string.Empty,
                    //FR_CURR_DATE            = DateTime.Now,
                    //FR_INIT_YMD_DIFF        = 0,
                    //FR_INIT_HM              = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")),
                    //TO_CURR_DATE            = DateTime.Now,
                    //TO_INIT_YMD_DIFF        = 0,
                    //TO_INIT_HM              = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")),
                    //ADDR                    = string.Empty,
                    //TEL_NO                  = string.Empty,
                    //USE_YN                  = "Y",
                    //IsSelected              = true,
                    //IsNew                   = true,
                    //IsSaved                 = false

                    OWNER               = string.Empty,     // 테이블 OWNER
                    TB_NM               = string.Empty,     // 테이블
                    BK_YN               = "Y",              // 백업여부
                    DEL_YN              = "N",              // 삭제여부
                    BK_COND             = string.Empty,     // 백업조건
                    BK_TB_NM            = string.Empty,     // 백업 테이블
                    BK_SEQ              = 1,                // 백업순서
                    REBUILD_YN          = "N",              // 백업후 리빌드 여부
                    USE_YN              = "Y",              // 사용여부
                    IsSelected          = true,
                    IsNew               = true
                };

                this.BackMstMgntList.Add(newItem);
                this.gridMaster.Focus();
                this.gridMaster.CurrentColumn = this.gridMaster.Columns.First();
                this.gridMaster.View.FocusedRowHandle = this.BackMstMgntList.Count - 1;

                this.BackMstMgntList[this.BackMstMgntList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
                this.BackMstMgntList[this.BackMstMgntList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region + 행삭제 버튼 클릭 이벤트
        /// <summary>
        /// 행삭제 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowDelete_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                //이미 등록된 데이터를 삭제하려고 할 때 에러메시지

                // 행추가된 그리드 Row중 선택된 Row를 삭제한다.
                this.DeleteGridRowItem();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region >> 그리드 관련 이벤트
        #region + 그리드 클릭 이벤트
        /// <summary>
        /// 그리드 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMaster_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view = (sender as GridControl).View as TableView;
                var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);
                if (hi.InRowCell)
                {
                    if (view.ActiveEditor == null)
                    {
                        view.ShowEditor();

                        if (view.ActiveEditor == null) { return; }
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                        }), DispatcherPriority.Render);
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

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
        private static void view_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            try
            {
                if (g_IsAuthAllYN == false)
                {
                    e.Cancel = true;
                    return;
                }

                TableView tv = sender as TableView;
                BackMstMgnt dataMember = tv.Grid.CurrentItem as BackMstMgnt;

                if (dataMember == null) { return; }


                switch (e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    // 센터코드, DB 접속 정보 컬럼은 수정이 되지 않도록 처리한다.
                    case "OWNER":
                    case "TB_NM":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    default: break;
                }
            }
            catch { throw; }
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
