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
using SMART.WCS.Control.Modules.Interface;

namespace SMART.WCS.UI.Optimization.Views.QPS
{
    /// <summary>
    /// Q1008_QPS.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Q1008_QPS : UserControl, Control.Modules.Interface.TabCloseInterface
    {
        public Q1008_QPS()
        {
            InitializeComponent();
        }

        bool TabCloseInterface.TabClosing()
        {
            return true;
        }
    }
}
