using SMART.WCS.Common_Etc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.Kiosk
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        BaseClass BaseClass = new BaseClass();

        public App()
        {
            //this.Startup += App_Startup;

            //this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            //this.Dispatcher.UnhandledExceptionFilter += Dispatcher_UnhandledExceptionFilter;

            //Run();
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            //try
            //{
            //    string[] arrStrArgs = Environment.GetCommandLineArgs();

            //    //MessageBox.Show(arrStrArgs.Length.ToString());

            //    this.BaseClass.MsgQuestion("22222", BaseEnumClass.CodeMessage.MESSAGE);

            //    switch (arrStrArgs.Length)
            //    {
            //        case 1:
            //            {
            //                this.BaseClass.CompanyCode = this.BaseClass.GetAppSettings("CompanyCode");
            //                // 데이터베이스 종류를 설정한다.
            //                this.BaseClass.MainDatabase = this.BaseClass.GetAppSettings("MainDatabase");

            //                StartupUri = new System.Uri("Login.xaml", UriKind.Relative);
            //            }
            //            break;

            //        case 7:
            //            {
            //                /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
            //                 * Argument
            //                 * [0] : 사용자 ID
            //                 * [1] : 사용자명
            //                 * [2] : 권한코드
            //                 * [3] : 센터코드
            //                 * [4] : 센터명
            //                 * [5] : 언어코드
            //                 *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/
            //                this.BaseClass.UserID       = arrStrArgs[1];    // 사용자 ID
            //                this.BaseClass.UserName     = arrStrArgs[2];    // 사용자 명
            //                this.BaseClass.RoleCode     = arrStrArgs[3];    // 권한코드
            //                this.BaseClass.CenterCD     = arrStrArgs[4];    // 센터코드
            //                this.BaseClass.CenterName   = arrStrArgs[5];    // 센터명
            //                this.BaseClass.CountryCode  = arrStrArgs[6];    // 언어코드

            //                StartupUri = new System.Uri("MainWindow.xaml", UriKind.Relative);
            //            }
            //            break;
            //    }
            //}
            //catch // (Exception err)
            //{

            //}
        }

        private void Dispatcher_UnhandledExceptionFilter(object sender, System.Windows.Threading.DispatcherUnhandledExceptionFilterEventArgs e)
        {
            //e.RequestCatch = true;
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //e.Handled = true;
        }

        ////Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                //base.OnStartup(e);
            }
            catch //(Exception err)
            {

            }
        }
    }
}
