using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.DataMembers
{
    public class ContentLabelInfo : PropertyNotifyExtensions
    {
         string _MENU_ID;
        public string MENU_ID
        {
            get{
                        return _MENU_ID;
            }set{
                if (_MENU_ID != value)
                {
                    _MENU_ID = value;
                    RaisePropertyChanged();
                }
           }
        }

         string _LABEL_TYPE_CD;
        public string LABEL_TYPE_CD
        {
            get{
                return _LABEL_TYPE_CD;
            }set{
                if (_LABEL_TYPE_CD != value)
                {
                    _LABEL_TYPE_CD = value;
                    RaisePropertyChanged();
                }
           }
        }

         string _LAN_CD;
        public string LAN_CD
        {
            get{
                        return _LAN_CD;
            }set{
                if (_LAN_CD != value)
                {
                    _LAN_CD = value;
                    RaisePropertyChanged();
                }
           }
        }

         string _LAN_DESC;
        public string LAN_DESC
        {
            get{
                        return _LAN_DESC;
            }set{
                if (_LAN_DESC != value)
                {
                    _LAN_DESC = value;
                    RaisePropertyChanged();
                }
           }
        }

        //20180913 hj.kim 필수판단
       
        string _REQD_YN;
        public string REQD_YN
        {
            get
            {
                return _REQD_YN;
            }
            set
            {
                if (_REQD_YN != value)
                {
                    _REQD_YN = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
