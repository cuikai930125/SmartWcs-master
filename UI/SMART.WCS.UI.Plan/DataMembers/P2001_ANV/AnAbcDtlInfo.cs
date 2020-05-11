using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P2001_ANV
{
    public class AnAbcDtlInfo : PropertyNotifyExtensions
    {
        public AnAbcDtlInfo()
        {

        }

        #region + EXT_ID - 구분
        private string _EXT_ID;
        public string EXT_ID
        {
            get { return this._EXT_ID; }
            set
            {
                if (this._EXT_ID != value)
                {
                    this._EXT_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ABC_CODE - ABC
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

        #region + FROM_PER_RANGE - FROM
        private int _FROM_PER_RANGE;
        public int FROM_PER_RANGE
        {
            get { return this._FROM_PER_RANGE; }
            set
            {
                if (this._FROM_PER_RANGE != value)
                {
                    this._FROM_PER_RANGE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TO_PER_RANGE - TO
        private int _TO_PER_RANGE;
        public int TO_PER_RANGE
        {
            get { return this._TO_PER_RANGE; }
            set
            {
                if (this._TO_PER_RANGE != value)
                {
                    this._TO_PER_RANGE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
