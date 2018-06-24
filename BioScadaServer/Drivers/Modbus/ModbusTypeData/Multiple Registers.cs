using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioScadaServer.Drivers.Modbus.ModbusTypeData
{
    [Serializable]
    public class Multiple_Registers
    {
        public byte[] GetPakageMessageToWrite(object New_Value)
        {
            throw new System.NotImplementedException();
        }

        public byte[] GetPakageMessageToRead()
        {
            throw new System.NotImplementedException();
        }

        public byte[] GetPakageResponseToRead()
        {
            throw new System.NotImplementedException();
        }

        public byte[] GetPakageResponseToWrite()
        {
            throw new System.NotImplementedException();
        }

        public object SetValueResponse(byte[] response)
        {
            throw new System.NotImplementedException();
        }
    }
}