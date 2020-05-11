using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1002
{
    public class UserMgnt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass             = new BaseClass();
        private bool g_isValidation     = false;

        #region * 그리드 색상 설정
        public UserMgnt()
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

        #region + USER_ID - 사용자 ID
        private string _USER_ID;
        public string USER_ID
        {
            get { return this._USER_ID; }
            set
            {
                if (this._USER_ID != value)
                {
                    this._USER_ID = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + USER_NM - 사용자명
        private string _USER_NM;
        public string USER_NM
        {
            get { return this._USER_NM; }
            set
            {
                if (this._USER_NM != value)
                {
                    this._USER_NM = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ROLE_CD - 권한코드
        private string _ROLE_CD;
        public string ROLE_CD
        {
            get { return this._ROLE_CD; }
            set
            {
                if (this._ROLE_CD != value)
                {
                    this._ROLE_CD = value;
                    this.RaisePropertyChanged();
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
                    this._USE_YN            = value;
                    this.USE_YN_CHECKED     = this.USE_YN.Equals("Y") ? true : false;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + USE_YN_CHECKED - 사용 여부 변환값
        private bool _USE_YN_CHECKED;
        public bool USE_YN_CHECKED
        {
            get { return this._USE_YN_CHECKED; }
            set
            {
                if (this._USE_YN_CHECKED != value)
                {
                    this._USE_YN_CHECKED = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PWD_INIT_YN - 비밀번호 초기화 여부
        private string _PWD_INIT_YN;
        public string PWD_INIT_YN
        {
            get { return this._PWD_INIT_YN; }
            set
            {
                if (this._PWD_INIT_YN != value)
                {
                    this._PWD_INIT_YN = value;
                    this.PWD_INIT_YN_CHECKED = this._PWD_INIT_YN.Equals("Y") ? true : false;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PWD_INIT_YN_CHECKED - 비밀번호 초기화 여부 변환값
        private bool _PWD_INIT_YN_CHECKED;
        public bool PWD_INIT_YN_CHECKED
        {
            get { return this._PWD_INIT_YN_CHECKED; }
            set
            {
                if (this._PWD_INIT_YN_CHECKED != value)
                {
                    this._PWD_INIT_YN_CHECKED = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
