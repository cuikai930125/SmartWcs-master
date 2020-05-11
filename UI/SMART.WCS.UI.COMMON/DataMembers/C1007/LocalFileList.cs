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
    public class LocalFileList : PropertyNotifyExtensions
    {
        BaseClass BaseClass = new BaseClass();

        #region * 그리드 색상 설정
        public LocalFileList()
        {
            //this.BackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#F9F9F9");
            this.ForegroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#000000");

            this.WeightBold = System.Windows.FontWeights.Normal;


        }

        public Brush BaseBackgroundBrush { get; set; }

        public Brush BackgroundBrush { get; set; }

        public Brush ForegroundBrush { get; set; }

        public FontWeight WeightBold { get; set; }
        #endregion

        #region + 파일명
        private string _FILE_NAME;
        /// <summary>
        /// 파일명
        /// </summary>
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
        #endregion

        #region + 파일 확장자
        private string _FILE_EXTENSION;
        /// <summary>
        /// 파일 확장자
        /// </summary>
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
        #endregion

        #region + 폴더명
        private string _LOCAL_DIRECTORY;
        /// <summary>
        /// 폴더명
        /// </summary>
        public string LOCAL_DIRECTORY
        {
            get { return this._LOCAL_DIRECTORY; }
            set
            {
                if (this._LOCAL_DIRECTORY != value)
                {
                    this._LOCAL_DIRECTORY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 서버 경로
        private string _SERVER_DIRECTORY;
        /// <summary>
        /// 서버 경로
        /// </summary>
        public string SERVER_DIRECTORY
        {
            get { return this._SERVER_DIRECTORY; }
            set
            {
                if (this._SERVER_DIRECTORY != value)
                {
                    this._SERVER_DIRECTORY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 로컬 파일 버전
        private string _LOCAL_FILE_VERSION;
        /// <summary>
        /// 로컬 파일 버전
        /// </summary>
        public string LOCAL_FILE_VERSION
        {
            get { return this._LOCAL_FILE_VERSION; }
            set
            {
                if (this._LOCAL_FILE_VERSION != value)
                {
                    this._LOCAL_FILE_VERSION = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

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

        private string _COMPARE_RESULT_CD;
        public string COMPARE_RESULT_CD
        {
            get { return this._COMPARE_RESULT_CD; }
            set
            {
                if (this._COMPARE_RESULT_CD != value)
                {
                    this._COMPARE_RESULT_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _COMPARE_RESULT;
        public string COMPARE_RESULT
        {
            get { return this._COMPARE_RESULT; }
            set
            {
                if (this._COMPARE_RESULT != value)
                {
                    this._COMPARE_RESULT = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
