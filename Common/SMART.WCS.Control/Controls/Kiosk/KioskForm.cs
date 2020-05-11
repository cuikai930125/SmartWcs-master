using SMART.WCS.Common;
using SMART.WCS.Control.Ctrl;
using SMART.WCS.Control.DataMembers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SMART.WCS.Control.Kiosk
{
    [ContentPropertyAttribute("Content")]
    [TemplatePart(Name = PART_TITLE, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_STATUS, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_DATETIME, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_BTN_PREV, Type = typeof(Button))]
    [TemplatePart(Name = PART_BTN_HOME, Type = typeof(Button))]
    [TemplatePart(Name = PART_MESSAGEBOX, Type = typeof(ContentPresenter))]
    public class KioskForm : ContentControl, INotifyPropertyChanged
    {
        private const string PART_TITLE         = "Part_Title";
        private const string PART_STATUS        = "Part_Status";
        private const string PART_DATETIME      = "Part_DateTime";
        private const string PART_BTN_PREV      = "Part_btnPrev";
        private const string PART_BTN_HOME      = "Part_btnHome";
        private const string PART_MESSAGEBOX    = "Part_MessageBox";

        /// <summary>
        /// 공통 Class를 이용하기 위한 BaseClass 선언
        /// </summary>
        public BaseClass BaseClass = new BaseClass();

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        public Button _btPrev;
        public Button _btHome;
        private TextBlock _txtDateTime;
        private ContentPresenter _MessageBoxHoloder;
        public event EventHandler<RoutedEventArgs> Close;
        public event EventHandler<OpenFormEventArgs> Open;

        public event EventHandler<RoutedEventArgs> PreviousButtonClick;
        public event EventHandler<RoutedEventArgs> HomeButtonClick;

        static KioskForm()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KioskForm), new FrameworkPropertyMetadata(typeof(KioskForm)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _btPrev = GetTemplateChild(PART_BTN_PREV) as Button;
            _btHome = GetTemplateChild(PART_BTN_HOME) as Button;
            _txtDateTime = GetTemplateChild(PART_DATETIME) as TextBlock;

            _MessageBoxHoloder = GetTemplateChild(PART_MESSAGEBOX) as ContentPresenter;

            if (_btPrev != null)
            {
                _btPrev.PreviewMouseLeftButtonUp += _btPrev_PreviewMouseLeftButtonUp;
            }

            if (_btHome != null)
            {
                _btHome.PreviewMouseLeftButtonUp += _btHome_PreviewMouseLeftButtonUp;
            }
            this.Loaded += KioskForm_Loaded;
        }

        #region > 
        public static readonly DependencyProperty TitleProperty =
                                    DependencyProperty.Register("Title", typeof(object), typeof(KioskForm),
                                                                    new FrameworkPropertyMetadata((object)null));

        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty SubTitleProperty =
                                    DependencyProperty.Register("SubTitle", typeof(object), typeof(KioskForm),
                                                                    new FrameworkPropertyMetadata((object)null));

        public object SubTitle
        {
            get { return (object)GetValue(SubTitleProperty); }
            set { SetValue(SubTitleProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
                                    DependencyProperty.Register("Status", typeof(int), typeof(KioskForm),
                                                                    new FrameworkPropertyMetadata(0));

        public int Status
        {
            get { return (int)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusTextProperty =
                                  DependencyProperty.Register("StatusText", typeof(string), typeof(KioskForm),
                                                                  new FrameworkPropertyMetadata(null));

        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }

        public static readonly DependencyProperty StatusMessageProperty =
                                  DependencyProperty.Register("StatusMessage", typeof(string), typeof(KioskForm),
                                                                  new FrameworkPropertyMetadata(null));

        public string StatusMessage
        {
            get { return (string)GetValue(StatusMessageProperty); }
            set { SetValue(StatusMessageProperty, value); }
        }

        public static readonly DependencyProperty LayOutTypeProperty =
                                    DependencyProperty.Register("LayOutType", typeof(int), typeof(KioskForm),
                                                                    new FrameworkPropertyMetadata(1));

        public int LayOutType
        {
            get { return (int)GetValue(LayOutTypeProperty); }
            set { SetValue(LayOutTypeProperty, value); }
        }

        public static readonly DependencyProperty IsSimpleFormProperty =
                                    DependencyProperty.Register("IsSimpleForm", typeof(bool), typeof(KioskForm),
                                                                    new FrameworkPropertyMetadata(false));

        public bool IsSimpleForm
        {
            get { return (bool)GetValue(IsSimpleFormProperty); }
            set { SetValue(IsSimpleFormProperty, value); }
        }
        #endregion

        public void NextForm()
        {
            KioskBase?.Next(new TimeSpan());
        }

        public void NextForm(TimeSpan ThreadSleep)
        {
            KioskBase?.Next(ThreadSleep);
        }

        public void GoToForm(TimeSpan ThreadSleep, int Index, object SendItem = null)
        {
            KioskBase?.GoToForm(Index, SendItem, ThreadSleep);
        }

        public void BackForm()
        {
            KioskBase?.Back(new TimeSpan());
        }

        public void BackForm(TimeSpan ThreadSleep)
        {
            KioskBase?.Back(ThreadSleep);
        }

        public KioskBase KioskBase
        {
            get
            {
                var _parent = this.FindParent<KioskBase>();

                return (_parent != null) ? _parent : null;
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// KioskBase 제어하는 Form Open
        /// </summary>
        /// <param name="fromIndex"></param>
        /// <param name="sendItem"></param>
        public void OpenForm(int fromIndex, object sendItem = null)
        {
            Open?.Invoke(this, new OpenFormEventArgs { FromIndex = fromIndex, Item = sendItem });
        }

        /// <summary>
        /// KioskBase 제어하는 Form Close
        /// </summary>
        public void CloseForm()
        {
            Close?.Invoke(this, new RoutedEventArgs());
        }

        public void SetFocus(object Key)
        {
            var _item = this.FindLogicalChildren<DependencyObject>().Where(f => f.GetValue(TagProperty) == Key);

            if (_item.Count() > 0)
            {
                (_item.First() as UIElement).Focus();
            }
        }

        public void ShowMessageBox(KioskMessageBox messagebox)
        {
            _MessageBoxHoloder.Content = messagebox;
            _MessageBoxHoloder.Visibility = Visibility.Visible;
        }

        public void ReMoveBox(KioskMessageBox messagebox)
        {
            _MessageBoxHoloder.Content = null;
            _MessageBoxHoloder.Visibility = Visibility.Collapsed;
        }

        private void KioskForm_Loaded(object sender, RoutedEventArgs e)
        {
            if (_txtDateTime != null)
            {
                TimeSpan ts = TimeSpan.FromMinutes(1);
                timer.Interval = ts;
                timer.Tick += Timer_Tick;
            }

            this.Focus();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!_txtDateTime.Dispatcher.CheckAccess())
                {
                    _txtDateTime.Dispatcher.Invoke(
                      System.Windows.Threading.DispatcherPriority.Normal,
                      new Action(
                        delegate ()
                        {
                            _txtDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        }
                    ));
                }
                else
                {
                    _txtDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void _btHome_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (HomeButtonClick != null)
            {
                HomeButtonClick(this, new RoutedEventArgs());
            }
        }

        private void _btPrev_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PreviousButtonClick != null)
            {
                PreviousButtonClick(this, new RoutedEventArgs());
            }
        }


    }
}
