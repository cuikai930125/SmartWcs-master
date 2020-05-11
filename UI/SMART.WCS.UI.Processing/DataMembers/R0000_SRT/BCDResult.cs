using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Processing.DataMembers.R0000_SRT
{
    public class BCDResult : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public BCDResult()
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
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + INDUCTION_NO - 투입 인덕션 번호
        private string _INDUCTION_NO;
        public string INDUCTION_NO
        {
            get { return this._INDUCTION_NO; }
            set
            {
                if (this._INDUCTION_NO != value)
                {
                    this._INDUCTION_NO = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CART_NO - 카트 번호
        private string _CART_NO;
        public string CART_NO
        {
            get { return this._CART_NO; }
            set
            {
                if (this._CART_NO != value)
                {
                    this._CART_NO = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CART_CNT - 카트 CNT
        private string _CART_CNT;
        public string CART_CNT
        {
            get { return this._CART_CNT; }
            set
            {
                if (this._CART_CNT != value)
                {
                    this._CART_CNT = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + INDT_YMD_HMS - 투입시간
        private string _INDT_YMD_HMS;
        public string INDT_YMD_HMS
        {
            get { return this._INDT_YMD_HMS; }
            set
            {
                if (this._INDT_YMD_HMS != value)
                {
                    this._INDT_YMD_HMS = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PLAN_CD - 계획 코드
        private string _PLAN_CD;
        public string PLAN_CD
        {
            get { return this._PLAN_CD; }
            set
            {
                if (this._PLAN_CD != value)
                {
                    this._PLAN_CD = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_CD - 박스 바코드
        private string _BOX_CD;
        public string BOX_CD
        {
            get { return this._BOX_CD; }
            set
            {
                if (this._BOX_CD != value)
                {
                    this._BOX_CD = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RGN_CD - 분류 코드
        private string _RGN_CD;
        public string RGN_CD
        {
            get { return this._RGN_CD; }
            set
            {
                if (this._RGN_CD != value)
                {
                    this._RGN_CD = value;
                    this.RaisePropertyChanged();
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
                    this.RaisePropertyChanged();
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
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PLAN_CHUTE_ID1 - 계획 슈트1
        private string _PLAN_CHUTE_ID1;
        public string PLAN_CHUTE_ID1
        {
            get { return this._PLAN_CHUTE_ID1; }
            set
            {
                if (this._PLAN_CHUTE_ID1 != value)
                {
                    this._PLAN_CHUTE_ID1 = value;
                    this.RaisePropertyChanged();
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
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SRT_RSN_CD - 소터 사유 코드
        private string _SRT_RSN_CD;
        public string SRT_RSN_CD
        {
            get { return this._SRT_RSN_CD; }
            set
            {
                if (this._SRT_RSN_CD != value)
                {
                    this._SRT_RSN_CD = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RSLT_CHUTE_ID - 실적 슈트
        private string _RSLT_CHUTE_ID;
        public string RSLT_CHUTE_ID
        {
            get { return this._RSLT_CHUTE_ID; }
            set
            {
                if (this._RSLT_CHUTE_ID != value)
                {
                    this._RSLT_CHUTE_ID = value;
                    this.RaisePropertyChanged();
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
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PROC_MSG - 처리 메시지
        private string _PROC_MSG;
        public string PROC_MSG
        {
            get { return this._PROC_MSG; }
            set
            {
                if (this._PROC_MSG != value)
                {
                    this._PROC_MSG = value;
                    this.RaisePropertyChanged();
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
    }
}
