// ----------------------------------------------------------------------
// <copyright file="SubtitleControl.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>28/05/2012 23:06:08</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using Ekona;

namespace NINOKUNI
{
    public partial class SubtitleControl : UserControl
    {
        string file;
        int id;
        IPluginHost pluginHost;
        Subtitle[] subs;

        char split = '\x3';

        public SubtitleControl()
        {
            InitializeComponent();
        }
        public SubtitleControl(IPluginHost pluginHost, string file, int id)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.file = file;
            this.id = id;

            subs = Read(file);
            listContent.SelectedIndex = 0;
            listContent_SelectedIndexChanged(null, null);
        }

        private Subtitle[] Read(string file)
        {
            List<Subtitle> subs = new List<Subtitle>();
            string text = File.ReadAllText(file, Encoding.GetEncoding("shift_jis"));
            int pos = 0;

            while (pos < text.Length)
            {
                Subtitle sub = new Subtitle();
                string line = Read_String(text, ref pos);

                if (line.Length == 0)
                    continue;

                if (line.Length > 0 && line[0] == '#')
                {
                    sub.type = SubType.Comment;
                    sub.data = Helper.SJISToLatin(line.Substring(1));
                }
                else if (line.StartsWith("/stream"))
                {
                    sub.type = SubType.Voice;
                    sub.data = line.Substring(8);
                }
                else if (line.StartsWith("/sync"))
                {
                    sub.type = SubType.SyncTime;
                    sub.data = line.Substring(6);
                }
                else if (line.StartsWith("/clear"))
                {
                    sub.type = SubType.Clear;
                }
                else
                {
                    sub.type = SubType.Text;
                    sub.data = Helper.SJISToLatin(line);
                }

                subs.Add(sub);
                listContent.Items.Add(sub.type);
            }

            return subs.ToArray();
        }
        public void Write(string file)
        {
            string text = "";

            for (int i = 0; i < subs.Length; i++)
            {
                switch (subs[i].type)
                {
                    case SubType.Text: text += Helper.LatinToSJIS(subs[i].data); break;
                    case SubType.SyncTime: text += "/sync " + Convert.ToInt32(subs[i].data); break;
                    case SubType.Voice: text += "/stream " + subs[i].data; break;
                    case SubType.Comment: text += '#' + Helper.LatinToSJIS(subs[i].data); break;
                    case SubType.Clear: text += "/clear"; break;
                }

                text += "\xD" + split;
            }

            File.WriteAllText(file, text, Encoding.GetEncoding("shift_jis"));
        }

        private string Read_String(string data, ref int pos)
        {
            string t = "";

            while (pos < data.Length)
            {
                char c = data[pos++];
                if (c == '\xD')
                    break;

                t += c;
            }

            pos++;   // split char, originally: 0x0A, now 0x03

            return t;
        }

        private void listContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            Subtitle sub = subs[listContent.SelectedIndex];

            switch (sub.type)
            {
                case SubType.Comment:
                    comboType.SelectedIndex = 4;
                    numSyncTime.Enabled = false;
                    txtVoice.Enabled = false;
                    txtSub.Enabled = true;
                    txtSub.Text = sub.data.Replace("\n", "\r\n");
                    txtSub.BackColor = Color.DarkSeaGreen;
                    break;

                case SubType.Text:
                    comboType.SelectedIndex = 0;
                    numSyncTime.Enabled = false;
                    txtVoice.Enabled = false;
                    txtSub.Enabled = true;
                    txtSub.Text = sub.data.Replace("\n", "\r\n");
                    txtSub.BackColor = Color.White;
                    break;

                case SubType.SyncTime:
                    comboType.SelectedIndex = 2;
                    numSyncTime.Enabled = true;
                    txtVoice.Enabled = false;
                    txtSub.Enabled = false;
                    numSyncTime.Value = Convert.ToInt32(sub.data);
                    break;

                case SubType.Voice:
                    comboType.SelectedIndex = 3;
                    numSyncTime.Enabled = false;
                    txtVoice.Enabled = true;
                    txtSub.Enabled = false;
                    txtVoice.Text = sub.data;
                    break;


                case SubType.Clear:
                    comboType.SelectedIndex = 1;
                    numSyncTime.Enabled = false;
                    txtVoice.Enabled = false;
                    txtSub.Enabled = false;
                    break;
            }
        }

        private void txtSub_TextChanged(object sender, EventArgs e)
        {
            subs[listContent.SelectedIndex].data = txtSub.Text.Replace("\r\n", "\n");
        }
        private void txtVoice_TextChanged(object sender, EventArgs e)
        {
            subs[listContent.SelectedIndex].data = txtVoice.Text;
        }
        private void numSyncTime_ValueChanged(object sender, EventArgs e)
        {
            subs[listContent.SelectedIndex].data = numSyncTime.Value.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string fileout = file + ".bin";
            if (File.Exists(fileout))
                return;

            Write(fileout);
            pluginHost.ChangeFile(id, fileout);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.FileName = Path.GetFileNameWithoutExtension(file).Substring(12) + ".xml";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            ExportXML(o.FileName);
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "Subtitles XML|*.xml|All files|*.*";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            ImportXML(o.FileName);

            listContent.Items.Clear();
            for (int i = 0; i < subs.Length; i++)
                listContent.Items.Add(subs[i].type);
            listContent.SelectedIndex = 0;
            listContent_SelectedIndexChanged(null, null);


            // Write file
            btnSave_Click(null, null);
        }

        private void ExportXML(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = doc.CreateElement("Subtitle");
            root.SetAttribute("Name", Path.GetFileNameWithoutExtension(file).Substring(12));

            for (int i = 0; i < subs.Length; i++)
            {
                XmlElement e;

                switch (subs[i].type)
                {
                    case SubType.Text: e = doc.CreateElement("Text"); e.InnerText = Helper.Format(subs[i].data, 4); break;
                    case SubType.SyncTime: e = doc.CreateElement("Sync"); e.SetAttribute("Time", subs[i].data); break;
                    case SubType.Voice: e = doc.CreateElement("Voice"); e.SetAttribute("File", subs[i].data); break;
                    case SubType.Comment: e = doc.CreateElement("Comment"); e.InnerText = Helper.Format(subs[i].data, 4); break;
                    case SubType.Clear: e = doc.CreateElement("Clear"); break;

                    default: e = doc.CreateElement("Unknown"); break;
                }

                root.AppendChild(e);
            }

            doc.AppendChild(root);
            doc.Save(xml);
        }
        public void ImportXML(string xml)
        {
            List<Subtitle> subs = new List<Subtitle>();

            XmlDocument doc = new XmlDocument();
            doc.Load(xml);

            XmlNode root = doc.ChildNodes[1];
            foreach (XmlNode n in root.ChildNodes)
            {
                if (n.NodeType != XmlNodeType.Element)
                    continue;

                Subtitle sub = new Subtitle();
                switch (n.Name)
                {
                    case "Text":
                        sub.type = SubType.Text;
                        sub.data = Helper.Reformat(n.InnerText, 4, true).Replace("\r\n", "\n");

                        // Check for errors
                        if (!Helper.Check(sub.data, 0))
                        {
                            string fill_text = sub.data;
                            bool check = false;

                            while (!check && fill_text.Length > 0)
                            {
                                fill_text = fill_text.Remove(fill_text.Length - 1);
                                check = Helper.Check(fill_text, 0);
                            }
                            MessageBox.Show("Text error\n\nWrong line:\n" + sub.data + "\n\nGood line:\n" + fill_text);
                            //continue;
                        }
                        break;
                    case "Comment":
                        sub.type = SubType.Comment;
                        sub.data = Helper.Reformat(n.InnerText, 4, true).Replace("\r\n", "\n");
                        break;
                    case "Voice":
                        sub.type = SubType.Voice;
                        sub.data = n.Attributes["File"].Value;
                        break;
                    case "Sync":
                        sub.type = SubType.SyncTime;
                        sub.data = n.Attributes["Time"].Value;
                        break;
                    case "Clear":
                        sub.type = SubType.Clear;
                        break;
                }

                subs.Add(sub);
            }

            this.subs = subs.ToArray();
        }
    }

    struct Subtitle
    {
        public SubType type;
        public string data;
    }
    enum SubType
    {
        Text,
        SyncTime,
        Voice,
        Comment,
        Clear
    }
}
