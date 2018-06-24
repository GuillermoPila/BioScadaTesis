using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BioScadaServer.Tools;
using BioScadaServer.BioScada;

namespace BioScadaServer.Drivers
{
    [Serializable]
    public class ConnectionManager
    {
        //private string name;
        //public string Name { get { return name; } set { name = value; } }

        private Experiment[] experiments;
        public Experiment[] Experiments
        {
            get { return experiments; }
            set
            {
                experiments = value;
                TraslateConnector();
            }
        }

        private static ConnectionManager _connects = null;
        private ConnectionManager()
        {
            Connectors = new IConnector[0];
            _drivers = new Driver[0];
        }

        public static ConnectionManager GetInstance()
        {
            if (_connects == null)
                _connects = new ConnectionManager();
            return _connects;
        }

        private IConnector[] Connectors;
        private void AddConnector(IConnector connect)
        {
            int length;
            if (!Connectors.Any(x => x == connect))
            {
                length = Connectors.Length;
                Array.Resize(ref Connectors, length + 1);
                Connectors.SetValue(connect, length);
            }
        }
        private void TraslateConnector()
        {
            for (int i = 0; i < experiments.Length; i++)
                for (int j = 0; j < experiments[i].Variables.Count; j++)
                    if (experiments[i].Variables[j].Connector != null)
                    for (int k = 0; k < experiments[i].Variables[j].Connector.Length; k++)
                    {
                        AddConnector(experiments[i].Variables[j].Connector[k]);
                        AddDrivers(experiments[i].Variables[j].Connector[k].Driver);
                    }
            for (int i = 0; i < Connectors.Length; i++)
            {
                Connectors[i].Driver.Connectors.Add(Connectors[i]);
            }
            
        }

        private Driver[] _drivers;
        private void AddDrivers(Driver driver)
        {
            int length;
            if (!_drivers.Any(x => x == driver))
            {
                length = _drivers.Length;
                Array.Resize(ref _drivers, length + 1);
                _drivers.SetValue(driver, length);
            }
        }

        public void InitializeDrivers()
        {
            for (int i = 0; i < _drivers.Length; i++)
            {
                _drivers[i].Initialize();
            }
        }

        public void Fin()
        {
            for (int i = 0; i < _drivers.Length; i++)
            {
                _drivers[i].Fin();
            }
        }

    }

    
}
