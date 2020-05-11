using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Control.Views;
using SMART.WCS.UI.Plan.DataMembers.P1008_SRT;
using System;
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

namespace SMART.WCS.UI.Plan.Views.SMS
{
    /// <summary>
    /// P1004_SRT.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class P1008_SRT : UserControl, TabCloseInterface
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
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        /// <summary>
        /// 엑셀 업로드 번호
        /// </summary>
        private string g_strUploadNo;
        #endregion

        #region ▩ Enum
        /// <summary>
        /// 조회, 슈트 플랜복사 
        /// </summary>
        private enum SEARCH_TYPE
        {
            SEARCH = 0,        // 조회
            POPUP = 1         // 슈트 플랜복사
        }
        #endregion

        #region ▩ 생성자
        public P1008_SRT()
        {
            InitializeComponent();
        }

        public P1008_SRT(List<string> _liMenuNavigation)
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

                this.RgnInfoList = new ObservableCollection<RgnInfo>();
                this.ChuteInfoList = new ObservableCollection<ChuteInfo>();
                this.ModifyRgnChuteInfoList = new ObservableCollection<ModifyRgnChuteInfo>();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > 권역코드 정보
        /// <summary>
        /// 권역코드 정보
        /// </summary>
        public static readonly DependencyProperty RgnInfoListProperty
            = DependencyProperty.Register("RgnInfoList", typeof(ObservableCollection<RgnInfo>), typeof(P1008_SRT)
                , new PropertyMetadata(new ObservableCollection<RgnInfo>()));

        /// <summary>
        /// 권역코드 정보
        /// </summary>
        private ObservableCollection<RgnInfo> RgnInfoList
        {
            get { return (ObservableCollection<RgnInfo>)GetValue(RgnInfoListProperty); }
            set { SetValue(RgnInfoListProperty, value); }
        }
        #endregion

        #region > 슈트 정보
        /// <summary>
        /// 슈트정보
        /// </summary>
        public static readonly DependencyProperty ChuteInfoListProperty
            = DependencyProperty.Register("ChuteInfoList", typeof(ObservableCollection<ChuteInfo>), typeof(P1008_SRT)
                , new PropertyMetadata(new ObservableCollection<ChuteInfo>()));

        /// <summary>
        /// 슈트정보
        /// </summary>
        private ObservableCollection<ChuteInfo> ChuteInfoList
        {
            get { return (ObservableCollection<ChuteInfo>)GetValue(ChuteInfoListProperty); }
            set { SetValue(ChuteInfoListProperty, value); }
        }
        #endregion

        #region > 권역, 슈트 정보 (수정되는 값)
        /// <summary>
        /// 슈트정보
        /// </summary>
        public static readonly DependencyProperty ModifyRgnChuteInfoListProperty
            = DependencyProperty.Register("ModifyRgnChuteInfo", typeof(ObservableCollection<ModifyRgnChuteInfo>), typeof(P1008_SRT)
                , new PropertyMetadata(new ObservableCollection<ModifyRgnChuteInfo>()));

        /// <summary>
        /// 슈트정보
        /// </summary>
        private ObservableCollection<ModifyRgnChuteInfo> ModifyRgnChuteInfoList
        {
            get { return (ObservableCollection<ModifyRgnChuteInfo>)GetValue(ModifyRgnChuteInfoListProperty); }
            set { SetValue(ModifyRgnChuteInfoListProperty, value); }
        }
        #endregion
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 컨트롤 초기화
        /// </summary>
        private void InitControl()
        {
            try
            {
                // 공통코드 조회 파라메터 string[]
                string[] commonParam_EQP_ID = { BaseClass.CenterCD, "SRT", BaseClass.UserID, string.Empty };
                this.BaseClass.BindingCommonComboBox(this.cboEqp, "EQP_ID", commonParam_EQP_ID, false);   // 설비
                if (this.BaseClass.ComboBoxItemCount(this.cboEqp) == 0)
                {
                    this.SetButtonAttributeByComboBox(false);
                }

                this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, true);  // 사용여부

                this.chkNotMnpgYN.IsChecked = true;

                this.SetButtonAttribute(false);
            }
            catch { throw; }
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        public void InitEvent()
        {
            try
            {
                // 조회 버튼 클릭
                this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
                // 저장 버튼 클릭
                this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;
                // 플랜 복사 버튼 클릭
                this.btnPlanCopy.PreviewMouseLeftButtonUp += BtnPlanCopy_PreviewMouseLeftButtonUp;
                // 엑셀 다운로드 버튼 클릭
                this.btnExcelUpload.PreviewMouseLeftButtonUp += BtnExcelUpload_PreviewMouseLeftButtonUp; ;
                // 템플릿 다운로드 버튼 클릭
                this.btnTemplateDownload.PreviewMouseLeftButtonUp += BtnTemplateDownload_PreviewMouseLeftButtonUp;

                // 권역, 슈트 정보 추가
                this.btnAdd.PreviewMouseLeftButtonUp += BtnAdd_PreviewMouseLeftButtonUp;
                // 추가된 권역, 슈트 정보 삭제
                this.btnDel.PreviewMouseLeftButtonUp += BtnDel_PreviewMouseLeftButtonUp;
            }
            catch { throw; }
        }

        #endregion
        #endregion

        #region > 기타함수
        private void SetButtonAttributeByComboBox(bool _isEnabled)
        {
            try
            {
                System.Windows.Visibility isVisibled  = System.Windows.Visibility.Collapsed;

                if (_isEnabled)
                {
                    isVisibled  = Visibility.Visible;
                }
                else
                {
                    isVisibled  = Visibility.Hidden;
                }

                // 저장 버튼
                this.btnSave.Visibility     = isVisibled;
                // 플랜 복사 버튼
                this.btnPlanCopy.Visibility = isVisibled;
                // 


            }
            catch { throw; }
        }

        #region >> SetButtonAttribute - 버튼 속성을 설정한다.
        /// <summary>
        /// 버튼 속성을 설정한다.
        /// </summary>
        /// <param name="_isEnabled"></param>
        private void SetButtonAttribute(bool _isEnabled)
        {
            try
            {
                this.txtChutePlanCodeCond.IsEnabled     = _isEnabled;
                this.txtChutePlanNMCond.IsEnabled       = _isEnabled;

                if (_isEnabled == true)
                {
                    this.txtChutePlanCodeCond.Background    = this.BaseClass.ConvertStringToMediaBrush("#FFFFFF");
                    this.txtChutePlanNMCond.Background      = this.BaseClass.ConvertStringToMediaBrush("#FFFFFF");

                }
                else
                {
                    this.txtChutePlanCodeCond.Background    = this.BaseClass.ConvertStringToMediaBrush("#F2F2F2");
                    this.txtChutePlanNMCond.Background      = this.BaseClass.ConvertStringToMediaBrush("#F2F2F2");
                }
            }
            catch { throw; }
        }
        #endregion

        #region >> TabClosing - 탭을 닫을 때 데이터 저장 여부 체크
        /// <summary>
        /// 탭을 닫을 때 데이터 저장 여부 체크
        /// </summary>
        /// <returns></returns>
        public bool TabClosing()
        {
            //return this.CheckModifyData();
            return true;
        }
        #endregion
        #endregion

        #region > 슈트 매핑 데이터 변경 여부 체크
        /// <summary>
        /// 슈트 매핑 데이터 변경 여부 체크
        /// </summary>
        /// <returns></returns>
        private bool CheckModifyChuteManp()
        {
            try
            {
                bool isRtnValue         = false;

                if (this.ModifyRgnChuteInfoList != null)
                {
                    var liRgnChuteList = this.ModifyRgnChuteInfoList.Where(p => p.IsNew || p.IsDelete).ToList();
                    if (liRgnChuteList.Count > 0)
                    {
                        // 저장하지 않은 데이터가 있습니다.|조회 하시겠습니까?
                        this.BaseClass.MsgQuestion("ASK_EXISTS_NO_SAVE_TO_SEARCH");
                        return this.BaseClass.BUTTON_CONFIRM_YN;
                    }
                }

                return isRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > 데이터 관련
        #region >> 조회 
        #region + GetSP_RGN_LIST_INQ - 권역 리스트 조회
        /// <summary>
        /// 권역 리스트 조회
        /// </summary>
        private void GetSP_RGN_LIST_INQ()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_P1008_SRT.SP_RGN_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RGN_LIST", "O_RSLT" };

                var strNotMpngYN    = this.chkNotMnpgYN.IsChecked == true ? "Y" : "N";
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_NOT_MPNG_YN",      strNotMpngYN);      // 미매핑여부
                #endregion

                #region 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }
                
                var strErrCode      = string.Empty;         // 오류 코드
                var strErrMsg       = string.Empty;         // 오류 메세지

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg))
                {
                    // 정상 조회된 경우
                    this.RgnInfoList = new ObservableCollection<RgnInfo>();
                    this.RgnInfoList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.RgnInfoList.ToObservableCollection(null);
                    this.BaseClass.MsgInfo(strErrMsg);
                }

                this.gridLeftTop.ItemsSource    = this.RgnInfoList;
                #endregion
            }
            catch { throw; }
        }
        #endregion

        #region + GetSP_CHUTE_LIST_INQ - 슈트 리스트 조회
        /// <summary>
        /// 슈트 리스트 조회
        /// </summary>
        private void GetSP_CHUTE_LIST_INQ()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_P1008_SRT.SP_CHUTE_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_CHUTE_LIST", "O_RSLT" };

                var strCenterCD     = this.BaseClass.CenterCD;                                  // 센터코드
                var strEqpID        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);     // 설비 ID
                var strNotMpngYN    = this.chkNotMnpgYN.IsChecked == true ? "Y" : "N";          // 미매핑 여부
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTR_CD",          strCenterCD);       // 센터코드
                dicInputParam.Add("P_EQP_ID",           strEqpID);          // 설비 ID
                dicInputParam.Add("P_NOT_MPNG_YN",      strNotMpngYN);      // 미매핑여부
                #endregion

                #region 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }
                
                var strErrCode      = string.Empty;         // 오류 코드
                var strErrMsg       = string.Empty;         // 오류 메세지

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg))
                {
                    // 정상 조회된 경우
                    this.ChuteInfoList = new ObservableCollection<ChuteInfo>();
                    this.ChuteInfoList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ChuteInfoList.ToObservableCollection(null);
                    this.BaseClass.MsgInfo(strErrMsg);
                }

                this.gridLeftBottom.ItemsSource    = this.ChuteInfoList;
                #endregion
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region + GetSP_RGN_CHUTE_LIST_INQ - 슈트 매핑조회
        /// <summary>
        /// 권역정보를 조회한다.
        /// </summary>
        /// <param name="_strChutePlanCD">슈트 플랜코드</param>
        /// <returns></returns>
        private void GetSP_RGN_CHUTE_LIST_INQ(string _strChutePlanCD)
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue = null;
                var strProcedureName = "PK_P1008_SRT.SP_RGN_CHUTE_LIST_INQ";
                Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
                string[] arrOutputParam = { "O_RGN_CHUTE_LIST", "O_RSLT" };

                var strCenterCD     = this.BaseClass.CenterCD;
                var strEqpID        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);
                var strPlanCD       = _strChutePlanCD;
                var strRgnCD        = this.txtRgnCD.Text.Trim();
                var strChuteID      = this.txtChute.Text.Trim();
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTR_CD",      strCenterCD);           // 센터코드
                dicInputParam.Add("P_EQP_ID",       strEqpID);              // 설비 ID
                dicInputParam.Add("P_PLAN_CD",      strPlanCD);             // 플랜 코드
                dicInputParam.Add("P_RGN_CD",       strRgnCD);              // 권역 코드
                dicInputParam.Add("P_CHUTE_ID",     strChuteID);            // 슈트 ID
                #endregion

                #region 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }
                
                var strErrCode      = string.Empty;         // 오류 코드
                var strErrMsg       = string.Empty;         // 오류 메세지

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg))
                {
                    // 정상 조회된 경우
                    this.ModifyRgnChuteInfoList = new ObservableCollection<ModifyRgnChuteInfo>();
                    this.ModifyRgnChuteInfoList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ModifyRgnChuteInfoList.ToObservableCollection(null);
                    this.BaseClass.MsgInfo(strErrMsg);
                }

                this.gridRight.ItemsSource    = this.ModifyRgnChuteInfoList;
                #endregion

                if (this.ModifyRgnChuteInfoList.Count() == 0)
                {
                    this.SetButtonAttribute(true);
                }
                else
                {
                    this.SetButtonAttribute(false);
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region >> 저장
        #region + SaveSP_CHUTE_PLAN_HDR_SAVE - 슈트 플랜 헤더 데이터 저장
        /// <summary>
        /// 슈트 플랜 헤더 데이터 저장
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <returns></returns>
        private bool SaveSP_CHUTE_PLAN_HDR_SAVE(BaseDataAccess _da)
        {
            try
            {
                bool isRtnValue = true;

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_P1008_SRT.SP_CHUTE_PLAN_HDR_SAVE";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strCenterCD             = this.BaseClass.CenterCD;                                  // 센터코드
                var strEqpID                = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);     // 설비 ID
                var strPlanCD               = this.txtChutePlanCodeCond.Text.Trim();                    // 슈트 플랜코드
                var strPlanNM               = this.txtChutePlanNMCond.Text.Trim();                      // 슈트 플랜명
                var strUserID               = this.BaseClass.UserID;                                    // 사용자 ID
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTER_CD",             strCenterCD);       // 센터코드
                dicInputParam.Add("P_EQP_ID",               strEqpID);          // 설비 ID
                dicInputParam.Add("P_PLAN_CD",              strPlanCD);         // 슈트 플랜코드
                dicInputParam.Add("P_PLAN_NM",              strPlanNM);         // 슈트 플랜명
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
            catch { throw; }
        }
        #endregion

        #region + SaveSP_CHUTE_PLAN_DTL_SAVE - 슈트 매핑 상세 저장
        /// <summary>
        /// 슈트 매핑 상세 저장
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_item">저장 대상 슈트 매핑 데이터</param>
        /// <returns></returns>
        private bool SaveSP_CHUTE_PLAN_DTL_SAVE(BaseDataAccess _da, ModifyRgnChuteInfo _item)
        {
            try
            {
                bool isRtnValue         = true;

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_P1008_SRT.SP_CHUTE_PLAN_DTL_SAVE";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strCenterCD             = this.BaseClass.CenterCD;                                  // 센터코드
                var strEqpID                = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);     // 설비 ID
                var strPlanCD               = this.txtChutePlanCodeCond.Text.Trim();                    // 슈트 플랜코드
                var strPlanNM               = this.txtChutePlanNMCond.Text.Trim();                      // 슈트 플랜명
                var strRgnCD                = _item.RGN_CD;                                             // 권역 코드
                var strChuteID              = _item.CHUTE_ID;                                           // 슈트 ID
                var strUserID               = this.BaseClass.UserID;                                    // 사용자 ID
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTER_CD",             strCenterCD);       // 센터코드
                dicInputParam.Add("P_EQP_ID",               strEqpID);          // 설비 ID
                dicInputParam.Add("P_PLAN_CD",              strPlanCD);         // 슈트 플랜코드
                dicInputParam.Add("P_PLAN_NM",              strPlanNM);         // 슈트 플랜명
                dicInputParam.Add("P_RGN_CD",               strRgnCD);          // 권역 코드
                dicInputParam.Add("P_CHUTE_ID",             strChuteID);        // 슈트 ID
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
            catch { throw; }
        }
        #endregion
        #endregion

        #region >> 삭제
        /// <summary>
        /// 슈트 매핑 데이터 삭제
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_item">삭제 대상 슈트 매핑 데이터</param>
        /// <returns></returns>
        private bool DeleteSP_CHUTE_PLAN_DTL_DEL(BaseDataAccess _da, ModifyRgnChuteInfo _item)
        {
            try
            {
                bool isRtnValue = true;

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_C1010.SP_COM_HDR_INS";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RSLT" };

                var strCenterCD             = this.BaseClass.CenterCD;                                  // 센터코드
                var strEqpID                = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);     // 설비 ID
                var strPlanCD               = this.txtChutePlanCodeCond.Text.Trim();                    // 플랜 코드
                var strRgnCD                = _item.RGN_CD;                                             // 권역 코드
                var strChuteID              = _item.CHUTE_ID;                                           // 슈트 ID
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
                dicInputParam.Add("P_EQP_ID",               strEqpID);          // 설비 ID
                dicInputParam.Add("P_PLAN_CD",              strPlanCD);         // 플랜코드
                dicInputParam.Add("P_RGN_CD",               strRgnCD);          // 권역코드
                dicInputParam.Add("P_CHUTE_ID",             strChuteID);        // 슈트 ID
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
                        BaseClass.MsgError("ERR_DEL");
                        isRtnValue = false;
                    }
                }

                return isRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 버튼 클릭 이벤트
        #region >> 조회 버튼 클릭
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.txtChutePlanCD.Text.Trim().Length == 0)
                {
                    // ERR_EMPTY_CHUTE_PLAN_CODE - 슈트 플랜코드를 입력하세요.
                    this.BaseClass.MsgError("ERR_EMPTY_CHUTE_PLAN_CODE");
                    this.txtChutePlanCD.Focus();
                    return;
                }

                // 슈트 매핑 데이터 변경 여부 체크
                if (this.CheckModifyChuteManp() == true) { return; }

                this.loadingScreen.IsSplashScreenShown = true;

                // 권역정보 조회
                this.GetSP_RGN_LIST_INQ();

                // 슈트 리스트 조회
                this.GetSP_CHUTE_LIST_INQ();

                // 슈트매핑 조회
                this.GetSP_RGN_CHUTE_LIST_INQ(this.txtChutePlanCD.Text.Trim());
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

        #region >> 저장 버튼 클릭
        /// <summary>
        /// 저장 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string strMessage   = string.Empty;

                if (this.ModifyRgnChuteInfoList.Count() == 0)
                {
                    // 저장할 슈트매핑 데이터가 없습니다.
                    this.BaseClass.MsgError("ERR_NOT_SAVE_CHUTE_MPNG");
                    return;
                }

                // 플랜코드 입력여부 체크
                if (this.txtChutePlanCodeCond.Text.Trim().Length == 0)
                {
                    // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                    strMessage = string.Format(this.BaseClass.GetResourceValue("ERR_NOT_INPUT"), this.BaseClass.GetResourceValue("PLAN_CD"));
                    this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                    return;
                }

                // 플랜명 입력여부 체크
                if (this.txtChutePlanNMCond.Text.Trim().Length == 0)
                {
                    // {0}이(가) 입력되지 않았습니다.
                    //ERR_NOT_INPUT
                    strMessage = string.Format(this.BaseClass.GetResourceValue("ERR_NOT_INPUT"), this.BaseClass.GetResourceValue("PLAN_NM"));
                    this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                    return;
                }

                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue     = false;

                //var liSelectedDeleteList    = this.ModifyRgnChuteInfoList.Where(p => p.IsSelected && p.IsDelete).ToList();
                //var liSelectedNewList       = this.ModifyRgnChuteInfoList.Where(p => p.IsSelected && p.IsNew && p.IsDelete == false).ToList();

                var liSelectedChuteMpng     = this.ModifyRgnChuteInfoList.Where(p => p.IsSelected).OrderBy(q => q.IsDelete == true).ToList();

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        int i = 0;

                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        isRtnValue = this.SaveSP_CHUTE_PLAN_HDR_SAVE(da);

                        foreach (var item in liSelectedChuteMpng)
                        {
                            if (item.IsDelete)
                            {
                                // 삭제 데이터 저장
                                isRtnValue = this.DeleteSP_CHUTE_PLAN_DTL_DEL(da, item);
                            }
                            else if (item.IsNew && (item.IsDelete == false))
                            {
                                isRtnValue = this.SaveSP_CHUTE_PLAN_DTL_SAVE(da, item);
                            }

                            if (isRtnValue == false) { break; }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우
                            da.CommitTransaction();

                            // 슈트매핑 조회
                            this.GetSP_RGN_CHUTE_LIST_INQ(this.txtChutePlanCD.Text.Trim());

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
                        // 상태바 (아이콘) 실행
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

        #region >> 플랜 복사 버튼 클릭
        /// <summary>
        /// 플랜 복사 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlanCopy_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strEqpID    = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);     // 설비 ID

                // 슈트 매핑 데이터 변경 여부 체크
                if (this.CheckModifyChuteManp() == true) { return; }

                using (P1008_SRT_01P frmPopup = new P1008_SRT_01P(strEqpID))
                {
                    frmPopup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    frmPopup.ShowDialog();

                    this.txtChutePlanCodeCond.Text      = frmPopup.NEW_PLAN_CD;
                    this.txtChutePlanNMCond.Text        = frmPopup.NEW_PLAN_NM;
                    var strPlanCD                       = frmPopup.CHUTE_PLAN_CD;

                    // 슈트 매핑 재조회
                    this.GetSP_RGN_CHUTE_LIST_INQ(strPlanCD);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 엑셀 업로드 버튼 클릭
        /// <summary>
        /// 엑셀 업로드 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelUpload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strEqpID        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp); // 설비 ID

                using (SWCS201_01P frmPopup = new SWCS201_01P(BaseEnumClass.ExcelUploadKind.CHUTE_MPNG_UPLOAD))
                {
                    frmPopup.ExcelUploadNo              += FrmPopup_ExcelUploadNo;
                    frmPopup.WindowStartupLocation      = WindowStartupLocation.CenterScreen;
                    frmPopup.EQP_ID                     = strEqpID;
                    frmPopup.ShowDialog();
                }

                if (this.g_strUploadNo == null) { return; }
                if (this.g_strUploadNo.Length > 0)
                {
                    using (P1008_SRT_02P frmResultPopup = new P1008_SRT_02P(strEqpID, this.g_strUploadNo))
                    {
                        frmResultPopup.WindowStartupLocation    = WindowStartupLocation.CenterScreen;
                        frmResultPopup.ShowDialog();
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void FrmPopup_ExcelUploadNo(string _strUploadNo)
        {
            try
            {
                this.g_strUploadNo  = _strUploadNo;
            }
            catch { throw; }
        }
        #endregion

        #region >> 템플릿 다운로드 버튼 클릭
        /// <summary>
        /// 템플릿 다운로드 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTemplateDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // ASK_TEMPLATE_DOWNLOAD - 템플릿을 다운로드 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_TEMPLATE_DOWNLOAD");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                List<TableView> tv = new List<TableView>();
                tv.Add(this.tvTemplateGrid);

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

        #region >> 권역, 슈트 정보 추가
        /// <summary>
        /// 추가된 권역, 슈트 정보 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.RgnInfoList.Where(p => p.IsSelected).Count() == 0)
                {
                    // 선택된 권역 정보가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT_RGN");
                    return;
                }

                if (this.ChuteInfoList.Where(p => p.IsSelected).Count() == 0)
                {
                    // 선택된 슈트 정보가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT_CHUTE");
                    return;
                }

                var liSelectedRgnInfo       = this.RgnInfoList.Where(p => p.IsSelected).ToList();
                var liSelectedChuteInfo     = this.ChuteInfoList.Where(p => p.IsSelected).ToList();

                foreach (var itemRgnInfo in liSelectedRgnInfo)
                {
                    foreach (var itemChuteInfo in liSelectedChuteInfo)
                    {
                        // 기존 조회 데이터 또는 신규 데이터는 추가되지 않도록
                        if (this.ModifyRgnChuteInfoList.Where(p => p.RGN_CD.Equals(itemRgnInfo.RGN_CD) && p.CHUTE_ID.Equals(itemChuteInfo.CHUTE_ID) && p.IsDelete == false).Count() > 0)
                        {
                            var strMessage  = this.BaseClass.GetResourceValue("ERR_EXIST_RGN_CHUTE");
                            strMessage      = string.Format(strMessage, itemRgnInfo.RGN_CD, itemChuteInfo.CHUTE_ID);
                            this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                            return;
                        }

                        if (this.ModifyRgnChuteInfoList.Where(p => p.RGN_CD.Equals(itemRgnInfo.RGN_CD) && p.CHUTE_ID.Equals(itemChuteInfo.CHUTE_ID) && p.IsDelete == true).Count() > 0)
                        {
                            this.ModifyRgnChuteInfoList.Where(p => p.RGN_CD.Equals(itemRgnInfo.RGN_CD) && p.CHUTE_ID.Equals(itemChuteInfo.CHUTE_ID) && p.IsDelete == true).ForEach(p => p.IsDelete = false);
                        }
                        else
                        {
                            var newChuteMpng = new ModifyRgnChuteInfo
                            {
                                    RGN_CD      = itemRgnInfo.RGN_CD
                                ,   RGN_NM      = itemRgnInfo.RGN_NM
                                ,   CHUTE_ID    = itemChuteInfo.CHUTE_ID
                            };

                        this.ModifyRgnChuteInfoList.Add(newChuteMpng);
                        }
                    }
                }

                this.gridRight.ItemsSource = this.ModifyRgnChuteInfoList.Where(p => p.IsDelete == false).ToList();

                this.gridRight.Focus();
                this.gridRight.CurrentColumn            = this.gridRight.Columns.First();
                this.gridRight.View.FocusedRowHandle    = this.ModifyRgnChuteInfoList.Count - 1;

                this.RgnInfoList.Where(p => p.IsSelected).ForEach(p => p.IsSelected = false);
                this.ChuteInfoList.Where(p => p.IsSelected).ForEach(p => p.IsSelected = false);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 추가된 권역, 슈트 정보 삭제
        /// <summary>
        /// 권역, 슈트 정보 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.ModifyRgnChuteInfoList.Where(p => p.IsSelected).Count() == 0)
                {
                    // 선택된 데이터가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return;
                }

                this.ModifyRgnChuteInfoList.Where(p => p.IsSelected).ForEach(p => p.IsDelete = true);

                this.gridRight.ItemsSource = this.ModifyRgnChuteInfoList.Where(p => p.IsDelete == false).ToList();
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
