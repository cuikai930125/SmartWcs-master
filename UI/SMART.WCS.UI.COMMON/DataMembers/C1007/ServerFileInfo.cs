using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SMART.WCS.UI.COMMON.DataMembers.C1007
{
    [DataContract]
    public class ServerFileInfoResponse : InterfaceHeader
    {
        /// <summary>
        /// 전송시간
        /// </summary>
        [DataMember(Order = 0)]
        public override DateTime TRANSFER_DT { get; set; }

        /// <summary>
        /// 파일 개수
        /// </summary>
        [DataMember(Order = 1)]
        public override int FILE_CNT { get; set; }

        /// <summary>
        /// 서버 파일 리스트
        /// </summary>
        [DataMember(Order = 2)]
        public List<ServerFileInfo> SERVER_FILE_LIST { get; set; }

        public ServerFileInfoResponse()
        {
            this.SERVER_FILE_LIST = new List<ServerFileInfo>();
        }
    }

    public class ServerFileInfo
    {
        /// <summary>
        /// 서버 파일명
        /// </summary>
        [DataMember(Order = 0)]
        public string SERVER_FILE_NM { get; set; }

        /// <summary>
        /// 로컬 파일버전
        /// </summary>
        [DataMember(Order = 1)]
        public string LOCAL_FILE_VERSION { get; set; }

        /// <summary>
        /// 서버 파일버전
        /// </summary>
        [DataMember(Order = 2)]
        public string SERVER_FILE_VERSION { get; set; }

        /// <summary>
        /// 버전 비교 상태값
        /// </summary>
        [DataMember(Order = 3)]
        public string STATUS_CD { get; set; }
    }
}
