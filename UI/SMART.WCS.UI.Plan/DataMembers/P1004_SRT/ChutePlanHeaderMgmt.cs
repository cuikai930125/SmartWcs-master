using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Plan.DataMembers.P1004_SRT
{
    public class ChutePlanHeaderMgmt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public ChutePlanHeaderMgmt()
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


        #region + P_CO_CD - 회사 코드
        private string _P_CO_CD;
        public string P_CO_CD
        {
            get { return this._P_CO_CD; }
            set
            {
                if (this._P_CO_CD != value)
                {
                    this._P_CO_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + P_CNTR_CD - 센터 코드
        private string _P_CNTR_CD;
        public string P_CNTR_CD
        {
            get { return this._P_CNTR_CD; }
            set
            {
                if (this._P_CNTR_CD != value)
                {
                    this._P_CNTR_CD = value;
                    RaisePropertyChanged();
                }
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
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PLAN_CD - PLAN 코드
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

        #region + PLAN_NM - PLAN 명
        private string _PLAN_NM;
        public string PLAN_NM
        {
            get { return this._PLAN_NM; }
            set
            {
                if (this._PLAN_NM != value)
                {
                    this._PLAN_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RGN_CD - 권역 코드
        private string _RGN_CD;
        public string RGN_CD
        {
            get { return this._RGN_CD; }
            set
            {
                if (this._RGN_CD != value)
                {
                    this._RGN_CD = value;
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

        #region + UPD_ID - 수정자 ID
        private string _UPD_ID;
        public string UPD_ID
        {
            get { return this._UPD_ID; }
            set
            {
                if (this._UPD_ID != value)
                {
                    this._UPD_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + UPD_DT - 수정일시
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

        #region + USE_YN - 사용 여부
        private string _USE_YN;
        public string USE_YN
        {
            get { return this._USE_YN; }
            set
            {
                if (this._USE_YN != value)
                {
                    this._USE_YN = value;
                    this.Checked = this.USE_YN.Equals("Y") ? true : false;
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

        #region + IsApplied - 적용 여부
        private bool _IsApplied;
        public bool IsApplied
        {
            get { return this._IsApplied; }
            set
            {
                if (this._IsApplied != value)
                {
                    this._IsApplied = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
