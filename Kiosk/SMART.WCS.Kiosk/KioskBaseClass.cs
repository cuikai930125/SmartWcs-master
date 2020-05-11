using SMART.WCS.Common_Etc.Data;
using SMART.WCS.Common_Etc.Data.DataMembers;
using System.Windows.Controls;

namespace SMART.WCS.Kiosk
{
    public class KioskBaseClass
    {
        public static string ComboBoxSelectedKeyValue(ComboBox _ctrlComboBox)
        {
            try
            {
                return ((ComboBoxInfo)_ctrlComboBox.SelectedItem).CODE;
                
            }
            catch { throw; }
        }
    }
}
