using System.Windows;

namespace SMART.WCS.Control.Kiosk
{
    public class KioskButton : System.Windows.Controls.Button
    {
        #region ▩ 생성자
        static KioskButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KioskButton), new FrameworkPropertyMetadata(typeof(KioskButton)));
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
