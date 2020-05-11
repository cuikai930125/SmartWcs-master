using SMART.WCS.Common;
using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Optimization.DataMembers.OPT0102
{
    public class OrderList : PropertyNotifyExtensions
    {
        #region + OPT_SEQ - 최적화 차수
        private int _OPT_SEQ;
        public int OPT_SEQ
        {
            get { return this._OPT_SEQ; }
            set
            {
                if (this._OPT_SEQ != value)
                {
                    this._OPT_SEQ = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ORD_NO - 오더 번호
        private string _ORD_NO;
        public string ORD_NO
        {
            get { return this._ORD_NO; }
            set
            {
                if (this._ORD_NO != value)
                {
                    this._ORD_NO = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + ORD_LINE_SEQ - 오더 라인 번호
        private int _ORD_LINE_SEQ;
        public int ORD_LINE_SEQ
        {
            get { return this._ORD_LINE_SEQ; }
            set
            {
                if (this._ORD_LINE_SEQ != value)
                {
                    this._ORD_LINE_SEQ = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + SKU_CD - SKU 코드
        private string _SKU_CD;
        public string SKU_CD
        {
            get { return this._SKU_CD; }
            set
            {
                if (this._SKU_CD != value)
                {
                    this._SKU_CD = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + LOC_CD - 로케이션 코드
        private string _LOC_CD;
        public string LOC_CD
        {
            get { return this._LOC_CD; }
            set
            {
                if (this._LOC_CD != value)
                {
                    this._LOC_CD = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + PLAN_SKU_SEQ - 계획 SKU순번
        private int _PLAN_SKU_SEQ;
        public int PLAN_SKU_SEQ
        {
            get { return this._PLAN_SKU_SEQ; }
            set
            {
                if (this._PLAN_SKU_SEQ != value)
                {
                    this._PLAN_SKU_SEQ = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + PLAN_ORD_SEQ - 계획 오더순번
        private int _PLAN_ORD_SEQ;
        public int PLAN_ORD_SEQ
        {
            get { return this._PLAN_ORD_SEQ; }
            set
            {
                if (this._PLAN_ORD_SEQ != value)
                {
                    this._PLAN_ORD_SEQ = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + OPT_SKU_SEQ - 최적화 SKU 순번
        private int _OPT_SKU_SEQ;
        public int OPT_SKU_SEQ
        {
            get { return this._OPT_SKU_SEQ; }
            set
            {
                if (this._OPT_SKU_SEQ != value)
                {
                    this._OPT_SKU_SEQ = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + OPT_ORD_SEQ - 최적화 오더순번
        private int _OPT_ORD_SEQ;
        public int OPT_ORD_SEQ
        {
            get { return this._OPT_ORD_SEQ; }
            set
            {
                if (this._OPT_ORD_SEQ != value)
                {
                    this._OPT_ORD_SEQ = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + OPT_SKU_LEN - 최적화 SKU LEN
        private decimal _OPT_SKU_LEN;
        public decimal OPT_SKU_LEN
        {
            get { return this._OPT_SKU_LEN; }
            set
            {
                if (this._OPT_SKU_LEN != value)
                {
                    this._OPT_SKU_LEN = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + OPT_ORD_LEN - 최적화 오더 LEN
        private decimal _OPT_ORD_LEN;
        public decimal OPT_ORD_LEN
        {
            get { return this._OPT_ORD_LEN; }
            set
            {
                if (this._OPT_ORD_LEN != value)
                {
                    this._OPT_ORD_LEN = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion

        #region + FLTR_DEL_YN - 필더 삭제건 제외 여부값
        private string _FLTR_DEL_YN;
        public string FLTR_DEL_YN
        {
            get { return this._FLTR_DEL_YN; }
            set
            {
                if (this._FLTR_DEL_YN != value)
                {
                    this._FLTR_DEL_YN   = value;
                    this.Checked        = this.FLTR_DEL_YN.Equals("Y") ? true : false;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + Checked - 사용여부 변환값
        private bool _Checked;
        public bool Checked
        {
            get { return this._Checked; }
            set
            {
                if (this._Checked != value)
                {
                    this._Checked = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ALLOC_QTY - 수량
        private decimal _ALLOC_QTY;
        public decimal ALLOC_QTY
        {
            get { return this._ALLOC_QTY; }
            set
            {
                if (this._ALLOC_QTY != value)
                {
                    this._ALLOC_QTY = value;
                    RaisePropertyChanged();
                }
            }

        }
        #endregion
    }
}
