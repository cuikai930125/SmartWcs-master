using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P2001_ANV_01P
{
    public class ObExtAbcInfo : PropertyNotifyExtensions
    {
        public ObExtAbcInfo()
        {

        }

        #region + ABC_CODE - ???
        private string _ABC_CODE;
        public string ABC_CODE
        {
            get { return this._ABC_CODE; }
            set
            {
                if (this._ABC_CODE != value)
                {
                    this._ABC_CODE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PERCENT_RANGE - ???
        private string _PERCENT_RANGE;
        public string PERCENT_RANGE
        {
            get { return this._PERCENT_RANGE; }
            set
            {
                if (this._PERCENT_RANGE != value)
                {
                    this._PERCENT_RANGE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + QTY    - ???
        private string _QTY;
        public string QTY
        {
            get { return this._QTY; }
            set
            {
                if (this._QTY != value)
                {
                    this._QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PERCENT - ???
        private string _PERCENT;
        public string PERCENT
        {
            get { return this._PERCENT; }
            set
            {
                if (this._PERCENT != value)
                {
                    this._PERCENT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TOTAL_QTY - ???
        private string _TOTAL_QTY;
        public string TOTAL_QTY
        {
            get { return this._TOTAL_QTY; }
            set
            {
                if (this._TOTAL_QTY != value)
                {
                    this._TOTAL_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion



    }
}
