using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.COMMON.DataMembers.C1018
{
    public class UploadRslt : PropertyNotifyExtensions
    {
        #region + RGN_CD - 권역 코드
        private string _RGN_CD;
        public string RGN_CD
        {
            get { return this._RGN_CD; }
            set
            {
                if (this._RGN_CD != value)
                {
                    this._RGN_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RGN_NM - 권역 명
        private string _RGN_NM;
        public string RGN_NM
        {
            get { return this._RGN_NM; }
            set
            {
                if (this._RGN_NM != value)
                {
                    this._RGN_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 엑셀 업로드 결과 메세지 - RSLT_MSG
        private string _RSLT_MSG;
        public string RSLT_MSG
        {
            get { return this._RSLT_MSG; }
            set
            {
                if (this._RSLT_MSG != value)
                {
                    this._RSLT_MSG = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
