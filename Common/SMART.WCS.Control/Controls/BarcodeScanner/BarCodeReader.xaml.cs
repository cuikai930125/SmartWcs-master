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

namespace SMART.WCS.Control
{
    /// <summary>
    /// BarCodeReader.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BarCodeReader : UserControl
    {
        #region ▩ 전역변수
        BaseClass BaseClass = new BaseClass();
        #endregion

        #region ▩ 생성자
        public BarCodeReader()
        {
            InitializeComponent();

        }
        #endregion

        #region ▩ 함수
        private void InitEvent()
        {
            try
            {
                this.Loaded += BarCodeReader_Loaded;
                this.Unloaded += BarCodeReader_Unloaded;
            }
            catch { throw; }
        }




        #endregion

        #region ▩ 이벤트
        /// <summary>
        /// 화면 로드 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarCodeReader_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }


        private void BarCodeReader_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion



    }
}
