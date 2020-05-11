using DevExpress.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SMART.WCS.Modules.Ctrl
{
    public class SelectorColumn : GridColumn
    {
        public static readonly DependencyProperty IsSelectAllProperty = DependencyProperty.Register(
                                                        "IsSelectAll",
                                                        typeof(bool),
                                                        typeof(SelectorColumn), new PropertyMetadata(false, new PropertyChangedCallback(IsSelectAllPropertyChanged)));

        private static void IsSelectAllPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public bool IsSelectAll
        {
            get { return (bool)GetValue(IsSelectAllProperty); }
            set { SetValue(IsSelectAllProperty, value); }
        }

        public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.Register(
                                                       "HeaderVisibility",
                                                       typeof(Visibility),
                                                       typeof(SelectorColumn), new PropertyMetadata(Visibility.Visible));

        public Visibility HeaderVisibility
        {
            get { return (Visibility)GetValue(HeaderVisibilityProperty); }
            set { SetValue(HeaderVisibilityProperty, value); }
        }

        public EventHandler<SelectorHeaderCheckedEventArgs> AllSelectChanged;
        public EventHandler<SelectorCellChekedEventArgs> CellSelectChanged;

        CheckEdit _Header = new CheckEdit();

        public static string CheckedColumnFieldName = "IsSelected";
        public static string SelectReadOnlyName = "SelectReadOnly";

        public SelectorColumn()
        {
            FieldName = CheckedColumnFieldName;
            UnboundType = UnboundColumnType.Boolean;
            EditSettings = new CheckEditSettings();
            Fixed = FixedStyle.None;
            HorizontalHeaderContentAlignment = HorizontalAlignment.Center;

            #region Header Template

            DataTemplate _template = new DataTemplate();
            FrameworkElementFactory GridFactory = new FrameworkElementFactory(typeof(System.Windows.Controls.Grid));
            GridFactory.SetValue(System.Windows.Controls.Grid.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            GridFactory.SetValue(System.Windows.Controls.Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
            GridFactory.SetValue(System.Windows.Controls.Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
            FrameworkElementFactory header = new FrameworkElementFactory(typeof(CheckEdit));
            header.SetValue(CheckEdit.VerticalAlignmentProperty, VerticalAlignment.Center);
            header.SetValue(CheckEdit.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            header.AddHandler(CheckEdit.EditValueChangedEvent, new EditValueChangedEventHandler(chkHeader_EditValueChanged));

            Binding binding = new Binding
            {
                Path = new PropertyPath("IsSelectAll"),
                Source = this,
                Mode = BindingMode.TwoWay,
                NotifyOnSourceUpdated = true
            };

            Binding bindingHeaderVisibility = new Binding
            {
                Path = new PropertyPath("HeaderVisibility"),
                Source = this,
                Mode = BindingMode.TwoWay,
                NotifyOnSourceUpdated = true
            };

            header.SetBinding(CheckEdit.VisibilityProperty, bindingHeaderVisibility);
            header.SetBinding(CheckEdit.EditValueProperty, binding);
            GridFactory.AppendChild(header);
            _template.VisualTree = GridFactory;

            HeaderTemplate = _template;
            #endregion
            //< DataTemplate >
            //< CheckBox IsChecked = "{Binding RowData.Row.IsSelected}" IsEnabled = "{Binding RowData.Row.IsTest}" HorizontalAlignment = "Center" VerticalAlignment = "Center" />

            //</ DataTemplate >
            #region Cell Template
            DataTemplate _cellTemplate = new DataTemplate();
            FrameworkElementFactory cellGridFactory = new FrameworkElementFactory(typeof(System.Windows.Controls.Grid));
            cellGridFactory.SetValue(System.Windows.Controls.Grid.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            cellGridFactory.SetValue(System.Windows.Controls.Grid.VerticalAlignmentProperty, VerticalAlignment.Stretch);

            FrameworkElementFactory cellFactory = new FrameworkElementFactory(typeof(CheckEdit));
            cellFactory.SetValue(CheckEdit.VerticalAlignmentProperty, VerticalAlignment.Center);
            cellFactory.SetValue(CheckEdit.HorizontalAlignmentProperty, HorizontalAlignment.Center);

            Binding cellBinding = new Binding
            {
                Path = new PropertyPath("RowData.Row.IsSelected")
                //Mode = BindingMode.TwoWay
            };

            Binding cellReadBinding = new Binding
            {
                Path = new PropertyPath("RowData.Row.SelectReadOnly")
                //Mode = BindingMode.TwoWay
            };

            Binding cellEnableBinding = new Binding
            {
                Path = new PropertyPath("RowData.Row.SelectReadOnly"),
                Converter = new DevExpress.Xpf.Core.BoolInverseConverter()
                //Mode = BindingMode.TwoWay
            };
            cellFactory.AddHandler(CheckEdit.EditValueChangedEvent, new EditValueChangedEventHandler(Cell_EditValueChanged));
            cellFactory.SetValue(CheckEdit.EditValueProperty, cellBinding);
            cellFactory.SetValue(CheckEdit.IsReadOnlyProperty, cellReadBinding);
            cellFactory.SetValue(CheckEdit.IsEnabledProperty, cellEnableBinding);


            cellGridFactory.AppendChild(cellFactory);
            _cellTemplate.VisualTree = cellGridFactory;

            CellTemplate = _cellTemplate;
            #endregion
        }


        //public void AllSelect()
        //{
        //    if (AllSelectChanged != null)
        //    {
        //        AllSelectChanged(this, new SelectorHeaderChekedEventArgs(true));
        //    }
        //}

        //public void AllUnSelect()
        //{
        //    if (AllSelectChanged != null)
        //    {
        //        AllSelectChanged(this, new SelectorHeaderChekedEventArgs(false));
        //    }
        //}

        private void Cell_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (CellSelectChanged != null)
            {
                if (e.NewValue is bool)
                {
                    var _value = (bool)e.NewValue;
                    if ((sender as FrameworkElement).DataContext != null)
                    {
                        if ((sender as FrameworkElement).DataContext as DevExpress.Xpf.Grid.EditGridCellData != null)
                        {
                            var _data = ((sender as FrameworkElement).DataContext as DevExpress.Xpf.Grid.EditGridCellData).RowData;
                            CellSelectChanged(this, new SelectorCellChekedEventArgs(_data, _value));
                        }
                    }
                }
            }
        }

        private void chkHeader_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (AllSelectChanged != null)
            {
                if (e.NewValue is bool)
                {
                    var _value = (bool)e.NewValue;
                    this.IsSelectAll = _value;
                    AllSelectChanged(this, new SelectorHeaderCheckedEventArgs(_value));
                }
            }
        }
    }

    public class SelectorHeaderCheckedEventArgs : EventArgs
    {
        public SelectorHeaderCheckedEventArgs(bool IsSelected)
        {
            IsCheck = IsSelected;
        }

        public bool IsCheck { get; set; }
    }

    public class SelectorCellChekedEventArgs : EventArgs
    {
        public SelectorCellChekedEventArgs(DevExpress.Xpf.Grid.RowData rowData, bool isSelected)
        {
            RowData = rowData;
            IsSelected = isSelected;
        }

        public DevExpress.Xpf.Grid.RowData RowData { get; set; }
        public bool IsSelected { get; set; }
    }
}
