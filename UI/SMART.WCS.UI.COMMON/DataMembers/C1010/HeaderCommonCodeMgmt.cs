using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1010
{
    class HeaderCommonCodeMgmt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public HeaderCommonCodeMgmt()
        {
            this.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
        }

        public Brush BaseBackgroundBrush { get; set; }

        public Brush BackgroundBrush { get; set; }
        #endregion

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

        #region + COM_HDR_CD - CODE 대분류
        private string _COM_HDR_CD;
        public string COM_HDR_CD
        {
            get { return this._COM_HDR_CD; }
            set
            {
                if (this._COM_HDR_CD != value)
                {
                    this._COM_HDR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + COM_HDR_NM - CODE 대분류 이름
        private string _COM_HDR_NM;
        public string COM_HDR_NM
        {
            get { return this._COM_HDR_NM; }
            set
            {
                if (this._COM_HDR_NM != value)
                {
                    this._COM_HDR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + USE_YN - 사용 여부
        private string _USE_YN;
        public string USE_YN
        {
            get { return this._USE_YN; }
            set
            {
                if (this._USE_YN != value)
                {
                    this._USE_YN = value;
                    this.Checked = this.USE_YN.Equals("Y") ? true : false;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + Checked - 사용여부 변환값
        private bool _Checked;
        public bool Checked
        {
            get { return this._Checked; }
            set
            {
                if (this._Checked != value)
                {
                    this._Checked = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SORT_SEQ - 정렬순서
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
        #endregion

        #region + ATTR01 - 문자속성 01
        private string _ATTR01;
        public string ATTR01
        {
            get { return this._ATTR01; }
            set
            {
                if (this._ATTR01 != value)
                {
                    this._ATTR01 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ATTR02 - 문자속성 02
        private string _ATTR02;
        public string ATTR02
        {
            get { return this._ATTR02; }
            set
            {
                if (this._ATTR02 != value)
                {
                    this._ATTR02 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ATTR03 - 문자속성 03
        private string _ATTR03;
        public string ATTR03
        {
            get { return this._ATTR03; }
            set
            {
                if (this._ATTR03 != value)
                {
                    this._ATTR03 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ATTR04 - 문자속성 04
        private string _ATTR04;
        public string ATTR04
        {
            get { return this._ATTR04; }
            set
            {
                if (this._ATTR04 != value)
                {
                    this._ATTR04 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ATTR05 - 문자속성 05
        private string _ATTR05;
        public string ATTR05
        {
            get { return this._ATTR05; }
            set
            {
                if (this._ATTR05 != value)
                {
                    this._ATTR05 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ATTR06 - 문자속성 06
        private string _ATTR06;
        public string ATTR06
        {
            get { return this._ATTR06; }
            set
            {
                if (this._ATTR06 != value)
                {
                    this._ATTR06 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ATTR07 - 문자속성 07
        private string _ATTR07;
        public string ATTR07
        {
            get { return this._ATTR07; }
            set
            {
                if (this._ATTR07 != value)
                {
                    this._ATTR07 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ATTR08 - 문자속성 08
        private string _ATTR08;
        public string ATTR08
        {
            get { return this._ATTR08; }
            set
            {
                if (this._ATTR08 != value)
                {
                    this._ATTR08 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ATTR09 - 문자속성 09
        private string _ATTR09;
        public string ATTR09
        {
            get { return this._ATTR09; }
            set
            {
                if (this._ATTR09 != value)
                {
                    this._ATTR09 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ATTR10 - 문자속성 10
        private string _ATTR10;
        public string ATTR10
        {
            get { return this._ATTR10; }
            set
            {
                if (this._ATTR10 != value)
                {
                    this._ATTR10 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + IsSaved - 저장 여부
        private bool _IsSaved;
        public bool IsSaved
        {
            get { return this._IsSaved; }
            set
            {
                if (this._IsSaved != value)
                {
                    this._IsSaved = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
