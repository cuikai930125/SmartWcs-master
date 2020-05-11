using SMART.WCS.Common_Etc.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SMART.WCS.Common_Etc.File
{
    public class File
    {
        #region ▩ 매개변수
        ///// <summary>
        ///// 프로그램이 실행되는 디렉토리 주소
        ///// </summary>
        private static readonly string START_PATH = System.Windows.Forms.Application.StartupPath + "\\";
        #endregion

        #region ▩ 함수
        #region GetLastFileName - 폴더 내 조건에 맞는 파일 중 가장 최근 파일명 가져오기
        /// <summary>
        /// 폴더 내 조건에 맞는 파일 중 가장 최근 파일명 가져오기
        /// </summary>
        /// <param name="_strDirName">폴더명</param>
        /// <param name="_strCondition">파일검색 조건값</param>
        /// <returns>가장 최근 파일명</returns>
        public static string GetLastFileName(string _strDirName, string _strCondition)
        {
            try
            {
                string strRtnValue = string.Empty;
                string strFullFileName = string.Empty;                                     // 전체 파일명
                string strFileName = string.Empty;                                     // 확장자를 제외한 파일명
                System.Collections.Generic.List<string> liFileName = new System.Collections.Generic.List<string>();    // 파일명을 저장하는 리스트
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(_strDirName);         // 지정한 폴더 정보를 가져온다.

                // 폴더 내 모든 파일 정보 중 조건에 맞는 파일명을 저장한다.
                foreach (System.IO.FileInfo fi in di.GetFiles())
                {
                    strFileName = System.IO.Path.GetFileNameWithoutExtension(fi.ToString());

                    // Log파일명과 조건-일자(strCondition)가 일치하는 값만 List 변수에 저장한다.
                    if (strFileName.Substring(0, 8).Equals(_strCondition))
                    {
                        liFileName.Add(strFileName);
                    }
                }

                // 조건에 맞는 값이 없는 경우 빈값을 리턴한다.
                if (liFileName.Count == 0) { return string.Empty; }

                #region 저장된 파일명을 정렬하여 가장 최근 파일을 매개변수(strReturnValue)에 저장한다. 
                var query = from p in liFileName
                            orderby p.Length descending
                            select p;

                foreach (string strValue in query)
                {
                    strRtnValue = strValue;
                    break;
                }
                #endregion

                return strRtnValue;
            }
            catch (System.Exception err)
            {
                throw err;
            }
        }

        internal static bool Exists(string fileName)
        {
            throw new NotImplementedException();
        }
        #endregion

        //#region GetLocalVersionInfo - WCS 프로그램 업데이트 로컬 버전을 조회
        ///// <summary>
        ///// WCS 프로그램 업데이트 로컬 버전을 조회
        ///// </summary>
        ///// <param name="_strCenterCD">센터코드</param>
        ///// <param name="_enumSystemCD">시스템 코드</param>
        ///// <param name="_strDBConnInfo">DB 연결 문자열</param>
        ///// <returns></returns>
        //public static List<string> GetLocalVersionInfo(string _strCenterCD, EnumClass.SystemCode _enumSystemCode, string _strDBConnInfo)
        //{
        //    try
        //    {
        //        // 리모트 (운영) 서버 프로그램 (시스템별) 버전
        //        var strRemoteProgVersion = string.Empty;
        //        // 로컬 프로그램 (시스템별) 버전
        //        var strLocalProgVersion = string.Empty;

        //        //                // 동탄 WCS 프로그램 로컬 경로
        //        //                string strAppPathInfo                       = @"C:\CJFC.WCS\";

        //        //#if DEBUG
        //        //#else
        //        //                //if (_strCenterCD.Equals("YJ") == true)
        //        //                //{
        //        //                //    // 양지 WCS 프로그램 로컬 경로
        //        //                //    strAppPathInfo  = @"C:\CJFC.WCS.YJ\";
        //        //                //}
        //        //                //else
        //        //                //{
        //        //                //    // 올리브영 WCS 프로그램 로컬 경로
        //        //                //    strAppPathInfo  = @"C:\CJFC.WCS.OY\";
        //        //                //}
        //        //#endif

        //        string strIniPathInfo = string.Format("{0}Version.ini", START_PATH);
        //        string strSystemValue = _enumSystemCode.ToString();

        //        var strRtnValue = string.Empty;
        //        var strProcedureName = "COMADM.SP_COMM_SYS_VER_INQ";
        //        string[] arrOutputParam = { "OUT_SYS_VER", "OUT_RESULT" };

        //        Dictionary<object, object> dicInputParam = new Dictionary<object, object>();
        //        dicInputParam.Add("P_CENTER_CD", _strCenterCD);          // 센터코드
        //        dicInputParam.Add("P_SYS_CD", strSystemValue);          // 시스템 코드

        //        // 리모트 (운영) 서버 프로그램 버전
        //        IniFile ini = new IniFile(strIniPathInfo);
        //        strLocalProgVersion = ini.Read("Version", strSystemValue);

        //        DataSet dsRtnValue = null;

        //        using (DataAccess da = new DataAccess())
        //        {
        //            dsRtnValue = da.GetSpDataSet(_strDBConnInfo, strProcedureName, dicInputParam, arrOutputParam);
        //        }

        //        if (dsRtnValue.Tables[1].Rows[0]["CODE"].ToString().Equals("100"))
        //        {
        //            if (dsRtnValue.Tables[0].Rows.Count > 0)
        //            {
        //                strRtnValue = dsRtnValue.Tables[0].Rows[0]["VER"].ToString();
        //            }
        //        }

        //        List<string> liRtnValue = new List<string>();
        //        liRtnValue.Add(strRtnValue);
        //        liRtnValue.Add(strLocalProgVersion);

        //        return liRtnValue;
        //    }
        //    catch { throw; }
        //}
        //#endregion

        #region WriteIniFile - INI파일 작성
        /// <summary>
        /// INI파일 작성
        /// </summary>
        /// <param name="_strFileName"></param>
        /// <param name="_strKeyName"></param>
        /// <param name="_strValue"></param>
        public static void WriteIniFile(string _strFileName, string _strKeyName, string _strValue)
        {
            try
            {
                //var strAppPathInfo      = System.Windows.Forms.Application.StartupPath + "\\";
                var strPathDll = string.Format("{0}{1}.ini", START_PATH, _strFileName);

                IniFile inif = new IniFile(@strPathDll);
                inif.Write(_strFileName, _strKeyName, _strValue);
            }
            catch { throw; }
        }
        #endregion

        #region > ReadIniFile - INI 확장자 파일 읽어오기
        /// <summary>
        /// INI 확장자 파일 읽어오기
        /// </summary>
        /// <param name="_strFileName">파일명</param>
        /// <param name="_strKeyName">키명</param>
        /// <returns></returns>
        public static string ReadIniFile(string _strFileName, string _strKeyName)
        {
            try
            {
                var strIniFileInfo = string.Empty;
                //var strAppPathInfo                      = System.Windows.Forms.Application.StartupPath + "\\";
                var strIniPathInfo = string.Format("{0}{1}.ini", START_PATH, _strFileName);

                // 리모트 (운영)      프로그램 버전
                IniFile ini = new IniFile(strIniPathInfo);
                strIniFileInfo = ini.Read(_strFileName, _strKeyName);

                return strIniFileInfo;
            }
            catch { throw; }
        }
        #endregion

        private static string extension(object _appName)
        {
            throw new NotImplementedException();
        }

        #region > ExistsFilesByDirectory - 폴더내 파일 존재 여부를 체크한다.
        /// <summary>
        /// 폴더내 파일 존재 여부를 체크한다.
        /// </summary>
        /// <param name="_strDirectoryName">폴더명</param>
        /// <returns></returns>
        public static bool ExistsFilesByDirectory(string _strDirectoryName)
        {
            try
            {
                bool bRtnValue = false;

                DirectoryInfo di = new DirectoryInfo(_strDirectoryName);
                bRtnValue = di.Exists;

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > DeleteFile - 폴더와 파일을 삭제한다.
        /// <summary>
        /// 폴더와 파일을 삭제한다.
        /// </summary>
        /// <param name="_strDirectoryName">폴더명</param>
        public static void DeleteFile(string _strDirectoryName)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(_strDirectoryName);
                System.IO.FileInfo[] files = di.GetFiles("*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    file.Attributes = FileAttributes.Normal;
                }

                Directory.Delete(_strDirectoryName, true);
            }
            catch { throw; }
        }
        #endregion

        #region > CreateDirectory - 폴더를 생성한다. (폴더 존재여부 확인 후)
        /// <summary>
        /// 폴더를 생성한다. (폴더 존재여부 확인 후)
        /// </summary>
        /// <param name="_strDirectoryName">폴더명</param>
        public static void CreateDirectory(string _strDirectoryName)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(_strDirectoryName);
                if (di.Exists == false)
                {
                    di.Create();
                }
            }
            catch { throw; }
        }

        public static void CreateDirectory(string _strDirectoryName, bool _isValue)
        {
            try
            {
                DirectorySecurity securityRules = new DirectorySecurity();
                securityRules.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));

                //DirectoryInfo di = new DirectoryInfo(_strDirectoryName, securityRules);
                DirectoryInfo di = Directory.CreateDirectory(_strDirectoryName, securityRules);
            }
            catch { throw; }
        }
        #endregion

        #region > GetListOnDirectory - 폴더내 폴더리스트를 가져온다.
        /// <summary>
        /// 폴더내 폴더리스트를 가져온다.
        /// </summary>
        /// <param name="_strDirectoryPath">폴더</param>
        /// <returns></returns>
        public static List<string> GetListOnDictionary(string _strDirectoryPath)
        {
            try
            {
                List<string> liDirectoryList    = new List<string>();
                DirectoryInfo di                = new DirectoryInfo(_strDirectoryPath);

                foreach (var item in di.GetDirectories())
                {
                    liDirectoryList.Add(item.Name);
                }

                return liDirectoryList;
            }
            catch { throw; }
        }
        #endregion

        #region > GetDirectoryFileDictionaryList - 폴더내 파일 리스트를 가져온다. (Directory 형식으로 반환)
        ///// <summary>
        ///// 폴더내 파일 리스트를 가져온다. (Directory 형식으로 반환)
        ///// </summary>
        ///// <param name="_strDirectoryPath">디렉토리 경로</param>
        ///// <returns></returns>
        //public static Dictionary<string, byte[]> GetDirectoryFileList(string _strDirectoryPath)
        //{
        //    try
        //    {
        //        //DirectoryInfo di = new DirectoryInfo(_strDirectoryPath);
        //        Dictionary<string, byte[]> dicDeployDirectoryInfo = new Dictionary<string, byte[]>();
        //        //byte[] arrByteFileInfo = null;
        //        //var strDeployDirectoryName = string.Empty;

        //        //foreach (var file in di.GetFiles())
        //        //{
        //        //    strDeployDirectoryName = _strDirectoryPath + $"{file.Name}";
        //        //    arrByteFileInfo = System.IO.File.ReadAllBytes(strDeployDirectoryName);
        //        //    dicDeployDirectoryInfo.Add(file.Name, arrByteFileInfo);
        //        //}

        //        //return dicDeployDirectoryInfo;

        //        List<string> liDirectoryFileList    = new List<string>();
        //        DirectoryInfo di                    = new DirectoryInfo(_strDirectoryPath);

        //        foreach (var file in di.GetFiles())
        //        {
        //            liDirectoryFileList.Add(file.Name);    
        //        }

        //        return dicDeployDirectoryInfo;
        //    }
        //    catch { throw; }
        //}
        #endregion

        #region > GetDeployLocalFileList - 로컬 배포 대상 경로를 가져온다.
        /// <summary>
        /// 로컬 배포 대상 경로를 가져온다.
        /// </summary>
        /// <param name="_strDirectoryPath">조회 대상 디렉토리 경로</param>
        /// <returns></returns>
        public static DataTable GetDeployLocalFileList(string _strDirectoryPath)
        {
            try
            {
                DataTable dtRtnValue        = null;
                DirectoryInfo di            = null;

                var strCommonCD = "LANG_RESO_TYPE";
                DataTable dtLangResourceType    = CommonComboBox.GetCommonData(strCommonCD, null, false);

                // 데이터테이블 스키마를 생성한다.
                dtRtnValue = Utility.HelperClass.CreateDataTableSchema(null, BaseEnumClass.CreateTableSchemaKind.DEPLOY_FILE_LIST);

                di              = new DirectoryInfo(_strDirectoryPath);
                DataRow drRow   = null;

                foreach (var fi in di.GetFiles())
                {
                    if (fi.Extension.Equals(".config"))
                    {
                        if (fi.Name.Equals("SMART.WCS.exe.config") == false) { continue; }
                    }

                    drRow                               = dtRtnValue.NewRow();
                    drRow["FILE_NAME"]                  = fi.Name;              // 파일명
                    drRow["FILE_EXTENSION"]             = fi.Extension;         // 확장자
                    drRow["FILE_LAST_MODIFY_DT"]        = fi.LastWriteTime;     // 최종 수정일시
                    drRow["FILE_DIRECTORY"]             = fi.DirectoryName;     // 폴더명
                    dtRtnValue.Rows.Add(drRow);
                }

                DirectoryInfo[] arrSubDirInfo   = di.GetDirectories();
                foreach (DirectoryInfo dirInfo in arrSubDirInfo)
                {
                    var iCount = dtLangResourceType.AsEnumerable().Where(p => p.Field<string>("CODE").Contains(dirInfo.Name)).Count();
                    if (iCount == 0) { continue; }

                    foreach (FileInfo fi in dirInfo.GetFiles())
                    {
                        drRow                           = dtRtnValue.NewRow();
                        drRow["FILE_NAME"]              = fi.Name;              // 파일명
                        drRow["FILE_EXTENSION"]         = fi.Extension;         // 확장자
                        drRow["FILE_LAST_MODIFY_DT"]    = fi.LastWriteTime;     // 최종 수정일시
                        drRow["FILE_DIRECTORY"]         = fi.DirectoryName;     // 폴더명
                        dtRtnValue.Rows.Add(drRow);
                    }
                }

                if (dtRtnValue.Rows.Count > 0)
                {
                    dtRtnValue = dtRtnValue.AsEnumerable().Where(p => p.Field<string>("FILE_EXTENSION").Equals(".xml") == false).CopyToDataTable();
                }

                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > GetDeployServerFileList - 서버 배포 경로 파일을 가져온다.
        /// <summary>
        /// 서버 배포 경로 파일을 가져온다.
        /// </summary>
        /// <param name="_strDirectoryPath">조회 대상 디렉토리 경로</param>
        /// <returns></returns>
        public static DataTable GetDeployServerFileList(string _strDirectoryPath)
        {
            try
            {
                DataTable dtRtnValue        = null;
                DirectoryInfo di            = null;

                // 데이터테이블 스키마를 생성한다.
                dtRtnValue = Utility.HelperClass.CreateDataTableSchema(null, BaseEnumClass.CreateTableSchemaKind.DEPLOY_FILE_LIST);

                di              = new DirectoryInfo(_strDirectoryPath);
                DataRow drRow   = null;

                foreach (var fi in di.GetFiles())
                {
                    if (fi.Extension.Equals(".config"))
                    {
                        if (fi.Name.Equals("SMART.WCS.exe.config") == false) { continue; }
                    }

                    drRow                               = dtRtnValue.NewRow();
                    drRow["FILE_NAME"]                  = fi.Name;              // 파일명
                    drRow["FILE_EXTENSION"]             = fi.Extension;         // 확장자
                    drRow["FILE_LAST_MODIFY_DT"]        = fi.LastWriteTime;     // 최종 수정일시
                    drRow["FILE_DIRECTORY"]             = fi.DirectoryName;     // 폴더명
                    dtRtnValue.Rows.Add(drRow);
                }

                DirectoryInfo[] arrSubDirInfo   = di.GetDirectories();
                foreach (DirectoryInfo dirInfo in arrSubDirInfo)
                {
                    foreach (FileInfo fi in dirInfo.GetFiles())
                    {
                        drRow                           = dtRtnValue.NewRow();
                        drRow["FILE_NAME"]              = fi.Name;              // 파일명
                        drRow["FILE_EXTENSION"]         = fi.Extension;         // 확장자
                        drRow["FILE_LAST_MODIFY_DT"]    = fi.LastWriteTime;     // 최종 수정일시
                        drRow["FILE_DIRECTORY"]         = fi.DirectoryName;     // 폴더명
                        dtRtnValue.Rows.Add(drRow);
                    }
                }

                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > GetDirectoryFileList - 폴더내 파일 리스트를 가져온다. (리스트 형식으로 반환)
        /// <summary>
        /// 폴더내 파일 리스트를 가져온다. (리스트 형식으로 반환)
        /// </summary>
        /// <param name="_strDirectoryPath">디렉토리 경로</param>
        /// <returns></returns>
        public static List<string> GetDirectoryFileList(string _strDirectoryPath)
        {
            try
            {
                List<string> liDirectoryFileList    = new List<string>();
                DirectoryInfo di                    = new DirectoryInfo(_strDirectoryPath);

                foreach (var file in di.GetFiles())
                {
                    liDirectoryFileList.Add(file.Name);    
                }

                return liDirectoryFileList;
            }
            catch { throw; }
        }
        #endregion

        #region > GetFileList - 파일 리스트를 조회한다.
        /// <summary>
        /// 파일 리스트를 조회한다.
        /// </summary>
        /// <param name="_strDirectoryPath">파일 경로</param>
        /// <returns></returns>
        public static List<string> GetFileList(string _strDirectoryPath)
        {
            try
            {
                List<string> liRtnValue     = new List<string>();

                DirectoryInfo di            = null;
                di                          = new DirectoryInfo(_strDirectoryPath);

                foreach (var fi in di.GetFiles())
                {
                    liRtnValue.Add(fi.FullName);
                }

                DirectoryInfo[] arrSubDirInfo   = di.GetDirectories();
                foreach (DirectoryInfo dirInfo in arrSubDirInfo)
                {
                    foreach (FileInfo fi in dirInfo.GetFiles())
                    {
                        liRtnValue.Add(fi.FullName);
                    }
                }

                return liRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region >> GetFileVersion - 파일버전을 가져온다.
        /// <summary>
        /// 파일버전을 가져온다.
        /// </summary>
        /// <param name="_strFilePath">버전 조회 대상 파일 경로 + 파일명 + 확장자</param>
        /// <returns></returns>
        public static string GetFileVersion(string _strFilePath)
        {
            try
            {
                //Assembly assembly = Assembly.LoadFrom(_strFilePath);
                //return assembly.GetName().Version.ToString();

                 string strFileVersion   = string.Empty;

                FileVersionInfo fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(_strFilePath);
                strFileVersion  = fileVersion.FileVersion;
                fileVersion = null;

                return strFileVersion;
            }
            catch { throw; }
        }
        #endregion

        #region > ArrayByteToFile - byte 배열 (압축파일) 데이터를 파일 형식으로 변환한다.
        /// <summary>
        /// byte 배열 (압축파일) 데이터를 파일 형식으로 변환한다.
        /// </summary>
        /// <param name="_strFilePath">파일경로/파일명</param>
        /// <param name="_arrByteZip">압축파일을 byte[] 형으로 변환한 데이터</param>
        /// <returns></returns>
        public static bool ArrayByteToFile(string _strFilePath, byte[] _arrByteZip)
        {
            try
            {
                var bRtnValue = false;

                using (var fs = new FileStream(_strFilePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(_arrByteZip, 0, _arrByteZip.Length);
                    bRtnValue = true;
                }

                return bRtnValue;
            }
            catch
            {
                return false;
                throw;
            }
        }
        #endregion

        #region > CompressToZip - 파일 압축 (여러개 파일을 하나의 압축파일로 변환)
        /// <summary>
        /// 파일 압축 (여러개 파일을 하나의 압축파일로 변환)
        /// </summary>
        /// <param name="_strFileName">파일명</param>
        /// <param name="_dicFileList">파일 리스트</param>
        public static void CompressToZip(string _strFileName, Dictionary<string, byte[]> _dicFileList)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in _dicFileList)
                    {
                        var demoFile = archive.CreateEntry(file.Key);

                        using (var entryStream = demoFile.Open())
                        using (var b = new BinaryWriter(entryStream))
                        {
                            b.Write(file.Value);
                        }
                    }
                }

                using (var fileStream = new FileStream(_strFileName, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }
        #endregion

        #region > ExtratToZip - 파일 압축해제 (경로를 지정하여 압축해제)
        /// <summary>
        /// 파일 압축해제 (경로를 지정하여 압축해제)
        /// </summary>
        /// <param name="_strZipFilePath">Zip파일 경로</param>
        /// <param name="_strBackupDirectory">압축해제 폴더</param>
        public static void ExtractToZip(string _strZipFilePath, string _strBackupDirectory)
        {
            try
            {
                using (ZipArchive zipArchive = ZipFile.OpenRead(_strZipFilePath))
                {
                    foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                    {
                        var strDirectoryPath = Path.GetDirectoryName(Path.Combine(_strBackupDirectory, zipArchiveEntry.FullName));

                        if (Directory.Exists(strDirectoryPath) == false)
                        {
                            Directory.CreateDirectory(strDirectoryPath);
                        }

                        zipArchiveEntry.ExtractToFile(Path.Combine(_strBackupDirectory, zipArchiveEntry.FullName), true);
                    }
                }
            }
            catch { throw; }
        }
        #endregion

        #region > CompressToZip - 파일 압축
        /// <summary>
        /// 파일 압축
        /// </summary>
        /// <param name="_strSourcePath">압축 대상폴더</param>
        /// <param name="_strZipPath">압축 파일 백업폴더</param>
        public static void CompressToZip(string _strSourcePath, string _strZipPath)
        {
            try
            {
                var liFileList = GetFileList(_strSourcePath);

                using (FileStream fs = new FileStream(_strZipPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (ZipArchive za = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        foreach (var file in liFileList)
                        {
                            var strPath = file.Substring(_strSourcePath.Length + 1);
                            za.CreateEntryFromFile(file, strPath);
                        }
                    }
                }
            }
            catch { throw; }
        }
        #endregion

        #region ExistsDirectoryFile - 폴더 내 파일 존재 여부 체크
        /// <summary>
        /// 폴더 내 파일 존재 여부 체크
        /// </summary>
        /// <param name="_strDirectoryName">배포 폴더명</param>
        /// <param name="_strCheckFileName">존재여부 체크 대상파일</param>
        /// <returns></returns>
        public static bool ExistsDirectoryFile(string _strDirectoryName, string _strCheckFileName)
        {
            try
            {
                bool bRtnValue = false;
                DirectoryInfo di = new DirectoryInfo(_strDirectoryName);

                foreach (var file in di.GetFiles())
                {
                    if (file.Name.Equals(_strCheckFileName) == true)
                    {
                        bRtnValue = true;
                        break;
                    }
                }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > GetFileName - 파일명을 가져온다. (확장자 제외)
        /// <summary>
        /// 파일명을 가져온다. (확장자 제외)
        /// </summary>
        /// <param name="_strFileFullPath">파일 전체경로</param>
        /// <returns></returns>
        public static string GetFileName(string _strFileFullPath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(_strFileFullPath);
        }
        #endregion

        #region > GetFileNameExtension - 파일명을 가져온다. (확장자 포함)
        /// <summary>
        /// 파일명을 가져온다. (확장자 포함)
        /// </summary>
        /// <param name="_strFileFullPath">파일 전체경로</param>
        /// <returns></returns>
        public static string GetFileNameExtension(string _strFileFullPath)
        {
            return System.IO.Path.GetFileName(_strFileFullPath);
        }
        #endregion

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
            //List<int> iDir = new List<int>(15);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_strURL + _strAppName);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string html = reader.ReadToEnd();

                        Regex regexObj = new Regex("<a[^>]*? href=\"(?<sURL>[^\"]+)\"[^>]*?>(?<dirName>.*?)</a>", RegexOptions.IgnoreCase);

                        // Regex regexObj = new Regex("<a[^>]*? href=\"(?<DownURL>[^\"]+)\"[^>]*?>(?<fileName>.*?)</a>", RegexOptions.IgnoreCase);

                        MatchCollection matches = regexObj.Matches(html);

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
                                    else if (matchData.Equals("conf"))
                                    {
                                        dir.Add(matchData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { throw; }

            //var sortedDir = dir.OrderBy(i => i);
            var sortedDir = dir.OrderByDescending(i => i);

            return sortedDir.ToArray();
        }
        #endregion

        #region > IsValidFilename - 파일 유효성 체크
        /// <summary>
        /// 파일 유효성 체크
        /// </summary>
        /// <param name="_strFileName">파일명</param>
        /// <returns></returns>
        private static bool IsValidFilename(string _strFileName)
        {
            string strTheseAreInvalidFileNameChars = new string(System.IO.Path.GetInvalidFileNameChars());
            Regex regInvalidFileName = new Regex("[" + Regex.Escape(strTheseAreInvalidFileNameChars) + "]");

            if (regInvalidFileName.IsMatch(_strFileName) || string.IsNullOrEmpty(Path.GetExtension(_strFileName))) { return false; };

            return true;
        }
        #endregion
        #endregion
    }
}
