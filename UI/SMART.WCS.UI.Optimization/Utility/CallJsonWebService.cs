using SMART.WCS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Optimization.Utility
{
    public class CallJsonWebService
    {
        static BaseClass BaseClass = new BaseClass();

        #region CallServiceRocExec - 최적화 수행처리 (OPT0102_QPS)
        /// <summary>
        /// 최적화 수행처리 (OPT0102_QPS)
        /// </summary>
        /// <param name="_strWrkPlanYmd">출고일자</param>
        /// <param name="_strDataSetID">데이터 그룹 ID</param>
        /// <param name="_strOptSeq">최적화 차수</param>
        /// <returns></returns>
        public static bool CallServiceRocExec(string _strWrkPlanYmd, string _strDataSetID, string _strOptSeq)
        {
            try
            {
                var strCompanyCD    = BaseClass.CompanyCode;                                // 회사 코드
                var strCenterCD     = BaseClass.CenterCD;                                   // 센터코드
                var strConnUrl      = BaseClass.GetAppSettings("JsonWebServiceURL_Sort");   // 웹서비스 URL
                var strUserID       = BaseClass.UserID;                                     // 사용자 ID

                // 저장 성공인 경우 최적화 웹서비스 (Json) 호출
                var strJsonParamValue = "{\"dataSetId\":\"" + _strDataSetID                       // 데이터 그룹 ID
                                        + "\"," + "\"coCd\":\"" + BaseClass.CompanyCode             // 회사코드
                                        + "\"," + "\"cntrCd\":\"" + strCenterCD                     // 센터코드
                                        + "\"," + "\"optSeq\":\"" + _strOptSeq                      // 최적화 차수
                                        + "\"," + "\"wrkPlanYmd\":\"" + _strWrkPlanYmd              // 출고일자
                                        + "\"," + "\"userId\":\"" + BaseClass.UserID + "\"}";       // 사용자 ID

                var responseValue = BaseClass.PostSendJson(strConnUrl, strJsonParamValue);

                return true;
            }
            catch { throw; }
        }
        #endregion

        public static bool CallServiceCellSimul(string _strWrkPlanYmd, string _strDataSetID, string _strOptSeq, int _iRocOptSeq)
        {
            try
            {
                var strCompanyCD        = BaseClass.CompanyCode;                                        // 회사 코드
                var strCenterCD         = BaseClass.CenterCD;                                           // 센터코드
                var strConnUrl          = BaseClass.GetAppSettings("JsonWebServiceURL_CellSimul");      // 웹서비스 URL
                var strUserID           = BaseClass.UserID;                                             // 사용자 ID

                // 저장 성공인 경우 최적화 웹서비스 (Json) 호출
                var strJsonParamValue   = "{\"dataSetId\":\"" + _strDataSetID                       // 데이터 그룹 ID
                                        + "\"," + "\"coCd\":\"" + BaseClass.CompanyCode             // 회사코드
                                        + "\"," + "\"cntrCd\":\"" + strCenterCD                     // 센터코드
                                        + "\"," + "\"optSeq\":\"" + _strOptSeq                      // 최적화 차수
                                        + "\"," + "\"wrkPlanYmd\":\"" + _strWrkPlanYmd              // 출고일자
                                        + "\"," + "\"orgOptSeq\":\"" + _iRocOptSeq.ToString()       // 셀최적화차수
                                        + "\"," + "\"userId\":\"" + BaseClass.UserID + "\"}";       // 사용자 ID

                //JsonWebServiceURL_Optimi

                var responseValue = BaseClass.PostSendJson(strConnUrl, strJsonParamValue);

                return true;
            }
            catch{ throw; }
        }

        public static bool CallServiceCellOpti(string _strWrkPlanYmd, string _strDataSetID, string _strOptSeq, int _iRocOptSeq)
        {
            try
            {
                var strCompanyCD        = BaseClass.CompanyCode;                                        // 회사 코드
                var strCenterCD         = BaseClass.CenterCD;                                           // 센터코드
                var strConnUrl          = BaseClass.GetAppSettings("JsonWebServiceURL_CellOpti");      // 웹서비스 URL
                var strUserID           = BaseClass.UserID;                                             // 사용자 ID

                // 저장 성공인 경우 최적화 웹서비스 (Json) 호출
                var strJsonParamValue   = "{\"dataSetId\":\"" + _strDataSetID                       // 데이터 그룹 ID
                                        + "\"," + "\"coCd\":\"" + BaseClass.CompanyCode             // 회사코드
                                        + "\"," + "\"cntrCd\":\"" + strCenterCD                     // 센터코드
                                        + "\"," + "\"optSeq\":\"" + _strOptSeq                      // 최적화 차수
                                        + "\"," + "\"wrkPlanYmd\":\"" + _strWrkPlanYmd              // 출고일자
                                        + "\"," + "\"userId\":\"" + BaseClass.UserID + "\"}";       // 사용자 ID

                //JsonWebServiceURL_Optimi

                var responseValue = BaseClass.PostSendJson(strConnUrl, strJsonParamValue);

                return true;
            }
            catch { throw; }
        }
    }
}
