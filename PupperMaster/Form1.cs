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
using ProcessCreationService;

namespace PuppetMaster
{
    public partial class Form1 : Form
    {

        PuppetMasterLogic pml;

        Queue commandsList = new Queue();

        public Form1(ConfigReader cr)
        {
            InitializeComponent();

            pml = new PuppetMasterLogic(cr);
        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            CommandReply cr = pml.SendCommand(tbCommand.Text);

            tbCommandLog.Text += cr.Error + "\r\n";

        }

        private void btnSequence_Click(object sender, EventArgs e)
        {
            string[] commands = System.IO.File.ReadAllLines(@"..\..\..\files\scripts\" + tbFileName.Text);

            foreach(string c in commands)
            {
                CommandReply cr = pml.SendCommand(c);

                tbCommandLog.Text += cr.Error + "\r\n";
            }
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            string[] commands = System.IO.File.ReadAllLines(@"..\..\..\files\scripts\" + tbFileName.Text);

            commandsList = new Queue(commands);

            btnNextStep.Enabled = true;
        }

        private void btnNextStep_Click(object sender, EventArgs e)
        {

            string command = (string) commandsList.Dequeue();

            CommandReply cr = pml.SendCommand(command);

            tbCommandLog.Text += cr.Error + "\r\n";

            if(commandsList.Count == 0)
                btnNextStep.Enabled = false;

        }
    }
}
