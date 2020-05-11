using SMART.WCS.Common.Extensions;

using System;

namespace SMART.WCS.UI.EMS.DataMembers.ECHK002
{
    public class EmsFail : PropertyNotifyExtensions
    {
        int _FAIL_NO;
        public int FAIL_NO
        {
            get
            {
                return _FAIL_NO;
            }
            set
            {
                if (_FAIL_NO != value)
                {
                    _FAIL_NO = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _FAIL_STAT_CD;
        /// <summary>
        /// 구분
        /// </summary>
        public string FAIL_STAT_CD
        {
            get
            {
                return _FAIL_STAT_CD;
            }
            set
            {
                if (_FAIL_STAT_CD != value)
                {
                    _FAIL_STAT_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _FALL_DT;
        /// <summary>
        /// 장애일시
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
        /// Null 대응
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

        string _EQP_ID;
        /// <summary>
        /// 설비
        /// </summary>
        public string EQP_ID
        {
            get
            {
                return _EQP_ID;
            }
            set
            {
                if (_EQP_ID != value)
                {
                    _EQP_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_NM;
        public string EQP_NM
        {
            get
            {
                return _EQP_NM;
            }
            set
            {
                if (_EQP_NM != value)
                {
                    _EQP_NM = value;
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
        /// 사유
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
        /// 조치사항
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

        DateTime _MSR_REG_DT;
        /// <summary>
        /// 조치일시
        /// </summary>
        public DateTime MSR_REG_DT
        {
            get
            {
                return _MSR_REG_DT;
            }
            set
            {
                if (_MSR_REG_DT != value)
                {
                    _MSR_REG_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Null 대응
        /// </summary>
        public DateTime? MSR_REG_DT_N
        {
            get
            {
                if (1 == MSR_REG_DT.Year)
                {
                    return null;
                }
                else
                {
                    return MSR_REG_DT;
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

        string _FAIL_DEV_CD;
        /// <summary>
        /// 구분
        /// </summary>
        public string FAIL_DEV_CD
        {
            get
            {
                return _FAIL_DEV_CD;
            }
            set
            {
                if (_FAIL_DEV_CD != value)
                {
                    _FAIL_DEV_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _FAIL_DEV_NM;
        public string FAIL_DEV_NM
        {
            get
            {
                return _FAIL_DEV_NM;
            }
            set
            {
                if (_FAIL_DEV_NM != value)
                {
                    _FAIL_DEV_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _FAIL_STATUS;
        /// <summary>
        /// 장애내용
        /// </summary>
        public string FAIL_STATUS
        {
            get
            {
                return _FAIL_STATUS;
            }
            set
            {
                if (_FAIL_STATUS != value)
                {
                    _FAIL_STATUS = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _FAIL_STATUS_NM;
        public string FAIL_STATUS_NM
        {
            get
            {
                return _FAIL_STATUS_NM;
            }
            set
            {
                if (_FAIL_STATUS_NM != value)
                {
                    _FAIL_STATUS_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
