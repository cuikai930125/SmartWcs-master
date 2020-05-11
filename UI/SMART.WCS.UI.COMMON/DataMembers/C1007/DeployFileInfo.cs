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
    public class DeployFileInfo : PropertyNotifyExtensions
    {
        BaseClass BaseClass = new BaseClass();

        #region * 그리드 색상 설정
        public DeployFileInfo()
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

        private string _FILE_LAST_MODIFY_DT;
        public string FILE_LAST_MODIFY_DT
        {
            get { return this._FILE_LAST_MODIFY_DT; }
            set
            {
                if (this._FILE_LAST_MODIFY_DT != value)
                {
                    this._FILE_LAST_MODIFY_DT = value;
                    RaisePropertyChanged();
                }
            }
        }

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
