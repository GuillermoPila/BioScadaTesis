using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using BioScadaServer.Variables;
using WpfComponent.Common;

namespace WpfComponent.Component
{
    public class ScadaDataFeedBack : Common.Component, IObjectReceiver
    {
        private string experimentName;
        public string ExperimentName { get { return experimentName; } set { experimentName = value; } }

        private WCFInteropSingleton Interop;
        public ScadaDataFeedBack()
        {
            Interop = WCFInteropSingleton.GetInstance();
            // Loaded += Load;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Typeface typeface = new Typeface("Times New Roman");
            FormattedText text = new FormattedText("DataFeedBack", CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, typeface, 12, Brushes.Black);

            drawingContext.DrawText(text, new Point(0, 0));
        }

        public void Receive(Dictionary<string, object> Objects)
        {
            Interop.SetValueVariable(Objects, experimentName);
        }
    }
}
