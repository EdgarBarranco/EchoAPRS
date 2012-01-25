using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EchoLink;
using Microsoft.Win32;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Threading;


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
            if (File.Exists("EchoAPRS.ini"))
            {
                toolStripStatusLabel1.Text = "Lading configuration.";
                FileStream filestream = new FileStream(@"EchoAPRS.ini", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                Config data = new Config();
                data = (Config)bf.Deserialize(filestream);
                textBox1.Text = data.SSID;
                textBox2.Text = data.Node;
                textBox3.Text = data.Long;
                textBox4.Text = data.Lat;
                textBox5.Text = data.Pass;
                textBox6.Text = data.Server;
                textBox7.Text = data.Port;
                textBox8.Text = data.Beacon;
                textBox9.Text = data.Path;
                checkBox1.Checked = data.Auto;
                filestream.Close();
                toolStripStatusLabel1.Text = "Stored configuration loaded.";
            }
            else
                toolStripStatusLabel1.Text = "No configuration found.";
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
            toolStripStatusLabel1.Text = "Collecting configuration.";
            Thread.Sleep(500);
            button2.Enabled = false;
            
            _ELInfo = new SetupConfig();
            _ELSession = new EchoLinkSession();
            RegistryKey SUBKEY;
            RegistryKey _ELkey = RegistryKey.OpenRemoteBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, "");
            string subkey = "Software\\K1RFD\\EchoLink\\Sysop";             
            SUBKEY = _ELkey.OpenSubKey(subkey);
            string temp_str;
            temp_str = _ELInfo.Callsign;
            if (temp_str.Contains("-"))
            {
                textBox1.Text = _ELInfo.Callsign.Remove(_ELInfo.Callsign.Length - 1);
                textBox1.AppendText("10");
            }
            else
            {
                textBox1.Text = _ELInfo.Callsign;
                textBox1.AppendText("-10");
            }
            string _ELNode = "";

            foreach (EchoLink.IStationEntry station in _ELSession.StationEntries)
            {
                if (_ELInfo.Callsign == station.Callsign)
                {
                    _ELNode = station.NodeNumber.ToString();
                    break;
                }
            }

            textBox2.Text = _ELNode;
            object APRSlat = SUBKEY.GetValue("APRSlat");
            temp_str = APRSlat.ToString();
            string APRSlat_str = temp_str.Replace(".","");
            textBox3.Text = process_lat_long(APRSlat_str);
            object APRSlon = SUBKEY.GetValue("APRSlon");
            temp_str = APRSlon.ToString();
            string APRSlon_str = temp_str.Replace(".","");
            textBox4.Text = process_lat_long(APRSlon_str);
            textBox5.Text = "PASSCODE";
            textBox6.Text = "srvr.aprs-is.net"; // http://www.aprs-is.net/SendOnlyPorts.aspx
            textBox7.Text = "8080";
            textBox8.Text = "30";
            textBox9.Text = "WIDW1-1,WIDE2-2";
            button2.Enabled = true;
            toolStripStatusLabel1.Text = "Default values loaded.";
        }

        string process_lat_long(string in_val)
        {   
            int temp = 0;
            float temp_fl = 0;
            string temp_str = "";
            temp = in_val.Length;
            
            if (in_val[temp - 1] == 'N')
            {
                temp_str = in_val.Replace("N", "");
                temp_str= "" + temp_str[0] + temp_str[1] + "." + temp_str[2] + temp_str[3] + temp_str[4] + temp_str[5] ;
                temp_fl = Convert.ToSingle(temp_str);
                return temp_fl.ToString();
            }
            else if (in_val[temp - 1] == 'S')
            {
                temp_str = in_val.Replace("S", "");
                temp_str = "-" + temp_str[0] + temp_str[1] + "." + temp_str[2] + temp_str[3] + temp_str[4] + temp_str[5] ;
                temp_fl = Convert.ToSingle(temp_str);
                return temp_fl.ToString();
            }

            if (in_val[temp - 1] == 'E')
            {
                temp_str = in_val.Replace("E", "");
                temp_str = "" + temp_str[0] + temp_str[1] + temp_str[2] + "." + temp_str[3] + temp_str[4] + temp_str[5] ;
                temp_fl = Convert.ToSingle(temp_str);
                return temp_fl.ToString();
            }
            else if (in_val[temp - 1] == 'W')
            {
                temp_str = in_val.Replace("W", "");
                temp_str = "-" + temp_str[0] + temp_str[1] + temp_str[2] + "." + temp_str[3] + temp_str[4] + temp_str[5] ;
                temp_fl = Convert.ToSingle(temp_str);
                return temp_fl.ToString();
               
            }
            return temp_fl.ToString();
        }

       

        [Serializable]
        internal class Config
        {
            public string SSID = string.Empty;
            public string Node = string.Empty;
            public string Long = string.Empty;
            public string Lat = string.Empty;
            public string Pass = string.Empty;
            public string Server = string.Empty;
            public string Port = string.Empty;
            public string Beacon = string.Empty;
            public string Path = string.Empty;
            public bool Auto = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Saving configuration.";
            button3.Enabled = false;
            Config data = new Config();
            data.SSID = textBox1.Text;
            data.Node = textBox2.Text;
            data.Long = textBox3.Text;
            data.Lat = textBox4.Text;
            data.Pass = textBox5.Text;
            data.Server = textBox6.Text;
            data.Port = textBox7.Text;
            data.Beacon = textBox8.Text;
            data.Path = textBox9.Text;
            data.Auto = checkBox1.Checked;

            FileStream filestream = new FileStream(@"EchoAPRS.ini", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(filestream, data);
            filestream.Close();
            button3.Enabled = true;
            toolStripStatusLabel1.Text = "Configuration Saved.";
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("EchoAPRS.ini"))
            {
                toolStripStatusLabel1.Text = "Lading configuration.";
                FileStream filestream = new FileStream(@"EchoAPRS.ini", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                Config data = new Config();
                data = (Config)bf.Deserialize(filestream);
                textBox1.Text = data.SSID;
                textBox2.Text = data.Node;
                textBox3.Text = data.Long;
                textBox4.Text = data.Lat;
                textBox5.Text = data.Pass;
                textBox6.Text = data.Server;
                textBox7.Text = data.Port;
                textBox8.Text = data.Beacon;
                textBox9.Text = data.Path;
                checkBox1.Checked = data.Auto;
                filestream.Close();
                toolStripStatusLabel1.Text = "Stored configuration loaded.";
            }
            else
                toolStripStatusLabel1.Text = "No configuration found.";
        }
    }
}
