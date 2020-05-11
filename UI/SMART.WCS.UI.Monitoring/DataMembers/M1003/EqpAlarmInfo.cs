using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Monitoring.DataMembers.M1003
{
    public class EqpAlarmInfo : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public EqpAlarmInfo()
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

        #region + EQP_ID - 설비ID
        private string _EQP_ID;
        public string EQP_ID
        {
            get { return this._EQP_ID; }
            set
            {
                if (this._EQP_ID != value)
                {
                    this._EQP_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + OCUR_DT - 발생일시
        private string _OCUR_DT;
        public string OCUR_DT
        {
            get { return this._OCUR_DT; }
            set
            {
                if (this._OCUR_DT != value)
                {
                    this._OCUR_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MODULE_CD - 모듈코드
        private string _MODULE_CD;
        public string MODULE_CD
        {
            get { return this._MODULE_CD; }
            set
            {
                if (this._MODULE_CD != value)
                {
                    this._MODULE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MODULE_DTL_CD - 모듈상세
        private string _MODULE_DTL_CD;
        public string MODULE_DTL_CD
        {
            get { return this._MODULE_DTL_CD; }
            set
            {
                if (this._MODULE_DTL_CD != value)
                {
                    this._MODULE_DTL_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ALARM_CD - 알람코드
        private string _ALARM_CD;
        public string ALARM_CD
        {
            get { return this._ALARM_CD; }
            set
            {
                if (this._ALARM_CD != value)
                {
                    this._ALARM_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ALARM_NM - 알람명
        private string _ALARM_NM;
        public string ALARM_NM
        {
            get { return this._ALARM_NM; }
            set
            {
                if (this._ALARM_NM != value)
                {
                    this._ALARM_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ALARM_STAT_CD - 알람상태
        private string _ALARM_STAT_CD;
        public string ALARM_STAT_CD
        {
            get { return this._ALARM_STAT_CD; }
            set
            {
                if (this._ALARM_STAT_CD != value)
                {
                    this._ALARM_STAT_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + UNIT_CD - 유닛코드
        private string _UNIT_CD;
        public string UNIT_CD
        {
            get { return this._UNIT_CD; }
            set
            {
                if (this._UNIT_CD != value)
                {
                    this._UNIT_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ALARM_DESC - 알람설명
        private string _ALARM_DESC;
        public string ALARM_DESC
        {
            get { return this._ALARM_DESC; }
            set
            {
                if (this._ALARM_DESC != value)
                {
                    this._ALARM_DESC = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ALARM_RSN - 알람원인
        private string _ALARM_RSN;
        public string ALARM_RSN
        {
            get { return this._ALARM_RSN; }
            set
            {
                if (this._ALARM_RSN != value)
                {
                    this._ALARM_RSN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ALARM_RCVRY - 알람해제방법
        private string _ALARM_RCVRY;
        public string ALARM_RCVRY
        {
            get { return this._ALARM_RCVRY; }
            set
            {
                if (this._ALARM_RCVRY != value)
                {
                    this._ALARM_RCVRY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + END_DT - 종료일시
        private string _END_DT;
        public string END_DT
        {
            get { return this._END_DT; }
            set
            {
                if (this._END_DT != value)
                {
                    this._END_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DIFF_HM - 처리시간
        private string _DIFF_HM;
        public string DIFF_HM
        {
            get { return this._DIFF_HM; }
            set
            {
                if (this._DIFF_HM != value)
                {
                    this._DIFF_HM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

    }
}
