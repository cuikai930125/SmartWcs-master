using SMART.WCS.Common.Extensions;

namespace SMART.WCS.UI.Optimization.DataMembers.OPT0106
{
    /// <summary>
    /// 셀 유형별 sum 정보 조회
    /// </summary>
    public class CellOptProcMgtRslt : PropertyNotifyExtensions
    {
        #region + 셀 타입
        /// <summary>
        /// 셀 타입
        /// </summary>
        public string CELL_TYPE_CD { get; set; }
        #endregion

        #region + 셀 CBM
        /// <summary>
        /// 셀 CBM
        /// </summary>
        public decimal CELL_CBM { get; set; }
        #endregion

        #region + 최대 점유셀
        /// <summary>
        /// 최대 점유셀
        /// </summary>
        public int MAX_CELL_QTY { get; set; }
        #endregion

        #region + 셀 수
        /// <summary>
        /// 셀 수
        /// </summary>
        public int CELL_QTY { get; set; }
        #endregion

        #region + SKU 수
        /// <summary>
        /// SKU 수
        /// </summary>
        public int SKU_CNT { get; set; }
        #endregion

        #region + 오더수
        /// <summary>
        /// 오더수
        /// </summary>
        public int ORD_CNT { get; set; }
        #endregion

        #region + 마스터 기준 SKU수
        /// <summary>
        /// 마스터 기준 SKU수
        /// </summary>
        public int MAST_SKU_CNT { get; set; }
        #endregion
    }
}
