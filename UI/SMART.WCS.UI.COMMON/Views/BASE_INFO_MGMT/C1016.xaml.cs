using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1016;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMART.WCS.UI.COMMON.Views.BASE_INFO_MGMT
{
    /// <summary>
    /// C1016.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1016 : UserControl, TabCloseInterface
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

        /// <summary>
        /// Header 클릭에 따른 관련 정보 수집
        /// </summary>
        private List<string> headerSource = new List<string>();
        #endregion

        #region ▩ 생성자
        public C1016()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        /// 
        public C1016(List<string> _liMenuNavigation)
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
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1016), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 박스 유형 관리
        #region >> 배치 차수 조회
        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public static readonly DependencyProperty BoxTypeProperty
            = DependencyProperty.Register("BoxTypeMgntList", typeof(ObservableCollection<BoxTypeMgnt>), typeof(C1016)
                , new PropertyMetadata(new ObservableCollection<BoxTypeMgnt>()));

        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public ObservableCollection<BoxTypeMgnt> BoxTypeMgntList
        {
            get { return (ObservableCollection<BoxTypeMgnt>)GetValue(BoxTypeProperty); }
            set { SetValue(BoxTypeProperty, value); }
        }
        #endregion

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1016), new PropertyMetadata(string.Empty));

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
            this.Loaded += C1016_Loaded;
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

            if (this.BoxTypeMgntList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
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

                iCheckedCount = this.BoxTypeMgntList.Where(p => p.IsSelected == true).Count();

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
            var DeleteRowItem = this.BoxTypeMgntList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();
            //이미 등록된 데이터를 삭제하려고 할 때 에러메시지
            if (DeleteRowItem.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }
            DeleteRowItem.ForEach(p => BoxTypeMgntList.Remove(p));
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >>  GetSP_BOX_TYPE_LIST_INQ -박스 유형 조회
        /// <summary>
        /// 출고처 데이터조회
        /// </summary>
        private async Task<DataSet> GetSP_BOX_TYPE_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1016.SP_BOX_TYPE_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_BOX_TYPE_LIST", "O_RSLT" };

            var strCompanyCd = BaseClass.CompanyCode;
            var strCenterCd = BaseClass.CenterCD;
            var strCstCD = this.uCtrlCst.CodeCst.Trim();                             // 고객사 코드
            var strCstNM = this.uCtrlCst.NameCst.Trim();                             // 고객사 명
            var strBoxTypeCd = this.txtBoxTypeCd.Text.Trim();                       // 출고처 코드
            var strBoxTypeNm = this.txtBoxTypeNm.Text.Trim();                       // 출고처 명
            var strUseYn = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);   // 사용여부

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCompanyCd);          // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCenterCd);         // 센터 코드
            dicInputParam.Add("P_CST_CD", strCstCD);             // 고객사 코드
            dicInputParam.Add("P_BOX_TYPE_CD", strBoxTypeCd);    // 출고처 코드
            dicInputParam.Add("P_BOX_TYPE_NM", strBoxTypeNm);    // 출고처 명
            dicInputParam.Add("P_USE_YN", strUseYn);             // 사용 여부
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

        #region >> InsertSP_BOX_TYPE_INS - 박스 유형 등록
        /// <summary>
        /// 출고처 등록
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> InsertSP_BOX_TYPE_INS(BaseDataAccess _da, BoxTypeMgnt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1016.SP_BOX_TYPE_INS";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strCompanyCd = BaseClass.CompanyCode;
            var strCenterCd = BaseClass.CenterCD;
            var strCstCD = this.uCtrlCst.CodeCst.Trim();                // 고객사 코드
            var strBoxTypeCd = _item.BOX_TYPE_CD;                       // 박스 유형 코드
            var strBoxTypeNm = _item.BOX_TYPE_NM;                       // 박스 유형 명
            var strBoxTypeDesc = _item.BOX_TYPE_DESC;                   // 박스 유형 설명
            var strBoxTypeGrpCd = _item.BOX_TYPE_GRP_CD;                // 박스 그룹 코드         
            var strBoxWthLen = _item.BOX_WTH_LEN;                       // 박스 가로 길이
            var strBoxVertLen = _item.BOX_VERT_LEN;                     // 박스 세로 길이
            var strBoxHgtLen = _item.BOX_HGT_LEN;                       // 박스 높이 길이
            var strLenUom = _item.LEN_UOM;                              // 길이 단위
            var strBoxCbm = _item.BOX_CBM;                              // 박스 체적
            var strCbmUom = _item.CBM_UOM;                              // CBM 단위
            var strBoxWgt = _item.BOX_WGT;                              // 박스 중량
            var strWgtUom = _item.WGT_UOM;                              //중량단위
            var strUseYN = _item.Checked == true ? "Y" : "N";           // 사용여부       

            var strUserID = this.BaseClass.UserID;                      // 사용자 ID
            var strErrCode = string.Empty;                              // 오류 코드
            var strErrMsg = string.Empty;                               // 오류 메세지
            #endregion

            #region Input 파라메터       
            dicInputParam.Add("P_CO_CD", strCompanyCd);                 // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCenterCd);                // 센터 코드
            dicInputParam.Add("P_CST_CD", strCstCD);                    // 고객사 코드
            dicInputParam.Add("P_BOX_TYPE_CD", strBoxTypeCd);           // 박스 유형 코드
            dicInputParam.Add("P_BOX_TYPE_NM", strBoxTypeNm);           // 박스 유형 명
            dicInputParam.Add("P_BOX_TYPE_DESC", strBoxTypeDesc);       // 박스 유형 설명
            dicInputParam.Add("P_BOX_TYPE_GRP_CD", strBoxTypeGrpCd);    // 박스 그룹 코드
            dicInputParam.Add("P_BOX_WTH_LEN", strBoxWthLen);           // 박스 가로 길이
            dicInputParam.Add("P_BOX_VERT_LEN", strBoxVertLen);         // 박스 세로 길이
            dicInputParam.Add("P_BOX_HGT_LEN", strBoxHgtLen);           // 박스 높이 길이
            dicInputParam.Add("P_LEN_UOM", strLenUom);                  // 길이 단위
            dicInputParam.Add("P_BOX_CBM", strBoxCbm);                  // 박스 체적
            dicInputParam.Add("P_CBM_UOM", strCbmUom);                  // CBM 단위
            dicInputParam.Add("P_BOX_WGT", strBoxWgt);                  // 박스 중량
            dicInputParam.Add("P_WGT_UOM", strWgtUom);                  //중량단위
            dicInputParam.Add("P_USE_YN", strUseYN);                    // 사용여부      
            dicInputParam.Add("P_USER_ID", strUserID);                    // 사용여부      

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

        #region >> UpdateSP_BOX_TYPE_UPD - 박스 유형 수정
        /// <summary>
        /// 출고처 수정
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> UpdateSP_BOX_TYPE_UPD(BaseDataAccess _da, BoxTypeMgnt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1016.SP_BOX_TYPE_UPD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strCompanyCd = BaseClass.CompanyCode;
            var strCenterCd = BaseClass.CenterCD;
            var strCstCD = _item.CST_CD;                                // 고객사 코드
            var strBoxTypeCd = _item.BOX_TYPE_CD;                       // 박스 유형 코드
            var strBoxTypeNm = _item.BOX_TYPE_NM;                       // 박스 유형 명
            var strBoxTypeDesc = _item.BOX_TYPE_DESC;                   // 박스 유형 설명
            var strBoxTypeGrpCd = _item.BOX_TYPE_GRP_CD;                // 박스 그룹 코드         
            var strBoxWthLen = _item.BOX_WTH_LEN;                       // 박스 가로 길이
            var strBoxVertLen = _item.BOX_VERT_LEN;                     // 박스 세로 길이
            var strBoxHgtLen = _item.BOX_HGT_LEN;                       // 박스 높이 길이
            var strLenUom = _item.LEN_UOM;                              // 길이 단위
            var strBoxCbm = _item.BOX_CBM;                              // 박스 체적
            var strCbmUom = _item.CBM_UOM;                              // CBM 단위
            var strBoxWgt = _item.BOX_WGT;                              // 박스 중량
            var strWgtUom = _item.WGT_UOM;                              //중량단위
            var strUseYN = _item.Checked == true ? "Y" : "N";           // 사용여부       
            var strUserID = this.BaseClass.UserID;                      // 사용자 ID

            var strErrCode = string.Empty;                              // 오류 코드
            var strErrMsg = string.Empty;                               // 오류 메세지
            #endregion

            #region Input 파라메터  
            dicInputParam.Add("P_CO_CD", strCompanyCd);                 // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCenterCd);                // 센터 코드
            dicInputParam.Add("P_CST_CD", strCstCD);                    // 고객사 코드
            dicInputParam.Add("P_BOX_TYPE_CD", strBoxTypeCd);           // 박스 유형 코드
            dicInputParam.Add("P_BOX_TYPE_NM", strBoxTypeNm);           // 박스 유형 명
            dicInputParam.Add("P_BOX_TYPE_DESC", strBoxTypeDesc);       // 박스 유형 설명
            dicInputParam.Add("P_BOX_TYPE_GRP_CD", strBoxTypeGrpCd);    // 박스 그룹 코드
            dicInputParam.Add("P_BOX_WTH_LEN", strBoxWthLen);           // 박스 가로 길이
            dicInputParam.Add("P_BOX_VERT_LEN", strBoxVertLen);         // 박스 세로 길이
            dicInputParam.Add("P_BOX_HGT_LEN", strBoxHgtLen);           // 박스 높이 길이
            dicInputParam.Add("P_LEN_UOM", strLenUom);                  // 길이 단위
            dicInputParam.Add("P_BOX_CBM", strBoxCbm);                  // 박스 체적
            dicInputParam.Add("P_CBM_UOM", strCbmUom);                  // CBM 단위
            dicInputParam.Add("P_BOX_WGT", strBoxWgt);                  // 박스 중량
            dicInputParam.Add("P_WGT_UOM", strWgtUom);                  //중량단위
            dicInputParam.Add("P_USE_YN", strUseYN);                    // 사용여부    
            dicInputParam.Add("P_USER_ID", strUserID);                  // 사용자 ID
            #endregion

            ////고객사 입력 없이 UPDATE 시도하는 경우 
            //if (string.IsNullOrEmpty(strCstCD) == true)
            //{
            //    this.BaseClass.MsgInfo("ERR_NOT_INPUT", "CST");
            //    return false;
            //}

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
        private void C1016_Loaded(object sender, RoutedEventArgs e)
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
        #region + 박스유형관리 조회버튼 클릭 이벤트
        /// <summary>
        /// 박스유형관리 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                //돋보기에서 조회된 고객사 코드
                var strCstCD = this.uCtrlCst.CodeCst.Trim();

                this.BoxTypeMgntList.ForEach(p => p.ClearError());

                // 출고처 데이터 조회
                DataSet dsRtnValue = await this.GetSP_BOX_TYPE_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.BoxTypeMgntList = new ObservableCollection<BoxTypeMgnt>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.BoxTypeMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.BoxTypeMgntList.ToObservableCollection(null);
                    this.BaseClass.MsgInfo(strErrMsg);
                }

                if (this.BoxTypeMgntList.Count == 0)
                {
                    this.BaseClass.MsgInfo("INFO_NOT_INQ"); //INFO_NOT_INQ : 조회된 데이터가 없습니다.
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.BoxTypeMgntList;

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

        #region + 박스유형관리 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 박스유형관리 엑셀 다운로드 버튼 클릭 이벤트
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

        #region + 박스유형관리 저장 버튼 클릭 이벤트
        /// <summary>
        /// 박스유형관리 저장 버튼 클릭 이벤트
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

                this.BoxTypeMgntList.ForEach(p => p.ClearError());

                var strMessage = "{0} 이(가) 입력되지 않았습니다.";

                foreach (var item in this.BoxTypeMgntList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.CST_CD) == true)
                        {
                            item.CellError("CST_CD", string.Format(strMessage, this.GetLabelDesc("CST_CD")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.CST_NM) == true)
                        {
                            item.CellError("CST_NM", string.Format(strMessage, this.GetLabelDesc("CST_NM")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.BOX_TYPE_CD) == true)
                        {
                            item.CellError("BOX_TYPE_CD", string.Format(strMessage, this.GetLabelDesc("BOX_TYPE_CD")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.BOX_TYPE_NM) == true)
                        {
                            item.CellError("BOX_TYPE_NM", string.Format(strMessage, this.GetLabelDesc("BOX_TYPE_NM")));
                            return;
                        }

                    }
                }

                var liSelectedRowData = this.BoxTypeMgntList.Where(p => p.IsSelected == true).ToList();

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
                                isRtnValue = await this.InsertSP_BOX_TYPE_INS(da, item);
                            }
                            else
                            {
                                isRtnValue = await this.UpdateSP_BOX_TYPE_UPD(da, item);
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

                            await this.GetSP_BOX_TYPE_LIST_INQ();

                            foreach (var item in liSelectedRowData)
                            {
                                item.IsSaved = true;
                                item.IsSelected = false;
                            }

                            //저장된 데이터 포함하여 데이터 조회
                            DataSet dsRtnValue = await this.GetSP_BOX_TYPE_LIST_INQ();

                            this.BoxTypeMgntList = new ObservableCollection<BoxTypeMgnt>();
                            this.BoxTypeMgntList.ToObservableCollection(dsRtnValue.Tables[0]);

                            this.gridMaster.ItemsSource = this.BoxTypeMgntList;

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
                        this.BaseClass.MsgInfo("ERR_SAVE"); //ERR_SAVE : 저장 중 오류가 발생했습니다.
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
            //돋보기에서 조회된 고객사 코드,고객사 이름
            var strCstCD = this.uCtrlCst.CodeCst.Trim();
            var strCstNM = this.uCtrlCst.NameCst.Trim();                             // 고객사 명

            //고객사 입력 없이 조회버튼 누를 경우 
            if (string.IsNullOrEmpty(strCstCD) == true)
            {
                this.BaseClass.MsgInfo("ERR_NOT_INPUT", "CST");
                return;
            }

            var newItem = new BoxTypeMgnt
            {
                CST_CD = strCstCD,
                CST_NM = strCstNM,
                CNTR_CD = string.Empty,
                BOX_TYPE_CD = string.Empty,
                BOX_TYPE_NM = string.Empty,
                BOX_TYPE_DESC = string.Empty,
                BOX_TYPE_GRP_CD = string.Empty,
                BOX_WTH_LEN = 0,
                BOX_VERT_LEN = 0,
                BOX_HGT_LEN = 0,
                LEN_UOM = string.Empty,
                BOX_CBM = 0,
                CBM_UOM = string.Empty,
                BOX_WGT = 0,
                WGT_UOM = string.Empty,
                USE_YN = "Y",
                IsSelected = true,
                IsNew = true,
                IsSaved = false
            };

            this.BoxTypeMgntList.Add(newItem);
            this.gridMaster.Focus();
            this.gridMaster.CurrentColumn = this.gridMaster.Columns.First();
            this.gridMaster.View.FocusedRowHandle = this.BoxTypeMgntList.Count - 1;

            this.BoxTypeMgntList[this.BoxTypeMgntList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
            this.BoxTypeMgntList[this.BoxTypeMgntList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);

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
            BoxTypeMgnt dataMember = tv.Grid.CurrentItem as BoxTypeMgnt;

            if (dataMember == null) { return; }


            switch (e.Column.FieldName)
            {
                // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                // 박스 유형 코드는 수정이 되지 않도록 처리한다.
                case "BOX_TYPE_CD":
                    if (dataMember.IsNew == false)
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
