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

namespace SMART.WCS.UI.COMMON.DataMembers.C1006
{
    public class NoticeInfoMgnt : PropertyNotifyExtensions, IDXDataErrorInfo
    {
        BaseClass BaseClass = new BaseClass();
        private bool g_isValidation = false;

        #region *생성자
        //public NoticeInfoMgnt(int noti_seq, string noti_use_yn, int noti_prty, string noti_title, string noti_cont,
        //                      string noti_from_dt, string noti_to_dt, string upd_id, string upd_dt, int pre_noti_seq)
        //{
        //    this.NOTI_SEQ       = noti_seq;
        //    this.NOTI_USE_YN    = noti_use_yn;
        //    this.NOTI_PRTY      = noti_prty;
        //    this.NOTI_TITLE     = noti_title;
        //    this.NOTI_CONT      = noti_cont;
        //    this.NOTI_FROM_DT   = noti_from_dt;
        //    this.NOTI_TO_DT     = noti_to_dt;
        //    this.UPD_ID         = upd_id;
        //    this.UPD_DT         = upd_dt;
        //    this.PRE_NOTI_SEQ   = pre_noti_seq;
        //}
    
        #endregion

        #region * 그리드 색상 설정
        public NoticeInfoMgnt()
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

        #region + 필드 변경 여부
        private bool _IS_CHANGED;
        public bool IS_CHANGED
        {
            get { return this._IS_CHANGED; }
            set
            {
                if(this._IS_CHANGED != value)
                {
                    this._IS_CHANGED = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + NOTI_SEQ - 공지사항 순번
        private int _NOTI_SEQ;
        public int NOTI_SEQ
        {
            get { return this._NOTI_SEQ; }
            set
            {
                if (this._NOTI_SEQ != value)
                {
                    this._NOTI_SEQ = value;
                    if (this.IS_CHANGED == false) { this.IS_CHANGED = true; }
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + NOTI_USE_YN - 공지사항 사용여부
        private string _NOTI_USE_YN;
        public string NOTI_USE_YN
        {
            get { return this._NOTI_USE_YN; }
            set
            {
                if (this._NOTI_USE_YN != value)
                {
                    this._NOTI_USE_YN = value;
                    if (this.IS_CHANGED == false) { this.IS_CHANGED = true; }
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + NOTI_PRTY - 공지사항 우선순위
        private int _NOTI_PRTY;
        public int NOTI_PRTY
        {
            get { return this._NOTI_PRTY; }
            set
            {
                if (this._NOTI_PRTY != value)
                {
                    this._NOTI_PRTY = value;
                    if (this.IS_CHANGED == false) { this.IS_CHANGED = true; }
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + NOTI_TITLE - 공지사항 제목
        private string _NOTI_TITLE;
        public string NOTI_TITLE
        {
            get { return this._NOTI_TITLE; }
            set
            {
                if (this._NOTI_TITLE != value)
                {
                    this._NOTI_TITLE = value;
                    if (this.IS_CHANGED == false) { this.IS_CHANGED = true; }
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + NOTI_CONT - 공지사항 내용
        private string _NOTI_CONT;
        public string NOTI_CONT
        {
            get { return this._NOTI_CONT; }
            set
            {
                if (this._NOTI_CONT != value)
                {
                    this._NOTI_CONT = value;
                    if (this.IS_CHANGED == false) { this.IS_CHANGED = true; }
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + NOTI_FROM_DT - 공지사항 시작일
        private string _NOTI_FROM_DT;
        public string NOTI_FROM_DT
        {
            get { return this._NOTI_FROM_DT; }
            set
            {
                if (this._NOTI_FROM_DT != value)
                {
                    if (value.Length == 8)
                    {
                        this._NOTI_FROM_DT = this.BaseClass.ConvertStringToDate(value);
                    }
                    
                    if (this.IS_CHANGED == false) { this.IS_CHANGED = true; }
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + NOTI_FROM_DT - 공지사항 종료일
        private string _NOTI_TO_DT;
        public string NOTI_TO_DT
        {
            get { return this._NOTI_TO_DT; }
            set
            {
                if (this._NOTI_TO_DT != value)
                {
                    if (value.Length == 8)
                    {
                        this._NOTI_TO_DT = this.BaseClass.ConvertStringToDate(value);
                    }

                    if (this.IS_CHANGED == false) { this.IS_CHANGED = true; }
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + UPD_ID - 마지막 수정자
        private string _UPD_ID;
        public string UPD_ID
        {
            get { return this._UPD_ID; }
            set
            {
                if (this._UPD_ID != value)
                {
                    this._UPD_ID = value;
                    if (this.IS_CHANGED == false) { this.IS_CHANGED = true; }
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + UPD_DT - 수정날짜
        private DateTime _UPD_DT;
        public DateTime UPD_DT
        {
            get { return this._UPD_DT; }
            set
            {
                if (this._UPD_DT != value)
                {
                    this._UPD_DT = value;
                    if (this.IS_CHANGED == false) { this.IS_CHANGED = true; }
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PRE_NOTI_SEQ - 수정 전 공지사항 순번
        private int _PRE_NOTI_SEQ;
        public int PRE_NOTI_SEQ
        {
            get { return this._PRE_NOTI_SEQ; }
            set
            {
                if (this._PRE_NOTI_SEQ != value)
                {
                    this._PRE_NOTI_SEQ = value;
                    if (this.IS_CHANGED == false) { this.IS_CHANGED = true; }
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
