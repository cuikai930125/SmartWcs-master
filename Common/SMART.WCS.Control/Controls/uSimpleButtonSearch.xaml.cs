using SMART.WCS.Common;
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

namespace SMART.WCS.Control.Controls
{
    /// <summary>
    /// uSimpleButtonSearch.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class uSimpleButtonSearch : UserControl
    {
        #region ▩ 매개변수 (Dependency Property)
        /// <summary>
        /// Base Class
        /// </summary>
        BaseClass BaseClass = new BaseClass();
        #endregion

        #region ▩ 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public uSimpleButtonSearch()
        {
            InitializeComponent();
        }
        #endregion

        #region ▩ Override
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 버튼 속성을 정의한다.
            this.DefineSimpleButton();
        }
        #endregion

        #region ▩ 함수
        #region > 버튼 속성 정의 (텍스트, 이미지, 커서, 사이즈)
        /// <summary>
        /// 버튼 속성 정의 (텍스트, 이미지, 커서, 사이즈)
        /// </summary>
        /// <param name="_strButtonType">버튼 타입</param>
        private void DefineSimpleButton()
        {
            try
            {
                this.btnCommonSimpleButton.Content      = this.BaseClass.GetResourceValue("SEARCH");
                this.btnCommonSimpleButton.IsEnabled    = true;
                this.btnCommonSimpleButton.Width        = 80;
                this.btnCommonSimpleButton.Height       = 32;
                this.btnCommonSimpleButton.FontWeight   = FontWeights.Bold;
                this.btnCommonSimpleButton.Cursor       = Cursors.Hand;
            }
            catch { throw; }
        }
        #endregion
        #endregion
    }
}
