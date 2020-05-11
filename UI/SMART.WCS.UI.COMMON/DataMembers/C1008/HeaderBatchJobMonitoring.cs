using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1008
{
    class HeaderBatchJobMonitoring : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public HeaderBatchJobMonitoring()
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

        #region + JOB_NO - JOB 번호
        private string _JOB_NO;
        public string JOB_NO
        {
            get { return this._JOB_NO; }
            set
            {
                if (this._JOB_NO != value)
                {
                    this._JOB_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + JOB_YMD - 작업일자
        private string _JOB_YMD;
        public string JOB_YMD
        {
            get { return this._JOB_YMD; }
            set
            {
                if (this._JOB_YMD != value)
                {
                    this._JOB_YMD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + JOB_ID - JOB ID
        private string _JOB_ID;
        public string JOB_ID
        {
            get { return this._JOB_ID; }
            set
            {
                if (this._JOB_ID != value)
                {
                    this._JOB_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + JOB_NM - JOB 명
        private string _JOB_NM;
        public string JOB_NM
        {
            get { return this._JOB_NM; }
            set
            {
                if (this._JOB_NM != value)
                {
                    this._JOB_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + STRT_DT - 시작일시
        private string _STRT_DT;
        public string STRT_DT
        {
            get { return this._STRT_DT; }
            set
            {
                if (this._STRT_DT != value)
                {
                    this._STRT_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + END_DT - 종료일시
        private string _END_DT;
        public string END_DT
        {
            get { return this._END_DT; }
            set
            {
                if (this._END_DT != value)
                {
                    this._END_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TASK_CNT - 대상건수
        private string _TASK_CNT;
        public string TASK_CNT
        {
            get { return this._TASK_CNT; }
            set
            {
                if (this._TASK_CNT != value)
                {
                    this._TASK_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PROC_CNT - 성공건수
        private string _PROC_CNT;
        public string PROC_CNT
        {
            get { return this._PROC_CNT; }
            set
            {
                if (this._PROC_CNT != value)
                {
                    this._PROC_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + FAIL_CNT - 실패건수
        private string _FAIL_CNT;
        public string FAIL_CNT
        {
            get { return this._FAIL_CNT; }
            set
            {
                if (this._FAIL_CNT != value)
                {
                    this._FAIL_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PROC_CD - 처리결과
        private string _PROC_CD;
        public string PROC_CD
        {
            get { return this._PROC_CD; }
            set
            {
                if (this._PROC_CD != value)
                {
                    this._PROC_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PARAM1 - Parameter1
        private string _PARAM1;
        public string PARAM1
        {
            get { return this._PARAM1; }
            set
            {
                if (this._PARAM1 != value)
                {
                    this._PARAM1 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PARAM2 - Parameter2
        private string _PARAM2;
        public string PARAM2
        {
            get { return this._PARAM2; }
            set
            {
                if (this._PARAM2 != value)
                {
                    this._PARAM2 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
        
    }
}
