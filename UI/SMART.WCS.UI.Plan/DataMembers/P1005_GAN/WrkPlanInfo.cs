using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P1005_GAN
{
    public class WrkPlanInfo : PropertyNotifyExtensions
    {
        public WrkPlanInfo()
        {
        
        }

        #region + BTCH_NO - ???
        private string _BTCH_NO;
        public string BTCH_NO
        {
            get { return this._BTCH_NO; }
            set
            {
                if (this._BTCH_NO != value)
                {
                    this._BTCH_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + IN_PLAN_YMD - ???
        private string _IN_PLAN_YMD;
        public string IN_PLAN_YMD
        {
            get { return this._IN_PLAN_YMD; }
            set
            {
                if (this._IN_PLAN_YMD != value)
                {
                    this._IN_PLAN_YMD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TOT_BOX_ID - ???
        private string _TOT_BOX_ID;
        public string TOT_BOX_ID
        {
            get { return this._TOT_BOX_ID; }
            set
            {
                if (this._TOT_BOX_ID != value)
                {
                    this._TOT_BOX_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ASN_NO - ???
        private string _ASN_NO;
        public string ASN_NO
        {
            get { return this._ASN_NO; }
            set
            {
                if (this._ASN_NO != value)
                {
                    this._ASN_NO = value;
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

        #region + SKU_NM - ???
        private string _SKU_NM;
        public string SKU_NM
        {
            get { return this._SKU_NM; }
            set
            {
                if (this._SKU_NM != value)
                {
                    this._SKU_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_IN_QTY - ???
        private string _BOX_IN_QTY;
        public string BOX_IN_QTY
        {
            get { return this._BOX_IN_QTY; }
            set
            {
                if (this._BOX_IN_QTY != value)
                {
                    this._BOX_IN_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_QTY - ???
        private string _BOX_QTY;
        public string BOX_QTY
        {
            get { return this._BOX_QTY; }
            set
            {
                if (this._BOX_QTY != value)
                {
                    this._BOX_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PLAN_QTY - ???
        private string _PLAN_QTY;
        public string PLAN_QTY
        {
            get { return this._PLAN_QTY; }
            set
            {
                if (this._PLAN_QTY != value)
                {
                    this._PLAN_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + WRK_STAT_CD - ???
        private string _WRK_STAT_CD;
        public string WRK_STAT_CD
        {
            get { return this._WRK_STAT_CD; }
            set
            {
                if (this._WRK_STAT_CD != value)
                {
                    this._WRK_STAT_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_ID - ???
        private string _EQP_ID;
        public string EQP_ID
        {
            get { return this._EQP_ID; }
            set
            {
                if (this._EQP_ID != value)
                {
                    this._EQP_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_NM - ???
        private string _EQP_NM;
        public string EQP_NM
        {
            get { return this._EQP_NM; }
            set
            {
                if (this._EQP_NM != value)
                {
                    this._EQP_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + STK_NO - ???
        private string _STK_NO;
        public string STK_NO
        {
            get { return this._STK_NO; }
            set
            {
                if (this._STK_NO != value)
                {
                    this._STK_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

    }
}