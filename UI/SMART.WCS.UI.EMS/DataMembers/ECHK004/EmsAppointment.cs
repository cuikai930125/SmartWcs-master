using SMART.WCS.Common.Extensions;

using System;

namespace SMART.WCS.UI.EMS.DataMembers.ECHK004
{
    public class EmsAppointment : PropertyNotifyExtensions
    {
        int _PK_ID;
        public int PK_ID
        {
            get
            {
                return _PK_ID;
            }
            set
            {
                if (_PK_ID != value)
                {
                    _PK_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

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



        DateTime _CHECK_DT;
        public DateTime CHECK_DT
        {
            get
            {
                return _CHECK_DT;
            }
            set
            {
                if (_CHECK_DT != value)
                {
                    _CHECK_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _END_DT;
        public DateTime END_DT
        {
            get
            {
                return _END_DT;
            }
            set
            {
                if (_END_DT != value)
                {
                    _END_DT = value;
                    RaisePropertyChanged();
                }
            }
        }



        string _CHECK_NM;
        public string CHECK_NM
        {
            get
            {
                return _CHECK_NM;
            }
            set
            {
                if (_CHECK_NM != value)
                {
                    _CHECK_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _STATUS_ID;
        public int STATUS_ID
        {
            get
            {
                return _STATUS_ID;
            }
            set
            {
                if (_STATUS_ID != value)
                {
                    _STATUS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }



        int _CATEGORY_ID;
        public int CATEGORY_ID
        {
            get
            {
                return _CATEGORY_ID;
            }
            set
            {
                if (_CATEGORY_ID != value)
                {
                    _CATEGORY_ID = value;
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

        // key id (check id, fail id )
        int _CHK_IDNO;
        public int CHK_IDNO
        {
            get
            {
                return _CHK_IDNO;
            }
            set
            {
                if (_CHK_IDNO != value)
                {
                    _CHK_IDNO = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
