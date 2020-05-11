using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1005
{
    public class MenuListByRole : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public MenuListByRole()
        {
            // this.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
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

        #region + MENU_ID - 메뉴ID
        private string _MENU_ID;
        public string MENU_ID
        {
            get { return this._MENU_ID; }
            set
            {
                if (this._MENU_ID != value)
                {
                    this._MENU_ID = value;
                    this.RaisePropertyChanged();
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
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + MENU_LVL - 메뉴 권한
        private string _MENU_LVL;
        public string MENU_LVL
        {
            get { return this._MENU_LVL; }
            set
            {
                if (this._MENU_LVL != value)
                {
                    this._MENU_LVL = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ROLE_MENU_CD - 권한 타입
        private string _ROLE_MENU_CD;
        public string ROLE_MENU_CD
        {
            get { return this._ROLE_MENU_CD; }
            set
            {
                if (this._ROLE_MENU_CD != value)
                {
                    this._ROLE_MENU_CD = value;
                    this.RaisePropertyChanged();
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
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PARENT_ID - 상위 메뉴 ID
        private string _PARENT_ID;
        public string PARENT_ID
        {
            get { return this._PARENT_ID; }
            set
            {
                if (this._PARENT_ID != value)
                {
                    this._PARENT_ID = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
