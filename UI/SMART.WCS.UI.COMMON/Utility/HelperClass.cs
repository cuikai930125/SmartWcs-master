using Newtonsoft.Json;
using SMART.WCS.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.COMMON.Utility
{
    public class HelperClass : DisposeClass
    {
        static BaseClass BaseClass = new BaseClass();

        #region > GetSendJson - Get Json
        /// <summary>
        /// Get Json
        /// </summary>
        /// <param name="_strUrl">접속 Url</param>
        /// <returns></returns>
        public static string GetSendJson(string _strUrl)
        {
            try
            {
                string strRtnValue      = string.Empty;

                // 헤더키 
                var strHeaderTokenKey   = BaseClass.DecryptAES256(BaseClass.GetAppSettings("HeaderKey"));
                // 헤더값
                var strHeaderTokenValue = BaseClass.DecryptAES256(BaseClass.GetAppSettings("HeaderValue"));

                HttpWebRequest request      = (HttpWebRequest)WebRequest.Create(_strUrl);
                request.ContentType         = "application/x-www-form-urlencoded";
                request.Method              = "GET";
                request.Headers.Add(strHeaderTokenKey, strHeaderTokenValue);

                HttpWebResponse response;
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding("UTF-8"), true))
                        {
                            strRtnValue     = reader.ReadToEnd();
                            strRtnValue     = JsonConvert.DeserializeObject(strRtnValue).ToString();
                        }
                    }
                }

                return strRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > PostSendJson - Post Json 
        /// <summary>
        /// Post Json 
        /// </summary>
        /// <param name="_strUrl">접속 Url</param>
        /// <param name="_strParamValue">Parameter값</param>
        /// <returns></returns>
        public static HttpWebResponse PostSendJson(string _strUrl, string _strParamValue)
        {
            try
            {
                HttpWebResponse response;

                // 헤더키
                var strHeaderTokenKey   = BaseClass.DecryptAES256(BaseClass.GetAppSettings("HeaderKey"));
                // 헤더값
                var strHeaderTokenValue = BaseClass.DecryptAES256(BaseClass.GetAppSettings("HeaderValue"));

                HttpWebRequest request  = (HttpWebRequest)WebRequest.Create(_strUrl);
                request.ContentType     = "application/json";
                request.Method          = "POST";
                request.Headers.Add(strHeaderTokenKey, strHeaderTokenValue);

                using (StreamWriter stream = new StreamWriter(request.GetRequestStream()))
                {
                    stream.Write(_strParamValue);
                    stream.Flush();
                    stream.Close();
                }

                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding("UTF-8"), true))
                        {
                            return response;
                        }
                    }
                }
            }
            catch { throw; }
        }
        #endregion
    }
}
