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
    /// 로케이션 배치정보 관리
    /// </summary>
    public class LocBtchInfoMgt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass             = new BaseClass();
        private bool g_isValidation     = false;

        #region * 그리드 색상 설정
        public LocBtchInfoMgt()
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

        #region + LOC_CD - 로케이션
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

        #region + Interval
        private int _BTCH_TERM;
        public int BTCH_TERM
        {
            get { return this._BTCH_TERM; }
            set
            {
                if (this._BTCH_TERM != value)
                {
                    this._BTCH_TERM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BTCH_PICK_SKU_CNT - SKU 수
        private int _BTCH_PICK_SKU_CNT;
        public int BTCH_PICK_SKU_CNT
        {
            get { return this._BTCH_PICK_SKU_CNT; }
            set
            {
                if (this._BTCH_PICK_SKU_CNT != value)
                {
                    this._BTCH_PICK_SKU_CNT = value;
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
