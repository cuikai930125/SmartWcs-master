using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1013;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DevExpress.XtraGrid;
using SMART.WCS.Control.Modules.Interface;
using System.Reflection;

namespace SMART.WCS.UI.COMMON.Views.BASE_INFO_MGMT
{
    /// <summary>
    /// C1013.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1013 : UserControl, TabCloseInterface
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
        /// BaseInfo 선언
        /// </summary>
        BaseInfo BaseInfo = new BaseInfo();

        /// <summary>
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public C1013()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        /// 
        public C1013(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

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

            //catch
            //{
            //    throw;
            //}
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1013), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 센터 관리
        #region >> 배치 차수 조회
        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public static readonly DependencyProperty ShipToProperty
            = DependencyProperty.Register("ShipToMgntList", typeof(ObservableCollection<ShipToMgnt>), typeof(C1013)
                , new PropertyMetadata(new ObservableCollection<ShipToMgnt>()));

        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public ObservableCollection<ShipToMgnt> ShipToMgntList  
        {
            get { return (ObservableCollection<ShipToMgnt>)GetValue(ShipToProperty); }
            set { SetValue(ShipToProperty, value); }
        }
        #endregion

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1013), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string TabFirstGridRowCount
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
            // 콤보박스
            //this.BaseClass.BindingCommonComboBox(this.CboDbConnType, "DB_CONN_TYPE", null, true); //DB 접속 타입
            this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, false); //사용여부

            // 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd_First.ToolTip = this.BaseClass.GetResourceValue("ROW_ADD");   //행추가
            this.btnRowDelete_First.ToolTip = this.BaseClass.GetResourceValue("ROW_DEL"); //행삭제

        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += C1013_Loaded;
            #endregion

            #region + 거래처 관리
            #region ++ 버튼 클릭 이벤트
            // 조회
            this.btnSearch_First.PreviewMouseLeftButtonUp += BtnSearch_First_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
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
            this.TabFirstGridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";               // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
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
            string strMessage = string.Empty;
            //string strSelectedName = string.Empty;

            if (this.ShipToMgntList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                strMessage = this.BaseClass.GetResourceValue("ERR_EXISTS_NO_SAVE_TAB", BaseEnumClass.ResourceType.MESSAGE);

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

                iCheckedCount = this.ShipToMgntList.Where(p => p.IsSelected == true).Count();

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
            var DeleteRowItem = this.ShipToMgntList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();
            //이미 등록된 데이터를 삭제하려고 할 때 에러메시지
            if (DeleteRowItem.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }
            DeleteRowItem.ForEach(p => ShipToMgntList.Remove(p));
        }

        #endregion
        #endregion
        #region > 데이터 관련
        #region >>  GetSP_SHIP_TO_LIST_INQ -출고처 조회
        /// <summary>
        /// 출고처 데이터조회
        /// </summary>
        private async Task<DataSet> GetSP_SHIP_TO_LIST_INQ(string customerCD)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1013.SP_SHIP_TO_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_SHIP_TO_LIST", "O_RSLT" };

            var strCstCD = this.uCtrlCst.CodeCst.Trim();                             // 고객사 코드
            var strCstNM = this.uCtrlCst.NameCst.Trim();                             // 고객사 명
            var strSTCd = this.txtSTCd.Text.Trim();                                  // 출고처 코드
            var strSTNm = this.txtSTNm.Text.Trim();                                  // 출고처 명
            var strRgnCd = this.txtRgnCd.Text.Trim();                                // 권역 코드
            var strRgnNm = this.txtRgnNm.Text.Trim();                                // 권역 명
            var strUseYn = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);   // 사용여부

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CST_CD", strCstCD);       // 고객사 코드
            dicInputParam.Add("P_SHIP_TO_CD", strSTCd);    // 출고처 코드
            dicInputParam.Add("P_SHIP_TO_NM", strSTNm);    // 출고처 명
            dicInputParam.Add("P_RGN_CD", strRgnCd);       // 권역 코드
            dicInputParam.Add("P_RGN_NM", strRgnNm);       // 권역 명
            dicInputParam.Add("P_USE_YN", strUseYn);       // 사용 여부
            #endregion


            #region 데이터 조회
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

        #region >> InsertSP_SHIP_TO_INS - 출고처 등록
        /// <summary>
        /// 출고처 등록
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> InsertSP_SHIP_TO_INS(BaseDataAccess _da, ShipToMgnt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1013.SP_SHIP_TO_INS";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            //var strCoCd = BaseClass.CompanyCode;                      // 회사 코드
            var strCstCD = this.uCtrlCst.CodeCst.Trim();                // 고객사 코드
            var strSTCd = _item.SHIP_TO_CD;                             // 출고처 코드
            var strSTNm = _item.SHIP_TO_NM;                             // 출고처 명
            var strAddr = _item.ADDR;                                   // 주소
            var strZipCd = _item.ZIP_CD;                                // ZIP 코드
            var strTelNo = _item.TEL_NO;                                // 연락처
            var strRgnCd = _item.RGN_CD;                                // 권역코드
            var strRgnNm = _item.RGN_NM;                                // 권역명
            var strCrtSprCd = _item.CRT_SPR_CD;
            _item.IsWMS = strCrtSprCd.Equals("WMS") ? true : false;

            var strUseYN = _item.Checked == true ? "Y" : "N";           // 사용여부                

            var strUserID = this.BaseClass.UserID;                      // 사용자 ID
            var strErrCode = string.Empty;                              // 오류 코드
            var strErrMsg = string.Empty;                               // 오류 메세지
            #endregion

            #region Input 파라메터       
            dicInputParam.Add("P_CST_CD", strCstCD);                    // 고객사 코드
            dicInputParam.Add("P_SHIP_TO_CD", strSTCd);                 // 출고처 코드
            dicInputParam.Add("P_SHIP_TO_NM", strSTNm);                 // 출고처 명
            dicInputParam.Add("P_ADDR", strAddr);                       // 주소
            dicInputParam.Add("P_ZIP_CD", strZipCd);                    // ZIP 코드
            dicInputParam.Add("P_TEL_NO", strTelNo);                    // 연락처
            dicInputParam.Add("P_RGN_CD", strRgnCd);                    // 권역 코드
            dicInputParam.Add("P_RGN_NM", strRgnNm);                    // 권역 명
            dicInputParam.Add("P_USE_YN", strUseYN);                    // 사용 여부
            dicInputParam.Add("P_USER_ID", strUserID);                  // 사용자 ID
            #endregion

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
                        this.BaseClass.MsgInfo(dtRtnValue.Rows[0]["MSG"].ToString());
                        isRtnValue = false;
                    }
                }
                else
                {
                    this.BaseClass.MsgInfo("ERR_SAVE"); //ERR_SAVE : 저장 중 오류가 발생했습니다.
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }
        #endregion

        #region >> UpdateSP_SHIP_TO_UPD - 출고처 수정
        /// <summary>
        /// 출고처 수정
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> UpdateSP_SHIP_TO_UPD(BaseDataAccess _da, ShipToMgnt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1013.SP_SHIP_TO_UPD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            //var strCoCd = BaseClass.CompanyCode;                      // 회사 코드
            var strCstCD = this.uCtrlCst.CodeCst.Trim();                // 고객사 코드        
            var strSTCd = _item.SHIP_TO_CD;                             // 출고처 코드
            var strSTNm = _item.SHIP_TO_NM;                             // 출고처 명
            var strAddr = _item.ADDR;                                   // 주소
            var strZipCd = _item.ZIP_CD;                                // ZIP 코드
            var strTelNo = _item.TEL_NO;                                // 연락처
            var strRgnCd = _item.RGN_CD;                                // 권역 코드
            var strRgnNm = _item.RGN_NM;                                // 권역 명
            var strUseYN = _item.Checked == true ? "Y" : "N";           // 사용 여부

            var strUserID = this.BaseClass.UserID;                      // 사용자 ID
            var strErrCode = string.Empty;                              // 오류 코드
            var strErrMsg = string.Empty;                               // 오류 메세지
            #endregion

            #region Input 파라메터       
            //dicInputParam.Add("P_CO_CD", strCoCd);                    // 고객사 코드
            dicInputParam.Add("P_CST_CD", strCstCD);                    // 고객사 코드
            dicInputParam.Add("P_SHIP_TO_CD", strSTCd);                 // 출고처 코드
            dicInputParam.Add("P_SHIP_TO_NM", strSTNm);                 // 출고처 명
            dicInputParam.Add("P_ADDR", strAddr);                       // 주소
            dicInputParam.Add("P_ZIP_CD", strZipCd);                    // ZIP 코드
            dicInputParam.Add("P_TEL_NO", strTelNo);                    // 연락처
            dicInputParam.Add("P_RGN_CD", strRgnCd);                    // 권역 코드
            dicInputParam.Add("P_RGN_NM", strRgnNm);                    // 권역 명
            dicInputParam.Add("P_USE_YN", strUseYN);                    // 사용 여부

            dicInputParam.Add("P_USER_ID", strUserID);                  // 사용자 ID
            #endregion

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
        #endregion

        #endregion
        #endregion

        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void C1013_Loaded(object sender, RoutedEventArgs e)
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

        #region > 출고처 관리
        #region >> 버튼 클릭 이벤트
        #region + 출고처 관리 조회버튼 클릭 이벤트
        /// <summary>
        /// 출고처 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 진민 봐조라~ (조회 전 변경 셀 확인)
            var ChangedRowData = this.ShipToMgntList.Where(p => p.IsSelected == true).ToList();

            if (ChangedRowData.Count > 0)
            {
                var strMessage = this.BaseClass.GetResourceValue("ASK_EXISTS_NO_SAVE_TO_SEARCH", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    ShipSearch();
                }
            }
            else
            {
                ShipSearch();
            }
        }
        #endregion

        #region + 출고처 조회
        private async void ShipSearch()
        {
            // 진민 봐조라~ (조회 기능이 이쪽으로 왔다)
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                //돋보기에서 조회된 고객사 코드
                var strCstCD = this.uCtrlCst.CodeCst.Trim();

                this.ShipToMgntList.ForEach(p => p.ClearError());

                //고객사 입력 없이 조회버튼 누를 경우 
                if (string.IsNullOrEmpty(strCstCD) == true)
                {
                    this.BaseClass.MsgInfo("ERR_NOT_INPUT", "CST");
                    return;
                }

                // 출고처 데이터 조회
                DataSet dsRtnValue = await this.GetSP_SHIP_TO_LIST_INQ(strCstCD);

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.ShipToMgntList = new ObservableCollection<ShipToMgnt>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.ShipToMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ShipToMgntList.ToObservableCollection(null);
                    this.BaseClass.MsgInfo(strErrMsg);
                }

                //컬럼 수정 가능여부 확인을 위한 isWMS값 설정
                foreach (var item in this.ShipToMgntList)
                {
                    if (item.CRT_SPR_CD.Equals("WMS"))
                    {
                        item.IsWMS = true;
                        //isWMS가 true인 행은 수정 불가한 색으로 표현
                        item.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
                        item.BaseBackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
                    }
                }

                if (this.ShipToMgntList.Count == 0)
                {
                    this.BaseClass.MsgInfo("INFO_NOT_INQ"); //INFO_NOT_INQ : 조회된 데이터가 없습니다.
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.ShipToMgntList;

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

        #region + 출고처 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 출고처 관리 엑셀 다운로드 버튼 클릭 이벤트
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

                tv.Add(this.MasterGrid);
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

        #region + 출고처 관리 저장 버튼 클릭 이벤트
        /// <summary>
        /// 출고처 관리 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSave_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.CheckGridRowSelected() == false) { return; }

                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;

                this.ShipToMgntList.ForEach(p => p.ClearError());
                               
                var strMessage = "{0} 이(가) 입력되지 않았습니다.";

                var strCstCD = this.uCtrlCst.CodeCst.Trim();      // 고객사 코드

                //고객사 입력 없이 저장 버튼 누를 경우 
                if (string.IsNullOrEmpty(strCstCD) == true)
                {
                    this.BaseClass.MsgInfo("ERR_NOT_INPUT", "CST");
                    return;
                }

                foreach (var item in this.ShipToMgntList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.SHIP_TO_CD) == true)
                        {
                            item.CellError("SHIP_TO_CD", string.Format(strMessage, this.GetLabelDesc("SHIP_TO_CD")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.SHIP_TO_NM) == true)
                        {
                            item.CellError("SHIP_TO_NM", string.Format(strMessage, this.GetLabelDesc("SHIP_TO_NM")));
                            return;
                        }
                    }
                }
              

                var liSelectedRowData = this.ShipToMgntList.Where(p => p.IsSelected == true).ToList();

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
                                isRtnValue = await this.InsertSP_SHIP_TO_INS(da, item);
                            }
                            else 
                            {
                                isRtnValue = await this.UpdateSP_SHIP_TO_UPD(da, item);
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

                            await this.GetSP_SHIP_TO_LIST_INQ(strCstCD);

                            foreach (var item in liSelectedRowData)
                            {
                                item.IsSaved = true;
                                item.IsSelected = false;
                            }

                            // 진민 봐조라~ (GetSP_SHIP_TO_LIST_INQ 메소드에서 isWMS를 확인하지 않고 있고, 할 수도 없기 때문에 ShipSearch() 함수 이용한다)
                            ShipSearch();
                            //DataSet dsRtnValue = await this.GetSP_SHIP_TO_LIST_INQ(strCstCD);

                            //this.ShipToMgntList = new ObservableCollection<ShipToMgnt>();
                            //this.ShipToMgntList.ToObservableCollection(dsRtnValue.Tables[0]);

                            //this.gridMaster.ItemsSource = this.ShipToMgntList;
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
            var newItem = new ShipToMgnt
            {
                CST_CD = string.Empty,
                SHIP_TO_CD = string.Empty,
                SHIP_TO_NM = string.Empty,
                ADDR = string.Empty,
                ZIP_CD = string.Empty,
                TEL_NO = string.Empty,
                RGN_CD = string.Empty,
                RGN_NM = string.Empty,
                CRT_SPR_CD = string.Empty,
                USE_YN = "Y",
                IsSelected = true,
                IsNew = true,
                IsSaved = false
            };

            this.ShipToMgntList.Add(newItem);
            this.gridMaster.Focus();
            this.gridMaster.CurrentColumn = this.gridMaster.Columns.First();
            this.gridMaster.View.FocusedRowHandle = this.ShipToMgntList.Count - 1;

            this.ShipToMgntList[this.ShipToMgntList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
            this.ShipToMgntList[this.ShipToMgntList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);

        }
        #endregion

        #region + 행삭제 버튼 클릭 이벤트
        private void BtnRowDelete_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 그리드 내 체크박스 선택 여부 체크
            if (this.CheckGridRowSelected() == false) { return; }

            // 행추가된 그리드 Row중 선택된 Row를 삭제한다.
            this.DeleteGridRowItem();
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
            if (g_IsAuthAllYN == false)
            {
                e.Cancel = true;
                return;
            }

            TableView tv = sender as TableView;
            ShipToMgnt dataMember = tv.Grid.CurrentItem as ShipToMgnt;

            if (dataMember == null) { return; }


            switch (e.Column.FieldName)
            {
                // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                // 출고처 코드는 수정이 되지 않도록 처리한다.
                case "SHIP_TO_CD":
                case "CRT_SPR_CD":
                    if (dataMember.IsNew == false)
                    {
                        e.Cancel = true;
                    }
                    break;
                default: break;

                
            }

            switch (e.Column.FieldName)
            {
                // CRT_SPR_CD가 WMS이면 모든 컬럼 수정이 불가하도록 처리한다.
                case "SHIP_TO_NM":
                case "ADDR":
                case "ZIP_CD":
                case "TEL_NO":
                case "RGN_CD":
                case "RGN_NM":
                    if (dataMember.IsWMS == true)
                    {
                        e.Cancel = true;
                    }
                    break;
                default: break;
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
