using SMART.WCS.Common_Etc;
using System;
using System.Collections.Generic;
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

namespace SMART.WCS.Kiosk
{
    /// <summary>
    /// MessageTest.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageTest : Window
    {
        BaseClass BaseClass = new BaseClass();

        public MessageTest()
        {
            InitializeComponent();
        }

        private void btnTest1_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //HelperClass.MsgInfo("ERR_EXISTS_NO_SAVE");
                //HelperClass.MsgError("ERR_EXISTS_NO_SAVE");
                HelperClass.MsgQuestion("ERR_EXISTS_NO_SAVE");
                var isRtnValue = HelperClass.BUTTON_CONFIRM_YN;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
    }
}
