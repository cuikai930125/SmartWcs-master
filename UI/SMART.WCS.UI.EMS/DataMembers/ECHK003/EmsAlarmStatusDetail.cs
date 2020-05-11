using SMART.WCS.Common.Extensions;

using System;

namespace SMART.WCS.UI.EMS.DataMembers.ECHK003
{
    public class EmsAlarmStatusDetail : PropertyNotifyExtensions
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

        string _EQP_MNFACT;
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




        DateTime _INST_DT;
        /// <summary>
        /// 설치 일자
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

        decimal _INST_QTY;
        /// <summary>
        /// 설치수량
        /// </summary>
        public decimal INST_QTY
        {
            get
            {
                return _INST_QTY;
            }
            set
            {
                if (_INST_QTY != value)
                {
                    _INST_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
