using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Optimization.DataMembers.OPT0107
{
    public class OrderCreateMgt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass                 = new BaseClass();
        private bool g_isValidation         = false;

        #region * 그리드 색상 설정
        public OrderCreateMgt()
        {
            this.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
        }

        public System.Windows.Media.Brush BaseBackgroundBrush { get; set; }

        public System.Windows.Media.Brush BackgroundBrush { get; set; }
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

        #region + 고객사 코드 - CST_CD
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

        #region + 데이터 그룹 - WAV_NO
        private string _WAV_NO;
        public string WAV_NO
        {
            get { return this._WAV_NO; }
            set
            {
                if (this._WAV_NO != value)
                {
                    this._WAV_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 오더 번호 - ORD_NO
        private string _ORD_NO;
        public string ORD_NO
        {
            get { return this._ORD_NO; }
            set
            {
                if (this._ORD_NO != value)
                {
                    this._ORD_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 오더 라인번호 - ORD_LINE_NO
        private string _ORD_LINE_NO;
        public string ORD_LINE_NO
        {
            get { return this._ORD_LINE_NO; }
            set
            {
                if (this._ORD_LINE_NO != value)
                {
                    this._ORD_LINE_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 거래처 코드 - SHIP_TO_CD
        private string _SHIP_TO_CD;
        public string SHIP_TO_CD
        {
            get { return this._SHIP_TO_CD; }
            set
            {
                if (this._SHIP_TO_CD != value)
                {
                    this._SHIP_TO_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU 코드 - SKU_CD
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

        #region + SKU CBM - SKU_CBM
        private decimal _SKU_CBM;
        public decimal SKU_CBM
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

        #region + SKU 가로 - SKU_WTH_LEN
        private decimal _SKU_WTH_LEN;
        public decimal SKU_WTH_LEN
        {
            get { return this._SKU_WTH_LEN; }
            set
            {
                if (this._SKU_WTH_LEN != value)
                {
                    this._SKU_WTH_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU 세로 - SKU_VERT_LEN
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

        #region + SKU 높이 - SKU_HGT_LEN
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

        #region + SKU 중량 - SKU_WGT
        private decimal _SKU_WGT;
        public decimal SKU_WGT
        {
            get { return this._SKU_WGT; }
            set
            {
                if (this._SKU_WGT != value)
                {
                    this._SKU_WGT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + Location 코드 - LOC_CD
        private string _LOC_CD;
        public string LOC_CD
        {
            get { return this._LOC_CD; }
            set
            {
                if (this._LOC_CD != value)
                {
                    this._LOC_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 계획 수량 - PLAN_QTY
        private int _PLAN_QTY;
        public int PLAN_QTY
        {
            get { return this._PLAN_QTY; }
            set
            {
                if  (this._PLAN_QTY != value)
                {
                    this._PLAN_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
