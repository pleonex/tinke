using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Tinke
{
    public partial class VisorHex : Form
    {
        Espera espera;
        int nByte;

        public VisorHex(string file, UInt32 offset, UInt32 size)
        {
            InitializeComponent();

            // TODO: Mejorar el rendimiento del visor Hexadecimal leyendo exclusivamente lo visible
            espera = new Espera("Leyendo datos...", true);
            nByte = (int)size;

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = offset;

            byte[] bytesFile = br.ReadBytes((int)size);
            br.Close();
            br.Dispose();

            backgroundWorker1.RunWorkerAsync(bytesFile);
            espera.ShowDialog();
        }

        public void Clear()
        {
            txtHex.Text = "";
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            espera.Set_ProgressValue((e.ProgressPercentage * 100) / nByte);
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[] value = (byte[])e.Argument;
            string text = "";

            for (int i = 0; ; )
            {
                string ascii = "";
                for (int j = 0; j < 0x10; j++)
                {
                    if (i >= value.Length) break;
                    text += Tools.Helper.DecToHex(value[i]) + ' ';
                    ascii += (value[i] > 0x1F && value[i] < 0x7F ? Char.ConvertFromUtf32(value[i]).ToString() + ' ' : ". ");
                    i++;
                    backgroundWorker1.ReportProgress(i);
                }
                text += new String(' ', 40 - ascii.Length) + ascii;
                text += "\r\n";
                if (i >= value.Length) break;
            }

            e.Result = text;
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            espera.Close();
            txtHex.Text = (string)e.Result;
            txtHex.Select(0, 0);
        }

    }
}
