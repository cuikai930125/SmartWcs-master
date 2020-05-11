using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Optimization.DataMembers.OPT0102
{
    public class BtchDivTotPickList : PropertyNotifyExtensions
    {
        #region + BTCH_DIV_CNT - 배치 차수
        private int _BTCH_DIV_CNT;
        public int BTCH_DIV_CNT
        {
            get { return this._BTCH_DIV_CNT; }
            set
            {
                if (this._BTCH_DIV_CNT != value)
                {
                    this._BTCH_DIV_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_CNT - 파킹 SKU수
        private int _SKU_CNT;
        public int SKU_CNT
        {
            get { return this._SKU_CNT; }
            set
            {
                if (this._SKU_CNT != value)
                {
                    this._SKU_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
