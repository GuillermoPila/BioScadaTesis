using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using BioScadaScript;
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
    /// Interaction logic for AddScript.xaml
    /// </summary>
    public partial class AddScript : UserControl
    {
        public static readonly RoutedEvent AddButtonClickEvent = EventManager.RegisterRoutedEvent(
            "AddButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddScript));

        public event RoutedEventHandler AddButtonClick
        {
            add { AddHandler(AddButtonClickEvent, value); }
            remove { RemoveHandler(AddButtonClickEvent, value); }
        }

        public static readonly RoutedEvent CloseClickEvent = EventManager.RegisterRoutedEvent(
            "CloseClickEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddScript));

        /// <summary>
        /// Expose the CloseClickEvent to external sources
        /// </summary>
        public event RoutedEventHandler CloseClick
        {
            add { AddHandler(CloseClickEvent, value); }
            remove { RemoveHandler(CloseClickEvent, value); }
        }

        private string nameScript;
        public string NameScript { get { return nameScript; } set { nameScript = value; } }

        private LanguageScript languageScript;
        public LanguageScript LanguageScript { get { return languageScript; } set { languageScript = value; } }

        public AddScript()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                nameScript = txtName.Text;
                if (radioButton3.IsChecked.Value)
                    languageScript = BioScadaScript.LanguageScript.Visual_Basic;
                else
                    languageScript = BioScadaScript.LanguageScript.C_Sharp;

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
