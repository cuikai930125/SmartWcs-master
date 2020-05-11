using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Modules.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1023_GAN;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace SMART.WCS.UI.COMMON.Views.GANTRY
{
    /// <summary>
    /// 토트박스관리 - PopUp
    /// </summary>
    public partial class C1023_GAN_01P : Window, IDisposable
    {
        #region ▩ 전역변수
        /// <summary>
        /// Base클래스 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        public static readonly DependencyProperty TotBoxTypeListProperty
           = DependencyProperty.Register("TotBoxTypeList", typeof(ObservableCollection<TotBoxInfo>), typeof(C1023_GAN_01P)
               , new PropertyMetadata(new ObservableCollection<TotBoxInfo>()));

        public ObservableCollection<TotBoxInfo> TotBoxTypeList
        {
            get { return (ObservableCollection<TotBoxInfo>)GetValue(TotBoxTypeListProperty); }
            set { SetValue(TotBoxTypeListProperty, value); }
        }
        #endregion

        #region ▩ 생성자
        /// <summary>
        /// 생성자
        /// </summary>
        public C1023_GAN_01P()
        {
            InitializeComponent();

            // 이벤트 초기화
            this.InitEvent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="totBoxTypeList">토트박스 타입 정보</param>
        public C1023_GAN_01P(ObservableCollection<TotBoxInfo> totBoxTypeList)
        {
            InitializeComponent();

            // 이벤트 초기화
            this.InitEvent();

            this.TotBoxTypeList = totBoxTypeList;
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
                //#region 폼 이벤트
                this.Loaded += C1023_GAN_01P_Loaded; ;

                // 폼 종료 버튼 클릭 이벤트
                this.btnCancel.Click += BtnCancel_Click;

                //화면 상단 X버튼 클릭 이벤트
                this.btnFormClose.Click += BtnFormClose_Click; ;

                // 저장
                this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;

                this.cboTotBoxType.SelectedIndexChanged += CboTotBoxType_SelectedIndexChanged;

            }
            catch { throw; }
        }

        /// <summary>
        /// 저장 버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isRtnValue = false;

                if (string.IsNullOrWhiteSpace(this.txtTotBoxFr.Text) == true)
                {
                    this.BaseClass.MsgError("ERR_TOT_BOX_NO_NOT_INPUT");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(cboTotBoxType.SelectedKey()) == true)
                {
                    this.BaseClass.MsgError("ERR_TOT_BOX_TYPE_NOT_INPUT");
                    return;
                }                
                else if (string.IsNullOrWhiteSpace(this.txtTotCnt.Text) == true
                || int.Parse(this.txtTotCnt.Text) == 0)
                {
                    this.BaseClass.MsgError("ERR_TOT_BOX_COUNT_NOT_INPUT");
                    return;
                }

                var strMessage = this.BaseClass.GetResourceValue("ASK_TOT_BOX_BATCH_SAVE", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    using (BaseDataAccess da = new BaseDataAccess())
                    {
                        try
                        {
                            da.BeginTransaction();

                            isRtnValue = this.SetSP_COMM_TOTE_SAVE_BTCH(da);

                            if (isRtnValue == true)
                            {
                                // 저장된 경우
                                da.CommitTransaction();

                                BaseClass.MsgInfo("CMPT_SAVE");

                            }
                            else
                            {
                                // 오류 발생하여 저장 실패한 경우
                                da.RollbackTransaction();
                            }
                        }
                        catch
                        {
                            if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                            {
                                da.RollbackTransaction();
                            }

                            BaseClass.MsgError("ERR_SAVE");
                            throw;
                        }
                        finally
                        {
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #region SetSP_COMM_TOTE_SAVE_BTCH
        /// <summary>
        /// TOTE BOX List Batch Save
        /// </summary>
        /// <param name="da"></param>
        /// <returns></returns>
        private bool SetSP_COMM_TOTE_SAVE_BTCH(BaseDataAccess da)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1023_GAN.SP_COMM_TOTE_SAVE_BTCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUT_RESULT" };

            var strCoCd = BaseClass.CompanyCode;                    // 회사 코드
            var strCntrCd = BaseClass.CenterCD;                     // 센터 코드           
            var strTotBcrFr = this.txtTotBoxFr.Text;                      // 토트박스 번호
            var strBoxTypeCd = this.cboTotBoxType.SelectedKey();                    // 토트박스 타입
            var strTotCnt = this.txtTotCnt.Text;        // 사용 여부
            var strUserID = this.BaseClass.UserID;                  // 사용자 ID

            var strErrCode = string.Empty;                         // 오류 코드
            var strErrMsg = string.Empty;                          // 오류 메세지
            #endregion

            #region Input 파라메터     
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);              // 센터 코드
            dicInputParam.Add("P_TOT_BCR_FR", strTotBcrFr);         // 토트박스 ID
            dicInputParam.Add("P_BOX_TYPE_CD", strBoxTypeCd);       // 토트박스 타입 코드
            dicInputParam.Add("P_TOT_CNT", strTotCnt);                // 사용 여부
            dicInputParam.Add("P_USER_ID", strUserID);              // 사용자 ID
            #endregion

            dtRtnValue = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        BaseClass.MsgInfo(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }
        #endregion

        private void CboTotBoxType_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            TotBoxInfo cItem = cboTotBoxType.SelectedItem as TotBoxInfo;
         
            txtWTH.Text = cItem.BOX_WTH_LEN;
            txtVERT.Text = cItem.BOX_VERT_LEN;
            txtHGT.Text = cItem.BOX_HGT_LEN;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void C1023_GAN_01P_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dtRtnValue = this.GetSP_COMM_TOTE_ADD();

                if (dtRtnValue != null)
                {
                    if (dtRtnValue.Rows.Count > 0)
                    {
                        txtMaxTotBoxId.Text = dtRtnValue.Rows[0]["MAX_BCR_NO"].ToString();
                        txtTotBoxFr.Text = dtRtnValue.Rows[0]["TOT_BOX_FR"].ToString();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.CloseProcess();
        }
        
        private void BtnFormClose_Click(object sender, RoutedEventArgs e)
        {
            this.CloseProcess();
        }

        /// <summary>
        /// 팝업 닫기 전 확인 메세지
        /// </summary>
        private void CloseProcess()
        {
            // 토트박스 일괄 등록을 취소하시겠습니까?
            this.BaseClass.MsgQuestion("ASK_TOT_BOX_BATCH_SAVE_CANCEL");
            if (this.BaseClass.BUTTON_CONFIRM_YN == true) 
            {
                this.Close();
            }
        }

        #endregion

        #region ▩ 함수
        /// <summary>
        /// GetSP_COMM_TOTE_ADD
        /// </summary>
        /// <returns></returns>
        private DataTable GetSP_COMM_TOTE_ADD()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataTable dsRtnValue = null;
            var strProcedureName = "PK_C1023_GAN.SP_COMM_TOTE_ADD";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                               // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            
            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                    // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);                  // 센터 코드
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                //dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                dsRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
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
        // ~SWCS101_01P()
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
