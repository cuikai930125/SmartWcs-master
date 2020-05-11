using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Grid;

using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.EMS.DataMembers.COM;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SMART.WCS.UI.EMS.Views.COM
{
    /// <summary>
    /// ECOM001_05P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ECOM001_05P : Window, IDisposable
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
        public ECOM001_05P()
        {
            InitializeComponent();

            btnFormClose.Click += btnFormClose_Click;

            this.SP_EMS_PART_SEARCH();
        }

        public ECOM001_05P(bool isMulti = false)
        {
            try
            {
                InitializeComponent();

                g_IsAuthAllYN = this.BaseClass.RoleCode.Trim().Equals("A") == true ? true : false;

                if (isMulti)
                {
                    btnApply.Visibility = Visibility.Visible;
                }

                btnFormClose.Click += btnFormClose_Click;

                // 분류 행 색상
                //
                var profitFormatCondition = new FormatCondition()
                {
                    ValueRule = ConditionRule.Expression,
                    Expression = "[CLS_DEV] == 'A'",
                    FieldName = null,
                    Format = new DevExpress.Xpf.Core.ConditionalFormatting.Format()
                    {
                        Background = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0x7F, 0x50))
                    }
                };

                gridPartView.FormatConditions.Add(profitFormatCondition);

                gridPartView.ShowingEditor += (s, e) =>
                {
                    e.Cancel = gridPart.CurrentColumn.FieldName != "IsChecked";
                };

                this.SP_EMS_PART_SEARCH();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        #region ▩ 속성
        public ObservableCollection<EmsPartRef> CurrPartRefList { get; private set; }

        public EmsPartRef CurrPartRef { get; private set; }

        public List<EmsPartRef> liEmsPartRef { get; set; }
        #endregion

        #region ▩ 함수
        #region Title Menu
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
                //this.BaseClass.Error(err);
            }
        }
        #endregion Title Menu

        private void SP_EMS_PART_SEARCH()
        {
            try
            {
                //var _USER_ID = this.BaseInfo.user_id.ToString();

                var param = new Dictionary<string, object>
                {
                    {"P_CENTER_CD",     this.BaseClass.CenterCD },
                    {"P_PART_NM", tbxPartNm.Text.Trim()}
                };

                var strOutParam = new[] { "P_EMS_PART_LIST" };
                string callProc = "PK_EMS_ECOM01_05P.SP_EMS_PART_SEARCH";

                using (BaseDataAccess da = new BaseDataAccess())
                {
                    var outData = da.GetSpDataSet(
                                callProc                      // 호출 프로시저
                            , param                        // Input 파라메터
                            , strOutParam                  // Output 파라메터
                    );

                    if (outData.Tables[0].Rows.Count > 0)
                    {
                        CurrPartRefList = new ObservableCollection<EmsPartRef>();
                        CurrPartRefList.ToObservableCollection(outData.Tables[0]);

                        gridPart.ItemsSource = CurrPartRefList;

                        gridPartView.ExpandAllNodes();

                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("CMPT_INQ"));
                    }
                    else
                    {
                        //this.ChangeStatusLabelEvent(HelperClass.GetMessageByCountryCode("NO_INQ_DATA"));
                    }
                }
            }
            catch { throw; }
        }

        #region 조회
        /// <summary>
        /// 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.SP_EMS_PART_SEARCH();
            }
            catch (Exception ex)
            {
                this.BaseClass.Error(ex);
                this.BaseClass.MsgError(ex.Message, BaseEnumClass.CodeMessage.MESSAGE);
            }
        }

        #endregion 조회

        #region 선택
        /// <summary>
        /// Grid Data 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridPart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridPartView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) { return; }

            if (gridPart.IsGroupRowHandle(rowHandle)) { return; }// A group row has been double clicked

            EmsPartRef target = gridPart.SelectedItem as EmsPartRef;

            if (null != target)
            {
                if ("A" == target.CLS_DEV)
                {
                    return;
                }

                CurrPartRef = target;
                this.Close();
            }
        }

        #endregion 선택


        #endregion

        #region ▩ 이벤트
        #region 적용
        /// <summary>
        /// 적용
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.liEmsPartRef = this.CurrPartRefList.Where(p => p.IsSelected).ToList();

                this.Close();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion 적용

        private void gridPart_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //int rowHandle = gridPartView.GetRowHandleByMouseEventArgs(e);
                //if (rowHandle == GridControl.InvalidRowHandle) { return; }

                //if (gridPart.IsGroupRowHandle(rowHandle)) { return; }// A group row has been double clicked

                //EmsPartRef target = gridPart.SelectedItem as EmsPartRef;

                //if (target.CLS_DEV == "A")
                //{
                //    target.IsSelected = false;
                //    return;
                //}

                //this.CurrPartRefList.Where(p => p.CLS_DEV == "A").ForEach(p => p.IsSelected = true);

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
        // ~ECOM001_05P()
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

        private void SelectorColumnBehavior_SelectorCellCheked(object sender, Modules.Ctrl.SelectorCellChekedEventArgs e)
        {
            try
            {
                this.CurrPartRefList.Where(p => p.CLS_DEV == "A").ForEach(p => p.IsSelected = false);
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
    }
}
