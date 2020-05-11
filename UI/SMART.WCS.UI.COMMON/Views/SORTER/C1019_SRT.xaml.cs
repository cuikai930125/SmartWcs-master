using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using LGCNS.ezControl.Core;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1019_SRT;

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

namespace SMART.WCS.UI.COMMON.Views.SORTER
{
    /// <summary>
    /// C1019_SRT.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1019_SRT : UserControl, TabCloseInterface
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
        /// 조회한 EQP_ID 저장
        /// </summary>
        private string SearchedEQPId = null;

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
        public C1019_SRT()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1019_SRT(List<string> _liMenuNavigation)
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

                if (this.BaseClass.CenterCD.Equals("BC") == true)
                {
                    this.lblConnStatus.Visibility   = Visibility.Hidden;
                    this.bdConnStatus.Visibility    = Visibility.Hidden;
                }
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
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1019_SRT), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 그리드 - 슈트 리스트
        public static readonly DependencyProperty ChuteMgmtListProperty
            = DependencyProperty.Register("ChuteMgmtList", typeof(ObservableCollection<ChuteMgmt>), typeof(C1019_SRT)
                , new PropertyMetadata(new ObservableCollection<ChuteMgmt>()));

        private ObservableCollection<ChuteMgmt> ChuteMgmtList
        {
            get { return (ObservableCollection<ChuteMgmt>)GetValue(ChuteMgmtListProperty); }
            set { SetValue(ChuteMgmtListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1019_SRT), new PropertyMetadata(string.Empty));

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
            string[] commonParam_EQP_ID = { BaseClass.CenterCD, "SRT", BaseClass.UserID, string.Empty };

            // 콤보박스 - 조회 (사용여부, 슈트 종류 코드, 위치 코드)
            this.BaseClass.BindingCommonComboBox(this.cboEqpId, "EQP_ID", commonParam_EQP_ID, false);   // 설비
            this.BaseClass.BindingCommonComboBox(this.cboZoneID, "ZONE_ID", null, true);                // ZONE ID
            this.BaseClass.BindingCommonComboBox(this.cboChuteUseCd, "CHUTE_USE_CD", null, true);       // 
            this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, false);

            // 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd_First.ToolTip = this.BaseClass.GetResourceValue("ROW_ADD");
            this.btnRowDelete_First.ToolTip = this.BaseClass.GetResourceValue("ROW_DEL");

            if (this.BaseClass.CenterCD.ToUpper().Equals("YS"))
            {
                this.gridMaster.Columns["CHUTE_DTL_USE_CD"].Visible = false;
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
            this.Loaded += C1019_SRT_Loaded;
            //화면 종료 시 ECS 연결 해제 
            this.Unloaded += C1019_SRT_Unloaded;
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

            this.tvMasterGrid.CellValueChanged += TvMasterGrid_CellValueChanged;
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

                iCheckedCount = this.ChuteMgmtList.Where(p => p.IsSelected == true).Count();

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
            var liChuteMgmt = this.ChuteMgmtList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();

            if (liChuteMgmt.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }

            liChuteMgmt.ForEach(p => ChuteMgmtList.Remove(p));
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

            if (this.ChuteMgmtList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
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
                    this.lblConnStatus.Content = "Connected";
                    this.bdConnStatus.Background = new SolidColorBrush(Colors.Green);
                    this.BaseClass.Info("ECS가 연결 되었습니다.");
                }
                else
                {
                    //ECS가 연결 되지 않았습니다. 
                    this.lblConnStatus.Content = "Disconnected";
                    this.bdConnStatus.Background = new SolidColorBrush(Colors.Red);
                    this.BaseClass.Info("ECS가 연결 되지 않았습니다.");
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 데이터 관련

        #region >> GetSP_CHUTE_LIST_INQ - Chute List 조회
        /// <summary>
        /// 슈트 관리 데이터 조회
        /// </summary>
        private DataSet GetSP_CHUTE_LIST_INQ(string searchedEQPId)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1019_SRT.SP_CHUTE_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_CHUTE_LIST_LIST", "O_RSLT" };

            var strCntrCd           = this.BaseClass.CenterCD;                                      // 센터 코드
            var strChuteId          = this.txtChuteId_First.Text.Trim();                            // 슈트 ID
            var strChuteUseCd       = this.BaseClass.ComboBoxSelectedKeyValue(this.cboChuteUseCd);  // 슈트용도코드
            var strUseYn            = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);       // 사용 여부
            var strZoneID           = this.BaseClass.ComboBoxSelectedKeyValue(this.cboZoneID);      // ZONE ID

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCntrCd);         // 센터 코드
            dicInputParam.Add("P_EQP_ID",           searchedEQPId);     // 설비 ID
            dicInputParam.Add("P_CHUTE_ID",         strChuteId);        // 슈트 ID
            dicInputParam.Add("P_CHUTE_USE_CD",     strChuteUseCd);     // 슈트용도코드
            dicInputParam.Add("P_USE_YN",           strUseYn);          // 사용 여부
            dicInputParam.Add("P_ZONE_ID",          strZoneID);         // ZONE ID
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

        #region >> InsertSP_CHUTE_INS - Chute 등록
        /// <summary>
        /// Chute 등록
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool InsertSP_CHUTE_INS(BaseDataAccess _da, ChuteMgmt _item)
        {
            bool isRtnValue = true;

            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue = null;
                var strProcedureName = "PK_C1019_SRT.SP_CHUTE_INS";
                Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
                string[] arrOutputParam = { "O_RSLT" };

                var strCntrCd = BaseClass.CenterCD;                     // 센터 코드
                var strEqpID = SearchedEQPId;                           // 설비 ID
                var strChuteId = _item.CHUTE_ID;                        // 슈트 ID
                var strChuteNm = _item.CHUTE_NM;                        // CHUTE_NM
                var strChuteUseCd = _item.CHUTE_USE_CD;                 // CHUTE_USE_CD
                var strChuteDtlUseCd = _item.CHUTE_DTL_USE_CD;          // CHUTE_DTL_USE_CD
                var strZONE_ID = _item.ZONE_ID;                         // ZONE_ID
                var strPlcChuteId = _item.PLC_CHUTE_ID;                 // PLC_CHUTE_ID
                var strUseYN = _item.Checked == true ? "Y" : "N";       // 사용 여부
                var strUserID = this.BaseClass.UserID;                  // 사용자 ID

                var strErrCode = string.Empty;                          // 오류 코드
                var strErrMsg = string.Empty;                           // 오류 메세지
                #endregion

                #region Input 파라메터     
                dicInputParam.Add("P_CNTR_CD", strCntrCd);                      // 센터 코드
                dicInputParam.Add("P_EQP_ID", strEqpID);                        // 설비 ID
                dicInputParam.Add("P_CHUTE_ID", strChuteId);                    // 슈트 ID
                dicInputParam.Add("P_CHUTE_NM", strChuteNm);                    // CHUTE_NM
                dicInputParam.Add("P_CHUTE_USE_CD", strChuteUseCd);             // CHUTE_USE_CD
                dicInputParam.Add("P_CHUTE_DTL_USE_CD", strChuteDtlUseCd);      // CHUTE_DTL_USE_CD
                dicInputParam.Add("P_ZONE_ID", strZONE_ID);                     // ZONE_ID
                dicInputParam.Add("P_PLC_CHUTE_ID", strPlcChuteId);             // PLC_CHUTE_ID
                dicInputParam.Add("P_USE_YN", strUseYN);                        // 사용 여부
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
            }
            catch { throw; }

            return isRtnValue;
        }
        #endregion

        #region >> UpdateSP_CHUTE_UPD - Chute 수정
        /// <summary>
        /// Chute 수정
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool UpdateSP_CHUTE_UPD(BaseDataAccess _da, ChuteMgmt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1019_SRT.SP_CHUTE_UPD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strCntrCd = BaseClass.CenterCD;                     // 센터 코드
            var strEqpID = _item.EQP_ID;                            // 설비 ID
            var strChuteId = _item.CHUTE_ID;                        // 슈트 ID
            var strChuteNm = _item.CHUTE_NM;                        // CHUTE_NM
            var strChuteUseCd = _item.CHUTE_USE_CD;                 // CHUTE_USE_CD
            var strChuteDtlUseCd = _item.CHUTE_DTL_USE_CD;          // CHUTE_DTL_USE_CD
            var strZONE_ID = _item.ZONE_ID;                         // ZONE_ID
            var strPlcChuteId = _item.PLC_CHUTE_ID;                 // PLC_CHUTE_ID
            var strUseYN = _item.Checked == true ? "Y" : "N";       // 사용 여부
            var strUserID = this.BaseClass.UserID;                  // 사용자 ID

            var strErrCode = string.Empty;                          // 오류 코드
            var strErrMsg = string.Empty;                           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                      // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpID);                        // 설비 ID
            dicInputParam.Add("P_CHUTE_ID", strChuteId);                    // 슈트 ID
            dicInputParam.Add("P_CHUTE_NM", strChuteNm);                    // CHUTE_NM
            dicInputParam.Add("P_CHUTE_USE_CD", strChuteUseCd);             // CHUTE_USE_CD
            dicInputParam.Add("P_CHUTE_DTL_USE_CD", strChuteDtlUseCd);      // CHUTE_DTL_USE_CD
            dicInputParam.Add("P_ZONE_ID", strZONE_ID);                     // ZONE_ID
            dicInputParam.Add("P_PLC_CHUTE_ID", strPlcChuteId);             // PLC_CHUTE_ID
            dicInputParam.Add("P_USE_YN", strUseYN);                        // 사용 여부
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
        private void C1019_SRT_Loaded(object sender, RoutedEventArgs e)
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

        #region > Unloaded 이벤트
        private void C1019_SRT_Unloaded(object sender, RoutedEventArgs e)
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

        #region > 슈트 관리
        #region >> 버튼 클릭 이벤트

        #region + 슈트 관리 조회버튼 클릭 이벤트
        /// <summary>
        /// 슈트 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var ChangedRowData = this.ChuteMgmtList.Where(p => p.IsSelected == true).ToList();

            if (ChangedRowData.Count > 0)
            {
                var strMessage = this.BaseClass.GetResourceValue("ASK_EXISTS_NO_SAVE_TO_SEARCH", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    ChuteSearch();
                }
            }
            else
            {
                ChuteSearch();
            }
        }
        #endregion

        #region + 슈트 관리 조회
        /// <summary>
        /// 슈트 관리 조회
        /// </summary>
        private void ChuteSearch()
        {
            SearchedEQPId = null;
            SearchedEQPId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqpId);

            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_CHUTE_LIST_INQ(SearchedEQPId);

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.ChuteMgmtList = new ObservableCollection<ChuteMgmt>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.ChuteMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.ChuteMgmtList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.ChuteMgmtList;

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

        #region + 슈트 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 슈트 관리 엑셀 다운로드 버튼 클릭 이벤트
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

        #region + 슈트 관리 저장 버튼 클릭 이벤트
        /// <summary>
        /// 슈트 관리 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //// 그리드 내 체크박스 선택 여부 체크
                //if (this.CheckGridRowSelected() == false) { return; }

                //// ASK_SAVE - 저장하시겠습니까?
                //this.BaseClass.MsgQuestion("ASK_SAVE");
                //if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;

                // 2020-04-09 양산 추가
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

                this.ChuteMgmtList.ForEach(p => p.ClearError());

                //var strMessage = "{0} 이(가) 입력되지 않았습니다.";

                foreach (var item in this.ChuteMgmtList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.CHUTE_ID) == true)
                        {
                            item.CellError("CHUTE_ID", string.Format(strMessage, this.GetLabelDesc("CHUTE_ID")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.CHUTE_USE_CD) == true)
                        {
                            item.CellError("CHUTE_USE_CD", string.Format(strMessage, this.GetLabelDesc("CHUTE_USE_CD")));
                            return;
                        }
                    }
                }

                var liSelectedRowData = this.ChuteMgmtList.Where(p => p.IsSelected == true).ToList();

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
                                isRtnValue = this.InsertSP_CHUTE_INS(da, item);
                            }
                            else
                            {
                                isRtnValue = this.UpdateSP_CHUTE_UPD(da, item);
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

                            // 체크박스 해제
                            foreach (var item in liSelectedRowData)
                            {
                                item.IsSelected = false;
                            }

                            // 저장 후 저장내용 List에 출력 : Header
                            DataSet dsRtnValue = this.GetSP_CHUTE_LIST_INQ(SearchedEQPId);

                            this.ChuteMgmtList = new ObservableCollection<ChuteMgmt>();
                            this.ChuteMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);

                            this.gridMaster.ItemsSource = this.ChuteMgmtList;

                            DataSet dsRtnCofigValue = new DataSet();
                            //ECS에 소터 설정 값 전송  
                            // DB에서 데이터 갖고 오는 프로시져 필요함. 

                            string searchedEQPId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqpId);

                            if (this.BaseClass.CenterCD.Equals("BC") == false)
                            {
                                dsRtnCofigValue = this.GetSP_COM_SET_CONFIG(searchedEQPId);
                            
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
                                    strMessage = this.BaseClass.GetResourceValue("NO_ECS_CONNECTION", BaseEnumClass.ResourceType.MESSAGE);
                                    this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
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
                    finally
                    {
                        // 상태바 (아이콘) 제거
                        this.loadingScreen.IsSplashScreenShown = false;

                        //// 체크박스 해제
                        //foreach (var item in liSelectedRowData)
                        //{
                        //    item.IsSelected = false;
                        //}

                        //// 저장 후 저장내용 List에 출력 : Header
                        //DataSet dsRtnValue = this.GetSP_CHUTE_LIST_INQ(SearchedEQPId);

                        //this.ChuteMgmtList = new ObservableCollection<ChuteMgmt>();
                        //this.ChuteMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);

                        //this.gridMaster.ItemsSource = this.ChuteMgmtList;
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
            var newItem = new ChuteMgmt
            {
                EQP_ID = SearchedEQPId
                ,
                CHUTE_ID = string.Empty
                ,
                CHUTE_NM = string.Empty
                ,
                CHUTE_USE_CD = string.Empty
                ,
                CHUTE_DTL_USE_CD = string.Empty
                ,
                ZONE_ID = string.Empty
                ,
                PLC_CHUTE_ID = string.Empty
                ,
                USE_YN = "Y"
                ,
                IsSelected = true
                ,
                IsNew = true
                ,
                IsSaved = false
            };

            this.ChuteMgmtList.Add(newItem);
            this.gridMaster.Focus();
            this.gridMaster.CurrentColumn = this.gridMaster.Columns.First();
            this.gridMaster.View.FocusedRowHandle = this.ChuteMgmtList.Count - 1;

            this.ChuteMgmtList[this.ChuteMgmtList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
            this.ChuteMgmtList[this.ChuteMgmtList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);
        }
        #endregion

        #region + 행삭제 버튼 클릭 이벤트
        private void BtnRowDelete_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CheckGridRowSelected() == false) { return; }

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
                //if (hi.Column.FieldName.Equals("USE_YN") == false) { return; }

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
                ChuteMgmt dataMember = tv.Grid.CurrentItem as ChuteMgmt;

                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    // 슈트 ID 컬럼은 수정이 되지 않도록 처리한다.
                    case "CHUTE_ID":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    case "CHUTE_DTL_USE_CD":
                        // 슈트용도코드 (CHUTE_USE_CD)가 REJECT인 경우만 슈트상세용도코드를 활성화한다.
                        if (dataMember.CHUTE_USE_CD.Equals("RJT") == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    default: break;
                }
            }
        }
        #endregion

        #region > 컬럼 데이터 변경 후 이벤트 (컬럼 변경 후 다른 컬럼 값 변경시 사용)
        /// <summary>
        /// 컬럼 데이터 변경 후 이벤트 (컬럼 변경 후 다른 컬럼 값 변경시 사용)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvMasterGrid_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                TableView tv = sender as TableView;
                ChuteMgmt dataMember = tv.Grid.CurrentItem as ChuteMgmt;

                if (e.Column.FieldName.Equals("CHUTE_USE_CD"))
                {
                    if (dataMember.CHUTE_USE_CD.Equals("NML")) { dataMember.CHUTE_DTL_USE_CD = string.Empty; }
                }

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
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
