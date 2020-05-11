using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P2001_ANV_01P
{
    public class ObExtCapaInfo : PropertyNotifyExtensions
    {
        public ObExtCapaInfo()
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

        #region + EXT_DATE - ???
        private string _EXT_DATE;
        public string EXT_DATE
        {
            get { return this._EXT_DATE; }
            set
            {
                if (this._EXT_DATE != value)
                {
                    this._EXT_DATE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + AGG_TYPE_CD1 - ???
        private string _AGG_TYPE_CD1;
        public string AGG_TYPE_CD1
        {
            get { return this._AGG_TYPE_CD1; }
            set
            {
                if (this._AGG_TYPE_CD1 != value)
                {
                    this._AGG_TYPE_CD1 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + AGG_TYPE_CD2 - ???
        private string _AGG_TYPE_CD2;
        public string AGG_TYPE_CD2
        {
            get { return this._AGG_TYPE_CD2; }
            set
            {
                if (this._AGG_TYPE_CD2 != value)
                {
                    this._AGG_TYPE_CD2 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + AGG_TYPE_CD3 - ???
        private string _AGG_TYPE_CD3;
        public string AGG_TYPE_CD3
        {
            get { return this._AGG_TYPE_CD3; }
            set
            {
                if (this._AGG_TYPE_CD3 != value)
                {
                    this._AGG_TYPE_CD3 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + AGG_TYPE_CD4 - ???
        private string _AGG_TYPE_CD4;
        public string AGG_TYPE_CD4
        {
            get { return this._AGG_TYPE_CD4; }
            set
            {
                if (this._AGG_TYPE_CD4 != value)
                {
                    this._AGG_TYPE_CD4 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PLT_QTY - ???
        private string _PLT_QTY;
        public string PLT_QTY
        {
            get { return this._PLT_QTY; }
            set
            {
                if (this._PLT_QTY != value)
                {
                    this._PLT_QTY = value;
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

        #region + EA_QTY - ???
        private string _EA_QTY;
        public string EA_QTY
        {
            get { return this._EA_QTY; }
            set
            {
                if (this._EA_QTY != value)
                {
                    this._EA_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion



    }
}
