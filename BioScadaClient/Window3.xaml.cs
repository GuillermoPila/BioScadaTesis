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
using System.Windows.Shapes;

namespace BioScadaClient
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        public Window3()
        {
            InitializeComponent();
        }

        public void Init()
        {
            BIT.Init();
            k120.Init();
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
           /* BIT.Height = Height / 543 * 281;
            BIT.Width = Width / 773 * 564;*/
            base.OnRender(drawingContext);
        }

        
    }
}
