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
    /// Interaction logic for AddExperiment.xaml
    /// </summary>
    public partial class AddExperiment : UserControl
    {
        public static readonly RoutedEvent AddButtonClickEvent = EventManager.RegisterRoutedEvent(
            "AddButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddExperiment));

        public event RoutedEventHandler AddButtonClick
        {
            add { AddHandler(AddButtonClickEvent, value); }
            remove { RemoveHandler(AddButtonClickEvent, value); }
        }

        public static readonly RoutedEvent CloseClickEvent = EventManager.RegisterRoutedEvent(
            "CloseClickEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddExperiment));

        
        public event RoutedEventHandler CloseClick
        {
            add { AddHandler(CloseClickEvent, value); }
            remove { RemoveHandler(CloseClickEvent, value); }
        }

        public AddExperiment()
        {
            InitializeComponent();
        }

        private string experimentName;
        public string ExperimentName{get { return experimentName; }set { experimentName = value; }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                experimentName = txtName.Text;
                txtName.Clear();
                RaiseEvent(new RoutedEventArgs(AddButtonClickEvent));
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CloseClickEvent));
        }
    }
}