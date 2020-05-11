using SMART.WCS.Common.Extensions;
using System.Collections.ObjectModel;
using System.Linq;

namespace SMART.WCS.UI.EMS.DataMembers.E1002
{
    public class PBSTreeItem : PropertyNotifyExtensions
    {
        string _PBS_ID;
        public string PBS_ID
        {
            get { return _PBS_ID; }
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
            get { return _PBS_NM; }
            set
            {
                if (_PBS_NM != value)
                {
                    _PBS_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PBS_DEV_CD;
        /// <summary>
        /// 구분
        /// </summary>
        public string PBS_DEV_CD
        {
            get { return _PBS_DEV_CD; }
            set
            {
                if (_PBS_DEV_CD != value)
                {
                    _PBS_DEV_CD = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _EQP_QTY;
        /// <summary>
        /// 설비 수량
        /// </summary>
        public int EQP_QTY
        {
            get { return _EQP_QTY; }
            set
            {
                if (_EQP_QTY != value)
                {
                    _EQP_QTY = value;
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

        string _PARENT_ID;
        /// <summary>
        /// Parent
        /// </summary>
        public string PARENT_ID
        {
            get { return _PARENT_ID; }
            set
            {
                if (_PARENT_ID != value)
                {
                    _PARENT_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        public PBSTreeItem Parent { get; set; }
        public ObservableCollection<PBSTreeItem> Items { get; set; }

        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool isStatic;
        /// <summary>
        /// 수정 금지 (헤더 항목)
        /// </summary>
        public bool IsStatic
        {
            get { return isStatic; }
            set
            {
                if (isStatic != value)
                {
                    isStatic = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<PBSTreeItem> FindDirectParent(ObservableCollection<PBSTreeItem> _items)
        {
            ObservableCollection<PBSTreeItem> ret = new ObservableCollection<PBSTreeItem>();
            if (_items.Contains(this) == true)
            {
                ret = _items;
            }
            else
            {
                foreach (PBSTreeItem item in _items)
                {
                    if (item.Items != null && item.Items.Contains(this) == true)
                    {
                        ret = FindDirectParent(item.Items);
                    }
                }
            }
            return ret;
        }

        // tree list node
        public object TreeNode;
        #endregion Tree 구조

        public PBSTreeItem()
        {
            Items = new ObservableCollection<PBSTreeItem>();
        }
    }
}
