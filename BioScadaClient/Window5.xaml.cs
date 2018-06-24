using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BioScadaClient
{
    /// <summary>
    /// Interaction logic for Window5.xaml
    /// </summary>
    public partial class Window5 : Window
    {
        public Window5()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string fich = ConfigurationManager.AppSettings["Fichero"];
            using (FileStream fs = new FileStream(@fich, FileMode.Open, FileAccess.Read))
            {
                ResourceDictionary resources = (ResourceDictionary)XamlReader.Load(fs);
                Application.Current.Resources.Add("Container", resources["Container"]);
            }
            ContainerIn.Children.Add((UIElement) Application.Current.Resources["Container"]);
        }
    }
}
