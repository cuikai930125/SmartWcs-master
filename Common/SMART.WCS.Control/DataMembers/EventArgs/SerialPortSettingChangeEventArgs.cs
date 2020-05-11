using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.DataMembers
{
    public class SerialPortSettingChangeEventArgs : EventArgs
    {
        public SerialPortInfo PortInfo { get; set; }
    }

    public class SerialPortInfo
    {
        public int PortNo { get; set; }
        public int BaudRate { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }

    public enum BaudRate
    {
        RateNONE = 0,
        Rate75 = 75,
        Rate150 = 150,
        Rate300 = 300,
        Rate600 = 600,
        Rate1200 = 1200,
        Rate2400 = 2400,
        Rate4800 = 4800,
        Rate9600 = 9600,
        Rate19200 = 19200,
        Rate38400 = 38400,
        Rate57600 = 57600,
        Rate115200 = 115200,
        Rate230400 = 230400
    }
}
