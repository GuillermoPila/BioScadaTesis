using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BioScadaServer.Drivers.BoolDriver
{
    public class DriverBool : Driver
    {
        private DriverBool()
        {

        }

        private static DriverBool _driver = null;
        public static DriverBool GetInstance()
        {
            if (_driver == null)
                _driver = new DriverBool();
            return _driver;
        }

        public void Initialize()
        {
            //ThreadPool.QueueUserWorkItem(Read);
            time = new Timer(Read, null, 0, 50);
        }

        private Timer time;
        private void Read(object state)
        {
            //DriverBool aux = (state as DriverBool);
          
                for (int i = 0; i < connects.Count; i++)
                {
                   connects[i].Read();
                }
            
        }

        public bool Write(IConnector connect, object value)
        {
            connect.Write(value);
            return true;
        }

        private List<IConnector> connects = new List<IConnector>();
        public List<IConnector> Connectors { get { return connects; } set { connects = value; } }

        public void ServerRequestStopDriver()
        {
            
        }

        public void ServerRequestStartDriver()
        {
          
        }

        public void Fin()
        {

        }
    }
}
