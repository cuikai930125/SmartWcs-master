using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.COMMON.DataMembers.C1002;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SMART.WCS.UI.Common.Views.SYS_MGMT
{
    /// <summary>
    /// 사용자 관리 및 사용자 별 담당 설비 관리
    /// </summary>
    public partial class C1002 : UserControl
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

        public TextBlock g_ctrlTextBlock = new TextBlock();
        #endregion

        #region ▩ 생성자 
        public C1002()
        {
            InitializeComponent();
        }

        public C1002(bool _isFlag)
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">메뉴 네비게이션 정보</param>
        public C1002(List<string> _liMenuNavigation)
        {
            InitializeComponent();

            try
            {
                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 상단에 설명 
                this.CommentArea.ScreenID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();

                this.RoleCD_ComboBox = this.BaseClass.CenterCD;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ Enum
        private enum ScreenGridKind
        {
            USER_MGT            = 0,
            EQP_LEFT_TO_RIGHT   = 1,
            EQP_RIGHT_TO_LEFT   = 2
        };
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1002), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 사용자 관리
        public static readonly DependencyProperty UserMgntListProperty
            = DependencyProperty.Register("UserMgntList", typeof(ObservableCollection<UserMgnt>), typeof(C1002)
                , new PropertyMetadata(new ObservableCollection<UserMgnt>()));

        public ObservableCollection<UserMgnt> UserMgntList
        {
            get { return (ObservableCollection<UserMgnt>)GetValue(UserMgntListProperty); }
            set { SetValue(UserMgntListProperty, value); }
        }
        #endregion

        #region > 담당 설비
        public static readonly DependencyProperty EqpRightToLeftListProperty
            = DependencyProperty.Register("EqpRightToLeftList", typeof(ObservableCollection<UserPerEqp>), typeof(C1002)
                , new PropertyMetadata(new ObservableCollection<UserPerEqp>()));

        public ObservableCollection<UserPerEqp> EqpRightToLeftList
        {
            get { return (ObservableCollection<UserPerEqp>)GetValue(EqpRightToLeftListProperty); }
            set { SetValue(EqpRightToLeftListProperty, value); }
        }
        #endregion

        #region > 미담당 설비
        public static readonly DependencyProperty EqpLeftToRightListProperty
            = DependencyProperty.Register("EqpLeftToRightList", typeof(ObservableCollection<UserPerEqp>), typeof(C1002)
                , new PropertyMetadata(new ObservableCollection<UserPerEqp>()));

        public ObservableCollection<UserPerEqp> EqpLeftToRightList
        {
            get { return (ObservableCollection<UserPerEqp>)GetValue(EqpLeftToRightListProperty); }
            set { SetValue(EqpLeftToRightListProperty, value); }
        }
        #endregion
   
        #region > Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1002), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string GridRowCount
        {
            get { return (string)GetValue(GridRowCountProperty); }
            set { SetValue(GridRowCountProperty, value); }
        }
        #endregion

        public string RoleCD_ComboBox { get; set; }
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        private void InitControl()
        {
            // 콤보박스 설정
            this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, true);
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 버튼 클릭 이벤트
            // 조회 버튼 클릭 이벤트
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSEARCH_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드 버튼 클릭 이벤트
            //this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            // 저장 버튼 클릭 이벤트
            this.btnSAVE.PreviewMouseLeftButtonUp += BtnSAVE_PreviewMouseLeftButtonUp;

            // 담당 설비 추가 버튼 클릭 이벤트 (▶)
            this.btnLeftToRight.PreviewMouseLeftButtonUp += BtnLeftToRight_PreviewMouseLeftButtonUp;
            // 담당 설비 제거 버튼 클릭 이벤트 (◀)
            this.btnRightToLeft.PreviewMouseLeftButtonUp += BtnRightToLeft_PreviewMouseLeftButtonUp;

            // 그리드 Row추가 버튼 클릭 이벤트
            this.btnRowAdd.PreviewMouseLeftButtonUp += BtnRowAdd_PreviewMouseLeftButtonUp;
            // 그리드 추가Row 삭제 버튼 클릭 이벤트
            this.btnRowDelete.PreviewMouseLeftButtonUp += BtnRowDelete_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 이벤트
            this.gridLeft_UserList.PreviewMouseLeftButtonUp += GridLeft_UserList_PreviewMouseLeftButtonUp;
            #endregion
        }
        #endregion
        #endregion

        #region > 기타 함수
        #region >> SetResultText - 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        /// <param name="_iTabIndex">Tab Index값</param>
        private void SetResultText()
        {
            var strResource                 = this.BaseClass.GetResourceValue("TOT_DATA_CNT");              // 텍스트 리소스
            var iTotalRowCount              = (this.gridLeft_UserList.ItemsSource as ICollection).Count;    // 전체 데이터 수
            this.GridRowCount               = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";        // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource                     = this.BaseClass.GetResourceValue("DATA_INQ");                  // 건의 데이터가 조회되었습니다.
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
            return this.CheckModifyData();
        }
        #endregion

        #region >> CheckModifyData - 각 탭의 데이터 저장 여부를 체크한다.
        /// <summary>
        /// 각 탭의 데이터 저장 여부를 체크한다.
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyData()
        {
            bool bRtnValue              = true;
            string strMessage           = string.Empty;
            //string strSelectedName      = string.Empty;

            if (this.UserMgntList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                strMessage          = this.BaseClass.GetResourceValue("ERR_EXISTS_NO_SAVE");
                bRtnValue           = false;
            }

            if (bRtnValue == false)
            {
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;

                if (bRtnValue == true)
                {
                    this.UserMgntList.Clear();
                    this.EqpRightToLeftList.Clear();
                    this.EqpRightToLeftList.Clear();
                }
            }

            return bRtnValue;
        }
        #endregion

        #region >> CheckGridRowSelected - 그리드 Row 수정 여부 
        /// <summary>
        /// 그리드 Row 수정 여부 
        /// </summary>
        /// <param name="_enumScreenGridKind">그리드 종류</param>
        /// <returns></returns>
        private bool CheckGridRowSelected(ScreenGridKind _enumScreenGridKind)
        {
            bool isRtnValue = true;
            string strMessage = string.Empty;
            int iCheckedCount = 0;

            switch (_enumScreenGridKind)
            {
                case ScreenGridKind.USER_MGT:
                    iCheckedCount = this.UserMgntList.Where(p => p.IsSelected == true).Count();
                    break;
                case ScreenGridKind.EQP_LEFT_TO_RIGHT:
                    iCheckedCount = this.EqpLeftToRightList.Where(p => p.IsSelected == true).Count();
                    break;
                case ScreenGridKind.EQP_RIGHT_TO_LEFT:
                    iCheckedCount = this.EqpRightToLeftList.Where(p => p.IsSelected == true).Count();
                    break;
            }

            if (iCheckedCount == 0)
            {
                // ERR_NO_SELECT - 선택된 데이터가 없습니다.
                this.BaseClass.MsgError("ERR_NO_SELECT");

                isRtnValue = false;
            }

            return isRtnValue;
        }
        #endregion

        #region >> DeleteGridRowItem - 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// <summary>
        /// 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// </summary>
        private void DeleteGridRowItem()
        {
            this.UserMgntList.Where(p => p.IsSelected == true && p.IsNew == true).ToList().ForEach(p =>
            {
                this.UserMgntList.Remove(p);
            });
        }
        #endregion
        #endregion

        #region > Validation - 유효성 검사 
        #region >> ValidationSearchUser - 사용자 관리 리스트 조회시 조건 유효성 체크
        /// <summary>
        /// 사용자 관리 리스트 조회시 조건 유효성 체크
        /// </summary>
        /// <returns></returns>
        private bool ValidationSearchUser()
        {
            if (this.txtUserID.Text.Trim().Length == 0)
            {
                // ERR_EMPTY_USER_ID - 사용자 ID를 입력하세요.
                this.BaseClass.MsgError("ERR_EMPTY_USER_ID");
                return false;
            }

            if (this.txtUserNM.Text.Trim().Length == 0)
            {
                // ERR_EMPTY_USER_NM - 사용자명을 입력하세요.
                this.BaseClass.MsgError("ERR_EMPTY_USER_NM");
                return false;
            }

            return true;
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_USER_LIST_INQ - 사용자 관리 리스트 조회
        /// <summary>
        /// 사용자 관리 리스트 조회
        /// </summary>
        /// <returns></returns>
        private async Task GetSP_USER_LIST_INQ()
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1002.SP_USER_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_USER_LIST", "O_RSLT" };

            var strCenterCD     = this.BaseClass.CenterCD;                                  // 센터코드
            var strMgmtUserID   = this.txtUserID.Text.Trim();                               // 사용자 ID
            var strUserNM       = this.txtUserNM.Text.Trim();                               // 사용자명
            var strUseYN        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);   // 사용여부
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCenterCD);       // 센터코드
            dicInputParam.Add("P_MGMT_USER_ID",     strMgmtUserID);     // 사용자 ID
            dicInputParam.Add("P_USER_NM",          strUserNM);         // 사용자명
            dicInputParam.Add("P_USE_YN",           strUseYN);          // 사용여부
            #endregion

            #region + 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            #region > 사용자 관리 
            if (dsRtnValue == null) { return; }
                
            var strErrCode          = string.Empty;     // 오류 코드
            var strErrMsg           = string.Empty;     // 오류 메세지

            if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
            {
                // 정상 처리된 경우
                this.UserMgntList = new ObservableCollection<UserMgnt>();
                this.UserMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
            }
            else
            {
                // 오류가 발생한 경우
                this.UserMgntList.ToObservableCollection(null);
                this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
            }

            this.gridLeft_UserList.ItemsSource = this.UserMgntList;

            if (this.UserMgntList.Count > 0)
            {
                this.BaseClass.SetGridRowAddFocuse(this.gridLeft_UserList, 0);
            }

            this.SetResultText();
            #endregion
        }
        #endregion

        #region >> InsertSP_USER_INS - 사용자 관리 신규 저장
        /// <summary>
        /// 사용자 관리 신규 저장
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_item">사용자 관리 데이터</param>
        /// <returns></returns>
        private async Task<bool> InsertSP_USER_INS(BaseDataAccess _da, UserMgnt _item)
        {
            bool isRtnValue     = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_C1002.SP_USER_INS";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                      // 센터코드
            var strMgmtUserID       = _item.USER_ID;                                // 사용자 ID
            var strUserNM           = _item.USER_NM;                                // 사용자명
            var strRoleCD           = _item.ROLE_CD;                                // 권한코드
            var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";     // 사용여부
            var strUserID           = this.BaseClass.UserID;                        // 사용자 ID
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_MGMT_USER_ID",         strMgmtUserID);     // 사용자 ID
            dicInputParam.Add("P_USER_NM",              strUserNM);         // 사용자명
            dicInputParam.Add("P_ROLE_CD",              strRoleCD);         // 권한코드
            dicInputParam.Add("P_USE_YN",               strUseYN);          // 사용여부
            dicInputParam.Add("P_USER_ID",              strUserID);         // 사용자 ID
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);
            #endregion

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    // ERR_SAVE - 저장 중 오류가 발생했습니다.
                    this.BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }
        #endregion

        private bool InsertSP_USER_INS2(BaseDataAccess _da, UserMgnt _item)
        {
            bool isRtnValue     = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_C1002.SP_USER_INS";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                      // 센터코드
            var strMgmtUserID       = _item.USER_ID;                                // 사용자 ID
            var strUserNM           = _item.USER_NM;                                // 사용자명
            var strRoleCD           = _item.ROLE_CD;                                // 권한코드
            var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";     // 사용여부
            var strUserID           = this.BaseClass.UserID;                        // 사용자 ID
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_MGMT_USER_ID",         strMgmtUserID);     // 사용자 ID
            dicInputParam.Add("P_USER_NM",              strUserNM);         // 사용자명
            dicInputParam.Add("P_ROLE_CD",              strRoleCD);         // 권한코드
            dicInputParam.Add("P_USE_YN",               strUseYN);          // 사용여부
            dicInputParam.Add("P_USER_ID",              strUserID);         // 사용자 ID
            #endregion

            #region + 데이터 조회
            dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            #endregion

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    // ERR_SAVE - 저장 중 오류가 발생했습니다.
                    this.BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }

        #region >> UpdateSP_USER_UPD - 사용자 관리 데이터 수정
        /// <summary>
        /// 사용자 관리 데이터 수정
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_item">수정 대상 데이터</param>
        /// <returns></returns>
        private async Task<bool> UpdateSP_USER_UPD(BaseDataAccess _da, UserMgnt _item)
        {
            bool isRtnValue         = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_C1002.SP_USER_UPD";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                              // 센터코드
            var strMgmtUserID       = _item.USER_ID;                                        // 사용자 ID
            var strUserNM           = _item.USER_NM;                                        // 사용자명
            var strRoleCD           = _item.ROLE_CD;                                        // 권한코드
            var strUseYN            = _item.USE_YN_CHECKED == true ? "Y" : "N";             // 사용여부
            var strPwdInitYN        = _item.PWD_INIT_YN_CHECKED == true ? "Y" : "N";        // 비밀번호 초기화 여부
            var strUserID           = this.BaseClass.UserID;                                // 사용자 ID
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_MGMT_USER_ID",         strMgmtUserID);         // 사용자 ID
            dicInputParam.Add("P_USER_NM",              strUserNM);         // 사용자명
            dicInputParam.Add("P_ROLE_CD",              strRoleCD);         // 권한코드
            dicInputParam.Add("P_USE_YN",               strUseYN);          // 사용여부
            dicInputParam.Add("P_PWD_INIT_YN",          strPwdInitYN);      // 비밀번호 초기화 여부
            dicInputParam.Add("P_USER_ID",              strUserID);         // 사용자 ID
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);
            #endregion

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    // ERR_SAVE - 저장 중 오류가 발생했습니다.
                    this.BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }
        #endregion

        private bool UpdateSP_USER_UPD2(BaseDataAccess _da, UserMgnt _item)
        {
            bool isRtnValue = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1002.SP_USER_UPD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strCenterCD = this.BaseClass.CenterCD;                              // 센터코드
            var strMgmtUserID = _item.USER_ID;                                        // 사용자 ID
            var strUserNM = _item.USER_NM;                                        // 사용자명
            var strRoleCD = _item.ROLE_CD;                                        // 권한코드
            var strUseYN = _item.USE_YN_CHECKED == true ? "Y" : "N";             // 사용여부
            var strPwdInitYN = _item.PWD_INIT_YN_CHECKED == true ? "Y" : "N";        // 비밀번호 초기화 여부
            var strUserID = this.BaseClass.UserID;                                // 사용자 ID
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCenterCD);       // 센터코드
            dicInputParam.Add("P_MGMT_USER_ID", strMgmtUserID);         // 사용자 ID
            dicInputParam.Add("P_USER_NM", strUserNM);         // 사용자명
            dicInputParam.Add("P_ROLE_CD", strRoleCD);         // 권한코드
            dicInputParam.Add("P_USE_YN", strUseYN);          // 사용여부
            dicInputParam.Add("P_PWD_INIT_YN", strPwdInitYN);      // 비밀번호 초기화 여부
            dicInputParam.Add("P_USER_ID", strUserID);         // 사용자 ID
            #endregion

            #region + 데이터 조회
            dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            #endregion

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    // ERR_SAVE - 저장 중 오류가 발생했습니다.
                    this.BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }

        #region >> GetSP_USER_EQP_LIST_INQ_TOT - 사용자 별 설비 정보 조회
        /// <summary>
        /// 사용자 별 설비 정보 조회
        /// </summary>
        /// <param name="_strUserID"></param>
        /// <returns></returns>
        private async Task GetSP_USER_EQP_LIST_INQ_TOT(string _strUserID)
        {
            try
            {
                //// 상태바 (아이콘) 실행
                //this.loadingScreen.IsSplashScreenShown = true;

                DataSet dsEqpList       = await this.GetSP_USER_EQP_LIST_INQ(_strUserID);

                if (dsEqpList == null) { return; }

                var strErrCode          = string.Empty;     // 오류 코드
                var strErrMsg           = string.Empty;     // 오류 메세지

                if (this.BaseClass.CheckResultDataProcess(dsEqpList, ref strErrCode, ref strErrMsg) == true)
                {
                    this.EqpLeftToRightList = new ObservableCollection<UserPerEqp>();
                    this.EqpLeftToRightList.ToObservableCollection(dsEqpList.Tables[0]);
                    this.gridRightLeft_CharEqp.ItemsSource = this.EqpLeftToRightList;

                    // 정상 처리된 경우
                    this.EqpRightToLeftList = new ObservableCollection<UserPerEqp>();
                    this.EqpRightToLeftList.ToObservableCollection(dsEqpList.Tables[1]);
                    this.gridRightRight_NotCharEqp.ItemsSource = this.EqpRightToLeftList;
                    
                }
                else
                {
                    this.EqpRightToLeftList.ToObservableCollection(null);
                    this.EqpLeftToRightList.ToObservableCollection(null);
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                //// 상태바 (아이콘) 제거
                //this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        #region >> GetSP_USER_EQP_LIST_INQ - 사용자 별 설비 정보
        /// <summary>
        /// 사용자 별 설비 정보
        /// </summary>
        /// <returns></returns>
        private async Task<DataSet> GetSP_USER_EQP_LIST_INQ(string _strUserID)
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1002.SP_USER_EQP_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_USER_EQP_MPNG_LIST", "O_USER_EQP_NO_MPNG_LIST", "O_RSLT" };

            var strCenterCD     = this.BaseClass.CenterCD;                                  // 센터코드
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCenterCD);       // 센터코드
            dicInputParam.Add("P_MGMT_USER_ID",     _strUserID);     // 사용자 ID
            #endregion

            #region + 데이터 조회
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
        #endregion

        #region >> DeleteSP_USER_EQP_DEL - 맵핑되어 있는 설비 정보를 삭제
        /// <summary>
        /// 맵핑되어 있는 설비 정보를 삭제
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_strUserID">선택된 사용자 ID</param>
        /// <param name="_item">삭제 대상 설비 정보</param>
        /// <returns></returns>
        private async Task<bool> DeleteSP_USER_EQP_DEL(BaseDataAccess _da, string _strUserID, UserPerEqp _item)
        {
            bool isRtnValue         = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_C1002.SP_USER_EQP_DEL";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;      // 센터코드
            var strMgmtUserID       = _strUserID;                   // 사용자 ID
            var strEqpID            = _item.EQP_ID;                 // 설비 ID
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_MGMT_USER_ID",         _strUserID);        // 사용자 ID
            dicInputParam.Add("P_EQP_ID",               strEqpID);          // 설비 ID
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);
            #endregion

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    // ERR_SAVE - 저장 중 오류가 발생했습니다.
                    this.BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }
        #endregion

        #region >> InsertSP_USER_EQP_INS - 맵핑되어있지 않은 설비 정보를 추가
        /// <summary>
        /// 맵핑되어있지 않은 설비 정보를 추가
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_strUserID">선택된 사용자 ID</param>
        /// <param name="_item">삭제 대상 설비 정보</param>
        /// <returns></returns>
        private async Task<bool> InsertSP_USER_EQP_INS(BaseDataAccess _da, string _strUserID, UserPerEqp _item)
        {
            bool isRtnValue         = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_C1002.SP_USER_EQP_INS";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;      // 센터코드
            var strMgmtUserID       = _strUserID;                   // 사용자 ID
            var strEqpID            = _item.EQP_ID;                 // 설비 ID
            var strUserID           = this.BaseClass.UserID;        // 로그인 사용자 ID
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_MGMT_USER_ID",         strMgmtUserID);     // 사용자 ID
            dicInputParam.Add("P_EQP_ID",               strEqpID);          // 설비 ID
            dicInputParam.Add("P_USER_ID",              strUserID);         // 사용자 ID
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);
            #endregion

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    // ERR_SAVE - 저장 중 오류가 발생했습니다.
                    this.BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 버튼 클릭 이벤트
        #region >> 조회 버튼 클릭 이벤트
        /// <summary>
        /// 조회 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSEARCH_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 사용자 관리 리스트 조회
                await this.GetSP_USER_LIST_INQ();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                if (this.UserMgntList.Count > 0)
                {
                    var liUserList = this.UserMgntList.FirstOrDefault();

                    if (liUserList.USER_ID.Length > 0)
                    {
                        await this.GetSP_USER_EQP_LIST_INQ_TOT(liUserList.USER_ID);
                    }
                }
                else
                {
                    this.gridRightLeft_CharEqp.ItemsSource      = null;
                    this.gridRightRight_NotCharEqp.ItemsSource  = null;
                }

                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

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
                tv.Add(this.tvLeftGrid_UserList);
                tv.Add(this.tvRightLeftGrid_CharEqp);
                tv.Add(this.tvRightRightGrid_NotCharEqp);
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

        #region >> 저장 버튼 클릭 이벤트
        /// <summary>
        /// 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSAVE_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 Row 수정 여부 체크
                if (this.CheckGridRowSelected(ScreenGridKind.USER_MGT) == false) { return; }

                var liSelectedItems = this.UserMgntList.Where(p => p.IsSelected == true).ToList();
                if (liSelectedItems.Count() == 0)
                {
                    this.BaseClass.MsgError("ERR_NOT_SELECT");
                    return;
                }


                // ASK_SAVE - 저장 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;
                // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                string strInputMessage = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.MESSAGE);
                // ERR_NOT_SELECT - {0}이(가) 선택되지 않았습니다.
                string strSelectMessage = this.BaseClass.GetResourceValue("ERR_NOT_SELECT", BaseEnumClass.ResourceType.MESSAGE);

                this.UserMgntList.ForEach(p => p.ClearError());

                foreach (var item in this.UserMgntList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.USER_ID) == true)
                        {
                            item.CellError("USER_ID", string.Format(strInputMessage, this.BaseClass.GetResourceValue("USER_ID")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.USER_NM) == true)
                        {
                            item.CellError("USER_NM", string.Format(strInputMessage, this.BaseClass.GetResourceValue("USER_NM")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.ROLE_CD) == true)
                        {
                            item.CellError("ROLE_CD", string.Format(strSelectMessage, this.BaseClass.GetResourceValue("ROLE_CD")));
                            return;
                        }
                    }
                }

                var liSelectedRowData = this.UserMgntList.Where(p => p.IsSelected == true).ToList();

                #region + 데이터 저장
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        // 트랜잭션 시작
                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            if (item.IsNew == true)
                            {
                                //isRtnValue = await this.InsertSP_USER_INS(da, item);
                                isRtnValue = this.InsertSP_USER_INS2(da, item);
                                if (isRtnValue == false) { break; }
                            }
                            else if (item.IsUpdate == true)
                            {
                                isRtnValue = this.UpdateSP_USER_UPD2(da, item);
                                if (isRtnValue == false) { break; }
                            }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();
                            // 상태바 (아이콘) 제거
                            this.loadingScreen.IsSplashScreenShown = false;
                            // CMPT_SAVE - 저장 되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");

                            // 사용자 리스트를 재조회한다.
                            this.BtnSEARCH_PreviewMouseLeftButtonUp(null, null);
                        }
                        else
                        {
                            // ERR_SAVE - 저장 중 오류가 발생 했습니다.
                            this.BaseClass.MsgError("ERR_SAVE");
                        }
                    }
                    catch { throw; }
                    finally
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = false;
                    }
                }
                #endregion
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 담당 설비 추가 버튼 클릭 이벤트
        /// <summary>
        /// 담당 설비 추가 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnLeftToRight_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 Row 수정 여부 체크
                if (this.CheckGridRowSelected(ScreenGridKind.EQP_LEFT_TO_RIGHT) == false) { return; }

                // ASK_EQP_MAPP_DEL - 설비 매핑을 삭제하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EQP_MAPP_DEL");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;

                // 사용자 리스트 그리드에서 현재 선택된 사용자
                var strSelectedUserID = ((UserMgnt)this.gridLeft_UserList.SelectedItem).USER_ID;

                this.EqpLeftToRightList.ForEach(p => p.ClearError());

                var liSelectedRowData = this.EqpLeftToRightList.Where(p => p.IsSelected == true).ToList();

                #region + 데이터 저장
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        // 트랜잭션 시작
                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            isRtnValue = await this.DeleteSP_USER_EQP_DEL(da, strSelectedUserID, item);
                            if (isRtnValue == false) { break; }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();

                            // CMPT_EQP_MAPP_DEL - 설비 매핑이 삭제되었습니다.
                            this.BaseClass.MsgInfo("CMPT_EQP_MAPP_DEL");

                            // 사용자 매핑 정보를 재조회한다.
                            await this.GetSP_USER_EQP_LIST_INQ_TOT(strSelectedUserID);
                        }
                        //else
                        //{
                        //    // ERR_SAVE - 저장 중 오류가 발생 했습니다.
                        //    this.BaseClass.MsgError("ERR_SAVE");
                        //}
                    }
                    catch { throw; }
                    finally
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        // 상태바 (아이콘) 제거
                        this.loadingScreen.IsSplashScreenShown = false;
                    }
                }
                #endregion
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 담당 설비 제거 버튼 클릭 이벤트
        /// <summary>
        /// 담당 설비 제거 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnRightToLeft_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 Row 수정 여부 체크
                if (this.CheckGridRowSelected(ScreenGridKind.EQP_RIGHT_TO_LEFT) == false) { return; }

                // ASK_EQP_MAPP_INS - 설비 매핑을 추가하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EQP_MAPP_INS");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;

                // 사용자 리스트 그리드에서 현재 선택된 사용자
                var strSelectedUserID = ((UserMgnt)this.gridLeft_UserList.SelectedItem).USER_ID;

                this.EqpRightToLeftList.ForEach(p => p.ClearError());

                var liSelectedRowData = this.EqpRightToLeftList.Where(p => p.IsSelected == true).ToList();

                #region + 데이터 저장
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        // 트랜잭션 시작
                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            isRtnValue = await this.InsertSP_USER_EQP_INS(da, strSelectedUserID, item);
                            if (isRtnValue == false) { break; }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();

                            // CMPT_EQP_MAPP_INS - 설비 매핑이 추가되었습니다.
                            this.BaseClass.MsgInfo("CMPT_EQP_MAPP_INS");

                            // 사용자 매핑 정보를 재조회한다.
                            await this.GetSP_USER_EQP_LIST_INQ_TOT(strSelectedUserID);
                        }
                        //else
                        //{
                        //    // ERR_SAVE - 저장 중 오류가 발생 했습니다.
                        //    this.BaseClass.MsgError("ERR_SAVE");
                        //}
                    }
                    catch { throw; }
                    finally
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        // 상태바 (아이콘) 제거
                        this.loadingScreen.IsSplashScreenShown = false;
                    }
                }
                #endregion
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 그리드 Row 추가 버튼 클릭 이벤트
        /// <summary>
        /// 그리드 Row 추가 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var newItme = new UserMgnt
                {
                    USER_ID                 = string.Empty
                    ,   USER_NM             = string.Empty
                    ,   ROLE_CD             = string.Empty
                    ,   USE_YN              = "Y"
                    ,   PWD_INIT_YN         = "Y"
                    ,   IsSelected          = true
                    ,   IsNew               = true
                };

                this.UserMgntList.Add(newItme);
                this.gridLeft_UserList.Focus();
                this.gridLeft_UserList.CurrentColumn            = this.gridLeft_UserList.Columns.First();
                this.gridLeft_UserList.View.FocusedRowHandle    = this.UserMgntList.Count - 1;

                this.UserMgntList[this.UserMgntList.Count - 1].BackgroundBrush        = new SolidColorBrush(Colors.White);
                this.UserMgntList[this.UserMgntList.Count - 1].BaseBackgroundBrush    = new SolidColorBrush(Colors.White);

                this.BaseClass.SetGridRowAddFocuse(this.gridLeft_UserList, this.UserMgntList.Count - 1);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 그리드 추가 Row 삭제 버튼 클릭 이벤트
        /// <summary>
        /// 그리드 추가 Row 삭제 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.CheckGridRowSelected(ScreenGridKind.USER_MGT) == false) { return; }

                // 행 추가된 그리드의 Row 중 선택된 Row를 삭제한다.
                this.DeleteGridRowItem();

                this.BaseClass.SetGridRowAddFocuse(this.gridLeft_UserList, this.UserMgntList.Count - 1);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion
        
        #region > 그리드 관련 이벤트
        #region >> 그리드 클릭 이벤트
        /// <summary>
        /// 그리드 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GridLeft_UserList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view        = (sender as GridControl).View as TableView;
                var hitInfo     = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hitInfo.Column == null) { return; }

                if (hitInfo.InRowCell)
                {
                    // 그리드 내 체크박스 선택 로직
                    switch (hitInfo.Column.FieldName)
                    {
                        case "USE_YN":
                        case "PWD_INIT_YN":
                            if (view.ActiveEditor == null)
                            {
                                view.ShowEditor();

                                if (view.ActiveEditor == null) { return; }
                                await Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                                }), DispatcherPriority.Render);
                            }

                            int iCurrentRowIndex = this.BaseClass.GetCurrentGridControlRowIndex(this.tvLeftGrid_UserList);
                            if (this.UserMgntList[iCurrentRowIndex].IsSelected == false) { }
                            break;
                    }

                    if (hitInfo.Column.FieldName.Trim().ToUpper().Equals("ISSELECTED") == false)
                    {
                        // 사용자 관리 그리드 중 선택된 Row의 사용자 ID
                        var strUserID = ((UserMgnt)this.gridLeft_UserList.SelectedItem).USER_ID;
                        if (strUserID.Length == 0) { return; }

                        await this.GetSP_USER_EQP_LIST_INQ_TOT(strUserID);
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

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

                TableView tv = sender as TableView;
                UserMgnt dataMember = tv.Grid.CurrentItem as UserMgnt;
                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    case "USER_ID":
                    case "USER_NM":
                        if (dataMember.IsNew == false)
                        {
                            if (dataMember.IsSelected == true) { dataMember.IsSelected = false; }
                            e.Cancel = true;
                        }
                        break;
                    case "INIT_YN":
                        if (dataMember.IsNew == true)
                        {
                            if (dataMember.IsSelected == true) { dataMember.IsSelected = false; }
                            e.Cancel = true;
                        }
                        break;
                }
            }
            catch { throw; }
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

        private void bcScanner_BarCodeReceived(object sender, Control.Barcode.BarCodeReceivedEventArgs e)
        {
            try
            {
                var strBarCode = e.BarCode;

                //this.bcScanner.CloseSerialScanner();

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
    }
}
