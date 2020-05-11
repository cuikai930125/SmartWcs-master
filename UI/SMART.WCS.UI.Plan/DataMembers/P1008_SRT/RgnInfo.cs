using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P1008_SRT
{
    /// <summary>
    /// 권역코드 정보
    /// </summary>
    public class RgnInfo : PropertyNotifyExtensions
    {
        /// <summary>
        /// 권역코드
        /// </summary>
        public string RGN_CD { get; set; }

        /// <summary>
        /// 권역명
        /// </summary>
        public string RGN_NM { get; set; }

        /// <summary>
        /// 택배사명
        /// </summary>
        public string DLV_CO_NM { get; set; }

        /// <summary>
        /// 택배사 권역코드
        /// </summary>
        public string DLV_CO_RGN_CD { get; set; }

        /// <summary>
        /// 택배사 권역명
        /// </summary>
        public string DLV_CO_RGN_NM { get; set; }
    }
}
