using SMART.WCS.Common.Extensions;

namespace SMART.WCS.UI.EMS.DataMembers.ECHK003
{
    public class EmsAlarmStatus : PropertyNotifyExtensions
    {
        string _CENTER_CD;
        public string CENTER_CD
        {
            get
            {
                return _CENTER_CD;
            }
            set
            {
                if (_CENTER_CD != value)
                {
                    _CENTER_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_MNGR_ID;
        /// <summary>
        /// 담당자
        /// </summary>
        public string EQP_MNGR_ID
        {
            get
            {
                return _EQP_MNGR_ID;
            }
            set
            {
                if (_EQP_MNGR_ID != value)
                {
                    _EQP_MNGR_ID = value;
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



        string _LIFE_CLE;
        /// <summary>
        /// 주기
        /// </summary>
        public string LIFE_CLE
        {
            get
            {
                return _LIFE_CLE;
            }
            set
            {
                if (_LIFE_CLE != value)
                {
                    _LIFE_CLE = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_STND;
        /// <summary>
        /// 규격
        /// </summary>
        public string PART_STND
        {
            get
            {
                return _PART_STND;
            }
            set
            {
                if (_PART_STND != value)
                {
                    _PART_STND = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _CHG_QTY;
        /// <summary>
        /// 교체수량
        /// </summary>
        public decimal CHG_QTY
        {
            get
            {
                return _CHG_QTY;
            }
            set
            {
                if (_CHG_QTY != value)
                {
                    _CHG_QTY = value;
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
            get
            {
                return _STOCK_QTY;
            }
            set
            {
                if (_STOCK_QTY != value)
                {
                    _STOCK_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _REST_QTY;
        /// <summary>
        /// 잔여수량
        /// </summary>
        public decimal REST_QTY
        {
            get
            {
                return _REST_QTY;
            }
            set
            {
                if (_REST_QTY != value)
                {
                    _REST_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
