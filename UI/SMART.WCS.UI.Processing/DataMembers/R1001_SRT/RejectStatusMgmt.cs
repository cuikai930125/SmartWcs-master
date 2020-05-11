using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Processing.DataMembers.R1001_SRT
{
    class RejectStatusMgmt : PropertyNotifyExtensions
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public RejectStatusMgmt()
        {
            //this.BackgroundBrush = BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
            this.ForegroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#000000");
        }

        public Brush BaseBackgroundBrush { get; set; }

        public Brush BackgroundBrush { get; set; }

        public Brush ForegroundBrush { get; set; }
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

        #region + BOX_BCD - 박스 바코드
        private string _BOX_BCD;
        public string BOX_BCD
        {
            get { return this._BOX_BCD; }
            set
            {
                if (this._BOX_BCD != value)
                {
                    this._BOX_BCD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RGN_BCD - 권역 코드
        private string _RGN_BCD;
        public string RGN_BCD
        {
            get { return this._RGN_BCD; }
            set
            {
                if (this._RGN_BCD != value)
                {
                    this._RGN_BCD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BCD_INFO - 바코드 정보
        private string _BCD_INFO;
        public string BCD_INFO
        {
            get { return this._BCD_INFO; }
            set
            {
                if (this._BCD_INFO != value)
                {
                    this._BCD_INFO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PLAN_CHUTE_ID1 - 계획슈트1
        private string _PLAN_CHUTE_ID1;
        public string PLAN_CHUTE_ID1
        {
            get { return this._PLAN_CHUTE_ID1; }
            set
            {
                if (this._PLAN_CHUTE_ID1 != value)
                {
                    this._PLAN_CHUTE_ID1 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PLAN_CHUTE_ID2 - 계획슈트2
        private string _PLAN_CHUTE_ID2;
        public string PLAN_CHUTE_ID2
        {
            get { return this._PLAN_CHUTE_ID2; }
            set
            {
                if (this._PLAN_CHUTE_ID2 != value)
                {
                    this._PLAN_CHUTE_ID2 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PLAN_CHUTE_ID3 - 계획슈트1
        private string _PLAN_CHUTE_ID3;
        public string PLAN_CHUTE_ID3
        {
            get { return this._PLAN_CHUTE_ID3; }
            set
            {
                if (this._PLAN_CHUTE_ID3 != value)
                {
                    this._PLAN_CHUTE_ID3 = value;
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

        #region + SRT_ERR_CD - 소터 오류 코드
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

        #region + SRT_ERR_NM - 소터 오류 명
        private string _SRT_ERR_NM;
        public string SRT_ERR_NM
        {
            get { return this._SRT_ERR_NM; }
            set
            {
                if (this._SRT_ERR_NM != value)
                {
                    this._SRT_ERR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SRT_RSN_CD - 소터 사유코드
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

        #region + SRT_RSN_NM - 소터 Reject 사유
        private string _SRT_RSN_NM;
        public string SRT_RSN_NM
        {
            get { return this._SRT_RSN_NM; }
            set
            {
                if (this._SRT_RSN_NM != value)
                {
                    this._SRT_RSN_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
        
        #region + SRT_WRK_CMPT_DT - 분류일시
        private string _SRT_WRK_CMPT_DT;
        public string SRT_WRK_CMPT_DT
        {
            get { return this._SRT_WRK_CMPT_DT; }
            set
            {
                if (this._SRT_WRK_CMPT_DT != value)
                {
                    this._SRT_WRK_CMPT_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + INV_BCD - 송장 바코드
        private string _INV_BCD;
        public string INV_BCD
        {
            get { return this._INV_BCD; }
            set
            {
                if (this._INV_BCD != value)
                {
                    this._INV_BCD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
