using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.COM
{
    public class EmsPartRef : PropertyNotifyExtensions
    {
        string _PART_CLS_ID;
        /// <summary>
        /// 분류 ID
        /// </summary>
        public string PART_CLS_ID
        {
            get
            {
                return _PART_CLS_ID;
            }
            set
            {
                if (_PART_CLS_ID != value)
                {
                    _PART_CLS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CLS_DEV;
        /// <summary>
        /// 구분
        /// </summary>
        public string CLS_DEV
        {
            get
            {
                return _CLS_DEV;
            }
            set
            {
                if (_CLS_DEV != value)
                {
                    _CLS_DEV = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_ID;
        /// <summary>
        /// 부품 ID
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

        string _KEY_ID;
        /// <summary>
        /// Key (Tree)
        /// </summary>
        public string KEY_ID
        {
            get
            {
                return _KEY_ID;
            }
            set
            {
                if (_KEY_ID != value)
                {
                    _KEY_ID = value;
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

        int _LIFE_CLE;
        /// <summary>
        /// 수명 주기
        /// </summary>
        public int LIFE_CLE
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
        /// 재고 수량
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

        string _PART_MNFACT;
        /// <summary>
        /// 제조사
        /// </summary>
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
    }
}
