using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.COMMON.DataMembers.C1022_GAN_01P
{
    public class ToteCellItemInfo : PropertyNotifyExtensions
    {
        public string CellId { get; set; }
        /// <summary>
        /// 'A' - 자주색 *1:Inbound*
        /// 'B' - 보라색 *2:Outbound*
        /// 'C' - 하늘색
        /// 'D' - 연두색
        /// 'E' - 노란색
        /// </summary>
        public string ColorType { get; set; }
        public int StockQty { get; set; }

        ////public bool IsSelectCell { get; set; }
        //private bool _IsSelectCell;
        //public bool IsSelectCell
        //{
        //    get { return this._IsSelectCell; }
        //    set
        //    {
        //        if (this._IsSelectCell != value)
        //        {
        //            this._IsSelectCell = value;
        //            RaisePropertyChanged();
        //        }
        //    }
        //}
    }
}
