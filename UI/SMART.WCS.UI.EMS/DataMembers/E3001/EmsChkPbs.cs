using SMART.WCS.Common.Extensions;

namespace SMART.WCS.UI.EMS.DataMembers.E3001
{
    public class EmsChkPbs : PropertyNotifyExtensions
    {
        string _PBS_ID;
        public string PBS_ID
        {
            get
            {
                return _PBS_ID;
            }
            set
            {
                if (_PBS_ID != value)
                {
                    _PBS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PBS_NM;
        public string PBS_NM
        {
            get
            {
                return _PBS_NM;
            }
            set
            {
                if (_PBS_NM != value)
                {
                    _PBS_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region Tree 구조
        int _PBS_LV;
        /// <summary>
        /// Level
        /// </summary>
        public int PBS_LV
        {
            get { return _PBS_LV; }
            set
            {
                if (_PBS_LV != value)
                {
                    _PBS_LV = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PARENT_PBS_ID;
        /// <summary>
        /// Parent
        /// </summary>
        public string PARENT_PBS_ID
        {
            get { return _PARENT_PBS_ID; }
            set
            {
                if (_PARENT_PBS_ID != value)
                {
                    _PARENT_PBS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion Tree 구조

        /// <summary>
        /// CheckBox
        /// </summary>
        public int IS_CHECKED
        {
            get { return IsChecked ? 1 : 0; }
            set
            {
                if (IsChecked && 0 == value)
                {
                    IsChecked = false;
                    RaisePropertyChanged();
                }

                if (!IsChecked && 1 == value)
                {
                    IsChecked = true;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
