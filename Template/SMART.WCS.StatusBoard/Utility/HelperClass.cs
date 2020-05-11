using SMART.WCS.Common;
using SMART.WCS.StatusBoard.Control;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using static SMART.WCS.StatusBoard.Control.uKioskMsgBox;

namespace SMART.WCS.StatusBoard.Utility
{
    public class HelperClass : IDisposable
    {
        static BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 팝업창에서 확인 버튼 클릭값 저장
        /// true : 확인, false : 취소
        /// </summary>
        public static bool BUTTON_CONFIRM_YN { get; set; }


        #region > 컨트롤 관련 (공통 메세지 박스)
        #region >> MsgInfo - Information 메세지 리소스 설정 및 메세지 박스 호출
        /// <summary>
        /// Information 메세지 리소스 설정 및 메세지 박스 호출
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        public static void MsgInfo(string _strMessageResourceCode)
        {
            MsgInfo(_strMessageResourceCode, string.Empty);
        }
        #endregion

        #region MsgInfo - Information 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// <summary>
        /// Information 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// </summary>
        /// <param name="_strMessage">팝업 메세지</param>
        /// <param name="_enumCodeMessage">코드, 메세지 여부</param>
        public static void MsgInfo(string _strMessage, BaseEnumClass.CodeMessage _enumCodeMessage)
        {
            if (_enumCodeMessage == BaseEnumClass.CodeMessage.CODE)
            {
                MsgInfo(_strMessage);
            }
            else
            {
                using (uKioskMsgBox frmInformation = new uKioskMsgBox(_strMessage, MsgBoxKind.Information))
                {
                    frmInformation.ClickResult += FrmMsgBox_ClickResult;
                    frmInformation.ShowDialog();
                }
            }
        }
        #endregion

        #region >> MsgInfo - Information 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// <summary>
        /// Information 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        /// <param name="_strConditionValue">멀티 항목 조건값</param>
        public static void MsgInfo(string _strMessageResourceCode, string _strConditionValue)
        {
            string strMessage = string.Empty;

            if (_strConditionValue.Length == 0)
            {
                strMessage = ConvertMsgBoxValue(_strMessageResourceCode);
            }
            else
            {
                strMessage = ConvertMsgBoxValue(_strMessageResourceCode, _strConditionValue);
            }

            using (uKioskMsgBox frmInformation = new uKioskMsgBox(strMessage, MsgBoxKind.Information))
            {
                frmInformation.ClickResult += FrmMsgBox_ClickResult;
                frmInformation.ShowDialog();
            }
        }
        #endregion

        #region >> MsgError - Error 메세지 리소스 설정 및 메세지 박스 호출
        /// <summary>
        /// Error 메세지 리소스 설정 및 메세지 박스 호출
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        public static void MsgError(string _strMessageResourceCode, bool _autoClose = false)
        {
            MsgError(_strMessageResourceCode, string.Empty, _autoClose);
        }
        #endregion

        #region >> MsgError - Error 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// <summary>
        /// Error 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// </summary>
        /// <param name="_strMessage">팝업 메세지</param>
        /// <param name="_enumCodeMessage">코드, 메세지 구분</param>
        public static void MsgError(string _strMessage, BaseEnumClass.CodeMessage _enumCodeMessage, bool _autoClose = false)
        {
            if (_enumCodeMessage == BaseEnumClass.CodeMessage.CODE)
            {
                MsgError(_strMessage, _autoClose);
            }
            else
            {
                using (uKioskMsgBox frmError = new uKioskMsgBox(_strMessage, MsgBoxKind.Error, _autoClose))
                {
                    frmError.ClickResult += FrmMsgBox_ClickResult;
                    frmError.ShowDialog();
                }
            }
        }
        #endregion

        #region >> MsgError - Error 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// <summary>
        /// Error 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        /// <param name="_strConditionValue">멀티 항목 조건값</param>
        public static void MsgError(string _strMessageResourceCode, string _strConditionValue, bool _autoClose = false)
        {
            string strMessage = string.Empty;

            if (_strConditionValue.Length == 0)
            {
                strMessage = ConvertMsgBoxValue(_strMessageResourceCode);
            }
            else
            {
                strMessage = ConvertMsgBoxValue(_strMessageResourceCode, _strConditionValue);
            }

            using (uKioskMsgBox frmError = new uKioskMsgBox(strMessage, MsgBoxKind.Error, _autoClose))
            {
                frmError.ClickResult += FrmMsgBox_ClickResult;
                frmError.ShowDialog();
            }
        }
        #endregion

        #region >> MsgQuestion - Question 메세지 리소스 설정 및 메세지 박스 호출
        /// <summary>
        /// Question 메세지 리소스 설정 및 메세지 박스 호출
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        public static void MsgQuestion(string _strMessageResourceCode)
        {
            MsgQuestion(_strMessageResourceCode, string.Empty);
        }
        #endregion

        #region >> MsgQuestion - Question 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// <summary>
        /// Question 메세지 처리 (코드가 아닌 메세지로 팝업창을 출력하는 경우 사용)
        /// </summary>
        /// <param name="_strMessage">팝업 메세지</param>
        /// <param name="_enumCodeMessage">코드, 메세지 구분</param>
        public static void MsgQuestion(string _strMessage, BaseEnumClass.CodeMessage _enumCodeMessage)
        {
            if (_enumCodeMessage == BaseEnumClass.CodeMessage.CODE)
            {
                MsgQuestion(_strMessage);
            }
            else
            {
                using (uKioskMsgBox frmQuestion = new uKioskMsgBox(_strMessage, MsgBoxKind.Question))
                {
                    frmQuestion.ClickResult += FrmMsgBox_ClickResult;
                    frmQuestion.ShowDialog();
                }
            }
        }
        #endregion

        #region >> MsgQuestion - Question 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// <summary>
        /// Question 메세지 리소스 설정 및 메세지 박스 호출 (박스 멀티 항목 처리)
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        /// <param name="_strConditionValue">멀티 항목 조건값</param>
        public static void MsgQuestion(string _strMessageResourceCode, string _strConditionValue)
        {
            string strMessage = string.Empty;

            if (_strConditionValue.Length == 0)
            {
                // 단일 메세지 처리
                strMessage = ConvertMsgBoxValue(_strMessageResourceCode);
            }
            else
            {
                // 멀티 항목 메세지 처리
                strMessage = ConvertMsgBoxValue(_strMessageResourceCode, _strConditionValue);
            }

            using (uKioskMsgBox frmQuestion = new uKioskMsgBox(strMessage, MsgBoxKind.Question))
            {
                frmQuestion.ClickResult += FrmMsgBox_ClickResult;
                frmQuestion.ShowDialog();
            }
        }
        #endregion

        #region >> Question 메시지 박스 Yes, No 여부값
        private static void FrmMsgBox_ClickResult(bool _bResult)
        {
            BUTTON_CONFIRM_YN = _bResult;
        }
        #endregion
        #endregion

        #region  > 내부함수
        #region >> ConvertMsgBoxValue - 메세지 박스 값 코드에 맞는 리소스 값을 반환한다.
        /// <summary>
        /// 메세지 박스 값 코드에 맞는 리소스 값을 반환한다.
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        /// <returns></returns>
        private static string ConvertMsgBoxValue(string _strMessageResourceCode)
        {
            return GetResourceValue(_strMessageResourceCode, BaseEnumClass.ResourceType.MESSAGE);
        }
        #endregion

        #region >> ConvertMsgBoxValue - 메세지 박스 값 코드에 맞는 리소스 값을 반환한다.
        /// <summary>
        /// 메세지 박스 값 코드에 맞는 리소스 값을 반환한다.
        /// </summary>
        /// <param name="_strMessageResourceCode">메세지 리소스 코드</param>
        /// 
        /// <returns></returns>
        private static string ConvertMsgBoxValue(string _strMessageResourceCode, string _strConditionValue)
        {
            string strResourceValue = string.Empty;
            string strConditionResourceValue = string.Empty;
            string strMessageValue = GetResourceValue(_strMessageResourceCode, BaseEnumClass.ResourceType.MESSAGE);

            for (int i = 0; i < _strConditionValue.Split('|').Length; i++)
            {
                strConditionResourceValue += GetResourceValue(_strConditionValue.Split('|')[i]) + "|";
            }

            if (strConditionResourceValue.Length > 0)
            {
                strConditionResourceValue = strConditionResourceValue.Substring(0, strConditionResourceValue.Length - 1);
            }


            for (int i = 0; i < strConditionResourceValue.Split('|').Length; i++)
            {

                strMessageValue = strMessageValue.Replace("{" + i.ToString() + "}", strConditionResourceValue.Split('|')[i]);
            }

            return strMessageValue;
        }
        #endregion

        #region >> 화면에 출력하는 메세지 조회 (공통 메세지, 화면별 메세지)
        #region >>> GetMessageByCommon - 공통 메세지 조회
        public static string GetMessageByCommon(string _strMessageCode)
        {
            try
            {
                return string.Empty;
            }
            catch { throw; }
        }
        #endregion

        #region >>> GetResourceValue - 리소스 정보를 조회한다. (언어코드가 없는 경우)
        /// <summary>
        /// 리소스 정보를 조회한다. (언어코드가 없는 경우)
        /// </summary>
        /// <param name="_strCodeValue">코드값</param>
        /// <returns></returns>
        public static string GetResourceValue(string _strCodeValue)
        {
            return GetResourceValue(_strCodeValue, BaseEnumClass.ResourceType.NORMAL);
        }
        #endregion

        #region >>> GetResourceValue - 리소스 정보를 조회한다. (언어코드가 없는 경우)
        /// <summary>
        /// 리소스 정보를 조회한다. (언어코드가 없는 경우)
        /// </summary>
        /// <param name="_strCodeValue">코드값</param>
        /// <param name="_enumResourceType">리소스 타입</param>
        /// <returns></returns>
        public static string GetResourceValue(string _strCodeValue, BaseEnumClass.ResourceType _enumResourceType)
        {
            switch (_enumResourceType)
            {
                case BaseEnumClass.ResourceType.MESSAGE:
                    _strCodeValue = $"(MSG){_strCodeValue}";
                    break;
                default: break;
            }

            ResourceManager rm = new ResourceManager("SMART.WCS.Resource.Language.LanguageResource", typeof(SMART.WCS.Resource.Language.LanguageResource).Assembly);
            CultureInfo cultureInfo = HelperClass.GetCountryName(BaseClass.CountryCode);
            string strResourceInfo;

            if (rm.GetString(_strCodeValue, cultureInfo) == null)
            {
                strResourceInfo = _strCodeValue;
            }
            else
            {
                if (rm.GetString(_strCodeValue, cultureInfo).Length == 0)
                {
                    strResourceInfo = _strCodeValue;
                }
                else
                {
                    strResourceInfo = rm.GetString(_strCodeValue, cultureInfo);
                }
            }

            return strResourceInfo;
        }
        #endregion

        #region >>> GetResourceValue - 리소스 정보를 조회한다. (언어코드를 파라메터로 설정하는 경우)
        /// <summary>
        /// 리소스 정보를 조회한다.
        /// </summary>
        /// <param name="_strCodeValue">코드값</param>
        /// <param name="_strCntryCode">국가코드</param>
        /// <returns></returns>
        public static string GetResourceValue(string _strCodeValue, string _strCntryCode)
        {
            return GetResourceValue(_strCodeValue, BaseEnumClass.ResourceType.NORMAL, _strCntryCode);
        }
        #endregion

        #region >>> GetResourceValue - 리소스 정보를 조회한다. (언어코드를 파라메터로 설정하는 경우)
        /// <summary>
        /// 리소스 정보를 조회한다.
        /// </summary>
        /// <param name="_strCodeValue">코드값</param>
        /// <param name="_enumResourceType">리소스 타입</param>
        /// <param name="_strCntryCode">국가코드</param>
        /// <returns></returns>
        public static string GetResourceValue(string _strCodeValue, BaseEnumClass.ResourceType _enumResourceType, string _strCntryCode)
        {
            switch (_enumResourceType)
            {
                case BaseEnumClass.ResourceType.MESSAGE:
                    _strCodeValue = $"(MSG){_strCodeValue}";
                    break;
                default: break;
            }

            // 리소스 코드가 없는 경우 공백으로 리턴한다.
            if (_strCodeValue.Length == 0) { return string.Empty; }

            ResourceManager rm = new ResourceManager("SMART.WCS.Resource.Language.LanguageResource", typeof(SMART.WCS.Resource.Language.LanguageResource).Assembly);
            CultureInfo cultureInfo = HelperClass.GetCountryName(_strCntryCode);
            string strResourceInfo;

            if (rm.GetString(_strCodeValue, cultureInfo) == null)
            {
                strResourceInfo = _strCodeValue;
            }
            else
            {
                if (rm.GetString(_strCodeValue, cultureInfo).Length == 0)
                {
                    strResourceInfo = _strCodeValue;
                }
                else
                {
                    strResourceInfo = rm.GetString(_strCodeValue, cultureInfo);
                }
            }

            return strResourceInfo;
        }
        #endregion

        #region >>> GetCultureInfo - 국가코드와 일치하는 언어 리소스를 가져온다.
        /// <summary>
        /// 국가코드와 일치하는 코드를 가져온다.
        /// </summary>
        /// <returns></returns>
        public static CultureInfo GetCountryName(string _strCntryCode)
        {
            CultureInfo ci = null;

            switch (_strCntryCode)
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
        #endregion
        #endregion

        public static bool SetConfig(string Property, string Value)
        {
            try
            {
                WriteIniFile("ScannerSetting", Property, Value);
                return true;
            }
            catch (Exception ex)
            {
                BaseClass.Error(ex);
                return false;
            }
        }

        public static string GetConfig(string Property)
        {
            return GetIniFile("ScannerSetting", Property);
        }

        #region WriteIniFile - INI파일 작성
        /// <summary>
        /// INI파일 작성
        /// </summary>
        /// <param name="_strFileName">파일명</param>
        /// <param name="_strKeyName">키값</param>
        /// <param name="_strValue">저장 대상값</param>
        private static void WriteIniFile(string _strFileName, string _strKeyName, string _strValue)
        {
            try
            {
                SMART.WCS.Common.File.File.WriteIniFile(_strFileName, _strKeyName, _strValue);
            }
            catch { throw; }
        }


        #endregion

        #region GetIniFile - INI파일 읽기
        /// <summary>
        /// INI파일 읽기
        /// </summary>
        /// <param name="_strFileName">파일명</param>
        /// <param name="_strKeyName">키값</param>
        /// <returns></returns>
        private static string GetIniFile(string _strFileName, string _strKeyName)
        {
            try
            {
                return SMART.WCS.Common.File.File.ReadIniFile(_strFileName, _strKeyName);
            }
            catch { throw; }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~HelperClass()
        // {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
