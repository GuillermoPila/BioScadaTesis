using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BioScadaServer.Alarms;
using BioScadaServer.BioScada;
using BioScadaServer.Drivers;
using BioScadaServer.Drivers.BoolDriver;
using BioScadaServer.Drivers.Modbus;
using BioScadaServer.Drivers.Modbus.ModbusTypeData;
using BioScadaServer.Variables;
using BioScadaServer.Drivers.IntDriver;
using System.Threading;

namespace BioScadaServer
{
    class Program
    {
        private static Random rand = new Random();
        //private SCADAServer server;
        //    private static ModbusDriver modDriver1;
        private static Variable[] vars;
        private static void AssignConnector(ConnectorModbusRTU Connector)
        {
            for (int i = 0; i < _All.Experiments.Count; i++)
            {
                List<Variable> vars = _All.Experiments[i].Variables.FindAll(x => x.Name == Connector.ConnectorName);
                for (int j = 0; j < vars.Count; j++)
                {
                    vars[j].Connector = new IConnector[] { Connector };
                }
            }
        }

        private static SerializeSystem _All;
        static void Main(string[] args)
        {

            #region Initialization

            Uri uri = new Uri(ConfigurationManager.AppSettings["addrHttp"]);
            Uri uri1 = new Uri(ConfigurationManager.AppSettings["addr"]);
            ServiceHost host = new ServiceHost(typeof(ServerService), uri1, uri);
            host.Open();
            Console.WriteLine("SCADA service listen on endpoint {0}", uri.ToString());
            Console.WriteLine("Press ENTER to stop Server service...");

            
            #region This Program
            //string Conn = ConfigurationManager.AppSettings["ConnectionBD"];
            //string File = ConfigurationManager.AppSettings["File"];
            //if (System.IO.File.Exists(File))
            //{
            //    _All = (SerializeSystem)Tools.BinarySerializer.Deserialize(File);
            //    string _PortName = ConfigurationManager.AppSettings["PortName"];
            //    DBLogger logg = new DBLogger(Conn);
            //    AlarmManeger manegerAlarm = new AlarmManeger();
            //    manegerAlarm.Alarms = _All.AlarmManeger.Alarms;
            //    manegerAlarm.Initialize();
            //    SerialComm serial1 = new SerialComm(_PortName, 9600, 8, Parity.None, StopBits.One);
            //    serial1.Open();
            //    ModbusStationRTU station = new ModbusStationRTU("Station1", 1, serial1, true);
            //    _All.Com = serial1;
            //    _All.Station = station;
            //    DriverModbusRTU _driv = DriverModbusRTU.GetInstance();
            //    for (int i = 0; i < _All.Connectors.Count; i++)
            //    {
            //        _All.Connectors[i].Driver = _driv;
            //        _All.Connectors[i].Station = station;
            //        AssignConnector(_All.Connectors[i]);
            //    }

            //    for (int i = 0; i < _All.Experiments.Count; i++)
            //    {
            //        for (int j = 0; j < _All.Experiments[i].Variables.Count; j++)
            //        {
            //            for (int k = 0; k < _All.Experiments[i].Variables[j].Receive.Length; k++)
            //            {
            //                if (_All.Experiments[i].Variables[j].Receive[k] is DBLogger)
            //                {
            //                    _All.Experiments[i].Variables[j].Receive[k] = logg;
            //                }

            //                if (_All.Experiments[i].Variables[j].Receive[k] is AlarmManeger)
            //                {
            //                    _All.Experiments[i].Variables[j].Receive[k] = manegerAlarm;
            //                }
            //            }
            //        }

            //    }

                
             
            //    Server server1 = Server.GetInstance();
            //    server1.Experiment = _All.Experiments;
            //    server1.Start();

            //    Notifier not = Notifier.GetInstance();
            //    not.Start();
            //    Console.WriteLine("Task finished! Press any key to finish...");
            //    Console.ReadLine();
            //    not.Dispose();
            //    server1.Dispose();
            //not.Dispose();
            //host.Abort();
            //host.Close();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //}
            //else
            //    Console.WriteLine("This file not exist");

            #endregion
              


            //DBLogger logger = new DBLogger(@"Data Source=guille\sqlexpress;Initial Catalog=ScadaLog;Integrated Security=True;Pooling=False");
            AlarmHi alarm1 = new AlarmHi();
            alarm1.Comment = "Apagar ventilador AlarmHi";
            alarm1.MaxValue = 150;
            alarm1.VariableName = "T0003";
            AlarmLo alarm2 = new AlarmLo();
            alarm2.Comment = "Temperatura baja AlarmLo";
            alarm2.MinValue = 20;
            alarm2.VariableName = "T0005";

            AlarmManeger managerAlarm = new AlarmManeger();
            managerAlarm.Alarms.Add(alarm1);
            managerAlarm.Alarms.Add(alarm2);
            managerAlarm.Initialize();
            //alarm2.Notifier += new Alarm.ActionAlarm(MiMetodo);
            //Alarm alarm3 = new Alarm(1, 600);
            //alarm3.Notifier += new Alarm.ActionAlarm(MiMetodo);
            #region Test

            IConnector[] conectorInt = new IConnector[]
                      {
                          new BoolConnector(){ConnectorName = "P00"},
                          new BoolConnector(){ConnectorName = "P01"},
                          new BoolConnector(){ConnectorName = "P02"},
                          new BoolConnector(){ConnectorName = "P03"},
                          new BoolConnector(){ConnectorName = "P04"},
                          new BoolConnector(){ConnectorName = "P05"},
                          new BoolConnector(){ConnectorName = "P06"},
                          new BoolConnector(){ConnectorName = "P07"},
                          new BoolConnector(){ConnectorName = "P08"},
                          new BoolConnector(){ConnectorName = "P09"},
                          new BoolConnector(){ConnectorName = "P10"},
                          new BoolConnector(){ConnectorName = "P11"},
                          new BoolConnector(){ConnectorName = "P40"},
                          new BoolConnector(){ConnectorName = "P41"},
                          new BoolConnector(){ConnectorName = "P42"},
                          new BoolConnector(){ConnectorName = "P43"},
                          new BoolConnector(){ConnectorName = "P44"},
                          new BoolConnector(){ConnectorName = "P45"},
                          new BoolConnector(){ConnectorName = "P46"},
                          new BoolConnector(){ConnectorName = "P47"},

                          new IntConnector(){ConnectorName = "T0003",Constant = false,MaxValue = 200,DefaultValue = 200},
                          new IntConnector(){ConnectorName = "T0005",Constant = false,MaxValue = 200,DefaultValue = 200},
                          new IntConnector(){ConnectorName = "D0000",Constant = true,MaxValue = 200,DefaultValue = 200},
                          new IntConnector(){ConnectorName = "D0002",Constant = true,MaxValue = 200,DefaultValue = 200}
                      };


            #endregion

            string PortName = ConfigurationManager.AppSettings["PortName"];

            Console.WriteLine(PortName);
            #region ModbusConnector
            SerialComm serial = new SerialComm(PortName, 9600, 8, Parity.None, StopBits.One);
            serial.Open();
            ModbusStationRTU station1 = new ModbusStationRTU("Station1", 1, serial, true);
            ConnectorModbusRTU[] conectorModbus = new ConnectorModbusRTU[]
                      {
                          new ConnectorModbusRTU("P00",station1,0,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P01",station1,1,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P02",station1,2,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P03",station1,3,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P04",station1,4,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P05",station1,5,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P06",station1,6,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P07",station1,7,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P08",station1,8,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P09",station1,9,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P10",station1,10,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P11",station1,11,new Discrete_Inputs()),

                          new ConnectorModbusRTU("P40",station1,64,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P41",station1,65,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P42",station1,66,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P43",station1,67,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P44",station1,68,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P45",station1,69,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P46",station1,70,new Discrete_Inputs()),
                          new ConnectorModbusRTU("P47",station1,71,new Discrete_Inputs()),
                          

                          new ConnectorModbusRTU("K0000",station1,12288,new Single_Register()),     //20
                          new ConnectorModbusRTU("K0001",station1,12289,new Single_Register()),

                          new ConnectorModbusRTU("D0000",station1,32768,new Single_Register()),   //22
                          new ConnectorModbusRTU("D0002",station1,32770,new Single_Register()),

                          new ConnectorModbusRTU("T0003",station1,20483,new Input_Registers()),  //24
                          new ConnectorModbusRTU("T0004",station1,20484,new Input_Registers()),
                          new ConnectorModbusRTU("T0005",station1,20485,new Input_Registers()),


                          new ConnectorModbusRTU("C0000",station1,24576,new Single_Register()),
                      };

            #endregion

            string varExample = ConfigurationManager.AppSettings["Example"];
            if (varExample.Equals("Example", StringComparison.OrdinalIgnoreCase))
                vars = new Variable[]
                       {
                           new Variable("P00",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[0]},Receive = new INotifierReceiverChange[]{/*logger*/}},
                           new Variable("P01",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[1]},Receive = new INotifierReceiverChange[]{/*logger*/}},
                           new Variable("P02",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[2]},Receive = new INotifierReceiverChange[]{/*logger*/}},
                           new Variable("P03",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[3]},Receive = new INotifierReceiverChange[]{/*logger*/}},
                           new Variable("P04",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[4]}},
                           new Variable("P05",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[5]}},
                           new Variable("P06",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[6]}},
                           new Variable("P07",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[7]}},
                           new Variable("P08",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[8]}},
                           new Variable("P09",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[9]}},
                           new Variable("P10",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[10]}},
                           new Variable("P11",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[11]}},
                        
                           new Variable("P40",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[12]},Receive = new INotifierReceiverChange[]{/*logger*/}},
                           new Variable("P41",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[13]},Receive = new INotifierReceiverChange[]{/*logger*/}},
                           new Variable("P42",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[14]}},
                           new Variable("P43",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[15]}},
                           new Variable("P44",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[16]}},
                           new Variable("P45",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[17]}},
                           new Variable("P46",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[18]}},
                           new Variable("P47",false){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[19]}},

                           new Variable("T0003",0){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[20]},Receive = new INotifierReceiverChange[]{/*logger,*/managerAlarm}},
                           new Variable("T0005",0){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[21]},Receive = new INotifierReceiverChange[]{/*logger,*/managerAlarm}},
                           new Variable("D0000",0){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[22]},Receive = new INotifierReceiverChange[]{/*logger*/}},
                           new Variable("D0002",0){RequestPeriod = 2,Connector = new IConnector[]{conectorInt[23]},Receive = new INotifierReceiverChange[]{/*logger*/}},
                           new Variable("Vol1",0){RequestPeriod = 2},
                           new Variable("Vol2",0){RequestPeriod = 2},
                       };
            else
                vars = new Variable[]
                       {
                           new Variable("P00",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[0]}},
                           new Variable("P01",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[1]}},
                           new Variable("P02",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[2]}},
                           new Variable("P03",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[3]}},
                           new Variable("P04",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[4]}},
                           new Variable("P05",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[5]}},
                           new Variable("P06",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[6]}},
                           new Variable("P07",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[7]}},
                           new Variable("P08",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[8]}},
                           new Variable("P09",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[9]}},
                           new Variable("P10",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[10]}},
                           new Variable("P11",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[11]}},
                        
                           new Variable("P40",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[12]}},
                           new Variable("P41",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[13]}},
                           new Variable("P42",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[14]}},
                           new Variable("P43",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[15]}},
                           new Variable("P44",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[16]}},
                           new Variable("P45",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[17]}},
                           new Variable("P46",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[18]}},
                           new Variable("P47",false){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[19]}},
                         
                       //    new Variable("K0000",0){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[20]}},
                       //    new Variable("K0001",0){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[21]}},

                           new Variable("D0000",0){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[22]}},
                           new Variable("D0002",0){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[23]}},

                           new Variable("T0003",0){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[24]}},
                           new Variable("T0005",0){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[26]}},
                           
                         //  new Variable("C0000",0){RequestPeriod = 2,Connector = new IConnector[]{conectorModbus[25]}}

                       };






            Experiment _Experiment = new Experiment(vars) { Name = "Exp1" };

            Server server = Server.GetInstance();
            server.Experiment.Add(_Experiment);
            server.Start();

            /* Notifier notifier = Notifier.GetInstance();
             notifier.Start();*/



            #endregion

            //SCADAServer server = new SCADAServer(vars, inputDrivers, outputDrivers);
            //DBLogger logger = new DBLogger(@"Data Source=guille\sqlexpress;Initial Catalog=SCADALog;Integrated Security=True;Pooling=False", vars);
            //SCADANotifier notifier = new SCADANotifier() { NotificationReceiver = logger };
            //notifier.Start();
            //server.Start(notifier);
            server.Start_Experiment("Exp1");

            //Console.WriteLine(vars[0].Value.ToString());
            Console.WriteLine("Task finished! Press any key to finish...");
            //Console.ReadLine();
            //server.Start_Experiment("Exp1");

            Notifier notif = Notifier.GetInstance();
            notif.Start();

            Console.ReadLine();

            notif.Stop();

            server.Dispose();
            //notifier.Dispose();
            host.Abort();
            host.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }


        public static void MiMetodo(object sender, ChangeNotification var)
        {
            Console.WriteLine("Alarm!!!!!  " + var.Item.Name + "  " + var.Item.NewValue);
        }


    }
}
