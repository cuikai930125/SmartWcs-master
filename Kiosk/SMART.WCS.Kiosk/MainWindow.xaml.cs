using SMART.WCS.Common_Etc;
using SMART.WCS.Common_Etc.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SMART.WCS.Kiosk
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        BaseClass BaseClass = new BaseClass();

        /// <summary>
        /// 타이머 객체 선언
        /// </summary>
        DispatcherTimer g_Timer = new DispatcherTimer();

        public MainWindow()
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
                this.btnSample1.PreviewMouseLeftButtonUp += BtnSample1_PreviewMouseLeftButtonUp;

                this.btnWindowClose.PreviewMouseLeftButtonUp += BtnWindowClose_PreviewMouseLeftButtonUp;
                this.btnWindowClose.IsEnabledChanged += BtnWindowClose_IsEnabledChanged;
                this.btnWindowClose.MouseEnter += BtnWindowClose_MouseEnter;
                this.btnWindowClose.MouseLeave += BtnWindowClose_MouseLeave;
            }
            catch { throw; }
        }

        private void BtnSample1_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.GetDatabaseConnectionInfo();
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

        #region >> GetDatabaseConnectionInfo - 센터 정보를 조회한다.
        /// <summary>
        /// 센터 정보를 조회한다.
        /// </summary>
        /// <returns></returns>
        private DataTable GetDatabaseConnectionInfo()
        {
            try
            {
                DataTable dtRtnValue = null;
                var strProcedureName = "PK_C1001.SP_CNTR_LIST_INQ";
                var dicInputParam = new Dictionary<string, object>();
                string[] arrOutputParam = { "O_CNTR_LIST", "O_RSLT" };

                using (FirstDataAccess dataAccess = new FirstDataAccess())
                {
                    var dsDatabaseConnectionInfo = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);

                    var strErrCode = string.Empty;
                    var strErrMsg = string.Empty;

                    if (this.BaseClass.CheckResultDataProcess(dsDatabaseConnectionInfo, ref strErrCode, ref strErrMsg) == true)
                    {
                        dtRtnValue = dsDatabaseConnectionInfo.Tables["O_CNTR_LIST"];
                    }
                    else
                    {
                        dtRtnValue = null;
                        MessageBox.Show(strErrMsg);
                    }
                }

                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion

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
                if (this.btnWindowClose.IsEnabled == true) { this.imgWindowClose.Opacity = 0.7; }
                else { this.imgWindowClose.Opacity = 0.3; }
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
    }
}
