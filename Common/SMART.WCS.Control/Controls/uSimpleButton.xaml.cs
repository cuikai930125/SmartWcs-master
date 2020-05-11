using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using SMART.WCS.Common;
using SMART.WCS.Control.DataMembers;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMART.WCS.Control
{
    /// <summary>
    /// uSimpleButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class uSimpleButton : UserControl
    {
        #region ▩ 매개변수 (Dependency Property)
        /// <summary>
        /// Base Class
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// Base Info - 세션 정보
        /// </summary>
        BaseInfo BaseInfo = new BaseInfo();

        /// <summary>
        /// 버튼명
        /// </summary>
        private string g_strButtonText = string.Empty;

        //public static readonly DependencyProperty Contentproperty = DependencyProperty.Register("Text", typeof(string), typeof(uSimpleButton)
        //    , new PropertyMetadata(null, new PropertyChangedCallback(ContentpropertyChanged)));

        public static readonly DependencyProperty ButtonTypeProperty = DependencyProperty.Register("ButtonType", typeof(string), typeof(uSimpleButton)
        , new PropertyMetadata(null, new PropertyChangedCallback(ButtonTypePropertyChanged)));

        //public static readonly DependencyProperty RoleCDProperty = DependencyProperty.Register("RoleCD", typeof(string), typeof(uSimpleButton)
        //    , new PropertyMetadata(string.Empty, new PropertyChangedCallback(RoleCDPropertyChanged)));

        //public static readonly DependencyProperty IsControlEnabledProperty = DependencyProperty.Register("IsControlEnabled", typeof(bool), typeof(uSimpleButton)
        //    , new PropertyMetadata(true, new PropertyChangedCallback(uSimpleButton_IsEnabledChanged)));

        public static readonly DependencyProperty IsBoldTypeYNProperty = DependencyProperty.Register("IsBoldTypeYN", typeof(bool), typeof(uSimpleButton)
            , new PropertyMetadata(false, new PropertyChangedCallback(IsBoldTypeYNPropertyChanged)));

        public static readonly DependencyProperty IsIconVisibleYNProperty = DependencyProperty.Register("IsIconVisibleYN", typeof(bool), typeof(uSimpleButton)
            , new PropertyMetadata(true, new PropertyChangedCallback(IsIconVisibleYNPropertyChanged)));

        public static readonly DependencyProperty ButtonWidthLengthProperty = DependencyProperty.Register("ButtonWidthLength", typeof(int), typeof(uSimpleButton)
            , new PropertyMetadata(0, new PropertyChangedCallback(ButtonWidthLengthPropertyChanged)));



        #endregion

        #region ▩ 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public uSimpleButton()
        {
            InitializeComponent();
            //this.BaseInfo               = ((BaseApp)System.Windows.Application.Current).BASE_INFO;
            //this.BaseInfo.COUNTRY_CD    = "KR";     // 국가코드
            //this.BaseInfo.ROLE_CD       = "R";      // 권한코드
        }
        #endregion

        #region ▩ Override
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 버튼 속성을 정의한다.
            this.DefineSimpleButton(this.ButtonType.Trim());

            //this.PreviewMouseLeftButtonDown += USimpleButton_PreviewMouseLeftButtonDown;
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var isEnabled = this.BaseClass.RoleCode.Equals("A") ? true : false;
            
            if (isEnabled == true)
            {
                base.OnPreviewMouseLeftButtonUp(e);
            }
            else
            {
                if (this.ButtonType == "EXCEL_DOWNLOAD")
                {   
                    base.OnPreviewMouseLeftButtonUp(e);
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void USimpleButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //var isEnabled = this.BaseClass.RoleCode.Equals("A") ? true : false;

            //this.OnPreviewMouseLeftButtonUp(e);

            ////var owner = sender as uSimpleButton;

            ////owner.IsEnabled = false;

            ////owner.IsHitTestVisible = false;



            //if (isEnabled == false)
            //{
            //    e.Handled = true;
            //    return;
            //}
        }

        private void Owner_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ▩ 속성
        #region ButtonType - 버튼 타입
        /// <summary>
        /// 버튼 타입 (버튼 종류)
        /// </summary>
        public string ButtonType
        {
            get { return (string)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }
        #endregion

        //#region RoleCD - 권한코드
        ///// <summary>
        ///// 권한코드
        ///// </summary>
        //public string RoleCD
        //{
        //    get { return (string)GetValue(RoleCDProperty); }
        //    set { SetValue(RoleCDProperty, value); }
        //}
        //#endregion

        #region IsBoldTypeYN - 버튼명 볼트체 적용 여부
        /// <summary>
        /// 버튼명 볼트체 적용 여부
        /// </summary>
        public bool IsBoldTypeYN
        {
            get { return (bool)GetValue(IsBoldTypeYNProperty); }
            set { SetValue(IsBoldTypeYNProperty, value); }
        }
        #endregion

        #region IsIconVisibleYN - 버튼 아이콘 활성화 여부
        /// <summary>
        /// 버튼 아이콘 활성화 여부
        /// </summary>
        public bool IsIconVisibleYN
        {
            get { return (bool)GetValue(IsIconVisibleYNProperty); }
            set { SetValue(IsIconVisibleYNProperty, value); }
        }
        #endregion

        #region ButtonWidthLength - 버튼 가로 길이 (강제로 사이즈 설정시 사용)
        /// <summary>
        /// 버튼 가로 길이 (강제로 사이즈 설정시 사용)
        /// </summary>
        public int ButtonWidthLength
        {
            get { return (int)GetValue(ButtonWidthLengthProperty); }
            set { SetValue(ButtonWidthLengthProperty, value); }
        }
        #endregion

        //public bool IsControlEnabled
        //{
        //    get { return (bool)GetValue(IsControlEnabledProperty); }
        //    set { SetValue(IsControlEnabledProperty, value); }
        //}

        #endregion

        #region ▩ 함수
        #region > 버튼 속성 정의 (텍스트, 이미지, 커서, 사이즈)
        /// <summary>
        /// 버튼 속성 정의 (텍스트, 이미지, 커서, 사이즈)
        /// </summary>
        /// <param name="_strButtonType">버튼 타입</param>
        private void DefineSimpleButton(string _strButtonType)
        {
            try
            {
                if (this.ButtonType.Length > 0)
                {
                    // 행추가/삭제 버튼인 경우 버튼명을 공백으로 처리한다.
                    if (this.ButtonType.Equals("ROW_ADD") == true || this.ButtonType.Equals("ROW_DEL") == true)
                    {
                        this.btnCommonSimpleButton.Content      = string.Empty;
                        this.g_strButtonText                    = string.Empty;
                    }
                    else
                    {
                        //if (this.ButtonType.Equals("SEARCH") == true)
                        //{
                        //    this.btnCommonSimpleButton.Background   = this.BaseClass.ConvertStringToMediaBrush("D04343");
                        //}

                        // 버튼 텍스트를 정의한다.
                        this.btnCommonSimpleButton.Content      = this.GetResourceText(_strButtonType);
                        this.g_strButtonText                    = this.btnCommonSimpleButton.Content.ToString();
                    }

                    if (this.IsIconVisibleYN == true)
                    {
                        // 버튼 이미지, 커서를 정의한다.
                        this.GetGlyph(this.ButtonType.Trim());
                    }

                    // 버튼 가로, 세로 사이즈를 설정한다.
                    if (this.ButtonWidthLength > 0)
                    {
                        this.Width = this.ButtonWidthLength;
                    }
                    else
                    {
                        // 리소스 값이 없는 경우 버튼 Width를 0으로 설정한다.
                        if (this.g_strButtonText.Length == 0)
                        {
                            this.Width = 30;
                        }
                        else
                        {
                            this.SetButtonWidth(this.ButtonType.Trim());
                        }
                    }

                    // 버튼 권한을 설정한다.
                    this.SetRole(this.ButtonType.Trim());
                }
            }
            catch { throw; }
        }
        #endregion

        #region > SetRole - 권한 설정
        /// <summary>
        /// 권한 설정
        /// </summary>
        /// <param name="_strButtonType">버튼 타입</param>
        private void SetRole(string _strButtonType)
        {
            var isEnabled   = this.BaseClass.RoleCode.Equals("A") ? true : false;

            switch (_strButtonType.ToUpper())
            {
                case "SEARCH":
                case "EXCEL_DOWNLOAD":
                    isEnabled = true;
                    break;
                default: break;
            }

            Utility.HelperClass.SetButtonIsEnabled(this.btnCommonSimpleButton, isEnabled);

            this.IsBoldTypeYN = isEnabled;
        }
        #endregion

        #region > GetGlyph - 버튼 타입별 이미지를 설정한다.
        /// <summary>
        /// 버튼 타입별 이미지를 설정한다.
        /// </summary>
        /// <param name="_strButtonType">버튼 타입</param>
        private void GetGlyph(string _strButtonType)
        {
            try
            {
                var strImageFileName    = string.Empty;
                var isEnabled           = false;

                // A : 전체 권한
                isEnabled = this.BaseClass.RoleCode.Equals("A") == true ? true : false;

                switch (_strButtonType.ToUpper())
                {
                    #region >> 조회
                    case "SEARCH":
                        strImageFileName = "Recurrence_32x32";
                        isEnabled = true;
                        break;
                    #endregion

                    #region >> 엑셀 다운로드
                    case "EXCEL_DOWNLOAD":
                        strImageFileName = "ExportToXLSX_32x32";
                        isEnabled = true;
                        break;
                    #endregion

                    #region >> 저장
                    case "SAVE":    // 저장
                        strImageFileName = "SaveTheme_32x32";
                        break;
                    #endregion

                    #region >> 삭제
                    case "DELETE":
                        break;
                    #endregion

                    #region >> 엑셀 업로드
                    case "EXCEL_UPLOAD":    // 엑셀 업로드
                        break;
                    #endregion

                    #region >> 장애이력

                    #region >> 행 추가
                    case "ROW_ADD":
                        strImageFileName    = "ico_lt_tree_exp";
                        break;
                    #endregion

                    #region >> 행 삭제
                    case "ROW_DEL":
                        strImageFileName    = "ico_lt_tree_close";
                        break;
                    #endregion
                    default: break;
                }

                if (strImageFileName.Length == 0) { return; }

                if (_strButtonType.ToUpper().Equals("ROW_ADD") == true || _strButtonType.ToUpper().Equals("ROW_DEL") == true)
                {
                    var strImagePath        = $"pack://application:,,,/SMART.WCS.Resource;component/Image/{strImageFileName}.png";
                    //imgButton.Source        = new BitmapImage(new Uri(@strImagePath, UriKind.Absolute));
                }
                else
                {
                    // 추가 코딩
                }
            }
            catch { throw; }
        }
        #endregion

        #region > GetResourceText - 버튼 타입별 리소스 정보 (버튼명)를 가져온다.
        /// <summary>
        /// 버튼 타입별 리소스 정보 (버튼명)를 가져온다.
        /// </summary>
        /// <param name="_strButtonType">버튼 타입</param>
        /// <returns></returns>
        private string GetResourceText(string _strButtonType)
        {
            try
            {
                if (this.BaseInfo == null) { return string.Empty; }
                //if (this.BaseInfo.COUNTRY_CD.Length == 0) { return string.Empty; }
                if (BaseClass.CountryCode.Length == 0) { return string.Empty; }

                var strCountryCode  = string.Empty;

                if (this.BaseClass.CountryCode.Length == 0)
                {
                    var strCultureName  = CultureInfo.CurrentCulture.ToString();
                    strCountryCode      = strCultureName.Substring(3, 2);
                }
                else
                {
                    strCountryCode =    this.BaseClass.CountryCode;
                }

                return this.BaseClass.GetResourceValue(_strButtonType, strCountryCode);
            }
            catch { throw; }
        }
        #endregion

        #region > SetButtonWidth - 버튼명 길에 따라 버튼 사이즈 설정
        /// <summary>
        /// 버튼명 길에 따라 버튼 사이즈 설정
        /// </summary>
        /// <param name="_strButtonType">버튼 타입 (버튼 종류)</param>
        private void SetButtonWidth(string _strButtonType)
        {
            try
            {
                double dBoldPlusValue = 0.0;

                double dTwoDigitValue = 80.0
                    , dThreeDigitValue = 90.0
                    , dFourDigitValue = 100.0
                    , dFiveDigitValue = 110.0
                    , dSixDigitValue = 120.0
                    , dSevenDigitValue = 130.0
                    , dEightDigitValue = 140.0;

                if (this.IsBoldTypeYN == true)
                {
                    dBoldPlusValue = (double)this.g_strButtonText.Length;
                    this.FontWeight = FontWeights.Bold;
                }

                switch (this.g_strButtonText.Length)
                {
                    case 2:
                        this.Width = dTwoDigitValue + dBoldPlusValue;
                        break;
                    case 3:
                        this.Width = dThreeDigitValue + dBoldPlusValue;
                        break;
                    case 4:
                        this.Width = dFourDigitValue + dBoldPlusValue;
                        break;
                    case 5:
                        this.Width = dFiveDigitValue + dBoldPlusValue;
                        break;
                    case 6:
                        this.Width = dSixDigitValue + dBoldPlusValue;
                        break;
                    case 7:
                        this.Width = dSevenDigitValue + dBoldPlusValue;
                        break;
                    case 8:
                        this.Width = dEightDigitValue + dBoldPlusValue;
                        break;
                    default: break;
                }
            }
            catch { throw; }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        private static void ButtonTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as uSimpleButton;

            if (owner != null)
            {
                //owner.txtSimpleButton.Content  = owner.Text; 
                //owner.txtSimpleButton.Content = ((SMART.WCS.Control.uSimpleButton)d).Text;
            }
        }

        //private static void RoleCDPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{

        //}

        private static void IsBoldTypeYNPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var owner = d as uSimpleButton;

            //if ((bool)e.NewValue == false)
            //{
            //    owner.SetButtonWidth();
            //}
        }

        private static void IsIconVisibleYNPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void ButtonWidthLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as uSimpleButton;

            //if ((bool)e.NewValue == false)
            //{
            //    //owner.SetButtonWidth();
            //}
        }

        //private static void uSimpleButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    //var owner = sender as uImageButton;
        //    //owner.ControlEnable();

        //}
        #endregion
    }
}
