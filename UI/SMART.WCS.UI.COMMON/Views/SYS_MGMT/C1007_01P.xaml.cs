using SMART.WCS.Common;
using SMART.WCS.UI.COMMON.DataMembers.C1007;

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;

namespace SMART.WCS.UI.COMMON.Views.SYS_MGMT
{
    /// <summary>
    /// C1007_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1007_01P : Window, IDisposable
    {
        #region ▩ 매개변수
        BaseClass BaseClass = new BaseClass();
        #endregion

        #region > 변수
        //다운상태값( 현재파일/총파일)
        private double g_dProgressValue = 0;

        //마지막 파일의 다운로드 성공체크
        private bool g_isSuccess = false;

        //업데이트 어플리게이션명
        private string g_strAppName;
        #endregion


        /// <summary>
        /// 서버 배포 경로
        /// </summary>
        private string g_strServerDeployPath = string.Empty;

        #region ▩ 생성자
        public C1007_01P()
        {
            InitializeComponent();
        }

        public C1007_01P(List<LocalFileList> _liDeployFileInfo)
        {
            try
            {
                InitializeComponent();

                this.FileUpload(_liDeployFileInfo);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        private async void FileUpload(List<LocalFileList> _liDeployFileInfo)
        {
            using (WebClient wc = new WebClient())         
            {
                try
                {
                    var strServerPath = $"{_liDeployFileInfo[0].SERVER_DIRECTORY}";
                    wc.UseDefaultCredentials = true;

                    pbUpdProgress.Value = 0;

                    wc.UploadFileCompleted += Wc_UploadFileCompleted;

                    for (int i = 0; i < _liDeployFileInfo.Count; i++)
                    {
                        string strDownloadCount = string.Format("({0}/{1})", (i + 1).ToString(), _liDeployFileInfo.Count.ToString());
                        this.txtDownloadCount.Text = strDownloadCount;

                        this.g_dProgressValue = 100 * (i + 1) / _liDeployFileInfo.Count;

                        if (i == _liDeployFileInfo.Count - 1)
                        {
                            this.g_isSuccess = true;
                        }

                        var strFilePath         = $"{_liDeployFileInfo[i].SERVER_DIRECTORY}/{_liDeployFileInfo[i].FILE_NAME}";
                        // C:\Deploy\Coupang\Buchen\bin\DevExpress.Charts.v19.1.Core.dll
                        var strLocalPath        = $"{_liDeployFileInfo[i].LOCAL_DIRECTORY}\\{_liDeployFileInfo[i].FILE_NAME}";
                        // C:\Deploy\Coupang\Buchen\bin
                        var strDeployPath       = _liDeployFileInfo[i].LOCAL_DIRECTORY;      // 배포 경로

                        await wc.UploadFileTaskAsync(new Uri(strFilePath), "POST", strLocalPath);
                    }

                }
                catch { throw; }
            }
        }

        private void Wc_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            try
            {
                //상태바를 갱신
                pbUpdProgress.Value = this.g_dProgressValue;

                //마지막 파일의 다운로드 완료, 에러가 없으면 메세지 표시하고 , 시스템을 기동한다. 
                if (this.g_isSuccess && e.Error == null)
                {
                    // 배포가 완료되었습니다.
                    this.BaseClass.MsgInfo("CMPT_DEPLOY");

                    this.Close();
                }
                else if (e.Error != null)    
                {
                    this.BaseClass.MsgError(e.Error.Message, BaseEnumClass.CodeMessage.MESSAGE);

                    this.Close();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);

                this.Close();
            }
        }

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
        // ~C1007_01P()
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
