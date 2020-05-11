using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Processing.DataMembers.R1003_SRT
{
    class ChuteResultDetail : PropertyNotifyExtensions
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public ChuteResultDetail()
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

        #region + SCAN_DT - 일자
        private string _SCAN_DT;
        public string SCAN_DT
        {
            get { return this._SCAN_DT; }
            set
            {
                if (this._SCAN_DT != value)
                {
                    this._SCAN_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_BCD - 박스 바코드
        private string _BOX_BCD;
        public string BOX_BCD
        {
            get { return this._BOX_BCD; }
            set
            {
                if (this._BOX_BCD != value)
                {
                    this._BOX_BCD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + INV_BCD - 송장 바코드
        private string _INV_BCD;
        public string INV_BCD
        {
            get { return this._INV_BCD; }
            set
            {
                if (this._INV_BCD != value)
                {
                    this._INV_BCD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PCL_TYPE_CD - 화물유형 코드
        private string _PCL_TYPE_CD;
        public string PCL_TYPE_CD
        {
            get { return this._PCL_TYPE_CD; }
            set
            {
                if (this._PCL_TYPE_CD != value)
                {
                    this._PCL_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CART_NO - 카트 번호
        private string _CART_NO;
        public string CART_NO
        {
            get { return this._CART_NO; }
            set
            {
                if (this._CART_NO != value)
                {
                    this._CART_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RECIRC_CNT - 회전 수
        private string _RECIRC_CNT;
        public string RECIRC_CNT
        {
            get { return this._RECIRC_CNT; }
            set
            {
                if (this._RECIRC_CNT != value)
                {
                    this._RECIRC_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SRT_WRK_CMPT_DT - 분류완료 일시
        private string _SRT_WRK_CMPT_DT;
        public string SRT_WRK_CMPT_DT
        {
            get { return this._SRT_WRK_CMPT_DT; }
            set
            {
                if (this._SRT_WRK_CMPT_DT != value)
                {
                    this._SRT_WRK_CMPT_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CART_CNT - 카트 수량
        private string _CART_CNT;
        public string CART_CNT
        {
            get { return this._CART_CNT; }
            set
            {
                if (this._CART_CNT != value)
                {
                    this._CART_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PROC_MSG - 처리 메시지
        private string _PROC_MSG;
        public string PROC_MSG
        {
            get { return this._PROC_MSG; }
            set
            {
                if (this._PROC_MSG != value)
                {
                    this._PROC_MSG = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
