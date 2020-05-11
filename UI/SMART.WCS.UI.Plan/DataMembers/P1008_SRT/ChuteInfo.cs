using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P1008_SRT
{
    public class ChuteInfo : PropertyNotifyExtensions
    {
        /// <summary>
        /// 슈트 ID
        /// </summary>
        public string CHUTE_ID { get; set; }

        /// <summary>
        /// 슈트명
        /// </summary>
        public string CHUTE_NM { get; set; }
    }
}
