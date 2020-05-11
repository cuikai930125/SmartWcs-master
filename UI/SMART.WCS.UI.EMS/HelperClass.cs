using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.UI.EMS
{
    class HelperClass
    {
    }

    public class EmsSession
    {
        #region Singleton

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static EmsSession Instance { get; private set; }


        /// <summary>
        /// Singleton 생성자
        /// </summary>
        static EmsSession()
        {
            Instance = new EmsSession();
        }

        // {Binding Source={x:Static local:GrapSession.Instance}, Path=ChatterMessageEnable}

        #endregion Singleton


        public string FileUploadUrl
        {
            //get { return "http://localhost/FileUpload.aspx"; }
            get { return ConfigurationManager.AppSettings["FileUploadUrl"].ToString(); }
        }

        public string FileDownloadUrl
        {
            //get { return "http://localhost/Files/"; }
            get { return ConfigurationManager.AppSettings["FileDownloadUrl"].ToString(); }
        }

        //public MainWindow MainForm { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        private EmsSession()
        {
        }
    }
}
