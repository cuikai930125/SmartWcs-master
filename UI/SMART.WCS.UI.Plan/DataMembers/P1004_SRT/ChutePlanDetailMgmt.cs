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
    class ChutePlanDetailMgmt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public ChutePlanDetailMgmt()
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

        #region + RGN_NM - 권역 명
        private string _RGN_NM;
        public string RGN_NM
        {
            get { return this._RGN_NM; }
            set
            {
                if (this._RGN_NM != value)
                {
                    this._RGN_NM = value;
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

        #region + CHUTE_NM - 슈트 명
        private string _CHUTE_NM;
        public string CHUTE_NM
        {
            get { return this._CHUTE_NM; }
            set
            {
                if (this._CHUTE_NM != value)
                {
                    this._CHUTE_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DLV_CO_NM - 택배사명
        private string _DLV_CO_NM;
        public string DLV_CO_NM
        {
            get { return this._DLV_CO_NM; }
            set
            {
                if (this._DLV_CO_NM != value)
                {
                    this._DLV_CO_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DLV_CO_RGN_CD - 택배사 지역코드
        private string _DLV_CO_RGN_CD;
        public string DLV_CO_RGN_CD
        {
            get { return this._DLV_CO_RGN_CD; }
            set
            {
                if (this._DLV_CO_RGN_CD != value)
                {
                    this._DLV_CO_RGN_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DLV_CO_RGN_NM - 택배사 지역명
        private string _DLV_CO_RGN_NM;
        public string DLV_CO_RGN_NM
        {
            get { return this._DLV_CO_RGN_NM; }
            set
            {
                if (this._DLV_CO_RGN_NM != value)
                {
                    this._DLV_CO_RGN_NM = value;
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
