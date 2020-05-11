using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Processing.DataMembers.R1008_GAN
{
    public class StockHistInfo : PropertyNotifyExtensions
    {
        public StockHistInfo()
        {

        }

        #region + WRK_DT - 작업일시
        private string _WRK_DT;
        public string WRK_DT
        {
            get { return this._WRK_DT; }
            set
            {
                if (this._WRK_DT != value)
                {
                    this._WRK_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + WRK_TYPE - 작업타입
        private string _WRK_TYPE;
        public string WRK_TYPE
        {
            get { return this._WRK_TYPE; }
            set
            {
                if (this._WRK_TYPE != value)
                {
                    this._WRK_TYPE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + WRK_TYPE_NM - 작업타입명
        private string _WRK_TYPE_NM;
        public string WRK_TYPE_NM
        {
            get { return this._WRK_TYPE_NM; }
            set
            {
                if (this._WRK_TYPE_NM != value)
                {
                    this._WRK_TYPE_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
        #region + EQP_ID - 설비 ID
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

        #region + EQP_ID - 설비명
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

        #region + CELL_ID - 셀 ID
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

        #region + CELL_TYPE_CD - 셀타입코드
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

        #region + CELL_TYPE_NM - 셀타입명
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

        #region + STK_HGT - 적재높이
        private string _STK_HGT;
        public string STK_HGT
        {
            get { return this._STK_HGT; }
            set
            {
                if (this._STK_HGT != value)
                {
                    this._STK_HGT = value;
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

        #region + SKU_CD - 상품코드
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

        #region + SKU_NM - 상품명
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

        #region + TOT_BOX_ID - 토트박스 ID
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

        #region + BOX_TYPE_CD - 토트박스 타입
        private string _BOX_TYPE_CD;
        public string BOX_TYPE_CD
        {
            get { return this._BOX_TYPE_CD; }
            set
            {
                if (this._BOX_TYPE_CD != value)
                {
                    this._BOX_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + STOCK_QTY - 적재수량
        private string _STOCK_QTY;
        public string STOCK_QTY
        {
            get { return this._STOCK_QTY; }
            set
            {
                if (this._STOCK_QTY != value)
                {
                    this._STOCK_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion


    }
}
