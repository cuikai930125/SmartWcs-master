using SMART.WCS.Common.Extensions;

using System;

namespace SMART.WCS.UI.EMS.DataMembers.E3001
{
    public class EmsChkPlanInfo : PropertyNotifyExtensions
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

        int _PART_SERIAL_NO;
        public int PART_SERIAL_NO
        {
            get
            {
                return _PART_SERIAL_NO;
            }
            set
            {
                if (_PART_SERIAL_NO != value)
                {
                    _PART_SERIAL_NO = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_ID;
        /// <summary>
        /// 부품
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

        string _PART_NM;
        public string PART_NM
        {
            get
            {
                return _PART_NM;
            }
            set
            {
                if (_PART_NM != value)
                {
                    _PART_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _ORG_INST_QTY;
        /// <summary>
        /// 당초설치수량
        /// </summary>
        public decimal ORG_INST_QTY
        {
            get
            {
                return _ORG_INST_QTY;
            }
            set
            {
                if (_ORG_INST_QTY != value)
                {
                    _ORG_INST_QTY = value;
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
        /// Null 대응
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
            set
            {
                if (ORG_INST_DT != value.Value)
                {
                    ORG_INST_DT = value.Value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _EXCHG_QTY;
        /// <summary>
        /// 교체수량
        /// </summary>
        public decimal EXCHG_QTY
        {
            get
            {
                return _EXCHG_QTY;
            }
            set
            {
                if (_EXCHG_QTY != value)
                {
                    _EXCHG_QTY = value;
                    RaisePropertyChanged();

                    if (!CheckQty())
                    {
                        _EXCHG_QTY = 0;
                        RaisePropertyChanged();
                    }
                }
            }
        }

        decimal _NEW_INST_QTY;
        /// <summary>
        /// 설치수량
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

        decimal _FALL_QTY;
        /// <summary>
        /// 폐기수량
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

                    if (!CheckQty())
                    {
                        _FALL_QTY = 0;
                        RaisePropertyChanged();
                    }
                }
            }
        }

        string _WORK_NOTE;
        /// <summary>
        /// 작업내용
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


        bool CheckQty()
        {
            if (null != QtyErrorAction)
            {
                if (0 < ORG_INST_QTY && (0 < FALL_QTY || 0 < EXCHG_QTY))
                {
                    if (FALL_QTY + EXCHG_QTY > ORG_INST_QTY)
                    {
                        QtyErrorAction();
                        return false;
                    }
                }
            }

            return true;
        }

        public Action QtyErrorAction { get; set; }
    }
}
