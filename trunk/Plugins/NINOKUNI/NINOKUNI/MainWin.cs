// ----------------------------------------------------------------------
// <copyright file="MainWin.cs" company="none">

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
// <date>28/08/2012 1:30:43</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using Ekona;

namespace NINOKUNI
{
    public partial class MainWin : UserControl
    {
        IPluginHost pluginHost;
        char slash = Path.DirectorySeparatorChar;
        string xmlconf;
        string xmlImport;

        public MainWin()
        {
            InitializeComponent();
        }
        public MainWin(IPluginHost pluginHost)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;

            xmlconf = Application.StartupPath + slash + "Plugins" + slash + "Ninokuni.xml";
            xmlImport = Application.StartupPath + slash + "Plugins" + slash + "Ninokuni" + slash + "ImportFiles.xml";
            XDocument doc = XDocument.Load(xmlconf);
            txtPath.Text = doc.Element("GameConfig").Element("Config").Element("CompilePath").Value;
            doc = null;
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            txtPath.BackColor = Color.Red;

            Compile_Events(txtPath.Text + slash + "Events" + slash);
            Compile_MiniText(txtPath.Text + slash + "Minitextos" + slash);
            Change_Scripts(txtPath.Text + slash + "Scripts" + slash);
            Compile_Subs(txtPath.Text + slash + "Subs" + slash);
            Change_Fonts(txtPath.Text + slash + "Fuentes" + slash);
            Change_System(txtPath.Text + slash + "ftc" + slash);
            Change_Files(@"C:\Users\Benito\Documents\My Dropbox\Ninokuni español\Imágenes\Traducidas");

            txtPath.BackColor = Color.LightGreen;
        }

        void Compile_Events(string path)
        {
            string fout;
            string fin;

            string mainQuest = path + "MainQuest.xml";
            if (File.Exists(mainQuest))
            {
                fout = pluginHost.Get_TempFile();
                fin = pluginHost.Search_File(0xF76);
                MQuestText mqc = new MQuestText(pluginHost, fin, 0xF76);
                mqc.Import(mainQuest);
                mqc.Write(fout);
                pluginHost.ChangeFile(0xF76, fout);
            }

            if (Directory.Exists(path + "SubQuest"))
            {
                string[] sub = Directory.GetFiles(path + "SubQuest", "*.xml", SearchOption.TopDirectoryOnly);
                sFolder subf = pluginHost.Search_Folder(0xF07E);
                for (int i = 0; i < subf.files.Count; i++)
                {
                    string cfile = Array.Find(sub, a => Path.GetFileNameWithoutExtension(a) == subf.files[i].name);
                    if (cfile == null)
                        continue;

                    string tempsub = Save_File(subf.files[i]);
                    fout = pluginHost.Get_TempFile();

                    SQcontrol sqc = new SQcontrol(pluginHost, tempsub, subf.files[i].id);
                    SQ sq = sqc.Read(tempsub);
                    sqc.Import_XML(cfile, ref sq);
                    sqc.Write(fout, sq);

                    pluginHost.ChangeFile(subf.files[i].id, fout);
                }
            }

            string scenario = path + "Scenario.xml";
            if (File.Exists(scenario))
            {
                fout = pluginHost.Get_TempFile();
                fin = pluginHost.Search_File(0xFF2);
                ScenarioText st = new ScenarioText(pluginHost, fin, 0xFF2);
                st.Import(scenario);
                st.Write(fout);
                pluginHost.ChangeFile(0xFF2, fout);
            }

            string system = path + "System.xml";
            if (File.Exists(system))
            {
                fout = pluginHost.Get_TempFile();
                fin = pluginHost.Search_File(0xFF4);
                SystemText st = new SystemText(fin, 0xFF4, pluginHost);
                st.Import(system);
                st.Write(fout);
                pluginHost.ChangeFile(0xFF4, fout);
            }
        }
        void Compile_MiniText(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                if (Path.GetExtension(file) != ".xml")
                    continue;

                sFile cfile = pluginHost.Search_File(Path.GetFileNameWithoutExtension(file) + ".dat").files[0];
                string temp_in = Save_File(cfile);
                string temp_out = pluginHost.Get_TempFile();

                IText itext = NINOKUNI.Text.TextControl.Create_IText(cfile.id);
                itext.Read(temp_in);
                itext.Import(file);
                itext.Write(temp_out);

                pluginHost.ChangeFile(cfile.id, temp_out);
            }
        }
        void Change_Scripts(string path)
        {
            if (Directory.Exists(path + "Map"))
            {
                sFolder map = pluginHost.Search_Folder(0xF119);
                string[] map_xmls = Directory.GetFiles(path + "Map", "*.bin", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < map.files.Count; i++)
                {
                    string smap = Array.Find(map_xmls, a => Path.GetFileName(a) == map.files[i].name);
                    if (smap == null)
                        continue;

                    pluginHost.ChangeFile(map.files[i].id, smap);
                }
            }

            // Mini-script
            if (Directory.Exists(path + "Mini" + Path.DirectorySeparatorChar + "BattleTutorial"))
            {
                sFolder btuto = pluginHost.Search_Folder(0xF02E);
                string[] dirs = Directory.GetDirectories(path + "Mini" + Path.DirectorySeparatorChar + "BattleTutorial");
                for (int i = 0; i < btuto.folders.Count; i++)
                {
                    string cdir = Array.Find(dirs, a => new DirectoryInfo(a).Name == btuto.folders[i].name);
                    if (cdir == null)
                        continue;

                    sFolder cfol = btuto.folders[i];
                    string[] cfiles = Directory.GetFiles(cdir, "*.bin", SearchOption.TopDirectoryOnly);
                    for (int j = 0; j < cfol.files.Count; j++)
                    {
                        string cf = Array.Find(cfiles, a => Path.GetFileName(a) == cfol.files[j].name);
                        if (cf == null)
                            continue;

                        pluginHost.ChangeFile(cfol.files[j].id, cf);
                    }
                }
            }

            if (Directory.Exists(path + "Mini" + Path.DirectorySeparatorChar + "_QuestReport"))
            {
                sFolder map = pluginHost.Search_Folder(0xF02A);
                string[] map_xmls = Directory.GetFiles(path + "Mini" + Path.DirectorySeparatorChar + "_QuestReport"
                    , "*.bin", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < map.files.Count; i++)
                {
                    string smap = Array.Find(map_xmls, a => Path.GetFileName(a) == map.files[i].name);
                    if (smap == null)
                        continue;

                    pluginHost.ChangeFile(map.files[i].id, smap);
                }
            }
        }
        void Compile_Subs(string path)
        {
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                string name = Path.GetFileNameWithoutExtension(files[i]) + ".txt";
                sFile cfile = pluginHost.Search_File(name).files[0];
                string tempFile = Save_File(cfile);
                string fout = pluginHost.Get_TempFile();

                SubtitleControl sc = new SubtitleControl(pluginHost, tempFile, cfile.id);
                sc.ImportXML(files[i]);
                sc.Write(fout);

                pluginHost.ChangeFile(cfile.id, fout);
            }
        }
        void Change_Fonts(string path)
        {
            string[] files = Directory.GetFiles(path);
            sFolder fonts = pluginHost.Search_Folder(0xF081);
            for (int i = 0; i < fonts.files.Count; i++)
            {
                string cf = Array.Find(files, a => Path.GetFileName(a) == fonts.files[i].name);
                if (cf == null)
                    continue;

                pluginHost.ChangeFile(fonts.files[i].id, cf);
            }
        }
        void Change_System(string path)
        {
            string[] files = Directory.GetFiles(path);
            sFolder ftc = pluginHost.Search_Folder(0xF378);
            for (int i = 0; i < ftc.files.Count; i++)
            {
                string cf = Array.Find(files, a => Path.GetFileName(a) == ftc.files[i].name);
                if (cf == null)
                    continue;

                if (ftc.files[i].name == "arm9.bin")
                {
                    byte[] d = File.ReadAllBytes(cf);
                    byte[] size = BitConverter.GetBytes((uint)new FileInfo(cf).Length);
                    d[0xB9C] = size[0];
                    d[0xB9D] = size[1];
                    d[0xB9E] = size[2];
                    File.WriteAllBytes(cf, d);
                }

                pluginHost.ChangeFile(ftc.files[i].id, cf);
            }
        }
        void Change_Files(string path)
        {
            XDocument doc = XDocument.Load(xmlImport);
            XElement root = doc.Element("NinoImport");

            foreach (XElement e in root.Elements("File"))
            {
                int id = Convert.ToInt32(e.Attribute("ID").Value, 16);
                string file_path = path + e.Value;
                if (File.Exists(file_path))
                    pluginHost.ChangeFile(id, file_path);
                else
                    Console.WriteLine("File not found " + file_path + " (" + id.ToString("X") + ')');
            }

            root = null;
            doc = null;
        }

        String Save_File(sFile file)
        {
            String outFile = pluginHost.Get_TempFile();
            if (File.Exists(outFile))
                File.Delete(outFile);

            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            br.BaseStream.Position = file.offset;
            File.WriteAllBytes(outFile, br.ReadBytes((int)file.size));
            br.Close();

            return outFile;
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog o = new FolderBrowserDialog();
            o.Description = "Select the folder with the translated files.";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            txtPath.Text = o.SelectedPath;
            o.Dispose();
            o = null;

            XDocument doc = XDocument.Load(xmlconf);
            doc.Element("GameConfig").Element("Config").Element("CompilePath").Value = txtPath.Text;
            doc.Save(xmlconf);
            doc = null;
        }
    }
}
