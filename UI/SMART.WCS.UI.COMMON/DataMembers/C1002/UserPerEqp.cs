using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1002
{
    public class UserPerEqp : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass             = new BaseClass();
        private bool g_isValidation     = false;

        #region * 그리드 색상 설정
        public UserPerEqp()
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

        #region + EQP_ID - 설비 ID
        private string _EQP_ID;
        public string EQP_ID
        {
            get { return this._EQP_ID; }
            set
            {
                if (this._EQP_ID != value)
                {
                    this._EQP_ID = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_NM - 설비명
        private string _EQP_NM;
        public string EQP_NM
        {
            get { return this._EQP_NM; }
            set
            {
                if (this._EQP_NM != value)
                {
                    this._EQP_NM = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_DESC - 설비 상세
        private string _EQP_DESC;
        public string EQP_DESC
        {
            get { return this._EQP_DESC; }
            set
            {
                if (this._EQP_DESC != value)
                {
                    this._EQP_DESC = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_TYPE_CD - 설비 타입코드
        private string _EQP_TYPE_CD;
        public string EQP_TYPE_CD
        {
            get { return this._EQP_TYPE_CD; }
            set
            {
                if (this._EQP_TYPE_CD != value)
                {
                    this._EQP_TYPE_CD = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + LOC_CD - Location 코드
        private string _LOC_CD;
        public string LOC_CD
        {
            get { return this._LOC_CD; }
            set
            {
                if (this._LOC_CD != value)
                {
                    this._LOC_CD = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
