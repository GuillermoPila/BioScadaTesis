using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using BioScadaServer.BioScada;
using BioScadaServer.Drivers;
using System.Runtime.Serialization;

namespace BioScadaServer.Variables
{
    [Serializable]
    [DataContract]
    public class Variable : IVariable
    {
        private string name;
        [DataMember]
        public string Name { get { return name; } set { name = value; } }

        private object value;
        [DataMember]
        public object Value
        {
            get { return value; }
            set
            {
                if (value != null)
                    if (!this.value.Equals(value))
                    {
                        this.value = value;

                        if (receive != null)
                            for (int i = 0; i < receive.Length; i++)
                            {
                                notifier.NotifyChange(new ChangeNotification(DateTime.Now,
                                                          new ChangeNotification.ItemVar() { Name = name, NewValue = value }),
                                                      receive[i]
                                    );
                            }
                    }

            }
        }

        private bool isEnabled;
        public bool IsEnabled { get { return isEnabled; } }

        private int requestPeriod;
        public int RequestPeriod { get { return requestPeriod; } set { requestPeriod = value; } }

        private IConnector[] connector;
        public IConnector[] Connector { get { return connector; } set { connector = value; } }

        private List<Experiment> experiment;
        public List<Experiment> Experiment { get { return experiment; } set { experiment = value; } }

        private INotifierReceiverChange[] receive;
        public INotifierReceiverChange[] Receive
        {
            get { return receive; }
            set { receive = value; }
        }

        private Notifier notifier;
        //  public Notifier Notifier { get { return notifier; } set { notifier = value; } }

        public Variable(string name, object value)
        {
            this.name = name;
            this.value = value;
            notifier = Notifier.GetInstance();
            isEnabled = false;
            experiment = new List<Experiment>();
        }

        public Variable(string name, object value, IConnector[] connectVar)
        {
            this.name = name;
            this.value = value;
            this.connector = connectVar;
            notifier = Notifier.GetInstance();
            isEnabled = false;
            experiment = new List<Experiment>();
        }

        private int _RefEnabled = 0;
        public void Start()
        {
            _RefEnabled++;
            if (_RefEnabled > 0)
                isEnabled = true;
        }

        public void Stop()
        {
            if (_RefEnabled > 0)
            {
                _RefEnabled--;
                isEnabled = _RefEnabled != 0;
            }
            else
            {
                _RefEnabled = 0;
                isEnabled = false;
            }
        }

        public bool RequestValueChange(object obj)
        {
            bool[] result = new bool[connector.Length];
            bool res = false;
            for (int i = 0; i < connector.Length; i++)
            {
                result[i] = connector[i].Driver.Write(connector[i], obj);
            }

            if (result.Contains(false))
            {
                res = false;
            }
            else
            {
                value = obj;
                res = true;

            }
            return res;
        }
    }
}
