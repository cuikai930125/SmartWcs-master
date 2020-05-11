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
    /// 거래처 공통팝업
    /// </summary>
    public partial class SWCS102_01P : Window, IDisposable
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
        /// 거래처 리스트 조회
        /// </summary>
        public static readonly DependencyProperty ShipListProperty = DependencyProperty.Register("ShipList"
                                ,   typeof(ObservableCollection<DataMembers.PopupShip>)
                                ,   typeof(SWCS102_01P), new PropertyMetadata(new ObservableCollection<DataMembers.PopupShip>()));

        public ObservableCollection<DataMembers.PopupShip> ShipList
        {
            get { return (ObservableCollection<DataMembers.PopupShip>)GetValue(ShipListProperty); }
            set { SetValue(ShipListProperty, value); }
        }
        #endregion

        #region ▩ 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public SWCS102_01P()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_strShipCD">거래처 코드</param>
        /// <param name="_strCstCD">거래처명</param>
        /// <param name="_strCstNM">고객사 코드</param>
        /// <param name="_bReadonly">고객사명</param>
        public SWCS102_01P(string _strShipCD, string _strCstCD, string _strCstNM)
        {
            try
            { 
                InitializeComponent();

                this.txtShipCD.Text         = _strShipCD;       // 거래처 코드
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

                this.txtCstCD.Focus();
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
            // 거래처 코드 텍스트박스 KeyDown 이벤트
            this.txtShipCD.KeyDown += TxtShipCD_KeyDown;
            // 거래처명 텍스트박스 KeyDown 이벤트
            this.txtShipNM.KeyDown += TxtShipNM_KeyDown;
        }


        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetCOMM_SHIP_POPUP_SEARCH - 거래처를 조회한다.
        /// <summary>
        /// 거래처를 조회한다.
        /// </summary>
        /// <returns></returns>
        private async Task<DataSet> GetCOMM_SHIP_POPUP_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1011.SP_SHIP_TO_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_SHIP_TO_LIST", "O_RSLT" };

            var strShipCD           = this.txtShipCD.Text.Trim();       // 거래처 코드
            var strShipNM           = this.txtShipNM.Text.Trim();       // 거래처명
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

                if (this.txtShipCD.Text.Trim().Length == 0) { return; }

                DataSet dsRtnValue = await this.GetCOMM_SHIP_POPUP_SEARCH();

                if (dsRtnValue.Tables[0].Rows.Count == 1)
                {
                    var strShipCD           = dsRtnValue.Tables[0].Rows[0]["SHIP_CD"].ToString();   // 거래처 코드
                    var strShipNM           = dsRtnValue.Tables[0].Rows[0]["SHIP_NM"].ToString();   // 거래처명
                    var strCstCD            = dsRtnValue.Tables[0].Rows[0]["CST_CD"].ToString();    // 고객사 코드
                    var strCstNM            = dsRtnValue.Tables[0].Rows[0]["CST_NM"].ToString();

                    // 부모창 (코드파인더 UserControl)로 값 전송
                    this.SearchResult(strShipCD, strShipNM, strCstCD, strCstNM);
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
            DataSet dsRtnValue = await this.GetCOMM_SHIP_POPUP_SEARCH();

            if (dsRtnValue.Tables[0].Rows.Count == 0)
            {
                // 조회 데이터가 없는 경우 메세지를 출력한다.
                // NO_INQ_DATA - 조회된 데이터가 없습니다.
                this.BaseClass.MsgInfo("NO_INQ_DATA");
            }
            else
            {
                this.ShipList = new ObservableCollection<DataMembers.PopupShip>();
                this.ShipList.ToObservableCollection(dsRtnValue.Tables[0]);

                this.gridFirst.ItemsSource = this.ShipList;
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
        #region >> 거래처 코드 텍스트박스 KeyDown 이벤트
        /// <summary>
        /// 거래처 코드 텍스트박스 KeyDown 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TxtShipCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DataSet dsRtnValue = await this.GetCOMM_SHIP_POPUP_SEARCH();

                if (dsRtnValue.Tables[0].Rows.Count == 0)
                {
                    // 조회 데이터가 없는 경우 메세지를 출력한다.
                    // NO_INQ_DATA - 조회된 데이터가 없습니다.
                    this.BaseClass.MsgInfo("NO_INQ_DATA");
                }
                else
                {
                    this.ShipList = new ObservableCollection<DataMembers.PopupShip>();
                    this.ShipList.ToObservableCollection(dsRtnValue.Tables[0]);

                    this.gridFirst.ItemsSource = this.ShipList;
                }
            }
        }
        #endregion

        #region >> 거래처명 텍스트박스 KeyDown 이벤트
        private async void TxtShipNM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DataSet dsRtnValue = await this.GetCOMM_SHIP_POPUP_SEARCH();

                if (dsRtnValue.Tables[0].Rows.Count == 0)
                {
                    // 조회 데이터가 없는 경우 메세지를 출력한다.
                    // NO_INQ_DATA - 조회된 데이터가 없습니다.
                    this.BaseClass.MsgInfo("NO_INQ_DATA");
                }
                else
                {
                    this.ShipList = new ObservableCollection<DataMembers.PopupShip>();
                    this.ShipList.ToObservableCollection(dsRtnValue.Tables[0]);

                    this.gridFirst.ItemsSource = this.ShipList;
                }
            }
        }
        #endregion
        #endregion

        #region > 그리드 이벤트
        private void TvFirstGrid_RowDoubleClick(object sender, DevExpress.Xpf.Grid.RowDoubleClickEventArgs e)
        {
            TableView tableView = sender as TableView;
            TableViewHitInfo tableViewHitInfo = e.HitInfo as TableViewHitInfo;
            object row = tableView.Grid.GetRow(tableViewHitInfo.RowHandle);

            var strShipCD = ((DataMembers.PopupShip)row).SHIP_CD;     // 거래처 코드
            var strShipNM = ((DataMembers.PopupShip)row).SHIP_NM;     // 거래처명
            var strCstCD = ((DataMembers.PopupShip)row).CST_CD;      // 고객사 코드
            var strCstName = ((DataMembers.PopupShip)row).CST_NM;      // 고객사 명


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
        // ~SWCS102_01P()
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
