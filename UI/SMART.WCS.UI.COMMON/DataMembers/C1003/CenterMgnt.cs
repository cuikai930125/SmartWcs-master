using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1003
{
    public class CenterMgnt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public CenterMgnt()
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

        #region + CNTR_NM - 센터 명
        private string _CNTR_NM;
        public string CNTR_NM
        {
            get { return this._CNTR_NM; }
            set
            {
                if (this._CNTR_NM != value)
                {
                    this._CNTR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DB_CONN_TYPE - DB 접속 타입
        private string _DB_CONN_TYPE;
        public string DB_CONN_TYPE
        {
            get { return this._DB_CONN_TYPE; }
            set
            {
                if (this._DB_CONN_TYPE != value)
                {
                    this._DB_CONN_TYPE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ORCL_CONN_STR - Oracle 접속 정보
        private string _ORCL_CONN_STR;
        public string ORCL_CONN_STR
        {
            get { return this._ORCL_CONN_STR; }
            set
            {
                if (this._ORCL_CONN_STR != value)
                {
                    this._ORCL_CONN_STR = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MS_CONN_STR - MS SQL 접속 정보
        private string _MS_CONN_STR;
        public string MS_CONN_STR
        {
            get { return this._MS_CONN_STR; }
            set
            {
                if (this._MS_CONN_STR != value)
                {
                    this._MS_CONN_STR = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MR_CONN_STR - MariaDB 접속 정보
        private string _MR_CONN_STR;
        public string MR_CONN_STR
        {
            get { return this._MR_CONN_STR; }
            set
            {
                if (this._MR_CONN_STR != value)
                {
                    this._MR_CONN_STR = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        private DateTime _FR_CURR_DATE;
        public DateTime FR_CURR_DATE
        {
            get { return this._FR_CURR_DATE; }
            set
            {
                if (this._FR_CURR_DATE != value)
                {
                    this._FR_CURR_DATE = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _FR_INIT_YMD_DIFF;
        public int FR_INIT_YMD_DIFF
        {
            get { return this._FR_INIT_YMD_DIFF; }
            set
            {
                if (this._FR_INIT_YMD_DIFF != value)
                {
                    this._FR_INIT_YMD_DIFF = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime _FR_INIT_HM;
        public DateTime FR_INIT_HM
        {
            get { return this._FR_INIT_HM; }
            set
            {
                if (this._FR_INIT_HM != value)
                {
                    this._FR_INIT_HM = value;
                    RaisePropertyChanged();
                }
            }
        }


        private DateTime _TO_CURR_DATE;
        public DateTime TO_CURR_DATE
        {
            get { return this._TO_CURR_DATE; }
            set
            {
                if (this._TO_CURR_DATE != value)
                {
                    this._TO_CURR_DATE = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _TO_INIT_YMD_DIFF;
        public int TO_INIT_YMD_DIFF
        {
            get { return this._TO_INIT_YMD_DIFF; }
            set
            {
                if (this._TO_INIT_YMD_DIFF != value)
                {
                    this._TO_INIT_YMD_DIFF = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime _TO_INIT_HM;
        public DateTime TO_INIT_HM
        {
            get { return this._TO_INIT_HM; }
            set
            {
                if (this._TO_INIT_HM != value)
                {
                    this._TO_INIT_HM = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region + IP_MGMT_YN - 사용 여부
        private string _IP_MGMT_YN;
        public string IP_MGMT_YN
        {
            get { return this._IP_MGMT_YN; }
            set
            {
                if (this._IP_MGMT_YN != value)
                {
                    this._IP_MGMT_YN = value;
                    this.IP_MGMT_Checked = this._IP_MGMT_YN.Equals("Y") ? true : false;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + IP_MGMT_Checked - 사용여부 변환값
        private bool _IP_MGMT_Checked;
        public bool IP_MGMT_Checked
        {
            get { return this._IP_MGMT_Checked; }
            set
            {
                if (this._IP_MGMT_Checked != value)
                {
                    this._IP_MGMT_Checked = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ADDR - 주소
        private string _ADDR;
        public string ADDR
        {
            get { return this._ADDR; }
            set
            {
                if (this._ADDR != value)
                {
                    if (value == null) { value = string.Empty; }
                    this._ADDR = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TEL_NO - 연락처
        private string _TEL_NO;
        public string TEL_NO
        {
            get { return this._TEL_NO; }
            set
            {
                if (this._TEL_NO != value)
                {
                    this._TEL_NO = value;
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
