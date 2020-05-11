using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P1007_GAN
{
    public class WrkPlanInfo : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public WrkPlanInfo()
        {
            //this.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
        }

        //public Brush BaseBackgroundBrush { get; set; }

        //public Brush BackgroundBrush { get; set; }
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

        #region + BTCH_NO - 배치번호
        private string _BTCH_NO;
        public string BTCH_NO
        {
            get { return this._BTCH_NO; }
            set
            {
                if (this._BTCH_NO != value)
                {
                    this._BTCH_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ORD_YMD - 오더일자
        private string _ORD_YMD;
        /// <summary>
        /// 회사코드
        /// </summary>
        public string ORD_YMD
        {
            get { return this._ORD_YMD; }
            set
            {
                if (this._ORD_YMD != value)
                {
                    this._ORD_YMD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + WAV_NO - 센터코드
        private string _CNTR_CD;
        /// <summary>
        /// 센터코드
        /// </summary>
        public string WAV_NO
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

        #region + ORD_NO - 토트박스 번호
        private string _TOT_BOX_ID;
        /// <summary>
        /// 토트박스 번호
        /// </summary>
        public string ORD_NO
        {
            get { return this._TOT_BOX_ID; }
            set
            {
                if (this._TOT_BOX_ID != value)
                {
                    this._TOT_BOX_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_CD - 토트박스 타입
        private string _BOX_TYPE_CD;
        /// <summary>
        /// 토트박스 타입
        /// </summary>
        public string SKU_CD
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

        #region + SKU_NM - 토트박스 길이
        private string _BOX_VERT_LEN;
        /// <summary>
        /// 토트박스 길이
        /// </summary>
        public string SKU_NM
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

        #region + BOX_IN_QTY - 토트박스 너비
        private decimal _BOX_IN_QTY;
        /// <summary>
        /// 토트박스 너비
        /// </summary>
        public decimal BOX_IN_QTY
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

        #region + BOX_HGT_LEN - 토트박스 높이
        private decimal _BOX_QTY;
        /// <summary>
        /// 토트박스 높이
        /// </summary>
        public decimal BOX_QTY
        {
            get { return this._BOX_QTY; }
            set
            {
                if (this._BOX_QTY != value)
                {
                    this._BOX_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PLAN_QTY - 토트박스 상태
        private decimal _PLAN_QTY;
        /// <summary>
        /// 토트박스 상태
        /// </summary>
        public decimal PLAN_QTY
        {
            get { return this._PLAN_QTY; }
            set
            {
                if (this._PLAN_QTY != value)
                {
                    this._PLAN_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + WRK_STAT_CD - 사용 여부
        private string _USE_YN;
        public string WRK_STAT_CD
        {
            get { return this._USE_YN; }
            set
            {
                if (this._USE_YN != value)
                {
                    this._USE_YN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_ID - 사용 여부
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

        #region + ORD_LINE_NO - 오더라인
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
        # endregion

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
