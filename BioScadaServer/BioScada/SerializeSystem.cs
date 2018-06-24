using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioScadaServer.Variables;
using BioScadaServer.Drivers;
using BioScadaServer.Drivers.Modbus;
using BioScadaServer.Alarms;

namespace BioScadaServer.BioScada
{
    [Serializable]
    public class SerializeSystem
    {
        public SerialComm Com;
        public AlarmManeger AlarmManeger;
        public List<Experiment> Experiments;
        
        public List<ConnectorModbusRTU> Connectors;
        public ModbusStationRTU Station;
        public DBLogger DB;
     }
}
