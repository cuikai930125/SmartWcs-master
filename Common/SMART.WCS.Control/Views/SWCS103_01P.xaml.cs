using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
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

namespace SMART.WCS.Control.Views
{
    /// <summary>
    /// SWCS103_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SWCS103_01P : Window, IDisposable
    {
        #region ▩ Delegate
        public delegate void SearchResultHandler(string _strCode, string _strName, string _strCstCode, string _strCstName);
        public event SearchResultHandler SearchResult;
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// BaseClass 선언
        /// </summary>
        private BaseClass BaseClass = new BaseClass();
        #endregion

        #region ▩ Dependency 선언
        /// <summary>
        /// SKU 리스트 조회
        /// </summary>
        public static readonly DependencyProperty SkuListProperty = DependencyProperty.Register("SkuList"
                                , typeof(System.Collections.ObjectModel.ObservableCollection<DataMembers.PopupSku>)
                                , typeof(SWCS103_01P), new PropertyMetadata(new ObservableCollection<DataMembers.PopupSku>()));

        public ObservableCollection<DataMembers.PopupSku> SkuList
        {
            get { return (ObservableCollection<DataMembers.PopupSku>)GetValue(SkuListProperty); }
            set { SetValue(SkuListProperty, value); }
        }
        #endregion

        #region ▩ 생성자
        public SWCS103_01P()
        {
            InitializeComponent();
        }

        public SWCS103_01P(string _strSkuCD, string _strCstCD, string _strCstNM)
        {
            try
            { 
                InitializeComponent();

                this.txtSkuCD.Text          = _strSkuCD;        // SKU 코드
                this.txtCstCD.Text          = _strCstCD;        // 고객사 코드
                this.txtCstNM.Text          = _strCstNM;        // 고객사명

                //// 그리드 안에서 버튼으로 호출된 경우 고객사코드와 고객사명 TextBox를 Readonly 처리한다.
                //this.txtCstCD.IsReadOnly    = _bReadonly;       // 고객사 코드
                //this.txtCstNM.IsReadOnly    = _bReadonly;       // 고객사명

                //if (_bReadonly == true)
                //{
                //    this.txtCstCD.Background    = this.BaseClass.ConvertStringToMediaBrush("#CCCCCC");
                //    this.txtCstNM.Background    = this.BaseClass.ConvertStringToMediaBrush("#CCCCCC");
                //}

                this.btnSearch.Cursor       = Cursors.Hand;
                this.tvFirstGrid.Cursor     = Cursors.Hand;

                this.InitEvent();

                this.txtSkuCD.Focus();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
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
            this.Loaded += SWCS102_01P_Loaded;

            // 조회 버튼 클릭 이벤트
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 종료 버튼 클릭 이벤트
            this.btnFormClose.PreviewMouseLeftButtonUp += BtnFormClose_PreviewMouseLeftButtonUp;
            // SKU 코드 텍스트박스 KeyDown 이벤트
            this.txtSkuCD.KeyDown += TxtSkuCD_KeyDown;
            // SKU명 텍스트박스 KeyDown 이벤트
            this.txtSkuNM.KeyDown += TxtSkuNM_KeyDown;
        }
        #endregion
        #endregion

        #region > 데이터 관련
        private async Task<DataSet> GetCOMM_SKU_POPUP_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1011.SP_CST_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_CST_LIST", "O_RSLT" };

            var strShipCD           = this.txtSkuCD.Text.Trim();        // SKU 코드
            var strShipNM           = this.txtSkuNM.Text.Trim();        // SKU명
            var strCstCD            = this.txtCstCD.Text.Trim();        // 고객사 코드
            var strCstNM            = this.txtCstNM.Text.Trim();        // 고객사 명
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
        private async void SWCS102_01P_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var _owner = Application.Current.MainWindow;

                #region 창을 center로 위치 변경
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
                //    if (_owner != null)
                //    {
                //        var _bound = _owner.RestoreBounds;

                //        boundWidth = _bound.Width;
                //        boundHeight = _bound.Height;

                //        this.Left = (boundWidth / 2 - this.Width / 2) + _bound.Left;
                //        this.Top = (boundHeight / 2 - this.Height / 2) + _bound.Top;
                //    }
                //}
                #endregion

                if (this.txtSkuCD.Text.Trim().Length == 0) { return; }

                DataSet dsRtnValue = await this.GetCOMM_SKU_POPUP_SEARCH();

                if (dsRtnValue.Tables[0].Rows.Count == 1)
                {
                    var strSkuCD        = dsRtnValue.Tables[0].Rows[0]["SKU_CD"].ToString();    // SKU 코드
                    var strSkuNM        = dsRtnValue.Tables[0].Rows[0]["SKU_NM"].ToString();    // SKU명
                    var strCstCD        = dsRtnValue.Tables[0].Rows[0]["CST_CD"].ToString();    // 고객사 코드
                    var strCstNM        = dsRtnValue.Tables[0].Rows[0]["CST_NM"].ToString();

                    // 부모창 (코드파인더 UserControl)로 값 전송
                    this.SearchResult(strSkuCD, strSkuNM, strCstCD, strCstNM);
                    this.Close();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
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
            DataSet dsRtnValue = await this.GetCOMM_SKU_POPUP_SEARCH();

            if (dsRtnValue.Tables[0].Rows.Count == 0)
            {
                // 조회 데이터가 없는 경우 메세지를 출력한다.
                // NO_INQ_DATA - 조회된 데이터가 없습니다.
                this.BaseClass.MsgInfo("NO_INQ_DATA");
            }
            else
            {
                this.SkuList = new ObservableCollection<DataMembers.PopupSku>();
                this.SkuList.ToObservableCollection(dsRtnValue.Tables[0]);

                this.gridFirst.ItemsSource = this.SkuList;
            }
        }
        #endregion

        #region >> 종료 버튼 클릭 이벤트
        /// <summary>
        /// 종료 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFormClose_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 부모창으로 데이터를 전달한다.
            this.SearchResult(string.Empty, string.Empty, string.Empty, string.Empty);
            this.Close();
        }
        #endregion
        #endregion

        #region > 텍스트박스 이벤트
        #region >> SK 코드 텍스트박스 KeyDown 이벤트
        /// <summary>
        /// SKU 코드 텍스트박스 KeyDown 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TxtSkuCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DataSet dsRtnValue = await this.GetCOMM_SKU_POPUP_SEARCH();

                if (dsRtnValue.Tables[0].Rows.Count == 0)
                {
                    // 조회 데이터가 없는 경우 메세지를 출력한다.
                    // NO_INQ_DATA - 조회된 데이터가 없습니다.
                    this.BaseClass.MsgInfo("NO_INQ_DATA");
                }
                else
                {
                    this.SkuList = new ObservableCollection<DataMembers.PopupSku>();
                    this.SkuList.ToObservableCollection(dsRtnValue.Tables[0]);

                    this.gridFirst.ItemsSource = this.SkuList;
                }
            }
        }
        #endregion

        #region >> SKU명 텍스트박스 KeyDown 이벤트
        /// <summary>
        /// SKU명 텍스트박스 KeyDown 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TxtSkuNM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DataSet dsRtnValue = await this.GetCOMM_SKU_POPUP_SEARCH();

                if (dsRtnValue.Tables[0].Rows.Count == 0)
                {
                    // 조회 데이터가 없는 경우 메세지를 출력한다.
                    // NO_INQ_DATA - 조회된 데이터가 없습니다.
                    this.BaseClass.MsgInfo("NO_INQ_DATA");
                }
                else
                {
                    this.SkuList = new ObservableCollection<DataMembers.PopupSku>();
                    this.SkuList.ToObservableCollection(dsRtnValue.Tables[0]);

                    this.gridFirst.ItemsSource = this.SkuList;
                }
            }
        }
        #endregion
        #endregion

        #region > 그리드 이벤트
        private void TvFirstGrid_RowDoubleClick(object sender, DevExpress.Xpf.Grid.RowDoubleClickEventArgs e)
        {
            TableView tableView                 = sender as TableView;
            TableViewHitInfo tableViewHitInfo   = e.HitInfo as TableViewHitInfo;
            object row                          = tableView.Grid.GetRow(tableViewHitInfo.RowHandle);

            var strShipCD   = ((DataMembers.PopupSku)row).SKU_CD;       // SKU 코드
            var strShipNM   = ((DataMembers.PopupSku)row).SKU_NM;       // SKU명
            var strCstCD    = ((DataMembers.PopupSku)row).CST_CD;       // 고객사 코드
            var strCstName  = ((DataMembers.PopupSku)row).CST_NM;       // 고객사 명


            if (strShipCD.Length > 0)
            {
                this.SearchResult(strShipCD, strShipNM, strCstCD, strCstName);
                this.Close();
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
        // ~SWCS103_01P()
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
