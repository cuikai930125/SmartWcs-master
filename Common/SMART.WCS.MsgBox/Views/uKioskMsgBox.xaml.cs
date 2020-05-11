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

namespace SMART.WCS.MsgBox.Views
{
    /// <summary>
    /// uKioskMsgBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class uKioskMsgBox : Window, IDisposable
    {
        #region ▩ Delegate
        public delegate void ButtonClickResult(bool _bResult);
        public event ButtonClickResult ClickResult;
        #endregion

        #region ▩ Enum
        /// <summary>
        /// 메세지 박스 종류
        /// </summary>
        public enum KioskMsgBoxKind
        {
            Information     = 1,
            Error           = 2,
            Question        = 3,
            None            = 99
        }
        #endregion

        #region ▩ 전역변수
        KioskMsgBoxKind g_enumMsgBoxKind = KioskMsgBoxKind.None;
        #endregion

        #region ▩ 생성자
        public uKioskMsgBox()
        {
            InitializeComponent();

            // 이벤트 초기화
            this.InitEvent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_strMessage">메세지</param>
        /// <param name="_enumMsgBoxKind">메세지 종류</param>
        public uKioskMsgBox(string _strMessage, KioskMsgBoxKind _enumMsgBoxKind)
        {
            InitializeComponent();

            this.g_enumMsgBoxKind = _enumMsgBoxKind;

            this.InitControlInfo();
            this.InitControlError();
            this.InitControlQuestion();

            this.lblContent.Text = this.GetMsgBoxValue(_strMessage);

            // 컨트롤 설정 및 값을 초기화한다.
            this.InitControl();

            // 이벤트 초기화한다.
            this.InitEvent();
        }
        #endregion

        #region ▩ 함수
        #region InitControl - 컨트롤 설정 및 값을 초기화한다.
        /// <summary>
        /// 컨트롤 설정 및 값을 초기화한다.
        /// </summary>
        private void InitControl()
        {



            
            this.btnError.Content       = "확인";
            this.btnYes.Content         = "예";
            this.btnNo.Content          = "아니오";
        }

        private void InitControlInfo()
        {
            if (this.g_enumMsgBoxKind == KioskMsgBoxKind.Information)
            {
                this.gridConfirmArea.Visibility     = Visibility.Visible;
                this.imgInfo.Visibility             = Visibility.Visible;
                this.btnConfirm.Content             = "확인";
            }
        }

        private void InitControlError()
        {
            if (this.g_enumMsgBoxKind == KioskMsgBoxKind.Error)
            {
                this.gridErrorArea.Visibility     = Visibility.Visible;
                this.imgError.Visibility          = Visibility.Visible;
                this.btnError.Content             = "확인";
            }
        }

        private void InitControlQuestion()
        {
            if (this.g_enumMsgBoxKind == KioskMsgBoxKind.Question)
            {
                this.gridQuesArea.Visibility        = Visibility.Visible;
                this.imgQues.Visibility             = Visibility.Visible;
                this.btnYes.Content                 = "예";
                this.btnNo.Content                  = "아니오";
            }
        }
        #endregion

        #region InitEvent - 이벤트 초기화한다.
        /// <summary>
        /// 이벤트 초기화한다.
        /// </summary>
        private void InitEvent()
        {
            try
            {
                // 확인 (정보 메세지 박스) 버튼 클릭 이벤트
                this.btnConfirm.PreviewMouseLeftButtonUp += BtnConfirm_PreviewMouseLeftButtonUp;

                // 확인 (오류 메세지 박스) 버튼 이벤트
                this.btnError.PreviewMouseLeftButtonUp += BtnError_PreviewMouseLeftButtonUp;

                // Question 메세지 박스 Yes 버튼 이벤트
                this.btnYes.PreviewMouseLeftButtonUp += BtnYes_PreviewMouseLeftButtonUp;

                // Question 메세지 박스 No 버튼 이벤트
                this.btnNo.PreviewMouseLeftButtonUp += BtnNo_PreviewMouseLeftButtonUp;
                //#endregion
            }
            catch { throw; }
        }
        #endregion

        /// <summary>
        /// 메세지 데이터를 변환한다.
        /// </summary>
        /// <param name="_strMessage"></param>
        /// <returns></returns>
        private string GetMsgBoxValue(string _strMessage)
        {
            // 메세지를 출력한다.
            var messageSplit        = _strMessage.Split('|');
            string strMessage       = string.Empty;

            for (int i = 0; i < messageSplit.Length; i++)
            {
                if (i == messageSplit.Length - 2)
                {
                    strMessage += messageSplit[i] + System.Environment.NewLine;
                }
                else
                {
                    if (messageSplit.Length == 1)
                    {
                        strMessage += messageSplit[i];
                    }
                    else
                    {
                        strMessage += messageSplit[i] + System.Environment.NewLine;
                    }
                }
            }

            return strMessage;
        }
        #endregion

        #region ▩ 이벤트
        #region > 버튼 이벤트
        #region >> btnFormClose_Click - 폼 종료 버튼 클릭
        /// <summary>
        /// 폼 종료 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFormClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 델리게이트를 이용하여 부모창으로 리턴값을 전달한다.
                this.ClickResult(false);
                this.Close();
            }
            catch { throw; }
        }
        #endregion

        #region >> Information 메세지 박스 확인 버튼 클릭 이벤트
        /// <summary>
        /// Information 메세지 박스 확인 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConfirm_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 델리게이트를 이용하여 부모창으로 리턴값을 전달한다.
            this.ClickResult(true);
            this.Close();
        }
        #endregion

        #region >> Error 메세지 박스 확인 버튼 클릭 이벤트
        /// <summary>
        /// Error 메세지 박스 확인 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnError_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 델리게이트를 이용하여 부모창으로 리턴값을 전달한다.
            this.ClickResult(true);
            this.Close();
        }
        #endregion

        #region >> Question 메세지 박스 Yes 버튼 클릭 이벤트
        /// <summary>
        /// Question 메세지 박스 Yes 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnYes_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 델리게이트를 이용하여 부모창으로 리턴값을 전달한다.
            this.ClickResult(true);
            this.Close();
        }
        #endregion

        #region >> Question 메세지 박스 No 버튼 클릭 이벤트
        /// <summary>
        /// Question 메세지 박스 No 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNo_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 델리게이트를 이용하여 부모창으로 리턴값을 전달한다.
            this.ClickResult(false);
            this.Close();
        }
        #endregion
        #endregion

        private void imgClose_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.ClickResult(false);
                this.Close();
            }
            catch { throw; }
        }

        #region > 폼 헤더 드래그
        /// <summary>
        /// 폼 헤더 드래그
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch { throw; }
        }
        #endregion

        #region > Enter키 이벤트
        /// <summary>
        /// Enter키 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmInformationMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ClickResult(true);
                this.Close();
                e.Handled = true;
            }
        }
        #endregion
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
        // ~uKioskMsgBox()
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
