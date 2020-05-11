using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.COM
{
    public class EmsEqpList : PropertyNotifyExtensions
    {
        string _EQP_CLS_ID;
        /// <summary>
        /// 분류 ID
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

        string _EQP_ID;
        /// <summary>
        /// 설비 ID
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

        string _PBS_ID;
        /// <summary>
        /// 설비 위치
        /// </summary>
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
    }
}
