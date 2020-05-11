using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1010;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMART.WCS.UI.COMMON.Views.BASE_INFO_MGMT
{
    /// <summary>
    /// C1010.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1010 : UserControl, TabCloseInterface
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
        /// Header 클릭에 따른 관련 정보 수집
        /// </summary>
        private List<string> headerSource = new List<string>();
        
        #endregion

        #region ▩ 생성자
        public C1010()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1010(List<string> _liMenuNavigation)
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

                //  공통코드를 사용하지 않는 콤보박스 설정
                //this.InitComboBoxInfo();
            }

            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1010), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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
        public static readonly DependencyProperty HeaderCommonCodeMgmtListProperty
            = DependencyProperty.Register("HeaderCommonCodeMgmtList", typeof(ObservableCollection<HeaderCommonCodeMgmt>), typeof(C1010)
                , new PropertyMetadata(new ObservableCollection<HeaderCommonCodeMgmt>()));

        private ObservableCollection<HeaderCommonCodeMgmt> HeaderCommonCodeMgmtList
        {
            get { return (ObservableCollection<HeaderCommonCodeMgmt>)GetValue(HeaderCommonCodeMgmtListProperty); }
            set { SetValue(HeaderCommonCodeMgmtListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty HeaderGridRowCountProperty
            = DependencyProperty.Register("HeaderGridRowCount", typeof(string), typeof(C1010), new PropertyMetadata(string.Empty));

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
        public static readonly DependencyProperty DetailCommonCodeMgmtListProperty
            = DependencyProperty.Register("DetailCommonCodeMgmtList", typeof(ObservableCollection<DetailCommonCodeMgmt>), typeof(C1010)
                , new PropertyMetadata(new ObservableCollection<DetailCommonCodeMgmt>()));

        private ObservableCollection<DetailCommonCodeMgmt> DetailCommonCodeMgmtList
        {
            get { return (ObservableCollection<DetailCommonCodeMgmt>)GetValue(DetailCommonCodeMgmtListProperty); }
            set { SetValue(DetailCommonCodeMgmtListProperty, value); }
        }

        #region >> Detail Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty DetailGridRowCountProperty
            = DependencyProperty.Register("DetailGridRowCount", typeof(string), typeof(C1010), new PropertyMetadata(string.Empty));

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
            // 콤보박스 - 조회 (사용여부)
            this.BaseClass.BindingCommonComboBox(this.cboUseYN_First, "USE_YN", null, false);
            this.BaseClass.BindingCommonComboBox(this.cboUseYN_Second, "USE_YN", null, false);

            // 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd_First.ToolTip = this.BaseClass.GetResourceValue("ROW_ADD");
            this.btnRowDelete_First.ToolTip = this.BaseClass.GetResourceValue("ROW_DEL");
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        private void InitEvent()
        {
            #region + Loaded 이벤트
            //this.Loaded += C1010_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // Header 조회
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_Header_PreviewMouseLeftButtonUp;
            // Header 및 Detail 저장
            this.btnSave_First.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;
            // Header 및 Detail 엑셀 다운로드
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;

            // Header 행 추가
            this.btnRowAdd_First.PreviewMouseLeftButtonUp += BtnRowAdd_First_PreviewMouseLeftButtonUp;
            // Header 행 삭제
            this.btnRowDelete_First.PreviewMouseLeftButtonUp += BtnRowDelete_First_PreviewMouseLeftButtonUp;
            // Detail 행 추가
            this.btnRowAdd_Second.PreviewMouseLeftButtonUp += BtnRowAdd_Second_PreviewMouseLeftButtonUp;
            // Detail 행 삭제
            this.btnRowDelete_Second.PreviewMouseLeftButtonUp += BtnRowDelete_Second_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 이벤트
            // Header 그리드 클릭 이벤트 - Detail 조회
            this.gridMasterHeader.PreviewMouseLeftButtonUp += DetailSearch_PreviewMouseLeftButtonUp;
            // Header 그리드 클릭 이벤트
            this.gridMasterHeader.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;
            // Detail 그리드 클릭 이벤트
            this.gridMasterDetail.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;
            
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

        #region >> CheckGridRowSelected - Header 그리드 체크박스 선택 유효성 체크
        /// <summary>
        /// Header 그리드 체크박스 선택 유효성 체크
        /// </summary>
        /// <returns></returns>
        private bool CheckGridRowSelected()
        {
            try
            {
                bool bRtnValue = false;
                int iHeaderCheckedCount = 0;
                int iDetailCheckedCount = 0;

                iHeaderCheckedCount = this.HeaderCommonCodeMgmtList.Where(p => p.IsSelected == true).Count();
                iDetailCheckedCount = this.DetailCommonCodeMgmtList.Where(p => p.IsSelected == true).Count();

                if (iHeaderCheckedCount != 0) { bRtnValue = true; }

                else if (iDetailCheckedCount != 0) { bRtnValue = true; }

                if (bRtnValue == false) { BaseClass.MsgError("ERR_NO_SELECT"); }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> DeleteHeaderGridRowItem - 선택한 Header 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// <summary>
        /// 선택한 Header 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// </summary>
        private void DeleteHeaderGridRowItem()
        {
            var liHeaderCommonCodeMgmt = this.HeaderCommonCodeMgmtList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();
            if (liHeaderCommonCodeMgmt.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }
            liHeaderCommonCodeMgmt.ForEach(p => HeaderCommonCodeMgmtList.Remove(p));
        }

        #endregion

        #region >> DeleteDetailGridRowItem - 선택한 Detail 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// <summary>
        /// 선택한 Detail 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// </summary>
        private void DeleteDetailGridRowItem()
        {
            var liDetailCommonCodeMgmt = this.DetailCommonCodeMgmtList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();
            if (liDetailCommonCodeMgmt.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }
            liDetailCommonCodeMgmt.ForEach(p => DetailCommonCodeMgmtList.Remove(p));
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
            bool bRtnValue = true;

            if (this.HeaderCommonCodeMgmtList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                bRtnValue = false;
            }

            if (this.DetailCommonCodeMgmtList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
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
        #endregion

        #region > 데이터 관련

        #region >> GetSP_COM_HDR_LIST_INQ - Common Code Header List 조회
        /// <summary>
        /// Common Code Header List 조회
        /// </summary>
        private DataSet GetSP_COM_HDR_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1010.SP_COM_HDR_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_COM_HDR_LIST", "O_RSLT" };

            var strHdrCd = this.txtComHdrCd_First.Text.Trim();                                         // CODE 대분류
            var strHdrNm = this.txtComHdrNm_First.Text.Trim();                                         // CODE 대분류 이름
            var strUseYn = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN_First);               // 사용 여부
            var strDtlCd = this.txtComDtlCd_First.Text.Trim();                                         // 상세코드
            var strDtlNm = this.txtComDtlNm_First.Text.Trim();                                         // 상세코드 명
            var strUseYn2 = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN_Second);             // 사용 여부

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_COM_HDR_CD", strHdrCd);                    // CODE 대분류
            dicInputParam.Add("P_COM_HDR_NM", strHdrNm);                    // CODE 대분류 이름
            dicInputParam.Add("P_USE_YN", strUseYn);                        // 사용 여부
            dicInputParam.Add("P_COM_DTL_CD", strDtlCd);                    // 상세코드
            dicInputParam.Add("P_COM_DTL_NM", strDtlNm);                    // 상세코드 명
            dicInputParam.Add("P_DTL_USE_YN", strUseYn2);                   // 사용여부 - 상세
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

        #region >> InsertSP_COM_HDR_INS - Common Code Header 등록 SP
        /// <summary>
        /// Common Code Header 등록 SP
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool InsertSP_COM_HDR_INS(BaseDataAccess _da, HeaderCommonCodeMgmt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1010.SP_COM_HDR_INS";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strHdrCd = _item.COM_HDR_CD;                                // CODE 대분류
            var strHdrNm = _item.COM_HDR_NM;                                // CODE 대분류 이름
            var strAttr01 = _item.ATTR01;                                   // 문자속성 01
            var strAttr02 = _item.ATTR02;                                   // 문자속성 02
            var strAttr03 = _item.ATTR03;                                   // 문자속성 03
            var strAttr04 = _item.ATTR04;                                   // 문자속성 04
            var strAttr05 = _item.ATTR05;                                   // 문자속성 05
            var strAttr06 = _item.ATTR06;                                   // 문자속성 06
            var strAttr07 = _item.ATTR07;                                   // 문자속성 07
            var strAttr08 = _item.ATTR08;                                   // 문자속성 08
            var strAttr09 = _item.ATTR09;                                   // 문자속성 09
            var strAttr10 = _item.ATTR10;                                   // 문자속성 10
            var strSortSeq = _item.SORT_SEQ;                                // 정렬순서
            var strUseYN = _item.Checked == true ? "Y" : "N";               // 사용 여부
            var strUserID = this.BaseClass.UserID;                          // 사용자 ID
            var strErrCode = string.Empty;                                  // 오류 코드
            var strErrMsg = string.Empty;                                   // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_COM_HDR_CD", strHdrCd);                    // CODE 대분류
            dicInputParam.Add("P_COM_HDR_NM", strHdrNm);                    // CODE 대분류 이름
            dicInputParam.Add("P_USE_YN", strUseYN);                        // 사용 여부
            dicInputParam.Add("P_ATTR01", strAttr01);                       // 문자속성 01
            dicInputParam.Add("P_ATTR02", strAttr02);                       // 문자속성 02
            dicInputParam.Add("P_ATTR03", strAttr03);                       // 문자속성 03
            dicInputParam.Add("P_ATTR04", strAttr04);                       // 문자속성 04
            dicInputParam.Add("P_ATTR05", strAttr05);                       // 문자속성 05
            dicInputParam.Add("P_ATTR06", strAttr06);                       // 문자속성 06
            dicInputParam.Add("P_ATTR07", strAttr07);                       // 문자속성 07
            dicInputParam.Add("P_ATTR08", strAttr08);                       // 문자속성 08
            dicInputParam.Add("P_ATTR09", strAttr09);                       // 문자속성 09
            dicInputParam.Add("P_ATTR10", strAttr10);                       // 문자속성 10
            dicInputParam.Add("P_SORT_SEQ", strSortSeq);                    // 정렬순서
            dicInputParam.Add("P_USER_ID", strUserID);                      // 사용자 ID
            #endregion

            dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

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

        #region >> UpdateSP_COM_HDR_UPD - Common Code Header 수정 SP
        /// <summary>
        /// Common Code Header 수정 SP
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool UpdateSP_COM_HDR_UPD(BaseDataAccess _da, HeaderCommonCodeMgmt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1010.SP_COM_HDR_UPD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strHdrCd = _item.COM_HDR_CD;                                // CODE 대분류
            var strHdrNm = _item.COM_HDR_NM;                                // CODE 대분류 이름
            var strUseYN = _item.Checked == true ? "Y" : "N";               // 사용 여부
            var strSortSeq = _item.SORT_SEQ;                                // 정렬순서
            var strAttr01 = _item.ATTR01;                                   // 문자속성 01
            var strAttr02 = _item.ATTR02;                                   // 문자속성 02
            var strAttr03 = _item.ATTR03;                                   // 문자속성 03
            var strAttr04 = _item.ATTR04;                                   // 문자속성 04
            var strAttr05 = _item.ATTR05;                                   // 문자속성 05
            var strAttr06 = _item.ATTR06;                                   // 문자속성 06
            var strAttr07 = _item.ATTR07;                                   // 문자속성 07
            var strAttr08 = _item.ATTR08;                                   // 문자속성 08
            var strAttr09 = _item.ATTR09;                                   // 문자속성 09
            var strAttr10 = _item.ATTR10;                                   // 문자속성 10
            var strUserID = this.BaseClass.UserID;                          // 사용자 ID       
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_COM_HDR_CD", strHdrCd);                    // CODE 대분류
            dicInputParam.Add("P_COM_HDR_NM", strHdrNm);                    // CODE 대분류 이름
            dicInputParam.Add("P_USE_YN", strUseYN);                        // 사용 여부
            dicInputParam.Add("P_SORT_SEQ", strSortSeq);                    // 정렬순서
            dicInputParam.Add("P_ATTR01", strAttr01);                       // 문자속성 01
            dicInputParam.Add("P_ATTR02", strAttr02);                       // 문자속성 02
            dicInputParam.Add("P_ATTR03", strAttr03);                       // 문자속성 03
            dicInputParam.Add("P_ATTR04", strAttr04);                       // 문자속성 04
            dicInputParam.Add("P_ATTR05", strAttr05);                       // 문자속성 05
            dicInputParam.Add("P_ATTR06", strAttr06);                       // 문자속성 06
            dicInputParam.Add("P_ATTR07", strAttr07);                       // 문자속성 07
            dicInputParam.Add("P_ATTR08", strAttr08);                       // 문자속성 08
            dicInputParam.Add("P_ATTR09", strAttr09);                       // 문자속성 09
            dicInputParam.Add("P_ATTR10", strAttr10);                       // 문자속성 10
            dicInputParam.Add("P_USER_ID", strUserID);                      // 사용자 ID
            #endregion

            dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

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

        //#region >>GetSP_COM_DTL_LIST_INQ_BTN - Common Code Detail List 조회 SP (조회 버튼 클릭 시)
        //private DataSet GetSP_COM_DTL_LIST_INQ_BTN()
        //{
        //    #region 파라메터 변수 선언 및 값 할당
        //    DataSet dsRtnValue = null;
        //    var strProcedureName = "PK_C1010.SP_COM_DTL_LIST_INQ_BTN";
        //    Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
        //    string[] arrOutputParam = { "O_COM_DTL_LIST", "O_RSLT" };

        //    var strHdrCd = this.txtComHdrCd_First.Text.Trim();                                       // CODE 대분류
        //    var strHdrNm = this.txtComHdrNm_First.Text.Trim();                                       // CODE 대분류 이름
        //    var strUseYn2 = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN_First);            // 사용 여부 - 헤더
        //    var strDtlCd = this.txtComDtlCd_First.Text.Trim();                                       // 상세분류 코드
        //    var strDtlNm = this.txtComDtlNm_First.Text.Trim();                                       // 상세분류 명
        //    var strUseYn = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN_Second);            // 사용 여부 - 디테일

        //    var strErrCode = string.Empty;                                                          // 오류 코드
        //    var strErrMsg = string.Empty;                                                           // 오류 메세지
        //    #endregion

        //    #region Input 파라메터
        //    dicInputParam.Add("P_COM_HDR_CD", strHdrCd);                                            // CODE 대분류
        //    dicInputParam.Add("P_COM_HDR_NM", strHdrNm);                                            // CODE 대분류 이름
        //    dicInputParam.Add("P_HDR_USE_YN", strUseYn2);                                           // 사용 여부 - 헤더
        //    dicInputParam.Add("P_COM_DTL_CD", strDtlCd);                                            // 상세분류 코드
        //    dicInputParam.Add("P_COM_DTL_NM", strDtlNm);                                            // 상세분류 명
        //    dicInputParam.Add("P_USE_YN", strUseYn);                                                // 사용 여부 - 디테일
        //    #endregion

        //    #region 데이터 조회
        //    using (BaseDataAccess dataAccess = new BaseDataAccess())
        //    {
        //        dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
        //    }
        //    #endregion

        //    return dsRtnValue;
        //}
        //#endregion

        #region >> GetSP_COM_DTL_LIST_INQ - Common Code Detail List 조회 SP (Header 더블 클릭 시)
        /// <summary>
        /// Common Code Detail List 조회 SP
        /// </summary>
        /// <param name="_da"></param>
        /// <param name="_item"></param>
        /// <returns></returns>
        private DataSet GetSP_COM_DTL_LIST_INQ(string headerCD)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1010.SP_COM_DTL_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_COM_DTL_LIST", "O_RSLT" };

            var strHdrCd = headerCD;                        //CODE 대분류
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_COM_HDR_CD", strHdrCd);    // CODE 대분류
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

        #region >> InsertSP_COM_DTL_INS - Common Code Detail 등록 SP
        /// <summary>
        /// Common Code Detail 등록 SP
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool InsertSP_COM_DTL_INS(BaseDataAccess _da, DetailCommonCodeMgmt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1010.SP_COM_DTL_INS";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };
            
            var strHdrCd = headerSource[0];                                 // CODE 대분류
            var strHdrNm = headerSource[1];                                 // CODE 대분류 이름
            var strDtlCd = _item.COM_DTL_CD;                                // 상세 분류
            var strDtlNm = _item.COM_DTL_NM;                                // 상세 분류 이름
            var strUseYN = _item.Checked == true ? "Y" : "N";               // 사용 여부
            var strAttr01 = _item.ATTR01;                                   // 문자속성 01
            var strAttr02 = _item.ATTR02;                                   // 문자속성 02
            var strAttr03 = _item.ATTR03;                                   // 문자속성 03
            var strAttr04 = _item.ATTR04;                                   // 문자속성 04
            var strAttr05 = _item.ATTR05;                                   // 문자속성 05
            var strAttr06 = _item.ATTR06;                                   // 문자속성 06
            var strAttr07 = _item.ATTR07;                                   // 문자속성 07
            var strAttr08 = _item.ATTR08;                                   // 문자속성 08
            var strAttr09 = _item.ATTR09;                                   // 문자속성 09
            var strAttr10 = _item.ATTR10;                                   // 문자속성 10
            var strSortSeq = _item.SORT_SEQ;                                // 정렬순서
            var strUserID = this.BaseClass.UserID;                          // 사용자 ID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_COM_HDR_CD", strHdrCd);                    // CODE 대분류
            dicInputParam.Add("P_COM_HDR_NM", strHdrNm);                    // CODE 대분류 이름
            dicInputParam.Add("P_COM_DTL_CD", strDtlCd);                    // 상세 분류
            dicInputParam.Add("P_COM_DTL_NM", strDtlNm);                    // 상세 분류 이름
            dicInputParam.Add("P_USE_YN", strUseYN);                        // 사용 여부
            dicInputParam.Add("P_ATTR01", strAttr01);                       // 문자속성 01
            dicInputParam.Add("P_ATTR02", strAttr02);                       // 문자속성 02
            dicInputParam.Add("P_ATTR03", strAttr03);                       // 문자속성 03
            dicInputParam.Add("P_ATTR04", strAttr04);                       // 문자속성 04
            dicInputParam.Add("P_ATTR05", strAttr05);                       // 문자속성 05
            dicInputParam.Add("P_ATTR06", strAttr06);                       // 문자속성 06
            dicInputParam.Add("P_ATTR07", strAttr07);                       // 문자속성 07
            dicInputParam.Add("P_ATTR08", strAttr08);                       // 문자속성 08
            dicInputParam.Add("P_ATTR09", strAttr09);                       // 문자속성 09
            dicInputParam.Add("P_ATTR10", strAttr10);                       // 문자속성 10
            dicInputParam.Add("P_SORT_SEQ", strSortSeq);                    // 정렬순서
            dicInputParam.Add("P_USER_ID", strUserID);                      // 사용자 ID
            #endregion

            dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

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

        #region >> UpdateSP_COM_DTL_UPD - Common Code Detail 수정 SP
        /// <summary>
        /// Common Code Detail 수정 SP
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool UpdateSP_COM_DTL_UPD(BaseDataAccess _da, DetailCommonCodeMgmt _item)
        {
            bool isRtnValue = true;
            
            if (headerSource[0] == null)
            {
                this.BaseClass.MsgError("ERR_NO_HDR_INFO");
                isRtnValue = false;

                return isRtnValue;
            }

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1010.SP_COM_DTL_UPD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strHdrCd = headerSource[0];                                 // CODE 대분류
            var strDtlCd = _item.COM_DTL_CD;                                // 상세 분류
            var strDtlNm = _item.COM_DTL_NM;                                // 상세 분류 이름
            var strUseYN = _item.Checked == true ? "Y" : "N";               // 사용 여부
            var strAttr01 = _item.ATTR01;                                   // 문자속성 01
            var strAttr02 = _item.ATTR02;                                   // 문자속성 02
            var strAttr03 = _item.ATTR03;                                   // 문자속성 03
            var strAttr04 = _item.ATTR04;                                   // 문자속성 04
            var strAttr05 = _item.ATTR05;                                   // 문자속성 05
            var strAttr06 = _item.ATTR06;                                   // 문자속성 06
            var strAttr07 = _item.ATTR07;                                   // 문자속성 07
            var strAttr08 = _item.ATTR08;                                   // 문자속성 08
            var strAttr09 = _item.ATTR09;                                   // 문자속성 09
            var strAttr10 = _item.ATTR10;                                   // 문자속성 10
            var strSortSeq = _item.SORT_SEQ;                                // 정렬순서
            var strUserID = this.BaseClass.UserID;                          // 사용자 ID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_COM_HDR_CD", strHdrCd);                    // CODE 대분류
            dicInputParam.Add("P_COM_DTL_CD", strDtlCd);                    // 상세 분류
            dicInputParam.Add("P_COM_DTL_NM", strDtlNm);                    // 상세 분류 이름
            dicInputParam.Add("P_USE_YN", strUseYN);                        // 사용 여부
            dicInputParam.Add("P_ATTR01", strAttr01);                       // 문자속성 01
            dicInputParam.Add("P_ATTR02", strAttr02);                       // 문자속성 02
            dicInputParam.Add("P_ATTR03", strAttr03);                       // 문자속성 03
            dicInputParam.Add("P_ATTR04", strAttr04);                       // 문자속성 04
            dicInputParam.Add("P_ATTR05", strAttr05);                       // 문자속성 05
            dicInputParam.Add("P_ATTR06", strAttr06);                       // 문자속성 06
            dicInputParam.Add("P_ATTR07", strAttr07);                       // 문자속성 07
            dicInputParam.Add("P_ATTR08", strAttr08);                       // 문자속성 08
            dicInputParam.Add("P_ATTR09", strAttr09);                       // 문자속성 09
            dicInputParam.Add("P_ATTR10", strAttr10);                       // 문자속성 10
            dicInputParam.Add("P_SORT_SEQ", strSortSeq);                    // 정렬순서
            dicInputParam.Add("P_USER_ID", strUserID);                      // 사용자 ID
            #endregion

            dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

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

        #endregion
        #endregion

        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void C1010_Loaded(object sender, RoutedEventArgs e)
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

        #region > 공통 코드 관리

        #region >> 버튼 클릭 이벤트

        #region + 공통 코드 관리 Header 리스트 조회 클릭 이벤트
        /// <summary>
        /// 공통 코드 관리 Header 리스트 조회 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_Header_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var ChangeRowData = this.HeaderCommonCodeMgmtList.Where(p => p.IsSelected == true).ToList();

            if (ChangeRowData.Count > 0)
            {
                var strMessage = this.BaseClass.GetResourceValue("ASK_EXISTS_NO_SAVE_TO_SEARCH", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    HeaderSearch();
                }
            }
            else
            {
                HeaderSearch();
            }
        }
        #endregion

        #region + 공통 코드 관리 Header 리스트 조회
        /// <summary>
        /// 공통 코드 관리 Header 리스트 조회
        /// </summary>
        private void HeaderSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 공통 코드 관리 헤더 리스트 조회
                DataSet dsRtnValue = this.GetSP_COM_HDR_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.HeaderCommonCodeMgmtList = new ObservableCollection<HeaderCommonCodeMgmt>();
                    // 오라클인 경우 TableName = TB_COM_CODE_MST
                    this.HeaderCommonCodeMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);

                    headerSource.Clear();

                    // 헤더리스트 조회될 때
                    if (this.HeaderCommonCodeMgmtList.Count() > 0)
                    {
                        // Header의 첫 번째 행의 공통 헤더 코드 가져오기
                        var obj_cd = this.HeaderCommonCodeMgmtList[0].COM_HDR_CD;
                        string headerCD = Convert.ToString(obj_cd);
                        headerSource.Add(headerCD);

                        // 공통 코드 관리 디테일 리스트 조회
                        DataSet dsRtnDetailValue = this.GetSP_COM_DTL_LIST_INQ(headerSource[0]);

                        if (dsRtnDetailValue == null) { return; }
                        strErrCode = string.Empty;
                        strErrMsg = string.Empty;

                        if (this.BaseClass.CheckResultDataProcess(dsRtnDetailValue, ref strErrCode, ref strErrMsg) == true)
                        {
                            // 정상 처리된 경우
                            this.DetailCommonCodeMgmtList = new ObservableCollection<DetailCommonCodeMgmt>();
                            // 오라클인 경우 TableName = TB_COM_CODE_MST
                            this.DetailCommonCodeMgmtList.ToObservableCollection(dsRtnDetailValue.Tables[0]);
                        }
                        else
                        {
                            // 오류가 발생한 경우
                            this.DetailCommonCodeMgmtList.ToObservableCollection(null);
                            BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                        }
                    }

                    else
                    {
                        this.DetailCommonCodeMgmtList.Clear();
                    }
                }
                else
                {
                    // 오류가 발생한 경우
                    this.HeaderCommonCodeMgmtList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }
                
                // 조회 데이터를 그리드에 바인딩
                this.gridMasterHeader.ItemsSource = this.HeaderCommonCodeMgmtList;
                this.gridMasterDetail.ItemsSource = this.DetailCommonCodeMgmtList;

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

        #region + 공통 코드 관리 저장 버튼 클릭 이벤트 - Header, Detail 모두
        /// <summary>
        /// 공통 코드 관리 저장 버튼 클릭 이벤트 - Header, Detail 모두
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {   
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false)
                {
                    return;
                }

                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;

                this.HeaderCommonCodeMgmtList.ForEach(p => p.ClearError());
                this.DetailCommonCodeMgmtList.ForEach(p => p.ClearError());

                var strMessage = "{0} 이(가) 입력되지 않았습니다.";


                foreach (var item in this.HeaderCommonCodeMgmtList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.COM_HDR_CD) == true)
                        {
                            item.CellError("COM_HDR_CD", string.Format(strMessage, this.GetLabelDesc("COM_HDR_CD")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.COM_HDR_NM) == true)
                        {
                            item.CellError("COM_HDR_NM", string.Format(strMessage, this.GetLabelDesc("COM_HDR_NM")));
                            return;
                        }
                    }
                }

                foreach (var item in this.DetailCommonCodeMgmtList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.COM_DTL_CD) == true)
                        {
                            item.CellError("COM_DTL_CD", string.Format(strMessage, this.GetLabelDesc("COM_DTL_CD")));
                            return;
                        }
                    }
                }

                var liSelectedHeaderRowData = this.HeaderCommonCodeMgmtList.Where(p => p.IsSelected == true).ToList();
                var liSelectedDetailRowData = this.DetailCommonCodeMgmtList.Where(p => p.IsSelected == true).ToList();

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        da.BeginTransaction();

                        foreach (var item in liSelectedHeaderRowData)
                        {
                            if (item.IsNew == true)
                            {
                                isRtnValue = this.InsertSP_COM_HDR_INS(da, item);
                            }
                            else
                            {
                                isRtnValue = this.UpdateSP_COM_HDR_UPD(da, item);
                            }

                            if (isRtnValue == false)
                            {
                                break;
                            }
                        }
                        
                        foreach (var item in liSelectedDetailRowData)
                        {
                            if (item.IsNew == true)
                            {
                                isRtnValue = this.InsertSP_COM_DTL_INS(da, item);
                            }
                            else
                            {
                                isRtnValue = this.UpdateSP_COM_DTL_UPD(da, item);
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
                            
                            BaseClass.MsgInfo("CMPT_SAVE");

                            foreach (var item in liSelectedHeaderRowData)
                            {
                                item.IsSaved = true;
                            }
                            foreach (var item in liSelectedDetailRowData)
                            {
                                item.IsSaved = true;
                            }
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

                        // 체크박스 해제
                        foreach (var item in liSelectedHeaderRowData)
                        {
                            item.IsSelected = false;
                        }
                        foreach (var item in liSelectedDetailRowData)
                        {
                            item.IsSelected = false;
                        }

                        // 저장 후 저장내용 List에 출력 : Header
                        DataSet dsRtnValue = this.GetSP_COM_HDR_LIST_INQ();
                        
                        this.HeaderCommonCodeMgmtList = new ObservableCollection<HeaderCommonCodeMgmt>();
                        this.HeaderCommonCodeMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);

                        this.gridMasterHeader.ItemsSource = this.HeaderCommonCodeMgmtList;

                        // 저장 후 저장내용 List에 출력 : Detail
                        DataSet dsRtnValue2 = this.GetSP_COM_DTL_LIST_INQ(headerSource[0]);

                        this.DetailCommonCodeMgmtList = new ObservableCollection<DetailCommonCodeMgmt>();
                        this.DetailCommonCodeMgmtList.ToObservableCollection(dsRtnValue2.Tables[0]);

                        this.gridMasterDetail.ItemsSource = this.DetailCommonCodeMgmtList;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }

        }
        #endregion

        #region + 공통 코드 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 공통 코드 관리 엑셀 다운로드 버튼 클릭 이벤트
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

        #region + Header 행추가 버튼 클릭 이벤트
        private void BtnRowAdd_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var newItem = new HeaderCommonCodeMgmt
            {
                COM_HDR_CD = string.Empty
                ,
                COM_HDR_NM = string.Empty
                ,
                USE_YN = "Y"
                ,
                SORT_SEQ = 0
                ,
                ATTR01 = string.Empty
                ,
                ATTR02 = string.Empty
                ,
                ATTR03 = string.Empty
                ,
                ATTR04 = string.Empty
                ,
                ATTR05 = string.Empty
                ,
                ATTR06 = string.Empty
                ,
                ATTR07 = string.Empty
                ,
                ATTR08 = string.Empty
                ,
                ATTR09 = string.Empty
                ,
                ATTR10 = string.Empty
                ,
                IsSelected = true
                ,
                IsNew = true
                ,
                IsSaved = false
            };

            this.HeaderCommonCodeMgmtList.Add(newItem);
            this.gridMasterHeader.Focus();
            this.gridMasterHeader.CurrentColumn = this.gridMasterHeader.Columns.First();
            this.gridMasterHeader.View.FocusedRowHandle = this.HeaderCommonCodeMgmtList.Count - 1;

            this.HeaderCommonCodeMgmtList[this.HeaderCommonCodeMgmtList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
            this.HeaderCommonCodeMgmtList[this.HeaderCommonCodeMgmtList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);
        }
        #endregion
        
        #region + Header 행삭제 버튼 클릭 이벤트
        private void BtnRowDelete_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CheckGridRowSelected() == false) { return; }
            this.DeleteHeaderGridRowItem();
        }
        #endregion

        #region + Detail 행추가 버튼 클릭 이벤트
        private void BtnRowAdd_Second_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var newItem = new DetailCommonCodeMgmt
            {
                COM_DTL_CD = string.Empty
                ,
                COM_DTL_NM = string.Empty
                ,
                COM_HDR_CD = headerSource[0]
                ,
                COM_HDR_NM = headerSource[1]
                ,
                USE_YN = "Y"
                ,
                SORT_SEQ = 0
                ,
                ATTR01 = string.Empty
                ,
                ATTR02 = string.Empty
                ,
                ATTR03 = string.Empty
                ,
                ATTR04 = string.Empty
                ,
                ATTR05 = string.Empty
                ,
                ATTR06 = string.Empty
                ,
                ATTR07 = string.Empty
                ,
                ATTR08 = string.Empty
                ,
                ATTR09 = string.Empty
                ,
                ATTR10 = string.Empty
                ,
                IsSelected = true
                ,
                IsNew = true
                ,
                IsSaved = false
            };

            this.DetailCommonCodeMgmtList.Add(newItem);
            this.gridMasterDetail.Focus();
            this.gridMasterDetail.CurrentColumn = this.gridMasterDetail.Columns.First();
            this.gridMasterDetail.View.FocusedRowHandle = this.DetailCommonCodeMgmtList.Count - 1;

            this.DetailCommonCodeMgmtList[this.DetailCommonCodeMgmtList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
            this.DetailCommonCodeMgmtList[this.DetailCommonCodeMgmtList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);
        }
        #endregion

        #region + Detail 행삭제 버튼 클릭 이벤트
        private void BtnRowDelete_Second_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CheckGridRowSelected() == false) { return; }
            this.DeleteDetailGridRowItem();
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
            var view = (sender as GridControl).View as TableView;
            var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

            if (hi.InRowCell)
            {
                if (hi.Column.FieldName.Equals("USE_YN") == false) { return; }

                if (view.ActiveEditor == null)
                {
                    view.ShowEditor();

                    if (view.ActiveEditor == null) { return; }
                    Dispatcher.BeginInvoke(new Action(() => {
                        view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                    }), DispatcherPriority.Render);
                }
            }
        }
        #endregion

        #region + Header List클릭 이벤트 (Detail 리스트 조회)
        /// <summary>
        /// Header List클릭 이벤트 (Detail 리스트 조회)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var view = (sender as GridControl).View as TableView;
            var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

            if (hi.InRowCell)
            {
                // 클릭한 행의 COM_HDR_CD 가져오기
                int clicked = hi.RowHandle;
                var obj_cd = gridMasterHeader.GetCellValue(clicked, gridMasterHeader.Columns[1]);
                string headerCD = Convert.ToString(obj_cd);

                // 클릭한 행의 COM_HDR_NM 가져오기
                var obj_nm = gridMasterHeader.GetCellValue(clicked, gridMasterHeader.Columns[2]);
                string headerNM = Convert.ToString(obj_nm);

                headerSource.Clear();
                headerSource.Add(headerCD);
                headerSource.Add(headerNM);

                try
                {
                    // 상태바 (아이콘) 실행
                    this.loadingScreen.IsSplashScreenShown = true;

                    // Detail 목록 조회
                    DataSet dsRtnValue = this.GetSP_COM_DTL_LIST_INQ(headerCD);

                    if (dsRtnValue == null) { return; }

                    var strErrCode = string.Empty;      //오류 코드
                    var strErrMsg = string.Empty;      // 오류 메세지

                    if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                    {
                        // 정상 처리된 경우
                        this.DetailCommonCodeMgmtList = new ObservableCollection<DetailCommonCodeMgmt>();
                        // 오라클인 경우 TableName = TB_COM_CODE_MST
                        this.DetailCommonCodeMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
                    }
                    else
                    {
                        // 오류가 발생한 경우
                        this.DetailCommonCodeMgmtList.ToObservableCollection(null);
                        BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                    }
                    
                    // 조회 데이터를 그리드에 바인딩
                    this.gridMasterDetail.ItemsSource = this.DetailCommonCodeMgmtList;


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

            TableView tv = sender as TableView;

            if (tv.Name.Equals("tvMasterGrid") == true)
            {
                HeaderCommonCodeMgmt dataMember = tv.Grid.CurrentItem as HeaderCommonCodeMgmt;

                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    // CODE 대분류 컬럼은 수정이 되지 않도록 처리한다.
                    case "COM_HDR_CD":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (tv.Name.Equals("tvDetailGrid") == true)
            {
                DetailCommonCodeMgmt dataMember = tv.Grid.CurrentItem as DetailCommonCodeMgmt;

                if (dataMember == null) { return; }

                switch(e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    // 상세 코드 컬럼은 수정이 되지 않도록 처리한다.
                    case "COM_DTL_CD":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    default:
                        break;
                }
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