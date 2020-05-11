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
    public class DeployServerFileList : PropertyNotifyExtensions
    {
        BaseClass BaseClass = new BaseClass();

        public Brush BackgroundBrush { get; set; }

        #region + _SERVER_FILE
        private string _SERVER_FILE;

        public string SERVER_FILE
        {
            get { return this._SERVER_FILE; }
            set
            {
                if (this._SERVER_FILE != value)
                {
                    this._SERVER_FILE = value;
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
    }
}
