using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.DataMembers
{
    public class FindSKUInfo : PropertyNotifyExtensions
    {
        string _CST_CD;
        public string CST_CD
        {
            get
            {
                return _CST_CD;
            }
            set
            {
                if (_CST_CD != value)
                {
                    _CST_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CST_NM;
        public string CST_NM
        {
            get
            {
                return _CST_NM;
            }
            set
            {
                if (_CST_NM != value)
                {
                    _CST_NM = value;
                    RaisePropertyChanged();
                }
            }
        }


        string _SKU_CD;
        public string SKU_CD
        {
            get
            {
                return _SKU_CD;
            }
            set
            {
                if (_SKU_CD != value)
                {
                    _SKU_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _SKU_NM;
        public string SKU_NM
        {
            get
            {
                return _SKU_NM;
            }
            set
            {
                if (_SKU_NM != value)
                {
                    _SKU_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

    }
}
