using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.Control.Ctrl
{
    public class GridColumnEventInfo : DependencyObject
    {
        public static readonly DependencyProperty GridColumnEventProperty = DependencyProperty.Register(
                                                             "GridColumnEvent",
                                                             typeof(RoutedEventHandler),
                                                             typeof(GridColumnEventInfo),
                                                           new PropertyMetadata(null));

        public RoutedEventHandler GridColumnEvent
        {
            get { return (RoutedEventHandler)GetValue(GridColumnEventProperty); }
            set { SetValue(GridColumnEventProperty, value); }
        }

        public static readonly DependencyProperty FieldNameProperty = DependencyProperty.Register(
                                                            "FieldName",
                                                            typeof(string),
                                                            typeof(GridColumnEventInfo),
                                                          new PropertyMetadata(null));

        public string FieldName
        {
            get { return (string)GetValue(GridColumnEventProperty); }
            set { SetValue(GridColumnEventProperty, value); }
        }
    }
}
