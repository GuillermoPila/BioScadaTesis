using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioScadaServer.Drivers.Modbus.ModbusTypeData;

namespace BioScadaServer.Drivers.Modbus.ModbusTypeData
{
    [Serializable]
    public class Discrete_Inputs : Single_Coil
    {
        public override byte[] GetPakageMessageToRead()
        {
            byte[] message = new byte[8];

            message[0] = ID_Station;
            message[1] = 0x02;
            message[2] = (byte)(Address >> 8);
            message[3] = (byte)Address;     //out 64
            message[4] = 0;
            message[5] = 1;  //de 8 en 8

            byte[] CRC = new byte[2];
            GetCRC(message, ref CRC);
            message[6] = CRC[0];
            message[7] = CRC[1];

            return message;
        }

        public override byte[] GetPakageMessageToWrite(object New_Value)
        {
            byte[] message = new byte[8];
            ushort output_Value;
            if ((bool)New_Value)
                output_Value = 255;
            else
                output_Value = 0;

            message[0] = ID_Station;
            message[1] = 0x05;

            byte MSB = (byte)(Address >> 8);
            message[2] = MSB;
            message[3] = (byte)(Address - (MSB << 8));

            byte MSBR = (byte)(output_Value >> 8);
            if (BitEndiang)
            {
                message[4] = (byte)(output_Value - (MSBR << 8));
                message[5] = MSBR;
            }
            else
            {
                message[4] = MSBR;
                message[5] = (byte)(output_Value - (MSBR << 8));
            }

            byte[] CRC = new byte[2];
            GetCRC(message, ref CRC);
            message[6] = CRC[0];
            message[7] = CRC[1];

            return message;
        }

        public override object SetValueResponse(byte[] response)
        {
            
            int value_read = response[3];
            

            return Convert.ToBoolean(value_read);
        }
    }
}