using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1008
{
    class DetailBatchJobMonitoring : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public DetailBatchJobMonitoring()
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

        #region + JOB_NO - JOB 번호
        private string _JOB_NO;
        public string JOB_NO
        {
            get { return this._JOB_NO; }
            set
            {
                if (this._JOB_NO != value)
                {
                    this._JOB_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + JOB_SEQ - 순번
        private string _JOB_SEQ;
        public string JOB_SEQ
        {
            get { return this._JOB_SEQ; }
            set
            {
                if (this._JOB_SEQ != value)
                {
                    this._JOB_SEQ = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ERR_CONT - 에러 내용
        private string _ERR_CONT;
        public string ERR_CONT
        {
            get { return this._ERR_CONT; }
            set
            {
                if (this._ERR_CONT != value)
                {
                    this._ERR_CONT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + UPD_DT - 처리일시
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

        #region + PARAM1 - Parameter1
        private string _PARAM1;
        public string PARAM1
        {
            get { return this._PARAM1; }
            set
            {
                if (this._PARAM1 != value)
                {
                    this._PARAM1 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PARAM2 - Parameter2
        private string _PARAM2;
        public string PARAM2
        {
            get { return this._PARAM2; }
            set
            {
                if (this._PARAM2 != value)
                {
                    this._PARAM2 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
        
    }
}
