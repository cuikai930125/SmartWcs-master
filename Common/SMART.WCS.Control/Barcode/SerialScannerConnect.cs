using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using SMART.WCS.Control.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static SMART.WCS.Control.Barcode.ScannerConnectResult;
using static SMART.WCS.Control.Barcode.SerialPortSettingChangeEventArgs;

namespace SMART.WCS.Control.Barcode
{
    public class SerialScannerConnect : IDisposable
    {
        #region ▩ 전체변수
        BaseClass BaseClass = new BaseClass();

        public event EventHandler<BarCodeReceivedEventArgs> BarCodeReceived;
        private const string BARCODE_ERROR_NOT_SHOW = "BarCodeErrorNotShow";
        public SerialPort g_serialPort             = null;
        public ScannerConnectResult g_connectResult = null;

        List<byte> g_byteBarCodeTempData = new List<byte>();
        #endregion

        #region ▩ 속성
        public byte StartPreFix { get; set; }
        public byte EndPreFix { get; set; }

        /// <summary>
        /// 포트번호
        /// </summary>
        public string PortName { get; set; }

        public int BaudRateValue { get; set; }
        #endregion

        #region ▩ 함수

        public ScannerConnectResult ConnectScanner()
        {
            this.g_connectResult = InitScanners();
            return this.g_connectResult;
        }

        private ScannerConnectResult InitScanners()
        {
            string strGetPortName     = string.Empty;
            foreach (var port in SerialPort.GetPortNames())
            {
                strGetPortName = port;
            }

            if (strGetPortName.Length == 0) { return null; }

            ScannerConnectResult result     = new ScannerConnectResult();
            string strErrMessgae            = this.BaseClass.GetResourceValue("ERR_BARSCN_DEVICE_NOT_FOUND");

            try
            {
                var strPortName                 = string.Empty;
                SerialPortInfo _SerialPortInfo  = this.SerialPortLoad();

                if (_SerialPortInfo != null)
                {
                    if (this.g_serialPort == null)
                    {
                        this.g_serialPort = new SerialPort();
                        var strSerialPortNo = $"COM{_SerialPortInfo.PortNo.ToWhiteSpaceOrString()}";

                        this.g_serialPort.PortName  = strSerialPortNo;
                        this.g_serialPort.BaudRate  = _SerialPortInfo.BaudRate;
                        this.g_serialPort.DataBits  = 8;
                        this.g_serialPort.StopBits  = StopBits.One;
                        this.g_serialPort.Parity    = Parity.None;
                        this.g_serialPort.Handshake = Handshake.None;

                        this.g_serialPort.DataReceived += SerialPort_DataReceived;
                        this.g_serialPort.Open();
                    }

                    if (this.g_serialPort?.IsOpen == true)
                    {
                        result.Result = true;
                    }
                    else
                    {
                        result.Result       = false;
                        result.Message      = strErrMessgae;
                    }
                }
            }
            catch (System.IO.IOException ex)
            {
                result.Result       = false;
                result.Message      += "\r\n" + ex.Message;
            }
            catch (Exception ex)
            {
                result.Result       = false;
                result.Message      = ex.Message;
            }

            return result;
        }

        public void SettingSave(bool Value)
        {
            var configFile  = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings    = configFile.AppSettings.Settings;
            if (settings[BARCODE_ERROR_NOT_SHOW] == null)
            {
                settings.Add(BARCODE_ERROR_NOT_SHOW, Value.ToString());
            }
            else
            {
                settings[BARCODE_ERROR_NOT_SHOW].Value = Value.ToString();
            }

            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }

        public bool SettingLoad()
        {
            bool _result        = false;
            var configFile      = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings        = configFile.AppSettings.Settings;

            if (settings[BARCODE_ERROR_NOT_SHOW] != null)
            {
                var _value = settings[BARCODE_ERROR_NOT_SHOW].Value;
                bool.TryParse(_value, out _result);
            }

            return _result;
        }

        public void ScannerStatus()
        {

            var _result = this.g_connectResult;

            if (!_result.Result && !SettingLoad())
            {
                string _MessageCode = null;


                switch (_result.ErrorCode)
                {
                    case ScannerErrorCode.NONE:
                        _MessageCode = "ERR_BARSCN_NONE";
                        break;
                    case ScannerErrorCode.DRIVER_NOT_LOAD:
                        _MessageCode = "ERR_BARSCN_DRIVER_NOT_LOAD";
                        break;
                    case ScannerErrorCode.DEVICE_NOT_FOUND:
                        _MessageCode = "ERR_BARSCN_DEVICE_NOT_FOUND";
                        break;
                    case ScannerErrorCode.SETTING_ERROR:
                        _MessageCode = "ERR_BARSCN_SETTING_ERROR";
                        break;
                    case ScannerErrorCode.EXCEPTION:
                        _MessageCode = "ERR_BARSCN_EXCEPTION";
                        break;
                    default:
                        break;
                }

                if (_MessageCode != null)
                {
                    //////var _DtMessage = GetMessage(_baseClass);
                    //////var msg = _DtMessage.AsEnumerable()
                    //////            .Where(row => row["MSG_CD"].ToWhiteSpaceOrString() == "ERR_BARSCN_DEVICE_NOT_FOUND")
                    //////            .Select(row => row["MSG"].ToWhiteSpaceOrString());

                    //////if (msg?.Count() > 0)
                    //////{
                    //////    _baseClass.WarningMsgOnce(msg.First(), this._baseInfo.country_cd);

                    //////    if (_baseClass.NOT_SHOW_YN)
                    //////    {
                    //////        SettingSave(true);
                    //////    }
                    //////}
                }
            }
        }

        public void CloseCoreScanner()
        {
            if (this.g_serialPort != null)
            {
                this.g_serialPort.DataReceived -= SerialPort_DataReceived;
                this.g_serialPort.Close();
                this.g_serialPort.Dispose();
                this.g_serialPort = null;
            }
        }

        /// <summary>
        //
        /// </summary>
        /// <returns>저장 또는 선택된 포트 ex) COM1 </returns>
        public SerialPortInfo SerialPortLoad()
        {
            SerialPortInfo _result      = null;
            int _resultPortNo           = -1;
            int _resultBaudRate         = -1;

            try
            {
                var _strPort        = GetConfig("PORT_NO");
                var _strBaudRate    = GetConfig("BAUDRATE");

                if (!string.IsNullOrWhiteSpace(_strPort))
                {
                    int.TryParse(_strPort, out _resultPortNo);
                }

                if (!string.IsNullOrWhiteSpace(_strBaudRate))
                {
                    int.TryParse(_strBaudRate, out _resultBaudRate);
                }

                string _SerialPort = $"COM{_resultPortNo.ToWhiteSpaceOrString()}";

                if (!SerialPort.GetPortNames().Contains(_SerialPort) && _resultBaudRate < -1)
                {
                    using (SWCS0301_01P frm = new SWCS0301_01P())
                    {

                        frm.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                        //if (frm.ShowDialog() == true)
                        //{
                            frm.ShowDialog();

                            if (frm.IsRtnValue == true)
                            {
                                var _portinfo = frm.SelectPortInfo;

                                if (_portinfo != null)
                                {
                                    SetConfig("PORT_NO", _portinfo.PortNo.ToString());
                                    SetConfig("BAUDRATE", _portinfo.BaudRate.ToString());

                                    _resultPortNo = _portinfo.PortNo;
                                    _resultBaudRate = _portinfo.BaudRate;
                                }
                                else
                                {
                                    _result = SerialPortLoad();
                                }
                            }
                            else
                            {
                                return null;
                            }
                            
                        //}
                        //else
                        //{
                        //    return null;
                        //}
                    }
                }
                else
                {
                    //_result = new SerialPortInfo { PortNo = _resultPortNo, BaudRate = _resultBaudRate };
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(new Exception("SerialPortLoad ERROR  : " + ex.ToString()));
            }

            if (_resultPortNo > -1 && _resultBaudRate > -1)
            {
                return new SerialPortInfo { PortNo = _resultPortNo, BaudRate = _resultBaudRate };
            }
            else
            {
                return null;
            }
        }

        #region SerialPortLoad - 시리얼 포트 정보를 조회하여 설정한다.
        /// <summary>
        /// 시리얼 포트 정보를 조회하여 설정한다.
        /// </summary>
        /// <param name="_strComPortName">컴포트 명</param>
        /// <returns></returns>
        public SerialPortInfo SerialPortLoad(string _strComPortName)
        {
            SerialPortInfo _result = null;
            int _resultPortNo = -1;
            int _resultBaudRate = -1;

            try
            {
                var _strPort = _strComPortName;
                var _strBaudRate = "9600";
                string _SerialPort = string.Empty;

                if (!string.IsNullOrWhiteSpace(_strPort))
                {
                    int.TryParse(_strPort, out _resultPortNo);
                }

                if (!string.IsNullOrWhiteSpace(_strBaudRate))
                {
                    int.TryParse(_strBaudRate, out _resultBaudRate);
                }

                if (_strComPortName.Contains("COM") == true)
                {
                    _SerialPort = _strComPortName;
                }
                else
                {
                    _SerialPort = $"COM{_resultPortNo.ToWhiteSpaceOrString()}";
                }

                if (!SerialPort.GetPortNames().Contains(_SerialPort) || _resultBaudRate < -1)
                {
                    var strMessage  = this.BaseClass.GetResourceValue("ERR_BARSCN_SERIAL_NOT_FOUND");
                    this.BaseClass.MsgError(string.Format(strMessage, _SerialPort), BaseEnumClass.CodeMessage.MESSAGE);
                }
                else
                {
                    _result = new SerialPortInfo { PortNo = _resultPortNo, BaudRate = _resultBaudRate };
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(new Exception("SerialPortLoad ERROR  : " + err.ToString()));
            }

            if (_resultPortNo > -1 && _resultBaudRate > -1)
            {
                return new SerialPortInfo { PortNo = _resultPortNo, BaudRate = _resultBaudRate };
            }
            else
            {
                return null;
            }
        }
        #endregion

        private string GetConfig(string Property)
        {
            return this.BaseClass.GetIniFile("ScannerSetting", Property);
        }

        private bool SetConfig(string Property, string Value)
        {
            try
            {
                this.BaseClass.WriteIniFile("ScannerSetting", Property, Value);
                return true;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
                return false;
            }
        }
        #endregion

        #region ▩ 이벤트
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[this.g_serialPort.ReadBufferSize];
            int bytesRead = this.g_serialPort.Read(buffer, 0, buffer.Length);

            this.g_byteBarCodeTempData.AddRange(buffer.ToList().Where(f => f != (byte)StartPreFix && f != (byte)EndPreFix && f != (byte)0));

            // CHOO
            //if (this._baseInfo.center_cd_string.Equals("YJ") == true)

            var aaa = buffer.Where(f => f != (byte)0).LastOrDefault();

            if (buffer.Where(f => f != (byte)0).LastOrDefault() == (byte)EndPreFix)
            {
                string data = Encoding.UTF8.GetString(this.g_byteBarCodeTempData.ToArray(), 0, this.g_byteBarCodeTempData.Count());
                //string data = serialPort.ReadLine();
                this.g_byteBarCodeTempData.Clear();

                if (!string.IsNullOrWhiteSpace(data.ToWhiteSpaceOrString()))
                {
                    var _barcode = data.Replace("\r", "").Replace("\n", "");

                    BarCodeReceived?.Invoke(this, new BarCodeReceivedEventArgs { BarCode = _barcode });
                }
            }
        }
        #endregion


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
