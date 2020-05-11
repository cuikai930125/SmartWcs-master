using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.COM
{
    public class EmsAppdFile : PropertyNotifyExtensions
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

        string _APPD_FILE_GROUP_CD;
        /// <summary>
        /// File Group
        /// </summary>
        public string APPD_FILE_GROUP_CD
        {
            get
            {
                return _APPD_FILE_GROUP_CD;
            }
            set
            {
                if (_APPD_FILE_GROUP_CD != value)
                {
                    _APPD_FILE_GROUP_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _APPD_FILE_GUID;
        /// <summary>
        /// GUID
        /// </summary>
        public string APPD_FILE_GUID
        {
            get
            {
                return _APPD_FILE_GUID;
            }
            set
            {
                if (_APPD_FILE_GUID != value)
                {
                    _APPD_FILE_GUID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _APPD_FILE_NM;
        public string APPD_FILE_NM
        {
            get
            {
                return _APPD_FILE_NM;
            }
            set
            {
                if (_APPD_FILE_NM != value)
                {
                    _APPD_FILE_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// File Name Only
        /// </summary>
        public string APPD_FILE_NM_V
        {
            get { return System.IO.Path.GetFileName(APPD_FILE_NM); }
        }


        string _APPD_FILE_DEV_CD;
        /// <summary>
        /// 구분
        /// </summary>
        public string APPD_FILE_DEV_CD
        {
            get
            {
                return _APPD_FILE_DEV_CD;
            }
            set
            {
                if (_APPD_FILE_DEV_CD != value)
                {
                    _APPD_FILE_DEV_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
