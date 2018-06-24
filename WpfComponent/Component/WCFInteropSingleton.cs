using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Media;
using System.Threading;
using System.Windows;
using BioScadaServer.Variables;
using BioScadaScript;
using WpfComponent.Common;


namespace WpfComponent.Component
{
    public class WCFInteropSingleton : FrameworkElement
    {
        public string LogUser { get; set; }
        public string LogPassword { get; set; }

        private List<Variable> calculatedVariables;
        public List<Variable> CalculatedVariables { get { return calculatedVariables; } set { calculatedVariables = value; } }

        private List<ScriptInAppDomain> scripts;
        public List<ScriptInAppDomain> Scripts { get { return scripts; } set { scripts = value; } }

        private Dictionary<string, Dictionary<string, object>> experimentVars;
        public Dictionary<string, Dictionary<string, object>> ExperimentVars
        {
            get { return experimentVars; }
            set { experimentVars = value; }
        }

        private int refreshFrequency;
        public int RefreshFrequency
        {
            get { return refreshFrequency; }
            set { refreshFrequency = value; }
        }

        private ServerClient client;
        public ServerClient Client
        {
            get { return client; }
            set { client = value; }
        }

        private static WCFInteropSingleton obj = null;
        private WCFInteropSingleton()
        {
            calculatedVariables = new List<Variable>();
            scripts = new List<ScriptInAppDomain>();
            experimentVars = new Dictionary<string, Dictionary<string, object>>();
            timer = new Timer(OnTiming, this, Timeout.Infinite, Timeout.Infinite);
        }

        public static WCFInteropSingleton GetInstance()
        {
            lock (lckObj)
            {
                if (obj == null)
                {
                    obj = new WCFInteropSingleton();
                }
                return obj;
            }
        }


        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Typeface typeface = new Typeface("Times New Roman");
            FormattedText text = new FormattedText("WCFInterop", CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, typeface, 12, Brushes.Black);

            drawingContext.DrawText(text, new Point(0, 0));
        }

        public void Initialize()
        {
            if (client != null)
                experimentVars = client.GetAllVariableExperiment();
        }

        private bool active = false;
        public bool Active
        {
            get { return active; }
            set
            {
                if (active == value)
                    return;
                active = value;
                if (active)
                {
                    //foreach(KeyValuePair<string, Dictionary<string,object>> pair in experimentVars)
                    //{
                    //    Start(pair.Key);
                    //}
                    for (int i = 0; i < scripts.Count; i++)
                    {
                        scripts[i].CompileInAppDomain(experimentVars);
                    }
                    timer.Change(0, Timeout.Infinite);
                }
                else
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        private Timer timer;
        private static object lckObj = new object();

        private static void OnTiming(object state)
        {
            WCFInteropSingleton container = state as WCFInteropSingleton;

            Variable[] vars;
            try
            {
                lock (lckObj)
                {
                    foreach (KeyValuePair<string, Dictionary<string, object>> pair in container.experimentVars)
                    {
                        vars = container.client.GetVariableExperiment(pair.Key);
                        if (vars.Length > 0)
                        {
                            for (int i = 0; i < vars.Length; i++)
                            {
                                pair.Value[vars[i].Name] = vars[i].Value;
                            }
                        }
                    }
                    for (int i = 0; i < container.scripts.Count; i++)
                    {
                        container.scripts[i].Execute(ref container.experimentVars);
                    }
                }
            }
            catch (Exception e)
            {
                container.Login();
            }
            container.timer.Change(container.RefreshFrequency, Timeout.Infinite);
        }

        public Dictionary<string, object> GetVariableExperiment(string NameExperiment)
        {
            //Variable[] vars = new Variable[0];
            try
            {
                lock (lckObj)
                    return experimentVars[NameExperiment];
            }
            catch (Exception e)
            {
                // MessageBox.Show("Se jodio GetVariableExperiment---------");
                return new Dictionary<string, object>();
            }
            //return vars;
        }

        public void SetValueVariable(Dictionary<string, object> Objects, string experiment)
        {
            try
            {
                bool b;
                foreach (KeyValuePair<string, object> obj in Objects)
                {
                    lock (lckObj)
                        b = client.SetValueVariable(obj.Key, obj.Value, experiment);
                }
            }
            catch
            {
                // MessageBox.Show("Se jodio SetValueVariable");
                Login();
            }

        }

        public void Start(string experiment)
        {
            try
            {
                lock (lckObj)
                    client.Start_Experiment(experiment);
            }
            catch (Exception e)
            {
                Login();
                //Start(experiment);
            }
        }

        public void Stop(string experiment)
        {
            try
            {
                lock (lckObj)
                    client.Stop_Experiment(experiment);
            }
            catch
            {
                Login();
                // Stop(experiment);
            }
        }

        public int Login()
        {
            try
            {
                return client.Login(LogUser, LogPassword);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }

        public void Leave()
        {
            try
            {
                client.Leave();
            }
            catch { }
        }
    }
}
