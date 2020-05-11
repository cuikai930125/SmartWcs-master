using SMART.WCS.Common.Extensions;

using System;

namespace SMART.WCS.UI.EMS.DataMembers.E3001
{
    public class EmsChkPlan : PropertyNotifyExtensions
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

        string _CHK_STAT_CD;
        /// <summary>
        /// 진행상태
        /// </summary>
        public string CHK_STAT_CD
        {
            get
            {
                return _CHK_STAT_CD;
            }
            set
            {
                if (_CHK_STAT_CD != value)
                {
                    _CHK_STAT_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_PLAN_NM;
        /// <summary>
        /// 점검명
        /// </summary>
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

        string _CHK_PLAN_MNGR_ID;
        /// <summary>
        /// 담당자
        /// </summary>
        public string CHK_PLAN_MNGR_ID
        {
            get
            {
                return _CHK_PLAN_MNGR_ID;
            }
            set
            {
                if (_CHK_PLAN_MNGR_ID != value)
                {
                    _CHK_PLAN_MNGR_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_PLAN_MNGR_NM;
        public string CHK_PLAN_MNGR_NM
        {
            get
            {
                return _CHK_PLAN_MNGR_NM;
            }
            set
            {
                if (_CHK_PLAN_MNGR_NM != value)
                {
                    _CHK_PLAN_MNGR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _CHK_PLAN_DUR;
        /// <summary>
        /// 점검기간
        /// </summary>
        public int CHK_PLAN_DUR
        {
            get
            {
                return _CHK_PLAN_DUR;
            }
            set
            {
                if (_CHK_PLAN_DUR != value)
                {
                    _CHK_PLAN_DUR = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _CHK_DT;
        /// <summary>
        /// 점검일자
        /// </summary>
        public DateTime CHK_DT
        {
            get
            {
                return _CHK_DT;
            }
            set
            {
                if (_CHK_DT != value)
                {
                    _CHK_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Null 대응
        /// </summary>
        public DateTime? CHK_DT_N
        {
            get
            {
                if (1 == CHK_DT.Year)
                {
                    return null;
                }
                else
                {
                    return CHK_DT;
                }
            }
        }

        int _CHK_COUNT;
        /// <summary>
        /// 조치건수
        /// </summary>
        public int CHK_COUNT
        {
            get
            {
                return _CHK_COUNT;
            }
            set
            {
                if (_CHK_COUNT != value)
                {
                    _CHK_COUNT = value;
                    RaisePropertyChanged();
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

        string _CHK_NOTE;
        /// <summary>
        /// 점검 방법
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

        private string _USE_YN;
        public string USE_YN
        {
            get { return this._USE_YN; }
            set
            {
                if (this._USE_YN != value)
                {
                    this._USE_YN = value;
                    this.USE_YN_CHECKED = this.USE_YN.Equals("Y") == true ? true : false;
                    RaisePropertyChanged();
                }
            }
        }

        #region + USE_YN_CHECKED 
        private bool _USE_YN_CHECKED;

        public bool USE_YN_CHECKED
        {
            get { return this._USE_YN_CHECKED; }
            set
            {
                if (this._USE_YN_CHECKED != value)
                {
                    this._USE_YN_CHECKED = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
