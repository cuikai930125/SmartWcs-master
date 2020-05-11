using SMART.WCS.Common.Extensions;

using System;

namespace SMART.WCS.UI.EMS.DataMembers.E3001
{
    public class EmsChkRstInfo : PropertyNotifyExtensions
    {
        int _CHK_ID;
        public int CHK_ID
        {
            get
            {
                return _CHK_ID;
            }
            set
            {
                if (_CHK_ID != value)
                {
                    _CHK_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_PLAN_NM;
        public string CHK_PLAN_NM
        {
            get
            {
                return _CHK_PLAN_NM;
            }
            set
            {
                if (_CHK_PLAN_NM != value)
                {
                    _CHK_PLAN_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_DEV_CD;
        /// <summary>
        /// 구분
        /// </summary>
        public string CHK_DEV_CD
        {
            get
            {
                return _CHK_DEV_CD;
            }
            set
            {
                if (_CHK_DEV_CD != value)
                {
                    _CHK_DEV_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _CHK_PLAN_DT;
        /// <summary>
        /// 예정일
        /// </summary>
        public DateTime CHK_PLAN_DT
        {
            get
            {
                return _CHK_PLAN_DT;
            }
            set
            {
                if (_CHK_PLAN_DT != value)
                {
                    _CHK_PLAN_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Null 대응
        /// </summary>
        public DateTime? CHK_PLAN_DT_N
        {
            get
            {
                if (1 == CHK_PLAN_DT.Year)
                {
                    return null;
                }
                else
                {
                    return CHK_PLAN_DT;
                }
            }
        }

        string _CHK_MNGR_ID;
        /// <summary>
        /// 담당자
        /// </summary>
        public string CHK_MNGR_ID
        {
            get
            {
                return _CHK_MNGR_ID;
            }
            set
            {
                if (_CHK_MNGR_ID != value)
                {
                    _CHK_MNGR_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_MNGR_NM;
        public string CHK_MNGR_NM
        {
            get
            {
                return _CHK_MNGR_NM;
            }
            set
            {
                if (_CHK_MNGR_NM != value)
                {
                    _CHK_MNGR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_RST;
        /// <summary>
        /// 점검결과
        /// </summary>
        public string CHK_RST
        {
            get
            {
                return _CHK_RST;
            }
            set
            {
                if (_CHK_RST != value)
                {
                    _CHK_RST = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string CHK_STAT_CD { get; set; }
    }
}
