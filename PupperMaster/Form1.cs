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

        Queue<string> commandsList = new Queue<string>();

        public Form1()
        {
            InitializeComponent();

            pml = new PuppetMasterLogic();

        }

        private void FormShown(object sender, EventArgs e)
        {

            while (commandsList.Count > 0)
            {
                string command = commandsList.Dequeue();
                tbCommandLog.AppendText(pml.SendCommand(command) + "\r\n");
            }
        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            tbCommandLog.AppendText(pml.SendCommand(tbCommand.Text) + "\r\n");

        }

        private void btnSequence_Click(object sender, EventArgs e)
        {
            string[] commands = System.IO.File.ReadAllLines(@"..\..\..\files\scripts\" + tbFileName.Text);

            foreach(string c in commands)
            {
                tbCommandLog.AppendText(pml.SendCommand(c) + "\r\n");
            }
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            string[] commands = System.IO.File.ReadAllLines(@"..\..\..\files\scripts\" + tbFileName.Text);

            commandsList.Clear();
            foreach (string command in commands)
            {
                commandsList.Enqueue(command);
            }

            btnNextStep.Enabled = true;
            btnSequence.Enabled = false;
            btnStep.Enabled = false;
        }

        private void btnNextStep_Click(object sender, EventArgs e)
        {

            string command = commandsList.Dequeue();

            tbCommandLog.AppendText(pml.SendCommand(command) + "\r\n");

            if(commandsList.Count == 0)
            {
                btnNextStep.Enabled = false;
                btnSequence.Enabled = true;
                btnStep.Enabled = true;
            }
                

        }
    }
}
