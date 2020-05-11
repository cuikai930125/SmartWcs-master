using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common.Extensions
{
    public class PropertyNotifyExtensions : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this._IsFlagReset == true)
            {
                this._IsFlagReset = false;
                this._IsUpdate = false;
                this._IsDelete = false;
                this._IsNew = false;
                this._IsSelected = false;
                RaisePropertyChanged("IsSelected");
            }
            else
            {
                if (PropertyChanged != null)
                {
                    if (IsDelete == true)
                    {
                        this.IsSelected = false;
                    }
                    else if (propertyName != "IsChecked" && propertyName != "IsSelected" && propertyName != "IsHide" && propertyName != "SelectReadOnly" && propertyName != "Error" && propertyName != "ErrorText" && propertyName != "ErrorProperty")
                    {
                        this.IsSelected = true;
                        this.IsUpdate = true;
                    }

                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        #region ▩ 속성
        #region > Error 
        private string _Error;
        public string Error
        {
            get { return this._Error; }
            set
            {
                this._Error = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region > IsFlagReset
        private bool _IsFlagReset;
        public void CRUDReset()
        {
            this._IsFlagReset = true;
            RaisePropertyChanged();
        }
        #endregion

        #region > IsUpdateUsed
        private bool _IsUpdateUsed;
        public bool IsUpdateUsed
        {
            get { return this._IsUpdateUsed; }
            set
            {
                this._IsUpdateUsed = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region > IsUpdate
        private bool _IsUpdate;

        /// <summary>
        /// 변경된 데이터
        /// </summary>
        public bool IsUpdate
        {
            get { return this._IsUpdate; }
            set
            {
                if (this._IsUpdate != value)
                {
                    this._IsUpdate = value;

                    if (this.IsUpdateUsed == false)
                    {
                        if (value == true)
                        {
                            this.IsSelected = true;
                        }
                        RaisePropertyChanged();
                    }
                }
            }
        }
        #endregion

        #region > IsSelected
        private bool _IsSelected;

        /// <summary>
        /// 선택
        /// </summary>
        public bool IsSelected
        {
            get { return this._IsSelected; }
            set
            {
                if (this._IsSelected != value)
                {
                    this._IsSelected = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region > IsChecked
        private bool _IsChecked;

        /// <summary>
        /// CheckBox Check
        /// </summary>
        public virtual bool IsChecked
        {
            get { return this._IsChecked; }
            set
            {
                if (this._IsChecked != value)
                {
                    this._IsChecked = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region > SelectReadOnly
        private bool _SelectReadOnly;

        /// <summary>
        /// 선택
        /// </summary>
        public bool SelectReadOnly
        {
            get { return this._SelectReadOnly; }
            set
            {
                if (this._SelectReadOnly != value)
                {
                    this._SelectReadOnly = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region > IsNew
        private bool _IsNew;
        /// <summary>
        /// 신규데이터
        /// </summary>
        public virtual bool IsNew
        {
            get { return this._IsNew; }
            set
            {
                this._IsNew = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region > IsDelete
        private bool _IsDelete;
        /// <summary>
        /// 삭제 아이템
        /// </summary>
        public bool IsDelete
        {
            get { return this._IsDelete; }
            set
            {
                this._IsDelete = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region > IsHide
        private bool _IsHide;

        /// <summary>
        /// datacontrol에서 숨기는 기능을 한다.
        /// True이면 숨김
        /// </summary>
        public bool IsHide
        {
            get { return _IsHide; }
            set
            {
                _IsHide = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        #endregion
    }
}
