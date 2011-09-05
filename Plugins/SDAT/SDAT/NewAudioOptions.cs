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

        bool loopFlag;
        int loopOffset;
        int loopLength;
        int compressFormat;
        int blockLength;
        int volume;

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
            numericVolume.Enabled = isSWAV;
        }
        private void ReadLanguage()
        {

        }

        private void checkLoop_CheckedChanged(object sender, EventArgs e)
        {
            groupLoop.Enabled = checkLoop.Checked;
            loopFlag = checkLoop.Checked;
        }

        public int Compression
        {
            get { return compressFormat; }
            set { comboEncoding.SelectedIndex = value; compressFormat = value; }
        }
        public bool Loop
        {
            get { return loopFlag; }
            set { checkLoop.Checked = value; loopFlag = value; }
        }
        public int LoopOffset
        {
            get { return loopOffset; }
            set { numericLoopOffset.Value = value; loopOffset = value; }
        }
        public int LoopLength
        {
            get { return loopLength; }
            set { numericLoopLength.Value = value; loopLength = value; }
        }
        public int BlockSize
        {
            get { return blockLength; }
            set { numericBlockLen.Value = value; blockLength = value; }
        }
        public int Volume
        {
            get { return volume; }
            set { numericVolume.Value = value; volume = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            compressFormat = comboEncoding.SelectedIndex;
        }
        private void numericLoopOffset_ValueChanged(object sender, EventArgs e)
        {
            loopOffset = (int)numericLoopOffset.Value;
        }
        private void numericLoopLength_ValueChanged(object sender, EventArgs e)
        {
            loopLength = (int)numericLoopLength.Value;
        }
        private void numericBlockLen_ValueChanged(object sender, EventArgs e)
        {
            blockLength = (int)numericBlockLen.Value;
        }
        private void numericVolume_ValueChanged(object sender, EventArgs e)
        {
            volume = (int)numericVolume.Value;
        }
    }
}
