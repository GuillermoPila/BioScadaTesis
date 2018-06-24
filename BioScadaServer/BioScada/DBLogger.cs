using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using BioScadaServer.Tools;
using BioScadaServer.Variables;

namespace BioScadaServer.BioScada
{
    [Serializable]
    public class DBLogger : INotifierReceiverChange, IDisposable
    {
        private string logTableNameBase = "logSCADA";
        public string LogTableNameBase
        {
            get { return logTableNameBase; }
            set { logTableNameBase = value; }
        }
        [NonSerialized]
        SqlConnection conn;
        [NonSerialized]
        private Dictionary<Type, SqlCommand> InsertCommands = new Dictionary<Type, SqlCommand>();

        public DBLogger(string ConnStr)
        {
            conn = new SqlConnection(ConnStr);
            conn.Open();
        }

        //public void Receive(VarChangeNotification notification)
        //{
        //    foreach (var item in notification.ItemVar)
        //    {
        //        Type valueType = item.NewValue.GetType();
        //        SqlDbType dbType = DBTypeConvertor.ToSqlDbType(valueType);
        //        if (!InsertCommands.ContainsKey(valueType))
        //        {
        //            CreateLogTable(valueType, dbType);
        //            SqlCommand sqlInsertCommand = GetInsertCommand(valueType, dbType);
        //            InsertCommands.Add(valueType, sqlInsertCommand);
        //        }
        //        SqlCommand insertCommand = InsertCommands[valueType];
        //        insertCommand.Parameters["@LogDate"].Value = notification.When;
        //        insertCommand.Parameters["@VarID"].Value = item.VariableID;
        //        insertCommand.Parameters["@Data"].Value = item.NewValue;
        //        insertCommand.ExecuteNonQuery();
        //    }
        //}

        private SqlCommand GetInsertCommand(Type type, SqlDbType dbType)
        {
            SqlCommand result = conn.CreateCommand();

            result.CommandText = string.Format("insert into [{0}](LogDate, VarName, Data) values (@LogDate, @VarName, @Data)", logTableNameBase + type.Name);
            result.Parameters.Add("@LogDate", SqlDbType.DateTime);
            result.Parameters.Add("@VarName", SqlDbType.VarChar);
            result.Parameters.Add("@Data", dbType);
            return result;
        }

        private void CreateLogTable(Type type, SqlDbType dbType)
        {
            string TableName = logTableNameBase + type.Name;
            string sqlCreateTable = string.Format(
                                       "if not exists (select * from sysobjects where id = object_id(N'[{0}]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) " +
                                       "begin " +
                                       "create table [{0}] (LogDate datetime, VarName varchar(50), Data {1})", TableName, dbType) +
                                   "end";
            using (var command = conn.CreateCommand())
            {
                command.CommandText = sqlCreateTable;
                command.ExecuteNonQuery();
            }

        }

        public void Dispose()
        {
            foreach (var pair in InsertCommands)
                pair.Value.Dispose();
            conn.Dispose();
        }

        public void ReceiverChange(ChangeNotification notifier)
        {

            Type valueType = notifier.Item.NewValue.GetType();
            SqlDbType dbType = DBTypeConvertor.ToSqlDbType(valueType);
            if (!InsertCommands.ContainsKey(valueType))
            {
                CreateLogTable(valueType, dbType);
                SqlCommand sqlInsertCommand = GetInsertCommand(valueType, dbType);
                InsertCommands.Add(valueType, sqlInsertCommand);
            }

            SqlCommand insertCommand = InsertCommands[valueType];
            insertCommand.Parameters["@LogDate"].Value = notifier.When;
            insertCommand.Parameters["@VarName"].Value = notifier.Item.Name;
            insertCommand.Parameters["@Data"].Value = notifier.Item.NewValue;
            try
            {
                insertCommand.ExecuteNonQuery();    
            }
            catch{}
            

        }
    }
}
