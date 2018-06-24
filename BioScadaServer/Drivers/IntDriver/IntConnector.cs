using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioScadaServer.Variables;

namespace BioScadaServer.Drivers.IntDriver
{
    [Serializable]
    public class IntConnector : IConnector
    {
        private string taskName;
        public string ConnectorName { get { return taskName; } set { taskName = value; } }

        private object taskValue;
        public object ConnectValue
        {
            get { return taskValue; }
            set
            {
                lock (lockObject)
                    taskValue = value;
            }
        }

        private readonly DriverInt driver = DriverInt.GetInstance();
        public Driver Driver
        {
            get { return driver; }

        }

        //private int showPeriod;
        //public int ShowPeriod { get { return showPeriod; } set { showPeriod = value; } }

        private bool constant;
        public bool Constant { get { return constant; } set { constant = value; } }

        private int defaultValue;
        public int DefaultValue { get { return defaultValue; } set { defaultValue = value; } }

        private int maxValue;
        public int MaxValue { get { return maxValue; } set { maxValue = value; } }

        static readonly Random rnd = new Random();
        public bool Read()
        {
            if (constant)
                ConnectValue = defaultValue;
            else
                ConnectValue = rnd.Next(0, maxValue);
            //Console.WriteLine(taskValue);
            return true;
        }

        readonly object lockObject = new object();
        public bool Write(object obj)
        {
            ConnectValue = obj;
            return true;
        }
    }
}
