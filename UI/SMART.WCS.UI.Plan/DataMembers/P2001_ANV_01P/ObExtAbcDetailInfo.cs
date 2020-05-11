using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P2001_ANV_01P
{
    public class ObExtAbcDetailInfo : PropertyNotifyExtensions
    {
        public ObExtAbcDetailInfo()
        {

        }

        #region + EXT_ID - ???
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

        #region + AGG_ID - ???
        private string _AGG_ID;
        public string AGG_ID
        {
            get { return this._AGG_ID; }
            set
            {
                if (this._AGG_ID != value)
                {
                    this._AGG_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_CD - ???
        private string _SKU_CD;
        public string SKU_CD
        {
            get { return this._SKU_CD; }
            set
            {
                if (this._SKU_CD != value)
                {
                    this._SKU_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_RANK - ???
        private string _EXT_RANK;
        public string EXT_RANK
        {
            get { return this._EXT_RANK; }
            set
            {
                if (this._EXT_RANK != value)
                {
                    this._EXT_RANK = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_QTY - ???
        private string _EXT_QTY;
        public string EXT_QTY
        {
            get { return this._EXT_QTY; }
            set
            {
                if (this._EXT_QTY != value)
                {
                    this._EXT_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_ACCM_QTY - ???
        private string _EXT_ACCM_QTY;
        public string EXT_ACCM_QTY
        {
            get { return this._EXT_ACCM_QTY; }
            set
            {
                if (this._EXT_ACCM_QTY != value)
                {
                    this._EXT_ACCM_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TOT_EXT_QTY - ???
        private string _TOT_EXT_QTY;
        public string TOT_EXT_QTY
        {
            get { return this._TOT_EXT_QTY; }
            set
            {
                if (this._TOT_EXT_QTY != value)
                {
                    this._TOT_EXT_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_ACCM_QTY_PER - ???
        private string _EXT_ACCM_QTY_PER;
        public string EXT_ACCM_QTY_PER
        {
            get { return this._EXT_ACCM_QTY_PER; }
            set
            {
                if (this._EXT_ACCM_QTY_PER != value)
                {
                    this._EXT_ACCM_QTY_PER = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_ACCM_RANK_PER - ???
        private string _EXT_ACCM_RANK_PER;
        public string EXT_ACCM_RANK_PER
        {
            get { return this._EXT_ACCM_RANK_PER; }
            set
            {
                if (this._EXT_ACCM_RANK_PER != value)
                {
                    this._EXT_ACCM_RANK_PER = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

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



    }
}
