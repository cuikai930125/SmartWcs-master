using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common.Extensions;

using System;
using System.Windows.Media;

namespace SMART.WCS.UI.EMS.DataMembers.E1002
{
    public class EmsPbs : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        private bool g_isValidation = false;

        #region * Validation - 그리드 데이터 유효성 체크
        public void ClearError()
        {
            this.Error = null;
            this.SetErrorInfo(new DevExpress.XtraEditors.DXErrorProvider.ErrorInfo(), null, ErrorType.None);
        }

        public void RowError(string errorText)
        {
            //_IsValidation = true;
            this.Error = errorText;
            //RaisePropertyChanged(ErrorProperty);
        }

        public void CellError(string _ErrorProperty, string _Error)
        {
            g_isValidation = true;
            this.Error = _Error;
            this.ErrorProperty = _ErrorProperty;
            RaisePropertyChanged(_ErrorProperty);
        }

        public void GetPropertyError(string propertyName, DevExpress.XtraEditors.DXErrorProvider.ErrorInfo info)
        {
            if (g_isValidation && ErrorProperty == propertyName)
            {
                SetErrorInfo(info, Error, ErrorType.Critical);
            }
        }

        public void GetError(DevExpress.XtraEditors.DXErrorProvider.ErrorInfo info)
        {
            if (Error != null)
            {
                SetErrorInfo(info, Error, ErrorType.Critical);
            }
        }

        void SetErrorInfo(DevExpress.XtraEditors.DXErrorProvider.ErrorInfo Info, string ErrorText, ErrorType errorType)
        {
            Info.ErrorText = ErrorText;
            Info.ErrorType = errorType;
        }

        string _ErrorProperty;
        public string ErrorProperty
        {
            get
            {
                return _ErrorProperty;
            }
            set
            {
                _ErrorProperty = value;
            }
        }
        #endregion

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

        string _PBS_DEV_CD;
        /// <summary>
        /// 구분
        /// </summary>
        public string PBS_DEV_CD
        {
            get
            {
                return _PBS_DEV_CD;
            }
            set
            {
                if (_PBS_DEV_CD != value)
                {
                    _PBS_DEV_CD = value;
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
        int _PBS_LV;
        /// <summary>
        /// Level
        /// </summary>
        public int PBS_LV
        {
            get { return _PBS_LV; }
            set
            {
                if (_PBS_LV != value)
                {
                    _PBS_LV = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PARENT_PBS_ID;
        /// <summary>
        /// Parent
        /// </summary>
        public string PARENT_PBS_ID
        {
            get { return _PARENT_PBS_ID; }
            set
            {
                if (_PARENT_PBS_ID != value)
                {
                    _PARENT_PBS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion Tree 구조

        int _EQP_QTY;
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
    }
}
