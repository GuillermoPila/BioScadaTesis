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
using WpfComponent.Common;

namespace WpfComponent.Component.Visuals
{
    /// <summary>
    /// Interaction logic for BIT_Component.xaml
    /// </summary>
    public partial class BIT_Component : UserControl
    {
        public BIT_Component()
        {
            InitializeComponent();
            //Init();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            bottles.Height = Height / 251 * 91;
            bottles.Width = Width / 507 * 234;
            base.OnRender(drawingContext);
        }

        //Esto es pa' probar
        public BiorreactorBottles Bottles
        {
            get { return bottles; }
            set { bottles = value; }
        }

        /*public double Filling
        {
            get { return bottles.Filling; }
            set
            {
                bottles.Filling = value;
            }
        }

        public double Drainning
        {
            get { return bottles.Drainning; }
            set
            {
                bottles.Drainning = value;
                
            }
        }

        public double MaxFillBottle1 { get { return bottles.MaxFillBottle1; } set { bottles.MaxFillBottle1 = value; } }
        public double MaxFillBottle2 { get { return bottles.MaxFillBottle2; } set { bottles.MaxFillBottle2 = value; } }*/

        public void Init()
        {
            ScadaDataSource dataSource = new ScadaDataSource("Exp1");

            ScadaDataPropertyChange propertyChange = new ScadaDataPropertyChange();

            //ScadaDataFeedBack dataFeedBack = new ScadaDataFeedBack();
            //dataFeedBack.ExperimentName = "Exp1";


            propertyChange.Items.Add(new Item() { Control = filling1, PropertyName = "LightOn", VariableName = "P40" });
            //propertyChange.Items.Add(new ScadaDataPropertyChange.Item() { Control = filling2, PropertyName = "LightOn", VariableName = "P40" });
            propertyChange.Items.Add(new Item() { Control = draing1, PropertyName = "LightOn", VariableName = "P41" });
            //propertyChange.Items.Add(new ScadaDataPropertyChange.Item() { Control = draing2, PropertyName = "LightOn", VariableName = "P41" });

            propertyChange.Items.Add(new Item() { Control = bottles, PropertyName = "MaxFillBottle1", VariableName = "D0000" });
            propertyChange.Items.Add(new Item() { Control = bottles, PropertyName = "MaxFillBottle2", VariableName = "D0002" });
            propertyChange.Items.Add(new Item() { Control = bottles, PropertyName = "Filling", VariableName = "Vol1" });
            propertyChange.Items.Add(new Item() { Control = bottles, PropertyName = "Drainning", VariableName = "Vol2" });



            dataSource.PropertyChange = propertyChange;
            dataSource.RefreshFrequency = 250;
            dataSource.Receivers.Add(new ObjectTransmitter.ReceiverItem() { Receiver = propertyChange });
            dataSource.Active = true;
        }
    }
}
