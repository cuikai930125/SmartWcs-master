using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1012
{
    public class SkuInfoMgmt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public SkuInfoMgmt()
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

        #region + SKU 무게
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

        #region + SKU_CBM - SKU CBM
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

        #region + SKU_CLS - SKU 분류 코드
        private string _SKU_CLS;
        public string SKU_CLS
        {
            get { return this._SKU_CLS ; }
            set
            {
                if (this._SKU_CLS != value)
                {
                    this._SKU_CLS = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_TMPT_TYPE_CD - 상품 온도 유형
        private string _SKU_TMPT_TYPE_CD;
        public string SKU_TMPT_TYPE_CD
        {
            get { return this._SKU_TMPT_TYPE_CD; }
            set
            {
                if (this._SKU_TMPT_TYPE_CD != value)
                {
                    this._SKU_TMPT_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_IN_QTY - 박스 당 개수
        private int _BOX_IN_QTY;
        public int BOX_IN_QTY
        {
            get { return this._BOX_IN_QTY; }
            set
            {
                if (this._BOX_IN_QTY != value)
                {
                    this._BOX_IN_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_WGT 박스 무게
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

        #region + BOX_WTH_LEN 박스 가로
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

        #region + BOX_VERT_LEN 박스 세로
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

        #region + BOX_HGT_LEN 박스 높이
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

        #region + BOX_CBM 박스 CBM
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

        #region + WGT_UOM 중량 단위
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

        #region + LEN_UOM 길이 단위
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

        #region + CBM_UOM CBM 단위
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

        #region + CELL_TYPE_CD 셀 타입 코드
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

        #region + CRT_SPR_CD 시스템 코드
        private string _CRT_SPR_CD;
        public string CRT_SPR_CD
        {
            get { return this._CRT_SPR_CD; }
            set
            {
                if (this._CRT_SPR_CD != value)
                {
                    this._CRT_SPR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + USE_YN 사용 여부
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
