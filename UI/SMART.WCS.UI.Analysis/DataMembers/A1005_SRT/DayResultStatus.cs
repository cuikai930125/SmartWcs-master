using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Analysis.DataMembers.A1005_SRT
{
    class DayResultStatus : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public DayResultStatus()
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

        #region + AGG_YMD - 일자
        private string _AGG_YMD;
        public string AGG_YMD
        {
            get { return this._AGG_YMD; }
            set
            {
                if (this._AGG_YMD != value)
                {
                    this._AGG_YMD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + NML_RSLT_QTY - 정상분류
        private string _NML_RSLT_QTY;
        public string NML_RSLT_QTY
        {
            get { return this._NML_RSLT_QTY; }
            set
            {
                if (this._NML_RSLT_QTY != value)
                {
                    this._NML_RSLT_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ERR_QTY - 오분류
        private string _ERR_QTY;
        public string ERR_QTY
        {
            get { return this._ERR_QTY; }
            set
            {
                if (this._ERR_QTY != value)
                {
                    this._ERR_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TOT_QTY - 합계
        private int _TOT_QTY;
        public int TOT_QTY
        {
            get { return Convert.ToInt32(this._NML_RSLT_QTY) + Convert.ToInt32(this._ERR_QTY); }
            set
            {
                if (this._TOT_QTY != value)
                {
                    this._TOT_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
