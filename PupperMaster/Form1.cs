using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster
{
    public partial class Form1 : Form
    {

        PuppetMasterLogic pml;

        public Form1(ConfigReader cr)
        {
            InitializeComponent();

            pml = new PuppetMasterLogic(cr);
        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            tbCommandLog.Text += pml.SendCommand(tbCommand.Text).ToString() + "\r\n";

        }

        private void tbCommandLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
