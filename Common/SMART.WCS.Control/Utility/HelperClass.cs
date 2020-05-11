using DevExpress.Xpf.Core;
using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SMART.WCS.Control.Utility
{
    public class HelperClass
    {
        #region > GetMessageByCommon - 공통 메세지를 조회한다.
        /// <summary>
        /// 공통메세지를 조회한다.
        /// </summary>
        /// <param name="_strMessageCode">메세지 코드</param>
        /// <returns></returns>
        public static string GetMessageByCommon(string _strMessageCode)
        {
            try
            {
                //BaseInfo baseInfo       = ((BaseApp)Application.Current).BASE_INFO;
                //var strMessage          = string.Empty;
                //var query               = baseInfo.COMMON_MESSAGE_LIST.AsEnumerable().Where(p => p.Field<string>("MSG_CD").Equals(_strMessageCode)).FirstOrDefault();

                //if (query == null)
                //{
                //    strMessage = "[오류] 화면에 출력할 메세지가 없습니다.";
                //}
                //else
                //{
                //    strMessage = query[0].ToString();
                //}

                //return strMessage;

                return string.Empty;
            }
            catch { throw; }            
        }
        #endregion

        #region > GetMessageByScreen - 화면내 메세지를 조회한다.
        /// <summary>
        /// 화면내 메세지를 조회한다.
        /// </summary>
        /// <param name="_strMessageCode">메세지 코드</param>
        /// <returns></returns>
        public static string GetMessageByScreen(string _strMessageCode)
        {
            try
            {
                //BaseInfo baseInfo       = ((BaseApp)Application.Current).BASE_INFO;
                //var strMessage          = string.Empty;

                //var query               = baseInfo.SCREEN_MESSAGE_LIST.AsEnumerable().Where(p => p.Field<string>("MSG_CD").Equals(_strMessageCode)).FirstOrDefault();

                //if (query == null)
                //{
                //    strMessage = "[오류] 화면에 출력할 메세지가 없습니다.";
                //}
                //else
                //{
                //    strMessage = query[0].ToString();
                //}

                //return strMessage;

                return string.Empty;
            }
            catch { throw; }
        }
        #endregion

        #region > GetMultiMessageByCommon - 공통 멀티Row 메세지를 조회한다.
        /// <summary>
        /// 공통 멀티Row 메세지를 조회한다.
        /// </summary>
        /// <param name="_strMessageCode">메세지 코드</param>
        /// <param name="_strMessageValue">메세지 값</param>
        /// <returns></returns>
        public static string GetMultiMessageByCommon(string _strMessageCode, string _strMessageValue)
        {
            try
            {
                //// 예) 
                //// _strMessageCode : MANDATORY_INPUT - {0}와 {1}는 필수로 입력해야 합니다.
                //// _strMessageValue : 고객사|거래처
                //// 결과 : 고객사와 거래처는 필수로 입력해야 합니다. 

                //BaseInfo baseInfo       = ((BaseApp)Application.Current).BASE_INFO;
                //var strMessage          = string.Empty;

                //var query               = baseInfo.COMMON_MESSAGE_LIST.AsEnumerable().Where(p => p.Field<string>("MSG_CD").Equals(_strMessageCode)).FirstOrDefault();

                //if (query == null)
                //{
                //    strMessage = "[오류] 화면에 출력할 메세지가 없습니다.";
                //}
                //else
                //{
                //    for (int i = 0; i < _strMessageValue.Split('|').Length; i++)
                //    {
                //        strMessage = query[0].ToString().Replace("{" + i.ToString() + "}", _strMessageValue.Split('|')[i]);
                //    }
                //}

                //return strMessage;

                return string.Empty;
            }
            catch { throw; }
        }
        #endregion

        #region > GetMultiScreenByCommon - 화면내 멀티Row 메세지를 조회한다.
        /// <summary>
        /// 화면내 멀티Row 메세지를 조회한다.
        /// </summary>
        /// <param name="_strMessageCode">메세지 코드</param>
        /// <param name="_strMessageValue">메세지 값</param>
        /// <returns></returns>
        public static string GetMultiScreenByCommon(string _strMessageCode, string _strMessageValue)
        {
            try
            {
                //// 예) 
                //// _strMessageCode : MANDATORY_INPUT - {0}와 {1}는 필수로 입력해야 합니다.
                //// _strMessageValue : 고객사|거래처
                //// 결과 : 고객사와 거래처는 필수로 입력해야 합니다. 

                //BaseInfo baseInfo       = ((BaseApp)Application.Current).BASE_INFO;
                //var strMessage          = string.Empty;

                //var query               = baseInfo.SCREEN_MESSAGE_LIST.AsEnumerable().Where(p => p.Field<string>("MSG_CD").Equals(_strMessageCode)).FirstOrDefault();

                //if (query == null)
                //{
                //    strMessage = "[오류] 화면에 출력할 메세지가 없습니다.";
                //}
                //else
                //{
                //    for (int i = 0; i < _strMessageValue.Split('|').Length; i++)
                //    {
                //        strMessage = query[0].ToString().Replace("{" + i.ToString() + "}", _strMessageValue.Split('|')[i]);
                //    }
                //}

                //return strMessage;

                return string.Empty;
            }
            catch { throw; }
        }
        #endregion

        public static void SetMessageListByScreen(string _strMenuID, DataTable _dtNewMessage)
        {
            try
            {
                //if (_dtNewMessage.Rows.Count == 0) { return; }

                //SMART.WCS.Control.BaseInfo _baseInfo = ((BaseApp)System.Windows.Application.Current).BASE_INFO;
                //if (_baseInfo.SCREEN_MESSAGE_LIST == null)
                //{
                //    _baseInfo.SCREEN_MESSAGE_LIST = _dtNewMessage.Clone();
                //}

                //foreach (DataRow dr in _dtNewMessage.Rows)
                //{
                //    DataRow drNewRow        = _baseInfo.SCREEN_MESSAGE_LIST.NewRow();
                //    drNewRow["MENU_ID"]     = _strMenuID;
                //    drNewRow["MSG_CD"]      = dr["MSG_CD"];
                //    drNewRow["CNTRY_CD"]    = dr["CNTRY_CD"];
                //    drNewRow["MSG"]         = dr["MSG"];
                //    _baseInfo.SCREEN_MESSAGE_LIST.Rows.Add(drNewRow);
                //}
            }
            catch { throw; }
        }

        #region GetCultureInfo - 국가코드와 일치하는 언어 리소스를 가져온다.
        /// <summary>
        /// 국가코드와 일치하는 언어 리소스를 가져온다.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetCultureInfo()
        {
            BaseClass BaseClass = new BaseClass();

            CultureInfo ci = null;

            switch (BaseClass.CountryCode)
            {
                case "KR":
                    ci = new CultureInfo("ko-KR");
                    break;
                case "EN":
                    ci = new CultureInfo("en-US");
                    break;
                case "CN":
                    break;
                case "TH":
                    ci = new CultureInfo("th-TH");
                    break;
            }

            return ci;
        }
        #endregion

        /// <summary>
        /// 버튼 IsEnabled 속성 및 버튼 커서 속성을 정의한다.
        /// </summary>
        /// <param name="_objButton"></param>
        /// <param name="_bEnabled"></param>
        public static void SetButtonIsEnabled(SimpleButton _objButton, bool _bEnabled)
        {
            try
            {
                if (_bEnabled == true)
                {
                    _objButton.IsEnabled        = true;
                    _objButton.Cursor           = Cursors.Hand;
                }
                else
                {
                    _objButton.IsEnabled        = false;
                    _objButton.Cursor           = Cursors.None;
                }
            }
            catch { throw; }
        }
    }
}
