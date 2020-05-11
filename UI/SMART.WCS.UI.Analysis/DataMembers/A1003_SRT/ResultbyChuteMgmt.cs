using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Analysis.DataMembers.A1003_SRT
{
    public class ResultbyChuteMgmt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public ResultbyChuteMgmt()
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

        #region + ZONE_ID - Zone ID
        private string _ZONE_ID;
        public string ZONE_ID
        {
            get { return this._ZONE_ID; }
            set
            {
                if (this._ZONE_ID != value)
                {
                    this._ZONE_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CHUTE_ID - 슈트 ID
        private string _CHUTE_ID;
        public string CHUTE_ID
        {
            get { return this._CHUTE_ID; }
            set
            {
                if (this._CHUTE_ID != value)
                {
                    this._CHUTE_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RSLT_QTY - 물량
        private string _RSLT_QTY;
        public string RSLT_QTY
        {
            get { return this._RSLT_QTY; }
            set
            {
                if (this._RSLT_QTY!= value)
                {
                    this._RSLT_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RSLT_PRCT - 물량 %
        private string _RSLT_PRCT;
        public string RSLT_PRCT
        {
            get { return this._RSLT_PRCT; }
            set
            {
                if (this._RSLT_PRCT != value)
                {
                    this._RSLT_PRCT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + NML_SRT_QTY - 정상분류 개수
        private string _NML_SRT_QTY;
        public string NML_SRT_QTY
        {
            get { return this._NML_SRT_QTY; }
            set
            {
                if (this._NML_SRT_QTY != value)
                {
                    this._NML_SRT_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TOTAL_ERR_QTY - 오류 총합
        private string _TOTAL_ERR_QTY;
        public string TOTAL_ERR_QTY
        {
            get { return this._TOTAL_ERR_QTY; }
            set
            {
                if (this._TOTAL_ERR_QTY != value)
                {
                    this._TOTAL_ERR_QTY = value;
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
