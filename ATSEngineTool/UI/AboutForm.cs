using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATSEngineTool
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            this.VersonLabel.Text = Program.Version.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/wilson212/ATSEngineTool/issues");
        }
    }
}
