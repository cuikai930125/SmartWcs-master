using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1025
{
    class IPMgmt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public IPMgmt()
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

        #region + IP_NO - IP NO
        private string _IP_NO;
        public string IP_NO
        {
            get { return this._IP_NO; }
            set
            {
                if (this._IP_NO != value)
                {
                    this._IP_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RMK - 비고
        private string _RMK;
        public string RMK
        {
            get { return this._RMK; }
            set
            {
                if (this._RMK != value)
                {
                    this._RMK = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + REG_ID - 등록자
        private string _REG_ID;
        public string REG_ID
        {
            get { return this._REG_ID; }
            set
            {
                if (this._REG_ID != value)
                {
                    this._REG_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + REG_DT - 등록일시
        private string _REG_DT;
        public string REG_DT
        {
            get { return this._REG_DT; }
            set
            {
                if (this._REG_DT != value)
                {
                    this._REG_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + UPD_ID - 수정자
        private string _UPD_ID;
        public string UPD_ID
        {
            get { return this._UPD_ID; }
            set
            {
                if (this._UPD_ID != value)
                {
                    this._UPD_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + UPD_DT - 수정일시
        private string _UPD_DT;
        public string UPD_DT
        {
            get { return this._UPD_DT; }
            set
            {
                if (this._UPD_DT != value)
                {
                    this._UPD_DT = value;
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
