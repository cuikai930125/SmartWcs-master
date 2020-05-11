using SMART.WCS.Common.Extensions;

namespace SMART.WCS.UI.EMS.DataMembers.E3001
{
    public class EmsChkRst : PropertyNotifyExtensions
    {
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

        string _EQP_MNFACT;
        /// <summary>
        /// 제조사
        /// </summary>
        public string EQP_MNFACT
        {
            get
            {
                return _EQP_MNFACT;
            }
            set
            {
                if (_EQP_MNFACT != value)
                {
                    _EQP_MNFACT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PBS_ID;
        public string PBS_ID
        {
            get
            {
                return _PBS_ID;
            }
            set
            {
                if (_PBS_ID != value)
                {
                    _PBS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PBS_NM;
        public string PBS_NM
        {
            get
            {
                return _PBS_NM;
            }
            set
            {
                if (_PBS_NM != value)
                {
                    _PBS_NM = value;
                }
                // 업데이트 처리 안함
            }
        }

        string _CHK_TRGT_YN;
        /// <summary>
        /// 점검대상여부
        /// </summary>
        public string CHK_TRGT_YN
        {
            get
            {
                return _CHK_TRGT_YN;
            }
            set
            {
                if (_CHK_TRGT_YN != value)
                {
                    _CHK_TRGT_YN = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_YN;
        /// <summary>
        /// 점검여부
        /// </summary>
        public string CHK_YN
        {
            get
            {
                return _CHK_YN;
            }
            set
            {
                if (_CHK_YN != value)
                {
                    _CHK_YN = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _CHK_RST_NOTE;
        /// <summary>
        /// 점검결과내용
        /// </summary>
        public string CHK_RST_NOTE
        {
            get
            {
                return _CHK_RST_NOTE;
            }
            set
            {
                if (_CHK_RST_NOTE != value)
                {
                    _CHK_RST_NOTE = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
