using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Optimization.Utility
{
    public class HelperClass
    {
        static BaseClass BaseClass = new BaseClass();

        #region 공통 사용 (자주 사용하는) 프로시저
        #region > GetSP_OPT_QPS_DATA_SET_LIST_INQ - 데이터 그룹 조회 (ComboBox 설정용)
        /// <summary>
        /// 데이터 그룹 조회 (ComboBox 설정용)
        /// </summary>
        /// <param name="_strWrkPlanYmd">출고일자</param>
        /// <returns></returns>
        public static async Task<DataTable> GetSP_OPT_QPS_DATA_SET_LIST_INQ(string _strWrkPlanYmd)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsDatasetListInqData                = null;
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "SP_OPT_QPS_DATA_SET_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_DATA_SET_LIST", "O_RTN_RSLT" };

            var strCenterCD = BaseClass.CenterCD;                                                      // 센터코드
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD"           , strCenterCD);         // 센터코드
            dicInputParam.Add("P_WRK_PLAN_YMD"      , _strWrkPlanYmd);      // 출고일자
            #endregion

            #region 데이터 조회
            using (BaseDataAccess da = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsDatasetListInqData = da.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            if (dsDatasetListInqData.Tables.Count == 2)
            {
                dtRtnValue = dsDatasetListInqData.Tables[0];
            }
            else
            {
                throw new Exception("조회 데이터가 잘못되었습니다. 리턴 테이블이 2개가 아닙니다.");
            }

            return dtRtnValue;
        }
        #endregion

        #region > GetSP_OPT_QPS_ROC_SEQ_LIST_INQ - ROC 최적화 차수
        /// <summary>
        /// ROC 최적화 차수
        /// </summary>
        /// <param name="_strWrkPlanYmd">출고일자</param>
        /// <param name="_strSelectedDataSetID">데이터 그룹 ID</param>
        /// <returns></returns>
        public static async Task<DataTable> GetSP_OPT_QPS_ROC_SEQ_LIST_INQ(string _strWrkPlanYmd, string _strSelectedDataSetID)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsDatasetListInqData                = null;
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "SP_OPT_QPS_ROC_SEQ_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_ROC_SEQ_LIST", "O_RTN_RSLT" };
            var strCenterCD                             = BaseClass.CenterCD;                              // 센터코드
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCenterCD);               // 센터코드
            dicInputParam.Add("P_WRK_PLAN_YMD",     _strWrkPlanYmd);            // 출고일자
            dicInputParam.Add("P_DATA_SET_ID",      _strSelectedDataSetID);     // Data Set 콤보박스에서 선택된 ID값
            #endregion

            #region 데이터 조회
            using (BaseDataAccess da = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsDatasetListInqData = da.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            if (dsDatasetListInqData.Tables.Count == 2)
            {
                dtRtnValue = dsDatasetListInqData.Tables[0];
            }
            else
            {
                throw new Exception("조회 데이터가 잘못되었습니다. 리턴 테이블이 2개가 아닙니다.");
            }

            return dtRtnValue;
        }
        #endregion

        #region > GetSP_OPT_QPS_CELL_OPT_LIST_INQ - 셀 최적화 차수
        /// <summary>
        /// 셀 최적화 차수
        /// </summary>
        /// <param name="_strWrkPlanYmd">출고일자</param>
        /// <param name="_strDataSetID">데이터 그룹 ID</param>
        /// <param name="_strOptTypeCD">최적화 타입코드</param>
        /// <param name="_iRocOptSeq">ROC 최적화 차수</param>
        /// <returns></returns>
        public static async Task<DataTable> GetSP_OPT_QPS_CELL_OPT_LIST_INQ(string _strWrkPlanYmd, string _strDataSetID, string _strOptTypeCD, int _iRocOptSeq)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsCellOptSeqData                    = null;
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "SP_OPT_QPS_CELL_OPT_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "P_CELL_OPT_SET_LIST", "O_RTN_RSLT" };
            var strCenterCD                             = BaseClass.CenterCD;                       // 센터코드
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",                  strCenterCD);       // 센터코드
            dicInputParam.Add("P_WRK_PLAN_YMD",             _strWrkPlanYmd);    // 출고일자
            dicInputParam.Add("P_DATA_SET_ID",              _strDataSetID);     // 데이터 그룹 ID
            dicInputParam.Add("P_OPT_TYPE_CD",              _strOptTypeCD);     // 최적화 타입코드
            dicInputParam.Add("P_ROC_OPT_SEQ",              _iRocOptSeq);       // ROC 최적화 차수
            #endregion

            #region 데이터 조회
            using (BaseDataAccess da = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsCellOptSeqData = da.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            if (dsCellOptSeqData.Tables.Count == 2)
            {
                dtRtnValue = dsCellOptSeqData.Tables[0];
            }
            else
            {
                throw new Exception("조회 데이터가 잘못되었습니다. 리턴 테이블이 2개가 아닙니다.");
            }

            return dtRtnValue;
        }
        #endregion

        #region > GetSP_OPT_QPS_MAX_CELL_OPT_INQ - 최적화 옵션 리스트 조회
        /// <summary>
        /// QPS 최적화 옵션 리스트를 조회한다. (가장 최근 데이터)
        /// </summary>
        /// <param name="_strOptTypeCD">최적화 타입코드 (CELL_OPT, CELL_SIMUL)</param>
        /// <returns></returns>
        public static async Task<DataTable> GetSP_OPT_QPS_MAX_CELL_OPT_INQ(string _strOptTypeCD, string _strDataSetID)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsCellOptSeqData                    = null;
            DataTable dtRtnValue                        = null;
            var strProcedureName                        = "SP_OPT_QPS_MAX_CELL_OPT_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_QPS_OPT_SET_LIST", "O_RTN_RSLT" };
            var strCenterCD                             = BaseClass.CenterCD;   // 센터코드
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);       // 센터코드
            dicInputParam.Add("P_OPT_TYPE_CD",          _strOptTypeCD);     // 최적화 타입코드
            dicInputParam.Add("P_DATA_SET_ID",          _strDataSetID);     // 데이터 그룹 ID
            #endregion

            #region 데이터 조회
            using (BaseDataAccess da = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsCellOptSeqData = da.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            if (dsCellOptSeqData.Tables.Count == 2)
            {
                dtRtnValue = dsCellOptSeqData.Tables[0];
            }
            else
            {
                throw new Exception("조회 데이터가 잘못되었습니다. 리턴 테이블이 2개가 아닙니다.");
            }

            return dtRtnValue;
        }
        #endregion
        #endregion
    }
}
