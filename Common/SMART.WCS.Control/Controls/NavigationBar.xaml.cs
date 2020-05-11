using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.Control
{
    /// <summary>
    /// 업무화면 네비게이션
    /// </summary>
    public partial class NavigationBar : UserControl
    {
        #region ▩ Detegate 선언
        #region > 즐겨찾기 변경후 메인화면 트리 컨트롤 Refresh 및 포커스 이동
        public delegate void UserControlCallEventHandler();
        public event UserControlCallEventHandler UserControlCallEvent;
        #endregion
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// Base 클래서 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 로드 여부
        /// </summary>
        private bool g_isLoaded = false;
        #endregion

        #region ▩ 생성자
        public NavigationBar()
        {
            InitializeComponent();

            // 이벤트 초기화
            this.InitEvent();
        }

        #region  ▩ 재정의
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ItemsSource != null)
            {
                SetMenuNavigationBarValue(ItemsSource);
            }
        }
        #endregion
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        public static readonly DependencyProperty itemsSourceProperty 
            = DependencyProperty.Register("ItemsSource", typeof(List<string>)
                , typeof(NavigationBar), new PropertyMetadata(new List<string>()));

        /// <summary>
        /// Navigation 데이터 바인딩용 ItemsSource
        /// </summary>
        public List<string> ItemsSource
        {
            get { return (List<string>)GetValue(itemsSourceProperty); }
            set { SetValue(itemsSourceProperty, value); }
        }


        public static readonly DependencyProperty MenuIDProperty
            = DependencyProperty.Register("MenuID", typeof(string)
                , typeof(NavigationBar), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 메뉴 ID (클래스명)
        /// </summary>
        public string MenuID
        {
            get { return (string)GetValue(MenuIDProperty); }
            set { SetValue(MenuIDProperty, value); }
        }

        /// <summary>
        /// ForuthLevel 속성
        /// </summary>
        public static readonly DependencyProperty FourthLevelProperty = DependencyProperty.Register("FourthLevel", typeof(string), typeof(NavigationBar), new PropertyMetadata(""));

        /// <summary>
        /// ForuthLevel
        /// </summary>
        public string FourthLevel
        {
            get { return (string)GetValue(FourthLevelProperty); }
            set { SetValue(FourthLevelProperty, value); }
        }

        /// <summary>
        /// ThirdLevel 속성
        /// </summary>
        public static readonly DependencyProperty ThirdLevelProperty = DependencyProperty.Register("ThirdLevel", typeof(string), typeof(NavigationBar), new PropertyMetadata(""));

        /// <summary>
        /// ThirdLevel
        /// </summary>
        public string ThirdLevel
        {
            get { return (string)GetValue(ThirdLevelProperty); }
            set { SetValue(ThirdLevelProperty, value); }
        }

        /// <summary>
        /// ThirdLevel 속성
        /// </summary>
        public static readonly DependencyProperty SecondLevelProperty = DependencyProperty.Register("SecondLevel", typeof(string), typeof(NavigationBar), new PropertyMetadata(""));

        /// <summary>
        /// ThirdLevel
        /// </summary>
        public string SecondLevel
        {
            get { return (string)GetValue(SecondLevelProperty); }
            set { SetValue(SecondLevelProperty, value); }
        }
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            // 화면 로드 이벤트
            this.Loaded += NavigationBar_Loaded;
            // 즐겨찾기 적용
            this.imgBookmarkApply.PreviewMouseLeftButtonUp += ImgBookmarkApply_PreviewMouseLeftButtonUp;
            // 즐겨찾기 해제
            this.imgBookmarkCanncellation.PreviewMouseLeftButtonUp += ImgBookmarkCanncellation_PreviewMouseLeftButtonUp;
        }
        #endregion
        #endregion

        #region > 기타
        #region >> SetMenuNavigationBarValue - 업무화면 메뉴 네비게이션 값을 설정한다.
        /// <summary>
        /// 업무화면 메뉴 네비게이션 값을 설정한다.
        /// </summary>
        /// <param name="_gridNavigation">시스템 그리드 컨트롤</param>
        /// <param name="_liMenuNavigation">메뉴 데이터</param>
        public void SetMenuNavigationBarValue(List<string> _liMenuNavigation)
        {
            try
            {
                // 메뉴 레벨수
                int iMenuDepth = _liMenuNavigation.Count();

                #region TextBlock 컨트롤을 찾아 Name을 저장한다.
                // TextBlock를 싸고 있는 Grid 태그를 중심으로 컨트롤을 구한다.
                for (int i = 0; i < _gridNavigation.Children.Count; i++)
                //for (int i = _gridNavigation.Children.Count - 1; i > 0; i--)
                {
                    // 자식 컨트롤을 저장한다.
                    var childControl = _gridNavigation.Children[i];

                    // 해당 컨트롤 찾기 (FindControl)
                    if (childControl is System.Windows.Controls.TextBlock)
                    {
                        System.Windows.Controls.TextBlock textBlock = (System.Windows.Controls.TextBlock)childControl;

                        if (iMenuDepth == 1)
                        {
                            switch (textBlock.Name)
                            {
                                case "lblFirstChar":
                                case "lblSecondNavigation":
                                case "lblSecondChar":
                                case "lblThirdNavigation":
                                case "lblThirdChar":
                                case "lblFourthNavigation":
                                case "lblFourthChar":
                                    textBlock.Visibility = Visibility.Hidden;
                                    textBlock.Width = 0;
                                    break;
                                case "lblFirstNavigation":
                                    textBlock.Text = _liMenuNavigation[0];
                                    break;
                                default: break;
                            }
                        }
                        else if (iMenuDepth == 2)
                        {
                            switch (textBlock.Name)
                            {
                                case "lblSecondChar":
                                case "lblThirdNavigation":
                                case "lblThirdChar":
                                case "lblFourthNavigation":
                                case "lblFourthChar":
                                    textBlock.Visibility = Visibility.Hidden;
                                    textBlock.Width = 0;
                                    break;
                                case "lblFirstNavigation":
                                    textBlock.Text = _liMenuNavigation[0];
                                    break;
                                case "lblFirstChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblSecondNavigation":
                                    textBlock.Text = _liMenuNavigation[1];
                                    break;
                                default: break;
                            }
                        }
                        else if (iMenuDepth == 3)
                        {
                            switch (textBlock.Name)
                            {
                                case "lblFourthNavigation":
                                case "lblFourthChar":
                                case "lblThirdChar":
                                    textBlock.Visibility = Visibility.Hidden;
                                    textBlock.Width = 0;
                                    break;
                                case "lblFirstNavigation":
                                    textBlock.Text = _liMenuNavigation[0];
                                    break;
                                case "lblFirstChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblSecondNavigation":
                                    textBlock.Text = _liMenuNavigation[1];
                                    break;
                                case "lblSecondChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblThirdNavigation":
                                    textBlock.Text = _liMenuNavigation[2];
                                    break;
                                default: break;
                            }
                        }
                        else if (iMenuDepth == 4)
                        {
                            switch (textBlock.Name)
                            {
                                case "lblFirstNavigation":
                                    textBlock.Text = _liMenuNavigation[0];
                                    break;
                                case "lblFirstChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblSecondNavigation":
                                    textBlock.Text = _liMenuNavigation[1];
                                    break;
                                case "lblSecondChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblThirdNavigation":
                                    textBlock.Text = _liMenuNavigation[2];
                                    break;
                                case "lblThirdChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                case "lblFourthNavigation":
                                    textBlock.Text = _liMenuNavigation[3];
                                    break;
                                case "lblFourthChar":
                                    textBlock.Margin = new Thickness(15, 0, 10, 0);
                                    break;
                                default: break;
                            }
                        }
                    }
                }
                #endregion
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_MENU_FVRT_INQ - 화면 (메뉴)별 즐겨찾기 여부 데이터 조회
        /// <summary>
        /// 화면 (메뉴)별 즐겨찾기 여부 데이터 조회
        /// </summary>
        /// <returns></returns>
        private async Task GetSP_MENU_FVRT_INQ()
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1000.SP_MENU_FVRT_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_FVRT_LIST", "O_RSLT" };

            var strCenterCD     = this.BaseClass.CenterCD;                                  // 센터코드
            var strUserID       = this.BaseClass.UserID;
            
            
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCenterCD);       // 센터코드
            dicInputParam.Add("P_USER_ID",          strUserID);         // 사용자 ID
            dicInputParam.Add("P_MENU_ID",          this.MenuID);       // 메뉴 ID
            #endregion

            #region + 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            #region > 북마크 적용 여부  
            if (dsRtnValue == null) { return; }
                
            var strErrCode          = string.Empty;     // 오류 코드
            var strErrMsg           = string.Empty;     // 오류 메세지

            if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
            {
                var isBookMarkValue     = dsRtnValue.Tables[0].Rows[0]["MENU_FVRT_YN"].Equals("Y") ? true : false;

                if (isBookMarkValue == true)
                {
                    this.imgBookmarkApply.Visibility            = Visibility.Visible;
                    this.imgBookmarkCanncellation.Visibility    = Visibility.Hidden;
                }
                else
                {
                    this.imgBookmarkApply.Visibility            = Visibility.Hidden;
                    this.imgBookmarkCanncellation.Visibility    = Visibility.Visible;
                }
            }
            #endregion
        }
        #endregion

        #region >> InsertSP_MENU_FVRT_INS - 즐겨찾기 등록
        /// <summary>
        /// 즐겨찾기 등록
        /// </summary>
        /// <param name="_da">데이터베이스 객체</param>
        /// <returns></returns>
        private async Task<bool> InsertSP_MENU_FVRT_INS(BaseDataAccess _da)
        {
            bool isRtnValue     = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "PK_C1000.SP_MENU_FVRT_INS";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                      // 센터코드
            var strUserID           = this.BaseClass.UserID;                        // 사용자 ID
            var strMenuID           = this.MenuID;
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",      strCenterCD);   // 센터코드   
            dicInputParam.Add("P_USER_ID",      strUserID);     // 사용자 ID
            dicInputParam.Add("P_MENU_ID",      strMenuID);     // 메뉴 ID
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);
            #endregion

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
        #endregion

        #region DeleteSP_MENU_FVRT_DEL - 즐겨찾기 해제
        /// <summary>
        /// 즐겨찾기 해제
        /// </summary>
        /// <param name="_da">데이터베이스 객체</param>
        /// <returns></returns>
        private async Task<bool> DeleteSP_MENU_FVRT_DEL(BaseDataAccess _da)
        {
            bool isRtnValue     = true;

            #region + 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "PK_C1000.SP_MENU_FVRT_DEL";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                      // 센터코드
            var strUserID           = this.BaseClass.UserID;                        // 사용자 ID
            var strMenuID           = this.MenuID;
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",      strCenterCD);   // 센터코드   
            dicInputParam.Add("P_USER_ID",      strUserID);     // 사용자 ID
            dicInputParam.Add("P_MENU_ID",      strMenuID);     // 메뉴 ID
            #endregion

            #region + 데이터 조회
            await System.Threading.Tasks.Task.Run(() =>
            {
                dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);
            #endregion

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
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 화면 로드 이벤트
        /// <summary>
        /// 화면 로드 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NavigationBar_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.g_isLoaded == true) { return; }

                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == true) { return; }

                await this.GetSP_MENU_FVRT_INQ();

                this.g_isLoaded = false;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 버튼 이벤트
        #region >> 즐겨찾기 적용 버튼 클릭 이벤트
        /// <summary>
        /// 즐겨찾기 적용 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImgBookmarkApply_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //// 현재 화면의 즐겨찾기를 해제하시겠습니까?
                //this.BaseClass.MsgQuestion("ASK_DEL_FVRT");
                //if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = true;

                #region + 데이터 저장
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 트랜잭션 시작
                        da.BeginTransaction();

                        // 즐겨찾기 해제
                        isRtnValue = await this.DeleteSP_MENU_FVRT_DEL(da);


                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();

                            //// CMPT - 완료 되었습니다.
                            //this.BaseClass.MsgInfo("CMPT");

                            await this.GetSP_MENU_FVRT_INQ();
                        }
                        else
                        {
                            // ERR_SAVE - 저장 중 오류가 발생 했습니다.
                            this.BaseClass.MsgError("ERR_SAVE");
                        }
                    }
                    catch { throw; }
                    finally
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }
                    }
                }
                #endregion

                // 유저컨트롤에서 발생한 이벤트를 부모창으로 전달
                // 즐겨찾기 변경 완료 여부 전달
                this.UserControlCallEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 즐겨찾기 해제 버튼 클릭 이벤트
        /// <summary>
        /// 즐겨찾기 해제 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImgBookmarkCanncellation_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //// 현재 화면을 즐겨찾기에 등록하시겠습니까?
                //this.BaseClass.MsgQuestion("ASK_REG_FVRT");
                //if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue = true;

                #region + 데이터 저장
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 트랜잭션 시작
                        da.BeginTransaction();

                        // 즐겨찾기 반영
                        isRtnValue = await this.InsertSP_MENU_FVRT_INS(da);

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();

                            //// CMPT - 완료 되었습니다.
                            //this.BaseClass.MsgInfo("CMPT");

                            await this.GetSP_MENU_FVRT_INQ();
                        }
                        else
                        {
                            da.RollbackTransaction();
                            // ERR_SAVE - 저장 중 오류가 발생 했습니다.
                            this.BaseClass.MsgError("ERR_SAVE");
                        }
                    }
                    catch { throw; }
                }
                #endregion

                // 유저컨트롤에서 발생한 이벤트를 부모창으로 전달
                // 즐겨찾기 변경 완료 여부 전달
                this.UserControlCallEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion
        #endregion
    }
}
