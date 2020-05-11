using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SMART.WCS.Control.Kiosk
{
    [TemplatePart(Name = PART_HEADER_BORDER, Type = typeof(string))]
    [TemplatePart(Name = PART_ITEM_PANEL, Type = typeof(string))]
    public class KioskListBox : System.Windows.Controls.ItemsControl
    {
        #region ▩ 전역변수
        private const string PART_HEADER_BORDER  = "Part_HeaderBorder";
        private const string PART_ITEM_PANEL     = "Part_ItemPanel";

        private StackPanel g_ItemPanel;
        private Border g_HeaderBorder;
        #endregion

        #region ▩ 생성자
        static KioskListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KioskListBox), new FrameworkPropertyMetadata(typeof(KioskListBox)));
        }
        #endregion

        #region ▩ 재정의
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.g_ItemPanel        = this.GetTemplateChild(PART_ITEM_PANEL) as StackPanel;
            this.g_HeaderBorder     = this.GetTemplateChild(PART_HEADER_BORDER) as Border;
        }

        public override void BeginInit()
        {
            base.BeginInit();

            this.Loaded += KioskListBox_Loaded;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == KioskListBox.LIneThicknessProperty
                || e.Property == KioskListBox.BorderBrushProperty)
            {
                ChangeBorderProperty();
            }
            else if (e.Property == KioskListBox.ItemMarginProperty)
            {
                ChangeItemsProperty();
            }
        }
        #endregion

        #region ▩ 객체 선언 및 속성 정의
        public static readonly DependencyProperty HeaderWidthProperty =
                            DependencyProperty.Register("HeaderWidth", typeof(GridLength), typeof(KioskListBox), new FrameworkPropertyMetadata(null));

        public GridLength HeaderWidth
        {
            get { return (GridLength)GetValue(HeaderWidthProperty); }
            set { SetValue(HeaderWidthProperty, value); }
        }


        public static readonly DependencyProperty ShowLinesProperty =
                            DependencyProperty.Register("ShowLines", typeof(bool), typeof(KioskListBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowLinesPropertyChanged)));

        private static void ShowLinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as KioskListBox;
            owner.UpdateListBox();
        }

        public bool ShowLines
        {
            get { return (bool)GetValue(ShowLinesProperty); }
            set { SetValue(ShowLinesProperty, value); }
        }

        public static readonly DependencyProperty LIneThicknessProperty =
                        DependencyProperty.Register("LIneThickness", typeof(double), typeof(KioskListBox), new FrameworkPropertyMetadata(null));

        public double LIneThickness
        {
            get { return (double)GetValue(LIneThicknessProperty); }
            set { SetValue(LIneThicknessProperty, value); }
        }

        public static readonly DependencyProperty ContentWidthProperty =
                            DependencyProperty.Register("ContentWidth", typeof(GridLength), typeof(KioskListBox), new FrameworkPropertyMetadata(null));

        public GridLength ContentWidth
        {
            get { return (GridLength)GetValue(ContentWidthProperty); }
            set { SetValue(ContentWidthProperty, value); }
        }

        public static readonly DependencyProperty HeaderBackGroundProperty =
                           DependencyProperty.Register("HeaderBackGround", typeof(SolidColorBrush), typeof(KioskListBox), new FrameworkPropertyMetadata(null));

        public SolidColorBrush HeaderBackGround
        {
            get { return (SolidColorBrush)GetValue(HeaderBackGroundProperty); }
            set { SetValue(HeaderBackGroundProperty, value); }
        }

        public static readonly DependencyProperty ItemMarginProperty =
                           DependencyProperty.Register("ItemMargin", typeof(Thickness), typeof(KioskListBox), new FrameworkPropertyMetadata(null));

        public Thickness ItemMargin
        {
            get { return (Thickness)GetValue(ItemMarginProperty); }
            set { SetValue(ItemMarginProperty, value); }
        }
        #endregion

        #region ▩ 함수
        private void ChangeBorderProperty()
        {
            if (this.g_ItemPanel != null)
            {
                foreach (var item in this.g_ItemPanel.Children.OfType<Border>())
                {
                    item.Height         = LIneThickness;
                    item.Background     = BorderBrush;
                }
            }
        }

        private void ChangeItemsProperty()
        {
            if (this.g_ItemPanel != null)
            {
                foreach (var item in this.g_ItemPanel.Children.OfType<KioskListBoxItem>())
                {
                    item.Margin = ItemMargin;
                }
            }
        }

        private void UpdateListBox()
        {

            if (this.g_ItemPanel != null)
            {
                this.g_ItemPanel.Children.RemoveRange(0, this.g_ItemPanel.Children.Count);

                object[] arrObjItemsList = Items.Cast<object>().ToArray();

                this.Items.Clear();

                for (int i = 0; i < arrObjItemsList.Length; i++)
                {
                    if (ShowLines == true)
                    {
                        if (i > 0 && i <= arrObjItemsList.Length - 1)
                        {
                            var _border = new Border { Height = LIneThickness, Background = BorderBrush ,Tag = "ListBoxItemsLine"};

                            this.g_ItemPanel.Children.Add(_border);
                        }
                    }
                    var kioskItem   = arrObjItemsList[i] as KioskListBoxItem;

                    kioskItem.HeaderWidth       = this.HeaderWidth;
                    kioskItem.ContentWidth      = this.ContentWidth;
                    kioskItem.Margin            = ItemMargin;

                    if (arrObjItemsList[i] is KioskListBoxItem)
                    {
                        this.g_ItemPanel.Children.Add(arrObjItemsList[i] as KioskListBoxItem);
                    }
                    else
                    {
                        this.g_ItemPanel.Children.Add(new Label { Content = arrObjItemsList[i] });
                    }
                }
            }
        }
        #endregion

        #region ▩ 이벤트
        private void KioskListBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.g_ItemPanel != null)
            {
                this.UpdateListBox();
                this.ChangeItemsProperty();
            }
        }
        #endregion
    }
}
