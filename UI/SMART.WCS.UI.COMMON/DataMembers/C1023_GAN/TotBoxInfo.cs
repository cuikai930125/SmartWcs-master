using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.COMMON.DataMembers.C1023_GAN
{
    public class TotBoxInfo : PropertyNotifyExtensions
    {
        //BOX_TYPE_CD
        //BOX_TYPE_NM
        //BOX_WTH_LEN
        //BOX_VERT_LEN
        //BOX_HGT_LEN

        #region + BOX_TYPE_CD - 토트박스 타입 코드
        private string _BOX_TYPE_CD;
        /// <summary>
        /// 회사코드
        /// </summary>
        public string BOX_TYPE_CD
        {
            get { return this._BOX_TYPE_CD; }
            set
            {
                if (this._BOX_TYPE_CD != value)
                {
                    this._BOX_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_TYPE_NM - 토트박스 타입
        private string _BOX_TYPE_NM;
        /// <summary>
        /// 회사코드
        /// </summary>
        public string BOX_TYPE_NM
        {
            get { return this._BOX_TYPE_NM; }
            set
            {
                if (this._BOX_TYPE_NM != value)
                {
                    this._BOX_TYPE_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_WTH_LEN - 토트박스 길이
        private string _BOX_WTH_LEN;
        /// <summary>
        /// 회사코드
        /// </summary>
        public string BOX_WTH_LEN
        {
            get { return this._BOX_WTH_LEN; }
            set
            {
                if (this._BOX_WTH_LEN != value)
                {
                    this._BOX_WTH_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_VERT_LEN - 토트박스 너비
        private string _BOX_VERT_LEN;
        /// <summary>
        /// 회사코드
        /// </summary>
        public string BOX_VERT_LEN
        {
            get { return this._BOX_VERT_LEN; }
            set
            {
                if (this._BOX_VERT_LEN != value)
                {
                    this._BOX_VERT_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BOX_HGT_LEN - 토트박스 높이
        private string _BOX_HGT_LEN;
        /// <summary>
        /// 회사코드
        /// </summary>
        public string BOX_HGT_LEN
        {
            get { return this._BOX_HGT_LEN; }
            set
            {
                if (this._BOX_HGT_LEN != value)
                {
                    this._BOX_HGT_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

    }
}
