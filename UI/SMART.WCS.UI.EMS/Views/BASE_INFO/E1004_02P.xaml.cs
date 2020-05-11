using DevExpress.Mvvm.Native;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.E1004;
using SMART.WCS.UI.EMS.Views.COM;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SMART.WCS.UI.EMS.Views.BASE_INFO
{
    /// <summary>
    /// EBSE004_02P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E1004_02P : Window, IDisposable
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
        #endregion

        #region ▩ 생성자
        public E1004_02P()
        {
            InitializeComponent();
        }

        public E1004_02P(EmsEqpInfo _target)
        {
            try
            {
                InitializeComponent();

                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                btnFormClose.Click += btnFormClose_Click;

                CurrEqpInfo = _target;

                // 신규 작성시에만 ID 편집 가능
                //
                gridPartView.ShowingEditor += (s, e) =>
                {
                    e.Cancel = gridPart.CurrentColumn.FieldName != "INST_DT" && gridPart.CurrentColumn.FieldName != "INST_QTY";
                };

                this.Loaded += EBSE004_02P_Loaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 속성
        private EmsEqpInfo CurrEqpInfo { get; set; }

        /// <summary>
        /// Grid Data
        /// </summary>
        private ObservableCollection<EmsPartByEqp> PartByEqpList { get; set; }
        #endregion

        #region ▩ 함수
        #region 부품 참조
        /// <summary>
        /// 부품 참조
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPartRef_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DateTime now = DateTime.Now;

                using (ECOM001_05P frmPartRef = new ECOM001_05P(true))
                {
                    frmPartRef.ShowDialog();

                    if (frmPartRef.liEmsPartRef != null)
                    {
                        foreach (DataMembers.COM.EmsPartRef item in frmPartRef.liEmsPartRef)
                        {
                            // 부품참조 창에서 선택한 데이터와 중복되는 경우 DataMember에 추가하지 않는다.
                            var isDuplicate = this.PartByEqpList.Where(p => p.PART_ID.Equals(item.PART_ID)).Count() > 0 ? true : false;

                            if (isDuplicate == false)
                            { 
                                EmsPartByEqp _item = new EmsPartByEqp()
                                {
                                    PART_ID         = item.PART_ID,
                                    PART_NM         = item.PART_NM,
                                    LIFE_CLE        = item.LIFE_CLE,
                                    PART_SERIAL_NO  = 0,
                                    INST_DT         = DateTime.Now,
                                    INST_QTY        = 1,
                                    IsNew           = true
                                };

                                PartByEqpList.Add(_item);
                            }
                        }
                    }

                    gridPart.Focus();
                    gridPart.CurrentColumn = gridPart.Columns.First();
                    gridPart.View.FocusedRowHandle = PartByEqpList.Count - 1;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion 부품 참조

        #region 조회
        /// <summary>
        /// 조회 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPartSearch_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (null != CurrEqpInfo && !string.IsNullOrEmpty(CurrEqpInfo.EQP_ID))
                {
                    PartByEqpSearch();

                    btnSave.IsEnabled = true;

                    btnRowDel.IsEnabled = true;
                    btnRowAdd.IsEnabled = true;
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 조회 함수
        /// </summary>
        private void PartByEqpSearch()
        {
            try
            {
                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_EQP_ID", CurrEqpInfo.EQP_ID}
                };

                var strOutParam = new[] { "P_EMS_DETAIL_LIST" };
                string callProc = "PK_EMS_EBSE004_02P.SP_EMS_EQP_DETAIL_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(  
                                    callProc                      // 호출 프로시저
                                ,   param                         // Input 파라메터
                                ,   strOutParam                   // Output 파라메터
                    );

                    PartByEqpList = new ObservableCollection<EmsPartByEqp>();

                    if (0 < outData.Tables.Count)
                    {
                        if (outData.Tables[0].Rows.Count > 0)
                        {
                            PartByEqpList.ToObservableCollection(outData.Tables[0]);
                        }
                    }

                    gridPart.ItemsSource = null;
                    gridPart.ItemsSource = PartByEqpList;
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 조회

        #region 저장
        /// <summary>
        /// 저장 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPartSave_Click(object sender, MouseButtonEventArgs e)
        {
            PartByEqpSave();
            PartByEqpSearch();
        }

        /// <summary>
        /// 저장 함수
        /// </summary>
        private void PartByEqpSave()
        {
            using (BaseDataAccess da = new BaseDataAccess())
            {
                try
                {
                    string _SUCCESS_CODE = "100";
                    var _saveItems = PartByEqpList.Where(f => f.IsUpdate || f.IsNew).ToList();

                    if (0 == _saveItems.Count)
                    {
                        return;
                    }

                    var _USER_ID = this.BaseClass.UserID;

                    da.BeginTransaction();

                    foreach (var item in _saveItems)
                    {
                        var param = new Dictionary<string, object>
                            {
                                { "P_CENTER_CD", this.BaseClass.CenterCD },
                                { "P_EQP_ID", CurrEqpInfo.EQP_ID },                                                     // 설비 ID
                                { "P_PART_SERIAL_NO", item.PART_SERIAL_NO },
                                { "P_INST_DT", new DateTime(item.INST_DT.Year, item.INST_DT.Month, item.INST_DT.Day) }, // 설치 일자
                                { "P_INST_QTY", item.INST_QTY },                                                        // 설치 수량
                                { "P_PART_ID", item.PART_ID },                                                          // 부품 ID
                                { "P_PART_NM", item.PART_NM },
                                {"P_USER_ID", _USER_ID}
                            };

                        var strOutParam = new[] { "P_RESULT" };
                        string callProc = "PK_EMS_EBSE004_02P.SP_EMS_EQP_DETAIL_SAVE";

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
                                BaseClass.MsgError(outData.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                                break;
                            }
                        }
                        else
                        {
                            _SUCCESS_CODE = "0";
                            da.RollbackTransaction();
                            this.BaseClass.MsgError("ERR_INPUT_TYPE");
                            break;
                        }
                    }

                    if (_SUCCESS_CODE == "100")
                    {
                        da.CommitTransaction();
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_SAVE_DATA"));

                        this.BaseClass.MsgInfo("CMPT_SAVE");
                        this.Close();
                    }

                }
                catch (Exception ex)
                {
                    da.RollbackTransaction();
                    this.BaseClass.Error(ex);
                    this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }
        }
        #endregion 저장

        #region row 삭제
        /// <summary>
        /// 삭제 버튼 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRowDelete_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DelEmsEqpMngr();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        /// <summary>
        /// 삭제 함수
        /// </summary>
        private void DelEmsEqpMngr()
        {
            EmsPartByEqp item = gridPart.SelectedItem as EmsPartByEqp;

            if (null != item)
            {
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    string _SUCCESS_CODE = "100";

                    try
                    {
                        var _USER_ID = this.BaseClass.UserID;

                        da.BeginTransaction();

                        //foreach (var item in _delItems)
                        {
                            if (item.IsNew != true)
                            {
                                var param = new Dictionary<string, object>
                                {
                                    { "P_CENTER_CD", this.BaseClass.CenterCD },
                                    { "P_EQP_ID", CurrEqpInfo.EQP_ID },
                                    { "P_PART_SERIAL_NO", item.PART_SERIAL_NO },
                                    { "P_USER_ID", _USER_ID }
                                };

                                var strOutParam = new[] { "P_RESULT" };
                                string callProc = "PK_EMS_EBSE004_02P.SP_EMS_EQP_DETAIL_DELETE";

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
                                    this.BaseClass.MsgError("ERR_INPUT_TYPE");
                                    //break;
                                }
                            }
                        }

                        if (_SUCCESS_CODE == "100")
                        {
                            da.CommitTransaction();

                            PartByEqpList.Remove(item);

                            gridPart.RefreshData();
                        }
                    }
                    catch (Exception ex)
                    {
                        da.RollbackTransaction();
                        this.BaseClass.Error(ex);
                        this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
                    }
                }
            }
            else
            {
                this.BaseClass.MsgError("ERR_NO_DEL");
            }
        }
        #endregion row 삭제
        #endregion

        #region ▩ 이벤트
        /// <summary>
        /// 화면 오픈시 선행 작업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EBSE004_02P_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= EBSE004_02P_Loaded;


            if (null != CurrEqpInfo && !string.IsNullOrEmpty(CurrEqpInfo.EQP_ID))
            {
                tbxEqpId.Text = CurrEqpInfo.EQP_ID;
                tbxEqpNm.Text = CurrEqpInfo.EQP_NM;
                tbxEqpMnfact.Text = CurrEqpInfo.EQP_MNFACT;
            }

            this.PartByEqpSearch();
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
                //this.BaseClass.Error(err);
            }
        }

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
                //this.BaseClass.Error(err);
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
        // ~EBSE004_02P()
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
