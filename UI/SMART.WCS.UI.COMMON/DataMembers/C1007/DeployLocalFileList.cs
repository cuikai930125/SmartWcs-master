using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SMART.WCS.UI.COMMON.DataMembers.C1007
{
    public class DeployLocalFileList : PropertyNotifyExtensions
    {
        BaseClass BaseClass = new BaseClass();

        public DeployLocalFileList()
        {
            this.ForegroundBrush    = new SolidColorBrush(Colors.Black);
            this.BackgroundBrush    = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
        }

        private Brush _BackgroundBrush;
        public Brush BackgroundBrush
        {
            get { return this._BackgroundBrush; }
            set
            {
                if (this._BackgroundBrush != value)
                {
                    this._BackgroundBrush = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Brush _ForegroundBrush;
        public Brush ForegroundBrush
        {
            get { return this._ForegroundBrush; }
            set
            {
                if (this._ForegroundBrush != value)
                {
                    this._ForegroundBrush = value;
                    RaisePropertyChanged();
                }
            }
        }

        private FontWeight _FONT_BOLD;
        public FontWeight FONT_BOLD
        {
            get { return this._FONT_BOLD; }
            set
            {
                if (this._FONT_BOLD != value)
                {
                    this._FONT_BOLD = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region + _LOCAL_FILE
        private string _LOCAL_FILE;

        public string LOCAL_FILE
        {
            get { return this._LOCAL_FILE; }
            set
            {
                if (this._LOCAL_FILE != value)
                {
                    this._LOCAL_FILE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + _UPD_DT
        private DateTime _UPD_DT;

        public DateTime UPD_DT
        {
            get { return this._UPD_DT; }
            set
            {
                if (this._UPD_DT != value)
                {
                    this._UPD_DT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + _APP_DIR
        private string _APP_DIR;

        public string APP_DIR
        {
            get { return this._APP_DIR; }
            set
            {
                if (this._APP_DIR != value)
                {
                    this._APP_DIR = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region DIFF_FLAG - 서버와 로컬 파일 수정일자가 다른경우 "D"로 표기
        #region + _DIFF_FLAG
        private string _DIFF_FLAG;

        public string DIFF_FLAG
        {
            get { return this._DIFF_FLAG; }
            set
            {
                if (this._DIFF_FLAG != value)
                {
                    this._DIFF_FLAG = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
        #endregion
    }
}
