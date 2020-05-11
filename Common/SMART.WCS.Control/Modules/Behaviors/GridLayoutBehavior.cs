using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.Modules.Behaviors
{
    public class GridLayoutBehavior : Behavior<GridControl>
    {
        public event EventHandler<MyEventArgs> LayoutChanged;

        bool IsLocked;
        readonly List<LayoutChangedType> ChangesCache = new List<LayoutChangedType>();
        readonly List<PropertyChangeNotifier> Notifiers = new List<PropertyChangeNotifier>();

        protected GridControl Grid { get { return AssociatedObject; } }
        public bool RaiseEventsImmediately { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            SubscribeColumns(Grid.Columns);
            Grid.Columns.CollectionChanged += ColumnsCollectionChanged;
            Grid.SortInfo.CollectionChanged += OnSortInfoChanged;
            Grid.FilterChanged += OnGridFilterChanged;
        }
        protected override void OnDetaching()
        {
            Notifiers.Clear();
            UnsubscribeColumns(Grid.Columns);
            Grid.Columns.CollectionChanged -= ColumnsCollectionChanged;
            Grid.SortInfo.CollectionChanged -= OnSortInfoChanged;
            Grid.FilterChanged -= OnGridFilterChanged;
            base.OnDetaching();
        }

        #region GridEventHandlers
        protected virtual void OnGridFilterChanged(object sender, RoutedEventArgs e)
        {
            ProcessLayoutChanging(LayoutChangedType.FilerChanged);
        }
        protected virtual void OnSortInfoChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ProcessLayoutChanging(LayoutChangedType.SortingChanged);
        }
        protected virtual void ColumnsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            bool hasChanges = false;
            var newColumns = Grid.Columns.Except(SubscribedColumns).ToList();
            var oldColumns = SubscribedColumns.Except(Grid.Columns).ToList();
            if (oldColumns.Any())
            {
                hasChanges = true;
                UnsubscribeColumns(oldColumns);
            }
            if (newColumns.Any())
            {
                hasChanges = true;
                SubscribeColumns(newColumns);
            }
            if (hasChanges)
                ProcessLayoutChanging(LayoutChangedType.ColumnsCollection);
        }

        protected virtual void ProcessLayoutChanging(LayoutChangedType type)
        {
            if (LayoutChanged == null)
                return;
            if (RaiseEventsImmediately)
                LayoutChanged(this, new MyEventArgs { LayoutChangedTypes = new List<LayoutChangedType> { type } });
            else
            {
                if (!ChangesCache.Contains(type))
                    ChangesCache.Add(type);
                if (IsLocked)
                    return;
                IsLocked = true;
                Dispatcher.BeginInvoke(new Action(() => {
                    IsLocked = false;
                    LayoutChanged(this, new MyEventArgs { LayoutChangedTypes = ChangesCache });
                    ChangesCache.Clear();
                }));
            }
        }
        #endregion

        #region ColumnEventHandlers
        List<GridColumn> SubscribedColumns = new List<GridColumn>();
        protected virtual void SubscribeColumns(IEnumerable<GridColumn> columns)
        {
            SubscribedColumns.AddRange(columns);
            foreach (GridColumn column in columns)
                Subscribe(column);
        }
        protected virtual void UnsubscribeColumns(IEnumerable<GridColumn> columns)
        {
            foreach (GridColumn column in columns)
            {
                SubscribedColumns.Remove(column);
                Unsubscribe(column);
            }
        }
        protected virtual void Subscribe(GridColumn column)
        {
            var actualWidthDescriptor = new PropertyChangeNotifier(column, BaseColumn.ActualWidthProperty);
            actualWidthDescriptor.ValueChanged += OnColumnWidthChanged;
            Notifiers.Add(actualWidthDescriptor);

            var visibleIndexDescriptor = new PropertyChangeNotifier(column, BaseColumn.VisibleIndexProperty);
            visibleIndexDescriptor.ValueChanged += OnColumnVisibleIndexChanged;
            Notifiers.Add(visibleIndexDescriptor);

            var groupIndexDescriptor = new PropertyChangeNotifier(column, GridColumn.GroupIndexProperty);
            groupIndexDescriptor.ValueChanged += OnColumnGroupIndexChanged;
            Notifiers.Add(groupIndexDescriptor);

            var visibleDescriptor = new PropertyChangeNotifier(column, BaseColumn.VisibleProperty);
            visibleDescriptor.ValueChanged += OnColumnVisibleChanged;
            Notifiers.Add(visibleDescriptor);
        }
        protected virtual void Unsubscribe(GridColumn column)
        {
            var notifiers = Notifiers.Where(x => x.PropertySource == column).ToList();
            foreach (var n in notifiers)
            {
                n.Dispose();
                Notifiers.Remove(n);
            }
        }
        protected virtual void OnColumnWidthChanged(object sender, EventArgs args)
        {
            ProcessLayoutChanging(LayoutChangedType.ColumnWidth);
        }
        protected virtual void OnColumnVisibleIndexChanged(object sender, EventArgs args)
        {
            ProcessLayoutChanging(LayoutChangedType.ColumnVisibleIndex);
        }
        protected virtual void OnColumnGroupIndexChanged(object sender, EventArgs args)
        {
            ProcessLayoutChanging(LayoutChangedType.ColumnGroupIndex);
        }
        protected virtual void OnColumnVisibleChanged(object sender, EventArgs args)
        {
            ProcessLayoutChanging(LayoutChangedType.ColumnVisible);
        }
        #endregion
    }

    public class MyEventArgs : EventArgs
    {
        public List<LayoutChangedType> LayoutChangedTypes { get; set; }
    }

    public enum LayoutChangedType
    {
        ColumnsCollection,
        FilerChanged,
        SortingChanged,
        ColumnGroupIndex,
        ColumnVisibleIndex,
        ColumnWidth,
        ColumnVisible,
        None
    }
}
