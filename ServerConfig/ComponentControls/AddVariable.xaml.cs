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
    /// Interaction logic for AddVariable.xaml
    /// </summary>
    public partial class AddVariable : UserControl
    {
        public AddVariable()
        {
            InitializeComponent();
        }

        public static readonly RoutedEvent AddButtonClickEvent = EventManager.RegisterRoutedEvent(
            "AddButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddVariable));

        public event RoutedEventHandler AddButtonClick
        {
            add { AddHandler(AddButtonClickEvent, value); }
            remove { RemoveHandler(AddButtonClickEvent, value); }
        }

        public static readonly RoutedEvent CloseClickEvent = EventManager.RegisterRoutedEvent(
            "CloseClickEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddVariable));


        public event RoutedEventHandler CloseClick
        {
            add { AddHandler(CloseClickEvent, value); }
            remove { RemoveHandler(CloseClickEvent, value); }
        }

        private string variableName;
        public string VariableName { get { return variableName; } set { variableName = value; } }

        private object variableInitialValue = 0;
        public object VariableInitialValue { get { return variableInitialValue; } set { variableInitialValue = value; } }

        private bool? variableLogg = false;
        public bool? VariableLogg { get { return variableLogg; } set { variableLogg = value; } }

        private bool? variableAlarm = false;
        public bool? VariableAlarm { get { return variableAlarm; } set { variableAlarm = value; } }

        private int requestPeriod = 1;
        public int RequestPeriod { get { return requestPeriod; } set { requestPeriod = value; } }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string aux = "";
            if ((sender as RadioButton).Content != null)
                aux = (sender as RadioButton).Content.ToString();
            switch (aux)
            {
                case "Int":
                    {
                        variableInitialValue = 0;
                        break;
                    }
                case "Double":
                    {
                        variableInitialValue = 0.0;
                        break;
                    }
                case "Bool":
                    {
                        variableInitialValue = false;
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
                    int aux = Convert.ToInt32(txtTime.Text);
                    if (aux >= 50)
                        requestPeriod = aux / 50;
                    else
                        requestPeriod = 1;
                    variableLogg = LogBD.IsChecked;
                    variableAlarm = AlarmContaint.IsChecked;
                    txtName.Clear();
                    LogBD.IsChecked = false;
                    AlarmContaint.IsChecked = false;
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
