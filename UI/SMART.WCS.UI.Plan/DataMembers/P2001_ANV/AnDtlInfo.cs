using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P2001_ANV
{
    public class AnDtlInfo : PropertyNotifyExtensions
    {
        public AnDtlInfo()
        {

        }

        #region + EXT_ID - 구분
        private string _EXT_ID;
        public string EXT_ID
        {
            get { return this._EXT_ID; }
            set
            {
                if (this._EXT_ID != value)
                {
                    this._EXT_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + COM_HDR_CD - 추출항목
        private string _COM_HDR_CD;
        public string COM_HDR_CD
        {
            get { return this._COM_HDR_CD; }
            set
            {
                if (this._COM_HDR_CD != value)
                {
                    this._COM_HDR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + COM_HDR_NM - 추출항목명
        private string _COM_HDR_NM;
        public string COM_HDR_NM
        {
            get { return this._COM_HDR_NM; }
            set
            {
                if (this._COM_HDR_NM != value)
                {
                    this._COM_HDR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
