using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioScadaServer.Drivers.Modbus.ModbusTypeData
{
    [Serializable]
    public class Single_Coil : Coils
    {
        public override object SetValueResponse(byte[] response)
        {
            int value_read = response[3];
            return Convert.ToBoolean(value_read);
        }
    }
}