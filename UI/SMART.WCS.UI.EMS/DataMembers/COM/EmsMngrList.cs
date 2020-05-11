using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.COM
{
    public class EmsMngrList : PropertyNotifyExtensions
    {
        string _EQP_MNGR_ID;
        public string EQP_MNGR_ID
        {
            get
            {
                return _EQP_MNGR_ID;
            }
            set
            {
                if (_EQP_MNGR_ID != value)
                {
                    _EQP_MNGR_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _MNGR_NM;
        public string MNGR_NM
        {
            get
            {
                return _MNGR_NM;
            }
            set
            {
                if (_MNGR_NM != value)
                {
                    _MNGR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _MNGR_TEL_NO;
        public string MNGR_TEL_NO
        {
            get
            {
                return _MNGR_TEL_NO;
            }
            set
            {
                if (_MNGR_TEL_NO != value)
                {
                    _MNGR_TEL_NO = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _VNDR_NM;
        /// <summary>
        /// 업체명
        /// </summary>
        public string VNDR_NM
        {
            get
            {
                return _VNDR_NM;
            }
            set
            {
                if (_VNDR_NM != value)
                {
                    _VNDR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
