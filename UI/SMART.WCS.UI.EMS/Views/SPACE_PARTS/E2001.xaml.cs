using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using SMART.WCS.UI.EMS.DataMembers.ESPA001;
using SMART.WCS.UI.EMS.Views.COM;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.EMS.Views.SPACE_PARTS
{
    /// <summary>
    /// E2001.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E2001 : UserControl
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
        /// 공통 Class를 이용하기 위한 BaseClass 선언
        /// </summary>
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public E2001()
        {
            InitializeComponent();
        }

        public E2001(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();
                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 조회 조건 처리 
                //GetEmsAlarmQryDev();
                searchEnd.DateTime = DateTime.Now;
                searchStart.DateTime = searchEnd.DateTime.AddDays(-10);

                this.btnDelete.PreviewMouseLeftButtonUp += BtnDelete_PreviewMouseLeftButtonUp;

                this.tbxPartDev.IsReadOnly      = true;
                this.tbxPartDevNM.IsReadOnly    = true;

                //GetAvailableDev();

                this.EmsPartSpaList         = new ObservableCollection<EmsPartSpa>();
                this.gridMain.ItemsSource   = EmsPartSpaList;

                gridMainView.ShowingEditor += (s, e) =>
                {
                    EmsPartSpa curr = gridMain.SelectedItem as EmsPartSpa;

                    if (curr.IsNew)
                    {
                        e.Cancel = gridMain.CurrentColumn.FieldName == "PART_NM" ||
                                   gridMain.CurrentColumn.FieldName == "PART_MNFACT" ||
                                   gridMain.CurrentColumn.FieldName == "PART_MODEL";
                    }
                    else
                    {
                        e.Cancel = gridMain.CurrentColumn.FieldName == "PART_ID" ||
                                   gridMain.CurrentColumn.FieldName == "PART_NM" ||
                                   gridMain.CurrentColumn.FieldName == "PART_MNFACT" ||
                                   gridMain.CurrentColumn.FieldName == "PART_MODEL";
                    }
                };
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        private ObservableCollection<EmsPartSpa> EmsPartSpaList { get; set; }
        #endregion

        #region ▩ 함수
        #region 조회
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, MouseButtonEventArgs e)
        {
            SearchPartSpa();
            //this.btnExcelHigh.IsEnabled = true;
        }

        int focused_handle = -1;
        /// <summary>
        /// 조회 함수
        /// </summary>
        /// <param name="btn"></param>
        private void SearchPartSpa(bool btn = false)
        {
            focused_handle = -1;

            //if (!btn && null != EmsPartSpaList)
            //{
            //    focused_handle = gridMain.View.FocusedRowHandle;
            //}

            //var ChangeRowData = this.HeaderCommonCodeMgmtList.Where(p => p.IsSelected == true).ToList();

            //if (ChangeRowData.Count > 0)
            //{
            //    var strMessage = this.BaseClass.GetResourceValue("ASK_EXISTS_NO_SAVE_TO_SEARCH", BaseEnumClass.ResourceType.MESSAGE);

            //    this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

            //    if (this.BaseClass.BUTTON_CONFIRM_YN == true)
            //    {
            //        HeaderSearch();
            //    }
            //}
            if (this.EmsPartSpaList != null)
            {
                if (this.EmsPartSpaList.Count() > 0)
                {
                    if (this.EmsPartSpaList.Where(p => p.IsSelected == true && p.IsNew == true).Count() > 0)
                    {
                        this.BaseClass.MsgQuestion("ASK_EXISTS_NO_SAVE_TO_SEARCH");
                        if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }
                    }
                }
            }

            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();
                string target = "";

                if (null != tbxPartDev.Tag && !string.IsNullOrEmpty(tbxPartDev.Tag.ToString()))
                {
                    target = string.IsNullOrEmpty(tbxPartDev.Text.Trim()) ? "" : tbxPartDev.Tag.ToString();
                }

                var strStartDate        = this.BaseClass.GetCalendarValue(this.searchStart);
                var strEndDate          = this.BaseClass.GetCalendarValue(this.searchEnd);

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_PART_CLS_ID", target},
                    {"P_STOCK_QTY", "" }, //curComm.COM_DETAIL_CD},
                    {"P_FROM_DT", strStartDate},
                    {"P_TO_DT", strEndDate}
                };

                var strOutParam = new[] { "P_EMS_WH_LIST" };
                string callProc = "PK_EMS_ESPA001.SP_EMS_WH_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    if (null != EmsPartSpaList)
                    {
                        EmsPartSpaList.Clear();
                        EmsPartSpaList = null;
                    }

                    EmsPartSpaList = new ObservableCollection<EmsPartSpa>();

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        EmsPartSpaList.ToObservableCollection(outData.Tables[0]);

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    gridMain.ItemsSource = EmsPartSpaList;

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        if (-1 < focused_handle)
                        {
                            gridMain.View.FocusedRowHandle = focused_handle;
                            gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsPartSpa>)[focused_handle];
                        }
                        else
                        {
                            gridMain.View.FocusedRowHandle = EmsPartSpaList.Count - 1;
                            gridMain.SelectedItem = (gridMain.ItemsSource as ObservableCollection<EmsPartSpa>)[EmsPartSpaList.Count - 1];
                        }
                    }

                    
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 조회

        #region Popup
        /// <summary>
        /// 부품 분류 Popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPartDev_Click(object sender, MouseButtonEventArgs e)
        {
            Image target = sender as Image;

            if (null == target)
            {
                return;
            }

            ECOM001_10P frmPartClsSearch = new ECOM001_10P();
            frmPartClsSearch.ShowDialog();

            if (null != frmPartClsSearch.CurrPartCls)
            {
                tbxPartDev.Text     = frmPartClsSearch.CurrPartCls.PART_CLS_ID.Trim();
                tbxPartDevNM.Text   = frmPartClsSearch.CurrPartCls.PART_CLS_NM.Trim();
                tbxPartDev.Tag      = frmPartClsSearch.CurrPartCls.PART_CLS_ID;
            }

            frmPartClsSearch = null;

            tbxPartDev.Focus();

        }

        /// <summary>
        /// 부품 참조 Popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPartRef_Click(object sender, MouseButtonEventArgs e)
        {
            using (ECOM001_05P frmPartRef = new ECOM001_05P())
            {

                frmPartRef.ShowDialog();

                if (null != frmPartRef.CurrPartRef)
                {
                    //if (this.EmsPartSpaList.Where(p => p.IsNew == false && p.IsUpdate == false && p.PART_ID.Equals(frmPartRef.CurrPartRef.PART_ID)).Count() > 0)
                    //{
                    //    // 선택하신 부품번호는\r\n기존에 등록된 부품번호입니다.
                    //    this.BaseClass.MsgInfo("INFO_EXISTS_REG_PART_ID");
                    //    return;
                    //}

                    EmsPartSpa _item = new EmsPartSpa()
                    {
                        WH_SEQ          = 0,
                        PART_ID         = frmPartRef.CurrPartRef.PART_ID,
                        PART_NM         = frmPartRef.CurrPartRef.PART_NM,
                        PART_MNFACT     = frmPartRef.CurrPartRef.PART_MNFACT,
                        PART_MODEL      = frmPartRef.CurrPartRef.PART_MODEL,
                        WH_DT           = DateTime.Now,
                        FROM_REF        = true,
                        IsNew           = true,
                        IsSelected      = true
                    };

                    EmsPartSpaList.Add(_item);

                    gridMain.Focus();
                    gridMain.CurrentColumn          = gridMain.Columns.First();
                    gridMain.View.FocusedRowHandle  = EmsPartSpaList.Count - 1;
                    gridMain.SelectedItem           = EmsPartSpaList[EmsPartSpaList.Count - 1];
                }
            }
        }
        #endregion Popup

        #region 부품 참조
        /// <summary>
        /// 부품 참조
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowAdd_Click(object sender, MouseButtonEventArgs e)
        {
            // CHOO
            // 2020-03-18
            // 그리드 속성이 수정안됨으로 설정되어 있기 때문에 행 추가 후 입력 불가
            var strCode = "A";
            if (strCode.Equals("A") == true) { return; }

            EmsPartSpa _item = new EmsPartSpa()
            {
                WH_SEQ = 0,
                PART_ID = "",
                WH_DT = DateTime.Now,
                FROM_REF = false,
                IsNew = true,
                IsSelected = true

            };

            EmsPartSpaList.Add(_item);

            gridMain.Focus();
            gridMain.CurrentColumn = gridMain.Columns.First();
            gridMain.View.FocusedRowHandle = EmsPartSpaList.Count - 1;
            gridMain.SelectedItem = EmsPartSpaList[EmsPartSpaList.Count - 1];
        }

        /// <summary>
        /// Cell 수정시 해당하는 값 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridMainView_HiddenEditor(object sender, EditorEventArgs e)
        {
            EmsPartSpa curr = gridMain.SelectedItem as EmsPartSpa;

            if (null == curr)
            {
                return;
            }

            if (!curr.FROM_REF && "PART_ID" == e.Column.FieldName && !string.IsNullOrEmpty(curr.PART_ID))
            {
                try
                {
                    //var _USER_ID = this.BaseInfo.user_id.ToString();

                    var param = new Dictionary<string, object>
                    {
                        {"P_CENTER_CD",     this.BaseClass.CenterCD },
                        {"P_PART_NM", ""}
                    };

                    var strOutParam = new[] { "P_EMS_PART_LIST" };
                    string callProc = "PK_EMS_ECOM01_05P.SP_EMS_PART_SEARCH";

                    using (BaseDataAccess da = new BaseDataAccess())
                    {
                        var outData = da.GetSpDataSet(
                                    callProc                      // 호출 프로시저
                                ,   param                        // Input 파라메터
                                ,   strOutParam                  // Output 파라메터
                        );

                        if (outData.Tables[0].Rows.Count > 0)
                        {
                            ObservableCollection<EmsPartRef> partRefList = new ObservableCollection<EmsPartRef>();
                            partRefList.ToObservableCollection(outData.Tables[0]);

                            var target = partRefList.Where(PL => PL.PART_ID == curr.PART_ID).SingleOrDefault();

                            if (null != target)
                            {
                                curr.PART_NM = target.PART_NM;
                                curr.PART_MNFACT = target.PART_MNFACT;
                                curr.PART_MODEL = target.PART_MODEL;

                                return;
                            }
                        }

                        curr.PART_ID = "";
                        curr.PART_NM = "";
                        curr.PART_MNFACT = "";
                        curr.PART_MODEL = "";
                    }
                }
                catch (Exception ex)
                {
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }
        }
        #endregion 부품 참조

        #region 저장
        /// <summary>
        /// 저장 함수 Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            if (SavePartSpa())
            {
                this.BaseClass.MsgInfo("CMPT_SAVE");

                this.EmsPartSpaList.ForEach(p => p.CRUDReset());
                SearchPartSpa();
            }
        }

        /// <summary>
        /// 저장 함수
        /// </summary>
        /// <returns></returns>
        private bool SavePartSpa()
        {
            if (this.EmsPartSpaList.Where(p => p.IsSelected).Count() == 0)
            {
                // Msg : 선택된 데이터가 없습니다.
                this.BaseClass.MsgError("ERR_NO_SELECT");
                return false;
            }

            using (BaseDataAccess da = new BaseDataAccess())
            {
                try
                {
                    string _SUCCESS_CODE = "100";
                    var _saveItems = EmsPartSpaList.Where(f => f.IsUpdate || f.IsNew).ToList();

                    if (0 == _saveItems.Count)
                    {
                        return false;
                    }

                    var _USER_ID = this.BaseClass.UserID;

                    da.BeginTransaction();

                    foreach (var item in _saveItems)
                    {
                        var strWhDate   = item.WH_DT.ToString("yyyyMMd");

                        var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD", this.BaseClass.CenterCD },
                                { "P_WH_SEQ", item.WH_SEQ },        // 일련번호
                                { "P_PART_ID", item.PART_ID },      // ID
                                { "P_WH_QTY", item.WH_QTY },        // 입고 수량
                                { "P_PR_PRICE", item.PR_PRICE },    // 구매 가격
                                //{ "P_WH_DT", new DateTime(item.WH_DT.Year, item.WH_DT.Month, item.WH_DT.Day) },     // 입고일
                                { "P_WH_DT",    strWhDate},
                                { "P_WH_NOTE", (null == item.WH_NOTE) ? "" : item.WH_NOTE },                        // 비고
                                {"P_USER_ID", _USER_ID}
                            };

                        var strOutParam = new[] { "P_RESULT" };
                        string callProc = "PK_EMS_ESPA001.SP_EMS_WH_SAVE";

                        var outData = da.GetSpDataSet(
                                    callProc                      // 호출 프로시저
                                ,   param                        // Input 파라메터
                                ,   strOutParam                  // Output 파라메터
                        );

                        if (outData.Tables[0].Rows.Count > 0)
                        {
                            if (outData.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
                            {
                                _SUCCESS_CODE = "0";
                                da.RollbackTransaction();
                                BaseClass.MsgInfo(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                                break;
                            }
                        }
                        else
                        {
                            _SUCCESS_CODE = "0";
                            da.RollbackTransaction();
                            this.BaseClass.MsgError("ERR_INPUT_TYPE");
                            break;
                        }
                    }

                    if (_SUCCESS_CODE == "100")
                    {
                        da.CommitTransaction();
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_SAVE_DATA"));

                        return true;
                    }

                }
                catch (Exception ex)
                {
                    da.RollbackTransaction();
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }

            return false;
        }
        #endregion 저장

        #region row 삭제
        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowDelete_Click(object sender, MouseButtonEventArgs e)
        {
            // DelPartSpa();

            if (this.ValidationRowDelete() == false) { return; }

            var liHeaderCommonCodeMgmt = this.EmsPartSpaList.Where(p => p.IsSelected == true && p.IsNew == true).ToList();
            if (liHeaderCommonCodeMgmt.Count() <= 0)
            {
                BaseClass.MsgError("ERR_DELETE");
            }
            liHeaderCommonCodeMgmt.ForEach(p => EmsPartSpaList.Remove(p));
        }

        /// <summary>
        /// 삭제 함수
        /// </summary>
        private void DelPartSpa()
        {
            var _delItems = EmsPartSpaList.Where(f => f.IsChecked).ToList();

            if (0 < _delItems.Count)
            {
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    string _SUCCESS_CODE = "100";

                    try
                    {
                        var _USER_ID = this.BaseClass.UserID;

                        da.BeginTransaction();

                        foreach (var item in _delItems)
                        {
                            if (item.IsNew != true)
                            {
                                var param = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                    { "P_WH_SEQ", item.WH_SEQ },
                                    { "P_USER_ID", _USER_ID }
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_ESPA001.SP_EMS_WH_DELETE";

                                var outData = da.GetSpDataSet(
                                        callProc                      // 호출 프로시저
                                    ,   param                        // Input 파라메터
                                    ,   strOutParam                  // Output 파라메터
                                );

                                if (outData.Tables[0].Rows.Count > 0)
                                {
                                    if (outData.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
                                    {
                                        _SUCCESS_CODE = "0";
                                        da.RollbackTransaction();
                                        BaseClass.MsgInfo(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                                        break;
                                    }
                                }
                                else
                                {
                                    _SUCCESS_CODE = "0";
                                    da.RollbackTransaction();
                                    this.BaseClass.MsgError("ERR_INPUT_TYPE");
                                    break;
                                }
                            }
                        }

                        if (_SUCCESS_CODE == "100")
                        {
                            da.CommitTransaction();

                            foreach (var item in _delItems)
                            {
                                //if (item.IsNew != true)
                                EmsPartSpaList.Remove(item);
                            }

                            gridMain.RefreshData();
                            //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_DELETE"));
                        }
                    }
                    catch (Exception ex)
                    {
                        da.RollbackTransaction();
                        this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                    }
                }
            }
            else
            {
                this.BaseClass.MsgError("NO_ERROR");
            }
        }
        #endregion row 삭제
        #endregion

        #region ▩ 이벤트

        #region > CheckSelectedItem - 그리드 선택 여부 체크
        /// <summary>
        /// 그리드 선택 여부 체크
        /// </summary>
        /// <returns></returns>
        private bool ValidationDelete()
        {
            try
            {
                if (this.EmsPartSpaList.Count() == 0) { return false; }

                if (this.EmsPartSpaList.Where(p => p.IsSelected).Count() == 0)
                {
                    // Msg : 선택된 데이터가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return false;
                }

                if (this.EmsPartSpaList.Where(p => p.IsSelected == true && p.IsNew == true).Count() > 0)
                {
                    // 행추가된 데이터가 있습니다.\r\n행추가된 데이터는 삭제할 수 없습니다.
                    this.BaseClass.MsgError("ERR_EXISTS_ADD_ROW");
                    return false;
                }

                this.BaseClass.MsgQuestion("ASK_DEL");
                return this.BaseClass.BUTTON_CONFIRM_YN;
            }
            catch { throw; }
        }
        #endregion

        private bool ValidationRowDelete()
        {
            try
            {
                if (this.EmsPartSpaList.Count() == 0) { return false; }

                if (this.EmsPartSpaList.Where(p => p.IsSelected).Count() == 0)
                {
                    // Msg : 선택된 데이터가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return false;
                }

                if (this.EmsPartSpaList.Where(p => p.IsSelected == true && p.IsNew == false).Count() > 0)
                {
                    // 조회된 데이터는 행삭제를 할 수 없습니다.\r\n삭제버튼을 이용하여 삭제해주세요.
                    this.BaseClass.MsgError("ERR_EXISTS_INQ_DATA");
                    return false;
                }

                return true;
            }
            catch { throw; }
        }

        #region > DeleteSP_EMS_WH_DELETE - 데이터 삭제
        private bool DeleteSP_EMS_WH_DELETE(BaseDataAccess _da, EmsPartSpa _item)
        {
            try
            {
                bool isRtnValue     = true;

                #region + 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_EMS_ESPA001.SP_EMS_WH_DELETE";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "P_RESULT" };

                var strCenterCD         = this.BaseClass.CenterCD;                      // 센터코드
                var iWhSeq              = _item.WH_SEQ;
                var strUserID           = this.BaseClass.UserID;                        // 사용자 ID
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("P_CNTR_CD",      strCenterCD);   // 센터코드   
                dicInputParam.Add("P_WH_SEQ",       iWhSeq);        
                dicInputParam.Add("P_USER_ID",      strUserID);     // 사용자 ID
                #endregion

                #region + 데이터 삭제
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
                #endregion

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("100") == false)
                        {
                            var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                            this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                            isRtnValue = false;
                        }
                    }
                    else
                    {
                        // ERR_SAVE - 삭제 중 오류가 발생했습니다.
                        this.BaseClass.MsgError("ERR_DELETE");
                        isRtnValue = false;
                    }
                }

                return isRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > 삭제 버튼 클릭 이벤트
        private void BtnDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.ValidationDelete() == false) { return; }

                bool isRtnValue         = true;
                var liSelectedItems     = this.EmsPartSpaList.Where(p => p.IsSelected);

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    // 상태바 (아이콘) 실행
                    this.loadingScreen.IsSplashScreenShown = true;

                    da.BeginTransaction();

                    try
                    {
                        foreach (var item in liSelectedItems)
                        {
                            isRtnValue = this.DeleteSP_EMS_WH_DELETE(da, item);
                            if (isRtnValue == false) { break; }
                        }

                        if (isRtnValue == true)
                        {
                            da.CommitTransaction();

                            // Msg : 삭제되었습니다.
                            this.BaseClass.MsgInfo("CMPT_DEL");

                            this.SearchPartSpa();
                        }
                    }
                    catch { throw; }
                    finally
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = false;

                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
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

        #region btnExcelDownload_PreviewMouseLeftButtonUp - 엑셀다운로드 버튼 클릭
        /// <summary>
        /// 엑셀다운로드 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Msg : DOWN_EXCEL - 엑셀 다운로드를 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EXCEL_DOWNLOAD");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                this.gridMainExcel.ItemsSource = EmsPartSpaList;

                List<TableView> tableView = new List<TableView>();
                tableView.Add(this.gridMainExcelView);
                this.BaseClass.GetExcelDownload(tableView);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
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
