using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART.WCS.Control.Views
{
    /// <summary>
    /// SWCS101_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SWCS101_01P : Window, IDisposable
    {
        #region ▩ Delegate
        public delegate void SearchResultHandler(string _strCode, string _strName);
        public event SearchResultHandler SearchResult;
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// Base클래스 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 고객사 코드
        /// </summary>
        private string g_strCstCD = string.Empty;
        #endregion

        #region ▩ Dependency 정의
        /// <summary>
        /// 고객사 리스트 조회
        /// </summary>
        public static readonly DependencyProperty CstListProperty = DependencyProperty.Register("CstList",
                                        typeof(ObservableCollection<SMART.WCS.Control.DataMembers.PopupCst>),
                                                typeof(SWCS101_01P), new PropertyMetadata(new ObservableCollection<SMART.WCS.Control.DataMembers.PopupCst>()));

        public ObservableCollection<SMART.WCS.Control.DataMembers.PopupCst> CstList
        {
            get { return (ObservableCollection<SMART.WCS.Control.DataMembers.PopupCst>)GetValue(CstListProperty); }
            set { SetValue(CstListProperty, value); }
        }
        #endregion

        #region ▩ 생성자
        public SWCS101_01P()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_strCstCode">고객사 코드</param>
        public SWCS101_01P(string _strCstCode)
        {
            InitializeComponent();

            // 폼명을 설정한다.
            this.Name = this.BaseClass.GetResourceValue("CST");

            // 고객사 코드
            this.g_strCstCD = _strCstCode;

            // 이벤트 초기화
            this.InitEvent();

            this.btnSearch.IsEnabled    = true;
            this.btnSearch.Cursor       = Cursors.Hand;

            this.txtCST_CD.Focus();
        }
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            this.Loaded += SWCS101_01P_Loaded;

            // 버튼 클릭 이벤트
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 창 종료 버튼 클릭 이벤트
            btnFormClose.Click += BtnFormClose_Click;

            // 텍스트박스 (고객사코드, 고객사명) Keydown 이벤트
            this.txtCST_CD.KeyDown += TxtCST_CD_KeyDown;
            this.txtCST_NM.KeyDown += TxtCST_NM_KeyDown;
        }
        #endregion
        #endregion

        #region > 데이터 관련
        private async Task<DataSet> GetCOMM_CST_POPUP_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1011.SP_CST_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_CST_LIST", "O_RSLT" };

            var strCstCD            = this.txtCST_CD.Text.Trim();       // 고객사 코드
            var strCstNM            = this.txtCST_NM.Text.Trim();       // 고객사 명
            var strUseYN            = "Y";                              // 사용 여부
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CST_CD",           strCstCD);          // 고객사 코드
            dicInputParam.Add("P_CST_NM",           strCstNM);          // 고객사 명
            dicInputParam.Add("P_USE_YN",           strUseYN);          // 사용여부
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 화면 로드 이벤트
        /// <summary>
        /// 화면 로드 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SWCS101_01P_Loaded(object sender, RoutedEventArgs e)
        {
            //var owner = Application.Current.MainWindow;

            //#region 창을 center로 위치 변경
            //double boundWidth = 0;
            //double boundHeight = 0;

            //if (WindowStartupLocation == WindowStartupLocation.CenterScreen)
            //{
            //    boundWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            //    boundHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            //    this.Left = boundWidth / 2 - this.Width / 2;
            //    this.Top = boundHeight / 2 - this.Height / 2;
            //}
            //else if (WindowStartupLocation == WindowStartupLocation.CenterOwner)
            //{
            //    if (owner != null)
            //    {
            //        var _bound = owner.RestoreBounds;

            //        boundWidth = _bound.Width;
            //        boundHeight = _bound.Height;

            //        this.Left = (boundWidth / 2 - this.Width / 2) + _bound.Left;
            //        this.Top = (boundHeight / 2 - this.Height / 2) + _bound.Top;
            //    }
            //}
            //#endregion

            if (this.g_strCstCD.Length > 0)
            {
                this.txtCST_CD.Text = this.g_strCstCD;

                DataSet dsRtnValue = await this.GetCOMM_CST_POPUP_SEARCH();

                if (dsRtnValue.Tables[0].Rows.Count == 0)
                {
                    this.SearchResult(string.Empty, string.Empty);
                }
                else if (dsRtnValue.Tables[0].Rows.Count == 1)
                {
                    string strCstCD = dsRtnValue.Tables[0].Rows[0]["CST_CD"].ToString();
                    string strCstNM = dsRtnValue.Tables[0].Rows[0]["CST_NM"].ToString();


                    this.SearchResult(strCstCD, strCstNM);

                    this.Close();
                }
                else
                {
                    this.CstList = new ObservableCollection<SMART.WCS.Control.DataMembers.PopupCst>();
                    this.CstList.ToObservableCollection(dsRtnValue.Tables[0]);

                    this.gridFirst.ItemsSource = this.CstList;
                }
            }
        }
        #endregion

        #region > 버튼 이벤트
        #region >> 조회 버튼 클릭 이벤트
        /// <summary>
        /// 조회 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataSet dsRtnValue = await this.GetCOMM_CST_POPUP_SEARCH();

            if (dsRtnValue.Tables[0].Rows.Count == 0)
            {
                // 조회 데이터가 없는 경우 메세지를 출력한다.
                // NO_INQ_DATA - 조회된 데이터가 없습니다.
                this.BaseClass.MsgInfo("NO_INQ_DATA");
            }
            else
            {
                this.CstList = new ObservableCollection<SMART.WCS.Control.DataMembers.PopupCst>();
                this.CstList.ToObservableCollection(dsRtnValue.Tables[0]);

                this.gridFirst.ItemsSource = this.CstList;
            }
        }
        #endregion

        #region >> 화면 종료 버튼 클릭 이벤트
        /// <summary>
        /// 화면 종료 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFormClose_Click(object sender, RoutedEventArgs e)
        {
            // 델리게이트를 이용하여 부모창으로 리턴값을 전달한다.
            this.SearchResult(string.Empty, string.Empty);
            this.Close();
        }
        #endregion
        #endregion

        #region > 텍스트박스 이벤트
        #region >> 고객사 코드 텍스트박스 Key Down 이벤트
        /// <summary>
        /// 고객사 텍스트박스 Key Down 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TxtCST_CD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DataSet dsRtnValue = await this.GetCOMM_CST_POPUP_SEARCH();

                if (dsRtnValue.Tables[0].Rows.Count == 0)
                {
                    // 조회 데이터가 없는 경우 메세지를 출력한다.
                    // NO_INQ_DATA - 조회된 데이터가 없습니다.
                    this.BaseClass.MsgInfo("NO_INQ_DATA");
                }
                else
                {
                    this.CstList = new ObservableCollection<SMART.WCS.Control.DataMembers.PopupCst>();
                    this.CstList.ToObservableCollection(dsRtnValue.Tables[0]);

                    this.gridFirst.ItemsSource = this.CstList;
                }
            }
        }
        #endregion

        #region >> 고객사명 텍스트박스 Key Down 이벤트
        /// <summary>
        /// 고객사명 텍스트박스 Key Down 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TxtCST_NM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DataSet dsRtnValue = await this.GetCOMM_CST_POPUP_SEARCH();

                if (dsRtnValue.Tables[0].Rows.Count == 0)
                {
                    // 조회 데이터가 없는 경우 메세지를 출력한다.
                    // NO_INQ_DATA - 조회된 데이터가 없습니다.
                    this.BaseClass.MsgInfo("NO_INQ_DATA");
                }
                else
                {
                    this.CstList = new ObservableCollection<SMART.WCS.Control.DataMembers.PopupCst>();
                    this.CstList.ToObservableCollection(dsRtnValue.Tables[0]);

                    this.gridFirst.ItemsSource = this.CstList;
                }
            }
        }
        #endregion
        #endregion

        #region > 그리드 이벤트
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

        #region >> 그리드 Row 더블클릭 이벤트
        /// <summary>
        /// 그리드 Row 더블클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvFirstGrid_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            TableView tableView                 = sender as TableView;
            TableViewHitInfo tableViewHitInfo   = e.HitInfo as TableViewHitInfo;
            object row                          = tableView.Grid.GetRow(tableViewHitInfo.RowHandle);

            // 고객사 코드
            string strCstCD = ((DataMembers.PopupCst)row).CST_CD;
            // 고객사 명
            string strCstName = ((DataMembers.PopupCst)row).CST_NM;

            if (strCstCD.Length > 0)
            {
                this.SearchResult(strCstCD, strCstName);
                this.Close();
            }
        }
        #endregion
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
        // ~SWCS101_01P()
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
