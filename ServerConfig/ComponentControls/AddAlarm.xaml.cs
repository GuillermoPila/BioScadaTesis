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
    /// Interaction logic for AddAlarm.xaml
    /// </summary>
    public partial class AddAlarm : UserControl
    {
        public AddAlarm()
        {
            InitializeComponent();
            txtMinValue.IsEnabled = false;
        }

        public static readonly RoutedEvent AddButtonClickEvent = EventManager.RegisterRoutedEvent(
            "AddButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddAlarm));

        public event RoutedEventHandler AddButtonClick
        {
            add { AddHandler(AddButtonClickEvent, value); }
            remove { RemoveHandler(AddButtonClickEvent, value); }
        }

        public static readonly RoutedEvent CloseClickEvent = EventManager.RegisterRoutedEvent(
            "CloseClickEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddAlarm));


        public event RoutedEventHandler CloseClick
        {
            add { AddHandler(CloseClickEvent, value); }
            remove { RemoveHandler(CloseClickEvent, value); }
        }

        private string variableName;
        public string VariableName { get { return variableName; } set { variableName = value; } }

        private BioScadaServer.Alarms.Alarm typeAlarm = new BioScadaServer.Alarms.AlarmHi();
        public BioScadaServer.Alarms.Alarm TypeAlarm { get { return typeAlarm; } set { typeAlarm = value; } }

        private string comment = "";
        public string Comment { get { return comment; } set { comment = value; } }

        private double minValueAlarm = 0;
        public double MinValueAlarm { get { return minValueAlarm; } set { minValueAlarm = value; } }

        private double maxValueAlarm = 100;
        public double MaxValueAlarm { get { return maxValueAlarm; } set { maxValueAlarm = value; } }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string aux = "";
            if ((sender as RadioButton).Content != null)
                aux = (sender as RadioButton).Content.ToString();
            switch (aux)
            {
                case "AlarmHi":
                    {
                        typeAlarm = new BioScadaServer.Alarms.AlarmHi();
                        lblMin.IsEnabled = false;
                        txtMinValue.IsEnabled = false;
                        lblMax.IsEnabled = true;
                        txtMaxValue.IsEnabled = true;
                        break;
                    }
                case "AlarmLo":
                    {
                        typeAlarm = new BioScadaServer.Alarms.AlarmLo();
                        lblMin.IsEnabled = true;
                        txtMinValue.IsEnabled = true;
                        lblMax.IsEnabled = false;
                        txtMaxValue.IsEnabled = false;
                        break;
                    }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                try
                {
                    variableName = txtName.Text;
                    comment = txtComment.Text;

                    if (txtMinValue.IsEnabled && txtMinValue.Text != "")
                        minValueAlarm = Convert.ToDouble(txtMinValue.Text);
                    if (txtMaxValue.IsEnabled && txtMaxValue.Text != "")
                        maxValueAlarm = Convert.ToDouble(txtMaxValue.Text);
                    
                    txtMinValue.Clear();
                    txtMaxValue.Clear();
                    txtName.Clear();
                    txtComment.Clear();
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
