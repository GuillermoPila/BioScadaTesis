using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using BioScadaScript;

namespace WpfComponent.Component
{
    public class ScadaDataProvider
    {
        private Dictionary<string, Dictionary<string, object>> _expreimentVars;
        public ScadaDataProvider()
        {
            Interop = WCFInteropSingleton.GetInstance();
            _expreimentVars = Interop.ExperimentVars;
            foreach (KeyValuePair<string, Dictionary<string, object>> experiment in _expreimentVars)
            {
                object o = FactoryDynamicObjectExperimentBinding.CreateDynamicObjectExperimentBinding(experiment.Key,
                                                                                                      experiment.Value);
                if (o == null)
                    break;
                ls.Add(o);
                Type _type = o.GetType();
                PropertyInfo[] prop = _type.GetProperties();

                foreach (KeyValuePair<string, object> variable in experiment.Value)
                {
                    for (int i = 0; i < prop.Length; i++)
                    {
                        if (prop[i].Name == variable.Key)
                        {
                            prop[i].SetValue(o, variable.Value, null);
                            break;
                        }
                    }
                }
            }
            timer = new Timer(OnTimming, this, 100, 100);
            refreshFrequency = 100;
            //var asd = new {Name="P00", Value=true};
        }
        ObservableCollection<object> ls = new ObservableCollection<object>();
        private Timer timer;
        private WCFInteropSingleton Interop;
        private static void OnTimming(object state)
        {
            ScadaDataProvider container = (state as ScadaDataProvider);
            WCFInteropSingleton _Interop = container.Interop;


            try
            {
                var experiment = _Interop.ExperimentVars;
                if (experiment.Count > 0)
                {
                    for (int i = 0; i < container.ls.Count; i++)
                    {
                        Type _type = container.ls[i].GetType();
                        PropertyInfo[] prop = _type.GetProperties();
                        foreach (KeyValuePair<string, object> variable in experiment[_type.Name])
                        {
                            for (int j = 0; j < prop.Length; j++)
                            {
                                if (prop[j].Name == variable.Key)
                                {
                                    prop[j].SetValue(container.ls[i], variable.Value, null);
                                    break;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                //MessageBox.Show("Se jodio OnTiming");
                //_Interop.Login(container.LogUser, container.LogPassword);
            }
            container.timer.Change(container.RefreshFrequency, Timeout.Infinite);
        }

        private int refreshFrequency;
        public int RefreshFrequency
        {
            get { return refreshFrequency; }
            set { refreshFrequency = value; }
        }

        public object GetExperiment(string exp)
        {
            return ls.First(x => x.GetType().Name == exp);
        }
    }
}
