using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SMART.WCS.UI.COMMON.DataMembers.C1007
{
    [DataContract]
    public class LocalFileInfoRequest : InterfaceHeader
    {
        /// <summary>
        /// 전송시간
        /// </summary>
        [DataMember (Order = 0)]
        public override DateTime TRANSFER_DT { get; set; }

        /// <summary>
        /// 파일 개수
        /// </summary>
        [DataMember (Order = 1)]
        public override int FILE_CNT { get; set; }

        /// <summary>
        /// 로컬 파일 리스트
        /// </summary>
        [DataMember (Order = 2)]
        public List<LocalFileInfo> LOCAL_FILE_LIST { get; set; }

        public LocalFileInfoRequest()
        {
            this.LOCAL_FILE_LIST = new List<LocalFileInfo>();
        }
    }

    /// <summary>
    /// 로컬 파일 리스트
    /// <br />(서버 파일과 비교하기 위한 배포 대상 로컬 (사용자 PC) 파일 리스트)
    /// </summary>
    public class LocalFileInfo
    {
        /// <summary>
        /// 로컬 파일명
        /// </summary>
        [DataMember(Order = 0)]
        public string LOCAL_FILE_NM { get; set; }

        /// <summary>
        /// 서버 파일버전
        /// </summary>
        [DataMember(Order = 1)]
        public string LOCAL_FILE_VERSION { get; set; }
    }
}
