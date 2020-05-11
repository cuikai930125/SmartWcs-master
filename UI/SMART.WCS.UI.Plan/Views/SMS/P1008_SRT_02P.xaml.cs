using DevExpress.Mvvm.Native;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.Plan.DataMembers.P1008_SRT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace SMART.WCS.UI.Plan.Views.SMS
{
    /// <summary>
    /// P1008_SRT_02P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class P1008_SRT_02P : Window, IDisposable
    {
        #region ▩ 전역변수
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 설비 ID
        /// </summary>
        private string g_strEqpID = string.Empty;

        /// <summary>
        /// 엑셀 업로드 번호
        /// </summary>
        private string g_strUploadNo = string.Empty;
        #endregion

        #region ▩ 생성자
        public P1008_SRT_02P()
        {
            InitializeComponent();
        }

        public P1008_SRT_02P(string _strEqpID, string _strUploadNo)
        {
            try
            {
                InitializeComponent();

                this.g_strEqpID     = _strEqpID;
                this.g_strUploadNo  = _strUploadNo;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 이벤트 초기화
                this.InitEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        #region > 슈트 매핑 리스트
        public static readonly DependencyProperty ChuteMpngListProperty
            = DependencyProperty.Register("ChuteMpngList", typeof(ObservableCollection<ChuteMpngUploadRslt>), typeof(P1008_SRT_02P)
                , new PropertyMetadata(new ObservableCollection<ChuteMpngUploadRslt>()));

        public ObservableCollection<ChuteMpngUploadRslt> ChuteMpngList
        {
            get { return (ObservableCollection<ChuteMpngUploadRslt>)GetValue(ChuteMpngListProperty); }
            set { SetValue(ChuteMpngListProperty, value); }
        }
        #endregion
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            this.BaseClass.BindingFirstComboBox(this.cboSuccYN, "SUCC_TYPE");

            if (this.BaseClass.ComboBoxItemCount(this.cboSuccYN) > 0)
            {
                this.cboSuccYN.SelectedIndex = 1;
            }
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        private void InitEvent()
        {
            // 조회 버튼 클릭 이벤트
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;

            // 확정 버튼 클릭 이벤트
            this.btnConfr.PreviewMouseLeftButtonUp += BtnConfr_PreviewMouseLeftButtonUp;
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> 엑셀 업로드 슈트 매핑 리스트 조회
        /// <summary>
        /// 엑셀 업로드 슈트 매핑 리스트 조회
        /// </summary>
        /// <returns></returns>
        private async Task<DataSet> GetSP_CHUTE_PLAN_UPLD_LIST_INQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_P1008_SRT.SP_CHUTE_PLAN_UPLD_LIST_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_UPLD_LIST", "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                                      // 센터코드
            var strSuccType         = this.BaseClass.ComboBoxSelectedKeyValue(this.cboSuccYN);      // 성공 여부
            var strUserID           = this.BaseClass.UserID;
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCenterCD);               // 센터코드
            dicInputParam.Add("P_EQP_ID",           this.g_strEqpID);           // 설비 ID
            dicInputParam.Add("P_UPLD_NO",          this.g_strUploadNo);        // 엑셀 업로드 번호
            dicInputParam.Add("P_USER_ID",          strUserID);                 // 사용자 ㅑㅇ
            dicInputParam.Add("P_PROC_RSLT_CD",     strSuccType);               // 성공 여부
            
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion

        #region >> 슈트 매핑 엑셀 업로드 정보 확정
        /// <summary>
        /// 슈트 매핑 엑셀 업로드 정보 확정
        /// </summary>
        /// <param name="_da">데이터베이스 엑세스 객체</param>
        /// <returns></returns>
        private async Task<bool> SaveSP_CHUTE_PLAN_UPLD_LIST_CONF(BaseDataAccess _da)
        {
            bool isRtnValue  = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue                          = null;
            var strProcedureName                        = "PK_P1008_SRT.SP_CHUTE_PLAN_UPLD_LIST_CONF";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_RSLT" };

            var strCenterCD         = this.BaseClass.CenterCD;                                  // 센터코드
            var strUserID           = this.BaseClass.UserID;                                    // 사용자 ID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD",              strCenterCD);               // 센터코드
            dicInputParam.Add("P_EQP_ID",               this.g_strEqpID);           // 설비 ID
            dicInputParam.Add("P_UPLD_NO",              this.g_strUploadNo);        // 엑셀 업로드 번호
            dicInputParam.Add("P_USER_ID",              strUserID);                 // 사용자 ID
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
                        this.BaseClass.MsgError(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue  = false;
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
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 버튼 클릭 이벤트
        #region >> 조회 버튼 클릭 이벤트
        /// <summary>
        /// 조회 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 슈트 매핑 리스트를 조회한다.
                DataSet dsRtnValue = await this.GetSP_CHUTE_PLAN_UPLD_LIST_INQ();

                if (dsRtnValue == null) { return; }
                var strErrCode      = string.Empty;
                var strErrMsg       = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == false)
                {
                    this.ChuteMpngList.ToObservableCollection(null);
                    // 오류 메세지 출력
                    this.BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }
                else
                {
                    this.ChuteMpngList = new ObservableCollection<ChuteMpngUploadRslt>();
                    this.ChuteMpngList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                
                // 조회 데이터를 그리드에 바인딩한다.
                this.gridMain.ItemsSource = this.ChuteMpngList;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        #region >> 확인 버튼 클릭 이벤트
        /// <summary>
        /// 확인 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnConfr_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //// 그리드 내 체크박스 선택 여부 체크
                //if (this.CheckGridRowSelected() == false) { return; }

                // ASK_CONF - 확정 처리를 하시겠습니까?
                this.BaseClass.MsgQuestion("ASK_CONF");
                if (this.BaseClass.BUTTON_CONFIRM_YN == false) { return; }

                bool isRtnValue         = true;

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        // 상태바 (아이콘) 실행
                        this.loadingScreen.IsSplashScreenShown = true;

                        da.BeginTransaction();

                        isRtnValue = await this.SaveSP_CHUTE_PLAN_UPLD_LIST_CONF(da);

                        if (isRtnValue == true)
                        {
                            // 저장된 경우 트랜잭션을 커밋처리한다.
                            da.CommitTransaction();

                            // CMPT_CONF - 확정 처리 되었습니다.
                            this.BaseClass.MsgInfo("CMPT_CONF");

                            this.BtnSearch_PreviewMouseLeftButtonUp(null, null);
                        }
                        else
                        {
                            // 오류 발생하여 저장 실패한 경우 트랜잭션을 롤백처리한다.
                            da.RollbackTransaction();
                        }
                    }
                    catch
                    {
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }

                        /// ERR_CONF - 확정 처리중 오류가 발생했습니다.
                        this.BaseClass.MsgError("ERR_CONF");
                        throw;
                    }
                    finally
                    {
                        // 상태바 (아이콘) 제거
                        this.loadingScreen.IsSplashScreenShown = false;
                    }
                }

                this.Close();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
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
        // ~P1008_SRT_02P()
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
