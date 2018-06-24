using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BioScadaClient
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public static readonly RoutedEvent LoginButtonClickEvent = EventManager.RegisterRoutedEvent(
            "LoginButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LoginControl));

        public event RoutedEventHandler LoginButtonClick
        {
            add { AddHandler(LoginButtonClickEvent, value); }
            remove { RemoveHandler(LoginButtonClickEvent, value); }
        }

        private string endPointT = "HTTP";
        public string EndPointT { get { return endPointT; } set { endPointT = value; } }

        public LoginControl()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private string userName;
        public string UserName { get { return userName; } set { userName = value; } }

        private string password;
        public string Password { get { return password; } set { password = value; } }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).Content != null)
                EndPointT = (sender as RadioButton).Content.ToString();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                userName = txtName.Text;
                password = txtPass.Password;
                txtName.Clear();
                txtPass.Clear();
                RaiseEvent(new RoutedEventArgs(LoginButtonClickEvent));
            }
            else
                MessageBox.Show("Debe especificar al menos un nombre de usuario");
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                LoginButton_Click(sender, new RoutedEventArgs());
        }
    }
}
