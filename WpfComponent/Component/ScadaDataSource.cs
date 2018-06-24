using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using BioScadaServer.Variables;
using WpfComponent.Common;


namespace WpfComponent.Component
{
    public class ScadaDataSource : Common.ObjectTransmitter, IObjectReceiver
    {
        public int RefreshFrequency { get; set; }


        private WCFInteropSingleton Interop;
        //private void Load(object sender, RoutedEventArgs e)
        //{

        //}

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
                   // Interop.Start(experimentName);
                    timer.Change(0, Timeout.Infinite);
                }
                else
                {
                    Interop.Stop(experimentName);
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        private Timer timer;
        private static void OnTiming(object state)
        {
            ScadaDataSource container = (state as ScadaDataSource);
            WCFInteropSingleton _Interop = container.Interop;

            Dictionary<string, object> variables;
            try
            {
                variables = _Interop.GetVariableExperiment(container.experimentName);
                if (variables.Count > 0)
                {
                    container.propertyChange.Receive(variables);
                    //container.Transmit(variables);
                }

            }
            catch (Exception e)
            {
                //MessageBox.Show("Se jodio OnTiming");
                //_Interop.Login(container.LogUser, container.LogPassword);
            }
            container.timer.Change(container.RefreshFrequency, Timeout.Infinite);
        }

        private ScadaDataPropertyChange propertyChange;
        public ScadaDataPropertyChange PropertyChange
        {
            get { return propertyChange; }
            set { propertyChange = value; }
        }

        private string experimentName;
        public string ExperimentName { get { return experimentName; } set { experimentName = value; } }

        public ScadaDataSource(string experimentName)
        {
            this.experimentName = experimentName;
            propertyChange = new ScadaDataPropertyChange();
            Interop = WCFInteropSingleton.GetInstance();
            timer = new Timer(OnTiming, this, Timeout.Infinite, Timeout.Infinite);
            // Loaded += Load;
        }

        public ScadaDataSource()
        {
            Interop = WCFInteropSingleton.GetInstance();
            timer = new Timer(OnTiming, this, Timeout.Infinite, Timeout.Infinite);
            //            Loaded += Load;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Typeface typeface = new Typeface("Times New Roman");
            FormattedText text = new FormattedText("DataSource", CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, typeface, 12, Brushes.Black);

            drawingContext.DrawText(text, new Point(0, 0));
        }

        public void Receive(Dictionary<string, object> Objects)
        {
            Interop.SetValueVariable(Objects, experimentName);
        }
    }
}
