using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using BioScadaServer.Variables;
using WpfComponent.Component;
using BioScadaScript;


namespace BioScadaClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //WCFInteropSingleton Interop = WCFInteropSingleton.GetInstance();
            //Interop.ExperimentVars.Add("Exp1", new Dictionary<string, object>());
            //string endPoint = ConfigurationManager.AppSettings["EndPoint"];
            //ServerClient c = new ServerClient(endPoint);
            //c.Login("admin", "admin");
            //Interop.Client = c;
            //Interop.RefreshFrequency = 100;
            //Interop.Active = true;
            base.OnStartup(e);
        }
    }
}
