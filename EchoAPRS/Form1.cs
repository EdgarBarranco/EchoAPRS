using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EchoLink;

namespace EchoAPRS
{
    public partial class MainForm1 : Form
    {
        EchoLinkSession _ELSession;
        SetupConfig _ELInfo;
        
        public MainForm1()
        {
            InitializeComponent();
            _ELSession = new EchoLinkSession();
            _ELSession.Closing += new _IEchoLinkSessionEvents_ClosingEventHandler(_ELSession_Closing);
            _ELSession.Connected += new _IEchoLinkSessionEvents_ConnectedEventHandler(_ELSession_Connected);
            _ELSession.Disconnected += new _IEchoLinkSessionEvents_DisconnectedEventHandler(_ELSession_Disconnected);
        }

        void _ELSession_Disconnected(string sCall)
        {
            throw new NotImplementedException();
        }

        void _ELSession_Connected(string sCall, string sName, string sAddr)
        {
            throw new NotImplementedException();
        }

        public void _ELSession_Closing()
        {
            _ELSession.Closing -= _ELSession_Closing;
            _ELSession = null;
                      
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Exit the rogram?",
                           "Exit",
                            MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes) Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _ELInfo = new SetupConfig();
            _ELSession = new EchoLinkSession();

            textBox1.Text = _ELInfo.Callsign.Remove(_ELInfo.Callsign.Length - 1);
            textBox1.AppendText("10");

            textBox2.Text = "Node#";
            textBox3.Text = "Longitud";
            textBox4.Text = "Latitud";
            textBox5.Text = "PASSCODE";
            textBox6.Text = "srvr.aprs-is.net"; // http://www.aprs-is.net/SendOnlyPorts.aspx
            textBox7.Text = "8080";
            textBox8.Text = "30";


        }

    }
}
