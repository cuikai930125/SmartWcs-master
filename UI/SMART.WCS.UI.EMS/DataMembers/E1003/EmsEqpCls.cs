using SMART.WCS.Common.Extensions;

using System;
using System.Windows.Media;

namespace SMART.WCS.UI.EMS.DataMembers.E1003
{
    public class EmsEqpCls : PropertyNotifyExtensions
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

        string _EQP_CLS_ID;
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

        #region Tree 구조
        int _EQP_CLS_LV;
        /// <summary>
        /// Level
        /// </summary>
        public int EQP_CLS_LV
        {
            get { return _EQP_CLS_LV; }
            set
            {
                if (_EQP_CLS_LV != value)
                {
                    _EQP_CLS_LV = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PARENT_EQP_CLS_ID;
        /// <summary>
        /// Parent
        /// </summary>
        public string PARENT_EQP_CLS_ID
        {
            get { return _PARENT_EQP_CLS_ID; }
            set
            {
                if (_PARENT_EQP_CLS_ID != value)
                {
                    _PARENT_EQP_CLS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion Tree 구조

        int _EQP_QTY;
        /// <summary>
        /// 설비 수량
        /// </summary>
        public int EQP_QTY
        {
            get { return _EQP_QTY; }
            set
            {
                if (_EQP_QTY != value)
                {
                    _EQP_QTY = value;
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

        #region + TREE_ID - 트리 ID
        private string _TREE_ID;

        public string TREE_ID
        {
            get { return this._TREE_ID; }
            set
            {
                if (this._TREE_ID != value)
                {
                    this._TREE_ID = value;
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

        private string _USE_YN;
        public string USE_YN
        {
            get { return this._USE_YN; }
            set
            {
                if (this._USE_YN != value)
                {
                    this._USE_YN            = value;
                    this.USE_YN_CHECKED     = this.USE_YN.Equals("Y") == true ? true : false;
                    RaisePropertyChanged();
                }
            }
        }

        #region + USE_YN_CHECKED - 정렬 순서
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
