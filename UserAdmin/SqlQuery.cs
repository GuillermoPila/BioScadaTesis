using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace UserAdmin
{
    internal class SQLQuery : IDisposable
    {
        private SqlConnection _Connection;

        public SQLQuery(string connectionString)
        {
            _Connection = new SqlConnection(connectionString);
            _Connection.Open();
        }

        public DataTable ExecuteData(string commandText)
        {
            SqlCommand command = new SqlCommand(commandText, _Connection);
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet set = new DataSet();
            command.CommandType = CommandType.Text;
            adapter.SelectCommand = command;
            adapter.Fill(set);

            adapter.Dispose();
            command.Dispose();
            if (set.Tables.Count > 0)
                return set.Tables[0];
            else
                return null;
        }

        public SqlDataReader ExecuteReader(string commandText)
        {
            SqlCommand command = new SqlCommand(commandText, _Connection);
            command.CommandType = CommandType.Text;

            SqlDataReader reader = command.ExecuteReader();
            command.Dispose();

            return reader;
        }

        #region ---------------------IDisposable---------------------

        public void Dispose()
        {
            if (_Connection != null)
            {
                _Connection.Close();
                _Connection.Dispose();
            }
            //_Transaction
        }

        #endregion
    }
}