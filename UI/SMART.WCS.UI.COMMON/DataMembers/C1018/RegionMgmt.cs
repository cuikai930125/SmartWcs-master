using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1018
{
    class RegionMgmt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public RegionMgmt()
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

        #region + DLV_CO_CD - 택배사 코드
        private string _DLV_CO_CD;
        public string DLV_CO_CD
        {
            get { return this._DLV_CO_CD; }
            set
            {
                if (this._DLV_CO_CD != value)
                {
                    this._DLV_CO_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DLV_CO_NM - 택배사 명
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

        #region + DLV_CO_RGN_CD - 택배사 지역 코드
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

        #region + DLV_CO_RGN_NM - 택배사 지역 코드
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

        #region + REG_DT - 등록일시
        private string _REG_DT;
        public string REG_DT
        {
            get { return this._REG_DT; }
            set
            {
                if (this._REG_DT != value)
                {
                    this._REG_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + REG_ID - 등록일시
        private string _REG_ID;
        public string REG_ID
        {
            get { return this._REG_ID; }
            set
            {
                if (this._REG_ID != value)
                {
                    this._REG_ID = value;
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

        #region + UPD_ID - 수정일시
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
    }
}
