using System.Windows;
using System.Windows.Controls;

namespace SMART.WCS.Control.Kiosk
{
    /// <summary>
    /// KioskListBoxItem.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KioskListBoxItem : UserControl
    {
        #region ▩ 생성자
        public KioskListBoxItem()
        {
            InitializeComponent();
        }
        #endregion

        #region ▩ 재정의
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        #endregion

        #region ▩ 객체 선언 및 속성 정의
        public static readonly DependencyProperty HeaderProperty =
                            DependencyProperty.Register("Header", typeof(UIElement), typeof(KioskListBoxItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(HeaderPropertyChanged)));


        public UIElement Header
        {
            get { return (UIElement)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private static void HeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as KioskListBoxItem;
            owner.UpdateChild();
        }

        public static new readonly DependencyProperty ContentProperty =
                            DependencyProperty.Register("Content", typeof(UIElement), typeof(KioskListBoxItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ContentPropertyChanged)));


        public new UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        private static void ContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as KioskListBoxItem;
            owner.UpdateChild();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            this.UpdateChild();

        }

        public static readonly DependencyProperty HeaderWidthProperty =
                            DependencyProperty.Register("HeaderWidth", typeof(GridLength), typeof(KioskListBoxItem), new FrameworkPropertyMetadata(null));

        public GridLength HeaderWidth
        {
            get { return (GridLength)GetValue(HeaderWidthProperty); }
            set { SetValue(HeaderWidthProperty, value); }
        }

        public static readonly DependencyProperty ContentWidthProperty =
                            DependencyProperty.Register("ContentWidth", typeof(GridLength), typeof(KioskListBoxItem), new FrameworkPropertyMetadata(null));

        public GridLength ContentWidth
        {
            get { return (GridLength)GetValue(ContentWidthProperty); }
            set { SetValue(ContentWidthProperty, value); }
        }
        #endregion

        #region ▩ 함수
        private void UpdateChild()
        {
            if (Part_Grid != null)
            {
                Part_Grid.Children.Clear();

                if (Header != null)
                {

                    Header.SetValue(Grid.ColumnProperty, 0);
                    Part_Grid.Children.Add(Header);
                }

                if (Content != null)
                {
                    (Content as UIElement).SetValue(Grid.ColumnProperty, 1);
                    Part_Grid.Children.Add((Content as UIElement));
                }
            }
        }
        #endregion
    }
}
