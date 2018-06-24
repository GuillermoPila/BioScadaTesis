using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using BioScadaServer.Variables;
using WpfComponent.Component;
using BioScadaScript;
using Image=System.Drawing.Image;
using Rectangle=System.Drawing.Rectangle;
using Script = BioScadaScript.Script;


namespace BioScadaClient
{
    /// <summary>
    /// Interaction logic for Temp.xaml
    /// </summary>
    public partial class Temp : Window
    {
        public Temp()
        {
            InitializeComponent();

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //    BIT.Height = Height/543*281;
            //    BIT.Width = Width/773*564;
            base.OnRender(drawingContext);
        }

        private void re_Click(object sender, RoutedEventArgs e)
        {
            
            ScadaDataPropertyChange ds = new ScadaDataPropertyChange();
            ds.Items.Add(new Item() { Control = new Button(), PropertyName = "Name", VariableName = "P00" });
            ds.Items.Add(new Item() { Control = new Button(), PropertyName = "Name", VariableName = "P00" });
            ds.Items.Add(new Item() { Control = new Button(), PropertyName = "Name", VariableName = "P00" });
            ddd.PropertyChange = ds;
            System.ComponentModel.Design.CollectionEditor df = new CollectionEditor(typeof(ScadaDataPropertyChange));
            
            _asss.Instance = asaaa;

        }
    }
}
