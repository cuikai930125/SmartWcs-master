using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.DataMembers
{
    public class PopupShip : PropertyNotifyExtensions
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
                return this._CST_NM;
            }
            set
            {
                if (this._CST_NM != value)
                {
                    this._CST_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region 거래처코드
        private string _SHIP_CD;

        public string SHIP_CD
        {
            get { return this._SHIP_CD; }
            set
            {
                if (this._SHIP_CD != value)
                {
                    this._SHIP_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region 거래처 명
        private string _SHIP_NM;

        public string SHIP_NM
        {
            get { return this._SHIP_NM; }
            set
            {
                if (this._SHIP_NM != value)
                {
                    this._SHIP_NM = value;
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
