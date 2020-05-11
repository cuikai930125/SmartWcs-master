using SMART.WCS.Common.Extensions;

using System;

namespace SMART.WCS.UI.EMS.DataMembers.ESPA001
{
    public class EmsPartSpa : PropertyNotifyExtensions
    {
        int _WH_SEQ;
        /// <summary>
        /// Index
        /// </summary>
        public int WH_SEQ
        {
            get { return _WH_SEQ; }
            set
            {
                if (_WH_SEQ != value)
                {
                    _WH_SEQ = value;
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

        DateTime _WH_DT;
        /// <summary>
        /// 입고일자
        /// </summary>
        public DateTime WH_DT
        {
            get
            {
                return _WH_DT;
            }
            set
            {
                if (_WH_DT != value)
                {
                    _WH_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Null 대응
        /// </summary>
        public DateTime? WH_DT_N
        {
            get
            {
                if (1 == WH_DT.Year)
                {
                    return null;
                }
                else
                {
                    return WH_DT;
                }
            }
            set
            {
                if (null != value && _WH_DT != value.Value)
                {
                    _WH_DT = value.Value;
                    RaisePropertyChanged();
                }
            }
        }

        int _PR_PRICE;
        /// <summary>
        /// 가격
        /// </summary>
        public int PR_PRICE
        {
            get { return _PR_PRICE; }
            set
            {
                if (_PR_PRICE != value)
                {
                    _PR_PRICE = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _WH_QTY;
        /// <summary>
        /// 입고 수량
        /// </summary>
        public decimal WH_QTY
        {
            get { return _WH_QTY; }
            set
            {
                if (_WH_QTY != value)
                {
                    _WH_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _WH_NOTE;
        /// <summary>
        /// 비고
        /// </summary>
        public string WH_NOTE
        {
            get
            {
                return _WH_NOTE;
            }
            set
            {
                if (_WH_NOTE != value)
                {
                    _WH_NOTE = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool _from_ref;
        /// <summary>
        /// 마스터 참조
        /// </summary>
        public bool FROM_REF
        {
            get
            {
                return _from_ref;
            }
            set
            {
                if (_from_ref != value)
                {
                    _from_ref = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
