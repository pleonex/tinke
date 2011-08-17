using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Media;
using System.Threading;
using PluginInterface;

namespace Sounds
{
    public partial class SoundControl : UserControl
    {
        sWAV sound;
        string wavFile;
        string loopFile;
        IPluginHost pluginHost;

        SoundPlayer soundPlayer;
        Thread loop;

        public SoundControl()
        {
            InitializeComponent();
        }
        public SoundControl(sWAV sound, IPluginHost pluginHost)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            ReadLanguage();

            this.sound = sound;
            wavFile = Path.GetTempFileName();
            WAV.Write(sound, wavFile);

            if (sound.loopFlag != 0)
                WAV.Write_Loop(sound, loopFile);
            else
                checkLoop.Enabled = false;

            Information();
        }
        private void ReadLanguage()
        {
            XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                Path.DirectorySeparatorChar + "SoundLang.xml");
            xml = xml.Element(pluginHost.Get_Language()).Element("SoundControl");

            columnProper.Text = xml.Element("S00").Value;
            columnValue.Text = xml.Element("S01").Value;
            btnExport.Text = xml.Element("S02").Value;
            btnImport.Text = xml.Element("S03").Value;
            checkLoop.Text = xml.Element("S04").Value;

            listProp.Items[0].Text = xml.Element("S06").Value;
            listProp.Items[1].Text = xml.Element("S07").Value;
            listProp.Items[2].Text = xml.Element("S08").Value;
            listProp.Items[3].Text = xml.Element("S09").Value;
            listProp.Items[4].Text = xml.Element("S0A").Value;
            listProp.Items[5].Text = xml.Element("S0B").Value;
            listProp.Items[6].Text = xml.Element("S0C").Value;
            listProp.Items[7].Text = xml.Element("S0D").Value;
            listProp.Items[8].Text = xml.Element("S0E").Value;
            listProp.Items[9].Text = xml.Element("S0F").Value;
            listProp.Items[10].Text = xml.Element("S10").Value;
            listProp.Items[11].Text = xml.Element("S11").Value;
            listProp.Items[12].Text = xml.Element("S12").Value;
            listProp.Items[13].Text = xml.Element("S13").Value;
            listProp.Items[14].Text = xml.Element("S14").Value;
        }

        private void Information()
        {
            // Remove older values
            if (listProp.Items[0].SubItems.Count == 2)
                for (int i = 0; i < listProp.Items.Count; i++)
                    listProp.Items[i].SubItems.RemoveAt(1);

            listProp.Items[0].SubItems.Add(new String(sound.chunkID));
            listProp.Items[1].SubItems.Add(sound.chunkSize.ToString());
            listProp.Items[2].SubItems.Add(sound.loopFlag.ToString());
            listProp.Items[3].SubItems.Add("0x" + String.Format("{0:X}", sound.loopOffset));
            listProp.Items[4].SubItems.Add(new String(sound.format));
            listProp.Items[5].SubItems.Add(new String(sound.wave.fmt.chunkID));
            listProp.Items[6].SubItems.Add(sound.wave.fmt.chunkSize.ToString());
            listProp.Items[7].SubItems.Add(Enum.GetName(typeof(WaveFormat), sound.wave.fmt.audioFormat));
            listProp.Items[8].SubItems.Add(sound.wave.fmt.numChannels.ToString());
            listProp.Items[9].SubItems.Add(sound.wave.fmt.sampleRate.ToString() + " Hz");
            listProp.Items[10].SubItems.Add("0x" + String.Format("{0:X}" ,sound.wave.fmt.byteRate));
            listProp.Items[11].SubItems.Add(sound.wave.fmt.blockAlign.ToString());
            listProp.Items[12].SubItems.Add(sound.wave.fmt.bitsPerSample.ToString());
            listProp.Items[13].SubItems.Add(new String(sound.wave.data.chunkID));
            listProp.Items[14].SubItems.Add(sound.wave.data.chunkSize.ToString());
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            btnStop.PerformClick();

            if (checkLoop.Checked)
            {
                loop = new Thread(Thread_Loop);
                loop.Start(new String[] { wavFile, loopFile });
            }
            else
            {
                soundPlayer = new SoundPlayer(wavFile);
                soundPlayer.Play();
            }
        }
        private void Thread_Loop(Object e)
        {
            string wave = ((String[])e)[0];
            string loopWave = ((String[])e)[1];

            SoundPlayer soundLoop = new SoundPlayer(loopWave);
            soundPlayer = new SoundPlayer(wave);

            soundPlayer.PlaySync();
            soundLoop.PlayLooping();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (soundPlayer is SoundPlayer)
                soundPlayer.Stop();
            if (loop is Thread)
                if (loop.ThreadState == ThreadState.Running)
                    loop.Abort();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = "wav";
            o.Filter = "WAVE audio (*.wav)|*.wav";
            o.OverwritePrompt = true;
            if (o.ShowDialog() == DialogResult.OK)
                File.Copy(wavFile, o.FileName, true);

            if (sound.loopFlag != 0)
            {
                XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                    Path.DirectorySeparatorChar + "SoundLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("SoundControl");
                if (MessageBox.Show(xml.Element("S05").Value, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    if (o.ShowDialog() == DialogResult.OK)
                        File.Copy(loopFile, o.FileName, true);
            }
        }
    }
}
