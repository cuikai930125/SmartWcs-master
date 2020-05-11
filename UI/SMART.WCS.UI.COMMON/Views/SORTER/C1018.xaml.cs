using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Control.Views;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1018;
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
using static SMART.WCS.Common.BaseEnumClass;

namespace SMART.WCS.UI.COMMON.Views.SORTER
{
    /// <summary>
    /// C1018.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1018 : UserControl
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
        /// 엑셀 업로드 번호
        /// </summary>
        private string g_strUploadNo;

        /// <summary>
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public C1018()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1018(List<string> _liMenuNavigation)
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

                // 공통코드를 사용하지 않는 콤보박스를 설정한다.
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
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1018), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #region > 그리드 - 권역 리스트
        public static readonly DependencyProperty RegionMgmtListProperty
            = DependencyProperty.Register("RegionMgmtList", typeof(ObservableCollection<RegionMgmt>), typeof(C1018)
                , new PropertyMetadata(new ObservableCollection<RegionMgmt>()));

        private ObservableCollection<RegionMgmt> RegionMgmtList
        {
            get { return (ObservableCollection<RegionMgmt>)GetValue(RegionMgmtListProperty); }
            set { SetValue(RegionMgmtListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1018), new PropertyMetadata(string.Empty));

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
            // 콤보박스 - 조회 (사용여부)
            this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, false);
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 버튼 클릭 이벤트
            // 조회
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_First_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            // 저장 버튼 클릭 이벤트
            this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;
            // 행추가 버튼 클릭 이벤트
            this.btnRowAdd.PreviewMouseLeftButtonUp += BtnRowAdd_PreviewMouseLeftButtonUp;
            // 행삭제 버튼 클릭 이벤트
            this.btnRowDelete.PreviewMouseLeftButtonUp += BtnRowDelete_PreviewMouseLeftButtonUp;
            // 엑셀 업로드 버튼 클릭 이벤트
            this.btnExcelUpload.PreviewMouseLeftButtonUp += BtnExcelUpload_PreviewMouseLeftButtonUp;
            // 템플릿 다운로드 버튼 클릭 이벤트
            this.btnTemplateDown.PreviewMouseLeftButtonUp += BtnTemplateDown_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 이벤트
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
        /// <summary>
        /// 그리드 체크박스 선택 유효성 체크
        /// </summary>
        /// <returns></returns>
        private bool CheckGridRowSelected()
        {
            try
            {
                bool bRtnValue          = true;
                string strMessage       = string.Empty;
                int iCheckedCount       = this.RegionMgmtList.Where(p => p.IsSelected == true).Count();

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
            this.RegionMgmtList.Where(p => p.IsSelected == true && p.IsNew == true).ToList().ForEach(p =>
            {
                RegionMgmtList.Remove(p);
            });
        }
        #endregion
        #endregion

        #region > 데이터 관련

        #region >> GetSP_RGN_LIST_INQ - Region List 조회
        /// <summary>
        /// Region List 조회
        /// </summary>
        private DataSet GetSP_RGN_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1018.SP_RGN_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RGN_LIST", "O_RSLT" };
            
            var strRgnCd = this.txtRgnCd.Text.Trim();                                         // 권역 코드
            var strRgnNm = this.txtRgnNm.Text.Trim();                                         // 권역 명
            var strUseYn = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN);            // 사용 여부

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_RGN_CD", strRgnCd);               // 권역 코드
            dicInputParam.Add("P_RGN_NM", strRgnNm);               // 권역 명
            dicInputParam.Add("P_USE_YN", strUseYn);               // 사용 여부
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

        #region 권역관리 데이터 저장 - SaveRGN_LIST_SAVE
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <param name="_item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private async Task<bool> SaveRGN_LIST_SAVE(BaseDataAccess _da, RegionMgmt _item)
        {
            try
            {
                bool isRtnValue             = true;

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_C1018.SP_RGN_LIST_SAVE";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam = { "O_RSLT" };

                var strRgnCD            = _item.RGN_CD;                         // 권역코드
                var strRgnNm            = _item.RGN_NM;                         // 권역명
                var strDlvCoCD          = _item.DLV_CO_CD;                      // 택배사 코드
                var strDlvCoNm          = _item.DLV_CO_NM;                      // 택배사명
                var strDlvCoRgnCD       = _item.DLV_CO_RGN_CD;                  // 택배사 지역코드
                var strDlvCoRgnNm       = _item.DLV_CO_RGN_NM;                  // 택배사 지역명
                var strUseYN            = _item.Checked == true ? "Y" : "N";    // 사용 여부
                var strUserID           = this.BaseClass.UserID;                 // 사용자 ID
                #endregion

                #region Input 파라메터     
                dicInputParam.Add("P_RGN_CD",           strRgnCD);          // 권역코드
                dicInputParam.Add("P_RGN_NM",           strRgnNm);          // 권역명
                dicInputParam.Add("P_DLV_CO_CD",        strDlvCoCD);        // 택배사 코드
                dicInputParam.Add("P_DLV_CO_NM",        strDlvCoNm);        // 택배사명
                dicInputParam.Add("P_DLV_CO_RGN_CD",    strDlvCoRgnCD);     // 택배사 지역코드
                dicInputParam.Add("P_DLV_CO_RGN_NM",    strDlvCoRgnNm);     // 택배사 지역명
                dicInputParam.Add("P_USE_YN",           strUseYN);          // 사용 여부
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
                            isRtnValue = false;
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
            catch { throw; }
        }
        #endregion
        #endregion

        #endregion

        #region ▩ 이벤트
        #region > 공통 사용 이벤트
        #region >> 엑셀 다운로드 결과 수신 (엑셀 파일명)
        private void FrmPopup_ExcelUploadNo(string _strUploadNo)
        {
            this.g_strUploadNo = _strUploadNo;
        }
        #endregion
        #endregion

        #region > 권역 관리

        #region >> 버튼 클릭 이벤트

        #region + 권역 관리 조회버튼 클릭 이벤트
        /// <summary>
        /// 권역 관리 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_First_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_RGN_LIST_INQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.RegionMgmtList = new ObservableCollection<RegionMgmt>();
                    // 오라클인 경우 TableName = TB_COM_RGN_MST
                    this.RegionMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.RegionMgmtList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.RegionMgmtList;

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

        #region + 권역 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 슈트 관리 엑셀 다운로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 조회된 데이터가 없는 경우
                if (this.gridMaster.VisibleRowCount == 0)
                {
                    this.BaseClass.MsgError("INFO_NOT_INQ");
                    return;
                }

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

        #region + 권역 관리 저장 버튼 클릭 이벤트
        /// <summary>
        /// 권역 관리 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isRtnValue = false;

                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                this.RegionMgmtList.ForEach(p => p.ClearError());

                // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                string strInputMessage = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.MESSAGE);

                foreach (var item in this.RegionMgmtList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.RGN_CD) == true)
                        {
                            item.CellError("RGN_CD", string.Format(strInputMessage, this.BaseClass.GetResourceValue("RGN_CD")));
                            return;
                        }
                    }
                }

                // ASK_SAVE - 저장하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                var liSelectedRowData = this.RegionMgmtList.Where(p => p.IsSelected == true).ToList();

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        da.BeginTransaction();

                        foreach (var item in liSelectedRowData)
                        {
                            isRtnValue = await this.SaveRGN_LIST_SAVE(da, item);

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
        /// 행추가 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var newItem = new RegionMgmt
                {
                    RGN_CD                  = string.Empty          // 권역코드
                    ,   RGN_NM              = string.Empty          // 권역명
                    ,   DLV_CO_CD           = string.Empty          // 택배사 코드
                    ,   DLV_CO_NM           = string.Empty          // 택배사 명
                    ,   DLV_CO_RGN_CD       = string.Empty          // 택배사 지역코드
                    ,   DLV_CO_RGN_NM       = string.Empty          // 택배사 지역명
                    ,   USE_YN              = "Y"                   // 사용여부
                    ,   IsSelected          = true
                    ,   IsNew               = true
                };

                this.RegionMgmtList.Add(newItem);
                this.gridMaster.Focus();
                this.gridMaster.CurrentColumn               = this.gridMaster.Columns.First();
                this.gridMaster.View.FocusedRowHandle       = this.RegionMgmtList.Count - 1;
            
                this.RegionMgmtList[this.RegionMgmtList.Count - 1].BackgroundBrush        = new SolidColorBrush(Colors.White);
                this.RegionMgmtList[this.RegionMgmtList.Count - 1].BaseBackgroundBrush    = new SolidColorBrush(Colors.White);

                this.BaseClass.SetGridRowAddFocuse(this.gridMaster, this.RegionMgmtList.Count - 1);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region + 행삭제 버튼 클릭 이벤트
        /// <summary>
        /// 행삭제 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRowDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 그리드 내 체크박스 선택 여부 체크
                if (this.CheckGridRowSelected() == false) { return; }

                // 행추가된 그리드 Row중 선택된 Row를 삭제한다.
                this.DeleteGridRowItem();

                this.BaseClass.SetGridRowAddFocuse(this.gridMaster, this.RegionMgmtList.Count - 1);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region + 엑셀 업로드 버튼 클릭 이벤트
        /// <summary>
        /// 엑셀 업로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelUpload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (SWCS201_01P frmPopup = new SWCS201_01P(ExcelUploadKind.RGN_MGT_UPLOAD))
                {
                    frmPopup.ExcelUploadNo          += FrmPopup_ExcelUploadNo;
                    frmPopup.WindowStartupLocation  = WindowStartupLocation.CenterScreen;
                    frmPopup.ShowDialog();
                }

                if (this.g_strUploadNo == null) { return; }
                if (this.g_strUploadNo.Length > 0)
                {
                    // 엑셀 데이터가 정상 저장된 경우만 결과 팝업을 호출한다.
                    if (this.g_strUploadNo.Length > 0)
                    {
                        using (C1018_01P frmResultPopup = new C1018_01P(this.g_strUploadNo))
                        {
                            frmResultPopup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            frmResultPopup.ShowDialog();
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

        #region + 템플릿 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 템플릿 다운로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTemplateDown_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // ASK_EXCEL_DOWNLOAD - 엑셀 다운로드를 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_TEMPLATE_DOWNLOAD");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                List<TableView> tv = new List<TableView>();
                tv.Add(this.tvTemplageGrid);

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

        #region >> 그리드 관련 이벤트

        #region + 그리드 클릭 이벤트
        private void GridMaster_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var view    = (sender as GridControl).View as TableView;
                var hi      = view.CalcHitInfo(e.OriginalSource as DependencyObject);

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

        #region + 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        private static void View_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            if (g_IsAuthAllYN == false)
            {
                e.Cancel = true;
                return;
            }

            TableView tv = sender as TableView;

            RegionMgmt rMgmt = tv.Grid.CurrentItem as RegionMgmt;

            switch (e.Column.FieldName)
            {
                case "RGN_CD":
                    if (rMgmt.IsNew == false)
                    {
                        e.Cancel = true;
                    }
                    break;

                case "REG_DT":
                case "REG_ID":
                case "UPD_DT":
                case "UPD_ID":
                    e.Cancel = true;
                    break;
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
