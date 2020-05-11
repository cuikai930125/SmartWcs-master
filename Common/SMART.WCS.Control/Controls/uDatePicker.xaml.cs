using DevExpress.Xpf.Editors;
using SMART.WCS.Common;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// uDatePicker.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class uDatePicker : DateEdit
    {
        #region ▩ 전역변수
        BaseClass BaseClass = new BaseClass();
        #endregion

        #region
        public uDatePicker()
        {
            InitializeComponent();
        }
        #endregion

        public uDatePicker(int iCtrlOption)
        {
            InitializeComponent();

            //DataTable dtRtnValue = Utility.HelperClass.GetDate_INIT_INQ();
            //if (dtRtnValue.Rows.Count > 0)
            //{
            //    //     v_fr_ymd
            //    //, v_fr_hm
            //    //, v_to_ymd
            //    //, v_to_hm

               




            //}


            //switch (iCtrlOption)
            //{
            //    case 1:
            //        break;
            //    case 2:
            //        break;
            //}
        }
    }
}
