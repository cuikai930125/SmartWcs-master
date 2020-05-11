using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.E1004
{
    public class EmsEqpInfo : PropertyNotifyExtensions
    {
        string _EQP_ID;
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


        string _EQP_CLS_ID;
        /// <summary>
        /// 분류
        /// </summary>
        public string EQP_CLS_ID
        {
            get
            {
                return _EQP_CLS_ID;
            }
            set
            {
                if (_EQP_CLS_ID != value)
                {
                    _EQP_CLS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_CLS_NM;
        public string EQP_CLS_NM
        {
            get
            {
                return _EQP_CLS_NM;
            }
            set
            {
                if (_EQP_CLS_NM != value)
                {
                    _EQP_CLS_NM = value;
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
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _INST_DT;
        /// <summary>
        /// 설치일자
        /// </summary>
        public DateTime INST_DT
        {
            get
            {
                return _INST_DT;
            }
            set
            {
                if (_INST_DT != value)
                {
                    _INST_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_STND;
        /// <summary>
        /// 설비 규격
        /// </summary>
        public string EQP_STND
        {
            get
            {
                return _EQP_STND;
            }
            set
            {
                if (_EQP_STND != value)
                {
                    _EQP_STND = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_MODEL;
        public string EQP_MODEL
        {
            get
            {
                return _EQP_MODEL;
            }
            set
            {
                if (_EQP_MODEL != value)
                {
                    _EQP_MODEL = value;
                    RaisePropertyChanged();
                }
            }
        }

        byte[] _EQP_IMG;
        /// <summary>
        /// Image Byte Data
        /// </summary>
        public byte[] EQP_IMG
        {
            get
            {
                return _EQP_IMG;
            }
            set
            {
                if (_EQP_IMG != value)
                {
                    _EQP_IMG = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_IMG_FILE_ID;
        /// <summary>
        /// Image GUID
        /// </summary>
        public string EQP_IMG_FILE_ID
        {
            get
            {
                return _EQP_IMG_FILE_ID;
            }
            set
            {
                if (_EQP_IMG_FILE_ID != value)
                {
                    _EQP_IMG_FILE_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _MNG_CHGR_ID;
        /// <summary>
        /// 담당자
        /// </summary>
        public string MNG_CHGR_ID
        {
            get
            {
                return _MNG_CHGR_ID;
            }
            set
            {
                if (_MNG_CHGR_ID != value)
                {
                    _MNG_CHGR_ID = value;
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

        string _USE_YN;
        public string USE_YN
        {
            get
            {
                return _USE_YN;
            }
            set
            {
                if (_USE_YN != value)
                {
                    _USE_YN = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _FILENAL_CHK_DT;
        /// <summary>
        /// 최근 점검일
        /// </summary>
        public DateTime FILENAL_CHK_DT
        {
            get
            {
                return _FILENAL_CHK_DT;
            }
            set
            {
                if (_FILENAL_CHK_DT != value)
                {
                    _FILENAL_CHK_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
