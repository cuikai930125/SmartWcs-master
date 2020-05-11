using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1004
{
    public class MenuMgmt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        public Brush BaseBackgroundBrush { get; set; }

        public Brush BackgroundBrush { get; set; }

        #region * Validation - 그리드 데이터 유효성 체크
        public MenuMgmt()
        {
            this.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
        }

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

        #region + MENU_ID - 메뉴 ID
        private string _MENU_ID;

        public string MENU_ID
        {
            get { return this._MENU_ID; }
            set
            {
                if (this._MENU_ID != value)
                {
                    this._MENU_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MENU_NM - 메뉴명
        private string _MENU_NM;

        public string MENU_NM
        {
            get { return this._MENU_NM; }
            set
            {
                if (this._MENU_NM != value)
                {
                    this._MENU_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MENU_DESC - 메뉴 상세
        private string _MENU_DESC;

        public string MENU_DESC
        {
            get { return this._MENU_DESC; }
            set
            {
                if (this._MENU_DESC != value)
                {
                    this._MENU_DESC = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MENU_LVL - 메뉴 레벨
        private int _MENU_LVL;

        public int MENU_LVL
        {
            get { return this._MENU_LVL; }
            set
            {
                if (this._MENU_LVL != value)
                {
                    this._MENU_LVL = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MENU_TYPE - 메뉴 타입
        private string _MENU_TYPE;

        public string MENU_TYPE
        {
            get { return this._MENU_TYPE; }
            set
            {
                if (this._MENU_TYPE != value)
                {
                    this._MENU_TYPE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MENU_URL - 메뉴 URL
        private string _MENU_URL;

        public string MENU_URL
        {
            get { return this._MENU_URL; }
            set
            {
                if (this._MENU_URL != value)
                {
                    this._MENU_URL = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MENU_ICON - 메뉴 아이콘
        private string _MENU_ICON;

        public string MENU_ICON
        {
            get { return this._MENU_ICON; }
            set
            {
                if (this._MENU_ICON != value)
                {
                    this._MENU_ICON = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TREE_ID - 트리 ID
        private string _TREE_ID;

        public string TREE_ID
        {
            get { return this._TREE_ID; }
            set
            {
                if (this._TREE_ID != value)
                {
                    this._TREE_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PARENT_ID - 부모 ID
        private string _PARENT_ID;

        public string PARENT_ID
        {
            get { return this._PARENT_ID; }
            set
            {
                if (this._PARENT_ID != value)
                {
                    this._PARENT_ID = value;
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
                    this.USE_YN_CHECKED = this.USE_YN.Equals("Y") == true ? true : false;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + USE_YN_CHECKED - 정렬 순서
        private bool _USE_YN_CHECKED;

        public bool USE_YN_CHECKED
        {
            get { return this._USE_YN_CHECKED; }
            set
            {
                if (this._USE_YN_CHECKED != value)
                {
                    this._USE_YN_CHECKED = value;
                    this._USE_YN_CHECKED = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SORT_SEQ - 정렬 순서
        private int _SORT_SEQ;

        public int SORT_SEQ
        {
            get { return this._SORT_SEQ; }
            set
            {
                if (this._SORT_SEQ != value)
                {
                    this._SORT_SEQ = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
