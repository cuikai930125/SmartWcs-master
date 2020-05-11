using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.DataMembers
{
    public class MainWinParam
    {
        #region 배치번호
        /// <summary>
        /// 배치번호
        /// </summary>
        public string BTCH_NO { get; set; }
        #endregion

        #region 설비 ID
        /// <summary>
        /// 설비 ID
        /// </summary>
        public string EQP_ID { get; set; }
        #endregion

        #region 고객사 코드
        /// <summary>
        /// 고객사 코드
        /// </summary>
        public string CST_CD { get; set; }
        #endregion

        #region 고객사 명
        /// <summary>
        /// 고객사 명
        /// </summary>
        public string CST_NM { get; set; }
        #endregion

        #region 거래처 코드
        /// <summary>
        /// 거래처 코드
        /// </summary>
        public string BIZPTNR_CD { get; set; }
        #endregion

        #region 거래처 명
        /// <summary>
        /// 거래처 명
        /// </summary>
        public string BIZPTNR_NM { get; set; }
        #endregion

        #region 출고일자
        /// <summary>
        /// 반품일자
        /// </summary>
        public string GI_YMD { get; set; }
        #endregion

        #region 반품일자
        /// <summary>
        /// 반품일자
        /// </summary>
        public string RTN_YMD { get; set; }
        #endregion

        #region 배치순번
        /// <summary>
        /// 배치순번
        /// </summary>
        public decimal BTCH_SEQ { get; set; }
        #endregion

        #region 배치구분
        /// <summary>
        /// 배치구분
        /// </summary>
        public string BTCH_SPR { get; set; }
        #endregion

        #region 중분류
        /// <summary>
        /// 중분류
        /// </summary>
        public string MID_CLAS { get; set; }
        #endregion

        #region 작업시작일자
        /// <summary>
        /// 작업시작일자
        /// </summary>
        public string WRK_STRT_DT { get; set; }
        #endregion

        #region 출고박스번호
        /// <summary>
        /// 출고박스번호
        /// </summary>
        public string GI_BOX_NO { get; set; }
        # endregion

        #region Menu ID
        /// <summary>
        /// 메뉴 ID
        /// </summary>
        public string MENU_ID { get; set; }
        #endregion



        public string ORDERDATE { get; set; }

        public string ORDERBATCH { get; set; }

        public string SORTID { get; set; }


        #region + 소터
        public string SRT { get; set; }
        #endregion

        #region + 시작일자
        public string FROM_DATE { get; set; }
        #endregion

        #region + 시작시간
        public string FROM_TIME { get; set; }
        #endregion

        #region + 종료일자
        public string TO_DATE { get; set; }
        #endregion

        #region + 종료시간
        public string TO_TIME { get; set; }
        #endregion

        #region + 소터 오류코드
        public string SRT_ERR_CD { get; set; }
        #endregion

        #region + 오류 사유코드
        public string SRT_RSN_CD { get; set; }
        #endregion
    }
}
