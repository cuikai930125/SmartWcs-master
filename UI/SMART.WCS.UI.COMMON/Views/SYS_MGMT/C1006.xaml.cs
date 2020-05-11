

using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.UI.COMMON.DataMembers.C1006;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SMART.WCS.UI.Common.Views.SYS_MGMT
{
    /// <summary>
    /// 공지사항 게시판 
    /// </summary>
    public partial class C1006 : UserControl, TabCloseInterface
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
        #region > BaseClass 생성
        BaseClass BaseClass = new BaseClass();
        #endregion

        #region > 공지사항 변수 선언
        // 그리드 클릭시 해당 공지사항 참조
        //NoticeInfoMgnt noticeInfoMgnt;
        #endregion

        #region > 플래그 리스트
        /// <summary>
        /// 데이터 수정 여부
        /// </summary>
        private bool g_isChanged    = false;

        /// <summary>
        /// 신규추가 여부
        /// </summary>
        private bool g_isNew        = false;
        #endregion
        #endregion

        #region ▩ Enum
        private enum ComboBoxKind : int
        {
            PRITY   = 0,
            USE_YN  = 1
        }
        #endregion

        #region ▩ 생성자

        #region > C1006생성자
        public C1006()
        {
            InitializeComponent();
        }


        public C1006(List<string> _liMenuNavigation)
        {
            DataContext = this;
            InitializeComponent();

            try
            {
                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 상단에 설명 
                this.CommentArea.ScreenID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 이벤트 초기화
                this.InitEvent();

                // 컨트롤 초기화
                this.InitControl();

                this.BtnSearch_PreviewMouseLeftButtonUp(null, null);
                //// 공지사항 입력란 비활성화
                //DisableNotiInfoStackPanel();
            }
            catch { throw; }
        }
        #endregion

        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의

        #region > 공지사항 목록 dependencyProperty
        public static readonly DependencyProperty NoticeInfoMgntListProperty
            = DependencyProperty.Register("NoticeInfoMgntList", typeof(ObservableCollection<NoticeInfoMgnt>), typeof(C1006)
                , new PropertyMetadata(new ObservableCollection<NoticeInfoMgnt>()));

        public ObservableCollection<NoticeInfoMgnt> NoticeInfoMgntList
        {
            get { return (ObservableCollection<NoticeInfoMgnt>)GetValue(NoticeInfoMgntListProperty); }
            set { SetValue(NoticeInfoMgntListProperty, value); }
        }
        #endregion

        #region > Grid Row수 dependencyProperty
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1006), new PropertyMetadata(string.Empty));
        public string GridRowCount
        {
            get { return (string)GetValue(GridRowCountProperty); }
            set { SetValue(GridRowCountProperty, value); }
        }
        #endregion

        #region > 공지사항 상세
        #region >> 제목 - NOTI_TITLE
        public static readonly DependencyProperty NOTI_TITLE_DTLProperty = DependencyProperty.Register("NOTI_TITLE_DTL",
            typeof(string), typeof(C1006), new PropertyMetadata(string.Empty, (d, e) => ((C1006)d).OnDataChanged()));

        public string NOTI_TITLE_DTL
        {
            get { return (string)GetValue(NOTI_TITLE_DTLProperty); }
            set { SetValue(NOTI_TITLE_DTLProperty, value); }
        }
        #endregion

        #region >> 작성자 - NOTI_WRITER
        public static readonly DependencyProperty NOTI_WRITERProperty = DependencyProperty.Register("NOTI_WRITER",
            typeof(string), typeof(C1006), new PropertyMetadata(string.Empty, (d, e) => ((C1006)d).OnDataChanged()));

        public string NOTI_WRITER
        {
            get { return (string)GetValue(NOTI_WRITERProperty); }
            set { SetValue(NOTI_WRITERProperty, value); }
        }
        #endregion

        #region >> 시작일자
        public static readonly DependencyProperty START_DATEProperty = DependencyProperty.Register("START_DATE",
            typeof(DateTime), typeof(C1006), new PropertyMetadata(DateTime.Now, (d, e) => ((C1006)d).OnDataChanged()));

        public DateTime START_DATE
        {
            get { return (DateTime)GetValue(START_DATEProperty); }
            set { SetValue(START_DATEProperty, value); }
        }
        #endregion

        #region >> 종료일자
        public static readonly DependencyProperty TO_DATEProperty = DependencyProperty.Register("TO_DATE",
            typeof(DateTime), typeof(C1006), new PropertyMetadata(DateTime.Now, (d, e) => ((C1006)d).OnDataChanged()));

        public DateTime TO_DATE
        {
            get { return (DateTime)GetValue(TO_DATEProperty); }
            set { SetValue(TO_DATEProperty, value); }
        }
        #endregion

        #region >> 내용
        public static readonly DependencyProperty NOTI_CONTProperty = DependencyProperty.Register("NOTI_CONT",
        typeof(string), typeof(C1006), new PropertyMetadata(string.Empty, (d, e) => ((C1006)d).OnDataChanged()));

        public string NOTI_CONT
        {
            get { return (string)GetValue(NOTI_CONTProperty); }
            set { SetValue(NOTI_CONTProperty, value); }
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
            try
            {
                #region + 콤보박스 컨트롤
                // 콤보박스
                this.BaseClass.BindingCommonComboBox(this.cboKeywordType, "NOTI_KEYWORD_TYPE", null, false);
                this.BaseClass.BindingCommonComboBox(this.cboSEARCH, "NOTI_SEARCH_TYPE", null, false);
                this.BaseClass.BindingCommonComboBox(this.cboPrty, "NOTI_PRTY", null, false);
                this.BaseClass.BindingFirstComboBox(this.cboUseYN, "USE_YN");
                // 콤보박스 default index 설정
                this.cboPrty.SelectedIndex = 2;

                #endregion

                // 컨트롤 Readonly 속성 - 프로그램 로드시 컨트롤 속성을 수정안되도록 설정한다.
                this.ChangeControlReadonlyAttribute(true);
            }
            catch { throw; }
            finally { this.g_isChanged = false; }
        }
        #endregion

        #region InitValue - 컨트롤 데이터 초기화
        /// <summary>
        /// 컨트롤 데이터 초기화
        /// </summary>
        private void InitValue()
        {
            try
            {
                #region >> 공지사항 입력란 및 플래그 초기화, 활성화
                //InitNotiInfoFlag();

                // 컨트롤 Readonly 속성 변경 - 수정 가능
                this.ChangeControlReadonlyAttribute(true);

                #region + 컨트롤 초기화 및 기본값 설정
                this.lblSeq.Text                = "0";
                this.NOTI_TITLE_DTL             = string.Empty;                 // 제목
                this.NOTI_WRITER                = string.Empty;                 // 작성자
                this.START_DATE                 = DateTime.Now;                 // 시작일자
                this.TO_DATE                    = DateTime.Now.AddDays(1);      // 종료일자
                this.cboPrty.SelectedIndex      = 2;                            // 우선순위 (하)
                this.cboUseYN.SelectedIndex     = 0;                            // 사용여부 (사용)
                this.NOTI_CONT                  = string.Empty;                 // 내용
                #endregion

                this.g_isChanged        = false;
                this.g_isNew            = false;
                #endregion
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
            this.Loaded += C1006_Loaded;
            #endregion

            #region + 버튼 이벤트
            // 조회버튼 이벤트
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 저장버튼 이벤트
            this.btnSAVE.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;
            // 공지사항 생성 버튼 이벤트
            this.btnCREATE.PreviewMouseLeftButtonUp += BtnAdd_PreviewMouseLeftButtonUp;
            // 공지사항 삭제 버튼 이벤트 
            this.btnDELETE.PreviewMouseLeftButtonUp += BtnDelete_PreviewMouseLeftButtonUp;
            #endregion

            #region + 달력 이벤트
            // 시작일자 (ReadOnly 속성 적용할 경우 컨트롤 선택시 달력창이 오픈되는것을 방지하기 위한 이벤트)
            this.deFromDate.PopupOpening += DateEdit_PopupOpening;
            // 종료일자 (ReadOnly 속성 적용할 경우 컨트롤 선택시 달력창이 오픈되는것을 방지하기 위한 이벤트)
            this.deToDate.PopupOpening += DateEdit_PopupOpening;
            #endregion

            #region + 콤보박스 이벤트
            // 우선순위
            this.cboPrty.SelectedIndexChanged += ComboEdit_SelectedIndexChanged;
            // 사용여부
            this.cboUseYN.SelectedIndexChanged += ComboEdit_SelectedIndexChanged;

            // 우선순위 (ReadOnly 속성 적용할 경우 컨트롤 선택시 달력창이 오픈되는것을 방지하기 위한 이벤트)
            this.cboPrty.PopupOpening += ComboBoxEdit_PopupOpening;
            // 사용여부 (ReadOnly 속성 적용할 경우 컨트롤 선택시 달력창이 오픈되는것을 방지하기 위한 이벤트)
            this.cboUseYN.PopupOpening += ComboBoxEdit_PopupOpening;
            #endregion

            #region + 그리드 이벤트
            this.gridMaster.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;
            #endregion
        }
        #endregion

        #endregion

        #region > CheckDataValidation - 입력, 수정 항목 Validation 체크
        /// <summary>
        /// 입력, 수정 항목 Validation 체크
        /// </summary>
        /// <returns></returns>
        private bool CheckDataValidation()
        {
            try
            {
                #region 1. 공지사항 종료일과 시작일 비교
                if (this.START_DATE.CompareTo(this.TO_DATE) > 0)
                {
                    this.BaseClass.MsgInfo("공지사항 시작일이 종료일보다 최근일 수 없습니다.", BaseEnumClass.CodeMessage.MESSAGE);
                    return false;
                }
                #endregion

                #region 2. 공지사항 제목 최대 길이
                if (this.NOTI_TITLE_DTL.Length > 100)
                {
                    this.BaseClass.MsgInfo("제목은 최대 100자 길이로 가능합니다.", BaseEnumClass.CodeMessage.MESSAGE);
                    return false;
                }
                #endregion

                #region 3. 공지사항 내용 최대 길이
                if (this.NOTI_CONT.Length > 5000)
                {
                    this.BaseClass.MsgInfo("내용은 최대 5000자 길이로 가능합니다.", BaseEnumClass.CodeMessage.MESSAGE);
                    return false;
                }
                #endregion

                #region 4. 공지사항 종료일과 현재날짜 비교
                if (TO_DATE.CompareTo(DateTime.Now) < 0)
                {
                    this.BaseClass.MsgInfo("현재날짜가 종료일보다 최근일 수 없습니다.", BaseEnumClass.CodeMessage.MESSAGE);
                    return false;
                }
                #endregion

                #region 5. 공지사항 제목 내용 빈 값검사
                if (this.NOTI_CONT.Length == 0 || this.NOTI_TITLE_DTL.Length == 0)
                {
                    this.BaseClass.MsgInfo("공지사항 제목 혹은 내용을 기입해주세요", BaseEnumClass.CodeMessage.MESSAGE);
                    return false;
                }
                #endregion

                return true;
            }
            catch { throw; }
        }
        #endregion

        #region > 기타 함수
        #region >> ChangeControlReadonlyAttribute - 컨트롤 Readonly 속성 변경
        /// <summary>
        /// 컨트롤 Readonly 속성 변경
        /// </summary>
        /// <param name="_isStatus">Readonly 속성값</param>
        private void ChangeControlReadonlyAttribute(bool _isStatus)
        {
            try
            {
                this.txtWriter.IsReadOnly = true;   // 작성자

                if (_isStatus == false && (this.BaseClass.RoleCode == "A") == false)
                {
                    _isStatus = true;
                }

                this.txtTitle.IsReadOnly        = _isStatus;        // 제목
                this.txtCont.IsReadOnly         = _isStatus;        // 내용
                this.deFromDate.IsReadOnly      = _isStatus;        // 시작일자
                this.deToDate.IsReadOnly        = _isStatus;        // 종료일
                this.cboPrty.IsReadOnly         = _isStatus;        // 우선순위
                this.cboUseYN.IsReadOnly        = _isStatus;        // 사용여부

                #region + ReadOnly 속성에 따른 컨트롤 배경색 설정
                Brush brushColor        = null;

                if (_isStatus == true)
                {
                    brushColor      = this.BaseClass.ConvertStringToMediaBrush("#F9F9F9");
                }
                else
                {
                    brushColor      = this.BaseClass.ConvertStringToMediaBrush("#FFFFFF");
                }

                this.txtWriter.Background       = this.BaseClass.ConvertStringToMediaBrush("#F9F9F9");  // 작성자
                this.txtTitle.Background        = brushColor;       // 제목
                this.deFromDate.Background      = brushColor;       // 공지사항 게시 시작일자
                this.deToDate.Background        = brushColor;       // 공지사항 게시 종료일자
                this.cboPrty.Background         = brushColor;       // 우선순위
                this.cboUseYN.Background        = brushColor;       // 사용여부
                this.txtCont.Background         = brushColor;       // 공지사항 내용
                #endregion
            }
            catch { throw; }
        }
        #endregion

        #region >> CheckModifyData - 탭의 데이터 저장 여부를 체크한다.
        /// <summary>
        /// 각 탭의 데이터 저장 여부를 체크한다.
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyData()
        {
            bool bRtnValue = true;

            // 수정 혹은 생성 중인 경우
            if (this.g_isChanged == true || this.g_isNew == true)
            {
                bRtnValue = false;
            }

            if (bRtnValue == false)
            {
                this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
                bRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
            }

            return bRtnValue;
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

        #region >> Task 비동기 조회
        private async Task asyncSearch()
        {
            #region + 상태바 실행
            this.loadingScreen.IsSplashScreenShown = true;
            #endregion

            DataSet dsRtnValue = null;
            #region + 공지사항 목록 조회
            dsRtnValue = await this.GetSP_NOTICE_LIST_INQ();

            if (dsRtnValue == null) { return; }

            var strErrCode = string.Empty;     // 오류 코드
            var strErrMsg = string.Empty;      // 오류 메세지

            if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
            {
                // 정상 처리된 경우
                this.NoticeInfoMgntList = new ObservableCollection<NoticeInfoMgnt>();
                // 오라클인 경우 TableName = O_NOTICE_LIST
                this.NoticeInfoMgntList.ToObservableCollection(dsRtnValue.Tables[0]);

                #region ++ yyyy-mm-dd 파싱
                foreach (var item in this.NoticeInfoMgntList)
                {
                    StringBuilder sbToDate = new StringBuilder();
                    StringBuilder sbFromDate = new StringBuilder();

                    sbToDate.Append(item.NOTI_TO_DT.Substring(0, 4));
                    sbToDate.Append("-");
                    sbToDate.Append(item.NOTI_TO_DT.Substring(4, 2));
                    sbToDate.Append("-");
                    sbToDate.Append(item.NOTI_TO_DT.Substring(6));

                    sbFromDate.Append(item.NOTI_FROM_DT.Substring(0, 4));
                    sbFromDate.Append("-");
                    sbFromDate.Append(item.NOTI_FROM_DT.Substring(4, 2));
                    sbFromDate.Append("-");
                    sbFromDate.Append(item.NOTI_FROM_DT.Substring(6));
                    item.NOTI_FROM_DT = sbFromDate.ToString();
                    item.NOTI_TO_DT = sbToDate.ToString();
                }
                #endregion
            }
            else
            {
                // 오류가 발생한 경우
                this.NoticeInfoMgntList.ToObservableCollection(null);
                this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
            }
            #endregion

            #region + 그리드 바인딩
            // 조회 데이터를 그리드에 바인딩한다.
            this.gridMaster.ItemsSource = this.NoticeInfoMgntList;
            #endregion
        }

        #endregion

        #region >> SetResultText - 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        private void SetResultText()
        {
            var strResource     = string.Empty;                                                     // 텍스트 리소스 (전체 데이터 수)
            var iTotalRowCount  = 0;                                                                // 조회 데이터 수

            strResource         = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                  // 텍스트 리소스
            iTotalRowCount      = (this.gridMaster.ItemsSource as ICollection).Count;               // 전체 데이터 수
            this.GridRowCount   = $"{strResource} : {iTotalRowCount.ToString()}";                   // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource         = this.BaseClass.GetResourceValue("DATA_INQ");                      // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");

        }
        #endregion
        #endregion

        #region > 데이터 관련

        #region >> GetSP_NOTICE_LIST_INQ - 공지사항 데이터 조회 (전체)
        private async Task<DataSet> GetSP_NOTICE_LIST_INQ()
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1006.SP_NOTICE_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_NOTI_LIST", "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                  // 센터코드
            var strDateTime         = DateTime.Now.ToString("yyyyMMdd");        // 오늘 날짜
            var strKeyword          = this.txt_noti_keyword.Text;               // 키워드
            string strKeywordType   = null;                                     // 키워드 타입

            strKeywordType  = this.BaseClass.ComboBoxSelectedKeyValue(this.cboKeywordType);

            if (this.cboKeywordType.SelectedIndex == 0)
            {
                strKeywordType = "W";
            }
            else if (this.cboKeywordType.SelectedIndex == 1)
            {
                strKeywordType = "C";
            }
            else if (this.cboKeywordType.SelectedIndex == 2)
            {
                strKeywordType = "T";
            }
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);           // 센터코드
            dicInputParam.Add("P_DATE",                 strDateTime);           // 오늘 날짜
            dicInputParam.Add("P_KEYWORD",              strKeyword);            // 키워드
            dicInputParam.Add("P_KEYWORD_TYPE",         strKeywordType);        // 키워드 타입
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

        #region >> GetSP_NEW_NOTICE_LIST_INQ - 새로운 공지사항 데이터 조회
        private async Task<DataSet> GetSP_NEW_NOTICE_LIST_INQ()
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1006.SP_NEW_NOTICE_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_NEW_NOTI_LIST", "O_RSLT" };

            var strCenterCD = this.BaseClass.CenterCD;                  // 센터코드
            var strUserID = this.BaseClass.UserID;                    // 유저아이디
            var strDateTime = DateTime.Now.ToString("yyyyMMdd");        // 오늘 날짜
            var strKeyword = this.txt_noti_keyword.Text;                // 키워드
            string strKeywordType = null;                               // 키워드 타입

            if (this.cboKeywordType.SelectedIndex == 0)
            {
                strKeywordType = "W";
            }
            else if (this.cboKeywordType.SelectedIndex == 1)
            {
                strKeywordType = "C";
            }
            else if (this.cboKeywordType.SelectedIndex == 2)
            {
                strKeywordType = "T";
            }
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCenterCD);             // 센터코드
            dicInputParam.Add("P_USER_ID", strUserID);               // 유저아이디
            dicInputParam.Add("P_DATE", strDateTime);                // 오늘 날짜
            dicInputParam.Add("P_KEYWORD", strKeyword);              // 키워드
            dicInputParam.Add("P_KEYWORD_TYPE", strKeywordType);     // 키워드 타입
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

        #region >> GetSP_OLD_NOTICE_LIST_INQ - 지난 공지사항 데이터 조회
        private async Task<DataSet> GetSP_OLD_NOTICE_LIST_INQ()
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1006.SP_OLD_NOTICE_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_OLD_NOTI_LIST", "O_RSLT" };

            var strCenterCD = this.BaseClass.CenterCD;                  // 센터코드
            var strDateTime = DateTime.Now.ToString("yyyyMMdd");        // 오늘 날짜
            var strKeyword = this.txt_noti_keyword.Text;                // 키워드
            string strKeywordType = null;                               // 키워드 타입
            if (this.cboKeywordType.SelectedIndex == 0)
            {
                strKeywordType = "W";
            }
            else if (this.cboKeywordType.SelectedIndex == 1)
            {
                strKeywordType = "C";
            }
            else if (this.cboKeywordType.SelectedIndex == 2)
            {
                strKeywordType = "T";
            }

            var aaa = this.BaseClass.ComboBoxSelectedKeyValue(this.cboKeywordType);
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCenterCD);             // 센터코드
            dicInputParam.Add("P_DATE", strDateTime);                // 오늘 날짜
            dicInputParam.Add("P_KEYWORD", strKeyword);              // 키워드
            dicInputParam.Add("P_KEYWORD_TYPE", strKeywordType);     // 키워드 타입
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

        #region >> GetSP_UNUSED_NOTICE_LIST_INQ - 미사용 공지사항 데이터 조회
        private async Task<DataSet> GetSP_UNUSED_NOTICE_LIST_INQ()
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1006.SP_UNUSED_NOTICE_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_UNUSED_NOTI_LIST", "O_RSLT" };

            var strCenterCD = this.BaseClass.CenterCD;                     // 센터코드
            var strKeyword = this.txt_noti_keyword.Text;                   // 키워드
            string strKeywordType = null;                                  // 키워드 타입
            if (this.cboKeywordType.SelectedIndex == 0)
            {
                strKeywordType = "W";
            }
            else if (this.cboKeywordType.SelectedIndex == 1)
            {
                strKeywordType = "C";
            }
            else if (this.cboKeywordType.SelectedIndex == 2)
            {
                strKeywordType = "T";
            }
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCenterCD);             // 센터코드
            dicInputParam.Add("P_KEYWORD", strKeyword);              // 키워드
            dicInputParam.Add("P_KEYWORD_TYPE", strKeywordType);     // 키워드 타입
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

        #region >> UpdateSP_NOTICE_TALLY_UPD 확인한 공지사항 체크
        private async Task<bool> UpdateSP_NOTICE_TALLY_UPD(int notiSeq, BaseDataAccess _da)
        {
            #region + 파라메터 변수 선언 및 값 할당
            bool isRtnValue                             = true;
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "PK_C1006.SP_NOTICE_TALLY_UPD";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD     = this.BaseClass.CenterCD;          // 센터코드
            var strUserID       = this.BaseClass.UserID;            // 유저아이디
            var nbNotiSEQ       = notiSeq;                          // 공지사항 순번

            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",      strCenterCD);       // 센터코드
            dicInputParam.Add("P_USER_ID",      strUserID);         // 셀 타입명
            dicInputParam.Add("P_NOTI_SEQ",     nbNotiSEQ);         // 셀 타입명
            #endregion

            #region + 데이터 저장
       
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);
            
            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        this.BaseClass.MsgError(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    this.BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }
            #endregion
            return isRtnValue;
        }


        #endregion

        #region >> UpdateSP_NOTICE_UPD - 공지사항 수정내역 저장
        /// <summary>
        /// 공지사항 수정내역 저장
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <returns></returns>
        private async Task<bool> UpdateSP_NOTICE_UPD(BaseDataAccess _da)
        {
            bool isRtnValue = true;

            try
            {
                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_C1006.SP_NOTICE_UPD";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strCenterCD         = this.BaseClass.CenterCD;                                                  // 센터코드
                var strUserID           = this.BaseClass.UserID;                                                    // 유저아이디
                var strUseYN            = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);                   // 사용여부
                var iNotiSEQ            = Convert.ToInt32(this.lblSeq.Text.Trim());                                 // 공지사항 순번
                var iNotiPRTY           = Convert.ToInt32(this.BaseClass.ComboBoxSelectedKeyValue(this.cboPrty));   // 공지사항 우선순위
                var strNotiFrDT         = START_DATE.ToString("yyyyMMdd");                                          // 공지사항 시작일
                var strNotiToDT         = TO_DATE.ToString("yyyyMMdd");                                             // 공지사항 종료일
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("P_CNTR_CD",                  strCenterCD);               // 센터코드
                dicInputParam.Add("P_USER_ID",                  strUserID);                 // 사용자 ID
                dicInputParam.Add("P_USE_YN",                   strUseYN);                  // 사용여부
                dicInputParam.Add("P_NOTI_SEQ",                 iNotiSEQ);                  // 순번
                dicInputParam.Add("P_NOTI_PRTY",                iNotiPRTY);                 // 우선순위
                dicInputParam.Add("P_NOTI_TITLE",               this.NOTI_TITLE_DTL);       // 제목
                dicInputParam.Add("P_NOTI_CONT",                this.NOTI_CONT);            // 내용
                dicInputParam.Add("P_NOTI_FROM_DT",             strNotiFrDT);               // 시작일자
                dicInputParam.Add("P_NOTI_TO_DT",               strNotiToDT);               // 종료일자
                #endregion

                #region + 공지사항 수정
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                        {
                            this.BaseClass.MsgError(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                            isRtnValue = false;
                        }
                    }
                    else
                    {
                        this.BaseClass.MsgError("수정 중 오류가 발생 했습니다.", BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                #endregion

                return isRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> InsertSP_NOTICE_INS - 공지사항 생성
        private async Task<bool> InsertSP_NOTICE_INS(BaseDataAccess _da)
        {
            bool isRtnValue = true;

            try
            { 
                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_C1006.SP_NOTICE_INS";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strCenterCD         = this.BaseClass.CenterCD;                                                  // 센터코드
                var strUserID           = this.BaseClass.UserID;                                                    // 유저아이디
                var iNotiPRTY           = Convert.ToInt32(this.BaseClass.ComboBoxSelectedKeyValue(this.cboPrty));   // 공지사항 우선순위
                var strNotiFrDT         = START_DATE.ToString("yyyyMMdd");                                          // 공지사항 시작일
                var strNotiToDT         = TO_DATE.ToString("yyyyMMdd");                                             // 공지사항 종료일
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("P_CNTR_CD",          strCenterCD);               // 센터코드
                dicInputParam.Add("P_USER_ID",          strUserID);                 // 사용자 ID
                dicInputParam.Add("P_NOTI_PRTY",        iNotiPRTY);                 // 우선순위
                dicInputParam.Add("P_NOTI_TITLE",       this.NOTI_TITLE_DTL);       // 제목
                dicInputParam.Add("P_NOTI_CONT",        this.NOTI_CONT);            // 내용
                dicInputParam.Add("P_NOTI_FROM_DT",     strNotiFrDT);               // 시작일자
                dicInputParam.Add("P_NOTI_TO_DT",       strNotiToDT);               // 종료일자
                #endregion

                #region + 공지사항 생성 
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                        {
                            this.BaseClass.MsgError(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                            isRtnValue = false;
                        }
                    }
                    else
                    {
                        this.BaseClass.MsgError("수정 중 오류가 발생 했습니다.", BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                #endregion

                return isRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> DeleteSP_NOTICE_DEL - 공지사항 삭제
        private bool DeleteSP_NOTICE_DEL(BaseDataAccess _da, int _iSelectedNoticeSeq)
        {
            bool isRtnValue = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "PK_C1006.SP_NOTICE_DEL";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD     = this.BaseClass.CenterCD;         // 센터코드
            var strUserID       = this.BaseClass.UserID;           // 유저아이디
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",      strCenterCD);               // 센터코드
            dicInputParam.Add("P_USER_ID",      strUserID);                 // 사용자 ID
            dicInputParam.Add("P_NOTI_SEQ",     _iSelectedNoticeSeq);       // 현재 선택된 공지사항 Sequence값
            #endregion

            #region + 공지사항 삭제
            dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        this.BaseClass.MsgError(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    this.BaseClass.MsgError("수정 중 오류가 발생 했습니다.", BaseEnumClass.CodeMessage.MESSAGE);
                    isRtnValue = false;
                }
            }
            #endregion

            return isRtnValue;
        }
        #endregion

        #endregion
        #endregion

        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void C1006_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.g_isChanged = false;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 공통 사용 이벤트
        #region >> OnDataChanged - 컨트롤 데이터 변경
        private void OnDataChanged()
        {
            this.g_isChanged = true;
        }
        #endregion

        #endregion

        #region > 버튼 클릭 이벤트
        #region >> 공지사항 조회버튼 클릭 이벤트
        /// <summary>
        /// 공지사항 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.g_isChanged == true)
                {
                    // ASK_UPDATE_DATA_INQ : 수정된 데이터가 있습니다.|조회하시겠습니까?
                    this.BaseClass.MsgQuestion("ASK_UPDATE_DATA_INQ");
                    if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }
                }

                this.loadingScreen.IsSplashScreenShown = true;

                DataSet dsRtnValue = null;

                #region + 공지사항 목록 조회
                if (this.cboSEARCH.SelectedIndex == 0)
                {
                    dsRtnValue = await this.GetSP_NEW_NOTICE_LIST_INQ();
                }
                else if (this.cboSEARCH.SelectedIndex == 1)
                {
                    dsRtnValue = await this.GetSP_NOTICE_LIST_INQ();
                }
                else if (this.cboSEARCH.SelectedIndex == 2)
                {
                    dsRtnValue = await this.GetSP_UNUSED_NOTICE_LIST_INQ();
                }
                else if (this.cboSEARCH.SelectedIndex == 3)
                {
                    dsRtnValue = await this.GetSP_OLD_NOTICE_LIST_INQ();
                }
                #endregion

                if (dsRtnValue == null) { return; }

                var strErrCode      = string.Empty;     // 오류 코드
                var strErrMsg       = string.Empty;     // 오류 메세지

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.NoticeInfoMgntList = new ObservableCollection<NoticeInfoMgnt>();
                    // 오라클인 경우 TableName = O_NOTICE_LIST
                    this.NoticeInfoMgntList.ToObservableCollection(dsRtnValue.Tables[0]);

                    #region + 그리드 바인딩
                    // 조회 데이터를 그리드에 바인딩한다.
                    this.gridMaster.ItemsSource = this.NoticeInfoMgntList;
                    #endregion

                    // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                    this.SetResultText();

                    if (this.NoticeInfoMgntList.Count == 0) { return; }

                    // 컨트롤 데이터 초기화
                    this.InitValue();
                }
                else
                {
                    // 오류가 발생한 경우
                    this.NoticeInfoMgntList.ToObservableCollection(null);
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                this.g_isChanged        = false;

                if (this.NoticeInfoMgntList.Count == 0)
                {
                    this.lblSeq.Text = "0";
                }

                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        /// <summary>
        /// 데이터와 일치하는 Row Index를 그리드 컨트롤에서 조회한다.
        /// </summary>
        /// <param name="_iNotiSeqNo"></param>
        /// <returns></returns>
        private int GetEqualDataRowIndex(int _iNotiSeqNo)
        {
            try
            {
                int iRowIndex = -1;
                
                for (int i = 0; i < this.gridMaster.VisibleRowCount; i++)
                {
                    if (_iNotiSeqNo.Equals(((NoticeInfoMgnt)this.gridMaster.GetRow(i)).PRE_NOTI_SEQ))
                    {
                        iRowIndex = i;
                        break;
                    }
                }

                return iRowIndex;
            }
            catch { throw; }
        }


        #region >> 공지사항 저장버튼 클릭 이벤트
        /// <summary>
        /// 공지사항 저장버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isRtnValue = true;

                // 신규, 수정된 데이터가 없는 경우 구문을 리턴한다.
                if (this.g_isChanged == false) { return; }

                var strCenterCD         = this.BaseClass.CenterCD;                                                      // 센터코드
                var strUserID           = this.BaseClass.UserID;                                                        // 유저아이디
                string strUseYN         = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);                       // 사용여부
                var iNoticeSeqNo        = Convert.ToInt32(this.lblSeq.Text);                                            // 공지사항 Sequence번호
                var iNoticePrty         = Convert.ToInt32(this.BaseClass.ComboBoxSelectedKeyValue(this.cboPrty)) + 1;   // 우선순위

                // 입력, 수정 항목 Validation 체크
                if (this.CheckDataValidation() == false) { return; }

                // 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                #region + 공지사항 저장 
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    // 상태바 (아이콘) 실행
                    this.loadingScreen.IsSplashScreenShown = true;

                    try
                    {
                        da.BeginTransaction();
                        // 신규 
                        if (this.g_isNew == true && this.g_isChanged == true)
                        {
                            isRtnValue = await this.InsertSP_NOTICE_INS(da);
                                
                        }
                        // 수정
                        else if (this.g_isChanged == true)
                        {
                            isRtnValue = await this.UpdateSP_NOTICE_UPD(da);
                        }

                        if (isRtnValue == false)
                        {
                            // 오류 발생하여 저장 실패한 경우
                            da.RollbackTransaction();
                        }
                        else
                        {
                            // 저장되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");

                            // 저장된 경우
                            da.CommitTransaction();

                            // 공지사항 조회 버튼 클릭
                            //BtnSearch_PreviewMouseLeftButtonUp(sender, e);

                            // 전체 공지사항 조회 실행
                            await asyncSearch();

                            if (this.g_isNew == false || this.g_isChanged == true)
                            {
                                if (this.NoticeInfoMgntList.Count > 0)
                                {
                                    var iEqualDataRowIndex = this.GetEqualDataRowIndex(iNoticeSeqNo);

                                    // 이전에 수정한 데이터의 Row를 찾는다.
                                    this.BaseClass.SetGridRowAddFocuse(this.gridMaster, iEqualDataRowIndex);

                                    this.lblSeq.Text        = ((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_SEQ.ToString();   // 공지사항 순번
                                    #region + 데이터 바인딩
                                    this.NOTI_TITLE_DTL         = (((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_TITLE);                                                          // 제목
                                    this.NOTI_WRITER            = (((NoticeInfoMgnt)this.gridMaster.SelectedItem).UPD_ID);                                                              // 작성자
                                    this.cboPrty.SelectedItem   = (((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_PRTY);                                                           // 우선순위
                                    this.cboUseYN.SelectedItem  = (((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_USE_YN);                                                         // 사용여부
                                    this.START_DATE             = Convert.ToDateTime(this.BaseClass.ConvertStringToDate(((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_FROM_DT));  // 시작일자
                                    this.TO_DATE                = Convert.ToDateTime(this.BaseClass.ConvertStringToDate(((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_TO_DT));    // 종료일자
                                    this.NOTI_CONT              = (((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_CONT);                                                           // 내용

                                    var strCurrentComboUseYN    = ((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_USE_YN;                                                           // 사용여부
                                    this.cboUseYN.SelectedIndex = strCurrentComboUseYN.Equals("Y") ? 0 : 2;

                                    var iCurrentComboPrity      = ((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_PRTY;                                                          // 우선순위
                                    this.cboPrty.SelectedIndex  = iCurrentComboPrity - 1;
                                    #endregion
                                }
                            }

                            // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                            this.SetResultText();

                            // 컨트롤 Readonly 속성 변경 - 수정 가능
                            this.ChangeControlReadonlyAttribute(false);
                        }
                    }
                    catch
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }
                        this.BaseClass.MsgError("저장 중 오류가 발생 했습니다.", BaseEnumClass.CodeMessage.MESSAGE);
                        throw;
                    }
                    finally
                    {
                        // 상태바 (아이콘) 제거
                        this.loadingScreen.IsSplashScreenShown = false;

                        this.g_isChanged        = false;
                        this.g_isNew            = false;
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

        #region >> 공지사항 신규 버튼 클릭 이벤트
        private void BtnAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                #region >> 작성중인 공지사항 취소의사 묻기 
                if (this.g_isChanged == true)
                {
                    // (MSG)ASK_ADD_DATA_MODIFY : 수정된 데이터가 있습니다.|신규 데이터를 입력하시겠습니까?
                    this.BaseClass.MsgQuestion("ASK_ADD_DATA_MODIFY");
                    if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }
                }
                #endregion

                #region >> 공지사항 입력란 및 플래그 초기화, 활성화
                // 컨트롤 데이터 초기화
                this.InitValue();

                // 공지사항 신규인 경우 작성자 컨트롤에 현재 로그인한 사용자를 매핑한다.
                this.NOTI_WRITER        = this.BaseClass.UserName;

                // 컨트롤 Readonly 속성 변경 - 수정 가능
                this.ChangeControlReadonlyAttribute(false);

                this.g_isChanged    = false;
                this.g_isNew        = true;
                #endregion
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 공지사항 삭제 버튼 클릭 이벤트
        /// <summary>
        /// 삭제버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 수정된 데이터가 있습니다. 체크 로직 추가

                var query = this.NoticeInfoMgntList.Where(p => p.IsSelected == true).ToList();
                if (query.Count() == 0)
                {
                    // ERR_NO_SELECT - 선택된 데이터가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return;
                }

                //// 공지사항 항목을 클릭한 경우
                //if (this.g_isGridClick == true)
                //{
                this.BaseClass.MsgQuestion("정말 삭제하시겠습니까?", BaseEnumClass.CodeMessage.MESSAGE);
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue     = true;

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        // 트랜잭션 시작
                        da.BeginTransaction();

                        foreach (var item in query)
                        {
                            var iSelectedNoticeSeq = item.NOTI_SEQ; // 현재 선택된 공지사항의 Sequence값

                            isRtnValue = this.DeleteSP_NOTICE_DEL(da, iSelectedNoticeSeq);
                            if (isRtnValue == false) { break; }
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = false;

                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
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

                        this.g_isChanged        = false;
                        this.g_isNew            = false;
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

        #region > 달력 이벤트
        #region >> 달력 컨트롤 ReadOnly 속성 적용할 경우 컨트롤 선택시 달력창이 오픈되는것을 방지하기 위한 이벤트
        /// <summary>
        /// 달력 컨트롤 ReadOnly 속성 적용할 경우 컨트롤 선택시 달력창이 오픈되는것을 방지하기 위한 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DateEdit_PopupOpening(object sender, OpenPopupEventArgs e)
        {
            try
            {
                e.Cancel = ((DateEdit)sender).IsReadOnly;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region > 콤보박스 이벤트
        #region >> 콤보박스 컨트롤 값 변경 시 이벤트
        /// <summary>
        /// 콤보박스 컨트롤 값 변경 시 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboEdit_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            try
            {   
                this.g_isChanged                = true; 
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 콤보박스 컨트롤 ReadOnly 속성 적용할 경우 컨트롤 선택시 달력창이 오픈되는것을 방지하기 위한 이벤트
        private void ComboBoxEdit_PopupOpening(object sender, OpenPopupEventArgs e)
        {
            try
            {
                e.Cancel = ((ComboBoxEdit)sender).IsReadOnly;
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

        private async void GridMaster_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view    = (sender as GridControl).View as TableView;
                var hi      = view.CalcHitInfo(e.OriginalSource as DependencyObject);

                if (hi.InRowCell == false) { return; }

                #region >> 작성중인 공지사항 취소의사 묻기 
                if (this.g_isChanged == true)
                {
                    // (MSG)ASK_UPDATE_DATA_INQ : 수정된 데이터가 있습니다.|조회하시겠습니까?
                    this.BaseClass.MsgQuestion("ASK_UPDATE_DATA_INQ");
                    if (this.BaseClass.BUTTON_CONFIRM_YN == false)
                    {
                        this.g_isChanged = false;
                        return;
                    }
                }
                #endregion

                #region >> 클릭한 행 공지사항 순번 가져오기 
                var iCurrentNotiSeq     = ((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_SEQ;  // 공지사항 순번
                this.lblSeq.Text        = iCurrentNotiSeq.ToString();
                #endregion

                // 리턴 bool값을 담는 변수
                bool isRtnValue = false;
                
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    //this.loadingScreen.IsSplashScreenShown = true;

                    try
                    {
                        #region + 데이터 바인딩
                        this.NOTI_TITLE_DTL         = (((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_TITLE);                                                          // 제목
                        this.NOTI_WRITER            = (((NoticeInfoMgnt)this.gridMaster.SelectedItem).UPD_ID);                                                              // 작성자
                        this.cboPrty.SelectedItem   = (((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_PRTY);                                                           // 우선순위
                        this.cboUseYN.SelectedItem  = (((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_USE_YN);                                                         // 사용여부
                        this.START_DATE             = Convert.ToDateTime(this.BaseClass.ConvertStringToDate(((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_FROM_DT));  // 시작일자
                        this.TO_DATE                = Convert.ToDateTime(this.BaseClass.ConvertStringToDate(((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_TO_DT));    // 종료일자
                        this.NOTI_CONT              = (((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_CONT);                                                           // 내용

                        var strCurrentComboUseYN    = ((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_USE_YN;                                                           // 사용여부
                        this.cboUseYN.SelectedIndex = strCurrentComboUseYN.Equals("Y") ? 0 : 2;

                        var iCurrentComboPrity      = ((NoticeInfoMgnt)this.gridMaster.SelectedItem).NOTI_PRTY;                                                          // 우선순위
                        this.cboPrty.SelectedIndex  = iCurrentComboPrity - 1;

                        // 컨트롤 Readonly 속성 변경 - 수정 가능
                        this.ChangeControlReadonlyAttribute(false);
                        #endregion

                        #region + 정상 처리시 클릭 유무 변경
                        this.g_isChanged        = false;
                        this.g_isNew            = false;
                        #endregion

                        #region + 확인한 공지사항으로 체크
                        da.BeginTransaction();
                        //isRtnValue = await UpdateSP_NOTICE_TALLY_UPD(notiSeq, da);
                        isRtnValue = await UpdateSP_NOTICE_TALLY_UPD(iCurrentNotiSeq, da);

                        if (isRtnValue == true)
                        {
                            // 저장된 경우
                            da.CommitTransaction();
                        }
                        else
                        {
                            // 오류 발생하여 저장 실패한 경우
                            da.RollbackTransaction();
                        }

                        #endregion
                    }
                    catch (Exception err)
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }
                        this.BaseClass.MsgError("저장 중 오류가 발생 했습니다.", BaseEnumClass.CodeMessage.MESSAGE);

                        this.BaseClass.Error(err);
                    }
                    finally
                    {
                        //// 상태바 (아이콘) 제거
                        //this.loadingScreen.IsSplashScreenShown = false;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
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
        #endregion
    }
}