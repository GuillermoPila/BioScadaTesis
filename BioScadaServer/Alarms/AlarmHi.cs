using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioScadaServer.BioScada;

namespace BioScadaServer.Alarms
{
    [Serializable]
    public class AlarmHi : Alarm
    {
        private object maxValue;
        public object MaxValue { get { return maxValue; } set { maxValue = value; } }

        public override event ActionAlarm Notifier;

        public override void ReceiverChange(ChangeNotification notifier)
        {
            base.ReceiverChange(notifier);
            if (Convert.ToDouble(notifier.Item.NewValue) > Convert.ToDouble(maxValue))
            {
                if (Notifier != null)
                    Notifier(this, notifier);
            }
        }
    }
}
