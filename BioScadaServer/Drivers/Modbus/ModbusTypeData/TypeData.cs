using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioScadaServer.Drivers.Modbus.ModbusTypeData;

namespace BioScadaServer.Drivers.Modbus.ModbusTypeData
{
    [Serializable]
    public abstract class TypeData : ITypeData
    {
        private int address;
        public int Address { get { return address; } set { address = value; } }

        private byte id_Station;
        public byte ID_Station { get { return id_Station; } set { id_Station = value; } }

        private bool bitEndiang;
        public bool BitEndiang { get { return bitEndiang; } set { bitEndiang = value; } }

        public virtual byte[] GetPakageMessageToWrite(object New_Value)
        {
            throw new System.NotImplementedException();
        }

        public virtual byte[] GetPakageMessageToRead()
        {
            throw new System.NotImplementedException();
        }

        public virtual byte[] GetPakageResponseToRead()
        {
            throw new System.NotImplementedException();
        }

        public virtual byte[] GetPakageResponseToWrite()
        {
            throw new System.NotImplementedException();
        }

        public virtual object SetValueResponse(byte[] response)
        {
            throw new System.NotImplementedException();
        }

        #region CRC Computation
        protected void GetCRC(byte[] message, ref byte[] CRC)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
        #endregion
    }
}