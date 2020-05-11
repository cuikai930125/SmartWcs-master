using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1027
{
    public class BackMstMgnt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public BackMstMgnt()
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

        #region + OWNER - 타이틀 OWNER (*)
        private string _OWNER;
        public string OWNER
        {
            get { return this._OWNER; }
            set
            {
                if (this._OWNER != value)
                {
                    this._OWNER = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TB_NM -  테이블 (*)
        private string _TB_NM;
        public string TB_NM
        {
            get { return this._TB_NM; }
            set
            {
                if (this._TB_NM != value)
                {
                    this._TB_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BK_YN - 백업여부
        private string _BK_YN;
        public string BK_YN
        {
            get { return this._BK_YN; }
            set
            {
                if (this._BK_YN != value)
                {
                    this._BK_YN = value;
                    this.BK_YN_Checked = this._BK_YN.Equals("Y") ? true : false;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BK_YN_Checked - 백업여부 변환값
        private bool _BK_YN_Checked;
        public bool BK_YN_Checked
        {
            get { return this._BK_YN_Checked; }
            set
            {
                if (this._BK_YN_Checked != value)
                {
                    this._BK_YN_Checked = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DEL_YN - 삭제여부
        private string _DEL_YN;
        public string DEL_YN
        {
            get { return this._DEL_YN; }
            set
            {
                if (this._DEL_YN != value)
                {
                    this._DEL_YN = value;
                    this.DEL_YN_Checked = this._DEL_YN_Checked.Equals("Y") ? true : false;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DEL_YN_Checked - 삭제여부 변환값
        private bool _DEL_YN_Checked;
        public bool DEL_YN_Checked
        {
            get { return this._DEL_YN_Checked; }
            set
            {
                if (this._DEL_YN_Checked != value)
                {
                    this._DEL_YN_Checked = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BK_COND - 백업조건
        private string _BK_COND;
        public string BK_COND
        {
            get { return this._BK_COND; }
            set
            {
                if (this._BK_COND != value)
                {
                    this._BK_COND = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BK_OWNER - 백업테이블 OWNER (*)
        private string _BK_OWNER;
        public string BK_OWNER
        {
            get { return this._BK_OWNER; }
            set
            {
                if (this._BK_OWNER != value)
                {
                    this._BK_OWNER = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BK_TB_NM - 백업테이블 (*)
        private string _BK_TB_NM;
        public string BK_TB_NM
        {
            get { return this._BK_TB_NM; }
            set
            {
                if (this._BK_TB_NM != value)
                {
                    this._BK_TB_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BK_SEQ - 백업순서
        private int _BK_SEQ;
        public int BK_SEQ
        {
            get { return this._BK_SEQ; }
            set
            {
                if (this._BK_SEQ != value)
                {
                    this._BK_SEQ = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + REBUILD_YN - 백업후 리빌드 여부
        private string _REBUILD_YN;
        public string REBUILD_YN
        {
            get { return this._REBUILD_YN; }
            set
            {
                if (this._REBUILD_YN != value)
                {
                    this._REBUILD_YN = value;
                    this.REBUILD_YN_Checked = this._REBUILD_YN.Equals("Y") ? true : false;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + REBUILD_YN_Checked - 백업후 리빌드 여부 변환값
        private bool _REBUILD_YN_Checked;
        public bool REBUILD_YN_Checked
        {
            get { return this._REBUILD_YN_Checked; }
            set
            {
                if (this._REBUILD_YN_Checked != value)
                {
                    this._REBUILD_YN_Checked = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + USE_YN - 사용여부
        private string _USE_YN;
        public string USE_YN
        {
            get { return this._USE_YN; }
            set
            {
                if (this._USE_YN != value)
                {
                    this._USE_YN = value;
                    this.USE_YN_Checked = this._USE_YN.Equals("Y") ? true : false;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + USE_YN_Checked - 사용여부 변환값
        private bool _USE_YN_Checked;
        public bool USE_YN_Checked
        {
            get { return this._USE_YN_Checked; }
            set
            {
                if (this._USE_YN_Checked != value)
                {
                    this._USE_YN_Checked = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
