using SMART.WCS.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Control.DataMembers
{
    public class DateTimeNow : DateTimeInfo
    {
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        public DateTimeNow()
        {
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;

            if (SMART.WCS.Modules.Behaviors.CommonProperties.GetIsDesignTime())
            {
                this.Now = DateTime.Now.ToString("yyyy/MM/dd ddd hh:mm:ss");
            }
            else
            { 
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Now = DateTime.Now.ToString("yyyy/MM/dd ddd hh:mm:ss");
        }
    }
    public class DateTimeInfo : PropertyNotifyExtensions
    {
        private string _Now;

        public string Now
        {
            get
            {
                return _Now;
            }
            set
            {
                if (_Now != value)
                {
                    _Now = value;
                    RaisePropertyChanged();
                }
            }
        }

    }
}
