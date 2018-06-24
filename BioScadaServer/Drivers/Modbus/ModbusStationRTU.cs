using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace BioScadaServer.Drivers.Modbus
{
    [Serializable]
    public class ModbusStationRTU
    {
        public string modbusStatus;


        private string stationName;
        public string StationName { get { return stationName; } set { stationName = value; } }

        private byte id;
        public byte ID { get { return id; } set { id = value; } }

        private SerialComm port;
        public SerialComm Port { get { return port; } set { port = value; } }

        private bool bit_Endiang;
        public bool Bit_Endiang{get { return bit_Endiang; }set { bit_Endiang = value; }}


        public ModbusStationRTU(string stationName, byte id, SerialComm port, bool bit_Endiang)
        {
            this.stationName = stationName;
            this.id = id;
            this.port = port;
            this.bit_Endiang = bit_Endiang;
        }

        
        
    }
}
