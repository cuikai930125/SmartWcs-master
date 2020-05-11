using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Modules.Extensions
{
    public static class EditBaseExtensions
    {
        public static void SetError(this DevExpress.Xpf.Editors.BaseEdit edit,string Error, DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorType)
        {
            DevExpress.Xpf.Editors.Helpers.BaseEditHelper.SetValidationError(
                  edit, new DevExpress.Xpf.Editors.Validation.BaseValidationError(Error, null, ErrorType));
            edit.ShowError = true;
            edit.ShowErrorToolTip = true;
            edit.Focus();
        }
    }
}
