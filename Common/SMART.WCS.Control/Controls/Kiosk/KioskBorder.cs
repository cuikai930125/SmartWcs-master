using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace SMART.WCS.Control.Kiosk
{
    [ContentPropertyAttribute("Content")]
    public class KioskBorder : System.Windows.Controls.ContentControl
    {
        #region ▩ 생성자
        static KioskBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KioskBorder), new FrameworkPropertyMetadata(typeof(KioskBorder)));
        }
        #endregion

        #region ▩ 재정의
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        #endregion
    }
}
