using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.COMMON.DataMembers.C1008
{
    public class ReProcess
    {
        //   "sortingId": "",
        //"trayCode": "",
        //"invoiceNumber": "1575354101438",
        //"boxCode": "",
        //"sortingCode": "",
        //"scanTime": "20191204162600",
        //"sortTime": "20191204162600",
        //"chuteNumber": "101",
        //"turnNumber": 1,
        //"imagePath": "",
        //"errorCode": ""


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

    public class Sorter
    {
        [DataMember (Order = 0)]
        public string sorterId { get; set; }

        [DataMember (Order = 1)]
        public string sortingMode { get; set; }

        public string version { get; set; }

        public List<Rules> rules { get; set; }

        public Sorter()
        {
            rules = new List<Rules>();
        }
    }

    public class Rules
    {
        [DataMember (Order = 0)]
        public string chuteNumber { get; set; }

        [DataMember (Order = 1)]
        public string sortingCode { get; set; }

        [DataMember (Order = 2)]
        public string sortingCodeLegacy { get; set; }
    }

    public class Barcode
    {
        [DataMember (Order = 0)]
        public string INVOICE_NUM { get; set; }

        [DataMember (Order = 1)]
        public string BOX_CODE { get; set; }

        [DataMember (Order = 2)]
        public string SORTING_CODE { get; set; }
    }
}
