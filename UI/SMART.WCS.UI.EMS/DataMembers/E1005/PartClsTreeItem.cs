using SMART.WCS.Common.Extensions;

using System.Collections.ObjectModel;

namespace SMART.WCS.UI.EMS.DataMembers.E1005
{
    public class PartClsTreeItem : PropertyNotifyExtensions
    {
        string _PART_CLS_ID;
        public string PART_CLS_ID
        {
            get
            {
                return _PART_CLS_ID;
            }
            set
            {
                if (_PART_CLS_ID != value)
                {
                    _PART_CLS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_CLS_NM;
        public string PART_CLS_NM
        {
            get
            {
                return _PART_CLS_NM;
            }
            set
            {
                if (_PART_CLS_NM != value)
                {
                    _PART_CLS_NM = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _PART_CLS_NOTE;
        /// <summary>
        /// 부품 분류 비고
        /// </summary>
        public string PART_CLS_NOTE
        {
            get
            {
                return _PART_CLS_NOTE;
            }
            set
            {
                if (_PART_CLS_NOTE != value)
                {
                    _PART_CLS_NOTE = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region Tree 구조
        int _PART_CLS_LV;
        /// <summary>
        /// Level
        /// </summary>
        public int PART_CLS_LV
        {
            get { return _PART_CLS_LV; }
            set { _PART_CLS_LV = value; ; RaisePropertyChanged(); }
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

        public PartClsTreeItem Parent { get; set; }
        public ObservableCollection<PartClsTreeItem> Items { get; set; }

        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; RaisePropertyChanged(); }
        }

        bool isStatic;
        /// <summary>
        /// 수정 금지 (헤더 항목)
        /// </summary>
        public bool IsStatic
        {
            get { return isStatic; }
            set { isStatic = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<PartClsTreeItem> FindDirectParent(ObservableCollection<PartClsTreeItem> _items)
        {
            ObservableCollection<PartClsTreeItem> ret = new ObservableCollection<PartClsTreeItem>();
            if (_items.Contains(this) == true)
            {
                ret = _items;
            }
            else
            {
                foreach (PartClsTreeItem item in _items)
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

        public PartClsTreeItem()
        {
            Items = new ObservableCollection<PartClsTreeItem>();
        }
    }
}
