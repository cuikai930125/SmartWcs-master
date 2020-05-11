using SMART.WCS.Common.Extensions;
using System.Collections.ObjectModel;
using System.Linq;

namespace SMART.WCS.UI.EMS.DataMembers.E1003
{
    public class EqpClsTreeItem : PropertyNotifyExtensions
    {
        string _EQP_CLS_ID;
        public string EQP_CLS_ID
        {
            get
            {
                return _EQP_CLS_ID;
            }
            set
            {
                if (_EQP_CLS_ID != value)
                {
                    _EQP_CLS_ID = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _EQP_CLS_NM;
        public string EQP_CLS_NM
        {
            get
            {
                return _EQP_CLS_NM;
            }
            set
            {
                if (_EQP_CLS_NM != value)
                {
                    _EQP_CLS_NM = value;
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

        int _EQP_CLS_LV;
        /// <summary>
        /// Level
        /// </summary>
        public int EQP_CLS_LV
        {
            get { return _EQP_CLS_LV; }
            set
            {
                if (_EQP_CLS_LV != value)
                {
                    _EQP_CLS_LV = value;
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

        public EqpClsTreeItem Parent { get; set; }
        public ObservableCollection<EqpClsTreeItem> Items { get; set; }

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

        public ObservableCollection<EqpClsTreeItem> FindDirectParent(ObservableCollection<EqpClsTreeItem> _items)
        {
            ObservableCollection<EqpClsTreeItem> ret = new ObservableCollection<EqpClsTreeItem>();
            if (_items.Contains(this) == true)
            {
                ret = _items;
            }
            else
            {
                foreach (EqpClsTreeItem item in _items)
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

        public EqpClsTreeItem()
        {
            Items = new ObservableCollection<EqpClsTreeItem>();
        }
    }
}
