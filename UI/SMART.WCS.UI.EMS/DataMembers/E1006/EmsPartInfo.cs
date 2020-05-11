using SMART.WCS.Common.Extensions;

using System;

namespace SMART.WCS.UI.EMS.DataMembers.E1006
{
    public class EmsPartInfo : PropertyNotifyExtensions
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

        string _KEY_ID;
        /// <summary>
        /// Key (Tree)
        /// </summary>
        public string KEY_ID
        {
            get
            {
                return _KEY_ID;
            }
            set
            {
                if (_KEY_ID != value)
                {
                    _KEY_ID = value;
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

        string _ORDER_DEV;
        /// <summary>
        /// 분류
        /// </summary>
        public string ORDER_DEV
        {
            get
            {
                return _ORDER_DEV;
            }
            set
            {
                if (_ORDER_DEV != value)
                {
                    _ORDER_DEV = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region Tree 구조
        int _CLS_LV;
        /// <summary>
        /// Level
        /// </summary>
        public int CLS_LV
        {
            get { return _CLS_LV; }
            set { _CLS_LV = value; ; RaisePropertyChanged(); }
        }

        string _PARENT_ID;
        /// <summary>
        /// Parent
        /// </summary>
        public string PARENT_ID
        {
            get
            {
                return _PARENT_ID;
            }
            set
            {
                if (_PARENT_ID != value)
                {
                    _PARENT_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion Tree 구조

        string _PART_CLS_NM;
        public string PART_CLS_NM
        {
            get
            {
                return _PART_CLS_NM;
            }
            set
            {
                if (_PART_CLS_NM != value)
                {
                    _PART_CLS_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_MNFACT;
        /// <summary>
        /// 제조사
        /// </summary>
        public string PART_MNFACT
        {
            get
            {
                return _PART_MNFACT;
            }
            set
            {
                if (_PART_MNFACT != value)
                {
                    _PART_MNFACT = value;
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
                if (_LIFE_CLE != value)
                {
                    _LIFE_CLE = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _ALM_LEAD_TIME;
        /// <summary>
        /// 사전알람
        /// </summary>
        public int ALM_LEAD_TIME
        {
            get { return _ALM_LEAD_TIME; }
            set
            {
                if (_ALM_LEAD_TIME != value)
                {
                    _ALM_LEAD_TIME = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_MODEL;
        public string PART_MODEL
        {
            get
            {
                return _PART_MODEL;
            }
            set
            {
                if (_PART_MODEL != value)
                {
                    _PART_MODEL = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_STND;
        /// <summary>
        /// 규격
        /// </summary>
        public string PART_STND
        {
            get
            {
                return _PART_STND;
            }
            set
            {
                if (_PART_STND != value)
                {
                    _PART_STND = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _STOCK_MNG_YN;
        /// <summary>
        /// 재고관리여부
        /// </summary>
        public string STOCK_MNG_YN
        {
            get
            {
                return _STOCK_MNG_YN;
            }
            set
            {
                if (_STOCK_MNG_YN != value)
                {
                    _STOCK_MNG_YN = value;
                    RaisePropertyChanged();
                }
            }
        }

        decimal _STOCK_QTY;
        /// <summary>
        /// 재고 수량
        /// </summary>
        public decimal STOCK_QTY
        {
            get { return _STOCK_QTY; }
            set
            {
                if (_STOCK_QTY != value)
                {
                    _STOCK_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _USE_EQP_CNT;
        /// <summary>
        /// 사용수량
        /// </summary>
        public int USE_EQP_CNT
        {
            get { return _USE_EQP_CNT; }
            set
            {
                if (_USE_EQP_CNT != value)
                {
                    _USE_EQP_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PURCH_NM;
        /// <summary>
        /// 구매처
        /// </summary>
        public string PURCH_NM
        {
            get
            {
                return _PURCH_NM;
            }
            set
            {
                if (_PURCH_NM != value)
                {
                    _PURCH_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        byte[] _PART_IMG;
        /// <summary>
        /// Image Byte Data
        /// </summary>
        public byte[] PART_IMG
        {
            get
            {
                return _PART_IMG;
            }
            set
            {
                if (_PART_IMG != value)
                {
                    _PART_IMG = value;
                    RaisePropertyChanged();
                }
            }
        }


        string _PART_IMG_FILE_ID;
        /// <summary>
        /// Image GUID
        /// </summary>
        public string PART_IMG_FILE_ID
        {
            get
            {
                return _PART_IMG_FILE_ID;
            }
            set
            {
                if (_PART_IMG_FILE_ID != value)
                {
                    _PART_IMG_FILE_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_CLS_ID;
        /// <summary>
        /// 부품 분류
        /// </summary>
        public string PART_CLS_ID
        {
            get
            {
                return _PART_CLS_ID;
            }
            set
            {
                if (_PART_CLS_ID != value)
                {
                    _PART_CLS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }



        DateTime _REG_DT;
        public DateTime REG_DT
        {
            get
            {
                return _REG_DT;
            }
            set
            {
                if (_REG_DT != value)
                {
                    _REG_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _RSTR_ID;
        public string RSTR_ID
        {
            get
            {
                return _RSTR_ID;
            }
            set
            {
                if (_RSTR_ID != value)
                {
                    _RSTR_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _UPD_DT;
        public DateTime UPD_DT
        {
            get
            {
                return _UPD_DT;
            }
            set
            {
                if (_UPD_DT != value)
                {
                    _UPD_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _UPDR_ID;
        public string UPDR_ID
        {
            get
            {
                return _UPDR_ID;
            }
            set
            {
                if (_UPDR_ID != value)
                {
                    _UPDR_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
