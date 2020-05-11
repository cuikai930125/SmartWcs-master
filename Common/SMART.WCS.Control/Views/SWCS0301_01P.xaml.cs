using SMART.WCS.Common;
using SMART.WCS.Control.DataMembers;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SMART.WCS.Control.Views
{
    /// <summary>
    /// 시리얼 스캐너 설정 
    /// </summary>
    public partial class SWCS0301_01P : Window, IDisposable
    {
        #region ▩ 전역변수
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 시리얼 스캐너 정보 로드 여부
        /// <br />연결이 안된 경우 false로 값 변경
        /// </summary>
        private bool g_isSerialScannerLoadYN = true;
        #endregion

        #region ▩ 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public SWCS0301_01P()
        {
            try
            {
                InitializeComponent();

                // 컨트롤 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 속성
        /// <summary>
        /// 시리얼 포트 정보
        /// </summary>
        public SerialPortInfo SelectPortInfo { get; set; }

        public bool IsRtnValue { get; set; }
        #endregion

        #region ▩ 함수
        #region > InitControl - 컨트롤 초기화
        /// <summary>
        /// 컨트롤 초기화
        /// </summary>
        private void InitControl()
        {
            try
            {
                this.cboBaudRate.ItemsSource = Enum.GetValues(typeof(BaudRate));

                this.InitSerialPortList();
            }
            catch { throw; }
        }
        #endregion

        #region > InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            try
            {
                this.btnFormClose.PreviewMouseLeftButtonUp += BtnFormClose_PreviewMouseLeftButtonUp;
                // 포트 검색 버튼 클릭 이벤트
                this.btnRefresh.PreviewMouseLeftButtonUp += BtnRefresh_PreviewMouseLeftButtonUp;
                // 포트 정보 적용 버튼 클릭 이벤트
                this.btnApply.PreviewMouseLeftButtonUp += BtnApply_PreviewMouseLeftButtonUp;
            }
            catch { throw; }
        }
        #endregion

        #region > InitSerialPortList - 포트 리스트 초기화
        /// <summary>
        /// 포트 리스트 초기화
        /// </summary>
        private void InitSerialPortList()
        {
            try
            {
                this.cboSerialPort.IsPopupOpen  = false;

                var arrStrSerialPort            = SerialPort.GetPortNames();
                this.cboSerialPort.ItemsSource  = arrStrSerialPort;

                if (arrStrSerialPort.Count() > 0)
                {
                    this.cboSerialPort.IsPopupOpen = true;
                }
                else
                {
                    this.g_isSerialScannerLoadYN = false;
                    // Msg : 시리얼 포트 정보를 로드하지 못했습니다.|스캐너 연결 상태를 확인해주세요.
                    this.BaseClass.MsgError("ERR_NOT_CONNECT_SERIAL_SCANNER");
                }
            }
            catch { throw; }
        }
        #endregion

        #endregion

        #region ▩ 이벤트
        /// <summary>
        /// 창 닫기 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFormClose_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.g_isSerialScannerLoadYN == true)
                {
                    // 시리얼 스캐너 정보를 변경하지 않으셨습니다.|창을 종료하시겠습니까?
                    this.BaseClass.MsgQuestion("ASK_DO_NOT_CHG_SERIAL_INFO");
                    if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }
                }

                this.Close();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 시리얼 포트 검색 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRefresh_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.InitSerialPortList();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 적용 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnApply_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int iPortNo     = -1;
                int iBaudRate   = -1;

                if (this.cboSerialPort.SelectedIndex == -1)
                {
                    // 시리얼번호 콤보박스 항목을 선택해주세요.
                    this.BaseClass.MsgError("SEL_COMBO_SERIAL_NO");
                    return;
                }

                if (this.cboBaudRate.SelectedIndex == -1)
                {
                    // BaudRate 콤보박스 항목을 선택해주세요.
                    this.BaseClass.MsgError("SEL_COMBO_BAUDRATE");
                    return;
                }

                if (this.cboSerialPort.SelectedIndex > -1 && this.cboBaudRate.SelectedIndex > -1)
                {
                    //var strPortNo       = this.BaseClass.ComboBoxSelectedKeyValue(this.cboSerialPort).Replace("COM", string.Empty);
                    //var strBaudRate     = this.BaseClass.ComboBoxSelectedKeyValue(this.cboBaudRate).ToUpper().Replace("RATE", string.Empty);

                    var strPortNo       = this.cboSerialPort.SelectedItem.ToString().Replace("COM", string.Empty);
                    var strBaudRate     = this.cboBaudRate.SelectedItem.ToString().ToUpper().Replace("RATE", string.Empty);

                    if (int.TryParse(strPortNo, out iPortNo) && int.TryParse(strBaudRate, out iBaudRate))
                    {
                        this.SelectPortInfo = new SerialPortInfo
                        {
                            PortNo      = iPortNo,
                            BaudRate    = iBaudRate
                        };
                    }
                }

                //(this.Parent as Window).DialogResult = true;
                this.IsRtnValue = true;
                this.Close();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #region > 그리드 이벤트
        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
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
        // ~SWCS0301_01P()
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
