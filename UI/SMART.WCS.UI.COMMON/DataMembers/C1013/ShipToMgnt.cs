﻿using DevExpress.XtraEditors.DXErrorProvider;
using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1013
{
    public class ShipToMgnt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region * 그리드 색상 설정
        public ShipToMgnt()
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

        #region + CST_CD - 고객사 코드
        private string _CST_CD;
        public string CST_CD
        {
            get { return this._CST_CD; }
            set
            {
                if (this._CST_CD != value)
                {
                    this._CST_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SHIP_TO_CD - 출고처코드
        private string _SHIP_TO_CD;
        public string SHIP_TO_CD
        {
            get { return this._SHIP_TO_CD; }
            set
            {
                if (this._SHIP_TO_CD != value)
                {
                    this._SHIP_TO_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SHIP_TO_NM - 출고처 명
        private string _SHIP_TO_NM;
        public string SHIP_TO_NM
        {
            get { return this._SHIP_TO_NM; }
            set
            {
                if (this._SHIP_TO_NM != value)
                {
                    this._SHIP_TO_NM = value;
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
                    this._ADDR = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ZIP_CD - 우편코드
        private string _ZIP_CD;
        public string ZIP_CD
        {
            get { return this._ZIP_CD; }
            set
            {
                if (this._ZIP_CD != value)
                {
                    this._ZIP_CD = value;
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

        #region + RGN_CD - 지역코드
        private string _RGN_CD;
        public string RGN_CD
        {
            get { return this._RGN_CD; }
            set
            {
                if (this._RGN_CD != value)
                {
                    this._RGN_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RGN_NM - 지역명
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

        #region + CRT_SPR_CD - 시스템 코드
        private string _CRT_SPR_CD;
        public string CRT_SPR_CD
        {
            get { return this._CRT_SPR_CD; }
            set
            {
                if (this._CRT_SPR_CD != value)
                {
                    this._CRT_SPR_CD = value;
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

        #region + IsWMS - true : 컬럼수정불가
        private bool _IsWMS;
        public bool IsWMS
        {
            get { return this._IsWMS; }
            set
            {
                if (this._IsWMS != value)
                {
                    this._IsWMS = value;
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
