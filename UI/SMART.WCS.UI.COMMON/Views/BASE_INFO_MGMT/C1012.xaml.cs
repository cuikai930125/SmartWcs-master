using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1012;
using System;
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

namespace SMART.WCS.UI.Common.Views.BASE_INFO_MGMT
{
    public partial class C1012 : UserControl, TabCloseInterface
    {
        #region ▩ Detegate 선언
        #region > 즐겨찾기 변경후 메인화면 트리 컨트롤 Refresh 및 포커스 이동
        public delegate void TreeControlRefreshEventHandler();
        public event TreeControlRefreshEventHandler TreeControlRefreshEvent;
        #endregion
        #endregion

        #region ▩ 전역변수

        #region > BaseClass 생성
        BaseClass BaseClass = new BaseClass();
        #endregion

        #region > 로직을 위한 변수 및 플래그
        string currentSkuCD = string.Empty;
        string currentCrtSprCD = string.Empty;
        bool isSearched = false;
        bool isGridClicked = false;
        #endregion

        #endregion

        #region ▩ 생성자

        #region > C1012생성자
        public C1012()
        {
            InitializeComponent();
        }

        public C1012(List<string> _liMenuNavigation)
        {
            InitializeComponent();
            try
            {
                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 상단에 설명 
                this.CommentArea.ScreenID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 이벤트 초기화
                this.InitEvent();

                // 컨트롤 초기화
                this.InitControl();
                TableView view = this.tvMasterGrid_Lower as TableView;
                view.ShowingEditor += View_ShowingEditor;
            }
            catch { throw; }
        }
        #endregion

        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의

        #region > Sku 정보 리스트
        public static readonly DependencyProperty SkuInfoMgmtListProperty
            = DependencyProperty.Register("SkuInfoMgmtList", typeof(ObservableCollection<SkuInfoMgmt>), typeof(C1012)
                , new PropertyMetadata(new ObservableCollection<SkuInfoMgmt>()));

        public ObservableCollection<SkuInfoMgmt> SkuInfoMgmtList
        {
            get { return (ObservableCollection<SkuInfoMgmt>)GetValue(SkuInfoMgmtListProperty); }
            set { SetValue(SkuInfoMgmtListProperty, value); }
        }
        #endregion

        #region > Sku Bcr 정보 리스트
        public static readonly DependencyProperty SkuBcrInfoMgmtListProperty
            = DependencyProperty.Register("SkuBcrInfoMgmtList", typeof(ObservableCollection<SkuBcrInfoMgmt>), typeof(C1012)
                , new PropertyMetadata(new ObservableCollection<SkuBcrInfoMgmt>()));

        public ObservableCollection<SkuBcrInfoMgmt> SkuBcrInfoMgmtList
        {
            get { return (ObservableCollection<SkuBcrInfoMgmt>)GetValue(SkuBcrInfoMgmtListProperty); }
            set { SetValue(SkuBcrInfoMgmtListProperty, value); }
        }

        #endregion

        #region > IsEnabled 정의
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1012), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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

        #endregion

        #region ▩ 함수

        #region > 초기화

        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.

        private void InitControl()
        {
            #region + 콤보박스 컨트롤
            // 콤보박스
            this.BaseClass.BindingFirstComboBox(this.cboUseYN, "USE_YN");
            // 콤보박스 default index 설정
            this.cboUseYN.SelectedIndex = 0;
            #endregion

            #region + 버튼 컨트롤
            // 버튼(행추가/행삭제) 툴팁 처리
            //   this.btnaddnotice.tooltip = this.baseclass.getresourcevalue("row_add");   //행추가
            //   this.btndeletenotice.tooltip = this.baseclass.getresourcevalue("row_del");   //행삭제
            #endregion
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화

        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += C1012_Loaded;
            #endregion

            #region + 버튼 이벤트
            // 조회버튼 이벤트
            this.btnSEARCH.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 저장버튼 이벤트
            this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;
            // Sku 정보 행추가 이벤트
            this.btnRowAdd_SkuInfo.PreviewMouseLeftButtonUp += BtnRowAdd_SkuInfo_PreviewMouseLeftButtonUp;
            // Sku 정보 행삭제 이벤트
            this.btnRowDelete_SkuInfo.PreviewMouseLeftButtonUp += BtnRowDelete_SkuInfo_PreviewMouseLeftButtonUp;
            // Sku Bcr 정보 행추가 이벤트
            this.btnRowAdd_SkuBcrInfo.PreviewMouseLeftButtonUp += BtnRowAdd_SkuBcrInfo_PreviewMouseLeftButtonUp;
            // Sku Bcr 정보 행삭제 이벤트
            this.btnRowDelete_SkuBcrInfo.PreviewMouseLeftButtonUp += BtnRowDelete_SkuBcrInfo_PreviewMouseLeftButtonUp;
            #endregion

            #region + 그리드 이벤트
            this.skuListGrid.PreviewMouseLeftButtonUp += SkuListGrid_PreviewMouseLeftButtonUp;

            this.skuBcrListGrid.PreviewMouseLeftButtonUp += SkuBcrListGrid_PreviewMouseLeftButtonUp;
            #endregion
        }

        #endregion

        #region > 기타 함수
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
                string strMessage = string.Empty;
                int iCheckedCount = 0;
                iCheckedCount = this.SkuInfoMgmtList.Where(p => p.IsSelected == true).Count();
                iCheckedCount += this.SkuBcrInfoMgmtList.Where(p => p.IsSelected == true).Count();
                if (iCheckedCount == 0)
                {
                    this.BaseClass.MsgInfo("선택된 데이터가 없습니다.");  // ERR_NO_SELECT : 선택된 데이터가 없습니다.
                    bRtnValue = false;
                }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> DeleteGridRow_item - 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// <summary>
        /// 선택한 그리드의 Row를 삭제한다. (행추가된 항목만 삭제 가능)
        /// </summary>
        private void DeleteSkuInfoGridRow_item()
        {
            #region
            var DeleteRow_item = this.SkuInfoMgmtList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();
            //이미 등록된 데이터를 삭제하려고 할 때 에러메시지
            if (DeleteRow_item.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }
            DeleteRow_item.ForEach(p => SkuInfoMgmtList.Remove(p));
            #endregion
        }

        private void DeleteSkuBcrInfoGridRow_item()
        {
            #region
            var DeleteRow_item = this.SkuBcrInfoMgmtList.Where(p => p.IsSelected == true && p.IsNew == true && p.IsSaved == false).ToList();
            //이미 등록된 데이터를 삭제하려고 할 때 에러메시지
            if (DeleteRow_item.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }
            DeleteRow_item.ForEach(p => SkuBcrInfoMgmtList.Remove(p));
            #endregion
        }
        #endregion

        #region >> ChangeSkuListCellStatus
        private void ChangeSkuListCellStatus()
        {
            // 시스템 코드가 WMS이면 모두 수정 불가
            if (currentCrtSprCD.Equals("WMS"))
            {
                // SKU_CD
                this.SKU_NM_REQ.ReadOnly = true;
                this.SKU_WGT.ReadOnly = true;
                this.SKU_WITH_LEN.ReadOnly = true;
                this.SKU_VERT_LEN.ReadOnly = true;
                this.SKU_HGT_LEN.ReadOnly = true;
                this.SKU_CBM.ReadOnly = true;
                this.SKU_CLS.ReadOnly = true;
                this.SKU_TMPT_TYPE.ReadOnly = true;
                this.BOX_INT_QTY.ReadOnly = true;
                this.BOX_WGT.ReadOnly = true;
                this.BOX_WTH_LEN.ReadOnly = true;
                this.BOX_VERT_LEN.ReadOnly = true;
                this.BOX_HGT_LEN.ReadOnly = true;
                this.BOX_CBM.ReadOnly = true;
                this.WGT_UOM.ReadOnly = true;
                this.LEN_UOM.ReadOnly = true;
                this.CBM_UOM.ReadOnly = true;
                this.CELL_TYPE_CD.ReadOnly = true;
                this.CRT_SPR_CD.ReadOnly = true;
                this.USE_YN.ReadOnly = false;
            }
            else if (currentCrtSprCD.Equals("WCS"))
            {
                // SKU_CD
                this.SKU_NM_REQ.ReadOnly = false;
                this.SKU_WGT.ReadOnly = false;
                this.SKU_WITH_LEN.ReadOnly = false;
                this.SKU_VERT_LEN.ReadOnly = false;
                this.SKU_HGT_LEN.ReadOnly = false;
                this.SKU_CBM.ReadOnly = false;
                this.SKU_CLS.ReadOnly = false;
                this.SKU_TMPT_TYPE.ReadOnly = false;
                this.BOX_INT_QTY.ReadOnly = false;
                this.BOX_WGT.ReadOnly = false;
                this.BOX_WTH_LEN.ReadOnly = false;
                this.BOX_VERT_LEN.ReadOnly = false;
                this.BOX_HGT_LEN.ReadOnly = false;
                this.BOX_CBM.ReadOnly = false;
                this.WGT_UOM.ReadOnly = false;
                this.LEN_UOM.ReadOnly = false;
                this.CBM_UOM.ReadOnly = false;
                this.CELL_TYPE_CD.ReadOnly = false;
                this.CRT_SPR_CD.ReadOnly = false;
                this.USE_YN.ReadOnly = false;
            }
            return;
        }

        #endregion

        #region >> ChangeSkuBcrListCellStatus
        private void ChangeSkuBcrListCellStatus(bool boolean)
        {
            if (boolean == true)
            {
                this.SKU_CD_REQ2.ReadOnly = true;
                this.BCR_NO_REQ.ReadOnly = true;
                this.BCR_SPR_CD.ReadOnly = true;
                this.CRT_SPR_CD2.ReadOnly = true;
                this.USE_YN2.ReadOnly = true;
            }
            else if (boolean == false)
            {
                this.SKU_CD_REQ2.ReadOnly = false;
                this.BCR_NO_REQ.ReadOnly = false;
                this.BCR_SPR_CD.ReadOnly = false;
                this.CRT_SPR_CD2.ReadOnly = false;
                this.USE_YN2.ReadOnly = false;
            }
        }

        #endregion

        #region >> CheckModifyData 
        private bool CheckModifyData()
        {
            bool bRtnValue = true;
            if (this.SkuInfoMgmtList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
            {
                // 셀 유형관리
                bRtnValue = false;
            }
            else if (this.SkuBcrInfoMgmtList.Any(p => p.IsNew == true || p.IsDelete == true || p.IsUpdate == true))
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

        #region >> TabClosing()
        public bool TabClosing()
        {
            return this.CheckModifyData();
        }
        #endregion

        #endregion

        #endregion

        #region > 데이터 관련
        #region >> GetSP_SKU_LIST_INQ - SKU 정보 데이터 조회
        private async Task<DataSet> GetSP_SKU_LIST_INQ()
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1012.SP_SKU_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_SKU_LIST", "O_RSLT" };

            //var strCustomerCD = this.BaseClass.cst
            var strCustomerCD = this.uCtrlCst.CodeCst.Trim();
            var strSkuCD = this.txtSkuCD.Text;
            var strSkuNM = this.txtSkuNM.Text;
            var strBcrNO = this.txtBcrNO.Text;

            string strUseYN = null;
            if (this.cboUseYN.SelectedIndex == 0)
            {
                strUseYN = "Y";
            }
            else if (this.cboUseYN.SelectedIndex == 1)
            {
                strUseYN = "N";
            }
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CST_CD", strCustomerCD);         // CST_CD
            dicInputParam.Add("P_SKU_CD", strSkuCD);              // SKU_CD
            dicInputParam.Add("P_SKU_NM", strSkuNM);              // SKU_NM
            dicInputParam.Add("P_BCR_NO", strBcrNO);              // BCR_NO
            dicInputParam.Add("P_USE_YN", strUseYN);              // USE_YN
            #endregion

            #region + 데이터 조회
            using (BaseDataAccess _da = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = _da.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion
            return dsRtnValue;
        }
        #endregion

        #region >> InsertSP_SKU_INS - SKU 정보 입력
        private async Task<bool> InsertSP_SKU_INS(SkuInfoMgmt _item, BaseDataAccess _da)
        {
            #region + 파라미터 변수 선언 및 값 할당
            bool isRtnValue = true;

            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1012.SP_SKU_INS";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strCSTCD = this.uCtrlCst.CodeCst.Trim();                        // CST_CD
            var strSKUCD = _item.SKU_CD;                                         // SKU_CD
            var strSKUNM = _item.SKU_NM;                                         // SKU_NM        
            var boxInQTY = _item.BOX_IN_QTY;                                     // BOX_IN_QTY
            var skuWGT = _item.SKU_WGT;                                          // SKU_WGT
            var boxWGT = _item.BOX_WGT;                                          // BOX_WGT
            var wgtUOM = _item.WGT_UOM;                                          // WGT_UOM
            var lenUOM = _item.LEN_UOM;                                          // LEN_UOM
            var cbmUOM = _item.CBM_UOM;                                          // CBM_UOM
            var skuCLS = _item.SKU_CLS;                                          // SKU_CLS
            var skuWthLEN = _item.SKU_WTH_LEN;                                   // SKU_WTH_LEN
            var skuVertLEN = _item.SKU_VERT_LEN;                                 // SKU_VERT_LEN
            var skuHgtLEN = _item.SKU_HGT_LEN;                                   // SKU_HGT_LEN
            var skuCBM = _item.SKU_CBM;                                          // SKU_CBM    
            var boxWthLEN = _item.BOX_WTH_LEN;                                   // BOX_WTH_LEN
            var boxVertLEN = _item.BOX_VERT_LEN;                                 // BOX_VERT_LEN
            var boxHgtLEN = _item.BOX_HGT_LEN;                                   // BOX_HGT_LEN
            var boxCBM = _item.BOX_CBM;                                          // BOX_CBM
            var skuTmptTypeCD = _item.SKU_TMPT_TYPE_CD;                          // SKU_TMPT_TYPE_CD
            var cellTypeCD = _item.CELL_TYPE_CD;                                 // CELL_TYPE_CD
            var useYN = _item.USE_YN;                                            // USE_YN
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CST_CD", strCSTCD);
            dicInputParam.Add("P_SKU_CD", strSKUCD);
            dicInputParam.Add("P_SKU_NM", strSKUNM);
            dicInputParam.Add("P_BOX_IN_QTY", boxInQTY);
            dicInputParam.Add("P_SKU_WGT", skuWGT);
            dicInputParam.Add("P_BOX_WGT", boxWGT);
            dicInputParam.Add("P_WGT_UOM", wgtUOM);
            dicInputParam.Add("P_LEN_UOM", lenUOM);
            dicInputParam.Add("P_CBM_UOM", cbmUOM);
            dicInputParam.Add("P_SKU_CLS", skuCLS);
            dicInputParam.Add("P_SKU_WTH_LEN", skuWthLEN);
            dicInputParam.Add("P_SKU_VERT_LEN", skuVertLEN);
            dicInputParam.Add("P_SKU_HGT_LEN", skuHgtLEN);
            dicInputParam.Add("P_SKU_CBM", skuCBM);
            dicInputParam.Add("P_BOX_WTH_LEN", boxWthLEN);
            dicInputParam.Add("P_BOX_VERT_LEN", boxVertLEN);
            dicInputParam.Add("P_BOX_HGT_LEN", boxHgtLEN);
            dicInputParam.Add("P_BOX_CBM", boxCBM);
            dicInputParam.Add("P_SKU_TMPT_TYPE_CD", skuTmptTypeCD);
            dicInputParam.Add("P_CELL_TYPE_CD", cellTypeCD);
            dicInputParam.Add("P_USE_YN", useYN);
            dicInputParam.Add("P_USER_ID", this.BaseClass.UserID);

            #endregion

            #region + 데이터 저장
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
                        isRtnValue = false;
                    }
                }
                else
                {
                    this.BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }
            #endregion
            return isRtnValue;
        }
        #endregion

        #region >> UpdateSP_SKU_UPD - SKU 정보 수정
        private async Task<bool> UpdateSP_SKU_UPD(SkuInfoMgmt _item, BaseDataAccess _da)
        {
            #region + 파라미터 변수 선언 및 값 할당
            bool isRtnValue = true;

            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1012.SP_SKU_UPD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var strCSTCD = this.uCtrlCst.CodeCst.Trim();                        // CST_CD
            var strSKUCD = _item.SKU_CD;                                         // SKU_CD
            var strSKUNM = _item.SKU_NM;                                         // SKU_NM        
            var boxInQTY = _item.BOX_IN_QTY;                                     // BOX_IN_QTY
            var skuWGT = _item.SKU_WGT;                                          // SKU_WGT
            var boxWGT = _item.BOX_WGT;                                          // BOX_WGT
            var wgtUOM = _item.WGT_UOM;                                          // WGT_UOM
            var lenUOM = _item.LEN_UOM;                                          // LEN_UOM
            var cbmUOM = _item.CBM_UOM;                                          // CBM_UOM
            var skuCLS = _item.SKU_CLS;                                          // SKU_CLS
            var skuWthLEN = _item.SKU_WTH_LEN;                                   // SKU_WTH_LEN
            var skuVertLEN = _item.SKU_VERT_LEN;                                 // SKU_VERT_LEN
            var skuHgtLEN = _item.SKU_HGT_LEN;                                   // SKU_HGT_LEN
            var skuCBM = _item.SKU_CBM;                                          // SKU_CBM    
            var boxWthLEN = _item.BOX_WTH_LEN;                                   // BOX_WTH_LEN
            var boxVertLEN = _item.BOX_VERT_LEN;                                 // BOX_VERT_LEN
            var boxHgtLEN = _item.BOX_HGT_LEN;                                   // BOX_HGT_LEN
            var boxCBM = _item.BOX_CBM;                                          // BOX_CBM
            var skuTmptTypeCD = _item.SKU_TMPT_TYPE_CD;                          // SKU_TMPT_TYPE_CD
            var cellTypeCD = _item.CELL_TYPE_CD;                                 // CELL_TYPE_CD
            var useYN = _item.USE_YN;                                            // USE_YN
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CST_CD", strCSTCD);
            dicInputParam.Add("P_SKU_CD", strSKUCD);
            dicInputParam.Add("P_SKU_NM", strSKUNM);
            dicInputParam.Add("P_BOX_IN_QTY", boxInQTY);
            dicInputParam.Add("P_SKU_WGT", skuWGT);
            dicInputParam.Add("P_BOX_WGT", boxWGT);
            dicInputParam.Add("P_WGT_UOM", wgtUOM);
            dicInputParam.Add("P_LEN_UOM", lenUOM);
            dicInputParam.Add("P_CBM_UOM", cbmUOM);
            dicInputParam.Add("P_SKU_CLS", skuCLS);
            dicInputParam.Add("P_SKU_WTH_LEN", skuWthLEN);
            dicInputParam.Add("P_SKU_VERT_LEN", skuVertLEN);
            dicInputParam.Add("P_SKU_HGT_LEN", skuHgtLEN);
            dicInputParam.Add("P_SKU_CBM", skuCBM);
            dicInputParam.Add("P_BOX_WTH_LEN", boxWthLEN);
            dicInputParam.Add("P_BOX_VERT_LEN", boxVertLEN);
            dicInputParam.Add("P_BOX_HGT_LEN", boxHgtLEN);
            dicInputParam.Add("P_BOX_CBM", boxCBM);
            dicInputParam.Add("P_SKU_TMPT_TYPE_CD", skuTmptTypeCD);
            dicInputParam.Add("P_CELL_TYPE_CD", cellTypeCD);
            dicInputParam.Add("P_USE_YN", useYN);
            dicInputParam.Add("P_USER_ID", this.BaseClass.UserID);

            #endregion

            #region + 데이터 저장
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
                        isRtnValue = false;
                    }
                }
                else
                {
                    this.BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }
            #endregion
            return isRtnValue;
        }

        #endregion

        #region  >> GetSP_SKU_BCR_LIST_INQ - BCR 정보 데이터 조회
        private async Task<DataSet> GetSP_SKU_BCR_LIST_INQ(string cst_cd, string sku_cd)
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1012.SP_SKU_BCR_LIST_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_SKU_BCR_LIST", "O_RSLT" };

            string strCustomerCD = cst_cd;      // CST_CD
            string strSkuCD = sku_cd;           // SKU_CD
            #endregion

            #region + Input 파라메터
            //dicInputParam.Add("P_CST_CD", strCustomerCD);             // CST_CD
            //dicInputParam.Add("P_SKU_CD", strSkuCD);                  // SKU_CD

            dicInputParam.Add("P_CST_CD", "LGCNS");
            dicInputParam.Add("P_SKU_CD", strSkuCD);
            #endregion

            #region + 데이터 조회
            using (BaseDataAccess _da = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = _da.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion
            return dsRtnValue;
        }
        #endregion

        #region >> InsertSP_SKU_BCR_INS - SKU BCR 정보 입력
        private async Task<bool> InsertSP_SKU_BCR_INS(SkuBcrInfoMgmt _item, BaseDataAccess _da)
        {
            #region + 파라미터 변수 선언 및 값 할당
            bool isRtnValue = true;
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1012.SP_SKU_BCR_INS";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var skuCD = _item.SKU_CD;          // SKU_CD
            var bcrNO = _item.BCR_NO;          // BCR_NO
            var bcrSprCD = _item.BCR_SPR_CD;      // BCR_SPR_CD
            var crtSprCD = _item.CRT_SPR_CD;      // CRT_SPR_cD
            var useYN = _item.USE_YN;          // USE_YN
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CST_CD", this.uCtrlCst.CodeCst.Trim());
            dicInputParam.Add("P_SKU_CD", skuCD);
            dicInputParam.Add("P_BCR_NO", bcrNO);
            dicInputParam.Add("P_BCR_SPR_CD", bcrSprCD);
            dicInputParam.Add("P_USE_YN", useYN);
            dicInputParam.Add("P_USER_ID", BaseClass.UserID);
            #endregion

            #region + 데이터 저장
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
                        isRtnValue = false;
                    }
                }
                else
                {
                    this.BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }
            #endregion

            return isRtnValue;
        }
        #endregion

        #region >> UpdateSP_SKU_BCR_UPD - SKU BCR 정보 수정
        private async Task<bool> UpdateSP_SKU_BCR_UPD(SkuBcrInfoMgmt _item, BaseDataAccess _da)
        {
            #region + 파라미터 변수 선언 및 값 할당
            bool isRtnValue = true;
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1012.SP_SKU_BCR_UPD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_RSLT" };

            var skuCD = _item.SKU_CD;          // SKU_CD
            var bcrNO = _item.BCR_NO;          // BCR_NO
            var bcrSprCD = _item.BCR_SPR_CD;      // BCR_SPR_CD
            var crtSprCD = _item.CRT_SPR_CD;      // CRT_SPR_cD
            var useYN = _item.USE_YN;          // USE_YN
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CST_CD", this.uCtrlCst.CodeCst.Trim());
            dicInputParam.Add("P_SKU_CD", skuCD);
            dicInputParam.Add("P_BCR_NO", bcrNO);
            dicInputParam.Add("P_BCR_SPR_CD", bcrSprCD);
            dicInputParam.Add("P_USE_YN", useYN);
            dicInputParam.Add("P_USER_ID", BaseClass.UserID);
            #endregion

            #region + 데이터 저장
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
                        isRtnValue = false;
                    }
                }
                else
                {
                    this.BaseClass.MsgError("ERR_SAVE");
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
        private void C1012_Loaded(object sender, RoutedEventArgs e)
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

        #region > 버튼 클릭 이벤트

        #region >> SKU 정보 조회버튼 클릭 이벤트
        private async void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.uCtrlCst.CodeCst == string.Empty)
            {
                this.BaseClass.MsgError("고객사를 먼저 선택해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
            }
            else
            {
                try
                {
                    // 플래그 전환
                    isSearched = true;

                    #region + 상태바 실행
                    this.loadingScreen.IsSplashScreenShown = true;
                    #endregion

                    DataSet dsRtnValue = null;

                    #region + 공지사항 목록 조회
                    dsRtnValue = await this.GetSP_SKU_LIST_INQ();

                    if (dsRtnValue == null) { return; }

                    var strErrCode = string.Empty;     // 오류 코드
                    var strErrMsg = string.Empty;      // 오류 메세지

                    if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                    {
                        // 정상 처리된 경우
                        this.SkuInfoMgmtList = new ObservableCollection<SkuInfoMgmt>();
                        this.SkuInfoMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
                    }
                    else
                    {
                        // 오류가 발생한 경우;
                        this.SkuInfoMgmtList = null;
                        this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                    }
                    #endregion

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

        }
        #endregion

        #region >> SKU 정보 저장버튼 클릭 이벤트
        private async void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            #region + 그리드 내 체크박스 선택 여부 체크 및 필수 값 확인
            if (this.CheckGridRowSelected() == false)
            {
                return;
            }
            bool isRtnValue = false;
            foreach (var _item in this.SkuInfoMgmtList)
            {
                _item.ClearError();
            }
            foreach (var _item in this.SkuBcrInfoMgmtList)
            {
                _item.ClearError();
            }

            var strMessage = "{0} 이(가) 입력되지 않았습니다.";

            foreach (var _item in this.SkuInfoMgmtList)
            {
                if (_item.IsNew || _item.IsUpdate)
                {
                    if (string.IsNullOrWhiteSpace(_item.SKU_CD) == true)
                    {
                        _item.CellError("SKU_CD", string.Format(strMessage, this.GetLabelDesc("SKU_CD")));
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(_item.SKU_NM) == true)
                    {
                        _item.CellError("SKU_NM", string.Format(strMessage, this.GetLabelDesc("SKU_NM")));
                        return;
                    }
                }
            }
            foreach (var _item in this.SkuBcrInfoMgmtList)
            {
                if (_item.IsNew || _item.IsUpdate)
                {
                    if (string.IsNullOrWhiteSpace(_item.SKU_CD) == true)
                    {
                        _item.CellError("SKU_CD", string.Format(strMessage, this.GetLabelDesc("SKU_CD")));
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(_item.BCR_NO) == true)
                    {
                        _item.CellError("BCR_NO", string.Format(strMessage, this.GetLabelDesc("BCR_NO")));
                        return;
                    }
                }
            }
            var liSelectedSkuInfoRowData = this.SkuInfoMgmtList.Where(p => p.IsSelected == true).ToList();
            var liSelectedSkuBcrInfoRowData = this.SkuBcrInfoMgmtList.Where(p => p.IsSelected == true).ToList();
            #endregion

            this.BaseClass.MsgQuestion("ASK_CHANGE_DATA_SAVE");

            #region + 저장 및 수정 실행 트랜잭션
            if (this.BaseClass.BUTTON_CONFIRM_YN == true)
            {
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        dataAccess.BeginTransaction();
                        foreach (var _item in liSelectedSkuInfoRowData)
                        {
                            if (_item.IsNew == true)
                            {
                                isRtnValue = await this.InsertSP_SKU_INS(_item, dataAccess);
                            }
                            else if (_item.IsUpdate == true)
                            {
                                isRtnValue = await this.UpdateSP_SKU_UPD(_item, dataAccess);
                            }
                            if (isRtnValue == false)
                            {
                                break;
                            }
                        }

                        foreach (var _item in liSelectedSkuBcrInfoRowData)
                        {
                            if (_item.IsNew == true)
                            {
                                isRtnValue = await this.InsertSP_SKU_BCR_INS(_item, dataAccess);
                            }
                            else if (_item.IsUpdate == true)
                            {
                                isRtnValue = await this.UpdateSP_SKU_BCR_UPD(_item, dataAccess);
                            }
                            if (isRtnValue == false)
                            {
                                break;
                            }
                        }
                        //foreach(var _item )
                        if (isRtnValue == true)
                        {
                            // 저장된 경우
                            dataAccess.CommitTransaction();
                            this.BaseClass.MsgInfo("CMPT_SAVE");
                            await this.GetSP_SKU_LIST_INQ();
                        }
                        else if (isRtnValue == false)
                        {
                            // 오류 발생하여 저장 실패한 경우
                            dataAccess.RollbackTransaction();
                        }
                    }
                    catch
                    {
                        if (dataAccess.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            dataAccess.RollbackTransaction();
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
            #endregion
        }
        #endregion

        #region >> SKU 정보 행추가 버튼 클릭 이벤트
        private void BtnRowAdd_SkuInfo_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.uCtrlCst.CodeCst == string.Empty)
            {
                this.BaseClass.MsgError("고객사를 먼저 선택해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
            }
            else if (isSearched == false)
            {
                this.BaseClass.MsgError("SKU리스트를 먼저 조회해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
            }
            else
            {
                SkuInfoMgmt new_item = new SkuInfoMgmt
                {
                    SKU_CD = string.Empty
                   ,
                    SKU_NM = string.Empty
                   ,
                    SKU_WTH_LEN = 0
                   ,
                    SKU_VERT_LEN = 0
                   ,
                    SKU_HGT_LEN = 0
                   ,
                    SKU_CBM = 0
                   ,
                    SKU_CLS = string.Empty
                   ,
                    SKU_TMPT_TYPE_CD = string.Empty
                   ,
                    BOX_IN_QTY = 0
                   ,
                    BOX_WGT = 0
                   ,
                    BOX_WTH_LEN = 0
                   ,
                    BOX_VERT_LEN = 0
                   ,
                    BOX_CBM = 0
                   ,
                    WGT_UOM = string.Empty
                   ,
                    LEN_UOM = string.Empty
                   ,
                    CBM_UOM = string.Empty
                   ,
                    CELL_TYPE_CD = string.Empty
                   ,
                    CRT_SPR_CD = string.Empty
                   ,
                    USE_YN = "Y"
                   ,
                    IsSelected = true
                   ,
                    IsNew = true
                   ,
                    Checked = true
                };

                this.SkuInfoMgmtList.Add(new_item);
                this.skuListGrid.Focus();
                this.skuListGrid.CurrentColumn = this.skuListGrid.Columns.First();
                this.skuListGrid.View.FocusedRowHandle = this.SkuInfoMgmtList.Count - 1;

                this.SkuInfoMgmtList[this.SkuInfoMgmtList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
                this.SkuInfoMgmtList[this.SkuInfoMgmtList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);
            }
        }
        #endregion

        #region >> SKU 정보 행삭제 버튼 클릭 이벤트
        private void BtnRowDelete_SkuInfo_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 그리드 내 체크박스 선택 여부 체크
            if (this.CheckGridRowSelected() == false) { return; }

            // 행추가된 그리드 Row중 선택된 Row를 삭제한다.
            this.DeleteSkuInfoGridRow_item();
        }
        #endregion

        #region >> SKU BCR 행추가 버튼 클릭 이벤트LG
        private void BtnRowAdd_SkuBcrInfo_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.uCtrlCst.CodeCst == string.Empty)
            {
                this.BaseClass.MsgError("고객사를 선택해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
            }
            else if (isGridClicked == false)
            {
                this.BaseClass.MsgError("특정 SKU를 먼저 선택해주세요.", BaseEnumClass.CodeMessage.MESSAGE);
            }
            else
            {
                // WMS인지 WCS인지 확인
                if (currentCrtSprCD.Equals("WMS"))
                {
                    BaseClass.MsgInfo("시스템코드가 WMS인 경우 BCR정보를 변경할 수 없습니다.", BaseEnumClass.CodeMessage.MESSAGE);
                    return;
                }

                SkuBcrInfoMgmt new_item = new SkuBcrInfoMgmt
                {
                    SKU_CD = currentSkuCD
                   ,
                    BCR_NO = string.Empty
                   ,
                    BCR_SPR_CD = string.Empty
                   ,
                    CRT_SPR_CD = string.Empty
                   ,
                    USE_YN = "Y"
                   ,
                    IsSelected = true
                   ,
                    IsNew = true
                   ,
                    Checked = true
                };

                // SKU 코드는 변하지 않는다.
                this.SKU_CD_REQ2.ReadOnly = true;

                this.SkuBcrInfoMgmtList.Add(new_item);
                this.skuBcrListGrid.Focus();
                this.skuBcrListGrid.CurrentColumn = this.skuListGrid.Columns.First();
                this.skuBcrListGrid.View.FocusedRowHandle = this.SkuInfoMgmtList.Count - 1;

                this.SkuBcrInfoMgmtList[this.SkuBcrInfoMgmtList.Count - 1].BackgroundBrush = new SolidColorBrush(Colors.White);
                this.SkuBcrInfoMgmtList[this.SkuBcrInfoMgmtList.Count - 1].BaseBackgroundBrush = new SolidColorBrush(Colors.White);
            }
        }

        #endregion

        #region >> SKU BCR 행삭제 버튼 클릭 이벤트
        private void BtnRowDelete_SkuBcrInfo_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // WMS인지 WCS인지 확인
            if (currentCrtSprCD.Equals("WMS"))
            {
                BaseClass.MsgInfo("시스템코드가 WMS인 경우 BCR정보를 변경할 수 없습니다.", BaseEnumClass.CodeMessage.MESSAGE);
                return;
            }

            // 그리드 내 체크박스 선택 여부 체크
            if (this.CheckGridRowSelected() == false) { return; }

            // 행추가된 그리드 Row중 선택된 Row를 삭제한다.
            this.DeleteSkuBcrInfoGridRow_item();
        }
        #endregion

        #endregion

        #region > 그리드 관련 이벤트

        #region >> 그리드 클릭 이벤트
        private async void SkuListGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var view = (sender as GridControl).View as TableView;
            var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);
            if (hi.InRowCell)
            {
                #region + 클릭한 행 정보 가져오기 
                int nbSkuInfoSEQ = hi.RowHandle;
                var obj = this.skuListGrid.GetCellValue(nbSkuInfoSEQ, skuListGrid.Columns[1]);
                var obj2 = this.skuListGrid.GetCellValue(nbSkuInfoSEQ, skuListGrid.Columns[20]);
                string cst_cd = this.uCtrlCst.CodeCst.Trim();                        // CST_CD
                currentSkuCD = Convert.ToString(obj);                                // SKU_CD
                currentCrtSprCD = Convert.ToString(obj2);                            // CRT_SPR_CD
                #endregion

                // 플래그 전환
                isGridClicked = true;

                #region + 행정보에 따른 셀 잠금
                ChangeSkuListCellStatus();
                #endregion

                #region + SKU_BCR 정보 조회
                try
                {
                    DataSet dsRtnValue = await this.GetSP_SKU_BCR_LIST_INQ(cst_cd, currentSkuCD);

                    if (dsRtnValue == null) { return; }

                    var strErrCode = string.Empty;     // 오류 코드
                    var strErrMsg = string.Empty;      // 오류 메세지

                    if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                    {
                        // 정상 처리된 경우
                        this.SkuBcrInfoMgmtList = new ObservableCollection<SkuBcrInfoMgmt>();
                        this.SkuBcrInfoMgmtList.ToObservableCollection(dsRtnValue.Tables[0]);
                    }
                    else
                    {
                        // 오류가 발생한 경우
                        this.SkuBcrInfoMgmtList = null;
                        this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                    }

                    #region + 그리드 바인딩
                    this.skuBcrListGrid.ItemsSource = this.SkuBcrInfoMgmtList;
                    #endregion
                }
                catch (Exception err)
                {
                    this.BaseClass.Error(err);
                }
                #endregion
            }
        }

        private void SkuBcrListGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var view = (sender as GridControl).View as TableView;
            var hi = view.CalcHitInfo(e.OriginalSource as DependencyObject);

            if (hi.InRowCell)
            {
                #region + 클릭한 행 정보 가져오기 
                int nbSkuInfoSEQ = hi.RowHandle;
                var obj = this.skuListGrid.GetCellValue(nbSkuInfoSEQ, skuListGrid.Columns[4]);

                string crtSprCD = Convert.ToString(obj);
                if (crtSprCD.Equals("WMS"))
                {
                    ChangeSkuBcrListCellStatus(true);
                }
                else if (crtSprCD.Equals("WCS"))
                {
                    ChangeSkuBcrListCellStatus(false);
                }
                #endregion
            }
        }
        #endregion

        #region + 그리드 내 필수값 컬럼 Editing 여부 처리 (해당 이벤트를 사용하는 경우 Xaml단 TableView 테그내 isEnabled 속성을 정의해야 한다.)
        private static void View_ShowingEditor(object sender, ShowingEditorEventArgs e)
        {
            TableView tv = sender as TableView;

            if (tv.Name.Equals("tvMasterGrid_First"))
            {
                SkuInfoMgmt dataMember = tv.Grid.CurrentItem as SkuInfoMgmt;

                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    // 센터코드, DB 접속 정보 컬럼은 수정이 되지 않도록 처리한다.
                    case "CRT_SPR_CD":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    case "SKU_CD":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    default: break;
                }
            }
            else
            {
                SkuBcrInfoMgmt dataMember = tv.Grid.CurrentItem as SkuBcrInfoMgmt;

                if (dataMember == null) { return; }

                switch (e.Column.FieldName)
                {
                    // 컬럼이 행추가 상태 (신규 Row 추가)가 아닌 경우
                    // 센터코드, DB 접속 정보 컬럼은 수정이 되지 않도록 처리한다.
                    case "BCR_NO":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    case "SKU_CD":
                        if (dataMember.IsNew == false)
                        {
                            e.Cancel = true;
                        }
                        break;
                    case "CRT_SPR_CD":
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
