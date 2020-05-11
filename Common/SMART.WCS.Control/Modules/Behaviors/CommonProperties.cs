using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.Modules.Behaviors
{
    public static class CommonProperties
    {
        public static readonly DependencyProperty GridColumnEventsProperty = DependencyProperty.RegisterAttached(
                                                                    "GridColumnEvents",
                                                                    typeof(IList),
                                                                    typeof(CommonProperties),
                                                                    new PropertyMetadata(new List<Control.Ctrl.GridColumnEventInfo>()));

        private static void OnGridColumnEventsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ///"Text", "Content" 프로퍼티가 있는 컨트롤에 출력해서 구분할 수 있도록 한다.
            ///
            //if (IsDesignTimeProperty)
        }
        public static void SetGridColumnEvents(UIElement element, IList value)
        {
            element.SetValue(GridColumnEventsProperty, value);
        }
        public static IList GetGridColumnEvents(UIElement element)
        {
            return (IList)element.GetValue(GridColumnEventsProperty);
        }


        public static readonly DependencyProperty LabelCdProperty = DependencyProperty.RegisterAttached(
                                                                    "LabelCd",
                                                                    typeof(string),
                                                                    typeof(CommonProperties),
                                                                new PropertyMetadata(null, OnLabelCdPropertyChanged));

        private static void OnLabelCdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ///"Text", "Content" 프로퍼티가 있는 컨트롤에 출력해서 구분할 수 있도록 한다.
            ///
            //if (IsDesignTimeProperty)
            SetLabel(d);
        }

        private static void SetLabel(DependencyObject d)
        {
            if (d is UIElement)
            {
                var _properties = new[] { "Text", "Content" };

                foreach (var item in _properties)
                {
                    object _desc = null;

                    var _prop = d.GetType().GetProperty(item);
                    var _descProp = (d as UIElement).GetValue(LabelDescProperty);
                    var _cdProp = (d as UIElement).GetValue(LabelCdProperty);

                    if (_descProp == null)
                    {
                        if (_cdProp != null)
                        {
                            _descProp = GetDesc(_cdProp.ToString());
                        }
                    }

                    if (_prop != null)
                    {
                        if (_descProp != null)
                        {
                            _desc = _descProp;
                        }
                        else
                        {
                            _desc = _cdProp;
                        }

                        _prop.SetValue(d, _desc);
                        break;
                    }
                }
            }
        }

        private static string GetDesc(string Code)
        {
            string _result = null;

            if (Code != null)
            {
                string _code = "";
                string _format = "{0}";

                var _array = Code.ToString().Split(',');

                _code = _array[0];

                if (_array.Count() > 1)
                {
                    _format = _array[1];
                }

                ResourceManager rm = new ResourceManager("SMART.WCS.Resource.Language.LanguageResource", typeof(SMART.WCS.Resource.Language.LanguageResource).Assembly);
                //CultureInfo cultureInfo = SMART.WCS.Control.Utility.HelperClass.GetCultureInfo();
                CultureInfo cultureInfo =  SMART.WCS.Common.Utility.HelperClass.GetCountryNameEmptyCntryCD();

                _result = rm.GetString(_code, cultureInfo);
            }

            return _result;
        }

        public static T GetLabel<T>(string key)
        {
            return (T)Convert.ChangeType(GetDesc(key), typeof(T));
        }

        public static void SetLabelCd(UIElement element, string value)
        {
            element.SetValue(LabelCdProperty, value);
        }
        public static string GetLabelCd(UIElement element)
        {
            return (string)element.GetValue(LabelCdProperty);
        }

        public static readonly DependencyProperty LabelDescProperty = DependencyProperty.RegisterAttached(
                                                                     "LabelDesc",
                                                                     typeof(string),
                                                                     typeof(CommonProperties),
                                                                   new PropertyMetadata(null, OnLabelDescPropertyChanged));

        private static void OnLabelDescPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetLabel(d);
        }

        public static void SetLabelDesc(UIElement element, string value)
        {
            element.SetValue(LabelDescProperty, value);
        }
        public static string GetLabelDesc(UIElement element)
        {
            return (string)element.GetValue(LabelDescProperty);
        }


        public static readonly DependencyProperty BehaviorsProperty
            = DependencyProperty.RegisterAttached("Behavior", typeof(Behavior), typeof(CommonProperties), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(BehaviorChanged)));

        private static void BehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DevExpress.Xpf.Grid.TableView && e.NewValue is Behavior)
            {
                BehaviorCollection Behaviors = DevExpress.Mvvm.UI.Interactivity.Interaction.GetBehaviors(d);

                if (Behaviors.CanFreeze)
                {
                    Behaviors.Add(e.NewValue as Behavior);
                }
            }
        }

        public static void SetBehavior(UIElement element, Behavior value)
        {
            element.SetValue(BehaviorsProperty, value);
        }

        public static Behavior GetBehavior(UIElement element)
        {
            return (Behavior)element.GetValue(BehaviorsProperty);
        }

        public static readonly DependencyProperty SetFocusProperty = DependencyProperty.RegisterAttached(
                                                                     "SetFocus",
                                                                     typeof(bool),
                                                                     typeof(CommonProperties),
                                                                   new PropertyMetadata(false, SetFocusPropertyChanged));

        private static void SetFocusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement)
            {
                if ((bool)e.NewValue)
                {
                    (d as UIElement).Focus();
                    SetSetFocus((d as UIElement), false);
                }
                else
                {
                    //(d as UIElement).Focusable = false;
                    //SetSetFocus((d as UIElement), false);
                }
            }
        }


        public static void SetSetFocus(UIElement element, bool value)
        {
            element.SetValue(SetFocusProperty, value);
        }
        public static bool GetSetFocus(UIElement element)
        {
            return (bool)element.GetValue(SetFocusProperty);
        }

        public static readonly DependencyProperty IsDesignTimeProperty = DependencyProperty.RegisterAttached(
                                                                  "IsDesignTime",
                                                                  typeof(bool),
                                                                  typeof(CommonProperties), new PropertyMetadata(DesignerProperties.GetIsInDesignMode(CurrentWindow())));
        public static bool GetIsDesignTime(UIElement element)
        {
            return (bool)element.GetValue(IsDesignTimeProperty);
            //return DesignerProperties.GetIsInDesignMode(CurrentWindow());
        }
        public static bool GetIsDesignTime()
        {
            return (bool)new UIElement().GetValue(IsDesignTimeProperty);
            //return DesignerProperties.GetIsInDesignMode(CurrentWindow());
        }

        public static void SetIsDesignTime(UIElement element, bool value)
        {
            //element.SetValue(IsDesignTimeProperty, value);
            throw new Exception("IsDesignTime is Read Only");
        }

        public static bool IsNotDesignTimeProperty { get { return DesignerProperties.GetIsInDesignMode(CurrentWindow()) ? false : true; } }

        public static System.Windows.Visibility DesignTimeVisibleProperty { get { return DesignerProperties.GetIsInDesignMode(CurrentWindow()) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; } }

        public static DependencyObject CurrentWindow()
        {
            //if (App.Current.MainWindow != null)
            if (SMART.WCS.Control.BaseApp.Current.MainWindow != null)
            {
                return SMART.WCS.Control.BaseApp.Current.MainWindow;
            }
            else
            {
                return new DependencyObject();
            }
        }
    }
}
