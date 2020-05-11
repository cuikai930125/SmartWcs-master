using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Optimization.DataMembers.OPT0105
{
    public class CellSimuRsltList : PropertyNotifyExtensions
    {
        #region + 배치순번
        /// <summary>
        /// 배치순번
        /// </summary>
        public string BTCH_SEQ { get; set; }
        #endregion

        #region + 셀 유형
        /// <summary>
        /// 셀 유형
        /// </summary>
        public string CELL_TYPE_CD { get; set; }
        #endregion

        #region + 점유셀수
        /// <summary>
        /// 점유셀수
        /// </summary>
        public int CELL_QTY { get; set; }
        #endregion
        
        #region + 보충누적 SKU 개수
        /// <summary>
        /// 보충누적 SKU 개수
        /// </summary>
        public int SUPP_SKU_CNT { get; set; }
        #endregion

        #region + 누적 SKU 개수
        /// <summary>
        /// 보충누적 SKU 개수
        /// </summary>
        public int ADD_SUPP_SKU_CNT { get; set; }
        #endregion

        #region + 데이터 그룹 ID
        /// <summary>
        /// 데이터 그룹 ID
        /// </summary>
        public string DATA_SET_ID { get; set; }
        #endregion

        #region + 최적화 차수
        /// <summary>
        /// 최적화 차수
        /// </summary>
        public int OPT_SEQ { get; set; }
        #endregion
    }
}
