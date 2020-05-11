using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.StatusBoard.DataMembers
{
    public class StatusBoard : PropertyNotifyExtensions
    {
        public StatusBoard() { }

        #region + Zone ID
        /// <summary>
        /// Zone ID
        /// </summary>
        public string ZONE_ID { get; set; }
        #endregion

        #region + Zone명
        /// <summary>
        /// Zone명
        /// </summary>
        public string ZONE_NM { get; set; }
        #endregion

        #region + 정상수량
        /// <summary>
        /// 정상수량
        /// </summary>
        public int NML_RSLT_QTY { get; set; }
        //private int _NML_RSLT_QTY;
        //public int NML_RSLT_QTY
        //{
        //    get { return this._NML_RSLT_QTY; }
        //    set
        //    {
        //        if (this._NML_RSLT_QTY != value)
        //        {
        //            this._NML_RSLT_QTY = value;
        //            RaisePropertyChanged();
        //        }
        //    }
        //}
        #endregion

        #region + 오류수량
        /// <summary>
        /// 오류수량
        /// </summary>
        public int ERR_QTY { get; set; }
        #endregion

        #region + 합계
        /// <summary>
        /// 합계
        /// </summary>
        public int TOT_RSLT_QTY { get; set; }
        #endregion

        #region + 정상분류율
        /// <summary>
        /// 상분류율
        /// </summary>
        public decimal RT_TOT_QTY { get; set; }
        #endregion
    }
}
