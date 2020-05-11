using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P2001_ANV_01P
{
    public class AnAggTypeInfo : PropertyNotifyExtensions
    {
        public AnAggTypeInfo()
        {

        }

        #region + COM_HDR_CD - 항목코드
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

        #region + COM_HDR_NM - 항목명
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
