using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Analysis.DataMembers.A1002_SRT
{
    class RejectReasonMgmt : PropertyNotifyExtensions
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public RejectReasonMgmt()
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

        #region + REJECT_TYPE - Reject 유형
        private string _REJECT_TYPE;
        public string REJECT_TYPE
        {
            get { return this._REJECT_TYPE; }
            set
            {
                if (this._REJECT_TYPE != value)
                {
                    this._REJECT_TYPE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SRT_ERR_CD - 오류 코드
        private string _SRT_ERR_CD;
        public string SRT_ERR_CD
        {
            get { return this._SRT_ERR_CD; }
            set
            {
                if (this._SRT_ERR_CD != value)
                {
                    this._SRT_ERR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SRT_RSN_CD - 결과 코드
        private string _SRT_RSN_CD;
        public string SRT_RSN_CD
        {
            get { return this._SRT_RSN_CD; }
            set
            {
                if (this._SRT_RSN_CD != value)
                {
                    this._SRT_RSN_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + REJECT_QTY - Reject 건수
        private string _REJECT_QTY;
        public string REJECT_QTY
        {
            get { return this._REJECT_QTY; }
            set
            {
                if (this._REJECT_QTY != value)
                {
                    this._REJECT_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + REJECT_PRCT - Reject %
        private string _REJECT_PRCT;
        public string REJECT_PRCT
        {
            get { return this._REJECT_PRCT; }
            set
            {
                if (this._REJECT_PRCT != value)
                {
                    this._REJECT_PRCT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
