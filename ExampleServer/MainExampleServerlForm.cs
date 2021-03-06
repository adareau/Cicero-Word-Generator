using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DataStructures;

namespace Virgil
{
    public partial class MainExampleServerlForm : Form
    {
        private ExampleServer server;


        public MainExampleServerlForm() : this(new ExampleServerSettings())
        {
        }

        public MainExampleServerlForm(ExampleServerSettings settings) {
            InitializeComponent();
            this.server = new ExampleServer(this, settings);
            this.server.messageLog += addMessageLogText;
            this.propertyGrid1.SelectedObject = settings;

        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            connectButton.Enabled = false;
            server.openConnection();
        }

        public delegate void voidVoidDelegate();

        public void reenableConnectButton()
        {
            if (this.InvokeRequired)
            {

                this.BeginInvoke(new voidVoidDelegate(reenableConnectButton));
            }
            else
                this.connectButton.Enabled = true;
        }



        public delegate void MessageEventCallDelegate(object sender, MessageEvent e);
        public void addMessageLogText(object sender, MessageEvent e)
        {
            if (this.InvokeRequired)
            {
                MessageEventCallDelegate ev = new MessageEventCallDelegate(addMessageLogText);
                this.BeginInvoke(ev, new object[] { sender, e });
            }
            else
            {
                this.textBox1.AppendText(e.myTime.ToString() + " " + sender.ToString() + ": " + e.ToString() + "\r\n");
            }
        }


    }
}