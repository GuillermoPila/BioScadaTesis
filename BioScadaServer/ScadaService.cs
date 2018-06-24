using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using BioScadaServer.Variables;
using BioScadaServer.BioScada;


namespace BioScadaServer
{

    [ServiceContract(Namespace = "BioScadaServer", SessionMode = SessionMode.Required)]
    interface IServer
    {
        [OperationContract(IsInitiating = true)]
        int Login(string name, string pass);

        [OperationContract(IsTerminating = true)]
        void Leave();

        [OperationContract(IsInitiating = false)]
        bool Start_Experiment(string name);
        [OperationContract(IsInitiating = false)]
        bool Stop_Experiment(string name);

        [OperationContract(IsInitiating = false)]
        [ServiceKnownType(typeof(Variable))]
        [ServiceKnownType(typeof(object[]))]
        Variable[] GetVariableExperiment(string name);

        [OperationContract(IsInitiating = false)]
        Dictionary<string, Dictionary<string, object>> GetAllVariableExperiment();

        [OperationContract(IsInitiating = false)]
        [ServiceKnownType(typeof(object[]))]
        bool SetValueVariable(string VariableName, object VariableValue, string ExperimentName);

        [OperationContract(IsInitiating = false)]
        string[] GetAlarms();

    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class ServerService : IServer
    {
        static Server server = Server.GetInstance();
        //static SQLQuery query = new SQLQuery(@"Data Source=guille\sqlexpress;Initial Catalog=SCADALog;Integrated Security=True;Pooling=False");

        static Dictionary<string, User> userRol = new Dictionary<string, User>();

        public int Login(string name, string pass)
        {
            Console.WriteLine("fffffffffffffffff");
            //MD5 hash = MD5.Create();
            //byte[] data = hash.ComputeHash(Encoding.Default.GetBytes(pass));
            //StringBuilder sBuilder = new StringBuilder();

            //for (int i = 0; i < data.Length; i++)
            //{
            //    sBuilder.Append(data[i].ToString("x2"));
            //}
            //string passw = sBuilder.ToString();
            //DataTable table =
            //    query.ExecuteData(string.Format("SELECT * FROM Usuario WHERE (pass = '{0}') AND ([user] = '{1}')", passw, name));
            //Console.WriteLine(name + "    " + passw);
            //int result = -1;
            //if (table.Rows.Count > 0)
            //{
            //    result = (int)table.Rows[0]["rol"];
            //    User user = new User();
            //    user.user = (string)table.Rows[0]["user"];
            //    user.pass = (string)table.Rows[0]["pass"];
            //    user.rol = (int)table.Rows[0]["rol"];
            //    if (!userRol.ContainsKey(OperationContext.Current.SessionId))
            //        userRol.Add(OperationContext.Current.SessionId, user);
            //}
            //return result;
            return 5;
        }

        public void Leave()
        {
            Console.WriteLine("Se fue  " + OperationContext.Current.SessionId);
        }

        public bool Start_Experiment(string name)
        {
           // Console.WriteLine("Start  " + name);
            return server.Start_Experiment(name);
        }

        public bool Stop_Experiment(string name)
        {
            return server.Stop_Experiment(name);
        }

        private Dictionary<string, List<Variable>> LastVarsExperiment = new Dictionary<string, List<Variable>>();
        public Variable[] GetVariableExperiment(string nameExperiment)
        {


            List<Variable> AllVars = server.GetVariableExperiment(nameExperiment);

            if (!LastVarsExperiment.ContainsKey(nameExperiment))
            {
                List<Variable> _Temp = new List<Variable>();
                for (int i = 0; i < AllVars.Count; i++)
                {
                    _Temp.Add(new Variable(AllVars[i].Name, AllVars[i].Value));
                }
                LastVarsExperiment.Add(nameExperiment, _Temp);
            }
            List<Variable> Result = new List<Variable>();

            List<Variable> aux = LastVarsExperiment[nameExperiment];
            for (int i = 0; i < AllVars.Count; i++)
            {
                if (aux.Count == AllVars.Count)
                    if (!aux[i].Value.Equals(AllVars[i].Value))
                    {
                        //Console.WriteLine(AllVars[i].Value + "   " + LastVarsExperiment["Exp1"][i].Value);
                        Result.Add(new Variable(AllVars[i].Name, AllVars[i].Value));
                        LastVarsExperiment[nameExperiment][i].Value = AllVars[i].Value;
                    }
            }
            return Result.ToArray();
        }

        public Dictionary<string, Dictionary<string, object>> GetAllVariableExperiment()
        {
            try
            {
                return server.GetAllVariableExperiment();
            }
            catch
            {
                //O devolver el error
                return new Dictionary<string, Dictionary<string, object>>();
            }
        }

        public bool SetValueVariable(string VariableName, object VariableValue, string ExperimentName)
        {

            try
            {
                if (userRol[OperationContext.Current.SessionId].rol > 3)
                    return server.SetValueVariable(VariableName, VariableValue, ExperimentName);
                else
                    return false;
            }
            catch
            {
                return false;
            }

        }

        List<string> LastAlarms = new List<string>();
        public string[] GetAlarms()
        {
            List<string> aux = server.Alarm;
            List<string> aux1 = new List<string>();

            foreach (var alarmNew in aux.Except(LastAlarms))
            {
                aux1.Add(alarmNew);
            }
            for (int i = 0; i < aux1.Count; i++)
            {
                LastAlarms.Add(aux1[i]);
            }
            return aux1.ToArray();
        }
    }

    public struct User
    {
        public string user;
        public string pass;
        public int rol;
    }
}

