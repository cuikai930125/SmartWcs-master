using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1009
{
    public class EcsLogMgmt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public EcsLogMgmt()
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


        #region + PRC_ID - 프로세스 ID
        private string _PRC_ID;
        public string PRC_ID
        {
            get { return this._PRC_ID; }
            set
            {
                if (this._PRC_ID != value)
                {
                    this._PRC_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TM_STAMP - 로그일시
        private string _TM_STAMP;
        public string TM_STAMP
        {
            get { return this._TM_STAMP; }
            set
            {
                if (this._TM_STAMP != value)
                {
                    this._TM_STAMP = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PID - PID
        private string _PID;
        public string PID
        {
            get { return this._PID; }
            set
            {
                if (this._PID != value)
                {
                    this._PID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MESSAGE - ECS 호출 로그
        private string _MESSAGE;
        public string MESSAGE
        {
            get { return this._MESSAGE; }
            set
            {
                if (this._MESSAGE != value)
                {
                    this._MESSAGE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
