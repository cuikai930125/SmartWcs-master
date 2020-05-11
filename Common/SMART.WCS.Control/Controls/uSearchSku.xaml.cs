using SMART.WCS.Common;
using System;
using System.Collections.Generic;
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

namespace SMART.WCS.Control.Controls
{
    /// <summary>
    /// SKU 검색용 코드파인더
    /// </summary>
    public partial class uSearchSku : UserControl
    {
        #region ▩ 전역변수
        BaseClass BaseClass = new BaseClass();
        #endregion

        #region ▩ DependencyProperty 선언
        #region > SKU 코드
        public static readonly DependencyProperty CodeSkuProperty = DependencyProperty.Register("CodeSku", typeof(string), typeof(uSearchSku), new PropertyMetadata(string.Empty));
        public string CodeShip
        {
            get { return (string)GetValue(CodeSkuProperty); }
            set { SetValue(CodeSkuProperty, value); }
        }
        #endregion

        #region > SKU명
        public static readonly DependencyProperty NameSkuProperty = DependencyProperty.Register("NameSku", typeof(string), typeof(uSearchSku), new PropertyMetadata(string.Empty));
        public string NameShip
        {
            get { return (string)GetValue(NameSkuProperty); }
            set { SetValue(NameSkuProperty, value); }
        }
        #endregion

        #region > 거래처 조회를 위해 선택된 고객사 코드
        public static readonly DependencyProperty CodeCstHiddenSkuProperty = DependencyProperty.Register("CodeCstHiddenSku", typeof(string), typeof(uSearchSku), new PropertyMetadata(string.Empty));
        public string CodeCstHiddenShip
        {
            get { return (string)GetValue(CodeCstHiddenSkuProperty); }
            set { SetValue(CodeCstHiddenSkuProperty, value); }
        }
        #endregion

        #region > 거래처 조회를 위해 선택된 고객사명
        public static readonly DependencyProperty NameCstHiddenSkuProperty = DependencyProperty.Register("NameCstHiddenSku", typeof(string), typeof(uSearchSku), new PropertyMetadata(string.Empty));
        public string NameCstHiddenShip
        {
            get { return (string)GetValue(NameCstHiddenSkuProperty); }
            set { SetValue(NameCstHiddenSkuProperty, value); }
        }
        #endregion

        #region > 거래처 코드 포커스
        public static readonly DependencyProperty IsFocusedSkuProperty = DependencyProperty.RegisterAttached("IsFocusedSku", typeof(bool), typeof(uSearchSku),
            new UIPropertyMetadata(false, null));

        public bool IsFocusedSku
        {
            set
            {
                if (value == true)
                {
                    this.txtSkuCode.Focus();
                }
            }
        }
        #endregion
        #endregion

        #region ▩ 생성자
        public uSearchSku()
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

                this.txtSkuCode.Text = this.CodeShip;
                this.txtSkuName.Text = this.NameShip;

                this.txtSkuCode.KeyDown += TxtSkuCode_KeyDown;
                this.txtSkuCode.PreviewKeyDown += TxtSkuCode_PreviewKeyDown;
            }
            catch { }
        }

        #region > 초기화
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
                //using (SWCS102_01P frmChild = new SWCS102_01P(this.CodeShip, this.CodeCstHiddenShip, this.NameCstHiddenShip))
                //{
                //    frmChild.SearchResult += FrmChild_SearchResult;
                //    frmChild.ShowDialog();
                //}
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
        private void TxtSkuCode_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                case Key.Enter:
                    //using (SWCS102_01P frmChild = new SWCS102_01P(this.CodeShip, this.CodeCstHiddenShip, this.NameCstHiddenShip))
                    //{
                    //    frmChild.SearchResult += FrmChild_SearchResult;
                    //    frmChild.ShowDialog();
                    //}
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
        private void TxtSkuCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (Keyboard.IsKeyDown(Key.Back) == true || Keyboard.IsKeyDown(Key.Delete) == true)
                {
                    this.txtSkuName.Text   = string.Empty;
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
                    this.txtSkuCode.Text        = _strCode.ToUpper();       // 거래처 코드
                    this.txtSkuName.Text        = _strName;                 // 거래처명
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
