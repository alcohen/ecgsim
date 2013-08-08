using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WfdbCsharpWrapper;

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
            string inword;
            int cnt=0;
            int totCnt = 0;
            int sendCnt = 0;
            int readCnt = 0;
            int iteration = 0;
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
                int loop = 0;
                int val;

                string outword;
                // serOut.BaudRate = 38400;
                serOut.BaudRate = 115200;
                serOut.Open();

                Console.Write("Ser wBuf Size ");
                Console.WriteLine(serOut.WriteBufferSize);
                Console.Write("Ser rBuf Size ");
                Console.WriteLine(serOut.ReadBufferSize);

                    Console.WriteLine("Iteration " + iteration);

                                  while (iteration++ < 100)
                                    {
                                        foreach (var s in samples)
                                        {


                        for (int i = 0; i < s.Length; i++)
                        {

                            loop++;
                            //   Console.WriteLine(loop);
                            //Console.Write(s[i].ToString());
                            //Console.Write(string.Format("D{0000}\t", s[i]));
                            //real         val = s[i];       // real

                            //if ((loop == 1) || (loop == 1000)) 
                            //    Console.WriteLine("BorE " + s[i]);
                            val = (s[i] - 1950) * 10;
                            if (val > 4096) val = 4096;
                            if (val < 0) val = 0;
                            //   val = loop;

                            outword = "D" + val.ToString("0000");
                            serOut.Write(outword);
                            //    Console.WriteLine(outword);


                            if ((readCnt = serOut.BytesToRead) > 0)
                            {
                                //     Console.Write("rCnt:");
                                //     Console.WriteLine(readCnt);
                                inword = serOut.ReadLine();
                                cnt = inword.Count(c => c == '>');
                                totCnt += cnt;

                                Console.Write(inword);
#if VERBOSE                         
                            Console.Write("$");
                            Console.Write(totCnt);
                            Console.Write("$c");
                            Console.WriteLine(sendCnt);
                            
#endif
                                Console.Write("]");
                                //   Console.WriteLine(cnt);
                                //  Thread.Sleep(1);
                            }
                            else
                            {
                                Console.WriteLine(",");
                                Thread.Sleep(1);
                            }
                        }
                        Console.WriteLine();
                        sendCnt++;

                        do
                        {
                            if ((readCnt = serOut.BytesToRead) > 0)
                            {
                                // Console.Write("rCnt:");
                                // Console.WriteLine(readCnt);
                                Console.Write("[");

                                inword = serOut.ReadLine();

                                cnt = inword.Count(c => c == '>');
                                totCnt += cnt;

                                Console.Write(inword);
#if VERBOSE
                       Console.Write("|");
                       Console.Write(totCnt);
                       Console.Write("|c");
                        Console.WriteLine(sendCnt);
                        Console.WriteLine(cnt);
#endif
                                Console.Write("]");
                                Console.Write(totCnt);
                                //  Console.Write("|c");
                                //  Console.WriteLine(cnt);
                                //    Thread.Sleep(1);
                            }
                            else
                            {
                                Console.Write(".");
                                Thread.Sleep(1);
                            }
                        } while ((sendCnt - totCnt) > 30);

                        }

                        Application.DoEvents();
                        if (btnStart.Enabled)
                        {
                            break;
                        }
                    }

            }
                    
                // Wait for final bytes to drain
                do
                {
                    if ((readCnt = serOut.BytesToRead) > 0)
                    {
                        Console.Write("rCnt:");
                        Console.WriteLine(readCnt);
                        inword = serOut.ReadLine();

                        cnt = inword.Count(c => c == '>');
                        totCnt += cnt;
                        Console.Write(inword);
                        Console.Write("-");
                        Console.Write(totCnt);
                        Console.Write("-");
                        Console.WriteLine(sendCnt);
                        Thread.Sleep(10);
                    }
                    else
                    {
                        Console.WriteLine("No1");
                        Thread.Sleep(1000);
                    }
                } while ((sendCnt - totCnt) > 0);
                Console.WriteLine(cnt);
                Console.WriteLine("End 1");
            if ((readCnt = serOut.BytesToRead) > 0)
            {

                inword = serOut.ReadLine();
                 Console.Write(inword);
            } else {
                Console.WriteLine("No2");
                Thread.Sleep(1000);
            }
           

            Thread.Sleep(1000);
            Console.WriteLine("End 2");
            if ((readCnt = serOut.BytesToRead) > 0)
            {

                inword = serOut.ReadLine();
                Console.Write(inword);
            }
            else
            {
                Console.WriteLine("No3.");
                Thread.Sleep(1000);
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
            //btnStart.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            serOut.Close();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void cmbComPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
