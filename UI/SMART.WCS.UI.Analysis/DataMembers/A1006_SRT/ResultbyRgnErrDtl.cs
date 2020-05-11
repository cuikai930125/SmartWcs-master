using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Analysis.DataMembers.A1006_SRT
{
    class ResultbyRgnErrDtl : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public ResultbyRgnErrDtl()
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
        //오류코드, 오류코드명, 설비오류코드,설비오류코드명,리젝구분,총수량
        //일자,분류코드,분류명,오류코드,오류명,설비오류코드,설비오류명,리젝구분,수량

        #region + ERR_CD - 오류코드
        private string _ERR_CD;
        public string ERR_CD
        {
            get { return this._ERR_CD; }
            set
            {
                if (this._ERR_CD != value)
                {
                    this._ERR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ERR_CD_NM - 오류코드명
        private string _ERR_CD_NM;
        public string ERR_CD_NM
        {
            get { return this._ERR_CD_NM; }
            set
            {
                if (this._ERR_CD_NM != value)
                {
                    this._ERR_CD_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_ERR_CD - 설비오류코드
        private string _EQP_ERR_CD;
        public string EQP_ERR_CD
        {
            get { return this._EQP_ERR_CD; }
            set
            {
                if (this._EQP_ERR_CD != value)
                {
                    this._EQP_ERR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_ERR_CD_NM - 설비오류코드명
        private string _EQP_ERR_CD_NM;
        public string EQP_ERR_CD_NM
        {
            get { return this._EQP_ERR_CD_NM; }
            set
            {
                if (this._EQP_ERR_CD_NM != value)
                {
                    this._EQP_ERR_CD_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RJT_TYPE - 리젝구분
        private string _RJT_TYPE;
        public string RJT_TYPE
        {
            get { return this._RJT_TYPE; }
            set
            {
                if (this._RJT_TYPE != value)
                {
                    this._RJT_TYPE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TOTAL_QTY - 총 수량
        private string _TOTAL_QTY;
        public string TOTAL_QTY
        {
            get { return this._TOTAL_QTY; }
            set
            {
                if (this._TOTAL_QTY != value)
                {
                    this._TOTAL_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + AGG_YMD - 일자
        private string _AGG_YMD;
        public string AGG_YMD
        {
            get { return this._AGG_YMD; }
            set
            {
                if (this._AGG_YMD != value)
                {
                    this._AGG_YMD  = this.BaseClass.ConvertStringToDate(value);

                    //this._AGG_YMD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RGN_BCD - 분류코드
        private string _RGN_BCD;
        public string RGN_BCD
        {
            get { return this._RGN_BCD; }
            set
            {
                if (this._RGN_BCD != value)
                {
                    this._RGN_BCD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RGN_NM - 분류명
        private string _RGN_NM;
        public string RGN_NM
        {
            get { return this._RGN_NM; }
            set
            {
                if (this._RGN_NM != value)
                {
                    this._RGN_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

    }
}
