using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.COM
{
    public class EmsChkList : PropertyNotifyExtensions
    {
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

        int _CHK_NO;
        public int CHK_NO
        {
            get
            {
                return _CHK_NO;
            }
            set
            {
                if (_CHK_NO != value)
                {
                    _CHK_NO = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_CONTENT;
        /// <summary>
        /// 내용
        /// </summary>
        public string CHK_CONTENT
        {
            get
            {
                return _CHK_CONTENT;
            }
            set
            {
                if (_CHK_CONTENT != value)
                {
                    _CHK_CONTENT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_NOTE;
        /// <summary>
        /// 비고
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
