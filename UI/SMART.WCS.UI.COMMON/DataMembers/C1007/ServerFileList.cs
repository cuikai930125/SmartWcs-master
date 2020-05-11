using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.COMMON.DataMembers.C1007
{
    public class ServerFileList : PropertyNotifyExtensions
    {
        private string _FILE_NAME;
        public string FILE_NAME
        {
            get { return this._FILE_NAME; }
            set
            {
                if (this._FILE_NAME != value)
                {
                    this._FILE_NAME = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _FILE_EXTENSION;
        public string FILE_EXTENSION
        {
            get { return this._FILE_EXTENSION; }
            set
            {
                if (this._FILE_EXTENSION != value)
                {
                    this._FILE_EXTENSION = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region + 서버 파일 버전
        private string _SERVER_FILE_VERSION;
        /// <summary>
        /// 서버 파일 버전
        /// </summary>

        public string SERVER_FILE_VERSION
        {
            get { return this._SERVER_FILE_VERSION; }
            set
            {
                if (this._SERVER_FILE_VERSION != value)
                {
                    this._SERVER_FILE_VERSION = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion


        private string _FILE_DIRECTORY;
        public string FILE_DIRECTORY
        {
            get { return this._FILE_DIRECTORY; }
            set
            {
                if (this._FILE_DIRECTORY != value)
                {
                    this._FILE_DIRECTORY = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _RESOURCE_KIND;
        public string RESOURCE_KIND
        {
            get { return this._RESOURCE_KIND; }
            set
            {
                if (this._RESOURCE_KIND != value)
                {
                    this._RESOURCE_KIND = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
