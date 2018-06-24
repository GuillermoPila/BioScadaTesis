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
using System.ComponentModel;


namespace WpfComponent.Component.Visuals
{
    /// <summary>
    /// Interaction logic for Switch.xaml
    /// </summary>
    public partial class Switch : UserControl,INotifyPropertyChanged
    {
        public Switch()
        {
            InitializeComponent();
        }
        public static DependencyProperty LightOnProperty;
        private static PropertyChangedCallback ChangeCallBack;

        static Switch()
        {
            ChangeCallBack += ChangeProperty;
            LightOnProperty = DependencyProperty.Register("LightOn", typeof(bool), typeof(Switch),
                                                        new FrameworkPropertyMetadata(ChangeCallBack));

        }

        private static void ChangeProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Convert.ToBoolean(e.NewValue))
            {
                (d as Switch).On.Visibility = System.Windows.Visibility.Visible;
                (d as Switch).Off.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                (d as Switch).On.Visibility = System.Windows.Visibility.Hidden;
                (d as Switch).Off.Visibility = System.Windows.Visibility.Visible;
            }
        }
        
        public bool LightOn
        {
            get { return (bool)GetValue(LightOnProperty); }
            set
            {
                SetValue(LightOnProperty, value);
            }
        }

        //private bool lightOn;
        //public bool LightOn
        //{
        //    get { return lightOn; }
        //    set
        //    {
        //        lightOn = value;
        //        if (value)
        //        {
        //            On.Visibility = System.Windows.Visibility.Visible;
        //            Off.Visibility = System.Windows.Visibility.Hidden;
        //        }
        //        else
        //        {
        //            On.Visibility = System.Windows.Visibility.Hidden;
        //            Off.Visibility = System.Windows.Visibility.Visible;
        //        }
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("LightOn"));
        //    }
        //}

        public string VariableName { get; set; }
        public Common.IObjectReceiver ComponentReceiver { get; set; }

        private void On_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //LightOn = false;
            ComponentReceiver.Receive(new Dictionary<string, object>() { { VariableName, false } });
        }

        private void Off_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //LightOn = true;
            ComponentReceiver.Receive(new Dictionary<string, object>() { { VariableName, true } });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
