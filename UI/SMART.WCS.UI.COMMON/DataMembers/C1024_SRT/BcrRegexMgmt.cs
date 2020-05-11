using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.COMMON.DataMembers.C1024_SRT
{
    public class BcrRegexMgmt : PropertyNotifyExtensions
    {
        //INV_NO_REGEX
        //BOX_BCD_REGEX
        //RGN_BCD_REGEX
        //INV_NO_REGEX_BEF
        //BOX_BCD_REGEX_BEF
        //RGN_BCD_REGEX_BEF

        #region + INV_NO_REGEX - Invoice 바코드(적용중)
        private string _INV_NO_REGEX;
        /// <summary>
        /// Invoice 바코드(적용중)
        /// </summary>
        public string INV_NO_REGEX
        {
            get { return this._INV_NO_REGEX; }
            set
            {
                if (this._INV_NO_REGEX != value)
                {
                    this._INV_NO_REGEX = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_BCD_REGEX - 박스 바코드(적용중)
        private string _BOX_BCD_REGEX;
        /// <summary>
        /// 박스 바코드(적용중)
        /// </summary>
        public string BOX_BCD_REGEX
        {
            get { return this._BOX_BCD_REGEX; }
            set
            {
                if (this._BOX_BCD_REGEX != value)
                {
                    this._BOX_BCD_REGEX = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RGN_BCD_REGEX - Sorting 코드(적용중)
        private string _RGN_BCD_REGEX;
        /// <summary>
        /// Sorting 코드(적용중)
        /// </summary>
        public string RGN_BCD_REGEX
        {
            get { return this._RGN_BCD_REGEX; }
            set
            {
                if (this._RGN_BCD_REGEX != value)
                {
                    this._RGN_BCD_REGEX = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + INV_NO_REGEX_BEF - Invoice 바코드(이전)
        private string _INV_NO_REGEX_BEF;
        /// <summary>
        /// Invoice 바코드(이전)
        /// </summary>
        public string INV_NO_REGEX_BEF
        {
            get { return this._INV_NO_REGEX_BEF; }
            set
            {
                if (this._INV_NO_REGEX_BEF != value)
                {
                    this._INV_NO_REGEX_BEF = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_BCD_REGEX_BEF - 박스 바코드(이전)
        private string _BOX_BCD_REGEX_BEF;
        /// <summary>
        /// 박스 바코드(이전)
        /// </summary>
        public string BOX_BCD_REGEX_BEF
        {
            get { return this._BOX_BCD_REGEX_BEF; }
            set
            {
                if (this._BOX_BCD_REGEX_BEF != value)
                {
                    this._BOX_BCD_REGEX_BEF = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RGN_BCD_REGEX_BEF - Sorting 코드(이전)
        private string _RGN_BCD_REGEX_BEF;
        /// <summary>
        /// Sorting 코드(이전)
        /// </summary>
        public string RGN_BCD_REGEX_BEF
        {
            get { return this._RGN_BCD_REGEX_BEF; }
            set
            {
                if (this._RGN_BCD_REGEX_BEF != value)
                {
                    this._RGN_BCD_REGEX_BEF = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + UPD_DT - 최종 UPDATE 일자
        private string _UPD_DT;
        /// <summary>
        /// Sorting 코드(이전)
        /// </summary>
        public string UPD_DT
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
    }
}
