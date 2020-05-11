using DevExpress.Mvvm.Native;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.Plan.DataMembers.P1004_SRT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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

namespace SMART.WCS.UI.Plan.Views.SMS
{
    /// <summary>
    /// P1008_SRT_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class P1008_SRT_01P : Window, IDisposable
    {
        #region ▩ 전역변수
        BaseClass BaseClass = new BaseClass();

        private string g_strEqpID = string.Empty;
        #endregion

        #region ▩ 생성자
        public P1008_SRT_01P()
        {
            InitializeComponent();
        }

        public P1008_SRT_01P(string _strEqpID)
        {
            try
            {
                InitializeComponent();

                this.g_strEqpID = _strEqpID;

                // 이벤트 초기화
                this.InitEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 속성
        /// <summary>
        /// 복사 대상 플랜 코드
        /// </summary>
        public string NEW_PLAN_CD { get; set; }

        /// <summary>
        /// 복사 대상 플랜명
        /// </summary>
        public string NEW_PLAN_NM { get; set; }

        public string CHUTE_PLAN_CD { get; set; }
        #endregion

        #region 
        #region > 그리드 - 슈트작업계획 헤더 리스트
        public static readonly DependencyProperty ChutePlanHeaderListProperty
            = DependencyProperty.Register("ChutePlanHeaderList", typeof(ObservableCollection<ChutePlanHeaderMgmt>), typeof(P1008_SRT_01P)
                , new PropertyMetadata(new ObservableCollection<ChutePlanHeaderMgmt>()));

        public ObservableCollection<ChutePlanHeaderMgmt> ChutePlanHeaderList
        {
            get { return (ObservableCollection<ChutePlanHeaderMgmt>)GetValue(ChutePlanHeaderListProperty); }
            set { SetValue(ChutePlanHeaderListProperty, value); }
        }

        #region >> Grid Row수
        /// <summary>
        /// Grid Row수
        /// </summary>
        public static readonly DependencyProperty HeaderGridRowCountProperty
            = DependencyProperty.Register("HeaderGridRowCount", typeof(string), typeof(P1008_SRT_01P), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Grid Row수
        /// </summary>
        public string HeaderGridRowCount
        {
            get { return (string)GetValue(HeaderGridRowCountProperty); }
            set { SetValue(HeaderGridRowCountProperty, value); }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 함수
        #region > InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            try
            {
                // 조회 버튼 클릭
                this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
                // 슈트 플랜복사 버튼 클릭
                this.btnChutePlanCopy.PreviewMouseLeftButtonUp += BtnChutePlanCopy_PreviewMouseLeftButtonUp;
                // 닫기 버튼 클릭
                this.btnClose.PreviewMouseLeftButtonUp += BtnClose_PreviewMouseLeftButtonUp;
                // 폼 닫기 버튼 (이미지) 클릭
                this.btnFormClose.PreviewMouseLeftButtonUp += BtnFormClose_PreviewMouseLeftButtonUp;
            }
            catch { throw; }
        }
        #endregion

        #region > 조회
        private void GetSP_CHUTE_PLAN_LIST_INQ()
        {
            try
            {
                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_P1004_SRT.SP_CHUTE_PLAN_LIST_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_CHUTE_PLAN_LIST", "O_RSLT" };

                var strCenterCD                 = this.BaseClass.CenterCD;          // 센터코드
                var strEqpID                    = this.g_strEqpID;                  // 설비 ID
                var strPlanCD                   = string.Empty;                     // 슈트 플랜코드
                var strPlanNM                   = this.txtChutePlanNM.Text.Trim();  // 슈트 플랜명
                var strRgnCD                    = string.Empty;                     // 권역 코드명
                var strChuteID                  = string.Empty;                     // 슈트 ID
                var strUseYN                    = string.Empty;                     // 사용여부
                var strZoneID                   = string.Empty;                     // Zone ID
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTR_CD",          strCenterCD);
                dicInputParam.Add("P_EQP_ID",           strEqpID);
                dicInputParam.Add("P_PLAN_NM",          strPlanNM);
                dicInputParam.Add("P_RGN_CD",           strRgnCD);
                dicInputParam.Add("P_CHUTE_ID",         strChuteID);
                dicInputParam.Add("P_USE_YN",           strUseYN);
                dicInputParam.Add("P_ZONE_ID",          strZoneID);
                #endregion

                #region 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }
                #endregion

                this.ChutePlanHeaderList = new ObservableCollection<ChutePlanHeaderMgmt>();
                this.ChutePlanHeaderList.ToObservableCollection(dsRtnValue.Tables[0]);
                this.gridMasterHeader.ItemsSource   = this.ChutePlanHeaderList;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 버튼 이벤트
        #region >> 조회 버튼 클릭
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.GetSP_CHUTE_PLAN_LIST_INQ();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        #region >> 슈트 플랜복사 버튼 클릭
        /// <summary>
        /// 슈트 플랜복사 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnChutePlanCopy_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string strMessage       = string.Empty;

                if (this.txtChutePlanCD.Text.Trim().Length == 0)
                {
                    // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                    strMessage = string.Format(this.BaseClass.GetResourceValue("ERR_NOT_INPUT"), this.BaseClass.GetResourceValue("PLAN_CD"));
                    this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                    this.txtChutePlanCD.Focus();
                    return;
                }

                if (this.txtChutePlanNM.Text.Trim().Length == 0)
                {
                    // ERR_NOT_INPUT - {0}이(가) 입력되지 않았습니다.
                    strMessage = string.Format(this.BaseClass.GetResourceValue("ERR_NOT_INPUT"), this.BaseClass.GetResourceValue("PLAN_NM"));
                    this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                    this.txtChutePlanNM.Focus();
                    return;
                }

                var liSelectedPlanList  = this.ChutePlanHeaderList.Where(p => p.IsSelected).ToList();

                if (liSelectedPlanList.Count() == 0)
                {
                    // 선택한 플랜코드가 없습니다.
                    this.BaseClass.MsgError("선택한 플랜코드가 없습니다.", BaseEnumClass.CodeMessage.MESSAGE);
                    return;
                }

                this.NEW_PLAN_CD        = this.txtChutePlanCD.Text.Trim();
                this.NEW_PLAN_NM        = this.txtChutePlanNM.Text.Trim();
                this.CHUTE_PLAN_CD      = liSelectedPlanList[0].PLAN_CD;

                this.Close();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 닫기 버튼 클릭
        /// <summary>
        /// 닫기 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> 폼 닫기 버튼 (이미지) 클릭
        /// <summary>
        /// 폼 닫기 버튼 (이미지) 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFormClose_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region > 그리드 이벤트
        private void SelectorColumnBehavior_SelectorCellCheked(object sender, Modules.Ctrl.SelectorCellChekedEventArgs e)
        {
            try
            {
                var selectedRowData = e.RowData.Row as ChutePlanHeaderMgmt;

                if (this.ChutePlanHeaderList.Where(p => p.IsSelected).Count() > 1)
                {
                    selectedRowData.IsSelected = false;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~P1008_SRT_01P()
        // {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion

        
    }
}
