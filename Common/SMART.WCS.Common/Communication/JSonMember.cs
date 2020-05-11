using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common.Communication
{
    public class JSonMember
    {
    }

    public class BaseResponse
    {
        /// <summary>
        /// 상태코드
        /// </summary>
        [DataMember (Order = 0)]
        public string code { get; set; }

        /// <summary>
        /// 상세
        /// </summary>
        [DataMember(Order = 1)]
        public string descryption { get; set; }
    }

    public class JSonNotSendSorterClass
    {
        public string sortingId { get; set; }

        public string trayCode { get; set; }

        public string invoiceNumber { get; set; }

        public string boxCode { get; set; }

        public string sortingCode { get; set; }

        public string scanTime { get; set; }

        public string sortTime { get; set; }

        public string chuteNumber { get; set; }

        public int turnNumber { get; set; }

        public string imagePath { get; set; }

        public string errCode { get; set; }
    }
}
