using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SMART.WCS.UI.EMS.Views.COM
{
    /// <summary>
    /// ECOM001_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ECOM001_01P : Window, IDisposable
    {
            #region ▩ Delegate
            public delegate void SearchResult(string _strCode, string _strName);
            public event SearchResult CustomerSearchResult;
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
        public ECOM001_01P()
        {
            try
            {
                InitializeComponent();

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                btnFormClose.Click += btnFormClose_Click;

                var profitFormatCondition = new FormatCondition()
                {
                    ValueRule = ConditionRule.Expression,
                    Expression = "[CLS_DEV] == 'A'",
                    FieldName = null,
                    Format = new DevExpress.Xpf.Core.ConditionalFormatting.Format()
                    {
                        Background = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0x7F, 0x50))
                    }
                };

                gridEqpView.FormatConditions.Add(profitFormatCondition);

                gridEqpView.ShowingEditor += (s, e) =>
                {
                    e.Cancel = true;
                };
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #region ▩ 속성
        /// <summary>
        /// Grid Data
        /// </summary>
        private ObservableCollection<EmsEqpList> CurrEqpList { get; set; }

        public string CurrEqpId { get; private set; }
        public string CurrEqpName { get; private set; }
        public string CurrPbsId { get; private set; }
        public string CurrPbsName { get; private set; }
        #endregion
        #endregion

        #region ▩ 함수
        #region 조회
        /// <summary>
        /// 조회버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchClick(object sender, MouseButtonEventArgs e)
        {
            SearchEqp();
            gridEqpView.ExpandAllNodes();
        }

        /// <summary>
        /// 조회 함수
        /// </summary>
        void SearchEqp()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD", this.BaseClass.CenterCD },
                    {"P_EQP_NM", tbxEqpName.Text.Trim()}
                };

                var strOutParam = new[] { "P_EMS_EQP_LIST" };
                string callProc = "PK_EMS_ECOM01_01P.SP_EMS_EQP_SEARCH";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        CurrEqpList = new ObservableCollection<EmsEqpList>();
                        CurrEqpList.ToObservableCollection(outData.Tables[0]);

                        gridEqp.ItemsSource = CurrEqpList;
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 조회

        #region 선택
        /// <summary>
        /// Grid Data 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridEqp_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int rowHandle = gridEqpView.GetRowHandleByMouseEventArgs(e);
                if (rowHandle == GridControl.InvalidRowHandle) { return; }

                if (gridEqp.IsGroupRowHandle(rowHandle)) { return; }// A group row has been double clicked

                EmsEqpList target = gridEqp.SelectedItem as EmsEqpList;

                if (null != target)
                {
                    CurrEqpId = target.EQP_ID;
                    CurrEqpName = target.EQP_NM;
                    CurrPbsId = target.PBS_ID;
                    CurrPbsName = target.PBS_NM;

                    this.Close();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion 선택

        #region Title Menu
        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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

        /// <summary>
        /// Close 이미지 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFormClose_Click(object sender, RoutedEventArgs e)
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
        #endregion Title Menu
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
        // ~ECOM001_01P()
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
