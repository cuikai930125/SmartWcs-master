using LGCNS.ezControl.Diagnostics;
using LGCNS.ezControl.Presentation;
using System;
using System.Windows.Threading;
using SMART.WCS.Common;
using System.Configuration;

namespace SMART.WCS.Control
{
    /// <summary>
    /// UCConnection.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UCConnection : UCCustomUserControl
    {
        #region Delegate
        public delegate void delegatePanelUIHandeler(int iEventID, params object[] args);
        #endregion

        #region Event Define
        public event delegatePanelUIHandeler OnPanelUIEvent;
        #endregion

        /// <summary>
        /// Base 클래서 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();        

        #region Constructor

        public UCConnection()
        {
            InitializeComponent();            
        }

        #endregion

        #region Override Method
        protected override void OnLinkedReferenceEvent(string strSubjectName, int iEventID, params object[] args)
        {
            base.OnLinkedReferenceEvent(strSubjectName, iEventID, args);

            DispatchEvent(iEventID, args);
        }

        protected override void OnReferenceLinked(LGCNS.ezControl.Core.CReference reference)
        {
            base.OnReferenceLinked(reference);

            if (reference.ReferenceState == LGCNS.ezControl.Common.enumReferenceState.Active)
            {
                OnPanelUIEvent?.Invoke((int)BaseEnumClass.EnumToUIEvent.Connected, null);

            }
            else if (reference.ReferenceState == LGCNS.ezControl.Common.enumReferenceState.Fault)
            {
                OnPanelUIEvent?.Invoke((int)BaseEnumClass.EnumToUIEvent.Disconnected, null);
            }
        }

        protected override void OnLinkedReferenceStateChanged(LGCNS.ezControl.Core.CReference refer, LGCNS.ezControl.Common.enumReferenceState state)
        {
            base.OnLinkedReferenceStateChanged(refer, state);

            if (refer.ReferenceState == LGCNS.ezControl.Common.enumReferenceState.Active)
            {
                OnPanelUIEvent?.Invoke((int)BaseEnumClass.EnumToUIEvent.Connected, null);
            }
            else if (refer.ReferenceState == LGCNS.ezControl.Common.enumReferenceState.Fault)
            {
                OnPanelUIEvent?.Invoke((int)BaseEnumClass.EnumToUIEvent.Disconnected, null);
            }
        }

        #endregion

        #region Public Method
        public object SendCommand(BaseEnumClass.EnumToCoreEvent cmd, params object[] args)
        {
            return FireToCoreEvent(cmd, args);
        }
        #endregion

        #region Private Method
        #region DispatchEvent (Core -> UI Event 실행 Method)
        private void DispatchEvent(int iEventID, params object[] args)
        {
            try
            {
                OnPanelUIEvent?.Invoke(iEventID, args);
            }
            catch (Exception ex)
            {
                SystemLogger.Log(LGCNS.ezControl.Diagnostics.Level.Exception, ex, this.Name);
            }
        }
        #endregion
        #region Common (UI 애니메이션 및 Button Click Event)
        #region Execute Wrapper
        private object FireToCoreEvent(BaseEnumClass.EnumToCoreEvent eventName, params object[] args)
        {
            try
            {
                object ret = new object();

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate {
                    ret = _LinkedReference.Execute(BaseEnumClass.UIEventReceiver, (int)eventName, args);
                }));

                return ret;
            }
            catch (Exception ex)
            {
                SystemLogger.Log(LGCNS.ezControl.Diagnostics.Level.Exception, ex, this.Name);
                return -1;
            }
        }

        #endregion
        #endregion

        #region ConnectionString 암호해제 
        public ConnectionStringSettings GetFactovaConnection()
        {
            ConnectionStringSettings con = new ConnectionStringSettings();
            con.ConnectionString = this.BaseClass.DecryptAES256(ConfigurationManager.ConnectionStrings[$"DB_CONNECTION_{this.BaseClass.CenterCD}"].ToString());
            con.ProviderName = "System.Data.SqlClient";
            return con;
        }
        #endregion
        #endregion


    }
}
