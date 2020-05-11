using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMART.WCS.Control
{
    public static class KioskMesssage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="MessageType"></param>
        /// <returns>MessageBoxResult.OK Or MessageBoxResult.Cancel</returns>
        public static System.Windows.MessageBoxResult Show(string messageBoxText, MessageTypes MessageType)
        {
            System.Windows.MessageBoxResult _result = System.Windows.MessageBoxResult.None;

            try
            {
                Thread _thread = new Thread(delegate ()
                {
                    KioskMessageBox messageBox = new KioskMessageBox
                    {
                        AllowsTransparency = true,
                        WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                        WindowStyle = System.Windows.WindowStyle.None
                    };

                    try
                    {
                        if (!messageBox.Dispatcher.CheckAccess())
                        {
                            messageBox.Dispatcher.Invoke(
                              System.Windows.Threading.DispatcherPriority.Normal,
                              new Action(
                                delegate ()
                                {
                                    messageBox.MessageText = messageBoxText.Replace("|", "\n");
                                    messageBox.MessageType = MessageType;
                                    messageBox.Visibility = System.Windows.Visibility.Visible;
                                }
                            ));
                        }
                        else
                        {
                            messageBox.MessageText = messageBoxText.Replace("|", "\n");
                            messageBox.MessageType = MessageType;
                            messageBox.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    var _reusult = messageBox.ShowDialog();
                    _result = messageBox.Result;
                });

                _thread.SetApartmentState(ApartmentState.STA);
                _thread.Start();
                _thread.Join();
            }
            catch (Exception)
            {
                throw;
            }
          

            return _result;
        }
    }

}
