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
        String lang;

        public PackControl()
        {
            InitializeComponent();
        }
        public PackControl(IPluginHost pluginHost)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            ReadLanguage();
        }
        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "EDGEWORTHLang.xml");
                xml = xml.Element(pluginHost.Get_Language());

                btnPack.Text = xml.Element("S00").Value;
                lang = xml.Element("S01").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Threading.Thread waiting = new System.Threading.Thread(ThreadWait);
            waiting.Start(lang);

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
