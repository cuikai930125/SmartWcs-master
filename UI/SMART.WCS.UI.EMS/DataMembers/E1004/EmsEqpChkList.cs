using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.E1004
{
    public class EmsEqpChkList : PropertyNotifyExtensions
    {
        public bool FromMaster { get; set; }


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

        string _EQP_ID;
        public string EQP_ID
        {
            get
            {
                return _EQP_ID;
            }
            set
            {
                if (_EQP_ID != value)
                {
                    _EQP_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _CHK_SERIAL_NO;
        public int CHK_SERIAL_NO
        {
            get
            {
                return _CHK_SERIAL_NO;
            }
            set
            {
                if (_CHK_SERIAL_NO != value)
                {
                    _CHK_SERIAL_NO = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_NOTE;
        /// <summary>
        /// 점검 방법
        /// </summary>
        public string CHK_NOTE
        {
            get
            {
                return _CHK_NOTE;
            }
            set
            {
                if (_CHK_NOTE != value)
                {
                    _CHK_NOTE = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _BASE_VAL;
        /// <summary>
        /// 기준값
        /// </summary>
        public string BASE_VAL
        {
            get
            {
                return _BASE_VAL;
            }
            set
            {
                if (_BASE_VAL != value)
                {
                    _BASE_VAL = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _UPPER_VAL;
        /// <summary>
        /// 상한값
        /// </summary>
        public string UPPER_VAL
        {
            get
            {
                return _UPPER_VAL;
            }
            set
            {
                if (_UPPER_VAL != value)
                {
                    _UPPER_VAL = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _LOWER_VAL;
        /// <summary>
        /// 하한값
        /// </summary>
        public string LOWER_VAL
        {
            get
            {
                return _LOWER_VAL;
            }
            set
            {
                if (_LOWER_VAL != value)
                {
                    _LOWER_VAL = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_ITEM;
        /// <summary>
        /// 점검 항목
        /// </summary>
        public string CHK_ITEM
        {
            get
            {
                return _CHK_ITEM;
            }
            set
            {
                if (_CHK_ITEM != value)
                {
                    _CHK_ITEM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_UNIT;
        /// <summary>
        /// 단위
        /// </summary>
        public string CHK_UNIT
        {
            get
            {
                return _CHK_UNIT;
            }
            set
            {
                if (_CHK_UNIT != value)
                {
                    _CHK_UNIT = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _REG_DT;
        public DateTime REG_DT
        {
            get
            {
                return _REG_DT;
            }
            set
            {
                if (_REG_DT != value)
                {
                    _REG_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _RSTR_ID;
        public string RSTR_ID
        {
            get
            {
                return _RSTR_ID;
            }
            set
            {
                if (_RSTR_ID != value)
                {
                    _RSTR_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _UPD_DT;
        public DateTime UPD_DT
        {
            get
            {
                return _UPD_DT;
            }
            set
            {
                if (_UPD_DT != value)
                {
                    _UPD_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _UPDR_ID;
        public string UPDR_ID
        {
            get
            {
                return _UPDR_ID;
            }
            set
            {
                if (_UPDR_ID != value)
                {
                    _UPDR_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
