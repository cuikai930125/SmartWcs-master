using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
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

namespace SMART.WCS.Main
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        BaseClass BaseClass = new BaseClass();

        public UserControl1()
        {
            InitializeComponent();
        }

        private void BtnAAA_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //var strEncryptValue = string.Empty;
                //strEncryptValue     = BaseClass.EncryptAES256("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle11g.ctfe9ihnllwc.ap-northeast-2.rds.amazonaws.com)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));USER ID=smartwcs;Password=smartwcs1234;Persist Security Info=True;");

                var strMainDataBaseName = this.BaseClass.GetAppSettings("MainDatabase");
                this.BaseClass.MainDatabase = strMainDataBaseName;

                var strProcedureName = "PK_COM_MAIN.SP_MENU_LIST";
                Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
                string[] arrOutputParam = { "O_MENU_LIST", "O_RSLT" };

                dicInputParam.Add("P_CNTR_CD", "KY");
                dicInputParam.Add("P_USER_ID", "CSH");

                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    var dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }
            }
            catch { throw; }
        }
    }
}
