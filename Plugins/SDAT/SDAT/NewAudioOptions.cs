using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace SDAT
{
    public partial class NewAudioOptions : Form
    {
        IPluginHost pluginHost;

        public NewAudioOptions()
        {
            InitializeComponent();
        }
        public NewAudioOptions(IPluginHost pluginHost, bool isSWAV)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;

            ReadLanguage();
            numericLoopLength.Enabled = isSWAV;
            numericBlockLen.Enabled = !isSWAV;
        }
        private void ReadLanguage()
        {

        }

        private void checkLoop_CheckedChanged(object sender, EventArgs e)
        {
            groupLoop.Enabled = checkLoop.Checked;
        }

        public int Compression
        {
            get { return comboEncoding.SelectedIndex; }
            set { comboEncoding.SelectedIndex = value; }
        }
        public bool Loop
        {
            get { return checkLoop.Checked; }
            set { checkLoop.Checked = value; }
        }
        public int LoopOffset
        {
            get { return (int)numericLoopOffset.Value; }
            set { numericLoopOffset.Value = value; }
        }
        public int LoopLength
        {
            get { return (int)numericLoopLength.Value; }
            set { numericLoopLength.Value = value; }
        }
        public int BlockSize
        {
            get { return (int)numericBlockLen.Value; }
            set { numericBlockLen.Value = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
