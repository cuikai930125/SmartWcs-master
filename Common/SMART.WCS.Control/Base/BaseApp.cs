using SMART.WCS.Control.Barcode;
using SMART.WCS.Control.DataMembers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.Control
{
    public class BaseApp : Application
    {
        public SerialScannerConnect SerialScanner;

        public BaseInfo BASE_INFO { get; set; }
        private SMART.WCS.Common.BaseClass BaseClass = new SMART.WCS.Common.BaseClass();
        public List<ContentLabelInfo> g_LangTable { get; set; }

        public BaseApp()
        {
            g_LangTable = new List<ContentLabelInfo>();

            this.Startup += App_Startup;
            this.DispatcherUnhandledException += BaseApp_DispatcherUnhandledException;
            //App.CountryInfoChanged += App_CountryInfoChanged;
            // this.BaseInfoChanged += App_BaseInfoChanged;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //if (CoreScanner != null)
            //{
            //    CoreScanner.CloseCoreScanner();
            //}

            SerialScanner.CloseCoreScanner();

            base.OnExit(e);
        }

        private void BaseApp_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //// BaseInfo -> db_conn_info 파라메터 추가
            //// 2018-11-01
            //// 추성호
            //this.BaseClass.Error(e.Exception, this.BASE_INFO.db_conn_info);
            e.Handled = true;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            DevExpress.Xpf.Grid.GridControl.AllowInfiniteGridSize = true;

            // 기준 정보
            BASE_INFO = new BaseInfo();

            // 바코드 스캐너 연결
            this.SerialScanner = new SerialScannerConnect();

            ////CoreScanner = new ScannerConnect();

            ///// db에서 가져오는 작업이 필요한 경우 별도 수정
            //CountryInfoItems = new List<BaseInfo>
            //    {
            //        new BaseInfo {
            //        country_cd      = "KR",
            //        country_name    = "한국어"
            //        },
            //        new BaseInfo {
            //        country_cd      = "EN",
            //        country_name    = "영어"
            //        }
            //        ,
            //        new BaseInfo {
            //        country_cd      = "CN",
            //        country_name    = "중국어"
            //        }
            //    };
        }

        private void App_BaseInfoChanged(object sender, EventArgs e)
        {
            //if (App.baseInfo != null)
            //{
            //    switch (App.baseInfo.country_cd)
            //    {
            //        case "KR":
            //            AppProperties.DefaultFontProperty = new FontFamily("맑은 고딕");
            //            break;
            //        case "EN":
            //            AppProperties.DefaultFontProperty = new FontFamily("Arial");
            //            break;
            //        case "CN":
            //            AppProperties.DefaultFontProperty = new FontFamily("Arial");
            //            break;
            //        default:
            //            break;
            //    }

            //    // 먼저 열려있는 창은 폰트를 변경해준다
            //    foreach (var item in App.Current.Windows)
            //    {
            //        (item as Window).FontFamily = AppProperties.DefaultFontProperty;
            //    }

            //    System.Windows.MessageBox.Show("지역코드 변경 : " + App.baseInfo.country_name);
            //}
        }

        private void App_CountryInfoChanged(object sender, EventArgs e)
        {
            //if (App.CountryInfo != null)
            //{
            //    switch (App.CountryInfo.Code)
            //    {
            //        case "KR":
            //            AppProperties.DefaultFontProperty = new FontFamily("맑은 고딕");
            //            break;
            //        case "EN":
            //            AppProperties.DefaultFontProperty = new FontFamily("Arial");
            //            break;
            //        case "CN":
            //            AppProperties.DefaultFontProperty = new FontFamily("Arial");
            //            break;
            //    }

            //    // 먼저 열려있는 창은 폰트를 변경해준다
            //    foreach (var item in App.Current.Windows)
            //    {
            //        (item as Window).FontFamily = AppProperties.DefaultFontProperty;
            //    }

            //    System.Windows.MessageBox.Show("지역코드 변경 : " + App.CountryInfo.Name);
            //}
        }
    }
}
