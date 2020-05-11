using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.COM
{
    public class EmsEqpClsList : PropertyNotifyExtensions
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

        int _EQP_CLS_LV;
        /// <summary>
        /// Level (Tree)
        /// </summary>
        public int EQP_CLS_LV
        {
            get
            {
                return _EQP_CLS_LV;
            }
            set
            {
                if (_EQP_CLS_LV != value)
                {
                    _EQP_CLS_LV = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PARENT_EQP_CLS_ID;
        /// <summary>
        /// 부모
        /// </summary>
        public string PARENT_EQP_CLS_ID
        {
            get
            {
                return _PARENT_EQP_CLS_ID;
            }
            set
            {
                if (_PARENT_EQP_CLS_ID != value)
                {
                    _PARENT_EQP_CLS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// 부모
        /// </summary>
        public EmsEqpClsList Parent { get; set; }
    }
}
