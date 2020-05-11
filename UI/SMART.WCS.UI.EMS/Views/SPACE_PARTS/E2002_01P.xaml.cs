using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.ESPA002;
using SMART.WCS.UI.EMS.Views.COM;
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

namespace SMART.WCS.UI.EMS.Views.SPACE_PARTS
{
    /// <summary>
    /// E2001_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E2002_01P : Window, IDisposable
    {
        #region ▩ Delegate
        public delegate void SearchResult(string _strCode, string _strName);
        public event SearchResult CustomerSearchResult;
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// 공통 Class를 이용하기 위한 BaseClass 선언
        /// </summary>
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;

        bool alreadySave = false;

        public bool IsSaved = false;
        #endregion

        #region ▩ 생성자
        public E2002_01P()
        {
            try
            {
                InitializeComponent();
                this.Name = this.GetType().Name;

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                btnFormClose.Click += btnFormClose_Click;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 속성
        public EmsStock CurrentStock { get; private set; }
        #endregion

        #region ▩ 함수
        #region 관리자 검색
        /// <summary>
        /// 관리자 검색
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StockMngrSearch_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (ECOM001_07P frmEqpMngrSearch = new ECOM001_07P())
                {
                    frmEqpMngrSearch.ShowDialog();

                    if (null != frmEqpMngrSearch.CurrMngr)
                    {
                        tbxManager.Text = frmEqpMngrSearch.CurrMngr.MNGR_NM;
                        tbxManager.Tag = frmEqpMngrSearch.CurrMngr.EQP_MNGR_ID;
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion 관리자 검색

        #region 저장
        /// <summary>
        /// 저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, MouseButtonEventArgs e)
        {
            if (this.tbxManager.Tag == null)
            {
                // 담당자를 입력하세요.
                this.BaseClass.MsgError("ERR_INPUT_MGNR");
                return;
            }

            if (this.tbxManager.Tag.ToString().Length == 0)
            {
                // 담당자를 입력하세요.
                this.BaseClass.MsgError("ERR_INPUT_MGNR");
                return;
            }

            if (null != tbxManager.Tag && !string.IsNullOrEmpty(tbxManager.Tag.ToString()))
            {
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    try
                    {
                        string _SUCCESS_CODE = "100";

                        var _USER_ID = this.BaseClass.UserID;

                        da.BeginTransaction();

                        var strInspDate = this.BaseClass.GetCalendarValue(this.dtInsp);

                        var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD",    this.BaseClass.CenterCD },
                                { "P_MNGR_ID", tbxManager.Tag.ToString() },     // 관리자
                                //{ "P_STOCK_INSP_DT", new DateTime(dtInsp.DateTime.Year, dtInsp.DateTime.Month, dtInsp.DateTime.Day) },  // 조사 일자
                                { "P_STOCK_INSP_DT",    strInspDate },
                                { "P_NOTE", tbxNote.Text.Trim() },  // 비고
                                { "P_USER_ID", _USER_ID }
                            };

                        var strOutParam = new[] { "P_RESULT" };
                        string callProc = "PK_EMS_ESPA002_01P.SP_EMS_STOCK_INSP_DT_INSERT";

                        var outData = da.GetSpDataSet(
                                    callProc                      // 호출 프로시저
                                ,   param                        // Input 파라메터
                                ,   strOutParam                  // Output 파라메터
                        );

                        if (outData.Tables[0].Rows.Count > 0)
                        {
                            if (outData.Tables[0].Rows[0]["CODE"].ToString() != _SUCCESS_CODE)
                            {
                                _SUCCESS_CODE = "0";
                                da.RollbackTransaction();
                                BaseClass.MsgInfo(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                            }
                        }
                        else
                        {
                            _SUCCESS_CODE = "0";
                            da.RollbackTransaction();
                            this.BaseClass.MsgError("INPUT_TYPE_ERR");
                        }

                        if (_SUCCESS_CODE == "100")
                        {
                            da.CommitTransaction();
                            //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_SAVE_DATA"));

                            // 저장되었습니다.
                            this.BaseClass.MsgInfo("CMPT_SAVE");
                            IsSaved = true;

                            CurrentStock = new EmsStock();
                            CurrentStock.MNGR_ID = tbxManager.Tag.ToString().Trim();
                            CurrentStock.MNGR_NM = tbxManager.Text.ToString().Trim();
                            CurrentStock.INSP_STAT_CD = "S";
                            CurrentStock.STOCK_INSP_DT = new DateTime(dtInsp.DateTime.Year, dtInsp.DateTime.Month, dtInsp.DateTime.Day);
                            CurrentStock.NOTE = tbxNote.Text.Trim();
                            CurrentStock.IsUpdate = false;

                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        da.RollbackTransaction();
                        this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                    }
                }
            }

        }
        #endregion 저장
        #endregion

        #region ▩ 이벤트
        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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

        /// <summary>
        /// Close 이미지 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFormClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
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
        // ~E2001_01P()
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
