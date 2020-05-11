using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SMART.WCS.Common.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common.Communication
{
    public class JsonWebService
    {
        BaseClass BaseClass = new BaseClass();

        #region > CallWebServiceNotSendSorter - 미전송 데이터 재전송 웹서비스 호출
        public void CallWebServiceNotSendSorter(DataTable _dtParam)
        {
            try
            {
                List<JSonNotSendSorterClass> liJsonClass = new List<JSonNotSendSorterClass>();

                foreach (DataRow drRow in _dtParam.Rows)
                {
                    JSonNotSendSorterClass jsonItem = new JSonNotSendSorterClass();
                    jsonItem.sortingId      = drRow["EQP_ID"].ToString();
                    jsonItem.trayCode       = drRow["CART_NO"].ToString();
                    jsonItem.invoiceNumber  = drRow["INV_BCD"].ToString();
                    jsonItem.boxCode        = drRow["BOX_BCD"].ToString();
                    jsonItem.sortingCode    = drRow["RGN_BCD"].ToString();
                    jsonItem.scanTime       = Convert.ToDateTime(drRow["SCAN_DT"]).ToString("yyyyMMddHHmmss");
                    jsonItem.sortTime       = Convert.ToDateTime(drRow["SRT_WRK_CMPT_DT"]).ToString("yyyyMMddHHmmss");
                    jsonItem.chuteNumber    = drRow["RSLT_CHUTE_ID"].ToString();
                    jsonItem.turnNumber     = Convert.ToInt32(drRow["RECIRC_CNT"]);
                    jsonItem.imagePath      = string.Empty;
                    jsonItem.errCode        = drRow["SRT_ERR_CD"].ToString();

                    liJsonClass.Add(jsonItem);
                }

                if (liJsonClass.Count() > 0)
                {
                    string strJSonValue = JsonConvert.SerializeObject(liJsonClass);
                }
            }
            catch { throw; }
        }
        #endregion

        #region ▩ 함수
        public void GetIF_SRT_RSLT_INQ()
        {
            try
            {
                #region + 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue                          = null;
                var strProcedureName                        = "PK_IF_SRT_RSLT_HUB_S.SP_IF_SRT_RSLT_INQ";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_IF_SND_LIST", "O_RSLT" };
                
                var strCenterCD         = this.BaseClass.CenterCD;          // 센터코드
                var strEqpID            = string.Empty;                     // 설비 ID
                var strYmd              = string.Empty;
                var strPID              = string.Empty;
                #endregion

                #region + Input 파라메터
                dicInputParam.Add("P_CNTR_CD",          strCenterCD);
                dicInputParam.Add("P_EQP_ID",           strEqpID);
                dicInputParam.Add("P_INDT_YMD",         strYmd);
                dicInputParam.Add("P_PID",              strPID);
                #endregion

                #region + 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    //await System.Threading.Tasks.Task.Run(() =>
                    //{
                        dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                    //}).ConfigureAwait(true);
                }
                #endregion

                if (dsRtnValue == null) { return; }

                string strErrCode           = string.Empty;     // 오류 코드
                string strErrMsg            = string.Empty;     // 오류 메세지

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    this.CallWebServiceNotSendSorter(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류 발생시 처리
                }
            }
            catch { throw; }
        }

        #endregion
    }
}
