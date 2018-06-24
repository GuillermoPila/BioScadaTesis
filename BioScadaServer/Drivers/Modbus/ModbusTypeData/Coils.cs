using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioScadaServer.Drivers.Modbus.ModbusTypeData;

namespace BioScadaServer.Drivers.Modbus.ModbusTypeData
{
    [Serializable]
    public class Coils : TypeData
    {
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

        public override byte[] GetPakageMessageToRead()
        {
            byte[] message = new byte[8];

            message[0] = ID_Station;
            message[1] = 0x01;
            message[2] = (byte)(Address >> 8);
            message[3] = (byte)Address;     //out 64
            message[4] = 0;
            message[5] = 8;  //de 8 en 8

            byte[] CRC = new byte[2];
            GetCRC(message, ref CRC);
            message[6] = CRC[0];
            message[7] = CRC[1];

            return message;
        }

        public override byte[] GetPakageResponseToRead()
        {
            return new byte[5 + 1];
        }

        public override byte[] GetPakageResponseToWrite()
        {
            return new byte[8];
        }

        public override object SetValueResponse(byte[] response)
        {
            short[] num = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096 };
            int value_read = response[3];
            List<short> res = new List<short>(); //= new short[response[2]];


            if (value_read != 0)
            {
                for (int i = 0; i < num.Length; i++)
                {
                    if (value_read == num[i])
                    {
                        res.Add((short)(i + 1));
                        break;
                    }
                    else
                    {
                        if (value_read < num[i])
                        {
                            res.Add((short)(i));
                            value_read -= num[i - 1];
                            i = -1;
                        }
                    }
                }
            }


            return res.ToArray();
        }
    }
}