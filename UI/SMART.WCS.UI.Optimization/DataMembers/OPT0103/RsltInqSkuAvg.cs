using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Optimization.DataMembers.OPT0103
{
    public class RsltInqSkuAvg : PropertyNotifyExtensions
    {
        #region + SKU LEN, Average
        private double _OPT_SKU_LEN;
        public double OPT_SKU_LEN
        {
            get { return this._OPT_SKU_LEN; }
            set
            {
                if (this._OPT_SKU_LEN != value)
                {
                    this._OPT_SKU_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU X좌표 값
        private double _OPT_SKU_ROW_NO;
        public double OPT_SKU_ROW_NO
        {
            get { return this._OPT_SKU_ROW_NO; }
            set
            {
                if (this._OPT_SKU_ROW_NO != value)
                {
                    this._OPT_SKU_ROW_NO = value;
                    this.POINT = new System.Windows.Point(OPT_SKU_LEN, OPT_SKU_ROW_NO);
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        private System.Windows.Point _POINT;
        public System.Windows.Point POINT
        {
            get { return _POINT; }
            set
            {
                if (this._POINT != value)
                {
                    this._POINT = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
