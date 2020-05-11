using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Views;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.Optimization.DataMembers.OPT0107;
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
using static SMART.WCS.Common.BaseEnumClass;

namespace SMART.WCS.UI.Optimization.Views.QPS
{
    /// <summary>
    /// 최적화 오더 생성
    /// </summary>
    public partial class O1007_QPS : UserControl
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
        /// 엑셀 업로드 번호
        /// </summary>
        private string g_strUploadNo;

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        //private enum ButtonKind
        //{
        //    SEARCH      = 0,
        //    SAVE        = 1,
        //    ALL         = 99
        //};

        #region ▩ Enum  
        private enum ColumnName
        {
            CST_CD          = 0,
            WAV_NO          = 1,
            ORD_NO          = 2,
            ORD_LINE_NO     = 3,
            SHIP_TO_CD      = 4,
            SKU_CD          = 5
        };
        #endregion

        #region ▩ 생성자
        public O1007_QPS()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public O1007_QPS(List<string> _liMenuNavigation)
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

                // 공통코드를 사용하지 않는 콤보박스를 설정한다.
                this.InitComboBoxInfo();

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
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(O1007_QPS), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 최적화 오더 리스트
        public static readonly DependencyProperty OptOrdListProperty
            = DependencyProperty.Register("OptOrdList", typeof(ObservableCollection<OrderCreateMgt>), typeof(O1007_QPS)
                , new PropertyMetadata(new ObservableCollection<OrderCreateMgt>()));

        public ObservableCollection<OrderCreateMgt> OptOrdList
        {
            get { return (ObservableCollection<OrderCreateMgt>)GetValue(OptOrdListProperty); }
            set { SetValue(OptOrdListProperty, value); }
        }
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
            // 달력 컨트롤 기본값 설정
            this.deWrkPlanWmd.DateTime = DateTime.Now;

            // tab1. 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd.ToolTip      = this.BaseClass.GetResourceValue("ROW_ADD");   //행추가
            this.btnRowDelete.ToolTip   = this.BaseClass.GetResourceValue("ROW_DEL");   //행삭제
        }
        #endregion

        #region >> InitComboBoxInfo - 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// <summary>
        /// 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// </summary>
        private async void InitComboBoxInfo()
        {
            #region ++ 공통코드 사용하지 않는 콤보박스 설정
            var strWrkPlanYmd = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);       // 출고일자
            // (공통코드 사용하지 않음)
            DataTable dtComboData = await Utility.HelperClass.GetSP_OPT_QPS_DATA_SET_LIST_INQ(strWrkPlanYmd);

            // 조회 데이터가 없는 경우 구문을 리턴한다.
            if (dtComboData.Rows.Count == 0)
            {
                this.cboDataSetID.ItemsSource = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);

                return;
            }

            // 콤보박스 설정을 위해 컬럼명을 변경한다.
            dtComboData = this.BaseClass.ModifyComboBoxColumnHeaderName(dtComboData);

            // DATA 그룹 데이터가 1개 컬럼으로 조회되기 때문에 콤보박스 설정을 위해 컬럼을 추가 생성한 후 데이터를 복사한다.
            dtComboData.Columns.Add("NAME", typeof(string));

            for (int i = 0; i < dtComboData.Rows.Count; i++)
            {
                dtComboData.Rows[i]["NAME"] = dtComboData.Rows[i]["CODE"].ToString();
            }

            this.cboDataSetID.ItemsSource = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);
            if (this.BaseClass.ComboBoxItemCount(this.cboDataSetID) > 0)
            {
                this.cboDataSetID.SelectedIndex = 0;
            }
            #endregion
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 달력 컨트롤 변경 이벤트
            // 달력 컨트롤 선택 변경
            this.deWrkPlanWmd.EditValueChanged += DeWrkPlanWmd_EditValueChanged;
            #endregion

            #region + 버튼 이벤트
            // 조회 버튼 클릭 이벤트
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp; ;
            // 저장 버튼 클릭 이벤트
            this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;
            // 엑셀 업로드 버튼 클릭 이벤트
            this.btnExcelUpload.PreviewMouseLeftButtonUp += BtnExcelUpload_PreviewMouseLeftButtonUp;
            // Template 다운 버튼 클릭 이벤트
            this.btnTemplateDown.PreviewMouseLeftButtonUp += BtnTemplateDown_PreviewMouseLeftButtonUp;

            // 행추가 버튼 클릭 이벤트
            this.btnRowAdd.PreviewMouseLeftButtonUp += BtnRowAdd_PreviewMouseLeftButtonUp;
            // 행삭제 버튼 클릭 이벤트
            this.btnRowDelete.PreviewMouseLeftButtonUp += BtnRowDelete_PreviewMouseLeftButtonUp;
            #endregion
        }




        #endregion
        #endregion

        #region > 기타 함수
        #region >> SetNonTransactionButtonAttribute - 조회 버튼 Enalbed 속성 정의
        ///// <summary>
        ///// 조회 버튼 Enalbed 속성 정의
        ///// </summary>
        ///// <param name="_isEnabled">버튼 Enabled 속성</param>
        //private void SetButtonAttribute(bool _isEnabled, ButtonKind _enumButtonKind)
        //{
        //    switch (_enumButtonKind)
        //    {
        //        case ButtonKind.ALL:

        //            // 버튼 Enabled 속성을 정의한다.
        //            this.btnSearch.IsEnabled        = _isEnabled;
        //            // 조회 버튼
        //            if (_isEnabled == true) { this.btnSearch.Cursor = Cursors.Hand; }
        //            else { this.btnSearch.Cursor = Cursors.None; }

        //            if (g_IsAuthAllYN.Equals("A") == true)
        //            {
        //                this.btnSave.IsEnabled = _isEnabled;
        //                if (_isEnabled == true) { this.btnSave.Cursor = Cursors.Hand; }
        //                else { this.btnSave.Cursor = Cursors.None; }
        //            }
        //            break;

        //        case ButtonKind.SEARCH:

        //            this.btnSearch.IsEnabled        = _isEnabled;

        //            if (_isEnabled == true) { this.btnSearch.Cursor = Cursors.Hand; }
        //            else { this.btnSearch.Cursor = Cursors.None; }
        //            break;

        //        case ButtonKind.SAVE:

        //            if (g_IsAuthAllYN.Equals("A") == true)
        //            {
        //                this.btnSave.IsEnabled = _isEnabled;

        //                if (_isEnabled == true) { this.btnSave.Cursor = Cursors.Hand; }
        //                else { this.btnSave.Cursor = Cursors.None; }
        //            }
        //            break;
        //    }
        //}
        #endregion

        #region >> SetResultText - 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        private void SetResultText()
        {
            var iTotalRowCount      = (this.gridMain.ItemsSource as ICollection).Count;     // 전체 데이터 수
            var strResource         = this.BaseClass.GetResourceValue("DATA_INQ");          // 건의 데이터가 조회되었습니다.
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

            if (this.OptOrdList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
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
                int iCheckedCount           = this.OptOrdList.Where(p => p.IsSelected == true).Count();

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
            this.OptOrdList.Where(p => p.IsSelected == true && p.IsNew == true).ToList().ForEach(p =>
            {
                OptOrdList.Remove(p);
            });
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_GI_ORD_LIST_INQ - 최적화 오더 리스트 조회
        /// <summary>
        /// 최적화 오더 리스트 조회
        /// </summary>
        /// <returns></returns>
        private async Task<DataSet> GetSP_GI_ORD_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_O1007_QPS.SP_GI_ORD_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_GI_ORD_LIST", "O_RTN_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                                      // 센터코드
            var strWrkPlanYmd       = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);           // 출고일자
            var strDataSetID        = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);   // 데이터 그룹
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_WRK_PLAN_YMD",         strWrkPlanYmd);     // 출고일자
            dicInputParam.Add("P_DATA_SET_ID",          strDataSetID);      // 데이터 그룹
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

        #region >> SaveSP_GI_ORD_LIST_SAVE - 오더 정보 저장
        /// <summary>
        /// 오더 정보 저장
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveSP_GI_ORD_LIST_SAVE(BaseDataAccess _da, OrderCreateMgt _item)
        {
            bool isRtnValue  = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_O1007_QPS.SP_GI_ORD_LIST_SAVE";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RTN_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                                  // 센터코드
            var strWavNo            = _item.WAV_NO;                                             // 데이터 그룹
            var strCstCD            = _item.CST_CD;                                             // 고객사 코드
            var strOrdNo            = _item.ORD_NO;                                             // 오더 번호
            var strOrdLineNo        = _item.ORD_LINE_NO;                                        // 오더 라인번호
            var strWrkPlanYmd       = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);       // 출고일자
            var strShipToCD         = _item.SHIP_TO_CD;                                         // 거래처 코드
            var strSkuCD            = _item.SKU_CD;                                             // SKU 코드
            decimal dSkuCbm         = _item.SKU_CBM;                                            // SKU CBM
            decimal dSkuWthLen      = _item.SKU_WTH_LEN;                                        // SKU 가로
            decimal dSkuVertLen     = _item.SKU_VERT_LEN;                                       // SKU 세로
            decimal dSkuHgtLen      = _item.SKU_HGT_LEN;                                        // SKU 높이
            decimal dSkuWgt         = _item.SKU_WGT;                                            // SKU 중량
            string strLocation      = _item.LOC_CD;                                             // 위치 코드
            decimal dPlanQty        = _item.PLAN_QTY;                                           // 계획 수량
            var strUserID           = this.BaseClass.UserID;                                    // 사용자 ID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCenterCD);       // 센터코드
            dicInputParam.Add("P_WAV_NO",           strWavNo);          // 데이터 그룹
            dicInputParam.Add("P_CST_CD",           strCstCD);          // 고객사 코드
            dicInputParam.Add("P_ORD_NO",           strOrdNo);          // 오더 번호
            dicInputParam.Add("P_ORD_LINE_NO",      strOrdLineNo);      // 오더 라인번호
            dicInputParam.Add("P_WRK_PLAN_YMD",     strWrkPlanYmd);     // 출고일자
            dicInputParam.Add("P_SHIP_TO_CD",       strShipToCD);       // 거래처 코드
            dicInputParam.Add("P_SKU_CD",           strSkuCD);          // SKU 코드
            dicInputParam.Add("P_SKU_CBM",          dSkuCbm);           // SKU CBM
            dicInputParam.Add("P_SKU_WTH_LEN",      dSkuWthLen);        // SKU 가로
            dicInputParam.Add("P_SKU_VERT_LEN",     dSkuVertLen);       // SKU 세로
            dicInputParam.Add("P_SKU_HGT_LEN",      dSkuHgtLen);        // SKU 높이 
            dicInputParam.Add("P_SKU_WGT",          dSkuWgt);           // SKU 중량
            dicInputParam.Add("P_LOC_CD",           strLocation);       // 위치 코드
            dicInputParam.Add("P_PLAN_QTY",         dPlanQty);          // 계획 수량
            dicInputParam.Add("P_USER_ID",          strUserID);         // 사용자 ID
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
        #endregion

        #region ▩ 이벤트
        #region > 공통 사용 이벤트
        #region >> 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        private static void View_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            if (g_IsAuthAllYN == false)
            {
                e.Cancel = true;
                return;
            }

            TableView tv                = sender as TableView;
            OrderCreateMgt OptOrdList   = tv.Grid.CurrentItem as OrderCreateMgt;

            if (OptOrdList == null) { return; }

            switch (e.Column.FieldName)
            {
                // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                // 고객사 코드, 데이터 그룹, 오더 번호, 오더 라인번호 컬럼은 수정이 되지 않도록 처리한다.
                case "CST_CD":
                case "WAV_NO":
                case "ORD_NO":
                case "ORD_LINE_NO":
                    if (OptOrdList.IsNew == false)
                    {
                        e.Cancel                = true;
                        OptOrdList.IsSelected   = false;
                    }
                    break;
                default: break;
            }
        }
        #endregion

        #region >> 엑셀 다운로드 결과 수신 (엑셀 파일명)
        private void FrmPopup_ExcelUploadNo(string _strUploadNo)
        {
            this.g_strUploadNo = _strUploadNo;
        }
        #endregion
        #endregion

        #region > 버튼 클릭 이벤트
        #region >> 조회 버튼 클릭 이벤트
        /// <summary>
        /// 조회 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 최적화 오더 리스트를 조회한다.
                DataSet dsRtnValue = await this.GetSP_GI_ORD_LIST_INQ();

                if (dsRtnValue == null) { return; }
                var strErrCode      = string.Empty;
                var strErrMsg       = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == false)
                {
                    this.OptOrdList.ToObservableCollection(null);
                    // 오류 메세지 출력
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }
                else
                {
                    this.OptOrdList = new ObservableCollection<OrderCreateMgt>();
                    this.OptOrdList.ToObservableCollection(dsRtnValue.Tables[0]);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMain.ItemsSource = this.OptOrdList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 실행
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
        private async void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                this.OptOrdList.ForEach(p => p.ClearError());

                bool isRtnValue = true;

                #region 컬럼 입력 유효성 체크 (Validation)
                // 컬럼 데이터 유효성 체크
                this.ValidationColumnData();
                #endregion

                var liSelectedRowData = this.OptOrdList.Where(p => p.IsSelected == true).ToList();

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            isRtnValue = await this.SaveSP_GI_ORD_LIST_SAVE(da, item);

                            if (isRtnValue == false) { break; }
                        }

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();

                            // CMPT_SAVE - 저장 되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");

                            this.BtnSearch_PreviewMouseLeftButtonUp(null, null);
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

                        /// ERR_SAVE - 저장 중 오류가 발생 했습니다.
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

        #region >> ValidationColumnData - 컬럼 데이터 유효성 체크
        /// <summary>
        /// 컬럼 데이터 유효성 체크
        /// </summary>
        /// <returns></returns>
        private bool ValidationColumnData()
        {
            bool isRtnValue     = true;
            var strMessage      = this.BaseClass.GetResourceValue("ERR_NOT_INPUT_2", ResourceType.MESSAGE);

            foreach (var item in this.OptOrdList)
            {
                if (item.IsNew == true || item.IsUpdate == true)
                {
                    #region + 고객사 코드
                    if (string.IsNullOrWhiteSpace(item.CST_CD) == true)
                    {
                        item.CellError("CST_CD", string.Format(strMessage, this.GetLabelDesc("CST_CD")));
                        return false;
                    }
                    #endregion

                    #region + 데이터 그룹
                    if (string.IsNullOrWhiteSpace(item.WAV_NO) == true)
                    {
                        item.CellError("WAV_NO", string.Format(strMessage, this.GetLabelDesc("DATA_SET_ID")));
                        return false;
                    }
                    #endregion

                    #region + 오더 번호
                    if (string.IsNullOrWhiteSpace(item.ORD_NO) == true)
                    {
                        item.CellError("ORD_NO", string.Format(strMessage, this.GetLabelDesc("ORD_NO")));
                        return false;
                    }
                    #endregion

                    #region + 오더 라인번호
                    if (string.IsNullOrWhiteSpace(item.ORD_LINE_NO) == true)
                    {
                        item.CellError("ORD_LINE_NO", string.Format(strMessage, this.GetLabelDesc("ORD_LINE_NO")));
                        return false;
                    }
                    #endregion

                    #region + 거래처 코드
                    if (string.IsNullOrWhiteSpace(item.SHIP_TO_CD) == true)
                    {
                        item.CellError("SHIP_TO_CD", string.Format(strMessage, this.GetLabelDesc("SHIP_TO_CD")));
                        return false;
                    }
                    #endregion

                    #region + SKU 코드
                    if (string.IsNullOrWhiteSpace(item.SKU_CD) == true)
                    {
                        item.CellError("SKU_CD", string.Format(strMessage, this.GetLabelDesc("SKU_CD")));
                        return false;
                    }
                    #endregion
                }
            }

            return isRtnValue;
        }
        #endregion

        #region >> 엑셀 업로드 버튼 클릭 이벤트
        /// <summary>
        /// 엑셀 업로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelUpload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (SWCS201_01P frmPopup = new SWCS201_01P(ExcelUploadKind.OPT_ORD_UPLOAD))
                {
                    frmPopup.ExcelUploadNo          += FrmPopup_ExcelUploadNo;
                    frmPopup.WindowStartupLocation  = WindowStartupLocation.CenterScreen;
                    frmPopup.ShowDialog();
                }

                if (this.g_strUploadNo == null) { return; }
                 
                // 엑셀 데이터가 정상 저장된 경우만 결과 팝업을 호출한다.
                if (this.g_strUploadNo.Length > 0)
                {
                    using (O1007_QPS_01P frmResultPopup = new O1007_QPS_01P(this.g_strUploadNo))
                    {
                        frmResultPopup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        frmResultPopup.ShowDialog();
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 템플릿 다운 버튼 클릭 이벤트
        /// <summary>
        /// 템플릿 다운 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTemplateDown_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // ASK_EXCEL_DOWNLOAD - 엑셀 다운로드를 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EXCEL_DOWNLOAD");
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

        #region >> 그리드 행추가 버튼 크릭 이벤트
        /// <summary>
        /// 그리드 행추가 버튼 크릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var newItem = new OrderCreateMgt
            {
                    CST_CD          = string.Empty
                ,   WAV_NO          = string.Empty
                ,   ORD_NO          = string.Empty
                ,   ORD_LINE_NO     = string.Empty
                ,   SHIP_TO_CD      = string.Empty
                ,   SKU_CD          = string.Empty
                ,   SKU_CBM         = 0
                ,   SKU_WTH_LEN     = 0
                ,   SKU_VERT_LEN    = 0
                ,   SKU_HGT_LEN     = 0
                ,   SKU_WGT         = 0
                ,   LOC_CD          = string.Empty
                ,   PLAN_QTY        = 1
                ,   IsSelected      = true
                ,   IsNew           = true
            };

            this.OptOrdList.Add(newItem);
            this.gridMain.Focus();
            this.gridMain.CurrentColumn             = this.gridMain.Columns.First();
            this.gridMain.View.FocusedRowHandle     = this.OptOrdList.Count - 1;

            this.OptOrdList[this.OptOrdList.Count - 1].BackgroundBrush      = new SolidColorBrush(Colors.White);
            this.OptOrdList[this.OptOrdList.Count - 1].BaseBackgroundBrush  = new SolidColorBrush(Colors.White);
        }
        #endregion

        #region >> 그리드 행삭제 버튼 클릭 이벤트
        /// <summary>
        /// 그리드 행삭제 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CheckGridRowSelected() == false) { return; }
            this.DeleteGridRowItem();
        }
        #endregion
        #endregion

        #region > 달력 컨트롤 이벤트
        #region >> 달력 컨트롤 변경 이벤트
        /// <summary>
        /// 달력 컨트롤 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeWrkPlanWmd_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            this.InitComboBoxInfo();
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
