using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Modules.Ctrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.Modules.Behaviors
{
    public class SelectorColumnBehavior : Behavior<GridControl>
    {
        public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.Register(
                                                      "HeaderVisibility",
                                                      typeof(Visibility),
                                                      typeof(SelectorColumnBehavior), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(HeaderVisibilityPropertyChanged)));

        private static void HeaderVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as SelectorColumnBehavior;
            owner.Column.HeaderVisibility = owner.HeaderVisibility;
        }

        public Visibility HeaderVisibility
        {
            get { return (Visibility)GetValue(HeaderVisibilityProperty); }
            set { SetValue(HeaderVisibilityProperty, value); }
        }


        public Dictionary<object, bool> Values = new Dictionary<object, bool>();
        protected SelectorColumn Column = new SelectorColumn();

        public GridControl Grid { get { return AssociatedObject; } }

        public string KeyExpression { get; set; }
        public TableView View { get { return (TableView)Grid.View; } }

        public event EventHandler<SelectorCellChekedEventArgs> SelectorCellCheked;

        protected override void OnAttached()
        {
            base.OnAttached();


            if (KeyExpression == null)
            {
                KeyExpression = "IsSelected";
            }

            Grid.CustomUnboundColumnData += OnCustomUnboundColumnData;
            Grid.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            Grid.PreviewKeyDown += OnPreviewKeyDown;
            Grid.Loaded += Grid_Loaded;
            Grid.ItemsSourceChanged += Grid_ItemsSourceChanged;
            //View.CellValueChanged += View_CellValueChanged;

        }

        private void View_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            //var info = View.CalcHitInfo((DependencyObject)e.OriginalSource);

            //for (int i = 0; i < Grid.VisibleItems.Count; i++)
            //{
            //    bool _readOnly = false;
            //    var _row = Grid.GetRow(i);

            //    _readOnly = (bool)_row.GetType().GetProperty(SelectorColumn.SelectReadOnlyName)?.GetValue(_row);

            //    if (!_readOnly)
            //    {
            //        Grid.SetCellValue(i, SelectorColumn.CheckedColumnFieldName, false);
            //    }
            //}

            //int iRowIndex = e.RowHandle;
            //Grid.SetCellValue(iRowIndex, SelectorColumn.CheckedColumnFieldName, true);


            //Grid.RefreshData();

        }

        private void Grid_ItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e)
        {
            Column.IsSelectAll = false;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var _selectColumn = Grid.Columns.Where(f => f is SelectorColumn);

            if (_selectColumn.Count() > 0)
            {
                //var _column = _selectColumn.First() as SelectorColumn;
                //_column.HeaderTemplate = _column.HeaderTemplate;
                //_column.CellTemplate = _column.CellTemplate;

            }
            else
            {
                Column = new SelectorColumn();

                Column.VisibleIndex = 0;


                if (Grid.Bands.Count() > 0)
                {
                    GridControlBand _bald = new GridControlBand();
                    _bald.Fixed = FixedStyle.Left;
                    _bald.OverlayHeaderByChildren = true;
                    _bald.VisibleIndex = 0;
                    _bald.Columns.Add(Column);

                    Grid.Bands.Insert(0, _bald);
                }
                else
                {
                    Grid.Columns.Insert(0, Column);
                }
            }
            Column.VisibleIndex = 0;
            Column.HeaderVisibility = HeaderVisibility;
            Column.Fixed = FixedStyle.Left;
            Column.AllSelectChanged += column_AllSelectChanged;
            Column.CellSelectChanged += CellSelectChanged;
            Column.Width = 40;
            Column.MaxWidth = 40;
            Column.MinWidth = 40;
            Column.AllowSorting = DevExpress.Utils.DefaultBoolean.False;

        }
        private void CellSelectChanged(object sender, SelectorCellChekedEventArgs e)
        {
            if (SelectorCellCheked != null)
            {
                SelectorCellCheked(sender, e);
            }
        }

        private void column_AllSelectChanged(object sender, SelectorHeaderCheckedEventArgs e)
        {
            for (int i = 0; i < Grid.VisibleItems.Count; i++)
            {
                bool _readOnly = false;
                var _row = Grid.GetRow(i);

                _readOnly = (bool)_row.GetType().GetProperty(SelectorColumn.SelectReadOnlyName)?.GetValue(_row);

                if (!_readOnly)
                {
                    Grid.SetCellValue(i, SelectorColumn.CheckedColumnFieldName, e.IsCheck);
                }
            }

            Grid.RefreshData();
        }

        protected override void OnDetaching()
        {
            Grid.CustomUnboundColumnData -= OnCustomUnboundColumnData;
            Grid.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            Grid.PreviewKeyDown -= OnPreviewKeyDown;
            View.CellValueChanged -= View_CellValueChanged;
            Grid.Columns.Remove(Column);
            base.OnDetaching();
        }
        protected virtual void OnCustomUnboundColumnData(object sender, GridColumnDataEventArgs e)
        {
            if (KeyExpression == null)
            {
                return;
            }

            SelectorColumn column = e.Column as SelectorColumn;
            if (column == null)
            {
                return;
            }

            var key = e.GetListSourceFieldValue(KeyExpression);
            if (e.IsGetData)
            {
                bool value;
                e.Value = Values.TryGetValue(key, out value) ? value : false;
            }

            if (e.IsSetData)
            {
                Values[key] = (bool)e.Value;
            }
        }
        protected virtual void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //var info = View.CalcHitInfo((DependencyObject)e.OriginalSource);
            //if (!(info.Column is SelectorColumn))
            //    return;
            //InvertSelection(info.RowHandle);
        }
        protected virtual void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Space)
            {
                return;
            }

            var info = View.CalcHitInfo((DependencyObject)e.OriginalSource);
            if (!info.InRow)
            {
                return;
            }

            var handles = Grid.GetSelectedRowHandles();
            foreach (var handle in handles)
            {
                InvertSelection(handle);
            }
        }
        protected virtual void InvertSelection(int rowHandle)
        {

            if (rowHandle >= 0)
            {
                var value = (bool)Grid.GetCellValue(rowHandle, SelectorColumn.CheckedColumnFieldName);
                Grid.SetCellValue(rowHandle, SelectorColumn.CheckedColumnFieldName, !value);
                Grid.RefreshRow(rowHandle);
            }
        }
    }
}
