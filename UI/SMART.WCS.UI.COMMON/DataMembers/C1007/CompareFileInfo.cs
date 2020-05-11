using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.COMMON.DataMembers.C1007
{
    public class CompareFileInfoRequest
    {
        [DataMember (Order = 0)]
        public List<CompareFileInfo> FileListArray { get; set; }

        public CompareFileInfoRequest()
        {
            this.FileListArray = new List<CompareFileInfo>();
        }
    }

    public class CompareFileInfo
    {
        [DataMember(Order = 0)]
        public string FileName { get; set; }

        [DataMember(Order = 1)]
        public string FileExtension { get; set; }

        [DataMember(Order = 2)]
        public DateTime FileLastModifyDateTime { get; set; }

        [DataMember(Order = 3)]
        public string FileDirectory { get; set; }
    }
}
