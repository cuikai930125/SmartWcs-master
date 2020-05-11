using SMART.WCS.Common;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SMART.WCS.Kiosk.Lib
{
    /// <summary>
    /// KioskMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KioskMain : Window, IDisposable
    {
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 타이머 객체 선언
        /// </summary>
        DispatcherTimer g_Timer = new DispatcherTimer();

        public KioskMain()
        {
            try
            {
                InitializeComponent();

                this.InitControl();

                this.InitEvent();

                this.InitValue();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void InitControl()
        {
            try
            {
                this.g_Timer.IsEnabled      = true;
                this.g_Timer.Interval       = TimeSpan.FromSeconds(1);
                this.g_Timer.Tick           += G_Timer_Tick;
                this.g_Timer.Start();
            }
            catch { throw; }
        }

        private void InitEvent()
        {
            try
            {
                //this.btnWindowClose.PreviewMouseLeftButtonUp += BtnWindowClose_PreviewMouseLeftButtonUp;
                //this.btnWindowClose.IsEnabledChanged += BtnWindowClose_IsEnabledChanged;
                //this.btnWindowClose.MouseEnter += BtnWindowClose_MouseEnter;
                //this.btnWindowClose.MouseLeave += BtnWindowClose_MouseLeave;

                this.imgWindowClose.PreviewMouseLeftButtonUp += ImgWindowClose_PreviewMouseLeftButtonUp;
                this.imgWindowClose.IsEnabledChanged += ImgWindowClose_IsEnabledChanged;
                this.imgWindowClose.MouseEnter += ImgWindowClose_MouseEnter;
                this.imgWindowClose.MouseLeave += ImgWindowClose_MouseLeave;
            }
            catch { throw; }
        }

        

        private void InitValue()
        {
            try
            {
                DataTable dtNew = new DataTable();
                dtNew           = this.CreateDataTableSchema(dtNew);

                DataRow drNew       = dtNew.NewRow();
                drNew["SEQ"]        = 1;
                drNew["SKU_NM"]     = "상품명";
                drNew["SKU_CD"]     = "상품코드";
                drNew["TEMPER"]     = "냉장";
                drNew["EXP_QTY"]    = 100;
                drNew["INS_QTY"]    = 200;
                drNew["INV_NO"]     = "송장번호";
                dtNew.Rows.Add(drNew);

                DataRow drNew2       = dtNew.NewRow();
                drNew2["SEQ"]        = 2;
                drNew2["SKU_NM"]     = "상품명";
                drNew2["SKU_CD"]     = "상품코드";
                drNew2["TEMPER"]     = "냉장";
                drNew2["EXP_QTY"]    = 100;
                drNew2["INS_QTY"]    = 200;
                drNew2["INV_NO"]     = "송장번호";
                dtNew.Rows.Add(drNew2);

                DataRow drNew3       = dtNew.NewRow();
                drNew3["SEQ"]        = 3;
                drNew3["SKU_NM"]     = "상품명";
                drNew3["SKU_CD"]     = "상품코드";
                drNew3["TEMPER"]     = "냉장";
                drNew3["EXP_QTY"]    = 100;
                drNew3["INS_QTY"]    = 200;
                drNew3["INV_NO"]     = "송장번호";
                dtNew.Rows.Add(drNew3);

                dgMain.ItemsSource = dtNew.AsDataView();
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private DataTable CreateDataTableSchema(DataTable _dtParam)
        {
            try
            {
                if (_dtParam == null) { _dtParam = new DataTable(); }

                _dtParam.Columns.Add("SEQ",             typeof(int));
                _dtParam.Columns.Add("SKU_NM",          typeof(string));
                _dtParam.Columns.Add("SKU_CD",          typeof(string));
                _dtParam.Columns.Add("TEMPER",          typeof(string));
                _dtParam.Columns.Add("EXP_QTY",         typeof(int));
                _dtParam.Columns.Add("INS_QTY",         typeof(int));
                _dtParam.Columns.Add("INV_NO",          typeof(string));

                return _dtParam;
            }
            catch { throw; }
        }

        #region ▩ 이벤트
        private void G_Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.lblClock.Text  = DateTime.Now.ToString("yyyy-MM-dd (ddd) HH:mm:ss");
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void BtnWindowClose_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.imgWindowClose.Opacity = 1;


            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void BtnWindowClose_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //if (this.btnWindowClose.IsEnabled == true) { this.imgWindowClose.Opacity = 0.7; }
                //else { this.imgWindowClose.Opacity = 0.3; }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void BtnWindowClose_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                this.imgWindowClose.Opacity = 0.9;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        private void BtnWindowClose_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                this.imgWindowClose.Opacity = 0.7;
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }
        #endregion

        private void ImgWindowClose_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.imgWindowClose.Opacity = 1;
            this.Close();
        }

        private void ImgWindowClose_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if (this.btnWindowClose.IsEnabled == true) { this.imgWindowClose.Opacity = 0.7; }
            //else { this.imgWindowClose.Opacity = 0.3; }
        }

        private void ImgWindowClose_MouseLeave(object sender, MouseEventArgs e)
        {
            this.imgWindowClose.Opacity = 0.9;
        }

        private void ImgWindowClose_MouseEnter(object sender, MouseEventArgs e)
        {
            this.imgWindowClose.Opacity = 0.7;
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
        // ~KioskMain()
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
