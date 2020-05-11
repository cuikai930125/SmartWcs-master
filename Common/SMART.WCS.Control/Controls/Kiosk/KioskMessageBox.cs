using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SMART.WCS.Control.Kiosk
{
    [ContentPropertyAttribute("Content")]
    [TemplatePart(Name = PART_BTN_OK, Type = typeof(Button))]
    [TemplatePart(Name = PART_BTN_CANCEL, Type = typeof(Button))]
    public class KioskMessageBox : System.Windows.Window
    {
        #region ▩ 전역변수

        private const string PART_BTN_OK        = "Part_btnOK";
        private const string PART_BTN_CANCEL    = "Part_btnCancel";

        private Button g_BtnOK          = null;
        private Button g_BtnCancel      = null;
        #endregion

        #region ▩ 생성자
        static KioskMessageBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KioskMessageBox), new FrameworkPropertyMetadata(typeof(KioskMessageBox)));
        }
        #endregion

        #region ▩ 재정의
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.Loaded += KioskMessageBox_Loaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.g_BtnCancel    = GetTemplateChild("Part_btnCancel") as Button;
            this.g_BtnOK        = GetTemplateChild("Part_btnOk") as Button;

            if (this.g_BtnCancel != null)
            {
                this.g_BtnCancel.PreviewMouseLeftButtonUp += G_BtnCancel_PreviewMouseLeftButtonUp;
            }

            if (this.g_BtnOK!= null)
            {
                this.g_BtnOK.PreviewMouseLeftButtonUp += G_BtnOk_PreviewMouseLeftButtonUp;
            }
        }
        #endregion

        #region ▩ 객체 선언 및 속성 정의
        public static readonly DependencyProperty MessageTextProperty =
                                    DependencyProperty.Register("MessageText", typeof(string), typeof(KioskMessageBox),
                                                                    new FrameworkPropertyMetadata(null));
        public string MessageText
        {
            get { return (string)GetValue(MessageTextProperty); }
            set { SetValue(MessageTextProperty, value); }
        }

        public static readonly DependencyProperty MessageTypeProperty =
                                   DependencyProperty.Register("MessageType", typeof(MessageTypes), typeof(KioskMessageBox),
                                                                   new FrameworkPropertyMetadata(MessageTypes.None));
        public MessageTypes MessageType
        {
            get { return (MessageTypes)GetValue(MessageTypeProperty); }
            set { SetValue(MessageTypeProperty, value); }
        }

        public static readonly DependencyProperty ResultProperty =
                               DependencyProperty.Register("Result", typeof(MessageBoxResult), typeof(KioskMessageBox),
                                                               new FrameworkPropertyMetadata(MessageBoxResult.None));
        public MessageBoxResult Result
        {
            get { return (MessageBoxResult)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }
        #endregion

        #region ▩ 이벤트

        private void KioskMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            var _task = Task.Factory.StartNew(() => {
                this.Dispatcher?.Invoke(() => {

                    if (this.g_BtnOK != null)
                    {
                        this.g_BtnOK.Focus();
                        this.g_BtnOK.Focusable = true;
                    }

                    this.PreviewKeyUp += KioskMessageBox_PreviewKeyUp;
                });
            });

            _task.Wait(300);
        }

        private void KioskMessageBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.Result             = MessageBoxResult.OK;
                this.DialogResult       = true;
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Result             = MessageBoxResult.Cancel;
                this.DialogResult       = false;
            }
        }

        private void G_BtnOk_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Result             = MessageBoxResult.OK;
            this.DialogResult       = true;
        }

        private void G_BtnCancel_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Result             = MessageBoxResult.Cancel;
            this.DialogResult       = false;
        }
        #endregion
    }

    public enum MessageTypes
    {
        None,
        Error,
        Info,
        Question
    }
}
