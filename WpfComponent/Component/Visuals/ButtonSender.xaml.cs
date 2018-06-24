using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfComponent.Common;

namespace WpfComponent.Component.Visuals
{
    /// <summary>
    /// Interaction logic for ButtonSender.xaml
    /// </summary>
    public partial class ButtonSender : UserControl
    {
        private LinearGradientBrush linear2;
        private LinearGradientBrush linear1;
        public ButtonSender()
        {
            InitializeComponent();

            GradientStopCollection gr1 = new GradientStopCollection();
            gr1.Add(new GradientStop(Colors.White, 0));
            gr1.Add(new GradientStop(Colors.Red, 0.5));
            linear2 = new LinearGradientBrush(gr1);

            GradientStopCollection gr2 = new GradientStopCollection();
            gr2.Add(new GradientStop(Colors.White, 0));
            gr2.Add(new GradientStop(Colors.Green, 0.5));
            linear1 = new LinearGradientBrush(gr2);
        }

        public IObjectReceiver ComponentReceiver { get; set; }

        public string VariableName { get; set; }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

            btn.Background = linear2;
            e.Source.ToString();
         //   ComponentReceiver.Receive(new Dictionary<string, object>() { { VariableName, true } });
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

            btn.Background = linear1;
          //  ComponentReceiver.Receive(new Dictionary<string, object>() { { VariableName, false } });
        }

        public bool IsChecked { get { return (bool)btn.IsChecked; } set { btn.IsChecked = value; } }

    }
}
