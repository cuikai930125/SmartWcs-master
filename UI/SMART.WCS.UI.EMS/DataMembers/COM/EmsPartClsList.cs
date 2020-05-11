using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.COM
{
    public class EmsPartClsList : PropertyNotifyExtensions
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

        string _PART_CLS_NM;
        public string PART_CLS_NM
        {
            get
            {
                return _PART_CLS_NM;
            }
            set
            {
                if (_PART_CLS_NM != value)
                {
                    _PART_CLS_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _PART_CLS_LV;
        /// <summary>
        /// Level (Tree)
        /// </summary>
        public int PART_CLS_LV
        {
            get
            {
                return _PART_CLS_LV;
            }
            set
            {
                if (_PART_CLS_LV != value)
                {
                    _PART_CLS_LV = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PARENT_PART_CLS_ID;
        /// <summary>
        /// 부모
        /// </summary>
        public string PARENT_PART_CLS_ID
        {
            get
            {
                return _PARENT_PART_CLS_ID;
            }
            set
            {
                if (_PARENT_PART_CLS_ID != value)
                {
                    _PARENT_PART_CLS_ID = value;
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
