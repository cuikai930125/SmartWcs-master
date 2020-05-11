using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Analysis.DataMembers.A1001_SRT
{
    public class ResultbytimeInfo : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public ResultbytimeInfo()
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

        #region + P_CO_CD - 회사 코드
        private string _P_CO_CD;
        public string P_CO_CD
        {
            get { return this._P_CO_CD; }
            set
            {
                if (this._P_CO_CD != value)
                {
                    this._P_CO_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + P_CNTR_CD - 센터 코드
        private string _P_CNTR_CD;
        public string P_CNTR_CD
        {
            get { return this._P_CNTR_CD; }
            set
            {
                if (this._P_CNTR_CD != value)
                {
                    this._P_CNTR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_ID - 설비 ID
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

        #region + SUM_YMD - 일자 SUMMARY
        private string _SUM_YMD;
        public string SUM_YMD
        {
            get { return this._SUM_YMD; }
            set
            {
                if (this._SUM_YMD != value)
                {
                    this._SUM_YMD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DISP_AGG_HM -시간대 SUMMARY
        private string _DISP_AGG_HM;
        public string DISP_AGG_HM
        {
            get { return this._DISP_AGG_HM; }
            set
            {
                if (this._DISP_AGG_HM != value)
                {
                    this._DISP_AGG_HM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + NML_SRT_QTY - 정상분류 개수
        private string _NML_SRT_QTY;
        public string NML_SRT_QTY
        {
            get { return this._NML_SRT_QTY; }
            set
            {
                if (this._NML_SRT_QTY != value)
                {
                    this._NML_SRT_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ERR_OVF_QTY - 만재 오류 개수
        private string _ERR_OVF_QTY;
        public string ERR_OVF_QTY
        {
            get { return this._ERR_OVF_QTY; }
            set
            {
                if (this._ERR_OVF_QTY != value)
                {
                    this._ERR_OVF_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ERR_NR_QTY - NO READ 오류 개수
        private string _ERR_NR_QTY;
        public string ERR_NR_QTY
        {
            get { return this._ERR_NR_QTY; }
            set
            {
                if (this._ERR_NR_QTY != value)
                {
                    this._ERR_NR_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ERR_RGN_QTY - 권역 코드 오류 개수
        private string _ERR_RGN_QTY;
        public string ERR_RGN_QTY
        {
            get { return this._ERR_RGN_QTY; }
            set
            {
                if (this._ERR_RGN_QTY != value)
                {
                    this._ERR_RGN_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ETC_ERR_QTY - ETC 오류 개수
        private string _ETC_ERR_QTY;
        public string ETC_ERR_QTY
        {
            get { return this._ETC_ERR_QTY; }
            set
            {
                if (this._ETC_ERR_QTY != value)
                {
                    this._ETC_ERR_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TOTAL_ERR_QTY - 오류 총합
        private decimal _TOTAL_ERR_QTY;
        public decimal TOTAL_ERR_QTY
        {
            get { return this._TOTAL_ERR_QTY; }
            set
            {
                if (this._TOTAL_ERR_QTY != value)
                {
                    this._TOTAL_ERR_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TOTAL_ALL - 정상+오류 총합
        private string _TOTAL_ALL;
        public string TOTAL_ALL
        {
            get { return this._TOTAL_ALL; }
            set
            {
                if (this._TOTAL_ALL != value)
                {
                    this._TOTAL_ALL = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + NML_SRT_RT - 정상분류율
        private string _NML_SRT_RT;
        public string NML_SRT_RT
        {
            get { return this._NML_SRT_RT; }
            set
            {
                if (this._NML_SRT_RT != value)
                {
                    this._NML_SRT_RT = value;
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
