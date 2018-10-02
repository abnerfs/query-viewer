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
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace QueryViewer
{
    public partial class QueryView : Form
    {
        public QueryView()
        {
            InitializeComponent();
            DataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            var Databases = SQL.ListDatabases();
            cmbDatabase.DisplayMember = "Text";
            cmbDatabase.ValueMember = "Value";
            cmbDatabase.Items.Clear();
            foreach(var Database in Databases)
            {
                cmbDatabase.Items.Add(new { Value = Database, Text = Database });
            }

            var CurDatabase = SQL.GetDatabase();
            cmbDatabase.SelectedItem = new { Value = CurDatabase, Text = CurDatabase };
            cmbDatabase.SelectedText = CurDatabase;
            cmbDatabase.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbDatabase.SelectedIndexChanged += (s, args) =>
            {
                var Item = cmbDatabase.SelectedItem;
                var Selected = Item.GetType().GetProperty("Value").GetValue(Item).ToString();
                SQL.SetDatabase(Selected);
            };

            btnExport.Enabled = false;
            this.ActiveControl = txtQuery;

            var ToolTip1 = new ToolTip();
            ToolTip1.SetToolTip(btnQuery, "F5");

            this.Text = $"{Program.Name} - {Program.Info.Server}";
        }

        private void button1_Click(object sender, EventArgs e)
        {

            btnExport.Enabled = false;

            var Query = txtQuery.Text;

            btnQuery.Enabled = false;

            var reader = SQL.ExecuteQuery(Query);

            DataGrid.ColumnCount = 0;
            if (reader != null)
            {
                 
                while (reader.Read())
                {
                    if (DataGrid.ColumnCount == 0)
                    {
                        btnExport.Enabled = true;
                        DataGrid.ColumnCount = reader.FieldCount;
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            DataGrid.Columns[i].HeaderText = reader.GetName(i);
                            DataGrid.Columns[i].Name = reader.GetName(i);
                            DataGrid.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        }
                    }

                    var Row = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader[i] != DBNull.Value)
                            Row.Add(reader[i].ToString());
                        else
                            Row.Add(null);
                    }
                    DataGrid.Rows.Add(Row.ToArray());
                }
                reader.Close();
            }

            btnQuery.Enabled = true;

            var CurDatabase = SQL.GetDatabase();
            cmbDatabase.SelectedItem = new { Value = CurDatabase, Text = CurDatabase };
            cmbDatabase.SelectedText = CurDatabase;
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ConnectionType == ConnectionTypeEnum.MySQL)
            {
                Program.MysqlConnection.Close();
            }
            else if (Program.ConnectionType == ConnectionTypeEnum.SQLServer)
            {
                Program.SqlServerConnection.Close();
            }
            DataGrid.ColumnCount = 0;

            this.DialogResult = DialogResult.OK;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Form = new About();
            Form.Show();
        }

        private void QueryView_Load(object sender, EventArgs e)
        {
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var FilePath = Util.SaveFileDialog("", "Excel Documents (*.xls)|*.xls");
            if(FilePath != null)
                Util.SaveExcel(DataGrid, FilePath, false);
        }

        private void QueryView_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if(btnQuery.Enabled)
                    button1_Click(this, e);
            }
            else if(e.Control && e.KeyCode == Keys.S)
            {
                if(saveCTRLSToolStripMenuItem.Enabled)
                    saveCTRLSToolStripMenuItem_Click(this, e);
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                if (openToolStripMenuItem.Enabled)
                    openToolStripMenuItem_Click(this, e);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var PathOpen = Util.OpenFileDialog(Program.FileName ?? "", "SQL Files (*.sql)|*.sql", Program.FileDirectory ?? "");
            if (PathOpen == null)
                return;

            txtQuery.Text = File.ReadAllText(PathOpen);
            Program.FileName = Path.GetFileName(PathOpen);
            Program.FileDirectory = Path.GetDirectoryName(PathOpen);
        }

        private void saveCTRLSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var PathSave = Util.SaveFileDialog(Program.FileName ?? "", "SQL Files (*.sql)|*.sql", Program.FileDirectory ?? "");
            if (PathSave == null)
                return;

            File.WriteAllText(PathSave, txtQuery.Text);
            Program.FileName = Path.GetFileName(PathSave);
            Program.FileDirectory = Path.GetDirectoryName(PathSave);
        }
    }

}
