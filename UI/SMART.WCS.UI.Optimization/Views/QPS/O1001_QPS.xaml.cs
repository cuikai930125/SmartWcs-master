using DevExpress.Xpf.Editors;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART.WCS.UI.Optimization.Views.QPS
{
    /// <summary>
    /// 최적화 기준 옵션 셋팅
    /// </summary>
    public partial class O1001_QPS : UserControl
    {
        #region ▩ Detegate 선언
        #region > 메인화면 하단 좌측 상태바 값 반영
        public delegate void ToolStripStatusEventHandler(string value);
        public event ToolStripStatusEventHandler ToolStripChangeStatusLabelEvent;
        #endregion

        #region > 즐겨찾기 변경후 메인화면 트리 컨트롤 Refresh 및 포커스 이동
        public delegate void TreeControlRefreshEventHandler();
        public event TreeControlRefreshEventHandler TreeControlRefreshEvent;
        #endregion
        #endregion

        #region ▩ 전역변수
        /// <summary>
        /// Base 클래서 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public O1001_QPS()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_liMenuNavigation">화면에 표현할 Navigator 정보</param>
        public O1001_QPS(List<string> _liMenuNavigation)
        {
            try
            {
                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource  = _liMenuNavigation;
                this.NavigationBar.MenuID       = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // 컨트롤 관련 초기화
                this.InitControl();

                // 공통코드를 사용하지 않는 콤보박스를 설정한다.
                this.InitComboBoxInfo();

                // 이벤트 초기화
                this.InitEvent();
                
                // 초기값은 옵션 기준이 표준일 때 데이터 확인
                this.BtnSearch_PreviewMouseLeftButtonUp(null, null);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 함수
        #region > 초기화
        #region >> InitControl - 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// <summary>
        /// 폼 로드시에 각 컨트롤의 속성을 초기화 및 정의한다.
        /// </summary>
        private void InitControl()
        {
            // 달력 컨트롤 기본값 설정
            this.deWrkPlanWmd.DateTime = DateTime.Now;

            #region + 콤보박스 설정 
            // Loc CD
            // 공통코드
            // STANDARD    표준
            // DATASET     DATASET
            this.BaseClass.BindingCommonComboBox(this.cboOptCD, "DATA_SET_SPR_CD", null, false);
            
            // Location Balance 
            // 공통코드
            // IGNORE_LOC	SKU별 구역무시
            // USE_LOC      SKU별 구역고려
            this.BaseClass.BindingCommonComboBox(this.cboLocBal, "LOC_BAL", null, false);
            
            // Sorting Mode
            // 공통코드
            // RANDOM       무작위 SKU순 정렬
            // ROC          ROC 로직 수행
            // SKU_ORD_CNT  연결 오더수 SKU순 정렬
            this.BaseClass.BindingCommonComboBox(this.cboSortMode, "SORT_MODE", null, false);

            // 구역간 배치 정보
            // 공통코드
            // USE_LOC_BTCH     구역간 배치정보 사용
            // IGNORE_LOC_BTCH  구역간 배치정보 사용??
            this.BaseClass.BindingCommonComboBox(this.cboLocBtchInfo, "LOC_BTCH_INFO", null, false);

            // SKU 빈도정보 사용
            // 공통코드
            // USE_SKU_FREQ     구역간 배치정보 사용.....
            // IGNORE_SKU_FREQ  구역간 배치정보 사용.....
            this.BaseClass.BindingCommonComboBox(this.cboSkuCelType, "SKU_CEL_TYPE", null, false);
            #endregion
        }
        #endregion

        #region >> InitComboBoxInfo - 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// <summary>
        /// 콤보박스 초기화 - 공통코드를 사용하지 않는 콤보박스를 설정한다.
        /// </summary>
        private void InitComboBoxInfo()
        {
            #region ++ 공통코드 사용하지 않는 콤보박스 설정
            this.cboOptCD.SelectedIndex = 0;
            this.cboOptCD.SelectedIndexChanged += CboOptCD_SelectedIndexChanged;
            CboOptCD_SelectedIndexChanged(null, null);
            #endregion
        }

        private void CboDataSetOpt_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region >> InitEvent - 이벤트 초기화
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            #region + 달력 컨트롤 변경 이벤트
            // 달력 컨트롤 선택 변경
            this.deWrkPlanWmd.EditValueChanged += DeWrkPlanWmd_EditValueChanged;
            #endregion
            
            #region + 버튼 이벤트
            // 조회 버튼 클릭 이벤트
            this.btnSearch.PreviewMouseLeftButtonUp += BtnSearch_PreviewMouseLeftButtonUp;
            // 저장 버튼 클릭 이벤트
            this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;
            #endregion
            
        }
        #endregion
        #endregion

        #region > 데이터 관련
        #region >> GetSP_OPT_QPS_ROC_SET_INQ - ROC 정보 조회 
        /// <summary>
        /// ROC 정보 조회
        /// </summary>
        /// <returns></returns>
        private async Task<DataSet> GetSP_QPS_ROC_SET_INQ(string OptTypeCD)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dtRtnValue = null;
            var strProcedureName = "PK_O1001_QPS.SP_QPS_ROC_SET_INQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_QPS_ROC_SET_LIST", "O_RTN_RSLT" };
            
            var strCenterCD = this.BaseClass.CenterCD;                                      // 센터코드
            var strOptTypeCD = OptTypeCD;                                                   // 최적화 타입
            var strWrkPlanYmd = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);         // 출고일자
            var strDataSetId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);  // DataSetID
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CNTR_CD", strCenterCD);                                    // 센터코드
            dicInputParam.Add("P_OPT_TYPE_CD", strOptTypeCD);                               // 최적화 타입
            dicInputParam.Add("P_WRK_PLAN_YMD", strWrkPlanYmd);                             // 출고일자
            dicInputParam.Add("P_DATA_SET_ID", strDataSetId);                                // DataSetID
            #endregion

            #region 데이터 조회
            using (BaseDataAccess da = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dtRtnValue = da.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }
            #endregion
            
            return dtRtnValue;
        }
        #endregion
        
        private async Task<DataSet> SaveSP_QPS_ROC_SET_SAVE(BaseDataAccess _da, Dictionary<string, object> _dicInputParam)
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_O1001_QPS.SP_QPS_ROC_SET_SAVE";
            string[] arrOutputParam = { "O_RTN_RSLT" };
            #endregion

            await System.Threading.Tasks.Task.Run(() =>
            {
                dsRtnValue = _da.GetSpDataSet(strProcedureName, _dicInputParam, arrOutputParam);
            }).ConfigureAwait(true);

            return dsRtnValue;
        }
        #endregion

        #region > 입력값 초기화
        public void Init_InputData()
        {
            this.txtBtchDivLim.Text = String.Empty;
            this.txtCellOptSeq.Text = String.Empty;
            this.txtMinSupSkuUnit.Text = String.Empty;
            this.txtRocOptSeq.Text = String.Empty;
            this.txtSupBtchDivCnt.Text = String.Empty;
            this.cboLocBal.Clear();
            this.cboLocBtchInfo.Clear();
            this.cboSkuCelType.Clear();
            this.cboSortMode.Clear();
        }
        #endregion

        #region > 데이터 유효성 체크
        public string DataValidationCheck()
        {
            if (txtBtchDivLim.Text.Equals(String.Empty))
            {
                txtBtchDivLim.Focus();
                return "BTCH_DIV_LIM";
            }

            if(cboLocBal.SelectedIndex < 0)
            {
                cboLocBal.Focus();
                return "LOC_BAL";
            }

            if(cboSortMode.SelectedIndex < 0)
            {
                cboSortMode.Focus();
                return "SORT_MODE";
            }

            if (txtSupBtchDivCnt.Text.Equals(String.Empty))
            {
                txtSupBtchDivCnt.Focus();
                return "SUP_BTCH_DIV_CNT";
            }

            if (txtMinSupSkuUnit.Text.Equals(String.Empty))
            {
                txtMinSupSkuUnit.Focus();
                return "MIN_SUP_SKU_UNIT";
            }

            if(cboLocBtchInfo.SelectedIndex < 0)
            {
                cboLocBtchInfo.Focus();
                return "LOC_BTCH_INFO";
            }

            if(cboSkuCelType.SelectedIndex < 0)
            {
                cboSkuCelType.Focus();
                return "SKU_CELL_TYPE";
            }

            return null;
        }
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
                //컨트롤에 입력된 값들 초기화
                Init_InputData();
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;
                DataSet Roc_dsRtnValue = null;
                DataSet Cell_dsRtnValue = null;
                
                Roc_dsRtnValue = await this.GetSP_QPS_ROC_SET_INQ("ROC");
                Cell_dsRtnValue = await this.GetSP_QPS_ROC_SET_INQ("CEL");

                if (Roc_dsRtnValue == null || Cell_dsRtnValue == null) { return; }
                var Roc_strErrCode = string.Empty;      // ROC 오류 코드
                var Roc_strErrMsg = string.Empty;       // ROC 오류 메세지
                var Cell_strErrCode = string.Empty;     // CELL 오류 코드
                var Cell_strErrMsg = string.Empty;      // CELL 오류 메세지
                //this.ToolStripChangeStatusLabelEvent($"{iTotalRowCount.ToString()}{strResource}");
                this.ToolStripChangeStatusLabelEvent(string.Empty);

                if (this.BaseClass.CheckResultDataProcess(Roc_dsRtnValue, ref Roc_strErrCode, ref Roc_strErrMsg) == true &&
                    this.BaseClass.CheckResultDataProcess(Cell_dsRtnValue, ref Cell_strErrCode, ref Cell_strErrMsg) == true)
                {
                    if(Roc_dsRtnValue.Tables[0].Rows.Count == 0)
                    {
                        this.BaseClass.MsgInfo("INFO_NOT_INQ");
                        return;
                    }
                    // 정상처리된 경우 가져온 DataTable에서 각 행별로 데이터 처리
                    foreach (DataRow dr in Roc_dsRtnValue.Tables[0].Rows)
                    {
                        if (dr["SET_CLAS_CD"].ToString().Equals("BTCH_DIV_LIM"))                     //배치분배차수인 경우
                        {
                            txtBtchDivLim.Text = dr["SET_CLAS_DTL_CD"].ToString();                   //텍스트 박스에 값 입력
                        }
                        else if (dr["SET_CLAS_CD"].ToString().Equals("LOC_BAL"))                     //Location Balance인 경우
                        {  
                            List<ComboBoxInfo> list = (List<ComboBoxInfo>)cboLocBal.ItemsSource;     //콤보박스 아이템 리스트를 가져와서
                            foreach(var cbo in list)
                            {
                                if (cbo.CODE.Equals(dr["SET_CLAS_DTL_CD"].ToString()))               //해당 아이템과 일치하는 아이템으로 선택
                                {
                                    cboLocBal.SelectedItem = cbo;
                                }
                            }
                        }
                        else                                                                         //Sorting Mode인 경우
                        {
                            List<ComboBoxInfo> list = (List<ComboBoxInfo>)cboSortMode.ItemsSource;   //콤보박스 아이템 리스트를 가져와서
                            foreach (var cbo in list)
                            {
                                if (cbo.CODE.Equals(dr["SET_CLAS_DTL_CD"].ToString()))               //해당 아이템과 일치하는 아이템으로 선택
                                {
                                    cboSortMode.SelectedItem = cbo;
                                }
                            }
                            txtRocOptSeq.Text = dr["OPT_SEQ"].ToString();
                        }
                    }

                    foreach(DataRow dr in Cell_dsRtnValue.Tables[0].Rows)
                    {
                        if (dr["SET_CLAS_CD"].ToString().Equals("SUP_BTCH_DIV_CNT"))                 //보충 배치 분할 갯수인 경우
                        {
                            txtSupBtchDivCnt.Text = dr["SET_CLAS_DTL_CD"].ToString();                //텍스트 박스에 값 입력
                        }
                        else if (dr["SET_CLAS_CD"].ToString().Equals("MIN_SUP_SKU_UNIT"))            //보충SKU최소단위인 경우
                        {
                            txtMinSupSkuUnit.Text = dr["SET_CLAS_DTL_CD"].ToString();                //텍스트 박스에 값 입력
                        }
                        else if (dr["SET_CLAS_CD"].ToString().Equals("LOC_BTCH_INFO"))               //구역간 배치 정보인 경우
                        {
                            List<ComboBoxInfo> list = (List<ComboBoxInfo>)cboLocBtchInfo.ItemsSource;//콤보박스 아이템 리스트를 가져와서
                            foreach (var cbo in list)
                            {
                                if (cbo.CODE.Equals(dr["SET_CLAS_DTL_CD"].ToString()))               //해당 아이템과 일치하는 아이템으로 선택
                                {
                                    cboLocBtchInfo.SelectedItem = cbo;
                                }
                            }
                        }
                        else                                                                         //SKU별 셀 유형인 경우
                        {
                            List<ComboBoxInfo> list = (List<ComboBoxInfo>)cboSkuCelType.ItemsSource; //콤보박스 아이템 리스트를 가져와서
                            foreach (var cbo in list)
                            {
                                if (cbo.CODE.Equals(dr["SET_CLAS_DTL_CD"].ToString()))               //해당 아이템과 일치하는 아이템으로 선택
                                {
                                    cboSkuCelType.SelectedItem = cbo;
                                }
                            }
                            txtCellOptSeq.Text = dr["OPT_SEQ"].ToString();
                        }
                    }
                }
                else
                {
                    // 오류 발생한 경우 - 조회 중 에러가 발생하였습니다.
                    this.BaseClass.MsgError("ERR_INQ");
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion
        
        #region >> 저장 버튼 클릭 이벤트
        /// <summary>
        /// 저장 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataSet dsRocRtnValue = null;
            DataSet dsCellRtnValue = null;

            //ROC, QPS CELL 옵션 셋팅 입력값 유효성 검사
            string dataValidationCheck = DataValidationCheck();

            if (dataValidationCheck != null)
            {
                this.BaseClass.MsgError("ERR_NOT_INPUT_2", dataValidationCheck);
                return;
            }

            //저장하시겠습니까? Y/N
            this.BaseClass.MsgQuestion("ASK_SAVE");
            //확인버튼 누른 경우
            if (this.BaseClass.BUTTON_CONFIRM_YN == true)
            {
                using (BaseDataAccess da = new BaseDataAccess())
                {
                    #region + 파라미터 및 변수 선언, Input 파라미터 저장
                    Dictionary<string, object> _dicInputParam = new Dictionary<string, object>();
                    Dictionary<string, string> dicRoc = new Dictionary<string, string>();                                                     // ROC Option Setting
                    Dictionary<string, string> dicCell = new Dictionary<string, string>();                                                    // QPS Option Setting
                    var strCenterCD = this.BaseClass.CenterCD;                                                                                // 센터코드
                    var strRocCD = "ROC";                                                                                                     // ROC타입 코드
                    var strCellCD = "CEL";                                                                                                    // CELL타입 코드
                    var numRocOptSeq = int.Parse(string.IsNullOrEmpty(txtRocOptSeq.Text.ToString()) ? "0" : txtRocOptSeq.Text.ToString());    // ROC타입 옵션번호
                    var numCellOptSeq = int.Parse(string.IsNullOrEmpty(txtCellOptSeq.Text.ToString()) ? "0" : txtCellOptSeq.Text.ToString()); // CELL타입 옵션번호
                    var strWrkPlanYmd = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);                                                   // 출고일자
                    var strDataSetId = this.BaseClass.ComboBoxSelectedKeyValue(this.cboDataSetID);                                            // DataSetID
                    var strUserID = this.BaseClass.UserID;                                                                                    // 사용자 ID

                    //ROC Option Setting, QPS Cell Option Setting의 키들과 값들을 저장
                    dicRoc.Add("BTCH_DIV_LIM", txtBtchDivLim.Text.ToString());
                    dicRoc.Add("LOC_BAL", ((ComboBoxInfo)cboLocBal.SelectedItem).CODE.ToString());
                    dicRoc.Add("SORT_MODE", ((ComboBoxInfo)cboSortMode.SelectedItem).CODE.ToString());
                    dicCell.Add("SUP_BTCH_DIV_CNT", txtSupBtchDivCnt.Text.ToString());
                    dicCell.Add("MIN_SUP_SKU_UNIT", txtMinSupSkuUnit.Text.ToString());
                    dicCell.Add("LOC_BTCH_INFO", ((ComboBoxInfo)cboLocBtchInfo.SelectedItem).CODE.ToString());
                    dicCell.Add("SKU_CEL_TYPE", ((ComboBoxInfo)cboSkuCelType.SelectedItem).CODE.ToString());
                    //Input 파라미터 저장
                    _dicInputParam.Add("P_CNTR_CD", strCenterCD);
                    _dicInputParam.Add("P_DATA_SET_ID", strDataSetId);
                    _dicInputParam.Add("P_USER_ID", strUserID);
                    #endregion

                    try
                    {
                        //상태바(아이콘 실행)
                        this.loadingScreen.IsSplashScreenShown = true;

                        //ROC Option Setting에 대한 데이터 저장
                        _dicInputParam.Add("P_OPT_TYPE_CD", strRocCD);
                        _dicInputParam.Add("P_OPT_SEQ", numRocOptSeq);
                        foreach (var Roc in dicRoc)
                        {//각 파라미터를 변경하며 데이터 저장
                            _dicInputParam.Add("P_SET_CLAS_CD", Roc.Key.ToString());
                            _dicInputParam.Add("P_SET_CLAS_DTL_CD", Roc.Value.ToString());

                            da.BeginTransaction();
                            dsRocRtnValue = await this.SaveSP_QPS_ROC_SET_SAVE(da, _dicInputParam);

                            _dicInputParam.Remove("P_SET_CLAS_CD");
                            _dicInputParam.Remove("P_SET_CLAS_DTL_CD");

                            if (dsRocRtnValue != null)
                            {
                                if (dsRocRtnValue.Tables[0].Rows.Count > 0)
                                {//결과가 이상없을 때
                                    if (dsRocRtnValue.Tables[0].Rows[0]["CODE"].ToString().Equals("0") == false)
                                    {
                                        this.BaseClass.MsgError(dsRocRtnValue.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                                        return;
                                    }
                                    da.CommitTransaction();
                                }
                                else
                                {//결과가 이상있을 때
                                    da.RollbackTransaction();
                                    this.BaseClass.MsgError("ERR_SAVE");
                                    return;
                                }
                            }
                        }

                        //QPS Cell Option Setting 값 저장
                        _dicInputParam.Remove("P_OPT_TYPE_CD");
                        _dicInputParam.Remove("P_OPT_SEQ");
                        _dicInputParam.Add("P_OPT_TYPE_CD", strCellCD);
                        _dicInputParam.Add("P_OPT_SEQ", numCellOptSeq);
                        foreach (var Cell in dicCell)
                        {
                            _dicInputParam.Add("P_SET_CLAS_CD", Cell.Key.ToString());
                            _dicInputParam.Add("P_SET_CLAS_DTL_CD", Cell.Value.ToString());

                            da.BeginTransaction();
                            dsCellRtnValue = await this.SaveSP_QPS_ROC_SET_SAVE(da, _dicInputParam);

                            _dicInputParam.Remove("P_SET_CLAS_CD");
                            _dicInputParam.Remove("P_SET_CLAS_DTL_CD");

                            if (dsCellRtnValue != null)
                            {
                                if (dsCellRtnValue.Tables[0].Rows.Count > 0)
                                {//결과가 이상없을 때
                                    if (dsCellRtnValue.Tables[0].Rows[0]["CODE"].ToString().Equals("0") == false)
                                    {
                                        this.BaseClass.MsgError(dsCellRtnValue.Tables[0].Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                                        return;
                                    }
                                    da.CommitTransaction();
                                }
                                else
                                {//결과가 이상있을 때
                                    da.RollbackTransaction();
                                    this.BaseClass.MsgError("ERR_SAVE");
                                    return;
                                }
                            }
                        }

                        this.BaseClass.MsgInfo("CMPT_SAVE");
                    }
                    catch
                    {
                        //트랜잭션 실행중 에러가 발생한 경우 트랜잭션 롤백
                        if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                        {
                            da.RollbackTransaction();
                        }
                        this.BaseClass.MsgError("ERR_SAVE");
                        throw;
                    }
                    finally
                    {
                        //상태바(아이콘)제거
                        this.loadingScreen.IsSplashScreenShown = false;
                    }
                }
            }
        }
        #endregion
        #endregion

        #region > 달력 컨트롤 이벤트
        #region >> 달력 컨트롤 변경 이벤트
        /// <summary>
        /// 달력 컨트롤 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeWrkPlanWmd_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            var strWrkPlanYmd = this.BaseClass.GetCalendarValue(this.deWrkPlanWmd);   // 출고일자
            DataTable dtComboData = await Utility.HelperClass.GetSP_OPT_QPS_DATA_SET_LIST_INQ(strWrkPlanYmd);

            // 조회 데이터가 없는 경우 구문을 리턴한다.
            if (dtComboData.Rows.Count == 0)
            {
                this.cboDataSetID.ItemsSource = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);
                return;
            }

            // 콤보박스 설정을 위해 컬럼명을 변경한다.
            dtComboData = this.BaseClass.ModifyComboBoxColumnHeaderName(dtComboData);

            // DATA 그룹 데이터가 1개 컬럼으로 조회되기 때문에 콤보박스 설정을 위해 컬럼을 추가 생성한 후 데이터를 복사한다.
            dtComboData.Columns.Add("NAME", typeof(string));
            for (int i = 0; i < dtComboData.Rows.Count; i++)
            {
                dtComboData.Rows[i]["NAME"] = dtComboData.Rows[i]["CODE"].ToString();
            }

            this.cboDataSetID.ItemsSource = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);
        }
        #endregion
        #endregion
        
        #region > 콤보박스 이벤트
        /// <summary>
        /// Data Set 콤보박스 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CboOptCD_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            Init_InputData();
            if (cboOptCD.SelectedIndex == 0)
            {   //콤보박스 선택이 표준일 경우 날짜와 DataSetID는 비활성화, 저장버튼 활성화, DataSetID STANDARD로 고정
                DataTable dtComboData = new DataTable();
                dtComboData.Columns.Add("CODE", typeof(string));
                dtComboData.Columns.Add("NAME", typeof(string));
                DataRow drComboData = dtComboData.NewRow();

                drComboData["CODE"] = "STANDARD";
                drComboData["NAME"] = "STANDARD";

                dtComboData.Rows.Add(drComboData);
                this.cboDataSetID.ItemsSource = ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboData);
                this.cboDataSetID.SelectedIndex = 0;

                this.deWrkPlanWmd.IsEnabled = false;
                this.cboDataSetID.IsEnabled = false;
                this.btnSave.IsEnabled = true;
                this.txtBtchDivLim.IsEnabled = true;
                this.cboLocBal.IsEnabled = true;
                this.cboSortMode.IsEnabled = true;
                this.txtSupBtchDivCnt.IsEnabled = true;
                this.txtMinSupSkuUnit.IsEnabled = true;
                this.cboLocBtchInfo.IsEnabled = true;
                this.cboSkuCelType.IsEnabled = true;

            }
            else
            {   //콤보박스 선택이 표준이 아닐 경우 날짜 선택/DataSetID콤보박스 활성화, 저장버튼 비활성화
                //날짜 변경 시 이벤트 추가 및 실행
                this.deWrkPlanWmd.IsEnabled = true;
                this.cboDataSetID.IsEnabled = true;
                this.btnSave.IsEnabled = false;
                deWrkPlanWmd.EditValueChanged += DeWrkPlanWmd_EditValueChanged;
                DeWrkPlanWmd_EditValueChanged(null, null);
                
                this.txtBtchDivLim.IsEnabled = false;
                this.cboLocBal.IsEnabled = false;
                this.cboSortMode.IsEnabled = false;
                this.txtSupBtchDivCnt.IsEnabled = false;
                this.txtMinSupSkuUnit.IsEnabled = false;
                this.cboLocBtchInfo.IsEnabled = false;
                this.cboSkuCelType.IsEnabled = false;
            }
            
        }
        #endregion

        #region > 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// <summary>
        /// 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// </summary>
        private void NavigationBar_UserControlCallEvent()
        {
            this.TreeControlRefreshEvent();
        }
        #endregion
        #endregion
    }
}