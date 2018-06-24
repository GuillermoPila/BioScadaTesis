using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace UserAdmin
{
    public class DataUsers : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private UsersCollection _UsersCollection;
        public UsersCollection UsersCollection
        {
            get
            {
                if (this._UsersCollection == null)
                {
                    this._UsersCollection = new UsersCollection();

                    using (SQLQuery sql = new SQLQuery(@"Data Source=guille\sqlexpress;Initial Catalog=SCADALog;Integrated Security=True;Pooling=False"))
                    {
                        DataTable table = sql.ExecuteData("SELECT * FROM Usuario");
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            _UsersCollection.Add(new User() { Password = (string)table.Rows[i]["pass"], Rol = (int)table.Rows[i]["rol"], UserName = (string)table.Rows[i]["user"] });
                        }
                    }
                }
                return this._UsersCollection;
            }
        }
    }

    public class User : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        static DataUsers _Northwind;
        public static DataUsers Northwind
        {
            get
            {
                if (_Northwind == null)
                {
                    _Northwind = new DataUsers();
                }

                return _Northwind;
            }
        }

        private string _userName = string.Empty;
        public string UserName
        {
            get
            {
                return this._userName;
            }

            set
            {
                if (this._userName != value)
                {
                    this._userName = value;
                    this.OnPropertyChanged("UserName");
                }
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get
            {
                return this._password;
            }

            set
            {
                if (this._password != value)
                {
                    this._password = value;
                    this.OnPropertyChanged("Password");
                }
            }
        }

        private int _rol = 1;
        public int Rol
        {
            get
            {
                return this._rol;
            }

            set
            {
                if (this._rol != value)
                {
                    this._rol = value;
                    this.OnPropertyChanged("Rol");
                }
            }
        }

        public override string ToString()
        {
            return string.Format("User: " + this.UserName);
        }
    }

    public class UsersCollection : System.Collections.ObjectModel.ObservableCollection<User>
    {
    }

}
