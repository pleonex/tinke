using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PluginInterface;

namespace Common
{
    public partial class iWav : UserControl
    {
        IPluginHost pluginHost;
        System.Media.SoundPlayer snd;

        public iWav(string archivo, IPluginHost pluginHost)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            ReadLanguage();

            FileStream fs = new FileStream(archivo, FileMode.Open);
            snd = new System.Media.SoundPlayer(fs);
            snd.Load();
            fs.Close();
            fs.Dispose();

            btnPlay.PerformClick();
        }
        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                    "Plugins" + Path.DirectorySeparatorChar + "CommonLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("iWav");

                btnPlay.Text = xml.Element("S00").Value;
                btnStop.Text = xml.Element("S01").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            snd.Play();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            snd.Stop();
        }
    }
}
