using SMART.WCS.Common.Extensions;

using System;

namespace SMART.WCS.UI.EMS.DataMembers.ESPA002
{
    public class EmsStockDetail : PropertyNotifyExtensions
    {
        //DateTime _STOCK_INSP_DT;
        /// <summary>
        /// 재고조사일자
        /// </summary>
        //public DateTime STOCK_INSP_DT
        //{
        //    get
        //    {
        //        return _STOCK_INSP_DT;
        //    }
        //    set
        //    {
        //        if (_STOCK_INSP_DT != value)
        //        {
        //            _STOCK_INSP_DT = value;
        //            RaisePropertyChanged();
        //        }
        //    }
        //}

        string _STOCK_INSP_DT;
        public string STOCK_INSP_DT
        {
            get
            {
                return _STOCK_INSP_DT;
            }
            set
            {
                if (_STOCK_INSP_DT != value)
                {
                    if (value.Length == 8)
                    {
                        _STOCK_INSP_DT = $"{value.Substring(0, 4)}-{value.Substring(4,2)}-{value.Substring(6,2)}";
                    }

                    //_STOCK_INSP_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_ID;
        /// <summary>
        /// 부품
        /// </summary>
        public string PART_ID
        {
            get
            {
                return _PART_ID;
            }
            set
            {
                if (_PART_ID != value)
                {
                    _PART_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_NM;
        public string PART_NM
        {
            get
            {
                return _PART_NM;
            }
            set
            {
                if (_PART_NM != value)
                {
                    _PART_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_MNFACT;
        public string PART_MNFACT
        {
            get
            {
                return _PART_MNFACT;
            }
            set
            {
                if (_PART_MNFACT != value)
                {
                    _PART_MNFACT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_MODEL;
        public string PART_MODEL
        {
            get
            {
                return _PART_MODEL;
            }
            set
            {
                if (_PART_MODEL != value)
                {
                    _PART_MODEL = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _STOCK_QTY;
        /// <summary>
        /// 재고수량
        /// </summary>
        public decimal STOCK_QTY
        {
            get { return _STOCK_QTY; }
            set
            {
                if (_STOCK_QTY != value)
                {
                    _STOCK_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _INSP_QTY;
        /// <summary>
        /// 조사수량
        /// </summary>
        public decimal INSP_QTY
        {
            get { return _INSP_QTY; }
            set
            {
                if (_INSP_QTY != value)
                {
                    _INSP_QTY = value;
                    RaisePropertyChanged();

                    //if(0 < STOCK_QTY)
                    {
                        LOSS_QTY = STOCK_QTY - INSP_QTY;
                    }
                }
            }
        }

        decimal _LOSS_QTY;
        /// <summary>
        /// 망실 수량
        /// </summary>
        public decimal LOSS_QTY
        {
            get { return _LOSS_QTY; }
            set
            {
                if (_LOSS_QTY != value)
                {
                    _LOSS_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _NOTE;
        /// <summary>
        /// 비고
        /// </summary>
        public string NOTE
        {
            get
            {
                return _NOTE;
            }
            set
            {
                if (_NOTE != value)
                {
                    _NOTE = value;
                    RaisePropertyChanged();
                }
            }
        }


        string _STOCK_MNG_YN;
        /// <summary>
        /// 재고관리여부
        /// </summary>
        public string STOCK_MNG_YN
        {
            get
            {
                return _STOCK_MNG_YN;
            }
            set
            {
                if (_STOCK_MNG_YN != value)
                {
                    _STOCK_MNG_YN = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
