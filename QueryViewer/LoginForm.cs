using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Sql;
using System.IO;
using MySql.Data.MySqlClient;

namespace QueryViewer
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            this.Text = $"{Program.Name} - Login";

            var StringCN = string.Empty;

            if (File.Exists(Program.FileData))
                StringCN = File.ReadAllText(Program.FileData);

            ConnectionInfo Last = null;

            if (!string.IsNullOrEmpty(StringCN))
            {
                var Connections = StringCN.Split('#');
                foreach (var Connection in Connections)
                {
                    var Parameters = Connection.Split(';');
                    if (Parameters.Length == 5)
                    {
                        var Info = new ConnectionInfo()
                        {
                            User = Parameters[0],
                            Database = Parameters[1],
                            Server = Parameters[2],
                            ConnectionType = (ConnectionTypeEnum)int.Parse(Parameters[3]),
                            Last = Parameters[4] == "last"
                        };

                        Program.Infos.Add(Info);

                        if (Info.Last)
                        {
                            txtUser.Text = Info.User;
                            txtDatabase.Text = Info.Database;

                            txtPassword.Select();
                            Last = Info;
                        }
                    }
                }
            }

            Func<ConnectionTypeEnum, object> GetComboItem = (Type) =>
            {
                var SQLType = new object();
                switch (Type)
                {
                    case ConnectionTypeEnum.MySQL:
                        SQLType = new { Id = Type, Name = "MySQL" };
                        break;
                    case ConnectionTypeEnum.SQLServer:
                        SQLType = new { Id = Type, Name = "SQLServer" };
                        break;
                };
                return SQLType;
            };

         
            cmbSQLType.DisplayMember = "Name";
            cmbSQLType.ValueMember = "Id";
            cmbSQLType.Items.Add(GetComboItem(ConnectionTypeEnum.MySQL));
            cmbSQLType.Items.Add(GetComboItem(ConnectionTypeEnum.SQLServer));
            cmbSQLType.DropDownStyle = ComboBoxStyle.DropDownList;

            txtServer.DisplayMember = "Description";
            txtServer.ValueMember = "Server";
            txtServer.Items.Add(new ConnectionInfo());
            txtServer.Items.AddRange(Program.Infos.ToArray());

            txtServer.SelectedIndexChanged += (s, args) =>
            {
                var cmb = (ComboBox)s;
                var Info = (ConnectionInfo)cmb.SelectedItem;
                if(Info != null)
                {
                    txtUser.Text = Info.User;
                    txtDatabase.Text = Info.Database;
                    txtServer.Text = Info.Server;
                    txtPassword.Text = "";
                    if(!string.IsNullOrEmpty(Info.Server))
                        txtPassword.Select();
                    
                    var SQLType = GetComboItem(Info.ConnectionType);
                    cmbSQLType.SelectedItem = SQLType;
                    cmbSQLType.Text = Util.GetAnomProperty(SQLType, "Name").ToString();
                }
            };

            if(Last != null)
            {
                var SQLType = GetComboItem(Last.ConnectionType);
                txtServer.SelectedItem = Last;
                cmbSQLType.SelectedItem = SQLType;
                cmbSQLType.Text = Util.GetAnomProperty(SQLType, "Name").ToString();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtServer_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if(String.IsNullOrEmpty(cmbSQLType.Text))
                    throw new InputException("Invalid SQL Type");

                if (string.IsNullOrEmpty(txtServer.Text))
                    throw new InputException("Invalid Server");

                if (string.IsNullOrEmpty(txtUser.Text))
                    throw new InputException("Invalid User");

                if (string.IsNullOrEmpty(txtPassword.Text))
                    throw new InputException("Invalid Password");


                Program.ConnectionType = (ConnectionTypeEnum) Util.GetAnomProperty(cmbSQLType.SelectedItem, "Id");

                var TmpInfo = new ConnectionInfo();

                var tmpServer = string.Empty;


                if (Program.ConnectionType == ConnectionTypeEnum.SQLServer)
                {
                    var StringBuilder = new SqlConnectionStringBuilder();
                    StringBuilder.UserID = txtUser.Text.Trim();
                    StringBuilder.Password = txtPassword.Text;
                    if(!string.IsNullOrEmpty(txtDatabase.Text))
                        StringBuilder.InitialCatalog = txtDatabase.Text.Trim();

                    if (txtServer.SelectedItem != null)
                        tmpServer = ((ConnectionInfo)txtServer.SelectedItem).Server;
                    else
                        tmpServer = txtServer.Text;

                    StringBuilder.DataSource = tmpServer;


                    Program.SqlServerConnection = new SqlConnection(StringBuilder.ConnectionString);
                    Program.SqlServerConnection.Open();

                    TmpInfo = new ConnectionInfo()
                    {
                        User = StringBuilder.UserID,
                        Server = tmpServer,
                        Database = StringBuilder.InitialCatalog,
                        ConnectionType = Program.ConnectionType
                    };

                }
                else if(Program.ConnectionType == ConnectionTypeEnum.MySQL)
                {
                    var StringBuilder = new MySqlConnectionStringBuilder();
                    StringBuilder.UserID = txtUser.Text.Trim();
                    StringBuilder.Password = txtPassword.Text;
                    if (!string.IsNullOrEmpty(txtDatabase.Text))
                        StringBuilder.Database = txtDatabase.Text.Trim();


                    if (txtServer.SelectedItem != null)
                        tmpServer = Util.GetAnomProperty(txtServer.SelectedItem, "Server").ToString();
                    else
                        tmpServer = txtServer.Text;

                    var ServerData = tmpServer.Split(new char[] { ':', ',' });
                    StringBuilder.Server = ServerData[0];
                    if (ServerData.Length > 1)
                    {
                        uint Port = 0;
                        if(uint.TryParse(ServerData[1], out Port))
                        {
                            StringBuilder.Port = Port;
                        }
                    }
                        

                    Program.MysqlConnection = new MySqlConnection(StringBuilder.ConnectionString);
                    Program.MysqlConnection.Open();


                    TmpInfo = new ConnectionInfo()
                    {
                        User = StringBuilder.UserID,
                        Server = tmpServer,
                        Database = StringBuilder.Database,
                        ConnectionType = Program.ConnectionType
                    };
                }
               

                TmpInfo.Last = true;
                Program.Info = TmpInfo;

                if (Program.Infos.FindIndex(x => x.Equals(TmpInfo)) == -1)
                    Program.Infos.Add(TmpInfo);

                var StringCN = string.Empty;
                foreach (var Info in Program.Infos)
                {
                    Info.Last = Info.Equals(TmpInfo);
                    StringCN += Info.StringSave();
                }

                File.WriteAllText(Program.FileData, StringCN);

                this.Hide();

                var Form = new QueryView();

                var Result = Form.ShowDialog();
                if (Result == DialogResult.OK)
                    this.Show();
                else
                    this.Close();
            }
            catch (InputException Ex)
            {
                MessageBox.Show(Ex.Msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) when(ex is SqlException || ex is MySqlException)
            {
                MessageBox.Show(ex.Message, "Error connecting to server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
