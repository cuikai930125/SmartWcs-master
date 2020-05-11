using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.COMMON.DataMembers.C1006
{
    public class NoticeDetail : PropertyNotifyExtensions
    {
        #region >> 제목 - NOTI_TITLE
        #region + NOTI_TITLE_DTL - 공지사항 제목
        private string _NOTI_TITLE_DTL;
        public string NOTI_TITLE_DTL
        {
            get { return this._NOTI_TITLE_DTL; }
            set
            {
                if (this._NOTI_TITLE_DTL != value)
                {
                    this._NOTI_TITLE_DTL = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
        #endregion
    }
}
