using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.COM
{
    public class EmsPartRepairHis : PropertyNotifyExtensions
    {
        string _PART_ID;
        /// <summary>
        /// 부품 ID
        /// </summary>
        public string PART_ID
        {
            get
            {
                return _PART_ID;
            }
            set
            {
                if (_PART_ID != value)
                {
                    _PART_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_ID;
        /// <summary>
        /// 설비 ID
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

        DateTime _CHK_DT;
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
        /// Null 값 대응
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

        string _WORK_NOTE;
        /// <summary>
        /// 작업 내용
        /// </summary>
        public string WORK_NOTE
        {
            get
            {
                return _WORK_NOTE;
            }
            set
            {
                if (_WORK_NOTE != value)
                {
                    _WORK_NOTE = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _FALL_QTY;
        /// <summary>
        /// 장애수량
        /// </summary>
        public decimal FALL_QTY
        {
            get
            {
                return _FALL_QTY;
            }
            set
            {
                if (_FALL_QTY != value)
                {
                    _FALL_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _NEW_INST_QTY;
        /// <summary>
        /// 신규설치수량
        /// </summary>
        public decimal NEW_INST_QTY
        {
            get
            {
                return _NEW_INST_QTY;
            }
            set
            {
                if (_NEW_INST_QTY != value)
                {
                    _NEW_INST_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _ORG_INST_DT;
        /// <summary>
        /// 당초설치일자
        /// </summary>
        public DateTime ORG_INST_DT
        {
            get
            {
                return _ORG_INST_DT;
            }
            set
            {
                if (_ORG_INST_DT != value)
                {
                    _ORG_INST_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Null 값 대응
        /// </summary>
        public DateTime? ORG_INST_DT_N
        {
            get
            {
                if (1 == ORG_INST_DT.Year)
                {
                    return null;
                }
                else
                {
                    return ORG_INST_DT;
                }
            }
        }

        string _REPAIR_DEV;
        /// <summary>
        /// 구분
        /// </summary>
        public string REPAIR_DEV
        {
            get
            {
                return _REPAIR_DEV;
            }
            set
            {
                if (_REPAIR_DEV != value)
                {
                    _REPAIR_DEV = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
