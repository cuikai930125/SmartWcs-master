using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Processing.DataMembers.R1002_SRT
{
    class ScanResultMgmt : PropertyNotifyExtensions
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public ScanResultMgmt()
        {
            //this.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
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

        #region + BOX_BCD - 박스바코드
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

        #region + RGN_BCD - 분류코드
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

        #region + BCD_INFO - 바코드
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

        #region + PLAN_CD - 플랜코드
        private string _PLAN_CD;
        public string PLAN_CD
        {
            get { return this._PLAN_CD; }
            set
            {
                if (this._PLAN_CD != value)
                {
                    this._PLAN_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + INDT_ID - 인덕션
        private string _INDT_ID;
        public string INDT_ID
        {
            get { return this._INDT_ID; }
            set
            {
                if (this._INDT_ID != value)
                {
                    this._INDT_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PLAN_CHUTE - 계획슈트
        private string _PLAN_CHUTE;
        public string PLAN_CHUTE
        {
            get { return this._PLAN_CHUTE; }
            set
            {
                if (this._PLAN_CHUTE != value)
                {
                    this._PLAN_CHUTE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RSLT_CHUTE_ID - 실적슈트
        private string _RSLT_CHUTE_ID;
        public string RSLT_CHUTE_ID
        {
            get { return this._RSLT_CHUTE_ID; }
            set
            {
                if (this._RSLT_CHUTE_ID != value)
                {
                    this._RSLT_CHUTE_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SCAN_DT - 투입일시
        private string _SCAN_DT;
        public string SCAN_DT
        {
            get { return this._SCAN_DT; }
            set
            {
                if (this._SCAN_DT != value)
                {
                    this._SCAN_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SRT_WRK_STAT_CD - 상태
        private string _SRT_WRK_STAT_CD;
        public string SRT_WRK_STAT_CD
        {
            get { return this._SRT_WRK_STAT_CD; }
            set
            {
                if (this._SRT_WRK_STAT_CD != value)
                {
                    this._SRT_WRK_STAT_CD = value;
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

        #region + SRT_ERR_CD - 오류내용
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

        #region + ERR_CODE - 오류 코드
        private string _ERR_CODE;
        public string ERR_CODE
        {
            get { return this._ERR_CODE; }
            set
            {
                if (this._ERR_CODE != value)
                {
                    this._ERR_CODE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SRT_RSN_CD - 설비사유
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

        private string _INDT_YMD_HMS;
        public string INDT_YMD_HMS
        {
            get { return this._INDT_YMD_HMS; }
            set
            {
                if (this._INDT_YMD_HMS != value)
                {
                    this._INDT_YMD_HMS = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region IF_YN - I/F 여부
        private string _IF_YN;
        public string IF_YN
        {
            get { return this._IF_YN; }
            set
            {
                if (this._IF_YN != value)
                {
                    this._IF_YN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
