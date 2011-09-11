using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace EDGEWORTH
{
    public partial class PackControl : UserControl
    {
        IPluginHost pluginHost;

        public PackControl()
        {
            InitializeComponent();
        }
        public PackControl(IPluginHost pluginHost)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Threading.Thread waiting = new System.Threading.Thread(ThreadWait);
            waiting.Start("Packing file...");

            Carpeta unpackedFiles = pluginHost.Get_DecompressedFiles(0xE2); // This is the ID of romfile.bin
            String newRomfile = System.IO.Path.GetTempFileName();
            PACK.Write(newRomfile, unpackedFiles);
            pluginHost.ChangeFile(0xE2, newRomfile);

            waiting.Abort();
        }
        private void ThreadWait(object name)
        {
            Espera wait = new Espera((string)name);
            wait.ShowDialog();
        }
    }
}
