using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioScadaServer.Drivers.BoolDriver
{
    [Serializable]
    public class BoolConnector : IConnector
    {
        private string taskName;
        public string ConnectorName { get { return taskName; } set { taskName = value; } }

        private object taskValue;
        public object ConnectValue
        {
            get { return taskValue; }
            set
            {
                lock(lckObj)
                taskValue = value;
            }
        }

        static Random rnd = new Random();
        readonly DriverBool driver = DriverBool.GetInstance();
        public Driver Driver
        {
            get { return driver; }
        }

        public bool Read()
        {
            int num = rnd.Next(0, 1000);
            if (num < 1000 / 2)
            {
                    taskValue = true;
            }
            else
            {
                    taskValue = false;
            }
            return true;
        }

        readonly object lckObj = new object();
        public bool Write(object obj)
        {
            lock (lckObj)
                taskValue = obj;
            return true;
        }

    }
}