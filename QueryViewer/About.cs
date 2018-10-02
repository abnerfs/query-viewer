using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryViewer
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            this.Text = $"{Program.Name} - About";
            txtVersion.Text = Program.Version;
        }

        private void About_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
