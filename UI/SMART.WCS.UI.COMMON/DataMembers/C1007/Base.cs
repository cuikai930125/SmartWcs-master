using System;
using System.Runtime.Serialization;

namespace SMART.WCS.UI.COMMON.DataMembers.C1007
{
    [DataContract]
    public class InterfaceHeader
    {
        /// <summary>
        /// 전송시간
        /// </summary>
        [DataMember(Order = 0)]
        public virtual DateTime TRANSFER_DT { get; set; }

        /// <summary>
        /// 파일개수
        /// </summary>
        [DataMember(Order = 1)]
        public virtual int FILE_CNT { get; set; }
    }
}
