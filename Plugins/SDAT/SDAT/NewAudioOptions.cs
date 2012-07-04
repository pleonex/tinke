using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Ekona;

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
        int sampleRate;

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
            comboEncoding.SelectedIndex = 1;
        }
        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                    "Plugins" + Path.DirectorySeparatorChar + "SDATLang.xml");
                xml = xml.Element(pluginHost.Get_Language());
                xml = xml.Element("NewAudioOptions");

                this.Text = xml.Element("S00").Value;
                label1.Text = xml.Element("S01").Value;
                checkLoop.Text = xml.Element("S02").Value;
                groupLoop.Text = xml.Element("S03").Value;
                label2.Text = xml.Element("S04").Value;
                label3.Text = xml.Element("S05").Value;
                label4.Text = xml.Element("S06").Value;
                label5.Text = xml.Element("S07").Value;
                btnAccept.Text = xml.Element("S08").Value;
                label6.Text = xml.Element("S09").Value;
                radioSec.Text = xml.Element("S0A").Value;
                radioMSec.Text = xml.Element("S0B").Value;
                radioSam.Text = xml.Element("S0C").Value;
            }
            catch { throw new Exception("There was an error reading the language file"); }
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
        public int SampleRate
        {
            set { sampleRate = value; }
            get { return sampleRate; }
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
            if (radioSam.Checked)
                loopOffset = (int)numericLoopOffset.Value;
            else if (radioSec.Checked)
                loopOffset = (int)(numericLoopOffset.Value * sampleRate);
            else if (radioMSec.Checked)
                loopOffset = (int)(numericLoopOffset.Value / 1000 * sampleRate);
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

        private void radioSec_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioSec.Checked)
                return;

            numericLoopOffset.Value = (decimal)((float)loopOffset / sampleRate);
        }
        private void radioSam_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioSam.Checked)
                return;
            numericLoopOffset.Value = loopOffset;
        }
        private void radioMSec_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioMSec.Checked)
                return;
            numericLoopOffset.Value = (decimal)((float)loopOffset / sampleRate * 1000);
        }
    }
}
