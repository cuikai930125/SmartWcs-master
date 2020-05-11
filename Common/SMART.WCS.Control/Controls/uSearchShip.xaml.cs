using SMART.WCS.Common;
using SMART.WCS.Control.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.Control.Controls
{
    /// <summary>
    /// 거래처 검색용 코드파인더
    /// </summary>
    public partial class uSearchShip : UserControl
    {
        #region ▩ 전역변수
        BaseClass BaseClass = new BaseClass();
        #endregion

        #region ▩ DependencyProperty 선언
        #region > 거래처 코드
        public static readonly DependencyProperty TextEditCodeProperty = DependencyProperty.Register("CodeShip", typeof(string), typeof(uSearchShip), new PropertyMetadata(string.Empty));
        public string CodeShip
        {
            get { return (string)GetValue(TextEditCodeProperty); }
            set { SetValue(TextEditCodeProperty, value); }
        }
        #endregion

        #region > 거래처명
        public static readonly DependencyProperty TextEditNameProperty = DependencyProperty.Register("NameShip", typeof(string), typeof(uSearchShip), new PropertyMetadata(string.Empty));
        public string NameShip
        {
            get { return (string)GetValue(TextEditCodeProperty); }
            set { SetValue(TextEditCodeProperty, value); }
        }
        #endregion

        #region > 거래처 조회를 위해 선택된 고객사 코드
        public static readonly DependencyProperty TextEditCodeHiddenproperty = DependencyProperty.Register("CodeCstHiddenShip", typeof(string), typeof(uSearchShip), new PropertyMetadata(string.Empty));
        public string CodeCstHiddenShip
        {
            get { return (string)GetValue(TextEditCodeProperty); }
            set { SetValue(TextEditCodeProperty, value); }
        }
        #endregion

        #region > 거래처 조회를 위해 선택된 고객사명
        public static readonly DependencyProperty TextEditNameHiddenproperty = DependencyProperty.Register("NameCstHiddenShip", typeof(string), typeof(uSearchShip), new PropertyMetadata(string.Empty));
        public string NameCstHiddenShip
        {
            get { return (string)GetValue(TextEditCodeProperty); }
            set { SetValue(TextEditCodeProperty, value); }
        }
        #endregion

        #region > 거래처 코드 포커스
        public static readonly DependencyProperty IsFocusedBizptnrProperty = DependencyProperty.RegisterAttached("IsFocusedShip", typeof(bool), typeof(uSearchShip),
            new UIPropertyMetadata(false, null));

        public bool IsFocusedShip
        {
            set
            {
                if (value == true)
                {
                    this.txtShipCode.Focus();
                }
            }
        }
        #endregion
        #endregion

        #region ▩ 생성자
        public uSearchShip()
        {
            InitializeComponent();

            // 이벤트 초기화
            this.InitEvent();
        }
        #endregion

        #region ▩ 함수
        public override void OnApplyTemplate()
        {
            try
            {
                base.OnApplyTemplate();

                this.txtShipCode.Text = this.CodeShip;
                this.txtShipName.Text = this.NameShip;

                this.txtShipCode.KeyDown += TxtShipCode_KeyDown;
                this.txtShipCode.PreviewKeyDown += TxtShipCode_PreviewKeyDown;
            }
            catch { }
        }

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            this.imgSearch.PreviewMouseLeftButtonUp += ImgSearch_PreviewMouseLeftButtonUp;
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 검색 이미지 버튼 클릭 이벤트
        /// <summary>
        /// 검색 이미지 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (SWCS102_01P frmChild = new SWCS102_01P(this.CodeShip, this.CodeCstHiddenShip, this.NameCstHiddenShip))
                {
                    frmChild.SearchResult += FrmChild_SearchResult;
                    frmChild.ShowDialog();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region > 코드 텍스트박스 KeyDown 이벤트 - Tab, Enter
        /// <summary>
        /// 코드 텍스트박스 KeyDown 이벤트 - Tab, Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtShipCode_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                case Key.Enter:
                    using (SWCS102_01P frmChild = new SWCS102_01P(this.CodeShip, this.CodeCstHiddenShip, this.NameCstHiddenShip))
                    {
                        frmChild.SearchResult += FrmChild_SearchResult;
                        frmChild.ShowDialog();
                    }
                    break;
                default: break;
            }
        }
        #endregion

        #region > 코드 텍스트박스 PreviewKeyDown - Delete, BackSpace
        /// <summary>
        /// 코드 텍스트박스 PreviewKeyDown - Delete, BackSpace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtShipCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (Keyboard.IsKeyDown(Key.Back) == true || Keyboard.IsKeyDown(Key.Delete) == true)
                {
                    this.txtShipName.Text   = string.Empty;
                }
            }
            catch { throw; }
        }
        #endregion

        #region > 거래처 검색팝업 결과 수신 Delegate
        /// <summary>
        /// 거래처 검색팝업 결과 수신 Delegate
        /// </summary>
        /// <param name="_strCode">거래처 코드</param>
        /// <param name="_strName">거래처명</param>
        /// <param name="_strCstCode">고객사 코드</param>
        /// <param name="_strCstName">고객사명</param>
        private void FrmChild_SearchResult(string _strCode, string _strName, string _strCstCode, string _strCstName)
        {
            try
            {
                if (_strCode.Length > 0)
                {
                    this.txtShipCode.Text       = _strCode.ToUpper();       // 거래처 코드
                    this.txtShipName.Text       = _strName;                 // 거래처명
                    this.txtCstCode.Text        = _strCstCode;              // 고객사 코드
                    this.txtCstName.Text        = _strCstName;              // 고객사명
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion
    }
}
