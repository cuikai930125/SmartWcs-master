using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1016
{
    public class BoxTypeMgnt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public BoxTypeMgnt()
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

        #region + CST_CD - 고객사 코드
        private string _CST_CD;
        public string CST_CD
        {
            get { return this._CST_CD; }
            set
            {
                if (this._CST_CD != value)
                {
                    this._CST_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CST_NM - 고객사명
        private string _CST_NM;
        public string CST_NM
        {
            get { return this._CST_NM; }
            set
            {
                if (this._CST_NM != value)
                {
                    this._CST_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CNTR_CD - 센터 코드
        private string _CNTR_CD;
        public string CNTR_CD
        {
            get { return this._CNTR_CD; }
            set
            {
                if (this._CNTR_CD != value)
                {
                    this._CNTR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_TYPE_CD - 박스 유형 코드
        private string _BOX_TYPE_CD;
        public string BOX_TYPE_CD
        {
            get { return this._BOX_TYPE_CD; }
            set
            {
                if (this._BOX_TYPE_CD != value)
                {
                    this._BOX_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_TYPE_NM - 박스 유형 명
        private string _BOX_TYPE_NM;
        public string BOX_TYPE_NM
        {
            get { return this._BOX_TYPE_NM; }
            set
            {
                if (this._BOX_TYPE_NM != value)
                {
                    this._BOX_TYPE_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_TYPE_DESC - 박스 유형 설명
        private string _BOX_TYPE_DESC;
        public string BOX_TYPE_DESC
        {
            get { return this._BOX_TYPE_DESC; }
            set
            {
                if (this._BOX_TYPE_DESC != value)
                {
                    this._BOX_TYPE_DESC = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_TYPE_GRP_CD - 박스 그룹 코드
        private string _BOX_TYPE_GRP_CD;
        public string BOX_TYPE_GRP_CD
        {
            get { return this._BOX_TYPE_GRP_CD; }
            set
            {
                if (this._BOX_TYPE_GRP_CD != value)
                {
                    this._BOX_TYPE_GRP_CD = value;
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

        #region + BOX_WTH_LEN - 박스 가로 길이
        private decimal _BOX_WTH_LEN;
        public decimal BOX_WTH_LEN
        {
            get { return this._BOX_WTH_LEN; }
            set
            {
                if (this._BOX_WTH_LEN != value)
                {
                    this._BOX_WTH_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_VERT_LEN - 박스 세로 길이
        private decimal _BOX_VERT_LEN;
        public decimal BOX_VERT_LEN
        {
            get { return this._BOX_VERT_LEN; }
            set
            {
                if (this._BOX_VERT_LEN != value)
                {
                    this._BOX_VERT_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_HGT_LEN - 박스 높이 길이
        private decimal _BOX_HGT_LEN;
        public decimal BOX_HGT_LEN
        {
            get { return this._BOX_HGT_LEN; }
            set
            {
                if (this._BOX_HGT_LEN != value)
                {
                    this._BOX_HGT_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + LEN_UOM - 길이 단위
        private string _LEN_UOM;
        public string LEN_UOM
        {
            get { return this._LEN_UOM; }
            set
            {
                if (this._LEN_UOM != value)
                {
                    this._LEN_UOM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_CBM - 박스 체적
        private decimal _BOX_CBM;
        public decimal BOX_CBM
        {
            get { return this._BOX_CBM; }
            set
            {
                if (this._BOX_CBM != value)
                {
                    this._BOX_CBM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CBM_UOM - CBM 단위
        private string _CBM_UOM;
        public string CBM_UOM
        {
            get { return this._CBM_UOM; }
            set
            {
                if (this._CBM_UOM != value)
                {
                    this._CBM_UOM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_WGT - 박스 중량
        private decimal _BOX_WGT;
        public decimal BOX_WGT
        {
            get { return this._BOX_WGT; }
            set
            {
                if (this._BOX_WGT != value)
                {
                    this._BOX_WGT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + WGT_UOM - 무게 UOM
        private string _WGT_UOM;
        public string WGT_UOM
        {
            get { return this._WGT_UOM; }
            set
            {
                if (this._WGT_UOM != value)
                {
                    this._WGT_UOM = value;
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

