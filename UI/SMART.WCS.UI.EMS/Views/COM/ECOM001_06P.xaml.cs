using DevExpress.Mvvm.Native;
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
    /// ECOM001_06P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ECOM001_06P : Window, IDisposable
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
        public ECOM001_06P()
        {
            InitializeComponent();
        }

        public ECOM001_06P(string _strTarget, string _strName)
        {
            try
            {

                InitializeComponent();

                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                btnFormClose.Click += btnFormClose_Click;

                gridEqpUseLstView.ShowingEditor += (s, e) =>
                {
                    e.Cancel = true;
                };

                CurrPart = _strTarget;

                tbxPartId.Text = _strTarget;
                tbxPartNm.Text = _strName;

                this.Loaded += ECOM001_06P_Loaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 속성
        private ObservableCollection<EmsEqpList> CurrEqpList { get; set; }

        private string CurrPart { get; set; }
        #endregion

        #region ▩ 함수
        #region 조회
        /// <summary>
        /// 조회
        /// </summary>
        void SearchPbs()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_PART_ID",       CurrPart}
                };

                var strOutParam = new[] { "P_EMS_EQP_LIST" };
                string callProc = "PK_EMS_ECOM01_06P.SP_EMS_USE_EQP_SEARCH";

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

                        gridEqpUseLst.ItemsSource = CurrEqpList;

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 조회

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
                //this.BaseClass.Error(err);
            }
        }

        #endregion Title Menu
        #endregion

        #region ▩ 이벤트
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
                //this.BaseClass.Error(err);
            }
        }

        // <summary>
        /// 화면 오픈시 선행 작업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ECOM001_06P_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ECOM001_06P_Loaded;

            SearchPbs();
        }
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
        // ~ECOM001_06P()
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
