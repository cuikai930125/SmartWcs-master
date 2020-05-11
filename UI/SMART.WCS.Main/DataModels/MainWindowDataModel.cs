using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Main.DataModels
{
    public class MainWindowDataModel : INotifyPropertyChanged
    {
        List<MainWindowDataModel> _TREE_COLUMN_LIST = new List<MainWindowDataModel>();

        #region ▩ 속성
        #region TreeColumnList - 트리 컨트롤 데이터 리스트
        /// <summary>
        /// 트리 컨트롤 데이터 리스트
        /// </summary>
        public List<MainWindowDataModel> TREE_COLUMN_LIST
        {
            get { return this._TREE_COLUMN_LIST; }
            set { this._TREE_COLUMN_LIST = value; }
        }
        #endregion

        #region MENU_ID - 메뉴 ID
        private string _MENU_ID;

        /// <summary>
        /// 메뉴 ID
        /// </summary>
        public string MENU_ID
        {
            get { return this._MENU_ID; }
            set
            {
                if (this._MENU_ID != value)
                {
                    this._MENU_ID = value;
                    RaisePropertyChanged("MENU_ID");
                }
            }
        }
        #endregion

        #region MENU_NM - 메뉴명
        private string _MENU_NM;

        /// <summary>
        /// 메뉴명
        /// </summary>
        public string MENU_NM
        {
            get { return this._MENU_NM; }
            set
            {
                if (this._MENU_NM != value)
                {
                    this._MENU_NM = value;
                    RaisePropertyChanged("MENU_NM");
                }
            }
        }
        #endregion

        #region MENU_LVL - 메뉴 레벨 번호
        private int _MENU_LVL;

        /// <summary>
        /// 메뉴 레벨 번호
        /// </summary>
        public int MENU_LVL
        {
            get { return this._MENU_LVL; }
            set
            {
                if (this._MENU_LVL != value)
                {
                    this._MENU_LVL = value;
                    RaisePropertyChanged("MENU_LVL");
                }
            }
        }
        #endregion

        #region MENU_TYPE - 메뉴 타입
        private string _MENU_TYPE;

        /// <summary>
        /// 메뉴타입
        /// </summary>
        public string MENU_TYPE
        {
            get { return this._MENU_TYPE; }
            set
            {
                if (this._MENU_TYPE != value)
                {
                    this._MENU_TYPE = value;
                    RaisePropertyChanged("MENU_TYPE");
                }
            }
        }
        #endregion

        #region MENU_URL - 메뉴 URL
        private string _MENU_URL;

        /// <summary>
        /// 메뉴 URL
        /// </summary>
        public string MENU_URL
        {
            get { return this._MENU_URL; }
            set
            {
                if (this._MENU_URL != value)
                {
                    this._MENU_URL = value;
                    RaisePropertyChanged("MENU_URL");
                }
            }
        }
        #endregion

        #region MENU_ICON - 메뉴 아이콘
        private string _MENU_ICON;

        /// <summary>
        /// 메뉴 아이콘
        /// </summary>
        public string MENU_ICON
        {
            get { return this._MENU_ICON; }
            set
            {
                if (this._MENU_ICON != value)
                {
                    this._MENU_ICON = value;
                    RaisePropertyChanged("MENU_ICON");
                }
            }
        }
        #endregion

        #region ROLE_MENU_CD - 권한 코드
        private string _ROLE_MENU_CD;

        /// <summary>
        /// 권한 코드
        /// </summary>
        public string ROLE_MENU_CD
        {
            get { return this._ROLE_MENU_CD; }
            set
            {
                if (this._ROLE_MENU_CD == value) { return; }
                this._ROLE_MENU_CD = value;
                RaisePropertyChanged("ROLE_MENU_CD");
            }
        }
        #endregion

        #region TREE_ID - 트리 ID
        private string _TREE_ID;

        /// <summary>
        /// 트리 ID
        /// </summary>
        public string TREE_ID
        {
            get { return this._TREE_ID; }
            set
            {
                if (this._TREE_ID != value)
                {
                    this._TREE_ID = value;
                    RaisePropertyChanged("TREE_ID");
                }
            }
        }
        #endregion

        #region PARENT_ID - 상위 메뉴 ID
        private string _PARENT_ID;

        /// <summary>
        /// 상위 메뉴 ID
        /// </summary>
        public string PARENT_ID
        {
            get { return this._PARENT_ID; }
            set
            {
                if (this._PARENT_ID == value) { return; }
                this._PARENT_ID = value;

                if (this.PARENT_ID.Length > 0)
                {
                    this.TOP_MENU_ID = this.PARENT_ID.Substring(0, 3);
                }

                RaisePropertyChanged("PARENT_ID");
            }
        }
        #endregion

        #region TOP_MENU_ID - 최상위 메뉴 ID
        private string _TOP_MENU_ID;

        public string TOP_MENU_ID
        {
            get { return this._TOP_MENU_ID; }
            set
            {
                if (this._TOP_MENU_ID != value)
                {
                    this._TOP_MENU_ID = value;
                    RaisePropertyChanged("TOP_MENU_ID");
                }
            }
        }
        #endregion

        #region LANG_CD - 다국어 적용 메뉴명
        /// <summary>
        /// 다국어 적용 메뉴명
        /// </summary>
        private string _LANG_CD;

        public string LANG_CD
        {
            get { return this._LANG_CD; }
            set
            {
                if (this._LANG_CD != value)
                {
                    this._LANG_CD = value;
                    RaisePropertyChanged("LANG_CD");
                }
            }
        }
        #endregion

        //#region POP_UP_YN - 외부실행 여부
        //private string _POP_UP_YN;



        #region TAB_HEADER_TITLE - 탭 컨트롤 헤더명
        /// <summary>
        /// 탭 컨트롤 헤더명
        /// </summary>
        public string TAB_HEADER_TITLE { get; set; }
        #endregion

        #region TAB_CONTENT - 탭 컨트롤 컨텐츠
        /// <summary>
        /// 탭 컨트롤 컨탠츠
        /// </summary>
        public object TAB_CONTENT { get; set; }
        #endregion
        //#endregion
        #endregion

        #region ▩ 생성자
        public MainWindowDataModel()
        {

        }
        public MainWindowDataModel(DataTable _dtTreeInfo)
        {
            try
            {
                var menuList = SMART.WCS.Common.Data.ConvertDataTableToList.DataTableToList<MainWindowDataModel>(_dtTreeInfo);

                this.TREE_COLUMN_LIST = menuList;
            }
            catch { throw; }
        }
        #endregion

        #region ▩ 이벤트
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null) { PropertyChanged(this, e); }
        }
        protected void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
