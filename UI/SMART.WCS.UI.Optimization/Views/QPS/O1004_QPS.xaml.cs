using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.Optimization.DataMembers.OPT0104;

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

namespace SMART.WCS.UI.Optimization.Views.QPS
{
    /// <summary>
    /// 셀 기준정보 관리
    /// </summary>
    public partial class O1004_QPS : UserControl, TabCloseInterface
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
        /// BaseInfo 선언
        /// </summary>
        BaseInfo BaseInfo = new BaseInfo();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public O1004_QPS()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public O1004_QPS(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                //  공통코드를 사용하지 않는 콤보박스 설정
                //this.InitComboBoxInfo();

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
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(O1004_QPS), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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
                TableView view          = source as TableView;
                view.ShowingEditor      += View_ShowingEditor;
            }
        }
        #endregion

        #region > tab1. 셀 유형 관리
        #region >> 셀 유형 관리
        /// <summary>
        /// 셀 유형 관리
        /// </summary>
        public static readonly DependencyProperty TabFirst_CellTypeMgntListProperty
            = DependencyProperty.Register("TabFirst_CellTypeMgntList", typeof(ObservableCollection<CellTypeMgt>), typeof(O1004_QPS)
                , new PropertyMetadata(new ObservableCollection<CellTypeMgt>()));

        /// <summary>
        /// 셀 유형 관리
        /// </summary>
        public ObservableCollection<CellTypeMgt> TabFirst_CellTypeMgntList
        {
            get { return (ObservableCollection<CellTypeMgt>)GetValue(TabFirst_CellTypeMgntListProperty); }
            set { SetValue(TabFirst_CellTypeMgntListProperty, value); }
        }
        #endregion

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty TabFirstGridRowCountProperty
            = DependencyProperty.Register("TabFirstGridRowCount", typeof(string), typeof(O1004_QPS), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string TabFirstGridRowCount
        {
            get { return (string)GetValue(TabFirstGridRowCountProperty); }
            set { SetValue(TabFirstGridRowCountProperty, value); }
        }
        #endregion
        #endregion

        #region > tab2. 로케이션 배치정보 관리
        #region >> 로케이션 배치정보 관리
        /// <summary>
        /// 로케이션 배치정보 관리
        /// </summary>
        public static readonly DependencyProperty TabSecond_LocBtchInfoMgtProperty
            = DependencyProperty.Register("TabSecond_LocBtchInfoMgtList", typeof(ObservableCollection<LocBtchInfoMgt>), typeof(O1004_QPS)
                , new PropertyMetadata(new ObservableCollection<LocBtchInfoMgt>()));

        /// <summary>
        /// 로케이션 배치정보 관리
        /// </summary>
        public ObservableCollection<LocBtchInfoMgt> TabSecond_LocBtchInfoMgtList
        {
            get { return (ObservableCollection<LocBtchInfoMgt>)GetValue(TabSecond_LocBtchInfoMgtProperty); }
            set { SetValue(TabSecond_LocBtchInfoMgtProperty, value); }
        }
        #endregion

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty TabSecondGridRowCountProperty
            = DependencyProperty.Register("TabSecondGridRowCount", typeof(string), typeof(O1004_QPS), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string TabSecondGridRowCount
        {
            get { return (string)GetValue(TabSecondGridRowCountProperty); }
            set { SetValue(TabSecondGridRowCountProperty, value); }
        }
        #endregion
        #endregion

        #region > tab3. SKU 셀유형 관리
        #region >> SKU 셀유형 관리
        /// <summary>
        /// SKU 셀유형 관리
        /// </summary>
        public static readonly DependencyProperty TabThird_SkuCellTypeMgtProperty
            = DependencyProperty.Register("TabThird_SkuCellTypeMgtList", typeof(ObservableCollection<SkuCellTypeMgt>), typeof(O1004_QPS)
                , new PropertyMetadata(new ObservableCollection<SkuCellTypeMgt>()));

        /// <summary>
        /// SKU 셀유형 관리
        /// </summary>
        public ObservableCollection<SkuCellTypeMgt> TabThird_SkuCellTypeMgtList
        {
            get { return (ObservableCollection<SkuCellTypeMgt>)GetValue(TabThird_SkuCellTypeMgtProperty); }
            set { SetValue(TabThird_SkuCellTypeMgtProperty, value); }
        }
        #endregion

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty TabThirdGridRowCountProperty
            = DependencyProperty.Register("TabThirdGridRowCount", typeof(string), typeof(O1004_QPS), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string TabThirdGridRowCount
        {
            get { return (string)GetValue(TabThirdGridRowCountProperty); }
            set { SetValue(TabThirdGridRowCountProperty, value); }
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
            #region tab1. 셀 유형관리
            // tab1. 콤보박스 (사용여부)
            //this.BaseClass.BindingFirstComboBox(this.cboUseYN_First, "USE_YN");

            this.BaseClass.BindingCommonComboBox(this.cboUseYN_First, "USE_YN", null, true);

            // tab1. 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd_First.ToolTip        = this.BaseClass.GetResourceValue("ROW_ADD");   //행추가
            this.btnRowDelete_First.ToolTip     = this.BaseClass.GetResourceValue("ROW_DEL");   //행삭제
            #endregion
            
            #region tab2. 로케이션 배치정보 관리
            // tab2. 콤보박스 (사용여부)
            this.BaseClass.BindingFirstComboBox(this.cboUseYN_Second, "USE_YN");
            // tab2. 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd_Second.ToolTip       = this.BaseClass.GetResourceValue("ROW_ADD");   // 행추가
            this.btnRowDelete_Second.ToolTip    = this.BaseClass.GetResourceValue("ROW_DEL");   // 행삭제
            #endregion

            #region tab3. SKU 셀 유형관리
            // tab3. 콤보박스 (사용여부)
            this.BaseClass.BindingFirstComboBox(this.cboUseYN_Third, "USE_YN");
            #endregion
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += OPT0105_Loaded;
            #endregion

            this.tabMain.SelectionChanged += TabMain_SelectionChanged;

            #region + tab1. 셀 유형 관리
            #region ++ tab1. 버튼 클릭 이벤트
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

            #region ++ tab1. Row 순번 채번 이벤트
            this.gridMaster_First.CustomUnboundColumnData += GridMaster_First_CustomUnboundColumnData;
            #endregion

            #region ++ tab1. 그리드 이벤트
            // 그리드 클릭 이벤트
            this.gridMaster_First.PreviewMouseLeftButtonUp += GridMaster_First_PreviewMouseLeftButtonUp;
            #endregion
            #endregion

            #region + tab2. 로케이션 배치정보 관리
            #region ++ tab2. 버튼 클릭 이벤트
            // 조회
            this.btnSearch_Second.PreviewMouseLeftButtonUp += BtnSearch_Second_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload_Second.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            // 저장
            this.btnSave_Second.PreviewMouseLeftButtonUp += BtnSave_Second_PreviewMouseLeftButtonUp;

            // 행 추가
            this.btnRowAdd_Second.PreviewMouseLeftButtonUp += BtnRowAdd_Second_PreviewMouseLeftButtonUp;
            // 행 삭제
            this.btnRowDelete_Second.PreviewMouseLeftButtonUp += BtnRowDelete_Second_PreviewMouseLeftButtonUp;
            #endregion

            #region ++ tab2. Row 순번 채번 이벤트
            this.gridMaster_Second.CustomUnboundColumnData += GridMaster_Second_CustomUnboundColumnData;
            #endregion

            #region ++ tab1. 그리드 이벤트
            // 그리드 클릭 이벤트
            this.gridMaster_Second.PreviewMouseLeftButtonUp += GridMaster_Second_PreviewMouseLeftButtonUp;
            #endregion
            #endregion

            #region + tab3. SKU 셀 유형관리
            #region ++ tab3. SKU 셀 유형관리
            // 조회
            this.btnSearch_Third.PreviewMouseLeftButtonUp += BtnSearch_Third_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload_Third.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            #endregion

            #region ++ tab2. Row 순번 채번 이벤트
            this.gridMaster_Third.CustomUnboundColumnData += GridMaster_Third_CustomUnboundColumnData;
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
        /// <param name="_iTabIndex">Tab Index값</param>
        private void SetResultText(int _iTabIndex)
        {
            string strResource          = string.Empty;      // 텍스트 리소스 (전체 데이터 수)
            int iTotalRowCount          = 0;                // 조회 데이터 수
            switch(_iTabIndex)
            {
                #region tab1. 셀 유형관리
                case 0:
                    strResource                     = this.BaseClass.GetResourceValue("TOT_DATA_CNT");              // 텍스트 리소스
                    iTotalRowCount                  = (this.gridMaster_First.ItemsSource as ICollection).Count;     // 전체 데이터 수
                    this.TabFirstGridRowCount       = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";        // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
                    strResource                     = this.BaseClass.GetResourceValue("DATA_INQ");                  // 건의 데이터가 조회되었습니다.
                    this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
                    break;
                #endregion

                #region tab2. 로케이션 배치정보 관리
                case 1:
                    strResource                     = this.BaseClass.GetResourceValue("TOT_DATA_CNT");              // 텍스트 리소스
                    iTotalRowCount                  = (this.gridMaster_First.ItemsSource as ICollection).Count;     // 전체 데이터 수
                    this.TabSecondGridRowCount      = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";        // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
                    strResource                     = this.BaseClass.GetResourceValue("DATA_INQ");                  // 건의 데이터가 조회되었습니다.
                    this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
                    break;
                #endregion

                #region tab3. SKU 셀 유형관리
                case 2:
                    strResource                     = this.BaseClass.GetResourceValue("TOT_DATA_CNT");              // 텍스트 리소스
                    iTotalRowCount                  = (this.gridMaster_First.ItemsSource as ICollection).Count;     // 전체 데이터 수
                    this.TabSecondGridRowCount      = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";        // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
                    strResource                     = this.BaseClass.GetResourceValue("DATA_INQ");                  // 건의 데이터가 조회되었습니다.
                    this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
                    break;
                    #endregion
            }
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
            string strSelectedName      = string.Empty;

            if (this.TabFirst_CellTypeMgntList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                // 셀 유형관리
                strSelectedName     = this.BaseClass.GetResourceValue("CELL_TYPE_MGT");
                strMessage          = this.BaseClass.GetResourceValue("ERR_EXISTS_NO_SAVE_TAB", BaseEnumClass.ResourceType.MESSAGE);
                //strMessage          = string.Format(strMessage, strSelectedName);

                bRtnValue           = false;
            }
            else if (this.TabSecond_LocBtchInfoMgtList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                strSelectedName     = this.GetLabelDesc("LOC_BTCH_INFO");
                strMessage          = this.BaseClass.GetResourceValue("ERR_EXISTS_NO_SAVE_TAB", BaseEnumClass.ResourceType.MESSAGE);
                strMessage          = string.Format(strMessage, strSelectedName);

                bRtnValue           = false;
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
                bool bRtnValue              = true;
                string strMessage           = string.Empty;
                int iCheckedCount           = 0;
                var iTabSelectedIndex       = this.tabMain.SelectedIndex;       // 현재 선택되어 있는 탭 인덱스 값

                switch (iTabSelectedIndex)
                {
                    case 0:
                        iCheckedCount   = this.TabFirst_CellTypeMgntList.Where(p => p.IsSelected == true).Count();
                        break;

                    case 1:
                        iCheckedCount   = this.TabSecond_LocBtchInfoMgtList.Where(p => p.IsSelected == true).Count();
                        break;
                }

                if (iCheckedCount == 0)
                {
                    // ERR_NO_SELECT - 선택된 데이터가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT");

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
            var iTabSelectedIndex   = this.tabMain.SelectedIndex;

            switch (iTabSelectedIndex)
            {
                #region + 셀 유형관리
                case 0:
                    this.TabFirst_CellTypeMgntList.Where(p => p.IsSelected == true && p.IsNew == true).ToList().ForEach(p =>
                    {
                        TabFirst_CellTypeMgntList.Remove(p);
                    });
                    break;
                #endregion

                #region + 로케이션 배치정보 관리
                case 1:
                    this.TabSecond_LocBtchInfoMgtList.Where(p => p.IsSelected == true && p.IsNew == true).ToList().ForEach(p =>
                    {
                        TabSecond_LocBtchInfoMgtList.Remove(p);
                    });
                    break;
                    #endregion
            }
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> tab1. 셀 유형관리
        #region + GetSP_QPS_CELL_LIST_INQ - 셀 유형관리 데이터 조회
        /// <summary>
        /// tab1. 셀 유형관리 데이터 조회
        /// </summary>
        private async Task<DataSet> GetSP_QPS_CELL_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_O1004_QPS.SP_QPS_CELL_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_CELL_LIST", "O_RTN_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                  // 센터코드
            var strCellTypeCD       = this.txtCellTypeCD_First.Text.Trim();     // 셀 타입코드
            var strCellTypeNM       = this.txtCellTypeNM_First.Text.Trim();     // 셀 타입명
            var strUseYN            = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN_First);
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_CELL_TYPE_CD",         strCellTypeCD);     // 셀 타입코드
            dicInputParam.Add("P_CELL_TYPE_NM",         strCellTypeNM);     // 셀 타입명
            dicInputParam.Add("P_USE_YN",               strUseYN);          // 사용 여부
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

        #region + SaveSP_QPS_CELL_SAVE - 셀 유형관리 데이터 저장
        /// <summary>
        /// tab1. 셀 유형관리 데이터 저장
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> SaveSP_QPS_CELL_SAVE(BaseDataAccess _da, CellTypeMgt _item)
        {
            bool isRtnValue  = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_O1004_QPS.SP_QPS_CELL_SAVE";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RTN_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;              // 센터코드
            var strEqpID            = _item.EQP_ID;                         // 설비 ID
            var strCellTypeCD       = _item.CELL_TYPE_CD;                   // 셀 타입 코드
            var strCellTypeNM       = _item.CELL_TYPE_NM;                   // 셀 타입명
            var dCellCbm            = _item.CELL_CBM;                       // 셀 CBM
            var dCellWthLen         = _item.CELL_WTH_LEN;                   // 가로
            var dCellVertLen        = _item.CELL_VERT_LEN;                  // 세로
            var dCellHgtLen         = _item.CELL_HGT_LEN;                   // 높이
            var dCellQty            = _item.CELL_QTY;                       // 셀 수
            var dMinFillRt          = _item.MIN_FILL_RT;                    // 충진율
            var strUseYN            = _item.Checked == true ? "Y" : "N";    // 사용 여부
            var strUserID           = this.BaseClass.UserID;                // 사용자 ID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_EQP_ID",               strEqpID);          // 설비 ID
            dicInputParam.Add("P_CELL_TYPE_CD",         strCellTypeCD);     // 셀 타입코드
            dicInputParam.Add("P_CELL_TYPE_NM",         strCellTypeNM);     // 셀 타입명
            dicInputParam.Add("P_CELL_CBM",             dCellCbm);          // 셀 CBM
            dicInputParam.Add("P_CELL_WTH_LEN",         dCellWthLen);       // 가로
            dicInputParam.Add("P_CELL_VERT_LEN",        dCellVertLen);      // 세로
            dicInputParam.Add("P_CELL_HGT_LEN",         dCellHgtLen);       // 높이
            dicInputParam.Add("P_CELL_QTY",             dCellQty);          // 셀 수
            dicInputParam.Add("P_MIN_FILL_RT",          dMinFillRt);        // 충진율
            dicInputParam.Add("P_USE_YN",               strUseYN);          // 사용 여부
            dicInputParam.Add("P_USER_ID",              strUserID);         // 사용자 ID
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
                        var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue  = false;
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

        #region >> tab2. 로케이션 배치정보 관리
        #region + GetSP_QPS_LOC_LIST_INQ - 로케이션 내역 조회
        /// <summary>
        /// tab1. QPS 로케이션 내역 조회
        /// </summary>
        private async Task<DataSet> GetSP_QPS_LOC_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_O1004_QPS.SP_QPS_LOC_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_LOC_LIST", "O_RTN_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                  // 센터코드
            var strLocCD            = this.txtLoc_Second.Text.Trim();           // 로케이션
            var strUseYN            = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN_Second);
            
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_LOC_CD",               strLocCD);          // 로케이션
            dicInputParam.Add("P_USE_YN",               strUseYN);          // 사용 여부
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

        #region + SaveSP_QPS_LOC_SAVE - 로케이션 배치정보 데이터 저장
        /// <summary>
        /// 로케이션 배치정보 데이터 저장
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> SaveSP_QPS_LOC_SAVE(BaseDataAccess _da, LocBtchInfoMgt _item)
        {
            bool isRtnValue  = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_O1004_QPS.SP_QPS_LOC_SAVE";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RTN_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;              // 센터코드
            var strLocCD            = _item.LOC_CD;                         // 로케이션
            var iBtchTerm           = _item.BTCH_TERM;                      // Interval
            var iBtchPickSkuCnt     = _item.BTCH_PICK_SKU_CNT;              // SKU 수
            var strUseYN            = _item.Checked == true ? "Y" : "N";    // 사용 여부
            var strUserID           = this.BaseClass.UserID;                // 사용자 ID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_LOC_CD",               strLocCD);          // 로케이션
            dicInputParam.Add("P_BTCH_TERM",            iBtchTerm);         // Interval
            dicInputParam.Add("P_BTCH_PICK_SKU_CNT",    iBtchPickSkuCnt);   // SKU 수
            dicInputParam.Add("P_USE_YN",               strUseYN);          // 사용 여부
            dicInputParam.Add("P_USER_ID",              strUserID);         // 사용자 ID
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
                        var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue  = false;
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

        #region >> tab3. SKU 셀 유형관리
        #region > GetSP_QPS_SKU_LIST_INQ - SKU 셀 유형관리 조회
        /// <summary>
        /// SKU 셀 유형관리 조회
        /// </summary>
        private async Task<DataSet> GetSP_QPS_SKU_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_O1004_QPS.SP_QPS_SKU_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_SKU_LIST", "O_RTN_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                  // 센터코드
            var strCstCD            = this.txtCstCD_Third.Text.Trim();          // 고객사 코드
            var strSkuCD            = this.txtSkuCD_Third.Text.Trim();          // SKU 코드
            var strSkuNm            = this.txtSkuNm_Third.Text.Trim();          // SKU 명
            var strUseYN            = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN_Second);
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_CST_CD",               strCstCD);          // 고객사 코드
            dicInputParam.Add("P_SKU_CD",               strSkuCD);          // SKU 코드
            dicInputParam.Add("P_SKU_NM",               strSkuNm);          // SKU 명
            dicInputParam.Add("P_USE_YN",               strUseYN);          // 사용 여부
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
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void OPT0105_Loaded(object sender, RoutedEventArgs e)
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

        #region > 공통 사용 이벤트
        #region 탭 컨트롤 선택 변경 이벤트
        /// <summary>
        /// 탭 컨트롤 선택 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabMain_SelectionChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e)
        {
            this.ToolStripChangeStatusLabelEvent(string.Empty);
        }
        #endregion

        #region >> 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        private static void View_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            if (g_IsAuthAllYN == false)
            {
                e.Cancel = true;
                return;
            }

            TableView tv = sender as TableView;
            

            if (tv.Name.Equals("tvMasterGrid_First") == true)
            {
                CellTypeMgt tabFirstDataMember = tv.Grid.CurrentItem as CellTypeMgt;

                if (tabFirstDataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    // 설비 ID, 셀 유형코드 컬럼은 수정이 되지 않도록 처리한다.
                    case "EQP_ID":
                    case "CELL_TYPE_CD":
                        if (tabFirstDataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    default: break;
                }
            }
            else if (tv.Name.Equals("tvMasterGrid_Second") == true)
            {
                LocBtchInfoMgt tabSecondDataMember = tv.Grid.CurrentItem as LocBtchInfoMgt;
                if (tabSecondDataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    case "LOC_CD":
                        if (tabSecondDataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                }
            }
            else if (tv.Name.Equals("tvMasterGrid_Third") == true)
            {

            }
        }
        #endregion

        #region >> 셀 유형 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// tab1. 셀 유형 관리 엑셀 다운로드 버튼 클릭 이벤트
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

                var iSelectedTabIndex = this.tabMain.SelectedIndex;
                List<TableView> tv = new List<TableView>();

                switch (iSelectedTabIndex)
                {
                    case 0:
                        tv.Add(this.tvMasterGrid_First);
                        break;
                    case 1:
                        tv.Add(this.tvMasterGrid_Second);
                        break;
                    case 2:
                        tv.Add(this.tvMasterGrid_Third);
                        break;
                }

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
        #endregion

        #region > tab1. 셀 유형관리
        #region >> 버튼 클릭 이벤트
        #region + 셀 유형 관리 조회버튼 클릭 이벤트
        /// <summary>
        /// tab1. 셀 유형 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                var aaa = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN_First);

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = await this.GetSP_QPS_CELL_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode          = string.Empty;     // 오류 코드
                var strErrMsg           = string.Empty;     // 오류 메세지
                var iSelectedTabIndex   = -1;               // 선택한 탭 인덱스 값

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.TabFirst_CellTypeMgntList = new ObservableCollection<CellTypeMgt>();
                    // 오라클인 경우 TableName = O_CELL_LIST
                    this.TabFirst_CellTypeMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.TabFirst_CellTypeMgntList.ToObservableCollection(null);
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                iSelectedTabIndex = this.tabMain.SelectedIndex;

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster_First.ItemsSource   = this.TabFirst_CellTypeMgntList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText(iSelectedTabIndex);
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

        #region + 셀 유형 관리 저장 버튼 클릭 이벤트
        /// <summary>
        /// tab1. 셀 유형 관리 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSave_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue     = false;

                this.TabFirst_CellTypeMgntList.ForEach(p => p.ClearError());

                // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                string strInputMessage = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.MESSAGE);

                foreach (var item in this.TabFirst_CellTypeMgntList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.EQP_ID) == true)
                        {
                            item.CellError("EQP_ID", string.Format(strInputMessage, this.GetLabelDesc("EQP_ID")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.CELL_TYPE_CD) == true)
                        {
                            item.CellError("CELL_TYPE_CD", string.Format(strInputMessage, this.GetLabelDesc("CELL_TYPE_CD")));
                            return;
                        }
                    }
                }

                var liSelectedRowData = this.TabFirst_CellTypeMgntList.Where(p => p.IsSelected == true).ToList();

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            isRtnValue = await this.SaveSP_QPS_CELL_SAVE(da, item);

                            if (isRtnValue == false) { break; }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();

                            // CMPT_SAVE - 저장 되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");

                            this.BtnSearch_First_PreviewMouseLeftButtonUp(null, null);
                        }
                        else
                        {
                            // 오류 발생하여 저장 실패한 경우 트랜잭션을 롤백처리한다.
                            da.RollbackTransaction();
                        }
                    }
                    catch
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        // ERR_SAVE - 저장 중 오류가 발생 했습니다.
                        this.BaseClass.MsgError("ERR_SAVE");
                        throw;
                    }
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
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region + 행추가 버튼 클릭 이벤트
        /// <summary>
        /// tab1. 행추가 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowAdd_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var newItem = new CellTypeMgt
            {
                EQP_ID                  = "QPS"
                ,   CELL_TYPE_CD        = string.Empty
                ,   CELL_TYPE_NM        = string.Empty
                ,   CELL_WTH_LEN        = 0
                ,   CELL_VERT_LEN       = 0
                ,   CELL_HGT_LEN        = 0
                ,   CELL_QTY            = 0
                ,   USE_YN              = "Y"
                ,   IsSelected          = true
                ,   IsNew               = true
            };

            this.TabFirst_CellTypeMgntList.Add(newItem);
            this.gridMaster_First.Focus();
            this.gridMaster_First.CurrentColumn             = this.gridMaster_First.Columns.First();
            this.gridMaster_First.View.FocusedRowHandle     = this.TabFirst_CellTypeMgntList.Count - 1;
            
            this.TabFirst_CellTypeMgntList[this.TabFirst_CellTypeMgntList.Count - 1].BackgroundBrush        = new SolidColorBrush(Colors.White);
            this.TabFirst_CellTypeMgntList[this.TabFirst_CellTypeMgntList.Count - 1].BaseBackgroundBrush    = new SolidColorBrush(Colors.White);

            this.BaseClass.SetGridRowAddFocuse(this.gridMaster_First, this.TabFirst_CellTypeMgntList.Count - 1);

        }
        #endregion

        #region + 행삭제 버튼 클릭 이벤트
        /// <summary>
        /// tab1. 행삭제 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowDelete_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 그리드 내 체크박스 선택 여부 체크
            if (this.CheckGridRowSelected() == false) { return; }

            // 행추가된 그리드 Row중 선택된 Row를 삭제한다.
            this.DeleteGridRowItem();

            this.BaseClass.SetGridRowAddFocuse(this.gridMaster_First, this.TabFirst_CellTypeMgntList.Count - 1);
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
        private void GridMaster_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var view = (sender as GridControl).View as TableView;
            var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);
            if (hi.InRowCell)
            {
                //if (hi.Column.FieldName.ToUpper().Equals("ISSELECTED"))
                //{
                //    var iIndex = this.BaseClass.GetCurrentGridControlRowIndex(this.tvMasterGrid_First);

                //    this.BaseClass.SetGridRowAddFocuse(this.gridMaster_First, iIndex);
                //}

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
        private void GridMaster_First_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            if (e.IsGetData == true)
            {
                e.Value = e.ListSourceRowIndex + 1;
            }
        }
        #endregion
        #endregion
        #endregion

        #region > tab2. 로케이션 배치정보 관리
        #region >> 버튼 클릭 이벤트
        #region + 로케이션 배치정보 관리 조회버튼 클릭 이벤트
        /// <summary>
        /// tab2. 로케이션 배치정보 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_Second_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                var strErrCode          = string.Empty;     // 오류 코드
                var strErrMsg           = string.Empty;     // 오류 메세지
                var iSelectedTabIndex   = -1;               // 선택한 탭 인덱스 값

                // 로케이션 내역 조회
                DataSet dsRtnValue = await this.GetSP_QPS_LOC_LIST_INQ();

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.TabSecond_LocBtchInfoMgtList = new ObservableCollection<LocBtchInfoMgt>();
                    // 오라클인 경우 TableName = O_CELL_LIST
                    this.TabSecond_LocBtchInfoMgtList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.TabSecond_LocBtchInfoMgtList.ToObservableCollection(null);
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                iSelectedTabIndex = this.tabMain.SelectedIndex;

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster_Second.ItemsSource   = this.TabSecond_LocBtchInfoMgtList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText(iSelectedTabIndex);
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

        #region + 로케이션 배치정보 관리 저장 버튼 클릭 이벤트
        /// <summary>
        /// tab2. 로케이션 배치정보 관리 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSave_Second_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                bool isRtnValue     = false;

                this.TabSecond_LocBtchInfoMgtList.ForEach(p => p.ClearError());

                var strMessage = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.MESSAGE);

                foreach (var item in this.TabSecond_LocBtchInfoMgtList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.LOC_CD) == true)
                        {
                            item.CellError("LOC_CD", string.Format(strMessage, this.GetLabelDesc("LOC")));
                            return;
                        }
                    }
                }

                var liSelectedRowData = this.TabSecond_LocBtchInfoMgtList.Where(p => p.IsSelected == true).ToList();

                if (liSelectedRowData.Count == 0)
                {
                    // ERR_NO_SELECT - 선택된 데이터가 없습니다.
                    this.BaseClass.MsgInfo("ERR_NO_SELECT");
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

                        // 트랜잭션 시작
                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            // 로케이션 배치정보 데이터 저장
                            isRtnValue = await this.SaveSP_QPS_LOC_SAVE(da, item);

                            if (isRtnValue == false) { break; }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우
                            da.CommitTransaction();

                            // CMPT_SAVE - 저장되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");

                            await this.GetSP_QPS_LOC_LIST_INQ();
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

                        // ERR_SAVE - 저장 중 오류가 발생했습니다.
                        this.BaseClass.MsgError("ERR_SAVE");
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
        /// <summary>
        /// tab2. 행추가 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowAdd_Second_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var newItem = new LocBtchInfoMgt
            {
                LOC_CD                  = string.Empty          // 로케이션
                ,   BTCH_TERM           = 0                     // Interval
                ,   BTCH_PICK_SKU_CNT   = 0                     // SKU 수
                ,   USE_YN              = "N"                   // 사용 여부
                ,   IsSelected          = true                  // 그리드 체크박스 선택
                ,   IsNew               = true                  // 신규 플래그
            };

            this.TabSecond_LocBtchInfoMgtList.Add(newItem);
            this.gridMaster_Second.Focus();
            this.gridMaster_Second.CurrentColumn            = this.gridMaster_Second.Columns.First();
            this.gridMaster_Second.View.FocusedRowHandle    = this.TabSecond_LocBtchInfoMgtList.Count - 1;

            this.TabSecond_LocBtchInfoMgtList[this.TabSecond_LocBtchInfoMgtList.Count - 1].BackgroundBrush        = new SolidColorBrush(Colors.White);
            this.TabSecond_LocBtchInfoMgtList[this.TabSecond_LocBtchInfoMgtList.Count - 1].BaseBackgroundBrush    = new SolidColorBrush(Colors.White);

            this.BaseClass.SetGridRowAddFocuse(this.gridMaster_Second, this.TabSecond_LocBtchInfoMgtList.Count - 1);
        }
        #endregion

        #region + 행삭제 버튼 클릭 이벤트
        /// <summary>
        /// tab2. 행삭제 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void BtnRowDelete_Second_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 그리드 내 체크박스 선택 여부 체크
            if (this.CheckGridRowSelected() == false) { return; }

            // 행추가된 그리드 Row중 선택된 Row를 삭제한다.
            this.DeleteGridRowItem();

            this.BaseClass.SetGridRowAddFocuse(this.gridMaster_Second, this.TabSecond_LocBtchInfoMgtList.Count - 1);
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
        private void GridMaster_Second_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
        private void GridMaster_Second_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
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
        #endregion
        #endregion

        #region > tab3. SKU 셀 유형관리
        #region >> 버튼 클릭 이벤트
        #region + SKU 셀 유형관리 조회버튼 클릭 이벤트
        /// <summary>
        /// tab3. SKU 셀 유형관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_Third_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                var strErrCode          = string.Empty;     // 오류 코드
                var strErrMsg           = string.Empty;     // 오류 메세지
                var iSelectedTabIndex   = -1;               // 선택한 탭 인덱스 값

                // SKU 셀 유형관리 데이터 조회
                DataSet dsRtnValue = await this.GetSP_QPS_SKU_LIST_INQ();

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.TabThird_SkuCellTypeMgtList = new ObservableCollection<SkuCellTypeMgt>();
                    // 오라클인 경우 TableName = O_CELL_LIST
                    this.TabThird_SkuCellTypeMgtList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.TabThird_SkuCellTypeMgtList.ToObservableCollection(null);
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                iSelectedTabIndex = this.tabMain.SelectedIndex;

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster_Third.ItemsSource   = this.TabThird_SkuCellTypeMgtList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText(iSelectedTabIndex);
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
        #endregion

        #region >> 그리드 관련 이벤트
        #region + 그리드 컬럼 Indicator 영역에 순번 표현 관련 이벤트
        /// <summary>
        /// 그리드 컬럼 Indicator 영역에 순번 표현 관련 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMaster_Third_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
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