using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Processing.DataMembers.R1007_GAN
{
    public class WrkInfo : PropertyNotifyExtensions
    {
        public WrkInfo()
        {

        }

        public bool GroupEquals(WrkInfo MasterGridInfo)
        {
            return (this.ECS_JOB_NO == MasterGridInfo.ECS_JOB_NO);
        }

        #region 주석
        // CO_CD
        // CNTR_CD
        // EQP_ID
        // BTCH_NO
        // GAN_CYCLE_NO
        // ECS_JOB_NO
        // GAN_WRK_STAT_CD
        // GAN_WRK_STAT_NM
        // GAN_JOB_IDCT_TYPE_CD
        // GAN_JOB_IDCT_TYPE_NM
        // PKR_PRIORITY_NO
        // PKR_ID    
        // PKR_MV_SEQ 
        // GAN_TASK_TYPE_CD
        // GAN_TASK_TYPE_NM
        // SKU_CD
        // SKU_NM
        // FR_CELL_ID
        // TO_CELL_ID
        // CELL_WRK_BOX_QTY
        // TASK_WRK_BOX_QTY
        // PA_STK_HGT
        // PU_TGT_HGT
        // DR_HGT_HGT
        // BEAM_MV_SEQ
        // GAN_WRK_IDCT_DATE
        // GAN_WRK_IDCT_CMPT_DATE
        // GAN_WRK_CMPT_DATE
        // GAN_ERR_CD
        // WRK_STAT_CD
        #endregion

        #region + CO_CD - 회사코드
        private string _CO_CD;
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

        #region + CNTR_CD - 센터코드
        private string _CNTR_CD;
        public string CNTR_CD
        {
            get { return this._CNTR_CD; }
            set
            {
                if (this._CNTR_CD != value)
                {
                    this._CNTR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + EQP_ID - 장비ID
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

        #region + BTCH_NO - 배치번호
        private string _BTCH_NO;
        public string BTCH_NO
        {
            get { return this._BTCH_NO; }
            set
            {
                if (this._BTCH_NO != value)
                {
                    this._BTCH_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_CYCLE_NO - 갠트리순환번호
        private string _GAN_CYCLE_NO;
        public string GAN_CYCLE_NO
        {
            get { return this._GAN_CYCLE_NO; }
            set
            {
                if (this._GAN_CYCLE_NO != value)
                {
                    this._GAN_CYCLE_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + ECS_JOB_NO - ECS JOB NO
        private string _ECS_JOB_NO;
        public string ECS_JOB_NO
        {
            get { return this._ECS_JOB_NO; }
            set
            {
                if (this._ECS_JOB_NO != value)
                {
                    this._ECS_JOB_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_WRK_STAT_CD - 작업처리상태코드
        private string _GAN_WRK_STAT_CD;
        public string GAN_WRK_STAT_CD
        {
            get { return this._GAN_WRK_STAT_CD; }
            set
            {
                if (this._GAN_WRK_STAT_CD != value)
                {
                    this._GAN_WRK_STAT_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_WRK_STAT_NM - 작업처리상태
        private string _GAN_WRK_STAT_NM;
        public string GAN_WRK_STAT_NM
        {
            get { return this._GAN_WRK_STAT_NM; }
            set
            {
                if (this._GAN_WRK_STAT_NM != value)
                {
                    this._GAN_WRK_STAT_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_JOB_IDCT_TYPE_CD - 갠트리작업유형코드
        private string _GAN_JOB_IDCT_TYPE_CD;
        public string GAN_JOB_IDCT_TYPE_CD
        {
            get { return this._GAN_JOB_IDCT_TYPE_CD; }
            set
            {
                if (this._GAN_JOB_IDCT_TYPE_CD != value)
                {
                    this._GAN_JOB_IDCT_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_JOB_IDCT_TYPE_NM - 갠트리작업유형
        private string _GAN_JOB_IDCT_TYPE_NM;
        public string GAN_JOB_IDCT_TYPE_NM
        {
            get { return this._GAN_JOB_IDCT_TYPE_NM; }
            set
            {
                if (this._GAN_JOB_IDCT_TYPE_NM != value)
                {
                    this._GAN_JOB_IDCT_TYPE_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PKR_PRIORITY_NO - 피커우선순위
        private string _PKR_PRIORITY_NO;
        /// <summary>
        /// 피커우선순위
        /// </summary>
        public string PKR_PRIORITY_NO
        {
            get { return this._PKR_PRIORITY_NO; }
            set
            {
                if (this._PKR_PRIORITY_NO != value)
                {
                    this._PKR_PRIORITY_NO = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PKR_ID     - 피커ID
        private string _PKR_ID;
        /// <summary>
        /// 피커ID
        /// </summary>
        public string PKR_ID
        {
            get { return this._PKR_ID; }
            set
            {
                if (this._PKR_ID != value)
                {
                    this._PKR_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PKR_MV_SEQ  - 피커이동순서
        private string _PKR_MV_SEQ;
        /// <summary>
        /// 피커이동순서
        /// </summary>
        public string PKR_MV_SEQ
        {
            get { return this._PKR_MV_SEQ; }
            set
            {
                if (this._PKR_MV_SEQ != value)
                {
                    this._PKR_MV_SEQ = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_TASK_TYPE_CD - 갠트리TASK유형코드
        private string _GAN_TASK_TYPE_CD;
        /// <summary>
        /// 갠트리TASK유형코드
        /// </summary>
        public string GAN_TASK_TYPE_CD
        {
            get { return this._GAN_TASK_TYPE_CD; }
            set
            {
                if (this._GAN_TASK_TYPE_CD != value)
                {
                    this._GAN_TASK_TYPE_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_TASK_TYPE_NM - 갠트리TASK유형코드
        private string _GAN_TASK_TYPE_NM;
        /// <summary>
        /// 갠트리TASK유형코드
        /// </summary>
        public string GAN_TASK_TYPE_NM
        {
            get { return this._GAN_TASK_TYPE_NM; }
            set
            {
                if (this._GAN_TASK_TYPE_NM != value)
                {
                    this._GAN_TASK_TYPE_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + SKU_CD - 상품코드
        private string _SKU_CD;
        /// <summary>
        /// 상품코드
        /// </summary>
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

        #region + SKU_NM - 상품명
        private string _SKU_NM;
        /// <summary>
        /// 상품명
        /// </summary>
        public string SKU_NM
        {
            get { return this._SKU_NM; }
            set
            {
                if (this._SKU_NM != value)
                {
                    this._SKU_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + FR_CELL_ID - FROM CEILL ID
        private string _FR_CELL_ID;
        /// <summary>
        /// FROM CEILL ID
        /// </summary>
        public string FR_CELL_ID
        {
            get { return this._FR_CELL_ID; }
            set
            {
                if (this._FR_CELL_ID != value)
                {
                    this._FR_CELL_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TO_CELL_ID - TO CELL ID
        private string _TO_CELL_ID;
        /// <summary>
        /// TO CELL ID
        /// </summary>
        public string TO_CELL_ID
        {
            get { return this._TO_CELL_ID; }
            set
            {
                if (this._TO_CELL_ID != value)
                {
                    this._TO_CELL_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        //#region + CELL_WRK_BOX_QTY - CELL 작업수량
        //private string _CELL_WRK_BOX_QTY;
        ///// <summary>
        ///// CELL 작업수량
        ///// </summary>
        //public string CELL_WRK_BOX_QTY
        //{
        //    get { return this._CELL_WRK_BOX_QTY; }
        //    set
        //    {
        //        if (this._CELL_WRK_BOX_QTY != value)
        //        {
        //            this._CELL_WRK_BOX_QTY = value;
        //            RaisePropertyChanged();
        //        }
        //    }
        //}
        //#endregion

        #region + CELL_PICK_BOX_QTY - CELL 피킹수량
        private string _CELL_PICK_BOX_QTY;
        /// <summary>
        /// TASK 작업수량
        /// </summary>
        public string CELL_PICK_BOX_QTY
        {
            get { return this._CELL_PICK_BOX_QTY; }
            set
            {
                if (this._CELL_PICK_BOX_QTY != value)
                {
                    this._CELL_PICK_BOX_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CELL_DROP_BOX_QTY - CELL 드롭수량
        private string _CELL_DROP_BOX_QTY;
        /// <summary>
        /// TASK 작업수량
        /// </summary>
        public string CELL_DROP_BOX_QTY
        {
            get { return this._CELL_DROP_BOX_QTY; }
            set
            {
                if (this._CELL_DROP_BOX_QTY != value)
                {
                    this._CELL_DROP_BOX_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + TASK_WRK_BOX_QTY - TASK 작업수량
        private string _TASK_WRK_BOX_QTY;
        /// <summary>
        /// TASK 작업수량
        /// </summary>
        public string TASK_WRK_BOX_QTY
        {
            get { return this._TASK_WRK_BOX_QTY; }
            set
            {
                if (this._TASK_WRK_BOX_QTY != value)
                {
                    this._TASK_WRK_BOX_QTY = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PA_STK_HGT - 적재스택높이
        private string _PA_STK_HGT;
        /// <summary>
        /// 적재스택높이
        /// </summary>
        public string PA_STK_HGT
        {
            get { return this._PA_STK_HGT; }
            set
            {
                if (this._PA_STK_HGT != value)
                {
                    this._PA_STK_HGT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + PU_TGT_HGT - 팍업대상높이
        private string _PU_TGT_HGT;
        /// <summary>
        /// 팍업대상높이
        /// </summary>
        public string PU_TGT_HGT
        {
            get { return this._PU_TGT_HGT; }
            set
            {
                if (this._PU_TGT_HGT != value)
                {
                    this._PU_TGT_HGT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + DR_HGT_HGT - 드롭대상높이
        private string _DR_HGT_HGT;
        /// <summary>
        /// 드롭대상높이
        /// </summary>
        public string DR_HGT_HGT
        {
            get { return this._DR_HGT_HGT; }
            set
            {
                if (this._DR_HGT_HGT != value)
                {
                    this._DR_HGT_HGT = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + BEAM_MV_SEQ - Beam이동순서
        private string _BEAM_MV_SEQ;
        /// <summary>
        /// Beam이동순서
        /// </summary>
        public string BEAM_MV_SEQ
        {
            get { return this._BEAM_MV_SEQ; }
            set
            {
                if (this._BEAM_MV_SEQ != value)
                {
                    this._BEAM_MV_SEQ = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_WRK_IDCT_DATE - ECS 작업지시일시
        private string _GAN_WRK_IDCT_DATE;
        /// <summary>
        /// ECS 작업지시일시
        /// </summary>
        public string GAN_WRK_IDCT_DATE
        {
            get { return this._GAN_WRK_IDCT_DATE; }
            set
            {
                if (this._GAN_WRK_IDCT_DATE != value)
                {
                    this._GAN_WRK_IDCT_DATE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_WRK_IDCT_CMPT_DATE - ???
        private string _GAN_WRK_IDCT_CMPT_DATE;
        /// <summary>
        /// ???
        /// </summary>
        public string GAN_WRK_IDCT_CMPT_DATE
        {
            get { return this._GAN_WRK_IDCT_CMPT_DATE; }
            set
            {
                if (this._GAN_WRK_IDCT_CMPT_DATE != value)
                {
                    this._GAN_WRK_IDCT_CMPT_DATE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_WRK_CMPT_DATE - ECS 작업완료일시
        private string _GAN_WRK_CMPT_DATE;
        /// <summary>
        /// ECS 작업완료일시
        /// </summary>
        public string GAN_WRK_CMPT_DATE
        {
            get { return this._GAN_WRK_CMPT_DATE; }
            set
            {
                if (this._GAN_WRK_CMPT_DATE != value)
                {
                    this._GAN_WRK_CMPT_DATE = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_ERR_CD - 에러코드
        private string _GAN_ERR_CD;
        /// <summary>
        /// 에러코드
        /// </summary>
        public string GAN_ERR_CD
        {
            get { return this._GAN_ERR_CD; }
            set
            {
                if (this._GAN_ERR_CD != value)
                {
                    this._GAN_ERR_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + GAN_ERR_NM - 에러코드명
        private string _GAN_ERR_NM;
        /// <summary>
        /// 에러코드명
        /// </summary>
        public string GAN_ERR_NM
        {
            get { return this._GAN_ERR_NM; }
            set
            {
                if (this._GAN_ERR_NM != value)
                {
                    this._GAN_ERR_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + WRK_STAT_CD - ???
        private string _WRK_STAT_CD;
        public string WRK_STAT_CD
        {
            get { return this._WRK_STAT_CD; }
            set
            {
                if (this._WRK_STAT_CD != value)
                {
                    this._WRK_STAT_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion



    }
}
