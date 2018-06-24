using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioScadaServer.BioScada;

namespace BioScadaServer.Alarms
{
    public class AlarmROC : Alarm
    {
        //private TimeSpan whenRateOfChange;
        //public TimeSpan WhenRateOfChange { get { return whenRateOfChange; } set { whenRateOfChange = value; } }

        private object vChange;
        public object VChange { get { return vChange; } set { vChange = value; } }

        public override event ActionAlarm Notifier;

        public override void ReceiverChange(ChangeNotification notifier)
        {
            double dif = Convert.ToDouble(notifier.Item.NewValue) - Convert.ToDouble(_Value);
            base.ReceiverChange(notifier);
            if (Convert.ToDouble(vChange) < dif)
            {
                if (Notifier != null)
                    Notifier(this, notifier);

                Console.WriteLine("Alarm!!!!!!!!!!!!!!!!");
            }
        }
    }
}
