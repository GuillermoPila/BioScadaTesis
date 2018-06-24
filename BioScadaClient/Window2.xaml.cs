using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfComponent.Component;
using BioScadaScript;

namespace BioScadaClient
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public WCFInteropSingleton Interop;


        private ServerClient _Client;


        private ContextMenu MenuDelete;
        public Window2()
        {
            InitializeComponent();


            //_Interop.Active = true;
            //_Interop.RefreshFrequency = 100;
            this.AddScript.AddButtonClick += AddScriptControl_AddButtonClick;
            this.AddScript.CloseClick += AddScript_CloseClick;

        }

        private void AddScript_CloseClick(object sender, RoutedEventArgs e)
        {

            ((Storyboard)this.Resources["hideScriptWindow"]).Begin(this);
            ScriptControlShow = false;
            //AddScript.Visibility = System.Windows.Visibility.Hidden;
        }

        public void InitializeWin()
        {
            Interop.RefreshFrequency = 100;

            MenuDelete = new ContextMenu();
            MenuItem menuItemD = (MenuItem)Resources["ListScriptDeleteContextMenu"];//new MenuItem() { Header = "Delete Script" };
            // menuItemD.Click += ListScriptDelete_Click;
            MenuDelete.Items.Add(menuItemD);
            try
            {
                foreach (KeyValuePair<string, Dictionary<string, object>> pair in Interop.ExperimentVars)
                {
                    TreeViewItem exp = new TreeViewItem();
                    exp.Header = pair.Key;
                    foreach (var pairV in pair.Value)
                    {
                        TreeViewItem variable = new TreeViewItem();
                        variable.Header = pairV.Key;
                        variable.PreviewMouseLeftButtonDown += VariableMouseDown;
                        //variable.AllowDrop = true;
                        exp.Items.Add(variable);
                    }
                    Experiment_Variable.Items.Add(exp);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void VariableMouseDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem it = (TreeViewItem)sender;
            string data = it.Parent.GetValue(TreeViewItem.HeaderProperty) + "_" + it.Header;
            DragDrop.DoDragDrop(it, data, DragDropEffects.Copy);
        }

        private void AddScriptControl_AddButtonClick(object sender, RoutedEventArgs e)
        {
            ScriptInAppDomain scr = new ScriptInAppDomain("", AddScript.NameScript, AddScript.LanguageScript);
            Interop.Scripts.Add(scr);
            TreeViewItem item = new TreeViewItem();
            item.Header = AddScript.NameScript;
            item.ContextMenu = MenuDelete;
            item.Selected += TreeViewItem_Selected;
            item.IsSelected = true;
            ListScript.Items.Add(item);
            AddScript_CloseClick(sender, e);
            //AddScript.Visibility = System.Windows.Visibility.Hidden;
        }

        private bool ScriptControlShow = false;
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!ScriptControlShow)
            {
                AddScript.Visibility = System.Windows.Visibility.Visible;
                ((Storyboard)this.Resources["showScriptWindow"]).Begin(this);
                ScriptControlShow = true;
            }
        }

        private ScriptInAppDomain scrActual = null;
        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            string nameScript = (sender as TreeViewItem).Header.ToString();
            scrActual = Interop.Scripts.Find(x => x.NameScript == nameScript);
            CodeScript.Text = scrActual.Code;
            groupBox3.Header = "Code Script " + scrActual.Language;
            Errors.Items.Clear();
            Errors.Items.Add(scrActual.Errors);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (scrActual != null)
            {
                try
                {
                    scrActual.Code = CodeScript.Text;
                    string error = scrActual.CompileInAppDomain(Interop.ExperimentVars);
                    Errors.Items.Clear();
                    Errors.Items.Add(error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < Application.Current.Windows.Count; i++)
                {
                    if (Application.Current.Windows[i] is Window3)
                        (Application.Current.Windows[i] as Window3).Init();
                }
                Interop.Active = true;
                Interop.Start("Exp1");
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        Window3 window3 = new Window3();
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            window3 = new Window3();
            window3.Init();
            window3.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Interop.Active = false;
            Interop.Stop("Exp1");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Interop.Leave();
            Application.Current.Shutdown();
        }

        private void CodeScript_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }

        private void CodeScript_Drop(object sender, DragEventArgs e)
        {
            string sd = e.Data.GetData(DataFormats.Text).ToString();
            (sender as TextBox).Text += sd;
        }

        private void ListScriptDelete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem saf = (e.OriginalSource as MenuItem);
            ContextMenu sda = (ContextMenu)saf.Parent;
            TreeViewItem item1 = (sda.PlacementTarget as TreeViewItem);

            string aux = item1.Header.ToString();
            ScriptInAppDomain auxScr =
                Interop.Scripts.Find(x => x.NameScript.Equals(aux, StringComparison.OrdinalIgnoreCase));
            if (auxScr != null)
            {
                Interop.Scripts.Remove(auxScr);
                ListScript.Items.Remove(item1);
            }


        }

        private void HelpContent_Click(object sender, RoutedEventArgs e)
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
