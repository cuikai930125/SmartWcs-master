using SMART.WCS.Common.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SMART.WCS.Control.Views
{
    /// <summary>
    /// 비밀번호 재설정
    /// </summary>
    public partial class SWCS201_02P : Window, IDisposable
    {
        #region ▩ Delegate
        // 비밀번호 변경 결과를 부모창으로 리턴
        public delegate void PasswordChgResult(bool _bResultYN);
        public event PasswordChgResult PasswordChangeResult;
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// 공통 Class를 이용하기 위한 BaseClass 선언
        /// </summary>
        private SMART.WCS.Common.BaseClass BaseClass = new SMART.WCS.Common.BaseClass();

        /// <summary>
        /// 공통 관리 데이터 매개변수를 선언한다.
        /// </summary>
        private SMART.WCS.Control.BaseInfo BaseInfo = new SMART.WCS.Control.BaseInfo();

        /// <summary>
        /// 사용자 ID
        /// </summary>
        private string g_strUserID  = string.Empty;

        /// <summary>
        /// 센터코드
        /// </summary>
        private string g_strCenterCD = string.Empty;
        #endregion

        #region ▩ 생성자
        public SWCS201_02P()
        {
            InitializeComponent();
        }

        public SWCS201_02P(string _strUserID, string _strCenterCD)
        {
            try
            {
                InitializeComponent();

                // 이벤트를 초기화한다.
                this.InitEvent();

                // 메세지 정의
                this.InitMessage();

                this.g_strUserID                = _strUserID;
                this.g_strCenterCD              = _strCenterCD;

                this.lblPASSWORD.Text           = this.BaseClass.GetResourceValue("PWD");       // 비밀번호
                this.lblPASSWORD_CONF.Text      = this.BaseClass.GetResourceValue("PWD_CONF");  // 비밀번호 확인

                // PWD_CHG_SWCS201_02_FIRST - 암호는 14자 이상이어야 합니다.
                // PWD_CHG_SWCS201_02_SECOND - 최소한 하나의 숫자를 제공해야 합니다.
                // PWD_CHG_SWCS201_02_THIRD - 하나 이상의 대문자를 제공합니다.
                // PWD_CHG_SWCS201_02_FOURTH - 하나 이상의 특수문자를 제공합니다.
                // PWD_CHG_SWCS201_02_FIFTH - 하나 이상의 소문자를 제공합니다.
                // PWD_CHG_SWCS201_02_SIXTH - 비밀번호를 변경하시면 메인 화면이 열립니다.
                this.lblNotice_First.Text       = this.BaseClass.GetResourceValue("PWD_CHG_SWCS201_02_FIRST");
                this.lblNotice_Second.Text      = this.BaseClass.GetResourceValue("PWD_CHG_SWCS201_02_SECOND");
                this.lblNotice_Third.Text       = this.BaseClass.GetResourceValue("PWD_CHG_SWCS201_02_THIRD");
                this.lblNotice_Fourth.Text      = this.BaseClass.GetResourceValue("PWD_CHG_SWCS201_02_FOURTH");
                this.lblNotice_Fifth.Text       = this.BaseClass.GetResourceValue("PWD_CHG_SWCS201_02_FIFTH");
                this.lblNotice_Sixth.Text       = this.BaseClass.GetResourceValue("PWD_CHG_SWCS201_02_SIXTH");
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitEvent - 이벤트를 초기화한다.
        /// <summary>
        /// 이벤트를 초기화한다.
        /// </summary>
        private void InitEvent()
        {
            try
            {
                // Loaded 이벤트
                this.Loaded += WWCS914_01P_Loaded;

                //화면 상단 X버튼 MouseEnter 이벤트
                this.btnImgClose.MouseEnter += BtnCancel_MouseEnter; ;

                //화면 상단 X버튼 MouseLeave 이벤트
                this.btnImgClose.MouseLeave += BtnCancel_MouseLeave;

                //화면 상단 X버튼 EnabledChanged 이벤트
                this.btnImgClose.IsEnabledChanged += btnCancel_IsEnabledChanged;

                this.btnImgClose.PreviewMouseLeftButtonUp += BtnCancel_PreviewMouseLeftButtonUp;
                this.btnCancel.PreviewMouseLeftButtonUp += BtnCancel_PreviewMouseLeftButtonUp;
                this.btnConfirm.PreviewMouseLeftButtonUp += BtnConfirm_PreviewMouseLeftButtonUp;
            }
            catch { throw; }
        }
        #endregion

        #region >> InitMessage - 화면내에서 사용하는 메세지 정보 조회
        /// <summary>
        /// 화면내에서 사용하는 메세지 정보 조회
        /// </summary>
        private void InitMessage()
        {
            try
            {
                

            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > Validation - 유효성 체크
        #region >> CheckValidatePassword - 비밀번호 유효성 체크
        /// <summary>
        /// 비밀번호 유효성 체크
        /// </summary>
        /// <returns></returns>
        private bool CheckValidatePassword()
        {
            bool bRtnValue = false;

            //Regex rxPassword = new Regex(@"^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", RegexOptions.IgnorePatternWhitespace);
            Regex regex = new Regex(@"(?=.*\d)(?=.*[\{\}\[\]\/?.,;:|\)*~`!^\-_+<>@\#$%&\\\=\(\'\'])(?=.*[a-z])(?=.*[A-Z]).{14,50}$");
            ////Regex rxPassword    = new Regex(@"(?=.*\d)(?=.*[\{\}\[\]\/?.,;:|\)*~`!^\-_+<>@\#$%&\\\=\(\'\"])(?=.*[a - z])(?=.*[A - Z]).{ 14,50}$;

            ////Regex regex = new Regex(@"(?=.*\d)(?=.*[\{\}\[\]\/?.,;:|\)*~`!^\-_+<>@\#$%&\\\=\(\'\'])(?=.*[a - z])(?=.*[A - Z]).{ 14,50}$", RegexOptions.IgnorePatternWhitespace);
            //Regex regex = new Regex(@"[a-zA-Z0-9~`!@#$%^&*()_\-+={}[\]|\\;:'""<>,.?/]");

            if (regex.IsMatch(this.txtPassword.Text.ToString()))
            {
                bRtnValue = true;
            }

            return bRtnValue;
        }
        #endregion

        #region >> CheckEqualPassword - 입력한 비밀번호와 비밀번호 확인 값이 동일한지 체크
        /// <summary>
        /// 입력한 비밀번호와 비밀번호 확인 값이 동일한지 체크
        /// </summary>
        /// <param name="_strPassword">비밀번호</param>
        /// <param name="_strPasswordConf">비밀번호 확인</param>
        /// <returns></returns>
        private bool CheckEqualPassword(string _strPassword, string _strPasswordConf)
        {
            try
            {
                bool bRtnValue = true;

                if (_strPassword.Equals(_strPasswordConf) == false)
                {
                    bRtnValue = true;
                }

                return bRtnValue;
            }
            catch { throw; }
        }
        #endregion
        #endregion

        #region > AskWindowClose - 창 종료 시 Question 창 출력
        /// <summary>
        /// 창 종료 시 Question 창 출력
        /// </summary>
        /// <returns>true:창종료, false:리턴</returns>
        private bool AskWindowClose()
        {
            try
            {
                // ASK_PWD_CHK_NOT_LOGIN - 비밀번호를 변경하지 않으셨습니다.|비밀번호를 변경하지 않으면 로그인 할 수 없습니다.|창을 종료하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_PWD_CHK_NOT_LOGIN");
                return this.BaseClass.BUTTON_CONFIRM_YN;
            }
            catch { throw; }
        }
        #endregion

        #region > 데이터 관련
        #region >> ChangeCOMM_USER_SET_PW - 비밀번호 변경
        /// <summary>
        /// 비밀번호 변경
        /// </summary>
        /// <returns></returns>
        private async Task<bool> UpdateSP_LOGIN_PWD_UPD(BaseDataAccess _da)
        {
            try
            {
                bool isRtnValue = true;

                #region 파라메터 변수 선언 및 값 할당
                DataTable dtRtnValue                        = null;
                var strProcedureName                        = "PK_C1001.SP_LOGIN_PWD_UPD";
                Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
                string[] arrOutputParam                     = { "O_RTN_RSLT" };

                var strPwd              = this.BaseClass.EncryptSHA256(this.txtPassword.Text.Trim());       // 단방향 암호화 데이터
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CNTR_CD",          this.g_strCenterCD);        // 센터코드
                dicInputParam.Add("P_USER_ID",          this.g_strUserID);          // 사용자 ID
                dicInputParam.Add("P_PWD",              strPwd);                    // 비밀번호
                #endregion

                await System.Threading.Tasks.Task.Run(() =>
                {
                    dtRtnValue = _da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);


                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                        {
                            var strMessage = dtRtnValue.Rows[0]["MSG"].ToString();
                            this.BaseClass.MsgError(strMessage);
                            isRtnValue = false;
                        }
                    }
                    else
                    {
                        // ERR_SAVE - 저장 중 오류가 발생했습니다.
                        this.BaseClass.MsgError("ERR_SAVE");
                        isRtnValue = false;
                    }
                }

                return isRtnValue;
            }
            catch
            {
                // ERR_SAVE_DATA - 저장 중 오류가 발생했습니다.
                this.BaseClass.MsgError("ERR_SAVE");

                throw;
            }
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 화면 로드 이벤트
        #region >> WWCS914_01P_Loaded - 화면 로드 이벤트
        /// <summary>
        /// 화면 로드 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WWCS914_01P_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //double boundWidth = 0;
                //double boundHeight = 0;

                //if (WindowStartupLocation == WindowStartupLocation.CenterScreen)
                //{
                //    boundWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                //    boundHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

                //    this.Left = boundWidth / 2 - this.Width / 2;
                //    this.Top = boundHeight / 2 - this.Height / 2;
                //}
                //else if (WindowStartupLocation == WindowStartupLocation.CenterOwner)
                //{
                //    if (Application.Current.MainWindow != null)
                //    {
                //        var _bound = Application.Current.MainWindow.RestoreBounds;

                //        boundWidth = _bound.Width;
                //        boundHeight = _bound.Height;

                //        this.Left = (boundWidth / 2 - this.Width / 2) + _bound.Left;
                //        this.Top = (boundHeight / 2 - this.Height / 2) + _bound.Top;
                //    }
                //}
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region > 버튼 이벤트
        #region >> btnPASSWORD_CHG_PreviewMouseLeftButtonUp - 비밀번호 변경 버튼 클릭
        /// <summary>
        /// 비밀번호 변경 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnConfirm_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isRtnValue         = true;

                // 1. 비밀번호 입력 체크
                if (this.txtPassword.Text.Trim().Length == 0)
                {
                    // ERR_NOT_INPUT_PWD - 비밀번호가 입력되지 않았습니다.
                    this.BaseClass.MsgError("ERR_NOT_INPUT_PWD");
                    this.txtPassword.Focus();
                    return;
                }

                // 2. 비밀번호 확인 입력 체크
                if (this.txtPasswordConf.Text.Trim().Length == 0)
                {
                    // ERR_NOT_INPUT_PWD_CONF - 비밀번호 확인값이 입력되지 않았습니다.
                    this.BaseClass.MsgError("ERR_NOT_INPUT_PWD_CONF");
                    this.txtPasswordConf.Focus();
                    return;
                }

                // 3. 비밀번호 일치 여부 체크
                if (this.txtPassword.Text.Trim().Equals(this.txtPasswordConf.Text.Trim()) == false)
                {
                    // ERR_PWD_EQUAL - 입력하신 비밀번호가 일치하지 않습니다.
                    this.BaseClass.MsgError("ERR_PWD_EQUAL");
                    return;
                }

                // 4. 비밀번호는 14자 이상이여야합니다.
                if (this.txtPassword.Text.Trim().Length < 14)
                {
                    // PWD_CHG_SWCS201_02_FIRST - 암호는 14자 이상이어야 합니다.
                    this.BaseClass.MsgError("PWD_CHG_SWCS201_02_FIRST");
                    return;
                }

                // 4. 비밀번호 Validation 체크
                if (this.CheckValidatePassword() == false)
                {
                    // ERR_PWD_ROLE - 비밀번호 규칙이 잘못되었습니다.
                    this.BaseClass.MsgError("ERR_PWD_ROLE");
                    return;
                }

                // 5. 오류가 없는 경우 비밀번호 변경 로직 수행
                // ASK_PWD_CHG - 비밀번호를 재설정하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_PWD_CHG");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        da.BeginTransaction();

                        isRtnValue = await this.UpdateSP_LOGIN_PWD_UPD(da);

                        if (isRtnValue == false)
                        {
                            da.RollbackTransaction();
                            // 비밀번호 재설정 실패
                            this.PasswordChangeResult(false);
                        }
                        else
                        {
                            // CMPT_PWD_CHK - 비밀번호가 재설정 되었습니다.|확인 버튼을 클릭하면 메인 화면이 열립니다.
                            this.BaseClass.MsgInfo("CMPT_PWD_CHK");
                            da.CommitTransaction();

                            // 비밀번호 재설정 성공
                            this.PasswordChangeResult(true);

                            this.Close();
                        }
                    }
                    catch
                    {
                        da.RollbackTransaction();
                        throw;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
                this.PasswordChangeResult(false);
            }
        }
        #endregion

        #region >> BtnClose_MouseEnter - 닫기 버튼 마우스 엔터
        /// <summary>
        /// 닫기 버튼 마우스 엔터
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                imgClose.Opacity = 0.9;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> btnCLOSE_PreviewMouseLeftButtonUp - 닫기 버튼 클릭
        /// <summary>
        /// 닫기 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.AskWindowClose() == true)
                {
                    this.PasswordChangeResult(false);
                    this.Close();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> btnClose_IsEnabledChanged
        private void btnCancel_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region >> BtnClose_MouseLeave 
        private void BtnCancel_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                imgClose.Opacity = 0.7;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region > Grid_PreviewMouseDown - 창 헤더를 마우스로 클릭 시 이동
        /// <summary>
        /// 창 헤더를 마우스로 클릭 시 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
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
        // ~SWCS201_02P()
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
