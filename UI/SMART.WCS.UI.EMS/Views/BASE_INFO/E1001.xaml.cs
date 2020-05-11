using DevExpress.Mvvm.Native;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control.Modules.Interface;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.EMS.DataMembers.COM;
using SMART.WCS.UI.EMS.DataMembers.E1001;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.EMS.Views.BASE_INFO
{
    /// <summary>
    /// E1001.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E1001 : UserControl, TabCloseInterface
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
        public E1001()
        {
            InitializeComponent();
        }

        public E1001(List<string> _liMenuNavigation)
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

                this.BaseClass.BindingCommonComboBox(this.cboUseYN, "USE_YN", null, true);

                // 2020-03-02 | 컬럼 Editing 속성이 지정되어 있지 않아서 추가 | 추성호
                for (int i = 0; i < this.gridMain.Columns.Count; i++)
                {
                    this.gridMain.Columns[i].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                }

                // 신규 작성시에만 ID 편집 가능
                //
                gridMainView.ShowingEditor += (s, e) =>
                {
                    EmsEqpMngr curr = gridMain.SelectedItem as EmsEqpMngr;

                    e.Cancel = gridMain.CurrentColumn.FieldName == "EQP_MNGR_ID" && !curr.IsNew;
                };

                this.EmsEqpMngrList         = new ObservableCollection<EmsEqpMngr>();
                this.gridMain.ItemsSource   = this.EmsEqpMngrList;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        public ObservableCollection<EmsCommCode> EmsEmplDevList { get; private set; }

        public static readonly DependencyProperty EmsEqpMngrListProperty
                                  = DependencyProperty.Register("EmsEqpMngrList",
                                      typeof(ObservableCollection<EmsEqpMngr>),
                                             typeof(E1001), new PropertyMetadata(new ObservableCollection<EmsEqpMngr>()));
        /// <summary>
        /// Grid Data
        /// </summary>
        public ObservableCollection<EmsEqpMngr> EmsEqpMngrList
        {
            get { return (ObservableCollection<EmsEqpMngr>)GetValue(EmsEqpMngrListProperty); }
            private set { SetValue(EmsEqpMngrListProperty, value); }
        }

        #region > Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(E1001), new PropertyMetadata(string.Empty));

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

        #region ▩ 함수
        #region >> SetResultText - 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        /// <param name="_iTabIndex">Tab Index값</param>
        private void SetResultText()
        {
            var strResource                 = this.BaseClass.GetResourceValue("TOT_DATA_CNT");              // 텍스트 리소스
            var iTotalRowCount              = (this.gridMain.ItemsSource as ICollection).Count;    // 전체 데이터 수
            this.GridRowCount               = $"{strResource} : {iTotalRowCount.ToString("#,##0")}";        // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource                     = this.BaseClass.GetResourceValue("DATA_INQ");                  // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        #region 조회
        private void btnSearchClick(object sender, MouseButtonEventArgs e)
        {
            if (this.EmsEqpMngrList.Where(p => p.IsNew || p.IsUpdate).Count() > 0)
            {
                // 저장되지 않은 데이터가 있습니다.|계속 조회하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_EXISTS_NO_SAVE_TO_SEARCH");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }
            }

            SearchEmsEqpMngr(true);
        }

        int focused_handle = -1;
        private void SearchEmsEqpMngr(bool btn = false)
        {
            focused_handle = -1;

            if (!btn && null != EmsEqpMngrList)
            {
                focused_handle = gridMain.View.FocusedRowHandle;
            }

            try
            {
                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD", this.BaseClass.CenterCD},
                    {"P_USE_YN", this.BaseClass.ComboBoxSelectedKeyValue(this.cboUseYN) }
                };

                var strOutParam = new[] { "P_EMS_EQP_MNGR_LIST" };
                string callProc = "PK_EMS_EBSE001.SP_EMS_EQP_MNGR_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                            callProc                    // 호출 프로시저
                        , param                         // Input 파라메터
                        , strOutParam                   // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        EmsEqpMngrList = new ObservableCollection<EmsEqpMngr>();
                        EmsEqpMngrList.ToObservableCollection(outData.Tables[0]);

                        if (EmsEqpMngrList.Count() > 0)
                        {
                            this.gridMain.ItemsSource = EmsEqpMngrList;
                        }
                        else
                        {
                            this.gridMain.ItemsSource = null;
                        }
                    }
                    else
                    {
                        this.ToolStripChangeStatusLabelEvent(this.BaseClass.GetResourceValue("INFO_NOT_INQ", BaseEnumClass.ResourceType.MESSAGE));
                    }

                    this.SetResultText();
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 조회

        #region 신규 row 추가
        /// <summary>
        /// 추가 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowAddClick(object sender, MouseButtonEventArgs e)
        {
            if (null == EmsEqpMngrList)
            {
                EmsEqpMngrList = new ObservableCollection<EmsEqpMngr>();
            }

            NewEmsEqpMngr();
        }

        /// <summary>
        /// 추가 함수
        /// </summary>
        private void NewEmsEqpMngr()
        {
            EmsEqpMngr _item = new EmsEqpMngr()
            {
                CENTER_CD   = this.BaseClass.CenterCD,
                EQP_MNGR_ID = "",
                MNGR_NM = "",
                MNGR_TEL_NO = "",
                EMPL_DEV_CD = "DEV01",
                VNDR_NM = "",
                USE_YN = "Y",
                RSTR_ID = this.BaseClass.UserID,
                UPDR_ID = this.BaseClass.UserID,
                IsNew = true,
                IsSelected = true
            };

            EmsEqpMngrList.Add(_item);

            gridMain.Focus();
            gridMain.CurrentColumn = gridMain.Columns.First();
            gridMain.View.FocusedRowHandle = EmsEqpMngrList.Count - 1;
        }
        #endregion 신규 row 추가

        #region row 삭제
        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowDeleteClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // DelEmsEqpMngr();

                if (ValidationRowDelete() == false) { return; }

                var liHeaderCommonCodeMgmt = this.EmsEqpMngrList.Where(p => p.IsSelected == true && p.IsNew == true).ToList();
                if (liHeaderCommonCodeMgmt.Count() <= 0)
                {
                    BaseClass.MsgError("ERR_DELETE");
                }
                liHeaderCommonCodeMgmt.ForEach(p => EmsEqpMngrList.Remove(p));
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private bool ValidationRowDelete()
        {
            try
            {
                if (this.EmsEqpMngrList.Count() == 0) { return false; }

                if (this.EmsEqpMngrList.Where(p => p.IsSelected).Count() == 0)
                {
                    // Msg : 선택된 데이터가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return false;
                }

                if (this.EmsEqpMngrList.Where(p => p.IsSelected == true && p.IsNew == false).Count() > 0)
                {
                    // 조회된 데이터는 행 삭제를 할 수 없습니다.\r\n삭제버튼을 이용하여 삭제해주세요.
                    this.BaseClass.MsgError("ERR_EXISTS_INQ");
                    return false;
                }

                return true;
            }
            catch { throw; }
        }

        /// <summary>
        /// 삭제 함수
        /// </summary>
        private void DelEmsEqpMngr()
        {
            var _delItems = this.EmsEqpMngrList.Where(p => p.IsSelected).ToList();

            if (0 < _delItems.Count)
            {
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    string _SUCCESS_CODE = "100";

                    try
                    {
                        da.BeginTransaction();

                        foreach (var item in _delItems)
                        {
                            if (item.IsNew != true)
                            {
                                var param = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD",        this.BaseClass.CenterCD },
                                    { "P_EQP_MNGR_ID",      item.EQP_MNGR_ID }
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_EBSE001.SP_EMS_EQP_MNGR_DELETE";

                                var outData = da.GetSpDataSet(
                                        callProc                        // 호출 프로시저
                                    ,   param                           // Input 파라메터
                                    ,   strOutParam                     // Output 파라메터
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
                                    this.BaseClass.MsgInfo("ERR_INPUT_TYPE");
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
                                EmsEqpMngrList.Remove(item);
                            }

                            gridMain.RefreshData();
                        }
                    }
                    catch (Exception ex)
                    {
                        da.RollbackTransaction();
                        this.BaseClass.Error(ex);
                        this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                    }
                }
            }
            else
            {
                // 삭제 대상을 선택하세요.
                this.BaseClass.MsgInfo("INFO_SEL_DEL");
            }
        }
        #endregion row 삭제

        #region 저장
        /// <summary>
        /// 저장 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveClick(object sender, MouseButtonEventArgs e)
        {
            var strMessage = "{0} 이(가) 입력되지 않았습니다.";

            foreach (var item in this.EmsEqpMngrList)
            {
                if (item.IsNew || item.IsUpdate)
                {
                    if (string.IsNullOrWhiteSpace(item.EQP_MNGR_ID) == true)
                    {
                        item.CellError("COM_HDR_CD", string.Format(strMessage, this.GetLabelDesc("ID")));
                        return;
                    }
                }
            }

            if (SaveEmsEqpMngr())
            {
                // CMPT_SAVE - 저장되었습니다.
                this.BaseClass.MsgInfo("CMPT_SAVE");

                SearchEmsEqpMngr();
            }
        }

        /// <summary>
        /// 저장 함수
        /// </summary>
        /// <returns></returns>
        private bool SaveEmsEqpMngr()
        {
            using (BaseDataAccess da = new BaseDataAccess())
            {
                try
                {
                    string _SUCCESS_CODE = "100";
                    var _saveItems = EmsEqpMngrList.Where(f => f.IsUpdate || f.IsNew).ToList();

                    //if (0 == _saveItems.Count)
                    if (this.EmsEqpMngrList.Where(p => p.IsSelected).Count() == 0)
                    {
                        // ERR_NO_SELECT - 선택된 데이터가 없습니다.
                        this.BaseClass.MsgError("ERR_NO_SELECT");
                        return false;
                    }

                    // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                    string strInputMessage = this.BaseClass.GetResourceValue("ERR_NOT_INPUT", BaseEnumClass.ResourceType.MESSAGE);

                    // ASK_SAVE - 저장 하시겠습니까?
                    this.BaseClass.MsgQuestion("ASK_SAVE");
                    if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return false; }

                    foreach (var item in _saveItems)
                    {
                        if (item.IsSelected == false) { continue; }

                        if (string.IsNullOrWhiteSpace(item.EQP_MNGR_ID))
                        {
                            item.CellError("USER_ID", string.Format(strInputMessage, this.BaseClass.GetResourceValue("ID")));
                            return false;
                        }

                        if (1 < EmsEqpMngrList.Where(f => f.EQP_MNGR_ID == item.EQP_MNGR_ID).ToList().Count)
                        {
                            // ERR_EXISTS_ID - ID가 존재합니다.
                            this.BaseClass.MsgError("ERR_EXISTS_ID");
                            return false;
                        }
                    }

                    var _USER_ID = this.BaseClass.UserID;

                    da.BeginTransaction();

                    foreach (var item in _saveItems)
                    {
                        var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD",        this.BaseClass.CenterCD },
                                { "P_EQP_MNGR_ID", item.EQP_MNGR_ID },  // ID
                                { "P_MNGR_NM", item.MNGR_NM },          // 이름
                                { "P_MNGR_TEL_NO", item.MNGR_TEL_NO },  // 전화
                                { "P_VNDR_NM", item.VNDR_NM },          // 회사명
                                { "P_EMPL_DEV_CD", item.EMPL_DEV_CD },  // 고용 구분
                                { "P_USE_YN", item.USE_YN },
                                {"P_USER_ID", _USER_ID}
                            };

                        var strOutParam = new[] { "P_RESULT" };
                        string callProc = "PK_EMS_EBSE001.SP_EMS_EQP_MNGR_SAVE";

                        var outData = da.GetSpDataSet(
                                callProc                // 호출 프로시저
                            , param                     // Input 파라메터
                            , strOutParam               // Output 파라메터
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
                            this.BaseClass.MsgInfo("ERR_INPUT_TYPE");
                            break;
                        }
                    }

                    if (_SUCCESS_CODE == "100")
                    {
                        da.CommitTransaction();
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    da.RollbackTransaction();
                    this.BaseClass.Error(ex);
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }

                return false;
            }
        }
        #endregion 저장

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

        public bool TabClosing()
        {
            try
            {
                bool isRtnValue = false;

                if (this.EmsEqpMngrList.Where(p => p.IsNew || p.IsUpdate).Count() > 0)
                {
                    // 저장되지 않은 데이터가 있습니다.|종료하시겠습니까?
                    this.BaseClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
                    isRtnValue = this.BaseClass.BUTTON_CONFIRM_YN;
                }

                return isRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion
    }
}
