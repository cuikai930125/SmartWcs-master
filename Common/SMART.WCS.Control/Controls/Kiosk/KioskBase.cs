using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SMART.WCS.Control.Kiosk
{
    [TemplatePart(Name = PART_FORM_HOLDER, Type = typeof(Grid))]
    public class KioskBase : UserControl
    {
        #region ▩ 전역변수
        private const string PART_FORM_HOLDER   = "Part_FormHolder";
        private Grid _FormHolder;
        #endregion

        #region ▩ 생성자
        static KioskBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KioskBase), new FrameworkPropertyMetadata(typeof(KioskBase)));
        }
        #endregion

        #region ▩ 재정의
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._FormHolder = this.GetTemplateChild(PART_FORM_HOLDER) as Grid;
        }

        public override void BeginInit()
        {
            base.BeginInit();

            this.Loaded += KioskBase_Loaded;
            this.Unloaded += KioskBase_Unloaded;
        }
        #endregion

        #region ▩ 객체 선언 및 속성 정의
        public static readonly DependencyProperty FormCollectionProperty =
                                    DependencyProperty.Register("FormCollection", typeof(List<UIElementInfo>), typeof(KioskBase),
                                                                    new FrameworkPropertyMetadata(new List<UIElementInfo>()));
        public List<UIElementInfo> FormCollection
        {
            get { return (List<UIElementInfo>)GetValue(FormCollectionProperty); }
            set { SetValue(FormCollectionProperty, value); }
        }

        public int CurrentIndex
        {
            get { return (CurrentForm != null) ? CurrentForm.Key : -1; }
        }

        public UIElementInfo CurrentForm
        {
            get { return FormCollection.SingleOrDefault(f => f.kioskForm.Visibility == Visibility.Visible); }
        }
        #endregion

        #region ▩ 함수
        public void AddForm(KioskForm kioskForm)
        {
            this.FormCollection.Add(new UIElementInfo(kioskForm, this.FormCollection.Count));
        }

        public void InsertForm(KioskForm kioskForm, int Index)
        {
            this.FormCollection.ForEach(f =>
            {
                if (f.Key >= Index)
                {
                    f.Key += 1;
                }
            });
            this.FormCollection.Insert(Index, new UIElementInfo(kioskForm, this.FormCollection.Count));
        }

        public void Next(TimeSpan Sleep, object sendItem = null)
        {
            GoToForm(CurrentIndex, CurrentIndex + 1, sendItem, Sleep);
        }

        public void Back(TimeSpan Sleep, object sendItem = null)
        {
            GoToForm(CurrentIndex, CurrentIndex - 1, sendItem, Sleep);
        }

        public void GoToForm(int ToIndex, object sendItem, TimeSpan Sleep)
        {
            if (ToIndex < this.FormCollection.Count)
            {
                GoToForm(CurrentIndex, ToIndex, sendItem, Sleep);
            }
        }

        private void GoToForm(int FromIndex, int ToIndex, object sendItem, TimeSpan Sleep)
        {
            if (FormCollection.Count() > 0)
            {
                UIElementInfo _FromItem = FormCollection.Find(f => f.Key == FromIndex);

                UIElementInfo _ToItem = FormCollection.Find(f => f.Key == ToIndex);

                if (_FromItem != null && _ToItem != null)
                {
                    _ToItem.kioskForm.Visibility = Visibility.Visible;
                    _ToItem.kioskForm.OpenForm(FromIndex, sendItem);

                    System.Threading.Thread.Sleep(Sleep);

                    _FromItem.kioskForm.Visibility = Visibility.Hidden;
                    _FromItem.kioskForm.CloseForm();
                }
            }
        }
        #endregion

        #region ▩ 이벤트
        private void KioskBase_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in FormCollection)
            {
                item.kioskForm.CloseForm();
                item.kioskForm = null;
            }

            FormCollection.RemoveRange(0, FormCollection.Count);
        }

        private void KioskBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormCollection.Count() > 0 && _FormHolder != null)
            {
                for (int i = 0; i < FormCollection.Count; i++)
                {
                    if (i == 0)
                    {
                        FormCollection[i].kioskForm.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        FormCollection[i].kioskForm.Visibility = Visibility.Hidden;
                    }

                    _FormHolder.Children.Add(FormCollection[i].kioskForm);
                }
            }
        }
        #endregion
    }

    public class UIElementInfo
    {
        public UIElementInfo(KioskForm form, int index)
        {
            this.kioskForm = form;
            this.Key = index;
        }

        public KioskForm kioskForm { get; set; }
        public int Key { get; set; }
    }
}
