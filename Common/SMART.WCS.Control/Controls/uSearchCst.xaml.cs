using SMART.WCS.Common;
using SMART.WCS.Control.Ctrl;
using SMART.WCS.Control.Views;
//using SMART.WCS.Control.Views;
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
    /// 고객사 검색 코드파인더
    /// </summary>
    public partial class uSearchCst : UserControl
    {
        #region ▩ 전역변수
        BaseClass BaseClass = new BaseClass();
        #endregion

        #region Dependency 정의
        public static readonly DependencyProperty TextEditCodeProperty = DependencyProperty.Register("CodeCst", typeof(string), typeof(uSearchCst)
            , new PropertyMetadata(string.Empty, new PropertyChangedCallback(TextEditCodePropertyChanged)));

        public static readonly DependencyProperty TextEditNameProperty = DependencyProperty.Register("NameCst", typeof(string), typeof(uSearchCst)
            , new PropertyMetadata(string.Empty, new PropertyChangedCallback(TextEditNamePropertyChanged)));

        public static readonly RoutedEvent CodeChangeProperty = EventManager.RegisterRoutedEvent("CodeChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(uSearchCst));
        public static readonly RoutedEvent NameChangeProperty = EventManager.RegisterRoutedEvent("NameChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(uSearchCst));

        public static readonly DependencyProperty IsFocusedCstProperty = DependencyProperty.RegisterAttached("IsFocusedCst", typeof(bool), typeof(uSearchCst),
            new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        #endregion

        #region ▩ 생성자
        public uSearchCst()
        {
            InitializeComponent();
        }
        #endregion

        #region ▩ 속성
        public string CodeCst
        {
            get { return (string)GetValue(TextEditCodeProperty); }
            set { SetValue(TextEditCodeProperty, value); }
        }

        public string NameCst
        {
            get { return (string)GetValue(TextEditNameProperty); }
            set { SetValue(TextEditNameProperty, value); }
        }

        public bool IsFocusedCst
        {
            //get { return (bool)GetValue(IsFocusProperty); }
            //set { SetValue(IsFocusProperty, value); }

            set
            {
                if (value == true)
                {
                    this.txtCstCode.Focus();
                }
            }
        }


        public event EventHandler<DependencyPropertyChangedEventArgs> InputChanged;
        public event EventHandler<DependencyPropertyChangedEventArgs> InputChangedName;

        public event RoutedEventHandler CodeChange
        {
            add { AddHandler(CodeChangeProperty, value); }
            remove { RemoveHandler(CodeChangeProperty, value); }
        }

        public event RoutedEventHandler NameChange
        {
            add { AddHandler(NameChangeProperty, value); }
            remove { RemoveHandler(NameChangeProperty, value); }
        }
        #endregion

        #region ▩ 함수
        public override void OnApplyTemplate()
        {
            try
            {
                base.OnApplyTemplate();

                this.txtCstCode.Text = this.CodeCst;
                this.txtCstName.Text = this.NameCst;

                this.txtCstCode.KeyDown             += TxtCstCode_KeyDown;
                this.txtCstCode.PreviewKeyDown      += TxtCstCode_PreviewKeyDown;
            }
            catch { }
        }

        private void RaiseChangeEvent()
        {
            RoutedEventArgs newCodeEventArgs = new RoutedEventArgs(uSearchCst.CodeChangeProperty);
            RaiseEvent(newCodeEventArgs);

            RoutedEventArgs newNameEventArgs = new RoutedEventArgs(uSearchCst.NameChangeProperty);
            RaiseEvent(newNameEventArgs);
        }
        #endregion

        #region ▩ 이벤트
        #region imgSearch_PreviewMouseLeftButtonUp - 검색 버튼 클릭
        /// <summary>
        /// 검색 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SWCS101_01P frmChild = new SWCS101_01P(this.CodeCst);
                frmChild.SearchResult += frmChild_SearchResult;
                frmChild.ShowDialog();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region TxtCstCode_KeyDown - 코드 텍스트박스 KeyDown - Tab, Enter
        /// <summary>
        /// 코드 텍스트박스 KeyDown - Tab, Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtCstCode_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                case Key.Enter:

                    if (this.CodeCst.Length > 0)
                    {
                        using (SWCS101_01P frmChild = new SWCS101_01P(this.CodeCst))
                        {
                            frmChild.SearchResult += frmChild_SearchResult;
                            frmChild.ShowDialog();
                        }
                    }
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region TxtCstCode_PreviewKeyDown - 코드 텍스트박스 PreviewKeyDown - Delete, Back
        /// <summary>
        /// 코드 텍스트박스 PreviewKeyDown - Delete, Back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtCstCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (Keyboard.IsKeyDown(Key.Back) == true)
                {
                    this.txtCstName.Text = string.Empty;
                }
                else if (Keyboard.IsKeyDown(Key.Delete) == true)
                {
                    this.txtCstName.Text = string.Empty;
                }
                else
                {
                    if (this.txtCstName.Text.Trim().Length > 0)
                    {
                        this.txtCstName.Text = string.Empty;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region frmChild_CustomerSearchResult - 고객사 검색 결과 반영
        /// <summary>
        /// 고객사 검색 결과 반영
        /// </summary>
        /// <param name="_strCode"></param>
        /// <param name="_strName"></param>
        private void frmChild_SearchResult(string _strCode, string _strName)
        {
            try
            {
                if (_strCode.Length > 0)
                {
                    this.txtCstCode.Text = _strCode.ToUpper();
                    this.txtCstName.Text = _strName;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        protected void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RaiseChangeEvent();
        }

        private static void TextEditCodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as uSearchCst;

            if (owner != null)
            {
                //    owner.txtButtonName.Text = owner.Text;
                //    owner.DefineImageButton();

                if (owner.InputChanged != null)
                {
                    owner.InputChanged(owner, e);

                    //DependencyObject aaa = (DependencyObject)owner.Parent;

                    UIElement parentControl = (UIElement)owner.Parent;

                    var ctrlCst = parentControl.FindLogicalChildren<uSearchCst>();
                    if (ctrlCst.Count() > 0)
                    {
                        var ctrlCstValue = ctrlCst.First();
                        // 검색된 고객사 명이 공백이 되는 경우 (초기화 상태)
                        // 거래처, SKU, 바코드 검색 데이터를 초기화한다.
                        if (ctrlCstValue.NameCst.Length == 0)
                        {
                            //#region 거래처
                            //var ctrlBizptnr = parentControl.FindLogicalChildren<searchBizptnr>();
                            //if (ctrlBizptnr.Count() > 0)
                            //{
                            //    var ctrlBizptnrValue = ctrlBizptnr.First();
                            //    ctrlBizptnrValue.CodeBizptnr = string.Empty;
                            //    ctrlBizptnrValue.NameBizptnr = string.Empty;
                            //}
                            //#endregion

                            //#region SKU
                            //var ctrlSku = parentControl.FindLogicalChildren<searchSku>();
                            //if (ctrlSku.Count() > 0)
                            //{
                            //    var ctrlSkuValue = ctrlSku.First();
                            //    ctrlSkuValue.CodeSku = string.Empty;
                            //    ctrlSkuValue.NameSku = string.Empty;
                            //}
                            //#endregion

                            //#region Barcode
                            //var ctrlBarcode = parentControl.FindLogicalChildren<searchSkuBarcode>();
                            //if (ctrlBarcode.Count() > 0)
                            //{
                            //    var ctrlBarcodeValue = ctrlBarcode.First();
                            //    ctrlBarcodeValue.CodeSkuBarCode = string.Empty;
                            //    ctrlBarcodeValue.NameSkuBarCode = string.Empty;
                            //}
                            //#endregion
                        }
                    }
                }
            }
        }

        private static void TextEditNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as uSearchCst;


            if (owner != null)
            {
                //    owner.txtButtonName.Text = owner.Text;
                //    owner.DefineImageButton();

                if (owner.InputChangedName != null)
                {
                    owner.InputChangedName(owner, e);
                }
            }
        }

        private static void OnIsFocusedPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            //var uie = (UIElement)d;
            //if ((bool)e.NewValue)
            //{
            //    uie.Focus(); // Don't care about false values.
            //}
        }
    }
}
