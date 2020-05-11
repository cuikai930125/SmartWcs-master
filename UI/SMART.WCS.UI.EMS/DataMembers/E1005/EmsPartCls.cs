using SMART.WCS.Common.Extensions;

using System;
using System.Windows.Media;

namespace SMART.WCS.UI.EMS.DataMembers.E1005
{
    public class EmsPartCls : PropertyNotifyExtensions
    {
        public Brush BaseBackgroundBrush { get; set; }

        public Brush BackgroundBrush { get; set; }

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

        string _PART_CLS_ID;
        /// <summary>
        /// 부품분류
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

        string _PART_CLS_NOTE;
        /// <summary>
        /// 부품 분류 비고
        /// </summary>
        public string PART_CLS_NOTE
        {
            get
            {
                return _PART_CLS_NOTE;
            }
            set
            {
                if (_PART_CLS_NOTE != value)
                {
                    _PART_CLS_NOTE = value;
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

        private int _SORT_SEQ;
        public int SORT_SEQ
        {
            get { return this._SORT_SEQ; }
            set
            {
                if (this._SORT_SEQ != value)
                {
                    this._SORT_SEQ = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _TREE_ID;
        public string TREE_ID
        {
            get { return this._TREE_ID; }
            set
            {
                this._TREE_ID = value;
                RaisePropertyChanged();
            }
        }

        #region Tree 구조
        int _PART_CLS_LV;
        /// <summary>
        /// Level
        /// </summary>
        public int PART_CLS_LV
        {
            get { return _PART_CLS_LV; }
            set { _PART_CLS_LV = value; ; RaisePropertyChanged(); }
        }

        string _PARENT_PART_CLS_ID;
        /// <summary>
        /// Parent
        /// </summary>
        public string PARENT_PART_CLS_ID
        {
            get { return _PARENT_PART_CLS_ID; }
            set { _PARENT_PART_CLS_ID = value; ; RaisePropertyChanged(); }
        }
        #endregion Tree 구조

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
