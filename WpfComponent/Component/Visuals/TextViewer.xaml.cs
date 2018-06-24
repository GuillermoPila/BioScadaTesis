using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfComponent.Common;

namespace WpfComponent.Component.Visuals
{
    /// <summary>
    /// Interaction logic for TextViewer.xaml
    /// </summary>
    public partial class TextViewer : UserControl, IObjectReceiver
    {
        public TextViewer()
        {
            InitializeComponent();
        }

        public void Receive(Dictionary<string, object> Objects)
        {
            if (lsbLog != null)
            {
                AddLogMessage("New reading of variables");
                foreach (KeyValuePair<string, object> pair in Objects)
                    AddLogMessage(string.Format("{0}='{1}'", pair.Key, pair.Value));
            }
        }

        private void AddLogMessage(string Message)
        {
            lsbLog.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                          (ThreadStart)(() => lsbLog.Items.Add(Message)));

        }

        public void AddAlarm(string[] alarms)
        {
            if (lsbLog != null)
            {
               // AddLogMessage("New reading of variables");
                for (int i = 0; i < alarms.Length; i++)
                {
                    AddLogMessage(alarms[i]);
                }
            }
        }

    }
}
