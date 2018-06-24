using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfComponent.Common;
using System.Threading;
using WpfComponent.Component;

namespace BioScadaClient
{
    /// <summary>
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        private ServerClient c;
        public Window4()
        {
            InitializeComponent();
            string endPoint = ConfigurationManager.AppSettings["EndPoint"];
            c = new ServerClient(endPoint);
            c.Login("guille", "guille");
            c.Start_Experiment("Exp1");
            timer = new Timer(OnAlarm, this, 0, 200);
        }

        Timer timer;

        private static void OnAlarm(object state)
        {
            string[] aux = (state as Window4).c.GetAlarms();
            (state as Window4).txtViewer.AddAlarm(aux);
        }

       


    }
}
