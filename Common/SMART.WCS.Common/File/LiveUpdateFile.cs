using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SMART.WCS.Common.File
{
    public class LiveUpdateFile : DisposeClass
    {
        #region > GetServerVersion - Server 파일 버전 체크
        /// <summary>
        /// Server 파일 버전 체크
        /// </summary>
        /// <param name="_strURL"></param>
        /// <returns></returns>
        public static string GetServerVersion(string _strURL)
        {
            try
            {
                string strRtnValue      = string.Empty;

                HttpWebRequest request      = (HttpWebRequest)WebRequest.Create(_strURL);
                HttpWebResponse response    = (HttpWebResponse)request.GetResponse();
                Stream receiveStream        = response.GetResponseStream();

                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    var strStreamReadToEnd  = readStream.ReadToEnd();
                    response.Close();

                    if (File.ValidateFile(strStreamReadToEnd))
                    {
                        strRtnValue = strStreamReadToEnd;
                    }
                }

                return strRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > GetServerVersion - Server 파일 버전 체크
        /// <summary>
        /// Server 파일 버전 체크
        /// </summary>
        /// <param name="_strURL"></param>
        /// <returns></returns>
        public static string GetLocalVersion(string _strFilePath)
        {
            try
            {
                string strRtnValue = string.Empty;

                if (string.IsNullOrEmpty(_strFilePath) 
                    || Path.GetInvalidPathChars().Intersect(_strFilePath.ToCharArray()).Count() != 0
                    || (new FileInfo(_strFilePath).Exists == false))
                {
                    strRtnValue = null;
                }
                else if (new FileInfo(_strFilePath).Exists == true)
                {
                    var strFileReadAllText = System.IO.File.ReadAllText(_strFilePath);

                    if (File.ValidateFile(strFileReadAllText) == true)
                    {
                        strRtnValue = strFileReadAllText;
                    }
                    else
                    {
                        strRtnValue = null;
                    }
                }

                return strRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > CreateLocalVersionFile - 로컬 버전파일 생성
        /// <summary>
        /// 로컬 버전파일 생성
        /// </summary>
        /// <param name="_strFolderPath">폴더 경로</param>
        /// <param name="_strFileName">파일명</param>
        /// <param name="_strVersion">버전</param>
        /// <returns></returns>
        public static string CreateLocalVersionFile(string _strFolderPath, string _strFileName, string _strVersion)
        {
            DirectoryInfo directoryInfo;
            FileInfo fileInfo;

            try
            {
                var strPath     = $"{_strFolderPath}\\{_strFileName}";

                directoryInfo   = new DirectoryInfo(_strFolderPath);
                fileInfo        = new FileInfo(strPath);

                if (directoryInfo.Exists == false)
                { 
                    Directory.CreateDirectory(_strFolderPath);
                }

                if (fileInfo.Exists == true)
                {
                    fileInfo.Delete();
                }
                else
                {
                    System.IO.File.WriteAllText(strPath, _strVersion);
                }

                return strPath;
            }
            catch { throw; }
        }
        #endregion

        public static string CreateTargetLocation(string _strDownloadToPath, string _strVersion)
        {
            try
            {
                var strFilePath     = $"{_strDownloadToPath}{_strVersion}";

                if (_strDownloadToPath.EndsWith("\\") == false)
                {
                    _strDownloadToPath = $"{_strDownloadToPath}\\";
                }

                DirectoryInfo newDirectory = new DirectoryInfo(strFilePath);

                if (newDirectory.Exists == false)
                {
                    newDirectory.Create();
                }

                return strFilePath;
            }
            catch { throw; }
        }

        #region > GetTargetDirInfo - 대상 폴더의 정보를 가져온다.
        /// <summary>
        /// 대상 폴더의 정보를 가져온다.
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sAppNM"></param>
        /// <returns></returns>
        public static string[] GetTargetDirInfo(string _strURL, string _strAppName)
        {
            List<string> dir = new List<string>(30);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_strURL + _strAppName);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string html = reader.ReadToEnd();

                        Regex regexObj              = new Regex("<a[^>]*? href=\"(?<sURL>[^\"]+)\"[^>]*?>(?<dirName>.*?)</a>", RegexOptions.IgnoreCase);
                        MatchCollection matches     = regexObj.Matches(html);

                        if (matches.Count > 0)
                        {
                            foreach (Match match in matches)
                            {
                                if (match.Success)
                                {
                                    string matchData = match.Groups["dirName"].Value.ToString();
                                    //숫자로만 되어진 경우가 대상
                                    if (Regex.IsMatch(matchData, @"^\d+$"))
                                    {
                                        if (!matchData.Equals("web.config"))
                                        {
                                            dir.Add(matchData);
                                            //iDir.Add(int.Parse(matchData));
                                        }
                                    }                                    
                                }
                            }
                        }
                    }
                }
            }
            catch { throw; }
            
            var sortedDir = dir.OrderByDescending(i => i);
            return sortedDir.ToArray();
        }
        #endregion
    }
}
