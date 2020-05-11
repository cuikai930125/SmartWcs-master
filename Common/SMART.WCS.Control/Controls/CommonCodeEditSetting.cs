using DevExpress.Xpf.Editors.Settings;
using SMART.WCS.Common;
using SMART.WCS.Modules.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.Control
{
    public class CommonCodeEditSetting : ComboBoxEditSettings
    {
        BaseClass BaseClass = new BaseClass();

        #region 공통코드
        public static readonly DependencyProperty CommonCodeProperty = DependencyProperty.RegisterAttached("CommonCode", typeof(string), typeof(CommonCodeEditSetting), null);

        public string CommonCode
        {
            get { return (string)GetValue(CommonCodeProperty); }
            set { SetValue(CommonCodeProperty, null); }
        }
        #endregion

        #region Input 파라메터
        #region > 조건 1
        public static readonly DependencyProperty Attr1_InputParam_Property
            = DependencyProperty.RegisterAttached("Attr1_InputParam", typeof(string), typeof(CommonCodeEditSetting), null);

        public string Attr1_InputParam
        {
            get { return (string)GetValue(Attr1_InputParam_Property); }
            set { SetValue(Attr1_InputParam_Property, value); }
        }
        #endregion

        #region > 조건 2
        public static readonly DependencyProperty Attr2_InputParam_Property
            = DependencyProperty.RegisterAttached("Attr2_InputParam", typeof(string), typeof(CommonCodeEditSetting), null);

        public string Attr2_InputParam
        {
            get { return (string)GetValue(Attr2_InputParam_Property); }
            set { SetValue(Attr2_InputParam_Property, value); }
        }
        #endregion

        #region > 조건 3
        public static readonly DependencyProperty Attr3_InputParam_Property
            = DependencyProperty.RegisterAttached("Attr3_InputParam", typeof(string), typeof(CommonCodeEditSetting), null);

        public string Attr3_InputParam
        {
            get { return (string)GetValue(Attr3_InputParam_Property); }
            set { SetValue(Attr3_InputParam_Property, value); }
        }
        #endregion

        #region > 조건 4
        public static readonly DependencyProperty Attr4_InputParam_Property
            = DependencyProperty.RegisterAttached("Attr4_INputParam", typeof(string), typeof(CommonCodeEditSetting), null);

        public string Attr4_InputParam
        {
            get { return (string)GetValue(Attr4_InputParam_Property); }
            set { SetValue(Attr4_InputParam_Property, value); }
        }
        #endregion
        #endregion

        #region 콤보박스 첫번째 Row 적용 여부
        public static readonly DependencyProperty FirstRowEmptyProperty
            = DependencyProperty.RegisterAttached("FirstRowEmpty", typeof(bool), typeof(CommonCodeEditSetting), null);

        public bool FirstRowEmpty
        {
            get { return (bool)GetValue(FirstRowEmptyProperty); }
            set { SetValue(FirstRowEmptyProperty, value); }
        }
        #endregion

        //public static readonly DependencyProperty IsAllTypeCodeAddProperty = DependencyProperty.RegisterAttached(
        //                                               "IsAllTypeCodeAdd",
        //                                               typeof(bool),
        //                                               typeof(CommonCodeEditSetting), new PropertyMetadata(false));

        //#region 추가 코드
        //public static readonly DependencyProperty CallTypeProperty = DependencyProperty.RegisterAttached(
        //                                                      "CallType",
        //                                                      typeof(string),
        //                                                      typeof(CommonCodeEditSetting), null);
        //public static readonly DependencyProperty Attr1Property = DependencyProperty.RegisterAttached(
        //                                                      "Attr1",
        //                                                      typeof(string),
        //                                                      typeof(CommonCodeEditSetting), null);
        //public static readonly DependencyProperty Attr2Property = DependencyProperty.RegisterAttached(
        //                                                      "Attr2",
        //                                                      typeof(string),
        //                                                      typeof(CommonCodeEditSetting), null);
        //public static readonly DependencyProperty Attr3Property = DependencyProperty.RegisterAttached(
        //                                                      "Attr3",
        //                                                      typeof(string),
        //                                                      typeof(CommonCodeEditSetting), null);
        //public static readonly DependencyProperty Attr4Property = DependencyProperty.RegisterAttached(
        //                                                      "Attr4",
        //                                                      typeof(string),
        //                                                      typeof(CommonCodeEditSetting), null);

        //public string CallType
        //{
        //    get { return (string)GetValue(CallTypeProperty); }
        //    set { SetValue(CallTypeProperty, value); }
        //}

        //public string Attr1
        //{
        //    get { return (string)GetValue(Attr1Property); }
        //    set { SetValue(Attr1Property, value); }
        //}
        //public string Attr2
        //{
        //    get { return (string)GetValue(Attr2Property); }
        //    set { SetValue(Attr2Property, value); }
        //}
        //public string Attr3
        //{
        //    get { return (string)GetValue(Attr3Property); }
        //    set { SetValue(Attr3Property, value); }
        //}
        //public string Attr4
        //{
        //    get { return (string)GetValue(Attr4Property); }
        //    set { SetValue(Attr4Property, value); }
        //}
        //#endregion 추가코드


        //public bool IsAllTypeCodeAdd
        //{
        //    get { return (bool)GetValue(IsAllTypeCodeAddProperty); }
        //    set { SetValue(IsAllTypeCodeAddProperty, value); }

        //}
        public CommonCodeEditSetting()
        { }

        public override void EndInit()
        {
            base.EndInit();

            string strConvertComboBoxCommonCode     = string.Empty;
            this.ValueMember                        = "CODE";
            this.DisplayMember                      = "NAME";

            if (this.Attr1_InputParam == null) { this.Attr1_InputParam = string.Empty; }
            if (this.Attr2_InputParam == null) { this.Attr2_InputParam = string.Empty; }
            if (this.Attr3_InputParam == null) { this.Attr3_InputParam = string.Empty; }
            if (this.Attr4_InputParam == null) { this.Attr4_InputParam = string.Empty; }

            /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
            * XAML단에서는 공통콤보박스를 설정하는 경우
            * InitializeComponent()를 수행하면서 콤보박스가 설정되기 때문에 Code Behind단에서
            * 콤보박스 Parameter를 설정할 수 있는 방법이 없다.
            * 때문에 권한코드 공통 콤보박스가 그리드에 설정되는 경우 코드를 변경해서 처리해야함
            ▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/

            switch (CommonCode)
            {
                case "ROLE_CD_GRID":
                    strConvertComboBoxCommonCode        = "ROLE_CD";
                    this.Attr1_InputParam               = this.BaseClass.CenterCD;
                    break;
            }


            if (this.Attr1_InputParam.Length > 0 || this.Attr2_InputParam.Length > 0 || this.Attr3_InputParam.Length > 0 || this.Attr4_InputParam.Length > 0)
            {
                string[] arrInputParam = { this.Attr1_InputParam, this.Attr2_InputParam, this.Attr3_InputParam, this.Attr4_InputParam };

                this.NullText = "SELECT";
                // 파라메터를 이용해서 코드 조회

                if (strConvertComboBoxCommonCode.Length == 0)
                {
                    this.ItemsSource = new CommonCodeExtensions(CommonCode, arrInputParam, FirstRowEmpty);
                }
                else
                {
                    this.ItemsSource = new CommonCodeExtensions(strConvertComboBoxCommonCode, arrInputParam, FirstRowEmpty);
                }
            }
            else
            {
                this.NullText = "SELECT";
                // 공통코드만으로 코드 조회
                this.ItemsSource = new CommonCodeExtensions(CommonCode);
            }
        }
    }
}
