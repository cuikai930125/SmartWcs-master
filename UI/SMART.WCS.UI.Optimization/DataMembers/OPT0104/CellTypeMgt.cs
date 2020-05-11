using DevExpress.Utils;
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
    /// 셀유형 관리
    /// </summary>
    public class CellTypeMgt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass             = new BaseClass();
        private bool g_isValidation     = false;

        #region * 그리드 색상 설정
        public CellTypeMgt()
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

        #region + CELL_TYPE_CD - 셀 유형 코드
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

        #region + CELL_TYPE_NM - 셀 유형명
        private string _CELL_TYPE_NM;
        public string CELL_TYPE_NM
        {
            get { return this._CELL_TYPE_NM; }
            set
            {
                if (this._CELL_TYPE_NM != value)
                {
                    this._CELL_TYPE_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CELL_WTH_LEN - 가로
        private decimal _CELL_WTH_LENGT;
        public decimal CELL_WTH_LEN
        {
            get { return this._CELL_WTH_LENGT; }
            set
            {
                if (this._CELL_WTH_LENGT != value)
                {
                    this._CELL_WTH_LENGT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CELL_VERT_LEN - 세로
        private decimal _CELL_VERT_LEN;
        public decimal CELL_VERT_LEN
        {
            get { return this._CELL_VERT_LEN; }
            set
            {
                if (this._CELL_VERT_LEN != value)
                {
                    this._CELL_VERT_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CELL_HGT_LEN - 높이
        private decimal _CELL_HGT_LEN;
        public decimal CELL_HGT_LEN
        {
            get { return this._CELL_HGT_LEN; }
            set
            {
                if (this._CELL_HGT_LEN != value)
                {
                    this._CELL_HGT_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CELL_QTY - 셀 수
        private decimal _CELL_QTY;
        public decimal CELL_QTY
        {
            get { return this._CELL_QTY; }
            set
            {
                if (this._CELL_QTY != value)
                {
                    this._CELL_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CELL_CBM - CBM
        private decimal _CELL_CBM;
        public decimal CELL_CBM
        {
            get { return this._CELL_CBM; }
            set
            {
                if (this._CELL_CBM != value)
                {
                    this._CELL_CBM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MIN_FILL_RT - 충진율
        private decimal _MIN_FILL_RT;
        public decimal MIN_FILL_RT
        {
            get { return this._MIN_FILL_RT; }
            set
            {
                if (this._MIN_FILL_RT != value)
                {
                    this._MIN_FILL_RT = value;
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
                    this._USE_YN        = value;
                    this.Checked        = this.USE_YN.Equals("Y") ? true : false;
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
