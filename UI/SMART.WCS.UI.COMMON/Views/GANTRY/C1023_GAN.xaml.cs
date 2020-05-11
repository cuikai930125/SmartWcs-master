using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1023_GAN;
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
using System.Windows.Threading;

namespace SMART.WCS.UI.COMMON.Views.GANTRY
{
    /// <summary>
    /// 공통 > Smart Gantry > 토트박스관리
    /// </summary>
    public partial class C1023_GAN : UserControl
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
        public C1023_GAN()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1023_GAN(List<string> _liMenuNavigation)
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
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1023_GAN), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        public static readonly DependencyProperty TotBoxTypeListProperty
            = DependencyProperty.Register("TotBoxTypeList", typeof(ObservableCollection<TotBoxInfo>), typeof(C1023_GAN)
                , new PropertyMetadata(new ObservableCollection<TotBoxInfo>()));

        public ObservableCollection<TotBoxInfo> TotBoxTypeList
        {
            get { return (ObservableCollection<TotBoxInfo>)GetValue(TotBoxTypeListProperty); }
            set { SetValue(TotBoxTypeListProperty, value); }
        }

        #region > 그리드 - 토트박스 리스트
        public static readonly DependencyProperty TotBoxMgntListProperty
            = DependencyProperty.Register("TotBoxMgntList", typeof(ObservableCollection<TotBoxMgnt>), typeof(C1023_GAN)
                , new PropertyMetadata(new ObservableCollection<TotBoxMgnt>()));

        public ObservableCollection<TotBoxMgnt> TotBoxMgntList
        {
            get { return (ObservableCollection<TotBoxMgnt>)GetValue(TotBoxMgntListProperty); }
            set { SetValue(TotBoxMgntListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1023_GAN), new PropertyMetadata(string.Empty));

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

            // 콤보박스 - 공통코드
            this.BaseClass.BindingCommonComboBox(this.cboTotBoxState, "TOT_BOX_STAT_CD", null, true);
            this.BaseClass.BindingCommonComboBox(this.cboUseYN_First, "USE_YN", null, false);

            // 콤보박스 - 토트박스 타입
            this.GetComboBoxData();

            // 버튼(행추가/행삭제) 툴팁 처리
            this.btnRowAdd.ToolTip = this.BaseClass.GetResourceValue("ROW_ADD");
            this.btnRowDelete.ToolTip = this.BaseClass.GetResourceValue("ROW_DEL");
        }

        private void GetComboBoxData()
        {
            DataTable dtComboData = null;

            var strProcedureName = "PK_C1023_GAN.SP_COMM_TOTE_TYPE_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                               // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            var strCstCd = this.BaseClass.CompanyCode;  //string.Empty;                             // 회사 코드

            dicInputParam.Add("P_CO_CD", strCoCd);
            dicInputParam.Add("P_CNTR_CD", strCntrCd);
            dicInputParam.Add("P_CST_CD", strCstCd);

            using (BaseDataAccess da = new BaseDataAccess())
            {
                dtComboData = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }

            List<ComboBoxInfo> ComboBoxInfo = new List<ComboBoxInfo>();

            foreach (DataRow citem in dtComboData.Rows)
            {
                ComboBoxInfo.Add(new ComboBoxInfo { CODE = citem["BOX_TYPE_CD"].ToString(), NAME = citem["BOX_TYPE_NM"].ToString() });                
            }

            //this.TotBoxTypeList.Add(new ComboBoxInfo { CODE = citem["BOX_TYPE_CD"].ToString(), NAME = citem["BOX_TYPE_NM"].ToString() });

            // 정상 처리된 경우
            this.TotBoxTypeList = new ObservableCollection<TotBoxInfo>();
            // 오라클인 경우 TableName = TB_COM_MENU_MST
            this.TotBoxTypeList.ToObservableCollection(dtComboData);


            // 바인딩 데이터가 있는 경우 첫번째 Row를 선택하도록 한다.
            if (ComboBoxInfo.Count > 0)
            {
                ComboBoxInfo.Insert(0, new ComboBoxInfo { CODE = " ", NAME = BaseClass.GetAllValueByLanguage() });  // 전체
            }

            this.cboTotBoxType.ItemsSource = ComboBoxInfo;
            this.cboTotBoxType.SelectedIndex = 0;
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += C1023_GAN_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 저장
            this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;
            // 일괄등록
            this.btnBtchReg.PreviewMouseLeftButtonUp += BtnBtchReg_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload_First.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
                       
            // 행 추가
            this.btnRowAdd.PreviewMouseLeftButtonUp += BtnRowAdd_PreviewMouseLeftButtonUp;
            // 행 삭제
            this.btnRowDelete.PreviewMouseLeftButtonUp += BtnRowDelete_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 이벤트
            // 그리드 클릭 이벤트
            this.gridMaster.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;

            // Equipment 리스트 그리드 순번 채번을 위한 이벤트
            this.gridMaster.CustomUnboundColumnData += GridMaster_CustomUnboundColumnData;

            this.tvMasterGrid.CellValueChanged += TvMasterGrid_CellValueChanged;

            #endregion
        }

        private void TvMasterGrid_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            this.TotBoxMgntList.ForEach(p => p.ClearError());

            if (e.Column.FieldName.Equals("BOX_TYPE_CD"))
            {
                TotBoxMgnt item = e.Row as TotBoxMgnt;

                var query = TotBoxTypeList.Where(w => w.BOX_TYPE_CD.Equals(item.BOX_TYPE_CD))
                     .Select(g => new
                     {
                         WTH = g.BOX_WTH_LEN,
                         VERT = g.BOX_VERT_LEN,
                         HGT = g.BOX_HGT_LEN
                     }).FirstOrDefault();

                (gridMaster.SelectedItem as TotBoxMgnt).BOX_WTH_LEN = query.WTH;
                (gridMaster.SelectedItem as TotBoxMgnt).BOX_VERT_LEN = query.VERT;
                (gridMaster.SelectedItem as TotBoxMgnt).BOX_HGT_LEN = query.HGT;
            }
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

                iCheckedCount = this.TotBoxMgntList.Where(p => p.IsSelected == true).Count();

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
            var liEquipmentMgnt = this.TotBoxMgntList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();

            if (liEquipmentMgnt.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }

            liEquipmentMgnt.ForEach(p => TotBoxMgntList.Remove(p));
        }

        #endregion

        #endregion

        #region > 데이터 관련

        #region >> GetSP_COMM_TOTE_SEARCH - 토트박스 리스트 조회
        /// <summary>
        /// 토트박스 리스트 데이터조회
        /// </summary>
        private DataSet GetSP_COMM_TOTE_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1023_GAN.SP_COMM_TOTE_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                               // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            var strTotBcrIdFr = this.txtTotBcrIdFr.Text.Trim();                                     // 시작 토트박스 번호
            var strTotBcrIdTo = this.txtTotBcrIdTo.Text.Trim();                                     // 종료 토트박스 번호
            var strBoxTypeCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboTotBoxType);         // 토트박스 타입
            var strTotBoxStatCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboTotBoxState);     // 토트박스 상태
            var strUseYn = this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN_First);            // 사용 여부

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                    // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                  // 센터 코드
            dicInputParam.Add("P_TOT_BCR_ID_FR", strTotBcrIdFr);        // 
            dicInputParam.Add("P_TOT_BCR_ID_TO", strTotBcrIdTo);        // 
            dicInputParam.Add("P_BOX_TYPE_CD", strBoxTypeCd);           // 토트박스 타입 코드
            dicInputParam.Add("P_TOT_BOX_STAT_CD", strTotBoxStatCd);    // 토트박스 상태 코드
            dicInputParam.Add("P_USE_YN", strUseYn);                    // 사용 여부
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

        bool FirstLoad = true;
        
        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void C1023_GAN_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstLoad)
                {
                    FirstLoad = false;
                    TotBoxListSearch();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 토트박스 리스트
        #region >> 버튼 클릭 이벤트
        #region + 토트박스 리스트 조회버튼 클릭 이벤트
        /// <summary>
        /// 토트박스 리스트 조회버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var ChangedRowData = this.TotBoxMgntList.Where(p => p.IsSelected == true).ToList();

            if (ChangedRowData.Count > 0)
            {
                var strMessage = this.BaseClass.GetResourceValue("ASK_EXISTS_NO_SAVE_TO_SEARCH", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    TotBoxListSearch();
                }
            }
            else
            {
                TotBoxListSearch();
            }
        }

        /// <summary>
        /// 토트박스 리스트 조회
        /// </summary>
        private void TotBoxListSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_COMM_TOTE_SEARCH();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.TotBoxMgntList = new ObservableCollection<TotBoxMgnt>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.TotBoxMgntList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.TotBoxMgntList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.TotBoxMgntList;

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

                if (this.TotBoxMgntList.Count == 0)
                {
                    this.BaseClass.MsgInfo("INFO_NOT_INQ");
                }
            }
        }
        #endregion

        #region + 토트박스 리스트 엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        /// 토트박스 리스트 엑셀 다운로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strMessage = this.BaseClass.GetResourceValue("ASK_EXCEL_DOWNLOAD", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    // 상태바 (아이콘) 실행
                    this.loadingScreen.IsSplashScreenShown = true;

                    List<TableView> tv = new List<TableView>();

                    tv.Add(this.tvMasterGrid);

                    this.BaseClass.GetExcelDownload(tv);
                }
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

        #region + 토트박스 리스트 저장 버튼 클릭 이벤트
        /// <summary>
        /// 토트박스 리스트 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isRtnValue = false;

                this.TotBoxMgntList.ForEach(p => p.ClearError());

                var strMessage = "{0} 이(가) 입력되지 않았습니다.";

                foreach (var item in this.TotBoxMgntList)
                {
                    if (item.IsNew || item.IsUpdate)
                    {
                        if (string.IsNullOrWhiteSpace(item.TOT_BOX_ID) == true)
                        {
                            item.CellError("TOT_BOX_ID", string.Format(strMessage, this.GetLabelDesc("TOT_BOX_NO")));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(item.BOX_TYPE_CD) == true)
                        {
                            item.CellError("BOX_TYPE_CD", string.Format(strMessage, this.GetLabelDesc("TOT_BOX_TYPE")));
                            return;
                        }

                        //if (string.IsNullOrEmpty(item.BOX_VERT_LEN) == true)
                        //{
                        //    item.CellError("BOX_VERT_LEN", string.Format(strMessage, this.GetLabelDesc("LEN")));
                        //    return;
                        //}

                        //if (string.IsNullOrEmpty(item.BOX_WTH_LEN) == true)
                        //{
                        //    item.CellError("BOX_WTH_LEN", string.Format(strMessage, this.GetLabelDesc("WID")));
                        //    return;
                        //}

                        //if (string.IsNullOrEmpty(item.BOX_HGT_LEN) == true)
                        //{
                        //    item.CellError("BOX_HGT_LEN", string.Format(strMessage, this.GetLabelDesc("HGT")));
                        //    return;
                        //}

                        //if (string.IsNullOrEmpty(item.TOT_BOX_STAT_CD) == true)
                        //{
                        //    item.CellError("TOT_BOX_STAT_CD", string.Format(strMessage, this.GetLabelDesc("STATE")));
                        //    return;
                        //}

                    }
                }

                var liSelectedRowData = this.TotBoxMgntList.Where(p => p.IsSelected == true).ToList();

                if (liSelectedRowData.Count > 0)
                {
                    this.BaseClass.MsgQuestion("ASK_CHANGE_DATA_SAVE");
                    if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                    {
                        using (BaseDataAccess da = new BaseDataAccess())
                        {
                            try
                            {
                                // 상태바 (아이콘) 실행
                                this.loadingScreen.IsSplashScreenShown = true;

                                da.BeginTransaction();

                                foreach (var item in liSelectedRowData)
                                {
                                    //if (item.IsNew == true)
                                    //{
                                    isRtnValue = this.SetSP_COMM_TOTE_SAVE(da, item);
                                    //}
                                    //else
                                    //{
                                    //    isRtnValue = this.UpdateSP_EQP_UPD(da, item);
                                    //}

                                    if (isRtnValue == false)
                                    {
                                        break;
                                    }
                                }

                                if (isRtnValue == true)
                                {
                                    // 저장된 경우
                                    da.CommitTransaction();

                                    // 상태바 (아이콘) 제거
                                    this.loadingScreen.IsSplashScreenShown = false;

                                    // 저장되었습니다.
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

                                // 토트박스 리스트 재조회
                                TotBoxListSearch();
                            }
                        }
                    }
                }
                else
                {
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                }  
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #endregion

        #region >> SetSP_COMM_TOTE_SAVE - 토트박스 정보 등록
        /// <summary>
        /// Equipment 등록
        /// </summary>
        /// <param name="da">DataAccess 객체</param>
        /// <param name="item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool SetSP_COMM_TOTE_SAVE(BaseDataAccess da, TotBoxMgnt item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1023_GAN.SP_COMM_TOTE_SAVE";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUT_RESULT" };

            var strCoCd = BaseClass.CompanyCode;                    // 회사 코드
            var strCntrCd = BaseClass.CenterCD;                     // 센터 코드           
            var strTotBcrId = item.TOT_BOX_ID;                      // 토트박스 번호
            var strBoxTypeCd = item.BOX_TYPE_CD;                    // 토트박스 타입
            var strUseYN = item.Checked == true ? "Y" : "N";        // 사용 여부
            var strUserID = this.BaseClass.UserID;                  // 사용자 ID

            var strErrCode = string.Empty;                         // 오류 코드
            var strErrMsg = string.Empty;                          // 오류 메세지
            #endregion

            #region Input 파라메터     
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);              // 센터 코드
            dicInputParam.Add("P_TOT_BCR_ID", strTotBcrId);         // 토트박스 ID
            dicInputParam.Add("P_BOX_TYPE_CD", strBoxTypeCd);       // 토트박스 타입 코드
            dicInputParam.Add("P_USE_YN", strUseYN);                // 사용 여부
            dicInputParam.Add("P_USER_ID", strUserID);              // 사용자 ID
            #endregion

            dtRtnValue = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

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

        #region + 일괄등록 버튼 클릭 이벤트
        /// <summary>
        /// 일괄등록 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBtchReg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TotBoxMgntList.Where(w => w.IsNew).Count() > 0)
            {
                this.BaseClass.MsgError("저장되지 않은 데이터가 있습니다.", BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }

            try
            {
                C1023_GAN_01P frmChild = new C1023_GAN_01P(this.TotBoxTypeList);
                frmChild.Owner = Window.GetWindow(this);
                frmChild.Closed += FrmChild_Closed;
                frmChild.ShowDialog();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void FrmChild_Closed(object sender, EventArgs e)
        {
            TotBoxListSearch();
        }
        #endregion

        #region + 행추가 버튼 클릭 이벤트
        private void BtnRowAdd_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {   
            if (TotBoxMgntList.Where(w => w.IsNew).Count() > 0)
            {
                this.BaseClass.MsgError("저장되지 않은 데이터가 있습니다.", BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }

            var newItem = new TotBoxMgnt
            {
                CO_CD = this.BaseClass.CompanyCode,
                CNTR_CD = this.BaseClass.CenterCD,
                TOT_BOX_ID = GetTotBoxFr(),
                BOX_TYPE_CD = string.Empty,
                BOX_VERT_LEN = string.Empty,
                BOX_WTH_LEN = string.Empty,
                BOX_HGT_LEN = string.Empty,
                TOT_BOX_STAT_CD = "0",
                USE_YN = "Y",
                IsSelected = true,
                IsNew = true,
                IsSaved = false
            };

            this.TotBoxMgntList.Add(newItem);
            this.gridMaster.Focus();
            this.gridMaster.CurrentColumn = this.gridMaster.Columns.First();
            this.gridMaster.View.FocusedRowHandle = this.TotBoxMgntList.Count - 1;

            //this.EquipmentMgntList[this.EquipmentMgntList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
            //this.EquipmentMgntList[this.EquipmentMgntList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);

            //var aaa = this.tvMasterGrid_First.FocusedRowHandle;
        }

        private string GetTotBoxFr()
        {
            string strTotBoxFr = string.Empty;

            try
            {
                DataTable dtRtnValue = this.GetSP_COMM_TOTE_ADD();

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        strTotBoxFr = dtRtnValue.Rows[0]["TOT_BOX_FR"].ToString();
                    }
                }
            }
            catch (Exception)
            {

            }

            return strTotBoxFr;
        }

        private DataTable GetSP_COMM_TOTE_ADD()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataTable dsRtnValue = null;
            var strProcedureName = "PK_C1023_GAN.SP_COMM_TOTE_ADD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                               // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                    // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                  // 센터 코드
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion

        #region + 행삭제 버튼 클릭 이벤트
        private void BtnRowDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TotBoxMgntList.Where(w => w.IsNew).Count() > 0)
            {
                if (this.CheckGridRowSelected() == false) { return; }

                var strMessage = this.BaseClass.GetResourceValue("ASK_ADD_ROW_DELETE", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    this.DeleteGridRowItem();
                }
            }
            else
            {
                this.DeleteGridRowItem();
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
            var view = (sender as GridControl).View as TableView;
            var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);
            if (hi.InRowCell)
            {
                if (hi.Column.FieldName.Equals("USE_YN") == false) { return; }

                if (view.ActiveEditor == null)
                {
                    view.ShowEditor();

                    if (view.ActiveEditor == null) { return; }
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
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
                TotBoxMgnt dataMember = tv.Grid.CurrentItem as TotBoxMgnt;

                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    // 토트박스ID 컬럼은 수정이 되지 않도록 처리한다.
                    case "TOT_BOX_ID":
                        //case "EQP_NM":
                        if (dataMember.IsNew == false)
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

        private void tvMasterGrid_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            //e.Cancel = !IsReadyToEdit();

            if(!this.IsReadyToEdit())
            {
                //BaseClass.MsgError("가용상태의 토트박스만 수정이 가능합니다.");
                // TODO : 체크박스 컬럼이 때때로 메세지 2번뜸. 로우 선택이랑 중복으로 의심.
                e.Cancel = true;
            }
        }

        private bool IsReadyToEdit()
        {
            var StatCd = ((TotBoxMgnt)this.gridMaster.SelectedItem).TOT_BOX_STAT_CD;
            return (StatCd == "0");
        }
    }

}
