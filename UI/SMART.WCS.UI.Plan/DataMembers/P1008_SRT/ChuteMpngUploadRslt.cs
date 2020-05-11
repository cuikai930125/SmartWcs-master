using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P1008_SRT
{
    public class ChuteMpngUploadRslt : PropertyNotifyExtensions
    {
        /// <summary>
        /// ATTR02 - 플랜코드(PLAN_CD)
        /// </summary>
        public string ATTR02 { get; set; }

        /// <summary>
        /// ATTR03 - 플랜명(PLAN_NM)
        /// </summary>
        public string ATTR03 { get; set; }

        /// <summary>
        /// ATTR04 - 권역코드(RGN_CD)
        /// </summary>
        public string ATTR04 { get; set; }

        /// <summary>
        /// ATTR05 - 슈트ID(ChuteID)
        /// </summary>
        public string ATTR05 { get; set; }

        /// <summary>
        /// 결과 코드
        /// </summary>
        public string PROC_RSLT_CD { get; set; }

        /// <summary>
        /// 결과 메세지
        /// </summary>
        public string PROC_RSLT_MSG { get; set; }
    }
}
