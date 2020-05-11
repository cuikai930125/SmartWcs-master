using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.DataMembers
{
    public class FindBIZInfo : PropertyNotifyExtensions
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


        string _BIZPTNR_CD;
        public string BIZPTNR_CD
        {
            get
            {
                return _BIZPTNR_CD;
            }
            set
            {
                if (_BIZPTNR_CD != value)
                {
                    _BIZPTNR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _BIZPTNR_NM;
        public string BIZPTNR_NM
        {
            get
            {
                return _BIZPTNR_NM;
            }
            set
            {
                if (_BIZPTNR_NM != value)
                {
                    _BIZPTNR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

    }
}
