using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using SMART.WCS.Common.Extensions;
using SMART.WCS.Control;
using SMART.WCS.Control.Ctrl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SMART.WCS.Modules.Behaviors
{
    public class ControlBaseBehavior : Behavior<ContentControl>
    {
        #region ▩ 전역변수
        /// <summary>
        /// 공통 Class를 이용하기 위한 BaseClass 선언
        /// </summary>
        private SMART.WCS.Common.BaseClass BaseClass = new SMART.WCS.Common.BaseClass();

        /// <summary>
        ///  GridTemplete
        /// </summary>
        private Grid GridTemplete;

        private BaseApp g_App = null;

        /// <summary>
        /// 메뉴 ID
        /// </summary>
        private string g_strMenuID = string.Empty;

        #endregion

        #region ▩ 생성자
        public ControlBaseBehavior()
        {
        }
        #endregion

        #region ▩ 함수
        #region override 함수
        protected override void OnAttached()
        {
            base.OnAttached();

            this.g_App = Application.Current as BaseApp;
     
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.Initialized += AssociatedObject_Initialized;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject is System.Windows.Controls.Control)
            {
            }

            if (this.AssociatedObject is DependencyObject)
            {
                var _contentType = this.AssociatedObject.GetType();
                var depedencyObject = this.AssociatedObject as DependencyObject;

                foreach (FieldInfo field in _contentType.GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    if (field.FieldType == typeof(DependencyProperty))
                    {
                        var dp = field.GetValue(_contentType) as DependencyProperty;
                        ResetValue(depedencyObject, dp);
                    }
                } 
            }
        }

        public void ResetValue( DependencyObject _this, DependencyProperty dp)
        {
            var md = dp.GetMetadata(_this.GetType());
            if (_this.GetValue(dp).Equals(md.DefaultValue))
            {
                var args = new DependencyPropertyChangedEventArgs(dp, DependencyProperty.UnsetValue, md.DefaultValue);
                md.PropertyChangedCallback?.Invoke(_this, args);
            }
            else
            {
                _this.ClearValue(dp);
            }
        }

        #endregion

        #region InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            try
            {
          
                #region 라벨 조회 및 적용
                this.InitLabel();
                #endregion

                InitGridControlHeight();
            }
            catch { throw; }
        }
        #endregion

        #region InitLabel - 라벨 조회 및 적용
        /// <summary>
        /// 라벨 조회 및 적용
        /// </summary>
        private void InitLabel()
        {
            try
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("**** 언어 추가 필요 ***** ");
#endif

                string strRequired = string.Empty;     //20180914 필수항목(*) 붙이기 위해 이용하는 변수

                #region 일반 컨트롤 라벨명 
                var controls = from ctrl in GridTemplete.FindLogicalChildren<UIElement>()
                                             .Where(f => f.GetValue(CommonProperties.LabelCdProperty) != null)
                               select new
                               {
                                   Control = ctrl,
                                   LabelCD = (string)ctrl.GetValue(CommonProperties.LabelCdProperty)
                               };


                #endregion

                foreach (var item in controls)
                {
                    string _code = "";
                    string _format = "{0}";

                    var _array = item.LabelCD.Split(',');

                    _code = _array[0];

                    //if (_array.Count() > 1)
                    //{
                    //    _format = _array[1];
                    //}

                    ResourceManager rm = new ResourceManager("SMART.WCS.Resource.Language.LanguageResource", typeof(SMART.WCS.Resource.Language.LanguageResource).Assembly);
                    CultureInfo cultureInfo = SMART.WCS.Control.Utility.HelperClass.GetCultureInfo();

                    var strLabel = rm.GetString(_code, cultureInfo);

                    item.Control.SetValue(CommonProperties.LabelDescProperty, string.Format(_format, strLabel));
                    //20180913 hj.kim 필수항목에 *붙이는 처리 end

#if DEBUG
                    System.Diagnostics.Debug.WriteLine($" {item.LabelCD} - Control Type : {item.Control.GetType().Name}");
#endif           
                }

                #region GridControl의 header를 가져와서 landesc를 넣는다.
                var _gridcontrols = GridTemplete.FindLogicalChildren<DevExpress.Xpf.Grid.GridControl>();

                foreach (var gridcontorl in _gridcontrols)
                {
                    foreach (var band in gridcontorl.Bands)
                    {
                        ResourceManager rm = new ResourceManager("SMART.WCS.Resource.Language.LanguageResource", typeof(SMART.WCS.Resource.Language.LanguageResource).Assembly);
                        CultureInfo cultureInfo = SMART.WCS.Control.Utility.HelperClass.GetCultureInfo();

                        var strLabel = rm.GetString(band.Header.ToWhiteSpaceOrString(), cultureInfo);
                        band.Header = strLabel;
                    }

                    foreach (var column in gridcontorl.FindGridControlColumn())
                    {
                        if (column.HeaderTemplate == null)
                        {
                            if (gridcontorl.View.AllowEditing)
                            {
                                if (column.AllowEditing == DevExpress.Utils.DefaultBoolean.Default)
                                {
                                    if (!column.ReadOnly)
                                    {
                                        column.HeaderStyle = this.AssociatedObject.FindResource("DefaultColumnStyle") as Style;
                                        //column.HeaderTemplate = this.AssociatedObject.FindResource("AllowEditingMarkTemplate") as DataTemplate;
                                    }
                                    else if (column.ReadOnly)
                                    {
                                        column.HeaderStyle = this.AssociatedObject.FindResource("ReadOnlyColumnStyle") as Style;
                                        column.HeaderTemplate = this.AssociatedObject.FindResource("DefaultHeaderTemplate") as DataTemplate;
                                    }
                                }
                                else
                                {
                                    if (column.AllowEditing == DevExpress.Utils.DefaultBoolean.True)
                                    {
                                        column.HeaderStyle = this.AssociatedObject.FindResource("DefaultColumnStyle") as Style;
                                        //column.HeaderTemplate = this.AssociatedObject.FindResource("AllowEditingMarkTemplate") as DataTemplate;
                                    }
                                    else if (column.AllowEditing == DevExpress.Utils.DefaultBoolean.False)
                                    {
                                        column.HeaderStyle = this.AssociatedObject.FindResource("ReadOnlyColumnStyle") as Style;
                                        column.HeaderTemplate = this.AssociatedObject.FindResource("DefaultHeaderTemplate") as DataTemplate;
                                    }
                                }
                            }
                            else
                            {
                                column.HeaderStyle = this.AssociatedObject.FindResource("ReadOnlyColumnStyle") as Style;
                                column.HeaderTemplate = this.AssociatedObject.FindResource("DefaultHeaderTemplate") as DataTemplate;

                            }
                        }

                        ResourceManager rm = new ResourceManager("SMART.WCS.Resource.Language.LanguageResource", typeof(SMART.WCS.Resource.Language.LanguageResource).Assembly);
                        CultureInfo cultureInfo = SMART.WCS.Control.Utility.HelperClass.GetCultureInfo();

                        var strLabel = rm.GetString(column.Header.ToWhiteSpaceOrString(), cultureInfo);
                        if (strLabel == null) { strLabel = string.Empty; }
                        column.Header = strLabel;
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($" {column.Header.ToWhiteSpaceOrString()} - GRID");
#endif
                    }
                }
                #endregion

                #region TreeListControl의 Header를 가져와서 다국어를 처리한다
                var treeListControl = GridTemplete.FindLogicalChildren<DevExpress.Xpf.Grid.TreeListControl>();
                
                foreach (var gridcontorl in treeListControl)
                {
                    foreach (var column in gridcontorl.Columns)
                    {
                        if (column.HeaderTemplate == null)
                        {
                            if (gridcontorl.View.AllowEditing)
                            {
                                if (column.AllowEditing == DevExpress.Utils.DefaultBoolean.Default)
                                {
                                    if (!column.ReadOnly)
                                    {
                                        column.HeaderStyle = this.AssociatedObject.FindResource("DefaultColumnStyle") as Style;
                                        //column.HeaderTemplate = this.AssociatedObject.FindResource("AllowEditingMarkTemplate") as DataTemplate;
                                    }
                                    else if (column.ReadOnly)
                                    {
                                        column.HeaderStyle = this.AssociatedObject.FindResource("ReadOnlyColumnStyle") as Style;
                                        column.HeaderTemplate = this.AssociatedObject.FindResource("DefaultHeaderTemplate") as DataTemplate;
                                    }
                                }
                                else
                                {
                                    if (column.AllowEditing == DevExpress.Utils.DefaultBoolean.True)
                                    {
                                        column.HeaderStyle = this.AssociatedObject.FindResource("DefaultColumnStyle") as Style;
                                        //column.HeaderTemplate = this.AssociatedObject.FindResource("AllowEditingMarkTemplate") as DataTemplate;
                                    }
                                    else if (column.AllowEditing == DevExpress.Utils.DefaultBoolean.False)
                                    {
                                        column.HeaderStyle = this.AssociatedObject.FindResource("ReadOnlyColumnStyle") as Style;
                                        column.HeaderTemplate = this.AssociatedObject.FindResource("DefaultHeaderTemplate") as DataTemplate;
                                    }
                                }
                            }
                            else
                            {
                                column.HeaderStyle = this.AssociatedObject.FindResource("ReadOnlyColumnStyle") as Style;
                                column.HeaderTemplate = this.AssociatedObject.FindResource("DefaultHeaderTemplate") as DataTemplate;

                            }
                        }

                        ResourceManager rm = new ResourceManager("SMART.WCS.Resource.Language.LanguageResource", typeof(SMART.WCS.Resource.Language.LanguageResource).Assembly);
                        CultureInfo cultureInfo = SMART.WCS.Control.Utility.HelperClass.GetCultureInfo();

                        var strLabel = rm.GetString(column.Header.ToWhiteSpaceOrString(), cultureInfo);
                        if (strLabel == null) { strLabel = string.Empty; }
                        column.Header = strLabel;
                    }
                }

                #endregion
            }
            catch { throw; }
        }
        #endregion

        private void InitGridControlHeight()
        {
            var _gridcontrols = GridTemplete.FindLogicalChildren<DevExpress.Xpf.Grid.GridControl>();
            int _inx = 0;
            foreach (var gridcontrol in _gridcontrols)
            {
                if (gridcontrol.Parent is Grid)
                {
                    gridcontrol.ShowBorder = true;

                    // string _rowName = this.AssociatedObject.GetType().Name + "GridRow" + _inx.ToString();
                    var _gridcontrolName = this.AssociatedObject.GetType().Name + "gridcontrol" + _inx.ToString();
                    var _rowIndex = (int)gridcontrol.GetValue(Grid.RowProperty);

                    if ((gridcontrol.Parent as Grid).RowDefinitions.Count() > 0)
                    {
                        RowDefinition _rowDefinition = null;

                        if ((gridcontrol.Parent as Grid).RowDefinitions.Count() < _rowIndex)
                        {
                            throw new Exception("Gridcontrol에 지정된 Grid.Row가 Gridcontrol이 속한 GRID의 최대 Row보다 큽니다.");
                        }
                        else
                        {
                            _rowDefinition = (gridcontrol.Parent as Grid).RowDefinitions[_rowIndex];
                        }

                        if (_rowDefinition != null)
                        {
                            if (_rowDefinition.Height.GridUnitType == GridUnitType.Star)
                            {
                                _rowDefinition.Height = new GridLength(_rowDefinition.Height.Value, GridUnitType.Star);
                            }
                        }
                    }

                    if (gridcontrol.Name == null)
                    {
                        gridcontrol.Name = _gridcontrolName;
                        this.AssociatedObject.RegisterName(_gridcontrolName, gridcontrol);
                    }


                    (gridcontrol.Parent as Grid).SizeChanged += (snd, evt) =>
                    {
                        double _actualHeight = double.NaN;

                        if ((snd as Grid).RowDefinitions.Count() > 0)
                        {
                            var _row = (snd as Grid).RowDefinitions[_rowIndex];
                            _actualHeight = _row.ActualHeight;
                        }
                        else
                        {
                            _actualHeight = (snd as Grid).ActualHeight;
                        }

                        _actualHeight -= gridcontrol.Margin.Top + gridcontrol.Margin.Bottom;

                        gridcontrol.Height = _actualHeight ;
                        gridcontrol.MinHeight = _actualHeight;
                        gridcontrol.MaxHeight = _actualHeight;
                    };

                    _inx++;
                }
            }
        }

        #region InitButtonEnabled - 버튼을 Enabled 속성을 초기화한다.
        /// <summary>
        /// 버튼을 Enabled 속성을 초기화한다.
        /// </summary>
        /// <param name="_isEnabled">활성화 여부</param>
        private void InitButtonEnabled(bool _isEnabled)
        {
            try
            {
                //var dtButtonInfo = this.GetControlInfo(CJFC.Common.EnumClass.ControlType.BUTTON);

                //foreach (DataRow drButtonEnabled in dtButtonInfo.Rows)
                //{
                //    // 데이터 조회전에 조회 버튼을 제외한 모든 버튼을 비활성화한다.
                //    switch (drButtonEnabled["LAN_CD"].ToString().ToUpper())
                //    {
                //        case "SAVE":
                //            this.btnSave.IsEnabled = _isEnabled;
                //            break;
                //        case "DELETE":
                //            this.btnDelete.IsEnabled = _isEnabled;
                //            break;
                //        case "NEW":
                //            this.btnNew.IsEnabled = _isEnabled;
                //            break;
                //        case "PRINT":
                //            this.btnPrint.IsEnabled = _isEnabled;
                //            break;
                //        case "DOWNLOAD":
                //            this.btnDownload.IsEnabled = _isEnabled;
                //            break;
                //        case "UPLOAD":
                //            this.btnUpload.IsEnabled = _isEnabled;
                //            break;
                //    }
                //}
            }
            catch { throw; }
        }
        #endregion

        #region GetGridComboData - 그리드 내 콤보박스 설정을 위한 공통코드 데이터를 조회한다.
        /// <summary>
        /// 그리드 내 콤보박스 설정을 위한 공통코드 데이터를 조회한다.
        /// </summary>
        /// <param name="_strColumnCode"></param>
        private Dictionary<object, object> GetGridComboData(
                    string _strColumnCode
                , string _strAttributeFirst
                , string _strAttributeSecond
                , string _strAttributeThird
            )
        {
            try
            {
                #region Input 파라메터 설정
                Dictionary<object, object> dicGridCommboParam = new Dictionary<object, object>();

                dicGridCommboParam.Add("P_TYPE_CD", _strColumnCode);        // 공통코드
                dicGridCommboParam.Add("P_ATTR_ONE", _strAttributeFirst);    // 조건 1
                dicGridCommboParam.Add("P_ATTR_TWO", _strAttributeSecond);   // 조건 2
                dicGridCommboParam.Add("P_ATTR_THREE", _strAttributeThird);   // 조건 3
                #endregion

                return dicGridCommboParam;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        private void AssociatedObject_Initialized(object sender, EventArgs e)
        {
            if (this.AssociatedObject.Content != null)
            {
                var _GridControls = this.AssociatedObject.FindLogicalChildren<DevExpress.Xpf.Grid.GridControl>();
                //var _buttonControls = this.AssociatedObject.FindLogicalChildren<uImageButton>();
                var buttonControls      = this.AssociatedObject.FindLogicalChildren<SimpleButton>();
                bool _IsEnable = false;

                // CHOO - 2019-09-03 수정 후 주석제거해야함
                ////if (this.g_App.BASE_INFO.ROLE_CD != "A")
                ////{
                    foreach (var item in _GridControls)
                    {
                        item.View.AllowEditing = _IsEnable;

                        item.View.PropertyChanged += (snd, evt) =>
                        {
                            if (evt.PropertyName == "AllowEditing")
                            {
                                item.View.AllowEditing = _IsEnable;
                            }
                        };
                    }

                    // CHOO
                    // 2019-09-17
                    // 버튼 Enabled 속성 정의 -> 여기서 처리할건지 확인필요
                    //foreach (var item in buttonControls)
                    //{
                    //    if (item.Content.ToString().ToLower() != ("SEARCH").ToLower() || item.Content.ToString().ToLower() != ("EXCEL_DOWNLOAD").ToLower())
                    //    {
                    //        item.IsEnabled = _IsEnable;

                    //        item.IsEnabledChanged += (snd, evt) =>
                    //        {
                    //            item.IsEnabled = _IsEnable;
                    //        };
                    //    }
                    //}
                ////}

                if (this.AssociatedObject.Content.GetType() == typeof(Grid))
                {
                    GridTemplete = this.AssociatedObject.Content as Grid;

                    if (FindControl.FindLogicalChildren<DevExpress.Xpf.Core.WaitIndicator>(GridTemplete).Count() == 0)
                    {
                        var _name = "_" + this.AssociatedObject.Tag.ToWhiteSpaceOrString() + Guid.NewGuid().ToString().Replace("-", "");

                        Border _border = new Border();
                        DevExpress.Xpf.Core.WaitIndicator _Indicator = new DevExpress.Xpf.Core.WaitIndicator();

                        _Indicator.SetBinding(DevExpress.Xpf.Core.WaitIndicator.NameProperty, _name);
                        _Indicator.Name = _name;
                        _border.Child = _Indicator;

                        Binding _binding = new Binding("DeferedVisibility");
                        _binding.Converter = new BooleanToVisibilityConverter();
                        _binding.Source = _Indicator;
                        _border.SetBinding(Border.VisibilityProperty, _binding);

                        _border.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0x0C, 0x00, 0x00, 0x00));

                        var _columnCount = GridTemplete.ColumnDefinitions.Count;
                        var _rowCount = GridTemplete.RowDefinitions.Count;

                        _border.SetValue(Grid.ColumnSpanProperty, (_columnCount > 0) ? _columnCount : 1);
                        _border.SetValue(Grid.RowSpanProperty, (_rowCount > 0) ? _rowCount : 1);

                        GridTemplete.Children.Add(_border);
                    }
                }
                else
                {
                    //throw new Exception("usercontrol의 상단은 Grid입니다.");
                }

                InitControl();
            }
        }

        #endregion
    }
}
