using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SMART.WCS.UI.EMS.Views.COM
{
    public class BandTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DevExpress.Xpf.Grid.GridControlBand band = (DevExpress.Xpf.Grid.GridControlBand)item;
            if (band.Columns.Count == 1)
            {
                return (DataTemplate)((System.Windows.Controls.Control)container).FindResource("SingleColumnBandTemplate");
            }
            return (DataTemplate)((System.Windows.Controls.Control)container).FindResource("MultiColumnBandTemplate");
        }
    }
}
