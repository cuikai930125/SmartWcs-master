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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMART.WCS.UI.COMMON.Views.SYS_MGMT
{
    /// <summary>
    /// C1097.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1097 : UserControl
    {

        private bool g_isLoaded = false;
        public C1097()
        {
            InitializeComponent();

            this.InitEvent();
        }

        private void InitEvent()
        {
            this.Loaded += C1097_Loaded;

            this.Unloaded += C1097_Unloaded;
        }

        private void C1097_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception err)
            {

            }
        }

        private void C1097_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (this.g_isLoaded == false)
                //{

                //    this.g_isLoaded = true;
                //}

                //this.Loaded -= C1097_Loaded;
            }
            catch (Exception err)
            {

            }
        }
    }
}
