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
using Ekona;

namespace Sounds
{
    public partial class SoundControl : UserControl
    {
        SoundBase soundBase;
        IPluginHost pluginHost;

        SoundPlayer soundPlayer;
        string wav_file;
        string wav_loop_file;
        Thread loop;

        public SoundControl()
        {
            InitializeComponent();
        }
        public SoundControl(SoundBase sb, IPluginHost pluginHost)
        {
            InitializeComponent();

            this.soundBase = sb;
            this.pluginHost = pluginHost;

            this.wav_file = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            soundBase.Save_WAV(wav_file, false);
            if (soundBase.CanLoop)
            {
                wav_loop_file = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
                soundBase.Save_WAV(wav_loop_file, true);
            }
  
            checkLoop.Enabled = soundBase.CanLoop;
            btnImport.Enabled = soundBase.CanEdit;

            ReadLanguage();
            Information();
        }

        private void ReadLanguage()
        {
            try
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
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }
        private void Information()
        {
            // Remove older values
            if (listProp.Items[0].SubItems.Count == 2)
                for (int i = 0; i < listProp.Items.Count; i++)
                    listProp.Items[i].SubItems.RemoveAt(1);

            listProp.Items[0].SubItems.Add(soundBase.Format);
            listProp.Items[1].SubItems.Add(soundBase.Channels.ToString());
            listProp.Items[2].SubItems.Add(soundBase.CanLoop.ToString());
            if (soundBase.CanLoop)
            {
                listProp.Items[3].SubItems.Add("0x" + soundBase.LoopBegin.ToString("x"));
                listProp.Items[4].SubItems.Add("0x" + soundBase.LoopEnd.ToString("x"));
            }
            listProp.Items[5].SubItems.Add(soundBase.SampleRate + " Hz");
            listProp.Items[6].SubItems.Add("0x" + soundBase.NumberSamples.ToString("x"));
            listProp.Items[7].SubItems.Add("0x" + soundBase.BlockSize.ToString("x"));
            listProp.Items[8].SubItems.Add("0x" + soundBase.SampleBitDepth.ToString("x"));
            listProp.Items[9].SubItems.Add(soundBase.Copyright);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            btnStop.PerformClick();

            if (!checkLoop.Checked)
            {
                soundPlayer = new SoundPlayer(wav_file);
                soundPlayer.Play();
            }
            else
            {
                loop = new Thread(Thread_Loop);
                loop.Start(new String[] { wav_file, wav_loop_file });
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
                soundBase.Save_WAV(o.FileName, false);

            if (soundBase.CanLoop)
            {
                XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                    Path.DirectorySeparatorChar + "SoundLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("SoundControl");
                if (MessageBox.Show(xml.Element("S05").Value, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (o.ShowDialog() == DialogResult.OK)
                    {
                        string path = Path.GetDirectoryName(o.FileName) + Path.DirectorySeparatorChar + 
                            Path.GetFileNameWithoutExtension(o.FileName) + "_loop.wav";
                        soundBase.Save_WAV(path, true);
                    }
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.DefaultExt = ".wav";
            o.Filter = "WAV sound file (*.wav)|*.wav";
            o.Multiselect = false;

            if (o.ShowDialog() != DialogResult.OK)
                return;

			try {
	            string imported = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
				soundBase.Import(o.FileName);

	            byte[] enc_data = soundBase.Encode();
	            soundBase.Write_File(imported, enc_data);

				wav_file = o.FileName;
				pluginHost.ChangeFile(soundBase.ID, imported);
			} catch (NotImplementedException ex) {
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
        }
    }
}
