using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.E1004
{
    public class EmsPartByEqp : PropertyNotifyExtensions
    {
        DateTime now;
        public EmsPartByEqp()
        {
            //now = new DateTime(1800, 1, 1);
            now = new DateTime(2000, 1, 1);
            _INST_DT = now;
        }


        int _PART_SERIAL_NO;
        public int PART_SERIAL_NO
        {
            get { return _PART_SERIAL_NO; }
            set { _PART_SERIAL_NO = value; ; RaisePropertyChanged(); }
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

        int _LIFE_CLE;
        /// <summary>
        /// 수명주기
        /// </summary>
        public int LIFE_CLE
        {
            get { return _LIFE_CLE; }
            set
            {
                if (_LIFE_CLE != value && 0 < value)
                {
                    _LIFE_CLE = value;
                    RaisePropertyChanged();

                    if (now != _INST_DT)
                    {
                        CHG_PLAN_DT = _INST_DT.AddDays(value);
                    }
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

                    if (0 < LIFE_CLE)
                    {
                        USE_DAYS = (DateTime.Now - value).Days + 1;
                        CHG_PLAN_DT = value.AddDays(LIFE_CLE - 1);
                    }
                }
            }
        }

        decimal _INST_QTY;
        /// <summary>
        /// 설치수량
        /// </summary>
        public decimal INST_QTY
        {
            get { return _INST_QTY; }
            set
            {
                if (_INST_QTY != value)
                {
                    _INST_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _USE_DAYS;
        /// <summary>
        /// 사용일수
        /// </summary>
        public int USE_DAYS
        {
            get
            {
                return _USE_DAYS;
            }
            set
            {
                if (_USE_DAYS != value)
                {
                    _USE_DAYS = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _REST_DAYS;
        /// <summary>
        /// 남은 일수
        /// </summary>
        public int REST_DAYS
        {
            get
            {
                return _REST_DAYS;
            }
            set
            {
                if (_REST_DAYS != value)
                {
                    _REST_DAYS = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _CHG_PLAN_DT;
        /// <summary>
        /// 예정일자
        /// </summary>
        public DateTime CHG_PLAN_DT
        {
            get
            {
                return _CHG_PLAN_DT;
            }
            set
            {
                if (_CHG_PLAN_DT != value)
                {
                    _CHG_PLAN_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
