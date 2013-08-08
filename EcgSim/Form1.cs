using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WfdbCsharpWrapper;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace EcgSim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            //using (var record = new Record("c:\\users\\Al\\Dropbox\\Medical Projects\\ECG Simulator\\EcgSim\\EcgSim\\data\\100s"))
            using (var record = new Record("data/aami-ec13/aami3a"))
            //using (var record = new Record(txtFile.Text.Substring(2, txtFile.Text.Length - 6)))
            {
                record.Open();
                //next line added by Al as test
                Frequency.InputFrequency = 125;
                
                var samples = record.GetSamples(1000);
                int val;
                string outword;
                serOut.BaudRate = 38400;
                serOut.Parity = Parity.None;
                serOut.DataBits = 8;
                serOut.StopBits = StopBits.One;
                serOut.Handshake = Handshake.None;
                serOut.Open();
                foreach (var s in samples)
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        //Console.Write(s[i].ToString());
                        //Console.Write(string.Format("D{0000}\t", s[i]));
                        val = s[i];
                        outword = "D" + val.ToString("0000");
                        Console.Write(outword);
                        serOut.Write(outword);
                    }
                    Console.WriteLine();
                    Thread.Sleep(5);
                    Application.DoEvents();
                    if (btnStart.Enabled)
                    {
                        break;
                    }
                }
            }
            serOut.Close();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            Wfdb.Quit();
        }

        private void cmbComPort_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //txtFile.Enabled = true;
            //tnFileSelect.Enabled = true;
            serOut.PortName = "COM" + cmbComPort.SelectedItem.ToString().Trim();
            btnStart.Enabled = true;
        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            //check if a valid file...
            if (File.Exists(txtFile.Text))
            {
                btnStart.Enabled = true;
                btnStop.Enabled = true;
            }
            else
            {
                btnStart.Enabled = false;
                btnStop.Enabled = false;
            }
        }

        private void btnFileSelect_Click(object sender, EventArgs e)
        {
            dlgFileSelect.Filter = "Physionet header files (*.head)|*.hea|All files (*.*)|*.*";
            dlgFileSelect.FilterIndex = 1;
            if (dlgFileSelect.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = dlgFileSelect.FileName;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //cmbComPort.Items = serOut
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            serOut.Close();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void serOut_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            txtSerIn.Text += e.ToString();
        }

        private void btnClearBox_Click(object sender, EventArgs e)
        {
            txtSerIn.Text = "";
        }

    }
}
