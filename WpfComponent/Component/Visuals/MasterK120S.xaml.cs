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
    /// Interaction logic for MasterK120S.xaml
    /// </summary>
    public partial class MasterK120S : UserControl
    {
        public MasterK120S()
        {
            InitializeComponent();
            //Init();
        }
        
        public void Init()
        {
            ScadaDataSource dataSource = new ScadaDataSource("Exp1");

            ScadaDataPropertyChange propertyChange = new ScadaDataPropertyChange();

            ScadaDataFeedBack dataFeedBack = new ScadaDataFeedBack();
            dataFeedBack.ExperimentName = "Exp1";
            for (int i = 0; i < stackPanelInButton.Children.Count; i++)
            {
                (stackPanelInButton.Children[i] as Switch).ComponentReceiver = dataFeedBack;
            }

            for (int i = 0; i < stackPanelOutButton.Children.Count; i++)
            {
                (stackPanelOutButton.Children[i] as Switch).ComponentReceiver = dataFeedBack;
            }

            propertyChange.Items.Add(new Item() { Control = In0, PropertyName = "LightOn", VariableName = "P00" });
            propertyChange.Items.Add(new Item() { Control = In1, PropertyName = "LightOn", VariableName = "P01" });
            propertyChange.Items.Add(new Item() { Control = In2, PropertyName = "LightOn", VariableName = "P02" });
            propertyChange.Items.Add(new Item() { Control = In3, PropertyName = "LightOn", VariableName = "P03" });
            propertyChange.Items.Add(new Item() { Control = In4, PropertyName = "LightOn", VariableName = "P04" });
            propertyChange.Items.Add(new Item() { Control = In5, PropertyName = "LightOn", VariableName = "P05" });
            propertyChange.Items.Add(new Item() { Control = In6, PropertyName = "LightOn", VariableName = "P06" });
            propertyChange.Items.Add(new Item() { Control = In7, PropertyName = "LightOn", VariableName = "P07" });
            propertyChange.Items.Add(new Item() { Control = In8, PropertyName = "LightOn", VariableName = "P08" });
            propertyChange.Items.Add(new Item() { Control = In9, PropertyName = "LightOn", VariableName = "P09" });
            propertyChange.Items.Add(new Item() { Control = In10, PropertyName = "LightOn", VariableName = "P10" });
            propertyChange.Items.Add(new Item() { Control = In11, PropertyName = "LightOn", VariableName = "P11" });

            propertyChange.Items.Add(new Item() { Control = Out40, PropertyName = "LightOn", VariableName = "P40" });
            propertyChange.Items.Add(new Item() { Control = Out41, PropertyName = "LightOn", VariableName = "P41" });
            propertyChange.Items.Add(new Item() { Control = Out42, PropertyName = "LightOn", VariableName = "P42" });
            propertyChange.Items.Add(new Item() { Control = Out43, PropertyName = "LightOn", VariableName = "P43" });
            propertyChange.Items.Add(new Item() { Control = Out44, PropertyName = "LightOn", VariableName = "P44" });
            propertyChange.Items.Add(new Item() { Control = Out45, PropertyName = "LightOn", VariableName = "P45" });
            propertyChange.Items.Add(new Item() { Control = Out46, PropertyName = "LightOn", VariableName = "P46" });
            propertyChange.Items.Add(new Item() { Control = Out47, PropertyName = "LightOn", VariableName = "P47" });

            propertyChange.Items.Add(new Item() { Control = BtnIn0, PropertyName = "LightOn", VariableName = "P00" });
            propertyChange.Items.Add(new Item() { Control = BtnIn1, PropertyName = "LightOn", VariableName = "P01" });
            propertyChange.Items.Add(new Item() { Control = BtnIn2, PropertyName = "LightOn", VariableName = "P02" });
            propertyChange.Items.Add(new Item() { Control = BtnIn3, PropertyName = "LightOn", VariableName = "P03" });
            propertyChange.Items.Add(new Item() { Control = BtnIn4, PropertyName = "LightOn", VariableName = "P04" });
            propertyChange.Items.Add(new Item() { Control = BtnIn5, PropertyName = "LightOn", VariableName = "P05" });
            propertyChange.Items.Add(new Item() { Control = BtnIn6, PropertyName = "LightOn", VariableName = "P06" });
            propertyChange.Items.Add(new Item() { Control = BtnIn7, PropertyName = "LightOn", VariableName = "P07" });
            propertyChange.Items.Add(new Item() { Control = BtnIn8, PropertyName = "LightOn", VariableName = "P08" });
            propertyChange.Items.Add(new Item() { Control = BtnIn9, PropertyName = "LightOn", VariableName = "P09" });
            propertyChange.Items.Add(new Item() { Control = BtnIn10, PropertyName = "LightOn", VariableName = "P10" });
            propertyChange.Items.Add(new Item() { Control = BtnIn11, PropertyName = "LightOn", VariableName = "P11" });

            propertyChange.Items.Add(new Item() { Control = BtnOut40, PropertyName = "LightOn", VariableName = "P40" });
            propertyChange.Items.Add(new Item() { Control = BtnOut41, PropertyName = "LightOn", VariableName = "P41" });
            propertyChange.Items.Add(new Item() { Control = BtnOut42, PropertyName = "LightOn", VariableName = "P42" });
            propertyChange.Items.Add(new Item() { Control = BtnOut43, PropertyName = "LightOn", VariableName = "P43" });
            propertyChange.Items.Add(new Item() { Control = BtnOut44, PropertyName = "LightOn", VariableName = "P44" });
            propertyChange.Items.Add(new Item() { Control = BtnOut45, PropertyName = "LightOn", VariableName = "P45" });
            propertyChange.Items.Add(new Item() { Control = BtnOut46, PropertyName = "LightOn", VariableName = "P46" });
            propertyChange.Items.Add(new Item() { Control = BtnOut47, PropertyName = "LightOn", VariableName = "P47" });


            dataSource.PropertyChange = propertyChange;
            dataSource.RefreshFrequency = 200;
            dataSource.Receivers.Add(new ObjectTransmitter.ReceiverItem() { Receiver = propertyChange });
            dataSource.Active = true;
        }
        
    }
}
