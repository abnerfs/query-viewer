using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;

namespace QueryViewer
{
    public class SQL
    {
        public static void SetDatabase(string Database)
        {
            var Query = $"USE {Database}";
            var Reader = SQL.ExecuteQuery(Query);
            Reader.Close();
        }

        public static string[] ListDatabases()
        {
            IDataReader reader = null;
            string Query = string.Empty;

            if(Program.ConnectionType == ConnectionTypeEnum.SQLServer)
            {
                Query = @"SELECT * FROM sys.databases d ";
            }
            else if(Program.ConnectionType == ConnectionTypeEnum.MySQL)
            {
                Query = "show databases";
            }

            var Retorno = new List<string>();
            if (Query != string.Empty)
            {
                reader = ExecuteQuery(Query);
                while (reader.Read())
                {
                    if (reader[0] != DBNull.Value)
                        Retorno.Add(reader.GetString(0));
                }
                reader.Close(); 
            }

            return Retorno.ToArray();
        }

        public static string GetDatabase()
        {
            if (Program.ConnectionType == ConnectionTypeEnum.SQLServer)
            {
                return Program.SqlServerConnection.Database;
            }
            else if (Program.ConnectionType == ConnectionTypeEnum.MySQL)
            {
                //   return Program.MysqlConnection.Database;
                var Query = "SELECT DATABASE();";
                var command = new MySqlCommand(Query, Program.MysqlConnection);
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    if (reader[0] != DBNull.Value)
                        return reader.GetString(0);

                    return "";
                }
            }
            else
            {
                return null;
            }
        }

        public static IDataReader ExecuteQuery(string Query)
        {
            try
            {
                if (string.IsNullOrEmpty(Query))
                    return null;

                if (Program.ConnectionType == ConnectionTypeEnum.SQLServer)
                {
                    var command = new SqlCommand(Query, Program.SqlServerConnection);
                    return command.ExecuteReader();
                }
                else if (Program.ConnectionType == ConnectionTypeEnum.MySQL)
                {
                    var command = new MySqlCommand(Query, Program.MysqlConnection);
                    return command.ExecuteReader();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex) when (ex is SqlException || ex is MySqlException)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
