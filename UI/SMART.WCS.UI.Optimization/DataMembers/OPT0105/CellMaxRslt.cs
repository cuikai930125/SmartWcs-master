using SMART.WCS.Common.Extensions;

namespace SMART.WCS.UI.Optimization.DataMembers.OPT0105
{
    /// <summary>
    /// 셀 유형별 셀 수
    /// </summary>
    public class CellMaxRslt : PropertyNotifyExtensions
    {
        #region + 셀 유형
        /// <summary>
        /// 셀 유형
        /// </summary>
        public string CELL_TYPE_CD { get; set; }
        #endregion

        #region + 최대 점유 셀 수
        /// <summary>
        /// 최대 점유 셀 수
        /// </summary>
        public int CELL_QTY { get; set; }
        #endregion
    }
}
