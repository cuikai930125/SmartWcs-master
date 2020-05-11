using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.DataMembers.P2001_ANV
{
    public class AnRlstInfo : PropertyNotifyExtensions
    {
        public AnRlstInfo()
        {

        }

        #region + EXT_ID - 추출 ID
        private int _EXT_ID = 0;
        /// <summary>
        /// 추출 ID
        /// </summary>
        public int EXT_ID
        {
            get { return this._EXT_ID; }
            set
            {
                if (this._EXT_ID != value)
                {
                    this._EXT_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_TAR_CD - 구분코드
        private string _EXT_TAR_CD = string.Empty;
        /// <summary>
        /// 추출구분 (출고, 입고, 재고)
        /// </summary>
        public string EXT_TAR_CD
        {
            get { return this._EXT_TAR_CD; }
            set
            {
                if (this._EXT_TAR_CD != value)
                {
                    this._EXT_TAR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_TAR_NM - 구분명
        private string _EXT_TAR_NM = string.Empty;
        /// <summary>
        /// 구분명
        /// </summary>
        public string EXT_TAR_NM
        {
            get { return this._EXT_TAR_NM; }
            set
            {
                if (this._EXT_TAR_NM != value)
                {
                    this._EXT_TAR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_TYPE_CD - 추출타입 코드
        private string _EXT_TYPE_CD = string.Empty;
        /// <summary>
        /// 추출타입 (CAPA, EIQ, ABC)
        /// </summary>
        public string EXT_TYPE_CD
        {
            get { return this._EXT_TYPE_CD; }
            set
            {
                if (this._EXT_TYPE_CD != value)
                {
                    this._EXT_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_TYPE_NM - 추출타입명
        private string _EXT_TYPE_NM = string.Empty;
        /// <summary>
        /// 추출타입명
        /// </summary>
        public string EXT_TYPE_NM
        {
            get { return this._EXT_TYPE_NM; }
            set
            {
                if (this._EXT_TYPE_NM != value)
                {
                    this._EXT_TYPE_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_DATE_TYPE_CD - 추출일자타입코드
        private string _EXT_DATE_TYPE_CD = string.Empty;
        /// <summary>
        /// 추출일자타입코드
        /// </summary>
        public string EXT_DATE_TYPE_CD
        {
            get { return this._EXT_DATE_TYPE_CD; }
            set
            {
                if (this._EXT_DATE_TYPE_CD != value)
                {
                    this._EXT_DATE_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ABC_TYPE_CD - ABC TYPE 코드
        private string _ABC_TYPE_CD = string.Empty;
        /// <summary>
        /// ABC TYPE 코드
        /// </summary>
        public string ABC_TYPE_CD
        {
            get { return this._ABC_TYPE_CD; }
            set
            {
                if (this._ABC_TYPE_CD != value)
                {
                    this._ABC_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ABC_TYPE_NM - ABC TYPE 명
        private string _ABC_TYPE_NM = string.Empty;
        /// <summary>
        /// ABC TYPE 명
        /// </summary>
        public string ABC_TYPE_NM
        {
            get { return this._ABC_TYPE_NM; }
            set
            {
                if (this._ABC_TYPE_NM != value)
                {
                    this._ABC_TYPE_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_FROM - FROM
        private string _EXT_FROM = string.Empty;
        /// <summary>
        /// FROM
        /// </summary>
        public string EXT_FROM
        {
            get { return this._EXT_FROM; }
            set
            {
                if (this._EXT_FROM != value)
                {
                    this._EXT_FROM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_TO - TO
        private string _EXT_TO = string.Empty;
        /// <summary>
        /// TO
        /// </summary>
        public string EXT_TO
        {
            get { return this._EXT_TO; }
            set
            {
                if (this._EXT_TO != value)
                {
                    this._EXT_TO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_DTL_ID_CNT - 추출항목 수
        private int _EXT_DTL_ID_CNT = 0;
        /// <summary>
        /// 추출항목 수
        /// </summary>
        public int EXT_DTL_ID_CNT
        {
            get { return this._EXT_DTL_ID_CNT; }
            set
            {
                if (this._EXT_DTL_ID_CNT != value)
                {
                    this._EXT_DTL_ID_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ABC_CODE_CNT - ABC 수
        private int _ABC_CODE_CNT = 0;
        /// <summary>
        /// ABC 수
        /// </summary>
        public int ABC_CODE_CNT
        {
            get { return this._ABC_CODE_CNT; }
            set
            {
                if (this._ABC_CODE_CNT != value)
                {
                    this._ABC_CODE_CNT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + COMMENTS - 설명
        private string _COMMENTS = string.Empty;
        /// <summary>
        /// 설명
        /// </summary>
        public string COMMENTS
        {
            get { return this._COMMENTS; }
            set
            {
                if (this._COMMENTS != value)
                {
                    this._COMMENTS = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CO_CD - 회사코드
        private string _CO_CD = string.Empty;
        /// <summary>
        /// 회사코드
        /// </summary>
        public string CO_CD
        {
            get { return this._CO_CD; }
            set
            {
                if (this._CO_CD != value)
                {
                    this._CO_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CO_NM - 회사명
        private string _CO_NM = string.Empty;
        /// <summary>
        /// 회사명
        /// </summary>
        public string CO_NM
        {
            get { return this._CO_NM; }
            set
            {
                if (this._CO_NM != value)
                {
                    this._CO_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EXT_COMP_YN - 추출여부
        private string _EXT_COMP_YN = string.Empty;
        /// <summary>
        /// 추출여부
        /// </summary>
        public string EXT_COMP_YN
        {
            get { return this._EXT_COMP_YN; }
            set
            {
                if (this._EXT_COMP_YN != value)
                {
                    this._EXT_COMP_YN = value;
                    RaisePropertyChanged();
                }
            }
        }
        # endregion


    }
}
