using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Soap;
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
using BioScadaServer.Variables;
using BioScadaServer.Drivers;
using BioScadaServer.Drivers.Modbus;
using BioScadaServer.Alarms;
using BioScadaServer.BioScada;
using Microsoft.Win32;


namespace ServerConfig
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            InitializeWin();

            //Control AddExperiment
            AddExperiment.AddButtonClick += ControlAddExperiment_AddClick;
            AddExperiment.CloseClick += ControlAddExperiment_CloseClick;

            //Control AddVariable
            AddVariable.AddButtonClick += ControlAddVariable_AddClick;
            AddVariable.CloseClick += ControlAddVariable_CloseClick;

            //Control AddConnector
            AddConnector.AddButtonClick += ControlAddConnector_AddClick;
            AddConnector.CloseClick += ControlAddConnector_CloseClick;

            //Control AddAlarm
            AddAlarm.AddButtonClick += ControlAddAlarm_AddClick;
            AddAlarm.CloseClick += ControlAddAlarm_CloseClick;

            string conn = ConfigurationManager.AppSettings["BDConnection"];
            _BD = new DBLogger(@conn);

            _AlarmMannager = new AlarmManeger();

            string port = ConfigurationManager.AppSettings["PortName"];
            int b_rate = Convert.ToInt32(ConfigurationManager.AppSettings["B_rate"]);
            int data_Bt = Convert.ToInt32(ConfigurationManager.AppSettings["Data_Bit"]);
            _PortSerial = new SerialComm(port, b_rate, data_Bt, Parity.None, StopBits.One);
            _Station = new ModbusStationRTU("station1", 1, _PortSerial, true);

        }


        private DBLogger _BD;
        private AlarmManeger _AlarmMannager;
        private SerialComm _PortSerial;
        private ModbusStationRTU _Station;
        List<ConnectorModbusRTU> _Connector = new List<ConnectorModbusRTU>();

        private void ControlAddConnector_AddClick(object sender, RoutedEventArgs e)
        {
            if (ExistVariableInExperiments(AddConnector.ConnectorName))
            {
                _Connector.Add(new ConnectorModbusRTU(AddConnector.ConnectorName, _Station, AddConnector.Address,
                                                      AddConnector.TypeValue));

                TreeViewItem item = new TreeViewItem();
                item.Header = AddConnector.ConnectorName;
                item.ContextMenu = _delConnectorContextMenu;
                List_Drivers.Items.Add(item);
                ControlAddConnector_CloseClick(sender, e);
            }
            else
                MessageBox.Show("This variable " + AddConnector.ConnectorName + " has not been created");
        }

        private bool ExistVariableInExperiments(string varName)
        {
            bool res = false;
            for (int i = 0; i < _ExperimentList.Count; i++)
            {
                if (_ExperimentList[i].Variables.Exists(x => x.Name == varName))
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        private void ControlAddAlarm_AddClick(object sender, RoutedEventArgs e)
        {
            if (ExistVariableInExperiments(AddAlarm.VariableName))
            {
                if (AddAlarm.TypeAlarm is AlarmHi)
                    _AlarmMannager.Alarms.Add(new AlarmHi() { Comment = AddAlarm.Comment, MaxValue = AddAlarm.MaxValueAlarm, VariableName = AddAlarm.VariableName });
                else
                    _AlarmMannager.Alarms.Add(new AlarmLo() { Comment = AddAlarm.Comment, MinValue = AddAlarm.MinValueAlarm, VariableName = AddAlarm.VariableName });

                TreeViewItem item = new TreeViewItem();
                item.Header = AddAlarm.VariableName;
                item.ContextMenu = _delAlarmContextMenu;
                Alarms.Items.Add(item);
                ControlAddAlarm_CloseClick(sender, e);
            }
            else
                MessageBox.Show("This variable " + AddAlarm.VariableName + " has not been created");
        }

        private void ControlAddVariable_AddClick(object sender, RoutedEventArgs e)
        {
            if (!_ExperimentList.Find(x => x.Name == _ActualNameExperiment).Variables.Exists(x => x.Name == AddVariable.VariableName))
            {
                if ((AddVariable.VariableLogg == true) && (AddVariable.VariableAlarm == true))
                    _ExperimentList.Find(x => x.Name == _ActualNameExperiment).Variables.Add(new Variable(AddVariable.VariableName, AddVariable.VariableInitialValue) { RequestPeriod = AddVariable.RequestPeriod, Receive = new INotifierReceiverChange[] { _BD, _AlarmMannager } });
                else
                    if (AddVariable.VariableLogg == true)
                        _ExperimentList.Find(x => x.Name == _ActualNameExperiment).Variables.Add(new Variable(AddVariable.VariableName, AddVariable.VariableInitialValue) { RequestPeriod = AddVariable.RequestPeriod, Receive = new INotifierReceiverChange[] { _BD } });
                    else
                        if (AddVariable.VariableAlarm == true)
                            _ExperimentList.Find(x => x.Name == _ActualNameExperiment).Variables.Add(new Variable(AddVariable.VariableName, AddVariable.VariableInitialValue) { RequestPeriod = AddVariable.RequestPeriod, Receive = new INotifierReceiverChange[] { _AlarmMannager } });
                        else
                            _ExperimentList.Find(x => x.Name == _ActualNameExperiment).Variables.Add(new Variable(AddVariable.VariableName, AddVariable.VariableInitialValue) { RequestPeriod = AddVariable.RequestPeriod });

                TreeViewItem item = new TreeViewItem();
                item.Header = AddVariable.VariableName;
                item.ContextMenu = _VariableContextMenu;
                _TargetTreeItemExperiment.Items.Add(item);
                ControlAddVariable_CloseClick(sender, e);
            }
            else
                MessageBox.Show("This variable name exist");
        }

        private void ControlAddVariable_CloseClick(object sender, RoutedEventArgs e)
        {
            AddVariable.Visibility = System.Windows.Visibility.Hidden;
        }

        private void ControlAddExperiment_CloseClick(object sender, RoutedEventArgs e)
        {
            AddExperiment.Visibility = System.Windows.Visibility.Hidden;
        }

        private void ControlAddConnector_CloseClick(object sender, RoutedEventArgs e)
        {
            AddConnector.Visibility = System.Windows.Visibility.Hidden;
        }

        private void ControlAddAlarm_CloseClick(object sender, RoutedEventArgs e)
        {
            AddAlarm.Visibility = System.Windows.Visibility.Hidden;
        }

        private void ControlAddExperiment_AddClick(object sender, RoutedEventArgs e)
        {
            if (!_ExperimentList.Exists(x => x.Name == AddExperiment.ExperimentName))
            {
                _ExperimentList.Add(new Experiment() { Name = AddExperiment.ExperimentName });
                TreeViewItem item = new TreeViewItem();
                item.Header = AddExperiment.ExperimentName;
                item.ContextMenu = _ExperimentContextMenu;
                List_Experiment.Items.Add(item);
                ControlAddExperiment_CloseClick(sender, e);
            }
            else
                MessageBox.Show("This experiment name exist");
        }

        private ContextMenu _ExperimentContextMenu;
        private ContextMenu _VariableContextMenu;
        private ContextMenu _delConnectorContextMenu;
        private ContextMenu _delAlarmContextMenu;

        List<Experiment> _ExperimentList = new List<Experiment>();
        private void InitializeWin()
        {
            _ExperimentContextMenu = new ContextMenu();
            //MenuItem itemInsert = new MenuItem();
            //itemInsert.Header = "Insert Variable";
            //itemInsert.Click += MenuVariable_InsertItem;

            //MenuItem itemDel = new MenuItem();
            //itemDel.Header = "Delete Experiment";
            //itemDel.Click += MenuVariable_DeleteItem;

            MenuItem item1 = (MenuItem)Resources["_ExperimentVariableCM"];
            MenuItem item2 = (MenuItem)Resources["_ExperimentVariableCMDel"];

            _ExperimentContextMenu.Items.Add(item1);
            _ExperimentContextMenu.Items.Add(item2);

            _VariableContextMenu = new ContextMenu();
            //MenuItem itemDelV = new MenuItem();
            //itemDelV.Header = "Delete Variable";
            //itemDelV.Click += MenuDelVariable_DeleteItem;

            MenuItem item3 = (MenuItem)Resources["_VariableCMDel"];
            _VariableContextMenu.Items.Add(item3);
            

            _delConnectorContextMenu = new ContextMenu();
            MenuItem item4 = (MenuItem)Resources["_ConnectorCMDel"];
            _delConnectorContextMenu.Items.Add(item4);

            _delAlarmContextMenu = new ContextMenu();
            MenuItem item5 = (MenuItem)Resources["_AlarmCMDel"];
            _delAlarmContextMenu.Items.Add(item5);
        }

        //private void MenuAssignedConnector_AssignedItem(object sender, RoutedEventArgs e)
        //{

        //}

        private void MenuDelAlarm_DeleteItem(object sender, RoutedEventArgs e)
        {
            MenuItem saf = (e.OriginalSource as MenuItem);
            ContextMenu sda = (ContextMenu)saf.Parent;
            TreeViewItem item = (sda.PlacementTarget as TreeViewItem);

            _AlarmMannager.Alarms.RemoveAll(x => x.VariableName == item.Header.ToString());
            Alarms.Items.Remove(item);
        }

        private void MenuDelConnector_DeleteItem(object sender, RoutedEventArgs e)
        {
            MenuItem saf = (e.OriginalSource as MenuItem);
            ContextMenu sda = (ContextMenu)saf.Parent;
            TreeViewItem item = (sda.PlacementTarget as TreeViewItem);

            _Connector.RemoveAll(x => x.ConnectorName == item.Header.ToString());
            List_Drivers.Items.Remove(item);
        }

        private void MenuDelVariable_DeleteItem(object sender, RoutedEventArgs e)
        {
            MenuItem saf = (e.OriginalSource as MenuItem);
            ContextMenu sda = (ContextMenu)saf.Parent;
            TreeViewItem item = (sda.PlacementTarget as TreeViewItem);
            TreeViewItem itemParent = (TreeViewItem)(sda.PlacementTarget as TreeViewItem).Parent;
            _ExperimentList.Find(x => x.Name == itemParent.Header.ToString()).Variables.RemoveAll(
                z => z.Name == item.Header.ToString());
            itemParent.Items.Remove(item);
        }

        private void MenuVariable_DeleteItem(object sender, RoutedEventArgs e)
        {
            MenuItem saf = (e.OriginalSource as MenuItem);
            ContextMenu sda = (ContextMenu)saf.Parent;
            TreeViewItem item = (sda.PlacementTarget as TreeViewItem);

            _ExperimentList.RemoveAll(x => x.Name == item.Header.ToString());
            List_Experiment.Items.Remove(item);
        }

        private string _ActualNameExperiment;

        private TreeViewItem _TargetTreeItemExperiment;
        private void MenuVariable_InsertItem(object sender, RoutedEventArgs e)
        {
            MenuItem saf = (e.OriginalSource as MenuItem);
            ContextMenu sda = (ContextMenu)saf.Parent;
            _TargetTreeItemExperiment = (sda.PlacementTarget as TreeViewItem);
            _ActualNameExperiment = _TargetTreeItemExperiment.Header.ToString();
            AddVariable.Visibility = System.Windows.Visibility.Visible;
            AddExperiment.Visibility = System.Windows.Visibility.Hidden;
            AddConnector.Visibility = System.Windows.Visibility.Hidden;
            AddAlarm.Visibility = System.Windows.Visibility.Hidden;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddExperiment.Visibility = System.Windows.Visibility.Visible;
            AddVariable.Visibility = System.Windows.Visibility.Hidden;
            AddConnector.Visibility = System.Windows.Visibility.Hidden;
            AddAlarm.Visibility = System.Windows.Visibility.Hidden;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            AddConnector.Visibility = System.Windows.Visibility.Visible;
            AddExperiment.Visibility = System.Windows.Visibility.Hidden;
            AddVariable.Visibility = System.Windows.Visibility.Hidden;
            AddAlarm.Visibility = System.Windows.Visibility.Hidden;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            AddAlarm.Visibility = System.Windows.Visibility.Visible;
            AddConnector.Visibility = System.Windows.Visibility.Hidden;
            AddExperiment.Visibility = System.Windows.Visibility.Hidden;
            AddVariable.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < _Connector.Count; i++)
            //{
            //    AssignConnector(_Connector[i]);
            //}

            _AllData = new SerializeSystem();
            _AllData.AlarmManeger = _AlarmMannager;
            _AllData.Com = _PortSerial;
            _AllData.Connectors = _Connector;
            _AllData.DB = _BD;
            _AllData.Experiments = _ExperimentList;
            _AllData.Station = _Station;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.ShowDialog();
            if (dialog.FileName != "")
                BinarySerializer.Serialize(_AllData, dialog.FileName);
        }

        private void AssignConnector(ConnectorModbusRTU Connector)
        {
            for (int i = 0; i < _ExperimentList.Count; i++)
            {
                List<Variable> vars = _ExperimentList[i].Variables.FindAll(x => x.Name == Connector.ConnectorName);
                for (int j = 0; j < vars.Count; j++)
                {
                    vars[j].Connector = new IConnector[] { Connector };
                }
            }
        }

        private SerializeSystem _AllData;
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openD = new OpenFileDialog();
                openD.ShowDialog();
                if (openD.FileName != "")
                {
                    _AllData = (SerializeSystem)BinarySerializer.Deserialize(openD.FileName);
                    PresentData(_AllData);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void PresentData(SerializeSystem data)
        {
            _AlarmMannager = data.AlarmManeger;
            _PortSerial = data.Com;
            _Connector = data.Connectors;
            _BD = data.DB;
            _ExperimentList = data.Experiments;
            _Station = data.Station;

            for (int i = 0; i < List_Experiment.Items.Count; i++)
            {
                List_Experiment.Items.RemoveAt(i);
            }
            for (int i = 0; i < List_Drivers.Items.Count; i++)
            {
                List_Drivers.Items.RemoveAt(i);
            }
            for (int i = 0; i < Alarms.Items.Count; i++)
            {
                Alarms.Items.RemoveAt(i);
            }
            for (int i = 0; i < _ExperimentList.Count; i++)
            {
                TreeViewItem itemExp = new TreeViewItem();
                itemExp.Header = _ExperimentList[i].Name;
                itemExp.ContextMenu = _ExperimentContextMenu;
                List_Experiment.Items.Add(itemExp);
                for (int j = 0; j < _ExperimentList[i].Variables.Count; j++)
                {
                    TreeViewItem itemVar = new TreeViewItem();
                    itemVar.Header = _ExperimentList[i].Variables[j].Name;
                    itemVar.ContextMenu = _VariableContextMenu;
                    itemExp.Items.Add(itemVar);
                }
            }
            for (int i = 0; i < _Connector.Count; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = _Connector[i].ConnectorName;
                item.ContextMenu = _delConnectorContextMenu;
                List_Drivers.Items.Add(item);
            }
            for (int i = 0; i < _AlarmMannager.Alarms.Count; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = _AlarmMannager.Alarms[i].VariableName;
                item.ContextMenu = _delAlarmContextMenu;
                Alarms.Items.Add(item);
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void HelpContents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("Help.chm");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
