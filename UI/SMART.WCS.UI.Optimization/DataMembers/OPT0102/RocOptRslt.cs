using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SMART.WCS.UI.Optimization.DataMembers.OPT0102
{
    public class RocOptRslt : PropertyNotifyExtensions
    {
        BaseClass BaseClass = new BaseClass();

        public RocOptRslt()
        {
            this.TitleBackgroundBrush = this.BaseClass.ConvertStringToSolidColorBrush("#E3E6EB");
        }

        public Brush TitleBackgroundBrush { get; set; }

        #region + COL_HEAD - 컬럼 헤더
        /// <summary>
        /// 컬럼 헤더
        /// </summary>
        private string _COL_HEAD;
        public string COL_HEAD
        {
            get { return this._COL_HEAD; }
            set
            {
                if (this._COL_HEAD != value)
                {
                    this._COL_HEAD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ORD_CNT - 오더수
        /// <summary>
        /// 오더수
        /// </summary>
        private int _ORD_CNT;
        public int ORD_CNT
        {
            get { return this._ORD_CNT; }
            set
            {
                if (this._ORD_CNT != value)
                {
                    this._ORD_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_CNT - SKU수
        /// <summary>
        /// 오더수
        /// </summary>
        private int _SKU_CNT;
        public int SKU_CNT
        {
            get { return this._SKU_CNT; }
            set
            {
                if (this._SKU_CNT != value)
                {
                    this._SKU_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
