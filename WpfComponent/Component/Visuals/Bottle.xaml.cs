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
    /// Interaction logic for Bottle.xaml
    /// </summary>
    public partial class Bottle : UserControl
    {
        public Bottle()
        {
            InitializeComponent();
            MaxLevelRect = ActualHeight / 2.14;
            maxLevel = 100;
            //typeFilling = Common.TypeFilling.Up_Down;
        }

        private double fillLevel = 50;
        public double FillLevel
        {
            get { return fillLevel; }
            set
            {
                if (value > maxLevel)
                    return;

                fillLevel = value * 100 / maxLevel;
                MaxLevelRect = ActualHeight / 2.14;
                Level.Height = MaxLevelRect * fillLevel / 100;
            }
        }

        private double maxLevel;
        public double MaxLevel { get { return maxLevel; } set { maxLevel = value; } }

        
        private double MaxLevelRect;
    }
}
