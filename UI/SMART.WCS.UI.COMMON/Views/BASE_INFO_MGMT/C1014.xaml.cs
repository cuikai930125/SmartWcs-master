using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using LGCNS.ezControl.Core;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1014;
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

namespace SMART.WCS.UI.COMMON.Views.BASE_INFO_MGMT
{
    /// <summary>
    /// C1014.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class C1014 : UserControl, TabCloseInterface
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
        /// 화면 로드 여부
        /// </summary>
        private bool g_IsLoaded = false;

        /// <summary>
        /// 화면 언로드 여부
        /// </summary>
        private bool g_IsUnLoaded = false;

        /// <summary>
        /// EzControl과 통신하기위한 Reference를 선언한다. 
        /// </summary>
        private CReference _reference = null;

        /// <summary>
        /// CJ때는 ElementNo를 DB에서 가져왔었는데 어차피 소터 하나인데 DB갔다오는게 안좋을 것 같아서 그냥 선언했음
        /// </summary>
        private int ElementNo = 1;

        /// <summary>
        /// 소터 가동 상태값을 받아온다.
        /// </summary>
        private short SorterStatus = 0;
        #endregion

        #region ▩ 생성자
        public C1014()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1014(List<string> _liMenuNavigation)
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
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1014), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 그리드 - 설비 리스트
        public static readonly DependencyProperty EquipmentMgntListProperty
            = DependencyProperty.Register("EquipmentMgntList", typeof(ObservableCollection<EquipmentMgnt>), typeof(C1014)
                , new PropertyMetadata(new ObservableCollection<EquipmentMgnt>()));

        public ObservableCollection<EquipmentMgnt> EquipmentMgntList
        {
            get { return (ObservableCollection<EquipmentMgnt>)GetValue(EquipmentMgntListProperty); }
            set { SetValue(EquipmentMgntListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1014), new PropertyMetadata(string.Empty));

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
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            // 공통코드 조회 파라메터 string[]
            string[] commonParam_LOCCode = { "KY", string.Empty, string.Empty, string.Empty };

            // 콤보박스 - 조회 (사용여부, 설비 종류 코드, 위치 코드)
            //this.BaseClass.BindingCommonComboBox(this.cboUseYN_First, "USE_YN", null, false);
            this.BaseClass.BindingCommonComboBox(this.cboEqpTypeCd, "EQP_TYPE_CD", null, true);
            //this.BaseClass.BindingCommonComboBox(this.cboLocCd, "LOC_CD", commonParam_LOCCode, true);

            // 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd_First.ToolTip        = this.BaseClass.GetResourceValue("ROW_ADD");
            this.btnRowDelete_First.ToolTip     = this.BaseClass.GetResourceValue("ROW_DEL");

            this.lblEqpTypeCD.Visibility    = Visibility.Hidden;
            this.cboEqpTypeCd.Visibility    = Visibility.Hidden;

            if (this.BaseClass.CenterCD.Equals("BC") == true)
            {
                this.gridMaster.Columns["EQP_TYPE_CD"].Visible      = true;
                this.gridMaster.Columns["PCL_DTM_CD"].Visible       = false;
                this.gridMaster.Columns["PCL_DTM_PROR_CD"].Visible  = false;
                this.gridMaster.Columns["CHUTE_OPR_CD"].Visible     = false;
                this.gridMaster.Columns["RECIRC_CNT"].Visible       = false;

                this.lblConnStatus.Visibility   = Visibility.Hidden;
                this.bdConnStatus.Visibility    = Visibility.Hidden;
            }
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 화면 이벤트
            this.Loaded += C1014_Loaded;
            //화면 종료 시 ECS 연결 해제 
            this.Unloaded += C1014_Unloaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 조회
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_First_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            // 저장
            this.btnSave_First.PreviewMouseLeftButtonUp += BtnSave_First_PreviewMouseLeftButtonUp;

            // 행 추가
            this.btnRowAdd_First.PreviewMouseLeftButtonUp += BtnRowAdd_First_PreviewMouseLeftButtonUp;
            // 행 삭제
            this.btnRowDelete_First.PreviewMouseLeftButtonUp += BtnRowDelete_First_PreviewMouseLeftButtonUp;
            #endregion
            
            #region + 그리드 이벤트
            // 그리드 클릭 이벤트
            this.gridMaster.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;

            // Equipment 리스트 그리드 순번 채번을 위한 이벤트
            this.gridMaster.CustomUnboundColumnData += GridMaster_CustomUnboundColumnData;

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
            this.GridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                       // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");

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
                int iCheckedCount = 0;

                iCheckedCount = this.EquipmentMgntList.Where(p => p.IsSelected == true).Count();
                 
                if (iCheckedCount == 0)
                {
                    bRtnValue = false;
                    BaseClass.MsgError("ERR_NO_SELECT");
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
            var liEquipmentMgnt = this.EquipmentMgntList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();

            if (liEquipmentMgnt.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }

            liEquipmentMgnt.ForEach(p => EquipmentMgntList.Remove(p));
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

            if (this.EquipmentMgntList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
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

        #region >>Connection Element - ECS 연결
        private void ConnectElement()
        {            
            try
            {
                ucConn1.ElementNo       = ElementNo;
                _reference              = CReferenceManager.GetReference("SorterServer", "SetConfiguration", "172.27.239.11", "9888", "Tcp", LGCNS.ezControl.Common.enumSessionType.Element, null);
                ucConn1.OnPanelUIEvent  += uCCommunication_OnPanelUIEvent;
                ucConn1.LinkedReference = _reference;

                if (_reference != null)
                    ucConn1.LinkedReference.Start();
            }
            catch { throw; }
        }
        #endregion

        #region >>Connection Element - ECS 연결 해제
        private void DisconnectElement()
        {
            try
            {
                if (ucConn1.LinkedReference != null)
                {
                    ucConn1.LinkedReference.Stop();
                    ucConn1.OnPanelUIEvent -= uCCommunication_OnPanelUIEvent;
                    ucConn1.LinkedReference = null;
                }
            }
            catch { throw; }
        }
        #endregion

        #region >> Core to UI Event  화면에서 ECS 데이터 수신
        void uCCommunication_OnPanelUIEvent(int iEventID, params object[] args)
        {
            try
            {
                if (this.BaseClass.CenterCD.Equals("BC")) { return; }

                switch ((BaseEnumClass.EnumToUIEvent)iEventID)
                {
                    case BaseEnumClass.EnumToUIEvent.Connected:
                        {
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                            {
                                SetConnectState(true);
                            }));
                        }
                        break;

                    case BaseEnumClass.EnumToUIEvent.Disconnected:
                        {
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                            {
                                SetConnectState(false);
                            }));
                        }
                        break;

                    case BaseEnumClass.EnumToUIEvent.SetConfigurationAck:
                        {
                            // 양산센터인 경우만 적용
                            if (this.BaseClass.CenterCD.ToUpper().Equals("YS"))
                            {
                                if (args[0].ToString().Equals("1"))
                                {
                                    // 소터가 가동 중입니다.소터 정지 후에 변경하세요.
                                    this.BaseClass.MsgInfo("INFO_SRT_PROC_STOP_CHG");
                                }
                                else if (args[0].ToString().Equals("2"))
                                {
                                    // 소터 설정이 변경되었습니다.
                                    this.BaseClass.MsgInfo("INFO_CMPT_SRT_SETTING");
                                }
                                else if (args[0].ToString().Equals("3"))
                                {
                                    // SMS와 PLC의 연결이 끊겨 있습니다.|확인 후 다시 시도해주세요.
                                    this.BaseClass.MsgInfo("INFO_SMS_PLC_NOT_CONNECT_RETRY");
                                }
                            }
                        }
                        break;

                    case BaseEnumClass.EnumToUIEvent.GetSorterStatusAck:
                        {
                            //양산센터인 경우만 적용
                            if (this.BaseClass.CenterCD.ToUpper().Equals("YS"))
                            {
                                //소터 가동 상태값 저장
                                this.SorterStatus = short.Parse(args[0].ToString());
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                this.BaseClass.Error(ex);
            }
        }
        #endregion

        #region >> ECS 연결 상태 값 변경
        void SetConnectState(bool state)
        {
            try
            {
                string strMessage = string.Empty;

                if (state)
                {
                    //ECS가 연결 되었습니다. 
                    this.lblConnStatus.Content      = "Connected";
                    this.bdConnStatus.Background    = new SolidColorBrush(Colors.Green);
                    this.BaseClass.Info("ECS가 연결 되었습니다.");
                }
                else
                {
                    //ECS가 연결 되지 않았습니다. 
                    this.lblConnStatus.Content      = "Disconnected";
                    this.bdConnStatus.Background    = new SolidColorBrush(Colors.Red);
                    this.BaseClass.Info("ECS가 연결 되지 않았습니다.");
                }
            }
            catch { throw; }
        }
        #endregion

        #endregion

        #region > 데이터 관련

        #region >> GetSP_EQP_LIST_INQ - Equipment List 조회
        /// <summary>
        /// 설비 관리 데이터조회
        /// </summary>
        private DataSet GetSP_EQP_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1014.SP_EQP_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_EQP_LIST", "O_RSLT" };
            
            var strCntrCd       = this.BaseClass.CenterCD;                                                // 센터 코드
            var strEqpID        = this.txtEqpId_First.Text.Trim();                                         // 설비 ID
            var strEqpNm        = this.txtEqpNm_First.Text.Trim();                                         // 설비 명
            //var strEqpTypeCd  = this.BaseClass.ComboBoxSelectedDisplayValue(this.cboEqpTypeCd);
            var strEqpTypeCd    = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqpTypeCd);          // 설비 종류 코드
            //var strLocCd      = this.BaseClass.ComboBoxSelectedKeyValue(this.cboLocCd);                  // 위치 코드
            //var strUseYn      = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN_First);            // 사용 여부

            var strErrCode      = string.Empty;         // 오류 코드
            var strErrMsg       = string.Empty;         // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                  // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpID);                    // 설비 ID
            dicInputParam.Add("P_EQP_NM", strEqpNm);                    // 설비 명
            dicInputParam.Add("P_EQP_TYPE_CD", strEqpTypeCd);           // 설비 종류 코드
            //dicInputParam.Add("P_LOC_CD", string.IsNullOrEmpty(strLocCd));                    // 위치 코드
            //dicInputParam.Add("P_USE_YN", strUseYn);                    // 사용 여부
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

        #region >> InsertSP_EQP_INS - Equipment 등록
        /// <summary>
        /// Equipment 등록
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool InsertSP_EQP_INS(BaseDataAccess _da, EquipmentMgnt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "PK_C1014.SP_EQP_INS";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };
            
            var strCntrCd       = BaseClass.CenterCD;                   // 센터 코드
            var strEqpID        = _item.EQP_ID;                         // 설비 ID
            var strEqpNm        = _item.EQP_NM;                         // 설비 명
            var strEqpDesc      = _item.EQP_DESC;                       // 설비 세부 정보
            var strEqpTypeCd    = _item.EQP_TYPE_CD;                    // 설비 종류 코드
            var strLinkEqpId    = _item.LINK_EQP_ID;                    // 연결설비ID
            var strLocCd        = _item.LOC_CD;                         // 위치 코드
            var iRecircCnt      = this.BaseClass.CenterCD.Equals("BC") ? 0 : Convert.ToInt32(_item.RECIRC_CNT);
            var strUseYN        = _item.Checked == true ? "Y" : "N";    // 사용 여부
            var strPclDtmCD     = _item.PCL_DTM_CD;                     // Parcel 결정
            var strPclDtmProrCD = _item.PCL_DTM_PROR_CD;                // Parcel 결정 우선순위
            var strChuteOprCD   = _item.CHUTE_OPR_CD;                   // 슈트운영코드
            var strUserID       = this.BaseClass.UserID;                // 사용자 ID
            var strErrCode      = string.Empty;                         // 오류 코드
            var strErrMsg       = string.Empty;                         // 오류 메세지
            #endregion

            #region Input 파라메터     
            dicInputParam.Add("P_CNTR_CD",          strCntrCd);         // 센터 코드
            dicInputParam.Add("P_EQP_ID",           strEqpID);          // 설비 ID
            dicInputParam.Add("P_EQP_NM",           strEqpNm);          // 설비 명
            dicInputParam.Add("P_EQP_DESC",         strEqpDesc);        // 설비 세부 정보
            dicInputParam.Add("P_EQP_TYPE_CD",      strEqpTypeCd);      // 설비 종류 코드
            dicInputParam.Add("P_LINK_EQP_ID",      strLinkEqpId);      // 연결설비ID
            dicInputParam.Add("P_LOC_CD", string.IsNullOrEmpty(strLocCd));          // 위치 코드
            dicInputParam.Add("P_RECIRC_CNT",       iRecircCnt);        // 순환횟수
            dicInputParam.Add("P_USE_YN",           strUseYN);          // 사용 여부
            dicInputParam.Add("P_USER_ID",          strUserID);         // 사용자 ID
            dicInputParam.Add("P_PCL_DTM_CD",       strPclDtmCD);       // Parcel 결정
            dicInputParam.Add("P_PCL_DTM_PROR_CD",  strPclDtmProrCD);   // parcel 결정 우선순위
            dicInputParam.Add("P_CHUTE_OPR_CD",     strChuteOprCD);     // 슈트운영코드
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

        #region >> UpdateSP_EQP_UPD - Equipment 수정
        /// <summary>
        /// Equipment 수정
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool UpdateSP_EQP_UPD(BaseDataAccess _da, EquipmentMgnt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "PK_C1014.SP_EQP_UPD";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCoCd         = BaseClass.CompanyCode;    // 회사 코드
            var strCntrCd       = BaseClass.CenterCD;       // 센터 코드
            var strEqpID        = _item.EQP_ID;             // 설비 ID
            var strEqpNm        = _item.EQP_NM;             // 설비 명
            var strEqpDesc      = _item.EQP_DESC;           // 설비 세부 정보
            var strEqpTypeCd    = _item.EQP_TYPE_CD;        // 설비 종류 코드
            var strLinkEqpId    = _item.LINK_EQP_ID;        // 연결설비ID
            var strLocCd        = _item.LOC_CD;             // 위치 코드

            var iRecircCnt = this.BaseClass.CenterCD.Equals("BC") ? 0 : Convert.ToInt32(_item.RECIRC_CNT);
            /*
            var strPcIp         = _item.PC_IP;              // 설비랑 통신하는 PC IP
            var strEcsCommNo    = _item.ECS_COMM_NO;        // 설비 ECS 통신 번호
            var strSerCommNo    = _item.SER_COMM_NO;        // 시리얼 통신 번호
            var strRecircCnt    = _item.RECIRC_CNT;         // 순환횟수
            var strZoneId       = _item.ZONE_ID;            // ZONE ID                       // 문자속성 10
            */

            var strPclDtmCD     = _item.PCL_DTM_CD;                     // Parcel 결정
            var strPclDtmProrCD = _item.PCL_DTM_PROR_CD;                // Parcel 결정 우선순위
            var strChuteOprCD   = _item.CHUTE_OPR_CD;                   // 슈트운영코드
            var strUseYN        = _item.Checked == true ? "Y" : "N";    // 사용 여부
            var strUserID       = this.BaseClass.UserID;                // 사용자 ID
            var strErrCode      = string.Empty;                         // 오류 코드
            var strErrMsg       = string.Empty;                         // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD",            strCoCd);           // 회사 코드       
            dicInputParam.Add("P_CNTR_CD",          strCntrCd);         // 센터 코드
            dicInputParam.Add("P_EQP_ID",           strEqpID);          // 설비 ID
            dicInputParam.Add("P_EQP_NM",           strEqpNm);          // 설비 명
            dicInputParam.Add("P_EQP_DESC",         strEqpDesc);        // 설비 세부 정보
            dicInputParam.Add("P_EQP_TYPE_CD",      strEqpTypeCd);      // 설비 종류 코드
            dicInputParam.Add("P_LINK_EQP_ID",      strLinkEqpId);      // 연결설비ID
            dicInputParam.Add("P_LOC_CD",           string.IsNullOrEmpty(strLocCd));          // 위치 코드
            dicInputParam.Add("P_RECIRC_CNT",       iRecircCnt);        // 순환횟수
            /*
            dicInputParam.Add("P_ECS_PC_IP", strPcIp);          // 설비랑 통신하는 PC IP
            dicInputParam.Add("P_ECS_COMM_NO", strEcsCommNo);   // 설비 ECS 통신 번호
            dicInputParam.Add("P_SER_COMM_NO", strSerCommNo);   // 시리얼 통신 번호
            dicInputParam.Add("P_RECIRC_CNT", strRecircCnt);    // 순환횟수
            dicInputParam.Add("P_ZONE", strZoneId);             // ZONE ID
            */

            dicInputParam.Add("P_USE_YN",               strUseYN);          // 사용 여부
            dicInputParam.Add("P_USER_ID",              strUserID);         // 사용자 ID
            dicInputParam.Add("P_PCL_DTM_CD",           strPclDtmCD);       // Parcel 결정
            dicInputParam.Add("P_PCL_DTM_PROR_CD",      strPclDtmProrCD);   // parcel 결정 우선순위
            dicInputParam.Add("P_CHUTE_OPR_CD",         strChuteOprCD);     // 슈트운영코드
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

        #region >> GetSP_COM_SET_CONFIG - 소터 설정 정보 조회
        /// <summary>
        /// 설비 관리 데이터조회
        /// </summary>
        private DataSet GetSP_COM_SET_CONFIG(string _eqpId)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_COMMON.SP_COM_SET_CONFIG";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_CONFIG", "O_RSLT" };

            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            var strEqpID = _eqpId;                                       // 설비 ID

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                  // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpID);                    // 설비 ID
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

        #endregion

        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void C1014_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.g_IsLoaded == false)
                {
                    if (this.BaseClass.CenterCD.Equals("BC") == false)
                    {
                        ConnectElement();
                    }

                    this.g_IsLoaded = true;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > UnLoaded 이벤트
        private void C1014_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.g_IsUnLoaded == false)
                {
                    DisconnectElement();

                    this.g_IsUnLoaded = true;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 설비 관리
        #region >> 버튼 클릭 이벤트
        #region + 설비 관리 조회버튼 클릭 이벤트
        /// <summary>
        /// 설비 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var ChangedRowData = this.EquipmentMgntList.Where(p => p.IsSelected == true).ToList();

            if (ChangedRowData.Count > 0)
            {
                var strMessage = this.BaseClass.GetResourceValue("ASK_EXISTS_NO_SAVE_TO_SEARCH", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    EquipmentListSearch();
                }
            }
            else
            {
                EquipmentListSearch();
            }
        }
        #endregion

        #region + 설비 관리 조회
        /// <summary>
        /// 설비 관리 조회
        /// </summary>
        private void EquipmentListSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_EQP_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.EquipmentMgntList = new ObservableCollection<EquipmentMgnt>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.EquipmentMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.EquipmentMgntList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.EquipmentMgntList;

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

        #region + 설비 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 설비 관리 엑셀 다운로드 버튼 클릭 이벤트
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
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }

        }
        #endregion

        #region + 설비 관리 저장 버튼 클릭 이벤트
        /// <summary>
        /// 설비 관리 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                var strMessage = "{0} 이(가) 입력되지 않았습니다.";

                //소터가 가동 중인지 확인하는 통신
                if (this.BaseClass.CenterCD.Equals("BC") == false)
                {
                    if (ucConn1.LinkedReference != null)
                    {
                        ucConn1.SendCommand(BaseEnumClass.EnumToCoreEvent.GetSorterStatus, 0);
                    }
                    else
                    {
                        strMessage = this.BaseClass.GetResourceValue("NO_ECS_CONNECTION", BaseEnumClass.ResourceType.MESSAGE);
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                    }
                }

                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                //소터가 정지상태가 아니라면
                if (this.SorterStatus != 2)
                {
                    if (this.SorterStatus == 1)
                    {
                        // 소터가 가동 중입니다.소터 정지 후에 변경하세요.
                        this.BaseClass.MsgInfo("INFO_SRT_PROC_STOP_CHG");
                    }
                    else if (this.SorterStatus == 3)
                    {
                        // SMS와 PLC의 연결이 끊겨 있습니다. 확인 후 다시 시도해주세요.
                        this.BaseClass.MsgInfo("INFO_SMS_PLC_NOT_CONNECT_RETRY");
                    }
                    return;
                }

                bool isRtnValue = false;

                this.EquipmentMgntList.ForEach(p => p.ClearError());

                foreach (var item in this.EquipmentMgntList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.EQP_ID) == true)
                        {
                            item.CellError("EQP_ID", string.Format(strMessage, this.GetLabelDesc("EQP_ID")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.EQP_NM) == true)
                        {
                            item.CellError("EQP_NM", string.Format(strMessage, this.GetLabelDesc("EQP_NM")));
                            return;
                        }
                    }
                }

                var liSelectedRowData = this.EquipmentMgntList.Where(p => p.IsSelected == true).ToList();

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            if (item.IsNew == true)
                            {
                                isRtnValue = this.InsertSP_EQP_INS(da, item);
                            }
                            else
                            {
                                isRtnValue = this.UpdateSP_EQP_UPD(da, item);
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

                            foreach (var item in liSelectedRowData)
                            {
                                item.IsSaved = true;
                            }

                            // 저장 후 저장내용 List에 출력 : Header
                            DataSet dsRtnValue = this.GetSP_EQP_LIST_INQ();

                            this.EquipmentMgntList = new ObservableCollection<EquipmentMgnt>();
                            this.EquipmentMgntList.ToObservableCollection(dsRtnValue.Tables[0]);

                            this.gridMaster.ItemsSource = this.EquipmentMgntList;

                            if (this.BaseClass.CenterCD.Equals("BC") == false)
                            {
                                DataSet dsRtnCofigValue = new DataSet();

                                //ECS에 소터 설정 값 전송  
                                // DB에서 데이터 갖고 오는 프로시져 필요함. 
                                foreach (var item in liSelectedRowData)
                                {
                                    dsRtnCofigValue = this.GetSP_COM_SET_CONFIG(item.EQP_ID);
                                }

                                string reCirculationCnt = dsRtnCofigValue.Tables[0].Rows[0]["RECIRC_CNT"].ToString();
                                string pbRjtChuteId = dsRtnCofigValue.Tables[0].Rows[0]["PB_RJT_CHUTE_ID"].ToString();
                                string pbRwrkChuteId = dsRtnCofigValue.Tables[0].Rows[0]["PB_RJT_CHUTE_ID"].ToString();
                                string boxRjtChuteId = dsRtnCofigValue.Tables[0].Rows[0]["BOX_RJT_CHUTE_ID"].ToString();
                                string boxRwrkChuteId = dsRtnCofigValue.Tables[0].Rows[0]["BOX_RJT_CHUTE_ID"].ToString();
                                string sorterMode = dsRtnCofigValue.Tables[0].Rows[0]["AI_MODE"].ToString();

                                if (ucConn1.LinkedReference != null)
                                {
                                    ucConn1.SendCommand(BaseEnumClass.EnumToCoreEvent.SetConfiguration,
                                    pbRjtChuteId, //PB Reject Chute,
                                    pbRwrkChuteId, //PB Rework Chute,
                                    boxRjtChuteId, //BOX Reject Chute,
                                    boxRwrkChuteId, //BOX Rework Chute,
                                    reCirculationCnt, //Recirculation Count,
                                    sorterMode); //Sorter type (1:AI, 2:Tray, 3:AI우선, 4:Tray우선)
                                }
                                else
                                {
                                    // ECS가 연결되지 않았습니다.
                                    this.BaseClass.MsgError("NO_ECS_CONNECTION");
                                }
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
            var newItem = new EquipmentMgnt
            {
                EQP_ID          = string.Empty,
                EQP_NM          = string.Empty,
                EQP_DESC        = string.Empty,
                EQP_TYPE_CD     = "SRT",
                LINK_EQP_ID     = string.Empty,
                LOC_CD          = string.Empty,
                USE_YN          = "Y",
                /*
                PC_IP           = string.Empty,
                ECS_COMM_NO     = string.Empty,
                SER_COMM_NO     = string.Empty,
                RECIRC_CNT      = string.Empty,
                ZONE_ID         = string.Empty,
                */

                PCL_DTM_CD      = string.Empty,     // Parcel 결정
                PCL_DTM_PROR_CD = string.Empty,     // parcel 결정 우선순위
                CHUTE_OPR_CD    = string.Empty,     // 슈트운영코드
                
                IsSelected      = true,
                IsNew           = true,
                IsSaved         = false
            };

            this.EquipmentMgntList.Add(newItem);
            this.gridMaster.Focus();
            this.gridMaster.CurrentColumn           = this.gridMaster.Columns.First();
            this.gridMaster.View.FocusedRowHandle   = this.EquipmentMgntList.Count - 1;

            this.EquipmentMgntList[this.EquipmentMgntList.Count - 1].BackgroundBrush        = new SolidColorBrush(Colors.White);
            this.EquipmentMgntList[this.EquipmentMgntList.Count - 1].BaseBackgroundBrush    = new SolidColorBrush(Colors.White);

            //var aaa = this.tvMasterGrid_First.FocusedRowHandle;
        }
        #endregion

        #region + 행삭제 버튼 클릭 이벤트
        private void BtnRowDelete_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CheckGridRowSelected() == false) { return; }

            this.DeleteGridRowItem();
            //var liSelectedRowData = this.EquipmentMgntList.Where(p => p.IsSelected == true).ToList();
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
                if (hi.Column.FieldName.Equals("EQP_ID")) { return; }

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
                EquipmentMgnt dataMember = tv.Grid.CurrentItem as EquipmentMgnt;

                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    // 설비 ID, 설비 명 컬럼은 수정이 되지 않도록 처리한다.
                    case "EQP_ID":
                    //case "EQP_NM":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    case "PCL_DTM_PROR_CD":
                        // Parcel결정 (PCL_DTM_CD)값이 AT인 경우만 선택할 수 있다.
                        if (dataMember.PCL_DTM_CD.Equals("AT") == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    default: break;
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