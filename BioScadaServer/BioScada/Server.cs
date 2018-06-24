using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Lifetime;
using System.ServiceModel;
using System.Text;
using System.Threading;
using BioScadaServer.Drivers;
using BioScadaServer.Drivers.IntDriver;
using BioScadaServer.Drivers.Modbus;
using BioScadaServer.Variables;

namespace BioScadaServer.BioScada
{

    public class Server : IDisposable
    {
        public static Server GetInstance()
        {
            if (_obj == null)
                _obj = new Server();
            return _obj;
        }

        private readonly ConnectionManager connectManager = ConnectionManager.GetInstance();
        //public ConnectionManager ma { get { return connectManager; } set { connectManager = value; } }

        private List<Experiment> experiment;
        public List<Experiment> Experiment { get { return experiment; } set { experiment = value; } }

        private List<string> alarm;
        public List<string> Alarm { get { return alarm; } set { alarm = value; } }

        public void ActionAlarmNotifier(object sender, ChangeNotification var)
        {
            try
            {
                Alarms.Alarm alarmOcurr = (sender as Alarms.Alarm);
              
                alarm.Add(alarmOcurr.Comment + "     " + alarmOcurr.When + "   " + alarmOcurr.VariableName + "     " + var.Item.NewValue);
                if (alarm.Count > 100)
                    alarm.RemoveAt(0);
            }
            catch { }

            //Console.WriteLine("Alarm!!!!!  " + var.Item.Name + "  " + var.Item.NewValue);
        }

        public void Start()
        {
            connectManager.Experiments = experiment.ToArray();
            connectManager.InitializeDrivers();
            CreateArrayVariable();
            ticksToRead = new int[_Variables.Length];
            for (int i = 0; i < _Variables.Length; i++)
            {
                ticksToRead[i] = 1;
            }
            timer = new Timer(CicloRequest, null, 0, 50);
        }

        public void Stop()
        {
            //timer.Change(long.MaxValue, long.MaxValue);
            timer.Dispose();
            connectManager.Fin();
        }

        // List<TimeSpan> myTemplist = new List<TimeSpan>();
        #region Private
        private void CicloRequest(object obj)
        {
            //DateTime temp = DateTime.Now;
            for (int i = 0; i < _Variables.Length; i++)
            {
                if (_Variables[i].IsEnabled)
                    if (ticksToRead[i]-- == 0)
                    {
                        ticksToRead[i] = _Variables[i].RequestPeriod;
                        if (_Variables[i].Connector != null)
                            for (int j = 0; j < _Variables[i].Connector.Length; j++)
                            {
                                _Variables[i].Connector[j].Driver.ServerRequestStopDriver();
                                //Console.Write("-");
                                _Variables[i].Value = _Variables[i].Connector[j].ConnectValue;
                                _Variables[i].Connector[j].Driver.ServerRequestStartDriver();
                                 //Console.WriteLine(_Variables[i].Name + "   " + _Variables[i].Value);
                            }
                    }
            }
            //myTemplist.Add(DateTime.Now - temp);
        }

        private Variable[] _Variables;
        private void AddVariable(Variable var)
        {
            int length = _Variables.Length;
            Array.Resize(ref _Variables, length + 1);
            _Variables.SetValue(var, length);
        }
        private void CreateArrayVariable()
        {
            for (int i = 0; i < experiment.Count; i++)
            {
                for (int j = 0; j < experiment[i].Variables.Count; j++)
                {
                    AddVariable(experiment[i].Variables[j]);
                }

            }
        }

        private Server()
        {
            _Variables = new Variable[0];
            experiment = new List<Experiment>();
            alarm = new List<string>();
        }

        #endregion

        private int[] ticksToRead;

        Timer timer;
        private static Server _obj = null;



        public void Dispose()
        {
            Stop();
        }

        private static object objLock = new object();

        public bool Start_Experiment(string name)
        {
            try
            {
                BioScada.Experiment e = experiment.Single(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                //lock (objLock)
                e.Start();
            }
            catch
            {
                Console.WriteLine("Foooooooooooooooooooooooollllllllllllllllllllllll");
                return false;
            }

            return true;
        }

        public bool Stop_Experiment(string name)
        {
            try
            {
                BioScada.Experiment e = experiment.Single(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                //lock (objLock)
                e.Stop();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public List<Variable> GetVariableExperiment(string name)
        {
            BioScada.Experiment e;
            try
            {
                e = experiment.Single(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            }
            catch
            {
                return new List<Variable>();
            }
            return e.Variables.FindAll(x => x.IsEnabled);
        }

        public Dictionary<string, Dictionary<string, object>> GetAllVariableExperiment()
        {
            Dictionary<string, Dictionary<string, object>> result = new Dictionary<string, Dictionary<string, object>>();
            try
            {
                for (int i = 0; i < experiment.Count; i++)
                {
                    result.Add(experiment[i].Name, experiment[i].Variables.ToDictionary(x => x.Name, z => z.Value));
                }
            }
            catch
            {

            }

            return result;
        }

        public bool SetValueVariable(string VariableName, object VariableValue, string ExperimentName)
        {
            bool res = false;
            //bool newVAlue = true;
            //if (variable.Value == null)
            //    newVAlue = false;
            BioScada.Experiment e = experiment.Find(x => x.Name == ExperimentName);
            Variable val = e.Variables.Find(x => x.Name == VariableName);

            if (val != null)
            {
                if (!val.Value.Equals(VariableValue))
                {
                    //Console.WriteLine("Entrooooooooooooooooooooooooooo");
                    //res = val.RequestValueChange(variable.Value);
                    res = val.RequestValueChange(VariableValue);
                }
                else
                    return true;
            }
            else
                return false;

            return res;

        }

        /*  public int GetValueVariable(string name)
          {

              int x = 0;

              lock (objLock)
              {
                  x = checkIfVarExists(name);
              }

              Console.WriteLine("LLegueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
              return x;
          }

          private int checkIfVarExists(string name)
          {
              foreach (Variable p in variables)
              {
                  if (p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                  {
                      return (int)p.Value;
                  }
              }
              return 0;
          }*/
    }
}
