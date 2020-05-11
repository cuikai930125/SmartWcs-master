using SMART.WCS.Common_Etc.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.Common_Etc.Logger
{
    public class Logger
    {
        #region ▩ 매개변수
        #region ▶ Readonly
        /// <summary>
        /// 로그파일 저장 용량 (5MB)
        /// </summary>
        private static readonly long MAX_LOG_SIZE = 5242880;

        /// <summary>
        /// 로그 파일 순번 자리수
        /// </summary>
        private static readonly int LOG_FILE_NO_DIGIT = 5;

        ///// <summary>
        ///// 프로그램이 실행되는 디렉토리 주소
        ///// </summary>
        private static readonly string START_PATH = System.Windows.Forms.Application.StartupPath;

        // CHOO
        //private static readonly string START_PATH = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        #endregion
        #endregion

        #region ▩ 호출 함수
        #region Error - 에러 발생시 로그를 작성한다.
        /// <summary>
        /// 에러 발생 시 로그를 작성한다.
        /// </summary>
        /// <param name="_exception">오류 (Exception) 객체 </param>
        public static void Error(System.Exception _exception)
        {
            try
            {
                string strDirName = string.Format("{0}\\{1}", START_PATH, "Log\\Error");                          // 로그 파일이 저장될 경로를 설정한다.
                string strFileName = System.DateTime.Now.ToString().Substring(0, 10).Replace("-", string.Empty);   // 현재일자를 파일명으로 설정한다.

                // 로그 폴더 / 파일 생성 로직을 처리한다.
                /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
				 * [0] 파일명 - 일자_순번.log
				 * [1] 기존 로그파일에 덮어쓰기 여부
				 *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/
                string[] arrFileInfo = CheckDirFile(strDirName, strFileName);

                // 오류 로그를 작성한다.
                CreateExceptionLog(_exception, arrFileInfo);
            }
            catch {  }
        }

        public static void Error(System.Exception _exception, string _strDbConnValue)
        {
            try
            {
                // 에러 로그를 작성한다.
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("Date/Time : [{0}]", System.DateTime.Now.ToString()));
                sb.Append(Microsoft.VisualBasic.ControlChars.CrLf);
                sb.Append(string.Format("Message : {0}", _exception.Message));
                sb.Append(Microsoft.VisualBasic.ControlChars.CrLf);
                sb.Append(string.Format("Source : {0}", _exception.Source));

                // Mac Address
                string strTerminalAddress       = HelperClass.GetMacAddress();
                // Error Message
                string strMessage               = sb.ToString();

                Dictionary<object, object> dicParam = new Dictionary<object, object>();
                dicParam.Add("P_TERMINAL_ADDR",         strTerminalAddress);
                dicParam.Add("P_TERMINAL_LOG",          strMessage);

                string[] arrOutputParam = { "P_RESULT" };

                //DataTable dtRtnValue = null;

                //using (DataAccess da = new DataAccess())
                //{
                //    try
                //    { 
                //        da.BeginTransaction(_strDbConnValue);

                //        dtRtnValue= da.GetSpDataTable(
                //                _strDbConnValue
                //            ,   "COMADM.PK_COMM_UI_RUN.SP_COMM_UI_ERR_LOG_INSERT"
                //            ,   dicParam
                //            ,   arrOutputParam
                //        );

                //        da.CommitTransaction();
                      
                //    }
                //    catch
                //    {
                //        da.RollbackTransaction();
                //        throw;
                //    }
                //}
            }
            catch { throw; }
        }
        #endregion

        #region Warning - 경고 로그를 작성한다.
        /// <summary>
        /// 경고 로그를 작성한다.
        /// </summary>
        /// <param name="_objWarning">경고 내용</param>
        public static void Warning(object _objWarning)
        {
            try
            {
                string strDirName       = string.Format("{0}\\{1}", START_PATH, "Log\\Warning");                            // 로그 파일이 저장될 경로를 설정한다.
                string strFileName      = System.DateTime.Now.ToString().Substring(0, 11).Replace("-", string.Empty);       // 현재일자를 파일명으로 설정한다.

                // 로그 폴더 / 파일 생성 로직을 처리한다.
                /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
				 * [0] 파일명 - 일자_순번.log
				 * [1] 기존 로그파일에 덮어쓰기 여부
				 *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/
                string[] arrFileInfo = CheckDirFile(strDirName, strFileName);

                // Warning 로그를 작성한다.
                CreateWarningLog(_objWarning.ToString(), arrFileInfo);
            }
            catch { throw; }
        }
        #endregion

        #region Information - 정보 로그를 작성한다.
        /// <summary>
        /// 정보 로그를 작성한다.
        /// </summary>
        /// <param name="_objInfo">정보 내용</param>
        public static void Info(object _objInfo)
        {
            try
            {
                string strDirName       = string.Format("{0}\\{1}", START_PATH, "Log\\Info");                               // 로그 파일이 저장될 경로를 설정한다.
                string strFileName      = System.DateTime.Now.ToString().Substring(0, 11).Replace("-", string.Empty);       // 현재일자를 파일명으로 설정한다.

                // 로그 폴더 / 파일 생성 로직을 처리한다.
                /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
				 * [0] 파일명 - 일자_순번.log
				 * [1] 기존 로그파일에 덮어쓰기 여부
				 *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/

                // handle thread sync...HK.Lee
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string[] arrFileInfo = CheckDirFile(strDirName, strFileName);

                    CreateInformationLog(_objInfo.ToString(), arrFileInfo);
                });

            }
            catch { throw; }
        }
        #endregion

        #region InputParamInfo - 데이터베이스 접속 파라메터 정보를 작성한다.
        /// <summary>
        /// 데이터베이스 접속 파라메터 정보를 작성한다.
        /// </summary>
        /// <param name="_objInfo">정보 내용</param>
        public static void InputParamInfo(object _objInfo)
        {
            try
            {
                string strDirName = string.Format("{0}\\{1}", START_PATH, "Log\\DBInputParamInfo");                               // 로그 파일이 저장될 경로를 설정한다.
                string strFileName = System.DateTime.Now.ToString().Substring(0, 11).Replace("-", string.Empty);       // 현재일자를 파일명으로 설정한다.

                // 로그 폴더 / 파일 생성 로직을 처리한다.
                /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
				 * [0] 파일명 - 일자_순번.log
				 * [1] 기존 로그파일에 덮어쓰기 여부
				 *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/

                // handle thread sync...HK.Lee
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string[] arrFileInfo = CheckDirFile(strDirName, strFileName);

                    CreateInformationLog(_objInfo.ToString(), arrFileInfo);
                });

            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region ▩ 내부 함수
        #region ▶ CheckDirfile - 로그폴더 / 파일 생성 로직을 처리한다.
        /// <summary>
        /// 로그 폴더 / 파일 생성 로직을 처리한다.
        /// <param name="_strDirName">로그 생성 폴더명</param>
        /// <param name="_strFileName">로그 파일명</param>
        /// </summary>
        /// <returns></returns>
        private static string[] CheckDirFile(string _strDirName, string _strFileName)
        {
            try
            {
                /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
				 * [0] 파일명 - 일자_순번.log
				 * [1] 기존 로그파일에 덮어쓰기 여부
				 *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/
                string[] arrRtnValue        = new string[2];
                arrRtnValue[1]              = "N";
                string strDirFileName       = string.Empty;       // 로그 경로 (폴더 + 파일명)

                // 폴더 중복 여부를 체크한다.
                if (System.IO.Directory.Exists(_strDirName) == false)
                {
                    // 폴더가 존재하지 않으면 로그를 저장하는 폴더를 생성한다.
                    System.IO.Directory.CreateDirectory(_strDirName);

                    // 최근 파일이 없는 경우 현재일자 + 000000 값으로 파일명을 설정한다.
                    arrRtnValue[0] = _strDirName + "\\" + _strFileName + "_" + Common_Etc.Data.DataConvert.GetAddZeroValue(0, LOG_FILE_NO_DIGIT);
                }
                else
                {
                    // 최근 파일명을 가져온다.
                    string strCurrentFileName = Common_Etc.File.File.GetLastFileName(_strDirName, _strFileName);

                    if (strCurrentFileName.Length == 0)
                    {
                        // 최근 파일이 없는 경우 현재일자 + 000000 값으로 파일명을 설정한다.
                        arrRtnValue[0] = _strDirName + "\\" + _strFileName + "_" + Common_Etc.Data.DataConvert.GetAddZeroValue(0, LOG_FILE_NO_DIGIT);
                    }
                    else
                    {
                        // 최근 파일이 존재하는 경우 파일크기를 체크하여 추가 생성 여부를 결정한다.
                        strDirFileName              = System.IO.Path.Combine(_strDirName, strCurrentFileName);
                        System.IO.FileInfo fi       = new System.IO.FileInfo(strDirFileName + ".log");
                        long lFileSize              = fi.Length;

                        if (MAX_LOG_SIZE < lFileSize)
                        {
                            // 로그 파일 크기가 기준 크기 (5MB)보다 큰 경우 새 파일을 생성하여 로그를 저장한다.
                            string strLastFileName      = strCurrentFileName.Split('_')[1].Split('.')[0];
                            _strFileName                = _strFileName + "_" + Common_Etc.Data.DataConvert.GetAddZeroValue(System.Convert.ToInt32(strLastFileName) + 1, LOG_FILE_NO_DIGIT);
                            arrRtnValue[0]              = System.IO.Path.Combine(_strDirName, _strFileName);
                        }
                        else
                        {
                            // 로그 파일 크기가 기준 크기 (5MB)보다 작은 경우 기존 로그 파일에 로그를 저장한다.
                            arrRtnValue[0] = strDirFileName;
                            arrRtnValue[1] = "Y";                   // 기존 파일에 덮어쓰는 경우 공백 처리를 위한 매개변수
                        } // if (파일 크기 체크)
                    } // if (기존 로그파일명 체크)
                } // if

                arrRtnValue[0] = string.Format("{0}.log", arrRtnValue[0]);

                return arrRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region ▶ CreateExceptionLog - Exception 로그를 작성한다.
        /// <summary>
        /// Exception 로그를 작성한다.
        /// </summary>
        /// <param name="_exception">Exception 오류 객체</param>
        /// <param name="_arrFileInfo">파일 정보</param>
        private static void CreateExceptionLog(System.Exception _exception, string[] _arrFileInfo)
        {
            try
            {
                /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
				 * [0] 파일명 - 일자_순번.log
				 * [1] 기존 로그파일에 덮어쓰기 여부
				 *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/
                string strDirFileName   = _arrFileInfo[0];  // 로그 작성 폴더/파일명
                string strOverWriteYN   = _arrFileInfo[1];  // 기존 로그 파일에 쓰기 여부

                using (System.IO.FileStream fs = new System.IO.FileStream(strDirFileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
                    {

                    }
                }

                // 에러 로그를 작성한다.
                using (System.IO.FileStream fs = new System.IO.FileStream(strDirFileName, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
                    {
                        if (strOverWriteYN == "Y")
                        {
                            sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                            sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                            sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                        }

                        sw.Write(string.Format("Date/Time : [{0}]", System.DateTime.Now.ToString()));
                        sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                        sw.Write("Message : " + _exception.Message);
                        sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                        sw.Write("Source : " + _exception.Source);
                        sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                        sw.Write("StackTrace : " + _exception.StackTrace);
                        sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);

                        if (_exception.InnerException != null)
                        {
                            sw.Write("Inner Exception : " + _exception.InnerException);
                            sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                        }

                        sw.Write("====================================================================================================");
                        sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                    }
                }
            }
            catch (System.Exception err)
            {
                throw err;
            }
        }
        #endregion

        #region ▶ CreateWarningLog - Warning 로그를 작성한다.
        /// <summary>
        /// Warning 로그를 작성한다.
        /// </summary>
        /// <param name="_strWarningLog">Warning 로그</param>
        /// <param name="_arrFileInfo">파일 정보</param>
        private static void CreateWarningLog(string _strWarningLog, string[] _arrFileInfo)
        {
            try
            {
                /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
				 * [0] 파일명 - 일자_순번.log
				 * [1] 기존 로그파일에 덮어쓰기 여부
				 *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/
                string strDirFileName       = _arrFileInfo[0];      // 로그 작성 폴더/파일명
                string strOverWriteYN       = _arrFileInfo[1];      // 기존 로그 파일에 쓰기 여부

                using (System.IO.FileStream fs = new System.IO.FileStream(strDirFileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
                    {

                    }
                }

                // 에러 로그를 작성한다.
                using (System.IO.FileStream fs = new System.IO.FileStream(strDirFileName, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
                    {
                        if (strOverWriteYN == "Y")
                        {
                            sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                            sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                            sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                        }

                        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                        string strTrace = string.Empty;

                        for (int i = st.FrameCount - 1; i > 2; i--)
                        {
                            System.Diagnostics.StackFrame sf        = new System.Diagnostics.StackFrame();
                            string strType                          = sf.GetMethod().ReflectedType.FullName;
                            string strMethod                        = sf.GetMethod().ToString();
                            string strSource                        = sf.GetFileName();
                            int iLine                               = sf.GetFileLineNumber();

                            if (!((strType.IndexOf("Microsoft.") > -1) || (strType.IndexOf("System.") > -1)))
                            {
                                strTrace += string.Format("\r\n   at {0}.{1} in {2}:line {3}", strType, strMethod, strSource, iLine);
                            }
                        }

                        sw.Write(string.Format("Date/Time : [{0}]", System.DateTime.Now.ToString()));
                        sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                        sw.Write("Message : " + _strWarningLog);
                        sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                        sw.Write("StackTrace : " + strTrace);
                        sw.Write("====================================================================================================");
                        sw.Write("StackTrace : " + strTrace);
                    }
                }
            }
            catch (System.Exception err)
            {
                throw err;
            }
        }
        #endregion

        #region ▶ CreateInformationLog - Information 로그를 작성한다.
        /// <summary>
        /// Information 로그를 작성한다.
        /// </summary>
        /// <param name="_strInfoLog">Information 로그</param>
        /// <param name="_arrFileInfo">파일 정보</param>
        private static void CreateInformationLog(string _strInfoLog, string[] _arrFileInfo)
        {
            try
            {
                /*▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩
				 * [0] 파일명 - 일자_순번.log
				 * [1] 기존 로그파일에 덮어쓰기 여부
				 *▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩*/
                string strDirFileName       = _arrFileInfo[0];      // 로그 작성 폴더/파일명
                string strOverWriteYN       = _arrFileInfo[1];      // 기존 로그 파일에 쓰기 여부

                using (System.IO.FileStream fs = new System.IO.FileStream(strDirFileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
                    {

                    }
                }

                // 에러 로그를 작성한다.
                using (System.IO.FileStream fs = new System.IO.FileStream(strDirFileName, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
                    {
                        if (strOverWriteYN == "Y")
                        {
                            sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                            sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                            sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                        }

                        sw.Write(string.Format("Date/Time : [{0}]", System.DateTime.Now.ToString()));
                        sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                        sw.Write("Message : " + _strInfoLog);
                        sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                        sw.Write("====================================================================================================");
                        sw.Write(Microsoft.VisualBasic.ControlChars.CrLf);
                    }
                }
            }
            catch (System.Exception err)
            {
                throw err;
            }
        }
        #endregion
        #endregion
    }
}
