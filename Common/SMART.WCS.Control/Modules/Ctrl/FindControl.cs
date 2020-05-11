using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SMART.WCS.Modules.Ctrl
{
    public static class FindControl
    {
        public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                if (obj is T)
                {
                    yield return obj as T;
                }

                foreach (DependencyObject child in LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>())
                {
                    foreach (T c in FindLogicalChildren<T>(child))
                    {
                        yield return c;
                    }
                }

            }
        }

        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null)
            {
                return null;
            }

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }

        public static IEnumerable<DevExpress.Xpf.Grid.ColumnBase> FindGridControlColumn(this DevExpress.Xpf.Grid.GridControl grid)
        {

            if (grid.Bands.Count() > 0 || grid.Columns.Count() > 0)
            {
                foreach (var band in grid.Bands)
                {
                    foreach (var column in band.Columns)
                    {
                        yield return column;
                    }

                }

                foreach (var column in grid.Columns)
                {
                    yield return column;
                }
            }
        }
    }
}
