using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioScadaServer.BioScada;

namespace BioScadaServer.Alarms
{
    public interface IAlarm
    {
        DateTime When { get; set; }
        string VariableName { get; set; }
        string Comment { get; set; }
        object Value { get; set; }
        event ActionAlarm Notifier;
        void ReceiverChange(ChangeNotification notifier);
    }
}
