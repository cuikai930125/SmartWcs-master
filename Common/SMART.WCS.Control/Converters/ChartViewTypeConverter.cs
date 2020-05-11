using DevExpress.Xpf.Charts.RangeControlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SMART.WCS.Control.Converters
{
    public class ChartViewTypeConverter : IValueConverter
    {
        public enum ChartViewType
        {
            Area,
            Bar,
            Line
        }

        RangeControlClientView Parse(ChartViewType type)
        {
            if (type == ChartViewType.Area)
                return new RangeControlClientAreaView();
            if (type == ChartViewType.Bar)
                return new RangeControlClientBarView();
            if (type == ChartViewType.Line)
                return new RangeControlClientLineView();
            return null;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ChartViewType)
                return Parse((ChartViewType)value);
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
