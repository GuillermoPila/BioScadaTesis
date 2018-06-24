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

namespace WpfComponent.Component.Visuals
{
    /// <summary>
    /// Interaction logic for Biorreactor.xaml
    /// </summary>
    public partial class BiorreactorBottles : UserControl
    {
        public BiorreactorBottles()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pomo1.Height = Height;
            Pomo1.Width = Width / 2;

            Pomo2.Height = Height;
            Pomo2.Width = Width / 2;
            base.OnRender(drawingContext);
        }

        //private double filling;
        public double Filling { get { return Pomo1.FillLevel; } set { Pomo1.FillLevel = value; } }

        //private double drainning;
        public double Drainning { get { return Pomo2.FillLevel; } set { Pomo2.FillLevel = value; } }

        //private void LevelBottles()
        //{
        //    if ((filling < MaxFillBottle1) && (filling > 0) && (MaxFillBottle1 > 0))
        //    {
        //        Pomo1.FillLevel = filling;// *100 / MaxFillBottle1;
        //        Pomo2.FillLevel = Pomo2.MaxLevel - filling;// *100 / MaxFillBottle1;
        //    }
        //    else
        //    {
        //        if ((drainning < MaxFillBottle2) && (drainning > 0) && (MaxFillBottle2 > 0))
        //        {
        //            Pomo1.FillLevel = Pomo1.MaxLevel - drainning;// *100 / MaxFillBottle2;
        //            Pomo2.FillLevel = drainning;// *100 / MaxFillBottle2;
        //        }
        //    }
        //}

        public double MaxFillBottle1 { get { return Pomo1.MaxLevel; } set { Pomo1.MaxLevel = value; } }
        public double MaxFillBottle2 { get { return Pomo2.MaxLevel; } set { Pomo2.MaxLevel = value; } }
    }
}
