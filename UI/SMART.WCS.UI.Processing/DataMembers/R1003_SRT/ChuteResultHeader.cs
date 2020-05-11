using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using SMART.WCS.Common.Extensions;

namespace SMART.WCS.UI.Processing.DataMembers.R1003_SRT
{
    class ChuteResultHeader : PropertyNotifyExtensions
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public ChuteResultHeader()
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

        #region + SCAN_DT - 일자
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

        #region + PLAN_CHUTE_ID1 - 계획슈트
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

        #region + RGN_BCD - 권역
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

        #region + SRT_ERR_CD - 에러사유
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

        #region + SRT_ERR_NM - 에러사유
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

        #region + SRT_RSN_NM - 설비사유
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

        #region + SRT_WRK_STAT_NM - 상태 명
        private string _SRT_WRK_STAT_NM;
        public string SRT_WRK_STAT_NM
        {
            get { return this._SRT_WRK_STAT_NM; }
            set
            {
                if (this._SRT_WRK_STAT_NM != value)
                {
                    this._SRT_WRK_STAT_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + QTY - 수량
        private string _QTY;
        public string QTY
        {
            get { return this._QTY; }
            set
            {
                if (this._QTY != value)
                {
                    this._QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
