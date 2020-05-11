using SMART.WCS.Common.Extensions;

namespace SMART.WCS.UI.EMS.DataMembers.E3001
{
    public class EmsChkList : PropertyNotifyExtensions
    {
        string _EQP_ID;
        /// <summary>
        /// 설비
        /// </summary>
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

        string _EQP_NM;
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

        string _PBS_ID;
        public string PBS_ID
        {
            get
            {
                return _PBS_ID;
            }
            set
            {
                if (_PBS_ID != value)
                {
                    _PBS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PBS_NM;
        public string PBS_NM
        {
            get
            {
                return _PBS_NM;
            }
            set
            {
                if (_PBS_NM != value)
                {
                    _PBS_NM = value;
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
    }
}
