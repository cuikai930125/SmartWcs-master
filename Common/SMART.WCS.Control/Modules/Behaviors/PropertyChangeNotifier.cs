using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SMART.WCS.Modules.Behaviors
{
    public class PropertyChangeNotifier : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(PropertyChangeNotifier), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnValuePropertyChanged)));

        static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyChangeNotifier)d).OnValueChanged(e);
        }

        WeakReference _propertySource;
        public event EventHandler ValueChanged;

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public DependencyObject PropertySource
        {
            get
            {
                return _propertySource != null && _propertySource.IsAlive ? (DependencyObject)_propertySource.Target : null;
            }
        }

        public PropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property)
        {
            if (propertySource == null) { throw new ArgumentNullException("propertySource"); }
            if (property == null) { throw new ArgumentNullException("property"); }

            _propertySource     = new WeakReference(propertySource);
            Binding binding     = new Binding();
            binding.Path        = new PropertyPath(property);
            binding.Mode        = BindingMode.OneWay;
            binding.Source      = propertySource;
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }

        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ValueChanged != null) { ValueChanged(this, EventArgs.Empty); }
        }
        public void Dispose()
        {
            ValueChanged = null;
            BindingOperations.ClearBinding(this, ValueProperty);
        }
    }
}
