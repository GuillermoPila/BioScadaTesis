using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BioScadaServer.Variables;
using WpfComponent.Common;
using WpfComponent.Component;
using System.Configuration;


namespace BioScadaClient
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        public Window1()
        {
            InitializeComponent();
            LoginCtrl.LoginButtonClick += LoginControl_LoginButtonClick;
        }

        private WCFInteropSingleton Interop;
        private ServerClient _Client;
        private void LoginControl_LoginButtonClick(object sender, RoutedEventArgs e)
        {
            Interop = WCFInteropSingleton.GetInstance();
            _Client = new ServerClient(LoginCtrl.EndPointT);
            Interop.Client = _Client;
            Interop.LogUser = LoginCtrl.UserName;
            Interop.LogPassword = LoginCtrl.Password;
            int enter = Interop.Login();
            if (enter > 0)
            {
                Interop.Initialize();
                Window2 win2 = new Window2();
                win2.Interop = Interop;
                win2.InitializeWin();
                win2.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Nombre de usuario o contraseña incorrecto", "Autenticación", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }

    }
}
