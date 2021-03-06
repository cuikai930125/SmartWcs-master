﻿using DevExpress.Mvvm.Native;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SMART.WCS.UI.EMS.Views.COM
{
    /// <summary>
    /// ECOM001_11P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ECOM001_11P : Window, IDisposable
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
        public ECOM001_11P()
        {
            InitializeComponent();
        }

        public ECOM001_11P(string _strTarget, string _strName)
        {
            try
            {
                InitializeComponent();

                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                // CHOO
                // 2020-03-15 오류 때문에 주석처리
                // 공통코드 정의한 후에 주석해제
                //GetComDev();

                btnFormClose.Click += btnFormClose_Click;

                gridMainView.ShowingEditor += (s, e) =>
                {
                    e.Cancel = true;
                };

                CurrPart = _strTarget;

                tbxPartId.Text = _strTarget;
                tbxPartNm.Text = _strName;

                this.Loaded += ECOM001_11P_Loaded;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 속성
        private string CurrPart { get; set; }

        /// <summary>
        /// Grid Data
        /// </summary>
        private ObservableCollection<EmsPartRepairHis> CurrList { get; set; }

        /// <summary>
        /// 구분
        /// </summary>
        public ObservableCollection<EmsCommCode> ChkDevList { get; private set; }
        #endregion

        #region ▩ 함수
        #region 공통코드 조회
        void GetComDev()
        {
            try
            {
                var _USER_ID = this.BaseClass.UserID;

                var paramDpt = new Dictionary<string, object>
                {
                    {"P_USER_ID", _USER_ID},
                    {"P_COM_HEAD_CD", "EMS_RST_FALL"}
                };

                var strOutParamDpt = new[] { "P_COM_DETAIL_CD_LIST", "P_RESULT" };
                string callProc = "PK_COMM_CODE_MGT.SP_COMM_CODE_DETAIL_SEARCH";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outDataDpt = da.GetSpDataSet(   
                                callProc                        // 호출 프로시저
                            ,   paramDpt                        // Input 파라메터
                            ,   strOutParamDpt                  // Output 파라메터
                    );

                    // 리턴 테이블 체크 
                    if (outDataDpt.Tables.Count > 1)
                    {
                        ChkDevList = new ObservableCollection<EmsCommCode>();
                        ChkDevList.ToObservableCollection(outDataDpt.Tables[0]);

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 공통코드 조회

        #region 조회
        /// <summary>
        /// 조회
        /// </summary>
        void SearchPbs()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_PART_ID", CurrPart}
                };

                var strOutParam = new[] { "P_EMS_CLS_LIST" };
                string callProc = "PK_EMS_ECOM01_11P.SP_EMS_PART_REPAIR_HIS_SEARCH";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            ,   param                        // Input 파라메터
                            ,   strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        CurrList = new ObservableCollection<EmsPartRepairHis>();
                        CurrList.ToObservableCollection(outData.Tables[0]);

                        gridMain.ItemsSource = CurrList;

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }
                }
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }
        #endregion 조회
        #endregion

        #region ▩ 이벤트
        /// <summary>
        /// 화면 오픈시 선행 작업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ECOM001_11P_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Loaded -= ECOM001_11P_Loaded;

                SearchPbs();
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
        // ~ECOM001_11P()
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
