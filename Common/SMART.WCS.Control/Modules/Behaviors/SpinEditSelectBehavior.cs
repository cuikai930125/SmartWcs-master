using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.Modules.Behaviors
{
    public class SpinEditSelectBehavior : Behavior<DevExpress.Xpf.Editors.SpinEdit>
    {
        private bool _IsGetFocus = false;

        public SpinEditSelectBehavior()
        {

        }

        protected override void OnAttached()
        {
            base.OnAttached();

            _IsGetFocus = this.AssociatedObject.Focus();
            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
            this.AssociatedObject.MouseUp += AssociatedObject_MouseUp;
        }

        private void AssociatedObject_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (this.AssociatedObject.Parent != null)
                {
                    if (this.AssociatedObject.Parent is System.Windows.Controls.Control)
                    {
                        (this.AssociatedObject.Parent as System.Windows.Controls.Control).Focus();
                    }
                }
            }
        }

        private void AssociatedObject_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_IsGetFocus)
            {
                this.AssociatedObject.SelectAll();
                _IsGetFocus = false;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.MouseUp -= AssociatedObject_MouseUp;
        }
    }
}
