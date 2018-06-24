using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioScadaServer.Drivers.Modbus.ModbusTypeData
{
    [Serializable]
    public class Input_Registers : ModbusTypeData.TypeData
    {
        
        public override byte[] GetPakageMessageToWrite(object New_Value)
        {
            byte[] message = new byte[8];
            message[0] = ID_Station;
            message[1] = 0x06;

            byte MSB = (byte)(Address >> 8);
            message[2] = MSB;
            message[3] = (byte)(Address - (MSB << 8));

            byte MSBR = (byte)((int)New_Value >> 8);

            message[4] = (byte)((int)New_Value - (MSBR << 8));
            message[5] = MSBR;

            byte[] CRC = new byte[2];
            GetCRC(message, ref CRC);
            message[6] = CRC[0];
            message[7] = CRC[1];

            return message;
        }

        public override byte[] GetPakageMessageToRead()
        {

            short registers = 1;

            byte[] message = new byte[8];

            message[0] = ID_Station;
            message[1] = 0x04;
            byte MSB = (byte)(Address >> 8);
            message[2] = MSB;
            message[3] = (byte)(Address - (MSB << 8));

            byte MSBR = (byte)(registers >> 8);
            message[4] = MSBR;
            message[5] = (byte)(registers - (MSBR << 8));

            byte[] CRC = new byte[2];
            GetCRC(message, ref CRC);
            message[6] = CRC[0];
            message[7] = CRC[1];

            return message;

        }

        public override byte[] GetPakageResponseToRead()
        {
            return new byte[5 + 2];
        }

        public override byte[] GetPakageResponseToWrite()
        {
            return new byte[8];
        }

        public override object SetValueResponse(byte[] response)
        {
            int localValue = 0;
            localValue = response[3];
            localValue <<= 8;
            localValue += response[4];
            return localValue;
        }
    }
}