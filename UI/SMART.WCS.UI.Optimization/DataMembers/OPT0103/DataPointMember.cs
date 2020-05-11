using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Optimization.DataMembers.OPT0103
{
    public class DataPointMember
    {
        public string CellTypeCd { get; private set; }

        public double Argument { get; private set; }

        public double Value { get; private set; }

        public DataPointMember(double arg, double val)
        {
            Argument        = arg;
            Value           = val;
        }

        public DataPointMember(string typeCd, double arg, double val)
        {
            this.CellTypeCd     = typeCd;
            Argument            = arg;
            Value               = val;
        }
    }
}
