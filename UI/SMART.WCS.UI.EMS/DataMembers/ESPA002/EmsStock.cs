using SMART.WCS.Common.Extensions;

using System;

namespace SMART.WCS.UI.EMS.DataMembers.ESPA002
{
    public class EmsStock : PropertyNotifyExtensions
    {
        DateTime _STOCK_INSP_DT;
        /// <summary>
        /// 재고조사일자
        /// </summary>
        public DateTime STOCK_INSP_DT
        {
            get
            {
                return _STOCK_INSP_DT;
            }
            set
            {
                if (_STOCK_INSP_DT != value)
                {
                    _STOCK_INSP_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _MNGR_ID;
        /// <summary>
        /// 담당자
        /// </summary>
        public string MNGR_ID
        {
            get
            {
                return _MNGR_ID;
            }
            set
            {
                if (_MNGR_ID != value)
                {
                    _MNGR_ID = value;
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

        string _NOTE;
        /// <summary>
        /// 비고
        /// </summary>
        public string NOTE
        {
            get
            {
                return _NOTE;
            }
            set
            {
                if (_NOTE != value)
                {
                    _NOTE = value;
                    RaisePropertyChanged();
                }
            }
        }


        string _INSP_STAT_CD;
        /// <summary>
        /// 진행상태
        /// </summary>
        public string INSP_STAT_CD
        {
            get
            {
                return _INSP_STAT_CD;
            }
            set
            {
                if (_INSP_STAT_CD != value)
                {
                    _INSP_STAT_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _COM_DETAIL_NM;
        /// <summary>
        /// 코드명
        /// </summary>
        public string COM_DETAIL_NM
        {
            get
            {
                return _COM_DETAIL_NM;
            }
            set
            {
                if (_COM_DETAIL_NM != value)
                {
                    _COM_DETAIL_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
