using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.Modules.Behaviors
{
    public class GridBehavior : Behavior<System.Windows.Controls.Grid>
    {
        public GridBehavior()
        {

        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            double _itemWidth = double.NaN;

            foreach (DependencyObject child in LogicalTreeHelper.GetChildren(this.AssociatedObject).OfType<DependencyObject>())
            {
                _itemWidth = Math.Max(_itemWidth, (double)child.GetValue(System.Windows.Controls.Control.WidthProperty));
            }

            foreach (DependencyObject child in LogicalTreeHelper.GetChildren(this.AssociatedObject).OfType<DependencyObject>())
            {
                child.SetValue(System.Windows.Controls.Control.WidthProperty, _itemWidth);
            }
        }
    }
}
