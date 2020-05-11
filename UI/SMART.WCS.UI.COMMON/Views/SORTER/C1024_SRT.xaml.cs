using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.COMMON.DataMembers.C1024_SRT;
using System;
using System.Text.RegularExpressions;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMART.WCS.UI.COMMON.Views.SORTER
{
    /// <summary>
    /// C1024_SRT.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1024_SRT : UserControl
    {
        #region ▩ Detegate 선언
        #region > 메인화면 하단 좌측 상태바 값 반영
        //public delegate void ToolStripStatusEventHandler(string value);
        //public event ToolStripStatusEventHandler ToolStripChangeStatusLabelEvent;
        #endregion

        #region > 즐겨찾기 변경후 메인화면 트리 컨트롤 Refresh 및 포커스 이동
        public delegate void TreeControlRefreshEventHandler();
        public event TreeControlRefreshEventHandler TreeControlRefreshEvent;
        #endregion
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// Base 클래스 선언
        /// </summary>
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// BaseInfo 선언
        /// </summary>
        BaseInfo BaseInfo = new BaseInfo();

        /// <summary>
        /// 화면 전체권한 부여 (true : 전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public C1024_SRT()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public C1024_SRT(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 상단에 설명 
                this.CommentArea.ScreenID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

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
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(C1024_SRT), new FrameworkPropertyMetadata(IsEnabledPropertyChanged));

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
          //      view.ShowingEditor += view_ShowingEditor;
            }
        }
        #endregion

        #region > 바코드 정규식 조회
        #region >> 배치 차수 조회
        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public static readonly DependencyProperty BcrRegexMgmtProperty
            = DependencyProperty.Register("BcrRegexMgmtList", typeof(ObservableCollection<BcrRegexMgmt>), typeof(C1024_SRT)
                , new PropertyMetadata(new ObservableCollection<BcrRegexMgmt>()));

        /// <summary>
        /// 배치 차수 조회
        /// </summary>
        public ObservableCollection<BcrRegexMgmt> BcrRegexMgmtList
        {
            get { return (ObservableCollection<BcrRegexMgmt>)GetValue(BcrRegexMgmtProperty); }
            set { SetValue(BcrRegexMgmtProperty, value); }
        }
        #endregion

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(C1024_SRT), new PropertyMetadata(string.Empty));

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
            // 공통코드 조회 파라메터 string[]
            string[] commonParam_EQP_ID = { BaseClass.CenterCD, "SRT", BaseClass.UserID, string.Empty };
            this.BaseClass.BindingCommonComboBox(this.CboSrt, "EQP_ID", commonParam_EQP_ID, false);//설비 ID

            // 화면 오픈시 바로 조회되도록
            #region + 적용후 테이블
            DataSet dsRtnValue = null;
            dsRtnValue = this.GetSP_BCD_REGEX_INQ();
            this.ShowSP_BCD_REGEX_INQ(dsRtnValue);
        }

        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += C1024_SRT_Loaded;
            #endregion

            #region + 바코드 정규식 조회
            #region ++ 버튼 클릭 이벤트
            // 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            #endregion

            #endregion
        }
        #endregion
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_BCD_REGEX_INQ - 바코드 정규식 조회
        /// <summary>
        /// 바코드 정규식 조회
        /// </summary>
        private DataSet GetSP_BCD_REGEX_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1024_SRT.SP_BCD_REGEX_INQ";
           
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_BCD_REGEX_LIST", "O_RSLT" };

            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            var strEqpId = this.BaseClass.ComboBoxSelectedKeyValue(this.CboSrt);                    //설비 ID

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                  // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpId);                    //설비 ID  
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

        #region >> ShowSP_BCD_REGEX_INQ - 바코드 정규식 조회
        /// <summary>
        /// 바코드 정규식 조회 결과 화면에 보여주기
        /// </summary>
        private void ShowSP_BCD_REGEX_INQ(DataSet dsRtnValue)
        {
            if (dsRtnValue == null) { return; }

            var strErrCode = string.Empty;
            var strErrMsg = string.Empty;

            if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
            {
                if (dsRtnValue.Tables[0].Rows.Count == 0)
                {
                    this.BaseClass.MsgInfo("INFO_NOT_INQ");
                    return;
                }
                // 정상 처리된 경우 
                foreach (DataRow dr in dsRtnValue.Tables[0].Rows)
                {
                    txtBoxBcdRegex.Text = dr["BOX_BCD_REGEX"].ToString();
                    txtInvNoRegex.Text = dr["INV_NO_REGEX"].ToString();
                    txtRgnBcdRegex.Text = dr["RGN_BCD_REGEX"].ToString();
                    txtBoxBcdRegex_bef.Text = dr["BOX_BCD_REGEX_BEF"].ToString();
                    txtInvNoRegex_bef.Text = dr["INV_NO_REGEX_BEF"].ToString();
                    txtRgnBcdRegex_bef.Text = dr["RGN_BCD_REGEX_BEF"].ToString();
                    txtUpdDt.Text = dr["UPD_DT"].ToString();
                }
            }
            else
            {
                // 오류 발생한 경우 - 조회 중 에러가 발생하였습니다.
                this.BaseClass.MsgError("ERR_INQ");
            }
        }
        #endregion

        #region >> TestRegex - 정규식 테스트 
        /// <summary>
        /// 텍스트박스 입력값이 바코드별 지정된 정규식에 맞는 형식인지 테스트한다. 
        /// 정규식에 부합하면 result textedit 창에 입력 텍스트 그대로, 아니면 빈칸으로 출력
        /// </summary>
        private string TestRegex(string regex, string test)
        {
            Regex reg = new Regex(@regex); //정규식 

            if (reg.IsMatch(test))
            {
                return reg.Match(test).ToString();
            }
            else
            {
                return "정규식에 일치하지 않는 바코드입니다.";
            }

        }
        #endregion
        #endregion

        #endregion

        #region ▩ 이벤트
        #region > Loaded 이벤트
        private void C1024_SRT_Loaded(object sender, RoutedEventArgs e)
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

        #region > 바코드 정규식 관리
        #region >> 버튼 클릭 이벤트
        #region + 바코드 정규식 관리 조회 버튼 클릭 이벤트
        /// <summary>
        /// 바코드 정규식 관리 조회 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                #region + 적용후 테이블
                DataSet dsRtnValue = null;
                dsRtnValue = this.GetSP_BCD_REGEX_INQ();

                this.ShowSP_BCD_REGEX_INQ(dsRtnValue);
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

        #region > 정규식 테스트 버튼 이벤트 (박스, Invoice, Sorting)
        /// <summary>
        /// 정규식 테스트 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        #region >> [적용중] 버튼 3개 (박스, Invoice, Sorting)
        private void btnRegexTest_box_Click(object sender, RoutedEventArgs e)
        {
            string strRegex = txtBoxBcdRegex.Text.Trim();
            string strTest = txtBoxBcdRegex_Test.Text.Trim();

            txtBoxBcdRegex_result.Text = TestRegex(strRegex, strTest);
        }

        private void btnRegexTest_inv_Click(object sender, RoutedEventArgs e)
        {
            string strRegex = txtInvNoRegex.Text.Trim();
            string strTest = txtInvNoRegex_Test.Text.Trim();

            txtInvNoRegex_result.Text = TestRegex(strRegex, strTest);
        }

        private void btnRegexTest_Rgn_Click(object sender, RoutedEventArgs e)
        {
            string strRegex = txtRgnBcdRegex.Text.Trim();
            string strTest  = txtRgnBcdRegex_Test.Text.Trim();
            
            txtRgnBcdRegex_result.Text = TestRegex(strRegex, strTest);
        }
        #endregion

        #region >> [이전] 버튼 3개 (박스, Invoice, Sorting)
        private void btnRegexTest_box_bef_Click(object sender, RoutedEventArgs e)
        {
            string strRegex = txtBoxBcdRegex_bef.Text.Trim();
            string strTest = txtBoxBcdRegex_Test_bef.Text.Trim();

            txtBoxBcdRegex_result_bef.Text = TestRegex(strRegex, strTest);
        }

        private void btnRegexTest_inv_bef_Click(object sender, RoutedEventArgs e)
        {
            string strRegex = txtInvNoRegex_bef.Text.Trim();
            string strTest = txtInvNoRegex_Test_bef.Text.Trim();

            txtInvNoRegex_result_bef.Text = TestRegex(strRegex, strTest);
        }

        private void btnRegexTest_Rgn_bef_Click(object sender, RoutedEventArgs e)
        {
            string strRegex = txtRgnBcdRegex_bef.Text.Trim();
            string strTest = txtRgnBcdRegex_Test_bef.Text.Trim();

            txtRgnBcdRegex_result_bef.Text = TestRegex(strRegex, strTest);
        }
        #endregion
        #endregion
        #endregion

    }
}
