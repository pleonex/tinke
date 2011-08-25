/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * Programador: pleoNeX
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Threading;
using PluginInterface;

namespace SDAT
{
    public partial class iSDAT : UserControl
    {
        sSDAT sdat;
        SoundPlayer soundPlayer;
        string wavFile = "";
        string loopFile = "";
        Thread loop;
        IPluginHost pluginHost;

        uint lastFolderID;
        uint lastFileID;

        public iSDAT()
        {
            InitializeComponent();
        }
        public iSDAT(sSDAT sdat, IPluginHost pluginHost)
        {
            InitializeComponent();

            this.sdat = sdat;
            this.pluginHost = pluginHost;
            ReadLanguage();

            treeFiles.Nodes.Add(CarpetaToNodo(sdat.files.root));
            treeFiles.Nodes[0].Expand();

            Set_LastFileID(sdat.files.root);
            Set_LastFolderID(sdat.files.root);
            lastFileID++;
            lastFolderID++;
        }
        protected override void Dispose(bool disposing)
        {
            if (loop is Thread)
                if (loop.ThreadState == ThreadState.Running)
                    loop.Abort();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + "\\Plugins\\SDATLang.xml");
                xml = xml.Element(pluginHost.Get_Language());
                xml = xml.Element("iSDAT");

                columnCampo.Text = xml.Element("S00").Value;
                columnValor.Text = xml.Element("S01").Value;
                listProp.Items[0].Text = xml.Element("S02").Value;
                listProp.Items[1].Text = xml.Element("S03").Value;
                listProp.Items[2].Text = xml.Element("S04").Value;
                listProp.Items[3].Text = xml.Element("S05").Value;
                checkLoop.Text = xml.Element("S06").Value;
                btnUncompress.Text = xml.Element("S07").Value;
                btnExtract.Text = xml.Element("S08").Value;
                btnMidi.Text = xml.Element("S09").Value;
                btnWav.Text = xml.Element("S0A").Value;
                btnImport.Text = xml.Element("S0B").Value;
                btnCreate.Text = xml.Element("S0C").Value;
                btnChangeFile.Text = xml.Element("S0D").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); } 
        }

        #region System folder administration
        private TreeNode CarpetaToNodo(Folder carpeta)
        {
            TreeNode currNode = new TreeNode();

            currNode = new TreeNode(carpeta.name, 0, 0);
            currNode.Tag = carpeta.id;
            currNode.Name = carpeta.name;


            if (carpeta.folders is List<Folder>)
                foreach (Folder subFolder in carpeta.folders)
                    currNode.Nodes.Add(CarpetaToNodo(subFolder));


            if (carpeta.files is List<Sound>)
            {
                foreach (Sound archivo in carpeta.files)
                {
                    int formato = FormatoToImage(archivo.type);
                    TreeNode fileNode = new TreeNode(archivo.name, formato, formato);
                    fileNode.Name = archivo.name;
                    fileNode.Tag = archivo.id;
                    currNode.Nodes.Add(fileNode);
                }
            }

            return currNode;

        }
        private int FormatoToImage(FormatSound formato)
        {
            switch (formato)
            {
                case FormatSound.SSEQ:
                    //return 2; No soportado este formato aún
                    return 8;
                case FormatSound.SSAR:
                    //return 2; No soportado este formato aún
                    return 8;
                case FormatSound.SBNK:
                    //return 4; No soportado este formato aún
                    return 8;
                case FormatSound.SWAV:
                    return 1;
                case FormatSound.SWAR:
                    return 7;
                case FormatSound.STRM:
                    return 1;
                default:
                    return 1;
            }
        }

        private void treeFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
             // Limpiar información anterior
            btnReproducir.Enabled = false;
            btnWav.Enabled = false;
            btnMidi.Enabled = false;
            btnImport.Enabled = false;
            btnUncompress.Enabled = false;
            btnChangeFile.Enabled = true;
            if (listProp.Items[0].SubItems.Count == 2)
                for (int i = 0; i < listProp.Items.Count; i++)
                    listProp.Items[i].SubItems.RemoveAt(1);

            int id = Convert.ToInt32(e.Node.Tag);
            if (id < 0x0F000)
            {
                Sound fileSelect = SearchFile(id, sdat.files.root);

                listProp.Items[0].SubItems.Add("0x" + String.Format("{0:X}", fileSelect.id));
                listProp.Items[1].SubItems.Add(fileSelect.name);
                listProp.Items[2].SubItems.Add("0x" + String.Format("{0:X}", fileSelect.offset));
                listProp.Items[3].SubItems.Add(fileSelect.size.ToString());

                btnExtract.Enabled = true;
                if (fileSelect.type == FormatSound.SWAV || fileSelect.type == FormatSound.STRM)
                {
                    btnReproducir.Enabled = true;
                    btnWav.Enabled = true;
                }
                else if (fileSelect.type == FormatSound.SWAR)
                {
                    btnUncompress.Enabled = true;
                }
                else if (fileSelect.type == FormatSound.SWAV)
                    btnChangeFile.Enabled = false;
                if (fileSelect.type == FormatSound.STRM)
                {
                    checkLoop.Enabled = true;
                    btnImport.Enabled = true;
                }
            }
            else
            {
                Folder folSelect = SearchFolder(id, sdat.files.root);

                listProp.Items[0].SubItems.Add("0x" + String.Format("{0:X}", folSelect.id));
                listProp.Items[1].SubItems.Add(folSelect.name);
                listProp.Items[2].SubItems.Add("");
                listProp.Items[3].SubItems.Add("");

                btnExtract.Enabled = true;
                btnChangeFile.Enabled = false;
            }
        }

        private Sound SearchFile()
        {
            return SearchFile(Convert.ToInt32(treeFiles.SelectedNode.Tag), sdat.files.root);
        }
        private Sound SearchFile(int id, Folder carpeta)
        {
            if (carpeta.files is List<Sound>)
                foreach (Sound archivo in carpeta.files)
                    if (archivo.id == id)
                        return archivo;


            if (carpeta.folders is List<Folder>)
            {
                foreach (Folder subFolder in carpeta.folders)
                {
                    Sound currFile = SearchFile(id, subFolder);
                    if (currFile.name is String)
                        return currFile;
                }
            }


            return new Sound();
        }
        private Folder SearchFolder(int id, Folder carpeta)
        {
            if (carpeta.id == id)
                return carpeta;

            if (carpeta.folders is List<Folder>)
            {
                foreach (Folder subFolder in carpeta.folders)
                {
                    Folder currFolder = SearchFolder(id, subFolder);
                    if (currFolder.name is String)
                        return currFolder;
                }
            }


            return new Folder();
        }
        public void Set_LastFileID(Folder currFolder)
        {
            if (currFolder.files is List<Sound>)
                foreach (Sound archivo in currFolder.files)
                    if (archivo.id > lastFileID)
                        lastFileID = archivo.id;

            if (currFolder.folders is List<Folder>)
                foreach (Folder subFolder in currFolder.folders)
                    Set_LastFileID(subFolder);

        }
        public void Set_LastFolderID(Folder currFolder)
        {
            if (currFolder.id > lastFolderID)
                lastFolderID = currFolder.id;

            if (currFolder.folders is List<Folder>)
                foreach (Folder subFolder in currFolder.folders)
                    Set_LastFolderID(subFolder);
        }
        public Folder Add_Files(Folder files, int id, Folder currFolder)
        {
            if (currFolder.files is List<Sound>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        files.id = (ushort)lastFolderID;
                        lastFolderID++;
                        currFolder.files.RemoveAt(i);
                        if (!(currFolder.folders is List<Folder>))
                            currFolder.folders = new List<Folder>();
                        currFolder.folders.Add(files);
                        return currFolder;
                    }
                }
            }


            if (currFolder.folders is List<Folder>)
            {
                foreach (Folder subFolder in currFolder.folders)
                {
                    Folder folder = Add_Files(files, id, subFolder);
                    if (folder.name is string)
                    {
                        currFolder.folders.Remove(subFolder);
                        currFolder.folders.Add(folder);
                        currFolder.folders.Sort(Comparacion_Directorios);
                        return currFolder;
                    }
                }
            }

            return new Folder();
        }
        private static int Comparacion_Directorios(Folder f1, Folder f2)
        {
            return String.Compare(f1.name, f2.name);
        }

        public void ChangeFile(int id, Sound fileChanged, Folder currFolder)
        {
            if (currFolder.files is List<Sound>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        currFolder.files[i] = fileChanged;
                    }
                }
            }


            if (currFolder.folders is List<Folder>)
                foreach (Folder subFolder in currFolder.folders)
                    ChangeFile(id, fileChanged, subFolder);
        }
        public void ChangeFile(int id, string newFilePath)
        {
            Sound newFile = new Sound();
            Sound oldFile = SearchFile(id, sdat.files.root);
            newFile.name = oldFile.name;
            newFile.id = (ushort)id;
            newFile.offset = 0x00;
            newFile.path = newFilePath;
            newFile.type = oldFile.type;
            newFile.size = (uint)new FileInfo(newFilePath).Length;

            ChangeFile(id, newFile, sdat.files.root);
        }
        #endregion

        private void btnCreate_Click(object sender, EventArgs e)
        {
            String fileout = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            Save_NewSDAT(fileout);
            sdat.archivo = fileout;

            pluginHost.ChangeFile(sdat.id, fileout);
        }
        private void btnExtract_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(treeFiles.SelectedNode.Tag);
            if (id < 0x0F000)
            {
                Sound currFile = SearchFile(id, sdat.files.root);

                SaveFileDialog o = new SaveFileDialog();
                o.FileName = currFile.name;
                if (o.ShowDialog() == DialogResult.OK)
                {
                    if (currFile.offset == 0x00)
                    {
                        File.Copy(currFile.path, o.FileName);
                        return;
                    }

                    BinaryReader br = new BinaryReader(new FileStream(sdat.archivo, FileMode.Open));
                    br.BaseStream.Position = currFile.offset;
                    File.WriteAllBytes(o.FileName, br.ReadBytes((int)currFile.size));
                    br.Close();
                }
            }
            else
            {
                Folder currFolder = SearchFolder(id, sdat.files.root);

                FolderBrowserDialog o = new FolderBrowserDialog();
                o.ShowNewFolderButton = true;
                o.Description = "Seleccione la carpeta donde se guardarán los archivos.";
                if (o.ShowDialog() == DialogResult.OK)
                {
                    Directory.CreateDirectory(o.SelectedPath + '\\' + currFolder.name);
                    RecursivoExtractFolder(currFolder,o.SelectedPath + '\\' + currFolder.name);
                }
            }
        }
        private void RecursivoExtractFolder(Folder currFolder, String path)
        {
            if (currFolder.files is List<Sound>)
                foreach (Sound archivo in currFolder.files)
                {
                        BinaryReader br = new BinaryReader(File.OpenRead(sdat.archivo));
                        br.BaseStream.Position = archivo.offset;
                        File.WriteAllBytes(path + '\\' + archivo.name, br.ReadBytes((int)archivo.size));
                        br.Close();
                }

            if (currFolder.folders is List<Folder>)
            {
                foreach (Folder subFolder in currFolder.folders)
                {
                    Directory.CreateDirectory(path + '\\' + subFolder.name);
                    RecursivoExtractFolder(subFolder, path + '\\' + subFolder.name);
                }
            }
        }
        private void btnUncompress_Click(object sender, EventArgs e)
        {
            btnUncompress.Enabled = false;
            string swar = SaveSelectedFile();

            sSWAV[] archivos = SWAR.ConvertToSWAV(SWAR.Read(swar));
            string[] swav = new string[archivos.Length];
            
            Folder carpeta = new Folder();
            carpeta.name = SearchFile().name;
            carpeta.files = new List<Sound>();

            for (int i = 0; i < archivos.Length; i++)
            {
                swav[i] = pluginHost.Get_TempFolder() + '\\' + Path.GetRandomFileName();
                SWAV.EscribirArchivo(archivos[i], swav[i]);

                Sound newSound = new Sound();
                newSound.id = lastFileID;
                lastFileID++;
                newSound.internalID = newSound.id;
                newSound.name = "SWAV_" + i.ToString() + ".swav";
                newSound.offset = 0x00;
                newSound.type = FormatSound.SWAV;
                newSound.path = swav[i];
                newSound.size = (uint)new FileInfo(swav[i]).Length;

                carpeta.files.Add(newSound);
            }

            // Lo añadimos al nodo
            sdat.files.root = Add_Files(carpeta, (int)SearchFile().id, sdat.files.root);
            
            TreeNode selected =  treeFiles.SelectedNode;
            selected = CarpetaToNodo(carpeta);

            // Agregamos los nodos al árbol
            TreeNode[] nodos = new TreeNode[selected.Nodes.Count];
            selected.Nodes.CopyTo(nodos, 0);
            treeFiles.SelectedNode.Tag = selected.Tag;
            selected.Nodes.Clear();

            treeFiles.SelectedNode.Nodes.AddRange((TreeNode[])nodos);
            treeFiles.SelectedNode.Expand();

        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            Sound selectedFile = SearchFile();
            String fileout = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();

            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.AddExtension = true;
            o.DefaultExt = "wav";
            o.Filter = "WAVE audio format (*.wav)|*.wav";
            if (o.ShowDialog() != DialogResult.OK)
                return;
            String filein = o.FileName;

            switch (selectedFile.type)
            {
                case FormatSound.STRM:
                    sWAV wav = WAV.Read(filein);
                    sSTRM strm = STRM.ConvertToSTRM(wav);
                    STRM.Write(strm, fileout);
                    break;

                case FormatSound.SSEQ:
                case FormatSound.SSAR:
                case FormatSound.SBNK:
                case FormatSound.SWAV:
                case FormatSound.SWAR:
                default:
                    break;
            }

            if (!File.Exists(fileout))
                return;
            ChangeFile((int)selectedFile.id, fileout);
        }
        private void btnChangeFile_Click(object sender, EventArgs e)
        {
            Sound selectedFile = SearchFile();

            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.AddExtension = true;
            o.DefaultExt = "strm";
            o.Filter = "STRM (*.strm)|*.strm|" +
                "SSEQ (*.sseq)|*.sseq|" +
                "SSAR (*.ssar)|*.ssar|" +
                "SBNK (*.sbnk)|*.sbnk|" +
                "SWAR (*.swar)|*.swar";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            ChangeFile((int)selectedFile.id, o.FileName);
        }

        private void btnWav_Click(object sender, EventArgs e)
        {
            string sound = SaveSelectedFile();

            SaveFileDialog o = new SaveFileDialog();
            o.FileName = SearchFile().name;
            o.Filter = "WAVE (*.wav)|*.wav";
            if (o.ShowDialog() == DialogResult.OK)
            {
                string wavSaved = o.FileName;

                switch(SearchFile().type)
                {
                    case FormatSound.SWAV:
                        WAV.Write(SWAV.ConvertirAWAV(SWAV.LeerArchivo(sound)), wavSaved);
                        break;
                    case FormatSound.STRM:
                        WAV.Write(STRM.ConvertToWAV(STRM.Read(sound), false), wavSaved);
                        break;
                }
            }

            File.Delete(sound);
        }
        private void btnReproducir_Click(object sender, EventArgs e)
        {
            try
            {
                btnStop.PerformClick();

                if (File.Exists(wavFile))
                    File.Delete(wavFile);
                if (File.Exists(loopFile))
                    File.Delete(loopFile);

                string sound = SaveSelectedFile();
                wavFile = Path.GetTempFileName();
                if (checkLoop.Checked)
                    loopFile = Path.GetTempFileName();

                switch(SearchFile().type)
                {
                    case FormatSound.SWAV:
                        WAV.Write(SWAV.ConvertirAWAV(SWAV.LeerArchivo(sound)), wavFile);
                        break;
                    case FormatSound.STRM:
                        WAV.Write(STRM.ConvertToWAV(STRM.Read(sound), false), wavFile);
                        WAV.Write(STRM.ConvertToWAV(STRM.Read(sound), true), loopFile);
                        break;
                }

                File.Delete(sound);

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
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

        private string SaveSelectedFile()
        {
            Sound fileSelect = SearchFile();
            string file = Path.GetTempFileName();

            if (fileSelect.offset == 0x00)
            {
                File.Copy(fileSelect.path, file, true);
                return file;
            }

            BinaryReader br = new BinaryReader(new FileStream(sdat.archivo, FileMode.Open));
            br.BaseStream.Position = fileSelect.offset;
            File.WriteAllBytes(file, br.ReadBytes((int)fileSelect.size));
            br.Close();

            return file;
        }

        private void Save_NewSDAT(string fileout)
        {
            /*
             * File format *
             * 
             * Header
             * |_ Offset of all sections
             * 
             * Symbol block
             *
             * 
             * Info block
             * |_ Records
             * |_ Entries
             * 
             * FAT block
             * |_ Offset
             * |_ Size
             * 
             * File block
             * |_ Files...
             */

            BinaryReader br = new BinaryReader(File.OpenRead(sdat.archivo));

            // Symbol section
            String symbTemp = Path.GetTempFileName();
            br.BaseStream.Position = sdat.cabecera.symbOffset;
            File.WriteAllBytes(symbTemp, br.ReadBytes((int)sdat.symbol.size));

            // Info section
            String infoTemp = Path.GetTempFileName();
            br.BaseStream.Position = sdat.cabecera.infoOffset;
            File.WriteAllBytes(infoTemp, br.ReadBytes((int)sdat.info.header.size));
            br.Close();

            // FAT section
            String fatTemp = Path.GetTempFileName();
            Write_FAT(fatTemp, sdat.cabecera.fileOffset + 0x10);

            // File section
            String fileTemp = Path.GetTempFileName();
            Write_Files(fileTemp); // File without header

            // Write the new SDAT file
            int file_size = (int)(sdat.cabecera.symbSize + sdat.cabecera.infoSize + sdat.cabecera.fatSize + sdat.cabecera.fileSize);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            // Common header
            bw.Write(sdat.generico.id);
            bw.Write(sdat.generico.endianess);
            bw.Write(sdat.generico.constant);
            bw.Write(file_size);
            bw.Write(sdat.generico.header_size);
            bw.Write(sdat.generico.nSection);
            // Header
            bw.Write(sdat.cabecera.symbOffset);
            bw.Write(sdat.cabecera.symbSize);
            bw.Write(sdat.cabecera.infoOffset);
            bw.Write(sdat.cabecera.infoSize);
            bw.Write(sdat.cabecera.fatOffset);
            bw.Write(sdat.cabecera.fatSize);
            bw.Write(sdat.cabecera.fileOffset);
            bw.Write(sdat.cabecera.fileSize);
            bw.Write(sdat.cabecera.reserved);

            // Write other sections
            bw.Write(File.ReadAllBytes(symbTemp));
            bw.Write(File.ReadAllBytes(infoTemp));
            bw.Write(File.ReadAllBytes(fatTemp));

            bw.Write(sdat.files.header.id);
            bw.Write(sdat.files.header.size);
            bw.Write(sdat.files.header.nSounds);
            bw.Write(sdat.files.header.reserved);
            bw.Write(File.ReadAllBytes(fileTemp));

            bw.Flush();
            bw.Close();

            File.Delete(symbTemp);
            File.Delete(infoTemp);
            File.Delete(fatTemp);
            File.Delete(fileTemp);

        }
        private void Write_FAT(string fileout, uint startOffset)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
            uint currOffset = startOffset;

            bw.Write(sdat.fat.header.id);
            bw.Write(sdat.fat.header.size);
            bw.Write(sdat.fat.header.nRecords);

            for (int i = 0; i < sdat.files.root.folders.Count; i++)
            {
                for (int j = 0; j < sdat.files.root.folders[i].files.Count; j++)
                {
                    bw.Write(currOffset);
                    bw.Write(sdat.files.root.folders[i].files[j].size);
                    bw.Write((uint)0x00);
                    bw.Write((uint)0x00);
                    currOffset += sdat.files.root.folders[i].files[j].size;
                }
            }

            bw.Flush();
            bw.Close();
        }
        private void Write_Files(string fileout)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
            BinaryReader br = new BinaryReader(File.OpenRead(sdat.archivo));

            for (int i = 0; i < sdat.files.root.folders.Count; i++)
            {
                for (int j = 0; j < sdat.files.root.folders[i].files.Count; j++)
                {
                    if (sdat.files.root.folders[i].files[j].offset == 0x00)
                    {
                        bw.Write(File.ReadAllBytes(sdat.files.root.folders[i].files[j].path));
                        bw.Flush();
                    }
                    else
                    {
                        br.BaseStream.Position = sdat.files.root.folders[i].files[j].offset;
                        bw.Write(br.ReadBytes((int)sdat.files.root.folders[i].files[j].size));
                        bw.Flush();
                    }
                }
            }

            bw.Close();

            sdat.files.header.size = (uint)new FileInfo(fileout).Length + 0x10;
            sdat.cabecera.fileSize = sdat.files.header.size;
        }
    }
}
