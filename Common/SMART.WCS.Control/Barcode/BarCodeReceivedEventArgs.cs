using System;

namespace SMART.WCS.Control.Barcode
{
    public class BarCodeReceivedEventArgs : EventArgs
    {
        public BarCodeType Type { get; set; }
        public string BarCode { get; set; }
    }

    public enum BarCodeType
    {
        BarCode
    }
}