using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingBox
{
    public partial class SWVersion : Form
    {
        public SWVersion()
        {
            InitializeComponent();
        }

        private void SWVersion_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = "V0.1";
        }
    }
}
