using SMART.WCS.Common;
using SMART.WCS.Common.DataBase;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMART.WCS.Control
{
    /// <summary>
    /// CommentArea.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommentArea : UserControl
    {
        #region ▩ 전역변수
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 화면 로드 여부
        /// </summary>
        private bool g_isLoaded = false;
        #endregion

        #region ▩ 생성자
        public CommentArea()
        {
            try
            {
                InitializeComponent();

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
        #region > 메뉴 ID
        public static readonly DependencyProperty ScreenIDProperty
            = DependencyProperty.Register("ScreenID", typeof(string)
                , typeof(NavigationBar), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 메뉴 ID (클래스명)
        /// </summary>
        public string ScreenID
        {
            get { return (string)GetValue(ScreenIDProperty); }
            set { SetValue(ScreenIDProperty, value); }
        }
        #endregion
        #endregion

        #region ▩ 함수
        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitEvent()
        {
            try
            {
                this.Loaded += CommentArea_Loaded;
            }
            catch { throw; }
        }

        #region > 데이터 관련
        #region >> GetSP_MENU_DESC_INQ - 화면 설명 (Comment)
        /// <summary>
        /// 화면 설명 (Comment)
        /// </summary>
        /// <returns></returns>
        private async Task GetSP_MENU_DESC_INQ()
        {
            #region + 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue                          = null;
            var strProcedureName                        = "PK_C1000.SP_MENU_DESC_INQ";
            Dictionary<string, object> dicInputParam    = new Dictionary<string, object>();
            string[] arrOutputParam                     = { "O_MENU_INFO", "O_RSLT" };

            var strCompanyCD    = this.BaseClass.CompanyCode;   // 회사코드
            var strCenterCD     = this.BaseClass.CenterCD;      // 센터코드
            
            #endregion

            #region + Input 파라메터
            dicInputParam.Add("P_CNTR_CD",          strCenterCD);           // 센터코드
            dicInputParam.Add("P_MENU_ID",          this.ScreenID);         // 메뉴 ID
            #endregion

            #region + 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                }).ConfigureAwait(true);
            }

            string strComment       = string.Empty;

            if (dsRtnValue.Tables.Count == 2)
            {
                if (dsRtnValue.Tables[0].Rows.Count > 0)
                {
                    if (dsRtnValue.Tables[0].Rows[0]["MENU_DESC"] != null)
                    {
                        strComment = this.lblCommentArea.Text = dsRtnValue.Tables[0].Rows[0]["MENU_DESC"].ToString();
                    }
                }

                this.lblCommentArea.Text = strComment;

                if (this.lblCommentArea.Text.Trim().Length > 0)
                {
                    this.imgBookmarkApply.Visibility = Visibility.Visible;
                }
            }
            #endregion
        }
        #endregion
        #endregion
        #endregion

        #region ▩ 이벤트
        /// <summary>
        /// 로드 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CommentArea_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.g_isLoaded == true) { return; }

                if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == true) { return; }

                await this.GetSP_MENU_DESC_INQ();

                this.g_isLoaded = false;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion
    }
}
