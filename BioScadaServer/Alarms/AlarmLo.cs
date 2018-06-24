using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioScadaServer.BioScada;

namespace BioScadaServer.Alarms
{
    [Serializable]
    public class AlarmLo : Alarm
    {
        private object minValue;
        public object MinValue { get { return minValue; } set { minValue = value; } }

        public override event ActionAlarm Notifier;

        public override void ReceiverChange(ChangeNotification notifier)
        {
            base.ReceiverChange(notifier);
            if (Convert.ToDouble(notifier.Item.NewValue) < Convert.ToDouble(minValue))
            {
                if (Notifier != null)
                    Notifier(this, notifier);
            }
        }
    }
}
