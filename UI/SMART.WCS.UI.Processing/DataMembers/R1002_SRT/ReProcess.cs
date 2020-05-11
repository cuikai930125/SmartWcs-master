using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Processing.DataMembers.R1002_SRT
{
    public class ReProcess
    {
        [DataMember(Order = 0)]
        public string sortingId { get; set; }

        [DataMember(Order = 1)]
        public string trayCode { get; set; }

        [DataMember(Order = 2)]
        public string invoiceNumber { get; set; }

        [DataMember(Order = 3)]
        public string boxCode { get; set; }

        [DataMember(Order = 4)]
        public string sortingCode { get; set; }

        [DataMember(Order = 5)]
        public string scanTime { get; set; }

        [DataMember(Order = 6)]
        public string sortTime { get; set; }

        [DataMember(Order = 7)]
        public string chuteNumber { get; set; }

        [DataMember(Order = 8)]
        public int turnNumber { get; set; }

        [DataMember(Order = 9)]
        public string imagePath { get; set; }

        [DataMember(Order = 10)]
        public string errorCode { get; set; }
    }
}
