using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.COM
{
    public class EmsErrHis : PropertyNotifyExtensions
    {
        DateTime _FALL_DT;
        /// <summary>
        /// 장애 일시
        /// </summary>
        public DateTime FALL_DT
        {
            get
            {
                return _FALL_DT;
            }
            set
            {
                if (_FALL_DT != value)
                {
                    _FALL_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Null 값 대응
        /// </summary>
        public DateTime? FALL_DT_N
        {
            get
            {
                if (1 == FALL_DT.Year)
                {
                    return null;
                }
                else
                {
                    return FALL_DT;
                }
            }
        }

        string _FAIL_MNGR_ID;
        /// <summary>
        /// 담당자
        /// </summary>
        public string FAIL_MNGR_ID
        {
            get
            {
                return _FAIL_MNGR_ID;
            }
            set
            {
                if (_FAIL_MNGR_ID != value)
                {
                    _FAIL_MNGR_ID = value;
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

        string _FAIL_NOTE;
        /// <summary>
        /// 장애 내용
        /// </summary>
        public string FAIL_NOTE
        {
            get
            {
                return _FAIL_NOTE;
            }
            set
            {
                if (_FAIL_NOTE != value)
                {
                    _FAIL_NOTE = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _FAIL_REASON;
        /// <summary>
        /// 장애 사유
        /// </summary>
        public string FAIL_REASON
        {
            get
            {
                return _FAIL_REASON;
            }
            set
            {
                if (_FAIL_REASON != value)
                {
                    _FAIL_REASON = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _MSR_NOTE;
        /// <summary>
        /// 조치 사항
        /// </summary>
        public string MSR_NOTE
        {
            get
            {
                return _MSR_NOTE;
            }
            set
            {
                if (_MSR_NOTE != value)
                {
                    _MSR_NOTE = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
