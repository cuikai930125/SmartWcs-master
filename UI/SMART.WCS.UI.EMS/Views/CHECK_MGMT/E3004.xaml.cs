using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Scheduling;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.ECHK004;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMART.WCS.UI.EMS.Views.CHECK_MGMT
{
    /// <summary>
    /// E3004.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class E3004 : UserControl
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
        /// 공통 Class를 이용하기 위한 BaseClass 선언
        /// </summary>
        private BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 전체권한 여부 (true:전체권한)
        /// </summary>
        private static bool g_IsAuthAllYN = false;
        #endregion

        #region ▩ 생성자
        public E3004()
        {
            DevExpress.Xpf.Scheduling.SchedulerControl.AllowInfiniteSize = true;

            InitializeComponent();
        }

        public E3004(List<string> _liMenuNavigation)
        {
            try
            {
                DevExpress.Xpf.Scheduling.SchedulerControl.AllowInfiniteSize = true;

                InitializeComponent();

                // 즐겨찾기 변경 여부를 가져오기 위한 이벤트 선언 (Delegate)
                this.NavigationBar.UserControlCallEvent += NavigationBar_UserControlCallEvent;

                // 네비게이션 메뉴 바인딩
                this.NavigationBar.ItemsSource = _liMenuNavigation;
                this.NavigationBar.MenuID = MethodBase.GetCurrentMethod().DeclaringType.Name; // 클래스 (파일명)

                // 화면 전체권한 여부
                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                emsScheduler.Start = DateTime.Now;
                SetSelectedDate(DateTime.Now);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의
        /// <summary>
        /// 알람현황
        /// </summary>
        public static readonly DependencyProperty CheckPointAlarmStatusListProperty
                                  = DependencyProperty.Register("CheckPointAlarmStatusList",
                                      typeof(ObservableCollection<EmsAppointment>),
                                             typeof(E3004), new PropertyMetadata(new ObservableCollection<EmsAppointment>()));
        public ObservableCollection<EmsAppointment> CheckPointAlarmStatusList
        {
            get { return (ObservableCollection<EmsAppointment>)GetValue(CheckPointAlarmStatusListProperty); }
            private set { SetValue(CheckPointAlarmStatusListProperty, value); }
        }
        #endregion

        #region ▩ 함수
        private void SetSelectedDate(DateTime baseDate)
        {
            List<DateTime> _defaultDate = new List<DateTime>();

            for (int index = 14; index > 0; index--)
            {
                _defaultDate.Add(baseDate.AddDays(-index));
            }
            for (int index = 0; index < 14; index++)
            {
                _defaultDate.Add(baseDate.AddDays(index));
            }
            datenavigator.SelectedDates = _defaultDate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TypeId">점검계획, 점검실적, 장애</param>
        /// <param name="IdNo">점검번호 , 장애 번호</param>
        private void SetOthersForm(AppointmentItem a, int IdNo)
        {
            //MessageBox.Show("link..");

            //MainWinParam objParam = new MainWinParam();
            //  objParam.BTCH_NO = ((SMS.DataMembers.SPCS201.MasterGrid)rowData).BTCH_NO;            // 배치번호
            //  objParam.EQP_ID = this.cboEQP.GetKeyValue(this.cboEQP.SelectedIndex).ToString();    // 설비 ID
            // objParam.WRK_STRT_DT = ((SMS.DataMembers.SPCS201.MasterGrid)rowData).WRK_YMD;            // 작업일자
            // objParam.CST_CD = ((SMS.DataMembers.SPCS201.MasterGrid)rowData).CST_CD;             // 고객사 코드
            // objParam.CST_NM = ((SMS.DataMembers.SPCS201.MasterGrid)rowData).CST_NM;             // 고객사 명
            //objParam.BTCH_SEQ = ((SMS.DataMembers.SPCS201.MasterGrid)rowData).BTCH_SEQ;           // 배치순번


            int typeId = int.Parse(a.LabelId.ToString());
            int Chk_no = int.Parse(a.TimeZoneId.ToString());



            if (typeId == 1)
            {
                //MessageBox.Show("장애" + Chk_no.ToString());
                //objParam.MENU_ID = "ECHK002_01P";                                                          // 메뉴 ID

                //ECHK002_01P frmEchkErrReg = new ECHK002_01P(-1);
                //frmEchkErrReg.ShowDialog();

                //EmsSession.Instance.MainForm.OpenMenuFrom("EMS0302", "ECHK002_01P", Chk_no.ToString());
                using (E3002_01P frm = new E3002_01P(Chk_no, ("2" == a.StatusId.ToString()) ? "CONF" : string.Empty))
                {
                    frm.ShowDialog();
                }
                
            }
            else if (typeId == 2)
            {
                //MessageBox.Show("계획" + Chk_no.ToString());
                //objParam.MENU_ID = "ECHK001_01P";                                                          // 메뉴 ID

                //ECHK001_01P frmChkReg = new ECHK001_01P(-1);
                //frmChkReg.ShowDialog();

                //EmsSession.Instance.MainForm.OpenMenuFrom("EMS0301", "ECHK001_04P", Chk_no.ToString());
                //ECHK001_04P frmRstReg = new ECHK001_04P(Chk_no, ("2" == a.StatusId.ToString()) ? "F" : "");

                using (E3001_04P frm = new E3001_04P(Chk_no, ("2" == a.StatusId.ToString()) ? "CONF" : string.Empty))
                {
                    frm.ShowDialog();
                }

            }
            else if (typeId == 3)
            {
                //MessageBox.Show("실적" + Chk_no.ToString());
                //objParam.MENU_ID = "ECHK001_04P";                                                          // 메뉴 ID

                //ECHK001_04P frmRstReg = new ECHK001_04P(-1);
                //frmRstReg.ShowDialog();

                //EmsSession.Instance.MainForm.OpenMenuFrom("EMS0301", "ECHK001_04P", Chk_no.ToString());
                //ECHK001_04P frmRstReg = new ECHK001_04P(Chk_no, ("2" == a.StatusId.ToString()) ? "F" : "");
                using (E3001_04P frm = new E3001_04P(Chk_no, ("2" == a.StatusId.ToString()) ? "CONF" : string.Empty))
                {
                    frm.ShowDialog();
                }
            }

            //this.SelectedMenuOpenEvent(objParam);
        }

        #region > 데이터 관련
        private void SearchEmsAlarmStatus()
        {
            // 조회 기간 

            //EmsCommCode curComm = cboQRY_DAYS.SelectedItem as EmsCommCode;
            //int qryDays = int.Parse(curComm.COM_DETAIL_CD);
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD}
                    ,{"P_YYYYMM", "201810"}     // 사용하지 않는 parameter 
                };

                var strOutParam = new[] { "P_EMS_STATUS_LIST" };
                string callProc = "PK_EMS_ECHK004.SP_EMS_MONTH_STATUS_SEL";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(callProc                      // 호출 프로시저
                            , param                        // Input 파라메터
                            , strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 1)
                    {
                        CheckPointAlarmStatusList = new ObservableCollection<EmsAppointment>();
                        CheckPointAlarmStatusList.ToObservableCollection(outData.Tables[0]);
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
        #endregion

        #region ▩ 이벤트
        #region > 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// <summary>
        /// 메인 화면 메뉴 (트리 리스트 컨트롤) 재조회를 위한 이벤트 (Delegate)
        /// </summary>
        private void NavigationBar_UserControlCallEvent()
        {
            try
            {
                this.TreeControlRefreshEvent();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #endregion

        private void btnSearchClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.SearchEmsAlarmStatus();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void emsScheduler_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (emsScheduler.SelectedAppointments == null || emsScheduler.SelectedAppointments.Count == 0)
                {
                    return;
                }

                AppointmentItem a = emsScheduler.SelectedAppointments[0];

                SetOthersForm(a, int.Parse(a.TimeZoneId));
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
    }
}
