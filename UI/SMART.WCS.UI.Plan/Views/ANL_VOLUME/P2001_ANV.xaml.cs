using DevExpress.Mvvm.Native;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.UI.Plan.DataMembers.P2001_ANV;
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

namespace SMART.WCS.UI.Plan.Views.ANL_VOLUME
{
    /// <summary>
    /// 출고물동량분석결과조회
    /// P2001_ANV.xaml에 대한 상호 작용 논리
    /// PK_P2001_ANV.SP_AN_CO_SEARCH
    /// PK_P2001_ANV.SP_AN_AGG_TYPE_SEARCH
    /// PK_P2001_ANV.SP_AN_EXT_TAR_SEARCH
    /// PK_P2001_ANV.SP_AN_RLST_SEARCH
    /// PK_P2001_ANV.SP_AN_DTL_RLST_SEARCH
    /// PK_P2001_ANV.SP_AN_EXT_COND_SAVE
    /// PK_P2001_ANV.SP_OB_EXT_CAPA
    /// PK_P2001_ANV.SP_OB_EXT_EIQ
    /// PK_P2001_ANV.SP_OB_EXT_ABC
    /// </summary>
    public partial class P2001_ANV : UserControl
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

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의

        #region >> IsColumnChecked - 체크 컬럼 헤더 체크 여부
        public static readonly DependencyProperty IsColumnCheckedProperty
            = DependencyProperty.Register("IsColumnChecked", typeof(bool), typeof(P2001_ANV)
                , new PropertyMetadata(false));

        public bool IsColumnChecked
        {
            get { return (bool)GetValue(IsColumnCheckedProperty); }
            set { SetValue(IsColumnCheckedProperty, value); }
        }
        #endregion

        #region >> AnRlstList - 추출 항목 리스트
        public static readonly DependencyProperty AnRlstListProperty
            = DependencyProperty.Register("AnRlstList", typeof(ObservableCollection<AnRlstInfo>), typeof(P2001_ANV)
                , new PropertyMetadata(new ObservableCollection<AnRlstInfo>()));

        public ObservableCollection<AnRlstInfo> AnRlstList
        {
            get { return (ObservableCollection<AnRlstInfo>)GetValue(AnRlstListProperty); }
            set { SetValue(AnRlstListProperty, value); }
        }
        #endregion

        #region >> SelecteAnRlsttem - 선택된 추출 항목
        public static readonly DependencyProperty _SelecteAnRlsttem
            = DependencyProperty.Register("SelecteAnRlsttem", typeof(AnRlstInfo), typeof(P2001_ANV)
                , new PropertyMetadata(new AnRlstInfo()));

        public AnRlstInfo SelecteAnRlsttem
        {
            get { return (AnRlstInfo)GetValue(_SelecteAnRlsttem); }
            set { SetValue(_SelecteAnRlsttem, value); }
        }
        # endregion
        
        #region >> AnDtlList - 상세 추출 항목 리스트
        public static readonly DependencyProperty AnDtlListProperty
            = DependencyProperty.Register("AnDtlList", typeof(ObservableCollection<AnDtlInfo>), typeof(P2001_ANV)
                , new PropertyMetadata(new ObservableCollection<AnDtlInfo>()));

        public ObservableCollection<AnDtlInfo> AnDtlList
        {
            get { return (ObservableCollection<AnDtlInfo>)GetValue(AnDtlListProperty); }
            set { SetValue(AnDtlListProperty, value); }
        }
        #endregion

        #region >> AnAbcDtlList - 상세 추출 항목 리스트 - ABC
        public static readonly DependencyProperty AnAbcDtlListProperty
            = DependencyProperty.Register("AnAbcDtlList", typeof(ObservableCollection<AnAbcDtlInfo>), typeof(P2001_ANV)
                , new PropertyMetadata(new ObservableCollection<AnAbcDtlInfo>()));

        public ObservableCollection<AnAbcDtlInfo> AnAbcDtlList
        {
            get { return (ObservableCollection<AnAbcDtlInfo>)GetValue(AnAbcDtlListProperty); }
            set { SetValue(AnAbcDtlListProperty, value); }
        }
        #endregion

        #region >> Grid Row수 (GridRowCount)
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty GridRowCountProperty
            = DependencyProperty.Register("GridRowCount", typeof(string), typeof(P2001_ANV),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string GridRowCount
        {
            get { return (string)GetValue(GridRowCountProperty); }
            set { SetValue(GridRowCountProperty, value); }
        }
        #endregion

        bool FirstLoad = true;

        #endregion

        #region ▩ 생성자
        public P2001_ANV()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public P2001_ANV(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                this.BaseInfo = ((SMART.WCS.Control.BaseApp)Application.Current).BASE_INFO;

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

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

        #region ▩ 함수

        #region > 초기화

        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            // 콤보박스 - 회사 정보
            this.GetAnCoData();

            // 콤보박스 - 구분 정보
            this.GetAnExtTarData();
        }

        private void GetAnCoData()
        {
            DataTable dtComboData = null;

            var strProcedureName = "PK_P2001_ANV.SP_AN_CO_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            using (BaseDataAccess da = new BaseDataAccess())
            {
                dtComboData = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }

            List<ComboBoxInfo> ComboBoxInfo = new List<ComboBoxInfo>();

            foreach (DataRow citem in dtComboData.Rows)
            {
                ComboBoxInfo.Add(new ComboBoxInfo { CODE = citem["CD"].ToString(), NAME = citem["NM"].ToString() });
            }

            this.cboAnCo.ItemsSource = ComboBoxInfo;

            if (ComboBoxInfo.Count > 0)
            {
                foreach (var item in ComboBoxInfo)
                {
                    if (item.CODE.Equals(this.BaseClass.CompanyCode))               //해당 아이템과 일치하는 아이템으로 선택
                    {
                        cboAnCo.SelectedItem = item;
                    }
                }

                if(cboAnCo.SelectedItem == null)
                {
                    this.cboAnCo.SelectedIndex = 0;
                }
            }
        }

        private void GetAnExtTarData()
        {
            DataTable dtComboData = null;

            var strProcedureName = "PK_P2001_ANV.SP_AN_EXT_TAR_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboAnCo);         // 회사 코드

            dicInputParam.Add("P_CO_CD", strCoCd);

            using (BaseDataAccess da = new BaseDataAccess())
            {
                dtComboData = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }

            List<ComboBoxInfo> ComboBoxInfo = new List<ComboBoxInfo>();

            foreach (DataRow citem in dtComboData.Rows)
            {
                ComboBoxInfo.Add(new ComboBoxInfo { CODE = citem["CD"].ToString(), NAME = citem["NM"].ToString() });
            }

            // 바인딩 데이터가 있는 경우 첫번째 Row를 선택하도록 한다.
            if (ComboBoxInfo.Count > 0)
            {
                ComboBoxInfo.Insert(0, new ComboBoxInfo { CODE = " ", NAME = BaseClass.GetAllValueByLanguage() });  // 전체
            }

            this.cboAnExtTar.ItemsSource = ComboBoxInfo;
            this.cboAnExtTar.SelectedIndex = 0;
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + Loaded 이벤트
            this.Loaded += P2001_ANV_Loaded;
            #endregion

            #region + 버튼 클릭 이벤트
            // 조회
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;

            // 신규
            this.btnAnExtCondCreate.PreviewMouseLeftButtonUp += BtnAnExtCondCreate_PreviewMouseLeftButtonUp;

            // 추출결과 확인
            this.btnAnExtCondCheck.PreviewMouseLeftButtonUp += BtnAnExtCondCheck_PreviewMouseLeftButtonUp;

            #endregion

            #region Grid 관련 이벤트
            this.gridMaster.PreviewMouseLeftButtonUp += GridMaster_PreviewMouseLeftButtonUp;

            #endregion
        }
        #endregion

        #endregion

        #region ▩ 이벤트

        #region > Loaded 이벤트
        private void P2001_ANV_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstLoad)
                {
                    FirstLoad = false;
                    this.AnRlstSearch();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AnRlstSearch();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// [신규] 버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAnExtCondCreate_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strCoCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboAnCo);         // 회사 코드
                var strCoNm = this.BaseClass.ComboBoxSelectedDisplayValue(this.cboAnCo);         // 회사 코드

                P2001_ANV_01P frmChild = new P2001_ANV_01P(strCoCd, strCoNm);
                frmChild.Owner = Window.GetWindow(this);
                frmChild.Closed += FrmChild_Closed;
                frmChild.ShowDialog();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// [추출결과확인] 버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAnExtCondCheck_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                P2001_ANV_01P frmChild = new P2001_ANV_01P(SelecteAnRlsttem);
                frmChild.Owner = Window.GetWindow(this);
                frmChild.Closed += FrmChild_Closed;
                frmChild.ShowDialog();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 팝업 클로즈 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmChild_Closed(object sender, EventArgs e)
        {
            AnRlstSearch();
        }

        /// <summary>
        /// 그리드 선텍 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMaster_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.SelecteAnRlsttem == null)
            {
                return;
            }
            else
            {
                this.AnDtlListSearch();
            }
        }
        #endregion

        #region > 데이터 관련 함수
        /// <summary>
        /// 상세 추출 항목 조회
        /// </summary>
        private void AnDtlListSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.AnDtlList = new ObservableCollection<AnDtlInfo>();
                this.AnAbcDtlList = new ObservableCollection<AnAbcDtlInfo>();

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_AN_DTL_RLST_SEARCH();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    if (this.SelecteAnRlsttem.EXT_TYPE_NM.Equals("ABC"))
                    {
                        gridDetail.Visibility = Visibility.Collapsed;
                        gridDetailABC.Visibility = Visibility.Visible;

                        this.AnAbcDtlList.ToObservableCollection(dsRtnValue.Tables[0]);
                    }
                    else
                    {
                        gridDetail.Visibility = Visibility.Visible;
                        gridDetailABC.Visibility = Visibility.Collapsed;

                        this.AnDtlList.ToObservableCollection(dsRtnValue.Tables[0]);
                    }
                }
                else
                {
                    // 오류가 발생한 경우
                    gridDetail.Visibility = Visibility.Visible;
                    gridDetailABC.Visibility = Visibility.Collapsed;

                    this.AnDtlList.ToObservableCollection(null);
                    this.AnAbcDtlList.ToObservableCollection(null);

                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetDetailResultText();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;

                if (this.AnRlstList.Count == 0)
                {
                    this.BaseClass.MsgInfo("INFO_NOT_INQ");
                }
            }
        }
        
        private DataSet GetSP_AN_DTL_RLST_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P2001_ANV.SP_AN_DTL_RLST_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.SelecteAnRlsttem.CO_CD;        // 회사 코드
            var strExtId = this.SelecteAnRlsttem.EXT_ID;    // 구분 코드
            var strExtTypeCd = this.SelecteAnRlsttem.EXT_TYPE_CD;    // 추출타입 코드

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_EXT_ID", strExtId);
            dicInputParam.Add("P_EXT_TYPE_CD", strExtTypeCd);
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }

        /// <summary>
        /// 추출 항목 리스트 조회
        /// </summary>
        private void AnRlstSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.AnRlstList = new ObservableCollection<AnRlstInfo>();   // 추출 항목 리스트
                this.AnDtlList = new ObservableCollection<AnDtlInfo>();     // 상세 추출 항목 리스트
                this.SelecteAnRlsttem = new AnRlstInfo();                   // 선택된 추출 항목

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_AN_RSLT_SEARCH();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.AnRlstList.ToObservableCollection(dsRtnValue.Tables[0]);

                    this.SelecteAnRlsttem = this.AnRlstList.FirstOrDefault();
                }
                else
                {
                    // 오류가 발생한 경우
                    this.AnRlstList.ToObservableCollection(null);
                    this.SelecteAnRlsttem = new AnRlstInfo();
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }

                // 조회 데이터를 그리드에 바인딩한다.
                //this.gridMaster.ItemsSource = this.AnRlstList;

                // 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
                this.SetResultText();

                if (this.SelecteAnRlsttem != null)
                {
                    //상세 추출 항목 조회
                    this.AnDtlListSearch();
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

                if (this.AnRlstList.Count == 0)
                {
                    this.BaseClass.MsgInfo("INFO_NOT_INQ");
                }

            }
        }

        #region >> GetSP_AN_RSLT_SEARCH - 물동량 추출 항목 리스트 조회
        /// <summary>
        /// 데이터조회
        /// </summary>
        private DataSet GetSP_AN_RSLT_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P2001_ANV.SP_AN_RLST_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboAnCo);        // 회사 코드
            var strExtTarCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboAnExtTar);    // 구분 코드

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_EXT_TAR_CD", strExtTarCd);
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

        #region >> SetResultText - 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// <summary>
        /// 데이터 조회 결과를 그리드 카운트 및 메인창 상태바에 설정한다.
        /// </summary>
        private void SetDetailResultText()
        {
            var strResource = string.Empty;                                                           // 텍스트 리소스 (전체 데이터 수)
            var iTotalRowCount = 0;                                                                   // 조회 데이터 수

            strResource = this.BaseClass.GetResourceValue("TOT_DATA_CNT");                            // 텍스트 리소스
            iTotalRowCount = (this.gridDetail.ItemsSource as ICollection).Count;                      // 전체 데이터 수
            this.GridRowCount = $"{strResource} : {iTotalRowCount.ToString()}";                       // 전체 데이터 수를 TextBlock 컨트롤에 바인딩한다.
            strResource = this.BaseClass.GetResourceValue("DATA_INQ");                                // 건의 데이터가 조회되었습니다.
            this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");

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
    }
}
