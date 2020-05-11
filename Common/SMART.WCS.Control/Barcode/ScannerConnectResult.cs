namespace SMART.WCS.Control.Barcode
{
    public class ScannerConnectResult
    {
        #region ▩ Enum
        public enum ScannerErrorCode
        {
            NONE                = -1,
            DRIVER_NOT_LOAD     = 0,
            DEVICE_NOT_FOUND    = 1,
            SETTING_ERROR       = 2,
            DEVICE_DISCONNECT   = 3,
            DEVICE_CONNECT      = 4,
            EXCEPTION           = 99
        }
        #endregion

        #region ▩ 속성
        private ScannerErrorCode _ErrorCode;

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

        public bool Result { get; set; }

        public string Message { get; set; }
        #endregion

        #region ▩ 함수
        public ScannerConnectResult()
        {
            this.ErrorCode = ScannerErrorCode.NONE;
        }
        #endregion
    }
}