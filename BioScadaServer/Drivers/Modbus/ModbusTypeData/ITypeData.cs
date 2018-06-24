using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioScadaServer.Drivers.Modbus.ModbusTypeData
{
    public interface ITypeData
    {
        byte[] GetPakageMessageToWrite(object New_Value);

        byte[] GetPakageMessageToRead();

        byte[] GetPakageResponseToRead();

        byte[] GetPakageResponseToWrite();

        object SetValueResponse(byte[] response);
    }
}