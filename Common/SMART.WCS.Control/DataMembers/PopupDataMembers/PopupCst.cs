using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.DataMembers
{
    public class PopupCst : PropertyNotifyExtensions
    {
        #region 고객사 코드
        private string _CST_CD;

        public string CST_CD
        {
            get
            {
                return _CST_CD;
            }
            set
            {
                if (_CST_CD != value)
                {
                    _CST_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region 고객사 명
        private string _CST_NM;

        public string CST_NM
        {
            get
            {
                return _CST_NM;
            }
            set
            {
                if (_CST_NM != value)
                {
                    _CST_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region 전화번호
        private string _TEL_NO;

        public string TEL_NO
        {
            get
            {
                return _TEL_NO;
            }
            set
            {
                if (_TEL_NO != value)
                {
                    _TEL_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region 주소
        private string _CST_ADDR1;

        public string CST_ADDR1
        {
            get
            {
                return _CST_ADDR1;
            }
            set
            {
                if (_CST_ADDR1 != value)
                {
                    _CST_ADDR1 = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
