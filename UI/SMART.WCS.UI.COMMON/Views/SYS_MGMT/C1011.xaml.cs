using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1011;
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

namespace SMART.WCS.UI.COMMON.Views.SYS_MGMT
{
    /// <summary>
    /// 고객사 관리
    /// </summary>
    public partial class C1011 : UserControl
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
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public C1011()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1011(List<string> _liMenuNavigation)
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
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1011), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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
                TableView view              = source as TableView;
                view.ShowingEditor          += View_ShowingEditor;
            }
        }
        #endregion

        #region > 고객 관리
        
        public static readonly DependencyProperty CustMgntListProperty
            = DependencyProperty.Register("CustMgntList", typeof(ObservableCollection<CustMgnt>), typeof(C1011)
                , new PropertyMetadata(new ObservableCollection<CustMgnt>()));
        
        public ObservableCollection<CustMgnt> CustMgntList
        {
            get { return (ObservableCollection<CustMgnt>)GetValue(CustMgntListProperty); }
            set { SetValue(CustMgntListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1011), new PropertyMetadata(string.Empty));

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
            // 콤보박스 (사용여부)
            this.BaseClass.BindingFirstComboBox(this.cboUseYN_First, "USE_YN");

            // 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd_First.ToolTip        = this.BaseClass.GetResourceValue("ROW_ADD");   //행추가
            this.btnRowDelete_First.ToolTip     = this.BaseClass.GetResourceValue("ROW_DEL");   //행삭제
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += C1011_Loaded;
            #endregion
           
            #region ++ 버튼 클릭 이벤트
            // 조회 ////
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
            this.gridMaster.CustomUnboundColumnData += GridMaster_First_CustomUnboundColumnData;
            #endregion

            #region ++ 그리드 이벤트
            // 그리드 클릭 이벤트
            this.gridMaster.PreviewMouseLeftButtonUp += GridMaster_First_PreviewMouseLeftButtonUp;
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
     
            strResource                     = this.BaseClass.GetResourceValue("TOT_DATA_CNT");              // 텍스트 리소스
            iTotalRowCount                  = (this.gridMaster.ItemsSource as ICollection).Count;     // 전체 데이터 수
            this.TabFirstGridRowCount       = $"{strResource} : {iTotalRowCount.ToString()}";        // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource                     = this.BaseClass.GetResourceValue("DATA_INQ");                  // 건의 데이터가 조회되었습니다.
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
                bool bRtnValue              = true;
                string strMessage           = string.Empty;
                int iCheckedCount           = 0;
                iCheckedCount   = this.CustMgntList.Where(p => p.IsSelected == true).Count();
                  

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
            #region
            var DeleteRowItem = this.CustMgntList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();
            //이미 등록된 데이터를 삭제하려고 할 때 에러메시지
            if (DeleteRowItem.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }
            DeleteRowItem.ForEach(p => CustMgntList.Remove(p));

            #endregion
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region + GetSP_LIST_INQ - 데이터 조회
        /// <summary>
        /// 데이터 조회
        /// </summary>
        private async Task<DataSet> GetSP_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1011.SP_CST_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_CST_LIST", "O_RSLT" };

            //var strCstCD    = this.uCtrlCst.CodeCst.Trim();                                     // 고객사 코드
            //var strCstNM    = this.uCtrlCst.NameCst.Trim();                                     // 고객사 명
            var strCstCD = this.txtCstCD.Text.Trim();                                           // 고객사 코드
            var strCstNM = this.txtCstNM.Text.Trim();                                           // 고객사 명
            var strUseYN    = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN_First);     // 사용여부
            #endregion
            
            dicInputParam.Add("P_CST_CD", strCstCD);    // 고객사 코드
            dicInputParam.Add("P_CST_NM", strCstNM);    // 고객사 명
            dicInputParam.Add("P_USE_YN", strUseYN);    // 사용 여부
            
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            
            return dsRtnValue;
        }
        #endregion

        #region + SaveSP_SAVE  데이터 저장
        /// <summary>
        /// 데이터 저장
        /// </summary>
        /// <param name="_da">DataAccess 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> SaveSP_SAVE(BaseDataAccess _da, CustMgnt _item, bool UPD)
        {
            bool isRtnValue  = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                        = null;
            string strProcedureName = String.Empty;
            if (UPD == true)
                strProcedureName = "PK_C1011.SP_CST_UPD";
            else if (UPD == false)
                strProcedureName = "PK_C1011.SP_CST_INS";

            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RTN_RSLT" };
            
            var strCstCD            = _item.CST_CD;      
            var strCstNM            = _item.CST_NM;
            var strAddr             = _item.ADDR;
            var strZipCD            = _item.ZIP_CD;
            var strTelNO            = _item.TEL_NO;
            var strUseYN            = _item.Checked == true ? "Y" : "N";
            var strUserID           = this.BaseClass.UserID;
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CST_CD", strCstCD);
            dicInputParam.Add("P_CST_NM", strCstNM);
            dicInputParam.Add("P_ADDR", strAddr);
            dicInputParam.Add("P_ZIP_CD", strZipCD);
            dicInputParam.Add("P_TEL_NO", strTelNO);
            dicInputParam.Add("P_USE_YN",               strUseYN);
            dicInputParam.Add("P_USER_ID",              strUserID);
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
                        this.BaseClass.MsgError(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue  = false;
                    }
                }
                else
                {
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
        #region > Loaded 이벤트
        private void C1011_Loaded(object sender, RoutedEventArgs e)
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
        #region >> 버튼 클릭 이벤트
        #region >>엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 엑셀 다운로드 버튼 클릭 이벤트
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
        
        #region + 조회버튼 클릭 이벤트
        /// <summary>
        /// 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.loadingScreen.IsSplashScreenShown = true;

                this.CustMgntList.ForEach(p => p.ClearError());

                DataSet dsRtnValue = await this.GetSP_LIST_INQ();

                if (dsRtnValue == null)
                {
                    this.BaseClass.MsgInfo("INFO_NOT_INQ");
                    return;
                }

                var strErrCode          = string.Empty;     // 오류 코드
                var strErrMsg           = string.Empty;     // 오류 메세지
                var iSelectedTabIndex   = -1;               // 선택한 탭 인덱스 값

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.CustMgntList = new ObservableCollection<CustMgnt>();
                    // 오라클인 경우 TableName = O_CELL_LIST
                    this.CustMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.CustMgntList.ToObservableCollection(null);
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }
                foreach(var c in CustMgntList)
                {
                    if (c.USE_YN.Equals("Y"))
                    {
                        c.Checked = true;
                    }
                }
                
                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.CustMgntList;
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

        #region + 저장 버튼 클릭 이벤트
        /// <summary>
        /// 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSave_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //저장하시겠습니까??
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if(this.BaseClass.BUTTON_CONFIRM_YN == true)
                {//저장하는 경우
                 // 그리드 내 체크박스 선택 여부 체크
                    if (this.CheckGridRowSelected() == false) { return; }

                    bool isRtnValue = false;

                    this.CustMgntList.ForEach(p => p.ClearError());

                    var strMessage = "{0} 이(가) 입력되지 않았습니다.";

                    foreach (var item in this.CustMgntList)
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
                        }
                    }

                    var liSelectedRowData = this.CustMgntList.Where(p => p.IsSelected == true).ToList();

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
                                    isRtnValue = await this.SaveSP_SAVE(da, item, false);
                                else if (item.IsUpdate == true)
                                    isRtnValue = await this.SaveSP_SAVE(da, item, true);

                                if (isRtnValue == false) { break; }
                            }

                            if (isRtnValue == true)
                            {
                                // 저장된 경우
                                da.CommitTransaction();
                                this.BaseClass.MsgInfo("CMPT_SAVE");
                                await this.GetSP_LIST_INQ();

                                foreach (CustMgnt cs in CustMgntList)
                                {
                                    if (cs.IsNew == true)
                                        cs.IsNew = false;
                                    cs.IsSaved = true;
                                    cs.IsSelected = false;
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
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region + 행추가 버튼 클릭 이벤트
        /// <summary>
        /// 행추가 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowAdd_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var newItem = new CustMgnt
                {
                    CST_CD                  = string.Empty
                    ,   CST_NM              = string.Empty
                    ,   ADDR                = string.Empty
                    ,   ZIP_CD              = string.Empty
                    ,   TEL_NO              = string.Empty
                    ,   CRT_SPR_CD          = string.Empty
                    ,   USE_YN              = "Y"
                    ,   IsNew               = true
                    ,   IsSelected          = true
                    ,   IsSaved             = false
                };
            
                this.CustMgntList.Add(newItem);
                this.gridMaster.Focus();
                this.gridMaster.CurrentColumn             = this.gridMaster.Columns.First();
                this.gridMaster.View.FocusedRowHandle     = this.CustMgntList.Count - 1;
            
                this.CustMgntList[this.CustMgntList.Count - 1].BackgroundBrush        = new SolidColorBrush(Colors.White);
                this.CustMgntList[this.CustMgntList.Count - 1].BaseBackgroundBrush    = new SolidColorBrush(Colors.White);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
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
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                // 행추가된 그리드 Row중 선택된 Row를 삭제한다.
                this.DeleteGridRowItem();
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
        private void GridMaster_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
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
            catch (Exception err)
            {
                this.BaseClass.Error(err);
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
            try
            {
                if (g_IsAuthAllYN == false)
                {
                    e.Cancel = true;
                    return;
                }

                TableView tv = sender as TableView;
                CustMgnt dataMember = tv.Grid.CurrentItem as CustMgnt;

                if (dataMember == null) { return; }

                if (dataMember.CRT_SPR_CD.ToString().Equals("WMS"))
                {   //CRT_SPR_CD 코드가 WMS인 경우 모든 컬럼 수정 불가
                    switch (e.Column.FieldName)
                    {
                        case "CST_CD":
                        case "CST_NM":
                        case "ADDR":
                        case "ZIP_CD":
                        case "TEL_NO":
                        case "CRT_SPR_CD":
                        case "USE_YN":
                            if (dataMember.IsNew == false)
                                e.Cancel = true;
                            break;
                        default: break;
                    }
                }
                else
                {   //CRT_SPR_CD 코드가 WCS인 경우
                    switch (e.Column.FieldName)
                    {
                        // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                        // 센터코드는 수정이 되지 않도록 처리한다.
                        case "CST_CD":
                            if (dataMember.IsNew == false)
                                e.Cancel = true;
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