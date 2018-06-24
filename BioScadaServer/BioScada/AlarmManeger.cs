using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioScadaServer.Alarms;

namespace BioScadaServer.BioScada
{
    
    public delegate void ActionAlarm(object sender, ChangeNotification notifier);

    [Serializable]
    public class AlarmManeger : INotifierReceiverChange
    {
        public List<Alarm> Alarms = new List<Alarm>();
        [NonSerialized]
        private Server serv = Server.GetInstance();
        
        public void Initialize()
        {
            for (int i = 0; i < Alarms.Count; i++)
            {
                Alarms[i].Notifier += serv.ActionAlarmNotifier;
            }
        }

        public void ReceiverChange(ChangeNotification notifier)
        {
            for (int i = 0; i < Alarms.Count; i++)
            {
                if (Alarms[i].VariableName == notifier.Item.Name)
                Alarms[i].ReceiverChange(notifier);
            }

        }
        
        //public static void ActionAlarmNotifier(object sender, ChangeNotification var)
        //{
        //    Console.WriteLine("Alarm!!!!!  " + var.Item.Name + "  " + var.Item.NewValue);
        //}
    }
}
