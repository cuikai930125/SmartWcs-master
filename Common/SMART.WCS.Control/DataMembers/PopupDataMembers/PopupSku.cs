using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.DataMembers
{
    public class PopupSku : PropertyNotifyExtensions
    {
        #region SKU 코드
        private string _SKU_CD;

        public string SKU_CD
        {
            get
            {
                return _SKU_CD;
            }
            set
            {
                if (_SKU_CD != value)
                {
                    _SKU_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region SKU 명
        private string _SKU_NM;

        public string SKU_NM
        {
            get
            {
                return _SKU_NM;
            }
            set
            {
                if (_SKU_NM != value)
                {
                    _SKU_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

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

        #region 고객사명
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

        #region 상품 바코드
        private string _SKU_BCR;

        public string SKU_BCR
        {
            get { return this._SKU_BCR; }
            set
            {
                if (this._SKU_BCR != value)
                {
                    this._SKU_BCR = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region BOX 바코드
        private string _BOX_BCR;

        public string BOX_BCR
        {
            get { return this._BOX_BCR; }
            set
            {
                if (this._BOX_BCR != value)
                {
                    this._BOX_BCR = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
