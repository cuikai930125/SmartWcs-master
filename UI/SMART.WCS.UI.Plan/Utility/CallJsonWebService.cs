using SMART.WCS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.Plan.Utility
{
    public class CallJsonWebService
    {
        static BaseClass BaseClass = new BaseClass();

        #region CallServiceGantryPicking - 입고 최적화 수행처리
        public static bool CallServiceGantryStocking(string _strBtchNo, string _strEqpId)
        {
            try
            {
                var strConnUrl = BaseClass.GetAppSettings("JsonWebServiceURL_GantryStocking");   // 웹서비스 URL
                var strBtchNo = _strBtchNo;                                                     // 배치번호
                var strCoCd = BaseClass.CompanyCode;                                            // 회사 코드
                var strCntrCd = BaseClass.CenterCD;                                             // 센터코드
                var strEqpId = _strEqpId;                                                       // 장비코드
                var strRstrId = BaseClass.UserID;                                               // 사용자 ID
                var strUserId = BaseClass.UserID;                                               // 사용자 ID

                // 저장 성공인 경우 최적화 웹서비스 (Json) 호출
                var strJsonParamValue = "{\"btchNo\":\"" + strBtchNo                       // 데이터 그룹 ID
                                        + "\"," + "\"coCd\":\"" + strCoCd             // 회사코드
                                        + "\"," + "\"cntrCd\":\"" + strCntrCd                     // 센터코드
                                        + "\"," + "\"eqpId\":\"" + strEqpId                      // 최적화 차수
                                        + "\"," + "\"rstrId\":\"" + strRstrId                // 사용자 ID
                                        + "\"," + "\"updrId\":\"" + strUserId + "\"}";       // 사용자 ID

                //JsonWebServiceURL_GantryPicking
                var responseValue = BaseClass.PostSendJson(strConnUrl, strJsonParamValue);

                return true;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region CallServiceGantryPicking - 출고 최적화 수행처리
        public static bool CallServiceGantryPicking(string _strBtchNo, string _strEqpId)
        {
            try
            {
                var strConnUrl = BaseClass.GetAppSettings("JsonWebServiceURL_GantryPicking");   // 웹서비스 URL
                var strBtchNo = _strBtchNo;                                                     // 배치번호
                var strCoCd = BaseClass.CompanyCode;                                            // 회사 코드
                var strCntrCd = BaseClass.CenterCD;                                             // 센터코드
                var strEqpId = _strEqpId;                                                       // 장비코드
                var strRstrId = BaseClass.UserID;                                               // 사용자 ID
                var strUserId = BaseClass.UserID;                                               // 사용자 ID

                // 저장 성공인 경우 최적화 웹서비스 (Json) 호출
                var strJsonParamValue = "{\"btchNo\":\"" + strBtchNo                       // 데이터 그룹 ID
                                        + "\"," + "\"coCd\":\"" + strCoCd             // 회사코드
                                        + "\"," + "\"cntrCd\":\"" + strCntrCd                     // 센터코드
                                        + "\"," + "\"eqpId\":\"" + strEqpId                      // 최적화 차수
                                        + "\"," + "\"rstrId\":\"" + strRstrId                // 사용자 ID
                                        + "\"," + "\"updrId\":\"" + strUserId + "\"}";       // 사용자 ID

                //JsonWebServiceURL_GantryPicking
                var responseValue = BaseClass.PostSendJson(strConnUrl, strJsonParamValue);

                return true;
            }
            catch
            { 
                throw; 
            }
        }

        #endregion


    }
}

