using SMART.WCS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SMART.WCS.UI.COMMON.Views.SYS_MGMT
{
    /// <summary>
    /// C1028_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class C1028_01P : Window, IDisposable
    {
        #region ▩ 전역변수
        /// <summary>
        /// BaseClass 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();
        #endregion

        #region ▩ 생성자
        public C1028_01P()
        {
            try
            {
                InitializeComponent();

                this.InitEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 함수
        private void InitEvent()
        {
            try
            {
                // 암호화 버튼 클릭 이벤트
                this.btnEncrypt.PreviewMouseLeftButtonUp += BtnEncrypt_PreviewMouseLeftButtonUp;
                // 복호화 버튼 클릭 이벤트
                this.btnDecrypt.PreviewMouseLeftButtonUp += BtnDecrypt_PreviewMouseLeftButtonUp;
                // 초기화 버튼 클릭 이벤트
                this.btnInit.PreviewMouseLeftButtonUp += BtnInit_PreviewMouseLeftButtonUp;
            }
            catch { throw; }
        }
        #endregion

        #region ▩ 이벤트
        #region > 버튼 이벤트
        /// <summary>
        /// 암호화 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEncrypt_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strOriginalValue = this.txtOriginalValue.Text.Trim();

                if (strOriginalValue.Length == 0) { this.txtEncryptValue.Text = string.Empty; }
                else { this.txtEncryptValue.Text = this.BaseClass.EncryptAES256(strOriginalValue); }

                this.BaseClass.MsgInfo("CMPT_CHG");
            }
            catch (Exception err)
            {
                var strMessage = this.BaseClass.GetResourceValue("ERR_CHG_CRYPT");
                this.BaseClass.MsgError($"{strMessage}|{err.Message}", BaseEnumClass.CodeMessage.MESSAGE);

                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 복호화 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDecrypt_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strOriginalValue = this.txtOriginalValue.Text.Trim();

                if (strOriginalValue.Length == 0) { this.txtDecryptValue.Text = string.Empty; }
                else { this.txtDecryptValue.Text = this.BaseClass.DecryptAES256(strOriginalValue); }

                this.BaseClass.MsgInfo("CMPT_CHG");
            }
            catch (Exception err)
            {
                // 복호화 변환 중 오류가 발생했습니다.
                var strMessage = this.BaseClass.GetResourceValue("ERR_CHG_DECRYPT");
                this.BaseClass.MsgError($"{strMessage}|{err.Message}", BaseEnumClass.CodeMessage.MESSAGE);

                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 초기화 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInit_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.txtOriginalValue.Text.Trim().Length > 0) { this.txtOriginalValue.Text = string.Empty; }
                if (this.txtEncryptValue.Text.Trim().Length > 0) { this.txtEncryptValue.Text = string.Empty; }
                if (this.txtDecryptValue.Text.Trim().Length > 0) { this.txtDecryptValue.Text = string.Empty; }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void BtnCancel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
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
        // ~C1028_01P()
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
