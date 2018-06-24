using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using BioScadaServer.Drivers.Modbus.ModbusTypeData;

namespace BioScadaServer.Drivers.Modbus
{
    [Serializable]
    public class DriverModbusRTU : Driver
    {
        [Serializable]
        public delegate void HandledReadWrite(ConnectorsObjectHandled Item);
        [Serializable]
        public class ConnectorsObjectHandled
        {
            public HandledReadWrite Function;
            public ConnectorModbusRTU[] Connectors;
            public object ObjectValue;
        }

        private static DriverModbusRTU _driver = null;

        private readonly List<ConnectorsObjectHandled> groupsRead;
        private readonly Queue<ConnectorsObjectHandled> groupsWrite;

        private DriverModbusRTU()
        {
            groupsRead = new List<ConnectorsObjectHandled>();
            groupsWrite = new Queue<ConnectorsObjectHandled>();
            connectorsCG = new List<ConnectorModbusRTU>();
        }

        public static DriverModbusRTU GetInstance()
        {
            if (_driver == null)
                _driver = new DriverModbusRTU();
            return _driver;
        }

        public void Initialize()
        {
            for (int i = 0; i < Connectors.Count; i++)
            {
                connectorsCG.Add(Connectors[i] as ConnectorModbusRTU);
            }
            CreateGroups(connectorsCG.ToArray());
            time = new Timer(Read, null, 0, int.MaxValue);
            //ThreadPool.QueueUserWorkItem(Read);
        }

        private readonly List<ConnectorModbusRTU> connectorsCG;
        private List<IConnector> connects = new List<IConnector>();
        public List<IConnector> Connectors { get { return connects; } set { connects = value; } }
        public void Fin()
        {

        }

        public bool Write(IConnector connect, object Value)
        {
            ConnectorModbusRTU connector = (connect as ConnectorModbusRTU);
            lock (lckgroupObj)
                groupsWrite.Enqueue(new ConnectorsObjectHandled() { Function = WriteCoil, Connectors = new[] { connector }, ObjectValue = Value });
            return true;
        }

        Dictionary<int, ConnectorModbusRTU> _ConnectorAddress = new Dictionary<int, ConnectorModbusRTU>();
        private void CreateGroups(ConnectorModbusRTU[] connectors)
        {
            List<ConnectorModbusRTU> coils = new List<ConnectorModbusRTU>();
            List<ConnectorModbusRTU> registers = new List<ConnectorModbusRTU>();

            for (int i = 0; i < connectors.Length; i++)
            {
                if ((connectors[i].TypeData is Coils) || (connectors[i].TypeData is Discrete_Inputs) || (connectors[i].TypeData is Single_Coil))
                {
                    coils.Add(connectors[i]);
                }
                else
                {
                    registers.Add(connectors[i]);
                }
            }

            IOrderedEnumerable<ConnectorModbusRTU> resultCoils = coils.OrderBy(x => x.StartAddress);
            _ConnectorAddress = resultCoils.ToDictionary(x => x.StartAddress);


            groupsRead.Add(new ConnectorsObjectHandled() { Function = ReadCoils, Connectors = resultCoils.ToArray() });
            groupsRead.Add(new ConnectorsObjectHandled() { Function = ReadRegisters, Connectors = registers.ToArray() });
        }

        private static readonly object lckgroupObj = new object();
        //List<TimeSpan> mylist = new List<TimeSpan>();
        [NonSerialized]
        private Timer time;
        private void Read(object state)
        {

            //while (true)
            //{
            //Console.Write("*");
            //DateTime mn = DateTime.Now;
            if (_ServerStop)
            {
                _SemaphoreServer.Set();
                _SemaphoreDriver.WaitOne();
            }

            for (int i = 0; i < groupsRead.Count; i++)
            {
                groupsRead[i].Function(groupsRead[i]);
            }


            if (groupsWrite.Any())
            {
                ConnectorsObjectHandled aux;
                lock (lckgroupObj)
                    aux = groupsWrite.Dequeue();
                try
                {
                    aux.Function(aux);
                }
                catch { }
            }
            //}
           //  mylist.Add(DateTime.Now - mn);
            time.Change(0, int.MaxValue);
        }

        private bool _ServerStop = false;
        [NonSerialized]
        ManualResetEvent _SemaphoreServer = new ManualResetEvent(false);// Semaphore(0, 1);
        [NonSerialized]
        ManualResetEvent _SemaphoreDriver = new ManualResetEvent(false);// Semaphore(0, 1);
        public void ServerRequestStopDriver()
        {
            lock (lckObj)
            {
                _ServerStop = true;
                _SemaphoreServer.WaitOne();
            }

        }

        public void ServerRequestStartDriver()
        {
            lock (lckObj)
            {
                _ServerStop = false;
                _SemaphoreDriver.Set();
            }
        }

        private void ReadCoils(ConnectorsObjectHandled par)
        {
            if (par.Connectors != null && par.Connectors.Length > 0)
                if (par.Connectors[0].Station.Port.Open())
                {

                    par.Connectors[0].Station.Port.DiscardOutBuffer();
                    par.Connectors[0].Station.Port.DiscardInBuffer();

                    byte[] message = new byte[8];
                    int contCoils = (par.Connectors[par.Connectors.Length - 1].StartAddress - par.Connectors[0].StartAddress) + 1;

                    message[0] = par.Connectors[0].Station.ID;
                    message[1] = 0x01;
                    message[2] = (byte)(par.Connectors[0].StartAddress >> 8);
                    message[3] = (byte)par.Connectors[0].StartAddress;     //out 64
                    message[4] = 0;
                    message[5] = (byte)contCoils;  //de 8 en 8

                    byte responceQuantity;
                    if (contCoils % 8 == 0)
                        responceQuantity = (byte)(contCoils / 8);
                    else
                        responceQuantity = (byte)((contCoils / 8) + 1);

                    byte[] CRC = new byte[2];
                    GetCRC(message, ref CRC);
                    message[6] = CRC[0];
                    message[7] = CRC[1];

                    byte[] response = new byte[5 + responceQuantity];



                    bool result = ReadWritePort(message, response, par.Connectors[0].Station);
                    if (result)
                    {
                        int count = par.Connectors[0].StartAddress;
                        for (int i = 0; i < responceQuantity; i++)
                        {
                            bool[] resValue = SetValueResponseCoils(response, 3 + i);

                            for (int j = 0; j < resValue.Length; j++)
                            {

                                //var val = HelperGetConnectorAddress(par.Connectors, count);
                                //if (val != null) val.ConnectValue = resValue[j];
                                if (_ConnectorAddress.ContainsKey(count))
                                    _ConnectorAddress[count].ConnectValue = resValue[j];

                                count++;
                            }

                        }
                    }
                    else
                        Console.WriteLine("Error Read Coils");

                }
        }

        private void ReadRegisters(ConnectorsObjectHandled par)
        {
            for (int i = 0; i < par.Connectors.Length; i++)
            {



                byte[] message = new byte[8];
                message[0] = 1;
                message[1] = 0x04;
                message[2] = (byte)(par.Connectors[i].StartAddress >> 8);
                message[3] = (byte)par.Connectors[i].StartAddress;     //out 64
                message[4] = 0;
                message[5] = 1;  //de 8 en 8
                byte[] CRC = new byte[2];
                GetCRC(message, ref CRC);
                message[6] = CRC[0];
                message[7] = CRC[1];
                byte[] response = new byte[7];



                bool result = ReadWritePort(message, response, par.Connectors[i].Station);

                if (result)
                    par.Connectors[i].ConnectValue = SetValueResponseRegisters(response);
                else
                    Console.WriteLine("Error Read Registers");

            }
        }

        public object SetValueResponseRegisters(byte[] response)
        {
            int localValue = 0;
            localValue = response[3];
            localValue <<= 8;
            localValue += response[4];
            return localValue;
        }
        private void WriteCoil(ConnectorsObjectHandled par)
        {
            for (int i = 0; i < par.Connectors.Length; i++)
            {
                par.Connectors[i].Write(par.ObjectValue);
            }
        }

        private readonly object lckObj = new object();
        private bool ReadWritePort(byte[] message, byte[] response, ModbusStationRTU station)
        {

            try
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
                    Console.WriteLine("Error CRC");
                    return false;
                }

            }
            catch
            {
                Console.WriteLine("Error Catch");
                return false;
            }

        }

        private bool[] SetValueResponseCoils(byte[] response, int numValue)
        {
            short[] num = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512 };
            int value_read = response[numValue];
            List<short> res = new List<short>(); //= new short[response[2]];
            bool[] result = new bool[8];



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
            for (int i = 0; i < res.Count; i++)
            {
                result[res[i] - 1] = true;
            }

            return result;
        }
        private static ConnectorModbusRTU HelperGetConnectorAddress(ConnectorModbusRTU[] connectorOrdered, int address)
        {
            for (int i = 0; i < connectorOrdered.Length; i++)
            {
                if (connectorOrdered[i].StartAddress <= address)
                {
                    if (connectorOrdered[i].StartAddress == address)
                        return connectorOrdered[i];
                }
                else
                    return null;
            }
            return null;
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
    }
}