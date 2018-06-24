using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace UserAdmin
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            ICommand deleteCommand = RadGridViewCommands.Delete;
            ICommand beginInsertCommand = RadGridViewCommands.BeginInsert;
            ICommand cancelRowEditCommand = RadGridViewCommands.CancelRowEdit;
            ICommand commitEditCommand = RadGridViewCommands.CommitEdit;
            InitializeComponent();
            //SQLQuery sql = new SQLQuery(@"Data Source=guille\sqlexpress;Initial Catalog=SCADALog;Integrated Security=True;Pooling=False");
            //RadGridView1.ItemsSource = sql.ExecuteData("SELECT * FROM Usuario");
            RadGridView1.ItemsSource = all.UsersCollection;
        }

        DataUsers all = new DataUsers();
        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            string conn = ConfigurationManager.AppSettings["BDConnection"];
            SQLQuery sql = new SQLQuery(@conn);
            try
            {
                string del = string.Format("Delete FROM Usuario");
                sql.ExecuteData(del);
                for (int i = 0; i < all.UsersCollection.Count; i++)
                {
                    string query = string.Format("INSERT INTO Usuario ([user],pass,rol) VALUES ('{0}','{1}',{2})", all.UsersCollection[i].UserName, all.UsersCollection[i].Password, all.UsersCollection[i].Rol);
                    sql.ExecuteData(query);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sql.Dispose();
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtUserName.Text != "")
            {
                MD5 hash = MD5.Create();
                byte[] data = hash.ComputeHash(Encoding.Default.GetBytes(txtPassword.Password));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                string passw = sBuilder.ToString();

                int rolUser = 1;
                int index = cboxRol.SelectedIndex;
                if (index > 0)
                    rolUser = 5;
                all.UsersCollection.Add(new User() { Password = passw, Rol = rolUser, UserName = txtUserName.Text });
                txtUserName.Clear();
                txtPassword.Clear();
            }
            else
                MessageBox.Show("Debe especificar un nombre de usuario");

        }

        private void btnAddCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
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
