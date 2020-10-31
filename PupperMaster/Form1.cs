using System;
using System.Collections;
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

        Queue commandsList = new Queue();

        public Form1(ConfigReader cr)
        {
            InitializeComponent();

            pml = new PuppetMasterLogic();

            if(cr != null)
            {
                foreach (string c in cr.InitialSetup)
                {
                    tbCommandLog.Text += pml.SendCommand(c) + "\r\n";
                }
            }
        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            tbCommandLog.Text += pml.SendCommand(tbCommand.Text) + "\r\n";

        }

        private void btnSequence_Click(object sender, EventArgs e)
        {
            string[] commands = System.IO.File.ReadAllLines(@"..\..\..\files\scripts\" + tbFileName.Text);

            foreach(string c in commands)
            {
                tbCommandLog.Text += pml.SendCommand(c) + "\r\n";
            }
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            string[] commands = System.IO.File.ReadAllLines(@"..\..\..\files\scripts\" + tbFileName.Text);

            commandsList = new Queue(commands);

            btnNextStep.Enabled = true;
            btnSequence.Enabled = false;
            btnStep.Enabled = false;
        }

        private void btnNextStep_Click(object sender, EventArgs e)
        {

            string command = (string) commandsList.Dequeue();

            tbCommandLog.Text += pml.SendCommand(command) + "\r\n";

            if(commandsList.Count == 0)
            {
                btnNextStep.Enabled = false;
                btnSequence.Enabled = true;
                btnStep.Enabled = true;
            }
                

        }
    }
}
