using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1023_GAN
{
    public class TotBoxMgnt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public TotBoxMgnt()
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

        #region + CO_CD - 회사코드
        private string _CO_CD;
        /// <summary>
        /// 회사코드
        /// </summary>
        public string CO_CD
        {
            get { return this._CO_CD; }
            set
            {
                if (this._CO_CD != value)
                {
                    this._CO_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CNTR_CD - 센터코드
        private string _CNTR_CD;
        /// <summary>
        /// 센터코드
        /// </summary>
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

        #region + TOT_BOX_ID - 토트박스 번호
        private string _TOT_BOX_ID;
        /// <summary>
        /// 토트박스 번호
        /// </summary>
        public string TOT_BOX_ID
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

        #region + BOX_TYPE_CD - 토트박스 타입
        private string _BOX_TYPE_CD;
        /// <summary>
        /// 토트박스 타입
        /// </summary>
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

        #region + BOX_VERT_LEN - 토트박스 길이
        private string _BOX_VERT_LEN;
        /// <summary>
        /// 토트박스 길이
        /// </summary>
        public string BOX_VERT_LEN
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

        #region + BOX_WTH_LEN - 토트박스 너비
        private string _BOX_WTH_LEN;
        /// <summary>
        /// 토트박스 너비
        /// </summary>
        public string BOX_WTH_LEN
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

        #region + BOX_HGT_LEN - 토트박스 높이
        private string _BOX_HGT_LEN;
        /// <summary>
        /// 토트박스 높이
        /// </summary>
        public string BOX_HGT_LEN
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

        #region + TOT_BOX_STAT_CD - 토트박스 상태
        private string _TOT_BOX_STAT_CD;
        /// <summary>
        /// 토트박스 상태
        /// </summary>
        public string TOT_BOX_STAT_CD
        {
            get { return this._TOT_BOX_STAT_CD; }
            set
            {
                if (this._TOT_BOX_STAT_CD != value)
                {
                    this._TOT_BOX_STAT_CD = value;
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
