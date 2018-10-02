using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace QueryViewer
{
    public static class Util
    {
        public static object GetAnomProperty(object Object, string Property)
        {
           return Object.GetType().GetProperty(Property).GetValue(Object);
        }

        public static string OpenFileDialog(string FileName, string Filter, string InitialPath = "")
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = Filter;
            ofd.FileName = FileName;
            ofd.CheckFileExists = true;
            ofd.InitialDirectory = InitialPath;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileName;
            }
            return null;
        }

        public static string SaveFileDialog(string FileName, string Filter, string InitialPath = "")
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = Filter;
            sfd.FileName = FileName;
            sfd.InitialDirectory = InitialPath;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                return sfd.FileName;
            }
            return null; 
        }

        public static void SaveExcel(DataGridView DataGrid, string Path, bool CloseAfter = true)
        {
            var excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = true;
            var workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            var sheet1 = (Worksheet)workbook.Sheets[1];

            int StartCol = 1;
            int StartRow = 1;
            int j = 0, i = 0;

            //Write Headers
            for (j = 0; j < DataGrid.Columns.Count; j++)
            {
                var myRange = (Range)sheet1.Cells[StartRow, StartCol + j];
                myRange.Value2 = DataGrid.Columns[j].HeaderText;
            }

            StartRow++;

            //Write datagridview content
            for (i = 0; i < DataGrid.Rows.Count; i++)
            {
                for (j = 0; j < DataGrid.Columns.Count; j++)
                {
                    try
                    {
                        var myRange = (Range)sheet1.Cells[StartRow + i, StartCol + j];
                        myRange.Value2 = DataGrid[j, i].Value == null ? "" : DataGrid[j, i].Value;
                    }
                    catch
                    {
                    }
                }
            }

            workbook.SaveAs(Path);
            if (CloseAfter)
                excel.Quit();
        }
    }
}
