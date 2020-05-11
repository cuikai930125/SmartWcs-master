using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.COMMON.DataMembers.C1022_GAN;
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

namespace SMART.WCS.UI.COMMON.Views.GANTRY
{
    /// <summary>
    /// 공통 > Smart Gantry > Ground 셀관리
    /// </summary>
    public partial class C1022_GAN : UserControl
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

        /// <summary>
        /// Header 클릭에 따른 관련 정보 수집
        /// </summary>
        private List<string> headerSource = new List<string>();
        #endregion

        #region > 그리드 -  리스트
        public static readonly DependencyProperty CellMstListProperty
            = DependencyProperty.Register("CellMstList", typeof(ObservableCollection<CellMstInfo>), typeof(C1022_GAN)
                , new PropertyMetadata(new ObservableCollection<CellMstInfo>()));

        public ObservableCollection<CellMstInfo> CellMstList
        {
            get { return (ObservableCollection<CellMstInfo>)GetValue(CellMstListProperty); }
            set { SetValue(CellMstListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1022_GAN), new PropertyMetadata(string.Empty));

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

        #region ▩ 생성자
        public C1022_GAN()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1022_GAN(List<string> _liMenuNavigation)
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
        #endregion ▩ 생성자

        private void InitControl()
        {
            // 콤보박스 - 공통코드
            this.BaseClass.BindingCommonComboBox(this.cboCellType, "CELL_TYPE_CD", null, true);

            // 콤보박스 - 설비정보
            this.GetEqpInfoData();
        }

        private void GetEqpInfoData()
        {
            DataTable dtComboData = null;

            var strProcedureName = "PK_C1022_GAN.SP_GET_EQP_INFO";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                               // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드

            dicInputParam.Add("P_CO_CD", strCoCd);
            dicInputParam.Add("P_CNTR_CD", strCntrCd);

            using (BaseDataAccess da = new BaseDataAccess())
            {
                dtComboData = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }

            List<ComboBoxInfo> ComboBoxInfo = new List<ComboBoxInfo>();

            foreach (DataRow citem in dtComboData.Rows)
            {
                ComboBoxInfo.Add(new ComboBoxInfo { CODE = citem["EQP_ID"].ToString(), NAME = citem["EQP_NM"].ToString() });
            }

            // 바인딩 데이터가 있는 경우 첫번째 Row를 선택하도록 한다.
            if (ComboBoxInfo.Count > 0)
            {
                ComboBoxInfo.Insert(0, new ComboBoxInfo { CODE = " ", NAME = BaseClass.GetAllValueByLanguage() });  // 전체
            }

            this.cboEqp.ItemsSource = ComboBoxInfo;
            this.cboEqp.SelectedIndex = 0;
        }

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += C1022_GAN_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 팝업
            this.btnPopup.PreviewMouseLeftButtonUp += BtnPopup_PreviewMouseLeftButtonUp;
            // 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 저장
            this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;
            // 엑셀 다운로드
            this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;
            #endregion
        }

        private void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isRtnValue = false;

                var liSelectedRowData = this.CellMstList.Where(p => p.IsSelected == true).ToList();

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
                                        //PK_C1022_GAN.SP_CELL_MST_SAVE
                                       isRtnValue = this.SetSP_CELL_MST_SAVE(da, item);
                                    
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

                                // 리스트 재조회
                                CellMstListSearch();
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

        #region >> SetSP_CELL_MST_SAVE - 저장
        /// <summary>
        /// Equipment 등록
        /// </summary>
        /// <param name="da">DataAccess 객체</param>
        /// <param name="item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool SetSP_CELL_MST_SAVE(BaseDataAccess da, CellMstInfo item)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1022_GAN.SP_CELL_MST_SAVE";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUT_RESULT" };

            var strCoCd = BaseClass.CompanyCode;                    // 회사 코드
            var strCntrCd = BaseClass.CenterCD;                     // 센터 코드           
            var strEqpId = item.EQP_ID;
            var strCellId = item.CELL_ID;
            var strPosX = item.POS_X;
            var strPosY = item.POS_Y;
            var strP1AccsYn = item.P1_ACCS_YN;
            var strP2AccsYn = item.P2_ACCS_YN;
            var strCellTypeCd = item.CELL_TYPE_CD;
            var strMaxCellHgt = item.MAX_CELL_HGT;
            var strUseYN = item.USE_YN;        // 사용 여부
            var strBasicCellHgt = item.BASIC_CELL_HGT;

            var strErrCode = string.Empty;                         // 오류 코드
            var strErrMsg = string.Empty;                          // 오류 메세지
            #endregion
            
            #region Input 파라메터     
            dicInputParam.Add("P_CO_CD", strCoCd);          // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);      // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpId);     // 설비ID
            dicInputParam.Add("P_CELL_ID", strCellId);   // 셀ID
            dicInputParam.Add("P_POS_X", strPosX);        // 
            dicInputParam.Add("P_POS_Y", strPosY);        // 
            dicInputParam.Add("P_P1_ACCS_YN", strP1AccsYn);   // 
            dicInputParam.Add("P_P2_ACCS_YN", strP2AccsYn);   // 
            dicInputParam.Add("P_CELL_TYPE_CD", strCellTypeCd); // 
            dicInputParam.Add("P_MAX_CELL_HGT", strMaxCellHgt); // 셀최대높이
            dicInputParam.Add("P_USE_YN", strUseYN);        // 사용 여부
            dicInputParam.Add("P_BASIC_CELL_HGT", strBasicCellHgt); // 셀기본높이

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

        bool FirstLoad = true;
        
        private void C1022_GAN_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstLoad)
                {
                    FirstLoad = false;
                    CellMstListSearch();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void BtnPopup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var EqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);

                if (string.IsNullOrEmpty(EqpId) || EqpId.Equals(""))    // "전체"
                {
                    this.BaseClass.MsgError("설비를 선택하세요.", BaseEnumClass.CodeMessage.MESSAGE);

                    return;
                }

                C1022_GAN_01P frmChild = new C1022_GAN_01P(EqpId);
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
            CellMstListSearch();
        }

        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CellMstListSearch();
        }

        private void CellMstListSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_CELL_MST_SEARCH();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.CellMstList = new ObservableCollection<CellMstInfo>();
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.CellMstList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.CellMstList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMaster.ItemsSource = this.CellMstList;

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

        private DataSet GetSP_CELL_MST_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1022_GAN.SP_CELL_MST_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                               // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            var strEqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboEqp);                    // 시작 토트박스 번호
            var strCellTypeId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCellType);          // 셀타입
            var strCellId = string.Empty;       // 셀 ID

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);              // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);          // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpId);            // 설비 ID  
            dicInputParam.Add("P_CELL_TYPE_CD", strCellTypeId); // 셀타입 코드
            dicInputParam.Add("P_CELL_ID", strCellId);          // 셀 ID  
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }

        #region +  엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        ///  엑셀 다운로드 버튼 클릭 이벤트
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

        #endregion


        private void ProgressBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ProgressBar)
            {
                MessageBox.Show((sender as ProgressBar).Value.ToString());
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock)
            {
                MessageBox.Show((sender as TextBlock).Text + "\n" + (sender as TextBlock).Tag);
            }
        }

       

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

        #endregion > 기타 함수

        #region > 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// <summary>
        /// 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// </summary>
        private void NavigationBar_UserControlCallEvent()
        {
            this.TreeControlRefreshEvent();
        }
        #endregion
    }


}
