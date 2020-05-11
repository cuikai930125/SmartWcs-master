using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.Utility
{
    public class ColorTypeToBGConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush rtnBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#EAEAEA"));  // Default

            if (value != null)
            {
                switch (value.ToString())
                {
                    case "A":
                        rtnBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF00DD"));
                        break;
                    case "B":
                        rtnBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#8041D9"));
                        break;
                    case "C":
                        rtnBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00D8FF"));
                        break;
                    case "D":
                        rtnBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1DDB16"));
                        break;
                    case "E":
                        rtnBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE400"));
                        break;
                    default:
                        break;
                }
            }

            return rtnBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
