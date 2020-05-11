using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Optimization.DataMembers.OPT0107
{
    public class OrderUploadRslt : PropertyNotifyExtensions
    {
        #region + 데이터 그룹 - WAV_NO
        private string _WAV_NO;
        public string WAV_NO
        {
            get { return this._WAV_NO; }
            set
            {
                if (this._WAV_NO != value)
                {
                    this._WAV_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 고객사 코드 - CST_CD
        private string _CST_CD;
        public string CST_CD
        {
            get { return this._CST_CD; }
            set
            {
                if (this._CST_CD != value)
                {
                    this._CST_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 오더 번호 - ORD_NO
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

        #region + 고객사 오더 번호 - CST_ORD_NO
        private string _CST_ORD_NO;
        public string CST_ORD_NO
        {
            get { return this._CST_ORD_NO; }
            set
            {
                if (this._CST_ORD_NO != value)
                {
                    this._CST_ORD_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 오더 상세 순번 - ORD_DTL_SEQ_NO
        private string _ORD_DTL_SEQ_NO;
        public string ORD_DTL_SEQ_NO
        {
            get { return this._ORD_DTL_SEQ_NO; }
            set
            {
                if (this._ORD_DTL_SEQ_NO  != value)
                {
                    this._ORD_DTL_SEQ_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 오더 라인번호 - ORD_LINE_NO
        private string _ORD_LINE_NO;
        public string ORD_LINE_NO
        {
            get { return this._ORD_LINE_NO; }
            set
            {
                if (this._ORD_LINE_NO != value)
                {
                    this._ORD_LINE_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU 코드 - SKU_CD
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

        #region + 계획 수량 - PLAN_QTY
        private int _PLAN_QTY;
        public int PLAN_QTY
        {
            get { return this._PLAN_QTY; }
            set
            {
                if (this._PLAN_QTY != value)
                {
                    this._PLAN_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 결과 수량 - RSLT_QTY
        private int _RSLT_QTY;
        public int RSLT_QTY
        {
            get { return this._RSLT_QTY; }
            set
            {
                if (this._RSLT_QTY != value)
                {
                    this._RSLT_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU CBM - SKU_CBM
        private decimal _SKU_CBM;
        public decimal SKU_CBM
        {
            get { return this._SKU_CBM; }
            set
            {
                if (this._SKU_CBM != value)
                {
                    this._SKU_CBM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU 가로 - SKU_WTH_LEN
        private decimal _SKU_WTH_LEN;
        public decimal SKU_WTH_LEN
        {
            get { return this._SKU_WTH_LEN; }
            set
            {
                if (this._SKU_WTH_LEN != value)
                {
                    this._SKU_WTH_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU 세로 - SKU_VERT_LEN
        private decimal _SKU_VERT_LEN;
        public decimal SKU_VERT_LEN
        {
            get { return this._SKU_VERT_LEN; }
            set
            {
                if (this._SKU_VERT_LEN != value)
                {
                    this._SKU_VERT_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU 높이 - SKU_HGT_LEN
        private decimal _SKU_HGT_LEN;
        public decimal SKU_HGT_LEN
        {
            get { return this._SKU_HGT_LEN; }
            set
            {
                if (this._SKU_HGT_LEN != value)
                {
                    this._SKU_HGT_LEN = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU 중량 - SKU_WGT
        private decimal _SKU_WGT;
        public decimal SKU_WGT
        {
            get { return this._SKU_WGT; }
            set
            {
                if (this._SKU_WGT != value)
                {
                    this._SKU_WGT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + Location 코드 - LOC_CD
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

        #region + 오더 시작코드 - ORD_STRT_CD
        private string _ORD_STRT_CD;
        public string ORD_STRT_CD
        {
            get { return this._ORD_STRT_CD; }
            set
            {
                if (this._ORD_STRT_CD != value)
                {
                    this._ORD_STRT_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 설비 ID- EQP_ID
        private string _EQP_ID;
        public string EQP_ID
        {
            get { return this._EQP_ID; }
            set
            {
                if (this._EQP_ID != value)
                {
                    this._EQP_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 데이터 그룹 - DATA_SET_ID
        private string _DATA_SET_ID;
        public string DATA_SET_ID
        {
            get { return this._DATA_SET_ID; }
            set
            {
                if (this._DATA_SET_ID != value)
                {
                    this._DATA_SET_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + 엑셀 업로드 결과 메세지 - RSLT_MSG
        private string _RSLT_MSG;
        public string RSLT_MSG
        {
            get { return this._RSLT_MSG; }
            set
            {
                if (this._RSLT_MSG != value)
                {
                    this._RSLT_MSG = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
