using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using BioScadaServer.BioScada;

namespace BioScadaServer.Alarms
{
    [Serializable]
    public class Alarm : IAlarm
    {

        protected DateTime when;
        public DateTime When { get { return when; } set { when = value; } }

        protected string variableName;
        public string VariableName { get { return variableName; } set { variableName = value; } }

        protected string comment;
        public string Comment { get { return comment; } set { comment = value; } }

        protected object _Value;
        public object Value { get { return _Value; } set { _Value = value; } }


        public virtual event ActionAlarm Notifier;

        public virtual void ReceiverChange(ChangeNotification notifier)
        {
            when = notifier.When;
            Value = notifier.Item.NewValue;
            //variableName = notifier.Item.Name;

        }
    }
}