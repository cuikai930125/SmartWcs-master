using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Optimization.DataMembers.OPT0104
{
    /// <summary>
    /// SKU 셀유형타입
    /// </summary>
    public class SkuCellTypeMgt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass             = new BaseClass();
        private bool g_isValidation     = false;

        #region * 그리드 색상 설정
        public SkuCellTypeMgt()
        {
            this.BackgroundBrush        = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
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

        #region + SKU_CD - SKU 코드
        private string _SKU_CD;
        public string SKU_CD
        {
            get { return this._SKU_CD; }
            set
            {
                if (this._SKU_CD != value)
                {
                    this._SKU_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_NM - SKU명
        private string _SKU_NM;
        public string SKU_NM
        {
            get { return this._SKU_NM; }
            set
            {
                if (this._SKU_NM != value)
                {
                    this._SKU_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_CBM - SKU CBM
        private int _SKU_CBM;
        public int SKU_CBM
        {
            get { return this._SKU_CBM; }
            set
            {
                if (this._SKU_CBM != value)
                {
                    this._SKU_CBM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_WTH_LEN - 가로
        private decimal _SKU_WTH_LENGT;
        public decimal SKU_WTH_LEN
        {
            get { return this._SKU_WTH_LENGT; }
            set
            {
                if (this._SKU_WTH_LENGT != value)
                {
                    this._SKU_WTH_LENGT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_VERT_LEN - 세로
        private decimal _SKU_VERT_LEN;
        public decimal SKU_VERT_LEN
        {
            get { return this._SKU_VERT_LEN; }
            set
            {
                if (this._SKU_VERT_LEN != value)
                {
                    this._SKU_VERT_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_HGT_LEN - 높이
        private decimal _SKU_HGT_LEN;
        public decimal SKU_HGT_LEN
        {
            get { return this._SKU_HGT_LEN; }
            set
            {
                if (this._SKU_HGT_LEN != value)
                {
                    this._SKU_HGT_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CELL_TYPE_CD - 셀유형코드
        private string _CELL_TYPE_CD;
        public string CELL_TYPE_CD
        {
            get { return this._CELL_TYPE_CD; }
            set
            {
                if (this._CELL_TYPE_CD != value)
                {
                    this._CELL_TYPE_CD = value;
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
