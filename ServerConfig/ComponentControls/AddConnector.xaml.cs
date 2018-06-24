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

namespace ServerConfig.ComponentControls
{
    /// <summary>
    /// Interaction logic for AddConnector.xaml
    /// </summary>
    public partial class AddConnector : UserControl
    {
        public AddConnector()
        {
            InitializeComponent();
        }

        public static readonly RoutedEvent AddButtonClickEvent = EventManager.RegisterRoutedEvent(
            "AddButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddConnector));

        public event RoutedEventHandler AddButtonClick
        {
            add { AddHandler(AddButtonClickEvent, value); }
            remove { RemoveHandler(AddButtonClickEvent, value); }
        }

        public static readonly RoutedEvent CloseClickEvent = EventManager.RegisterRoutedEvent(
            "CloseClickEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddConnector));


        public event RoutedEventHandler CloseClick
        {
            add { AddHandler(CloseClickEvent, value); }
            remove { RemoveHandler(CloseClickEvent, value); }
        }

        private string connectorName;
        public string ConnectorName { get { return connectorName; } set { connectorName = value; } }

        private BioScadaServer.Drivers.Modbus.ModbusTypeData.TypeData typeValue = new BioScadaServer.Drivers.Modbus.ModbusTypeData.Discrete_Inputs();
        public BioScadaServer.Drivers.Modbus.ModbusTypeData.TypeData TypeValue { get { return typeValue; } set { typeValue = value; } }

        private int address = 0;
        public int Address { get { return address; } set { address = value; } }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string aux = "";
            if ((sender as RadioButton).Content != null)
                aux = (sender as RadioButton).Content.ToString();
            switch (aux)
            {
                case "Discrete_Inputs":
                    {
                        typeValue = new BioScadaServer.Drivers.Modbus.ModbusTypeData.Discrete_Inputs();
                        break;
                    }
                case "Single_Register":
                    {
                        typeValue = new BioScadaServer.Drivers.Modbus.ModbusTypeData.Single_Register();
                        break;
                    }
                case "Input_Registers":
                    {
                        typeValue = new BioScadaServer.Drivers.Modbus.ModbusTypeData.Input_Registers();
                        break;
                    }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text) && !string.IsNullOrEmpty(txtAddress.Text))
            {
                try
                {
                    connectorName = txtName.Text;
                    address = Convert.ToInt32(txtAddress.Text);

                    txtName.Clear();
                    txtAddress.Clear();
                    RaiseEvent(new RoutedEventArgs(AddButtonClickEvent));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CloseClickEvent));
        }
    }
}
