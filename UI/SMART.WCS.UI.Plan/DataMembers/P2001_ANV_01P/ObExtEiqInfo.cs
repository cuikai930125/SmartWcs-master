using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P2001_ANV_01P
{
    public class ObExtEiqInfo : PropertyNotifyExtensions
    {
        public ObExtEiqInfo()
        {

        }

        #region + EXT_ID - 추출ID
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

        #region + EXT_DATE - 추출일자
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

        #region + ORD_CNT - 주문수 - 총량
        private string _ORD_CNT;
        public string ORD_CNT
        {
            get { return this._ORD_CNT; }
            set
            {
                if (this._ORD_CNT != value)
                {
                    this._ORD_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ORD_DD_AVG_CNT - 주문수 - 일평균
        private string _ORD_DD_AVG_CNT;
        public string ORD_DD_AVG_CNT
        {
            get { return this._ORD_DD_AVG_CNT; }
            set
            {
                if (this._ORD_DD_AVG_CNT != value)
                {
                    this._ORD_DD_AVG_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ORD_DD_MAX_CNT - 주문수 - 일MAX
        private string _ORD_DD_MAX_CNT;
        public string ORD_DD_MAX_CNT
        {
            get { return this._ORD_DD_MAX_CNT; }
            set
            {
                if (this._ORD_DD_MAX_CNT != value)
                {
                    this._ORD_DD_MAX_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ORD_LINE_CNT - ???
        private string _ORD_LINE_CNT;
        public string ORD_LINE_CNT
        {
            get { return this._ORD_LINE_CNT; }
            set
            {
                if (this._ORD_LINE_CNT != value)
                {
                    this._ORD_LINE_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_CNT - 상품수 - 총량
        private string _SKU_CNT;
        public string SKU_CNT
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

        #region + SKU_DD_AVG_CNT - 상품수 - 일평균
        private string _SKU_DD_AVG_CNT;
        public string SKU_DD_AVG_CNT
        {
            get { return this._SKU_DD_AVG_CNT; }
            set
            {
                if (this._SKU_DD_AVG_CNT != value)
                {
                    this._SKU_DD_AVG_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_DD_MAX_CNT - 상품수 - 일MAX
        private string _SKU_DD_MAX_CNT;
        public string SKU_DD_MAX_CNT
        {
            get { return this._SKU_DD_MAX_CNT; }
            set
            {
                if (this._SKU_DD_MAX_CNT != value)
                {
                    this._SKU_DD_MAX_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SUM_QTY - 주문수량 - 총량
        private string _SUM_QTY;
        public string SUM_QTY
        {
            get { return this._SUM_QTY; }
            set
            {
                if (this._SUM_QTY != value)
                {
                    this._SUM_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SUM_DD_AVG_QTY - 주문수량 - 일평균
        private string _SUM_DD_AVG_QTY;
        public string SUM_DD_AVG_QTY
        {
            get { return this._SUM_DD_AVG_QTY; }
            set
            {
                if (this._SUM_DD_AVG_QTY != value)
                {
                    this._SUM_DD_AVG_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SUM_DD_MAX_QTY - 주문수량 - 일MAX
        private string _SUM_DD_MAX_QTY;
        public string SUM_DD_MAX_QTY
        {
            get { return this._SUM_DD_MAX_QTY; }
            set
            {
                if (this._SUM_DD_MAX_QTY != value)
                {
                    this._SUM_DD_MAX_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + AVG_SKU_CNT_BY_ORD - 주문당 평균상품수
        private string _AVG_SKU_CNT_BY_ORD;
        public string AVG_SKU_CNT_BY_ORD
        {
            get { return this._AVG_SKU_CNT_BY_ORD; }
            set
            {
                if (this._AVG_SKU_CNT_BY_ORD != value)
                {
                    this._AVG_SKU_CNT_BY_ORD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + AVG_QTY_BY_ORD - 주문당 평균상품수량
        private string _AVG_QTY_BY_ORD;
        public string AVG_QTY_BY_ORD
        {
            get { return this._AVG_QTY_BY_ORD; }
            set
            {
                if (this._AVG_QTY_BY_ORD != value)
                {
                    this._AVG_QTY_BY_ORD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + AVG_QTY_BY_SKU - 상품당 평균 출하수량
        private string _AVG_QTY_BY_SKU;
        public string AVG_QTY_BY_SKU
        {
            get { return this._AVG_QTY_BY_SKU; }
            set
            {
                if (this._AVG_QTY_BY_SKU != value)
                {
                    this._AVG_QTY_BY_SKU = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
