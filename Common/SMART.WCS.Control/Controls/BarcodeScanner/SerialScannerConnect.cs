using SMART.WCS.Common;
using SMART.WCS.Control.DataMembers;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.BarcodeScanner
{
    public class SerialScannerConnect : IDisposable
    {
        #region ▩ 전역변수
        BaseClass BaseClass = new BaseClass();
        /// <summary>
        /// 바코드 이벤트
        /// </summary>
        public event EventHandler<BarcodeReceivedEventArgs> BarcodeReceived;

        private const string _BarCodeErrorNotShow = "BarCodeErrorNotShow";

        /// <summary>
        /// 시리얼 포트 객체 선언
        /// </summary>
        private SerialPort g_serialPort;

        public ScannerConnectResult ConnectResult = null;

        List<byte> _tempDate = new List<byte>();
        #endregion

        #region ▩ 생성자
        public SerialScannerConnect()
        {

        }
        #endregion

        #region ▩ 속성
        public byte StartPreFix { get; set; }

        public byte EndPreFix { get; set; }

        public int PortNo { get; set; }

        public int BaudRateValue { get; set; }

        /// <summary>
        /// 시리얼 통신 방법
        /// </summary>
        public BaseEnumClass.SerialCommMethod SerialCommMethod { get; set; }
        #endregion

        #region ▩ 함수
        public ScannerConnectResult ConnectScanner()
        {
            return null;
        }

        private ScannerConnectResult InitScanner()
        {
            try
            {
                ScannerConnectResult result     = new ScannerConnectResult();
                SerialPortInfo serialPortInfo   = null;
                string strPortName              = string.Empty;

                switch (this.SerialCommMethod)
                {
                    case BaseEnumClass.SerialCommMethod.SERIAL_STX_ETX:
                        
                        break;

                    case BaseEnumClass.SerialCommMethod.SERIAL_CR_LF:
                        break;
                }
            }
            catch { throw; }
        }

        #endregion


        //public SerialPortInfo SerialPortLoad(int _iPortNo, int _iBaudRate)
        //{
        //    try
        //    {
        //        SerialPortInfo _result      = null;
        //        string _SerialPort          = $"COM{_iPortNo}";

        //    }
        //    catch { throw; }
        //}


        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~SerialScannerConnect()
        // {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
