using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SMART.WCS.Control.Converters
{
    public class DefaultBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DevExpress.Utils.DefaultBoolean editability = DevExpress.Utils.DefaultBoolean.False;
            bool ifClosedDate = (bool)value;
            if (ifClosedDate)
                editability = DevExpress.Utils.DefaultBoolean.True;
            return editability;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Hop ColumnEditabilityConverter ConvertBack");
        }
    }
}
