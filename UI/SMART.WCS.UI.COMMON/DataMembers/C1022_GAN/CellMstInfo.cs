using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.COMMON.DataMembers.C1022_GAN
{
    public class CellMstInfo : PropertyNotifyExtensions
    {
        public CellMstInfo()
        {

        }

        #region + CO_CD - ???
        private string _CO_CD;
        public string CO_CD
        {
            get { return this._CO_CD; }
            set
            {
                if (this._CO_CD != value)
                {
                    this._CO_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CNTR_CD - ???
        private string _CNTR_CD;
        public string CNTR_CD
        {
            get { return this._CNTR_CD; }
            set
            {
                if (this._CNTR_CD != value)
                {
                    this._CNTR_CD = value;
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

        #region + CELL_ID - ???
        private string _CELL_ID;
        public string CELL_ID
        {
            get { return this._CELL_ID; }
            set
            {
                if (this._CELL_ID != value)
                {
                    this._CELL_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + COL_NO - ???
        private string _COL_NO;
        public string COL_NO
        {
            get { return this._COL_NO; }
            set
            {
                if (this._COL_NO != value)
                {
                    this._COL_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ROW_NO - ???
        private string _ROW_NO;
        public string ROW_NO
        {
            get { return this._ROW_NO; }
            set
            {
                if (this._ROW_NO != value)
                {
                    this._ROW_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + POS_X - ???
        private string _POS_X;
        public string POS_X
        {
            get { return this._POS_X; }
            set
            {
                if (this._POS_X != value)
                {
                    this._POS_X = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + POS_Y - ???
        private string _POS_Y;
        public string POS_Y
        {
            get { return this._POS_Y; }
            set
            {
                if (this._POS_Y != value)
                {
                    this._POS_Y = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + P1_ACCS_YN - ???
        private string _P1_ACCS_YN;
        public string P1_ACCS_YN
        {
            get { return this._P1_ACCS_YN; }
            set
            {
                if (this._P1_ACCS_YN != value)
                {
                    this._P1_ACCS_YN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + P2_ACCS_YN - ???
        private string _P2_ACCS_YN;
        public string P2_ACCS_YN
        {
            get { return this._P2_ACCS_YN; }
            set
            {
                if (this._P2_ACCS_YN != value)
                {
                    this._P2_ACCS_YN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CELL_TYPE_CD - ???
        private string _CELL_TYPE_CD;
        public string CELL_TYPE_CD
        {
            get { return this._CELL_TYPE_CD; }
            set
            {
                if (this._CELL_TYPE_CD != value)
                {
                    this._CELL_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CELL_TYPE_NM - ???
        private string _CELL_TYPE_NM;
        public string CELL_TYPE_NM
        {
            get { return this._CELL_TYPE_NM; }
            set
            {
                if (this._CELL_TYPE_NM != value)
                {
                    this._CELL_TYPE_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion


        #region + BASIC_CELL_HGT - 셀기본높이
        private string _BASIC_CELL_HGT;
        public string BASIC_CELL_HGT
        {
            get { return this._BASIC_CELL_HGT; }
            set
            {
                if (this._BASIC_CELL_HGT != value)
                {
                    this._BASIC_CELL_HGT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MAX_CELL_HGT - 셀최대높이
        private string _MAX_CELL_HGT;
        public string MAX_CELL_HGT
        {
            get { return this._MAX_CELL_HGT; }
            set
            {
                if (this._MAX_CELL_HGT != value)
                {
                    this._MAX_CELL_HGT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + USE_YN  - ???
        private string _USE_YN;
        public string USE_YN
        {
            get { return this._USE_YN; }
            set
            {
                if (this._USE_YN != value)
                {
                    this._USE_YN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion


    }
}
