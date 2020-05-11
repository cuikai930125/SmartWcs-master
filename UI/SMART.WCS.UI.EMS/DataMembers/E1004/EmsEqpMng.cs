using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.E1004
{
    public class EmsEqpMng : PropertyNotifyExtensions
    {
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

        string _EQP_CLS_ID;
        /// <summary>
        /// 설비 분류
        /// </summary>
        public string EQP_CLS_ID
        {
            get
            {
                return _EQP_CLS_ID;
            }
            set
            {
                if (_EQP_CLS_ID != value)
                {
                    _EQP_CLS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_CLS_NM;
        public string EQP_CLS_NM
        {
            get
            {
                return _EQP_CLS_NM;
            }
            set
            {
                if (_EQP_CLS_NM != value)
                {
                    _EQP_CLS_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _ORDER_DEV;
        /// <summary>
        /// 분류
        /// </summary>
        public string ORDER_DEV
        {
            get
            {
                return _ORDER_DEV;
            }
            set
            {
                if (_ORDER_DEV != value)
                {
                    _ORDER_DEV = value;
                    RaisePropertyChanged();
                }
            }
        }

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

        #region Tree 구조
        string _PARENT_ID;
        /// <summary>
        /// 부모
        /// </summary>
        public string PARENT_ID
        {
            get
            {
                return _PARENT_ID;
            }
            set
            {
                if (_PARENT_ID != value)
                {
                    _PARENT_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _CLS_LV;
        /// <summary>
        /// Level
        /// </summary>
        public int CLS_LV
        {
            get { return _CLS_LV; }
            set
            {
                if (_CLS_LV != value)
                {
                    _CLS_LV = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion Tree 구조

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

        DateTime _INST_DT;
        /// <summary>
        /// 설치 일자
        /// </summary>
        public DateTime INST_DT
        {
            get
            {
                return _INST_DT;
            }
            set
            {
                if (_INST_DT != value)
                {
                    _INST_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Null 대응
        /// </summary>
        public DateTime? INST_DT_N
        {
            get
            {
                if (1 == INST_DT.Year)
                {
                    return null;
                }
                else
                {
                    return INST_DT;
                }
            }
        }

        string _EQP_STND;
        /// <summary>
        /// 설비 규격
        /// </summary>
        public string EQP_STND
        {
            get
            {
                return _EQP_STND;
            }
            set
            {
                if (_EQP_STND != value)
                {
                    _EQP_STND = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_MODEL;
        public string EQP_MODEL
        {
            get
            {
                return _EQP_MODEL;
            }
            set
            {
                if (_EQP_MODEL != value)
                {
                    _EQP_MODEL = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _USE_YN;
        public string USE_YN
        {
            get
            {
                return _USE_YN;
            }
            set
            {
                if (_USE_YN != value)
                {
                    _USE_YN = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _MTTR;
        public string MTTR
        {
            get
            {
                return _MTTR;
            }
            set
            {
                if (_MTTR != value)
                {
                    _MTTR = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _MTBF;
        public string MTBF
        {
            get
            {
                return _MTBF;
            }
            set
            {
                if (_MTBF != value)
                {
                    _MTBF = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
