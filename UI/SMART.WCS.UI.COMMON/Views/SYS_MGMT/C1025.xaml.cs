using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1025;
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
    /// C1025.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1025 : UserControl, TabCloseInterface
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
        #endregion

        #region ▩ 생성자
        public C1025()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1025(List<string> _liMenuNavigation)
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
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1025), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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
        public static readonly DependencyProperty IPMgmtListProperty
            = DependencyProperty.Register("IPMgmtList", typeof(ObservableCollection<IPMgmt>), typeof(C1025)
                , new PropertyMetadata(new ObservableCollection<IPMgmt>()));

        private ObservableCollection<IPMgmt> IPMgmtList
        {
            get { return (ObservableCollection<IPMgmt>)GetValue(IPMgmtListProperty); }
            set { SetValue(IPMgmtListProperty, value); }
        }
    
        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1025), new PropertyMetadata(string.Empty));

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
            // 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd_First.ToolTip = this.BaseClass.GetResourceValue("ROW_ADD");
            this.btnRowDelete_First.ToolTip = this.BaseClass.GetResourceValue("ROW_DEL");
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        private void InitEvent()
        {
            #region + 버튼 클릭 이벤트
            // 조회
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_PreviewMosueLeftButtonUp;
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
        private bool CheckGridRowSelected()
        {
            try
            {
                bool bRtnValue = true;
                int iCheckedCount = 0;

                iCheckedCount = this.IPMgmtList.Where(p => p.IsSelected == true).Count();

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

        #region >> DeleteGridRowItem - 선택한 그리드의 Row를 삭제한다.
        /// <summary>
        /// 선택한 그리드의 Row를 삭제한다.
        /// </summary>
        private void DeleteGridRowItem()
        {
            var liEquipmentMgnt = this.IPMgmtList.Where(p => p.IsSelected == true).ToList();

            if (liEquipmentMgnt.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }

            liEquipmentMgnt.ForEach(p => IPMgmtList.Remove(p));
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

            if (this.IPMgmtList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
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

        #region >> GetSP_IP_LIST_INQ - IP List 조회
        /// <summary>
        /// IP List 조회
        /// </summary>
        /// <returns></returns>
        private DataSet GetSP_IP_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1025.SP_IP_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_IP_LIST", "O_RSLT" };

            var strCenterCD     = this.BaseClass.CenterCD;      // 센터코드
            var strIpNo         = this.txtIpNo.Text.Trim();     // IP NO

            var strErrCode = string.Empty;                  // 오류 코드
            var strErrMsg = string.Empty;                   // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",      strCenterCD);      // 센터코드
            dicInputParam.Add("P_IP_NO",        strIpNo);          // IP_NO
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

        #region >> InsertSP_IP_LIST_INS - IP 항목 추가
        /// <summary>
        /// IP 항목 추가
        /// </summary>
        /// <param name="_da"></param>
        /// <param name="_item"></param>
        /// <returns></returns>
        private bool InsertSP_IP_LIST_INS(BaseDataAccess _da, IPMgmt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1025.SP_IP_LIST_INS";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };
            
            var strCenterCD         = this.BaseClass.CenterCD;      // 센터코드ㅜ
            var strIpNo             = _item.IP_NO;                  // IP NO
            var strRmk              = _item.RMK;                    // 비고
            var strUserId           = this.BaseClass.UserID;        // 사용자 ID

            var strErrCode      = string.Empty;     // 오류 코드
            var strErrMsg       = string.Empty;     // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",      strCenterCD);       // 센터코드
            dicInputParam.Add("P_IP_NO",        strIpNo);           // IP 주소
            dicInputParam.Add("P_RMK",          strRmk);            // 비고
            dicInputParam.Add("P_USER_ID",      strUserId);         // 사용자 ID
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

        #region >> UpdateSP_IP_LIST_UPD - IP List 수정
        private bool UpdateSP_IP_LIST_UPD(BaseDataAccess _da, IPMgmt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1025.SP_IP_LIST_UPD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strCenterCD     = this.BaseClass.CenterCD;      // 센터코드
            var strIpNo         = _item.IP_NO;                  // IP NO
            var strRmk          = _item.RMK;                    // 비고
            var strUserId       = this.BaseClass.UserID;        // 사용자 ID

            var strErrCode = string.Empty;                  // 오류 코드
            var strErrMsg = string.Empty;                   // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",      strCenterCD);       // 센터코드
            dicInputParam.Add("P_IP_NO",        strIpNo);
            dicInputParam.Add("P_RMK",          strRmk);
            dicInputParam.Add("P_USER_ID",      strUserId);
            #endregion

            #region 데이터 조회
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
            #endregion

            return isRtnValue;
        }
        #endregion

        #region >> DeleteSP_IP_LIST_DEL - IP 항목 삭제
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (</param>
        /// <returns></returns>
        private bool DeleteSP_IP_LIST_DEL(BaseDataAccess _da, IPMgmt _item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1025.SP_IP_LIST_DEL";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;      // 센터코드
            var strIpNo             = _item.IP_NO;                  // IP NO

            var strErrCode = string.Empty;                  // 오류 코드
            var strErrMsg = string.Empty;                   // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",      strCenterCD);   // 센터코드
            dicInputParam.Add("P_IP_NO",        strIpNo);
            #endregion

            #region 데이터 조회
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
            #endregion

            return isRtnValue;
        }
        #endregion

        #endregion

        #endregion

        #region ▩ 이벤트

        #region > Loaded 이벤트
        private void C1025_Loaded(object sender, RoutedEventArgs e)
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

        #region > IP 리스트
        #region >> 버튼 클릭 이벤트

        #region + IP 리스트 조회버튼 클릭 이벤트
        /// <summary>
        /// IP 리스트 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMosueLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var ChangedRowData = this.IPMgmtList.Where(p => p.IsSelected == true).ToList();

                if (ChangedRowData.Count > 0)
                {
                    var strMessage = this.BaseClass.GetResourceValue("ASK_EXISTS_NO_SAVE_TO_SEARCH", BaseEnumClass.ResourceType.MESSAGE);

                    this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

                    if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                    {
                        IPMgmtListSearch();
                    }
                }
                else
                {
                    IPMgmtListSearch();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region + IP 리스트 조회
        /// <summary>
        /// IP 리스트 조회
        /// </summary>
        private void IPMgmtListSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // IP 리스트 조회
                DataSet dsRtnValue = this.GetSP_IP_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.IPMgmtList = new ObservableCollection<IPMgmt>();
                    // 오라클인 경우 TableName = TB_COM_IP_MST
                    this.IPMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.IPMgmtList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.IPMgmtList;

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

        #region + IP 리스트 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// IP 리스트 엑셀 다운로드 버튼 클릭 이벤트
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

        #region + IP 리스트 저장 버튼 클릭 이벤트
        private void BtnSave_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;

                this.IPMgmtList.ForEach(p => p.ClearError());

                // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                string strInputMessage = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.MESSAGE);

                // 필수값 미입력에 따른 에러 메시지 출력
                foreach (var item in this.IPMgmtList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.IP_NO) == true)
                        {
                            item.CellError("IP_NO", string.Format(strInputMessage, this.BaseClass.GetResourceValue("IP_NO")));
                        }
                    }
                }

                var liSelectedRowData = this.IPMgmtList.Where(p => p.IsSelected).ToList();

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
                                isRtnValue = this.InsertSP_IP_LIST_INS(da, item);
                            }
                            else
                            {
                                isRtnValue = this.UpdateSP_IP_LIST_UPD(da, item);
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
                        foreach (var item in liSelectedRowData)
                        {
                            item.IsSelected = false;
                        }

                        // 저장 후 저장내용 List에 출력 : Header
                        DataSet dsRtnValue = this.GetSP_IP_LIST_INQ();

                        this.IPMgmtList = new ObservableCollection<IPMgmt>();
                        this.IPMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);

                        this.gridMaster.ItemsSource = this.IPMgmtList;
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
        private void BtnRowAdd_First_PreviewMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            try
            {
                var newItem = new IPMgmt
                {
                    IP_NO = string.Empty
                    ,
                    RMK = string.Empty
                    ,
                    REG_ID = string.Empty
                    ,
                    REG_DT = string.Empty
                    ,
                    UPD_ID = string.Empty
                    ,
                    UPD_DT = string.Empty
                    ,
                    IsSelected = true
                    ,
                    IsNew = true
                    ,
                    IsSaved = false
                };

                this.IPMgmtList.Add(newItem);
                this.gridMaster.Focus();
                this.gridMaster.CurrentColumn = this.gridMaster.Columns.First();
                this.gridMaster.View.FocusedRowHandle = this.IPMgmtList.Count - 1;

                this.IPMgmtList[this.IPMgmtList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
                this.IPMgmtList[this.IPMgmtList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region + 행삭제 버튼 클릭 이벤트
        private void BtnRowDelete_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.CheckGridRowSelected() == false) { return; }

                // ASK_DEL : 삭제하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_DEL");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = false;

                var liSelectedRowData = this.IPMgmtList.Where(p => p.IsSelected).ToList();

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            isRtnValue = this.DeleteSP_IP_LIST_DEL(da, item);
                        }

                        if (isRtnValue == true)
                        {
                            // 삭제된 경우
                            da.CommitTransaction();

                            BaseClass.MsgInfo("CMPT_DEL");

                            foreach (var item in liSelectedRowData)
                            {
                                item.IsSaved = true;
                            }
                        }
                        else
                        {
                            da.RollbackTransaction();
                        }
                    }
                    catch
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        BaseClass.MsgError("ERR_DEL_DATA");
                        throw;
                    }
                    finally
                    {
                        // 상태바 (아이콘) 제거
                        this.loadingScreen.IsSplashScreenShown = false;

                        // 체크박스 해제
                        foreach (var item in liSelectedRowData)
                        {
                            item.IsSelected = false;
                        }

                        // 저장 후 저장내용 List에 출력 : Header
                        DataSet dsRtnValue = this.GetSP_IP_LIST_INQ();

                        this.IPMgmtList = new ObservableCollection<IPMgmt>();
                        this.IPMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);

                        this.gridMaster.ItemsSource = this.IPMgmtList;
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
                //var view = (sender as GridControl).View as TableView;
                //var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);
                //if (hi.InRowCell)
                //{
                //    if (view.ActiveEditor == null)
                //    {
                //        view.ShowEditor();

                //        if (view.ActiveEditor == null) { return; }
                //        Dispatcher.BeginInvoke(new Action(() => {
                //            view.ActiveEditor.EditValue = !(bool)view.ActiveEditor.EditValue;
                //        }), DispatcherPriority.Render);
                //    }
                //}
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
            try
            {
                if (g_IsAuthAllYN == false)
                {
                    e.Cancel = true;
                    return;
                }

                TableView tv = sender as TableView;

                if (tv.Name.Equals("tvMasterGrid") == true)
                {
                    IPMgmt dataMember = tv.Grid.CurrentItem as IPMgmt;

                    if (dataMember == null) { return; }

                    switch (e.Column.FieldName)
                    {
                        // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                        // 설비 ID, 설비 명 컬럼은 수정이 되지 않도록 처리한다.
                        case "IP_NO":
                            if (dataMember.IsNew == false)
                            {
                                e.Cancel = true;
                            }
                            break;
                        default: break;
                    }
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
