using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.BarcodeScanner
{
    public class ScannerConnectResult
    {
        public ScannerConnectResult()
        {
            this.ErrorCode = ScannerErrorCode.NONE;
        }

        public bool Result { get; set; }

        public string Message { get; set; }

        public ScannerErrorCode ErrorCode
        {
            get
            {
                return _ErrorCode;
            }

            set
            {
                this._ErrorCode = value;
            }
        }

        private ScannerErrorCode _ErrorCode;

    }

    public enum ScannerErrorCode
    {
        NONE = -1,
        DRIVER_NOT_LOAD = 0,
        DEVICE_NOT_FOUND = 1,
        SETTING_ERROR = 2,
        DEVICE_DISCONNECT = 3,
        DEVICE_CONNECT = 4,
        EXCEPTION = 99
    }
}
