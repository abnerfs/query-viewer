using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;


namespace QueryViewer
{
    public enum ConnectionTypeEnum
    {
        SQLServer,
        MySQL
    }

    static class Program
    {
        public static string FileDirectory { get; set; }
        public static string FileName { get; set; }
        public static ConnectionTypeEnum ConnectionType { get; set; }
        public static SqlConnection SqlServerConnection { get; set; }
        public static MySqlConnection MysqlConnection { get; set; }
        public static ConnectionInfo Info { get; set; }

        public const string FileData = "QueryViewer.data";
        public const string Name = "AbNeR Query Viewer";
        public const string Version = "1.0.0";

        public static List<ConnectionInfo> Infos { get; set; } = new List<ConnectionInfo>();

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }

    public class InputException : Exception
    {
        public string Msg { get; set; }

        public InputException(string Msg)
        {
            this.Msg = Msg;
        }
    }
}
