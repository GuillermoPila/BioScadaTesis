using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioScadaServer.Drivers.Modbus.ModbusTypeData;
using System.Threading;


namespace BioScadaServer.Drivers.Modbus
{
    [Serializable]
    public class ConnectorModbusRTU : IConnector
    {
        private ModbusStationRTU station;
        public ModbusStationRTU Station { get { return station; } set { station = value; } }

        private string taskName;
        public string ConnectorName { get { return taskName; } set { taskName = value; } }

        private object connectValue;
        public object ConnectValue { get { return connectValue; } set { connectValue = value; } }

        private DriverModbusRTU _driver = DriverModbusRTU.GetInstance();
        public Driver Driver
        {
            get { return _driver; }set{ _driver = (DriverModbusRTU) value;}
        }

        private int startAddress;
        public int StartAddress { get { return startAddress; } set { startAddress = value; } }

        private ModbusTypeData.TypeData typeData;
        public ModbusTypeData.TypeData TypeData { get { return typeData; } set { typeData = value; } }


        private string taskStatus;
        public string TaskStatus { get { return taskStatus; } set { taskStatus = value; } }

        public ConnectorModbusRTU()
        {

        }

        public ConnectorModbusRTU(string taskName, ModbusStationRTU station, int startAddress, ModbusTypeData.TypeData typeData)
        {
            this.taskName = taskName;
            this.station = station;
            this.startAddress = startAddress;
            this.typeData = typeData;
            this.typeData.Address = startAddress;
            this.typeData.ID_Station = station.ID;
            this.typeData.BitEndiang = station.Bit_Endiang;
        }

        private static readonly object lckObj = new object();
        private readonly object lckObjSetValue = new object();

        public bool Read()
        {
            if (station.Port.Open())
            {
                //_pool.Release();
                station.Port.DiscardOutBuffer();
                station.Port.DiscardInBuffer();

                byte[] message = typeData.GetPakageMessageToRead();
                byte[] response = typeData.GetPakageResponseToRead();



                bool result = ReadWritePort(message, response, station);

                if (result)
                {
                    //lock (lckObjSetValue)
                        connectValue = typeData.SetValueResponse(response);
                    taskStatus = "Read successful";
                    //Console.WriteLine(taskStatus);

                    return true;
                }
                else
                {
                    taskStatus = "CRC error in Readdddddddd";
                    return false;
                }
            }
            else
            {
                taskStatus = "Serial port not open";
                return false;
            }

        }

        private bool ReadWritePort(byte[] message, byte[] response, ModbusStationRTU station)
        {

            try
            {
                lock (lckObj)
                {
                    station.Port.Write(message, 0, message.Length);
                    for (int i = 0; i < response.Length; i++)
                    {
                        response[i] = (byte)station.Port.ReadByte();
                    }

                    if (CheckResponse(response))
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Nooooooooooooooooooooooooooooooo CRC");
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

        }

        public bool Write(object New_Value)
        {

            if (station.Port.Open())
            {

                station.Port.DiscardOutBuffer();
                station.Port.DiscardInBuffer();
                
                byte[] message = typeData.GetPakageMessageToWrite(New_Value);
                byte[] response = typeData.GetPakageResponseToWrite();


                if (ReadWritePort(message, response, station))
                {
                    Console.WriteLine("Escribioooooooooooooooooooooooooooo");
                    return true;
                }

                else
                {
                    
                    return false;
                }


            }
            else
            {
                taskStatus = "Serial port not open";
                Console.WriteLine(taskStatus);
                return false;
            }
        }


        #region CRC Computation
        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

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

        #region Check Response
        private bool CheckResponse(byte[] response)
        {

            //Perform a basic CRC check:
            byte[] CRC = new byte[2];
            GetCRC(response, ref CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
            {

                return true;
            }
            else
            {

                return false;
            }

        }
        #endregion

        #region Get Response
        private void GetResponse(ref byte[] response)
        {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.
            for (int i = 0; i < response.Length; i++)
            {
                response[i] = (byte)(station.Port.ReadByte());
            }
        }
        #endregion

        /*  private ModbusTypeData.TypeData GetTypeData(TypeData type)
        {
            switch (type)
            {
                case Modbus.TypeData.Coils:
                    return new Coils(startAddress, station.ID, station.Bit_Endiang, 8);

                case Modbus.TypeData.DiscreteInputs:
                    return new Discrete_Inputs(startAddress, station.ID, station.Bit_Endiang, 8);

                case Modbus.TypeData.Inputs_Registers:
                    return new Input_Registers(startAddress, station.ID);

                case Modbus.TypeData.SingleCoil:
                    return new Single_Coil(startAddress, station.ID, station.Bit_Endiang, 8);
                case Modbus.TypeData.SingleRegister:
                    return new Single_Register(startAddress, station.ID);
                default:
                    return new Single_Register(startAddress, station.ID);
            }
        }*/

    }



}