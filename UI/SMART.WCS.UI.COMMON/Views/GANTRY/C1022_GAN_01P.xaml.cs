using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Common.Extensions;
using SMART.WCS.UI.COMMON.DataMembers.C1022_GAN;
using SMART.WCS.UI.COMMON.DataMembers.C1022_GAN_01P;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.COMMON.Views.GANTRY
{
    /// <summary>
    /// Ground 셀관리 - PopUp
    /// </summary>
    public partial class C1022_GAN_01P : Window, IDisposable
    {
        #region ▩ 전역변수
        /// <summary>
        /// Base클래스 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        #endregion

        public static readonly DependencyProperty CellInfoDatasProperty
           = DependencyProperty.Register("CellInfoDatas", typeof(List<List<ToteCellItemInfo>>), typeof(C1022_GAN_01P)
               , new PropertyMetadata(new List<List<ToteCellItemInfo>>()));

        public List<List<ToteCellItemInfo>> CellInfoDatas
        {
            get { return (List<List<ToteCellItemInfo>>)GetValue(CellInfoDatasProperty); }
            set { SetValue(CellInfoDatasProperty, value); }
        }

        public static readonly DependencyProperty StockInfoListProperty
            = DependencyProperty.Register("StockInfoList", typeof(ObservableCollection<StockInfo>), typeof(C1022_GAN_01P)
                , new PropertyMetadata(new ObservableCollection<StockInfo>()));

        public ObservableCollection<StockInfo> StockInfoList
        {
            get { return (ObservableCollection<StockInfo>)GetValue(StockInfoListProperty); }
            set { SetValue(StockInfoListProperty, value); }
        }

        public static readonly DependencyProperty SelectCellInfoProperty
            = DependencyProperty.Register("SelectCellInfo", typeof(CellMstInfo), typeof(C1022_GAN_01P)
                , new PropertyMetadata(null));

        public CellMstInfo SelectCellInfo
        {
            get { return (CellMstInfo)GetValue(SelectCellInfoProperty); }
            set { SetValue(SelectCellInfoProperty, value); }
        }
        
        string EqpId = string.Empty;

        /// <summary>
        /// 선택된 셀 ID
        /// </summary>
        string SelectCellId = string.Empty;

        public C1022_GAN_01P(string pEqpId)
        {
            InitializeComponent();

            this.EqpId = pEqpId;

            // 컨트롤 관련 초기화
            this.InitControl();

            // 이벤트 초기화
            InitEvent();
        }

        private void InitControl()
        {
            // 콤보박스 - 공통코드
            this.BaseClass.BindingCommonComboBox(this.cboCellType, "CELL_TYPE_CD", null, false);
        }

        #region InitEvent - 이벤트 초기화한다.
        /// <summary>
        /// 이벤트 초기화한다.
        /// </summary>
        private void InitEvent()
        {
            try
            {
                //#region 폼 이벤트
                this.Loaded += C1022_GAN_01P_Loaded;

                this.KeyDown += C1022_GAN_01P_KeyDown;

                // 저장
                this.btnSave_Tab1.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;

                // 폼 종료 버튼 클릭 이벤트
                this.btnCancel.Click += BtnCancel_Click;

                //화면 상단 X버튼 클릭 이벤트
                this.btnFormClose.Click += BtnFormClose_Click;

                tvMasterGrid.CellMerge += tvMasterGrid_CellMerge;
            }
            catch { throw; }
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectCellId))
            {
                GetSP_CELL_MST_SEARCH();
            }
        }

        private void C1022_GAN_01P_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key.Equals(Key.F5))
            {
                //MessageBox.Show("F5 눌렀군.");
                
                CellInfoSearch();
            }
        }

        private void C1022_GAN_01P_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CellInfoSearch();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void CellInfoSearch()
        {
            try
            {
                // 선택셀 정보 초기화
                SelectCellId = string.Empty;
                CellDetailClear();

                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_CELL_INFO();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                #region MakeCellInfoData
                // MakeCellInfoData

                CellInfoDatas = new List<List<ToteCellItemInfo>>();
                List<ToteCellItemInfo> CellInfoData = new List<ToteCellItemInfo>();

                foreach (DataRow row in dsRtnValue.Tables["O_LIST"].Rows)   // 9
                {
                    string[] RowCellInfo = row["CELL_INFO"].ToString().Split('^');

                    CellInfoData = new List<ToteCellItemInfo>();

                    foreach (var item in RowCellInfo)
                    {
                        if (item.Trim().Length > 0)
                        {
                            string[] cCellInfo = item.Split(',');

                            CellInfoData.Add(new ToteCellItemInfo
                            {
                                CellId = cCellInfo[0].ToString(),
                                ColorType = cCellInfo[1].ToString(),
                                StockQty = int.Parse(cCellInfo[2].ToString())
                            });
                        }
                        else
                        {
                            break;
                        }
                    }

                    CellInfoDatas.Add(CellInfoData);
                }
                #endregion

                lstCellStack.ItemsSource = CellInfoDatas;

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }

        private DataSet GetSP_CELL_INFO()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_C1022_GAN.SP_CELL_INFO";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                               // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            var strEqpId = this.EqpId;

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);              // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);          // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpId);            // 설비 ID            
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
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

                this.BaseClass.MsgQuestion("ASK_CHANGE_DATA_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    using (BaseDataAccess da = new BaseDataAccess())
                    {
                        try
                        {
                            // 상태바 (아이콘) 실행
                            this.loadingScreen.IsSplashScreenShown = true;

                            da.BeginTransaction();
                            
                            //PK_C1022_GAN.SP_CELL_MST_SAVE
                            isRtnValue = this.SetSP_CELL_MST_SAVE(da);

                            if (isRtnValue == true)
                            {
                                // 저장된 경우
                                da.CommitTransaction();

                                // 상태바 (아이콘) 제거
                                this.loadingScreen.IsSplashScreenShown = false;

                                // 저장되었습니다.
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
                            // 상태바 (아이콘) 제거
                            this.loadingScreen.IsSplashScreenShown = false;

                            // 재조회
                            CellInfoSearch();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #region >> SetSP_CELL_MST_SAVE - 저장
        /// <summary>
        /// Equipment 등록
        /// </summary>
        /// <param name="da">DataAccess 객체</param>
        /// <param name="item">저장 대상 아이템 (Row 데이터)</param>
        /// <returns></returns>
        private bool SetSP_CELL_MST_SAVE(BaseDataAccess da)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1022_GAN.SP_CELL_MST_SAVE";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUT_RESULT" };

            var strCoCd = BaseClass.CompanyCode;                    // 회사 코드
            var strCntrCd = BaseClass.CenterCD;                     // 센터 코드           
            var strEqpId = SelectCellInfo.EQP_ID;
            var strCellId = SelectCellInfo.CELL_ID;
            var strPosX = SelectCellInfo.POS_X;
            var strPosY = SelectCellInfo.POS_Y;
            var strP1AccsYn = cboP1AccsYn.SelectedItem.ToWhiteSpaceOrString();
            var strP2AccsYn = cboP2AccsYn.SelectedItem.ToWhiteSpaceOrString();
            var strCellTypeCd = this.BaseClass.ComboBoxSelectedKeyValue(this.cboCellType);  // 셀타입
            var strMaxCellHgt = SelectCellInfo.MAX_CELL_HGT;
            var strUseYN = cboUseYn.SelectedItem.ToWhiteSpaceOrString();        // 사용 여부
            var strBasicCellHgt = SelectCellInfo.BASIC_CELL_HGT;

            var strErrCode = string.Empty;                         // 오류 코드
            var strErrMsg = string.Empty;                          // 오류 메세지
            #endregion

            #region Input 파라메터     
            dicInputParam.Add("P_CO_CD", strCoCd);          // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);      // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpId);     // 설비ID
            dicInputParam.Add("P_CELL_ID", strCellId);   // 셀ID
            dicInputParam.Add("P_POS_X", strPosX);        // 
            dicInputParam.Add("P_POS_Y", strPosY);        // 
            dicInputParam.Add("P_P1_ACCS_YN", strP1AccsYn);   // 
            dicInputParam.Add("P_P2_ACCS_YN", strP2AccsYn);   // 
            dicInputParam.Add("P_CELL_TYPE_CD", strCellTypeCd); // 
            dicInputParam.Add("P_MAX_CELL_HGT", strMaxCellHgt); // 셀최대높이
            dicInputParam.Add("P_USE_YN", strUseYN);        // 사용 여부
            dicInputParam.Add("P_BASIC_CELL_HGT", strBasicCellHgt); // 셀기본높이

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

        private void BtnFormClose_Click(object sender, RoutedEventArgs e)
        {
            this.CloseProcess();
        }

        /// <summary>
        /// 팝업 닫기 전 확인 메세지
        /// </summary>
        private void CloseProcess()
        {
            this.Close();
        }

        #endregion

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty((sender as TextBlock).Tag.ToString()))
            {
                this.SelectCellId = (sender as TextBlock).Tag.ToString();

                foreach (var Lv1 in CellInfoDatas)
                {
                    foreach (var Lv2Item in Lv1)
                    {
                        if(Lv2Item.CellId.Equals(this.SelectCellId))
                        {
                            Lv2Item.IsSelected = true;
                        }
                        else
                        {
                            Lv2Item.IsSelected = false;
                        }
                    }
                }

                GetCellDetailInfo();
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty((sender as Border).Tag.ToString()))
            {
                this.SelectCellId = (sender as Border).Tag.ToString();

                foreach (var Lv1 in CellInfoDatas)
                {
                    foreach (var Lv2Item in Lv1)
                    {
                        if (Lv2Item.CellId.Equals(this.SelectCellId))
                        {
                            Lv2Item.IsSelected = true;
                        }
                        else
                        {
                            Lv2Item.IsSelected = false;
                        }
                    }
                }

                GetCellDetailInfo();
            }
        }

        private void GetCellDetailInfo()
        {
            if (!string.IsNullOrEmpty(this.SelectCellId))
            {
                GetSP_CELL_MST_SEARCH();
                GetSP_STOCK_SEARCH();
            }
        }

        private void CellDetailClear()
        {
            this.SelectCellInfo = null;
            this.StockInfoList = null;
        }

        // PK_C1022_GAN.SP_CELL_MST_SEARCH			
        private void GetSP_CELL_MST_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_C1022_GAN.SP_CELL_MST_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            var strCoCd = this.BaseClass.CompanyCode;                                               // 회사 코드
            var strCntrCd = this.BaseClass.CenterCD;                                                // 센터 코드
            var strEqpId = this.EqpId;
            var strCellTypeId = string.Empty;
            var strCellId = this.SelectCellId;

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);              // 회사 코드
            dicInputParam.Add("P_CNTR_CD", strCntrCd);          // 센터 코드
            dicInputParam.Add("P_EQP_ID", strEqpId);            // 설비 ID  
            dicInputParam.Add("P_CELL_TYPE_CD", strCellTypeId); // 셀타입 ID 
            dicInputParam.Add("P_CELL_ID", strCellId);          // 셀 ID      
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dtRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            if (dtRtnValue != null)
            {
                // CO_CD
                // CNTR_CD
                // EQP_ID
                // CELL_ID
                // COL_NO
                // ROW_NO
                // POS_X
                // POS_Y
                // P1_ACCS_YN
                // P2_ACCS_YN
                // CELL_TYPE_CD
                // CELL_TYPE_NM
                // MAX_CELL_HGT
                // USE_YN

                this.SelectCellInfo = (ConvertDataTableToList.DataTableToList<CellMstInfo>(dtRtnValue)).FirstOrDefault();
                //txtCellId.Text = dtRtnValue.Rows[0]["CELL_ID"].ToString();
                //txtCellType.Text = dtRtnValue.Rows[0]["CELL_TYPE_NM"].ToString();
                //txtPosX.Text = dtRtnValue.Rows[0]["POS_X"].ToString();
                //txtPosY.Text = dtRtnValue.Rows[0]["POS_Y"].ToString();
                //txtP1AccsYn.Text = dtRtnValue.Rows[0]["P1_ACCS_YN"].ToString();
                //txtP2AccsYn.Text = dtRtnValue.Rows[0]["P2_ACCS_YN"].ToString();
                //txtUseYn.Text = dtRtnValue.Rows[0]["USE_YN"].ToString();
                //txtMaxCellHgt.Text = dtRtnValue.Rows[0]["MAX_CELL_HGT"].ToString();
            }
            else
            {
                this.SelectCellInfo = null;

                //txtCellId.Text = string.Empty;
                //txtCellType.Text = string.Empty;
                //txtPosX.Text = string.Empty;
                //txtPosY.Text = string.Empty;
                //txtP1AccsYn.Text = string.Empty;
                //txtP2AccsYn.Text = string.Empty;
                //txtUseYn.Text = string.Empty;
                //txtMaxCellHgt.Text = string.Empty;
            }
        }

        //PK_R1008_GAN.SP_STOCK_SEARCH
        private void GetSP_STOCK_SEARCH()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                #region 파라메터 변수 선언 및 값 할당
                DataSet dsRtnValue = null;
                var strProcedureName = "PK_R1008_GAN.SP_STOCK_SEARCH";
                Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
                string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

                var strCoCd = this.BaseClass.CompanyCode;       // 회사 코드
                var strCntrCd = this.BaseClass.CenterCD;        // 센터 코드
                var strEqpId = this.EqpId;                      // 설비ID
                var strCellId = this.SelectCellId;              // 셀ID
                var strCellTypeCd = string.Empty;               // 셀타입 코드

                var strErrCode = string.Empty;          // 오류 코드
                var strErrMsg = string.Empty;           // 오류 메세지
                #endregion

                #region Input 파라메터
                dicInputParam.Add("P_CO_CD", strCoCd);              // 회사 코드
                dicInputParam.Add("P_CNTR_CD", strCntrCd);          // 센터 코드
                dicInputParam.Add("P_EQP_ID", strEqpId);            // 설비 ID   
                dicInputParam.Add("P_CELL_ID", strCellId);          // 셀 ID 
                dicInputParam.Add("P_CELL_TYPE_CD", strCellTypeCd);     // 셀타입 코드 
                #endregion

                #region 데이터 조회
                using (BaseDataAccess dataAccess = new BaseDataAccess())
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }
                #endregion

                if (dsRtnValue != null)
                {
                    // 정상 처리된 경우
                    this.StockInfoList = new ObservableCollection<StockInfo>();
                    
                    // 오라클인 경우 TableName = TB_COM_MENU_MST
                    this.StockInfoList.ToObservableCollection(dsRtnValue.Tables[0]);
                }

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }

        private void tvMasterGrid_CellMerge (object sender, CellMergeEventArgs e)
        {
            try
            {
                List<string> _Fields = new List<string> { "STK_HGT" };

                if (_Fields.Contains(e.Column.FieldName))
                {
                    var _row1 = this.gridMaster.GetRow(e.RowHandle1);
                    var _row2 = this.gridMaster.GetRow(e.RowHandle2);

                    if (_row1 != null && _row2 != null)
                    {
                        if (_row1 is StockInfo && _row2 is StockInfo)
                        {
                            e.Merge = (_row1 as StockInfo).GroupEquals(_row2 as StockInfo);
                            e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
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
