using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.COM
{
    public class EmsPbsList : PropertyNotifyExtensions
    {
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

        #region Tree 구조
        string _PARENT_PBS_ID;
        /// <summary>
        /// Parent
        /// </summary>
        public string PARENT_PBS_ID
        {
            get
            {
                return _PARENT_PBS_ID;
            }
            set
            {
                if (_PARENT_PBS_ID != value)
                {
                    _PARENT_PBS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _PBS_LV;
        /// <summary>
        /// Level
        /// </summary>
        public int PBS_LV
        {
            get
            {
                return _PBS_LV;
            }
            set
            {
                if (_PBS_LV != value)
                {
                    _PBS_LV = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Parent
        /// </summary>
        public EmsPbsList Parent { get; set; }
        #endregion Tree 구조
    }
}
