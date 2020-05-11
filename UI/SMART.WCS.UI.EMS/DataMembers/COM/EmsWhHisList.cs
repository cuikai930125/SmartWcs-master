using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.COM
{
    public class EmsWhHisList : PropertyNotifyExtensions
    {
        DateTime _CHK_DT;
        public DateTime CHK_DT
        {
            get
            {
                return _CHK_DT;
            }
            set
            {
                if (_CHK_DT != value)
                {
                    _CHK_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_NM;
        /// <summary>
        /// 설비명
        /// </summary>
        public string EQP_NM
        {
            get
            {
                return _EQP_NM;
            }
            set
            {
                if (_EQP_NM != value)
                {
                    _EQP_NM = value;
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
            get
            {
                return _WH_QTY;
            }
            set
            {
                if (_WH_QTY != value)
                {
                    _WH_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _OUT_QTY;
        /// <summary>
        /// 출고수량
        /// </summary>
        public decimal OUT_QTY
        {
            get
            {
                return _OUT_QTY;
            }
            set
            {
                if (_OUT_QTY != value)
                {
                    _OUT_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _REST_QTY;
        /// <summary>
        /// 잔여 수량
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
