using DevExpress.Mvvm.Native;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMART.WCS.UI.EMS.Views.BASE_INFO
{
    /// <summary>
    /// E1007.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E1007 : UserControl
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
        public E1007()
        {
            InitializeComponent();
        }

        public E1007(List<string> _liMenuNavigation)
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

                SetBaseButton();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        public ObservableCollection<EmsChkList> MasterChkList { get; private set; }
        #endregion

        #region ▩ 함수
        #region 권한별 버튼 활성화
        private void SetBaseButton()
        {
            try
            {
                bool isHighEnabled = false;
                bool isHighNewEnabled = false;

                isHighNewEnabled =g_IsAuthAllYN;

                // 그리드 초기화
                //this.btnSaveHigh.IsEnabled = isHighEnabled;
                ////this.btnRowAddHigh.IsEnabled = isHighNewEnabled;
                //this.btnRowAddHigh.IsEnabled = isHighEnabled;
                //this.btnRowDeleteHigh.IsEnabled = isHighEnabled;
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 

        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SearchChkListMst();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 조회 함수
        /// </summary>
        private void SearchChkListMst()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD", this.BaseClass.CenterCD }
                };

                var strOutParam = new[] { "P_EMS_CHK_LIST" };
                string callProc = "PK_EMS_EBSE007.SP_EMS_CHK_LIST_MST_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    MasterChkList = new ObservableCollection<EmsChkList>();

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        MasterChkList.ToObservableCollection(outData.Tables[0]);
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }

                    gridMain.ItemsSource = MasterChkList;
                    //gridMainView.BestFitColumns();
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }

        #region 저장

        /// <summary>
        /// 저장 함수
        /// </summary>
        /// <returns></returns>
        private bool SaveChkListMst()
        {
            using (BaseDataAccess da = new BaseDataAccess())
            {
                try
                {
                    string _SUCCESS_CODE = "100";
                    var _saveItems = MasterChkList.Where(f => f.IsUpdate || f.IsNew).ToList();

                    //if (0 == _saveItems.Count)
                    //{
                    //    return false;
                    //}

                    if (this.MasterChkList.Where(p => p.IsSelected).Count() == 0)
                    {
                        // ERR_NO_SELECT - 선택된 데이터가 없습니다.
                        this.BaseClass.MsgError("ERR_NO_SELECT");
                        return false;
                    }

                    var _USER_ID = this.BaseClass.UserID;

                    this.BaseClass.MsgQuestion("ASK_SAVE");
                    if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return false; }

                    da.BeginTransaction();

                    foreach (var item in _saveItems)
                    {
                        if (item.IsSelected == false) { continue; }

                        var param = new Dictionary<string, object>
                        {
                            { "P_CENTER_CD", this.BaseClass.CenterCD },
                            { "P_CHK_NO", item.CHK_NO },                        // 순번
                            { "P_CHK_CONTENT", item.CHK_CONTENT },              // 내용
                            { "P_CHK_NOTE", item.CHK_NOTE },                    // 비고
                            {"P_USER_ID", _USER_ID}
                        };

                        var strOutParam = new[] { "P_RESULT" };
                        string callProc = "PK_EMS_EBSE007.SP_EMS_CHK_LIST_MST_SAVE";

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
                            this.BaseClass.GetResourceValue("ERR_INPUT_TYPE");
                            break;
                        }
                    }

                    if (_SUCCESS_CODE == "100")
                    {
                        da.CommitTransaction();
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_SAVE_DATA"));
                        this.BaseClass.MsgInfo("CMPT_SAVE");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    da.RollbackTransaction();
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }

                return false;
            }
        }
        #endregion 저장

        #region 삭제
        /// <summary>
        /// 삭제 함수
        /// </summary>
        private void DelChkListMst()
        {
            //var _delItems = MasterChkList.Where(f => f.IsChecked).ToList();
            var _delItems = MasterChkList.Where(f => f.IsSelected).ToList();

            if (0 < _delItems.Count)
            {
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    string _SUCCESS_CODE = "100";

                    try
                    {
                        this.BaseClass.MsgQuestion("ASK_DEL");
                        if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                        var _USER_ID = this.BaseClass.UserID;

                        da.BeginTransaction();

                        foreach (var item in _delItems)
                        {
                            if (item.IsNew != true)
                            {
                                var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD", this.BaseClass.CenterCD },
                                { "P_CHK_NO", item.CHK_NO },
                                {"P_USER_ID", _USER_ID}
                            };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_EBSE007.SP_EMS_CHK_LIST_MST_DELETE";

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

                            this.BaseClass.MsgInfo("CMPT_DEL");

                            foreach (var item in _delItems)
                            {
                                //if (item.IsNew != true)
                                MasterChkList.Remove(item);
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
                //this.BaseClass.MsgError("")
            }
        }
        #endregion 삭제

        #region 신규 추가
        /// <summary>
        /// 추가 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowAddClick(object sender, MouseButtonEventArgs e)
        {
            NewChkList();
        }

        /// <summary>
        /// 추가 함수
        /// </summary>
        private void NewChkList()
        {
            EmsChkList _item = new EmsChkList()
            {
                CENTER_CD = this.BaseClass.CenterCD,
                CHK_NO = 0,
                CHK_CONTENT = "",
                CHK_NOTE = "",
                RSTR_ID = this.BaseClass.UserID,
                UPDR_ID = this.BaseClass.UserID,
                IsNew = true,
                IsSelected = true
            };

            MasterChkList.Add(_item);

            gridMain.Focus();
            gridMain.CurrentColumn = gridMain.Columns.First();
            gridMain.View.FocusedRowHandle = MasterChkList.Count - 1;
        }

        #endregion 신규 추가

        #region > CheckSelectedItem - 그리드 선택 여부 체크
        /// <summary>
        /// 그리드 선택 여부 체크
        /// </summary>
        /// <returns></returns>
        private bool ValidationDelete()
        {
            try
            {
                if (this.MasterChkList.Count() == 0) { return false; }

                if (this.MasterChkList.Where(p => p.IsSelected).Count() == 0)
                {
                    // Msg : 선택된 데이터가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return false;
                }

                if (this.MasterChkList.Where(p => p.IsSelected == true && p.IsNew == true).Count() > 0)
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
                if (this.MasterChkList.Count() == 0) { return false; }

                if (this.MasterChkList.Where(p => p.IsSelected).Count() == 0)
                {
                    // Msg : 선택된 데이터가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return false;
                }

                if (this.MasterChkList.Where(p => p.IsSelected == true && p.IsNew == false).Count() > 0)
                {
                    // 조회된 데이터는 행삭제를 할 수 없습니다.\r\n삭제버튼을 이용하여 삭제해주세요.
                    this.BaseClass.MsgError("ERR_EXISTS_INQ_DATA");
                    return false;
                }

                return true;
            }
            catch { throw; }
        }

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

        private void btnSaveClick(object sender, MouseButtonEventArgs e)
        {
            if (SaveChkListMst())
            {
                SearchChkListMst();
            }
        }

        private void btnDelete_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.MasterChkList.Where(p => p.IsSelected).Count() == 0)
                {
                    // ERR_NO_SELECT - 선택된 데이터가 없습니다.
                    this.BaseClass.MsgError("ERR_NO_SELECT");
                    return;
                }

                this.DelChkListMst();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void btnRowDeleteClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.ValidationRowDelete() == false) { return; }

                var liHeaderCommonCodeMgmt = this.MasterChkList.Where(p => p.IsSelected == true && p.IsNew == true).ToList();
                if (liHeaderCommonCodeMgmt.Count() <= 0)
                {
                    BaseClass.MsgError("ERR_DELETE");
                }
                liHeaderCommonCodeMgmt.ForEach(p => MasterChkList.Remove(p));
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
    }
}
