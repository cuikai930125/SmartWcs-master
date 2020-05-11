using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS.DataMembers.E1001
{
    public class EmsEqpMngr : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        private bool g_isValidation = false;

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

        string _CENTER_CD;
        public string CENTER_CD
        {
            get
            {
                return _CENTER_CD;
            }
            set
            {
                if (_CENTER_CD != value)
                {
                    _CENTER_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_MNGR_ID;
        /// <summary>
        /// ID
        /// </summary>
        public string EQP_MNGR_ID
        {
            get
            {
                return _EQP_MNGR_ID;
            }
            set
            {
                if (_EQP_MNGR_ID != value)
                {
                    _EQP_MNGR_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _MNGR_NM;
        public string MNGR_NM
        {
            get
            {
                return _MNGR_NM;
            }
            set
            {
                if (_MNGR_NM != value)
                {
                    _MNGR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _MNGR_TEL_NO;
        public string MNGR_TEL_NO
        {
            get
            {
                return _MNGR_TEL_NO;
            }
            set
            {
                if (_MNGR_TEL_NO != value)
                {
                    _MNGR_TEL_NO = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EMPL_DEV_CD;
        /// <summary>
        /// 고용 구분
        /// </summary>
        public string EMPL_DEV_CD
        {
            get
            {
                return _EMPL_DEV_CD;
            }
            set
            {
                if (_EMPL_DEV_CD != value)
                {
                    _EMPL_DEV_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _VNDR_NM;
        /// <summary>
        /// 업체명
        /// </summary>
        public string VNDR_NM
        {
            get
            {
                return _VNDR_NM;
            }
            set
            {
                if (_VNDR_NM != value)
                {
                    _VNDR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _USE_YN;
        public string USE_YN
        {
            get
            {
                return _USE_YN;
            }
            set
            {
                if (_USE_YN != value)
                {
                    _USE_YN = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _REG_DT;
        public DateTime REG_DT
        {
            get
            {
                return _REG_DT;
            }
            set
            {
                if (_REG_DT != value)
                {
                    _REG_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _RSTR_ID;
        public string RSTR_ID
        {
            get
            {
                return _RSTR_ID;
            }
            set
            {
                if (_RSTR_ID != value)
                {
                    _RSTR_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        DateTime _UPD_DT;
        public DateTime UPD_DT
        {
            get
            {
                return _UPD_DT;
            }
            set
            {
                if (_UPD_DT != value)
                {
                    _UPD_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _UPDR_ID;
        public string UPDR_ID
        {
            get
            {
                return _UPDR_ID;
            }
            set
            {
                if (_UPDR_ID != value)
                {
                    _UPDR_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
