using SMART.WCS.Common;
using SMART.WCS.Control.Ctrl;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.Control.Barcode
{
    /// <summary>
    /// BarCodeReader.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BarCodeReader : UserControl
    {
        #region ▩ 전역변수

        private BaseClass BaseClass = new BaseClass();

        private BaseApp g_baseApp = null;
        
        private SerialScannerConnect g_serialScanner = null;

        private bool g_isLoaed = false;
        private bool g_isFocus = false;

        private bool g_isFirstLoad = true;
        #endregion

        #region ▩ Override
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (IsSerialPortMode && SerialPortNo < 1)
            {
                SerialPortNo = 1;
            }
        }
        #endregion

        #region ▩ 생성자
        public BarCodeReader()
        {
            try
            {
                InitializeComponent();
                this.Loaded += BarCodeReader_Loaded;
                this.Unloaded += BarCodeReader_Unloaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        public event EventHandler<BarCodeReceivedEventArgs> BarCodeReceived;

        public static readonly DependencyProperty InputParentProperty =
                              DependencyProperty.Register("InputParent", typeof(System.Windows.Controls.Control), typeof(BarCodeReader), new FrameworkPropertyMetadata(null));

        public System.Windows.Controls.Control InputParent
        {
            get { return (System.Windows.Controls.Control)GetValue(InputParentProperty); }
            set { SetValue(InputParentProperty, value); }
        }

        public static readonly DependencyProperty IsSerialPortModeProperty =
                      DependencyProperty.Register("IsSerialPortMode", typeof(bool), typeof(BarCodeReader), null);

        public bool IsSerialPortMode
        {
            get { return (bool)GetValue(IsSerialPortModeProperty); }
            set { SetValue(IsSerialPortModeProperty, value); }
        }

        public static readonly DependencyProperty SerialPortNoProperty =
                      DependencyProperty.Register("SerialPortNo", typeof(int), typeof(BarCodeReader), null);

        public int SerialPortNo
        {
            get { return (int)GetValue(SerialPortNoProperty); }
            set { SetValue(SerialPortNoProperty, value); }
        }

        public bool IsLogin
        {
            get { return (bool)GetValue(IsLoginProperty); }
            set { SetValue(IsLoginProperty, value); }
        }

        public static readonly DependencyProperty IsLoginProperty =
                DependencyProperty.Register("IsLogin", typeof(bool), typeof(BarCodeReader), null);

        #region 포트번호
        public static readonly DependencyProperty SerialPortNameProperty =
            DependencyProperty.Register("SerialPortName", typeof(string), typeof(BarCodeReader), null);

        public string SerialPortName
        {
            get { return (string)GetValue(SerialPortNameProperty); }
            set { SetValue(SerialPortNameProperty, value); }
        }
        #endregion


        public static readonly DependencyProperty BaudRateProperty =
                      DependencyProperty.Register("BaudRate", typeof(int), typeof(BarCodeReader), null);

        public int BaudRate
        {
            get { return (int)GetValue(BaudRateProperty); }
            set { SetValue(BaudRateProperty, value); }
        }

        #region 핸드스캐너 사용여부
        public static readonly DependencyProperty HandScannerProperty =
            DependencyProperty.Register("HandScanner", typeof(bool), typeof(BarCodeReader), null);

        public bool HandScanner
        {
            get { return (bool)GetValue(HandScannerProperty); }
            set { SetValue(HandScannerProperty, value); }
        }
        #endregion

        #region > 시리얼 스캐너 통신 규약
        public static readonly DependencyProperty SerialScannerCommCodeProperty = 
            DependencyProperty.Register("SerialScannerCommCode", typeof(BaseEnumClass.SerialScannerCode), typeof(BarCodeReader), null);

        public BaseEnumClass.SerialScannerCode SerialScannerCommCode
        {
            get { return (BaseEnumClass.SerialScannerCode)GetValue(SerialScannerCommCodeProperty); }
            set { SetValue(SerialScannerCommCodeProperty, value); }
        }
        #endregion
        #endregion

        //public void CloseSerialScanner()
        //{
        //    this.g_serialScanner.CloseCoreScanner();
        //}

        #region ▩ 함수
        private bool LoadScanner()
        {
            try
            {
                bool isRtnValue = false;

                if (IsSerialPortMode)
                {
                    this.g_serialScanner = this.g_baseApp.SerialScanner;

                    if (this.g_serialScanner != null)
                    {
                        switch (this.SerialScannerCommCode)
                        {
                            case BaseEnumClass.SerialScannerCode.CR_LF:
                                this.g_serialScanner.StartPreFix    = (byte)13; // CR
                                this.g_serialScanner.EndPreFix      = (byte)10; // LF
                                break;

                            case BaseEnumClass.SerialScannerCode.STX_ETX:
                            default:
                                this.g_serialScanner.StartPreFix    = (byte)2;  // STX
                                this.g_serialScanner.EndPreFix      = (byte)3;  // ETX
                                break;
                        }

                        // 화면에서 포트 번호를 설정하는 경우 포트번호를 Class로 넘겨서 사용
                        this.g_serialScanner.PortName   = this.SerialPortName;
                        var _result                     = this.g_serialScanner.ConnectScanner();

                        isRtnValue = _result == null ? false : true;

                        if (_result != null)
                        {
                            if (_result.Result)
                            {
                                this.g_serialScanner.BarCodeReceived += Scanner_BarCodeReceived;
                            }
                            else
                            {
                                this.g_serialScanner.ScannerStatus();
                            }
                        }
                    }
                }

                return isRtnValue;
            }
            catch { throw; }
        }

        #region > LoadBarCodeScanner - 바코드 스캐너 로드 및 이벤트 생성
        /// <summary>
        /// 바코드 스캐너 로드 및 이벤트 생성
        /// </summary>
        public void LoadBarCodeScanner()
        {
            if (InputParent != null)
            {
                if (IsEnabled && ! this.g_isLoaed)
                {
                    this.g_isLoaed = true;

                    if (LoadScanner() == false) { return; }

                    this.Focus();
                    this.g_isFocus = true;
                    InputParent.GotFocus += InputParent_GotFocus;
                    InputParent.LostFocus += InputParent_LostFocus;
                    InputParent.MouseEnter += InputParent_MouseEnter;
                    InputParent.MouseDown += InputParent_MouseDown;
                    InputParent.MouseLeave += InputParent_MouseLeave;
                }
            }
        }
        #endregion

        #region > UnLoadBarCodeScanner - 이벤트 제거
        public void UnLoadBarCodeScanner()
        {
            if (InputParent != null)
            {
                this.g_isFocus = false;
                this.Loaded -= BarCodeReader_Loaded;
                this.Unloaded -= BarCodeReader_Unloaded;

                InputParent.GotFocus -= InputParent_GotFocus;
                InputParent.LostFocus -= InputParent_LostFocus;
                InputParent.MouseEnter -= InputParent_MouseEnter;
                InputParent.MouseDown -= InputParent_MouseDown;
                InputParent.MouseLeave -= InputParent_MouseLeave;
            }

            if (this.g_serialScanner != null)
            {
                this.g_serialScanner.BarCodeReceived -= Scanner_BarCodeReceived;
                this.g_serialScanner = null;
            }
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        private void Scanner_BarCodeReceived(object sender, BarCodeReceivedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => {

                bool _IsVisible = false;

                if (this.InputParent is UserControl)
                {
                    var _window = this.InputParent.FindParent<Window>();

                    if (_window != null)
                    {
                        if (_window.IsActive)
                        {
                            _IsVisible = (this.InputParent as UserControl).IsVisible;
                        }
                    }
                }

                if (this.InputParent is Window)
                {
                    //var _window = this.InputParent.FindParent<Window>();

                    //if (_window != null)
                    //{
                    //    if (_window.IsActive)
                    //    {
                    //        _IsVisible = (this.InputParent as Window).IsVisible;
                    //    }
                    //}

                    var _window = this.InputParent.FindParent<Window>();

                    if (_window == null)
                    {
                        _window = this.InputParent as Window;

                        if (_window != null)
                        { 
                            //{
                            //    if (_window.IsActive)
                            //    {
                            //        _IsVisible = (this.InputParent as Window).IsVisible;
                            //    }
                            _IsVisible = true;
                        }
                    }
                    else
                    {
                        _window = this.InputParent.FindParent<Window>();

                        if (_window != null)
                        {
                            if (_window.IsActive)
                            {
                                _IsVisible = (this.InputParent as UserControl).IsVisible;
                            }
                        }
                    }
                }

                //if (this.InputParent is KioskForm)
                //{
                //    var _window = this.InputParent.FindParent<Window>();

                //    if (_window.IsActive)
                //    {
                //        _IsVisible = (this.InputParent as KioskForm).IsVisible;
                //    }
                //}

                if (_IsVisible)
                {
                    System.Diagnostics.Debug.WriteLine("BarCodeReader : " + e.BarCode);

                    BarCodeReceived?.Invoke(this, new BarCodeReceivedEventArgs { BarCode = e.BarCode });
                }
            }));
        }

        private void BarCodeReader_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.g_isFirstLoad == true)
            {
                this.g_isFirstLoad = false;

                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    if (Application.Current is BaseApp)
                    {
                        this.g_baseApp = (Application.Current as BaseApp);

                        if (this.IsLogin == true) { return; }

                        LoadBarCodeScanner();
                    }
                }
            }
        }

        private void BarCodeReader_Unloaded(object sender, RoutedEventArgs e)
        {
            UnLoadBarCodeScanner();
        }

        private void InputParent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.g_isFocus = true;
        }

        private void InputParent_MouseLeave(object sender, MouseEventArgs e)
        {
            this.g_isFocus = false;
        }

        private void InputParent_MouseEnter(object sender, MouseEventArgs e)
        {
            this.g_isFocus = true;
        }

        private void InputParent_LostFocus(object sender, RoutedEventArgs e)
        {
            this.g_isFocus = false;
        }

        private void InputParent_GotFocus(object sender, RoutedEventArgs e)
        {
            this.g_isFocus = true;
        }
        #endregion
    }
}
