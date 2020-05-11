using SMART.WCS.Common_Etc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common_Etc.Data.DataMembers
{
    public class ComboBoxInfo : PropertyNotifyExtensions
    {
        #region CODE - 콤보박스 코드
        private string _CODE;
        public string CODE
        {
            get { return this._CODE; }
            set
            {
                if (this._CODE != value)
                {
                    this._CODE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region NAME - 콤보박스 명
        private string _NAME;
        public string NAME
        {
            get { return this._NAME; }
            set
            {
                if (this._NAME != value)
                {
                    this._NAME = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
