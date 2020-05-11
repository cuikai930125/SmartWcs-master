using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Modules.Extensions
{
    public static class GridControlExtension
    {
        /// <summary>
        /// 그리드컨트롤의 selectorColumn이 있는 경우 전체 선택인지 아닌지의 여부를 반환 
        /// </summary>
        /// <param name="gridControl">this</param>
        /// <returns>전체 선택여부</returns>
        public static bool IsSelectAll(this GridControl gridControl)
        {
            bool _result = false;

            if (gridControl.Columns.Count() > 0)
            {
                if (gridControl.Columns[0] is Modules.Ctrl.SelectorColumn)
                {
                    _result = (gridControl.Columns[0] as Modules.Ctrl.SelectorColumn).IsSelectAll;
                }
            }

            return _result;
        }

        /// <summary>
        /// 그리드컨트롤의 selectorColumn이 있는 경우 전체 선택인지 아닌지의 여부를 설정
        /// </summary>
        /// <param name="gridControl"></param>
        public static void IsSelectAll(this GridControl gridControl,bool IsSelect)
        {
            if(gridControl.Columns.Count() > 0)
            {
                if(gridControl.Columns[0] is Modules.Ctrl.SelectorColumn)
                {
                    (gridControl.Columns[0] as Modules.Ctrl.SelectorColumn).IsSelectAll = IsSelect;
                }
            }
        }

        public static int GetVisibleRowIndex(this GridControl gridControl,object Data)
        {
            int _result = -1;

            for (int i = 0; i < gridControl.VisibleRowCount; i++)
            {
                if(Data.Equals(gridControl.GetRow(i)))
                {
                    _result = i;
                    break;
                }
            }

            return _result;
           
        }
    }
}
